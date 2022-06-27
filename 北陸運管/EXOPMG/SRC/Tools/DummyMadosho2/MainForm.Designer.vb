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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TableSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.DataGridView2 = New JR.ExOpmg.DummyMadosho2.XlsDataGridView()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LogDispGrid = New System.Windows.Forms.DataGridView()
        Me.LogDispHeaderPanel = New System.Windows.Forms.Panel()
        Me.LogDispFilterEditButton = New System.Windows.Forms.Button()
        Me.LogDispClearButton = New System.Windows.Forms.Button()
        Me.LogDispCheckBox = New System.Windows.Forms.CheckBox()
        Me.LogDispFilter = New System.Windows.Forms.TextBox()
        Me.LogDispFilterLabel = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.MachineProfileFetchButton = New System.Windows.Forms.Button()
        Me.MachineStatesInitButton = New System.Windows.Forms.Button()
        Me.RandRiyoDataStoreButton = New System.Windows.Forms.Button()
        Me.RiyoDataSendButton = New System.Windows.Forms.Button()
        Me.ViewModePanel = New System.Windows.Forms.Panel()
        Me.UpboundProcStateRadioButton = New System.Windows.Forms.RadioButton()
        Me.SimWorkingDirDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableSplitContainer.Panel1.SuspendLayout()
        Me.TableSplitContainer.Panel2.SuspendLayout()
        Me.TableSplitContainer.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.LogDispGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.LogDispHeaderPanel.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.ViewModePanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 28)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableSplitContainer)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanel1)
        Me.SplitContainer1.Size = New System.Drawing.Size(997, 579)
        Me.SplitContainer1.SplitterDistance = 321
        Me.SplitContainer1.TabIndex = 1
        '
        'TableSplitContainer
        '
        Me.TableSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TableSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.TableSplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.TableSplitContainer.Name = "TableSplitContainer"
        '
        'TableSplitContainer.Panel1
        '
        Me.TableSplitContainer.Panel1.Controls.Add(Me.DataGridView1)
        '
        'TableSplitContainer.Panel2
        '
        Me.TableSplitContainer.Panel2.Controls.Add(Me.DataGridView2)
        Me.TableSplitContainer.Size = New System.Drawing.Size(997, 321)
        Me.TableSplitContainer.SplitterDistance = 384
        Me.TableSplitContainer.TabIndex = 0
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToOrderColumns = True
        Me.DataGridView1.AllowUserToResizeRows = False
        Me.DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridView1.RowTemplate.Height = 21
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(380, 317)
        Me.DataGridView1.StandardTab = True
        Me.DataGridView1.TabIndex = 0
        '
        'DataGridView2
        '
        Me.DataGridView2.AllowUserToAddRows = False
        Me.DataGridView2.AllowUserToDeleteRows = False
        Me.DataGridView2.AllowUserToOrderColumns = True
        Me.DataGridView2.AllowUserToResizeRows = False
        Me.DataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridView2.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridView2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView2.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.RowHeadersVisible = False
        Me.DataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridView2.RowTemplate.Height = 21
        Me.DataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridView2.Size = New System.Drawing.Size(605, 317)
        Me.DataGridView2.StandardTab = True
        Me.DataGridView2.TabIndex = 0
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.LogDispGrid, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.LogDispHeaderPanel, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.FlowLayoutPanel1, 0, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(993, 250)
        Me.TableLayoutPanel1.TabIndex = 1
        '
        'LogDispGrid
        '
        Me.LogDispGrid.AllowUserToAddRows = False
        Me.LogDispGrid.AllowUserToDeleteRows = False
        Me.LogDispGrid.AllowUserToResizeRows = False
        Me.LogDispGrid.BackgroundColor = System.Drawing.SystemColors.Window
        Me.LogDispGrid.CausesValidation = False
        Me.LogDispGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal
        Me.LogDispGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.LogDispGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LogDispGrid.DefaultCellStyle = DataGridViewCellStyle3
        Me.LogDispGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LogDispGrid.Location = New System.Drawing.Point(3, 58)
        Me.LogDispGrid.Name = "LogDispGrid"
        Me.LogDispGrid.ReadOnly = True
        Me.LogDispGrid.RowHeadersVisible = False
        Me.LogDispGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader
        Me.LogDispGrid.RowTemplate.Height = 21
        Me.LogDispGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.LogDispGrid.ShowCellErrors = False
        Me.LogDispGrid.ShowEditingIcon = False
        Me.LogDispGrid.ShowRowErrors = False
        Me.LogDispGrid.Size = New System.Drawing.Size(987, 189)
        Me.LogDispGrid.StandardTab = True
        Me.LogDispGrid.TabIndex = 2
        '
        'LogDispHeaderPanel
        '
        Me.LogDispHeaderPanel.Controls.Add(Me.LogDispFilterEditButton)
        Me.LogDispHeaderPanel.Controls.Add(Me.LogDispClearButton)
        Me.LogDispHeaderPanel.Controls.Add(Me.LogDispCheckBox)
        Me.LogDispHeaderPanel.Controls.Add(Me.LogDispFilter)
        Me.LogDispHeaderPanel.Controls.Add(Me.LogDispFilterLabel)
        Me.LogDispHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LogDispHeaderPanel.Location = New System.Drawing.Point(0, 29)
        Me.LogDispHeaderPanel.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.LogDispHeaderPanel.Name = "LogDispHeaderPanel"
        Me.LogDispHeaderPanel.Size = New System.Drawing.Size(993, 26)
        Me.LogDispHeaderPanel.TabIndex = 1
        '
        'LogDispFilterEditButton
        '
        Me.LogDispFilterEditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LogDispFilterEditButton.Location = New System.Drawing.Point(937, 2)
        Me.LogDispFilterEditButton.Name = "LogDispFilterEditButton"
        Me.LogDispFilterEditButton.Size = New System.Drawing.Size(53, 23)
        Me.LogDispFilterEditButton.TabIndex = 54
        Me.LogDispFilterEditButton.Text = "編集"
        Me.LogDispFilterEditButton.UseVisualStyleBackColor = True
        '
        'LogDispClearButton
        '
        Me.LogDispClearButton.Location = New System.Drawing.Point(102, 2)
        Me.LogDispClearButton.Name = "LogDispClearButton"
        Me.LogDispClearButton.Size = New System.Drawing.Size(53, 23)
        Me.LogDispClearButton.TabIndex = 51
        Me.LogDispClearButton.Text = "クリア"
        Me.LogDispClearButton.UseVisualStyleBackColor = True
        '
        'LogDispCheckBox
        '
        Me.LogDispCheckBox.AutoSize = True
        Me.LogDispCheckBox.Checked = True
        Me.LogDispCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.LogDispCheckBox.Location = New System.Drawing.Point(6, 5)
        Me.LogDispCheckBox.Name = "LogDispCheckBox"
        Me.LogDispCheckBox.Size = New System.Drawing.Size(90, 16)
        Me.LogDispCheckBox.TabIndex = 50
        Me.LogDispCheckBox.Text = "新着ログ取込"
        Me.LogDispCheckBox.UseVisualStyleBackColor = True
        '
        'LogDispFilter
        '
        Me.LogDispFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LogDispFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LogDispFilter.Location = New System.Drawing.Point(228, 4)
        Me.LogDispFilter.Name = "LogDispFilter"
        Me.LogDispFilter.ReadOnly = True
        Me.LogDispFilter.Size = New System.Drawing.Size(703, 19)
        Me.LogDispFilter.TabIndex = 53
        '
        'LogDispFilterLabel
        '
        Me.LogDispFilterLabel.AutoSize = True
        Me.LogDispFilterLabel.Location = New System.Drawing.Point(184, 7)
        Me.LogDispFilterLabel.Name = "LogDispFilterLabel"
        Me.LogDispFilterLabel.Size = New System.Drawing.Size(38, 12)
        Me.LogDispFilterLabel.TabIndex = 52
        Me.LogDispFilterLabel.Text = "フィルタ"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.Label1)
        Me.FlowLayoutPanel1.Controls.Add(Me.MachineProfileFetchButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.MachineStatesInitButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.RandRiyoDataStoreButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.RiyoDataSendButton)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(993, 26)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 8)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "能動処理"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'MachineProfileFetchButton
        '
        Me.MachineProfileFetchButton.AutoSize = True
        Me.MachineProfileFetchButton.Location = New System.Drawing.Point(62, 3)
        Me.MachineProfileFetchButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MachineProfileFetchButton.Name = "MachineProfileFetchButton"
        Me.MachineProfileFetchButton.Size = New System.Drawing.Size(106, 23)
        Me.MachineProfileFetchButton.TabIndex = 0
        Me.MachineProfileFetchButton.Text = "機器構成読み取り"
        Me.MachineProfileFetchButton.UseVisualStyleBackColor = True
        '
        'MachineStatesInitButton
        '
        Me.MachineStatesInitButton.AutoSize = True
        Me.MachineStatesInitButton.Location = New System.Drawing.Point(174, 3)
        Me.MachineStatesInitButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MachineStatesInitButton.Name = "MachineStatesInitButton"
        Me.MachineStatesInitButton.Size = New System.Drawing.Size(106, 23)
        Me.MachineStatesInitButton.TabIndex = 1
        Me.MachineStatesInitButton.Text = "機器状態初期化"
        Me.MachineStatesInitButton.UseVisualStyleBackColor = True
        '
        'RandRiyoDataStoreButton
        '
        Me.RandRiyoDataStoreButton.AutoSize = True
        Me.RandRiyoDataStoreButton.Location = New System.Drawing.Point(286, 3)
        Me.RandRiyoDataStoreButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.RandRiyoDataStoreButton.Name = "RandRiyoDataStoreButton"
        Me.RandRiyoDataStoreButton.Size = New System.Drawing.Size(115, 23)
        Me.RandRiyoDataStoreButton.TabIndex = 2
        Me.RandRiyoDataStoreButton.Text = "RND利用データ蓄積"
        Me.RandRiyoDataStoreButton.UseVisualStyleBackColor = True
        '
        'RiyoDataSendButton
        '
        Me.RiyoDataSendButton.AutoSize = True
        Me.RiyoDataSendButton.Location = New System.Drawing.Point(407, 3)
        Me.RiyoDataSendButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.RiyoDataSendButton.Name = "RiyoDataSendButton"
        Me.RiyoDataSendButton.Size = New System.Drawing.Size(106, 23)
        Me.RiyoDataSendButton.TabIndex = 3
        Me.RiyoDataSendButton.Text = "利用データ送信"
        Me.RiyoDataSendButton.UseVisualStyleBackColor = True
        '
        'ViewModePanel
        '
        Me.ViewModePanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ViewModePanel.Controls.Add(Me.UpboundProcStateRadioButton)
        Me.ViewModePanel.Location = New System.Drawing.Point(0, 0)
        Me.ViewModePanel.Name = "ViewModePanel"
        Me.ViewModePanel.Size = New System.Drawing.Size(997, 24)
        Me.ViewModePanel.TabIndex = 0
        '
        'UpboundProcStateRadioButton
        '
        Me.UpboundProcStateRadioButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UpboundProcStateRadioButton.AutoSize = True
        Me.UpboundProcStateRadioButton.Checked = True
        Me.UpboundProcStateRadioButton.Location = New System.Drawing.Point(895, 5)
        Me.UpboundProcStateRadioButton.Name = "UpboundProcStateRadioButton"
        Me.UpboundProcStateRadioButton.Size = New System.Drawing.Size(99, 16)
        Me.UpboundProcStateRadioButton.TabIndex = 0
        Me.UpboundProcStateRadioButton.TabStop = True
        Me.UpboundProcStateRadioButton.Text = "データ発生状態"
        Me.UpboundProcStateRadioButton.UseVisualStyleBackColor = True
        '
        'SimWorkingDirDialog
        '
        Me.SimWorkingDirDialog.Description = "シミュレータ本体の起動ディレクトリを選択してください。"
        Me.SimWorkingDirDialog.RootFolder = System.Environment.SpecialFolder.MyComputer
        Me.SimWorkingDirDialog.ShowNewFolderButton = False
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(997, 607)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ViewModePanel)
        Me.Name = "MainForm"
        Me.Text = "多重窓口処理機向け 利用データサーバ"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableSplitContainer.Panel1.ResumeLayout(False)
        Me.TableSplitContainer.Panel2.ResumeLayout(False)
        Me.TableSplitContainer.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.LogDispGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.LogDispHeaderPanel.ResumeLayout(False)
        Me.LogDispHeaderPanel.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ViewModePanel.ResumeLayout(False)
        Me.ViewModePanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableSplitContainer As System.Windows.Forms.SplitContainer
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridView2 As JR.ExOpmg.DummyMadosho2.XlsDataGridView
    Friend WithEvents MachineProfileFetchButton As System.Windows.Forms.Button
    Friend WithEvents RiyoDataSendButton As System.Windows.Forms.Button
    Friend WithEvents ViewModePanel As System.Windows.Forms.Panel
    Friend WithEvents UpboundProcStateRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents SimWorkingDirDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents MachineStatesInitButton As System.Windows.Forms.Button
    Friend WithEvents RandRiyoDataStoreButton As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LogDispHeaderPanel As System.Windows.Forms.Panel
    Friend WithEvents LogDispFilterEditButton As System.Windows.Forms.Button
    Friend WithEvents LogDispClearButton As System.Windows.Forms.Button
    Friend WithEvents LogDispCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents LogDispFilter As System.Windows.Forms.TextBox
    Friend WithEvents LogDispFilterLabel As System.Windows.Forms.Label
    Friend WithEvents LogDispGrid As System.Windows.Forms.DataGridView

End Class
