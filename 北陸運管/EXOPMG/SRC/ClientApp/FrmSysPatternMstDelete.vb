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

''' <summary>�p�^�[���폜</summary>
''' <remarks>
''' �p�^�[�����̂�ύX���A�u�폜�v�{�^�����N���b�N���邱�Ƃɂ��A
''' ���Y�f�[�^���^�p�Ǘ��T�[�o���폜����B
''' </remarks>
Public Class FrmSysPatternMstDelete
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
    Friend WithEvents lblPtnNoTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNameTitle As System.Windows.Forms.Label
    Friend WithEvents lblPtnNo As System.Windows.Forms.Label
    Friend WithEvents lblPtnName As System.Windows.Forms.Label
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents pnlPtnDelete As System.Windows.Forms.Panel
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblPtnNoTitle = New System.Windows.Forms.Label()
        Me.lblPtnNameTitle = New System.Windows.Forms.Label()
        Me.lblPtnNo = New System.Windows.Forms.Label()
        Me.lblPtnName = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.pnlPtnDelete = New System.Windows.Forms.Panel()
        Me.pnlPtnDelete.SuspendLayout()
        Me.SuspendLayout()
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
        'lblPtnName
        '
        Me.lblPtnName.Font = New System.Drawing.Font("�l�r �S�V�b�N", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblPtnName.Location = New System.Drawing.Point(165, 261)
        Me.lblPtnName.Name = "lblPtnName"
        Me.lblPtnName.Size = New System.Drawing.Size(180, 21)
        Me.lblPtnName.TabIndex = 4
        Me.lblPtnName.Text = "�w�w�w�w�w�w�w�w�w�w"
        Me.lblPtnName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.Silver
        Me.btnDelete.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(426, 116)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(90, 32)
        Me.btnDelete.TabIndex = 0
        Me.btnDelete.Text = "��  ��"
        Me.btnDelete.UseVisualStyleBackColor = False
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
        'pnlPtnDelete
        '
        Me.pnlPtnDelete.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnNoTitle)
        Me.pnlPtnDelete.Controls.Add(Me.btnReturn)
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnNameTitle)
        Me.pnlPtnDelete.Controls.Add(Me.btnDelete)
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnNo)
        Me.pnlPtnDelete.Controls.Add(Me.lblPtnName)
        Me.pnlPtnDelete.Location = New System.Drawing.Point(0, 0)
        Me.pnlPtnDelete.Name = "pnlPtnDelete"
        Me.pnlPtnDelete.Size = New System.Drawing.Size(594, 418)
        Me.pnlPtnDelete.TabIndex = 0
        '
        'FrmSysPatternMstDelete
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(590, 414)
        Me.Controls.Add(Me.pnlPtnDelete)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmSysPatternMstDelete"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "�p�^�[���폜"
        Me.pnlPtnDelete.ResumeLayout(False)
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
    Private ReadOnly FormTitle As String = "�p�^�[���ݒ�폜"

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
    Private sModelcode As String = ""

    Public Property Modelcode() As String
        Get
            Return sModelcode
        End Get
        Set(ByVal value As String)
            sModelcode = value
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
    ''' Pattern�f�[�^�폜��ʂ̃f�[�^����������
    ''' </summary>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRet As Boolean = False
        LbInitCallFlg = True    '���֐��ďo�t���O
        LbEventStop = True      '�C�x���g�����n�e�e
        Dim dtMstTable As DataTable
        Try
            Log.Info("Method started.")

            '�f�[�^���擾����B
            dtMstTable = GetMstTable()

            If dtMstTable Is Nothing OrElse dtMstTable.Rows.Count = 0 Then
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
    Private Sub FrmSysPatternMstDelete_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If LbInitCallFlg = False Then   '�����������Ăяo����Ă��Ȃ��ꍇ�̂ݎ��{
                If InitFrmData() = False Then   '��������
                    Me.Close()
                    Exit Sub
                End If
            End If

            '��ʔw�i�F�iBackColor�j��ݒ肷��
            pnlPtnDelete.BackColor = Config.BackgroundColor
            lblPtnNameTitle.BackColor = Config.BackgroundColor
            lblPtnNoTitle.BackColor = Config.BackgroundColor
            lblPtnName.BackColor = Config.BackgroundColor
            lblPtnNo.BackColor = Config.BackgroundColor

            '�{�^���w�i�F�iBackColor�j��ݒ肷��
            btnDelete.BackColor = Config.ButtonColor
            btnReturn.BackColor = Config.ButtonColor

            '�o�^�[��No��ݒ肷��B
            lblPtnNo.Text = sPatternNo

            '�o�^�[���̒l��ݒ肷��B
            lblPtnName.Text = sPatternName

            Me.btnDelete.Focus()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    ''' <summary>
    ''' �u�폜�v�{�^������������ƁADB�֐ݒ肳�ꂽ�p�^�[�������폜����B
    ''' </summary>
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        '�폜�{�^��������
        If LbEventStop Then Exit Sub
        Dim dtMstTable As DataTable

        Try
            LbEventStop = True
            '�폜�{�^�������B
            FrmBase.LogOperation(sender, e, Me.Text)

            '�폜�m�F����
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyDelete).Equals(System.Windows.Forms.DialogResult.Yes) Then
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

                '�폜����
                Call DeletePattern()
                '�폜����������ɏI�����܂����B
                FrmBase.LogOperation(Lexis.DeleteCompleted) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B
                AlertBox.Show(Lexis.DeleteCompleted)
                FrmBase.LogOperation(Lexis.OkButtonClicked)
                Me.Close()
            Else
                FrmBase.LogOperation(Lexis.NoButtonClicked)
                FrmBase.LogOperation(Lexis.DeleteFailed) 'TODO: ���Ȃ��Ƃ��u����v���O�ł͂Ȃ��B�ڍא݌v���܂ߊm�F�B
                btnDelete.Select()
            End If
        Catch ex As DatabaseException
            AlertBox.Show(Lexis.DatabaseOpenErrorOccurred)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex) '�\�����ʃG���[���������܂����B
            AlertBox.Show(Lexis.DeleteFailed)
            btnDelete.Select()
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

#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' DB�֐ݒ肳�ꂽ�o�^�[�������폜����B
    ''' </summary>
    Private Sub DeletePattern()

        Dim sSQL As String = ""
        Dim sBuilder As New StringBuilder

        Dim dbCtl As DatabaseTalker
        dbCtl = New DatabaseTalker

        Try
            sBuilder.AppendLine(" DELETE FROM M_PATTERN_DATA ")
            sBuilder.AppendLine(" WHERE PATTERN_NO = " & Utility.SetSglQuot(sPatternNo))
            sBuilder.AppendLine(" AND MST_KIND = " & Utility.SetSglQuot(sKind))
            sBuilder.AppendLine(" AND MODEL_CODE = " & Utility.SetSglQuot(sModelcode))
            sSQL = sBuilder.ToString()

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            dbCtl.ExecuteSQLToWrite(sSQL)

            dbCtl.TransactionCommit()
            dbCtl.ConnectClose()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            btnDelete.Select()
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

            sBuilder.AppendLine(" SELECT PATTERN_NAME,  UPDATE_DATE")
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
