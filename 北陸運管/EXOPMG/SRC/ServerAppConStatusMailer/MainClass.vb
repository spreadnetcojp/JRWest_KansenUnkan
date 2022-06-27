' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2013/12/10  (NES)����  ������ԏ��̒ǉ��Ή�
'   0.2      2017/04/10  (NES)����  ������ԕ�Ή��ɂāA���[��������
'                                   ���@�ԍ���Integer�œ��͂���i�����ւ�
'                                   ���ɏ������w��\�Ƃ���j�悤�ɓ���
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' �@��ڑ���ԃ��[�������v���Z�X�̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "�萔��ϐ�"
    '���C���E�B���h�E
    Private Shared oMainForm As ServerAppForm

    '����ƃX���b�h�ւ̏I���v���t���O
    Private Shared quitWorker As Integer
#End Region

    ''' <summary>
    ''' �@��ڑ���ԃ��[�������v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �@��ڑ���ԃ��[�������v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppConStatusMailer")
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

                Log.Init(sLogBasePath, "ConStatusMailer")
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
    ''' ����I��DB����@��ڑ���Ԃ��擾���A�ُ킪����΁A
    ''' ���[���ɋL�q���đ��M����B
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Try
            Log.Info("The worker thread started.")

            Dim oSmtpClient As SmtpClient = Nothing
            If Not Config.MailSmtpServerName.Equals("") Then
                oSmtpClient = New SmtpClient()
                oSmtpClient.Host = Config.MailSmtpServerName
                oSmtpClient.Port = Config.MailSmtpPort
                oSmtpClient.Credentials = New NetworkCredential(Config.MailSmtpUserName, Config.MailSmtpPassword)
                oSmtpClient.Timeout = Config.MailSendLimitTicks
            End If

            '�O�񌟍������i���K���ς݁j
            Dim lastSearchTime As DateTime = Normalize(DateTime.Now.AddMilliseconds(-Config.MailSendDelayTicks))

            '�L�����ԑт̊J�n�����ƏI���������i0��0������̌o�ߕ��̌`���Łj
            'mailStartMinutesInDay��mailEndMinutesInDay�ɎZ�o���Ă����B
            '���̍ہAmailStartMinutesInDay <= mailEndMinutesInDay�ɂȂ�悤�A
            '�K�v�ɉ�����mailEndMinutesInDay�ɂ͕␳�������Ă����B
            'NOTE: mailStartMinutesInDay == mailEndMinutesInDay�͐����Ȑݒ�
            '�ł���A�L�����ԑт����̂P���Ԃ����ł��邱�Ƃ��Ӗ�����B
            Dim mailStartMinutesInDay As Integer = Config.MailStartHour * 60 + Config.MailStartMinute
            Dim mailEndMinutesInDay As Integer = Config.MailEndHour * 60 + Config.MailEndMinute
            If mailStartMinutesInDay > mailEndMinutesInDay Then
                mailEndMinutesInDay += 24 * 60
            End If

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()

                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

                If oSmtpClient IsNot Nothing Then
                    Dim now As DateTime = Normalize(DateTime.Now.AddMilliseconds(-Config.MailSendDelayTicks))
                    If now > lastSearchTime Then
                        'mailStartMinutesInDay�ȏ�ɂȂ�悤�ɕ␳����
                        '���ݎ������i0��0������̌o�ߕ��̌`���Łj���߂�B
                        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
                        If mailStartMinutesInDay > nowMinutesInDay Then
                            nowMinutesInDay += 24 * 60
                        End If

                        '�L�����ԑт̂ݑ��M���s���B
                        If nowMinutesInDay <= mailEndMinutesInDay Then
                            SearchAndSend(lastSearchTime, now, oSmtpClient)
                        End If

                        lastSearchTime = now
                    ElseIf now < lastSearchTime Then
                        '�V�X�e��������2�����ȏ�߂��ꂽ�ꍇ�́A
                        '�ŏI���{�����𐳋K�������V�X�e�������ɍ��킹��B
                        Dim span As TimeSpan = lastSearchTime - now
                        Dim cycles As Integer = span.Minutes \ Config.MailSendCycle
                        If cycles > 1 Then
                            Log.Warn("The system time goes back into the past.")
                            lastSearchTime = now
                        End If
                    End If
                End If

                Thread.Sleep(Config.PollIntervalTicks)
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        End Try
    End Sub

    Private Shared Function Normalize(ByVal time As DateTime) As DateTime
        Dim minutes As Integer = (time.Hour * 60 + time.Minute) - (Config.MailStartHour * 60 + Config.MailStartMinute)
        If minutes < 0 Then
            minutes += 24 * 60
            minutes = (minutes \ Config.MailSendCycle) * Config.MailSendCycle
            minutes -= 24 * 60
        Else
            minutes = (minutes \ Config.MailSendCycle) * Config.MailSendCycle
        End If
        Dim originTime As New DateTime(time.Year, time.Month, time.Day, Config.MailStartHour, Config.MailStartMinute, 0)
        Return originTime.AddMinutes(minutes)
    End Function

    ''' <summary>
    ''' �T�[�`����у��[�����������B
    ''' </summary>
    Private Shared Sub SearchAndSend(ByVal prevTime As DateTime, ByVal curTime As DateTime, ByVal oSmtpClient As SmtpClient)
        Dim oMailBody As New StringBuilder()
        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim agents As DataRowCollection
            Dim writeCount As Integer

            '�@��\���}�X�^����A���݉^�p����Ă���ׂ��Ď��Ղ̈ꗗ�i�x�~���@�͏��O�j���擾����B
            agents = SelectUnitsInService(dbCtl, EkConstants.ModelCodeKanshiban, sServiceDate).Rows

            '���[���{�����̉��D�@��ԏ��Z�N�V������ҏW�B
            '�@��\������擾�����Ď��ՒP�ʂŏ������s���B
            writeCount = 0
            For Each agent As DataRow In agents
                Dim sAgentStationName As String = agent.Field(Of String)("STATION_NAME")
                Dim sAgentCornerName As String = agent.Field(Of String)("CORNER_NAME")
                Dim agentUnitNumber As Integer = agent.Field(Of Integer)("UNIT_NO")

                '���Y�Ď��ՂƐڑ����Ă��Ȃ��܂���prevTime�ȍ~�ɐڑ������ꍇ�́A
                '����̊Ď����Ԃɂ����ĉ^�ǃT�[�o�̊Ď��Ղ̒ʐM�ُ킪�������|���L�ڂ��A
                '���̊Ď��Քz���̏��́A����ȏ�L�ڂ��Ȃ��B
                Dim oConnectDate As Object = SelectDirectConnectDate(dbCtl, EkConstants.ModelCodeKanshiban, agent, DbConstants.PortPurposeGeneralData)
                If oConnectDate Is Nothing OrElse _
                   CType(oConnectDate, DateTime) >= prevTime Then
                    'NOTE: �w���ƃR�[�i�[�������ŊĎ��Ղ���肷�邱�Ƃ��ł���̂�
                    '�s���ł���i�^�p�Ɉˑ�����j���߁A�f�t�H���g�̃��[�������ł́A
                    '�R�[�i�[���̌�Ɂu�Ď��Ձv�ƒf�����ꂽ�����ŁA���@�ԍ���
                    '�L�ڂ��邱�Ƃɂ��Ă���B
                    'TODO: �t�ɁA���Y�Ď��Ղ��S�����鑼�̃R�[�i�[�ɂ��Ă��A
                    '�^�ǂƂ̒ʐM�ُ킪�������Ă�|��\�����Ȃ��Ă悢�̂��H
                    '���[������M�������[�U�́A�@��\���ƏƂ炵���킹����ŁA
                    '�֌W������D�@�ňُ킪�������Ă���\���𐄎@���Ă����̂��H
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.KsbLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.KsbOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '�@��\���}�X�^����A���Y�Ď��Քz���Ō��݉^�p����Ă���ׂ�
                '���D�@�̈ꗗ�i�x�~���@�͏��O�j���@��ڑ���Ԃ̗���Ŏ擾����B
                '�܂��A�ꗗ�ɂ́ATRUST����t���������̂Ƃ���B
                '���̗�́A�@��ڑ���Ԃ��o�^����Ă��Ȃ��ꍇ��
                '�@��ڑ���Ԃ̎��W�����Â��ꍇ�i�Ď��ՂƐڑ��͂ł��Ă��Ă��A
                '�Ď��Ղ�����D�@�ڑ���Ԃ̒ʒm�������ꍇ�j�ɁANULL�ɂȂ�B
                Dim units As DataRowCollection = SelectGateUnitsWithConStatusInService(dbCtl, EkConstants.ModelCodeGate, agent, sServiceDate, curTime).Rows

                'TRUST��NULL�ɂȂ��Ă���@�킪�z���ɂP�ł�����Ď��Ղɂ��ẮA
                '�S�Ă̋@��ڑ���Ԃ��M���ł��Ȃ��|���L�ڂ��A���̊Ď��Քz���̏��́A
                '����ȏ�L�ڂ��Ȃ��B
                Dim isTrustAll As Boolean = True
                For Each unit As DataRow In units
                    If unit.Field(Of String)("TRUST") Is Nothing Then
                        isTrustAll = False
                        Exit For
                    End If
                Next unit
                If Not isTrustAll Then
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.KsbLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.KsbOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '�@��\������擾�������D�@�P�ʂŏ������s���B
                For Each unit As DataRow In units
                    Dim sStationName As String = unit.Field(Of String)("STATION_NAME")
                    Dim sCornerName As String = unit.Field(Of String)("CORNER_NAME")
                    Dim unitNumber As Integer = unit.Field(Of Integer)("UNIT_NO")

                    '���D�@�d�����~�Ȃ�A���̎|���L�ڂ��A���̉��D�@�̏��́A
                    '����ȏ�L�ڂ��Ȃ��B
                    If unit.Field(Of Integer)("KAIDENGEN") = 2 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                        oMailBody.AppendLine(Lexis.GatePowerErrorInConStatusMailBody.Gen())
                        Continue For
                    End If

                    '�Ď��Ձ|�吧�䕔���~�Ȃ�A���̎|���L�ڂ��A���̉��D�@�̏��́A
                    '����ȏ�L�ڂ��Ȃ��B
                    If unit.Field(Of Integer)("KANSICONNECT") = 1 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                        oMailBody.AppendLine(Lexis.GateMainKsbErrorInConStatusMailBody.Gen())
                        Continue For
                    End If

                    '�吧�䕔�|ICU���~�Ȃ�A���̎|���L�ڂ��A���̉��D�@�̏��́A
                    '����ȏ�L�ڂ��Ȃ��B
                    If unit.Field(Of Integer)("SHUSECONNECT") = 1 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                        oMailBody.AppendLine(Lexis.GateMainIcuErrorInConStatusMailBody.Gen())
                        Continue For
                    End If

                    '���̑��́~������΁A����s�ɂ��̎|���L�ڂ���B
                    If unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("HAISINICMCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))

                        Dim fieldPos As Integer = 0
                        If unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 Then
                            oMailBody.Append(Lexis.GateMainDsvErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If
                        If unit.Field(Of Integer)("HAISINICMCONNECT") = 1 Then
                            If fieldPos <> 0 Then
                                oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                            End If
                            oMailBody.Append(Lexis.GateIcuDsvErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If
                        If unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 Then
                            If fieldPos <> 0 Then
                                oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                            End If
                            oMailBody.Append(Lexis.GateIcuTktErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If

                        oMailBody.AppendLine()
                    End If
                Next unit
            Next agent

            'Ver0,1 ADD START ������ԏ��̒ǉ��Ή�
            '�@��\���}�X�^����A���݉^�p����Ă���ׂ�������
            '�ꗗ�i�x�~���@�͏��O�j���@��ڑ���Ԃ̗���Ŏ擾����B
            '�ꗗ�ɂ́ATRUST����t���������̂Ƃ���B
            '���̗�́A�@��ڑ���Ԃ��o�^����Ă��Ȃ��ꍇ��
            '�@��ڑ���Ԃ̎��W�����Â��ꍇ�ɁANULL�ɂȂ�B
            agents = SelectTktUnitsWithConStatusInService(dbCtl, EkConstants.ModelCodeTokatsu, sServiceDate, curTime).Rows

            '���[���{�����̓�����ԏ��Z�N�V������ҏW�B
            '�@��\������擾���������P�ʂŏ������s���B
            writeCount = 0
            For Each agent As DataRow In agents
                Dim sStationName As String = agent.Field(Of String)("STATION_NAME")
                Dim sCornerName As String = agent.Field(Of String)("CORNER_NAME")
                Dim unitNumber As Integer = agent.Field(Of Integer)("UNIT_NO")

                'TRUST��NULL�ɂȂ��Ă��铝���ɂ��ẮA
                '�@��ڑ���Ԃ��M���ł��Ȃ��|���L�ڂ��A
                '����ȏ�̏��͋L�ڂ��Ȃ��B
                If agent.Field(Of String)("TRUST") Is Nothing Then
                    'NOTE: �w���ƃR�[�i�[�������œ�������肷�邱�Ƃ��ł���̂�
                    '�s���ł���i�^�p�Ɉˑ�����j���߁A�f�t�H���g�̃��[�������ł́A
                    '�R�[�i�[���̌�Ɂu�����v�ƒf�����ꂽ�����ŁA���@�ԍ���
                    '�L�ڂ��邱�Ƃɂ��Ă���B
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.TktPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '���Y�����Ɛڑ����Ă��Ȃ��܂���prevTime�ȍ~�ɐڑ������ꍇ�́A
                '����̊Ď����Ԃɂ����ĉ^�ǃT�[�o�̓����̒ʐM�ُ킪�������|���L�ڂ��A
                '���̓����̏��́A����ȏ�L�ڂ��Ȃ��B
                Dim oConnectDate As Object = SelectDirectConnectDate(dbCtl, EkConstants.ModelCodeTokatsu, agent, DbConstants.PortPurposeGeneralData)
                If oConnectDate Is Nothing OrElse _
                   CType(oConnectDate, DateTime) >= prevTime Then
                    'NOTE: �w���ƃR�[�i�[�������œ�������肷�邱�Ƃ��ł���̂�
                    '�s���ł���i�^�p�Ɉˑ�����j���߁A�f�t�H���g�̃��[�������ł́A
                    '�R�[�i�[���̌�Ɂu�����v�ƒf�����ꂽ�����ŁA���@�ԍ���
                    '�L�ڂ��邱�Ƃɂ��Ă���B
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.TktPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                If agent.Field(Of Integer)("IDCENTERCONNECT") = 1 Then
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.TktPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                    oMailBody.AppendLine(Lexis.TktIdcErrorInConStatusMailBody.Gen())
                End If
            Next agent
            'Ver0,1 ADD END ������ԏ��̒ǉ��Ή�

            '�@��\���}�X�^����A���݉^�p����Ă���ׂ������̈ꗗ�i�x�~���@�͏��O�j���擾����B
            agents = SelectUnitsInService(dbCtl, EkConstants.ModelCodeTokatsu, sServiceDate).Rows

            '���[���{�����̑�����ԏ��Z�N�V������ҏW�B
            '�@��\������擾���������P�ʂŏ������s���B
            writeCount = 0
            For Each agent As DataRow In agents
                Dim sAgentStationName As String = agent.Field(Of String)("STATION_NAME")
                Dim sAgentCornerName As String = agent.Field(Of String)("CORNER_NAME")
                Dim agentUnitNumber As Integer = agent.Field(Of Integer)("UNIT_NO")

                '���Y�����Ɛڑ����Ă��Ȃ��܂���prevTime�ȍ~�ɐڑ������ꍇ�́A
                '����̊Ď����Ԃɂ����ĉ^�ǃT�[�o�̓����̒ʐM�ُ킪�������|���L�ڂ��A
                '���̓����z���̏��́A����ȏ�L�ڂ��Ȃ��B
                Dim oConnectDate As Object = SelectDirectConnectDate(dbCtl, EkConstants.ModelCodeTokatsu, agent, DbConstants.PortPurposeGeneralData)
                If oConnectDate Is Nothing OrElse _
                   CType(oConnectDate, DateTime) >= prevTime Then
                    'NOTE: �w���ƃR�[�i�[�������œ�������肷�邱�Ƃ��ł���̂�
                    '�s���ł���i�^�p�Ɉˑ�����j���߁A�f�t�H���g�̃��[�������ł́A
                    '�R�[�i�[���̌�Ɂu�����v�ƒf�����ꂽ�����ŁA���@�ԍ���
                    '�L�ڂ��邱�Ƃɂ��Ă���B
                    'TODO: �t�ɁA���Y�������S�����鑼�̃R�[�i�[�ɂ��Ă��A
                    '�^�ǂƂ̒ʐM�ُ킪�������Ă�|��\�����Ȃ��Ă悢�̂��H
                    '���[������M�������[�U�́A�@��\���ƏƂ炵���킹����ŁA
                    '�֌W���鑋���ňُ킪�������Ă���\���𐄎@���Ă����̂��H
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.MadoPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '�@��\���}�X�^����A���Y�����z���Ō��݉^�p����Ă���ׂ�
                '�����̈ꗗ�i�x�~���@�͏��O�j���@��ڑ���Ԃ̗���Ŏ擾����B
                '�܂��A�ꗗ�ɂ́ATRUST����t���������̂Ƃ���B
                '���̗�́A�@��ڑ���Ԃ��o�^����Ă��Ȃ��ꍇ��
                '�@��ڑ���Ԃ̎��W�����Â��ꍇ�ɁANULL�ɂȂ�B
                Dim units As DataRowCollection = SelectMadoUnitsWithConStatusInService(dbCtl, EkConstants.ModelCodeMadosho, agent, sServiceDate, curTime).Rows

                'TRUST��NULL�ɂȂ��Ă���@�킪�z���ɂP�ł����铝���ɂ��ẮA
                '�S�Ă̋@��ڑ���Ԃ��M���ł��Ȃ��|���L�ڂ��A���̓����z���̏��́A
                '����ȏ�L�ڂ��Ȃ��B
                Dim isTrustAll As Boolean = True
                For Each unit As DataRow In units
                    If unit.Field(Of String)("TRUST") Is Nothing Then
                        isTrustAll = False
                        Exit For
                    End If
                Next unit
                If Not isTrustAll Then
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.MadoPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '�@��\������擾���������P�ʂŏ������s���B
                For Each unit As DataRow In units
                    If unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("EXTOKATUDLCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("KANSICONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 Then

                        Dim sStationName As String = unit.Field(Of String)("STATION_NAME")
                        Dim sCornerName As String = unit.Field(Of String)("CORNER_NAME")
                        Dim unitNumber As Integer = unit.Field(Of Integer)("UNIT_NO")

                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.MadoPartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.MadoLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))

                        Dim fieldPos As Integer = 0
                        If unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 Then
                            oMailBody.Append(Lexis.MadoTktIdErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If
                        If unit.Field(Of Integer)("EXTOKATUDLCONNECT") = 1 Then
                            If fieldPos <> 0 Then
                                oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                            End If
                            oMailBody.Append(Lexis.MadoTktDlErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        Else
                            If unit.Field(Of Integer)("KANSICONNECT") = 1 Then
                                If fieldPos <> 0 Then
                                    oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                                End If
                                oMailBody.Append(Lexis.MadoKsbErrorInConStatusMailBody.Gen())
                                fieldPos += 1
                            End If
                            If unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 Then
                                If fieldPos <> 0 Then
                                    oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                                End If
                                oMailBody.Append(Lexis.MadoDsvErrorInConStatusMailBody.Gen())
                                fieldPos += 1
                            End If
                        End If

                        oMailBody.AppendLine()
                    End If
                Next unit
            Next agent

            'NOTE: �^�ǂƑ����̃R�l�N�V�����ɂ��ẮA���s�@��
            '�\���̋@�\���Ȃ����Ƃ���A�����ł��ʒm�͕s�v�炵���B
            '�i���s�@�́A�펞�ڑ��łȂ��̂ɔ���ΏۊO��
            '�Ȃ��Ă���̂��Ǝv���邪...�j

            'NOTE: ���p�f�[�^�p�̃R�l�N�V�����ɂ��Ă��A���[���@�\��
            '�L����JR�����{�ł́A�d�v�ł͂Ȃ����߁A���[���ɂ��ʒm��
            '�s�v�ł�����̂Ƃ���B

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return
        Finally
            dbCtl.ConnectClose()
        End Try

        If oMailBody.Length <> 0 Then
            Using oMail As New MailMessage()
                Try
                    '���[���w�b�_��FROM�∶���ҏW�B
                    oMail.From =  New MailAddress(Config.MailFromAddr)
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
                    Dim sTimestamp As String = curTime.ToString(Lexis.DateTimeFormatInConStatusMailSubject.Gen())
                    oMail.Subject = String.Format( _
                       "=?{0}?B?{1}?=", _
                       oSubjectEncoding.BodyName, _
                       Convert.ToBase64String(oSubjectEncoding.GetBytes(Lexis.ConStatusMailSubject.Gen(sTimestamp)), Base64FormattingOptions.None))

                    '���[���{����ҏW�B
                    Dim oAltView As AlternateView = _
                       AlternateView.CreateAlternateViewFromString( _
                          oMailBody.ToString(), _
                          Encoding.GetEncoding(Config.MailBodyEncoding), _
                          MediaTypeNames.Text.Plain)
                    oAltView.TransferEncoding = Config.MailTransferEncoding
                    oMail.AlternateViews.Add(oAltView)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    Return
                End Try

                Try
                    '���[���𑗐M�B
                    oSmtpClient.Send(oMail)
                    Log.Info("���[���𑗐M���܂����B")
                Catch ex As SmtpException
                    Log.Error("Exception caught.", ex)
                    Log.Error("���M���s���[�����e:" & vbCrLf & oMail.Subject & vbCrLf & oMailBody.ToString())
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                End Try
            End Using
        End If
    End Sub

    Private Shared Function SelectUnitsInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal sServiceDate As String) As DataTable
        Dim sSQL As String = _
           "SELECT M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.ADDRESS, M.STATION_NAME, M.CORNER_NAME" _
           & " FROM M_MACHINE M" _
           & " WHERE M.MODEL_CODE = '" & sModel & "'" _
           & " AND M.ADDRESS <> ''" _
           & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
           & " AND NOT EXISTS (SELECT *" _
                              & " FROM M_RESTING_MACHINE R" _
                              & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                              & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                              & " AND M.CORNER_CODE = R.CORNER_CODE" _
                              & " AND M.MODEL_CODE = R.MODEL_CODE" _
                              & " AND M.UNIT_NO = R.UNIT_NO)"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function

    'Ver0,1 ADD START ������ԏ��̒ǉ��Ή�
    Private Shared Function SelectTktUnitsWithConStatusInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal sServiceDate As String, ByVal curTime As DateTime) As DataTable
        Dim sTrustLimitTime As String = curTime.AddMilliseconds(-Config.MadoConStatusTrustLimitTicks).ToString("yyyy/MM/dd HH:mm:ss.fff")
        Dim sSQL As String = _
           "SELECT T.*, S.MODEL_CODE AS TRUST, S.IDCENTERCONNECT" _
           & " FROM (" _
             & "SELECT M.MODEL_CODE, M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.STATION_NAME, M.CORNER_NAME" _
             & " FROM M_MACHINE M" _
             & " WHERE M.MODEL_CODE = '" & sModel & "'" _
             & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                          & " FROM M_MACHINE" _
                                          & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
             & " AND NOT EXISTS (SELECT *" _
                                & " FROM M_RESTING_MACHINE R" _
                                & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                                & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                                & " AND M.CORNER_CODE = R.CORNER_CODE" _
                                & " AND M.MODEL_CODE = R.MODEL_CODE" _
                                & " AND M.UNIT_NO = R.UNIT_NO)) AS T" _
           & " LEFT JOIN (" _
             & "SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                   & " IDCENTERCONNECT" _
               & " FROM D_CON_STATUS" _
               & " WHERE SYUSYU_DATE >= '" & sTrustLimitTime & "') AS S" _
           & " ON T.MODEL_CODE = S.MODEL_CODE" _
           & " AND T.RAIL_SECTION_CODE = S.RAIL_SECTION_CODE" _
           & " AND T.STATION_ORDER_CODE = S.STATION_ORDER_CODE" _
           & " AND T.CORNER_CODE = S.CORNER_CODE" _
           & " AND T.UNIT_NO = S.UNIT_NO"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function
    'Ver0,1 ADD END ������ԏ��̒ǉ��Ή�

    Private Shared Function SelectGateUnitsWithConStatusInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal agent As DataRow, ByVal sServiceDate As String, ByVal curTime As DateTime) As DataTable
        Dim sTrustLimitTime As String = curTime.AddMilliseconds(-Config.GateConStatusTrustLimitTicks).ToString("yyyy/MM/dd HH:mm:ss.fff")
        Dim sSQL As String = _
           "SELECT T.*, S.MODEL_CODE AS TRUST, S.KAIDENGEN, S.KANSICONNECT, S.SHUSECONNECT, S.HAISINSYUCONNECT, S.HAISINICMCONNECT, S.EXTOKATUCONNECT" _
           & " FROM (" _
             & "SELECT M.MODEL_CODE, M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.STATION_NAME, M.CORNER_NAME" _
             & " FROM M_MACHINE M" _
             & " WHERE M.MODEL_CODE = '" & sModel & "'" _
             & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                          & " FROM M_MACHINE" _
                                          & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
             & " AND M.MONITOR_ADDRESS = '" & agent.Field(Of String)("ADDRESS") & "'" _
             & " AND NOT EXISTS (SELECT *" _
                                & " FROM M_RESTING_MACHINE R" _
                                & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                                & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                                & " AND M.CORNER_CODE = R.CORNER_CODE" _
                                & " AND M.MODEL_CODE = R.MODEL_CODE" _
                                & " AND M.UNIT_NO = R.UNIT_NO)) AS T" _
           & " LEFT JOIN (" _
             & "SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                   & " KAIDENGEN, KANSICONNECT, SHUSECONNECT, HAISINSYUCONNECT, HAISINICMCONNECT, EXTOKATUCONNECT" _
               & " FROM D_CON_STATUS" _
               & " WHERE SYUSYU_DATE >= '" & sTrustLimitTime & "') AS S" _
           & " ON T.MODEL_CODE = S.MODEL_CODE" _
           & " AND T.RAIL_SECTION_CODE = S.RAIL_SECTION_CODE" _
           & " AND T.STATION_ORDER_CODE = S.STATION_ORDER_CODE" _
           & " AND T.CORNER_CODE = S.CORNER_CODE" _
           & " AND T.UNIT_NO = S.UNIT_NO"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function

    Private Shared Function SelectMadoUnitsWithConStatusInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal agent As DataRow, ByVal sServiceDate As String, ByVal curTime As DateTime) As DataTable
        Dim sTrustLimitTime As String = curTime.AddMilliseconds(-Config.MadoConStatusTrustLimitTicks).ToString("yyyy/MM/dd HH:mm:ss.fff")
        Dim sSQL As String = _
           "SELECT T.*, S.MODEL_CODE AS TRUST, S.KANSICONNECT, S.HAISINSYUCONNECT, S.EXTOKATUDLCONNECT, S.EXTOKATUCONNECT" _
           & " FROM (" _
             & "SELECT M.MODEL_CODE, M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.STATION_NAME, M.CORNER_NAME" _
             & " FROM M_MACHINE M" _
             & " WHERE M.MODEL_CODE = '" & sModel & "'" _
             & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                          & " FROM M_MACHINE" _
                                          & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
             & " AND M.MONITOR_ADDRESS = '" & agent.Field(Of String)("ADDRESS") & "'" _
             & " AND NOT EXISTS (SELECT *" _
                                & " FROM M_RESTING_MACHINE R" _
                                & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                                & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                                & " AND M.CORNER_CODE = R.CORNER_CODE" _
                                & " AND M.MODEL_CODE = R.MODEL_CODE" _
                                & " AND M.UNIT_NO = R.UNIT_NO)) AS T" _
           & " LEFT JOIN (" _
             & "SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                   & " KANSICONNECT, HAISINSYUCONNECT, EXTOKATUDLCONNECT, EXTOKATUCONNECT" _
               & " FROM D_CON_STATUS" _
               & " WHERE SYUSYU_DATE >= '" & sTrustLimitTime & "') AS S" _
           & " ON T.MODEL_CODE = S.MODEL_CODE" _
           & " AND T.RAIL_SECTION_CODE = S.RAIL_SECTION_CODE" _
           & " AND T.STATION_ORDER_CODE = S.STATION_ORDER_CODE" _
           & " AND T.CORNER_CODE = S.CORNER_CODE" _
           & " AND T.UNIT_NO = S.UNIT_NO"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function

    Private Shared Function SelectDirectConnectDate(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal agent As DataRow, ByVal sPortPurpose As String) As Object
        Dim sSQL As String = _
           "SELECT CONNECT_DATE" _
           & " FROM S_DIRECT_CON_STATUS" _
           & " WHERE MODEL_CODE = '" & sModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & agent.Field(Of String)("RAIL_SECTION_CODE") & "'" _
           & " AND STATION_ORDER_CODE = '" & agent.Field(Of String)("STATION_ORDER_CODE") & "'" _
           & " AND CORNER_CODE = " & agent.Field(Of Integer)("CORNER_CODE").ToString() _
           & " AND UNIT_NO = " & agent.Field(Of Integer)("UNIT_NO").ToString() _
           & " AND PORT_KBN = '" & sPortPurpose & "'"
        Return dbCtl.ExecuteSQLToReadScalar(sSQL)
    End Function

End Class
