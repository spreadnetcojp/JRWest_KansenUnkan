' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
'   0.1      2013/11/11  (NES)����  �t�F�[�Y�Q�����Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DBCommon.OPMGUtility
Imports JR.ExOpmg.Common
Imports System.Data.SqlClient
Imports System.IO
Imports System.Deployment.Application

''' <summary> ���O�C�� </summary>
''' <remarks>
''' �h�c�R�[�h�̓o�^�f�[�^�����݂��邩�A���b�N���ꂽ���A�p�X���[�h����v���邩���`�F�b�N����B
''' �A�����ĎO��A�Ԉ�����p�X���[�h�����͂����ƁA���̂h�c�R�[�h�����b�N�����B
''' </remarks>
Public Class FrmLogin
    Inherits FrmBase

    Private Const KEYNAME As String = "USER_ID"         '�L�[��
    Private Const SECTIONNAME As String = "LOGIN"       '�Z�N�V������

    Private sAuth As String = ""    '����
    Private sLstUID As String = ""  '�O��o�^���ꂽ�h�c�R�[�h���L�^����B
    Private nTimes As Integer = 1   '�����h�c�R�[�h�Ń��O�C�����s������
    Private nLockout As Integer = 3 '���b�N�A�E�g���郍�O�C�����s��


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

    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    Friend WithEvents txtPWD As System.Windows.Forms.TextBox
    Friend WithEvents txtID As System.Windows.Forms.TextBox
    Friend WithEvents lblPWD As System.Windows.Forms.Label
    Friend WithEvents lblID As System.Windows.Forms.Label
    Friend WithEvents btnEnd As System.Windows.Forms.Button
    Friend WithEvents btnLogin As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtPWD = New System.Windows.Forms.TextBox
        Me.txtID = New System.Windows.Forms.TextBox
        Me.lblPWD = New System.Windows.Forms.Label
        Me.lblID = New System.Windows.Forms.Label
        Me.btnEnd = New System.Windows.Forms.Button
        Me.btnLogin = New System.Windows.Forms.Button
        Me.pnlBodyBase.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.txtID)
        Me.pnlBodyBase.Controls.Add(Me.btnEnd)
        Me.pnlBodyBase.Controls.Add(Me.lblPWD)
        Me.pnlBodyBase.Controls.Add(Me.lblID)
        Me.pnlBodyBase.Controls.Add(Me.btnLogin)
        Me.pnlBodyBase.Controls.Add(Me.txtPWD)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2006/08/29(��)  10:05"
        '
        'txtPWD
        '
        Me.txtPWD.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPWD.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.txtPWD.Location = New System.Drawing.Point(528, 256)
        Me.txtPWD.MaxLength = 8
        Me.txtPWD.Name = "txtPWD"
        Me.txtPWD.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPWD.Size = New System.Drawing.Size(80, 23)
        Me.txtPWD.TabIndex = 1
        '
        'txtID
        '
        Me.txtID.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtID.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.txtID.Location = New System.Drawing.Point(528, 208)
        Me.txtID.MaxLength = 9
        Me.txtID.Name = "txtID"
        Me.txtID.Size = New System.Drawing.Size(80, 23)
        Me.txtID.TabIndex = 0
        '
        'lblPWD
        '
        Me.lblPWD.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblPWD.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPWD.Location = New System.Drawing.Point(356, 256)
        Me.lblPWD.Name = "lblPWD"
        Me.lblPWD.Size = New System.Drawing.Size(160, 23)
        Me.lblPWD.TabIndex = 31
        Me.lblPWD.Text = "�p�X���[�h"
        Me.lblPWD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblID
        '
        Me.lblID.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblID.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblID.Location = New System.Drawing.Point(356, 208)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(160, 23)
        Me.lblID.TabIndex = 30
        Me.lblID.Text = "�h�c�R�[�h"
        Me.lblID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnEnd
        '
        Me.btnEnd.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnEnd.Font = New System.Drawing.Font("�l�r �S�V�b�N", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnEnd.Location = New System.Drawing.Point(532, 336)
        Me.btnEnd.Name = "btnEnd"
        Me.btnEnd.Size = New System.Drawing.Size(145, 48)
        Me.btnEnd.TabIndex = 3
        Me.btnEnd.Text = "���@�~"
        Me.btnEnd.UseVisualStyleBackColor = False
        '
        'btnLogin
        '
        Me.btnLogin.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnLogin.Font = New System.Drawing.Font("�l�r �S�V�b�N", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnLogin.Location = New System.Drawing.Point(316, 336)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.Size = New System.Drawing.Size(145, 48)
        Me.btnLogin.TabIndex = 2
        Me.btnLogin.Text = "���O�C��"
        Me.btnLogin.UseVisualStyleBackColor = False
        '
        'FrmLogin
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmLogin"
        Me.pnlBodyBase.ResumeLayout(False)
        Me.pnlBodyBase.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�t�H�[�����[�h"

    ''' <summary>�t�H�[�����[�h</summary>
    '''  <remarks>
    ''' �t�H�[�����[�h
    ''' </remarks>
    Private Sub FrmLogin_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Log.Info("Method started.")

        '��ʃ^�C�g��
        lblTitle.Text = "���O�C��"

        '��ԕۑ��t�@�C������O�񐬌��ɓo�^���ꂽ�h�c�R�[�h���擾����B
        txtID.Text = getLstUsrID()

        '�����ݒ�t�@�C�����烍�b�N�A�E�g�E���O�C�����s�񐔂��擾����B
        nLockout = Config.MaxInvalidPasswordAttempts

        Log.Info("Method ended.")
    End Sub
#End Region

#Region "�{�^���N���b�N"

    ''' <summary>�u���O�C���v�{�^������</summary>
    '''  <remarks>
    ''' �u���O�C���v�{�^������
    ''' </remarks>
    Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        Call waitCursor(True)
        LogOperation(sender, e)    '�{�^���������O

        Dim sUsrID As String = ""   '���͂��ꂽ�h�c�R�[�h
        Dim sPwd As String = ""     '���͂��ꂽ�p�X���[�h
        Dim sDBPwd As String = ""   'DB���猟�������p�X���[�h
        Dim sLockSts As String = ""
        Dim dt As DataTable
        sUsrID = txtID.Text
        sPwd = txtPWD.Text

        Try
            '���͂��ꂽ�h�c�R�[�h������
            dt = getData(sUsrID)
            If dt Is Nothing Then
                sLstUID = sUsrID
                nTimes = 1
                Exit Sub
            End If

            '���͂��ꂽ�h�c�R�[�h�ɑΉ�����o�^�f�[�^���Ȃ��ꍇ
            If checkUser(dt) = False Then
                sLstUID = sUsrID
                nTimes = 1
                Exit Sub
            End If

            sDBPwd = dt.Rows(0).Item("PASSWORD").ToString
            sAuth = dt.Rows(0).Item("AUTHORITY_LEVEL").ToString
            sLockSts = dt.Rows(0).Item("LOCK_STS").ToString
            '�o�^�f�[�^�����b�N�A�E�g���ǂ������`�F�b�N����
            If checkLock(sLockSts) = False Then
                sLstUID = sUsrID
                nTimes = 1
                Exit Sub
            End If
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@START-----------
            FrmBase.DetailSet = New ArrayList
            Dim i As Integer = 0
            For i = 4 To dt.Columns.Count - 1
                FrmBase.DetailSet.Add(dt.Rows(0)(i).ToString())
            Next
            '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD�@END-------------

            If sPwd = sDBPwd Then '���O�C������
                '��ԕۑ��t�@�C���ɂh�c�R�[�h���i�[����
                setUsrID(sUsrID)
                '���[�U�����i�[����
                GlobalVariables.UserId = sUsrID
                sLstUID = ""
                nTimes = 1
                '���j���[�ɑJ��
                openMenu(sUsrID, sAuth)

            Else '���O�C�����s
                If sLstUID = sUsrID Then
                    nTimes = nTimes + 1
                Else
                    nTimes = 1 '�O��̃��O�C���������̂ƈقȂ�A�񐔂ɂP���ēx�ݒ肷��B
                End If

                sLstUID = sUsrID
                Log.Info("�h�c�R�[�h�ƃp�X���[�h����v���܂���B")
                AlertBox.Show(Lexis.LoginFailedBecauseThePasswordIsIncorrect)
                txtPWD.Text = ""
                txtPWD.Focus()

                '�����h�c�R�[�h�Ń��O�C�������b�N�A�E�g����񐔎��݂�ƁA���b�N�A�E�g����
                If nTimes >= nLockout Then
                    lockID(sUsrID)
                End If
            End If
        Catch ex As DatabaseException
            If ex.TargetSite.Name = "getData" Or ex.TargetSite.Name = "lockID" Then
                'DB�ڑ��Ɏ��s���܂����B
                Log.Error("DB�ڑ��Ɏ��s���܂����B")
                AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            Else
                '���O�C�������Ɏ��s���܂����B
                Log.Error("���O�C�������Ɏ��s���܂����B")
                AlertBox.Show(Lexis.LoginFailed)
            End If

        Catch ex As Exception
            '���O�C�������Ɏ��s���܂����B
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.LoginFailed)

        Finally
            dt = Nothing
            sUsrID = Nothing
            sPwd = Nothing
            sDBPwd = Nothing
            sLockSts = Nothing
            Call waitCursor(False)
        End Try

    End Sub

    ''' <summary>�u���~�v�{�^���N���b�N�� </summary>
    '''  <remarks>
    ''' �u���~�v�{�^���N���b�N��
    ''' </remarks>
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        LogOperation(sender, e)    '�{�^���������O
        Me.Close()
    End Sub

#End Region

#Region "��ԕۑ��t�@�C������O�񃍃O�C���ɐ��������h�c�R�[�h���擾����B"
    ''' <summary>
    ''' ��ԕۑ��t�@�C������O�񃍃O�C���ɐ��������h�c�R�[�h���擾����B
    ''' </summary>
    ''' <returns>�O�񃍃O�C�����ꂽ�h�c�R�[�h</returns>
    Private Function getLstUsrID() As String

        Dim sLstUsrID As String = ""

        Try
            sLstUsrID = Constant.GetIni(SECTIONNAME, KEYNAME, Config.CookieFilePath)
            If sLstUsrID Is Nothing Then sLstUsrID = ""
        Catch ex As Exception
            Log.Info("SECTIONNAME :" & SECTIONNAME & "KEYNAME :" & KEYNAME & "FILENAME:" & Config.CookieFilePath)
        End Try

        Return sLstUsrID

    End Function
#End Region

#Region "��ԕۑ��t�@�C���ɍ��񃍃O�C���ɐ��������h�c�R�[�h����������"
    ''' <summary>
    ''' ��ԕۑ��t�@�C���ɍ��񃍃O�C���ɐ��������h�c�R�[�h���������ށB
    ''' </summary>
    ''' <param name="sUsrID">�h�c�R�[�h</param>
    Private Sub setUsrID(ByVal sUsrID As String)

        Dim bFlg As Boolean = False

        Try
            bFlg = Constant.SetIni(SECTIONNAME, KEYNAME, Config.CookieFilePath, sUsrID)
        Catch ex As Exception
            Log.Info("sectionName :" & SECTIONNAME & "keyName :" & KEYNAME & "FILENAME:" & Config.CookieFilePath & "USERID:" & sUsrID)
        End Try
    End Sub

#End Region

#Region "���j���[��ʂɓn���ׂ��̃��[�U�̌����l���擾����"

    ''' <summary>���j���[��ʂɓn���ׂ��̃��[�U�̌����l���擾����B</summary>
    ''' <param name="sNowUID">���͂��ꂽ�h�c�R�[�h</param>
    ''' <returns>�擾���ꂽ�����l�A�p�X���[�h�A���b�N�̃t���O</returns>
    Private Function getData(ByVal sNowUID As String) As DataTable
        Dim sSql As String = ""
        Dim dt As DataTable
        Dim dbCtl As DatabaseTalker

        '�e�[�u��:ID�f�[�^
        '�擾���ꂽ����:�p�X���[�h
        '�擾���ꂽ����:�������x��
        '�擾���ꂽ����:���b�N�̃t���O
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή� MOD�@START-----------
        sSql = " SELECT USER_ID,PASSWORD,AUTHORITY_LEVEL,LOCK_STS," _
            & " MST_FUNC1,MST_FUNC2,MST_FUNC3,MST_FUNC4,MST_FUNC5," _
            & " PRG_FUNC1,PRG_FUNC2,PRG_FUNC3,PRG_FUNC4,PRG_FUNC5," _
            & " MNT_FUNC1,MNT_FUNC2,MNT_FUNC3,MNT_FUNC4,MNT_FUNC5,MNT_FUNC6,MNT_FUNC7,MNT_FUNC8,MNT_FUNC9,MNT_FUNC10," _
            & " SYS_FUNC1,SYS_FUNC2,SYS_FUNC3,SYS_FUNC4,SYS_FUNC5 " _
            & " FROM M_USER " _
            & " WHERE USER_ID=" & "'" & sNowUID & "'"
        '-------Ver0.1�@�t�F�[�Y�Q�����Ή� MOD�@END-------------
        dbCtl = New DatabaseTalker
        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw ex
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
            sSql = Nothing
        End Try

        Return dt

    End Function
#End Region

#Region "���Y���[�U�����b�N����"

    ''' <summary> ���Y���[�U�����b�N����B</summary>
    ''' <param name="sNowUID">���͂��ꂽ�h�c�R�[�h</param>
    Private Sub lockID(ByVal sNowUID As String)
        Dim sSql As String = ""
        Dim dbCtl As DatabaseTalker

        sSql = "UPDATE M_USER SET LOCK_STS='1' WHERE USER_ID=" & "'" & sNowUID & "'"
        dbCtl = New DatabaseTalker
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSql)
            dbCtl.TransactionCommit()
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            Throw ex
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
            sSql = Nothing
        End Try
    End Sub

#End Region

#Region "���j���[��ʂɑJ��"

    ''' <summary>���j���[��ʂɑJ��</summary>
    ''' <param name="sUsrID">�h�c�R�[�h</param>
    ''' <param name="sAuth">����</param>
    Private Sub openMenu(ByVal sUsrID As String, ByVal sAuth As String)
        '�^�p�Ǘ����j���[��ʂɒl�������n���B
        FrmBase.Authority = sAuth
        '���j���[��ʂ�\������
        Dim hFrmMainMenu As New FrmMainMenu
        Me.Hide()
        hFrmMainMenu.ShowDialog()
        hFrmMainMenu.Dispose()
        GlobalVariables.UserId = ""
        txtPWD.Text = ""
        Me.Show()
        txtPWD.Focus()

    End Sub
#End Region

#Region "�`�F�b�N"
    ''' <summary>���b�N�A�E�g�`�F�b�N����B</summary>
    ''' <param name="sLockSts">���b�N�A�E�g���</param>
    ''' <returns>���b�N�A�E�g�ꍇ�Afalse��Ԃ��B�ł͂Ȃ���΁Atrue��Ԃ��B</returns>
    Private Function checkLock(ByVal sLockSts As String) As Boolean
        Dim bRet As Boolean = True

        '���[�U�����b�N���ǂ������`�F�b�N����
        If sLockSts = "1" Then
            Log.Info("�h�c�R�[�h�����b�N�A�E�g����Ă��܂��B")
            AlertBox.Show(Lexis.LoginFailedBecauseTheIdCodeHasBeenLockedOut)

            txtPWD.Text = ""
            txtID.Focus()
            bRet = False
        End If
        Return bRet

    End Function

    ''' <summary>�h�c�R�[�h���`�F�b�N����B</summary>
    ''' <param name="dt">���������h�c�R�[�h</param>
    ''' <returns>�h�c�R�[�h���Ȃ��̏ꍇ�Afalse��Ԃ��B�ł͂Ȃ���΁Atrue��Ԃ��B</returns>
    Private Function checkUser(ByVal dt As DataTable) As Boolean
        Dim nCount As Integer = 0
        Dim bRet As Boolean = True
        nCount = dt.Rows.Count

        If nCount = 0 Then
            Log.Info("���O�C�����ꂽ�h�c�R�[�h�͓o�^����Ă��܂���B")
            AlertBox.Show(Lexis.LoginFailedBecauseTheIdCodeIsIncorrect)
            txtPWD.Text = ""
            txtID.Focus()
            bRet = False
        End If
        Return bRet

    End Function
#End Region

End Class