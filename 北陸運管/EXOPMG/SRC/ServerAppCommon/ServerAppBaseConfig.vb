' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Messaging

Imports JR.ExOpmg.Common

Public Class ServerAppBaseConfig
    Inherits BaseConfig

    '�v���Z�X���ʕ�����
    Public Shared AppIdentifier As String

    '�풓�v���Z�X�̎��ʕ�����ꗗ
    Public Shared ResidentApps As String()

    '�^�ǃT�[�o�����L���鎖�Ǝ�
    Public Shared SelfCompany As EkCompany

    '�^�ǃT�[�o���Ǌ�����G���A
    Public Shared SelfArea As Integer

    '�L�����O���
    Public Shared LogKindsMask As Integer

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    '���p�f�[�^�pDB��
    Public Shared RiyoDataDatabaseName As String

    '�V�����w�茔����f�[�^�pDB��
    Public Shared ShiteiDataDatabaseName As String
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    '��ʃT�C�Y
    Public Shared FormWidth As Integer
    Public Shared FormHeight As Integer

    '��ʕ\���ʒu
    Public Shared FormPosX As Integer
    Public Shared FormPosY As Integer

    '��ʃ^�C�g��
    Public Shared FormTitle As String

    '�Ǘ��n�X���b�h�̃|�[�����O�̊Ԋu
    Public Shared PollIntervalTicks As Integer

    '���Ȑf�f�Ԋu
    Public Shared SelfDiagnosisIntervalTicks As Integer

    '�����ؖ��t�@�C���̊Ǘ��f�B���N�g����
    Public Shared ResidentAppPulseDirPath As String

    '�}�X�^/�v���O�����̊Ǘ��f�B���N�g����
    Public Shared MasProDirPath As String

    '���p�f�[�^�̊Ǘ��f�B���N�g�����i�w�ʃf�B���N�g���̃x�[�X�j
    Public Shared RiyoDataDirPath As String

    '���p�f�[�^�̉w�ʃf�B���N�g�����̏���
    Public Shared RiyoDataStationBaseDirNameFormat As String

    '�����ΏۊO���p�f�[�^�i�[�f�B���N�g����
    Public Shared RiyoDataRejectDirPathInStationBase As String

    '��M����̗��p�f�[�^�i�[�f�B���N�g����
    Public Shared RiyoDataInputDirPathInStationBase As String

    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    '�o�^�ςݗ��p�f�[�^�i�[�f�B���N�g����
    Public Shared RiyoDataOutputDirPathInStationBase As String
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------

    '�W�v�����p�f�[�^�i�[�f�B���N�g����
    Public Shared RiyoDataTallyingDirPathInStationBase As String

    '�W�v�ςݗ��p�f�[�^�f�B���N�g���̈ړ���f�B���N�g����
    Public Shared RiyoDataTrashDirPathInStationBase As String

    '�e�o�^�n�v���Z�X�̏����ΏۊO�f�[�^�̊i�[�f�B���N�g����
    Public Shared RejectDirPathForApps As Dictionary(Of String, String)

    '�e�o�^�n�v���Z�X�̏����Ώۃf�[�^�̊i�[�f�B���N�g����
    Public Shared InputDirPathForApps As Dictionary(Of String, String)

    '�e�o�^�n�v���Z�X�̓����s���œo�^�ł��Ȃ������f�[�^�̊i�[�f�B���N�g����
    Public Shared SuspenseDirPathForApps As Dictionary(Of String, String)

    '�e�o�^�n�v���Z�X�̏����ُ�œo�^�ł��Ȃ������f�[�^�̊i�[�f�B���N�g����
    Public Shared QuarantineDirPathForApps As Dictionary(Of String, String)

    '�e�o�^�n�v���Z�X�̓o�^�ς݃f�[�^�̊i�[�f�B���N�g����
    Public Shared TrashDirPathForApps As Dictionary(Of String, String)

    '�e�o�^�n�v���Z�X���S������f�[�^�̎}�ԍő�l
    Public Shared MaxBranchNumberForApps As Dictionary(Of String, Integer)

    '���p�f�[�^�̎}�ԍő�l
    Public Shared RiyoDataMaxBranchNumber As Integer

    '�e�v���Z�X�̃��b�Z�[�W�L���[�̃p�X
    Public Shared MqPathForApps As Dictionary(Of String, String)

    '�e�v���Z�X�̃��b�Z�[�W�L���[�i�I�v�V�����j
    Public Shared MessageQueueForApps As Dictionary(Of String, MessageQueue)

    'INI�t�@�C�����̃Z�N�V������
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const FORM_SECTION As String = "Form"
    Protected Const MQ_SECTION As String = "MessageQueue"
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const NETWORK_SECTION As String = "Network"
    Protected Const TIME_INFO_SECTION As String = "TimeInfo"
    Protected Const TELEGRAPHER_MODE_SECTION As String = "TelegrapherMode"
    Protected Const SCHEDULE_SECTION As String = "Schedule"
    Protected Const SNMP_APP_NUMBER_SECTION As String = "SnmpAppNumber"
    Protected Const REGULATION_SECTION As String = "Regulation"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[�i�o�^�n�v���Z�X����Config�ł��g�p�\�j
    Protected Const BASE_DIR_PATH_KEY As String = "RecordingBaseDirPath"
    Protected Const REJECT_DIR_PATH_KEY As String = "RejectDirPathInRecordingBase"
    Protected Const INPUT_DIR_PATH_KEY As String = "InputDirPathInRecordingBase"
    Protected Const SUSPENSE_DIR_PATH_KEY As String = "SuspenseDirPathInRecordingBase"
    Protected Const QUARANTINE_DIR_PATH_KEY As String = "QuarantineDirPathInRecordingBase"
    Protected Const TRASH_DIR_PATH_KEY As String = "TrashDirPathInRecordingBase"
    Protected Const MAX_BRANCH_NUMBER_KEY As String = "MaxBranchNumber"
    Protected Const MQ_PATH_KEY As String = "MqPath"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const RESIDENT_APPS_KEY As String = "ResidentApps"
    Private Const SELF_COMPANY_KEY As String = "SelfCompany"
    Private Const SELF_AREA_KEY As String = "SelfArea"
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    Private Const RIYO_DATA_DATABASE_NAME_KEY As String = "RiyoDataDatabaseName"
    Private Const SHITEI_DATA_DATABASE_NAME_KEY As String = "ShiteiDataDatabaseName"
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------
    Private Const FORM_SIZE_KEY As String = "Size"
    Private Const FORM_POSITION_KEY As String = "Pos"
    Private Const FORM_TITLE_KEY As String = "Title"
    Private Const POLL_INTERVAL_KEY As String = "PollIntervalTicks"
    Private Const SELF_DIAGNOSIS_INTERVAL_KEY As String = "SelfDiagnosisIntervalTicks"
    Private Const PULSE_DIR_PATH_KEY As String = "ResidentAppPulseDirPath"
    Private Const MASPRO_DIR_PATH_KEY As String = "MasProDirPath"
    Private Const RIYO_DATA_DIR_PATH_KEY As String = "RiyoDataDirPath"
    Private Const RIYO_DATA_STATION_BASE_DIR_NAME_FORMAT_KEY As String = "RiyoDataStationBaseDirNameFormat"
    Private Const RIYO_DATA_REJECT_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataRejectDirPathInStationBase"
    Private Const RIYO_DATA_INPUT_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataInputDirPathInStationBase"
    '-------Ver0.1 ������ԕ�Ή� ADD START-----------
    Private Const RIYO_DATA_OUTPUT_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataOutputDirPathInStationBase"
    '-------Ver0.1 ������ԕ�Ή� ADD END-------------
    Private Const RIYO_DATA_TALLYING_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataTallyingDirPathInStationBase"
    Private Const RIYO_DATA_TRASH_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataTrashDirPathInStationBase"
    Private Const RIYO_DATA_MAX_BRANCH_NUMBER_KEY As String = "RiyoDataMaxBranchNumber"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̂ǂ̃v���Z�X�ɂ��K�{�̐ݒ�l����荞�ށB</summary>
    Public Shared Sub ServerAppBaseInit(ByVal sIniFilePath As String, ByVal sAppIdentifier As String, Optional ByVal needMessageQueue As Boolean = False)
        AppIdentifier = sAppIdentifier
        BaseInit(sIniFilePath)

        Dim arrTemp As String()
        Try
            ReadFileElem(CREDENTIAL_SECTION, RESIDENT_APPS_KEY)
            ResidentApps = LastReadValue.Split(","c)

            ReadFileElem(CREDENTIAL_SECTION, SELF_COMPANY_KEY)
            SelfCompany = DirectCast([Enum].Parse(GetType(EkCompany), LastReadValue), EkCompany)

            ReadFileElem(CREDENTIAL_SECTION, SELF_AREA_KEY)
            SelfArea = Integer.Parse(LastReadValue)

            ReadFileElem(LOGGING_SECTION, sAppIdentifier & LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            '-------Ver0.1 ������ԕ�Ή� ADD START-----------
            ReadFileElem(DATABASE_SECTION, RIYO_DATA_DATABASE_NAME_KEY)
            RiyoDataDatabaseName = LastReadValue

            ReadFileElem(DATABASE_SECTION, SHITEI_DATA_DATABASE_NAME_KEY)
            ShiteiDataDatabaseName = LastReadValue
            '-------Ver0.1 ������ԕ�Ή� ADD END-------------

            ReadFileElem(FORM_SECTION, FORM_SIZE_KEY)
            arrTemp = LastReadValue.Split(","c)
            FormWidth = Integer.Parse(arrTemp(0))
            FormHeight = Integer.Parse(arrTemp(1))

            ReadFileElem(FORM_SECTION, sAppIdentifier & FORM_POSITION_KEY)
            arrTemp = LastReadValue.Split(","c)
            FormPosX = Integer.Parse(arrTemp(0))
            FormPosY = Integer.Parse(arrTemp(1))

            ReadFileElem(FORM_SECTION, sAppIdentifier & FORM_TITLE_KEY)
            FormTitle = LastReadValue

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & POLL_INTERVAL_KEY)
            PollIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, sAppIdentifier & SELF_DIAGNOSIS_INTERVAL_KEY)
            SelfDiagnosisIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(PATH_SECTION, PULSE_DIR_PATH_KEY)
            ResidentAppPulseDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, MASPRO_DIR_PATH_KEY)
            MasProDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, RIYO_DATA_DIR_PATH_KEY)
            RiyoDataDirPath = LastReadValue

            ReadFileElem(PATH_SECTION, RIYO_DATA_STATION_BASE_DIR_NAME_FORMAT_KEY)
            RiyoDataStationBaseDirNameFormat = LastReadValue

            ReadFileElem(PATH_SECTION, RIYO_DATA_REJECT_DIR_PATH_IN_STATION_BASE_KEY)
            RiyoDataRejectDirPathInStationBase = LastReadValue

            ReadFileElem(PATH_SECTION, RIYO_DATA_INPUT_DIR_PATH_IN_STATION_BASE_KEY)
            RiyoDataInputDirPathInStationBase = LastReadValue

            '-------Ver0.1 ������ԕ�Ή� ADD START-----------
            ReadFileElem(PATH_SECTION, RIYO_DATA_OUTPUT_DIR_PATH_IN_STATION_BASE_KEY)
            RiyoDataOutputDirPathInStationBase = LastReadValue
            '-------Ver0.1 ������ԕ�Ή� ADD END-------------

            ReadFileElem(PATH_SECTION, RIYO_DATA_TALLYING_DIR_PATH_IN_STATION_BASE_KEY)
            RiyoDataTallyingDirPathInStationBase = LastReadValue

            ReadFileElem(PATH_SECTION, RIYO_DATA_TRASH_DIR_PATH_IN_STATION_BASE_KEY)
            RiyoDataTrashDirPathInStationBase = LastReadValue

            RejectDirPathForApps = New Dictionary(Of String, String)
            InputDirPathForApps = New Dictionary(Of String, String)
            SuspenseDirPathForApps = New Dictionary(Of String, String)
            QuarantineDirPathForApps = New Dictionary(Of String, String)
            TrashDirPathForApps = New Dictionary(Of String, String)
            MaxBranchNumberForApps = New Dictionary(Of String, Integer)

            ReadFileElem(PATH_SECTION, REJECT_DIR_PATH_KEY)
            Dim sRejectDirPath As String = LastReadValue
            ReadFileElem(PATH_SECTION, INPUT_DIR_PATH_KEY)
            Dim sInputDirPath As String = LastReadValue
            ReadFileElem(PATH_SECTION, SUSPENSE_DIR_PATH_KEY)
            Dim sSuspenseDirPath As String = LastReadValue
            ReadFileElem(PATH_SECTION, QUARANTINE_DIR_PATH_KEY)
            Dim sQuarantineDirPath As String = LastReadValue
            ReadFileElem(PATH_SECTION, TRASH_DIR_PATH_KEY)
            Dim sTrashDirPath As String = LastReadValue

            ReadFileElem(PATH_SECTION, "ConStatus" & BASE_DIR_PATH_KEY)
            RejectDirPathForApps.Add("ForConStatus", Utility.CombinePathWithVirtualPath(LastReadValue, sRejectDirPath))
            InputDirPathForApps.Add("ForConStatus", Utility.CombinePathWithVirtualPath(LastReadValue, sInputDirPath))
            SuspenseDirPathForApps.Add("ForConStatus", Utility.CombinePathWithVirtualPath(LastReadValue, sSuspenseDirPath))
            QuarantineDirPathForApps.Add("ForConStatus", Utility.CombinePathWithVirtualPath(LastReadValue, sQuarantineDirPath))
            TrashDirPathForApps.Add("ForConStatus", Utility.CombinePathWithVirtualPath(LastReadValue, sTrashDirPath))

            ReadFileElem(PATH_SECTION, "KsbConfig" & BASE_DIR_PATH_KEY)
            RejectDirPathForApps.Add("ForKsbConfig", Utility.CombinePathWithVirtualPath(LastReadValue, sRejectDirPath))
            InputDirPathForApps.Add("ForKsbConfig", Utility.CombinePathWithVirtualPath(LastReadValue, sInputDirPath))
            SuspenseDirPathForApps.Add("ForKsbConfig", Utility.CombinePathWithVirtualPath(LastReadValue, sSuspenseDirPath))
            QuarantineDirPathForApps.Add("ForKsbConfig", Utility.CombinePathWithVirtualPath(LastReadValue, sQuarantineDirPath))
            TrashDirPathForApps.Add("ForKsbConfig", Utility.CombinePathWithVirtualPath(LastReadValue, sTrashDirPath))

            ReadFileElem(PATH_SECTION, "BesshuData" & BASE_DIR_PATH_KEY)
            RejectDirPathForApps.Add("ForBesshuData", Utility.CombinePathWithVirtualPath(LastReadValue, sRejectDirPath))
            InputDirPathForApps.Add("ForBesshuData", Utility.CombinePathWithVirtualPath(LastReadValue, sInputDirPath))
            SuspenseDirPathForApps.Add("ForBesshuData", Utility.CombinePathWithVirtualPath(LastReadValue, sSuspenseDirPath))
            QuarantineDirPathForApps.Add("ForBesshuData", Utility.CombinePathWithVirtualPath(LastReadValue, sQuarantineDirPath))
            TrashDirPathForApps.Add("ForBesshuData", Utility.CombinePathWithVirtualPath(LastReadValue, sTrashDirPath))

            ReadFileElem(PATH_SECTION, "MeisaiData" & BASE_DIR_PATH_KEY)
            RejectDirPathForApps.Add("ForMeisaiData", Utility.CombinePathWithVirtualPath(LastReadValue, sRejectDirPath))
            InputDirPathForApps.Add("ForMeisaiData", Utility.CombinePathWithVirtualPath(LastReadValue, sInputDirPath))
            SuspenseDirPathForApps.Add("ForMeisaiData", Utility.CombinePathWithVirtualPath(LastReadValue, sSuspenseDirPath))
            QuarantineDirPathForApps.Add("ForMeisaiData", Utility.CombinePathWithVirtualPath(LastReadValue, sQuarantineDirPath))
            TrashDirPathForApps.Add("ForMeisaiData", Utility.CombinePathWithVirtualPath(LastReadValue, sTrashDirPath))

            ReadFileElem(PATH_SECTION, "FaultData" & BASE_DIR_PATH_KEY)
            RejectDirPathForApps.Add("ForFaultData", Utility.CombinePathWithVirtualPath(LastReadValue, sRejectDirPath))
            InputDirPathForApps.Add("ForFaultData", Utility.CombinePathWithVirtualPath(LastReadValue, sInputDirPath))
            SuspenseDirPathForApps.Add("ForFaultData", Utility.CombinePathWithVirtualPath(LastReadValue, sSuspenseDirPath))
            QuarantineDirPathForApps.Add("ForFaultData", Utility.CombinePathWithVirtualPath(LastReadValue, sQuarantineDirPath))
            TrashDirPathForApps.Add("ForFaultData", Utility.CombinePathWithVirtualPath(LastReadValue, sTrashDirPath))

            ReadFileElem(PATH_SECTION, "KadoData" & BASE_DIR_PATH_KEY)
            RejectDirPathForApps.Add("ForKadoData", Utility.CombinePathWithVirtualPath(LastReadValue, sRejectDirPath))
            InputDirPathForApps.Add("ForKadoData", Utility.CombinePathWithVirtualPath(LastReadValue, sInputDirPath))
            SuspenseDirPathForApps.Add("ForKadoData", Utility.CombinePathWithVirtualPath(LastReadValue, sSuspenseDirPath))
            QuarantineDirPathForApps.Add("ForKadoData", Utility.CombinePathWithVirtualPath(LastReadValue, sQuarantineDirPath))
            TrashDirPathForApps.Add("ForKadoData", Utility.CombinePathWithVirtualPath(LastReadValue, sTrashDirPath))

            ReadFileElem(PATH_SECTION, "TrafficData" & BASE_DIR_PATH_KEY)
            RejectDirPathForApps.Add("ForTrafficData", Utility.CombinePathWithVirtualPath(LastReadValue, sRejectDirPath))
            InputDirPathForApps.Add("ForTrafficData", Utility.CombinePathWithVirtualPath(LastReadValue, sInputDirPath))
            SuspenseDirPathForApps.Add("ForTrafficData", Utility.CombinePathWithVirtualPath(LastReadValue, sSuspenseDirPath))
            QuarantineDirPathForApps.Add("ForTrafficData", Utility.CombinePathWithVirtualPath(LastReadValue, sQuarantineDirPath))
            TrashDirPathForApps.Add("ForTrafficData", Utility.CombinePathWithVirtualPath(LastReadValue, sTrashDirPath))

            ReadFileElem(REGULATION_SECTION, "ConStatus" & MAX_BRANCH_NUMBER_KEY)
            MaxBranchNumberForApps.Add("ForConStatus", Integer.Parse(LastReadValue))

            ReadFileElem(REGULATION_SECTION, "KsbConfig" & MAX_BRANCH_NUMBER_KEY)
            MaxBranchNumberForApps.Add("ForKsbConfig", Integer.Parse(LastReadValue))

            ReadFileElem(REGULATION_SECTION, "BesshuData" & MAX_BRANCH_NUMBER_KEY)
            MaxBranchNumberForApps.Add("ForBesshuData", Integer.Parse(LastReadValue))

            ReadFileElem(REGULATION_SECTION, "MeisaiData" & MAX_BRANCH_NUMBER_KEY)
            MaxBranchNumberForApps.Add("ForMeisaiData", Integer.Parse(LastReadValue))

            ReadFileElem(REGULATION_SECTION, "FaultData" & MAX_BRANCH_NUMBER_KEY)
            MaxBranchNumberForApps.Add("ForFaultData", Integer.Parse(LastReadValue))

            ReadFileElem(REGULATION_SECTION, "KadoData" & MAX_BRANCH_NUMBER_KEY)
            MaxBranchNumberForApps.Add("ForKadoData", Integer.Parse(LastReadValue))

            ReadFileElem(REGULATION_SECTION, "TrafficData" & MAX_BRANCH_NUMBER_KEY)
            MaxBranchNumberForApps.Add("ForTrafficData", Integer.Parse(LastReadValue))

            ReadFileElem(REGULATION_SECTION, RIYO_DATA_MAX_BRANCH_NUMBER_KEY)
            RiyoDataMaxBranchNumber = Integer.Parse(LastReadValue)

            'NOTE: �����炭�v���Z�X�}�l�[�W�������Q�Ƃ��Ȃ�
            '�i����MessageQueueForApps���Q�Ƃ���j���A
            'MqPath�֘A�̃L�[���̂Ɉˑ�����ӏ����Ǐ������邽�߂ɁA
            '�����ō쐬���Ă���B
            MqPathForApps = New Dictionary(Of String, String)
            '-------Ver0.1 ������ԕ�Ή� ADD START-----------
            CreateItemOfMqPath("AlertMailer")
            '-------Ver0.1 ������ԕ�Ή� ADD END-------------
            CreateItemOfMqPath("ToKanshiban")
            CreateItemOfMqPath("ToTokatsu")
            CreateItemOfMqPath("ToMadosho")
            CreateItemOfMqPath("ToKanshiban2")
            CreateItemOfMqPath("ToMadosho2")
            CreateItemOfMqPath("ToNkan")
            CreateItemOfMqPath("ForConStatus")
            CreateItemOfMqPath("ForKsbConfig")
            CreateItemOfMqPath("ForBesshuData")
            CreateItemOfMqPath("ForMeisaiData")
            CreateItemOfMqPath("ForFaultData")
            CreateItemOfMqPath("ForKadoData")
            CreateItemOfMqPath("ForTrafficData")
            '-------Ver0.1 ������ԕ�Ή� ADD START-----------
            CreateItemOfMqPath("ForRiyoData")
            '-------Ver0.1 ������ԕ�Ή� ADD END-------------

            If needMessageQueue Then
                MessageQueueForApps = New Dictionary(Of String, MessageQueue)
                For Each oMqInfo As KeyValuePair(Of String, String) In MqPathForApps
                    MessageQueueForApps.Add(oMqInfo.Key, CreateMessageQueue(oMqInfo.Value))
                Next oMqInfo
            End If
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    'NOTE: ServerAppBaseInit��needMessageQueue��True��n�����ꍇ�̂݁A
    '�v���Z�X�I�����ɌĂяo���K�v����B
    Public Shared Sub ServerAppBaseDispose()
        If MessageQueueForApps IsNot Nothing Then
            For Each oMessageQueue As MessageQueue In MessageQueueForApps.Values
                oMessageQueue.Close()
            Next oMessageQueue
        End If
    End Sub

    Private Shared Sub CreateItemOfMqPath(ByVal sId As String)
        If ResidentApps.Contains(sId) Then
            ReadFileElem(MQ_SECTION, sId & MQ_PATH_KEY)
            MqPathForApps.Add(sId, LastReadValue)
        End If
    End Sub

    Private Shared Function CreateMessageQueue(ByVal sLastReadValue As String) As MessageQueue
        Dim oFilter As New MessagePropertyFilter()
        oFilter.ClearAll()
        oFilter.AppSpecific = True
        oFilter.Body = True

        Dim oMessageQueue As New MessageQueue(sLastReadValue)
        oMessageQueue.MessageReadPropertyFilter = oFilter
        oMessageQueue.Formatter = New XmlMessageFormatter(New Type() {GetType([String])})
        Return oMessageQueue
    End Function

End Class
