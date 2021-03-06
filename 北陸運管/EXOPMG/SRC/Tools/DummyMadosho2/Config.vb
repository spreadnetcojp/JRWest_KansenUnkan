' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/06/27  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

'TODO: アプリの動作中に変更したいものは、ここから除去し、MainForm.vbの
'UiStateClassに宣言する。

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
       {"RAIL_SECTION_CODE", "線区"}, _
       {"STATION_ORDER_CODE", "駅順"}, _
       {"STATION_NAME", "駅名"}, _
       {"CORNER_CODE", "コーナー"}, _
       {"CORNER_NAME", "コーナー名"}, _
       {"UNIT_NO", "号機"}, _
       {"LAST_CONFIRMED", "取込日時"}}

    Public Shared ReadOnly Table1VisibleFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
       {"RAIL_SECTION_CODE", "000."}, _
       {"STATION_ORDER_CODE", "000."}, _
       {"STATION_NAME", "黒部宇奈月..."}, _
       {"CORNER_CODE", "00."}, _
       {"CORNER_NAME", "中央乗換出口..."}, _
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
       {"RAIL_SECTION_CODE", "線区"}, _
       {"STATION_ORDER_CODE", "駅順"}, _
       {"STATION_NAME", "駅名"}, _
       {"CORNER_CODE", "コーナー"}, _
       {"CORNER_NAME", "コーナー名"}, _
       {"UNIT_NO", "号機"}}

    Public Shared ReadOnly Table2VisibleFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
       {"RAIL_SECTION_CODE", "線区..."}, _
       {"STATION_ORDER_CODE", "駅順..."}, _
       {"STATION_NAME", "黒部宇奈月..."}, _
       {"CORNER_CODE", "コーナー.."}, _
       {"CORNER_NAME", "中央乗換出口..."}, _
       {"UNIT_NO", "号機..."}}

    'TODO: INIファイルからGetFileSectionAsDataTable()で読むようにする。名前もLatchConfItemsに変えるとよい。
    'KeyフィールドがString型になるが、DataGridViewCellのTagにXlsFieldをセットして、XlsDataGridViewに処理を任せる。
    Public Shared MenuTableOfLatchConf As DataTable

    Public Shared DateTimeFormatInGui As String = "yyyy/MM/dd HH:mm:ss"

    Public Shared UserContextDirName As String = "#9999"

    '有効ログ種別
    Public Shared LogKindsMask As Integer

    '自プロセスのメッセージキューのパス
    Public Shared SelfMqPath As String

    '強制実行ボタンの送信先メッセージキューのパス（作業ディレクトリ名以前）
    Public Shared TargetMqPath As String

    'データベース仕様における機種
    Public Shared ModelSym As String

    'データベース仕様における端末機種
    Public Shared TermModelSym As String

    'シミュレータ起動ディレクトリ内における機種ディレクトリの相対パス
    Public Shared ModelPathInSimWorkingDir As String

    'ログ表示のメッセージ列幅（0以下の場合はウィンドウに合わせる）
    Public Shared LogDispMessageColumnWidth As Integer

    '画面に保持するログの最大レコード数
    Public Shared LogDispMaxRowsCount As Integer

    '起動時にログ表示フィルタの履歴をクリアするか否か
    Public Shared ClearLogDispFilterHisOnBoot As Boolean

    'ログ表示フィルタの最大履歴数
    Public Shared LogDispFilterMaxHisCount As Integer

    'ログ表示フィルタの初期履歴
    Public Shared LogDispFilterInitialHis As DataTable

    '検索ボックスの最大履歴数
    Public Shared SearchBoxMaxHisCount As Integer

    '検索ボックスの初期履歴
    Public Shared SearchBoxInitialHis As DataTable

    '置換対象線区
    Public Shared ReplaceableRailSections As Dictionary(Of String, String)

    '駅コンボボックスのアイテム
    Public Shared StationItems As DataTable

    '通過方向コンボボックスのアイテム
    Public Shared PassDirectionItems As DataTable

    'ラッチ形態コンボボックスのアイテム
    Public Shared LatchConfItems As DataTable

    '大小区分コンボボックスのアイテム
    Public Shared AdultChildItems As DataTable

    '性別区分コンボボックスのアイテム
    Public Shared MaleFemaleItems As DataTable

    '利用ありなしコンボボックスのアイテム
    Public Shared IcUseUnuseItems As DataTable

    '大小ビットコンボボックスのアイテム
    Public Shared AdultChildFlagItems As DataTable

    '性別ビットコンボボックスのアイテム
    Public Shared MaleFemaleFlagItems As DataTable

    '通勤通学ビットコンボボックスのアイテム
    Public Shared CommutingFlagItems As DataTable

    '併算割引ビットコンボボックスのアイテム
    Public Shared CombinedDiscountFlagItems As DataTable

    '割引ビットコンボボックスのアイテム
    Public Shared DiscountFlagItems As DataTable

    '再発行ビットコンボボックスのアイテム
    Public Shared ReissueFlagItems As DataTable

    'テストビットコンボボックスのアイテム
    Public Shared TestFlagItems As DataTable

    '運改ビットコンボボックスのアイテム
    Public Shared FreightRateAmendFlagItems As DataTable

    '連絡ビットコンボボックスのアイテム
    Public Shared ConnectionFlagItems As DataTable

    '連続ビットコンボボックスのアイテム
    Public Shared ContinuumFlagItems As DataTable

    '当駅有効券ビットコンボボックスのアイテム
    Public Shared TicketValidityFlagItems As DataTable

    '回収放出ビットコンボボックスのアイテム
    Public Shared WithdrawFlagItems As DataTable

    '併用ビットコンボボックスのアイテム
    Public Shared CombineFlagItems As DataTable

    '座席種別コンボボックスのアイテム
    Public Shared SeatKindItems As DataTable

    '集計券種コンボボックスのアイテム
    Public Shared TicketKindItems As DataTable

    '割引コンボボックスのアイテム
    Public Shared DiscountKindItems As DataTable

    '有無コンボボックスのアイテム
    Public Shared AbsencePresenceItems As DataTable

    'INIファイル内のセクション名
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const MQ_SECTION As String = "MessageQueue"
    Protected Const PATH_SECTION As String = "Path"
    Protected Const UI_SECTION As String = "UserInterface"
    Protected Const LOG_DISP_FILTER_INITIAL_HIS_SECTION As String = "LogDispFilterInitialHis"

    'INIファイル内における各設定項目のキー
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

    ''' <summary>INIファイルから駅務機器シミュレータに必須の全設定値を取り込む。</summary>
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

            LatchConfItems = GetFileSectionAsDataTable("LatchConfItems")

            AdultChildItems = GetFileSectionAsDataTable("AdultChildItems")

            MaleFemaleItems = GetFileSectionAsDataTable("MaleFemaleItems")

            IcUseUnuseItems = GetFileSectionAsDataTable("IcUseUnuseItems")

            AdultChildFlagItems = GetFileSectionAsDataTable("AdultChildFlagItems")

            MaleFemaleFlagItems = GetFileSectionAsDataTable("MaleFemaleFlagItems")

            CommutingFlagItems = GetFileSectionAsDataTable("CommutingFlagItems")

            CombinedDiscountFlagItems = GetFileSectionAsDataTable("CombinedDiscountFlagItems")

            DiscountFlagItems = GetFileSectionAsDataTable("DiscountFlagItems")

            ReissueFlagItems = GetFileSectionAsDataTable("ReissueFlagItems")

            TestFlagItems = GetFileSectionAsDataTable("TestFlagItems")

            FreightRateAmendFlagItems = GetFileSectionAsDataTable("FreightRateAmendFlagItems")

            ConnectionFlagItems = GetFileSectionAsDataTable("ConnectionFlagItems")

            ContinuumFlagItems = GetFileSectionAsDataTable("ContinuumFlagItems")

            TicketValidityFlagItems = GetFileSectionAsDataTable("TicketValidityFlagItems")

            WithdrawFlagItems = GetFileSectionAsDataTable("WithdrawFlagItems")

            CombineFlagItems = GetFileSectionAsDataTable("CombineFlagItems")

            SeatKindItems = GetFileSectionAsDataTable("SeatKindItems")

            TicketKindItems = GetFileSectionAsDataTable("TicketKindItems")

            DiscountKindItems = GetFileSectionAsDataTable("DiscountKindItems")

            AbsencePresenceItems = GetFileSectionAsDataTable("AbsencePresenceItems")
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try

        MenuTableOfLatchConf = New DataTable()
        MenuTableOfLatchConf.Columns.Add("Value", GetType(String))
        MenuTableOfLatchConf.Columns.Add("Key", GetType(Byte))
        MenuTableOfLatchConf.Rows.Add("専用口", &H1)
        MenuTableOfLatchConf.Rows.Add("乗換口", &H2)
        MenuTableOfLatchConf.Rows.Add("精算所", &H3)
        MenuTableOfLatchConf.Rows.Add("ラッチ外出札所", &H4)
        MenuTableOfLatchConf.Rows.Add("乗換出札所", &H5)
    End Sub

End Class
