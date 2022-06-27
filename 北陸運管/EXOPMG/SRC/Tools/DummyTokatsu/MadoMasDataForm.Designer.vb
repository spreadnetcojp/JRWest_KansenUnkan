<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MadoMasDataForm
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.MachineIdPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.MachineIdLabel = New System.Windows.Forms.Label()
        Me.MachineIdTextBox = New System.Windows.Forms.TextBox()
        Me.DataKindPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.DataKindLabel = New System.Windows.Forms.Label()
        Me.DataKindTextBox = New System.Windows.Forms.TextBox()
        Me.DataSubKindPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.DataSubKindLabel = New System.Windows.Forms.Label()
        Me.DataSubKindTextBox = New System.Windows.Forms.TextBox()
        Me.DataVersionPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.DataVersionLabel = New System.Windows.Forms.Label()
        Me.DataVersionTextBox = New System.Windows.Forms.TextBox()
        Me.DataAcceptDatePanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.DataAcceptDateLabel = New System.Windows.Forms.Label()
        Me.DataAcceptDateTextBox = New System.Windows.Forms.TextBox()
        Me.DataHashValuePanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.DataHashValueLabel = New System.Windows.Forms.Label()
        Me.DataHashValueTextBox = New System.Windows.Forms.TextBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.FileFooterViewPage = New System.Windows.Forms.TabPage()
        Me.FooterDataGridView = New System.Windows.Forms.DataGridView()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.MachineIdPanel.SuspendLayout()
        Me.DataKindPanel.SuspendLayout()
        Me.DataSubKindPanel.SuspendLayout()
        Me.DataVersionPanel.SuspendLayout()
        Me.DataAcceptDatePanel.SuspendLayout()
        Me.DataHashValuePanel.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.FileFooterViewPage.SuspendLayout()
        CType(Me.FooterDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.FlowLayoutPanel1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TabControl1, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(913, 498)
        Me.TableLayoutPanel1.TabIndex = 12
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.MachineIdPanel)
        Me.FlowLayoutPanel1.Controls.Add(Me.DataKindPanel)
        Me.FlowLayoutPanel1.Controls.Add(Me.DataSubKindPanel)
        Me.FlowLayoutPanel1.Controls.Add(Me.DataVersionPanel)
        Me.FlowLayoutPanel1.Controls.Add(Me.DataAcceptDatePanel)
        Me.FlowLayoutPanel1.Controls.Add(Me.DataHashValuePanel)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(907, 52)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'MachineIdPanel
        '
        Me.MachineIdPanel.AutoSize = True
        Me.MachineIdPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.MachineIdPanel.Controls.Add(Me.MachineIdLabel)
        Me.MachineIdPanel.Controls.Add(Me.MachineIdTextBox)
        Me.MachineIdPanel.Location = New System.Drawing.Point(3, 3)
        Me.MachineIdPanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.MachineIdPanel.Name = "MachineIdPanel"
        Me.MachineIdPanel.Size = New System.Drawing.Size(188, 20)
        Me.MachineIdPanel.TabIndex = 5
        '
        'MachineIdLabel
        '
        Me.MachineIdLabel.AutoSize = True
        Me.MachineIdLabel.Location = New System.Drawing.Point(3, 3)
        Me.MachineIdLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.MachineIdLabel.Name = "MachineIdLabel"
        Me.MachineIdLabel.Size = New System.Drawing.Size(64, 12)
        Me.MachineIdLabel.TabIndex = 0
        Me.MachineIdLabel.Text = "中継機器ID"
        '
        'MachineIdTextBox
        '
        Me.MachineIdTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.MachineIdTextBox.Location = New System.Drawing.Point(73, 0)
        Me.MachineIdTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.MachineIdTextBox.Name = "MachineIdTextBox"
        Me.MachineIdTextBox.ReadOnly = True
        Me.MachineIdTextBox.Size = New System.Drawing.Size(112, 19)
        Me.MachineIdTextBox.TabIndex = 1
        Me.MachineIdTextBox.TabStop = False
        '
        'DataKindPanel
        '
        Me.DataKindPanel.AutoSize = True
        Me.DataKindPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.DataKindPanel.Controls.Add(Me.DataKindLabel)
        Me.DataKindPanel.Controls.Add(Me.DataKindTextBox)
        Me.DataKindPanel.Location = New System.Drawing.Point(203, 3)
        Me.DataKindPanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.DataKindPanel.Name = "DataKindPanel"
        Me.DataKindPanel.Size = New System.Drawing.Size(109, 20)
        Me.DataKindPanel.TabIndex = 6
        '
        'DataKindLabel
        '
        Me.DataKindLabel.AutoSize = True
        Me.DataKindLabel.Location = New System.Drawing.Point(3, 3)
        Me.DataKindLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.DataKindLabel.Name = "DataKindLabel"
        Me.DataKindLabel.Size = New System.Drawing.Size(57, 12)
        Me.DataKindLabel.TabIndex = 0
        Me.DataKindLabel.Text = "データ種別"
        '
        'DataKindTextBox
        '
        Me.DataKindTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.DataKindTextBox.Location = New System.Drawing.Point(66, 0)
        Me.DataKindTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.DataKindTextBox.Name = "DataKindTextBox"
        Me.DataKindTextBox.ReadOnly = True
        Me.DataKindTextBox.Size = New System.Drawing.Size(40, 19)
        Me.DataKindTextBox.TabIndex = 1
        Me.DataKindTextBox.TabStop = False
        '
        'DataSubKindPanel
        '
        Me.DataSubKindPanel.AutoSize = True
        Me.DataSubKindPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.DataSubKindPanel.Controls.Add(Me.DataSubKindLabel)
        Me.DataSubKindPanel.Controls.Add(Me.DataSubKindTextBox)
        Me.DataSubKindPanel.Location = New System.Drawing.Point(324, 3)
        Me.DataSubKindPanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.DataSubKindPanel.Name = "DataSubKindPanel"
        Me.DataSubKindPanel.Size = New System.Drawing.Size(96, 20)
        Me.DataSubKindPanel.TabIndex = 7
        '
        'DataSubKindLabel
        '
        Me.DataSubKindLabel.AutoSize = True
        Me.DataSubKindLabel.Location = New System.Drawing.Point(3, 3)
        Me.DataSubKindLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.DataSubKindLabel.Name = "DataSubKindLabel"
        Me.DataSubKindLabel.Size = New System.Drawing.Size(44, 12)
        Me.DataSubKindLabel.TabIndex = 0
        Me.DataSubKindLabel.Text = "エリアNo"
        '
        'DataSubKindTextBox
        '
        Me.DataSubKindTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.DataSubKindTextBox.Location = New System.Drawing.Point(53, 0)
        Me.DataSubKindTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.DataSubKindTextBox.Name = "DataSubKindTextBox"
        Me.DataSubKindTextBox.ReadOnly = True
        Me.DataSubKindTextBox.Size = New System.Drawing.Size(40, 19)
        Me.DataSubKindTextBox.TabIndex = 1
        Me.DataSubKindTextBox.TabStop = False
        Me.DataSubKindTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'DataVersionPanel
        '
        Me.DataVersionPanel.AutoSize = True
        Me.DataVersionPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.DataVersionPanel.Controls.Add(Me.DataVersionLabel)
        Me.DataVersionPanel.Controls.Add(Me.DataVersionTextBox)
        Me.DataVersionPanel.Location = New System.Drawing.Point(432, 3)
        Me.DataVersionPanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.DataVersionPanel.Name = "DataVersionPanel"
        Me.DataVersionPanel.Size = New System.Drawing.Size(115, 20)
        Me.DataVersionPanel.TabIndex = 8
        '
        'DataVersionLabel
        '
        Me.DataVersionLabel.AutoSize = True
        Me.DataVersionLabel.Location = New System.Drawing.Point(3, 3)
        Me.DataVersionLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.DataVersionLabel.Name = "DataVersionLabel"
        Me.DataVersionLabel.Size = New System.Drawing.Size(47, 12)
        Me.DataVersionLabel.TabIndex = 0
        Me.DataVersionLabel.Text = "代表Ver"
        '
        'DataVersionTextBox
        '
        Me.DataVersionTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.DataVersionTextBox.Location = New System.Drawing.Point(56, 0)
        Me.DataVersionTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.DataVersionTextBox.Name = "DataVersionTextBox"
        Me.DataVersionTextBox.ReadOnly = True
        Me.DataVersionTextBox.Size = New System.Drawing.Size(56, 19)
        Me.DataVersionTextBox.TabIndex = 1
        Me.DataVersionTextBox.TabStop = False
        Me.DataVersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'DataAcceptDatePanel
        '
        Me.DataAcceptDatePanel.AutoSize = True
        Me.DataAcceptDatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.DataAcceptDatePanel.Controls.Add(Me.DataAcceptDateLabel)
        Me.DataAcceptDatePanel.Controls.Add(Me.DataAcceptDateTextBox)
        Me.DataAcceptDatePanel.Location = New System.Drawing.Point(559, 3)
        Me.DataAcceptDatePanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.DataAcceptDatePanel.Name = "DataAcceptDatePanel"
        Me.DataAcceptDatePanel.Size = New System.Drawing.Size(254, 20)
        Me.DataAcceptDatePanel.TabIndex = 11
        '
        'DataAcceptDateLabel
        '
        Me.DataAcceptDateLabel.AutoSize = True
        Me.DataAcceptDateLabel.Location = New System.Drawing.Point(3, 3)
        Me.DataAcceptDateLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.DataAcceptDateLabel.Name = "DataAcceptDateLabel"
        Me.DataAcceptDateLabel.Size = New System.Drawing.Size(101, 12)
        Me.DataAcceptDateLabel.TabIndex = 0
        Me.DataAcceptDateLabel.Text = "中継機器受信日時"
        '
        'DataAcceptDateTextBox
        '
        Me.DataAcceptDateTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.DataAcceptDateTextBox.Location = New System.Drawing.Point(110, 0)
        Me.DataAcceptDateTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.DataAcceptDateTextBox.Name = "DataAcceptDateTextBox"
        Me.DataAcceptDateTextBox.ReadOnly = True
        Me.DataAcceptDateTextBox.Size = New System.Drawing.Size(141, 19)
        Me.DataAcceptDateTextBox.TabIndex = 1
        Me.DataAcceptDateTextBox.TabStop = False
        '
        'DataHashValuePanel
        '
        Me.DataHashValuePanel.AutoSize = True
        Me.DataHashValuePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.DataHashValuePanel.Controls.Add(Me.DataHashValueLabel)
        Me.DataHashValuePanel.Controls.Add(Me.DataHashValueTextBox)
        Me.DataHashValuePanel.Location = New System.Drawing.Point(3, 29)
        Me.DataHashValuePanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.DataHashValuePanel.Name = "DataHashValuePanel"
        Me.DataHashValuePanel.Size = New System.Drawing.Size(338, 20)
        Me.DataHashValuePanel.TabIndex = 10
        '
        'DataHashValueLabel
        '
        Me.DataHashValueLabel.AutoSize = True
        Me.DataHashValueLabel.Location = New System.Drawing.Point(3, 3)
        Me.DataHashValueLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.DataHashValueLabel.Name = "DataHashValueLabel"
        Me.DataHashValueLabel.Size = New System.Drawing.Size(52, 12)
        Me.DataHashValueLabel.TabIndex = 0
        Me.DataHashValueLabel.Text = "ハッシュ値"
        '
        'DataHashValueTextBox
        '
        Me.DataHashValueTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.DataHashValueTextBox.Location = New System.Drawing.Point(61, 0)
        Me.DataHashValueTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.DataHashValueTextBox.Name = "DataHashValueTextBox"
        Me.DataHashValueTextBox.ReadOnly = True
        Me.DataHashValueTextBox.Size = New System.Drawing.Size(274, 19)
        Me.DataHashValueTextBox.TabIndex = 1
        Me.DataHashValueTextBox.TabStop = False
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.FileFooterViewPage)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(3, 61)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(907, 434)
        Me.TabControl1.TabIndex = 1
        '
        'FileFooterViewPage
        '
        Me.FileFooterViewPage.Controls.Add(Me.FooterDataGridView)
        Me.FileFooterViewPage.Location = New System.Drawing.Point(4, 22)
        Me.FileFooterViewPage.Name = "FileFooterViewPage"
        Me.FileFooterViewPage.Padding = New System.Windows.Forms.Padding(3)
        Me.FileFooterViewPage.Size = New System.Drawing.Size(899, 408)
        Me.FileFooterViewPage.TabIndex = 0
        Me.FileFooterViewPage.Text = "フッタ情報"
        Me.FileFooterViewPage.UseVisualStyleBackColor = True
        '
        'FooterDataGridView
        '
        Me.FooterDataGridView.AllowUserToAddRows = False
        Me.FooterDataGridView.AllowUserToDeleteRows = False
        Me.FooterDataGridView.AllowUserToOrderColumns = True
        Me.FooterDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.FooterDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.FooterDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.FooterDataGridView.DefaultCellStyle = DataGridViewCellStyle2
        Me.FooterDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FooterDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.FooterDataGridView.Name = "FooterDataGridView"
        Me.FooterDataGridView.ReadOnly = True
        Me.FooterDataGridView.RowHeadersVisible = False
        Me.FooterDataGridView.RowTemplate.Height = 21
        Me.FooterDataGridView.Size = New System.Drawing.Size(893, 402)
        Me.FooterDataGridView.StandardTab = True
        Me.FooterDataGridView.TabIndex = 1
        '
        'MadoMasDataForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(913, 498)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "MadoMasDataForm"
        Me.Text = "MadoMasDataForm"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.MachineIdPanel.ResumeLayout(False)
        Me.MachineIdPanel.PerformLayout()
        Me.DataKindPanel.ResumeLayout(False)
        Me.DataKindPanel.PerformLayout()
        Me.DataSubKindPanel.ResumeLayout(False)
        Me.DataSubKindPanel.PerformLayout()
        Me.DataVersionPanel.ResumeLayout(False)
        Me.DataVersionPanel.PerformLayout()
        Me.DataAcceptDatePanel.ResumeLayout(False)
        Me.DataAcceptDatePanel.PerformLayout()
        Me.DataHashValuePanel.ResumeLayout(False)
        Me.DataHashValuePanel.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.FileFooterViewPage.ResumeLayout(False)
        CType(Me.FooterDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents MachineIdPanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents MachineIdLabel As System.Windows.Forms.Label
    Friend WithEvents MachineIdTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DataKindPanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents DataKindLabel As System.Windows.Forms.Label
    Friend WithEvents DataKindTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DataSubKindPanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents DataSubKindLabel As System.Windows.Forms.Label
    Friend WithEvents DataSubKindTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DataVersionPanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents DataVersionLabel As System.Windows.Forms.Label
    Friend WithEvents DataVersionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DataAcceptDatePanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents DataAcceptDateLabel As System.Windows.Forms.Label
    Friend WithEvents DataAcceptDateTextBox As System.Windows.Forms.TextBox
    Friend WithEvents DataHashValuePanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents DataHashValueLabel As System.Windows.Forms.Label
    Friend WithEvents DataHashValueTextBox As System.Windows.Forms.TextBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents FileFooterViewPage As System.Windows.Forms.TabPage
    Friend WithEvents FooterDataGridView As System.Windows.Forms.DataGridView
End Class
