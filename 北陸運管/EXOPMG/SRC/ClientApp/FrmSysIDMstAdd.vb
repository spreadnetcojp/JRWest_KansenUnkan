' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DBCommon.OPMGUtility
Imports System.Data.SqlClient
Imports System.Text

''' <summary>DB�֓��͂��ꂽ���[�U�̏��</summary>
''' <remarks>
''' ���͍���:�h�c�R�[�h�A�p�X���[�h�A�m�F�p�p�X���[�h�A�����A���b�N�A�E�g�B
''' </remarks>
Public Class FrmSysIDMstAdd
    Inherits System.Windows.Forms.Form

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub


    ' Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    '����: �ȉ��̃v���V�[�W���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g�p���ĕύX�ł��܂��B  
    '�R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    Friend WithEvents pnlBase As System.Windows.Forms.Panel
    Friend WithEvents lblID As System.Windows.Forms.Label
    Friend WithEvents lblRePwd As System.Windows.Forms.Label
    Friend WithEvents lblPwd As System.Windows.Forms.Label
    Friend WithEvents txtIDCode As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword2 As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents grpAuth As System.Windows.Forms.GroupBox
    Friend WithEvents rbtAdmin As System.Windows.Forms.RadioButton
    Friend WithEvents rbtSysmnt As System.Windows.Forms.RadioButton
    Friend WithEvents rbtUsumnt As System.Windows.Forms.RadioButton
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents chkLockout As System.Windows.Forms.CheckBox
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pnlBase = New System.Windows.Forms.Panel()
        Me.pnlMain = New System.Windows.Forms.Panel()
        Me.grpAuth = New System.Windows.Forms.GroupBox()
        Me.rbtSysmnt = New System.Windows.Forms.RadioButton()
        Me.rbtUsumnt = New System.Windows.Forms.RadioButton()
        Me.rbtAdmin = New System.Windows.Forms.RadioButton()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.chkLockout = New System.Windows.Forms.CheckBox()
        Me.txtPassword2 = New System.Windows.Forms.TextBox()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.txtIDCode = New System.Windows.Forms.TextBox()
        Me.lblRePwd = New System.Windows.Forms.Label()
        Me.lblPwd = New System.Windows.Forms.Label()
        Me.lblID = New System.Windows.Forms.Label()
        Me.pnlBase.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.grpAuth.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlBase
        '
        Me.pnlBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBase.Controls.Add(Me.pnlMain)
        Me.pnlBase.Controls.Add(Me.btnStop)
        Me.pnlBase.Controls.Add(Me.btnInsert)
        Me.pnlBase.Controls.Add(Me.chkLockout)
        Me.pnlBase.Controls.Add(Me.txtPassword2)
        Me.pnlBase.Controls.Add(Me.txtPassword)
        Me.pnlBase.Controls.Add(Me.txtIDCode)
        Me.pnlBase.Controls.Add(Me.lblRePwd)
        Me.pnlBase.Controls.Add(Me.lblPwd)
        Me.pnlBase.Controls.Add(Me.lblID)
        Me.pnlBase.Location = New System.Drawing.Point(0, 0)
        Me.pnlBase.Name = "pnlBase"
        Me.pnlBase.Size = New System.Drawing.Size(594, 418)
        Me.pnlBase.TabIndex = 0
        '
        'pnlMain
        '
        Me.pnlMain.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlMain.Controls.Add(Me.grpAuth)
        Me.pnlMain.Location = New System.Drawing.Point(41, 186)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(510, 80)
        Me.pnlMain.TabIndex = 4
        Me.pnlMain.TabStop = True
        '
        'grpAuth
        '
        Me.grpAuth.BackColor = System.Drawing.SystemColors.ControlLight
        Me.grpAuth.Controls.Add(Me.rbtSysmnt)
        Me.grpAuth.Controls.Add(Me.rbtUsumnt)
        Me.grpAuth.Controls.Add(Me.rbtAdmin)
        Me.grpAuth.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.grpAuth.Location = New System.Drawing.Point(10, 8)
        Me.grpAuth.Name = "grpAuth"
        Me.grpAuth.Size = New System.Drawing.Size(490, 60)
        Me.grpAuth.TabIndex = 4
        Me.grpAuth.TabStop = False
        Me.grpAuth.Text = "���@��"
        '
        'rbtSysmnt
        '
        Me.rbtSysmnt.AutoSize = True
        Me.rbtSysmnt.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbtSysmnt.Location = New System.Drawing.Point(347, 24)
        Me.rbtSysmnt.Name = "rbtSysmnt"
        Me.rbtSysmnt.Size = New System.Drawing.Size(123, 17)
        Me.rbtSysmnt.TabIndex = 7
        Me.rbtSysmnt.TabStop = True
        Me.rbtSysmnt.Text = "�V�X�e���Ǘ���"
        Me.rbtSysmnt.UseVisualStyleBackColor = True
        '
        'rbtUsumnt
        '
        Me.rbtUsumnt.AutoSize = True
        Me.rbtUsumnt.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbtUsumnt.Location = New System.Drawing.Point(171, 24)
        Me.rbtUsumnt.Name = "rbtUsumnt"
        Me.rbtUsumnt.Size = New System.Drawing.Size(95, 17)
        Me.rbtUsumnt.TabIndex = 6
        Me.rbtUsumnt.TabStop = True
        Me.rbtUsumnt.Text = "�^�p�Ǘ���"
        Me.rbtUsumnt.UseVisualStyleBackColor = True
        '
        'rbtAdmin
        '
        Me.rbtAdmin.AutoSize = True
        Me.rbtAdmin.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbtAdmin.Location = New System.Drawing.Point(26, 24)
        Me.rbtAdmin.Name = "rbtAdmin"
        Me.rbtAdmin.Size = New System.Drawing.Size(67, 17)
        Me.rbtAdmin.TabIndex = 5
        Me.rbtAdmin.TabStop = True
        Me.rbtAdmin.Text = "��ʎ�"
        Me.rbtAdmin.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.BackColor = System.Drawing.Color.Silver
        Me.btnStop.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnStop.Location = New System.Drawing.Point(459, 356)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(90, 32)
        Me.btnStop.TabIndex = 10
        Me.btnStop.Text = "�I�@��"
        Me.btnStop.UseVisualStyleBackColor = False
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.Color.Silver
        Me.btnInsert.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnInsert.Location = New System.Drawing.Point(320, 356)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(90, 32)
        Me.btnInsert.TabIndex = 9
        Me.btnInsert.Text = "�o  �^"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'chkLockout
        '
        Me.chkLockout.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.chkLockout.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chkLockout.Location = New System.Drawing.Point(52, 289)
        Me.chkLockout.Name = "chkLockout"
        Me.chkLockout.Size = New System.Drawing.Size(110, 23)
        Me.chkLockout.TabIndex = 8
        Me.chkLockout.Text = "���b�N�A�E�g"
        Me.chkLockout.UseVisualStyleBackColor = False
        '
        'txtPassword2
        '
        Me.txtPassword2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPassword2.Location = New System.Drawing.Point(161, 119)
        Me.txtPassword2.MaxLength = 8
        Me.txtPassword2.Name = "txtPassword2"
        Me.txtPassword2.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword2.Size = New System.Drawing.Size(65, 20)
        Me.txtPassword2.TabIndex = 3
        '
        'txtPassword
        '
        Me.txtPassword.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPassword.Location = New System.Drawing.Point(161, 79)
        Me.txtPassword.MaxLength = 8
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(65, 20)
        Me.txtPassword.TabIndex = 2
        '
        'txtIDCode
        '
        Me.txtIDCode.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtIDCode.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtIDCode.Location = New System.Drawing.Point(161, 39)
        Me.txtIDCode.MaxLength = 8
        Me.txtIDCode.Name = "txtIDCode"
        Me.txtIDCode.Size = New System.Drawing.Size(65, 20)
        Me.txtIDCode.TabIndex = 1
        '
        'lblRePwd
        '
        Me.lblRePwd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblRePwd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblRePwd.Location = New System.Drawing.Point(46, 119)
        Me.lblRePwd.Name = "lblRePwd"
        Me.lblRePwd.Size = New System.Drawing.Size(110, 18)
        Me.lblRePwd.TabIndex = 7
        Me.lblRePwd.Text = "�p�X���[�h�m�F"
        Me.lblRePwd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPwd
        '
        Me.lblPwd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblPwd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPwd.Location = New System.Drawing.Point(46, 79)
        Me.lblPwd.Name = "lblPwd"
        Me.lblPwd.Size = New System.Drawing.Size(110, 18)
        Me.lblPwd.TabIndex = 8
        Me.lblPwd.Text = "�p�X���[�h"
        Me.lblPwd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblID
        '
        Me.lblID.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblID.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblID.Location = New System.Drawing.Point(46, 39)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(110, 18)
        Me.lblID.TabIndex = 9
        Me.lblID.Text = "�h�c�R�[�h"
        Me.lblID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmSysIDMstAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlBase)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysIDMstAdd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ID�}�X�^�o�^"
        Me.pnlBase.ResumeLayout(False)
        Me.pnlBase.PerformLayout()
        Me.pnlMain.ResumeLayout(False)
        Me.grpAuth.ResumeLayout(False)
        Me.grpAuth.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�錾�̈�iPrivate�j"

    'DB�֓��͂��ꂽ�����R�[�h�B
    Private Const DB_AUTH_SYS As String = "1"
    Private Const DB_AUTH_ADMIN As String = "2"
    Private Const DB_AUTH_USUAL As String = "3"

    'DB�֓��͂��ꂽ���b�N�t���O�B
    Private Const DB_LOCK_NOMAL As String = "0"
    Private Const DB_LOCKING As String = "1"

    '�o�^���[�U��ID���擾����B
    Private sLoginID As String = ""

#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' ���[�f�B���O�@���C���E�B���h�E
    ''' �������:�u��ʎҁv���I�������B  
    ''' </summary>
    Private Sub FrmSysIDMstLogin_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bRet As Boolean = False

        Try
            Log.Info("Method started.")

            '��ʔw�i�F�iBackColor�j��ݒ肷��
            pnlBase.BackColor = Config.BackgroundColor
            pnlMain.BackColor = Config.BackgroundColor
            grpAuth.BackColor = Config.BackgroundColor
            rbtAdmin.BackColor = Config.BackgroundColor
            rbtUsumnt.BackColor = Config.BackgroundColor
            rbtSysmnt.BackColor = Config.BackgroundColor
            chkLockout.BackColor = Config.BackgroundColor
            lblID.BackColor = Config.BackgroundColor
            lblPwd.BackColor = Config.BackgroundColor
            lblRePwd.BackColor = Config.BackgroundColor

            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnInsert.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor

            '�����ID���擾����
            sLoginID = GlobalVariables.UserId

            Me.txtIDCode.Focus()

            '������Ԃ� �u��ʎҁv���I�������B
            rbtAdmin.Checked = True
            bRet = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRet = False
        Finally
            If bRet Then
                Log.Info("The form proc ended.")
            Else
                Log.Error("The form proc abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
                bRet = False
            End If
        End Try

    End Sub

    ''' <summary>
    ''' �u�o�^�v�{�^������������ƁADB�֐V�������[�U���o�^�����B
    ''' </summary>
    Private Sub btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click

        Try
            '�o�^�{�^�������B
            FrmBase.LogOperation(sender, e, Me.Text)

            If CheckAll() = True Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInsert).Equals(System.Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Me.Text)
                    Call WaitCursor(True)
                    Call AddNewID()
                    FrmBase.LogOperation(Lexis.InsertCompleted, Me.Text) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B   '�o�^����������ɏI�����܂����B
                    AlertBox.Show(Lexis.InsertCompleted)
                    FrmBase.LogOperation(Lexis.OkButtonClicked, Me.Text)
                    Me.Close()
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked, Me.Text)
                    btnInsert.Select()
                End If
            End If

        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnInsert.Select()
            Exit Sub
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '�\�����ʃG���[���������܂����B
            AlertBox.Show(Lexis.InsertFailed)
            btnInsert.Select()
            Exit Sub

        Finally

            Call WaitCursor(False)

        End Try

    End Sub

    ''' <summary>
    ''' �u�I���v�{�^������������ƁA�{��ʂ��I�������B 
    ''' </summary>
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        '�I���{�^�������B
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

    ''' <summary>�u�h�c�R�[�h�v�A�u�p�X���[�h�v�A�u�p�X���[�h�m�F�v�̓��͒l����������</summary>
    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtIDCode.KeyPress, txtPassword.KeyPress, txtPassword2.KeyPress
        Select Case e.KeyChar
            Case "0".ToCharArray To "9".ToCharArray
            Case "a".ToCharArray To "z".ToCharArray
            Case "A".ToCharArray To "Z".ToCharArray
            Case Chr(8)
            Case Else
                e.Handled = True
        End Select
    End Sub

#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' �u�o�^�v�{�^�������������ۂɂ��ׂẴR���g���[���̒l���`�F�b�N����B
    ''' </summary>
    ''' <returns>�f�[�^���@�t���O</returns>
    Private Function CheckAll() As Boolean

        '���֐��̖߂�l
        Dim bRetAll As Boolean = False

        If System.String.IsNullOrEmpty(txtIDCode.Text) Then
            '���͒l���s���ł��BID���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblID.Text)
            txtIDCode.Focus()
        ElseIf txtIDCode.Text.Length <> 8 OrElse checkCharacter(txtIDCode.Text.Trim) = False Then
            '���͒l���s���ł��BID�̒�����8�����łȂ��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForIdCode)
            txtIDCode.Focus()
        ElseIf Not IsLogined() Then
            'ID�R�[�h�̏d���`�F�b�N
            txtIDCode.Focus()
        ElseIf System.String.IsNullOrEmpty(txtPassword.Text) Then
            '���͒l���s���ł��B�p�X���[�h���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblPwd.Text)
            txtPassword.Focus()
        ElseIf (txtPassword.Text.Length < 4 OrElse txtPassword.Text.Length > 8) OrElse _
                checkCharacter(txtPassword.Text.Trim) = False Then
            '���͒l���s���ł��B�p�X���[�h�̒�����4�`8�����łȂ��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPassword)
            txtPassword.Focus()
        ElseIf System.String.IsNullOrEmpty(txtPassword2.Text) Then
            '���͒l���s���ł��B�p�X���[�h�m�F�l���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblRePwd.Text)
            txtPassword2.Focus()
        ElseIf Not txtPassword2.Text.Equals(txtPassword.Text) Then
            '���͒l���s���ł��B�p�X���[�h�m�F�l�ƃp�X���[�h����v���Ȃ��B
            AlertBox.Show(Lexis.ThePasswordsDifferFromOneAnother)
            txtPassword.Focus()
        Else
            bRetAll = True
        End If

        Return bRetAll

    End Function

    ''' <summary>
    ''' ID�R�[�h�̏d���`�F�b�N
    ''' </summary>
    ''' <returns>ID�R�[�h�̏d���t���O</returns>
    Private Function IsLogined() As Boolean
        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        Dim dt As New DataTable
        Dim nRtn As Integer

        Try

            sBuilder.AppendLine("SELECT COUNT(1) FROM M_USER WHERE USER_ID = " + Utility.SetSglQuot(txtIDCode.Text.ToString))
            sSQL = sBuilder.ToString()

            nRtn = FrmBase.BaseSqlDataTableFill(sSQL, dt)
            Select Case nRtn
                Case -9
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    Return False
                Case Else
                    If Convert.ToInt64(dt.Rows(0)(0)) = 0 Then
                        Return True
                    Else
                        AlertBox.Show(Lexis.TheIdCodeAlreadyExists, txtIDCode.Text)
                        Return False
                    End If
                   
            End Select

        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try

    End Function


    ''' <summary>
    ''' DB�֐ݒ肳�ꂽ���[�U�����C���T�[�g����B
    ''' </summary>
    Private Sub AddNewID()

        Dim sSQL As String = ""

        Dim sBuilder As New StringBuilder

        Dim dbCtl As DatabaseTalker

        dbCtl = New DatabaseTalker

        Try
            '���[�U�A�p�X���[�h���擾����B
            Dim sUserid As String = txtIDCode.Text
            Dim sPwd As String = txtPassword.Text

            Dim sAuthority As String = ""
            Dim sLock As String = ""

            '�[��ID
            Dim sClient As String = Config.MachineName
            '���[�U�������擾����B
            If rbtAdmin.Checked = True Then
                sAuthority = DB_AUTH_USUAL
            ElseIf rbtUsumnt.Checked = True Then
                sAuthority = DB_AUTH_ADMIN
            Else
                sAuthority = DB_AUTH_SYS
            End If

            '���b�N�t���O���擾����B
            If chkLockout.Checked = True Then
                sLock = DB_LOCKING
            Else
                sLock = DB_LOCK_NOMAL
            End If

            sBuilder.AppendLine("INSERT INTO M_USER(INSERT_DATE,INSERT_USER_ID, INSERT_MACHINE_ID, ")
            sBuilder.AppendLine("UPDATE_DATE,UPDATE_USER_ID, UPDATE_MACHINE_ID, ")
            sBuilder.AppendLine("USER_ID,PASSWORD,AUTHORITY_LEVEL,LOCK_STS)  VALUES(GETDATE(), ")
            sBuilder.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine("GETDATE(), ")
            sBuilder.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sUserid) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sPwd) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sAuthority) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sLock) & ")")
            sSQL = sBuilder.ToString()

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)

        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

    End Sub

#End Region

#Region "�J�[�\���҂�"

    ''' <summary>
    ''' �J�[�\���҂�
    ''' </summary>
    ''' <param name="bWait">true:�҂��J�n�@false:�҂��I��</param>
    ''' <remarks>�J�[�\���������v�ɂȂ�</remarks>
    Private Sub WaitCursor(Optional ByVal bWait As Boolean = True)

        If bWait = True Then
            Me.Cursor = Cursors.WaitCursor
            Me.Enabled = False
        Else
            Me.Cursor = Cursors.Default
            Me.Enabled = True
        End If

    End Sub

#End Region

End Class