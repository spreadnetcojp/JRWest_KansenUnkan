' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Messaging

Imports JR.ExOpmg.Common

Public Class ServerAppBaseConfig
    Inherits BaseConfig

    'プロセス識別文字列
    Public Shared AppIdentifier As String

    '常駐プロセスの識別文字列一覧
    Public Shared ResidentApps As String()

    '運管サーバを所有する事業者
    Public Shared SelfCompany As EkCompany

    '運管サーバが管轄するエリア
    Public Shared SelfArea As Integer

    '有効ログ種別
    Public Shared LogKindsMask As Integer

    '-------Ver0.1 次世代車補対応 ADD START-----------
    '利用データ用DB名
    Public Shared RiyoDataDatabaseName As String

    '新幹線指定券入場データ用DB名
    Public Shared ShiteiDataDatabaseName As String
    '-------Ver0.1 次世代車補対応 ADD END-------------

    '画面サイズ
    Public Shared FormWidth As Integer
    Public Shared FormHeight As Integer

    '画面表示位置
    Public Shared FormPosX As Integer
    Public Shared FormPosY As Integer

    '画面タイトル
    Public Shared FormTitle As String

    '管理系スレッドのポーリングの間隔
    Public Shared PollIntervalTicks As Integer

    '自己診断間隔
    Public Shared SelfDiagnosisIntervalTicks As Integer

    '生存証明ファイルの管理ディレクトリ名
    Public Shared ResidentAppPulseDirPath As String

    'マスタ/プログラムの管理ディレクトリ名
    Public Shared MasProDirPath As String

    '利用データの管理ディレクトリ名（駅別ディレクトリのベース）
    Public Shared RiyoDataDirPath As String

    '利用データの駅別ディレクトリ名の書式
    Public Shared RiyoDataStationBaseDirNameFormat As String

    '処理対象外利用データ格納ディレクトリ名
    Public Shared RiyoDataRejectDirPathInStationBase As String

    '受信直後の利用データ格納ディレクトリ名
    Public Shared RiyoDataInputDirPathInStationBase As String

    '-------Ver0.1 次世代車補対応 ADD START-----------
    '登録済み利用データ格納ディレクトリ名
    Public Shared RiyoDataOutputDirPathInStationBase As String
    '-------Ver0.1 次世代車補対応 ADD END-------------

    '集計中利用データ格納ディレクトリ名
    Public Shared RiyoDataTallyingDirPathInStationBase As String

    '集計済み利用データディレクトリの移動先ディレクトリ名
    Public Shared RiyoDataTrashDirPathInStationBase As String

    '各登録系プロセスの処理対象外データの格納ディレクトリ名
    Public Shared RejectDirPathForApps As Dictionary(Of String, String)

    '各登録系プロセスの処理対象データの格納ディレクトリ名
    Public Shared InputDirPathForApps As Dictionary(Of String, String)

    '各登録系プロセスの内部都合で登録できなかったデータの格納ディレクトリ名
    Public Shared SuspenseDirPathForApps As Dictionary(Of String, String)

    '各登録系プロセスの書式異常で登録できなかったデータの格納ディレクトリ名
    Public Shared QuarantineDirPathForApps As Dictionary(Of String, String)

    '各登録系プロセスの登録済みデータの格納ディレクトリ名
    Public Shared TrashDirPathForApps As Dictionary(Of String, String)

    '各登録系プロセスが担当するデータの枝番最大値
    Public Shared MaxBranchNumberForApps As Dictionary(Of String, Integer)

    '利用データの枝番最大値
    Public Shared RiyoDataMaxBranchNumber As Integer

    '各プロセスのメッセージキューのパス
    Public Shared MqPathForApps As Dictionary(Of String, String)

    '各プロセスのメッセージキュー（オプション）
    Public Shared MessageQueueForApps As Dictionary(Of String, MessageQueue)

    'INIファイル内のセクション名
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

    'INIファイル内における各設定項目のキー（登録系プロセス等のConfigでも使用可能）
    Protected Const BASE_DIR_PATH_KEY As String = "RecordingBaseDirPath"
    Protected Const REJECT_DIR_PATH_KEY As String = "RejectDirPathInRecordingBase"
    Protected Const INPUT_DIR_PATH_KEY As String = "InputDirPathInRecordingBase"
    Protected Const SUSPENSE_DIR_PATH_KEY As String = "SuspenseDirPathInRecordingBase"
    Protected Const QUARANTINE_DIR_PATH_KEY As String = "QuarantineDirPathInRecordingBase"
    Protected Const TRASH_DIR_PATH_KEY As String = "TrashDirPathInRecordingBase"
    Protected Const MAX_BRANCH_NUMBER_KEY As String = "MaxBranchNumber"
    Protected Const MQ_PATH_KEY As String = "MqPath"

    'INIファイル内における各設定項目のキー
    Private Const RESIDENT_APPS_KEY As String = "ResidentApps"
    Private Const SELF_COMPANY_KEY As String = "SelfCompany"
    Private Const SELF_AREA_KEY As String = "SelfArea"
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    '-------Ver0.1 次世代車補対応 ADD START-----------
    Private Const RIYO_DATA_DATABASE_NAME_KEY As String = "RiyoDataDatabaseName"
    Private Const SHITEI_DATA_DATABASE_NAME_KEY As String = "ShiteiDataDatabaseName"
    '-------Ver0.1 次世代車補対応 ADD END-------------
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
    '-------Ver0.1 次世代車補対応 ADD START-----------
    Private Const RIYO_DATA_OUTPUT_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataOutputDirPathInStationBase"
    '-------Ver0.1 次世代車補対応 ADD END-------------
    Private Const RIYO_DATA_TALLYING_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataTallyingDirPathInStationBase"
    Private Const RIYO_DATA_TRASH_DIR_PATH_IN_STATION_BASE_KEY As String = "RiyoDataTrashDirPathInStationBase"
    Private Const RIYO_DATA_MAX_BRANCH_NUMBER_KEY As String = "RiyoDataMaxBranchNumber"

    ''' <summary>INIファイルから運管サーバのどのプロセスにも必須の設定値を取り込む。</summary>
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

            '-------Ver0.1 次世代車補対応 ADD START-----------
            ReadFileElem(DATABASE_SECTION, RIYO_DATA_DATABASE_NAME_KEY)
            RiyoDataDatabaseName = LastReadValue

            ReadFileElem(DATABASE_SECTION, SHITEI_DATA_DATABASE_NAME_KEY)
            ShiteiDataDatabaseName = LastReadValue
            '-------Ver0.1 次世代車補対応 ADD END-------------

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

            '-------Ver0.1 次世代車補対応 ADD START-----------
            ReadFileElem(PATH_SECTION, RIYO_DATA_OUTPUT_DIR_PATH_IN_STATION_BASE_KEY)
            RiyoDataOutputDirPathInStationBase = LastReadValue
            '-------Ver0.1 次世代車補対応 ADD END-------------

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

            'NOTE: おそらくプロセスマネージャしか参照しない
            '（他はMessageQueueForAppsを参照する）が、
            'MqPath関連のキー名称に依存する箇所を局所化するために、
            'ここで作成している。
            MqPathForApps = New Dictionary(Of String, String)
            '-------Ver0.1 次世代車補対応 ADD START-----------
            CreateItemOfMqPath("AlertMailer")
            '-------Ver0.1 次世代車補対応 ADD END-------------
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
            '-------Ver0.1 次世代車補対応 ADD START-----------
            CreateItemOfMqPath("ForRiyoData")
            '-------Ver0.1 次世代車補対応 ADD END-------------

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

    'NOTE: ServerAppBaseInitのneedMessageQueueにTrueを渡した場合のみ、
    'プロセス終了時に呼び出す必要あり。
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
