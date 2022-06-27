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
        Me.OriginPathTextBox = New System.Windows.Forms.TextBox()
        Me.ReplicantsListPathTextBox = New System.Windows.Forms.TextBox()
        Me.ExecButton = New System.Windows.Forms.Button()
        Me.ExePathTextBox = New System.Windows.Forms.TextBox()
        Me.ExePathLabel = New System.Windows.Forms.Label()
        Me.OriginalPathLabel = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'OriginPathTextBox
        '
        Me.OriginPathTextBox.Location = New System.Drawing.Point(117, 71)
        Me.OriginPathTextBox.Name = "OriginPathTextBox"
        Me.OriginPathTextBox.Size = New System.Drawing.Size(422, 19)
        Me.OriginPathTextBox.TabIndex = 0
        '
        'ReplicantsListPathTextBox
        '
        Me.ReplicantsListPathTextBox.Location = New System.Drawing.Point(117, 118)
        Me.ReplicantsListPathTextBox.Name = "ReplicantsListPathTextBox"
        Me.ReplicantsListPathTextBox.Size = New System.Drawing.Size(422, 19)
        Me.ReplicantsListPathTextBox.TabIndex = 1
        '
        'ExecButton
        '
        Me.ExecButton.Location = New System.Drawing.Point(458, 171)
        Me.ExecButton.Name = "ExecButton"
        Me.ExecButton.Size = New System.Drawing.Size(81, 28)
        Me.ExecButton.TabIndex = 2
        Me.ExecButton.Text = "Go!"
        Me.ExecButton.UseVisualStyleBackColor = True
        '
        'ExePathTextBox
        '
        Me.ExePathTextBox.Location = New System.Drawing.Point(117, 26)
        Me.ExePathTextBox.Name = "ExePathTextBox"
        Me.ExePathTextBox.Size = New System.Drawing.Size(422, 19)
        Me.ExePathTextBox.TabIndex = 3
        Me.ExePathTextBox.Text = "C:\EXOPMG\OBJ\Debug\ExOpmgEkimuSim.exe"
        '
        'ExePathLabel
        '
        Me.ExePathLabel.AutoSize = True
        Me.ExePathLabel.Location = New System.Drawing.Point(14, 29)
        Me.ExePathLabel.Name = "ExePathLabel"
        Me.ExePathLabel.Size = New System.Drawing.Size(57, 12)
        Me.ExePathLabel.TabIndex = 4
        Me.ExePathLabel.Text = "exeファイル"
        '
        'OriginalPathLabel
        '
        Me.OriginalPathLabel.AutoSize = True
        Me.OriginalPathLabel.Location = New System.Drawing.Point(14, 74)
        Me.OriginalPathLabel.Name = "OriginalPathLabel"
        Me.OriginalPathLabel.Size = New System.Drawing.Size(78, 12)
        Me.OriginalPathLabel.TabIndex = 5
        Me.OriginalPathLabel.Text = "原本ディレクトリ"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 121)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 12)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "複製名一覧ファイル"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(577, 215)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.OriginalPathLabel)
        Me.Controls.Add(Me.ExePathLabel)
        Me.Controls.Add(Me.ExePathTextBox)
        Me.Controls.Add(Me.ExecButton)
        Me.Controls.Add(Me.ReplicantsListPathTextBox)
        Me.Controls.Add(Me.OriginPathTextBox)
        Me.Name = "Form1"
        Me.Text = "EkimuSim環境複製"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OriginPathTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ReplicantsListPathTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ExecButton As System.Windows.Forms.Button
    Friend WithEvents ExePathTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ExePathLabel As System.Windows.Forms.Label
    Friend WithEvents OriginalPathLabel As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label

End Class
