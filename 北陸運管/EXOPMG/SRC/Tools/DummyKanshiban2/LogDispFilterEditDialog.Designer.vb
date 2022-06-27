<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LogDispFilterEditDialog
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.DirectEditButton = New System.Windows.Forms.RadioButton()
        Me.EasyEditButton = New System.Windows.Forms.RadioButton()
        Me.SourceSelectorGrid = New System.Windows.Forms.DataGridView()
        Me.SourceSelectorLabel = New System.Windows.Forms.Label()
        Me.AllSourcesSelectButton = New System.Windows.Forms.Button()
        Me.AllSourcesDeselectButton = New System.Windows.Forms.Button()
        Me.EasyEditPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.EasyEditRefreshButton = New System.Windows.Forms.Button()
        Me.KindSelectorGrid = New System.Windows.Forms.DataGridView()
        Me.SourceSelectorPanel = New System.Windows.Forms.Panel()
        Me.KindSelectorPanel = New System.Windows.Forms.Panel()
        Me.KindSelectorLabel = New System.Windows.Forms.Label()
        Me.AllKindsDeselectButton = New System.Windows.Forms.Button()
        Me.AllKindsSelectButton = New System.Windows.Forms.Button()
        Me.GridMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItemOfSelect = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemOfDeselect = New System.Windows.Forms.ToolStripMenuItem()
        Me.Filter = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.SourceSelectorGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.EasyEditPanel.SuspendLayout()
        CType(Me.KindSelectorGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SourceSelectorPanel.SuspendLayout()
        Me.KindSelectorPanel.SuspendLayout()
        Me.GridMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(549, 494)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 27)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 21)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 21)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "キャンセル"
        '
        'DirectEditButton
        '
        Me.DirectEditButton.AutoSize = True
        Me.DirectEditButton.Location = New System.Drawing.Point(12, 12)
        Me.DirectEditButton.Name = "DirectEditButton"
        Me.DirectEditButton.Size = New System.Drawing.Size(71, 16)
        Me.DirectEditButton.TabIndex = 0
        Me.DirectEditButton.TabStop = True
        Me.DirectEditButton.Text = "直接編集"
        Me.DirectEditButton.UseVisualStyleBackColor = True
        '
        'EasyEditButton
        '
        Me.EasyEditButton.AutoSize = True
        Me.EasyEditButton.Location = New System.Drawing.Point(12, 75)
        Me.EasyEditButton.Name = "EasyEditButton"
        Me.EasyEditButton.Size = New System.Drawing.Size(71, 16)
        Me.EasyEditButton.TabIndex = 1
        Me.EasyEditButton.TabStop = True
        Me.EasyEditButton.Text = "簡易編集"
        Me.EasyEditButton.UseVisualStyleBackColor = True
        '
        'SourceSelectorGrid
        '
        Me.SourceSelectorGrid.AllowUserToAddRows = False
        Me.SourceSelectorGrid.AllowUserToDeleteRows = False
        Me.SourceSelectorGrid.AllowUserToResizeColumns = False
        Me.SourceSelectorGrid.AllowUserToResizeRows = False
        Me.SourceSelectorGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SourceSelectorGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal
        Me.SourceSelectorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.SourceSelectorGrid.ColumnHeadersVisible = False
        Me.SourceSelectorGrid.Location = New System.Drawing.Point(3, 57)
        Me.SourceSelectorGrid.Name = "SourceSelectorGrid"
        Me.SourceSelectorGrid.RowHeadersVisible = False
        Me.SourceSelectorGrid.RowTemplate.Height = 21
        Me.SourceSelectorGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.SourceSelectorGrid.Size = New System.Drawing.Size(323, 321)
        Me.SourceSelectorGrid.StandardTab = True
        Me.SourceSelectorGrid.TabIndex = 0
        '
        'SourceSelectorLabel
        '
        Me.SourceSelectorLabel.AutoSize = True
        Me.SourceSelectorLabel.Location = New System.Drawing.Point(3, 5)
        Me.SourceSelectorLabel.Name = "SourceSelectorLabel"
        Me.SourceSelectorLabel.Size = New System.Drawing.Size(74, 12)
        Me.SourceSelectorLabel.TabIndex = 0
        Me.SourceSelectorLabel.Text = "出力元を選択"
        '
        'AllSourcesSelectButton
        '
        Me.AllSourcesSelectButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AllSourcesSelectButton.Location = New System.Drawing.Point(203, 0)
        Me.AllSourcesSelectButton.Name = "AllSourcesSelectButton"
        Me.AllSourcesSelectButton.Size = New System.Drawing.Size(57, 23)
        Me.AllSourcesSelectButton.TabIndex = 1
        Me.AllSourcesSelectButton.Text = "全選択"
        Me.AllSourcesSelectButton.UseVisualStyleBackColor = True
        '
        'AllSourcesDeselectButton
        '
        Me.AllSourcesDeselectButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AllSourcesDeselectButton.Location = New System.Drawing.Point(266, 0)
        Me.AllSourcesDeselectButton.Name = "AllSourcesDeselectButton"
        Me.AllSourcesDeselectButton.Size = New System.Drawing.Size(57, 23)
        Me.AllSourcesDeselectButton.TabIndex = 2
        Me.AllSourcesDeselectButton.Text = "全解除"
        Me.AllSourcesDeselectButton.UseVisualStyleBackColor = True
        '
        'EasyEditPanel
        '
        Me.EasyEditPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.EasyEditPanel.ColumnCount = 3
        Me.EasyEditPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.EasyEditPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
        Me.EasyEditPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.EasyEditPanel.Controls.Add(Me.EasyEditRefreshButton, 0, 0)
        Me.EasyEditPanel.Controls.Add(Me.KindSelectorGrid, 2, 2)
        Me.EasyEditPanel.Controls.Add(Me.SourceSelectorGrid, 0, 2)
        Me.EasyEditPanel.Controls.Add(Me.SourceSelectorPanel, 0, 1)
        Me.EasyEditPanel.Controls.Add(Me.KindSelectorPanel, 2, 1)
        Me.EasyEditPanel.Location = New System.Drawing.Point(26, 97)
        Me.EasyEditPanel.Name = "EasyEditPanel"
        Me.EasyEditPanel.RowCount = 3
        Me.EasyEditPanel.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.EasyEditPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28.0!))
        Me.EasyEditPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.EasyEditPanel.Size = New System.Drawing.Size(669, 381)
        Me.EasyEditPanel.TabIndex = 3
        '
        'EasyEditRefreshButton
        '
        Me.EasyEditRefreshButton.Location = New System.Drawing.Point(3, 0)
        Me.EasyEditRefreshButton.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.EasyEditRefreshButton.Name = "EasyEditRefreshButton"
        Me.EasyEditRefreshButton.Size = New System.Drawing.Size(75, 23)
        Me.EasyEditRefreshButton.TabIndex = 5
        Me.EasyEditRefreshButton.Text = "リスト更新"
        Me.EasyEditRefreshButton.UseVisualStyleBackColor = True
        '
        'KindSelectorGrid
        '
        Me.KindSelectorGrid.AllowUserToAddRows = False
        Me.KindSelectorGrid.AllowUserToDeleteRows = False
        Me.KindSelectorGrid.AllowUserToResizeColumns = False
        Me.KindSelectorGrid.AllowUserToResizeRows = False
        Me.KindSelectorGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.KindSelectorGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal
        Me.KindSelectorGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.KindSelectorGrid.ColumnHeadersVisible = False
        Me.KindSelectorGrid.Location = New System.Drawing.Point(342, 57)
        Me.KindSelectorGrid.Name = "KindSelectorGrid"
        Me.KindSelectorGrid.RowHeadersVisible = False
        Me.KindSelectorGrid.RowTemplate.Height = 21
        Me.KindSelectorGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.KindSelectorGrid.Size = New System.Drawing.Size(324, 321)
        Me.KindSelectorGrid.StandardTab = True
        Me.KindSelectorGrid.TabIndex = 3
        '
        'SourceSelectorPanel
        '
        Me.SourceSelectorPanel.Controls.Add(Me.SourceSelectorLabel)
        Me.SourceSelectorPanel.Controls.Add(Me.AllSourcesDeselectButton)
        Me.SourceSelectorPanel.Controls.Add(Me.AllSourcesSelectButton)
        Me.SourceSelectorPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.SourceSelectorPanel.Location = New System.Drawing.Point(3, 30)
        Me.SourceSelectorPanel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.SourceSelectorPanel.Name = "SourceSelectorPanel"
        Me.SourceSelectorPanel.Size = New System.Drawing.Size(323, 24)
        Me.SourceSelectorPanel.TabIndex = 1
        '
        'KindSelectorPanel
        '
        Me.KindSelectorPanel.Controls.Add(Me.KindSelectorLabel)
        Me.KindSelectorPanel.Controls.Add(Me.AllKindsDeselectButton)
        Me.KindSelectorPanel.Controls.Add(Me.AllKindsSelectButton)
        Me.KindSelectorPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.KindSelectorPanel.Location = New System.Drawing.Point(342, 30)
        Me.KindSelectorPanel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.KindSelectorPanel.Name = "KindSelectorPanel"
        Me.KindSelectorPanel.Size = New System.Drawing.Size(324, 24)
        Me.KindSelectorPanel.TabIndex = 2
        '
        'KindSelectorLabel
        '
        Me.KindSelectorLabel.AutoSize = True
        Me.KindSelectorLabel.Location = New System.Drawing.Point(3, 5)
        Me.KindSelectorLabel.Name = "KindSelectorLabel"
        Me.KindSelectorLabel.Size = New System.Drawing.Size(62, 12)
        Me.KindSelectorLabel.TabIndex = 0
        Me.KindSelectorLabel.Text = "種別を選択"
        '
        'AllKindsDeselectButton
        '
        Me.AllKindsDeselectButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AllKindsDeselectButton.Location = New System.Drawing.Point(267, 0)
        Me.AllKindsDeselectButton.Name = "AllKindsDeselectButton"
        Me.AllKindsDeselectButton.Size = New System.Drawing.Size(57, 23)
        Me.AllKindsDeselectButton.TabIndex = 2
        Me.AllKindsDeselectButton.Text = "全解除"
        Me.AllKindsDeselectButton.UseVisualStyleBackColor = True
        '
        'AllKindsSelectButton
        '
        Me.AllKindsSelectButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AllKindsSelectButton.Location = New System.Drawing.Point(204, 0)
        Me.AllKindsSelectButton.Name = "AllKindsSelectButton"
        Me.AllKindsSelectButton.Size = New System.Drawing.Size(57, 23)
        Me.AllKindsSelectButton.TabIndex = 1
        Me.AllKindsSelectButton.Text = "全選択"
        Me.AllKindsSelectButton.UseVisualStyleBackColor = True
        '
        'GridMenuStrip
        '
        Me.GridMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemOfSelect, Me.ToolStripMenuItemOfDeselect})
        Me.GridMenuStrip.Name = "GridMenuStrip"
        Me.GridMenuStrip.Size = New System.Drawing.Size(125, 48)
        '
        'ToolStripMenuItemOfSelect
        '
        Me.ToolStripMenuItemOfSelect.Name = "ToolStripMenuItemOfSelect"
        Me.ToolStripMenuItemOfSelect.Size = New System.Drawing.Size(124, 22)
        Me.ToolStripMenuItemOfSelect.Text = "選択する"
        '
        'ToolStripMenuItemOfDeselect
        '
        Me.ToolStripMenuItemOfDeselect.Name = "ToolStripMenuItemOfDeselect"
        Me.ToolStripMenuItemOfDeselect.Size = New System.Drawing.Size(124, 22)
        Me.ToolStripMenuItemOfDeselect.Text = "解除する"
        '
        'Filter
        '
        Me.Filter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Filter.FormattingEnabled = True
        Me.Filter.Location = New System.Drawing.Point(26, 35)
        Me.Filter.Name = "Filter"
        Me.Filter.Size = New System.Drawing.Size(669, 20)
        Me.Filter.TabIndex = 2
        '
        'LogDispFilterEditDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(707, 532)
        Me.Controls.Add(Me.Filter)
        Me.Controls.Add(Me.EasyEditPanel)
        Me.Controls.Add(Me.EasyEditButton)
        Me.Controls.Add(Me.DirectEditButton)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(600, 300)
        Me.Name = "LogDispFilterEditDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "フィルタ編集"
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.SourceSelectorGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.EasyEditPanel.ResumeLayout(False)
        CType(Me.KindSelectorGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SourceSelectorPanel.ResumeLayout(False)
        Me.SourceSelectorPanel.PerformLayout()
        Me.KindSelectorPanel.ResumeLayout(False)
        Me.KindSelectorPanel.PerformLayout()
        Me.GridMenuStrip.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents DirectEditButton As System.Windows.Forms.RadioButton
    Friend WithEvents EasyEditButton As System.Windows.Forms.RadioButton
    Friend WithEvents SourceSelectorGrid As System.Windows.Forms.DataGridView
    Friend WithEvents SourceSelectorLabel As System.Windows.Forms.Label
    Friend WithEvents AllSourcesSelectButton As System.Windows.Forms.Button
    Friend WithEvents AllSourcesDeselectButton As System.Windows.Forms.Button
    Friend WithEvents EasyEditPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents SourceSelectorPanel As System.Windows.Forms.Panel
    Friend WithEvents GridMenuStrip As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItemOfSelect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemOfDeselect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Filter As System.Windows.Forms.ComboBox
    Friend WithEvents KindSelectorGrid As System.Windows.Forms.DataGridView
    Friend WithEvents KindSelectorPanel As System.Windows.Forms.Panel
    Friend WithEvents KindSelectorLabel As System.Windows.Forms.Label
    Friend WithEvents AllKindsDeselectButton As System.Windows.Forms.Button
    Friend WithEvents AllKindsSelectButton As System.Windows.Forms.Button
    Friend WithEvents EasyEditRefreshButton As System.Windows.Forms.Button

End Class
