' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2013/11/07  (NES)�͘e  �t�F�[�Y�Q�Ή�
'                                   �ESNMPTrap�Ώۋy�у��[���ΏۑΉ�
'   0.2      2014/06/01       ����  �k���ُ탁�[���Ή�
'   0.3      2014/06/01  (NES)�͘e  �ُ탁�[���ڍגǉ��Ή�
'   0.4      2017/04/10  (NES)����  ������ԕ�Ή��ɂāA���[��������
'                                   ���@�ԍ���Integer�œ��͂���i�����ւ�
'                                   ���ɏ������w��\�Ƃ���j�悤�ɓ���
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Text
Imports System.Threading
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp

''' <summary>
''' �ُ�f�[�^��DB�o�^
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "�錾�̈�iPrivate�j"
    Private Structure MailSource
        Public RailSectionCode As String
        Public StationOrderCode As String
        Public CornerCode As String
        Public ModelCode As String
        Public UnitNo As Integer
        Public OccurDate As String
        Public ErrCode As String
        Public ErrItem As String
        'Ver0.3 ADD
        Public Dtl_Info As String
        'Ver0.3 MOD
        Public Sub New( _
           ByVal sRailSectionCode As String, _
           ByVal sStationOrderCode As String, _
           ByVal sCornerCode As String, _
           ByVal sModelCode As String, _
           ByVal unitNo As Integer, _
           ByVal sOccurDate As String, _
           ByVal sErrCode As String, _
           ByVal sErrItem As String, _
           ByVal sDtl_Info As String)
            Me.RailSectionCode = sRailSectionCode
            Me.StationOrderCode = sStationOrderCode
            Me.CornerCode = sCornerCode
            Me.ModelCode = sModelCode
            Me.UnitNo = unitNo
            Me.OccurDate = sOccurDate
            Me.ErrCode = sErrCode
            Me.ErrItem = sErrItem
            'Ver0.3 ADD
            Me.Dtl_Info = sDtl_Info
        End Sub
    End Structure
    Private Shared iniInfoAry() As RecDataStructure.DefineInfo
    Private Shared mailStartMinutesInDay As Integer
    Private Shared mailEndMinutesInDay As Integer
    Private Shared oMailQueue As Queue(Of MailSource)
    Private Shared oMailEvent As ManualResetEvent
    'Ver0.1 ADD
    Private Shared isFtpData As Boolean     'Ftp�f�[�^�L��
#End Region

#Region "���\�b�h�iMain�j"
    ''' <summary>
    ''' �ُ�f�[�^�捞�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �ُ�f�[�^�捞�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForFaultData")
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

                Log.Init(sLogBasePath, "ForFaultData")
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

                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "FaultData_001", iniInfoAry) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                If Not Config.MailSmtpServerName.Equals("") Then
                    '�L�����ԑт̊J�n�����ƏI���������i0��0������̌o�ߕ��̌`���Łj
                    'mailStartMinutesInDay��mailEndMinutesInDay�ɎZ�o���Ă����B
                    '���̍ہAmailStartMinutesInDay <= mailEndMinutesInDay�ɂȂ�悤�A
                    '�K�v�ɉ�����mailEndMinutesInDay�ɂ͕␳�������Ă����B
                    'NOTE: mailStartMinutesInDay == mailEndMinutesInDay�͐����Ȑݒ�
                    '�ł���A�L�����ԑт����̂P���Ԃ����ł��邱�Ƃ��Ӗ�����B
                    mailStartMinutesInDay = Config.MailStartHour * 60 + Config.MailStartMinute
                    mailEndMinutesInDay = Config.MailEndHour * 60 + Config.MailEndMinute
                    If mailStartMinutesInDay > mailEndMinutesInDay Then
                        mailEndMinutesInDay += 24 * 60
                    End If

                    'NOTE: oMailEvent�̎Q�Ɛ�I�u�W�F�N�g���s�v�ɂȂ�̂́A
                    '�ǂ݂̂��v���Z�X���I������Ƃ��ł���B����āA
                    '����Dispose�́A���̍ۂ̃K�[�x�[�W�R���N�V�����Ɉς˂�B
                    oMailQueue = New Queue(Of MailSource)
                    oMailEvent = New ManualResetEvent(False)
                    Dim oMailerThread As New Thread(AddressOf MainClass.MailingLoop)
                    oMailerThread.IsBackground = True
                    oMailerThread.Name = "Mailer"
                    oMailerThread.Start()
                End If

                RecServerAppBaseMain(AddressOf RecordToDatabase)
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
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
    '''  �ُ�f�[�^�捞
    ''' </summary>
    ''' <param name="sFilePath">�o�^����ׂ��f�[�^���i�[���ꂽ�t�@�C���̐�΃p�X��</param>
    ''' <returns>�o�^�̌���</returns>
    ''' <remarks>
    ''' �f�[�^�o�^�X���b�h�ŌĂяo�����B
    ''' </remarks>
    Private Shared Function RecordToDatabase(ByVal sFilePath As String) As RecordingResult
        Dim modelCode As Integer = UpboundDataPath.GetEkCode(sFilePath).Model
        Dim sModelCode As String = Format(modelCode, "00")
        Dim dlineInfoLst As List(Of String()) = Nothing
        Dim dlineInfoLstNew As List(Of String()) = Nothing
        Dim recResultFinal As RecordingResult
        Try

            'dat�t�@�C���f�[�^�擾
            If GetInfoFromDataFileComm(sFilePath, sModelCode, dlineInfoLst) = False Then
                Return RecordingResult.ParseError
            End If

            '�`�F�b�N
            If CheckData(dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                'NOTE: ���݂̂Ƃ��낱�������s����邱�Ƃ͂��蓾�Ȃ����A
                'CheckData���\�b�h��DB���Q�Ƃ���悤�ɉ��������΁A
                '���������s����邱�Ƃ�����͂��B�����āA���������s�����̂́A
                '�f�[�^�̏����ɕs��������ꍇ�ł͂Ȃ��ADB�̃A�N�Z�X�Ȃǂ�
                '���s�����i�����^�C���ُ̈킪���������j�ꍇ�ł���B
                '���̏ꍇ�A���[���̑��M��SNMP�ʒm���s����\�������邪�A
                '�s���Ȃ��\���̕��������i�����̏ꍇ��DB���Q�Ƃ���j
                '��A�ǂ݂̂�dlineInfoLstNew�ɂ͑S�Ă̐��탌�R�[�h���o�^
                '����Ă��Ȃ��i�������́A�s���ȃ��R�[�h���c���Ă���j����
                '����Ȃ��B����āA���[���̑��M��SNMP�ʒm�͒��߂āA
                '�����Ń��\�b�h���I�����邱�Ƃɂ���i���ہADB�ւ̓o�^��
                '���߂Ă���̂ŁA����͈�ѐ��̂������ł���j�B
                Return RecordingResult.IOError
            End If

            'DB�o�^
            If BatchAppComm.PutDataToDBCommon(iniInfoAry, dlineInfoLstNew, "D_FAULT_DATA") = True Then
                recResultFinal = RecordingResult.Success
            Else
                recResultFinal = RecordingResult.IOError
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen("A6", Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try

        'Ver0.1 ADD �d���f�[�^�̂Ƃ��̂�
        If Not isFtpData Then
            '���[����SNMP�ɂ��ʒm
            MailOrTrapIfNeeded(sModelCode, iniInfoAry, dlineInfoLstNew)
            'Ver0.1 ADD
        End If

        Return recResultFinal
    End Function
#End Region

#Region "���\�b�h�iPrivate�j"
    ''' <summary>
    ''' DAT�t�@�C���̉��
    ''' </summary>
    ''' <param name="datFileName">dat�t�@�C����</param>
    ''' <param name="clientKind"></param>
    '''  <param name="lineInfoLst">��͂����f�[�^</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Private Shared Function GetInfoFromDataFileComm(ByVal datFileName As String, _
                                                ByVal clientKind As String, _
                                                ByRef lineInfoLst As List(Of String())) As Boolean
        'Ver0.1 MOD �p�����[�^(isFtpData)�ǉ�
        'If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, datFileName, clientKind, 780, 17, lineInfoLst, "A6") = False Then
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, datFileName, clientKind, 780, 17, lineInfoLst, "A6", isFtpData) = False Then
            Return False
        End If

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim jIndex As Integer = 0
        Dim iFlag As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim iUnitNoW As Integer = 0 '�Ď��Տꍇ
        Dim iUnitNoG As Integer = 0 '�Ď��ՈȊO�ꍇ

        '�S�����R�[�h
        For i = 0 To lineInfoLst.Count - 1

            '1���R�[�h�擾
            lineInfo = lineInfoLst.Item(i)

            iFlag = 2
            '�S���t�B�[���h
            For j = 0 To iniInfoAry.Length - 1
                Select Case iniInfoAry(j).FIELD_NAME
                    Case "UNIT_NO" '���@
                        iFlag = iFlag - 1
                        jIndex = j
                        iUnitNoW = Integer.Parse(lineInfo(j))
                    Case "UNIT_NOG" '���@�ԍ�
                        iFlag = iFlag - 1
                        iUnitNoG = Integer.Parse(lineInfo(j))
                End Select
                If iFlag = 0 Then Exit For
            Next

            If iUnitNoW = 0 Then '�Ď��ՈȊO�ꍇ
                lineInfo(jIndex) = iUnitNoG.ToString
            End If
        Next

        Return True

    End Function

    ''' <summary>
    ''' �ُ�f�[�^�̃`�F�b�N
    ''' </summary>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dlineInfoLstNew">�`�F�b�N��A���m�Idat�t�@�C�����e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Private Shared Function CheckData(ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String) As Boolean

        Dim iFlag As Integer = 0

        Dim i As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim lineInfoNew(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:�G���[������;false:�G���[���Ȃ�

        dlineInfoLstNew = New List(Of String())

        '�S�����R�[�h
        For i = 0 To dlineInfoLst.Count - 1

            '1���R�[�h�擾
            lineInfo = dlineInfoLst.Item(i)

            '������
            isHaveErr = False
            iFlag = 2

            '�S���t�B�[���h
            For j As Integer = 0 To iniInfoAry.Length - 1

                If iFlag = 0 Then Exit For

                Select Case iniInfoAry(j).FIELD_NAME
                    Case "DATA_KIND" '�f�[�^���
                        iFlag = iFlag - 1
                        If (Not lineInfo(j) = "A6") AndAlso (Not lineInfo(j) = "C3") Then
                            isHaveErr = True
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (i + 1).ToString, "�f�[�^���"))
                            BatchAppComm.SetCollectionData(datFileName, "A6") '�t�@�C�������
                            Exit For
                        End If
                    Case "ERR_CODE" '�G���[�R�[�h
                        iFlag = iFlag - 1
                        If lineInfo(j) = "000000" Then
                            isHaveErr = True
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, (i + 1).ToString, iniInfoAry(j).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo)
                            Exit For
                        End If
                End Select
            Next

            If isHaveErr = False Then
                '���ʂ̃`�F�b�N
                If BatchAppComm.CheckDataComm(i + 1, iniInfoAry, lineInfo, datFileName) = False Then
                    Continue For
                End If
                dlineInfoLstNew.Add(lineInfo)
            End If
        Next

        Return True

    End Function

    ''' <summary>
    ''' SNMP�ʒm�̎��{����у��[�����M�X���b�h�ւُ̈�f�[�^���[�����M�˗����s���B
    ''' </summary>
    ''' <param name="sModelNumber">�v���g�R���`���̋@��R�[�h</param>
    ''' <param name="iniInfoAry">INI�t�@�C�����e</param>
    ''' <param name="dlineInfoLst">�f�[�^</param>
    Public Shared Sub MailOrTrapIfNeeded(ByVal sModelNumber As String, _
                                         ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                         ByVal dlineInfoLst As List(Of String()))

        Dim portNumber As Integer = BatchAppComm.GetPortNumber(sModelNumber)
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            For i As Integer = 0 To dlineInfoLst.Count - 1
                Dim sRailSectionCode As String = Nothing
                Dim sStationOrderCode As String = Nothing
                Dim sCornerCode As String = Nothing
                Dim unitNo As Integer
                Dim sOccurDate As String = Nothing
                Dim sModelCode As String = Nothing
                Dim sErrType As String = Nothing
                Dim sActStep As String = Nothing
                Dim sErrCode As String = Nothing
                Dim sErrItem As String = ""
                'Ver0.3 ADD
                Dim sDtl_Info As String = ""
                For j As Integer = 0 To iniInfoAry.Length - 1
                    'OPT: iniInfoAry�̏�񌹂͉^�ǃT�[�o���g�̒�`�t�@�C���Ȃ̂ŁA
                    '�啶�����͕s�v�ł���B��`�t�@�C�����P�[�X�C���Z���V�e�B�u��
                    '�������̂ł���΁A���x����炸��GetDefineInfo()�̒���
                    '��x�������{���Ă����΂悢�B
                    Select Case iniInfoAry(j).FIELD_NAME.ToUpperInvariant()
                        Case "RAIL_SECTION_CODE"
                            sRailSectionCode = dlineInfoLst.Item(i)(j)
                        Case "STATION_ORDER_CODE"
                            sStationOrderCode = dlineInfoLst.Item(i)(j)
                        Case "CORNER_CODE"
                            sCornerCode = dlineInfoLst.Item(i)(j)
                        Case "UNIT_NO"
                            unitNo = Integer.Parse(dlineInfoLst.Item(i)(j))
                        Case "OCCUR_DATE"
                            sOccurDate = dlineInfoLst.Item(i)(j)
                        Case "MODEL_CODE"
                            sModelCode = dlineInfoLst.Item(i)(j)
                        Case "ERROR_TYPE"
                            sErrType = dlineInfoLst.Item(i)(j)
                        Case "ACT_STEP"
                            sActStep = dlineInfoLst.Item(i)(j)
                        Case "ERR_CODE"
                            sErrCode = dlineInfoLst.Item(i)(j)
                        Case "ERR_ITEM"
                            sErrItem = dlineInfoLst.Item(i)(j)
                            'Ver0.3 ADD START
                        Case "DTL_INFO"
                            sDtl_Info = dlineInfoLst.Item(i)(j)
                            'Ver0.3 ADD END
                    End Select
                Next
                '----------- 0.2  �k���ُ탁�[���ʒm�Ή�   MOD  START------------------------
                Dim sSQL As String = _
                   "SELECT SNMP_SEVERITY" _
                   & " FROM M_NOTIFIABLE_ERR_CODE" _
                   & " WHERE ERR_CODE = '" & sErrCode & "'" _
                   & " AND MODEL_CODE = '" & sModelCode & "'" _
                   & " AND ((STATION_ORDER_CODE = '999' AND RAIL_SECTION_CODE = '999')" _
                    & " OR (STATION_ORDER_CODE = '999' AND RAIL_SECTION_CODE = '" & sRailSectionCode & "')" _
                    & " OR (STATION_ORDER_CODE = '" & sStationOrderCode & "' AND RAIL_SECTION_CODE = '" & sRailSectionCode & "'))"
                '----------- 0.2  �k���ُ탁�[���ʒm�Ή�   MOD  �@�@END------------------------
                Dim oSeverity As Object = dbCtl.ExecuteSQLToReadScalar(sSQL)
                If oSeverity IsNot Nothing Then
                    'NOTE: sModel�́AM_NOTIFIABLE_ERR_CODE�e�[�u���ɓo�^����Ă��邱�Ƃ���A
                    'SnmpAppNumberForWarningFaultOfModels��SnmpAppNumberForCriticalFaultOfModels��
                    '�o�^����Ă���@��ł�����̂Ƃ݂Ȃ��B
                    Dim appNumber As Integer = 0
                    Select Case CStr(oSeverity)
                        Case DbConstants.SnmpSeverityWarning
                            appNumber = Config.SnmpAppNumberForWarningFaultOfModels(sModelCode)
                        Case DbConstants.SnmpSeverityCritical
                            appNumber = Config.SnmpAppNumberForCriticalFaultOfModels(sModelCode)
                    End Select

                    If appNumber <> 0 Then
                        'TODO: �ُ�f�[�^�̔������������̏ꍇ�ɁAsErrType��sActStep��0�ɂȂ邱�Ƃ�
                        '�O��ɂ��Ă���B�����A�����łȂ��Ȃ�ASNMP_CALL���C�u�����̎d�l�ɍ��킹��
                        '�Ō�̈����́A�����p�ɓ��ʂɕҏW���Ȃ���΂Ȃ�Ȃ��B
                        SnmpTrap.Act( _
                           appNumber, _
                           sModelCode, _
                           sRailSectionCode, _
                           sStationOrderCode, _
                           sCornerCode, _
                           unitNo, _
                           portNumber, _
                           sErrType & sActStep & sErrCode)
                    End If

                    If Not Config.MailSmtpServerName.Equals("") Then
                        'mailStartMinutesInDay�ȏ�ɂȂ�悤�ɕ␳����
                        '���ݎ������i0��0������̌o�ߕ��̌`���Łj���߂�B
                        Dim now As DateTime = DateTime.Now
                        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
                        If mailStartMinutesInDay > nowMinutesInDay Then
                            nowMinutesInDay += 24 * 60
                        End If

                        '�L�����ԑт̏ꍇ�̂݃��[�����M���s���B
                        If nowMinutesInDay <= mailEndMinutesInDay Then
                            'Ver0.3 MOD
                            Dim mailSource As New MailSource(sRailSectionCode, sStationOrderCode, sCornerCode, sModelCode, unitNo, sOccurDate, sErrCode, sErrItem, sDtl_Info)
                            SyncLock oMailQueue
                                oMailQueue.Enqueue(mailSource)
                            End SyncLock
                            oMailEvent.Set()
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try

    End Sub

    ''' <summary>
    ''' ���[�����M�X���b�h�̃��C�����[�v�B
    ''' </summary>
    Private Shared Sub MailingLoop()

        Dim oSmtpClient As New SmtpClient()
        oSmtpClient.Host = Config.MailSmtpServerName
        oSmtpClient.Port = Config.MailSmtpPort
        oSmtpClient.Credentials = New NetworkCredential(Config.MailSmtpUserName, Config.MailSmtpPassword)
        oSmtpClient.Timeout = Config.MailSendLimitTicks

        Do
            oMailEvent.WaitOne()

            'NOTE: oMailEvent.Reset()�̌�ŃL���[�C���O����Ă�����̂�
            '�S�ď������邽�߁A���̃^�C�~���O�ŐV���Ȃ��̂��L���[�C���O
            '����AoMailEvent.Set()����Ă��\��Ȃ��B
            oMailEvent.Reset()
            Do
                Dim isSendFailedOnce As Boolean = False

                Dim mailSource As MailSource
                SyncLock oMailQueue
                    If oMailQueue.Count = 0 Then Exit Do
                    mailSource = oMailQueue.Dequeue()
                End SyncLock

                Using oMail As New MailMessage()
                    Dim sMailBody As String
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
                        Dim sStation As String
                        Dim sCorner As String
                        Dim sModel As String
                        Dim table As DataTable = SelectNames(mailSource.RailSectionCode, mailSource.StationOrderCode, mailSource.CornerCode, mailSource.ModelCode)
                        If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                            sStation = table.Rows(0).Field(Of String)("STATION_NAME")
                            sCorner = table.Rows(0).Field(Of String)("CORNER_NAME")
                            sModel = table.Rows(0).Field(Of String)("MODEL_NAME")
                        Else
                            sStation = mailSource.RailSectionCode & mailSource.StationOrderCode
                            sCorner = mailSource.CornerCode
                            sModel = mailSource.ModelCode
                        End If
                        Dim oSubjectEncoding As Encoding = Encoding.GetEncoding(Config.MailSubjectEncoding)
                        oMail.Subject = String.Format( _
                           "=?{0}?B?{1}?=", _
                           oSubjectEncoding.BodyName, _
                           Convert.ToBase64String(oSubjectEncoding.GetBytes(Lexis.FaultDataMailSubject.Gen(sStation, sCorner, sModel, mailSource.UnitNo)), Base64FormattingOptions.None))

                        '���[���{����ҏW�B
                        Dim occurDate As DateTime = DateTime.ParseExact(mailSource.OccurDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                        'Ver0.3 MOD START
                        If mailSource.ModelCode = "G" Then
                            sMailBody = Lexis.FaultDataMailBody.Gen(occurDate.ToString(Lexis.DateTimeFormatInFaultDataMailBody.Gen()), mailSource.ErrCode, mailSource.ErrItem & vbCrLf & mailSource.Dtl_Info)
                        Else
                            sMailBody = Lexis.FaultDataMailBody.Gen(occurDate.ToString(Lexis.DateTimeFormatInFaultDataMailBody.Gen()), mailSource.ErrCode, mailSource.ErrItem)
                        End If
                        'Ver0.3 MOD END
                        Dim oAltView As AlternateView = _
                           AlternateView.CreateAlternateViewFromString( _
                              sMailBody, _
                              Encoding.GetEncoding(Config.MailBodyEncoding), _
                              MediaTypeNames.Text.Plain)
                        oAltView.TransferEncoding = Config.MailTransferEncoding
                        oMail.AlternateViews.Add(oAltView)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                        Continue Do
                    End Try

                    If Config.MailSendFailureSpreads AndAlso isSendFailedOnce Then
                        Log.Error("���M�҂����[���̑��M�𒆎~���܂����B")
                        Log.Error("���M���~���[�����e:" & vbCrLf & oMail.Subject & vbCrLf & sMailBody)
                        Continue Do
                    End If

                    Try
                        '���[���𑗐M�B
                        oSmtpClient.Send(oMail)
                        Log.Info("���[���𑗐M���܂����B")
                    Catch ex As SmtpFailedRecipientsException
                        Log.Error("Exception caught.", ex)
                        Log.Error("���M���s���[�����e:" & vbCrLf & oMail.Subject & vbCrLf & sMailBody)
                    Catch ex As SmtpException
                        Log.Error("Exception caught.", ex)
                        Log.Error("���M���s���[�����e:" & vbCrLf & oMail.Subject & vbCrLf & sMailBody)
                        isSendFailedOnce = True
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End Using
            Loop
        Loop

    End Sub

    Private Shared Function SelectNames(ByVal sRailSectionCode As String, _
                                        ByVal sStationOrderCode As String, _
                                        ByVal sCornerCode As String, _
                                        ByVal sModelCode As String) As DataTable

        Dim sSQL As String = _
           "SELECT TOP 1 STATION_NAME, CORNER_NAME, MODEL_NAME" _
           & " FROM M_MACHINE" _
           & " WHERE RAIL_SECTION_CODE = '" & sRailSectionCode & "'" _
           & " AND STATION_ORDER_CODE = '" & sStationOrderCode & "'" _
           & " AND CORNER_CODE = " & sCornerCode _
           & " AND MODEL_CODE = '" & sModelCode & "'" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & EkServiceDate.GenString() & "')"

        Dim table As DataTable = Nothing
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            table = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
        Return table

    End Function
#End Region

End Class
