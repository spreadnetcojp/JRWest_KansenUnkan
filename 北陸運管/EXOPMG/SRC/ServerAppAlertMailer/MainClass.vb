' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Messaging
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' �x�񃁁[�����M�v���Z�X�̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "�萔��ϐ�"
    '���C���E�B���h�E
    Private Shared oMainForm As ServerAppForm

    '����ƃX���b�h�ւ̏I���v���t���O
    Private Shared quitWorker As Integer

    '���b�Z�[�W�L���[
    Private Shared oMessageQueue As MessageQueue

    'SMTP�N���C�A���g
    Private Shared oSmtpClient As SmtpClient
#End Region

    ''' <summary>
    ''' �x�񃁁[�����M�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �x�񃁁[�����M�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppAlertMailer")
        If m.WaitOne(0, False) Then
            Try
                Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
                If sLogBasePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
                    Return
                End If

                Dim sIniFilePath As String = Constant.GetEnv(REG_SERVER_INI)
                If sIniFilePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_SERVER_INI)
                    Return
                End If

                Log.Init(sLogBasePath, "AlertMailer")
                Log.Info("�v���Z�X�J�n")

                Try
                    Lexis.Init(sIniFilePath)
                    Config.Init(sIniFilePath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End Try

                Log.SetKindsMask(Config.LogKindsMask)

                If Not Config.MailSmtpServerName.Equals("") Then
                    oSmtpClient = New SmtpClient()
                    oSmtpClient.Host = Config.MailSmtpServerName
                    oSmtpClient.Port = Config.MailSmtpPort
                    oSmtpClient.Credentials = New NetworkCredential(Config.MailSmtpUserName, Config.MailSmtpPassword)
                    oSmtpClient.Timeout = Config.MailSendLimitTicks
                End If

                '���b�Z�[�W���[�v���A�C�h����ԂɂȂ�O�i���A����I�ɂ�����s��
                '�X���b�h���N������O�j�ɁA�����ؖ��t�@�C�����X�V���Ă����B
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '����ƃX���b�h���J�n����B
                Dim oWorkerThread As New Thread(AddressOf MainClass.WorkingLoop)
                Log.Info("Starting the worker thread...")
                quitWorker = 0
                oWorkerThread.Name = "Worker"
                oWorkerThread.Start()

                '�E�C���h�E�v���V�[�W�������s����B
                'NOTE: ���̃��\�b�h�����O���X���[����邱�Ƃ͂Ȃ��B
                ServerAppBaseMain(oMainForm)

                Try
                    '����ƃX���b�h�ɏI����v������B
                    Log.Info("Sending quit request to the worker thread...")
                    Thread.VolatileWrite(quitWorker, 1)

                    'NOTE: �ȉ��Ŏ���ƃX���b�h���I�����Ȃ��ꍇ�A
                    '����ƃX���b�h�͐����ؖ����s��Ȃ��͂��ł���A
                    '�󋵂ւ̑Ώ��̓v���Z�X�}�l�[�W���ōs����z��ł���B

                    '����ƃX���b�h�̏I����҂B
                    Log.Info("Waiting for the worker thread to quit...")
                    oWorkerThread.Join()
                    Log.Info("The worker thread has quit.")
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oWorkerThread.Abort()
                End Try
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                If oMainForm IsNot Nothing Then
                    oMainForm.Dispose()
                End If

                'NOTE: .NET Framework 4.0 �ȍ~�ł́A�ȉ���ǉ�����B
                '�Ȃ��A.NET Framework 4.5 �ȍ~�ł́ASubject ��
                '�G���R�[�h���@�ɂ���������Ȃ���΂Ȃ�Ȃ��̂ŁA���ӁB
                'If oSmtpClient IsNot Nothing Then
                '    oSmtpClient.Dispose()
                'End If

                Config.Dispose()

                Log.Info("�v���Z�X�I��")

                'NOTE: ������ʂ�Ȃ��Ă��A���̃X���b�h�̏��łƂƂ��ɉ�������
                '�悤�Ȃ̂ŁA�ň��̐S�z�͂Ȃ��B
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub

    ''' <summary>
    ''' ����ƃX���b�h�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �x�񃁁[�����M�v���iMSMQ���b�Z�[�W�j�̎�M��҂�������B
    ''' �x�񃁁[�����M�v������M�����ۂ́ASMTP�Ń��[���T�[�o�ɑ��M����B
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Try
            Log.Info("The worker thread started.")

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            Dim fewSpan As New TimeSpan(0, 0, 0, 0, Config.PollIntervalTicks)
            Dim oFilter As New MessagePropertyFilter()
            oFilter.ClearAll()
            oFilter.AppSpecific = True
            oFilter.Body = True

            'NOTE: Config.MyMqPath�̃��b�Z�[�W�L���[�́A
            '���̎��_�ŕK�����݂��Ă���O��ł���B���݂��Ă��Ȃ���΁A
            '�V�X�e���Ɉُ킪����́A���̃v���Z�X�͋N������ɏI������ׂ�
            '�ł���B
            oMessageQueue = New MessageQueue(Config.MyMqPath)
            oMessageQueue.MessageReadPropertyFilter = oFilter
            oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([ExtAlertMailSendRequestBody])})

            oDiagnosisTimer.Start(TickTimer.GetSystemTick())
            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()
                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

                Dim oMessage As Message = Nothing
                Try
                    '���莞�ԃ��b�Z�[�W��҂B
                    'NOTE: MessageQueue.Receive()�̃^�C���A�E�g�́A���ۂ�
                    '�����̎��Ԃ��o�߂����ۂł͂Ȃ��A�Ăяo�����_��
                    '�V�X�e�������Ɉ����̎��Ԃ�����������T�����߂���ŁA
                    '�V�X�e��������T�ȏ�ɂȂ����ۂɍs����悤�ł���B
                    '�܂�A�Ăяo���̊ԂɃV�X�e��������1���Ԗ߂����΁A
                    '�Ăяo������߂�̂́A�u�����̎���+1���ԁv�o�ߌ��
                    '�Ȃ��Ă��܂��A���̊Ԃ́A�q�X���b�h�̐����Ď���
                    '�e�v���Z�X�ւ̐����ؖ����s�����Ƃ��ł��Ȃ��Ȃ�B
                    '���C���X���b�h����̏I���v���ɂ������ł��Ȃ��Ȃ�B
                    '�������A���̂��Ƃ����ɂȂ�悤�ȑ傫�Ȏ����␳��
                    '�s���邱�Ƃ͂Ȃ����낤���A������TimeSpan.Zero��n���āA
                    '�ʂ̕��@��CPU�̉�����Ԃ���邷��悤�ɂ���΁A
                    '���b�Z�[�W��M�ɑ΂��锽���������Ȃ�i���b�Z�[�W����
                    '���\���ቺ����j�͂��ł��邽�߁A�ȉ��̂Ƃ���A
                    'MessageQueue.Receive()�ő҂��Ƃɂ��Ă���B
                    oMessage = oMessageQueue.Receive(fewSpan)
                Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                    '���b�Z�[�W��M�҂��ɖ߂�B
                    Continue While
                End Try

                If oMessage.AppSpecific <> ExtAlertMailSendRequest.FormalKind Then
                    Log.Error("Unwelcome ExtMessage received.")
                    '���b�Z�[�W��M�҂��ɖ߂�B
                    Continue While
                End If

                Dim sMailTitle As String = Nothing
                Dim sMailBody As String = Nothing
                Try
                    Dim oMailSendRequest As New ExtAlertMailSendRequest(oMessage)
                    sMailTitle = oMailSendRequest.MailTitle
                    sMailBody = oMailSendRequest.MailBody
                Catch ex As Exception
                    Log.Error("Exception caught on parsing ExtMessage.", ex)
                    '���b�Z�[�W��M�҂��ɖ߂�B
                    Continue While
                End Try

                Log.Info("ExtAlertMailSendRequest received." & vbCrLf & sMailTitle & vbCrLf & sMailBody)

                '���M�����݂�B�ʐM�ُ�����o�����ꍇ�́A���̊������b�Z�[�W���S�ēǂݎ̂Ă�B
                If oSmtpClient IsNot Nothing AndAlso TrySendAlertMail(sMailTitle, sMailBody) = -2 AndAlso Config.MailSendFailureSpreads Then
                    While Thread.VolatileRead(quitWorker) = 0
                        Try
                            oMessage = oMessageQueue.Receive(TimeSpan.Zero)
                        Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.IOTimeout
                            '���b�Z�[�W��M�҂��ɖ߂�B
                            Exit While
                        End Try

                        If oMessage.AppSpecific <> ExtAlertMailSendRequest.FormalKind Then
                            Log.Error("Unwelcome ExtMessage (AppSpecific: " & oMessage.AppSpecific.ToString() & ") received.")
                            '�������b�Z�[�W��o���ɖ߂�B
                            Continue While
                        End If

                        Try
                            Dim oMailSendRequest As New ExtAlertMailSendRequest(oMessage)
                            sMailTitle = oMailSendRequest.MailTitle
                            sMailBody = oMailSendRequest.MailBody
                        Catch ex As Exception
                            Log.Error("Exception caught on parsing ExtMessage.", ex)
                            '�������b�Z�[�W��o���ɖ߂�B
                            Continue While
                        End Try

                        Log.Info("ExtAlertMailSendRequest received." & vbCrLf & sMailTitle & vbCrLf & sMailBody)
                        Log.Warn("�v�����̂Ă܂����B")
                    End While
                End If
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        Finally
            If oMessageQueue IsNot Nothing Then
                oMessageQueue.Close()
            End If
        End Try
    End Sub

    Private Shared Function TrySendAlertMail(ByVal sSubject As String, ByVal sBody As String) As Integer
        Using oMail As New MailMessage()
            Try
                '���[���w�b�_��FROM�∶���ҏW�B
                oMail.From = New MailAddress(Config.MailFromAddr)
                For i As Integer = 0 To Config.MailToAddrs.Length - 1
                    If Not String.IsNullOrEmpty(Config.MailToAddrs(i)) Then
                        oMail.To.Add(Config.MailToAddrs(i))
                    End If
                Next
                For i As Integer = 0 To Config.MailCcAddrs.Length - 1
                    If Not String.IsNullOrEmpty(Config.MailCcAddrs(i)) Then
                        oMail.CC.Add(Config.MailCcAddrs(i))
                    End If
                Next
                For i As Integer = 0 To Config.MailBccAddrs.Length - 1
                    If Not String.IsNullOrEmpty(Config.MailBccAddrs(i)) Then
                        oMail.Bcc.Add(Config.MailBccAddrs(i))
                    End If
                Next

                '���[���̌�����ҏW�B
                Dim oSubjectEncoding As Encoding = Encoding.GetEncoding(Config.MailSubjectEncoding)
                oMail.Subject = String.Format( _
                   "=?{0}?B?{1}?=", _
                   oSubjectEncoding.BodyName, _
                   Convert.ToBase64String(oSubjectEncoding.GetBytes(sSubject), Base64FormattingOptions.None))

                '���[���{����ҏW�B
                Dim oAltView As AlternateView = _
                   AlternateView.CreateAlternateViewFromString( _
                      sBody, _
                      Encoding.GetEncoding(Config.MailBodyEncoding), _
                      MediaTypeNames.Text.Plain)
                oAltView.TransferEncoding = Config.MailTransferEncoding
                oMail.AlternateViews.Add(oAltView)

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                Log.Fatal("�����܂��͖{���ُ̈�ő��M���ł��܂���ł����B")
                Return -4
            End Try

            Try
                '���[���𑗐M�B
                oSmtpClient.Send(oMail)

            Catch ex As SmtpFailedRecipientsException
                Log.Debug("Exception caught.", ex)
                Log.Error("�ꕔ�̈���(" & ex.FailedRecipient & ")�ւ̑��M���ł��܂���ł����B")
                Return -1
            Catch ex As SmtpException
                Log.Error("Exception caught.", ex)
                Log.Error("�ʐM�ُ�ɂ�著�M���ł��܂���ł����B")
                Return -2
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                Log.Fatal("�\�����ʈُ�ɂ�著�M���ł��܂���ł����B")
                Return -3
            End Try
        End Using

        Log.Info("���M���܂����B")
        Return 0
    End Function

End Class
