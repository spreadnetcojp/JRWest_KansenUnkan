<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ApplicableListForm
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
        Me.ListContentTextBox = New System.Windows.Forms.TextBox()
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
        Me.ListVersionPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.ListVersionLabel = New System.Windows.Forms.Label()
        Me.ListVersionTextBox = New System.Windows.Forms.TextBox()
        Me.ListAcceptDatePanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.ListAcceptDateLabel = New System.Windows.Forms.Label()
        Me.ListAcceptDateTextBox = New System.Windows.Forms.TextBox()
        Me.ListHashValuePanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.ListHashValueLabel = New System.Windows.Forms.Label()
        Me.ListHashValueTextBox = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.MachineIdPanel.SuspendLayout()
        Me.DataKindPanel.SuspendLayout()
        Me.DataSubKindPanel.SuspendLayout()
        Me.DataVersionPanel.SuspendLayout()
        Me.ListVersionPanel.SuspendLayout()
        Me.ListAcceptDatePanel.SuspendLayout()
        Me.ListHashValuePanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'ListContentTextBox
        '
        Me.ListContentTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.ListContentTextBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListContentTextBox.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ListContentTextBox.Location = New System.Drawing.Point(3, 61)
        Me.ListContentTextBox.Multiline = True
        Me.ListContentTextBox.Name = "ListContentTextBox"
        Me.ListContentTextBox.ReadOnly = True
        Me.ListContentTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.ListContentTextBox.Size = New System.Drawing.Size(697, 387)
        Me.ListContentTextBox.TabIndex = 1
        Me.ListContentTextBox.TabStop = False
        Me.ListContentTextBox.WordWrap = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.FlowLayoutPanel1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ListContentTextBox, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(703, 451)
        Me.TableLayoutPanel1.TabIndex = 11
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
        Me.FlowLayoutPanel1.Controls.Add(Me.ListVersionPanel)
        Me.FlowLayoutPanel1.Controls.Add(Me.ListAcceptDatePanel)
        Me.FlowLayoutPanel1.Controls.Add(Me.ListHashValuePanel)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(697, 52)
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
        'ListVersionPanel
        '
        Me.ListVersionPanel.AutoSize = True
        Me.ListVersionPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ListVersionPanel.Controls.Add(Me.ListVersionLabel)
        Me.ListVersionPanel.Controls.Add(Me.ListVersionTextBox)
        Me.ListVersionPanel.Location = New System.Drawing.Point(559, 3)
        Me.ListVersionPanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.ListVersionPanel.Name = "ListVersionPanel"
        Me.ListVersionPanel.Size = New System.Drawing.Size(99, 20)
        Me.ListVersionPanel.TabIndex = 9
        '
        'ListVersionLabel
        '
        Me.ListVersionLabel.AutoSize = True
        Me.ListVersionLabel.Location = New System.Drawing.Point(3, 3)
        Me.ListVersionLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ListVersionLabel.Name = "ListVersionLabel"
        Me.ListVersionLabel.Size = New System.Drawing.Size(47, 12)
        Me.ListVersionLabel.TabIndex = 0
        Me.ListVersionLabel.Text = "リストVer"
        '
        'ListVersionTextBox
        '
        Me.ListVersionTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.ListVersionTextBox.Location = New System.Drawing.Point(56, 0)
        Me.ListVersionTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.ListVersionTextBox.Name = "ListVersionTextBox"
        Me.ListVersionTextBox.ReadOnly = True
        Me.ListVersionTextBox.Size = New System.Drawing.Size(40, 19)
        Me.ListVersionTextBox.TabIndex = 1
        Me.ListVersionTextBox.TabStop = False
        Me.ListVersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ListAcceptDatePanel
        '
        Me.ListAcceptDatePanel.AutoSize = True
        Me.ListAcceptDatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ListAcceptDatePanel.Controls.Add(Me.ListAcceptDateLabel)
        Me.ListAcceptDatePanel.Controls.Add(Me.ListAcceptDateTextBox)
        Me.ListAcceptDatePanel.Location = New System.Drawing.Point(3, 29)
        Me.ListAcceptDatePanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.ListAcceptDatePanel.Name = "ListAcceptDatePanel"
        Me.ListAcceptDatePanel.Size = New System.Drawing.Size(254, 20)
        Me.ListAcceptDatePanel.TabIndex = 11
        '
        'ListAcceptDateLabel
        '
        Me.ListAcceptDateLabel.AutoSize = True
        Me.ListAcceptDateLabel.Location = New System.Drawing.Point(3, 3)
        Me.ListAcceptDateLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ListAcceptDateLabel.Name = "ListAcceptDateLabel"
        Me.ListAcceptDateLabel.Size = New System.Drawing.Size(101, 12)
        Me.ListAcceptDateLabel.TabIndex = 0
        Me.ListAcceptDateLabel.Text = "中継機器受信日時"
        '
        'ListAcceptDateTextBox
        '
        Me.ListAcceptDateTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.ListAcceptDateTextBox.Location = New System.Drawing.Point(110, 0)
        Me.ListAcceptDateTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.ListAcceptDateTextBox.Name = "ListAcceptDateTextBox"
        Me.ListAcceptDateTextBox.ReadOnly = True
        Me.ListAcceptDateTextBox.Size = New System.Drawing.Size(141, 19)
        Me.ListAcceptDateTextBox.TabIndex = 1
        Me.ListAcceptDateTextBox.TabStop = False
        '
        'ListHashValuePanel
        '
        Me.ListHashValuePanel.AutoSize = True
        Me.ListHashValuePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ListHashValuePanel.Controls.Add(Me.ListHashValueLabel)
        Me.ListHashValuePanel.Controls.Add(Me.ListHashValueTextBox)
        Me.ListHashValuePanel.Location = New System.Drawing.Point(269, 29)
        Me.ListHashValuePanel.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        Me.ListHashValuePanel.Name = "ListHashValuePanel"
        Me.ListHashValuePanel.Size = New System.Drawing.Size(338, 20)
        Me.ListHashValuePanel.TabIndex = 10
        '
        'ListHashValueLabel
        '
        Me.ListHashValueLabel.AutoSize = True
        Me.ListHashValueLabel.Location = New System.Drawing.Point(3, 3)
        Me.ListHashValueLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.ListHashValueLabel.Name = "ListHashValueLabel"
        Me.ListHashValueLabel.Size = New System.Drawing.Size(52, 12)
        Me.ListHashValueLabel.TabIndex = 0
        Me.ListHashValueLabel.Text = "ハッシュ値"
        '
        'ListHashValueTextBox
        '
        Me.ListHashValueTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.ListHashValueTextBox.Location = New System.Drawing.Point(61, 0)
        Me.ListHashValueTextBox.Margin = New System.Windows.Forms.Padding(3, 0, 3, 1)
        Me.ListHashValueTextBox.Name = "ListHashValueTextBox"
        Me.ListHashValueTextBox.ReadOnly = True
        Me.ListHashValueTextBox.Size = New System.Drawing.Size(274, 19)
        Me.ListHashValueTextBox.TabIndex = 1
        Me.ListHashValueTextBox.TabStop = False
        '
        'ApplicableListForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(703, 451)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "ApplicableListForm"
        Me.Text = "ApplicableListForm"
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
        Me.ListVersionPanel.ResumeLayout(False)
        Me.ListVersionPanel.PerformLayout()
        Me.ListAcceptDatePanel.ResumeLayout(False)
        Me.ListAcceptDatePanel.PerformLayout()
        Me.ListHashValuePanel.ResumeLayout(False)
        Me.ListHashValuePanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ListContentTextBox As System.Windows.Forms.TextBox
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
    Friend WithEvents ListHashValueLabel As System.Windows.Forms.Label
    Friend WithEvents ListHashValueTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ListVersionPanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents ListVersionLabel As System.Windows.Forms.Label
    Friend WithEvents ListVersionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ListHashValuePanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents ListAcceptDatePanel As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents ListAcceptDateLabel As System.Windows.Forms.Label
    Friend WithEvents ListAcceptDateTextBox As System.Windows.Forms.TextBox
End Class
