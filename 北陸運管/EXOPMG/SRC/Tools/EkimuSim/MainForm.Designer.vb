<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.FileSelDialog = New System.Windows.Forms.OpenFileDialog()
        Me.LineStatusPollTimer = New System.Windows.Forms.Timer(Me.components)
        Me.ConButton = New System.Windows.Forms.Button()
        Me.LoggerTextBox = New System.Windows.Forms.TextBox()
        Me.LoggerClearButton = New System.Windows.Forms.Button()
        Me.SeqTabControl = New System.Windows.Forms.TabControl()
        Me.BasicTabPage = New System.Windows.Forms.TabPage()
        Me.CapRcvFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.CapSndFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.CapRcvTelegsCheckBox = New System.Windows.Forms.CheckBox()
        Me.CapSndTelegsCheckBox = New System.Windows.Forms.CheckBox()
        Me.TimeDataGetButton = New System.Windows.Forms.Button()
        Me.ComSartButton = New System.Windows.Forms.Button()
        Me.AutomaticComStartCheckBox = New System.Windows.Forms.CheckBox()
        Me.ActiveOneTabPage = New System.Windows.Forms.TabPage()
        Me.ActiveOneApplyFileLabel = New System.Windows.Forms.Label()
        Me.ActiveOneApplyFileTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveOneApplyFileSelButton = New System.Windows.Forms.Button()
        Me.ActiveOneReplyLimitLabel = New System.Windows.Forms.Label()
        Me.ActiveOneReplyLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveOneReplyLimitUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveOneExecIntervalLabel = New System.Windows.Forms.Label()
        Me.ActiveOneExecIntervalNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveOneExecIntervalUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveOneExecButton = New System.Windows.Forms.Button()
        Me.ActiveUllTabPage = New System.Windows.Forms.TabPage()
        Me.ActiveUllObjCodeLabel = New System.Windows.Forms.Label()
        Me.ActiveUllObjCodeTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveUllTransferFileLabel = New System.Windows.Forms.Label()
        Me.ActiveUllTransferFileTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveUllTransferFileSelButton = New System.Windows.Forms.Button()
        Me.ActiveUllReplyLimitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllReplyLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveUllReplyLimitUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllTransferLimitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllTransferLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveUllTransferLimitUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllExecIntervalLabel = New System.Windows.Forms.Label()
        Me.ActiveUllExecIntervalNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveUllExecIntervalUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllExecButton = New System.Windows.Forms.Button()
        Me.PassiveGetTabPage = New System.Windows.Forms.TabPage()
        Me.PassiveGetDataGridView = New System.Windows.Forms.DataGridView()
        Me.PassiveGetObjCodeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassiveGetApplyFileColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassiveGetForceReplyNakCheckBox = New System.Windows.Forms.CheckBox()
        Me.PassiveGetNakCauseNumberLabel = New System.Windows.Forms.Label()
        Me.PassiveGetNakCauseNumberTextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassiveGetNakCauseTextLabel = New System.Windows.Forms.Label()
        Me.PassiveGetNakCauseTextTextBox = New System.Windows.Forms.TextBox()
        Me.PassiveUllTabPage = New System.Windows.Forms.TabPage()
        Me.PassiveUllDataGridView = New System.Windows.Forms.DataGridView()
        Me.PassiveUllObjCodeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassiveUllApplyFileColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassiveUllStartLabel = New System.Windows.Forms.Label()
        Me.PassiveUllForceReplyNakCheckBox = New System.Windows.Forms.CheckBox()
        Me.PassiveUllNakCauseNumberLabel = New System.Windows.Forms.Label()
        Me.PassiveUllNakCauseNumberTextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassiveUllNakCauseTextLabel = New System.Windows.Forms.Label()
        Me.PassiveUllNakCauseTextTextBox = New System.Windows.Forms.TextBox()
        Me.PassiveUllFinishLabel = New System.Windows.Forms.Label()
        Me.PassiveUllTransferLimitLabel = New System.Windows.Forms.Label()
        Me.PassiveUllTransferLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.PassiveUllTransferLimitUnitLabel = New System.Windows.Forms.Label()
        Me.PassiveUllReplyLimitLabel = New System.Windows.Forms.Label()
        Me.PassiveUllReplyLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.PassiveUllReplyLimitUnitLabel = New System.Windows.Forms.Label()
        Me.PassivePostTabPage = New System.Windows.Forms.TabPage()
        Me.PassivePostDataGridView = New System.Windows.Forms.DataGridView()
        Me.PassivePostObjCodeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassivePostForceReplyNakCheckBox = New System.Windows.Forms.CheckBox()
        Me.PassivePostNakCauseNumberLabel = New System.Windows.Forms.Label()
        Me.PassivePostNakCauseNumberTextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassivePostNakCauseTextLabel = New System.Windows.Forms.Label()
        Me.PassivePostNakCauseTextTextBox = New System.Windows.Forms.TextBox()
        Me.PassiveDllTabPage = New System.Windows.Forms.TabPage()
        Me.PassiveDllFinishDetailLabel = New System.Windows.Forms.Label()
        Me.PassiveDllSimulateStoringCheckBox = New System.Windows.Forms.CheckBox()
        Me.PassiveDllDataGridView = New System.Windows.Forms.DataGridView()
        Me.PassiveDllObjCodeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassiveDllStartLabel = New System.Windows.Forms.Label()
        Me.PassiveDllForceReplyNakCheckBox = New System.Windows.Forms.CheckBox()
        Me.PassiveDllNakCauseNumberLabel = New System.Windows.Forms.Label()
        Me.PassiveDllNakCauseNumberTextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassiveDllNakCauseTextLabel = New System.Windows.Forms.Label()
        Me.PassiveDllNakCauseTextTextBox = New System.Windows.Forms.TextBox()
        Me.PassiveDllFinishLabel = New System.Windows.Forms.Label()
        Me.PassiveDllTransferLimitLabel = New System.Windows.Forms.Label()
        Me.PassiveDllTransferLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.PassiveDllTransferLimitUnitLabel = New System.Windows.Forms.Label()
        Me.PassiveDllReplyLimitLabel = New System.Windows.Forms.Label()
        Me.PassiveDllReplyLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.PassiveDllReplyLimitUnitLabel = New System.Windows.Forms.Label()
        Me.PassiveDllResultantVersionOfSlot1Label = New System.Windows.Forms.Label()
        Me.PassiveDllResultantVersionOfSlot1TextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassiveDllResultantVersionOfSlot2Label = New System.Windows.Forms.Label()
        Me.PassiveDllResultantVersionOfSlot2TextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassiveDllResultantFlagOfFullLabel = New System.Windows.Forms.Label()
        Me.PassiveDllResultantFlagOfFullTextBox = New System.Windows.Forms.TextBox()
        Me.ScenarioTabPage = New System.Windows.Forms.TabPage()
        Me.ScenarioExecIntervalLabel = New System.Windows.Forms.Label()
        Me.ScenarioExecIntervalNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ScenarioExecIntervalUnitLabel = New System.Windows.Forms.Label()
        Me.ScenarioFileLabel = New System.Windows.Forms.Label()
        Me.ScenarioFileTextBox = New System.Windows.Forms.TextBox()
        Me.ScenarioFileSelButton = New System.Windows.Forms.Button()
        Me.ScenarioExecButton = New System.Windows.Forms.Button()
        Me.ActiveOneExecTimer = New System.Windows.Forms.Timer(Me.components)
        Me.ActiveUllExecTimer = New System.Windows.Forms.Timer(Me.components)
        Me.PassiveGetRowHeaderMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PassiveGetDelMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PassiveGetApplyFileMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PassiveGetSelMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PassiveUllRowHeaderMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PassiveUllDelMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PassiveUllApplyFileMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PassiveUllSelMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PassivePostRowHeaderMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PassivePostDelMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PassiveDllRowHeaderMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PassiveDllDelMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ScenarioExecTimer = New System.Windows.Forms.Timer(Me.components)
        Me.LoggerPreviewCheckBox = New System.Windows.Forms.CheckBox()
        Me.SeqTabControl.SuspendLayout()
        Me.BasicTabPage.SuspendLayout()
        Me.ActiveOneTabPage.SuspendLayout()
        CType(Me.ActiveOneReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveOneExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ActiveUllTabPage.SuspendLayout()
        CType(Me.ActiveUllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveUllExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassiveGetTabPage.SuspendLayout()
        CType(Me.PassiveGetDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassiveUllTabPage.SuspendLayout()
        CType(Me.PassiveUllDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveUllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassivePostTabPage.SuspendLayout()
        CType(Me.PassivePostDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassiveDllTabPage.SuspendLayout()
        CType(Me.PassiveDllDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveDllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveDllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ScenarioTabPage.SuspendLayout()
        CType(Me.ScenarioExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassiveGetRowHeaderMenu.SuspendLayout()
        Me.PassiveGetApplyFileMenu.SuspendLayout()
        Me.PassiveUllRowHeaderMenu.SuspendLayout()
        Me.PassiveUllApplyFileMenu.SuspendLayout()
        Me.PassivePostRowHeaderMenu.SuspendLayout()
        Me.PassiveDllRowHeaderMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'LineStatusPollTimer
        '
        Me.LineStatusPollTimer.Interval = 500
        '
        'ConButton
        '
        Me.ConButton.Location = New System.Drawing.Point(17, 293)
        Me.ConButton.Name = "ConButton"
        Me.ConButton.Size = New System.Drawing.Size(94, 28)
        Me.ConButton.TabIndex = 1
        Me.ConButton.Text = "接続"
        Me.ConButton.UseVisualStyleBackColor = True
        '
        'LoggerTextBox
        '
        Me.LoggerTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LoggerTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.LoggerTextBox.Location = New System.Drawing.Point(12, 325)
        Me.LoggerTextBox.Multiline = True
        Me.LoggerTextBox.Name = "LoggerTextBox"
        Me.LoggerTextBox.ReadOnly = True
        Me.LoggerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.LoggerTextBox.Size = New System.Drawing.Size(701, 201)
        Me.LoggerTextBox.TabIndex = 4
        Me.LoggerTextBox.WordWrap = False
        '
        'LoggerClearButton
        '
        Me.LoggerClearButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LoggerClearButton.Location = New System.Drawing.Point(603, 293)
        Me.LoggerClearButton.Name = "LoggerClearButton"
        Me.LoggerClearButton.Size = New System.Drawing.Size(106, 28)
        Me.LoggerClearButton.TabIndex = 3
        Me.LoggerClearButton.Text = "ログ表示をクリア"
        Me.LoggerClearButton.UseVisualStyleBackColor = True
        '
        'SeqTabControl
        '
        Me.SeqTabControl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SeqTabControl.Controls.Add(Me.BasicTabPage)
        Me.SeqTabControl.Controls.Add(Me.ActiveOneTabPage)
        Me.SeqTabControl.Controls.Add(Me.ActiveUllTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassiveGetTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassiveUllTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassivePostTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassiveDllTabPage)
        Me.SeqTabControl.Controls.Add(Me.ScenarioTabPage)
        Me.SeqTabControl.Location = New System.Drawing.Point(13, 1)
        Me.SeqTabControl.Name = "SeqTabControl"
        Me.SeqTabControl.SelectedIndex = 0
        Me.SeqTabControl.Size = New System.Drawing.Size(700, 288)
        Me.SeqTabControl.TabIndex = 0
        '
        'BasicTabPage
        '
        Me.BasicTabPage.Controls.Add(Me.CapRcvFilesCheckBox)
        Me.BasicTabPage.Controls.Add(Me.CapSndFilesCheckBox)
        Me.BasicTabPage.Controls.Add(Me.CapRcvTelegsCheckBox)
        Me.BasicTabPage.Controls.Add(Me.CapSndTelegsCheckBox)
        Me.BasicTabPage.Controls.Add(Me.TimeDataGetButton)
        Me.BasicTabPage.Controls.Add(Me.ComSartButton)
        Me.BasicTabPage.Controls.Add(Me.AutomaticComStartCheckBox)
        Me.BasicTabPage.Location = New System.Drawing.Point(4, 22)
        Me.BasicTabPage.Name = "BasicTabPage"
        Me.BasicTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.BasicTabPage.Size = New System.Drawing.Size(692, 262)
        Me.BasicTabPage.TabIndex = 0
        Me.BasicTabPage.Text = "基本"
        Me.BasicTabPage.UseVisualStyleBackColor = True
        '
        'CapRcvFilesCheckBox
        '
        Me.CapRcvFilesCheckBox.AutoSize = True
        Me.CapRcvFilesCheckBox.Location = New System.Drawing.Point(409, 85)
        Me.CapRcvFilesCheckBox.Name = "CapRcvFilesCheckBox"
        Me.CapRcvFilesCheckBox.Size = New System.Drawing.Size(134, 16)
        Me.CapRcvFilesCheckBox.TabIndex = 5
        Me.CapRcvFilesCheckBox.Text = "受信ファイルを保存する"
        Me.CapRcvFilesCheckBox.UseVisualStyleBackColor = True
        '
        'CapSndFilesCheckBox
        '
        Me.CapSndFilesCheckBox.AutoSize = True
        Me.CapSndFilesCheckBox.Location = New System.Drawing.Point(409, 63)
        Me.CapSndFilesCheckBox.Name = "CapSndFilesCheckBox"
        Me.CapSndFilesCheckBox.Size = New System.Drawing.Size(134, 16)
        Me.CapSndFilesCheckBox.TabIndex = 4
        Me.CapSndFilesCheckBox.Text = "送信ファイルを保存する"
        Me.CapSndFilesCheckBox.UseVisualStyleBackColor = True
        '
        'CapRcvTelegsCheckBox
        '
        Me.CapRcvTelegsCheckBox.AutoSize = True
        Me.CapRcvTelegsCheckBox.Location = New System.Drawing.Point(409, 41)
        Me.CapRcvTelegsCheckBox.Name = "CapRcvTelegsCheckBox"
        Me.CapRcvTelegsCheckBox.Size = New System.Drawing.Size(124, 16)
        Me.CapRcvTelegsCheckBox.TabIndex = 3
        Me.CapRcvTelegsCheckBox.Text = "受信電文を保存する"
        Me.CapRcvTelegsCheckBox.UseVisualStyleBackColor = True
        '
        'CapSndTelegsCheckBox
        '
        Me.CapSndTelegsCheckBox.AutoSize = True
        Me.CapSndTelegsCheckBox.Location = New System.Drawing.Point(409, 19)
        Me.CapSndTelegsCheckBox.Name = "CapSndTelegsCheckBox"
        Me.CapSndTelegsCheckBox.Size = New System.Drawing.Size(124, 16)
        Me.CapSndTelegsCheckBox.TabIndex = 2
        Me.CapSndTelegsCheckBox.Text = "送信電文を保存する"
        Me.CapSndTelegsCheckBox.UseVisualStyleBackColor = True
        '
        'TimeDataGetButton
        '
        Me.TimeDataGetButton.Location = New System.Drawing.Point(171, 73)
        Me.TimeDataGetButton.Name = "TimeDataGetButton"
        Me.TimeDataGetButton.Size = New System.Drawing.Size(145, 28)
        Me.TimeDataGetButton.TabIndex = 7
        Me.TimeDataGetButton.Text = "整時データ要求 実行"
        Me.TimeDataGetButton.UseVisualStyleBackColor = True
        '
        'ComSartButton
        '
        Me.ComSartButton.Location = New System.Drawing.Point(20, 73)
        Me.ComSartButton.Name = "ComSartButton"
        Me.ComSartButton.Size = New System.Drawing.Size(145, 28)
        Me.ComSartButton.TabIndex = 6
        Me.ComSartButton.Text = "接続初期化要求 実行"
        Me.ComSartButton.UseVisualStyleBackColor = True
        '
        'AutomaticComStartCheckBox
        '
        Me.AutomaticComStartCheckBox.AutoSize = True
        Me.AutomaticComStartCheckBox.Location = New System.Drawing.Point(20, 19)
        Me.AutomaticComStartCheckBox.Name = "AutomaticComStartCheckBox"
        Me.AutomaticComStartCheckBox.Size = New System.Drawing.Size(197, 16)
        Me.AutomaticComStartCheckBox.TabIndex = 1
        Me.AutomaticComStartCheckBox.Text = "接続後に開始シーケンスを自動実行"
        Me.AutomaticComStartCheckBox.UseVisualStyleBackColor = True
        '
        'ActiveOneTabPage
        '
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneApplyFileLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneApplyFileTextBox)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneApplyFileSelButton)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneReplyLimitLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneReplyLimitNumericUpDown)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneReplyLimitUnitLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecIntervalLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecIntervalNumericUpDown)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecIntervalUnitLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecButton)
        Me.ActiveOneTabPage.Location = New System.Drawing.Point(4, 22)
        Me.ActiveOneTabPage.Name = "ActiveOneTabPage"
        Me.ActiveOneTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.ActiveOneTabPage.Size = New System.Drawing.Size(692, 262)
        Me.ActiveOneTabPage.TabIndex = 1
        Me.ActiveOneTabPage.Text = "電文送信"
        Me.ActiveOneTabPage.UseVisualStyleBackColor = True
        '
        'ActiveOneApplyFileLabel
        '
        Me.ActiveOneApplyFileLabel.AutoSize = True
        Me.ActiveOneApplyFileLabel.Location = New System.Drawing.Point(15, 18)
        Me.ActiveOneApplyFileLabel.Name = "ActiveOneApplyFileLabel"
        Me.ActiveOneApplyFileLabel.Size = New System.Drawing.Size(100, 12)
        Me.ActiveOneApplyFileLabel.TabIndex = 1
        Me.ActiveOneApplyFileLabel.Text = "RAW電文ファイル名"
        '
        'ActiveOneApplyFileTextBox
        '
        Me.ActiveOneApplyFileTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneApplyFileTextBox.Location = New System.Drawing.Point(133, 15)
        Me.ActiveOneApplyFileTextBox.Name = "ActiveOneApplyFileTextBox"
        Me.ActiveOneApplyFileTextBox.Size = New System.Drawing.Size(491, 19)
        Me.ActiveOneApplyFileTextBox.TabIndex = 2
        '
        'ActiveOneApplyFileSelButton
        '
        Me.ActiveOneApplyFileSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneApplyFileSelButton.Location = New System.Drawing.Point(630, 13)
        Me.ActiveOneApplyFileSelButton.Name = "ActiveOneApplyFileSelButton"
        Me.ActiveOneApplyFileSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ActiveOneApplyFileSelButton.TabIndex = 3
        Me.ActiveOneApplyFileSelButton.Text = "選択"
        Me.ActiveOneApplyFileSelButton.UseVisualStyleBackColor = True
        '
        'ActiveOneReplyLimitLabel
        '
        Me.ActiveOneReplyLimitLabel.AutoSize = True
        Me.ActiveOneReplyLimitLabel.Location = New System.Drawing.Point(38, 53)
        Me.ActiveOneReplyLimitLabel.Name = "ActiveOneReplyLimitLabel"
        Me.ActiveOneReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.ActiveOneReplyLimitLabel.TabIndex = 4
        Me.ActiveOneReplyLimitLabel.Text = "応答受信期限"
        '
        'ActiveOneReplyLimitNumericUpDown
        '
        Me.ActiveOneReplyLimitNumericUpDown.Location = New System.Drawing.Point(133, 51)
        Me.ActiveOneReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.ActiveOneReplyLimitNumericUpDown.Name = "ActiveOneReplyLimitNumericUpDown"
        Me.ActiveOneReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveOneReplyLimitNumericUpDown.TabIndex = 5
        '
        'ActiveOneReplyLimitUnitLabel
        '
        Me.ActiveOneReplyLimitUnitLabel.AutoSize = True
        Me.ActiveOneReplyLimitUnitLabel.Location = New System.Drawing.Point(219, 53)
        Me.ActiveOneReplyLimitUnitLabel.Name = "ActiveOneReplyLimitUnitLabel"
        Me.ActiveOneReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveOneReplyLimitUnitLabel.TabIndex = 6
        Me.ActiveOneReplyLimitUnitLabel.Text = "ms"
        '
        'ActiveOneExecIntervalLabel
        '
        Me.ActiveOneExecIntervalLabel.AutoSize = True
        Me.ActiveOneExecIntervalLabel.Location = New System.Drawing.Point(15, 88)
        Me.ActiveOneExecIntervalLabel.Name = "ActiveOneExecIntervalLabel"
        Me.ActiveOneExecIntervalLabel.Size = New System.Drawing.Size(105, 12)
        Me.ActiveOneExecIntervalLabel.TabIndex = 7
        Me.ActiveOneExecIntervalLabel.Text = "実行間隔（0は単発）"
        '
        'ActiveOneExecIntervalNumericUpDown
        '
        Me.ActiveOneExecIntervalNumericUpDown.Location = New System.Drawing.Point(133, 86)
        Me.ActiveOneExecIntervalNumericUpDown.Maximum = New Decimal(New Integer() {86400000, 0, 0, 0})
        Me.ActiveOneExecIntervalNumericUpDown.Name = "ActiveOneExecIntervalNumericUpDown"
        Me.ActiveOneExecIntervalNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveOneExecIntervalNumericUpDown.TabIndex = 8
        '
        'ActiveOneExecIntervalUnitLabel
        '
        Me.ActiveOneExecIntervalUnitLabel.AutoSize = True
        Me.ActiveOneExecIntervalUnitLabel.Location = New System.Drawing.Point(219, 88)
        Me.ActiveOneExecIntervalUnitLabel.Name = "ActiveOneExecIntervalUnitLabel"
        Me.ActiveOneExecIntervalUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveOneExecIntervalUnitLabel.TabIndex = 9
        Me.ActiveOneExecIntervalUnitLabel.Text = "ms"
        '
        'ActiveOneExecButton
        '
        Me.ActiveOneExecButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneExecButton.Location = New System.Drawing.Point(594, 80)
        Me.ActiveOneExecButton.Name = "ActiveOneExecButton"
        Me.ActiveOneExecButton.Size = New System.Drawing.Size(86, 28)
        Me.ActiveOneExecButton.TabIndex = 10
        Me.ActiveOneExecButton.Text = "実行"
        Me.ActiveOneExecButton.UseVisualStyleBackColor = True
        '
        'ActiveUllTabPage
        '
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllObjCodeLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllObjCodeTextBox)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferFileLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferFileTextBox)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferFileSelButton)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllReplyLimitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllReplyLimitNumericUpDown)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllReplyLimitUnitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferLimitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferLimitNumericUpDown)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferLimitUnitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecIntervalLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecIntervalNumericUpDown)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecIntervalUnitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecButton)
        Me.ActiveUllTabPage.Location = New System.Drawing.Point(4, 22)
        Me.ActiveUllTabPage.Name = "ActiveUllTabPage"
        Me.ActiveUllTabPage.Size = New System.Drawing.Size(692, 262)
        Me.ActiveUllTabPage.TabIndex = 2
        Me.ActiveUllTabPage.Text = "能動的ULL"
        Me.ActiveUllTabPage.UseVisualStyleBackColor = True
        '
        'ActiveUllObjCodeLabel
        '
        Me.ActiveUllObjCodeLabel.AutoSize = True
        Me.ActiveUllObjCodeLabel.Location = New System.Drawing.Point(14, 18)
        Me.ActiveUllObjCodeLabel.Name = "ActiveUllObjCodeLabel"
        Me.ActiveUllObjCodeLabel.Size = New System.Drawing.Size(57, 12)
        Me.ActiveUllObjCodeLabel.TabIndex = 1
        Me.ActiveUllObjCodeLabel.Text = "データ種別"
        '
        'ActiveUllObjCodeTextBox
        '
        Me.ActiveUllObjCodeTextBox.Location = New System.Drawing.Point(77, 15)
        Me.ActiveUllObjCodeTextBox.MaxLength = 2
        Me.ActiveUllObjCodeTextBox.Name = "ActiveUllObjCodeTextBox"
        Me.ActiveUllObjCodeTextBox.Size = New System.Drawing.Size(34, 19)
        Me.ActiveUllObjCodeTextBox.TabIndex = 2
        '
        'ActiveUllTransferFileLabel
        '
        Me.ActiveUllTransferFileLabel.AutoSize = True
        Me.ActiveUllTransferFileLabel.Location = New System.Drawing.Point(133, 18)
        Me.ActiveUllTransferFileLabel.Name = "ActiveUllTransferFileLabel"
        Me.ActiveUllTransferFileLabel.Size = New System.Drawing.Size(75, 12)
        Me.ActiveUllTransferFileLabel.TabIndex = 3
        Me.ActiveUllTransferFileLabel.Text = "転送ファイル名"
        '
        'ActiveUllTransferFileTextBox
        '
        Me.ActiveUllTransferFileTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllTransferFileTextBox.Location = New System.Drawing.Point(214, 15)
        Me.ActiveUllTransferFileTextBox.Name = "ActiveUllTransferFileTextBox"
        Me.ActiveUllTransferFileTextBox.Size = New System.Drawing.Size(410, 19)
        Me.ActiveUllTransferFileTextBox.TabIndex = 4
        '
        'ActiveUllTransferFileSelButton
        '
        Me.ActiveUllTransferFileSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllTransferFileSelButton.Location = New System.Drawing.Point(630, 13)
        Me.ActiveUllTransferFileSelButton.Name = "ActiveUllTransferFileSelButton"
        Me.ActiveUllTransferFileSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ActiveUllTransferFileSelButton.TabIndex = 5
        Me.ActiveUllTransferFileSelButton.Text = "選択"
        Me.ActiveUllTransferFileSelButton.UseVisualStyleBackColor = True
        '
        'ActiveUllReplyLimitLabel
        '
        Me.ActiveUllReplyLimitLabel.AutoSize = True
        Me.ActiveUllReplyLimitLabel.Location = New System.Drawing.Point(279, 53)
        Me.ActiveUllReplyLimitLabel.Name = "ActiveUllReplyLimitLabel"
        Me.ActiveUllReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.ActiveUllReplyLimitLabel.TabIndex = 9
        Me.ActiveUllReplyLimitLabel.Text = "応答受信期限"
        '
        'ActiveUllReplyLimitNumericUpDown
        '
        Me.ActiveUllReplyLimitNumericUpDown.Location = New System.Drawing.Point(360, 51)
        Me.ActiveUllReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.ActiveUllReplyLimitNumericUpDown.Name = "ActiveUllReplyLimitNumericUpDown"
        Me.ActiveUllReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveUllReplyLimitNumericUpDown.TabIndex = 10
        '
        'ActiveUllReplyLimitUnitLabel
        '
        Me.ActiveUllReplyLimitUnitLabel.AutoSize = True
        Me.ActiveUllReplyLimitUnitLabel.Location = New System.Drawing.Point(446, 53)
        Me.ActiveUllReplyLimitUnitLabel.Name = "ActiveUllReplyLimitUnitLabel"
        Me.ActiveUllReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveUllReplyLimitUnitLabel.TabIndex = 11
        Me.ActiveUllReplyLimitUnitLabel.Text = "ms"
        '
        'ActiveUllTransferLimitLabel
        '
        Me.ActiveUllTransferLimitLabel.AutoSize = True
        Me.ActiveUllTransferLimitLabel.Location = New System.Drawing.Point(12, 53)
        Me.ActiveUllTransferLimitLabel.Name = "ActiveUllTransferLimitLabel"
        Me.ActiveUllTransferLimitLabel.Size = New System.Drawing.Size(117, 12)
        Me.ActiveUllTransferLimitLabel.TabIndex = 6
        Me.ActiveUllTransferLimitLabel.Text = "転送期限（0は無期限）"
        '
        'ActiveUllTransferLimitNumericUpDown
        '
        Me.ActiveUllTransferLimitNumericUpDown.Location = New System.Drawing.Point(135, 51)
        Me.ActiveUllTransferLimitNumericUpDown.Maximum = New Decimal(New Integer() {43200000, 0, 0, 0})
        Me.ActiveUllTransferLimitNumericUpDown.Name = "ActiveUllTransferLimitNumericUpDown"
        Me.ActiveUllTransferLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveUllTransferLimitNumericUpDown.TabIndex = 7
        '
        'ActiveUllTransferLimitUnitLabel
        '
        Me.ActiveUllTransferLimitUnitLabel.AutoSize = True
        Me.ActiveUllTransferLimitUnitLabel.Location = New System.Drawing.Point(221, 53)
        Me.ActiveUllTransferLimitUnitLabel.Name = "ActiveUllTransferLimitUnitLabel"
        Me.ActiveUllTransferLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveUllTransferLimitUnitLabel.TabIndex = 8
        Me.ActiveUllTransferLimitUnitLabel.Text = "ms"
        '
        'ActiveUllExecIntervalLabel
        '
        Me.ActiveUllExecIntervalLabel.AutoSize = True
        Me.ActiveUllExecIntervalLabel.Location = New System.Drawing.Point(17, 88)
        Me.ActiveUllExecIntervalLabel.Name = "ActiveUllExecIntervalLabel"
        Me.ActiveUllExecIntervalLabel.Size = New System.Drawing.Size(105, 12)
        Me.ActiveUllExecIntervalLabel.TabIndex = 12
        Me.ActiveUllExecIntervalLabel.Text = "実行間隔（0は単発）"
        '
        'ActiveUllExecIntervalNumericUpDown
        '
        Me.ActiveUllExecIntervalNumericUpDown.Location = New System.Drawing.Point(135, 86)
        Me.ActiveUllExecIntervalNumericUpDown.Maximum = New Decimal(New Integer() {86400000, 0, 0, 0})
        Me.ActiveUllExecIntervalNumericUpDown.Name = "ActiveUllExecIntervalNumericUpDown"
        Me.ActiveUllExecIntervalNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveUllExecIntervalNumericUpDown.TabIndex = 13
        '
        'ActiveUllExecIntervalUnitLabel
        '
        Me.ActiveUllExecIntervalUnitLabel.AutoSize = True
        Me.ActiveUllExecIntervalUnitLabel.Location = New System.Drawing.Point(221, 88)
        Me.ActiveUllExecIntervalUnitLabel.Name = "ActiveUllExecIntervalUnitLabel"
        Me.ActiveUllExecIntervalUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveUllExecIntervalUnitLabel.TabIndex = 14
        Me.ActiveUllExecIntervalUnitLabel.Text = "ms"
        '
        'ActiveUllExecButton
        '
        Me.ActiveUllExecButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllExecButton.Location = New System.Drawing.Point(594, 80)
        Me.ActiveUllExecButton.Name = "ActiveUllExecButton"
        Me.ActiveUllExecButton.Size = New System.Drawing.Size(86, 28)
        Me.ActiveUllExecButton.TabIndex = 15
        Me.ActiveUllExecButton.Text = "実行"
        Me.ActiveUllExecButton.UseVisualStyleBackColor = True
        '
        'PassiveGetTabPage
        '
        Me.PassiveGetTabPage.Controls.Add(Me.PassiveGetDataGridView)
        Me.PassiveGetTabPage.Controls.Add(Me.PassiveGetForceReplyNakCheckBox)
        Me.PassiveGetTabPage.Controls.Add(Me.PassiveGetNakCauseNumberLabel)
        Me.PassiveGetTabPage.Controls.Add(Me.PassiveGetNakCauseNumberTextBox)
        Me.PassiveGetTabPage.Controls.Add(Me.PassiveGetNakCauseTextLabel)
        Me.PassiveGetTabPage.Controls.Add(Me.PassiveGetNakCauseTextTextBox)
        Me.PassiveGetTabPage.Location = New System.Drawing.Point(4, 22)
        Me.PassiveGetTabPage.Name = "PassiveGetTabPage"
        Me.PassiveGetTabPage.Size = New System.Drawing.Size(692, 262)
        Me.PassiveGetTabPage.TabIndex = 3
        Me.PassiveGetTabPage.Text = "GET電文受信"
        Me.PassiveGetTabPage.UseVisualStyleBackColor = True
        '
        'PassiveGetDataGridView
        '
        Me.PassiveGetDataGridView.AllowUserToDeleteRows = False
        Me.PassiveGetDataGridView.AllowUserToResizeRows = False
        Me.PassiveGetDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassiveGetDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassiveGetObjCodeColumn, Me.PassiveGetApplyFileColumn})
        Me.PassiveGetDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassiveGetDataGridView.MultiSelect = False
        Me.PassiveGetDataGridView.Name = "PassiveGetDataGridView"
        Me.PassiveGetDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassiveGetDataGridView.RowTemplate.Height = 21
        Me.PassiveGetDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassiveGetDataGridView.Size = New System.Drawing.Size(686, 231)
        Me.PassiveGetDataGridView.StandardTab = True
        Me.PassiveGetDataGridView.TabIndex = 1
        '
        'PassiveGetObjCodeColumn
        '
        Me.PassiveGetObjCodeColumn.HeaderText = "データ種別"
        Me.PassiveGetObjCodeColumn.Name = "PassiveGetObjCodeColumn"
        Me.PassiveGetObjCodeColumn.Width = 96
        '
        'PassiveGetApplyFileColumn
        '
        Me.PassiveGetApplyFileColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.PassiveGetApplyFileColumn.HeaderText = "返信データファイル"
        Me.PassiveGetApplyFileColumn.Name = "PassiveGetApplyFileColumn"
        '
        'PassiveGetForceReplyNakCheckBox
        '
        Me.PassiveGetForceReplyNakCheckBox.AutoSize = True
        Me.PassiveGetForceReplyNakCheckBox.Location = New System.Drawing.Point(3, 243)
        Me.PassiveGetForceReplyNakCheckBox.Name = "PassiveGetForceReplyNakCheckBox"
        Me.PassiveGetForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassiveGetForceReplyNakCheckBox.TabIndex = 2
        Me.PassiveGetForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassiveGetForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassiveGetNakCauseNumberLabel
        '
        Me.PassiveGetNakCauseNumberLabel.AutoSize = True
        Me.PassiveGetNakCauseNumberLabel.Location = New System.Drawing.Point(102, 245)
        Me.PassiveGetNakCauseNumberLabel.Name = "PassiveGetNakCauseNumberLabel"
        Me.PassiveGetNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveGetNakCauseNumberLabel.TabIndex = 3
        Me.PassiveGetNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassiveGetNakCauseNumberTextBox
        '
        Me.PassiveGetNakCauseNumberTextBox.Location = New System.Drawing.Point(184, 241)
        Me.PassiveGetNakCauseNumberTextBox.Mask = "999"
        Me.PassiveGetNakCauseNumberTextBox.Name = "PassiveGetNakCauseNumberTextBox"
        Me.PassiveGetNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassiveGetNakCauseNumberTextBox.TabIndex = 4
        '
        'PassiveGetNakCauseTextLabel
        '
        Me.PassiveGetNakCauseTextLabel.AutoSize = True
        Me.PassiveGetNakCauseTextLabel.Location = New System.Drawing.Point(259, 245)
        Me.PassiveGetNakCauseTextLabel.Name = "PassiveGetNakCauseTextLabel"
        Me.PassiveGetNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveGetNakCauseTextLabel.TabIndex = 5
        Me.PassiveGetNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassiveGetNakCauseTextTextBox
        '
        Me.PassiveGetNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetNakCauseTextTextBox.Location = New System.Drawing.Point(341, 241)
        Me.PassiveGetNakCauseTextTextBox.MaxLength = 50
        Me.PassiveGetNakCauseTextTextBox.Name = "PassiveGetNakCauseTextTextBox"
        Me.PassiveGetNakCauseTextTextBox.Size = New System.Drawing.Size(348, 19)
        Me.PassiveGetNakCauseTextTextBox.TabIndex = 6
        '
        'PassiveUllTabPage
        '
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllDataGridView)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllStartLabel)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllForceReplyNakCheckBox)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllNakCauseNumberLabel)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllNakCauseNumberTextBox)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllNakCauseTextLabel)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllNakCauseTextTextBox)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllFinishLabel)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllTransferLimitLabel)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllTransferLimitNumericUpDown)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllTransferLimitUnitLabel)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllReplyLimitLabel)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllReplyLimitNumericUpDown)
        Me.PassiveUllTabPage.Controls.Add(Me.PassiveUllReplyLimitUnitLabel)
        Me.PassiveUllTabPage.Location = New System.Drawing.Point(4, 22)
        Me.PassiveUllTabPage.Name = "PassiveUllTabPage"
        Me.PassiveUllTabPage.Size = New System.Drawing.Size(692, 262)
        Me.PassiveUllTabPage.TabIndex = 4
        Me.PassiveUllTabPage.Text = "受動的ULL"
        Me.PassiveUllTabPage.UseVisualStyleBackColor = True
        '
        'PassiveUllDataGridView
        '
        Me.PassiveUllDataGridView.AllowUserToDeleteRows = False
        Me.PassiveUllDataGridView.AllowUserToResizeRows = False
        Me.PassiveUllDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassiveUllDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassiveUllObjCodeColumn, Me.PassiveUllApplyFileColumn})
        Me.PassiveUllDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassiveUllDataGridView.MultiSelect = False
        Me.PassiveUllDataGridView.Name = "PassiveUllDataGridView"
        Me.PassiveUllDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassiveUllDataGridView.RowTemplate.Height = 21
        Me.PassiveUllDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassiveUllDataGridView.Size = New System.Drawing.Size(686, 201)
        Me.PassiveUllDataGridView.StandardTab = True
        Me.PassiveUllDataGridView.TabIndex = 1
        '
        'PassiveUllObjCodeColumn
        '
        Me.PassiveUllObjCodeColumn.HeaderText = "データ種別"
        Me.PassiveUllObjCodeColumn.Name = "PassiveUllObjCodeColumn"
        Me.PassiveUllObjCodeColumn.Width = 96
        '
        'PassiveUllApplyFileColumn
        '
        Me.PassiveUllApplyFileColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.PassiveUllApplyFileColumn.HeaderText = "転送ファイル"
        Me.PassiveUllApplyFileColumn.Name = "PassiveUllApplyFileColumn"
        '
        'PassiveUllStartLabel
        '
        Me.PassiveUllStartLabel.AutoSize = True
        Me.PassiveUllStartLabel.Location = New System.Drawing.Point(1, 217)
        Me.PassiveUllStartLabel.Name = "PassiveUllStartLabel"
        Me.PassiveUllStartLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveUllStartLabel.TabIndex = 2
        Me.PassiveUllStartLabel.Text = "開始時"
        '
        'PassiveUllForceReplyNakCheckBox
        '
        Me.PassiveUllForceReplyNakCheckBox.AutoSize = True
        Me.PassiveUllForceReplyNakCheckBox.Location = New System.Drawing.Point(56, 215)
        Me.PassiveUllForceReplyNakCheckBox.Name = "PassiveUllForceReplyNakCheckBox"
        Me.PassiveUllForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassiveUllForceReplyNakCheckBox.TabIndex = 3
        Me.PassiveUllForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassiveUllForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassiveUllNakCauseNumberLabel
        '
        Me.PassiveUllNakCauseNumberLabel.AutoSize = True
        Me.PassiveUllNakCauseNumberLabel.Location = New System.Drawing.Point(142, 217)
        Me.PassiveUllNakCauseNumberLabel.Name = "PassiveUllNakCauseNumberLabel"
        Me.PassiveUllNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveUllNakCauseNumberLabel.TabIndex = 4
        Me.PassiveUllNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassiveUllNakCauseNumberTextBox
        '
        Me.PassiveUllNakCauseNumberTextBox.Location = New System.Drawing.Point(224, 213)
        Me.PassiveUllNakCauseNumberTextBox.Mask = "999"
        Me.PassiveUllNakCauseNumberTextBox.Name = "PassiveUllNakCauseNumberTextBox"
        Me.PassiveUllNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassiveUllNakCauseNumberTextBox.TabIndex = 5
        '
        'PassiveUllNakCauseTextLabel
        '
        Me.PassiveUllNakCauseTextLabel.AutoSize = True
        Me.PassiveUllNakCauseTextLabel.Location = New System.Drawing.Point(280, 217)
        Me.PassiveUllNakCauseTextLabel.Name = "PassiveUllNakCauseTextLabel"
        Me.PassiveUllNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveUllNakCauseTextLabel.TabIndex = 6
        Me.PassiveUllNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassiveUllNakCauseTextTextBox
        '
        Me.PassiveUllNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllNakCauseTextTextBox.Location = New System.Drawing.Point(362, 213)
        Me.PassiveUllNakCauseTextTextBox.MaxLength = 50
        Me.PassiveUllNakCauseTextTextBox.Name = "PassiveUllNakCauseTextTextBox"
        Me.PassiveUllNakCauseTextTextBox.Size = New System.Drawing.Size(326, 19)
        Me.PassiveUllNakCauseTextTextBox.TabIndex = 7
        '
        'PassiveUllFinishLabel
        '
        Me.PassiveUllFinishLabel.AutoSize = True
        Me.PassiveUllFinishLabel.Location = New System.Drawing.Point(1, 244)
        Me.PassiveUllFinishLabel.Name = "PassiveUllFinishLabel"
        Me.PassiveUllFinishLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveUllFinishLabel.TabIndex = 8
        Me.PassiveUllFinishLabel.Text = "終了時"
        '
        'PassiveUllTransferLimitLabel
        '
        Me.PassiveUllTransferLimitLabel.AutoSize = True
        Me.PassiveUllTransferLimitLabel.Location = New System.Drawing.Point(55, 244)
        Me.PassiveUllTransferLimitLabel.Name = "PassiveUllTransferLimitLabel"
        Me.PassiveUllTransferLimitLabel.Size = New System.Drawing.Size(117, 12)
        Me.PassiveUllTransferLimitLabel.TabIndex = 9
        Me.PassiveUllTransferLimitLabel.Text = "転送期限（0は無期限）"
        '
        'PassiveUllTransferLimitNumericUpDown
        '
        Me.PassiveUllTransferLimitNumericUpDown.Location = New System.Drawing.Point(178, 242)
        Me.PassiveUllTransferLimitNumericUpDown.Maximum = New Decimal(New Integer() {43200000, 0, 0, 0})
        Me.PassiveUllTransferLimitNumericUpDown.Name = "PassiveUllTransferLimitNumericUpDown"
        Me.PassiveUllTransferLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveUllTransferLimitNumericUpDown.TabIndex = 10
        '
        'PassiveUllTransferLimitUnitLabel
        '
        Me.PassiveUllTransferLimitUnitLabel.AutoSize = True
        Me.PassiveUllTransferLimitUnitLabel.Location = New System.Drawing.Point(264, 244)
        Me.PassiveUllTransferLimitUnitLabel.Name = "PassiveUllTransferLimitUnitLabel"
        Me.PassiveUllTransferLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.PassiveUllTransferLimitUnitLabel.TabIndex = 11
        Me.PassiveUllTransferLimitUnitLabel.Text = "ms"
        '
        'PassiveUllReplyLimitLabel
        '
        Me.PassiveUllReplyLimitLabel.AutoSize = True
        Me.PassiveUllReplyLimitLabel.Location = New System.Drawing.Point(299, 244)
        Me.PassiveUllReplyLimitLabel.Name = "PassiveUllReplyLimitLabel"
        Me.PassiveUllReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.PassiveUllReplyLimitLabel.TabIndex = 12
        Me.PassiveUllReplyLimitLabel.Text = "応答受信期限"
        '
        'PassiveUllReplyLimitNumericUpDown
        '
        Me.PassiveUllReplyLimitNumericUpDown.Location = New System.Drawing.Point(382, 242)
        Me.PassiveUllReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.PassiveUllReplyLimitNumericUpDown.Name = "PassiveUllReplyLimitNumericUpDown"
        Me.PassiveUllReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveUllReplyLimitNumericUpDown.TabIndex = 13
        '
        'PassiveUllReplyLimitUnitLabel
        '
        Me.PassiveUllReplyLimitUnitLabel.AutoSize = True
        Me.PassiveUllReplyLimitUnitLabel.Location = New System.Drawing.Point(468, 244)
        Me.PassiveUllReplyLimitUnitLabel.Name = "PassiveUllReplyLimitUnitLabel"
        Me.PassiveUllReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.PassiveUllReplyLimitUnitLabel.TabIndex = 14
        Me.PassiveUllReplyLimitUnitLabel.Text = "ms"
        '
        'PassivePostTabPage
        '
        Me.PassivePostTabPage.Controls.Add(Me.PassivePostDataGridView)
        Me.PassivePostTabPage.Controls.Add(Me.PassivePostForceReplyNakCheckBox)
        Me.PassivePostTabPage.Controls.Add(Me.PassivePostNakCauseNumberLabel)
        Me.PassivePostTabPage.Controls.Add(Me.PassivePostNakCauseNumberTextBox)
        Me.PassivePostTabPage.Controls.Add(Me.PassivePostNakCauseTextLabel)
        Me.PassivePostTabPage.Controls.Add(Me.PassivePostNakCauseTextTextBox)
        Me.PassivePostTabPage.Location = New System.Drawing.Point(4, 22)
        Me.PassivePostTabPage.Name = "PassivePostTabPage"
        Me.PassivePostTabPage.Size = New System.Drawing.Size(692, 262)
        Me.PassivePostTabPage.TabIndex = 5
        Me.PassivePostTabPage.Text = "POST電文受信"
        Me.PassivePostTabPage.UseVisualStyleBackColor = True
        '
        'PassivePostDataGridView
        '
        Me.PassivePostDataGridView.AllowUserToDeleteRows = False
        Me.PassivePostDataGridView.AllowUserToResizeRows = False
        Me.PassivePostDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassivePostDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassivePostObjCodeColumn})
        Me.PassivePostDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassivePostDataGridView.MultiSelect = False
        Me.PassivePostDataGridView.Name = "PassivePostDataGridView"
        Me.PassivePostDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassivePostDataGridView.RowTemplate.Height = 21
        Me.PassivePostDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassivePostDataGridView.Size = New System.Drawing.Size(156, 234)
        Me.PassivePostDataGridView.StandardTab = True
        Me.PassivePostDataGridView.TabIndex = 1
        '
        'PassivePostObjCodeColumn
        '
        Me.PassivePostObjCodeColumn.HeaderText = "データ種別"
        Me.PassivePostObjCodeColumn.Name = "PassivePostObjCodeColumn"
        Me.PassivePostObjCodeColumn.Width = 96
        '
        'PassivePostForceReplyNakCheckBox
        '
        Me.PassivePostForceReplyNakCheckBox.AutoSize = True
        Me.PassivePostForceReplyNakCheckBox.Location = New System.Drawing.Point(3, 243)
        Me.PassivePostForceReplyNakCheckBox.Name = "PassivePostForceReplyNakCheckBox"
        Me.PassivePostForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassivePostForceReplyNakCheckBox.TabIndex = 2
        Me.PassivePostForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassivePostForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassivePostNakCauseNumberLabel
        '
        Me.PassivePostNakCauseNumberLabel.AutoSize = True
        Me.PassivePostNakCauseNumberLabel.Location = New System.Drawing.Point(102, 245)
        Me.PassivePostNakCauseNumberLabel.Name = "PassivePostNakCauseNumberLabel"
        Me.PassivePostNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassivePostNakCauseNumberLabel.TabIndex = 3
        Me.PassivePostNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassivePostNakCauseNumberTextBox
        '
        Me.PassivePostNakCauseNumberTextBox.Location = New System.Drawing.Point(184, 241)
        Me.PassivePostNakCauseNumberTextBox.Mask = "999"
        Me.PassivePostNakCauseNumberTextBox.Name = "PassivePostNakCauseNumberTextBox"
        Me.PassivePostNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassivePostNakCauseNumberTextBox.TabIndex = 4
        '
        'PassivePostNakCauseTextLabel
        '
        Me.PassivePostNakCauseTextLabel.AutoSize = True
        Me.PassivePostNakCauseTextLabel.Location = New System.Drawing.Point(259, 245)
        Me.PassivePostNakCauseTextLabel.Name = "PassivePostNakCauseTextLabel"
        Me.PassivePostNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassivePostNakCauseTextLabel.TabIndex = 5
        Me.PassivePostNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassivePostNakCauseTextTextBox
        '
        Me.PassivePostNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassivePostNakCauseTextTextBox.Location = New System.Drawing.Point(341, 241)
        Me.PassivePostNakCauseTextTextBox.MaxLength = 50
        Me.PassivePostNakCauseTextTextBox.Name = "PassivePostNakCauseTextTextBox"
        Me.PassivePostNakCauseTextTextBox.Size = New System.Drawing.Size(348, 19)
        Me.PassivePostNakCauseTextTextBox.TabIndex = 6
        '
        'PassiveDllTabPage
        '
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllFinishDetailLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllSimulateStoringCheckBox)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllDataGridView)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllStartLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllForceReplyNakCheckBox)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllNakCauseNumberLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllNakCauseNumberTextBox)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllNakCauseTextLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllNakCauseTextTextBox)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllFinishLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllTransferLimitLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllTransferLimitNumericUpDown)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllTransferLimitUnitLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllReplyLimitLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllReplyLimitNumericUpDown)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllReplyLimitUnitLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllResultantVersionOfSlot1Label)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllResultantVersionOfSlot1TextBox)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllResultantVersionOfSlot2Label)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllResultantVersionOfSlot2TextBox)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllResultantFlagOfFullLabel)
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllResultantFlagOfFullTextBox)
        Me.PassiveDllTabPage.Location = New System.Drawing.Point(4, 22)
        Me.PassiveDllTabPage.Name = "PassiveDllTabPage"
        Me.PassiveDllTabPage.Size = New System.Drawing.Size(692, 262)
        Me.PassiveDllTabPage.TabIndex = 6
        Me.PassiveDllTabPage.Text = "受動的DLL"
        Me.PassiveDllTabPage.UseVisualStyleBackColor = True
        '
        'PassiveDllFinishDetailLabel
        '
        Me.PassiveDllFinishDetailLabel.AutoSize = True
        Me.PassiveDllFinishDetailLabel.Location = New System.Drawing.Point(175, 16)
        Me.PassiveDllFinishDetailLabel.Name = "PassiveDllFinishDetailLabel"
        Me.PassiveDllFinishDetailLabel.Size = New System.Drawing.Size(87, 12)
        Me.PassiveDllFinishDetailLabel.TabIndex = 15
        Me.PassiveDllFinishDetailLabel.Text = "終了電文の詳細"
        '
        'PassiveDllSimulateStoringCheckBox
        '
        Me.PassiveDllSimulateStoringCheckBox.AutoSize = True
        Me.PassiveDllSimulateStoringCheckBox.Location = New System.Drawing.Point(224, 36)
        Me.PassiveDllSimulateStoringCheckBox.Name = "PassiveDllSimulateStoringCheckBox"
        Me.PassiveDllSimulateStoringCheckBox.Size = New System.Drawing.Size(326, 16)
        Me.PassiveDllSimulateStoringCheckBox.TabIndex = 16
        Me.PassiveDllSimulateStoringCheckBox.Text = "転送成功時に送信する電文は、保存も実施した体の電文とする"
        Me.PassiveDllSimulateStoringCheckBox.UseVisualStyleBackColor = True
        '
        'PassiveDllDataGridView
        '
        Me.PassiveDllDataGridView.AllowUserToDeleteRows = False
        Me.PassiveDllDataGridView.AllowUserToResizeRows = False
        Me.PassiveDllDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassiveDllDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassiveDllObjCodeColumn})
        Me.PassiveDllDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassiveDllDataGridView.MultiSelect = False
        Me.PassiveDllDataGridView.Name = "PassiveDllDataGridView"
        Me.PassiveDllDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassiveDllDataGridView.RowTemplate.Height = 21
        Me.PassiveDllDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassiveDllDataGridView.Size = New System.Drawing.Size(156, 203)
        Me.PassiveDllDataGridView.StandardTab = True
        Me.PassiveDllDataGridView.TabIndex = 1
        '
        'PassiveDllObjCodeColumn
        '
        Me.PassiveDllObjCodeColumn.HeaderText = "データ種別"
        Me.PassiveDllObjCodeColumn.Name = "PassiveDllObjCodeColumn"
        Me.PassiveDllObjCodeColumn.Width = 96
        '
        'PassiveDllStartLabel
        '
        Me.PassiveDllStartLabel.AutoSize = True
        Me.PassiveDllStartLabel.Location = New System.Drawing.Point(1, 217)
        Me.PassiveDllStartLabel.Name = "PassiveDllStartLabel"
        Me.PassiveDllStartLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveDllStartLabel.TabIndex = 2
        Me.PassiveDllStartLabel.Text = "開始時"
        '
        'PassiveDllForceReplyNakCheckBox
        '
        Me.PassiveDllForceReplyNakCheckBox.AutoSize = True
        Me.PassiveDllForceReplyNakCheckBox.Location = New System.Drawing.Point(56, 215)
        Me.PassiveDllForceReplyNakCheckBox.Name = "PassiveDllForceReplyNakCheckBox"
        Me.PassiveDllForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassiveDllForceReplyNakCheckBox.TabIndex = 3
        Me.PassiveDllForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassiveDllForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassiveDllNakCauseNumberLabel
        '
        Me.PassiveDllNakCauseNumberLabel.AutoSize = True
        Me.PassiveDllNakCauseNumberLabel.Location = New System.Drawing.Point(142, 217)
        Me.PassiveDllNakCauseNumberLabel.Name = "PassiveDllNakCauseNumberLabel"
        Me.PassiveDllNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveDllNakCauseNumberLabel.TabIndex = 4
        Me.PassiveDllNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassiveDllNakCauseNumberTextBox
        '
        Me.PassiveDllNakCauseNumberTextBox.Location = New System.Drawing.Point(224, 213)
        Me.PassiveDllNakCauseNumberTextBox.Mask = "999"
        Me.PassiveDllNakCauseNumberTextBox.Name = "PassiveDllNakCauseNumberTextBox"
        Me.PassiveDllNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassiveDllNakCauseNumberTextBox.TabIndex = 5
        '
        'PassiveDllNakCauseTextLabel
        '
        Me.PassiveDllNakCauseTextLabel.AutoSize = True
        Me.PassiveDllNakCauseTextLabel.Location = New System.Drawing.Point(280, 217)
        Me.PassiveDllNakCauseTextLabel.Name = "PassiveDllNakCauseTextLabel"
        Me.PassiveDllNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveDllNakCauseTextLabel.TabIndex = 6
        Me.PassiveDllNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassiveDllNakCauseTextTextBox
        '
        Me.PassiveDllNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllNakCauseTextTextBox.Location = New System.Drawing.Point(362, 213)
        Me.PassiveDllNakCauseTextTextBox.MaxLength = 50
        Me.PassiveDllNakCauseTextTextBox.Name = "PassiveDllNakCauseTextTextBox"
        Me.PassiveDllNakCauseTextTextBox.Size = New System.Drawing.Size(326, 19)
        Me.PassiveDllNakCauseTextTextBox.TabIndex = 7
        '
        'PassiveDllFinishLabel
        '
        Me.PassiveDllFinishLabel.AutoSize = True
        Me.PassiveDllFinishLabel.Location = New System.Drawing.Point(1, 244)
        Me.PassiveDllFinishLabel.Name = "PassiveDllFinishLabel"
        Me.PassiveDllFinishLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveDllFinishLabel.TabIndex = 8
        Me.PassiveDllFinishLabel.Text = "終了時"
        '
        'PassiveDllTransferLimitLabel
        '
        Me.PassiveDllTransferLimitLabel.AutoSize = True
        Me.PassiveDllTransferLimitLabel.Location = New System.Drawing.Point(55, 244)
        Me.PassiveDllTransferLimitLabel.Name = "PassiveDllTransferLimitLabel"
        Me.PassiveDllTransferLimitLabel.Size = New System.Drawing.Size(117, 12)
        Me.PassiveDllTransferLimitLabel.TabIndex = 9
        Me.PassiveDllTransferLimitLabel.Text = "転送期限（0は無期限）"
        '
        'PassiveDllTransferLimitNumericUpDown
        '
        Me.PassiveDllTransferLimitNumericUpDown.Location = New System.Drawing.Point(178, 242)
        Me.PassiveDllTransferLimitNumericUpDown.Maximum = New Decimal(New Integer() {43200000, 0, 0, 0})
        Me.PassiveDllTransferLimitNumericUpDown.Name = "PassiveDllTransferLimitNumericUpDown"
        Me.PassiveDllTransferLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveDllTransferLimitNumericUpDown.TabIndex = 10
        '
        'PassiveDllTransferLimitUnitLabel
        '
        Me.PassiveDllTransferLimitUnitLabel.AutoSize = True
        Me.PassiveDllTransferLimitUnitLabel.Location = New System.Drawing.Point(264, 244)
        Me.PassiveDllTransferLimitUnitLabel.Name = "PassiveDllTransferLimitUnitLabel"
        Me.PassiveDllTransferLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.PassiveDllTransferLimitUnitLabel.TabIndex = 11
        Me.PassiveDllTransferLimitUnitLabel.Text = "ms"
        '
        'PassiveDllReplyLimitLabel
        '
        Me.PassiveDllReplyLimitLabel.AutoSize = True
        Me.PassiveDllReplyLimitLabel.Location = New System.Drawing.Point(299, 244)
        Me.PassiveDllReplyLimitLabel.Name = "PassiveDllReplyLimitLabel"
        Me.PassiveDllReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.PassiveDllReplyLimitLabel.TabIndex = 12
        Me.PassiveDllReplyLimitLabel.Text = "応答受信期限"
        '
        'PassiveDllReplyLimitNumericUpDown
        '
        Me.PassiveDllReplyLimitNumericUpDown.Location = New System.Drawing.Point(382, 242)
        Me.PassiveDllReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.PassiveDllReplyLimitNumericUpDown.Name = "PassiveDllReplyLimitNumericUpDown"
        Me.PassiveDllReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveDllReplyLimitNumericUpDown.TabIndex = 13
        '
        'PassiveDllReplyLimitUnitLabel
        '
        Me.PassiveDllReplyLimitUnitLabel.AutoSize = True
        Me.PassiveDllReplyLimitUnitLabel.Location = New System.Drawing.Point(468, 244)
        Me.PassiveDllReplyLimitUnitLabel.Name = "PassiveDllReplyLimitUnitLabel"
        Me.PassiveDllReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.PassiveDllReplyLimitUnitLabel.TabIndex = 14
        Me.PassiveDllReplyLimitUnitLabel.Text = "ms"
        '
        'PassiveDllResultantVersionOfSlot1Label
        '
        Me.PassiveDllResultantVersionOfSlot1Label.AutoSize = True
        Me.PassiveDllResultantVersionOfSlot1Label.Location = New System.Drawing.Point(222, 61)
        Me.PassiveDllResultantVersionOfSlot1Label.Name = "PassiveDllResultantVersionOfSlot1Label"
        Me.PassiveDllResultantVersionOfSlot1Label.Size = New System.Drawing.Size(164, 12)
        Me.PassiveDllResultantVersionOfSlot1Label.TabIndex = 17
        Me.PassiveDllResultantVersionOfSlot1Label.Text = "監視盤保持バージョン（1世代目）"
        '
        'PassiveDllResultantVersionOfSlot1TextBox
        '
        Me.PassiveDllResultantVersionOfSlot1TextBox.Location = New System.Drawing.Point(392, 58)
        Me.PassiveDllResultantVersionOfSlot1TextBox.Mask = "99999999"
        Me.PassiveDllResultantVersionOfSlot1TextBox.Name = "PassiveDllResultantVersionOfSlot1TextBox"
        Me.PassiveDllResultantVersionOfSlot1TextBox.Size = New System.Drawing.Size(83, 19)
        Me.PassiveDllResultantVersionOfSlot1TextBox.TabIndex = 18
        '
        'PassiveDllResultantVersionOfSlot2Label
        '
        Me.PassiveDllResultantVersionOfSlot2Label.AutoSize = True
        Me.PassiveDllResultantVersionOfSlot2Label.Location = New System.Drawing.Point(222, 86)
        Me.PassiveDllResultantVersionOfSlot2Label.Name = "PassiveDllResultantVersionOfSlot2Label"
        Me.PassiveDllResultantVersionOfSlot2Label.Size = New System.Drawing.Size(164, 12)
        Me.PassiveDllResultantVersionOfSlot2Label.TabIndex = 19
        Me.PassiveDllResultantVersionOfSlot2Label.Text = "監視盤保持バージョン（2世代目）"
        '
        'PassiveDllResultantVersionOfSlot2TextBox
        '
        Me.PassiveDllResultantVersionOfSlot2TextBox.Location = New System.Drawing.Point(392, 83)
        Me.PassiveDllResultantVersionOfSlot2TextBox.Mask = "99999999"
        Me.PassiveDllResultantVersionOfSlot2TextBox.Name = "PassiveDllResultantVersionOfSlot2TextBox"
        Me.PassiveDllResultantVersionOfSlot2TextBox.Size = New System.Drawing.Size(83, 19)
        Me.PassiveDllResultantVersionOfSlot2TextBox.TabIndex = 20
        '
        'PassiveDllResultantFlagOfFullLabel
        '
        Me.PassiveDllResultantFlagOfFullLabel.AutoSize = True
        Me.PassiveDllResultantFlagOfFullLabel.Location = New System.Drawing.Point(294, 111)
        Me.PassiveDllResultantFlagOfFullLabel.Name = "PassiveDllResultantFlagOfFullLabel"
        Me.PassiveDllResultantFlagOfFullLabel.Size = New System.Drawing.Size(78, 12)
        Me.PassiveDllResultantFlagOfFullLabel.TabIndex = 21
        Me.PassiveDllResultantFlagOfFullLabel.Text = "受信可能フラグ"
        '
        'PassiveDllResultantFlagOfFullTextBox
        '
        Me.PassiveDllResultantFlagOfFullTextBox.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.PassiveDllResultantFlagOfFullTextBox.Location = New System.Drawing.Point(392, 108)
        Me.PassiveDllResultantFlagOfFullTextBox.MaxLength = 2
        Me.PassiveDllResultantFlagOfFullTextBox.Name = "PassiveDllResultantFlagOfFullTextBox"
        Me.PassiveDllResultantFlagOfFullTextBox.Size = New System.Drawing.Size(34, 19)
        Me.PassiveDllResultantFlagOfFullTextBox.TabIndex = 22
        '
        'ScenarioTabPage
        '
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioExecIntervalLabel)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioExecIntervalNumericUpDown)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioExecIntervalUnitLabel)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioFileLabel)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioFileTextBox)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioFileSelButton)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioExecButton)
        Me.ScenarioTabPage.Location = New System.Drawing.Point(4, 22)
        Me.ScenarioTabPage.Name = "ScenarioTabPage"
        Me.ScenarioTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.ScenarioTabPage.Size = New System.Drawing.Size(692, 262)
        Me.ScenarioTabPage.TabIndex = 7
        Me.ScenarioTabPage.Text = "シナリオ"
        Me.ScenarioTabPage.UseVisualStyleBackColor = True
        '
        'ScenarioExecIntervalLabel
        '
        Me.ScenarioExecIntervalLabel.AutoSize = True
        Me.ScenarioExecIntervalLabel.Location = New System.Drawing.Point(7, 62)
        Me.ScenarioExecIntervalLabel.Name = "ScenarioExecIntervalLabel"
        Me.ScenarioExecIntervalLabel.Size = New System.Drawing.Size(105, 12)
        Me.ScenarioExecIntervalLabel.TabIndex = 4
        Me.ScenarioExecIntervalLabel.Text = "実行間隔（0は単発）"
        '
        'ScenarioExecIntervalNumericUpDown
        '
        Me.ScenarioExecIntervalNumericUpDown.Location = New System.Drawing.Point(125, 60)
        Me.ScenarioExecIntervalNumericUpDown.Maximum = New Decimal(New Integer() {86400000, 0, 0, 0})
        Me.ScenarioExecIntervalNumericUpDown.Name = "ScenarioExecIntervalNumericUpDown"
        Me.ScenarioExecIntervalNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ScenarioExecIntervalNumericUpDown.TabIndex = 5
        '
        'ScenarioExecIntervalUnitLabel
        '
        Me.ScenarioExecIntervalUnitLabel.AutoSize = True
        Me.ScenarioExecIntervalUnitLabel.Location = New System.Drawing.Point(211, 62)
        Me.ScenarioExecIntervalUnitLabel.Name = "ScenarioExecIntervalUnitLabel"
        Me.ScenarioExecIntervalUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ScenarioExecIntervalUnitLabel.TabIndex = 6
        Me.ScenarioExecIntervalUnitLabel.Text = "ms"
        '
        'ScenarioFileLabel
        '
        Me.ScenarioFileLabel.AutoSize = True
        Me.ScenarioFileLabel.Location = New System.Drawing.Point(7, 18)
        Me.ScenarioFileLabel.Name = "ScenarioFileLabel"
        Me.ScenarioFileLabel.Size = New System.Drawing.Size(87, 12)
        Me.ScenarioFileLabel.TabIndex = 1
        Me.ScenarioFileLabel.Text = "シナリオファイル名"
        '
        'ScenarioFileTextBox
        '
        Me.ScenarioFileTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ScenarioFileTextBox.Location = New System.Drawing.Point(100, 15)
        Me.ScenarioFileTextBox.Name = "ScenarioFileTextBox"
        Me.ScenarioFileTextBox.Size = New System.Drawing.Size(524, 19)
        Me.ScenarioFileTextBox.TabIndex = 2
        '
        'ScenarioFileSelButton
        '
        Me.ScenarioFileSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ScenarioFileSelButton.Location = New System.Drawing.Point(630, 13)
        Me.ScenarioFileSelButton.Name = "ScenarioFileSelButton"
        Me.ScenarioFileSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ScenarioFileSelButton.TabIndex = 3
        Me.ScenarioFileSelButton.Text = "選択"
        Me.ScenarioFileSelButton.UseVisualStyleBackColor = True
        '
        'ScenarioExecButton
        '
        Me.ScenarioExecButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ScenarioExecButton.Location = New System.Drawing.Point(594, 54)
        Me.ScenarioExecButton.Name = "ScenarioExecButton"
        Me.ScenarioExecButton.Size = New System.Drawing.Size(86, 28)
        Me.ScenarioExecButton.TabIndex = 7
        Me.ScenarioExecButton.Text = "実行"
        Me.ScenarioExecButton.UseVisualStyleBackColor = True
        '
        'ActiveOneExecTimer
        '
        '
        'ActiveUllExecTimer
        '
        '
        'PassiveGetRowHeaderMenu
        '
        Me.PassiveGetRowHeaderMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PassiveGetDelMenuItem})
        Me.PassiveGetRowHeaderMenu.Name = "PassiveGetRowHeaderMenu"
        Me.PassiveGetRowHeaderMenu.Size = New System.Drawing.Size(101, 26)
        '
        'PassiveGetDelMenuItem
        '
        Me.PassiveGetDelMenuItem.Name = "PassiveGetDelMenuItem"
        Me.PassiveGetDelMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.PassiveGetDelMenuItem.Text = "削除"
        '
        'PassiveGetApplyFileMenu
        '
        Me.PassiveGetApplyFileMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PassiveGetSelMenuItem})
        Me.PassiveGetApplyFileMenu.Name = "PassiveGetApplyFileMenu"
        Me.PassiveGetApplyFileMenu.Size = New System.Drawing.Size(101, 26)
        '
        'PassiveGetSelMenuItem
        '
        Me.PassiveGetSelMenuItem.Name = "PassiveGetSelMenuItem"
        Me.PassiveGetSelMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.PassiveGetSelMenuItem.Text = "選択"
        '
        'PassiveUllRowHeaderMenu
        '
        Me.PassiveUllRowHeaderMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PassiveUllDelMenuItem})
        Me.PassiveUllRowHeaderMenu.Name = "PassiveUllRowHeaderMenu"
        Me.PassiveUllRowHeaderMenu.Size = New System.Drawing.Size(101, 26)
        '
        'PassiveUllDelMenuItem
        '
        Me.PassiveUllDelMenuItem.Name = "PassiveUllDelMenuItem"
        Me.PassiveUllDelMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.PassiveUllDelMenuItem.Text = "削除"
        '
        'PassiveUllApplyFileMenu
        '
        Me.PassiveUllApplyFileMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PassiveUllSelMenuItem})
        Me.PassiveUllApplyFileMenu.Name = "PassiveUllApplyFileMenu"
        Me.PassiveUllApplyFileMenu.Size = New System.Drawing.Size(101, 26)
        '
        'PassiveUllSelMenuItem
        '
        Me.PassiveUllSelMenuItem.Name = "PassiveUllSelMenuItem"
        Me.PassiveUllSelMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.PassiveUllSelMenuItem.Text = "選択"
        '
        'PassivePostRowHeaderMenu
        '
        Me.PassivePostRowHeaderMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PassivePostDelMenuItem})
        Me.PassivePostRowHeaderMenu.Name = "PassivePostRowHeaderMenu"
        Me.PassivePostRowHeaderMenu.Size = New System.Drawing.Size(101, 26)
        '
        'PassivePostDelMenuItem
        '
        Me.PassivePostDelMenuItem.Name = "PassivePostDelMenuItem"
        Me.PassivePostDelMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.PassivePostDelMenuItem.Text = "削除"
        '
        'PassiveDllRowHeaderMenu
        '
        Me.PassiveDllRowHeaderMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PassiveDllDelMenuItem})
        Me.PassiveDllRowHeaderMenu.Name = "PassiveDllRowHeaderMenu"
        Me.PassiveDllRowHeaderMenu.Size = New System.Drawing.Size(101, 26)
        '
        'PassiveDllDelMenuItem
        '
        Me.PassiveDllDelMenuItem.Name = "PassiveDllDelMenuItem"
        Me.PassiveDllDelMenuItem.Size = New System.Drawing.Size(100, 22)
        Me.PassiveDllDelMenuItem.Text = "削除"
        '
        'ScenarioExecTimer
        '
        '
        'LoggerPreviewCheckBox
        '
        Me.LoggerPreviewCheckBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LoggerPreviewCheckBox.AutoSize = True
        Me.LoggerPreviewCheckBox.Checked = True
        Me.LoggerPreviewCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.LoggerPreviewCheckBox.Location = New System.Drawing.Point(483, 300)
        Me.LoggerPreviewCheckBox.Name = "LoggerPreviewCheckBox"
        Me.LoggerPreviewCheckBox.Size = New System.Drawing.Size(94, 16)
        Me.LoggerPreviewCheckBox.TabIndex = 2
        Me.LoggerPreviewCheckBox.Text = "ログを表示する"
        Me.LoggerPreviewCheckBox.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(725, 531)
        Me.Controls.Add(Me.LoggerPreviewCheckBox)
        Me.Controls.Add(Me.LoggerTextBox)
        Me.Controls.Add(Me.LoggerClearButton)
        Me.Controls.Add(Me.ConButton)
        Me.Controls.Add(Me.SeqTabControl)
        Me.Name = "MainForm"
        Me.Text = "駅務機器"
        Me.SeqTabControl.ResumeLayout(False)
        Me.BasicTabPage.ResumeLayout(False)
        Me.BasicTabPage.PerformLayout()
        Me.ActiveOneTabPage.ResumeLayout(False)
        Me.ActiveOneTabPage.PerformLayout()
        CType(Me.ActiveOneReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveOneExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ActiveUllTabPage.ResumeLayout(False)
        Me.ActiveUllTabPage.PerformLayout()
        CType(Me.ActiveUllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveUllExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassiveGetTabPage.ResumeLayout(False)
        Me.PassiveGetTabPage.PerformLayout()
        CType(Me.PassiveGetDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassiveUllTabPage.ResumeLayout(False)
        Me.PassiveUllTabPage.PerformLayout()
        CType(Me.PassiveUllDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveUllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassivePostTabPage.ResumeLayout(False)
        Me.PassivePostTabPage.PerformLayout()
        CType(Me.PassivePostDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassiveDllTabPage.ResumeLayout(False)
        Me.PassiveDllTabPage.PerformLayout()
        CType(Me.PassiveDllDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveDllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveDllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ScenarioTabPage.ResumeLayout(False)
        Me.ScenarioTabPage.PerformLayout()
        CType(Me.ScenarioExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassiveGetRowHeaderMenu.ResumeLayout(False)
        Me.PassiveGetApplyFileMenu.ResumeLayout(False)
        Me.PassiveUllRowHeaderMenu.ResumeLayout(False)
        Me.PassiveUllApplyFileMenu.ResumeLayout(False)
        Me.PassivePostRowHeaderMenu.ResumeLayout(False)
        Me.PassiveDllRowHeaderMenu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FileSelDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents LineStatusPollTimer As System.Windows.Forms.Timer
    Friend WithEvents ConButton As System.Windows.Forms.Button
    Friend WithEvents LoggerTextBox As System.Windows.Forms.TextBox
    Friend WithEvents LoggerClearButton As System.Windows.Forms.Button
    Friend WithEvents SeqTabControl As System.Windows.Forms.TabControl
    Friend WithEvents BasicTabPage As System.Windows.Forms.TabPage
    Friend WithEvents ActiveOneTabPage As System.Windows.Forms.TabPage
    Friend WithEvents ActiveUllTabPage As System.Windows.Forms.TabPage
    Friend WithEvents PassiveGetTabPage As System.Windows.Forms.TabPage
    Friend WithEvents PassivePostTabPage As System.Windows.Forms.TabPage
    Friend WithEvents PassiveUllTabPage As System.Windows.Forms.TabPage
    Friend WithEvents PassiveDllTabPage As System.Windows.Forms.TabPage
    Friend WithEvents TimeDataGetButton As System.Windows.Forms.Button
    Friend WithEvents ComSartButton As System.Windows.Forms.Button
    Friend WithEvents AutomaticComStartCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapRcvFilesCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapSndFilesCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapRcvTelegsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapSndTelegsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ActiveOneApplyFileLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveOneApplyFileTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ActiveOneApplyFileSelButton As System.Windows.Forms.Button
    Friend WithEvents ActiveOneReplyLimitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveOneReplyLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveOneReplyLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveOneExecIntervalLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveOneExecIntervalNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveOneExecIntervalUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveOneExecButton As System.Windows.Forms.Button
    Friend WithEvents ActiveOneExecTimer As System.Windows.Forms.Timer
    Friend WithEvents ActiveUllObjCodeLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllObjCodeTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ActiveUllTransferFileLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllTransferFileTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ActiveUllTransferFileSelButton As System.Windows.Forms.Button
    Friend WithEvents ActiveUllReplyLimitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllReplyLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveUllReplyLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllTransferLimitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllTransferLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveUllTransferLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllExecIntervalLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllExecIntervalNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveUllExecIntervalUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllExecButton As System.Windows.Forms.Button
    Friend WithEvents ActiveUllExecTimer As System.Windows.Forms.Timer
    Friend WithEvents PassiveGetDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PassiveGetForceReplyNakCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassiveGetNakCauseNumberLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveGetNakCauseNumberTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveGetNakCauseTextLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveGetNakCauseTextTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassiveGetRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveGetDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveGetApplyFileMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveGetSelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveUllDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PassiveUllStartLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllForceReplyNakCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassiveUllNakCauseNumberLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllNakCauseNumberTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveUllNakCauseTextLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllNakCauseTextTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassiveUllFinishLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllTransferLimitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllTransferLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents PassiveUllTransferLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllReplyLimitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllReplyLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents PassiveUllReplyLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveUllRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveUllDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveUllApplyFileMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveUllSelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassivePostDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PassivePostForceReplyNakCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassivePostNakCauseNumberLabel As System.Windows.Forms.Label
    Friend WithEvents PassivePostNakCauseNumberTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassivePostNakCauseTextLabel As System.Windows.Forms.Label
    Friend WithEvents PassivePostNakCauseTextTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassivePostRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassivePostDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveDllDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PassiveDllStartLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllForceReplyNakCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassiveDllNakCauseNumberLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllNakCauseNumberTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveDllNakCauseTextLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllNakCauseTextTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassiveDllFinishLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllTransferLimitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllTransferLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents PassiveDllTransferLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllReplyLimitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllReplyLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents PassiveDllReplyLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveDllDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassivePostObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveDllObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveDllResultantVersionOfSlot1Label As System.Windows.Forms.Label
    Friend WithEvents PassiveDllResultantVersionOfSlot1TextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveDllResultantVersionOfSlot2Label As System.Windows.Forms.Label
    Friend WithEvents PassiveDllResultantVersionOfSlot2TextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveDllResultantFlagOfFullLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllResultantFlagOfFullTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassiveDllFinishDetailLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllSimulateStoringCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ScenarioTabPage As System.Windows.Forms.TabPage
    Friend WithEvents ScenarioFileLabel As System.Windows.Forms.Label
    Friend WithEvents ScenarioFileTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ScenarioFileSelButton As System.Windows.Forms.Button
    Friend WithEvents ScenarioExecButton As System.Windows.Forms.Button
    Friend WithEvents ScenarioExecTimer As System.Windows.Forms.Timer
    Friend WithEvents PassiveGetObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveGetApplyFileColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveUllObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveUllApplyFileColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents LoggerPreviewCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ScenarioExecIntervalLabel As System.Windows.Forms.Label
    Friend WithEvents ScenarioExecIntervalNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ScenarioExecIntervalUnitLabel As System.Windows.Forms.Label

End Class
