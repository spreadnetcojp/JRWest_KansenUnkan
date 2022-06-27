<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMaintenanceToolMenu
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
        Me.btnButton4 = New System.Windows.Forms.Button()
        Me.btnButton3 = New System.Windows.Forms.Button()
        Me.btnButton2 = New System.Windows.Forms.Button()
        Me.btnButton1 = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnButton4
        '
        Me.btnButton4.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton4.Location = New System.Drawing.Point(80, 260)
        Me.btnButton4.Name = "btnButton4"
        Me.btnButton4.Size = New System.Drawing.Size(700, 48)
        Me.btnButton4.TabIndex = 3
        Me.btnButton4.Text = "Button4"
        Me.btnButton4.UseVisualStyleBackColor = False
        '
        'btnButton3
        '
        Me.btnButton3.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton3.Location = New System.Drawing.Point(80, 190)
        Me.btnButton3.Name = "btnButton3"
        Me.btnButton3.Size = New System.Drawing.Size(700, 48)
        Me.btnButton3.TabIndex = 2
        Me.btnButton3.Text = "Button3"
        Me.btnButton3.UseVisualStyleBackColor = False
        '
        'btnButton2
        '
        Me.btnButton2.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton2.Location = New System.Drawing.Point(80, 120)
        Me.btnButton2.Name = "btnButton2"
        Me.btnButton2.Size = New System.Drawing.Size(700, 48)
        Me.btnButton2.TabIndex = 1
        Me.btnButton2.Text = "Button2"
        Me.btnButton2.UseVisualStyleBackColor = False
        '
        'btnButton1
        '
        Me.btnButton1.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton1.Location = New System.Drawing.Point(80, 50)
        Me.btnButton1.Name = "btnButton1"
        Me.btnButton1.Size = New System.Drawing.Size(700, 48)
        Me.btnButton1.TabIndex = 0
        Me.btnButton1.Text = "Button1"
        Me.btnButton1.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.Location = New System.Drawing.Point(723, 476)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(121, 23)
        Me.btnReturn.TabIndex = 4
        Me.btnReturn.Text = "Button3"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'FrmMaintenanceToolMenu
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.ClientSize = New System.Drawing.Size(860, 518)
        Me.Controls.Add(Me.btnReturn)
        Me.Controls.Add(Me.btnButton4)
        Me.Controls.Add(Me.btnButton3)
        Me.Controls.Add(Me.btnButton1)
        Me.Controls.Add(Me.btnButton2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmMaintenanceToolMenu"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "保守ツール"
        Me.ResumeLayout(False)

    End Sub
    Protected WithEvents btnButton1 As System.Windows.Forms.Button
    Protected WithEvents btnButton2 As System.Windows.Forms.Button
    Protected WithEvents btnButton3 As System.Windows.Forms.Button
    Protected WithEvents btnButton4 As System.Windows.Forms.Button
    Protected WithEvents btnReturn As System.Windows.Forms.Button
End Class
