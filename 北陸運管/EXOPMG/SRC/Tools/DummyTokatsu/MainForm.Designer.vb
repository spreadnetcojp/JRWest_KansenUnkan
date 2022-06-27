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
        Me.DataGridView2 = New JR.ExOpmg.DummyTokatsu.XlsDataGridView()
        Me.ViewModePanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.ProStatusRadioButton = New System.Windows.Forms.RadioButton()
        Me.MasStatusRadioButton = New System.Windows.Forms.RadioButton()
        Me.MadoConStatusRadioButton = New System.Windows.Forms.RadioButton()
        Me.TktConStatusRadioButton = New System.Windows.Forms.RadioButton()
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
        Me.MasClearButton = New System.Windows.Forms.Button()
        Me.MasDeliverButton = New System.Windows.Forms.Button()
        Me.MasSweepButton = New System.Windows.Forms.Button()
        Me.ProDirectInstallButton = New System.Windows.Forms.Button()
        Me.ProDeliverButton = New System.Windows.Forms.Button()
        Me.ProApplyButton = New System.Windows.Forms.Button()
        Me.ProSweepButton = New System.Windows.Forms.Button()
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
        Me.SplitContainer1.Size = New System.Drawing.Size(997, 607)
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
        Me.TableLayoutPanelUpper.Size = New System.Drawing.Size(993, 332)
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
        Me.TableSplitContainer.Size = New System.Drawing.Size(987, 304)
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
        Me.DataGridView2.Size = New System.Drawing.Size(595, 300)
        Me.DataGridView2.StandardTab = True
        Me.DataGridView2.TabIndex = 0
        '
        'ViewModePanel
        '
        Me.ViewModePanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ViewModePanel.AutoSize = True
        Me.ViewModePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ViewModePanel.Controls.Add(Me.ProStatusRadioButton)
        Me.ViewModePanel.Controls.Add(Me.MasStatusRadioButton)
        Me.ViewModePanel.Controls.Add(Me.MadoConStatusRadioButton)
        Me.ViewModePanel.Controls.Add(Me.TktConStatusRadioButton)
        Me.ViewModePanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.ViewModePanel.Location = New System.Drawing.Point(103, 0)
        Me.ViewModePanel.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.ViewModePanel.Name = "ViewModePanel"
        Me.ViewModePanel.Size = New System.Drawing.Size(887, 22)
        Me.ViewModePanel.TabIndex = 1
        '
        'ProStatusRadioButton
        '
        Me.ProStatusRadioButton.AutoSize = True
        Me.ProStatusRadioButton.Location = New System.Drawing.Point(759, 3)
        Me.ProStatusRadioButton.Name = "ProStatusRadioButton"
        Me.ProStatusRadioButton.Size = New System.Drawing.Size(125, 16)
        Me.ProStatusRadioButton.TabIndex = 3
        Me.ProStatusRadioButton.Text = "窓プロ配信適用状態"
        Me.ProStatusRadioButton.UseVisualStyleBackColor = True
        '
        'MasStatusRadioButton
        '
        Me.MasStatusRadioButton.AutoSize = True
        Me.MasStatusRadioButton.Location = New System.Drawing.Point(656, 3)
        Me.MasStatusRadioButton.Name = "MasStatusRadioButton"
        Me.MasStatusRadioButton.Size = New System.Drawing.Size(97, 16)
        Me.MasStatusRadioButton.TabIndex = 2
        Me.MasStatusRadioButton.Text = "マスタ配信状態"
        Me.MasStatusRadioButton.UseVisualStyleBackColor = True
        '
        'MadoConStatusRadioButton
        '
        Me.MadoConStatusRadioButton.AutoSize = True
        Me.MadoConStatusRadioButton.Location = New System.Drawing.Point(555, 3)
        Me.MadoConStatusRadioButton.Name = "MadoConStatusRadioButton"
        Me.MadoConStatusRadioButton.Size = New System.Drawing.Size(95, 16)
        Me.MadoConStatusRadioButton.TabIndex = 1
        Me.MadoConStatusRadioButton.Text = "窓処接続状態"
        Me.MadoConStatusRadioButton.UseVisualStyleBackColor = True
        '
        'TktConStatusRadioButton
        '
        Me.TktConStatusRadioButton.AutoSize = True
        Me.TktConStatusRadioButton.Checked = True
        Me.TktConStatusRadioButton.Location = New System.Drawing.Point(454, 3)
        Me.TktConStatusRadioButton.Name = "TktConStatusRadioButton"
        Me.TktConStatusRadioButton.Size = New System.Drawing.Size(95, 16)
        Me.TktConStatusRadioButton.TabIndex = 0
        Me.TktConStatusRadioButton.TabStop = True
        Me.TktConStatusRadioButton.Text = "統括接続状態"
        Me.TktConStatusRadioButton.UseVisualStyleBackColor = True
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
        Me.TableLayoutPanelLower.Size = New System.Drawing.Size(993, 263)
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
        Me.LogDispGrid.Size = New System.Drawing.Size(987, 202)
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
        Me.FlowLayoutPanel1.Controls.Add(Me.MasClearButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.MasDeliverButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.MasSweepButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.ProDirectInstallButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.ProDeliverButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.ProApplyButton)
        Me.FlowLayoutPanel1.Controls.Add(Me.ProSweepButton)
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
        'MasClearButton
        '
        Me.MasClearButton.AutoSize = True
        Me.MasClearButton.Location = New System.Drawing.Point(174, 3)
        Me.MasClearButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MasClearButton.Name = "MasClearButton"
        Me.MasClearButton.Size = New System.Drawing.Size(87, 23)
        Me.MasClearButton.TabIndex = 1
        Me.MasClearButton.Text = "マスタクリア"
        Me.UsageTip.SetToolTip(Me.MasClearButton, "右表で選択中の窓処について、保持しているマスタおよびマスタ適用リストをクリアします。")
        Me.MasClearButton.UseVisualStyleBackColor = True
        '
        'MasDeliverButton
        '
        Me.MasDeliverButton.AutoSize = True
        Me.MasDeliverButton.Location = New System.Drawing.Point(267, 3)
        Me.MasDeliverButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MasDeliverButton.Name = "MasDeliverButton"
        Me.MasDeliverButton.Size = New System.Drawing.Size(122, 23)
        Me.MasDeliverButton.TabIndex = 2
        Me.MasDeliverButton.Text = "配信待ちマスタ全配信"
        Me.UsageTip.SetToolTip(Me.MasDeliverButton, "右表で選択中の窓処へ、統括にて配信保留中の全マスタを配信します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "DummyTokatsu制御用シナリオでは、シナリオを開始したタイミングや" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "統括が窓処マスタ" & _
                "を受信したタイミングにおいて、この処理を自動で呼び出します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "この処理を手動で呼び出す必要があるのは、配信保留の要因を解除したとき、つまり" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "窓処の統括DL系" & _
                "状態を「接続」に戻したときのみです。")
        Me.MasDeliverButton.UseVisualStyleBackColor = True
        '
        'MasSweepButton
        '
        Me.MasSweepButton.AutoSize = True
        Me.MasSweepButton.Location = New System.Drawing.Point(395, 3)
        Me.MasSweepButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MasSweepButton.Name = "MasSweepButton"
        Me.MasSweepButton.Size = New System.Drawing.Size(99, 23)
        Me.MasSweepButton.TabIndex = 3
        Me.MasSweepButton.Text = "マスタ洗い替え"
        Me.UsageTip.SetToolTip(Me.MasSweepButton, "左表で選択中の統括から余分なマスタを消去します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ここで言う余分なマスタとは、配下のどの窓処にも存在しないマスタのことです。")
        Me.MasSweepButton.UseVisualStyleBackColor = True
        '
        'ProDirectInstallButton
        '
        Me.ProDirectInstallButton.AutoSize = True
        Me.ProDirectInstallButton.Location = New System.Drawing.Point(500, 3)
        Me.ProDirectInstallButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ProDirectInstallButton.Name = "ProDirectInstallButton"
        Me.ProDirectInstallButton.Size = New System.Drawing.Size(99, 23)
        Me.ProDirectInstallButton.TabIndex = 4
        Me.ProDirectInstallButton.Text = "窓プロ直接投入"
        Me.UsageTip.SetToolTip(Me.ProDirectInstallButton, "右表で選択中の窓処に対して、プログラム（CABファイル）を直接投入します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "待機面に投入しますので、それを適用面に移すには、窓処の再起動（適用待ち窓プロ全適用）" & _
                "が必要です。")
        Me.ProDirectInstallButton.UseVisualStyleBackColor = True
        '
        'ProDeliverButton
        '
        Me.ProDeliverButton.AutoSize = True
        Me.ProDeliverButton.Location = New System.Drawing.Point(605, 3)
        Me.ProDeliverButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ProDeliverButton.Name = "ProDeliverButton"
        Me.ProDeliverButton.Size = New System.Drawing.Size(126, 23)
        Me.ProDeliverButton.TabIndex = 5
        Me.ProDeliverButton.Text = "配信待ち窓プロ全配信"
        Me.UsageTip.SetToolTip(Me.ProDeliverButton, "右表で選択中の窓処へ、統括にて配信保留中の全窓処プログラムを配信します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "DummyTokatsu制御用シナリオでは、シナリオを開始したタイミングや" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "統括が窓" & _
                "処プログラムを受信したタイミングにおいて、この処理を自動で呼び出します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "この処理を手動で呼び出す必要があるのは、配信保留の要因を解除したとき、つまり" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "窓処" & _
                "の統括DL系状態を「接続」に戻したときのみです。")
        Me.ProDeliverButton.UseVisualStyleBackColor = True
        '
        'ProApplyButton
        '
        Me.ProApplyButton.AutoSize = True
        Me.ProApplyButton.Location = New System.Drawing.Point(737, 3)
        Me.ProApplyButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ProApplyButton.Name = "ProApplyButton"
        Me.ProApplyButton.Size = New System.Drawing.Size(126, 23)
        Me.ProApplyButton.TabIndex = 6
        Me.ProApplyButton.Text = "適用待ち窓プロ全適用"
        Me.UsageTip.SetToolTip(Me.ProApplyButton, resources.GetString("ProApplyButton.ToolTip"))
        Me.ProApplyButton.UseVisualStyleBackColor = True
        '
        'ProSweepButton
        '
        Me.ProSweepButton.AutoSize = True
        Me.ProSweepButton.Location = New System.Drawing.Point(869, 3)
        Me.ProSweepButton.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ProSweepButton.Name = "ProSweepButton"
        Me.ProSweepButton.Size = New System.Drawing.Size(99, 23)
        Me.ProSweepButton.TabIndex = 7
        Me.ProSweepButton.Text = "窓プロ洗い替え"
        Me.UsageTip.SetToolTip(Me.ProSweepButton, "左表で選択中の統括から余分な窓処プログラムを消去します。" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ここで言う余分な窓処プログラムとは、配下のどの窓処にも存在しないプログラムのことです。")
        Me.ProSweepButton.UseVisualStyleBackColor = True
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
        Me.ClientSize = New System.Drawing.Size(997, 607)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "MainForm"
        Me.Text = "多重統括向け 運用データサーバ"
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
    Friend WithEvents DataGridView2 As JR.ExOpmg.DummyTokatsu.XlsDataGridView
    Friend WithEvents MachineProfileFetchButton As System.Windows.Forms.Button
    Friend WithEvents MasDeliverButton As System.Windows.Forms.Button
    Friend WithEvents MasStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents MadoConStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents SimWorkingDirDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ProDeliverButton As System.Windows.Forms.Button
    Friend WithEvents ProStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents ProApplyButton As System.Windows.Forms.Button
    Friend WithEvents SymbolizeCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents TableLayoutPanelUpper As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ViewModePanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents TktConStatusRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents MasSweepButton As System.Windows.Forms.Button
    Friend WithEvents ProSweepButton As System.Windows.Forms.Button
    Friend WithEvents UsageTip As System.Windows.Forms.ToolTip
    Friend WithEvents ProDirectInstallButton As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanelLower As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LogDispHeaderPanel As System.Windows.Forms.Panel
    Friend WithEvents LogDispFilterEditButton As System.Windows.Forms.Button
    Friend WithEvents LogDispClearButton As System.Windows.Forms.Button
    Friend WithEvents LogDispCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents LogDispFilter As System.Windows.Forms.TextBox
    Friend WithEvents LogDispFilterLabel As System.Windows.Forms.Label
    Friend WithEvents LogDispGrid As System.Windows.Forms.DataGridView
    Friend WithEvents MasClearButton As System.Windows.Forms.Button

End Class
