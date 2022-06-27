' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/08/08  (NES)小林  新規作成
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
    Public Shared ReadOnly UnknownHashValue As String = "(不明)"

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

    'NOTE: 1は駅名表示OFFで表示、2は駅名表示ONで表示、3は常に表示
    Public Shared ReadOnly Table1VisibleFieldNamesKinds As New Dictionary(Of String, Integer) From { _
       {"RAIL_SECTION_CODE", 1}, _
       {"STATION_ORDER_CODE", 1}, _
       {"STATION_NAME", 2}, _
       {"CORNER_CODE", 1}, _
       {"CORNER_NAME", 2}, _
       {"UNIT_NO", 3}, _
       {"LAST_CONFIRMED", 3}}

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
        "MODEL_CODE", _
        "MODEL_NAME", _
        "RAIL_SECTION_CODE", _
        "STATION_ORDER_CODE", _
        "STATION_NAME", _
        "CORNER_CODE", _
        "CORNER_NAME", _
        "UNIT_NO"}

    'NOTE: 1は駅名表示OFFで表示、2は駅名表示ONで表示、3は常に表示
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
       {"MODEL_CODE", "機種"}, _
       {"MODEL_NAME", "機種"}, _
       {"RAIL_SECTION_CODE", "線区"}, _
       {"STATION_ORDER_CODE", "駅順"}, _
       {"STATION_NAME", "駅名"}, _
       {"CORNER_CODE", "コーナー"}, _
       {"CORNER_NAME", "コーナー名"}, _
       {"UNIT_NO", "号機"}}

    Public Shared ReadOnly Table2VisibleFieldNamesCanonicalValues As New Dictionary(Of String, String) From { _
       {"MODEL_CODE", "機.."}, _
       {"MODEL_NAME", "統括..."}, _
       {"RAIL_SECTION_CODE", "000."}, _
       {"STATION_ORDER_CODE", "000."}, _
       {"STATION_NAME", "黒部宇奈月..."}, _
       {"CORNER_CODE", "00."}, _
       {"CORNER_NAME", "中央乗換出口..."}, _
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

    'ユーザコードコンボボックスのアイテム
    Public Shared CompanyCodeItems As DataTable

    'プログラム適用対象エリアコンボボックスのアイテム
    Public Shared IcAreaItems As DataTable

    'プログラム区分コンボボックスのアイテム
    Public Shared ProgramDistributionItems As DataTable

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
        MenuTableOfTktNegaStatus.Rows.Add("切断", &H1)
        MenuTableOfTktNegaStatus.Rows.Add("接続", &H2)

        MenuTableOfTktMeisaiStatus = New DataTable()
        MenuTableOfTktMeisaiStatus.Columns.Add("Value", GetType(String))
        MenuTableOfTktMeisaiStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfTktMeisaiStatus.Rows.Add("切断", &H1)
        MenuTableOfTktMeisaiStatus.Rows.Add("接続", &H2)

        MenuTableOfTktOnlineStatus = New DataTable()
        MenuTableOfTktOnlineStatus.Columns.Add("Value", GetType(String))
        MenuTableOfTktOnlineStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfTktOnlineStatus.Rows.Add("切断", &H1)
        MenuTableOfTktOnlineStatus.Rows.Add("接続", &H2)

        MenuTableOfMadoDlsStatus = New DataTable()
        MenuTableOfMadoDlsStatus.Columns.Add("Value", GetType(String))
        MenuTableOfMadoDlsStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoDlsStatus.Rows.Add("未設定", &H0)
        MenuTableOfMadoDlsStatus.Rows.Add("切断", &H1)
        MenuTableOfMadoDlsStatus.Rows.Add("接続", &H2)

        MenuTableOfMadoKsbStatus = New DataTable()
        MenuTableOfMadoKsbStatus.Columns.Add("Value", GetType(String))
        MenuTableOfMadoKsbStatus.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoKsbStatus.Rows.Add("未設定", &H0)
        MenuTableOfMadoKsbStatus.Rows.Add("切断", &H1)
        MenuTableOfMadoKsbStatus.Rows.Add("接続", &H2)

        MenuTableOfMadoTk1Status = New DataTable()
        MenuTableOfMadoTk1Status.Columns.Add("Value", GetType(String))
        MenuTableOfMadoTk1Status.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoTk1Status.Rows.Add("切断", &H1)
        MenuTableOfMadoTk1Status.Rows.Add("接続", &H2)

        MenuTableOfMadoTk2Status = New DataTable()
        MenuTableOfMadoTk2Status.Columns.Add("Value", GetType(String))
        MenuTableOfMadoTk2Status.Columns.Add("Key", GetType(Byte))
        MenuTableOfMadoTk2Status.Rows.Add("切断", &H1)
        MenuTableOfMadoTk2Status.Rows.Add("接続", &H2)
    End Sub

End Class
