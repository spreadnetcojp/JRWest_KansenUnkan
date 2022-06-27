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
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TableLayoutPanelUpper = New System.Windows.Forms.TableLayoutPanel()
        Me.TableSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.DataGridView2 = New JR.ExOpmg.DummyKanshiban.XlsDataGridView()
        Me.ViewModePanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.UpboundProcStateRadioButton = New System.Windows.Forms.RadioButton()
        Me.KsbConfigRadioButton = New System.Windows.Forms.RadioButton()
        Me.KsbProStatusRadioButton = New System.Windows.Forms.RadioButton()
        Me.ProStatusRadioButton = New System.Windows.Forms.RadioButton()
        Me.MasStatusRadioButton = New System.Windows.Forms.RadioButton()
        Me.ConStatusRadioButton = New System.Windows.Forms.RadioButton()
        Me.SymbolizeCheckBox = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanelLower = New System.Windows.Forms.TableLayoutPanel()
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
        Me.ConStatusSendButton = New System.Windows.Forms.Button()
        Me.MasClearButton = New System.Windows.Forms.Button()
        Me.MasDeliverButton = New System.Windows.Forms.Button()
        Me.ProDirectInstallButton = New System.Windows.Forms.Button()
        Me.ProDeliverButton = New System.Windows.Forms.Button()
        Me.ProApplyButton = New System.Windows.Forms.Button()
        Me.KsbProDirectInstallButton = New System.Windows.Forms.Button()
        Me.KsbProDeliverButton = New System.Windows.Forms.Button()
        Me.KsbProApplyButton = New System.Windows.Forms.Button()
        Me.KsbConfigSendButton = New System.Windows.Forms.Button()
        Me.UpboundDataClearButton = New System.Windows.Forms.Button()
        Me.RandFaultDataStoreButton = New System.Windows.Forms.Button()
        Me.RandFaultDataSendButton = New System.Windows.Forms.Button()
        Me.KadoDataRandUpdateButton = New System.Windows.Forms.Button()
        Me.KadoDataCommitButton = New System.Windows.Forms.Button()
        Me.SimWorkingDirDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.UsageTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableLayoutPanelUpper.SuspendLayout()
        Me.TableSplitContainer.Panel1.SuspendLayout()
        Me.TableSplitContainer.Panel2.SuspendLayout()
        Me.TableSplitContainer.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ViewModePanel.SuspendLayout()
        Me.TableLayoutPanelLower.SuspendLayout()
        CType(Me.LogDispGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.LogDispHeaderPanel.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanelUpper)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanelLower)
        Me.SplitContainer1.Size = New System.Drawing.Size(1008, 607)
        Me.SplitContainer1.SplitterDistance = 336
        Me.SplitContainer1.TabIndex = 0
        '
        'TableLayoutPanelUpper
        '
        Me.TableLayoutPanelUpper.ColumnCount = 2
        Me.TableLayoutPanelUpper.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanelUpper.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelUpper.Controls.Add(Me.TableSplitContainer, 0, 1)
        Me.TableLayoutPanelUpper.Controls.Add(Me.ViewModePanel, 1, 0)
        Me.TableLayoutPanelUpper.Controls.Add(Me.SymbolizeCheckBox, 0, 0)
        Me.TableLayoutPanelUpper.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanelUpper.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanelUpper.Name = "TableLayoutPanelUpper"
        Me.TableLayoutPanelUpper.RowCount = 2
        Me.TableLayoutPanelUpper.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelUpper.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelUpper.Size = New System.Drawing.Size(1004, 332)
        Me.TableLayoutPanelUpper.TabIndex = 0
        '
        'TableSplitContainer
        '
        Me.TableSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.TableLayoutPanelUpper.SetColumnSpan(Me.TableSplitContainer, 2)
        Me.TableSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.TableSplitContainer.Location = New System.Drawing.Point(3, 25)
        Me.TableSplitContainer.Name = "TableSplitContainer"
        '
        'TableSplitContainer.Panel1
        '
        Me.TableSplitContainer.Panel1.Controls.Add(Me.DataGridView1)
        '
        'TableSplitContainer.Panel2
        '
        Me.TableSplitContainer.Panel2.Controls.Add(Me.DataGridView2)
        Me.TableSplitContainer.Size = New System.Drawing.Size(998, 304)
        Me.TableSplitContainer.SplitterDistance = 384
        Me.TableSplitContainer.TabIndex = 2
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
        Me.DataGridView1.Size = New System.Drawing.Size(380, 300)
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
        Me.DataGridView2.Size = New System.Drawing.Size(606, 300)
        Me.DataGridView2.StandardTab = True
        Me.DataGridView2.TabIndex = 0
        '
        'ViewModePanel
        '
        Me.ViewModePanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ViewModePanel.AutoSize = True
        Me.ViewModePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ViewModePanel.Controls.Add(Me.UpboundProcStateRadioButton)
        Me.ViewModePanel.Controls.Add(Me.KsbConfigRadioButton)
        Me.ViewModePanel.Controls.Add(Me.KsbProStatusRadioButton)
        Me.ViewModePanel.Controls.Add(Me.ProStatusRadioButton)
        Me.ViewModePanel.Controls.Add(Me.MasStatusRadioButton)
        Me.ViewModePanel.Controls.Add(Me.ConStatusRadioButton)
        Me.ViewModePanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.ViewModePanel.Location = New System.Drawing.Point(103, 0)
        Me.ViewModePanel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.ViewModePanel.Name = "ViewModePanel"
        Me.ViewModePanel.Size = New System.Drawing.Size(898, 22)
        Me.ViewModePanel.TabIndex = 1
        '
        'UpboundProcStateRadioButton
        '
        Me.UpboundProcStateRadioButton.AutoSize = True
        Me.UpboundProcStateRadioButton.Location = New System.Drawing.Point(776, 3)
        Me.UpboundProcStateRadioButton.Name = "UpboundProcStateRadioButton"
        Me.UpboundProcStateRadioButton.Size = New System.Drawing.Size(119, 16)
        Me.UpboundProcStateRadioButton.TabIndex = 5
        Me.UpboundProcStateRadioButton.TabStop = True
        Me.UpboundProcStateRadioButton.Text = "上りデータ発生状態"
        Me.UpboundProcStateRadioButton.UseVisualStyleBackColor = True
        '
        'KsbConfigRadioButton
        '
        Me.KsbConfigRadioButton.AutoSize = True
        Me.KsbConfigRadioButton.Location = New System.Drawing.Point(663, 3)
        Me.KsbConfigRadioButton.Name = "KsbConfigRadioButton"
        Me.KsbConfigRadioButton.Size = New System.Drawing.Size(107, 16)
        Me.KsbConfigRadioButton.TabIndex = 4
        Me.KsbConfigRadioButton.TabStop = True
        Me.KsbConfigRadioButton.Text = "監視盤設定状態"
        Me.KsbConfigRadioButton.UseVisualStyleBackColor = True
        '
        'KsbProStatusRadioButton
        '
        Me.KsbProStatusRadioButton.AutoSize = True
        Me.KsbProStatusRadioButton.Location = New System.Drawing.Point(532, 3)
        Me.KsbProStatusRadioButton.Name = "KsbProStatusRadioButton"
        Me.KsbProStatusRadioButton.Size = New System.Drawing.Size(125, 16)
        Me.KsbProStatusRadioButton.TabIndex = 3
        Me.KsbProStatusRadioButton.TabStop = True
        Me.KsbProStatusRadioButton.Text = "監プロ配信適用状態"
        Me.KsbProStatusRadioButton.UseVisualStyleBackColor = True
        '
        'ProStatusRadioButton
        '
        Me.ProStatusRadioButton.AutoSize = True
        Me.ProStatusRadioButton.Location = New System.Drawing.Point(401, 3)
        Me.ProStatusRadioButton.Name = "ProStatusRadioButton"
        Me.ProStatusRadioButton.Size = New System.Drawing.Size(125, 16)
        Me.ProStatusRadioButton.TabIndex = 2
        Me.ProStatusRadioButton.TabStop = True
        Me.ProStatusRadioButton.Text = "改プロ配信適用状態"
        Me.ProStatusRadioButton.UseVisualStyleBackColor = True
        '
        'MasStatusRadioButton
        '
        Me.MasStatusRadioButton.AutoSize = True
        Me.MasStatusRadioButton.Location = New System.Drawing.Point(298, 3)
        Me.MasStatusRadioButton.Name = "MasStatusRadioButton"
        Me.MasStatusRadioButton.Size = New System.Drawing.Size(97, 16)
        Me.MasStatusRadioButton.TabIndex = 1
        Me.MasStatusRadioButton.TabStop = True
        Me.MasStatusRadioButton.Text = "マスタ配信状態"
        Me.MasStatusRadioButton.UseVisualStyleBackColor = True
        '
        'ConStatusRadioButton
        '
        Me.ConStatusRadioButton.AutoSize = True
        Me.ConStatusRadioButton.Checked = True
        Me.ConStatusRadioButton.Location = New System.Drawing.Point(221, 3)
        Me.ConStatusRadioButton.Name = "ConStatusRadioButton"
        Me.ConStatusRadioButton.Size = New System.Drawing.Size(71, 16)
        Me.ConStatusRadioButton.TabIndex = 0
        Me.ConStatusRadioButton.TabStop = True
        Me.ConStatusRadioButton.Text = "接続状態"
        Me.ConStatusRadioButton.UseVisualStyleBackColor = True
        '
        'SymbolizeCheckBox
        '
        Me.SymbolizeCheckBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SymbolizeCheckBox.AutoSize = True
        Me.SymbolizeCheckBox.Location = New System.Drawing.Point(3, 3)
        Me.SymbolizeCheckBox.Name = "SymbolizeCheckBox"
        Me.SymbolizeCheckBox.Size = New System.Drawing.Size(94, 16)
        Me.SymbolizeCheckBox.TabIndex = 0
        Me.SymbolizeCheckBox.Text = "駅名表示"
        Me.SymbolizeCheckBox.UseVisualStyleBackColor = True
        '
        'TableLayoutPanelLower
        '
        Me.TableLayoutPanelLower.ColumnCount = 1
        Me.TableLayoutPanelLower.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelLower.Controls.Add(Me.LogDispGrid, 0, 2)
        Me.TableLayoutPanelLower.Controls.Add(Me.LogDispHeaderPanel, 0, 1)
        Me.TableLayoutPanelLower.Controls.Add(Me.FlowLayoutPanel1, 0, 0)
        Me.TableLayoutPanelLower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanelLower.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanelLower.Name = "TableLayoutPanelLower"
        Me.TableLayoutPanelLower.RowCount = 3
        Me.TableLayoutPanelLower.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelLower.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelLower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelLower.Size = New System.Drawing.Size(1004, 263)
        Me.TableLayoutPanelLower.TabIndex = 0
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
        Me.LogDispGrid.Location = New System.Drawing.Point(3, 84)
        Me.LogDispGrid.Name = "LogDispGrid"
        Me.LogDispGrid.ReadOnly = True
        Me.LogDispGrid.RowHeadersVisible = False
        Me.LogDispGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader
        Me.LogDispGrid.RowTemplate.Height = 21
        Me.LogDispGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.LogDispGrid.ShowCellErrors = False
        Me.LogDispGrid.ShowEditingIcon = False
        Me.LogDispGrid.ShowRowErrors = False
        Me.LogDispGrid.Size = New System.Drawing.Size(998, 176)
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
        Me.LogDispHeaderPanel.Location = New System.Drawing.Point(0, 55)
        Me.LogDispHeaderPanel.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.LogDispHeaderPanel.Name = "LogDispHeaderPanel"
        Me.LogDispHeaderPanel.Size = New System.Drawing.Size(1004, 26)
        Me.LogDispHeaderPanel.TabIndex = 1
        '
        'LogDispFilterEditButton
        '
        Me.LogDispFilterEditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LogDispFilterEditButton.Location = New System.Drawing.Point(948, 2)
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
        Me.LogDispFilter.Size = New System.Drawing.Size(714, 19)
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
        Me.FlowLayoutPanel1.Controls.Add(Me.ConStatusSendButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.MasClearButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.MasDeliverButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.ProDirectInstallButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.ProDeliverButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.ProApplyButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.KsbProDirectInstallButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.KsbProDeliverButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.KsbProApplyButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.KsbConfigSendButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.UpboundDataClearButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.RandFaultDataStoreButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.RandFaultDataSendButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.KadoDataRandUpdateButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.KadoDataCommitButton)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(1004, 52)
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
        Me.UsageTip.SetToolTip(Me.Label1, "能動処理は、シナリオからだけでなく、ここにあるボタンからも実行できます")
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
        Me.UsageTip.SetToolTip(Me.MachineProfileFetchButton, "シミュレータ本体のTMPフォルダから機器構成を読み取ります。")
        Me.MachineProfileFetchButton.UseVisualStyleBackColor = True
        '
        'ConStatusSendButton
        '
        Me.ConStatusSendButton.AutoSize = True
        Me.ConStatusSendButton.Location = New System.Drawing.Point(174, 3)
        Me.ConStatusSendButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ConStatusSendButton.Name = "ConStatusSendButton"
        Me.ConStatusSendButton.Size = New System.Drawing.Size(87, 23)
        Me.ConStatusSendButton.TabIndex = 1
        Me.ConStatusSendButton.Text = "接続状態送信"
        Me.UsageTip.SetToolTip(Me.ConStatusSendButton, "左表で選択中の監視盤から運管サーバへ、改札機接続状態を送信します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "DummyKanshiban制御用シナリオでは、周期的にこの処理を自動で呼び出します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "こ" & _
                "の処理を手動で呼び出す必要があるのは、変更した接続状態を即座に運管サーバに" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "送信したいときのみです。")
        Me.ConStatusSendButton.UseVisualStyleBackColor = True
        '
        'MasClearButton
        '
        Me.MasClearButton.AutoSize = True
        Me.MasClearButton.Location = New System.Drawing.Point(267, 3)
        Me.MasClearButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MasClearButton.Name = "MasClearButton"
        Me.MasClearButton.Size = New System.Drawing.Size(87, 23)
        Me.MasClearButton.TabIndex = 2
        Me.MasClearButton.Text = "マスタクリア"
        Me.UsageTip.SetToolTip(Me.MasClearButton, "左表で選択中の監視盤とその配下の改札機について、保持しているマスタおよびマスタ適用リストをクリアします。")
        Me.MasClearButton.UseVisualStyleBackColor = True
        '
        'MasDeliverButton
        '
        Me.MasDeliverButton.AutoSize = True
        Me.MasDeliverButton.Location = New System.Drawing.Point(360, 3)
        Me.MasDeliverButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MasDeliverButton.Name = "MasDeliverButton"
        Me.MasDeliverButton.Size = New System.Drawing.Size(122, 23)
        Me.MasDeliverButton.TabIndex = 3
        Me.MasDeliverButton.Text = "配信待ちマスタ全配信"
        Me.UsageTip.SetToolTip(Me.MasDeliverButton, "左表で選択中の監視盤から改札機へ、配信保留中の全マスタを配信します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "DummyKanshiban制御用シナリオでは、シナリオを開始したタイミングや" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "監視盤が" & _
                "改札機マスタを受信したタイミングにおいて、この処理を自動で呼び出します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "この処理を手動で呼び出す必要があるのは、配信保留の要因を解除したとき、つまり" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "改札" & _
                "機の主制状態を「正常」に戻したときのみです。")
        Me.MasDeliverButton.UseVisualStyleBackColor = True
        '
        'ProDirectInstallButton
        '
        Me.ProDirectInstallButton.AutoSize = True
        Me.ProDirectInstallButton.Location = New System.Drawing.Point(488, 3)
        Me.ProDirectInstallButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ProDirectInstallButton.Name = "ProDirectInstallButton"
        Me.ProDirectInstallButton.Size = New System.Drawing.Size(106, 23)
        Me.ProDirectInstallButton.TabIndex = 4
        Me.ProDirectInstallButton.Text = "改プロ直接投入"
        Me.UsageTip.SetToolTip(Me.ProDirectInstallButton, "左表で選択中の監視盤の配下の全改札機に対して、プログラム（CABファイル）を直接投入します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "待機面に投入しますので、それを適用面に移すには、改札機の再起動（適" & _
                "用待ち改プロ全適用）が必要です。")
        Me.ProDirectInstallButton.UseVisualStyleBackColor = True
        '
        'ProDeliverButton
        '
        Me.ProDeliverButton.AutoSize = True
        Me.ProDeliverButton.Location = New System.Drawing.Point(600, 3)
        Me.ProDeliverButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ProDeliverButton.Name = "ProDeliverButton"
        Me.ProDeliverButton.Size = New System.Drawing.Size(126, 23)
        Me.ProDeliverButton.TabIndex = 5
        Me.ProDeliverButton.Text = "配信待ち改プロ全配信"
        Me.UsageTip.SetToolTip(Me.ProDeliverButton, "左表で選択中の監視盤から改札機へ、配信保留中の全改札機プログラムを配信します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "DummyKanshiban制御用シナリオでは、シナリオを開始したタイミングや" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "監視盤が改札機プログラムを受信したタイミングにおいて、この処理を自動で呼び出します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "この処理を手動で呼び出す必要があるのは、配信保留の要因を解除したとき、" & _
                "つまり" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "改札機の主制状態を「正常」に戻したときのみです。")
        Me.ProDeliverButton.UseVisualStyleBackColor = True
        '
        'ProApplyButton
        '
        Me.ProApplyButton.AutoSize = True
        Me.ProApplyButton.Location = New System.Drawing.Point(732, 3)
        Me.ProApplyButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ProApplyButton.Name = "ProApplyButton"
        Me.ProApplyButton.Size = New System.Drawing.Size(126, 23)
        Me.ProApplyButton.TabIndex = 6
        Me.ProApplyButton.Text = "適用待ち改プロ全適用"
        Me.UsageTip.SetToolTip(Me.ProApplyButton, resources.GetString("ProApplyButton.ToolTip"))
        Me.ProApplyButton.UseVisualStyleBackColor = True
        '
        'KsbProDirectInstallButton
        '
        Me.KsbProDirectInstallButton.AutoSize = True
        Me.KsbProDirectInstallButton.Location = New System.Drawing.Point(864, 3)
        Me.KsbProDirectInstallButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.KsbProDirectInstallButton.Name = "KsbProDirectInstallButton"
        Me.KsbProDirectInstallButton.Size = New System.Drawing.Size(106, 23)
        Me.KsbProDirectInstallButton.TabIndex = 7
        Me.KsbProDirectInstallButton.Text = "監プロ直接投入"
        Me.UsageTip.SetToolTip(Me.KsbProDirectInstallButton, "左表で選択中の監視盤に対して、プログラム（CABファイル）を直接投入します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "待機面に投入しますので、それを適用面に移すには、監視盤の再起動（適用待ち監プロ全適" & _
                "用）が必要です。")
        Me.KsbProDirectInstallButton.UseVisualStyleBackColor = True
        '
        'KsbProDeliverButton
        '
        Me.KsbProDeliverButton.AutoSize = True
        Me.KsbProDeliverButton.Location = New System.Drawing.Point(3, 29)
        Me.KsbProDeliverButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.KsbProDeliverButton.Name = "KsbProDeliverButton"
        Me.KsbProDeliverButton.Size = New System.Drawing.Size(126, 23)
        Me.KsbProDeliverButton.TabIndex = 8
        Me.KsbProDeliverButton.Text = "配信待ち監プロ全配信"
        Me.UsageTip.SetToolTip(Me.KsbProDeliverButton, "左表で選択中の監視盤において、配信保留中の監視盤プログラムを内部へ配信します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "DummyKanshiban制御用シナリオでは、シナリオを開始したタイミングや" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "監視盤が監視盤プログラムを受信したタイミングにおいて、この処理を自動で呼び出します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "この処理を手動で呼び出す必要があるのは、配信保留の要因を解除したときの" & _
                "みです。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "つまり、現状では、この処理を手動で呼び出す必要はありません。")
        Me.KsbProDeliverButton.UseVisualStyleBackColor = True
        '
        'KsbProApplyButton
        '
        Me.KsbProApplyButton.AutoSize = True
        Me.KsbProApplyButton.Location = New System.Drawing.Point(135, 29)
        Me.KsbProApplyButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.KsbProApplyButton.Name = "KsbProApplyButton"
        Me.KsbProApplyButton.Size = New System.Drawing.Size(126, 23)
        Me.KsbProApplyButton.TabIndex = 9
        Me.KsbProApplyButton.Text = "適用待ち監プロ全適用"
        Me.UsageTip.SetToolTip(Me.KsbProApplyButton, resources.GetString("KsbProApplyButton.ToolTip"))
        Me.KsbProApplyButton.UseVisualStyleBackColor = True
        '
        'KsbConfigSendButton
        '
        Me.KsbConfigSendButton.AutoSize = True
        Me.KsbConfigSendButton.Location = New System.Drawing.Point(267, 29)
        Me.KsbConfigSendButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.KsbConfigSendButton.Name = "KsbConfigSendButton"
        Me.KsbConfigSendButton.Size = New System.Drawing.Size(123, 23)
        Me.KsbConfigSendButton.TabIndex = 10
        Me.KsbConfigSendButton.Text = "監視盤設定情報送信"
        Me.UsageTip.SetToolTip(Me.KsbConfigSendButton, "未実装です。")
        Me.KsbConfigSendButton.UseVisualStyleBackColor = True
        '
        'UpboundDataClearButton
        '
        Me.UpboundDataClearButton.AutoSize = True
        Me.UpboundDataClearButton.Location = New System.Drawing.Point(396, 29)
        Me.UpboundDataClearButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.UpboundDataClearButton.Name = "UpboundDataClearButton"
        Me.UpboundDataClearButton.Size = New System.Drawing.Size(99, 23)
        Me.UpboundDataClearButton.TabIndex = 11
        Me.UpboundDataClearButton.Text = "上りデータクリア"
        Me.UsageTip.SetToolTip(Me.UpboundDataClearButton, "左表で選択中の監視盤とその配下の改札機について、保持している上りデータをクリアします。")
        Me.UpboundDataClearButton.UseVisualStyleBackColor = True
        '
        'RandFaultDataStoreButton
        '
        Me.RandFaultDataStoreButton.AutoSize = True
        Me.RandFaultDataStoreButton.Location = New System.Drawing.Point(501, 29)
        Me.RandFaultDataStoreButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.RandFaultDataStoreButton.Name = "RandFaultDataStoreButton"
        Me.RandFaultDataStoreButton.Size = New System.Drawing.Size(115, 23)
        Me.RandFaultDataStoreButton.TabIndex = 12
        Me.RandFaultDataStoreButton.Text = "RND異常データ蓄積"
        Me.UsageTip.SetToolTip(Me.RandFaultDataStoreButton, "左表で選択中の監視盤とその配下の改札機において、ランダムな異常データを生成し、" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "再収集用に蓄積します。")
        Me.RandFaultDataStoreButton.UseVisualStyleBackColor = True
        '
        'RandFaultDataSendButton
        '
        Me.RandFaultDataSendButton.AutoSize = True
        Me.RandFaultDataSendButton.Location = New System.Drawing.Point(622, 29)
        Me.RandFaultDataSendButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.RandFaultDataSendButton.Name = "RandFaultDataSendButton"
        Me.RandFaultDataSendButton.Size = New System.Drawing.Size(115, 23)
        Me.RandFaultDataSendButton.TabIndex = 13
        Me.RandFaultDataSendButton.Text = "RND異常データ送信"
        Me.UsageTip.SetToolTip(Me.RandFaultDataSendButton, "左表で選択中の監視盤とその配下の改札機において、ランダムな異常データを生成し、" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "運管サーバに即時送信します。")
        Me.RandFaultDataSendButton.UseVisualStyleBackColor = True
        '
        'KadoDataRandUpdateButton
        '
        Me.KadoDataRandUpdateButton.AutoSize = True
        Me.KadoDataRandUpdateButton.Location = New System.Drawing.Point(743, 29)
        Me.KadoDataRandUpdateButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.KadoDataRandUpdateButton.Name = "KadoDataRandUpdateButton"
        Me.KadoDataRandUpdateButton.Size = New System.Drawing.Size(115, 23)
        Me.KadoDataRandUpdateButton.TabIndex = 14
        Me.KadoDataRandUpdateButton.Text = "稼保データRND更新"
        Me.UsageTip.SetToolTip(Me.KadoDataRandUpdateButton, "左表で選択中の監視盤の配下の全改札機について、稼動保守データをランダムに更新し、" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "運管サーバからの収集に備えます。")
        Me.KadoDataRandUpdateButton.UseVisualStyleBackColor = True
        '
        'KadoDataCommitButton
        '
        Me.KadoDataCommitButton.AutoSize = True
        Me.KadoDataCommitButton.Location = New System.Drawing.Point(864, 29)
        Me.KadoDataCommitButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.KadoDataCommitButton.Name = "KadoDataCommitButton"
        Me.KadoDataCommitButton.Size = New System.Drawing.Size(115, 23)
        Me.KadoDataCommitButton.TabIndex = 15
        Me.KadoDataCommitButton.Text = "稼保データ収集完了"
        Me.UsageTip.SetToolTip(Me.KadoDataCommitButton, resources.GetString("KadoDataCommitButton.ToolTip"))
        Me.KadoDataCommitButton.UseVisualStyleBackColor = True
        '
        'SimWorkingDirDialog
        '
        Me.SimWorkingDirDialog.Description = "シミュレータ本体の起動ディレクトリを選択してください。"
        Me.SimWorkingDirDialog.RootFolder = System.Environment.SpecialFolder.MyComputer
        Me.SimWorkingDirDialog.ShowNewFolderButton = False
        '
        'UsageTip
        '
        Me.UsageTip.AutoPopDelay = 20000
        Me.UsageTip.InitialDelay = 500
        Me.UsageTip.IsBalloon = True
        Me.UsageTip.ReshowDelay = 100
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1008, 607)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "MainForm"
        Me.Text = "多重監視盤向け 運用・保守データサーバ"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableLayoutPanelUpper.ResumeLayout(False)
        Me.TableLayoutPanelUpper.PerformLayout()
        Me.TableSplitContainer.Panel1.ResumeLayout(False)
        Me.TableSplitContainer.Panel2.ResumeLayout(False)
        Me.TableSplitContainer.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ViewModePanel.ResumeLayout(False)
        Me.ViewModePanel.PerformLayout()
        Me.TableLayoutPanelLower.ResumeLayout(False)
        Me.TableLayoutPanelLower.PerformLayout()
        CType(Me.LogDispGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.LogDispHeaderPanel.ResumeLayout(False)
        Me.LogDispHeaderPanel.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableSplitContainer As System.Windows.Forms.SplitContainer
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridView2 As JR.ExOpmg.DummyKanshiban.XlsDataGridView
    Friend WithEvents MachineProfileFetchButton As System.Windows.Forms.Button
    Friend WithEvents MasDeliverButton As System.Windows.Forms.Button
    Friend WithEvents MasStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents ConStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents SimWorkingDirDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ProDeliverButton As System.Windows.Forms.Button
    Friend WithEvents ProStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents ProApplyButton As System.Windows.Forms.Button
    Friend WithEvents KsbConfigRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents ConStatusSendButton As System.Windows.Forms.Button
    Friend WithEvents KsbConfigSendButton As System.Windows.Forms.Button
    Friend WithEvents KsbProDeliverButton As System.Windows.Forms.Button
    Friend WithEvents KsbProApplyButton As System.Windows.Forms.Button
    Friend WithEvents KsbProStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents SymbolizeCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents TableLayoutPanelUpper As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ViewModePanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents ProDirectInstallButton As System.Windows.Forms.Button
    Friend WithEvents KsbProDirectInstallButton As System.Windows.Forms.Button
    Friend WithEvents UsageTip As System.Windows.Forms.ToolTip
    Friend WithEvents TableLayoutPanelLower As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LogDispHeaderPanel As System.Windows.Forms.Panel
    Friend WithEvents LogDispFilterEditButton As System.Windows.Forms.Button
    Friend WithEvents LogDispClearButton As System.Windows.Forms.Button
    Friend WithEvents LogDispCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents LogDispFilter As System.Windows.Forms.TextBox
    Friend WithEvents LogDispFilterLabel As System.Windows.Forms.Label
    Friend WithEvents LogDispGrid As System.Windows.Forms.DataGridView
    Friend WithEvents UpboundDataClearButton As System.Windows.Forms.Button
    Friend WithEvents RandFaultDataStoreButton As System.Windows.Forms.Button
    Friend WithEvents RandFaultDataSendButton As System.Windows.Forms.Button
    Friend WithEvents KadoDataRandUpdateButton As System.Windows.Forms.Button
    Friend WithEvents UpboundProcStateRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents MasClearButton As System.Windows.Forms.Button
    Friend WithEvents KadoDataCommitButton As System.Windows.Forms.Button

End Class
