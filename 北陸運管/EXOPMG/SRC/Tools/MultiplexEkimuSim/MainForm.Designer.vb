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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.FileSelDialog = New System.Windows.Forms.OpenFileDialog()
        Me.StatusPollTimer = New System.Windows.Forms.Timer(Me.components)
        Me.ConButton = New System.Windows.Forms.Button()
        Me.LogDispClearButton = New System.Windows.Forms.Button()
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
        Me.LogDispCheckBox = New System.Windows.Forms.CheckBox()
        Me.ScenarioTabPage = New System.Windows.Forms.TabPage()
        Me.ScenarioStopButton = New System.Windows.Forms.Button()
        Me.ScenarioStartButton = New System.Windows.Forms.Button()
        Me.ScenarioStartDateTimePicker = New System.Windows.Forms.DateTimePicker()
        Me.ScenarioStartDateTimeLabel = New System.Windows.Forms.Label()
        Me.ScenarioStartDateTimeCheckBox = New System.Windows.Forms.CheckBox()
        Me.ScenarioFileLabel = New System.Windows.Forms.Label()
        Me.ScenarioFileTextBox = New System.Windows.Forms.TextBox()
        Me.ScenarioFileSelButton = New System.Windows.Forms.Button()
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
        Me.PassiveDllResultantFlagOfFullTextBox = New System.Windows.Forms.TextBox()
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
        Me.PassivePostTabPage = New System.Windows.Forms.TabPage()
        Me.PassivePostDataGridView = New System.Windows.Forms.DataGridView()
        Me.PassivePostObjCodeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassivePostForceReplyNakCheckBox = New System.Windows.Forms.CheckBox()
        Me.PassivePostNakCauseNumberLabel = New System.Windows.Forms.Label()
        Me.PassivePostNakCauseNumberTextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassivePostNakCauseTextLabel = New System.Windows.Forms.Label()
        Me.PassivePostNakCauseTextTextBox = New System.Windows.Forms.TextBox()
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
        Me.PassiveGetTabPage = New System.Windows.Forms.TabPage()
        Me.PassiveGetDataGridView = New System.Windows.Forms.DataGridView()
        Me.PassiveGetObjCodeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassiveGetApplyFileColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PassiveGetForceReplyNakCheckBox = New System.Windows.Forms.CheckBox()
        Me.PassiveGetNakCauseNumberLabel = New System.Windows.Forms.Label()
        Me.PassiveGetNakCauseNumberTextBox = New System.Windows.Forms.MaskedTextBox()
        Me.PassiveGetNakCauseTextLabel = New System.Windows.Forms.Label()
        Me.PassiveGetNakCauseTextTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveUllTabPage = New System.Windows.Forms.TabPage()
        Me.ActiveUllFinishReplyLimitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllFinishReplyLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveUllFinishReplyLimitUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllTransferNameTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveUllTransferNameLabel = New System.Windows.Forms.Label()
        Me.ActiveUllTransferNameSelButton = New System.Windows.Forms.Button()
        Me.ActiveUllObjCodeLabel = New System.Windows.Forms.Label()
        Me.ActiveUllObjCodeTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveUllApplyFileTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveUllApplyFileLabel = New System.Windows.Forms.Label()
        Me.ActiveUllApplyFileSelButton = New System.Windows.Forms.Button()
        Me.ActiveUllStartReplyLimitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllStartReplyLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveUllStartReplyLimitUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllTransferLimitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllTransferLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveUllTransferLimitUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllExecIntervalLabel = New System.Windows.Forms.Label()
        Me.ActiveUllExecIntervalNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveUllExecIntervalUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveUllExecButton = New System.Windows.Forms.Button()
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
        Me.ConnectionTabPage = New System.Windows.Forms.TabPage()
        Me.DisconButton = New System.Windows.Forms.Button()
        Me.TimeDataGetButton = New System.Windows.Forms.Button()
        Me.ComSartButton = New System.Windows.Forms.Button()
        Me.AutomaticComStartCheckBox = New System.Windows.Forms.CheckBox()
        Me.CapRcvFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.CapSndFilesCheckBox = New System.Windows.Forms.CheckBox()
        Me.CapRcvTelegsCheckBox = New System.Windows.Forms.CheckBox()
        Me.CapSndTelegsCheckBox = New System.Windows.Forms.CheckBox()
        Me.SeqTabControl = New System.Windows.Forms.TabControl()
        Me.CapTabPage = New System.Windows.Forms.TabPage()
        Me.ClientDataGridView = New System.Windows.Forms.DataGridView()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SymbolizeCheckBox = New System.Windows.Forms.CheckBox()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.LogDispFilterEditButton = New System.Windows.Forms.Button()
        Me.LogDispFilter = New System.Windows.Forms.TextBox()
        Me.LogDispFilterLabel = New System.Windows.Forms.Label()
        Me.LogDispGrid = New System.Windows.Forms.DataGridView()
        Me.UsageToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.PassiveGetRowHeaderMenu.SuspendLayout()
        Me.PassiveGetApplyFileMenu.SuspendLayout()
        Me.PassiveUllRowHeaderMenu.SuspendLayout()
        Me.PassiveUllApplyFileMenu.SuspendLayout()
        Me.PassivePostRowHeaderMenu.SuspendLayout()
        Me.PassiveDllRowHeaderMenu.SuspendLayout()
        Me.ScenarioTabPage.SuspendLayout()
        Me.PassiveDllTabPage.SuspendLayout()
        CType(Me.PassiveDllDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveDllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveDllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassivePostTabPage.SuspendLayout()
        CType(Me.PassivePostDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassiveUllTabPage.SuspendLayout()
        CType(Me.PassiveUllDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PassiveUllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PassiveGetTabPage.SuspendLayout()
        CType(Me.PassiveGetDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ActiveUllTabPage.SuspendLayout()
        CType(Me.ActiveUllFinishReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveUllStartReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveUllExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ActiveOneTabPage.SuspendLayout()
        CType(Me.ActiveOneReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveOneExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ConnectionTabPage.SuspendLayout()
        Me.SeqTabControl.SuspendLayout()
        Me.CapTabPage.SuspendLayout()
        CType(Me.ClientDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.LogDispGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusPollTimer
        '
        Me.StatusPollTimer.Interval = 500
        '
        'ConButton
        '
        Me.ConButton.Location = New System.Drawing.Point(20, 19)
        Me.ConButton.Name = "ConButton"
        Me.ConButton.Size = New System.Drawing.Size(145, 28)
        Me.ConButton.TabIndex = 1
        Me.ConButton.Text = "接続"
        Me.ConButton.UseVisualStyleBackColor = True
        '
        'LogDispClearButton
        '
        Me.LogDispClearButton.Location = New System.Drawing.Point(100, 5)
        Me.LogDispClearButton.Name = "LogDispClearButton"
        Me.LogDispClearButton.Size = New System.Drawing.Size(53, 23)
        Me.LogDispClearButton.TabIndex = 1
        Me.LogDispClearButton.Text = "クリア"
        Me.LogDispClearButton.UseVisualStyleBackColor = True
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
        'LogDispCheckBox
        '
        Me.LogDispCheckBox.AutoSize = True
        Me.LogDispCheckBox.Checked = True
        Me.LogDispCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.LogDispCheckBox.Location = New System.Drawing.Point(4, 8)
        Me.LogDispCheckBox.Name = "LogDispCheckBox"
        Me.LogDispCheckBox.Size = New System.Drawing.Size(90, 16)
        Me.LogDispCheckBox.TabIndex = 0
        Me.LogDispCheckBox.Text = "新着ログ取込"
        Me.LogDispCheckBox.UseVisualStyleBackColor = True
        '
        'ScenarioTabPage
        '
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioStopButton)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioStartButton)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioStartDateTimePicker)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioStartDateTimeLabel)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioStartDateTimeCheckBox)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioFileLabel)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioFileTextBox)
        Me.ScenarioTabPage.Controls.Add(Me.ScenarioFileSelButton)
        Me.ScenarioTabPage.Location = New System.Drawing.Point(4, 22)
        Me.ScenarioTabPage.Name = "ScenarioTabPage"
        Me.ScenarioTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.ScenarioTabPage.Size = New System.Drawing.Size(662, 182)
        Me.ScenarioTabPage.TabIndex = 7
        Me.ScenarioTabPage.Text = "シナリオ"
        Me.ScenarioTabPage.UseVisualStyleBackColor = True
        '
        'ScenarioStopButton
        '
        Me.ScenarioStopButton.Location = New System.Drawing.Point(160, 122)
        Me.ScenarioStopButton.Name = "ScenarioStopButton"
        Me.ScenarioStopButton.Size = New System.Drawing.Size(145, 28)
        Me.ScenarioStopButton.TabIndex = 19
        Me.ScenarioStopButton.Text = "停止"
        Me.ScenarioStopButton.UseVisualStyleBackColor = True
        '
        'ScenarioStartButton
        '
        Me.ScenarioStartButton.Location = New System.Drawing.Point(9, 122)
        Me.ScenarioStartButton.Name = "ScenarioStartButton"
        Me.ScenarioStartButton.Size = New System.Drawing.Size(145, 28)
        Me.ScenarioStartButton.TabIndex = 18
        Me.ScenarioStartButton.Text = "開始"
        Me.ScenarioStartButton.UseVisualStyleBackColor = True
        '
        'ScenarioStartDateTimePicker
        '
        Me.ScenarioStartDateTimePicker.CustomFormat = "yyyy/MM/dd  HH:mm:ss"
        Me.ScenarioStartDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.ScenarioStartDateTimePicker.Location = New System.Drawing.Point(100, 79)
        Me.ScenarioStartDateTimePicker.Name = "ScenarioStartDateTimePicker"
        Me.ScenarioStartDateTimePicker.Size = New System.Drawing.Size(151, 19)
        Me.ScenarioStartDateTimePicker.TabIndex = 16
        '
        'ScenarioStartDateTimeLabel
        '
        Me.ScenarioStartDateTimeLabel.AutoSize = True
        Me.ScenarioStartDateTimeLabel.Location = New System.Drawing.Point(41, 83)
        Me.ScenarioStartDateTimeLabel.Name = "ScenarioStartDateTimeLabel"
        Me.ScenarioStartDateTimeLabel.Size = New System.Drawing.Size(53, 12)
        Me.ScenarioStartDateTimeLabel.TabIndex = 15
        Me.ScenarioStartDateTimeLabel.Text = "指定日時"
        '
        'ScenarioStartDateTimeCheckBox
        '
        Me.ScenarioStartDateTimeCheckBox.AutoSize = True
        Me.ScenarioStartDateTimeCheckBox.Location = New System.Drawing.Point(23, 55)
        Me.ScenarioStartDateTimeCheckBox.Name = "ScenarioStartDateTimeCheckBox"
        Me.ScenarioStartDateTimeCheckBox.Size = New System.Drawing.Size(214, 16)
        Me.ScenarioStartDateTimeCheckBox.TabIndex = 14
        Me.ScenarioStartDateTimeCheckBox.Text = "指定日時まで待ってからMainを実行する"
        Me.ScenarioStartDateTimeCheckBox.UseVisualStyleBackColor = True
        '
        'ScenarioFileLabel
        '
        Me.ScenarioFileLabel.AutoSize = True
        Me.ScenarioFileLabel.Location = New System.Drawing.Point(19, 18)
        Me.ScenarioFileLabel.Name = "ScenarioFileLabel"
        Me.ScenarioFileLabel.Size = New System.Drawing.Size(75, 12)
        Me.ScenarioFileLabel.TabIndex = 11
        Me.ScenarioFileLabel.Text = "シナリオファイル"
        '
        'ScenarioFileTextBox
        '
        Me.ScenarioFileTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ScenarioFileTextBox.Location = New System.Drawing.Point(100, 15)
        Me.ScenarioFileTextBox.Name = "ScenarioFileTextBox"
        Me.ScenarioFileTextBox.Size = New System.Drawing.Size(498, 19)
        Me.ScenarioFileTextBox.TabIndex = 12
        '
        'ScenarioFileSelButton
        '
        Me.ScenarioFileSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ScenarioFileSelButton.Location = New System.Drawing.Point(604, 13)
        Me.ScenarioFileSelButton.Name = "ScenarioFileSelButton"
        Me.ScenarioFileSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ScenarioFileSelButton.TabIndex = 13
        Me.ScenarioFileSelButton.Text = "選択"
        Me.ScenarioFileSelButton.UseVisualStyleBackColor = True
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
        Me.PassiveDllTabPage.Controls.Add(Me.PassiveDllResultantFlagOfFullTextBox)
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
        Me.PassiveDllTabPage.Location = New System.Drawing.Point(4, 22)
        Me.PassiveDllTabPage.Name = "PassiveDllTabPage"
        Me.PassiveDllTabPage.Size = New System.Drawing.Size(662, 182)
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
        Me.PassiveDllDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassiveDllDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassiveDllObjCodeColumn})
        Me.PassiveDllDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassiveDllDataGridView.MultiSelect = False
        Me.PassiveDllDataGridView.Name = "PassiveDllDataGridView"
        Me.PassiveDllDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassiveDllDataGridView.RowTemplate.Height = 21
        Me.PassiveDllDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassiveDllDataGridView.Size = New System.Drawing.Size(156, 122)
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
        Me.PassiveDllStartLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllStartLabel.AutoSize = True
        Me.PassiveDllStartLabel.Location = New System.Drawing.Point(1, 136)
        Me.PassiveDllStartLabel.Name = "PassiveDllStartLabel"
        Me.PassiveDllStartLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveDllStartLabel.TabIndex = 2
        Me.PassiveDllStartLabel.Text = "開始時"
        '
        'PassiveDllForceReplyNakCheckBox
        '
        Me.PassiveDllForceReplyNakCheckBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllForceReplyNakCheckBox.AutoSize = True
        Me.PassiveDllForceReplyNakCheckBox.Location = New System.Drawing.Point(56, 136)
        Me.PassiveDllForceReplyNakCheckBox.Name = "PassiveDllForceReplyNakCheckBox"
        Me.PassiveDllForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassiveDllForceReplyNakCheckBox.TabIndex = 3
        Me.PassiveDllForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassiveDllForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassiveDllNakCauseNumberLabel
        '
        Me.PassiveDllNakCauseNumberLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllNakCauseNumberLabel.AutoSize = True
        Me.PassiveDllNakCauseNumberLabel.Location = New System.Drawing.Point(142, 136)
        Me.PassiveDllNakCauseNumberLabel.Name = "PassiveDllNakCauseNumberLabel"
        Me.PassiveDllNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveDllNakCauseNumberLabel.TabIndex = 4
        Me.PassiveDllNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassiveDllNakCauseNumberTextBox
        '
        Me.PassiveDllNakCauseNumberTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllNakCauseNumberTextBox.Location = New System.Drawing.Point(224, 132)
        Me.PassiveDllNakCauseNumberTextBox.Mask = "999"
        Me.PassiveDllNakCauseNumberTextBox.Name = "PassiveDllNakCauseNumberTextBox"
        Me.PassiveDllNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassiveDllNakCauseNumberTextBox.TabIndex = 5
        '
        'PassiveDllNakCauseTextLabel
        '
        Me.PassiveDllNakCauseTextLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllNakCauseTextLabel.AutoSize = True
        Me.PassiveDllNakCauseTextLabel.Location = New System.Drawing.Point(280, 136)
        Me.PassiveDllNakCauseTextLabel.Name = "PassiveDllNakCauseTextLabel"
        Me.PassiveDllNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveDllNakCauseTextLabel.TabIndex = 6
        Me.PassiveDllNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassiveDllNakCauseTextTextBox
        '
        Me.PassiveDllNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllNakCauseTextTextBox.Location = New System.Drawing.Point(362, 132)
        Me.PassiveDllNakCauseTextTextBox.MaxLength = 50
        Me.PassiveDllNakCauseTextTextBox.Name = "PassiveDllNakCauseTextTextBox"
        Me.PassiveDllNakCauseTextTextBox.Size = New System.Drawing.Size(292, 19)
        Me.PassiveDllNakCauseTextTextBox.TabIndex = 7
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
        'PassiveDllFinishLabel
        '
        Me.PassiveDllFinishLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllFinishLabel.AutoSize = True
        Me.PassiveDllFinishLabel.Location = New System.Drawing.Point(1, 163)
        Me.PassiveDllFinishLabel.Name = "PassiveDllFinishLabel"
        Me.PassiveDllFinishLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveDllFinishLabel.TabIndex = 8
        Me.PassiveDllFinishLabel.Text = "終了時"
        '
        'PassiveDllTransferLimitLabel
        '
        Me.PassiveDllTransferLimitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllTransferLimitLabel.AutoSize = True
        Me.PassiveDllTransferLimitLabel.Location = New System.Drawing.Point(55, 163)
        Me.PassiveDllTransferLimitLabel.Name = "PassiveDllTransferLimitLabel"
        Me.PassiveDllTransferLimitLabel.Size = New System.Drawing.Size(117, 12)
        Me.PassiveDllTransferLimitLabel.TabIndex = 9
        Me.PassiveDllTransferLimitLabel.Text = "転送期限（0は無期限）"
        '
        'PassiveDllTransferLimitNumericUpDown
        '
        Me.PassiveDllTransferLimitNumericUpDown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllTransferLimitNumericUpDown.Location = New System.Drawing.Point(178, 161)
        Me.PassiveDllTransferLimitNumericUpDown.Maximum = New Decimal(New Integer() {43200000, 0, 0, 0})
        Me.PassiveDllTransferLimitNumericUpDown.Name = "PassiveDllTransferLimitNumericUpDown"
        Me.PassiveDllTransferLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveDllTransferLimitNumericUpDown.TabIndex = 10
        '
        'PassiveDllTransferLimitUnitLabel
        '
        Me.PassiveDllTransferLimitUnitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllTransferLimitUnitLabel.AutoSize = True
        Me.PassiveDllTransferLimitUnitLabel.Location = New System.Drawing.Point(264, 163)
        Me.PassiveDllTransferLimitUnitLabel.Name = "PassiveDllTransferLimitUnitLabel"
        Me.PassiveDllTransferLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.PassiveDllTransferLimitUnitLabel.TabIndex = 11
        Me.PassiveDllTransferLimitUnitLabel.Text = "ms"
        '
        'PassiveDllReplyLimitLabel
        '
        Me.PassiveDllReplyLimitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllReplyLimitLabel.AutoSize = True
        Me.PassiveDllReplyLimitLabel.Location = New System.Drawing.Point(299, 163)
        Me.PassiveDllReplyLimitLabel.Name = "PassiveDllReplyLimitLabel"
        Me.PassiveDllReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.PassiveDllReplyLimitLabel.TabIndex = 12
        Me.PassiveDllReplyLimitLabel.Text = "応答受信期限"
        '
        'PassiveDllReplyLimitNumericUpDown
        '
        Me.PassiveDllReplyLimitNumericUpDown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllReplyLimitNumericUpDown.Location = New System.Drawing.Point(382, 161)
        Me.PassiveDllReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.PassiveDllReplyLimitNumericUpDown.Name = "PassiveDllReplyLimitNumericUpDown"
        Me.PassiveDllReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveDllReplyLimitNumericUpDown.TabIndex = 13
        '
        'PassiveDllReplyLimitUnitLabel
        '
        Me.PassiveDllReplyLimitUnitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveDllReplyLimitUnitLabel.AutoSize = True
        Me.PassiveDllReplyLimitUnitLabel.Location = New System.Drawing.Point(468, 163)
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
        Me.PassivePostTabPage.Size = New System.Drawing.Size(662, 182)
        Me.PassivePostTabPage.TabIndex = 5
        Me.PassivePostTabPage.Text = "POST電文受信"
        Me.PassivePostTabPage.UseVisualStyleBackColor = True
        '
        'PassivePostDataGridView
        '
        Me.PassivePostDataGridView.AllowUserToDeleteRows = False
        Me.PassivePostDataGridView.AllowUserToResizeRows = False
        Me.PassivePostDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassivePostDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassivePostDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassivePostObjCodeColumn})
        Me.PassivePostDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassivePostDataGridView.MultiSelect = False
        Me.PassivePostDataGridView.Name = "PassivePostDataGridView"
        Me.PassivePostDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassivePostDataGridView.RowTemplate.Height = 21
        Me.PassivePostDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassivePostDataGridView.Size = New System.Drawing.Size(156, 153)
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
        Me.PassivePostForceReplyNakCheckBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassivePostForceReplyNakCheckBox.AutoSize = True
        Me.PassivePostForceReplyNakCheckBox.Location = New System.Drawing.Point(3, 164)
        Me.PassivePostForceReplyNakCheckBox.Name = "PassivePostForceReplyNakCheckBox"
        Me.PassivePostForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassivePostForceReplyNakCheckBox.TabIndex = 2
        Me.PassivePostForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassivePostForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassivePostNakCauseNumberLabel
        '
        Me.PassivePostNakCauseNumberLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassivePostNakCauseNumberLabel.AutoSize = True
        Me.PassivePostNakCauseNumberLabel.Location = New System.Drawing.Point(102, 164)
        Me.PassivePostNakCauseNumberLabel.Name = "PassivePostNakCauseNumberLabel"
        Me.PassivePostNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassivePostNakCauseNumberLabel.TabIndex = 3
        Me.PassivePostNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassivePostNakCauseNumberTextBox
        '
        Me.PassivePostNakCauseNumberTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassivePostNakCauseNumberTextBox.Location = New System.Drawing.Point(184, 160)
        Me.PassivePostNakCauseNumberTextBox.Mask = "999"
        Me.PassivePostNakCauseNumberTextBox.Name = "PassivePostNakCauseNumberTextBox"
        Me.PassivePostNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassivePostNakCauseNumberTextBox.TabIndex = 4
        '
        'PassivePostNakCauseTextLabel
        '
        Me.PassivePostNakCauseTextLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassivePostNakCauseTextLabel.AutoSize = True
        Me.PassivePostNakCauseTextLabel.Location = New System.Drawing.Point(259, 164)
        Me.PassivePostNakCauseTextLabel.Name = "PassivePostNakCauseTextLabel"
        Me.PassivePostNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassivePostNakCauseTextLabel.TabIndex = 5
        Me.PassivePostNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassivePostNakCauseTextTextBox
        '
        Me.PassivePostNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassivePostNakCauseTextTextBox.Location = New System.Drawing.Point(341, 160)
        Me.PassivePostNakCauseTextTextBox.MaxLength = 50
        Me.PassivePostNakCauseTextTextBox.Name = "PassivePostNakCauseTextTextBox"
        Me.PassivePostNakCauseTextTextBox.Size = New System.Drawing.Size(313, 19)
        Me.PassivePostNakCauseTextTextBox.TabIndex = 6
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
        Me.PassiveUllTabPage.Size = New System.Drawing.Size(662, 182)
        Me.PassiveUllTabPage.TabIndex = 4
        Me.PassiveUllTabPage.Text = "受動的ULL"
        Me.PassiveUllTabPage.UseVisualStyleBackColor = True
        '
        'PassiveUllDataGridView
        '
        Me.PassiveUllDataGridView.AllowUserToDeleteRows = False
        Me.PassiveUllDataGridView.AllowUserToResizeRows = False
        Me.PassiveUllDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassiveUllDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassiveUllObjCodeColumn, Me.PassiveUllApplyFileColumn})
        Me.PassiveUllDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassiveUllDataGridView.MultiSelect = False
        Me.PassiveUllDataGridView.Name = "PassiveUllDataGridView"
        Me.PassiveUllDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassiveUllDataGridView.RowTemplate.Height = 21
        Me.PassiveUllDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassiveUllDataGridView.Size = New System.Drawing.Size(651, 120)
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
        Me.PassiveUllApplyFileColumn.HeaderText = "転送データ"
        Me.PassiveUllApplyFileColumn.Name = "PassiveUllApplyFileColumn"
        '
        'PassiveUllStartLabel
        '
        Me.PassiveUllStartLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllStartLabel.AutoSize = True
        Me.PassiveUllStartLabel.Location = New System.Drawing.Point(1, 136)
        Me.PassiveUllStartLabel.Name = "PassiveUllStartLabel"
        Me.PassiveUllStartLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveUllStartLabel.TabIndex = 2
        Me.PassiveUllStartLabel.Text = "開始時"
        '
        'PassiveUllForceReplyNakCheckBox
        '
        Me.PassiveUllForceReplyNakCheckBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllForceReplyNakCheckBox.AutoSize = True
        Me.PassiveUllForceReplyNakCheckBox.Location = New System.Drawing.Point(56, 136)
        Me.PassiveUllForceReplyNakCheckBox.Name = "PassiveUllForceReplyNakCheckBox"
        Me.PassiveUllForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassiveUllForceReplyNakCheckBox.TabIndex = 3
        Me.PassiveUllForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassiveUllForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassiveUllNakCauseNumberLabel
        '
        Me.PassiveUllNakCauseNumberLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllNakCauseNumberLabel.AutoSize = True
        Me.PassiveUllNakCauseNumberLabel.Location = New System.Drawing.Point(142, 136)
        Me.PassiveUllNakCauseNumberLabel.Name = "PassiveUllNakCauseNumberLabel"
        Me.PassiveUllNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveUllNakCauseNumberLabel.TabIndex = 4
        Me.PassiveUllNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassiveUllNakCauseNumberTextBox
        '
        Me.PassiveUllNakCauseNumberTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllNakCauseNumberTextBox.Location = New System.Drawing.Point(224, 132)
        Me.PassiveUllNakCauseNumberTextBox.Mask = "999"
        Me.PassiveUllNakCauseNumberTextBox.Name = "PassiveUllNakCauseNumberTextBox"
        Me.PassiveUllNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassiveUllNakCauseNumberTextBox.TabIndex = 5
        '
        'PassiveUllNakCauseTextLabel
        '
        Me.PassiveUllNakCauseTextLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllNakCauseTextLabel.AutoSize = True
        Me.PassiveUllNakCauseTextLabel.Location = New System.Drawing.Point(280, 136)
        Me.PassiveUllNakCauseTextLabel.Name = "PassiveUllNakCauseTextLabel"
        Me.PassiveUllNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveUllNakCauseTextLabel.TabIndex = 6
        Me.PassiveUllNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassiveUllNakCauseTextTextBox
        '
        Me.PassiveUllNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllNakCauseTextTextBox.Location = New System.Drawing.Point(362, 132)
        Me.PassiveUllNakCauseTextTextBox.MaxLength = 50
        Me.PassiveUllNakCauseTextTextBox.Name = "PassiveUllNakCauseTextTextBox"
        Me.PassiveUllNakCauseTextTextBox.Size = New System.Drawing.Size(292, 19)
        Me.PassiveUllNakCauseTextTextBox.TabIndex = 7
        '
        'PassiveUllFinishLabel
        '
        Me.PassiveUllFinishLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllFinishLabel.AutoSize = True
        Me.PassiveUllFinishLabel.Location = New System.Drawing.Point(1, 163)
        Me.PassiveUllFinishLabel.Name = "PassiveUllFinishLabel"
        Me.PassiveUllFinishLabel.Size = New System.Drawing.Size(41, 12)
        Me.PassiveUllFinishLabel.TabIndex = 8
        Me.PassiveUllFinishLabel.Text = "終了時"
        '
        'PassiveUllTransferLimitLabel
        '
        Me.PassiveUllTransferLimitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllTransferLimitLabel.AutoSize = True
        Me.PassiveUllTransferLimitLabel.Location = New System.Drawing.Point(55, 163)
        Me.PassiveUllTransferLimitLabel.Name = "PassiveUllTransferLimitLabel"
        Me.PassiveUllTransferLimitLabel.Size = New System.Drawing.Size(117, 12)
        Me.PassiveUllTransferLimitLabel.TabIndex = 9
        Me.PassiveUllTransferLimitLabel.Text = "転送期限（0は無期限）"
        '
        'PassiveUllTransferLimitNumericUpDown
        '
        Me.PassiveUllTransferLimitNumericUpDown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllTransferLimitNumericUpDown.Location = New System.Drawing.Point(178, 161)
        Me.PassiveUllTransferLimitNumericUpDown.Maximum = New Decimal(New Integer() {43200000, 0, 0, 0})
        Me.PassiveUllTransferLimitNumericUpDown.Name = "PassiveUllTransferLimitNumericUpDown"
        Me.PassiveUllTransferLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveUllTransferLimitNumericUpDown.TabIndex = 10
        '
        'PassiveUllTransferLimitUnitLabel
        '
        Me.PassiveUllTransferLimitUnitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllTransferLimitUnitLabel.AutoSize = True
        Me.PassiveUllTransferLimitUnitLabel.Location = New System.Drawing.Point(264, 163)
        Me.PassiveUllTransferLimitUnitLabel.Name = "PassiveUllTransferLimitUnitLabel"
        Me.PassiveUllTransferLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.PassiveUllTransferLimitUnitLabel.TabIndex = 11
        Me.PassiveUllTransferLimitUnitLabel.Text = "ms"
        '
        'PassiveUllReplyLimitLabel
        '
        Me.PassiveUllReplyLimitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllReplyLimitLabel.AutoSize = True
        Me.PassiveUllReplyLimitLabel.Location = New System.Drawing.Point(299, 163)
        Me.PassiveUllReplyLimitLabel.Name = "PassiveUllReplyLimitLabel"
        Me.PassiveUllReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.PassiveUllReplyLimitLabel.TabIndex = 12
        Me.PassiveUllReplyLimitLabel.Text = "応答受信期限"
        '
        'PassiveUllReplyLimitNumericUpDown
        '
        Me.PassiveUllReplyLimitNumericUpDown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllReplyLimitNumericUpDown.Location = New System.Drawing.Point(382, 161)
        Me.PassiveUllReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.PassiveUllReplyLimitNumericUpDown.Name = "PassiveUllReplyLimitNumericUpDown"
        Me.PassiveUllReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.PassiveUllReplyLimitNumericUpDown.TabIndex = 13
        '
        'PassiveUllReplyLimitUnitLabel
        '
        Me.PassiveUllReplyLimitUnitLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveUllReplyLimitUnitLabel.AutoSize = True
        Me.PassiveUllReplyLimitUnitLabel.Location = New System.Drawing.Point(468, 163)
        Me.PassiveUllReplyLimitUnitLabel.Name = "PassiveUllReplyLimitUnitLabel"
        Me.PassiveUllReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.PassiveUllReplyLimitUnitLabel.TabIndex = 14
        Me.PassiveUllReplyLimitUnitLabel.Text = "ms"
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
        Me.PassiveGetTabPage.Size = New System.Drawing.Size(662, 182)
        Me.PassiveGetTabPage.TabIndex = 3
        Me.PassiveGetTabPage.Text = "GET電文受信"
        Me.PassiveGetTabPage.UseVisualStyleBackColor = True
        '
        'PassiveGetDataGridView
        '
        Me.PassiveGetDataGridView.AllowUserToDeleteRows = False
        Me.PassiveGetDataGridView.AllowUserToResizeRows = False
        Me.PassiveGetDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.PassiveGetDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.PassiveGetObjCodeColumn, Me.PassiveGetApplyFileColumn})
        Me.PassiveGetDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.PassiveGetDataGridView.MultiSelect = False
        Me.PassiveGetDataGridView.Name = "PassiveGetDataGridView"
        Me.PassiveGetDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.PassiveGetDataGridView.RowTemplate.Height = 21
        Me.PassiveGetDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.PassiveGetDataGridView.Size = New System.Drawing.Size(651, 150)
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
        Me.PassiveGetApplyFileColumn.HeaderText = "返信データ（電文データ部）"
        Me.PassiveGetApplyFileColumn.Name = "PassiveGetApplyFileColumn"
        '
        'PassiveGetForceReplyNakCheckBox
        '
        Me.PassiveGetForceReplyNakCheckBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetForceReplyNakCheckBox.AutoSize = True
        Me.PassiveGetForceReplyNakCheckBox.Location = New System.Drawing.Point(3, 163)
        Me.PassiveGetForceReplyNakCheckBox.Name = "PassiveGetForceReplyNakCheckBox"
        Me.PassiveGetForceReplyNakCheckBox.Size = New System.Drawing.Size(80, 16)
        Me.PassiveGetForceReplyNakCheckBox.TabIndex = 2
        Me.PassiveGetForceReplyNakCheckBox.Text = "NAKを返信"
        Me.PassiveGetForceReplyNakCheckBox.UseVisualStyleBackColor = True
        '
        'PassiveGetNakCauseNumberLabel
        '
        Me.PassiveGetNakCauseNumberLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetNakCauseNumberLabel.AutoSize = True
        Me.PassiveGetNakCauseNumberLabel.Location = New System.Drawing.Point(102, 163)
        Me.PassiveGetNakCauseNumberLabel.Name = "PassiveGetNakCauseNumberLabel"
        Me.PassiveGetNakCauseNumberLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveGetNakCauseNumberLabel.TabIndex = 3
        Me.PassiveGetNakCauseNumberLabel.Text = "NAK事由番号"
        '
        'PassiveGetNakCauseNumberTextBox
        '
        Me.PassiveGetNakCauseNumberTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetNakCauseNumberTextBox.Location = New System.Drawing.Point(184, 160)
        Me.PassiveGetNakCauseNumberTextBox.Mask = "999"
        Me.PassiveGetNakCauseNumberTextBox.Name = "PassiveGetNakCauseNumberTextBox"
        Me.PassiveGetNakCauseNumberTextBox.Size = New System.Drawing.Size(50, 19)
        Me.PassiveGetNakCauseNumberTextBox.TabIndex = 4
        '
        'PassiveGetNakCauseTextLabel
        '
        Me.PassiveGetNakCauseTextLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetNakCauseTextLabel.AutoSize = True
        Me.PassiveGetNakCauseTextLabel.Location = New System.Drawing.Point(259, 163)
        Me.PassiveGetNakCauseTextLabel.Name = "PassiveGetNakCauseTextLabel"
        Me.PassiveGetNakCauseTextLabel.Size = New System.Drawing.Size(76, 12)
        Me.PassiveGetNakCauseTextLabel.TabIndex = 5
        Me.PassiveGetNakCauseTextLabel.Text = "NAK事由文言"
        '
        'PassiveGetNakCauseTextTextBox
        '
        Me.PassiveGetNakCauseTextTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PassiveGetNakCauseTextTextBox.Location = New System.Drawing.Point(341, 160)
        Me.PassiveGetNakCauseTextTextBox.MaxLength = 50
        Me.PassiveGetNakCauseTextTextBox.Name = "PassiveGetNakCauseTextTextBox"
        Me.PassiveGetNakCauseTextTextBox.Size = New System.Drawing.Size(313, 19)
        Me.PassiveGetNakCauseTextTextBox.TabIndex = 6
        '
        'ActiveUllTabPage
        '
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllFinishReplyLimitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllFinishReplyLimitNumericUpDown)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllFinishReplyLimitUnitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferNameTextBox)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferNameLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferNameSelButton)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllObjCodeLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllObjCodeTextBox)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllApplyFileTextBox)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllApplyFileLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllApplyFileSelButton)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllStartReplyLimitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllStartReplyLimitNumericUpDown)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllStartReplyLimitUnitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferLimitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferLimitNumericUpDown)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllTransferLimitUnitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecIntervalLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecIntervalNumericUpDown)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecIntervalUnitLabel)
        Me.ActiveUllTabPage.Controls.Add(Me.ActiveUllExecButton)
        Me.ActiveUllTabPage.Location = New System.Drawing.Point(4, 22)
        Me.ActiveUllTabPage.Name = "ActiveUllTabPage"
        Me.ActiveUllTabPage.Size = New System.Drawing.Size(662, 182)
        Me.ActiveUllTabPage.TabIndex = 2
        Me.ActiveUllTabPage.Text = "能動的ULL"
        Me.ActiveUllTabPage.UseVisualStyleBackColor = True
        '
        'ActiveUllFinishReplyLimitLabel
        '
        Me.ActiveUllFinishReplyLimitLabel.AutoSize = True
        Me.ActiveUllFinishReplyLimitLabel.Location = New System.Drawing.Point(24, 122)
        Me.ActiveUllFinishReplyLimitLabel.Name = "ActiveUllFinishReplyLimitLabel"
        Me.ActiveUllFinishReplyLimitLabel.Size = New System.Drawing.Size(101, 12)
        Me.ActiveUllFinishReplyLimitLabel.TabIndex = 15
        Me.ActiveUllFinishReplyLimitLabel.Text = "終了応答受信期限"
        '
        'ActiveUllFinishReplyLimitNumericUpDown
        '
        Me.ActiveUllFinishReplyLimitNumericUpDown.Location = New System.Drawing.Point(135, 120)
        Me.ActiveUllFinishReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.ActiveUllFinishReplyLimitNumericUpDown.Name = "ActiveUllFinishReplyLimitNumericUpDown"
        Me.ActiveUllFinishReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveUllFinishReplyLimitNumericUpDown.TabIndex = 16
        '
        'ActiveUllFinishReplyLimitUnitLabel
        '
        Me.ActiveUllFinishReplyLimitUnitLabel.AutoSize = True
        Me.ActiveUllFinishReplyLimitUnitLabel.Location = New System.Drawing.Point(221, 122)
        Me.ActiveUllFinishReplyLimitUnitLabel.Name = "ActiveUllFinishReplyLimitUnitLabel"
        Me.ActiveUllFinishReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveUllFinishReplyLimitUnitLabel.TabIndex = 17
        Me.ActiveUllFinishReplyLimitUnitLabel.Text = "ms"
        '
        'ActiveUllTransferNameTextBox
        '
        Me.ActiveUllTransferNameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllTransferNameTextBox.Location = New System.Drawing.Point(180, 15)
        Me.ActiveUllTransferNameTextBox.Name = "ActiveUllTransferNameTextBox"
        Me.ActiveUllTransferNameTextBox.Size = New System.Drawing.Size(413, 19)
        Me.ActiveUllTransferNameTextBox.TabIndex = 4
        '
        'ActiveUllTransferNameLabel
        '
        Me.ActiveUllTransferNameLabel.AutoSize = True
        Me.ActiveUllTransferNameLabel.Location = New System.Drawing.Point(133, 18)
        Me.ActiveUllTransferNameLabel.Name = "ActiveUllTransferNameLabel"
        Me.ActiveUllTransferNameLabel.Size = New System.Drawing.Size(41, 12)
        Me.ActiveUllTransferNameLabel.TabIndex = 3
        Me.ActiveUllTransferNameLabel.Text = "転送名"
        '
        'ActiveUllTransferNameSelButton
        '
        Me.ActiveUllTransferNameSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllTransferNameSelButton.Location = New System.Drawing.Point(599, 13)
        Me.ActiveUllTransferNameSelButton.Name = "ActiveUllTransferNameSelButton"
        Me.ActiveUllTransferNameSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ActiveUllTransferNameSelButton.TabIndex = 5
        Me.ActiveUllTransferNameSelButton.Text = "選択"
        Me.ActiveUllTransferNameSelButton.UseVisualStyleBackColor = True
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
        'ActiveUllApplyFileTextBox
        '
        Me.ActiveUllApplyFileTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllApplyFileTextBox.Location = New System.Drawing.Point(77, 44)
        Me.ActiveUllApplyFileTextBox.Name = "ActiveUllApplyFileTextBox"
        Me.ActiveUllApplyFileTextBox.Size = New System.Drawing.Size(516, 19)
        Me.ActiveUllApplyFileTextBox.TabIndex = 7
        '
        'ActiveUllApplyFileLabel
        '
        Me.ActiveUllApplyFileLabel.AutoSize = True
        Me.ActiveUllApplyFileLabel.Location = New System.Drawing.Point(12, 47)
        Me.ActiveUllApplyFileLabel.Name = "ActiveUllApplyFileLabel"
        Me.ActiveUllApplyFileLabel.Size = New System.Drawing.Size(57, 12)
        Me.ActiveUllApplyFileLabel.TabIndex = 6
        Me.ActiveUllApplyFileLabel.Text = "転送データ"
        '
        'ActiveUllApplyFileSelButton
        '
        Me.ActiveUllApplyFileSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllApplyFileSelButton.Location = New System.Drawing.Point(599, 42)
        Me.ActiveUllApplyFileSelButton.Name = "ActiveUllApplyFileSelButton"
        Me.ActiveUllApplyFileSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ActiveUllApplyFileSelButton.TabIndex = 8
        Me.ActiveUllApplyFileSelButton.Text = "選択"
        Me.ActiveUllApplyFileSelButton.UseVisualStyleBackColor = True
        '
        'ActiveUllStartReplyLimitLabel
        '
        Me.ActiveUllStartReplyLimitLabel.AutoSize = True
        Me.ActiveUllStartReplyLimitLabel.Location = New System.Drawing.Point(24, 85)
        Me.ActiveUllStartReplyLimitLabel.Name = "ActiveUllStartReplyLimitLabel"
        Me.ActiveUllStartReplyLimitLabel.Size = New System.Drawing.Size(101, 12)
        Me.ActiveUllStartReplyLimitLabel.TabIndex = 9
        Me.ActiveUllStartReplyLimitLabel.Text = "開始応答受信期限"
        '
        'ActiveUllStartReplyLimitNumericUpDown
        '
        Me.ActiveUllStartReplyLimitNumericUpDown.Location = New System.Drawing.Point(135, 83)
        Me.ActiveUllStartReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.ActiveUllStartReplyLimitNumericUpDown.Name = "ActiveUllStartReplyLimitNumericUpDown"
        Me.ActiveUllStartReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveUllStartReplyLimitNumericUpDown.TabIndex = 10
        '
        'ActiveUllStartReplyLimitUnitLabel
        '
        Me.ActiveUllStartReplyLimitUnitLabel.AutoSize = True
        Me.ActiveUllStartReplyLimitUnitLabel.Location = New System.Drawing.Point(221, 85)
        Me.ActiveUllStartReplyLimitUnitLabel.Name = "ActiveUllStartReplyLimitUnitLabel"
        Me.ActiveUllStartReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveUllStartReplyLimitUnitLabel.TabIndex = 11
        Me.ActiveUllStartReplyLimitUnitLabel.Text = "ms"
        '
        'ActiveUllTransferLimitLabel
        '
        Me.ActiveUllTransferLimitLabel.AutoSize = True
        Me.ActiveUllTransferLimitLabel.Location = New System.Drawing.Point(264, 102)
        Me.ActiveUllTransferLimitLabel.Name = "ActiveUllTransferLimitLabel"
        Me.ActiveUllTransferLimitLabel.Size = New System.Drawing.Size(117, 12)
        Me.ActiveUllTransferLimitLabel.TabIndex = 12
        Me.ActiveUllTransferLimitLabel.Text = "転送期限（0は無期限）"
        '
        'ActiveUllTransferLimitNumericUpDown
        '
        Me.ActiveUllTransferLimitNumericUpDown.Location = New System.Drawing.Point(387, 100)
        Me.ActiveUllTransferLimitNumericUpDown.Maximum = New Decimal(New Integer() {43200000, 0, 0, 0})
        Me.ActiveUllTransferLimitNumericUpDown.Name = "ActiveUllTransferLimitNumericUpDown"
        Me.ActiveUllTransferLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveUllTransferLimitNumericUpDown.TabIndex = 13
        '
        'ActiveUllTransferLimitUnitLabel
        '
        Me.ActiveUllTransferLimitUnitLabel.AutoSize = True
        Me.ActiveUllTransferLimitUnitLabel.Location = New System.Drawing.Point(473, 102)
        Me.ActiveUllTransferLimitUnitLabel.Name = "ActiveUllTransferLimitUnitLabel"
        Me.ActiveUllTransferLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveUllTransferLimitUnitLabel.TabIndex = 14
        Me.ActiveUllTransferLimitUnitLabel.Text = "ms"
        '
        'ActiveUllExecIntervalLabel
        '
        Me.ActiveUllExecIntervalLabel.AutoSize = True
        Me.ActiveUllExecIntervalLabel.Location = New System.Drawing.Point(24, 160)
        Me.ActiveUllExecIntervalLabel.Name = "ActiveUllExecIntervalLabel"
        Me.ActiveUllExecIntervalLabel.Size = New System.Drawing.Size(105, 12)
        Me.ActiveUllExecIntervalLabel.TabIndex = 18
        Me.ActiveUllExecIntervalLabel.Text = "実行間隔（0は単発）"
        '
        'ActiveUllExecIntervalNumericUpDown
        '
        Me.ActiveUllExecIntervalNumericUpDown.Location = New System.Drawing.Point(135, 158)
        Me.ActiveUllExecIntervalNumericUpDown.Maximum = New Decimal(New Integer() {86400000, 0, 0, 0})
        Me.ActiveUllExecIntervalNumericUpDown.Name = "ActiveUllExecIntervalNumericUpDown"
        Me.ActiveUllExecIntervalNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveUllExecIntervalNumericUpDown.TabIndex = 19
        '
        'ActiveUllExecIntervalUnitLabel
        '
        Me.ActiveUllExecIntervalUnitLabel.AutoSize = True
        Me.ActiveUllExecIntervalUnitLabel.Location = New System.Drawing.Point(221, 160)
        Me.ActiveUllExecIntervalUnitLabel.Name = "ActiveUllExecIntervalUnitLabel"
        Me.ActiveUllExecIntervalUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveUllExecIntervalUnitLabel.TabIndex = 20
        Me.ActiveUllExecIntervalUnitLabel.Text = "ms"
        '
        'ActiveUllExecButton
        '
        Me.ActiveUllExecButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveUllExecButton.Location = New System.Drawing.Point(563, 152)
        Me.ActiveUllExecButton.Name = "ActiveUllExecButton"
        Me.ActiveUllExecButton.Size = New System.Drawing.Size(86, 28)
        Me.ActiveUllExecButton.TabIndex = 21
        Me.ActiveUllExecButton.Text = "実行"
        Me.ActiveUllExecButton.UseVisualStyleBackColor = True
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
        Me.ActiveOneTabPage.Size = New System.Drawing.Size(662, 182)
        Me.ActiveOneTabPage.TabIndex = 1
        Me.ActiveOneTabPage.Text = "電文送信"
        Me.ActiveOneTabPage.UseVisualStyleBackColor = True
        '
        'ActiveOneApplyFileLabel
        '
        Me.ActiveOneApplyFileLabel.AutoSize = True
        Me.ActiveOneApplyFileLabel.Location = New System.Drawing.Point(12, 18)
        Me.ActiveOneApplyFileLabel.Name = "ActiveOneApplyFileLabel"
        Me.ActiveOneApplyFileLabel.Size = New System.Drawing.Size(53, 12)
        Me.ActiveOneApplyFileLabel.TabIndex = 1
        Me.ActiveOneApplyFileLabel.Text = "送信電文"
        '
        'ActiveOneApplyFileTextBox
        '
        Me.ActiveOneApplyFileTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneApplyFileTextBox.Location = New System.Drawing.Point(71, 15)
        Me.ActiveOneApplyFileTextBox.Name = "ActiveOneApplyFileTextBox"
        Me.ActiveOneApplyFileTextBox.Size = New System.Drawing.Size(522, 19)
        Me.ActiveOneApplyFileTextBox.TabIndex = 2
        '
        'ActiveOneApplyFileSelButton
        '
        Me.ActiveOneApplyFileSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneApplyFileSelButton.Location = New System.Drawing.Point(599, 13)
        Me.ActiveOneApplyFileSelButton.Name = "ActiveOneApplyFileSelButton"
        Me.ActiveOneApplyFileSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ActiveOneApplyFileSelButton.TabIndex = 3
        Me.ActiveOneApplyFileSelButton.Text = "選択"
        Me.ActiveOneApplyFileSelButton.UseVisualStyleBackColor = True
        '
        'ActiveOneReplyLimitLabel
        '
        Me.ActiveOneReplyLimitLabel.AutoSize = True
        Me.ActiveOneReplyLimitLabel.Location = New System.Drawing.Point(12, 53)
        Me.ActiveOneReplyLimitLabel.Name = "ActiveOneReplyLimitLabel"
        Me.ActiveOneReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.ActiveOneReplyLimitLabel.TabIndex = 4
        Me.ActiveOneReplyLimitLabel.Text = "応答受信期限"
        '
        'ActiveOneReplyLimitNumericUpDown
        '
        Me.ActiveOneReplyLimitNumericUpDown.Location = New System.Drawing.Point(123, 51)
        Me.ActiveOneReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.ActiveOneReplyLimitNumericUpDown.Name = "ActiveOneReplyLimitNumericUpDown"
        Me.ActiveOneReplyLimitNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveOneReplyLimitNumericUpDown.TabIndex = 5
        '
        'ActiveOneReplyLimitUnitLabel
        '
        Me.ActiveOneReplyLimitUnitLabel.AutoSize = True
        Me.ActiveOneReplyLimitUnitLabel.Location = New System.Drawing.Point(212, 53)
        Me.ActiveOneReplyLimitUnitLabel.Name = "ActiveOneReplyLimitUnitLabel"
        Me.ActiveOneReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveOneReplyLimitUnitLabel.TabIndex = 6
        Me.ActiveOneReplyLimitUnitLabel.Text = "ms"
        '
        'ActiveOneExecIntervalLabel
        '
        Me.ActiveOneExecIntervalLabel.AutoSize = True
        Me.ActiveOneExecIntervalLabel.Location = New System.Drawing.Point(12, 88)
        Me.ActiveOneExecIntervalLabel.Name = "ActiveOneExecIntervalLabel"
        Me.ActiveOneExecIntervalLabel.Size = New System.Drawing.Size(105, 12)
        Me.ActiveOneExecIntervalLabel.TabIndex = 7
        Me.ActiveOneExecIntervalLabel.Text = "実行間隔（0は単発）"
        '
        'ActiveOneExecIntervalNumericUpDown
        '
        Me.ActiveOneExecIntervalNumericUpDown.Location = New System.Drawing.Point(123, 86)
        Me.ActiveOneExecIntervalNumericUpDown.Maximum = New Decimal(New Integer() {86400000, 0, 0, 0})
        Me.ActiveOneExecIntervalNumericUpDown.Name = "ActiveOneExecIntervalNumericUpDown"
        Me.ActiveOneExecIntervalNumericUpDown.Size = New System.Drawing.Size(80, 19)
        Me.ActiveOneExecIntervalNumericUpDown.TabIndex = 8
        '
        'ActiveOneExecIntervalUnitLabel
        '
        Me.ActiveOneExecIntervalUnitLabel.AutoSize = True
        Me.ActiveOneExecIntervalUnitLabel.Location = New System.Drawing.Point(212, 88)
        Me.ActiveOneExecIntervalUnitLabel.Name = "ActiveOneExecIntervalUnitLabel"
        Me.ActiveOneExecIntervalUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveOneExecIntervalUnitLabel.TabIndex = 9
        Me.ActiveOneExecIntervalUnitLabel.Text = "ms"
        '
        'ActiveOneExecButton
        '
        Me.ActiveOneExecButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneExecButton.Location = New System.Drawing.Point(563, 80)
        Me.ActiveOneExecButton.Name = "ActiveOneExecButton"
        Me.ActiveOneExecButton.Size = New System.Drawing.Size(86, 28)
        Me.ActiveOneExecButton.TabIndex = 10
        Me.ActiveOneExecButton.Text = "実行"
        Me.ActiveOneExecButton.UseVisualStyleBackColor = True
        '
        'ConnectionTabPage
        '
        Me.ConnectionTabPage.Controls.Add(Me.DisconButton)
        Me.ConnectionTabPage.Controls.Add(Me.ConButton)
        Me.ConnectionTabPage.Controls.Add(Me.TimeDataGetButton)
        Me.ConnectionTabPage.Controls.Add(Me.ComSartButton)
        Me.ConnectionTabPage.Controls.Add(Me.AutomaticComStartCheckBox)
        Me.ConnectionTabPage.Location = New System.Drawing.Point(4, 22)
        Me.ConnectionTabPage.Name = "ConnectionTabPage"
        Me.ConnectionTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.ConnectionTabPage.Size = New System.Drawing.Size(662, 182)
        Me.ConnectionTabPage.TabIndex = 0
        Me.ConnectionTabPage.Text = "接続切断"
        Me.ConnectionTabPage.UseVisualStyleBackColor = True
        '
        'DisconButton
        '
        Me.DisconButton.Location = New System.Drawing.Point(171, 19)
        Me.DisconButton.Name = "DisconButton"
        Me.DisconButton.Size = New System.Drawing.Size(145, 28)
        Me.DisconButton.TabIndex = 2
        Me.DisconButton.Text = "切断"
        Me.DisconButton.UseVisualStyleBackColor = True
        '
        'TimeDataGetButton
        '
        Me.TimeDataGetButton.Location = New System.Drawing.Point(20, 106)
        Me.TimeDataGetButton.Name = "TimeDataGetButton"
        Me.TimeDataGetButton.Size = New System.Drawing.Size(145, 28)
        Me.TimeDataGetButton.TabIndex = 5
        Me.TimeDataGetButton.Text = "整時データ要求 実行"
        Me.TimeDataGetButton.UseVisualStyleBackColor = True
        '
        'ComSartButton
        '
        Me.ComSartButton.Location = New System.Drawing.Point(20, 62)
        Me.ComSartButton.Name = "ComSartButton"
        Me.ComSartButton.Size = New System.Drawing.Size(145, 28)
        Me.ComSartButton.TabIndex = 4
        Me.ComSartButton.Text = "接続初期化要求 実行"
        Me.ComSartButton.UseVisualStyleBackColor = True
        '
        'AutomaticComStartCheckBox
        '
        Me.AutomaticComStartCheckBox.AutoSize = True
        Me.AutomaticComStartCheckBox.Location = New System.Drawing.Point(335, 26)
        Me.AutomaticComStartCheckBox.Name = "AutomaticComStartCheckBox"
        Me.AutomaticComStartCheckBox.Size = New System.Drawing.Size(197, 16)
        Me.AutomaticComStartCheckBox.TabIndex = 3
        Me.AutomaticComStartCheckBox.Text = "接続後に開始シーケンスを自動実行"
        Me.AutomaticComStartCheckBox.UseVisualStyleBackColor = True
        '
        'CapRcvFilesCheckBox
        '
        Me.CapRcvFilesCheckBox.AutoSize = True
        Me.CapRcvFilesCheckBox.Location = New System.Drawing.Point(16, 86)
        Me.CapRcvFilesCheckBox.Name = "CapRcvFilesCheckBox"
        Me.CapRcvFilesCheckBox.Size = New System.Drawing.Size(134, 16)
        Me.CapRcvFilesCheckBox.TabIndex = 4
        Me.CapRcvFilesCheckBox.Text = "受信ファイルを保存する"
        Me.CapRcvFilesCheckBox.UseVisualStyleBackColor = True
        '
        'CapSndFilesCheckBox
        '
        Me.CapSndFilesCheckBox.AutoSize = True
        Me.CapSndFilesCheckBox.Location = New System.Drawing.Point(16, 64)
        Me.CapSndFilesCheckBox.Name = "CapSndFilesCheckBox"
        Me.CapSndFilesCheckBox.Size = New System.Drawing.Size(134, 16)
        Me.CapSndFilesCheckBox.TabIndex = 3
        Me.CapSndFilesCheckBox.Text = "送信ファイルを保存する"
        Me.CapSndFilesCheckBox.UseVisualStyleBackColor = True
        '
        'CapRcvTelegsCheckBox
        '
        Me.CapRcvTelegsCheckBox.AutoSize = True
        Me.CapRcvTelegsCheckBox.Location = New System.Drawing.Point(16, 42)
        Me.CapRcvTelegsCheckBox.Name = "CapRcvTelegsCheckBox"
        Me.CapRcvTelegsCheckBox.Size = New System.Drawing.Size(124, 16)
        Me.CapRcvTelegsCheckBox.TabIndex = 2
        Me.CapRcvTelegsCheckBox.Text = "受信電文を保存する"
        Me.CapRcvTelegsCheckBox.UseVisualStyleBackColor = True
        '
        'CapSndTelegsCheckBox
        '
        Me.CapSndTelegsCheckBox.AutoSize = True
        Me.CapSndTelegsCheckBox.Location = New System.Drawing.Point(16, 20)
        Me.CapSndTelegsCheckBox.Name = "CapSndTelegsCheckBox"
        Me.CapSndTelegsCheckBox.Size = New System.Drawing.Size(124, 16)
        Me.CapSndTelegsCheckBox.TabIndex = 1
        Me.CapSndTelegsCheckBox.Text = "送信電文を保存する"
        Me.CapSndTelegsCheckBox.UseVisualStyleBackColor = True
        '
        'SeqTabControl
        '
        Me.SeqTabControl.Controls.Add(Me.ConnectionTabPage)
        Me.SeqTabControl.Controls.Add(Me.ActiveOneTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassiveGetTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassivePostTabPage)
        Me.SeqTabControl.Controls.Add(Me.ActiveUllTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassiveUllTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassiveDllTabPage)
        Me.SeqTabControl.Controls.Add(Me.ScenarioTabPage)
        Me.SeqTabControl.Controls.Add(Me.CapTabPage)
        Me.SeqTabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SeqTabControl.Location = New System.Drawing.Point(0, 0)
        Me.SeqTabControl.Name = "SeqTabControl"
        Me.SeqTabControl.SelectedIndex = 0
        Me.SeqTabControl.Size = New System.Drawing.Size(670, 208)
        Me.SeqTabControl.TabIndex = 0
        '
        'CapTabPage
        '
        Me.CapTabPage.Controls.Add(Me.CapRcvFilesCheckBox)
        Me.CapTabPage.Controls.Add(Me.CapSndTelegsCheckBox)
        Me.CapTabPage.Controls.Add(Me.CapRcvTelegsCheckBox)
        Me.CapTabPage.Controls.Add(Me.CapSndFilesCheckBox)
        Me.CapTabPage.Location = New System.Drawing.Point(4, 22)
        Me.CapTabPage.Name = "CapTabPage"
        Me.CapTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.CapTabPage.Size = New System.Drawing.Size(662, 182)
        Me.CapTabPage.TabIndex = 8
        Me.CapTabPage.Text = "キャプチャ"
        Me.CapTabPage.UseVisualStyleBackColor = True
        '
        'ClientDataGridView
        '
        Me.ClientDataGridView.AllowUserToAddRows = False
        Me.ClientDataGridView.AllowUserToDeleteRows = False
        Me.ClientDataGridView.AllowUserToOrderColumns = True
        Me.ClientDataGridView.AllowUserToResizeRows = False
        Me.ClientDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ClientDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.ClientDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ClientDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.ClientDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.ClientDataGridView.Location = New System.Drawing.Point(0, 23)
        Me.ClientDataGridView.Name = "ClientDataGridView"
        Me.ClientDataGridView.ReadOnly = True
        Me.ClientDataGridView.RowHeadersVisible = False
        Me.ClientDataGridView.RowTemplate.Height = 21
        Me.ClientDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.ClientDataGridView.Size = New System.Drawing.Size(332, 518)
        Me.ClientDataGridView.StandardTab = True
        Me.ClientDataGridView.TabIndex = 1
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.ClientDataGridView)
        Me.SplitContainer1.Panel1.Controls.Add(Me.SymbolizeCheckBox)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(1008, 541)
        Me.SplitContainer1.SplitterDistance = 332
        Me.SplitContainer1.TabIndex = 0
        '
        'SymbolizeCheckBox
        '
        Me.SymbolizeCheckBox.AutoSize = True
        Me.SymbolizeCheckBox.Location = New System.Drawing.Point(3, 3)
        Me.SymbolizeCheckBox.Name = "SymbolizeCheckBox"
        Me.SymbolizeCheckBox.Size = New System.Drawing.Size(72, 16)
        Me.SymbolizeCheckBox.TabIndex = 0
        Me.SymbolizeCheckBox.Text = "駅名表示"
        Me.SymbolizeCheckBox.UseVisualStyleBackColor = True
        '
        'SplitContainer2
        '
        Me.SplitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.SeqTabControl)
        Me.SplitContainer2.Panel1MinSize = 210
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.LogDispFilterEditButton)
        Me.SplitContainer2.Panel2.Controls.Add(Me.LogDispFilter)
        Me.SplitContainer2.Panel2.Controls.Add(Me.LogDispFilterLabel)
        Me.SplitContainer2.Panel2.Controls.Add(Me.LogDispGrid)
        Me.SplitContainer2.Panel2.Controls.Add(Me.LogDispCheckBox)
        Me.SplitContainer2.Panel2.Controls.Add(Me.LogDispClearButton)
        Me.SplitContainer2.Size = New System.Drawing.Size(672, 541)
        Me.SplitContainer2.SplitterDistance = 210
        Me.SplitContainer2.TabIndex = 0
        '
        'LogDispFilterEditButton
        '
        Me.LogDispFilterEditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LogDispFilterEditButton.Location = New System.Drawing.Point(614, 5)
        Me.LogDispFilterEditButton.Name = "LogDispFilterEditButton"
        Me.LogDispFilterEditButton.Size = New System.Drawing.Size(53, 23)
        Me.LogDispFilterEditButton.TabIndex = 4
        Me.LogDispFilterEditButton.Text = "編集"
        Me.LogDispFilterEditButton.UseVisualStyleBackColor = True
        '
        'LogDispFilter
        '
        Me.LogDispFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LogDispFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LogDispFilter.Location = New System.Drawing.Point(217, 7)
        Me.LogDispFilter.Name = "LogDispFilter"
        Me.LogDispFilter.ReadOnly = True
        Me.LogDispFilter.Size = New System.Drawing.Size(391, 19)
        Me.LogDispFilter.TabIndex = 3
        '
        'LogDispFilterLabel
        '
        Me.LogDispFilterLabel.AutoSize = True
        Me.LogDispFilterLabel.Location = New System.Drawing.Point(173, 10)
        Me.LogDispFilterLabel.Name = "LogDispFilterLabel"
        Me.LogDispFilterLabel.Size = New System.Drawing.Size(38, 12)
        Me.LogDispFilterLabel.TabIndex = 2
        Me.LogDispFilterLabel.Text = "フィルタ"
        '
        'LogDispGrid
        '
        Me.LogDispGrid.AllowUserToAddRows = False
        Me.LogDispGrid.AllowUserToDeleteRows = False
        Me.LogDispGrid.AllowUserToResizeRows = False
        Me.LogDispGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LogDispGrid.BackgroundColor = System.Drawing.SystemColors.Window
        Me.LogDispGrid.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.LogDispGrid.CausesValidation = False
        Me.LogDispGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal
        Me.LogDispGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.LogDispGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LogDispGrid.DefaultCellStyle = DataGridViewCellStyle2
        Me.LogDispGrid.Location = New System.Drawing.Point(0, 32)
        Me.LogDispGrid.Name = "LogDispGrid"
        Me.LogDispGrid.ReadOnly = True
        Me.LogDispGrid.RowHeadersVisible = False
        Me.LogDispGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader
        Me.LogDispGrid.RowTemplate.Height = 21
        Me.LogDispGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.LogDispGrid.ShowCellErrors = False
        Me.LogDispGrid.ShowEditingIcon = False
        Me.LogDispGrid.ShowRowErrors = False
        Me.LogDispGrid.Size = New System.Drawing.Size(670, 293)
        Me.LogDispGrid.StandardTab = True
        Me.LogDispGrid.TabIndex = 5
        '
        'UsageToolTip
        '
        Me.UsageToolTip.AutoPopDelay = 30000
        Me.UsageToolTip.InitialDelay = 500
        Me.UsageToolTip.IsBalloon = True
        Me.UsageToolTip.ReshowDelay = 100
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1008, 541)
        Me.Controls.Add(Me.SplitContainer1)
        Me.MinimumSize = New System.Drawing.Size(920, 300)
        Me.Name = "MainForm"
        Me.Text = "多重駅務機器"
        Me.PassiveGetRowHeaderMenu.ResumeLayout(False)
        Me.PassiveGetApplyFileMenu.ResumeLayout(False)
        Me.PassiveUllRowHeaderMenu.ResumeLayout(False)
        Me.PassiveUllApplyFileMenu.ResumeLayout(False)
        Me.PassivePostRowHeaderMenu.ResumeLayout(False)
        Me.PassiveDllRowHeaderMenu.ResumeLayout(False)
        Me.ScenarioTabPage.ResumeLayout(False)
        Me.ScenarioTabPage.PerformLayout()
        Me.PassiveDllTabPage.ResumeLayout(False)
        Me.PassiveDllTabPage.PerformLayout()
        CType(Me.PassiveDllDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveDllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveDllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassivePostTabPage.ResumeLayout(False)
        Me.PassivePostTabPage.PerformLayout()
        CType(Me.PassivePostDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassiveUllTabPage.ResumeLayout(False)
        Me.PassiveUllTabPage.PerformLayout()
        CType(Me.PassiveUllDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PassiveUllReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PassiveGetTabPage.ResumeLayout(False)
        Me.PassiveGetTabPage.PerformLayout()
        CType(Me.PassiveGetDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ActiveUllTabPage.ResumeLayout(False)
        Me.ActiveUllTabPage.PerformLayout()
        CType(Me.ActiveUllFinishReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveUllStartReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveUllTransferLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveUllExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ActiveOneTabPage.ResumeLayout(False)
        Me.ActiveOneTabPage.PerformLayout()
        CType(Me.ActiveOneReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveOneExecIntervalNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ConnectionTabPage.ResumeLayout(False)
        Me.ConnectionTabPage.PerformLayout()
        Me.SeqTabControl.ResumeLayout(False)
        Me.CapTabPage.ResumeLayout(False)
        Me.CapTabPage.PerformLayout()
        CType(Me.ClientDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        Me.SplitContainer2.ResumeLayout(False)
        CType(Me.LogDispGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FileSelDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents StatusPollTimer As System.Windows.Forms.Timer
    Friend WithEvents ConButton As System.Windows.Forms.Button
    Friend WithEvents LogDispClearButton As System.Windows.Forms.Button
    Friend WithEvents ActiveOneExecTimer As System.Windows.Forms.Timer
    Friend WithEvents ActiveUllExecTimer As System.Windows.Forms.Timer
    Friend WithEvents PassiveGetRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveGetDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveGetApplyFileMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveGetSelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveUllRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveUllDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveUllApplyFileMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveUllSelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassivePostRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassivePostDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PassiveDllRowHeaderMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PassiveDllDelMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LogDispCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ScenarioTabPage As System.Windows.Forms.TabPage
    Friend WithEvents ScenarioFileLabel As System.Windows.Forms.Label
    Friend WithEvents ScenarioFileTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ScenarioFileSelButton As System.Windows.Forms.Button
    Friend WithEvents PassiveDllTabPage As System.Windows.Forms.TabPage
    Friend WithEvents PassiveDllFinishDetailLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllSimulateStoringCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassiveDllDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PassiveDllStartLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllForceReplyNakCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassiveDllNakCauseNumberLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllNakCauseNumberTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveDllNakCauseTextLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllNakCauseTextTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassiveDllResultantFlagOfFullTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassiveDllFinishLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllTransferLimitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllTransferLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents PassiveDllTransferLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllReplyLimitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllReplyLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents PassiveDllReplyLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveDllResultantVersionOfSlot1Label As System.Windows.Forms.Label
    Friend WithEvents PassiveDllResultantVersionOfSlot1TextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveDllResultantVersionOfSlot2Label As System.Windows.Forms.Label
    Friend WithEvents PassiveDllResultantVersionOfSlot2TextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveDllResultantFlagOfFullLabel As System.Windows.Forms.Label
    Friend WithEvents PassivePostTabPage As System.Windows.Forms.TabPage
    Friend WithEvents PassivePostDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PassivePostForceReplyNakCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassivePostNakCauseNumberLabel As System.Windows.Forms.Label
    Friend WithEvents PassivePostNakCauseNumberTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassivePostNakCauseTextLabel As System.Windows.Forms.Label
    Friend WithEvents PassivePostNakCauseTextTextBox As System.Windows.Forms.TextBox
    Friend WithEvents PassiveUllTabPage As System.Windows.Forms.TabPage
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
    Friend WithEvents PassiveGetTabPage As System.Windows.Forms.TabPage
    Friend WithEvents PassiveGetDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PassiveGetForceReplyNakCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PassiveGetNakCauseNumberLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveGetNakCauseNumberTextBox As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PassiveGetNakCauseTextLabel As System.Windows.Forms.Label
    Friend WithEvents PassiveGetNakCauseTextTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ActiveUllTabPage As System.Windows.Forms.TabPage
    Friend WithEvents ActiveUllObjCodeLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllObjCodeTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ActiveUllApplyFileTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ActiveUllApplyFileLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllApplyFileSelButton As System.Windows.Forms.Button
    Friend WithEvents ActiveUllStartReplyLimitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllStartReplyLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveUllStartReplyLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllTransferLimitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllTransferLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveUllTransferLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllExecIntervalLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllExecIntervalNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveUllExecIntervalUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllExecButton As System.Windows.Forms.Button
    Friend WithEvents ActiveOneTabPage As System.Windows.Forms.TabPage
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
    Friend WithEvents ConnectionTabPage As System.Windows.Forms.TabPage
    Friend WithEvents CapRcvFilesCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapSndFilesCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapRcvTelegsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapSndTelegsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents TimeDataGetButton As System.Windows.Forms.Button
    Friend WithEvents ComSartButton As System.Windows.Forms.Button
    Friend WithEvents AutomaticComStartCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents SeqTabControl As System.Windows.Forms.TabControl
    Friend WithEvents DisconButton As System.Windows.Forms.Button
    Friend WithEvents ClientDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents CapTabPage As System.Windows.Forms.TabPage
    Friend WithEvents UsageToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents PassiveDllObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassivePostObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ScenarioStartDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents ScenarioStartDateTimeLabel As System.Windows.Forms.Label
    Friend WithEvents ScenarioStartDateTimeCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ScenarioStopButton As System.Windows.Forms.Button
    Friend WithEvents ScenarioStartButton As System.Windows.Forms.Button
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents ActiveUllTransferNameTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ActiveUllTransferNameLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllTransferNameSelButton As System.Windows.Forms.Button
    Friend WithEvents PassiveUllObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveUllApplyFileColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveGetObjCodeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PassiveGetApplyFileColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ActiveUllFinishReplyLimitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveUllFinishReplyLimitNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveUllFinishReplyLimitUnitLabel As System.Windows.Forms.Label
    Friend WithEvents SymbolizeCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents LogDispGrid As System.Windows.Forms.DataGridView
    Friend WithEvents LogDispFilterEditButton As System.Windows.Forms.Button
    Friend WithEvents LogDispFilter As System.Windows.Forms.TextBox
    Friend WithEvents LogDispFilterLabel As System.Windows.Forms.Label

End Class
