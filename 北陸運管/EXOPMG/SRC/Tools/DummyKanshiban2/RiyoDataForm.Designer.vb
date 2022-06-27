<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RiyoDataForm
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
        Me.TermMachineIdLabel = New System.Windows.Forms.Label()
        Me.TermMachineIdTextBox = New System.Windows.Forms.TextBox()
        Me.FileReadButton = New System.Windows.Forms.Button()
        Me.FileRewriteButton = New System.Windows.Forms.Button()
        Me.FileAppendButton = New System.Windows.Forms.Button()
        Me.MonitorMachineIdLabel = New System.Windows.Forms.Label()
        Me.MonitorMachineIdTextBox = New System.Windows.Forms.TextBox()
        Me.RiyoDataOpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.RiyoDataRewriteFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.StoreButton = New System.Windows.Forms.Button()
        Me.SendButton = New System.Windows.Forms.Button()
        Me.BaseHeaderSetButton = New System.Windows.Forms.Button()
        Me.EntDateReplaceButton = New System.Windows.Forms.Button()
        Me.OrgStaReplaceButton = New System.Windows.Forms.Button()
        Me.EntStaReplaceButton = New System.Windows.Forms.Button()
        Me.MinDateReplaceButton = New System.Windows.Forms.Button()
        Me.MaxDateReplaceButton = New System.Windows.Forms.Button()
        Me.DstStaReplaceButton = New System.Windows.Forms.Button()
        Me.RiyoDataAppendFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.SearchBox = New System.Windows.Forms.ComboBox()
        Me.SearchPrevButton = New System.Windows.Forms.Button()
        Me.SearchNextButton = New System.Windows.Forms.Button()
        Me.HotToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.RiyoDataGridView = New JR.ExOpmg.DummyKanshiban2.XlsDataGridView()
        CType(Me.RiyoDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TermMachineIdLabel
        '
        Me.TermMachineIdLabel.AutoSize = True
        Me.TermMachineIdLabel.Location = New System.Drawing.Point(34, 8)
        Me.TermMachineIdLabel.Name = "TermMachineIdLabel"
        Me.TermMachineIdLabel.Size = New System.Drawing.Size(40, 12)
        Me.TermMachineIdLabel.TabIndex = 0
        Me.TermMachineIdLabel.Text = "機器ID"
        '
        'TermMachineIdTextBox
        '
        Me.TermMachineIdTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.TermMachineIdTextBox.Location = New System.Drawing.Point(80, 5)
        Me.TermMachineIdTextBox.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.TermMachineIdTextBox.Name = "TermMachineIdTextBox"
        Me.TermMachineIdTextBox.ReadOnly = True
        Me.TermMachineIdTextBox.Size = New System.Drawing.Size(112, 19)
        Me.TermMachineIdTextBox.TabIndex = 0
        Me.TermMachineIdTextBox.TabStop = False
        '
        'FileReadButton
        '
        Me.FileReadButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileReadButton.Location = New System.Drawing.Point(342, 3)
        Me.FileReadButton.Name = "FileReadButton"
        Me.FileReadButton.Size = New System.Drawing.Size(75, 23)
        Me.FileReadButton.TabIndex = 0
        Me.FileReadButton.Text = "読込"
        Me.FileReadButton.UseVisualStyleBackColor = True
        '
        'FileRewriteButton
        '
        Me.FileRewriteButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileRewriteButton.Location = New System.Drawing.Point(604, 29)
        Me.FileRewriteButton.Name = "FileRewriteButton"
        Me.FileRewriteButton.Size = New System.Drawing.Size(75, 23)
        Me.FileRewriteButton.TabIndex = 12
        Me.FileRewriteButton.TabStop = False
        Me.FileRewriteButton.Text = "上書保存"
        Me.FileRewriteButton.UseVisualStyleBackColor = True
        '
        'FileAppendButton
        '
        Me.FileAppendButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FileAppendButton.Location = New System.Drawing.Point(685, 29)
        Me.FileAppendButton.Name = "FileAppendButton"
        Me.FileAppendButton.Size = New System.Drawing.Size(75, 23)
        Me.FileAppendButton.TabIndex = 13
        Me.FileAppendButton.TabStop = False
        Me.FileAppendButton.Text = "追記保存"
        Me.FileAppendButton.UseVisualStyleBackColor = True
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
        'RiyoDataOpenFileDialog
        '
        Me.RiyoDataOpenFileDialog.Filter = "DATファイル|*.dat|BINファイル|*.bin|すべてのファイル|*.*"
        '
        'RiyoDataRewriteFileDialog
        '
        Me.RiyoDataRewriteFileDialog.Filter = "DATファイル|*.dat|BINファイル|*.bin|すべてのファイル|*.*"
        Me.RiyoDataRewriteFileDialog.OverwritePrompt = False
        '
        'StoreButton
        '
        Me.StoreButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.StoreButton.Location = New System.Drawing.Point(785, 29)
        Me.StoreButton.Name = "StoreButton"
        Me.StoreButton.Size = New System.Drawing.Size(115, 23)
        Me.StoreButton.TabIndex = 14
        Me.StoreButton.Text = "監視盤に蓄積"
        Me.StoreButton.UseVisualStyleBackColor = True
        '
        'SendButton
        '
        Me.SendButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SendButton.Location = New System.Drawing.Point(907, 29)
        Me.SendButton.Name = "SendButton"
        Me.SendButton.Size = New System.Drawing.Size(115, 23)
        Me.SendButton.TabIndex = 15
        Me.SendButton.Text = "監視盤から全送出"
        Me.SendButton.UseVisualStyleBackColor = True
        '
        'BaseHeaderSetButton
        '
        Me.BaseHeaderSetButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BaseHeaderSetButton.Location = New System.Drawing.Point(423, 3)
        Me.BaseHeaderSetButton.Name = "BaseHeaderSetButton"
        Me.BaseHeaderSetButton.Size = New System.Drawing.Size(75, 23)
        Me.BaseHeaderSetButton.TabIndex = 1
        Me.BaseHeaderSetButton.Text = "ヘッダ設定"
        Me.BaseHeaderSetButton.UseVisualStyleBackColor = True
        '
        'EntDateReplaceButton
        '
        Me.EntDateReplaceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.EntDateReplaceButton.Location = New System.Drawing.Point(685, 3)
        Me.EntDateReplaceButton.Name = "EntDateReplaceButton"
        Me.EntDateReplaceButton.Size = New System.Drawing.Size(75, 23)
        Me.EntDateReplaceButton.TabIndex = 4
        Me.EntDateReplaceButton.Text = "入場日置換"
        Me.EntDateReplaceButton.UseVisualStyleBackColor = True
        '
        'OrgStaReplaceButton
        '
        Me.OrgStaReplaceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OrgStaReplaceButton.Location = New System.Drawing.Point(785, 3)
        Me.OrgStaReplaceButton.Name = "OrgStaReplaceButton"
        Me.OrgStaReplaceButton.Size = New System.Drawing.Size(75, 23)
        Me.OrgStaReplaceButton.TabIndex = 5
        Me.OrgStaReplaceButton.Text = "発駅置換"
        Me.OrgStaReplaceButton.UseVisualStyleBackColor = True
        '
        'EntStaReplaceButton
        '
        Me.EntStaReplaceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.EntStaReplaceButton.Location = New System.Drawing.Point(947, 3)
        Me.EntStaReplaceButton.Name = "EntStaReplaceButton"
        Me.EntStaReplaceButton.Size = New System.Drawing.Size(75, 23)
        Me.EntStaReplaceButton.TabIndex = 7
        Me.EntStaReplaceButton.Text = "入場駅置換"
        Me.EntStaReplaceButton.UseVisualStyleBackColor = True
        '
        'MinDateReplaceButton
        '
        Me.MinDateReplaceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MinDateReplaceButton.Location = New System.Drawing.Point(523, 3)
        Me.MinDateReplaceButton.Name = "MinDateReplaceButton"
        Me.MinDateReplaceButton.Size = New System.Drawing.Size(75, 23)
        Me.MinDateReplaceButton.TabIndex = 2
        Me.MinDateReplaceButton.Text = "開始日置換"
        Me.MinDateReplaceButton.UseVisualStyleBackColor = True
        '
        'MaxDateReplaceButton
        '
        Me.MaxDateReplaceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MaxDateReplaceButton.Location = New System.Drawing.Point(604, 3)
        Me.MaxDateReplaceButton.Name = "MaxDateReplaceButton"
        Me.MaxDateReplaceButton.Size = New System.Drawing.Size(75, 23)
        Me.MaxDateReplaceButton.TabIndex = 3
        Me.MaxDateReplaceButton.Text = "終了日置換"
        Me.MaxDateReplaceButton.UseVisualStyleBackColor = True
        '
        'DstStaReplaceButton
        '
        Me.DstStaReplaceButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DstStaReplaceButton.Location = New System.Drawing.Point(866, 3)
        Me.DstStaReplaceButton.Name = "DstStaReplaceButton"
        Me.DstStaReplaceButton.Size = New System.Drawing.Size(75, 23)
        Me.DstStaReplaceButton.TabIndex = 6
        Me.DstStaReplaceButton.Text = "着駅置換"
        Me.DstStaReplaceButton.UseVisualStyleBackColor = True
        '
        'RiyoDataAppendFileDialog
        '
        Me.RiyoDataAppendFileDialog.Filter = "DATファイル|*.dat|BINファイル|*.bin|すべてのファイル|*.*"
        Me.RiyoDataAppendFileDialog.OverwritePrompt = False
        '
        'SearchBox
        '
        Me.SearchBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SearchBox.FormattingEnabled = True
        Me.SearchBox.Location = New System.Drawing.Point(382, 31)
        Me.SearchBox.Name = "SearchBox"
        Me.SearchBox.Size = New System.Drawing.Size(164, 20)
        Me.SearchBox.TabIndex = 9
        Me.HotToolTip.SetToolTip(Me.SearchBox, "Ctrl + F")
        '
        'SearchPrevButton
        '
        Me.SearchPrevButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SearchPrevButton.Location = New System.Drawing.Point(343, 29)
        Me.SearchPrevButton.Name = "SearchPrevButton"
        Me.SearchPrevButton.Size = New System.Drawing.Size(33, 23)
        Me.SearchPrevButton.TabIndex = 8
        Me.SearchPrevButton.Text = "←"
        Me.HotToolTip.SetToolTip(Me.SearchPrevButton, "Shift + F3")
        Me.SearchPrevButton.UseVisualStyleBackColor = True
        '
        'SearchNextButton
        '
        Me.SearchNextButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SearchNextButton.Location = New System.Drawing.Point(552, 29)
        Me.SearchNextButton.Name = "SearchNextButton"
        Me.SearchNextButton.Size = New System.Drawing.Size(33, 23)
        Me.SearchNextButton.TabIndex = 10
        Me.SearchNextButton.Text = "→"
        Me.HotToolTip.SetToolTip(Me.SearchNextButton, "F3")
        Me.SearchNextButton.UseVisualStyleBackColor = True
        '
        'RiyoDataGridView
        '
        Me.RiyoDataGridView.AllowUserToAddRows = False
        Me.RiyoDataGridView.AllowUserToDeleteRows = False
        Me.RiyoDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RiyoDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.RiyoDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.RiyoDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.RiyoDataGridView.Location = New System.Drawing.Point(4, 54)
        Me.RiyoDataGridView.Name = "RiyoDataGridView"
        Me.RiyoDataGridView.RowHeadersVisible = False
        Me.RiyoDataGridView.RowTemplate.Height = 21
        Me.RiyoDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.RiyoDataGridView.Size = New System.Drawing.Size(1018, 674)
        Me.RiyoDataGridView.StandardTab = True
        Me.RiyoDataGridView.TabIndex = 11
        '
        'RiyoDataForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1025, 731)
        Me.Controls.Add(Me.SearchNextButton)
        Me.Controls.Add(Me.SearchPrevButton)
        Me.Controls.Add(Me.SearchBox)
        Me.Controls.Add(Me.DstStaReplaceButton)
        Me.Controls.Add(Me.MaxDateReplaceButton)
        Me.Controls.Add(Me.MinDateReplaceButton)
        Me.Controls.Add(Me.EntStaReplaceButton)
        Me.Controls.Add(Me.OrgStaReplaceButton)
        Me.Controls.Add(Me.EntDateReplaceButton)
        Me.Controls.Add(Me.BaseHeaderSetButton)
        Me.Controls.Add(Me.TermMachineIdTextBox)
        Me.Controls.Add(Me.TermMachineIdLabel)
        Me.Controls.Add(Me.MonitorMachineIdTextBox)
        Me.Controls.Add(Me.MonitorMachineIdLabel)
        Me.Controls.Add(Me.SendButton)
        Me.Controls.Add(Me.FileAppendButton)
        Me.Controls.Add(Me.FileRewriteButton)
        Me.Controls.Add(Me.FileReadButton)
        Me.Controls.Add(Me.StoreButton)
        Me.Controls.Add(Me.RiyoDataGridView)
        Me.MinimumSize = New System.Drawing.Size(910, 38)
        Me.Name = "RiyoDataForm"
        Me.Text = "利用データ編集"
        CType(Me.RiyoDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TermMachineIdLabel As System.Windows.Forms.Label
    Friend WithEvents TermMachineIdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents RiyoDataGridView As JR.ExOpmg.DummyKanshiban2.XlsDataGridView
    Friend WithEvents FileReadButton As System.Windows.Forms.Button
    Friend WithEvents FileRewriteButton As System.Windows.Forms.Button
    Friend WithEvents FileAppendButton As System.Windows.Forms.Button
    Friend WithEvents MonitorMachineIdLabel As System.Windows.Forms.Label
    Friend WithEvents MonitorMachineIdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents RiyoDataOpenFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents RiyoDataRewriteFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents StoreButton As System.Windows.Forms.Button
    Friend WithEvents SendButton As System.Windows.Forms.Button
    Friend WithEvents BaseHeaderSetButton As System.Windows.Forms.Button
    Friend WithEvents EntDateReplaceButton As System.Windows.Forms.Button
    Friend WithEvents OrgStaReplaceButton As System.Windows.Forms.Button
    Friend WithEvents EntStaReplaceButton As System.Windows.Forms.Button
    Friend WithEvents MinDateReplaceButton As System.Windows.Forms.Button
    Friend WithEvents MaxDateReplaceButton As System.Windows.Forms.Button
    Friend WithEvents DstStaReplaceButton As System.Windows.Forms.Button
    Friend WithEvents RiyoDataAppendFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents SearchBox As System.Windows.Forms.ComboBox
    Friend WithEvents HotToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents SearchPrevButton As System.Windows.Forms.Button
    Friend WithEvents SearchNextButton As System.Windows.Forms.Button
End Class
