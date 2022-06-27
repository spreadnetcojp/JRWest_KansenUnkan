<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ActiveUllTransferNameForm
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
        Me.CancButton = New System.Windows.Forms.Button()
        Me.OkayButton = New System.Windows.Forms.Button()
        Me.MenuGrid = New JR.ExOpmg.MultiplexEkimuSim.MenuDataGridView()
        Me.MenuGridDispNameColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MenuGridFileNameColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.MenuGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CancButton
        '
        Me.CancButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CancButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancButton.Location = New System.Drawing.Point(307, 465)
        Me.CancButton.Name = "CancButton"
        Me.CancButton.Size = New System.Drawing.Size(73, 28)
        Me.CancButton.TabIndex = 2
        Me.CancButton.Text = "キャンセル"
        Me.CancButton.UseVisualStyleBackColor = True
        '
        'OkayButton
        '
        Me.OkayButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OkayButton.Location = New System.Drawing.Point(228, 465)
        Me.OkayButton.Name = "OkayButton"
        Me.OkayButton.Size = New System.Drawing.Size(73, 28)
        Me.OkayButton.TabIndex = 1
        Me.OkayButton.Text = "OK"
        Me.OkayButton.UseVisualStyleBackColor = True
        '
        'MenuGrid
        '
        Me.MenuGrid.AllowUserToAddRows = False
        Me.MenuGrid.AllowUserToDeleteRows = False
        Me.MenuGrid.AllowUserToResizeColumns = False
        Me.MenuGrid.AllowUserToResizeRows = False
        Me.MenuGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MenuGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.MenuGrid.ColumnHeadersVisible = False
        Me.MenuGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.MenuGridDispNameColumn, Me.MenuGridFileNameColumn})
        Me.MenuGrid.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.MenuGrid.Location = New System.Drawing.Point(0, 0)
        Me.MenuGrid.MultiSelect = False
        Me.MenuGrid.Name = "MenuGrid"
        Me.MenuGrid.ReadOnly = True
        Me.MenuGrid.RowHeadersVisible = False
        Me.MenuGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.MenuGrid.RowTemplate.Height = 21
        Me.MenuGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.MenuGrid.Size = New System.Drawing.Size(393, 458)
        Me.MenuGrid.StandardTab = True
        Me.MenuGrid.TabIndex = 0
        '
        'MenuGridDispNameColumn
        '
        Me.MenuGridDispNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.MenuGridDispNameColumn.HeaderText = "DispName"
        Me.MenuGridDispNameColumn.Name = "MenuGridDispNameColumn"
        Me.MenuGridDispNameColumn.ReadOnly = True
        '
        'MenuGridFileNameColumn
        '
        Me.MenuGridFileNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.MenuGridFileNameColumn.HeaderText = "FileName"
        Me.MenuGridFileNameColumn.Name = "MenuGridFileNameColumn"
        Me.MenuGridFileNameColumn.ReadOnly = True
        '
        'ActiveUllTransferNameForm
        '
        Me.AcceptButton = Me.OkayButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CancButton
        Me.ClientSize = New System.Drawing.Size(393, 499)
        Me.Controls.Add(Me.MenuGrid)
        Me.Controls.Add(Me.CancButton)
        Me.Controls.Add(Me.OkayButton)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ActiveUllTransferNameForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "候補一覧"
        CType(Me.MenuGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CancButton As System.Windows.Forms.Button
    Friend WithEvents OkayButton As System.Windows.Forms.Button
    Friend WithEvents MenuGrid As JR.ExOpmg.MultiplexEkimuSim.MenuDataGridView
    Friend WithEvents MenuGridDispNameColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MenuGridFileNameColumn As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
