<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KadoDataForm
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
        Me.SourceMachineIdLabel = New System.Windows.Forms.Label()
        Me.SourceMachineIdTextBox = New System.Windows.Forms.TextBox()
        Me.FileReadButton1 = New System.Windows.Forms.Button()
        Me.FileRewriteButton1 = New System.Windows.Forms.Button()
        Me.FileAppendButton1 = New System.Windows.Forms.Button()
        Me.MonitorMachineIdLabel = New System.Windows.Forms.Label()
        Me.MonitorMachineIdTextBox = New System.Windows.Forms.TextBox()
        Me.KadoDataOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.KadoDataRewriteFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.ManFileUpdateButton = New System.Windows.Forms.Button()
        Me.BaseHeaderSetButton = New System.Windows.Forms.Button()
        Me.AllHeadersSetButton = New System.Windows.Forms.Button()
        Me.KadoDataAppendFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.HotToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.SearchNextButton = New System.Windows.Forms.Button()
        Me.SearchPrevButton = New System.Windows.Forms.Button()
        Me.SearchBox = New System.Windows.Forms.ComboBox()
        Me.KeyFieldsAutoAdjustCheckBox = New System.Windows.Forms.CheckBox()
        Me.SummariesSetButton = New System.Windows.Forms.Button()
        Me.FileReadButton2 = New System.Windows.Forms.Button()
        Me.FileRewriteButton2 = New System.Windows.Forms.Button()
        Me.FileAppendButton2 = New System.Windows.Forms.Button()
        Me.KadoDataGridView = New JR.ExOpmg.DummyKanshiban.XlsDataGridView()
        CType(Me.KadoDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SourceMachineIdLabel
        '
        Me.SourceMachineIdLabel.AutoSize = True
        Me.SourceMachineIdLabel.Location = New System.Drawing.Point(34, 8)
        Me.SourceMachineIdLabel.Name = "SourceMachineIdLabel"
        Me.SourceMachineIdLabel.Size = New System.Drawing.Size(40, 12)
        Me.SourceMachineIdLabel.TabIndex = 0
        Me.SourceMachineIdLabel.Text = "機器ID"
        '
        'SourceMachineIdTextBox
        '
        Me.SourceMachineIdTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.SourceMachineIdTextBox.Location = New System.Drawing.Point(80, 5)
        Me.SourceMachineIdTextBox.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.SourceMachineIdTextBox.Name = "SourceMachineIdTextBox"
        Me.SourceMachineIdTextBox.ReadOnly = True
        Me.SourceMachineIdTextBox.Size = New System.Drawing.Size(112, 19)
        Me.SourceMachineIdTextBox.TabIndex = 0
        Me.SourceMachineIdTextBox.TabStop = False
        '
        'FileReadButton1
        '
        Me.FileReadButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileReadButton1.Location = New System.Drawing.Point(209, 3)
        Me.FileReadButton1.Name = "FileReadButton1"
        Me.FileReadButton1.Size = New System.Drawing.Size(75, 23)
        Me.FileReadButton1.TabIndex = 0
        Me.FileReadButton1.Text = "読込(稼)"
        Me.FileReadButton1.UseVisualStyleBackColor = True
        '
        'FileRewriteButton1
        '
        Me.FileRewriteButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileRewriteButton1.Font = New System.Drawing.Font("MS UI Gothic", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FileRewriteButton1.Location = New System.Drawing.Point(459, 29)
        Me.FileRewriteButton1.Name = "FileRewriteButton1"
        Me.FileRewriteButton1.Size = New System.Drawing.Size(75, 23)
        Me.FileRewriteButton1.TabIndex = 9
        Me.FileRewriteButton1.TabStop = False
        Me.FileRewriteButton1.Text = "上書保存(稼)"
        Me.FileRewriteButton1.UseVisualStyleBackColor = True
        '
        'FileAppendButton1
        '
        Me.FileAppendButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileAppendButton1.Font = New System.Drawing.Font("MS UI Gothic", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FileAppendButton1.Location = New System.Drawing.Point(627, 29)
        Me.FileAppendButton1.Name = "FileAppendButton1"
        Me.FileAppendButton1.Size = New System.Drawing.Size(75, 23)
        Me.FileAppendButton1.TabIndex = 11
        Me.FileAppendButton1.TabStop = False
        Me.FileAppendButton1.Text = "追記保存(稼)"
        Me.FileAppendButton1.UseVisualStyleBackColor = True
        '
        'MonitorMachineIdLabel
        '
        Me.MonitorMachineIdLabel.AutoSize = True
        Me.MonitorMachineIdLabel.Location = New System.Drawing.Point(10, 34)
        Me.MonitorMachineIdLabel.Name = "MonitorMachineIdLabel"
        Me.MonitorMachineIdLabel.Size = New System.Drawing.Size(64, 12)
        Me.MonitorMachineIdLabel.TabIndex = 0
        Me.MonitorMachineIdLabel.Text = "監視機器ID"
        '
        'MonitorMachineIdTextBox
        '
        Me.MonitorMachineIdTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.MonitorMachineIdTextBox.Location = New System.Drawing.Point(80, 31)
        Me.MonitorMachineIdTextBox.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.MonitorMachineIdTextBox.Name = "MonitorMachineIdTextBox"
        Me.MonitorMachineIdTextBox.ReadOnly = True
        Me.MonitorMachineIdTextBox.Size = New System.Drawing.Size(112, 19)
        Me.MonitorMachineIdTextBox.TabIndex = 0
        Me.MonitorMachineIdTextBox.TabStop = False
        '
        'KadoDataOpenFileDialog
        '
        Me.KadoDataOpenFileDialog.Filter = "DATファイル|*.dat|BINファイル|*.bin|すべてのファイル|*.*"
        '
        'KadoDataRewriteFileDialog
        '
        Me.KadoDataRewriteFileDialog.Filter = "DATファイル|*.dat|BINファイル|*.bin|すべてのファイル|*.*"
        Me.KadoDataRewriteFileDialog.OverwritePrompt = False
        '
        'ManFileUpdateButton
        '
        Me.ManFileUpdateButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ManFileUpdateButton.Location = New System.Drawing.Point(796, 29)
        Me.ManFileUpdateButton.Name = "ManFileUpdateButton"
        Me.ManFileUpdateButton.Size = New System.Drawing.Size(115, 23)
        Me.ManFileUpdateButton.TabIndex = 14
        Me.ManFileUpdateButton.Text = "機器に反映"
        Me.ManFileUpdateButton.UseVisualStyleBackColor = True
        '
        'BaseHeaderSetButton
        '
        Me.BaseHeaderSetButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BaseHeaderSetButton.Location = New System.Drawing.Point(370, 3)
        Me.BaseHeaderSetButton.Name = "BaseHeaderSetButton"
        Me.BaseHeaderSetButton.Size = New System.Drawing.Size(75, 23)
        Me.BaseHeaderSetButton.TabIndex = 2
        Me.BaseHeaderSetButton.Text = "ヘッダ設定"
        Me.BaseHeaderSetButton.UseVisualStyleBackColor = True
        '
        'AllHeadersSetButton
        '
        Me.AllHeadersSetButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AllHeadersSetButton.Location = New System.Drawing.Point(451, 3)
        Me.AllHeadersSetButton.Name = "AllHeadersSetButton"
        Me.AllHeadersSetButton.Size = New System.Drawing.Size(75, 23)
        Me.AllHeadersSetButton.TabIndex = 3
        Me.AllHeadersSetButton.Text = "前半設定"
        Me.AllHeadersSetButton.UseVisualStyleBackColor = True
        '
        'KadoDataAppendFileDialog
        '
        Me.KadoDataAppendFileDialog.Filter = "DATファイル|*.dat|BINファイル|*.bin|すべてのファイル|*.*"
        Me.KadoDataAppendFileDialog.OverwritePrompt = False
        '
        'SearchNextButton
        '
        Me.SearchNextButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SearchNextButton.Location = New System.Drawing.Point(412, 29)
        Me.SearchNextButton.Name = "SearchNextButton"
        Me.SearchNextButton.Size = New System.Drawing.Size(33, 23)
        Me.SearchNextButton.TabIndex = 7
        Me.SearchNextButton.Text = "→"
        Me.HotToolTip.SetToolTip(Me.SearchNextButton, "F3")
        Me.SearchNextButton.UseVisualStyleBackColor = True
        '
        'SearchPrevButton
        '
        Me.SearchPrevButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SearchPrevButton.Location = New System.Drawing.Point(209, 29)
        Me.SearchPrevButton.Name = "SearchPrevButton"
        Me.SearchPrevButton.Size = New System.Drawing.Size(33, 23)
        Me.SearchPrevButton.TabIndex = 5
        Me.SearchPrevButton.Text = "←"
        Me.HotToolTip.SetToolTip(Me.SearchPrevButton, "Shift + F3")
        Me.SearchPrevButton.UseVisualStyleBackColor = True
        '
        'SearchBox
        '
        Me.SearchBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SearchBox.FormattingEnabled = True
        Me.SearchBox.Location = New System.Drawing.Point(248, 31)
        Me.SearchBox.Name = "SearchBox"
        Me.SearchBox.Size = New System.Drawing.Size(158, 20)
        Me.SearchBox.TabIndex = 6
        Me.HotToolTip.SetToolTip(Me.SearchBox, "Ctrl + F")
        '
        'KeyFieldsAutoAdjustCheckBox
        '
        Me.KeyFieldsAutoAdjustCheckBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.KeyFieldsAutoAdjustCheckBox.AutoSize = True
        Me.KeyFieldsAutoAdjustCheckBox.Checked = True
        Me.KeyFieldsAutoAdjustCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.KeyFieldsAutoAdjustCheckBox.Location = New System.Drawing.Point(644, 7)
        Me.KeyFieldsAutoAdjustCheckBox.Name = "KeyFieldsAutoAdjustCheckBox"
        Me.KeyFieldsAutoAdjustCheckBox.Size = New System.Drawing.Size(267, 16)
        Me.KeyFieldsAutoAdjustCheckBox.TabIndex = 13
        Me.KeyFieldsAutoAdjustCheckBox.Text = "機器反映前に処理日時とシーケンスNoを自動設定"
        Me.KeyFieldsAutoAdjustCheckBox.UseVisualStyleBackColor = True
        '
        'SummariesSetButton
        '
        Me.SummariesSetButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SummariesSetButton.Location = New System.Drawing.Point(540, 3)
        Me.SummariesSetButton.Name = "SummariesSetButton"
        Me.SummariesSetButton.Size = New System.Drawing.Size(75, 23)
        Me.SummariesSetButton.TabIndex = 4
        Me.SummariesSetButton.Text = "合計値設定"
        Me.SummariesSetButton.UseVisualStyleBackColor = True
        Me.SummariesSetButton.Visible = False
        '
        'FileReadButton2
        '
        Me.FileReadButton2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileReadButton2.Location = New System.Drawing.Point(290, 3)
        Me.FileReadButton2.Name = "FileReadButton2"
        Me.FileReadButton2.Size = New System.Drawing.Size(75, 23)
        Me.FileReadButton2.TabIndex = 1
        Me.FileReadButton2.Text = "読込(保)"
        Me.FileReadButton2.UseVisualStyleBackColor = True
        '
        'FileRewriteButton2
        '
        Me.FileRewriteButton2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileRewriteButton2.Font = New System.Drawing.Font("MS UI Gothic", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FileRewriteButton2.Location = New System.Drawing.Point(540, 29)
        Me.FileRewriteButton2.Name = "FileRewriteButton2"
        Me.FileRewriteButton2.Size = New System.Drawing.Size(75, 23)
        Me.FileRewriteButton2.TabIndex = 10
        Me.FileRewriteButton2.TabStop = False
        Me.FileRewriteButton2.Text = "上書保存(保)"
        Me.FileRewriteButton2.UseVisualStyleBackColor = True
        '
        'FileAppendButton2
        '
        Me.FileAppendButton2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileAppendButton2.Font = New System.Drawing.Font("MS UI Gothic", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FileAppendButton2.Location = New System.Drawing.Point(708, 29)
        Me.FileAppendButton2.Name = "FileAppendButton2"
        Me.FileAppendButton2.Size = New System.Drawing.Size(75, 23)
        Me.FileAppendButton2.TabIndex = 12
        Me.FileAppendButton2.TabStop = False
        Me.FileAppendButton2.Text = "追記保存(保)"
        Me.FileAppendButton2.UseVisualStyleBackColor = True
        '
        'KadoDataGridView
        '
        Me.KadoDataGridView.AllowUserToAddRows = False
        Me.KadoDataGridView.AllowUserToDeleteRows = False
        Me.KadoDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.KadoDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.KadoDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.KadoDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.KadoDataGridView.Location = New System.Drawing.Point(4, 54)
        Me.KadoDataGridView.Name = "KadoDataGridView"
        Me.KadoDataGridView.RowHeadersVisible = False
        Me.KadoDataGridView.RowTemplate.Height = 21
        Me.KadoDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.KadoDataGridView.Size = New System.Drawing.Size(907, 674)
        Me.KadoDataGridView.StandardTab = True
        Me.KadoDataGridView.TabIndex = 8
        '
        'KadoDataForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(914, 731)
        Me.Controls.Add(Me.FileAppendButton2)
        Me.Controls.Add(Me.FileRewriteButton2)
        Me.Controls.Add(Me.FileReadButton2)
        Me.Controls.Add(Me.SummariesSetButton)
        Me.Controls.Add(Me.KeyFieldsAutoAdjustCheckBox)
        Me.Controls.Add(Me.SearchNextButton)
        Me.Controls.Add(Me.SearchPrevButton)
        Me.Controls.Add(Me.SearchBox)
        Me.Controls.Add(Me.AllHeadersSetButton)
        Me.Controls.Add(Me.BaseHeaderSetButton)
        Me.Controls.Add(Me.SourceMachineIdTextBox)
        Me.Controls.Add(Me.SourceMachineIdLabel)
        Me.Controls.Add(Me.MonitorMachineIdTextBox)
        Me.Controls.Add(Me.MonitorMachineIdLabel)
        Me.Controls.Add(Me.FileAppendButton1)
        Me.Controls.Add(Me.FileRewriteButton1)
        Me.Controls.Add(Me.FileReadButton1)
        Me.Controls.Add(Me.ManFileUpdateButton)
        Me.Controls.Add(Me.KadoDataGridView)
        Me.MinimumSize = New System.Drawing.Size(930, 38)
        Me.Name = "KadoDataForm"
        Me.Text = "稼動保守データ編集"
        CType(Me.KadoDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SourceMachineIdLabel As System.Windows.Forms.Label
    Friend WithEvents SourceMachineIdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents KadoDataGridView As JR.ExOpmg.DummyKanshiban.XlsDataGridView
    Friend WithEvents FileReadButton1 As System.Windows.Forms.Button
    Friend WithEvents FileRewriteButton1 As System.Windows.Forms.Button
    Friend WithEvents FileAppendButton1 As System.Windows.Forms.Button
    Friend WithEvents MonitorMachineIdLabel As System.Windows.Forms.Label
    Friend WithEvents MonitorMachineIdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents KadoDataOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents KadoDataRewriteFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents ManFileUpdateButton As System.Windows.Forms.Button
    Friend WithEvents BaseHeaderSetButton As System.Windows.Forms.Button
    Friend WithEvents AllHeadersSetButton As System.Windows.Forms.Button
    Friend WithEvents KadoDataAppendFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents HotToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents SearchNextButton As System.Windows.Forms.Button
    Friend WithEvents SearchPrevButton As System.Windows.Forms.Button
    Friend WithEvents SearchBox As System.Windows.Forms.ComboBox
    Friend WithEvents KeyFieldsAutoAdjustCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents SummariesSetButton As System.Windows.Forms.Button
    Friend WithEvents FileReadButton2 As System.Windows.Forms.Button
    Friend WithEvents FileRewriteButton2 As System.Windows.Forms.Button
    Friend WithEvents FileAppendButton2 As System.Windows.Forms.Button
End Class
