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
Imports System.Text

''' <summary>DB�֏C�����ꂽ���[�U�̏��</summary>
''' <remarks>
''' �C������:�p�X���[�h�A�m�F�p�p�X���[�h�A�����A���b�N�A�E�g
''' �h�c�R�[�h�́A��A�N�e�B�u�Ƃ���B
''' </remarks>
Public Class FrmSysIDMstUpdate
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
    Friend WithEvents rbtSysmnt As System.Windows.Forms.RadioButton
    Friend WithEvents rbtUsumnt As System.Windows.Forms.RadioButton
    Friend WithEvents rbtAdmin As System.Windows.Forms.RadioButton
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents chkLockout As System.Windows.Forms.CheckBox
    Friend WithEvents grpAuth As System.Windows.Forms.GroupBox
    Friend WithEvents pnlBase As System.Windows.Forms.Panel
    Friend WithEvents txtPassword2 As System.Windows.Forms.TextBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblRePwd As System.Windows.Forms.Label
    Friend WithEvents lblPwd As System.Windows.Forms.Label
    Friend WithEvents lblTitleID As System.Windows.Forms.Label
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents lblID As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.rbtSysmnt = New System.Windows.Forms.RadioButton()
        Me.rbtUsumnt = New System.Windows.Forms.RadioButton()
        Me.rbtAdmin = New System.Windows.Forms.RadioButton()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.chkLockout = New System.Windows.Forms.CheckBox()
        Me.grpAuth = New System.Windows.Forms.GroupBox()
        Me.pnlBase = New System.Windows.Forms.Panel()
        Me.lblID = New System.Windows.Forms.Label()
        Me.pnlMain = New System.Windows.Forms.Panel()
        Me.txtPassword2 = New System.Windows.Forms.TextBox()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.lblRePwd = New System.Windows.Forms.Label()
        Me.lblPwd = New System.Windows.Forms.Label()
        Me.lblTitleID = New System.Windows.Forms.Label()
        Me.grpAuth.SuspendLayout()
        Me.pnlBase.SuspendLayout()
        Me.pnlMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'rbtSysmnt
        '
        Me.rbtSysmnt.AutoSize = True
        Me.rbtSysmnt.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rbtSysmnt.Location = New System.Drawing.Point(347, 24)
        Me.rbtSysmnt.Name = "rbtSysmnt"
        Me.rbtSysmnt.Size = New System.Drawing.Size(123, 17)
        Me.rbtSysmnt.TabIndex = 6
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
        Me.rbtUsumnt.TabIndex = 5
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
        Me.rbtAdmin.TabIndex = 4
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
        Me.btnStop.TabIndex = 9
        Me.btnStop.Text = "�I�@��"
        Me.btnStop.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.Color.Silver
        Me.btnUpdate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(320, 356)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(90, 32)
        Me.btnUpdate.TabIndex = 8
        Me.btnUpdate.Text = "�C  ��"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'chkLockout
        '
        Me.chkLockout.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.chkLockout.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.chkLockout.Location = New System.Drawing.Point(52, 289)
        Me.chkLockout.Name = "chkLockout"
        Me.chkLockout.Size = New System.Drawing.Size(110, 23)
        Me.chkLockout.TabIndex = 7
        Me.chkLockout.Text = "���b�N�A�E�g"
        Me.chkLockout.UseVisualStyleBackColor = False
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
        Me.grpAuth.TabIndex = 3
        Me.grpAuth.TabStop = False
        Me.grpAuth.Text = "���@��"
        '
        'pnlBase
        '
        Me.pnlBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBase.Controls.Add(Me.lblID)
        Me.pnlBase.Controls.Add(Me.pnlMain)
        Me.pnlBase.Controls.Add(Me.btnStop)
        Me.pnlBase.Controls.Add(Me.btnUpdate)
        Me.pnlBase.Controls.Add(Me.chkLockout)
        Me.pnlBase.Controls.Add(Me.txtPassword2)
        Me.pnlBase.Controls.Add(Me.txtPassword)
        Me.pnlBase.Controls.Add(Me.lblRePwd)
        Me.pnlBase.Controls.Add(Me.lblPwd)
        Me.pnlBase.Controls.Add(Me.lblTitleID)
        Me.pnlBase.Location = New System.Drawing.Point(0, 0)
        Me.pnlBase.Name = "pnlBase"
        Me.pnlBase.Size = New System.Drawing.Size(594, 418)
        Me.pnlBase.TabIndex = 0
        '
        'lblID
        '
        Me.lblID.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblID.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblID.Location = New System.Drawing.Point(161, 39)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(100, 18)
        Me.lblID.TabIndex = 7
        Me.lblID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlMain
        '
        Me.pnlMain.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlMain.Controls.Add(Me.grpAuth)
        Me.pnlMain.Location = New System.Drawing.Point(41, 186)
        Me.pnlMain.Name = "pnlMain"
        Me.pnlMain.Size = New System.Drawing.Size(510, 80)
        Me.pnlMain.TabIndex = 3
        Me.pnlMain.TabStop = True
        '
        'txtPassword2
        '
        Me.txtPassword2.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPassword2.Location = New System.Drawing.Point(161, 119)
        Me.txtPassword2.MaxLength = 8
        Me.txtPassword2.Name = "txtPassword2"
        Me.txtPassword2.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword2.Size = New System.Drawing.Size(65, 20)
        Me.txtPassword2.TabIndex = 2
        '
        'txtPassword
        '
        Me.txtPassword.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPassword.Location = New System.Drawing.Point(161, 79)
        Me.txtPassword.MaxLength = 8
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(65, 20)
        Me.txtPassword.TabIndex = 1
        '
        'lblRePwd
        '
        Me.lblRePwd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblRePwd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblRePwd.Location = New System.Drawing.Point(46, 119)
        Me.lblRePwd.Name = "lblRePwd"
        Me.lblRePwd.Size = New System.Drawing.Size(110, 18)
        Me.lblRePwd.TabIndex = 9
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
        'lblTitleID
        '
        Me.lblTitleID.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblTitleID.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitleID.Location = New System.Drawing.Point(46, 39)
        Me.lblTitleID.Name = "lblTitleID"
        Me.lblTitleID.Size = New System.Drawing.Size(110, 18)
        Me.lblTitleID.TabIndex = 6
        Me.lblTitleID.Text = "�h�c�R�[�h"
        Me.lblTitleID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmSysIDMstUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlBase)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysIDMstUpdate"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ID�}�X�^�C��"
        Me.grpAuth.ResumeLayout(False)
        Me.grpAuth.PerformLayout()
        Me.pnlBase.ResumeLayout(False)
        Me.pnlBase.PerformLayout()
        Me.pnlMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�錾�̈�iPrivate�j"

    ''' <summary>
    ''' ���������ďo����
    ''' �iTrue:���������ďo�ς݁AFalse:�����������ďo(Form_Load���ŏ����������{)�j
    ''' </summary>
    Private LbInitCallFlg As Boolean = False

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean

    'DB�֓��͂��ꂽ�����R�[�h�B
    Private Const DB_AUTH_SYS As String = "1"
    Private Const DB_AUTH_ADMIN As String = "2"
    Private Const DB_AUTH_USUAL As String = "3"

    'DB�֓��͂��ꂽ���b�N�t���O�B
    Private Const DB_LOCK_NOMAL As String = "0"
    Private Const DB_LOCKING As String = "1"

    '�C�����[�U��ID���擾����B
    Private sLoginID As String = ""

    'FrmSysIDMst��ʂ�ID�l���擾����B
    Private sUserid As String = ""

    Public Property Userid() As String
        Get
            Return sUserid
        End Get
        Set(ByVal value As String)
            sUserid = value
        End Set
    End Property

    'FrmSysIDMst��ʂ̃p�X���[�h�l���擾����B
    Private sPwd As String = ""

    'FrmSysIDMst��ʂ̌����l���擾����B
    Private sAuthority As String = ""

    'FrmSysIDMst��ʂ̃��b�N�t���O���擾����B
    Private sLock As String = ""

    '�X�V����
    Private oldDate As String = ""

    '�X�V����
    Private newDate As String = ""

#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>�h�c�f�[�^�C����ʂ̃f�[�^����������</summary>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e
        Dim dtMstTable As New DataTable
        Dim nRtn As Integer
        Dim sSql As String = ""
        Try
            Log.Info("Method started.")

            '�����ID���擾����B
            sLoginID = GlobalVariables.UserId

            '�f�[�^���擾����B
            sSql = LfGetSelectString()
            nRtn = FrmBase.BaseSqlDataTableFill(sSql, dtMstTable)
            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    Return bRet
                    Exit Function
                Case Else
                    If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
                        '���������Ɉ�v����f�[�^�͑��݂��܂���B
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Return bRet
                        Exit Function
                    Else
                        sPwd = dtMstTable.Rows(0).Item("PASSWORD").ToString
                        sAuthority = dtMstTable.Rows(0).Item("AUTHORITY_LEVEL").ToString
                        sLock = dtMstTable.Rows(0).Item("LOCK_STS").ToString
                        oldDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
                    End If
            End Select

            bRet = True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
            bRet = False
        Finally
            If bRet Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
            End If
            LbEventStop = False '�C�x���g�����n�m
        End Try

        Return bRet

    End Function

#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' ���[�f�B���O�@���C���E�B���h�E  
    ''' </summary>
    Private Sub FrmSysIDMstUpdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrmData() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

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
            lblTitleID.BackColor = Config.BackgroundColor

            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnUpdate.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor

            '���[�U��ݒ肷��B
            lblID.Text = sUserid

            '�p�X���[�h��ݒ肷��B
            txtPassword.Text = sPwd

            '�p�X���[�h�m�F��ݒ肷��B
            txtPassword2.Text = sPwd

            '������ݒ肷��B
            If sAuthority = DB_AUTH_SYS Then
                rbtSysmnt.Checked = True
            ElseIf sAuthority = DB_AUTH_ADMIN Then
                rbtUsumnt.Checked = True
            ElseIf sAuthority = DB_AUTH_USUAL Then
                rbtAdmin.Checked = True
            Else
                rbtSysmnt.Checked = False
                rbtUsumnt.Checked = False
                rbtAdmin.Checked = False
            End If

            '���b�N�t���O��ݒ肷��B
            If sLock = DB_LOCKING Then
                chkLockout.Checked = True
            Else
                chkLockout.Checked = False
            End If

            Me.txtPassword.Focus()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    '�u�C���v�{�^������������ƁA�f�[�^�x�[�X�ɂČ��ݑI�����ꂽ���[�U���X�V����B
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Dim dtMstTable As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer

        Try
            LbEventStop = True
            '�C���{�^�������B
            FrmBase.LogOperation(sender, e, Me.Text)

            If CheckAll() = True Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUpdate).Equals(System.Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Me.Text)
                    Call WaitCursor(True)
                    '�f�[�^���擾����B
                    sSql = LfGetSelectString()
                    nRtn = FrmBase.BaseSqlDataTableFill(sSql, dtMstTable)
                    Select Case nRtn
                        Case -9             '�c�a�I�[�v���G���[
                            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                            Exit Sub
                        Case Else
                            If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
                                '���������Ɉ�v����f�[�^�͑��݂��܂���B
                                AlertBox.Show(Lexis.CompetitiveOperationDetected)
                                Exit Sub
                            Else
                                 newDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
                            End If
                    End Select

                    '�r���`�F�b�N
                    If Not oldDate.Equals(newDate) Then
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Exit Sub
                    End If

                    '�X�V����
                    Call UpdateID()
                    FrmBase.LogOperation(Lexis.UpdateCompleted, Me.Text) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B   '�X�V����������ɏI�����܂����B
                    AlertBox.Show(Lexis.UpdateCompleted)
                    FrmBase.LogOperation(Lexis.OkButtonClicked, Me.Text)
                    Me.Close()
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked, Me.Text)
                    btnUpdate.Select()
                End If
            End If

        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnUpdate.Select()
            Exit Sub

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '�\�����ʃG���[���������܂����B
            AlertBox.Show(Lexis.UpdateFailed)
            btnUpdate.Select()
            Exit Sub

        Finally
            LbEventStop = False
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

    ''' <summary>�u�p�X���[�h�v�Ɓu�p�X���[�h�m�F�v�̓��͒l����������</summary>
    Private Sub txtPwd_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPassword.KeyPress, txtPassword2.KeyPress
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
    ''' �u�C���v�{�^�������������ۂɂ��ׂẴR���g���[���̒l���`�F�b�N����B
    ''' </summary>
    ''' <returns>�f�[�^���@�t���O</returns>
    Private Function CheckAll() As Boolean

        '���֐��̖߂�l
        Dim bRetAll As Boolean = False

        If System.String.IsNullOrEmpty(txtPassword.Text) Then
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
    ''' �f�[�^�x�[�X�ɂČ��ݑI�����ꂽ���[�U���X�V����B
    ''' </summary>
    Private Sub UpdateID()

        Dim sSQL As String = ""

        Dim sBuilder As New StringBuilder

        Dim dbCtl As DatabaseTalker
        dbCtl = New DatabaseTalker

        Try
            '�p�X���[�h���擾����B
            sPwd = txtPassword.Text

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

            '�[��ID
            Dim sClient As String = Config.MachineName
            sBuilder.AppendLine(" UPDATE M_USER SET UPDATE_DATE = GETDATE(),")
            sBuilder.AppendLine(" UPDATE_USER_ID = " & Utility.SetSglQuot(sLoginID.ToString) & ",")
            sBuilder.AppendLine(" UPDATE_MACHINE_ID = " & Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(" PASSWORD = " & Utility.SetSglQuot(sPwd) & ",")
            sBuilder.AppendLine(" AUTHORITY_LEVEL = " & Utility.SetSglQuot(sAuthority) & ",")
            sBuilder.AppendLine(" LOCK_STS = " & Utility.SetSglQuot(sLock))
            sBuilder.AppendLine(" WHERE USER_ID = " & Utility.SetSglQuot(sUserid))

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

    ''' <summary>
    ''' �f�[�^���擾����B
    ''' </summary>
    Private Function LfGetSelectString() As String
        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine(" SELECT PASSWORD, AUTHORITY_LEVEL, LOCK_STS,  UPDATE_DATE")
            sBuilder.AppendLine("  FROM M_USER  ")
            sBuilder.AppendLine("  WHERE USER_ID = " & Utility.SetSglQuot(sUserid))
            sSQL = sBuilder.ToString()

            Return sSQL
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        End Try

    End Function

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