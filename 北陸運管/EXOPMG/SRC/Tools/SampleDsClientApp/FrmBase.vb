' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e  �V�K�쐬
'   0.1      2013/03/01  (NES)����  ���샍�O�@�\��ǉ�
'   0.2      2013/05/13  (NES)����  �f�W�N���C�A���g�����A�v����
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports System.Deployment.Application
Imports System.Reflection
Imports System.Text

Public Class FrmBase
    Inherits System.Windows.Forms.Form

    ''' <summary>���샍�O�t�@�C���̕�������</summary>
    Private Const sOpLogName As String = "Operation"

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
    Friend WithEvents timTimer As System.Windows.Forms.Timer
    Protected WithEvents lblTitle As System.Windows.Forms.Label
    Public WithEvents pnlBodyBase As System.Windows.Forms.Panel
    Public WithEvents lblToday As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.lblTitle = New System.Windows.Forms.Label
        Me.lblToday = New System.Windows.Forms.Label
        Me.pnlBodyBase = New System.Windows.Forms.Panel
        Me.timTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTitle.Font = New System.Drawing.Font("�l�r �S�V�b�N", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 32)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(1014, 56)
        Me.lblTitle.TabIndex = 1
        Me.lblTitle.Text = "Title"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblToday.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblToday.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblToday.Location = New System.Drawing.Point(0, 0)
        Me.lblToday.Name = "lblToday"
        Me.lblToday.Size = New System.Drawing.Size(1014, 32)
        Me.lblToday.TabIndex = 0
        Me.lblToday.Text = "YYYY/MM/DD(�m)�@hh:mm"
        Me.lblToday.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlBodyBase.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnlBodyBase.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.pnlBodyBase.Location = New System.Drawing.Point(0, 88)
        Me.pnlBodyBase.Name = "pnlBodyBase"
        Me.pnlBodyBase.Size = New System.Drawing.Size(1014, 656)
        Me.pnlBodyBase.TabIndex = 2
        '
        'timTimer
        '
        '
        'FrmBase
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Controls.Add(Me.lblToday)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pnlBodyBase)
        Me.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.Name = "FrmBase"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.ResumeLayout(False)

    End Sub

#End Region

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< �C�x���g

    ''' <summary>
    ''' [�t�H�[�����[�h]
    ''' </summary>
    Private Sub FrmBase_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '�V�X�e��������\������
        timTimer.Interval = 100
        timTimer.Enabled = True

        '���u���{�o�[�W��������\������
        
        Dim sVersion As String = ""
        If ApplicationDeployment.IsNetworkDeployed Then
            Dim deploy As ApplicationDeployment
            deploy = ApplicationDeployment.CurrentDeployment

            '�o�[�W�����́A[���W���[]�A[�}�C�i]�A[�r���h]��\������B[���r�W����] �͕\�����Ȃ��B
            sVersion = "Ver" & deploy.CurrentVersion.ToString()
            Dim intFindPos As Integer = sVersion.LastIndexOf(".")
            sVersion = Mid(sVersion, 1, intFindPos)

        End If

        Me.Text = String.Format("{0} {1}", Lexis.FormTitle.Gen(), sVersion)

        '��ʔw�i�F�iBackColor�j��ݒ肷��B
        '���A�}�X�^�o�[�W������ʁA�v���O�����o�[�W������ʂɂ��ẮA
        '�w���{�^���ɏ����ɉ����ĐF������K�v�����邽�߁A
        '�e��ʂɂĔw�i�F��ݒ肷��B
        If Me.Name <> "FrmMstDispVersion" And Me.Name <> "FrmPrgDispVersion" Then
            LfSetBackColor(Me)
        End If
    End Sub

    ''' <summary>
    ''' [Timer.Tick�C�x���g]
    ''' </summary>
    Private Sub timTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles timTimer.Tick

        timTimer.Interval = 1000

        '�V�X�e��������\������
        Dim dNow As DateTime
        dNow = Now
        lblToday.Text = dNow.ToString("yyyy/MM/dd(ddd)  HH:mm")
    End Sub

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< ���\�b�h

    ''' <summary>
    ''' [�w��R���g���[�����S�R���g���[���擾]
    ''' </summary>
    ''' <param name="top">�ΏۃR���g���[��</param>
    ''' <returns>�z�u����Ă���R���g���[���z��</returns>
    Public Shared Function BaseGetAllControls(ByVal top As Control) As Control()
        Dim buf As ArrayList = New ArrayList
        For Each c As Control In top.Controls
            buf.Add(c)
            buf.AddRange(BaseGetAllControls(c))
        Next
        Return CType(buf.ToArray(GetType(Control)), Control())
    End Function

    ''' <summary>
    ''' [�w��R���g���[�����S�R���g���[��Enable=False]
    ''' </summary>
    ''' <param name="ctl">�ݒ�Ώۉ�ʃR���g���[��</param>
    ''' <param name="bLabel">���x�����܂܂�Ă���ꍇ�A���x�����ΏۂƂ���ꍇ�ATrue�B�ΏۂƂ��Ȃ��ꍇFalse(��̫��)�B</param>
    Public Shared Sub BaseCtlDisabled(ByVal ctl As Control, Optional ByVal bLabel As Boolean = False)
        Dim all As Control() = BaseGetAllControls(ctl)
        For Each c As Control In all
            Try
                If TypeOf c Is Label Then
                    If bLabel Then
                        c.Enabled = False
                    End If
                ElseIf TypeOf c Is Panel Then
                ElseIf TypeOf c Is GroupBox Then
                Else
                    c.Enabled = False
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub

    ''' <summary>
    ''' [�w��R���g���[�����S�R���g���[��Enable=True]
    ''' </summary>
    ''' <param name="ctl">�ݒ�Ώۉ�ʃR���g���[��</param>
    Public Shared Sub BaseCtlEnabled(ByVal ctl As Control)
        Dim all As Control() = BaseGetAllControls(ctl)
        For Each c As Control In all
            Try
                c.Enabled = True
            Catch ex As Exception
            End Try
        Next
    End Sub

    ''' <summary>
    ''' �J�[�\���҂�
    ''' </summary>
    ''' <param name="bWait">true:�҂��J�n�@false:�҂��I��</param>
    ''' <remarks>�J�[�\���������v�ɂȂ�</remarks>
    Public Sub LfWaitCursor(Optional ByVal bWait As Boolean = True)
        If bWait = True Then
            Me.Cursor = Cursors.WaitCursor
        Else
            Me.Cursor = Cursors.Default
        End If
    End Sub

    ''' <summary>
    ''' [�x�[�X��ʔw�i�F�ݒ�]
    ''' �p����̉�ʓ��ɂ���R���g���[���i��������j�̔w�i�F��ݒ肷��B
    ''' </summary>
    ''' <param name="ctl">�ݒ�Ώۉ�ʃR���g���[��</param>
    Private Shared Sub LfSetBackColor(ByVal ctl As Control)
        LfSetBackColorCore(ctl)
        Dim all As Control() = BaseGetAllControls(ctl)
        For Each c As Control In all
            LfSetBackColorCore(c)
        Next
    End Sub

    ''' <summary>
    ''' [�w��R���g���[���w�i�F�ݒ�]
    ''' �ΏۃR���g���[���̔w�i�F��ݒ肷��B
    ''' �A���A�ΏۃR���g���[���̎�ސ�������i�R�[�h���Q�Ɓj�B
    ''' �ʓr�A���ʈȊO�Őݒ肷��ꍇ�͊e��ʂɂď������邱�ƁB
    ''' </summary>
    ''' <param name="ctl">�ΏۃR���g���[��</param>
    Private Shared Sub LfSetBackColorCore(ByVal ctl As Control)
        Dim bFlg As Boolean = False
        If TypeOf ctl Is Button Then
            ctl.BackColor = Config.ButtonColor
        Else
            '�w�i�F��ݒ肷��R���g���[��
            If TypeOf ctl Is Form Then bFlg = True
            If TypeOf ctl Is Panel Then bFlg = True
            If TypeOf ctl Is GroupBox Then bFlg = True
            If TypeOf ctl Is Label Then bFlg = True
            If TypeOf ctl Is RadioButton Then bFlg = True
            If TypeOf ctl Is TabPage Then bFlg = True
            If bFlg Then
                Try
                    ctl.BackColor = Config.BackgroundColor
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' �J�[�\���҂�
    ''' </summary>
    ''' <param name="bWait">true:�҂��J�n�@false:�҂��I��</param>
    ''' <remarks>�J�[�\���������v�ɂȂ�</remarks>
    Protected Sub waitCursor(Optional ByVal bWait As Boolean = True)

        If bWait = True Then
            Me.Cursor = Cursors.WaitCursor
            Me.Enabled = False
        Else
            Me.Cursor = Cursors.Default
            Me.Enabled = True
        End If

    End Sub

    ''' <summary>
    ''' �C�ӕ����ő��엚�����L�^����B
    ''' </summary>
    ''' <param name="oSentence">�L�^����</param>
    ''' <param name="args">0�ȏ�̏����ݒ�ΏۃI�u�W�F�N�g���܂� Object�z��</param>
    Public Shared Sub LogOperation(ByVal oSentence As Sentence, ByVal ParamArray args As Object())
        Log.Extra(sOpLogName, New StackTrace(0, True).GetFrame(1).GetMethod(), oSentence.Gen(args))
    End Sub

    ''' <summary>
    ''' �q��ʂ̑��엚�����L�^����B
    ''' </summary>
    ''' <param name="oSender">�C�x���g���M���̃I�u�W�F�N�g</param>
    ''' <param name="oEventArgs">�C�x���g�̕t���f�[�^</param>
    ''' <param name="sFormTitle">�q��ʂ̃^�C�g��</param>
    Public Shared Sub LogOperation(ByVal oSender As Object, ByVal oEventArgs As System.EventArgs, ByVal sFormTitle As String)
        LogOperationCore(New StackTrace(0, True).GetFrame(1).GetMethod(), oSender, oEventArgs, sFormTitle & Lexis.DialogSuffix.Gen())
    End Sub

    ''' <summary>
    ''' ���엚�����L�^����B
    ''' </summary>
    ''' <param name="oSender">�C�x���g���M���̃I�u�W�F�N�g</param>
    ''' <param name="oEventArgs">�C�x���g�̕t���f�[�^</param>
    Protected Sub LogOperation(ByVal oSender As Object, ByVal oEventArgs As System.EventArgs)
        LogOperationCore(New StackTrace(0, True).GetFrame(1).GetMethod(), oSender, oEventArgs, lblTitle.Text & Lexis.WindowSuffix.Gen())
    End Sub

    ''' <summary>
    ''' ���엚�����L�^����B
    ''' </summary>
    ''' <param name="oSender">�C�x���g���M���̃I�u�W�F�N�g</param>
    ''' <param name="oEventArgs">�C�x���g�̕t���f�[�^</param>
    ''' <param name="sFormTitle">��ʃ^�C�g��</param>
    Private Shared Sub LogOperationCore(ByVal oCaller As MethodBase, ByVal oSender As Object, ByVal oEventArgs As System.EventArgs, ByVal sFormTitle As String)
        If TypeOf oSender Is Control Then
            'TODO: StackTrace����Ăь���MethodName���擾���A���ꂪ
            'oSender.GetType().GetEvent("Foo").GetRaiseMethod()��
            '��v���邩�`�F�b�N����B�����āA��v����ꍇ�̂݁A
            '��p�����iLexis.SenderTypeNameFoo�j���g����
            '�L�^���s���悤�ɂ���B
            Dim oControl As Control = CType(oSender, Control)
            Select Case True
                Case TypeOf oSender Is DateTimePicker
                    Dim oDateTimePicker As DateTimePicker = CType(oSender, DateTimePicker)
                    Dim oValue As DateTime = oDateTimePicker.Value
                    Log.Extra(sOpLogName, oCaller, Lexis.DateTimePickerValueChanged.Gen(sFormTitle, oControl.Name, oValue.ToString("yyyy/MM/dd HH:mm:ss")))
                Case TypeOf oSender Is ComboBox
                    Dim oComboBox As ComboBox = CType(oSender, ComboBox)
                    Dim oSelection As Object = oComboBox.SelectedItem
                    If oSelection IsNot Nothing Then
                        Log.Extra(sOpLogName, oCaller, Lexis.ComboBoxSelectionChanged.Gen(sFormTitle, oControl.Name,  oSelection.ToString()))
                    Else
                        Log.Extra(sOpLogName, oCaller, Lexis.ComboBoxSelectionChangedToNothing.Gen(sFormTitle, oControl.Name))
                    End If
                Case TypeOf oSender Is Button
                    Log.Extra(sOpLogName, oCaller, Lexis.ButtonClicked.Gen(sFormTitle, oControl.Name))
                Case Else
                    Log.Extra(sOpLogName, oCaller, Lexis.SomeControlInvoked.Gen(sFormTitle, oControl.Name, oControl.GetType().ToString()))
            End Select
        Else
            Log.Fatal("The method called with invalid argument.")
        End If
    End Sub

End Class
