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
Imports System.Text

''' <summary>�G���A�C��</summary>
''' <remarks>
''' �G���A���̂�ύX���A�u�C���v�{�^�����N���b�N���邱�Ƃɂ��A
''' �ݒ���e���^�p�Ǘ��T�[�o�ɓo�^����B
''' </remarks>
Public Class FrmSysAreaMstUpdate
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
    Friend WithEvents txtAreaname As System.Windows.Forms.TextBox
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents txtAreano As System.Windows.Forms.Label
    Friend WithEvents lblAreaname As System.Windows.Forms.Label
    Friend WithEvents lblAreano As System.Windows.Forms.Label
    Friend WithEvents pnlAreaUpdate As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtAreaname = New System.Windows.Forms.TextBox()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.txtAreano = New System.Windows.Forms.Label()
        Me.lblAreaname = New System.Windows.Forms.Label()
        Me.lblAreano = New System.Windows.Forms.Label()
        Me.pnlAreaUpdate = New System.Windows.Forms.Panel()
        Me.pnlAreaUpdate.SuspendLayout()
        Me.SuspendLayout()
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
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.Color.Silver
        Me.btnUpdate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(426, 116)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(90, 32)
        Me.btnUpdate.TabIndex = 2
        Me.btnUpdate.Text = "�C  ��"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'txtAreano
        '
        Me.txtAreano.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtAreano.Location = New System.Drawing.Point(165, 121)
        Me.txtAreano.Name = "txtAreano"
        Me.txtAreano.Size = New System.Drawing.Size(50, 21)
        Me.txtAreano.TabIndex = 4
        Me.txtAreano.Text = "XX"
        Me.txtAreano.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblAreaname
        '
        Me.lblAreaname.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblAreaname.Location = New System.Drawing.Point(53, 261)
        Me.lblAreaname.Name = "lblAreaname"
        Me.lblAreaname.Size = New System.Drawing.Size(110, 21)
        Me.lblAreaname.TabIndex = 0
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
        Me.lblAreano.TabIndex = 3
        Me.lblAreano.Text = "�G���ANo"
        Me.lblAreano.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlAreaUpdate
        '
        Me.pnlAreaUpdate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlAreaUpdate.Controls.Add(Me.lblAreaname)
        Me.pnlAreaUpdate.Controls.Add(Me.btnStop)
        Me.pnlAreaUpdate.Controls.Add(Me.txtAreaname)
        Me.pnlAreaUpdate.Controls.Add(Me.btnUpdate)
        Me.pnlAreaUpdate.Controls.Add(Me.lblAreano)
        Me.pnlAreaUpdate.Controls.Add(Me.txtAreano)
        Me.pnlAreaUpdate.Location = New System.Drawing.Point(0, 0)
        Me.pnlAreaUpdate.Name = "pnlAreaUpdate"
        Me.pnlAreaUpdate.Size = New System.Drawing.Size(594, 418)
        Me.pnlAreaUpdate.TabIndex = 0
        '
        'FrmSysAreaMstUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlAreaUpdate)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysAreaMstUpdate"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "�G���A�C��"
        Me.pnlAreaUpdate.ResumeLayout(False)
        Me.pnlAreaUpdate.PerformLayout()
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

    '�o�^���[�U��ID���擾����B
    Private sLoginID As String = ""

    '�G���ANo���擾����B
    Private sAreaNo As String = ""

    Public Property AreaNo() As String
        Get
            Return sAreaNo
        End Get
        Set(ByVal value As String)
            sAreaNo = value
        End Set
    End Property

    '�G���A���̂��擾����B
    Private sAreaName As String = ""

    '�@���ʂ��擾����B
    Private sModelCode As String = ""

    Public Property ModelCode() As String
        Get
            Return sModelCode
        End Get
        Set(ByVal value As String)
            sModelCode = value
        End Set
    End Property

    '�X�V����
    Private oldDate As String = ""

    '�X�V����
    Private newDate As String = ""

#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>�G���A�C����ʂ̃f�[�^����������</summary>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e
        Dim dt As New DataTable
        Dim nRtn As Integer
        Dim sSql As String = ""

        Try
            Log.Info("Method started.")

            '�����ID���擾����
            sLoginID = GlobalVariables.UserId

            '�f�[�^���擾����B
            sSql = LfGetSelectString()
            nRtn = FrmBase.BaseSqlDataTableFill(sSql, dt)
            Select Case nRtn
                Case -9             '�c�a�I�[�v���G���[
                    AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                    Return bRet
                Case Else
                    If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                        '���������Ɉ�v����f�[�^�͑��݂��܂���B
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Return bRet
                    Else
                        sAreaName = dt.Rows(0).Item("AREA_NAME").ToString
                        oldDate = dt.Rows(0).Item("UPDATE_DATE").ToString
                    End If
            End Select

            bRet = True

        Catch ex As Exception
            '��ʕ\�������Ɏ��s���܂����B
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
    Private Sub FrmSysAreaMstUpdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrmData() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

            '��ʔw�i�F�iBackColor�j��ݒ肷��
            pnlAreaUpdate.BackColor = Config.BackgroundColor
            lblAreaname.BackColor = Config.BackgroundColor
            lblAreano.BackColor = Config.BackgroundColor
            txtAreano.BackColor = Config.BackgroundColor

            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnUpdate.BackColor = Config.ButtonColor
            btnStop.BackColor = Config.ButtonColor

            '�G���ANo��\������B
            Me.txtAreano.Text = sAreaNo
            '�G���A���̂�\������B
            Me.txtAreaname.Text = sAreaName

            Me.txtAreaname.ImeMode = Windows.Forms.ImeMode.Hiragana
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' �u�C���v�{�^������������ƁADB�֐ݒ肳�ꂽ�G���A�����C������B
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim nRtn As Integer

        Try
            LbEventStop = True
            '�C���{�^��������
            FrmBase.LogOperation(sender, e, Me.Text)

            If CheckAll() = True Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUpdate).Equals(System.Windows.Forms.DialogResult.Yes) Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked, Me.Text)
                    Call WaitCursor(True)
                    '�f�[�^���擾����B
                    sSql = LfGetSelectString()
                    nRtn = FrmBase.BaseSqlDataTableFill(sSql, dt)
                    Select Case nRtn
                        Case -9             '�c�a�I�[�v���G���[
                            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
                            Exit Sub
                        Case Else
                            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                                '���������Ɉ�v����f�[�^�͑��݂��܂���B
                                AlertBox.Show(Lexis.CompetitiveOperationDetected)
                                Exit Sub
                            Else
                                newDate = dt.Rows(0).Item("UPDATE_DATE").ToString
                            End If
                    End Select
                    
                    '�r���`�F�b�N
                    If Not oldDate.Equals(newDate) Then
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Exit Sub
                    End If

                    '�X�V����
                    Call UpdateArea()
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
        '�I���{�^��������
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

    ''' <summary>�u�G���A���́v�̓��͒l����������</summary>
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
    ''' �u�C���v�{�^�������������ۂɂ��ׂẴR���g���[���̒l���`�F�b�N����B
    ''' </summary>
    ''' <returns>�f�[�^���@�t���O</returns>
    Private Function CheckAll() As Boolean

        '���֐��̖߂�l
        Dim bRetAll As Boolean = False

        If System.String.IsNullOrEmpty(txtAreaname.Text) Then
            '�G���A����:�l���k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblAreaname.Text)
            txtAreaname.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtAreaname.Text.ToString, 10, 2, True) = -4 Then
            '���͒l���s���ł��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            txtAreaname.Focus()
        ElseIf Me.txtAreaname.Text.ToString.Trim() = "" Then
            '���͒l���s���ł��B�S�p�X�y�[�X�̂ݓ��͂����ꍇ
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForAreaName)
            txtAreaname.Focus()
        Else
            bRetAll = True
        End If

        Return bRetAll

    End Function

    ''' <summary>
    ''' DB�֐ݒ肳�ꂽ�G���A�����C������B
    ''' </summary>
    Private Sub UpdateArea()

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        Dim dbCtl As DatabaseTalker

        '�[��ID
        Dim sClient As String = Config.MachineName
        dbCtl = New DatabaseTalker

        Try
            '�G���A���̂��擾����B
            sAreaName = txtAreaname.Text

            sBuilder.AppendLine(" UPDATE M_AREA_DATA SET UPDATE_DATE = GETDATE(),")
            sBuilder.AppendLine(" UPDATE_USER_ID = " & Utility.SetSglQuot(sLoginID.ToString) & ",")
            sBuilder.AppendLine(" UPDATE_MACHINE_ID = " & Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(" AREA_NAME = " & Utility.SetSglQuot(sAreaName))
            sBuilder.AppendLine(" WHERE MODEL_CODE = " & Utility.SetSglQuot(sModelCode))
            sBuilder.AppendLine(" AND AREA_NO = " & Utility.SetSglQuot(sAreaNo))

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

        '�f�[�^���擾����B
        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Try
            sBuilder.AppendLine(" SELECT AREA_NAME, UPDATE_DATE")
            sBuilder.AppendLine("  FROM M_AREA_DATA  ")
            sBuilder.AppendLine(" WHERE MODEL_CODE = " & Utility.SetSglQuot(sModelCode))
            sBuilder.AppendLine(" AND AREA_NO = " & Utility.SetSglQuot(sAreaNo))
            sSQL = sBuilder.ToString()

            Return sSQL

        Catch ex As Exception
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
