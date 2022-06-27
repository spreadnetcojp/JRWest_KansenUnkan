' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>ベースメニューフォーム</summary>
''' <remarks></remarks>

Public Class FrmBaseMenu
    Inherits FrmBase

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

    ' Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使って変更してください。  
    ' コード エディタを使って変更しないでください。
    Protected WithEvents btnButton13 As System.Windows.Forms.Button
    Protected WithEvents btnReturn As System.Windows.Forms.Button
    Protected WithEvents btnButton12 As System.Windows.Forms.Button
    Protected WithEvents btnButton11 As System.Windows.Forms.Button
    Protected WithEvents btnButton10 As System.Windows.Forms.Button
    Protected WithEvents btnButton9 As System.Windows.Forms.Button
    Protected WithEvents btnButton8 As System.Windows.Forms.Button
    Protected WithEvents btnButton7 As System.Windows.Forms.Button
    Protected WithEvents btnButton6 As System.Windows.Forms.Button
    Protected WithEvents btnButton5 As System.Windows.Forms.Button
    Protected WithEvents btnButton4 As System.Windows.Forms.Button
    Protected WithEvents btnButton3 As System.Windows.Forms.Button
    Protected WithEvents btnButton2 As System.Windows.Forms.Button
    Protected WithEvents btnButton1 As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnButton13 = New System.Windows.Forms.Button
        Me.btnReturn = New System.Windows.Forms.Button
        Me.btnButton12 = New System.Windows.Forms.Button
        Me.btnButton11 = New System.Windows.Forms.Button
        Me.btnButton10 = New System.Windows.Forms.Button
        Me.btnButton9 = New System.Windows.Forms.Button
        Me.btnButton8 = New System.Windows.Forms.Button
        Me.btnButton7 = New System.Windows.Forms.Button
        Me.btnButton6 = New System.Windows.Forms.Button
        Me.btnButton5 = New System.Windows.Forms.Button
        Me.btnButton4 = New System.Windows.Forms.Button
        Me.btnButton3 = New System.Windows.Forms.Button
        Me.btnButton2 = New System.Windows.Forms.Button
        Me.btnButton1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2006/08/10(木)  10:04"
        '
        'btnButton13
        '
        Me.btnButton13.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton13.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton13.Location = New System.Drawing.Point(599, 548)
        Me.btnButton13.Name = "btnButton13"
        Me.btnButton13.Size = New System.Drawing.Size(368, 48)
        Me.btnButton13.TabIndex = 15
        Me.btnButton13.Text = "Button13"
        Me.btnButton13.UseVisualStyleBackColor = False
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(844, 628)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(124, 48)
        Me.btnReturn.TabIndex = 16
        Me.btnReturn.Text = "BtnReturn"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnButton12
        '
        Me.btnButton12.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton12.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton12.Location = New System.Drawing.Point(599, 468)
        Me.btnButton12.Name = "btnButton12"
        Me.btnButton12.Size = New System.Drawing.Size(368, 48)
        Me.btnButton12.TabIndex = 14
        Me.btnButton12.Text = "Button12"
        Me.btnButton12.UseVisualStyleBackColor = False
        '
        'btnButton11
        '
        Me.btnButton11.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton11.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton11.Location = New System.Drawing.Point(599, 388)
        Me.btnButton11.Name = "btnButton11"
        Me.btnButton11.Size = New System.Drawing.Size(368, 48)
        Me.btnButton11.TabIndex = 13
        Me.btnButton11.Text = "Button11"
        Me.btnButton11.UseVisualStyleBackColor = False
        '
        'btnButton10
        '
        Me.btnButton10.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton10.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton10.Location = New System.Drawing.Point(599, 308)
        Me.btnButton10.Name = "btnButton10"
        Me.btnButton10.Size = New System.Drawing.Size(368, 48)
        Me.btnButton10.TabIndex = 12
        Me.btnButton10.Text = "Button10"
        Me.btnButton10.UseVisualStyleBackColor = False
        '
        'btnButton9
        '
        Me.btnButton9.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton9.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton9.Location = New System.Drawing.Point(599, 228)
        Me.btnButton9.Name = "btnButton9"
        Me.btnButton9.Size = New System.Drawing.Size(368, 48)
        Me.btnButton9.TabIndex = 11
        Me.btnButton9.Text = "Button9"
        Me.btnButton9.UseVisualStyleBackColor = False
        '
        'btnButton8
        '
        Me.btnButton8.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton8.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton8.Location = New System.Drawing.Point(599, 148)
        Me.btnButton8.Name = "btnButton8"
        Me.btnButton8.Size = New System.Drawing.Size(368, 48)
        Me.btnButton8.TabIndex = 10
        Me.btnButton8.Text = "Button8"
        Me.btnButton8.UseVisualStyleBackColor = False
        '
        'btnButton7
        '
        Me.btnButton7.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton7.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton7.Location = New System.Drawing.Point(47, 628)
        Me.btnButton7.Name = "btnButton7"
        Me.btnButton7.Size = New System.Drawing.Size(368, 48)
        Me.btnButton7.TabIndex = 9
        Me.btnButton7.Text = "Button7"
        Me.btnButton7.UseVisualStyleBackColor = False
        '
        'btnButton6
        '
        Me.btnButton6.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton6.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton6.Location = New System.Drawing.Point(47, 548)
        Me.btnButton6.Name = "btnButton6"
        Me.btnButton6.Size = New System.Drawing.Size(368, 48)
        Me.btnButton6.TabIndex = 8
        Me.btnButton6.Text = "Button6"
        Me.btnButton6.UseVisualStyleBackColor = False
        '
        'btnButton5
        '
        Me.btnButton5.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton5.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton5.Location = New System.Drawing.Point(47, 468)
        Me.btnButton5.Name = "btnButton5"
        Me.btnButton5.Size = New System.Drawing.Size(368, 48)
        Me.btnButton5.TabIndex = 7
        Me.btnButton5.Text = "Button5"
        Me.btnButton5.UseVisualStyleBackColor = False
        '
        'btnButton4
        '
        Me.btnButton4.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton4.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton4.Location = New System.Drawing.Point(47, 388)
        Me.btnButton4.Name = "btnButton4"
        Me.btnButton4.Size = New System.Drawing.Size(368, 48)
        Me.btnButton4.TabIndex = 6
        Me.btnButton4.Text = "Button4"
        Me.btnButton4.UseVisualStyleBackColor = False
        '
        'btnButton3
        '
        Me.btnButton3.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton3.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton3.Location = New System.Drawing.Point(47, 308)
        Me.btnButton3.Name = "btnButton3"
        Me.btnButton3.Size = New System.Drawing.Size(368, 48)
        Me.btnButton3.TabIndex = 5
        Me.btnButton3.Text = "Button3"
        Me.btnButton3.UseVisualStyleBackColor = False
        '
        'btnButton2
        '
        Me.btnButton2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton2.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton2.Location = New System.Drawing.Point(47, 228)
        Me.btnButton2.Name = "btnButton2"
        Me.btnButton2.Size = New System.Drawing.Size(368, 48)
        Me.btnButton2.TabIndex = 4
        Me.btnButton2.Text = "Button2"
        Me.btnButton2.UseVisualStyleBackColor = False
        '
        'btnButton1
        '
        Me.btnButton1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnButton1.Font = New System.Drawing.Font("ＭＳ ゴシック", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnButton1.Location = New System.Drawing.Point(47, 148)
        Me.btnButton1.Name = "btnButton1"
        Me.btnButton1.Size = New System.Drawing.Size(368, 48)
        Me.btnButton1.TabIndex = 3
        Me.btnButton1.Text = "Button1"
        Me.btnButton1.UseVisualStyleBackColor = False
        '
        'FrmBaseMenu
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Controls.Add(Me.btnButton13)
        Me.Controls.Add(Me.btnReturn)
        Me.Controls.Add(Me.btnButton12)
        Me.Controls.Add(Me.btnButton11)
        Me.Controls.Add(Me.btnButton10)
        Me.Controls.Add(Me.btnButton9)
        Me.Controls.Add(Me.btnButton8)
        Me.Controls.Add(Me.btnButton7)
        Me.Controls.Add(Me.btnButton6)
        Me.Controls.Add(Me.btnButton5)
        Me.Controls.Add(Me.btnButton4)
        Me.Controls.Add(Me.btnButton3)
        Me.Controls.Add(Me.btnButton2)
        Me.Controls.Add(Me.btnButton1)
        Me.Name = "FrmBaseMenu"
        Me.Controls.SetChildIndex(Me.pnlBodyBase, 0)
        Me.Controls.SetChildIndex(Me.btnButton1, 0)
        Me.Controls.SetChildIndex(Me.btnButton2, 0)
        Me.Controls.SetChildIndex(Me.btnButton3, 0)
        Me.Controls.SetChildIndex(Me.btnButton4, 0)
        Me.Controls.SetChildIndex(Me.btnButton5, 0)
        Me.Controls.SetChildIndex(Me.btnButton6, 0)
        Me.Controls.SetChildIndex(Me.btnButton7, 0)
        Me.Controls.SetChildIndex(Me.btnButton8, 0)
        Me.Controls.SetChildIndex(Me.btnButton9, 0)
        Me.Controls.SetChildIndex(Me.btnButton10, 0)
        Me.Controls.SetChildIndex(Me.btnButton11, 0)
        Me.Controls.SetChildIndex(Me.btnButton12, 0)
        Me.Controls.SetChildIndex(Me.btnReturn, 0)
        Me.Controls.SetChildIndex(Me.btnButton13, 0)
        Me.Controls.SetChildIndex(Me.lblTitle, 0)
        Me.Controls.SetChildIndex(Me.lblToday, 0)
        Me.ResumeLayout(False)

    End Sub

#End Region

    'フォームロード
    Private Sub FrmBaseMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
    End Sub

End Class
