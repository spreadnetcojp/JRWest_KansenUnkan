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

''' <summary>�G���A�o�^</summary>
''' <remarks>
''' �G���A���̂���͂��A�u�o�^�v�{�^�����N���b�N���邱�Ƃɂ��A
''' �ݒ���e���^�p�Ǘ��T�[�o�ɓo�^����B
''' </remarks>
Public Class FrmSysAreaMstAdd
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
    Friend WithEvents lblAreaname As System.Windows.Forms.Label
    Friend WithEvents lblAreano As System.Windows.Forms.Label
    Friend WithEvents txtAreaname As System.Windows.Forms.TextBox
    Friend WithEvents txtAreano As System.Windows.Forms.TextBox
    Friend WithEvents pnlPtnAdd As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnInsert = New System.Windows.Forms.Button()
        Me.lblAreaname = New System.Windows.Forms.Label()
        Me.lblAreano = New System.Windows.Forms.Label()
        Me.txtAreaname = New System.Windows.Forms.TextBox()
        Me.txtAreano = New System.Windows.Forms.TextBox()
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
        Me.btnStop.TabIndex = 3
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
        Me.btnInsert.TabIndex = 2
        Me.btnInsert.Text = "�o  �^"
        Me.btnInsert.UseVisualStyleBackColor = False
        '
        'lblAreaname
        '
        Me.lblAreaname.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreaname.Location = New System.Drawing.Point(53, 261)
        Me.lblAreaname.Name = "lblAreaname"
        Me.lblAreaname.Size = New System.Drawing.Size(110, 21)
        Me.lblAreaname.TabIndex = 4
        Me.lblAreaname.Text = "�G���A����"
        Me.lblAreaname.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAreano
        '
        Me.lblAreano.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblAreano.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreano.Location = New System.Drawing.Point(53, 121)
        Me.lblAreano.Name = "lblAreano"
        Me.lblAreano.Size = New System.Drawing.Size(110, 21)
        Me.lblAreano.TabIndex = 0
        Me.lblAreano.Text = "�G���ANo"
        Me.lblAreano.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtAreaname
        '
        Me.txtAreaname.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtAreaname.Location = New System.Drawing.Point(165, 261)
        Me.txtAreaname.MaxLength = 10
        Me.txtAreaname.Name = "txtAreaname"
        Me.txtAreaname.Size = New System.Drawing.Size(170, 22)
        Me.txtAreaname.TabIndex = 1
        '
        'txtAreano
        '
        Me.txtAreano.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtAreano.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtAreano.Location = New System.Drawing.Point(165, 121)
        Me.txtAreano.MaxLength = 2
        Me.txtAreano.Name = "txtAreano"
        Me.txtAreano.Size = New System.Drawing.Size(30, 22)
        Me.txtAreano.TabIndex = 0
        '
        'pnlPtnAdd
        '
        Me.pnlPtnAdd.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnAdd.Controls.Add(Me.lblAreano)
        Me.pnlPtnAdd.Controls.Add(Me.btnStop)
        Me.pnlPtnAdd.Controls.Add(Me.txtAreaname)
        Me.pnlPtnAdd.Controls.Add(Me.btnInsert)
        Me.pnlPtnAdd.Controls.Add(Me.txtAreano)
        Me.pnlPtnAdd.Controls.Add(Me.lblAreaname)
        Me.pnlPtnAdd.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnAdd.Name = "pnlPtnAdd"
        Me.pnlPtnAdd.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnAdd.TabIndex = 0
        '
        'FrmSysAreaMstAdd
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysAreaMstAdd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "�G���A�o�^"
        Me.pnlPtnAdd.ResumeLayout(False)
        Me.pnlPtnAdd.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�錾�̈�iPrivate�j"

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean

    ''' <summary>
    '''�o�^���[�U��ID���擾����B
    ''' </summary>
    Private sLoginID As String = ""

    ''' <summary>
    '''�@��R�[�h���擾����
    ''' </summary>
    Private sModelCode As String = ""

    Public Property ModelCode() As String
        Get
            Return sModelCode
        End Get
        Set(ByVal value As String)
            sModelCode = value
        End Set
    End Property
#End Region

#Region "�C�x���g"

    ''' <summary>
    ''' ���[�f�B���O�@���C���E�B���h�E
    ''' </summary>
    Private Sub FrmSysAreaMstAdd_load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim bRtn As Boolean = False
        LbEventStop = True      '�C�x���g�����n�e�e

        Try
            Log.Info("Method started.")

            '��ʔw�i�F�iBackColor�j��ݒ肷��
            pnlPtnAdd.BackColor = Config.BackgroundColor
            lblAreaname.BackColor = Config.BackgroundColor
            lblAreano.BackColor = Config.BackgroundColor

            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnInsert.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor
            Me.txtAreaname.ImeMode = Windows.Forms.ImeMode.Hiragana

            '�����ID���擾����
            sLoginID = GlobalVariables.UserId

            Me.txtAreano.Focus()

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("The form proc ended.")
            Else
                Log.Error("The form proc abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If

            LbEventStop = False '�C�x���g�����n�m
        End Try

    End Sub
    ''' <summary>
    ''' �u�o�^�v�{�^������������ƁADB�֐V�����G���A���o�^�����
    ''' </summary>
    Private Sub btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsert.Click

        Try
            '�o�^�{�^�������B
            FrmBase.LogOperation(sender, e, Text)
            If CheckAll() Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyInsert).Equals(Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Text)                      '�͂��{�^������
                    Call WaitCursor(True)
                    Call AddArea()
                    FrmBase.LogOperation(Lexis.InsertCompleted, Text) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B '�o�^����������ɏI�����܂����B
                    AlertBox.Show(Lexis.InsertCompleted)
                    FrmBase.LogOperation(Lexis.OkButtonClicked, Text)                       'OK�{�^������
                    Me.Close()
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked, Text)                       '�������{�^������
                    btnInsert.Select()
                End If
            End If

        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
            btnInsert.Select()
            Exit Sub

        Catch ex As Exception

            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.InsertFailed)      '�o�^�����s
            btnInsert.Select()
            Exit Sub
        Finally

            Call WaitCursor(False)

        End Try

    End Sub

    ''' <summary>
    ''' �u�I���v�{�^������������ƁA�{��ʂ��I�������B 
    ''' </summary>
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStop.Click
        '�I���{�^�������B
        FrmBase.LogOperation(sender, e, Text)
        Me.Close()
    End Sub

    ''' <summary>
    ''' �u�G���ANo�v�̓��͒l����������
    ''' </summary>
    Private Sub txtAreaNo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtAreano.KeyPress
        Select Case e.KeyChar
            Case "0".ToCharArray To "9".ToCharArray
            Case Chr(8)
            Case Else
                e.Handled = True
        End Select
    End Sub

    ''' <summary>
    ''' �u�G���A���́v�̓��͒l����������
    ''' </summary>
    Private Sub txtAreaName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtAreaname.KeyPress

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
    ''' �u�o�^�v�{�^�������������ۂɂ��ׂẴR���g���[���̒l���`�F�b�N����B
    ''' </summary>
    ''' <remarks>�f�[�^���@�t���O</remarks>
    Private Function CheckAll() As Boolean
        '���֐��̖߂�l
        Dim bRetAll As Boolean = False

        If System.String.IsNullOrEmpty(Me.txtAreano.Text) Then
            '���͒l���s���ł��B�G���ANo�̒l���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, Me.lblAreano.Text)
            Me.txtAreano.Focus()
        ElseIf Me.txtAreano.Text.Length <> 2 Then
            '���͒l���s���ł��B�G���ANo�̒�����2�����łȂ��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaNo)
            Me.txtAreano.Focus()
        ElseIf CheckIsExist(Me.txtAreano.Text) Then
            '���͒l���s���ł��B�G���ANoXX�͊��ɓo�^����Ă��܂��B
            AlertBox.Show(Lexis.TheAreaNoAlreadyExists, Me.txtAreano.Text)
            Me.txtAreano.Focus()
        ElseIf System.String.IsNullOrEmpty(Me.txtAreaname.Text) Then
            '���͒l���s���ł��B�G���A���̂̒l���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, Me.lblAreaname.Text)
            Me.txtAreaname.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtAreaname.Text.ToString, 10, 2, True) = -4 Then
            '���͒l���s���ł��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            Me.txtAreaname.Focus()
        ElseIf Me.txtAreaname.Text.ToString.Trim() = "" Then
            '���͒l���s���ł��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            Me.txtAreaname.Focus()
        ElseIf CheckAreaCount() Then
            '�@��P�ʂœo�^�ł���G���A�����𒴂��Ă��܂��B
            AlertBox.Show(Lexis.AreaNoIsFull)
            Me.txtAreano.Focus()
        Else
            bRetAll = True
        End If

        Return bRetAll
    End Function

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

    ''' <summary>
    ''' DB�֐ݒ肳�ꂽ�G���A�����C���T�[�g����B
    ''' </summary>
    Private Sub AddArea()

        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker
        Dim sBuilder As New StringBuilder

        '�G���ANo�A�G���A���̂��擾����B
        Dim sAreanNo As String = txtAreano.Text
        Dim sAreaName As String = txtAreaname.Text

        '�[��ID
        Dim sClient As String = Config.MachineName
        dbCtl = New DatabaseTalker

        Try
            sBuilder.AppendLine(" INSERT INTO M_AREA_DATA (")
            sBuilder.AppendLine(" INSERT_DATE,")
            sBuilder.AppendLine(" INSERT_USER_ID,")
            sBuilder.AppendLine(" INSERT_MACHINE_ID,")
            sBuilder.AppendLine(" UPDATE_DATE,")
            sBuilder.AppendLine(" UPDATE_USER_ID,")
            sBuilder.AppendLine(" UPDATE_MACHINE_ID,")
            sBuilder.AppendLine(" MODEL_CODE,")
            sBuilder.AppendLine(" AREA_NO,")
            sBuilder.AppendLine(" AREA_NAME)")
            sBuilder.AppendLine(" VALUES(GETDATE(),")
            sBuilder.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine("GETDATE(),")
            sBuilder.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sModelCode) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sAreanNo) & ",")
            sBuilder.AppendLine(Utility.SetSglQuot(sAreaName) & ")")
            sSQL = sBuilder.ToString

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw New DatabaseException
            btnInsert.Select()
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

    End Sub

    ''' <summary>
    ''' �G���ANo�̏d���`�F�b�N
    ''' </summary>
    ''' <returns>true:�G���ANo���d���ł��Bfalse:�G���AN���̏d�����Ȃ��B</returns>
    Private Function CheckIsExist(ByVal sAreaNo As String) As Boolean
        Dim Flag As Boolean = False
        Dim sSQL As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        Try
            sSQL = String.Format("SELECT COUNT(1) FROM M_AREA_DATA WHERE AREA_NO = {0} AND MODEL_CODE = {1}", _
                                 Utility.SetSglQuot(txtAreano.Text), Utility.SetSglQuot(sModelCode))

            nRtn = FrmBase.BaseSqlDataTableFill(sSQL, dtMstTable)

            If nRtn = -9 Then
                Throw New OPMGException()
            End If

            If Convert.ToInt64(dtMstTable.Rows(0)(0)) = 1 Then
                Flag = True
            Else
                Flag = False
            End If

        Catch ex As OPMGException
            Throw New OPMGException(ex)
        End Try
        Return Flag
    End Function

    ''' <summary>
    ''' �o�^�ł���ő�G���A���`�F�b�N
    ''' </summary>
    ''' <returns>true:�G���A�����𒴂��Ă��܂��Bfalse:�G���A�����𒴂��Ȃ��B</returns>
    Private Function CheckAreaCount() As Boolean
        Dim Flag As Boolean = False
        Dim sSQL As String = ""
        Dim nRtn As Integer
        Dim dtMstTable As New DataTable
        Try
            sSQL = String.Format("SELECT COUNT(1) FROM M_AREA_DATA WHERE MODEL_CODE = {0}", Utility.SetSglQuot(sModelCode))
            nRtn = FrmBase.BaseSqlDataTableFill(sSQL, dtMstTable)

            If nRtn = -9 Then
                Throw New OPMGException()
            End If

            If Convert.ToInt64(dtMstTable.Rows(0)(0)) >= 10 Then
                Flag = True
            End If

        Catch ex As OPMGException
            Throw New OPMGException(ex)
        End Try
        Return Flag
    End Function

#End Region

End Class
