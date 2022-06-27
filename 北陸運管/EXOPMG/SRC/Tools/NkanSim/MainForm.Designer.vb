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
        Me.ComSartButton = New System.Windows.Forms.Button()
        Me.InquiryButton = New System.Windows.Forms.Button()
        Me.AutomaticComStartCheckBox = New System.Windows.Forms.CheckBox()
        Me.ActiveOneTabPage = New System.Windows.Forms.TabPage()
        Me.ActiveOneApplyFileLabel = New System.Windows.Forms.Label()
        Me.ActiveOneApplyFileTextBox = New System.Windows.Forms.TextBox()
        Me.ActiveOneApplyFileSelButton = New System.Windows.Forms.Button()
        Me.ActiveOneReplyLimitUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveOneReplyLimitNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveOneReplyLimitLabel = New System.Windows.Forms.Label()
        Me.ActiveOneExecRateLabel = New System.Windows.Forms.Label()
        Me.ActiveOneExecRateNumericUpDown = New System.Windows.Forms.NumericUpDown()
        Me.ActiveOneExecRateUnitLabel = New System.Windows.Forms.Label()
        Me.ActiveOneExecButton = New System.Windows.Forms.Button()
        Me.PassivePostTabPage = New System.Windows.Forms.TabPage()
        Me.ActiveOneExecTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SeqTabControl.SuspendLayout()
        Me.BasicTabPage.SuspendLayout()
        Me.ActiveOneTabPage.SuspendLayout()
        CType(Me.ActiveOneReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ActiveOneExecRateNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LineStatusPollTimer
        '
        Me.LineStatusPollTimer.Interval = 500
        '
        'ConButton
        '
        Me.ConButton.Location = New System.Drawing.Point(17, 183)
        Me.ConButton.Name = "ConButton"
        Me.ConButton.Size = New System.Drawing.Size(94, 28)
        Me.ConButton.TabIndex = 0
        Me.ConButton.Text = "接続"
        Me.ConButton.UseVisualStyleBackColor = True
        '
        'LoggerTextBox
        '
        Me.LoggerTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LoggerTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.LoggerTextBox.Location = New System.Drawing.Point(12, 217)
        Me.LoggerTextBox.Multiline = True
        Me.LoggerTextBox.Name = "LoggerTextBox"
        Me.LoggerTextBox.ReadOnly = True
        Me.LoggerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.LoggerTextBox.Size = New System.Drawing.Size(701, 242)
        Me.LoggerTextBox.TabIndex = 1
        Me.LoggerTextBox.WordWrap = False
        '
        'LoggerClearButton
        '
        Me.LoggerClearButton.Location = New System.Drawing.Point(117, 183)
        Me.LoggerClearButton.Name = "LoggerClearButton"
        Me.LoggerClearButton.Size = New System.Drawing.Size(106, 28)
        Me.LoggerClearButton.TabIndex = 2
        Me.LoggerClearButton.Text = "ログ表示をクリア"
        Me.LoggerClearButton.UseVisualStyleBackColor = True
        '
        'SeqTabControl
        '
        Me.SeqTabControl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SeqTabControl.Controls.Add(Me.BasicTabPage)
        Me.SeqTabControl.Controls.Add(Me.ActiveOneTabPage)
        Me.SeqTabControl.Controls.Add(Me.PassivePostTabPage)
        Me.SeqTabControl.Location = New System.Drawing.Point(13, 1)
        Me.SeqTabControl.Name = "SeqTabControl"
        Me.SeqTabControl.SelectedIndex = 0
        Me.SeqTabControl.Size = New System.Drawing.Size(700, 176)
        Me.SeqTabControl.TabIndex = 0
        '
        'BasicTabPage
        '
        Me.BasicTabPage.Controls.Add(Me.CapRcvFilesCheckBox)
        Me.BasicTabPage.Controls.Add(Me.CapSndFilesCheckBox)
        Me.BasicTabPage.Controls.Add(Me.CapRcvTelegsCheckBox)
        Me.BasicTabPage.Controls.Add(Me.CapSndTelegsCheckBox)
        Me.BasicTabPage.Controls.Add(Me.ComSartButton)
        Me.BasicTabPage.Controls.Add(Me.InquiryButton)
        Me.BasicTabPage.Controls.Add(Me.AutomaticComStartCheckBox)
        Me.BasicTabPage.Location = New System.Drawing.Point(4, 22)
        Me.BasicTabPage.Name = "BasicTabPage"
        Me.BasicTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.BasicTabPage.Size = New System.Drawing.Size(692, 150)
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
        'ComSartButton
        '
        Me.ComSartButton.Location = New System.Drawing.Point(20, 73)
        Me.ComSartButton.Name = "ComSartButton"
        Me.ComSartButton.Size = New System.Drawing.Size(145, 28)
        Me.ComSartButton.TabIndex = 6
        Me.ComSartButton.Text = "開局シーケンス 実行"
        Me.ComSartButton.UseVisualStyleBackColor = True
        '
        'InquiryButton
        '
        Me.InquiryButton.Location = New System.Drawing.Point(171, 73)
        Me.InquiryButton.Name = "InquiryButton"
        Me.InquiryButton.Size = New System.Drawing.Size(145, 28)
        Me.InquiryButton.TabIndex = 7
        Me.InquiryButton.Text = "要求シーケンス 実行"
        Me.InquiryButton.UseVisualStyleBackColor = True
        '
        'AutomaticComStartCheckBox
        '
        Me.AutomaticComStartCheckBox.AutoSize = True
        Me.AutomaticComStartCheckBox.Location = New System.Drawing.Point(20, 19)
        Me.AutomaticComStartCheckBox.Name = "AutomaticComStartCheckBox"
        Me.AutomaticComStartCheckBox.Size = New System.Drawing.Size(197, 16)
        Me.AutomaticComStartCheckBox.TabIndex = 1
        Me.AutomaticComStartCheckBox.Text = "接続後に開局シーケンスを自動実行"
        Me.AutomaticComStartCheckBox.UseVisualStyleBackColor = True
        '
        'ActiveOneTabPage
        '
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneApplyFileLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneApplyFileTextBox)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneApplyFileSelButton)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneReplyLimitUnitLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneReplyLimitNumericUpDown)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneReplyLimitLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecRateLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecRateNumericUpDown)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecRateUnitLabel)
        Me.ActiveOneTabPage.Controls.Add(Me.ActiveOneExecButton)
        Me.ActiveOneTabPage.Location = New System.Drawing.Point(4, 22)
        Me.ActiveOneTabPage.Name = "ActiveOneTabPage"
        Me.ActiveOneTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.ActiveOneTabPage.Size = New System.Drawing.Size(692, 150)
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
        Me.ActiveOneApplyFileTextBox.Size = New System.Drawing.Size(487, 19)
        Me.ActiveOneApplyFileTextBox.TabIndex = 2
        '
        'ActiveOneApplyFileSelButton
        '
        Me.ActiveOneApplyFileSelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneApplyFileSelButton.Location = New System.Drawing.Point(626, 13)
        Me.ActiveOneApplyFileSelButton.Name = "ActiveOneApplyFileSelButton"
        Me.ActiveOneApplyFileSelButton.Size = New System.Drawing.Size(50, 23)
        Me.ActiveOneApplyFileSelButton.TabIndex = 3
        Me.ActiveOneApplyFileSelButton.Text = "選択"
        Me.ActiveOneApplyFileSelButton.UseVisualStyleBackColor = True
        '
        'ActiveOneReplyLimitUnitLabel
        '
        Me.ActiveOneReplyLimitUnitLabel.AutoSize = True
        Me.ActiveOneReplyLimitUnitLabel.Location = New System.Drawing.Point(295, 51)
        Me.ActiveOneReplyLimitUnitLabel.Name = "ActiveOneReplyLimitUnitLabel"
        Me.ActiveOneReplyLimitUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveOneReplyLimitUnitLabel.TabIndex = 6
        Me.ActiveOneReplyLimitUnitLabel.Text = "ms"
        '
        'ActiveOneReplyLimitNumericUpDown
        '
        Me.ActiveOneReplyLimitNumericUpDown.Location = New System.Drawing.Point(133, 49)
        Me.ActiveOneReplyLimitNumericUpDown.Maximum = New Decimal(New Integer() {600000, 0, 0, 0})
        Me.ActiveOneReplyLimitNumericUpDown.Name = "ActiveOneReplyLimitNumericUpDown"
        Me.ActiveOneReplyLimitNumericUpDown.Size = New System.Drawing.Size(156, 19)
        Me.ActiveOneReplyLimitNumericUpDown.TabIndex = 5
        Me.ActiveOneReplyLimitNumericUpDown.Value = New Decimal(New Integer() {60000, 0, 0, 0})
        '
        'ActiveOneReplyLimitLabel
        '
        Me.ActiveOneReplyLimitLabel.AutoSize = True
        Me.ActiveOneReplyLimitLabel.Location = New System.Drawing.Point(38, 51)
        Me.ActiveOneReplyLimitLabel.Name = "ActiveOneReplyLimitLabel"
        Me.ActiveOneReplyLimitLabel.Size = New System.Drawing.Size(77, 12)
        Me.ActiveOneReplyLimitLabel.TabIndex = 4
        Me.ActiveOneReplyLimitLabel.Text = "応答受信期限"
        '
        'ActiveOneExecRateLabel
        '
        Me.ActiveOneExecRateLabel.AutoSize = True
        Me.ActiveOneExecRateLabel.Location = New System.Drawing.Point(15, 87)
        Me.ActiveOneExecRateLabel.Name = "ActiveOneExecRateLabel"
        Me.ActiveOneExecRateLabel.Size = New System.Drawing.Size(105, 12)
        Me.ActiveOneExecRateLabel.TabIndex = 7
        Me.ActiveOneExecRateLabel.Text = "実行周期（0は単発）"
        '
        'ActiveOneExecRateNumericUpDown
        '
        Me.ActiveOneExecRateNumericUpDown.Location = New System.Drawing.Point(133, 83)
        Me.ActiveOneExecRateNumericUpDown.Maximum = New Decimal(New Integer() {86400000, 0, 0, 0})
        Me.ActiveOneExecRateNumericUpDown.Name = "ActiveOneExecRateNumericUpDown"
        Me.ActiveOneExecRateNumericUpDown.Size = New System.Drawing.Size(156, 19)
        Me.ActiveOneExecRateNumericUpDown.TabIndex = 8
        '
        'ActiveOneExecRateUnitLabel
        '
        Me.ActiveOneExecRateUnitLabel.AutoSize = True
        Me.ActiveOneExecRateUnitLabel.Location = New System.Drawing.Point(295, 85)
        Me.ActiveOneExecRateUnitLabel.Name = "ActiveOneExecRateUnitLabel"
        Me.ActiveOneExecRateUnitLabel.Size = New System.Drawing.Size(20, 12)
        Me.ActiveOneExecRateUnitLabel.TabIndex = 9
        Me.ActiveOneExecRateUnitLabel.Text = "ms"
        '
        'ActiveOneExecButton
        '
        Me.ActiveOneExecButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ActiveOneExecButton.Location = New System.Drawing.Point(590, 77)
        Me.ActiveOneExecButton.Name = "ActiveOneExecButton"
        Me.ActiveOneExecButton.Size = New System.Drawing.Size(86, 28)
        Me.ActiveOneExecButton.TabIndex = 10
        Me.ActiveOneExecButton.Text = "実行"
        Me.ActiveOneExecButton.UseVisualStyleBackColor = True
        '
        'PassivePostTabPage
        '
        Me.PassivePostTabPage.Location = New System.Drawing.Point(4, 22)
        Me.PassivePostTabPage.Name = "PassivePostTabPage"
        Me.PassivePostTabPage.Size = New System.Drawing.Size(692, 150)
        Me.PassivePostTabPage.TabIndex = 4
        Me.PassivePostTabPage.Text = "POST電文受信"
        Me.PassivePostTabPage.UseVisualStyleBackColor = True
        '
        'ActiveOneExecTimer
        '
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(725, 464)
        Me.Controls.Add(Me.SeqTabControl)
        Me.Controls.Add(Me.LoggerClearButton)
        Me.Controls.Add(Me.LoggerTextBox)
        Me.Controls.Add(Me.ConButton)
        Me.Name = "MainForm"
        Me.Text = "Ｎ間"
        Me.SeqTabControl.ResumeLayout(False)
        Me.BasicTabPage.ResumeLayout(False)
        Me.BasicTabPage.PerformLayout()
        Me.ActiveOneTabPage.ResumeLayout(False)
        Me.ActiveOneTabPage.PerformLayout()
        CType(Me.ActiveOneReplyLimitNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ActiveOneExecRateNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents PassivePostTabPage As System.Windows.Forms.TabPage
    Friend WithEvents ComSartButton As System.Windows.Forms.Button
    Friend WithEvents InquiryButton As System.Windows.Forms.Button
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
    Friend WithEvents ActiveOneExecRateLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveOneExecRateNumericUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents ActiveOneExecRateUnitLabel As System.Windows.Forms.Label
    Friend WithEvents ActiveOneExecButton As System.Windows.Forms.Button
    Friend WithEvents ActiveOneExecTimer As System.Windows.Forms.Timer

End Class
