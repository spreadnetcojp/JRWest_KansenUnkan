' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/08/08  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

'TODO: �A�v���̓��쒆�ɕύX���������̂́A�������珜�����AMainForm.vb��
'UiStateClass�ɐ錾����B

Public Class Config
    Inherits BaseConfig

    Public Shared ReadOnly EmptyTime As DateTime = DateTime.MinValue
    Public Shared ReadOnly UnknownTime As DateTime = DateTime.MaxValue
    Public Shared ReadOnly UnknownHashValue As String = "(�s��)"

    Public Shared ReadOnly FieldNamesTypes As New Dictionary(Of String, Type) From { _
       {"LAST_CONFIRMED", GetType(DateTime)}, _
       {"MACHINE_ID", GetType(String)}, _
       {"TERM_MACHINE_ID", GetType(String)}, _
       {"STATION_NAME", GetType(String)}, _
       {"RAIL_SECTION_CODE", GetType(String)}, _
       {"STATION_ORDER_CODE", GetType(String)}, _
       {"CORNER_NAME", GetType(String)}, _
       {"CORNER_CODE", GetType(Integer)}, _
       {"MODEL_NAME", GetType(String)}, _
       {"MODEL_CODE", GetType(String)}, _
       {"UNIT_NO", GetType(Integer)}, _
       {"ADDRESS", GetType(String)}, _
       {"AREA_CODE", GetType(Integer)}}

    Public Shared ReadOnly MachineProfileFieldNames As String() = { _
        "STATION_NAME", _
        "RAIL_SECTION_CODE", _
        "STATION_ORDER_CODE", _
        "CORNER_NAME", _
        "CORNER_CODE", _
        "MODEL_NAME", _
        "MODEL_CODE", _
        "UNIT_NO", _
        "ADDRESS", _
        "AREA_CODE"}

    Public Shared MachineProfileFieldNamesIndices As Dictionary(Of String, Integer)

    Public Shared ReadOnly Table1FieldNames As String() = { _
        "MACHINE_ID", _
        "RAIL_SECTION_CODE", _
        "STATION_ORDER_CODE", _
        "STATION_NAME", _
        "CORNER_CODE", _
        "CORNER_NAME", _
        "UNIT_NO", _
        "LAST_CONFIRMED"}

    Public Shared ReadOnly Table1VisibleFieldNames As String() = { _
        "RAIL_SECTION_CODE", _
        "STATION_ORDER_CODE", _
        "STATION_NAME", _
        "CORNER_CODE", _
        "CORNER_NAME", _
        "UNIT_NO", _
        "LAST_CONFIRMED"}

    'NOTE: 1�͉w���\��OFF�ŕ\���A2�͉w���\��ON�ŕ\���A3�͏�ɕ\��
    Public Shared ReadOnly Table1VisibleFieldNamesKinds As New Dictionary(Of String, Integer) From { _
       {"RAIL_SECTION_CODE", 1}, _
       {"STATION_ORDER_CODE", 1}, _
       {"STATION_NAME", 2}, _
       {"CORNER_CODE", 1}, _
       {"CORNER_NAME", 2}, _
       {"UNIT_NO", 3}, _
       {"LAST_CONFIRMED", 3}}

    Public Shared ReadOnly Table1VisibleFieldNamesTitles As New Dictionary(Of String, String) From { _
       {"RAIL_SECTION_CODE", "����"}, _
       {"STATION_ORDER_CODE", "�w��"}, _
       {"STATION_NAME", "�w��"}, _
       {"CORNER_CODE", "�R�[�i�["}, _
       {"CORNER_NAME", "�R�[�i�[��"}, _
       {"UNIT_NO", "���@"}, _
       {"LAST_CONFIRMED", "�捞����"}}

    Public Shared ReadOnly Table1VisibleFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
       {"RAIL_SECTION_CODE", "000."}, _
       {"STATION_ORDER_CODE", "000."}, _
       {"STATION_NAME", "�����F�ތ�..."}, _
       {"CORNER_CODE", "00."}, _
       {"CORNER_NAME", "�����抷�o��..."}, _
       {"UNIT_NO", "00."}, _
       {"LAST_CONFIRMED", "0000/00/00..."}}

    Public Shared ReadOnly Table2FieldNames As String() = { _
        "MACHINE_ID", _
        "TERM_MACHINE_ID", _
        "MODEL_CODE", _
        "MODEL_NAME", _
        "RAIL_SECTION_CODE", _
        "STATION_ORDER_CODE", _
        "STATION_NAME", _
        "CORNER_CODE", _
        "CORNER_NAME", _
        "UNIT_NO"}

    Public Shared ReadOnly Table2VisibleFieldNames As String() = { _
        "MODEL_CODE", _
        "MODEL_NAME", _
        "RAIL_SECTION_CODE", _
        "STATION_ORDER_CODE", _
        "STATION_NAME", _
        "CORNER_CODE", _
        "CORNER_NAME", _
        "UNIT_NO"}

    'NOTE: 1�͉w���\��OFF�ŕ\���A2�͉w���\��ON�ŕ\���A3�͏�ɕ\��
    Public Shared ReadOnly Table2VisibleFieldNamesKinds As New Dictionary(Of String, Integer) From { _
       {"MODEL_CODE", 1}, _
       {"MODEL_NAME", 2}, _
       {"RAIL_SECTION_CODE", 1}, _
       {"STATION_ORDER_CODE", 1}, _
       {"STATION_NAME", 2}, _
       {"CORNER_CODE", 1}, _
       {"CORNER_NAME", 2}, _
       {"UNIT_NO", 3}}

    Public Shared ReadOnly Table2VisibleFieldNamesTitles As New Dictionary(Of String, String) From { _
       {"MODEL_CODE", "�@��"}, _
       {"MODEL_NAME", "�@��"}, _
       {"RAIL_SECTION_CODE", "����"}, _
       {"STATION_ORDER_CODE", "�w��"}, _
       {"STATION_NAME", "�w��"}, _
       {"CORNER_CODE", "�R�[�i�["}, _
       {"CORNER_NAME", "�R�[�i�[��"}, _
       {"UNIT_NO", "���@"}}

    Public Shared ReadOnly Table2VisibleFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
       {"MODEL_CODE", "�@.."}, _
       {"MODEL_NAME", "����..."}, _
       {"RAIL_SECTION_CODE", "000."}, _
       {"STATION_ORDER_CODE", "000."}, _
       {"STATION_NAME", "�����F�ތ�..."}, _
       {"CORNER_CODE", "00."}, _
       {"CORNER_NAME", "�����抷�o��..."}, _
       {"UNIT_NO", "00."}}

    Public Shared MenuTableOfTktNegaStatus As DataTable
    Public Shared MenuTableOfTktMeisaiStatus As DataTable
    Public Shared MenuTableOfTktOnlineStatus As DataTable
    Public Shared MenuTableOfMadoDlsStatus As DataTable
    Public Shared MenuTableOfMadoKsbStatus As DataTable
    Public Shared MenuTableOfMadoTk1Status As DataTable
    Public Shared MenuTableOfMadoTk2Status As DataTable

    Public Shared DateTimeFormatInGui As String = "yyyy/MM/dd HH:mm:ss"

    Public Shared UserContextDirName As String = "#9999"

    '�L�����O���
    Public Shared LogKindsMask As Integer

    '���v���Z�X�̃��b�Z�[�W�L���[�̃p�X
    Public Shared SelfMqPath As String

    '�������s�{�^���̑��M�惁�b�Z�[�W�L���[�̃p�X�i��ƃf�B���N�g�����ȑO�j
    Public Shared TargetMqPath As String

    '�f�[�^�x�[�X�d�l�ɂ�����@��
    Public Shared ModelSym As String

    '�f�[�^�x�[�X�d�l�ɂ�����[���@��
    Public Shared TermModelSym As String

    '�V�~�����[�^�N���f�B���N�g�����ɂ�����@��f�B���N�g���̑��΃p�X
    Public Shared ModelPathInSimWorkingDir As String

    '���O�\���̃��b�Z�[�W�񕝁i0�ȉ��̏ꍇ�̓E�B���h�E�ɍ��킹��j
    Public Shared LogDispMessageColumnWidth As Integer

    '��ʂɕێ����郍�O�̍ő僌�R�[�h��
    Public Shared LogDispMaxRowsCount As Integer

    '�N�����Ƀ��O�\���t�B���^�̗������N���A���邩�ۂ�
    Public Shared ClearLogDispFilterHisOnBoot As Boolean

    '���O�\���t�B���^�̍ő嗚��
    Public Shared LogDispFilterMaxHisCount As Integer

    '���O�\���t�B���^�̏�������
    Public Shared LogDispFilterInitialHis As DataTable

    '���[�U�R�[�h�R���{�{�b�N�X�̃A�C�e��
    Public Shared CompanyCodeItems As DataTable

    '�v���O�����K�p�ΏۃG���A�R���{�{�b�N�X�̃A�C�e��
    Public Shared IcAreaItems As DataTable

    '�v���O�����敪�R���{�{�b�N�X�̃A�C�e��
    Public Shared ProgramDistributionItems As DataTable

    'INI�t�@�C�����̃Z�N�V������
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const MQ_SECTION As String = "MessageQueue"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const UI_SECTION As String = "UserInterface"
    Protected Const LOG_DISP_FILTER_INITIAL_HIS_SECTION As String = "LogDispFilterInitialHis"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const SELF_MQ_PATH_KEY As String = "SelfMqPath"
    Private Const TARGET_MQ_PATH_KEY As String = "TargetMqPath"
    Private Const MODEL_SYM_KEY As String = "ModelSym"
    Private Const TERM_MODEL_SYM_KEY As String = "TermModelSym"
    Private Const MODEL_PATH_IN_SIM_WORKING_DIR_KEY As String = "ModelPathInSimWorkingDir"
    Private Const LOG_DISP_MESSAGE_COLUMN_WIDTH_KEY As String = "LogDispMessageColumnWidth"
    Private Const LOG_DISP_MAX_ROWS_COUNT_KEY As String = "LogDispMaxRowsCount"
    Private Const CLEAR_LOG_DISP_FILTER_HIS_ON_BOOT_COUNT_KEY As String = "ClearLogDispFilterHisOnBoot"
    Private Const LOG_DISP_FILTER_MAX_HIS_COUNT_KEY As String = "LogDispFilterMaxHisCount"

    ''' <summary>INI�t�@�C������w���@��V�~�����[�^�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        MachineProfileFieldNamesIndices = New Dictionary(Of String, Integer)()
        For i As Integer = 0 To MachineProfileFieldNames.Length - 1
            MachineProfileFieldNamesIndices(MachineProfileFieldNames(i)) = i
        Next i

        Try
            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(MQ_SECTION, SELF_MQ_PATH_KEY)
            SelfMqPath = LastReadValue

            ReadFileElem(MQ_SECTION, TARGET_MQ_PATH_KEY)
            TargetMqPath = LastReadValue

            ReadFileElem(DATABASE_SECTION, MODEL_SYM_KEY)
            ModelSym = LastReadValue

            ReadFileElem(DATABASE_SECTION, TERM_MODEL_SYM_KEY)
            TermModelSym = LastReadValue

            ReadFileElem(PATH_SECTION, MODEL_PATH_IN_SIM_WORKING_DIR_KEY)
            ModelPathInSimWorkingDir = LastReadValue

            ReadFileElem(UI_SECTION, LOG_DISP_MESSAGE_COLUMN_WIDTH_KEY)
            LogDispMessageColumnWidth = Integer.Parse(LastReadValue)

            ReadFileElem(UI_SECTION, LOG_DISP_MAX_ROWS_COUNT_KEY)
            LogDispMaxRowsCount = Integer.Parse(LastReadValue)

            ReadFileElem(UI_SECTION, CLEAR_LOG_DISP_FILTER_HIS_ON_BOOT_COUNT_KEY)
            ClearLogDispFilterHisOnBoot = Boolean.Parse(LastReadValue)

            ReadFileElem(UI_SECTION, LOG_DISP_FILTER_MAX_HIS_COUNT_KEY)
            LogDispFilterMaxHisCount = Integer.Parse(LastReadValue)

            LogDispFilterInitialHis = GetFileSectionAsDataTable(LOG_DISP_FILTER_INITIAL_HIS_SECTION)

            CompanyCodeItems = GetFileSectionAsDataTable("CompanyCodeItems")

            IcAreaItems = GetFileSectionAsDataTable("IcAreaItems")

            ProgramDistributionItems = GetFileSectionAsDataTable("ProgramDistributionItems")
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try

        MenuTableOfTktNegaStatus = New DataTable()
        MenuTableOfTktNegaStatus.Columns.Add("Value", GetType(String))
        MenuTableOfTktNegaStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfTktNegaStatus.Rows.Add("�ؒf", &H1)
        MenuTableOfTktNegaStatus.Rows.Add("�ڑ�", &H2)

        MenuTableOfTktMeisaiStatus = New DataTable()
        MenuTableOfTktMeisaiStatus.Columns.Add("Value", GetType(String))
        MenuTableOfTktMeisaiStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfTktMeisaiStatus.Rows.Add("�ؒf", &H1)
        MenuTableOfTktMeisaiStatus.Rows.Add("�ڑ�", &H2)

        MenuTableOfTktOnlineStatus = New DataTable()
        MenuTableOfTktOnlineStatus.Columns.Add("Value", GetType(String))
        MenuTableOfTktOnlineStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfTktOnlineStatus.Rows.Add("�ؒf", &H1)
        MenuTableOfTktOnlineStatus.Rows.Add("�ڑ�", &H2)

        MenuTableOfMadoDlsStatus = New DataTable()
        MenuTableOfMadoDlsStatus.Columns.Add("Value", GetType(String))
        MenuTableOfMadoDlsStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoDlsStatus.Rows.Add("���ݒ�", &H0)
        MenuTableOfMadoDlsStatus.Rows.Add("�ؒf", &H1)
        MenuTableOfMadoDlsStatus.Rows.Add("�ڑ�", &H2)

        MenuTableOfMadoKsbStatus = New DataTable()
        MenuTableOfMadoKsbStatus.Columns.Add("Value", GetType(String))
        MenuTableOfMadoKsbStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoKsbStatus.Rows.Add("���ݒ�", &H0)
        MenuTableOfMadoKsbStatus.Rows.Add("�ؒf", &H1)
        MenuTableOfMadoKsbStatus.Rows.Add("�ڑ�", &H2)

        MenuTableOfMadoTk1Status = New DataTable()
        MenuTableOfMadoTk1Status.Columns.Add("Value", GetType(String))
        MenuTableOfMadoTk1Status.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoTk1Status.Rows.Add("�ؒf", &H1)
        MenuTableOfMadoTk1Status.Rows.Add("�ڑ�", &H2)

        MenuTableOfMadoTk2Status = New DataTable()
        MenuTableOfMadoTk2Status.Columns.Add("Value", GetType(String))
        MenuTableOfMadoTk2Status.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoTk2Status.Rows.Add("�ؒf", &H1)
        MenuTableOfMadoTk2Status.Rows.Add("�ڑ�", &H2)
    End Sub

End Class
