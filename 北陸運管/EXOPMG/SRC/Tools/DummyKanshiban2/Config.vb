' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/04/28  (NES)����  �V�K�쐬
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
        "RAIL_SECTION_CODE", _
        "STATION_ORDER_CODE", _
        "STATION_NAME", _
        "CORNER_CODE", _
        "CORNER_NAME", _
        "UNIT_NO"}

    Public Shared ReadOnly Table2VisibleFieldNamesTitles As New Dictionary(Of String, String) From { _
       {"RAIL_SECTION_CODE", "����"}, _
       {"STATION_ORDER_CODE", "�w��"}, _
       {"STATION_NAME", "�w��"}, _
       {"CORNER_CODE", "�R�[�i�["}, _
       {"CORNER_NAME", "�R�[�i�[��"}, _
       {"UNIT_NO", "���@"}}

    Public Shared ReadOnly Table2VisibleFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
       {"RAIL_SECTION_CODE", "����..."}, _
       {"STATION_ORDER_CODE", "�w��..."}, _
       {"STATION_NAME", "�����F�ތ�..."}, _
       {"CORNER_CODE", "�R�[�i�[.."}, _
       {"CORNER_NAME", "�����抷�o��..."}, _
       {"UNIT_NO", "���@..."}}

    'TODO: INI�t�@�C������GetFileSectionAsDataTable()�œǂނ悤�ɂ���B���O��LatchConfItems�ɕς���Ƃ悢�B
    'Key�t�B�[���h��String�^�ɂȂ邪�ADataGridViewCell��Tag��XlsField���Z�b�g���āAXlsDataGridView�ɏ�����C����B
    Public Shared MenuTableOfLatchConf As DataTable

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

    '�����{�b�N�X�̍ő嗚��
    Public Shared SearchBoxMaxHisCount As Integer

    '�����{�b�N�X�̏�������
    Public Shared SearchBoxInitialHis As DataTable

    '�u���Ώې���
    Public Shared ReplaceableRailSections As Dictionary(Of String, String)

    '�w�R���{�{�b�N�X�̃A�C�e��
    Public Shared StationItems As DataTable

    '�ʘH�����R���{�{�b�N�X�̃A�C�e��
    Public Shared PassDirectionItems As DataTable

    '�召�敪�R���{�{�b�N�X�̃A�C�e��
    Public Shared AdultChildItems As DataTable

    '���ʋ敪�R���{�{�b�N�X�̃A�C�e��
    Public Shared MaleFemaleItems As DataTable

    '���Ȏ�ʃR���{�{�b�N�X�̃A�C�e��
    Public Shared SeatKindItems As DataTable

    '�W�v����R���{�{�b�N�X�̃A�C�e��
    Public Shared TicketKindItems As DataTable

    '�����R���{�{�b�N�X�̃A�C�e��
    Public Shared DiscountKindItems As DataTable

    '�L�������R���{�{�b�N�X�̃A�C�e��
    Public Shared TicketValidityItems As DataTable

    '�L���R���{�{�b�N�X�̃A�C�e��
    Public Shared AbsencePresenceItems As DataTable

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

            ReadFileElem(UI_SECTION, "SearchBoxMaxHisCount")
            SearchBoxMaxHisCount = Integer.Parse(LastReadValue)

            SearchBoxInitialHis = GetFileSectionAsDataTable("SearchBoxInitialHis")

            ReplaceableRailSections = GetFileSectionAsDictionary("ReplaceableRailSections")

            StationItems = GetFileSectionAsDataTable("StationItems")

            PassDirectionItems = GetFileSectionAsDataTable("PassDirectionItems")

            AdultChildItems = GetFileSectionAsDataTable("AdultChildItems")

            MaleFemaleItems = GetFileSectionAsDataTable("MaleFemaleItems")

            SeatKindItems = GetFileSectionAsDataTable("SeatKindItems")

            TicketKindItems = GetFileSectionAsDataTable("TicketKindItems")

            DiscountKindItems = GetFileSectionAsDataTable("DiscountKindItems")

            TicketValidityItems = GetFileSectionAsDataTable("TicketValidityItems")

            AbsencePresenceItems = GetFileSectionAsDataTable("AbsencePresenceItems")
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try

        MenuTableOfLatchConf = New DataTable()
        MenuTableOfLatchConf.Columns.Add("Value", GetType(String))
        MenuTableOfLatchConf.Columns.Add("Key", GetType(Byte))
        MenuTableOfLatchConf.Rows.Add("��p��", &H1)
        MenuTableOfLatchConf.Rows.Add("�抷��", &H2)
        'MenuTableOfLatchConf.Rows.Add("���Z��", &H3)
        'MenuTableOfLatchConf.Rows.Add("���b�`�O�o�D��", &H4)
        'MenuTableOfLatchConf.Rows.Add("�抷�o�D��", &H5)
    End Sub

End Class
