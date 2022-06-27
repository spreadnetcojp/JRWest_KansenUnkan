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

''' <summary>�p�^�[���C��</summary>
''' <remarks>
''' �p�^�[�����̂�ύX���A�u�C���v�{�^�����N���b�N���邱�Ƃɂ��A
''' �ݒ���e���^�p�Ǘ��T�[�o�ɓo�^����B
''' </remarks>
Public Class FrmSysPatternMstUpdate
    Inherits System.Windows.Forms.Form

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    '�t�H�[�����R���|�[�l���g�̈ꗗ���N���[���A�b�v���邽�߂� dispose ���I�[�o�[���C�h���܂��B
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    '����: �ȉ��̃v���V�[�W���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g�p���ĕύX�ł��܂��B  
    '�R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    Friend WithEvents txtPtnName As System.Windows.Forms.TextBox
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents lblPtnNo As System.Windows.Forms.Label
    Friend WithEvents lblPtnNameTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNoTitle As System.Windows.Forms.Label
    Friend WithEvents pnlPtnUpdate As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtPtnName = New System.Windows.Forms.TextBox()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.lblPtnNo = New System.Windows.Forms.Label()
        Me.lblPtnNameTitle = New System.Windows.Forms.Label()
        Me.lblPtnNoTitle = New System.Windows.Forms.Label()
        Me.pnlPtnUpdate = New System.Windows.Forms.Panel()
        Me.pnlPtnUpdate.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtPtnName
        '
        Me.txtPtnName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtPtnName.Location = New System.Drawing.Point(165, 261)
        Me.txtPtnName.MaxLength = 10
        Me.txtPtnName.Name = "txtPtnName"
        Me.txtPtnName.Size = New System.Drawing.Size(170, 22)
        Me.txtPtnName.TabIndex = 0
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.Silver
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(426, 255)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(90, 32)
        Me.btnReturn.TabIndex = 1
        Me.btnReturn.Text = "�I  ��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnUpdate
        '
        Me.btnUpdate.BackColor = System.Drawing.Color.Silver
        Me.btnUpdate.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnUpdate.Location = New System.Drawing.Point(426, 116)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(90, 32)
        Me.btnUpdate.TabIndex = 0
        Me.btnUpdate.Text = "�C  ��"
        Me.btnUpdate.UseVisualStyleBackColor = False
        '
        'lblPtnNo
        '
        Me.lblPtnNo.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNo.Location = New System.Drawing.Point(165, 121)
        Me.lblPtnNo.Name = "lblPtnNo"
        Me.lblPtnNo.Size = New System.Drawing.Size(50, 21)
        Me.lblPtnNo.TabIndex = 3
        Me.lblPtnNo.Text = "XX"
        Me.lblPtnNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPtnNameTitle
        '
        Me.lblPtnNameTitle.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnNameTitle.Location = New System.Drawing.Point(53, 261)
        Me.lblPtnNameTitle.Name = "lblPtnNameTitle"
        Me.lblPtnNameTitle.Size = New System.Drawing.Size(110, 21)
        Me.lblPtnNameTitle.TabIndex = 2
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
        'pnlPtnUpdate
        '
        Me.pnlPtnUpdate.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnUpdate.Controls.Add(Me.lblPtnNameTitle)
        Me.pnlPtnUpdate.Controls.Add(Me.btnReturn)
        Me.pnlPtnUpdate.Controls.Add(Me.txtPtnName)
        Me.pnlPtnUpdate.Controls.Add(Me.btnUpdate)
        Me.pnlPtnUpdate.Controls.Add(Me.lblPtnNoTitle)
        Me.pnlPtnUpdate.Controls.Add(Me.lblPtnNo)
        Me.pnlPtnUpdate.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnUpdate.Name = "pnlPtnUpdate"
        Me.pnlPtnUpdate.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnUpdate.TabIndex = 0
        '
        'FrmSysPatternMstUpdate
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnUpdate)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysPatternMstUpdate"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "�p�^�[���C��"
        Me.pnlPtnUpdate.ResumeLayout(False)
        Me.pnlPtnUpdate.PerformLayout()
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
    Private ReadOnly FormTitle As String = "�p�^�[���ݒ�C��"

    '�C�����[�U��ID���擾����B
    Private sLoginID As String = ""
    Public Property LoginID() As String
        Get
            Return sLoginID
        End Get
        Set(ByVal value As String)
            sLoginID = value
        End Set
    End Property

    '�o�^�[��No���擾����B
    Private sPatternNo As String = ""

    Public Property PatternNo() As String
        Get
            Return sPatternNo
        End Get
        Set(ByVal value As String)
            sPatternNo = value
        End Set
    End Property

    '�o�^�[��Name���擾����B
    Private sPatternName As String = ""

    Public Property PatternName() As String
        Get
            Return sPatternName
        End Get
        Set(ByVal value As String)
            sPatternName = value
        End Set
    End Property

    '�@��code���擾����B
    Private sModelcode As String = ""

    Public Property Modelcode() As String
        Get
            Return sModelcode
        End Get
        Set(ByVal value As String)
            sModelcode = value
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

    '�o�^�[���l���擾����B
    Private sPattern As String = ""

    '�X�V����
    Private oldDate As String = ""

    '�X�V����
    Private newDate As String = ""

#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' Pattern�f�[�^�C����ʂ̃f�[�^����������
    ''' </summary>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e
        Dim dtMstTable As DataTable
        Try
            Log.Info("Method started.")

            '�����ID���擾����B
            sLoginID = GlobalVariables.UserId

            '�f�[�^���擾����B
            dtMstTable = GetMstTable()

            If dtMstTable Is Nothing Or dtMstTable.Rows.Count = 0 Then
                '���������Ɉ�v����f�[�^�͑��݂��܂���B
                AlertBox.Show(Lexis.CompetitiveOperationDetected)
                Return bRet
                Exit Function
            Else
                sPatternName = dtMstTable.Rows(0).Item("PATTERN_NAME").ToString
                oldDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
            End If

            bRet = True

        Catch ex As Exception

            '��ʕ\�������Ɏ��s���܂����B
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRet = False

        Finally
            If bRet Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd) '�J�n�ُ탁�b�Z�[�W
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
    Private Sub FrmSysPatternMstUpdate_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrmData() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

            '��ʔw�i�F�iBackColor�j��ݒ肷��
            pnlPtnUpdate.BackColor = Config.BackgroundColor
            lblPtnNameTitle.BackColor = Config.BackgroundColor
            lblPtnNoTitle.BackColor = Config.BackgroundColor
            lblPtnNo.BackColor = Config.BackgroundColor

            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnUpdate.BackColor = Config.ButtonColor
            btnReturn.BackColor = Config.ButtonColor
            Me.txtPtnName.ImeMode = Windows.Forms.ImeMode.Hiragana

            '�o�^�[��No��ݒ肷��B
            lblPtnNo.Text = sPatternNo

            '�o�^�[���̒l��ݒ肷��B
            txtPtnName.Text = sPatternName

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' �u�C���v�{�^������������ƁADB�֐ݒ肳�ꂽ�p�^�[�������C������B
    ''' </summary>
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If LbEventStop Then Exit Sub
        Dim dtMstTable As DataTable

        Try
            LbEventStop = True
            '�C���{�^�������B
            FrmBase.LogOperation(sender, e, Me.Text)

            If CheckAll() = True Then
                If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUpdate) = DialogResult.Yes Then
                    FrmBase.LogOperation(Lexis.YesButtonClicked)
                    Call waitCursor(True)
                    '�f�[�^���擾����B
                    dtMstTable = GetMstTable()

                    If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
                        '���������Ɉ�v����f�[�^�͑��݂��܂���B
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Exit Sub
                    Else
                        newDate = dtMstTable.Rows(0).Item("UPDATE_DATE").ToString
                    End If

                    '�r���`�F�b�N
                    If Not oldDate.Equals(newDate) Then
                        AlertBox.Show(Lexis.CompetitiveOperationDetected)
                        Exit Sub
                    End If

                    '�X�V����
                    Call UpdatePattern()
                    FrmBase.LogOperation(Lexis.UpdateCompleted) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B   '�X�V����������ɏI�����܂����B
                    AlertBox.Show(Lexis.UpdateCompleted)
                    FrmBase.LogOperation(Lexis.OkButtonClicked)
                    Me.Close()
                Else
                    FrmBase.LogOperation(Lexis.NoButtonClicked)
                    FrmBase.LogOperation(Lexis.UpdateFailed) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B
                    btnUpdate.Select()
                End If
            End If
        Catch ex As DatabaseException
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)  '�\�����ʃG���[���������܂����B
            AlertBox.Show(Lexis.UpdateFailed)
            btnUpdate.Select()
            Exit Sub
        Finally
            LbEventStop = False
            Call waitCursor(False)
        End Try

    End Sub

    ''' <summary>
    ''' �u�I���v�{�^������������ƁA�{��ʂ��I�������B 
    ''' </summary>
    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click
        '�I���{�^�������B
        FrmBase.LogOperation(sender, e, Me.Text)
        Me.Close()
    End Sub

    ''' <summary>
    ''' �o�^�[���̓��͒l����������
    ''' </summary>
    Private Sub txtPattern_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPtnName.KeyPress

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

        If System.String.IsNullOrEmpty(txtPtnName.Text) Then
            '���͒l���s���ł��B�o�^�[���̒l�̓k���ł���B
            AlertBox.Show(Lexis.InputParameterIsIncomplete, lblPtnNameTitle.Text)
            txtPtnName.Focus()
        ElseIf OPMGUtility.CheckString(Me.txtPtnName.Text.ToString, 10, 2, True) = -4 Then
            '���͒l���s���ł��B
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            txtPtnName.Focus()
        ElseIf Me.txtPtnName.Text.ToString.Trim() = "" Then
            '���͒l���s���ł��B�S�p�X�y�[�X�̂ݓ��͂����ꍇ
            AlertBox.Show(Lexis.TheInputValueIsUnsuitableForPatternName)
            txtPtnName.Focus()
        Else
            bRetAll = True
        End If
        Return bRetAll

    End Function

    ''' <summary>
    ''' DB�֐ݒ肳�ꂽ�o�^�[�������X�V����B
    ''' </summary>
    Private Sub UpdatePattern()

        Dim sSQL As String = ""

        Dim sBuilder As New StringBuilder

        Dim dbCtl As New DatabaseTalker
        dbCtl = New DatabaseTalker

        Try
            '�o�^�[���̒l���擾����B
            sPattern = txtPtnName.Text

            '�[��ID
            Dim sClient As String = Config.MachineName
            sBuilder.AppendLine(" UPDATE M_PATTERN_DATA SET UPDATE_DATE = GETDATE(),")
            sBuilder.AppendLine(" UPDATE_USER_ID = " & Utility.SetSglQuot(sLoginID.ToString) & ",")
            sBuilder.AppendLine(" UPDATE_MACHINE_ID = " & Utility.SetSglQuot(sClient) & ",")
            sBuilder.AppendLine(" PATTERN_NAME = " & Utility.SetSglQuot(sPattern))
            sBuilder.AppendLine(" WHERE PATTERN_NO = " & Utility.SetSglQuot(sPatternNo))
            sBuilder.AppendLine(" AND MST_KIND = " & Utility.SetSglQuot(sKind))
            sBuilder.AppendLine(" AND MODEL_CODE = " & Utility.SetSglQuot(sModelcode))

            sSQL = sBuilder.ToString()

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()

        Catch ex As Exception
            dbCtl.TransactionRollBack()
            btnUpdate.Select()
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

    End Sub

    ''' <summary>
    ''' �f�[�^���擾����B
    ''' </summary>
    Private Function GetMstTable() As DataTable

        '�f�[�^���擾����B
        Dim dtMstTable As New DataTable
        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder
        Dim nRtn As Integer

        Try
            sBuilder.AppendLine(" SELECT PATTERN_NO, PATTERN_NAME, UPDATE_DATE ")
            sBuilder.AppendLine(" FROM M_PATTERN_DATA ")
            sBuilder.AppendLine(" WHERE PATTERN_NO = " & Utility.SetSglQuot(sPatternNo))
            sBuilder.AppendLine(" AND MST_KIND = " & Utility.SetSglQuot(sKind))
            sBuilder.AppendLine(" AND MODEL_CODE = " & Utility.SetSglQuot(sModelcode))
            sSQL = sBuilder.ToString()

            nRtn = FrmBase.BaseSqlDataTableFill(sSQL, dtMstTable)

            If nRtn = -9 Then
                Throw New OPMGException()
            End If
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try

        Return dtMstTable

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
