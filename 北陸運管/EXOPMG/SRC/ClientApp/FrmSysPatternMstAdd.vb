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
Imports System.Data.SqlClient
Imports System.Text

''' <summary>�p�^�[���o�^</summary>
''' <remarks>
''' �p�^�[�����̂���͂��A�u�o�^�v�{�^�����N���b�N���邱�Ƃɂ��A
''' �ݒ���e���^�p�Ǘ��T�[�o�ɓo�^����B
''' </remarks>
Public Class FrmSysPatternMstAdd
    Inherits System.Windows.Forms.Form

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    '�t�H�[�����R���|�[�l���g�̈ꗗ���N���[���A�b�v���邽�߂� dispose ���I�[�o�[���C�h���܂��B
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    '����: �ȉ��̃v���V�[�W���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g�p���ĕύX�ł��܂��B  
    '�R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnInsert As System.Windows.Forms.Button
    Friend WithEvents lblPtnNameTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNoTitle As System.Windows.Forms.Label
    Friend WithEvents txtPatternname As System.Windows.Forms.TextBox
    Friend WithEvents txtPatternno As System.Windows.Forms.TextBox
    Friend WithEvents pnlPtnAdd As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.lblPtnNameTitle = New System.Windows.Forms.Label()
        Me.lblPtnNoTitle = New System.Windows.Forms.Label()
        Me.txtPatternname = New System.Windows.Forms.TextBox()
        Me.txtPatternno = New System.Windows.Forms.TextBox()
        Me.pnlPtnAdd = New System.Windows.Forms.Panel()
        Me.pnlPtnAdd.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnStop
        '
        Me.btnStop.BackColor = System.Drawing.Color.Silver
        Me.btnStop.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnStop.Location = New System.Drawing.Point(426, 255)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(90, 32)
        Me.btnStop.TabIndex = 4
        Me.btnStop.Text = "�I�@��"
        Me.btnStop.UseVisualStyleBackColor = False
        '
        'btnInsert
        '
        Me.btnInsert.BackColor = System.Drawing.Color.Silver
        Me.btnInsert.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnInsert.Location = New System.Drawing.Point(426, 116)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(90, 32)
        Me.btnInsert.TabIndex = 3
        Me.btnInsert.Text = "�o  �^"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'lblPtnNameTitle
        '
        Me.lblPtnNameTitle.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNameTitle.Location = New System.Drawing.Point(53, 261)
        Me.lblPtnNameTitle.Name = "lblPtnNameTitle"
        Me.lblPtnNameTitle.Size = New System.Drawing.Size(110, 21)
        Me.lblPtnNameTitle.TabIndex = 4
        Me.lblPtnNameTitle.Text = "�p�^�[������"
        Me.lblPtnNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnNoTitle
        '
        Me.lblPtnNoTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblPtnNoTitle.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNoTitle.Location = New System.Drawing.Point(53, 121)
        Me.lblPtnNoTitle.Name = "lblPtnNoTitle"
        Me.lblPtnNoTitle.Size = New System.Drawing.Size(110, 21)
        Me.lblPtnNoTitle.TabIndex = 0
        Me.lblPtnNoTitle.Text = "�p�^�[��No"
        Me.lblPtnNoTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtPatternname
        '
        Me.txtPatternname.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPatternname.Location = New System.Drawing.Point(165, 261)
        Me.txtPatternname.MaxLength = 10
        Me.txtPatternname.Name = "txtPatternname"
        Me.txtPatternname.Size = New System.Drawing.Size(170, 22)
        Me.txtPatternname.TabIndex = 2
        '
        'txtPatternno
        '
        Me.txtPatternno.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPatternno.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtPatternno.Location = New System.Drawing.Point(165, 121)
        Me.txtPatternno.MaxLength = 2
        Me.txtPatternno.Name = "txtPatternno"
        Me.txtPatternno.Size = New System.Drawing.Size(30, 22)
        Me.txtPatternno.TabIndex = 1
        '
        'pnlPtnAdd
        '
        Me.pnlPtnAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnAdd.Controls.Add(Me.lblPtnNoTitle)
        Me.pnlPtnAdd.Controls.Add(Me.btnStop)
        Me.pnlPtnAdd.Controls.Add(Me.txtPatternname)
        Me.pnlPtnAdd.Controls.Add(Me.btnInsert)
        Me.pnlPtnAdd.Controls.Add(Me.txtPatternno)
        Me.pnlPtnAdd.Controls.Add(Me.lblPtnNameTitle)
        Me.pnlPtnAdd.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnAdd.Name = "pnlPtnAdd"
        Me.pnlPtnAdd.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnAdd.TabIndex = 0
        '
        'FrmSysPatternMstAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysPatternMstAdd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "�p�^�[���o�^"
        Me.pnlPtnAdd.ResumeLayout(False)
        Me.pnlPtnAdd.PerformLayout()
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

    ''' <summary>
    ''' ��ʖ�
    ''' </summary>
    Private ReadOnly FormTitle As String = "�p�^�[���ݒ�o�^"

    '�o�^���[�U��ID���擾����B
    Private sLoginID As String = ""
#End Region

#Region "�錾�̈�iPublic�j"
    Public Property LoginID() As String
        Get
            Return sLoginID
        End Get
        Set(ByVal value As String)
            sLoginID = value
        End Set
    End Property

    '�}�X�^��ʂ��擾����B
    Private sKind As String = ""

    Public Property Kind() As String
        Get
            Return sKind
        End Get
        Set(ByVal value As String)
            sKind = value
        End Set
    End Property

    '���������̃t���O���擾����B
    Private bMstChecked As Boolean = False

    Public Property CheckFlag() As Boolean
        Get
            Return bMstChecked
        End Get
        Set(ByVal value As Boolean)
            bMstChecked = value
        End Set
    End Property

    '�@��R�[�h���擾����
    Private sModelCode As String = ""

    Public Property ModelCode() As String
        Get
            Return sModelCode
        End Get
        Set(ByVal value As String)
            sModelCode = value
        End Set
    End Property

    '�@��^�C�v���擾����
    Private sMachType As String = ""

    Public Property MachType() As String
        Get
            Return sMachType
        End Get
        Set(ByVal value As String)
            sMachType = value
        End Set
    End Property
#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' ���[�f�B���O�@���C���E�B���h�E
    ''' </summary>
    Private Sub FrmSysPatternMstAdd_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        LbEventStop = True

        Try
            Log.Info("Method started.")

            '��ʔw�i�F�iBackColor�j��ݒ肷��
            pnlPtnAdd.BackColor = Config.BackgroundColor
            lblPtnNameTitle.BackColor = Config.BackgroundColor
            lblPtnNoTitle.BackColor = Config.BackgroundColor

            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnInsert.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor
            Me.txtPatternname.ImeMode = Windows.Forms.ImeMode.Hiragana

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method proc ended.")
            Else
                Log.Error("Method proc abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If

            LbEventStop = False '�C�x���g�����n�m
        End Try
    End Sub
#End Region

#Region "�C�x���g"
    ''' <summary>
    ''' �u�o�^�v�{�^������������ƁADB�֐V�����p�^�[�����o�^�����
    ''' </summary>
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click
        Try
            '�o�^�{�^�������B
            FrmBase.LogOperation(sender, e, Me.Text)
            If CheckAll() Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInsert).Equals(System.Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Text)     'Yes�{�^������
                    Call waitCursor(True)

                    If addPattern() > 0 Then
                        '�o�^����������ɏI�����܂����B
                        Log.Info("Insert finished.")
                        If AlertBox.Show(AlertBoxAttr.OK, Lexis.InsertCompleted).Equals(System.Windows.Forms.DialogResult.OK) Then
                            FrmBase.LogOperation(Lexis.OkButtonClicked, Text)
                            Me.Close()
                        End If
                    End If
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked, Text)
                    btnInsert.Select()
                End If
            End If

        Catch ex As Exception

            Log.Fatal("Unwelcome Exception caught.", ex)  '�\�����ʃG���[���������܂����B
            AlertBox.Show(Lexis.InsertFailed)
            btnInsert.Select()
            Exit Sub
        Finally
            Call waitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �u�I���v�{�^������������ƁA�{��ʂ��I�������B 
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        '�I���{�^�������B
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

    ''' <summary>�u�p�^�[��No�v�̓��͒l����������</summary>
    Private Sub txtPtnNo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPatternno.KeyPress
        Select Case e.KeyChar
            Case "0".ToCharArray To "9".ToCharArray
            Case Chr(8)
            Case Else
                e.Handled = True
        End Select
    End Sub

    ''' <summary>�u�p�^�[�����́v�̓��͒l����������</summary>
    Private Sub txtPtnName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPatternname.KeyPress

        Dim Encode As Encoding
        Encode = Encoding.GetEncoding("Shift_JIS")

        If e.KeyChar.ToString.Length = Encode.GetByteCount(e.KeyChar.ToString) / 2 Then
            e.Handled = False
        ElseIf e.KeyChar = Chr(8) Then
            e.Handled = False
        Else
            e.Handled = True
        End If

    End Sub
#End Region

#Region "���\�b�h�iPrivate�j"
    ''' <summary>
    ''' �o�^�v�{�^�������������ۂɂ��ׂẴR���g���[���̒l���`�F�b�N����B
    ''' </summary>
    ''' <remarks>�f�[�^���@�t���O</remarks>
    Private Function CheckAll() As Boolean
        '���֐��̖߂�l
        Dim bFlag As Boolean = False
        If System.String.IsNullOrEmpty(Me.txtPatternno.Text) Then
            '���͒l���s���ł��B�p�^�[��No�̒l���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblPtnNoTitle.Text)
            Me.txtPatternno.Focus()
        ElseIf Me.txtPatternno.Text.Length <> 2 Then
            '���͒l���s���ł��B�p�^�[��No�̒�����2�����łȂ��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternNo)
            Me.txtPatternno.Focus()
        ElseIf CheckIsExist(Me.txtPatternno.Text) Then
            '���͒l���s���ł��B�p�^�[��NoXX�͊��ɓo�^����Ă��܂��B
            AlertBox.Show(Lexis.ThePatternNoAlreadyExists, Me.txtPatternno.Text)
            Me.txtPatternno.Focus()
        ElseIf System.String.IsNullOrEmpty(Me.txtPatternname.Text) Then
            '���͒l���s���ł��B�p�^�[�����̂̒l���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblPtnNameTitle.Text)
            Me.txtPatternname.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtPatternname.Text.ToString, 10, 2, True) = -4 Then
            '���͒l���s���ł��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            Me.txtPatternname.Focus()
        ElseIf Me.txtPatternname.Text.ToString.Trim() = "" Then
            '���͒l���s���ł��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            Me.txtPatternname.Focus()
        ElseIf CheckMachKennsu(sModelCode) = True Then
            '�����𒴂��Ă��܂�
            AlertBox.Show(Lexis.PatternNoIsFull)
            Me.txtPatternno.Focus()
        Else
            bFlag = True
        End If

        Return bFlag
    End Function
    ''' <summary>
    ''' �@��P�ʍő匏���`�F�b�N
    ''' </summary>
    ''' <returns>true:�����𒴂��Ă��܂��Bfalse:�����𒴂����Ȃ��B</returns>
    Private Function CheckMachKennsu(ByVal sModelCode As String) As Boolean
        Dim Flag As Boolean = False
        Dim sBuilder As New StringBuilder
        Dim dbCtl As DatabaseTalker = New DatabaseTalker
        Dim Kennsu As Integer

        Try
            sBuilder.AppendLine("SELECT COUNT(1) FROM M_PATTERN_DATA WHERE MODEL_CODE= " + Utility.SetSglQuot(sModelCode))
            dbCtl.ConnectOpen()
            Kennsu = CInt(dbCtl.ExecuteSQLToReadScalar(sBuilder.ToString))

            If Kennsu > 100 Then
                Flag = True
            Else
                Flag = False
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)  '�\�����ʃG���[���������܂����B
            Flag = False
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
        Return Flag
    End Function

    ''' <summary>
    ''' �p�^�[��No�̏d���`�F�b�N
    ''' </summary>
    ''' <returns>true:�p�^�[��No���d���ł��Bfalse:�p�^�[��No���̏d�����Ȃ��B</returns>
    Private Function CheckIsExist(ByVal PatternNo As String) As Boolean
        Dim Flag As Boolean = False
        Dim sBuilder As New StringBuilder
        Dim dtMstTable As DataTable = New DataTable
        Dim dbCtl As DatabaseTalker = New DatabaseTalker
        Dim iNum As Integer
        Try
            sBuilder.AppendLine(String.Format("SELECT COUNT(1) FROM M_PATTERN_DATA WHERE PATTERN_NO = {0} AND MODEL_CODE={1} AND MST_KIND={2}", _
                                                     Utility.SetSglQuot(txtPatternno.Text), Utility.SetSglQuot(sModelCode), Utility.SetSglQuot(sKind)))

            dbCtl.ConnectOpen()
            iNum = CInt(dbCtl.ExecuteSQLToReadScalar(sBuilder.ToString))
            If iNum = 1 Then
                Flag = True
            Else
                Flag = False
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)  '�\�����ʃG���[���������܂����B
            Flag = False
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
        End Try
        Return Flag
    End Function

    ''' <summary>
    ''' DB�֐ݒ肳�ꂽ�p�^�[�������C���T�[�g����B
    ''' </summary>
    Private Function addPattern() As Integer

        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker
        Dim iRetrun As Integer
        '�p�^�[��No�A�p�^�[�����̂��擾����B
        Dim sPatternNo As String = txtPatternno.Text
        Dim sPatternName As String = txtPatternname.Text
        Dim sClient As String

        dbCtl = New DatabaseTalker
        Try
            '�����ID���擾����B
            sLoginID = GlobalVariables.UserId
            sClient = Config.MachineName

            sSQL = " INSERT INTO M_PATTERN_DATA(" _
                     & " INSERT_DATE," _
                     & " INSERT_USER_ID," _
                     & " INSERT_MACHINE_ID," _
                     & " UPDATE_DATE," _
                     & " UPDATE_USER_ID, " _
                     & " UPDATE_MACHINE_ID, " _
                     & " MODEL_CODE," _
                     & " MST_KIND," _
                     & " PATTERN_NO," _
                     & " PATTERN_NAME)" _
                     & " VALUES(GETDATE()," _
                     & Utility.SetSglQuot(sLoginID) & "," _
                     & Utility.SetSglQuot(sClient) & "," _
                     & "GETDATE()," _
                     & Utility.SetSglQuot(sLoginID) & "," _
                     & Utility.SetSglQuot(sClient) & "," _
                     & Utility.SetSglQuot(sModelCode) & "," _
                     & Utility.SetSglQuot(sKind) & "," _
                     & Utility.SetSglQuot(sPatternNo) & "," _
                     & Utility.SetSglQuot(sPatternName) & ")"

            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            iRetrun = dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()

        Catch ex As DatabaseException

            '���ɓo�^����Ă��܂��B
            If TypeOf ex.InnerException Is SqlException Then
                If (CType(ex.InnerException, SqlException).Number = 2627) Then
                    Call waitCursor(False)
                    Me.txtPatternno.Focus()

                End If
            End If
            dbCtl.TransactionRollBack()
            'DB�ڑ��Ɏ��s
            Log.Fatal("Unwelcome Exception caught.", ex)   '�o�^�����Ɏ��s���܂����B
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnInsert.Select()
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try
        Return iRetrun
    End Function
#End Region

#Region "�J�[�\���҂�"

    ''' <summary>
    ''' �J�[�\���҂�
    ''' </summary>
    ''' <param name="bWait">true:�҂��J�n�@false:�҂��I��</param>
    ''' <remarks>�J�[�\���������v�ɂȂ�</remarks>
    Private Sub waitCursor(Optional ByVal bWait As Boolean = True)

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
