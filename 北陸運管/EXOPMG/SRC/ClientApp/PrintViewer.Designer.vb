<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintViewer
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            viewerControl2.Clear()
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.viewerControl2 = New AdvanceSoftware.VBReport7.ViewerControl()
        Me.buttonClose = New System.Windows.Forms.Button()
        Me.splitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.splitContainer1.Panel1.SuspendLayout()
        Me.splitContainer1.Panel2.SuspendLayout()
        Me.splitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'viewerControl2
        '
        Me.viewerControl2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.viewerControl2.Location = New System.Drawing.Point(0, 0)
        Me.viewerControl2.MinimumSize = New System.Drawing.Size(200, 100)
        Me.viewerControl2.Name = "viewerControl2"
        Me.viewerControl2.Size = New System.Drawing.Size(768, 523)
        Me.viewerControl2.TabIndex = 0
        Me.viewerControl2.UseTwoExcelTasks = True
        '
        'buttonClose
        '
        Me.buttonClose.Location = New System.Drawing.Point(3, 3)
        Me.buttonClose.Name = "buttonClose"
        Me.buttonClose.Size = New System.Drawing.Size(100, 22)
        Me.buttonClose.TabIndex = 1
        Me.buttonClose.Text = "閉じる"
        Me.buttonClose.UseVisualStyleBackColor = True
        '
        'splitContainer1
        '
        Me.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.splitContainer1.Name = "splitContainer1"
        Me.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitContainer1.Panel1
        '
        Me.splitContainer1.Panel1.Controls.Add(Me.buttonClose)
        '
        'splitContainer1.Panel2
        '
        Me.splitContainer1.Panel2.Controls.Add(Me.viewerControl2)
        Me.splitContainer1.Size = New System.Drawing.Size(768, 552)
        Me.splitContainer1.SplitterDistance = 25
        Me.splitContainer1.TabIndex = 3
        '
        'PrintViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(768, 552)
        Me.Controls.Add(Me.splitContainer1)
        Me.Name = "PrintViewer"
        Me.Text = "帳票の表示"
        Me.splitContainer1.Panel1.ResumeLayout(False)
        Me.splitContainer1.Panel2.ResumeLayout(False)
        Me.splitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents buttonClose As System.Windows.Forms.Button
    Private WithEvents splitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents viewerControl2 As AdvanceSoftware.VBReport7.ViewerControl

    Public Sub New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

    End Sub
End Class
