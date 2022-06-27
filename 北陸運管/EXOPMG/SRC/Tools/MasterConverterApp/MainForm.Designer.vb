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
        Me.cmbPattern = New System.Windows.Forms.ComboBox()
        Me.cmbMaster = New System.Windows.Forms.ComboBox()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.btnConvert = New System.Windows.Forms.Button()
        Me.btnFileOpen = New System.Windows.Forms.Button()
        Me.txtPattern = New System.Windows.Forms.TextBox()
        Me.txtVersion = New System.Windows.Forms.TextBox()
        Me.txtFileName = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'cmbPattern
        '
        Me.cmbPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPattern.Enabled = False
        Me.cmbPattern.FormattingEnabled = True
        Me.cmbPattern.Location = New System.Drawing.Point(138, 186)
        Me.cmbPattern.Name = "cmbPattern"
        Me.cmbPattern.Size = New System.Drawing.Size(67, 20)
        Me.cmbPattern.TabIndex = 6
        '
        'cmbMaster
        '
        Me.cmbMaster.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbMaster.Enabled = False
        Me.cmbMaster.FormattingEnabled = True
        Me.cmbMaster.Items.AddRange(New Object() {"ああああああああああああああああああああ", "ああああああああああああああああああああ", "ああああああああああああああああああああ"})
        Me.cmbMaster.Location = New System.Drawing.Point(138, 116)
        Me.cmbMaster.Name = "cmbMaster"
        Me.cmbMaster.Size = New System.Drawing.Size(305, 20)
        Me.cmbMaster.TabIndex = 4
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Enabled = False
        Me.cmbModel.FormattingEnabled = True
        Me.cmbModel.Location = New System.Drawing.Point(138, 81)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(125, 20)
        Me.cmbModel.TabIndex = 3
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(461, 209)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(84, 34)
        Me.btnExit.TabIndex = 8
        Me.btnExit.Text = "終　了"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'btnConvert
        '
        Me.btnConvert.Enabled = False
        Me.btnConvert.Location = New System.Drawing.Point(461, 164)
        Me.btnConvert.Name = "btnConvert"
        Me.btnConvert.Size = New System.Drawing.Size(84, 34)
        Me.btnConvert.TabIndex = 7
        Me.btnConvert.Text = "変換出力"
        Me.btnConvert.UseVisualStyleBackColor = True
        '
        'btnFileOpen
        '
        Me.btnFileOpen.Location = New System.Drawing.Point(461, 26)
        Me.btnFileOpen.Name = "btnFileOpen"
        Me.btnFileOpen.Size = New System.Drawing.Size(84, 34)
        Me.btnFileOpen.TabIndex = 2
        Me.btnFileOpen.Text = "参　照"
        Me.btnFileOpen.UseVisualStyleBackColor = True
        '
        'txtPattern
        '
        Me.txtPattern.Location = New System.Drawing.Point(138, 222)
        Me.txtPattern.Name = "txtPattern"
        Me.txtPattern.ReadOnly = True
        Me.txtPattern.Size = New System.Drawing.Size(188, 19)
        Me.txtPattern.TabIndex = 32
        Me.txtPattern.TabStop = False
        '
        'txtVersion
        '
        Me.txtVersion.Enabled = False
        Me.txtVersion.Location = New System.Drawing.Point(138, 152)
        Me.txtVersion.MaxLength = 3
        Me.txtVersion.Name = "txtVersion"
        Me.txtVersion.Size = New System.Drawing.Size(58, 19)
        Me.txtVersion.TabIndex = 5
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(138, 33)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(305, 19)
        Me.txtFileName.TabIndex = 1
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(31, 225)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(66, 12)
        Me.Label6.TabIndex = 29
        Me.Label6.Text = "パターン名称"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(31, 190)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 12)
        Me.Label5.TabIndex = 28
        Me.Label5.Text = "パターンNo"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(31, 155)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(50, 12)
        Me.Label4.TabIndex = 27
        Me.Label4.Text = "バージョン"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(31, 120)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 12)
        Me.Label3.TabIndex = 26
        Me.Label3.Text = "マスタデータ名称"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(31, 85)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 12)
        Me.Label2.TabIndex = 25
        Me.Label2.Text = "機種名称"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(31, 37)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(75, 12)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "変換元ファイル"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(563, 275)
        Me.Controls.Add(Me.cmbPattern)
        Me.Controls.Add(Me.cmbMaster)
        Me.Controls.Add(Me.cmbModel)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.btnConvert)
        Me.Controls.Add(Me.btnFileOpen)
        Me.Controls.Add(Me.txtPattern)
        Me.Controls.Add(Me.txtVersion)
        Me.Controls.Add(Me.txtFileName)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "駅務機器マスタ変換"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmbPattern As System.Windows.Forms.ComboBox
    Friend WithEvents cmbMaster As System.Windows.Forms.ComboBox
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents btnConvert As System.Windows.Forms.Button
    Friend WithEvents btnFileOpen As System.Windows.Forms.Button
    Friend WithEvents txtPattern As System.Windows.Forms.TextBox
    Friend WithEvents txtVersion As System.Windows.Forms.TextBox
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
