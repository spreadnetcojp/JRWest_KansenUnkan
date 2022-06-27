' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading
Imports System.Text

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' 機器接続状態登録プロセスのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "宣言領域（Private）"

    ''' <summary>
    ''' テーブル名
    ''' </summary>
    Private Const ConStatus_TableName As String = "D_CON_STATUS"

    ''' <summary>
    ''' 改札機データ種別
    ''' </summary>
    Private Const DataKind_G As String = "55"

    ''' <summary>
    ''' 窓口処理機データ種別
    ''' </summary>
    Private Const DataKind_Y As String = "89"

#End Region

#Region "Main"

    ''' <summary>
    ''' 機器接続状態登録プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 機器接続状態登録プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForConStatus")
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

                Log.Init(sLogBasePath, "ForConStatus")
                Log.Info("プロセス開始")

                Try
                    Lexis.Init(sIniFilePath)
                    Config.Init(sIniFilePath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End Try

                Log.SetKindsMask(Config.LogKindsMask)

                RecServerAppBaseMain(AddressOf RecordToDatabase)

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                Config.Dispose()
                Log.Info("プロセス終了")

                'NOTE: ここを通らなくても、このスレッドの消滅とともに解放される
                'ようなので、最悪の心配はない。
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub

#End Region

#Region "Private"

    ''' <summary>
    ''' 機器接続状態登録処理。
    ''' </summary>
    ''' <param name="sFilePath">登録するべきデータが格納されたファイルの絶対パス名</param>
    ''' <returns>登録の結果</returns>
    ''' <remarks>
    ''' データ登録スレッドで呼び出される。
    ''' </remarks>
    Private Shared Function RecordToDatabase(ByVal sFilePath As String) As RecordingResult

        Dim defineInfo() As RecDataStructure.DefineInfo = Nothing '定義情報
        Dim lstData As New List(Of String())                        'データ情報
        Dim lstDataNew As New List(Of String())                     '処理したデータ情報

        Dim dataKind(0) As Byte
        Dim fileNameInfo As RecDataStructure.BaseInfo = Nothing
        Dim code As EkCode

        Try
            'データ種別を取得
            Using fs As New FileStream(sFilePath, FileMode.Open)
                fs.Read(dataKind, 0, 1)
            End Using

            If Hex(dataKind(0)) = DataKind_G Then  '改札機接続状態監視
                fileNameInfo = Nothing
                'ファイル名を解析する
                code = UpboundDataPath.GetEkCode(sFilePath)
                'ファイル名から線区取得
                fileNameInfo.STATION_CODE.RAIL_SECTION_CODE = code.RailSection.ToString("D3")
                'ファイル名から駅順取得
                fileNameInfo.STATION_CODE.STATION_ORDER_CODE = code.StationOrder.ToString("D3")
                'ファイル名からコーナー取得
                fileNameInfo.CORNER_CODE = code.Corner.ToString("D4")
                'ファイル名から収集日時取得 
                fileNameInfo.PROCESSING_TIME = UpboundDataPath.GetTimestamp(sFilePath).ToString()

                'OPT: defineInfoを３つ用意して、GetDefineInfoはMainメソッドにて
                '一度だけ行う方がよいが、３種類の設定値があるという以前に
                'defineInfoがImmutableでないため、毎回つくりなおしているという
                '話もあり、対応するなら注意しなければならない。

                '定義情報を取得する。
                defineInfo = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath_G, "ConStatus", defineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If

                'データを取得する。
                lstData = New List(Of String())
                If MainClass.GetInfoFromDataFile(defineInfo, sFilePath, DataKind_G, lstData, fileNameInfo) = False Then
                    Return RecordingResult.ParseError
                End If

                'チェックを行う。
                lstDataNew = New List(Of String())
                If MainClass.CheckData(defineInfo, lstData, lstDataNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If

                'DB登録
                If BatchAppComm.PutDataToDBCommon(defineInfo, lstDataNew, ConStatus_TableName) = False Then
                    Return RecordingResult.IOError
                End If
            ElseIf Hex(dataKind(0)) = DataKind_Y Then     '窓口処理機接続状態監視
                fileNameInfo = Nothing
                'ファイル名から収集日時取得
                fileNameInfo.PROCESSING_TIME = UpboundDataPath.GetTimestamp(sFilePath).ToString()

                '定義情報を取得する。
                defineInfo = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath_Y, "ConStatus", defineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If
                'データを取得する。
                lstData = New List(Of String())
                If MainClass.GetInfoFromDataFile(defineInfo, sFilePath, DataKind_Y, lstData, fileNameInfo) = False Then
                    Return RecordingResult.ParseError
                End If

                'チェック
                lstDataNew = New List(Of String())
                If MainClass.CheckData(defineInfo, lstData, lstDataNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If

                'DB登録
                If BatchAppComm.PutDataToDBCommon(defineInfo, lstDataNew, ConStatus_TableName) = False Then
                    Return RecordingResult.IOError
                End If

                '機種がX場合
                '定義情報を取得する。
                defineInfo = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath_X, "ConStatus", defineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If
                'データを取得する。
                lstData = New List(Of String())
                If MainClass.GetInfoFromFileName(defineInfo, sFilePath, lstData) = False Then
                    Return RecordingResult.ParseError
                End If

                'チェック
                lstDataNew = New List(Of String())
                If MainClass.CheckData(defineInfo, lstData, lstDataNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If

                'DB登録
                If BatchAppComm.PutDataToDBCommon(defineInfo, lstDataNew, ConStatus_TableName) = False Then
                    Return RecordingResult.IOError
                End If
            Else
                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, 1, "データ種別"))
                '収集データの登録
                BatchAppComm.SetCollectionData(sFilePath, DataKind_G)
            End If

            '登録が成功した場合
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Hex(dataKind(0)), Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' 窓口処理機接続状態監視、機種がX場合、データの解析
    ''' </summary>
    ''' <param name="iniInfoAry">INIファイル内容</param>
    ''' <param name="fileName">データファイル名</param>
    ''' <param name="dlineInfoLst">データリスト</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>取得した電文フォーマット定義情報にて機器接続状態データを解析する</remarks>
    Private Shared Function GetInfoFromFileName(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByVal fileName As String, _
                                                ByRef dlineInfoLst As List(Of String())) As Boolean
        Dim bRtn As Boolean = False
        Dim nHeadSize_Y As Integer = 4
        Dim fs As FileStream = Nothing
        Dim bData() As Byte
        Dim j As Integer
        Dim dataInfo() As String
        Dim code As EkCode
        Dim sDataKind As String = "89" 'データ種別

        'ヘッド部
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        Try
            fs = New FileStream(fileName, FileMode.Open)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return bRtn
        End Try

        Try
            If dlineInfoLst Is Nothing Then
                dlineInfoLst = New List(Of String())
            Else
                dlineInfoLst.Clear()
            End If

            ReDim bData(nHeadSize_Y)

            'バイナリデータ取得
            fs.Read(bData, 0, nHeadSize_Y)

            ReDim dataInfo(iniInfoAry.Length - 1)

            headInfo.DATA_KIND = "89"
            '機種
            headInfo.MODEL_CODE = "X"

            code = UpboundDataPath.GetEkCode(fileName)
            For j = 0 To iniInfoAry.Length - 1
                'ヘッド場合
                Select Case UCase(iniInfoAry(j).FIELD_NAME)
                    Case "MODEL_CODE" '機種
                        dataInfo(j) = headInfo.MODEL_CODE
                    Case "RAIL_SECTION_CODE"    'サイバネ線区コード
                        dataInfo(j) = code.RailSection.ToString("D3")
                        headInfo.STATION_CODE.RAIL_SECTION_CODE = dataInfo(j)
                    Case "STATION_ORDER_CODE"   'サイバネ駅順コード
                        dataInfo(j) = code.StationOrder.ToString("D3")
                        headInfo.STATION_CODE.STATION_ORDER_CODE = dataInfo(j)
                    Case "CORNER_CODE"  'コーナーコード
                        dataInfo(j) = code.Corner.ToString("D4")
                        headInfo.CORNER_CODE = dataInfo(j)
                    Case "SYUSYU_DATE"  '収集日時
                        dataInfo(j) = UpboundDataPath.GetTimestamp(fileName).ToString
                    Case "IDCENTERCONNECT"  '明細
                        dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET).ToString
                    Case "UNIT_NO"
                        dataInfo(j) = code.Unit.ToString()
                        headInfo.UNIT_NO = CInt(dataInfo(j))
                End Select
            Next
            dlineInfoLst.Add(dataInfo)

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(sDataKind, Path.GetFileNameWithoutExtension(fileName)))

            Return bRtn
        Finally
            fs.Close()
        End Try

        Return bRtn
    End Function

    ''' <summary>
    ''' 機器接続状態データの解析
    ''' </summary>
    ''' <param name="iniInfoAry">INIファイル内容</param>
    ''' <param name="fileName">データファイル名</param>
    ''' <param name="sDataKind">データ種別</param>
    ''' <param name="dlineInfoLst">データリスト</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>取得した電文フォーマット定義情報にて機器接続状態データを解析する</remarks>
    Private Shared Function GetInfoFromDataFile(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByVal fileName As String, _
                                                ByVal sDataKind As String, _
                                                ByRef dlineInfoLst As List(Of String()), _
                                                ByVal fileNameInfo As RecDataStructure.BaseInfo) As Boolean
        Dim bRtn As Boolean = False
        Dim nHeadSize_Y As Integer = 4
        Dim nDataSize_Y As Integer = 15
        Dim nDataSize_G As Integer = 113
        Dim fs As FileStream = Nothing
        Dim bData() As Byte
        Dim i As Integer
        Dim j As Integer
        Dim dataInfo() As String
        Dim iDataCnt As Integer = 0
        Dim strCode As String = ""

        'ヘッド部
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        Try
            fs = New FileStream(fileName, FileMode.Open)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return bRtn
        End Try

        Try

            If dlineInfoLst Is Nothing Then
                dlineInfoLst = New List(Of String())
            Else
                dlineInfoLst.Clear()
            End If

            If sDataKind = DataKind_G Then     '改札機接続状態監視

                ReDim bData(nDataSize_G)
                'バイナリデータ取得
                fs.Read(bData, 0, nDataSize_G)

                iDataCnt = 16

                For i = 0 To iDataCnt - 1
                    ReDim dataInfo(iniInfoAry.Length - 1)
                    headInfo = Nothing
                    headInfo.STATION_CODE.RAIL_SECTION_CODE = fileNameInfo.STATION_CODE.RAIL_SECTION_CODE
                    headInfo.STATION_CODE.STATION_ORDER_CODE = fileNameInfo.STATION_CODE.STATION_ORDER_CODE
                    headInfo.CORNER_CODE = fileNameInfo.CORNER_CODE
                    headInfo.MODEL_CODE = "G"

                    For j = 0 To iniInfoAry.Length - 1
                        'ヘッド場合()
                        Select Case UCase(iniInfoAry(j).FIELD_NAME)
                            Case "DATA_KIND" 'データ種別
                                dataInfo(j) = Hex(bData(iniInfoAry(j).BYTE_OFFSET))
                                headInfo.DATA_KIND = dataInfo(j)
                            Case "MODEL_CODE" '機種
                                dataInfo(j) = headInfo.MODEL_CODE
                            Case "RAIL_SECTION_CODE"  'サイバネ線区コード
                                dataInfo(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE
                            Case "STATION_ORDER_CODE"  'サイバネ駅順コード
                                dataInfo(j) = headInfo.STATION_CODE.STATION_ORDER_CODE
                            Case "CORNER_CODE"  'コーナーコード
                                dataInfo(j) = headInfo.CORNER_CODE
                            Case "SYUSYU_DATE"  '収集日時
                                dataInfo(j) = fileNameInfo.PROCESSING_TIME
                            Case "UNIT_NO"
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i).ToString
                                headInfo.UNIT_NO = CInt(dataInfo(j))
                            Case Else
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i).ToString
                        End Select
                    Next

                    dlineInfoLst.Add(dataInfo)
                Next

            ElseIf sDataKind = DataKind_Y Then    '窓口処理機接続状態監視
                Dim isChkErr As Boolean = False

                '1レコードのデータ条数取得
                iDataCnt = CInt(Int((fs.Length - nHeadSize_Y) / nDataSize_Y))

                ReDim bData(nDataSize_Y * iDataCnt + nHeadSize_Y)

                'バイナリデータ取得
                fs.Read(bData, 0, nDataSize_Y * iDataCnt + nHeadSize_Y)

                For i = 0 To iDataCnt - 1
                    ReDim dataInfo(iniInfoAry.Length - 1)

                    isChkErr = False
                    headInfo = Nothing
                    '機種
                    headInfo.MODEL_CODE = "Y"

                    For j = 0 To iniInfoAry.Length - 1
                        'ヘッド場合
                        Select Case UCase(iniInfoAry(j).FIELD_NAME)
                            Case "DATA_KIND" 'データ種別
                                dataInfo(j) = Hex(bData(iniInfoAry(j).BYTE_OFFSET))
                                headInfo.DATA_KIND = dataInfo(j)
                                Continue For
                            Case "MODEL_CODE" '機種
                                dataInfo(j) = headInfo.MODEL_CODE
                                Continue For
                            Case "RAIL_SECTION_CODE"    'サイバネ線区コード
                                strCode = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y, _
                                                                                iniInfoAry(j).BYTE_LEN)
                                '数字チェックを行う
                                If OPMGUtility.checkNumber(strCode) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '収集データの登録
                                    BatchAppComm.SetCollectionData(fileName, DataKind_Y)
                                    isChkErr = True
                                    Exit For
                                End If
                                dataInfo(j) = Format(CInt(strCode), "000")
                                headInfo.STATION_CODE.RAIL_SECTION_CODE = dataInfo(j)
                                Continue For
                            Case "STATION_ORDER_CODE"   'サイバネ駅順コード
                                strCode = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y, _
                                                                                iniInfoAry(j).BYTE_LEN)
                                '数字チェックを行う
                                If OPMGUtility.checkNumber(strCode) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '収集データの登録
                                    BatchAppComm.SetCollectionData(fileName, DataKind_Y)
                                    isChkErr = True
                                    Exit For
                                End If
                                dataInfo(j) = Format(CInt(strCode), "000")
                                headInfo.STATION_CODE.STATION_ORDER_CODE = dataInfo(j)
                                Continue For
                            Case "CORNER_CODE"  'コーナーコード
                                strCode = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y, _
                                                                                iniInfoAry(j).BYTE_LEN)
                                '数字チェックを行う
                                If OPMGUtility.checkNumber(strCode) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '収集データの登録
                                    BatchAppComm.SetCollectionData(fileName, DataKind_Y)
                                    isChkErr = True
                                    Exit For
                                End If
                                dataInfo(j) = Format(CInt(strCode), "0000")
                                headInfo.CORNER_CODE = dataInfo(j)
                                Continue For
                            Case "SYUSYU_DATE"  '収集日時
                                dataInfo(j) = fileNameInfo.PROCESSING_TIME
                                Continue For
                            Case "IDCENTERCONNECT"  '明細
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET).ToString
                            Case "UNIT_NO"
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y).ToString
                                headInfo.UNIT_NO = CInt(dataInfo(j))
                            Case Else
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y).ToString
                        End Select
                    Next

                    If isChkErr = False Then
                        dlineInfoLst.Add(dataInfo)
                    End If
                Next
            End If

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(sDataKind, Path.GetFileNameWithoutExtension(fileName)))

            Return bRtn
        Finally
            fs.Close()
        End Try

        Return bRtn
    End Function

    ''' <summary>
    ''' 機器接続状態データのチェック
    ''' </summary>
    ''' <param name="iniInfoAry">iniファイル</param>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dlineInfoLstNew">datファイル内容</param>
    ''' <param name="datFileName">データファイル名</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>解析処理による取得データをチェックする</remarks>
    Private Shared Function CheckData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String) As Boolean
        Dim bRtn As Boolean = False
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:エラー;false:エラーない
        Dim dataKind As String = ""

        '機器構成マスタSQL
        Dim strSQL As String = "SELECT COUNT(1) FROM V_MACHINE_NOW WHERE RAIL_SECTION_CODE = {0} AND STATION_ORDER_CODE = {1} AND CORNER_CODE = {2} AND MODEL_CODE = {3} AND UNIT_NO = {4}"
        Dim dbCtl As DatabaseTalker = Nothing
        Dim sRail_Code As String = ""
        Dim sStation_Code As String = ""
        Dim sCorner_Code As String = ""
        Dim sModel_Code As String = ""
        Dim sUnit_No As String = ""
        Dim nRtn As Integer
        Dim nFlag_G As Integer = 16 '号機チェック用

        Try
            dlineInfoLstNew = New List(Of String())
            dbCtl = New DatabaseTalker
            dbCtl.ConnectOpen()

            For i = 0 To dlineInfoLst.Count - 1

                isHaveErr = False

                lineInfo = dlineInfoLst.Item(i)

                For j = 0 To iniInfoAry.Length - 1

                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "DATA_KIND"    'データ種別
                            dataKind = lineInfo(j)
                            Continue For
                        Case "RAIL_SECTION_CODE", _
                             "STATION_ORDER_CODE", _
                             "CORNER_CODE"         'サイバネ線区コード,サイバネ駅順コード,コーナーコード
                            If OPMGUtility.checkNumber(lineInfo(j)) = False Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                '収集データの登録
                                BatchAppComm.SetCollectionData(datFileName, dataKind)
                                If dataKind = DataKind_G Then
                                    Return bRtn
                                End If
                                Exit For
                            Else
                                If CLng(lineInfo(j)) = 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '収集データの登録
                                    BatchAppComm.SetCollectionData(datFileName, dataKind)
                                    If dataKind = DataKind_G Then
                                        Return bRtn
                                    End If
                                    Exit For
                                End If
                            End If
                            Select Case iniInfoAry(j).FIELD_NAME
                                Case "RAIL_SECTION_CODE"
                                    sRail_Code = lineInfo(j)

                                Case "STATION_ORDER_CODE"
                                    sStation_Code = lineInfo(j)

                                Case "CORNER_CODE"
                                    sCorner_Code = lineInfo(j)
                            End Select
                            Continue For
                        Case "UNIT_NO"  '号機番号
                            If OPMGUtility.checkNumber(lineInfo(j)) = False Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                '収集データの登録
                                BatchAppComm.SetCollectionData(datFileName, dataKind)
                                Exit For
                            Else
                                If CInt(lineInfo(j)) = 0 Then
                                    isHaveErr = True
                                    If dataKind = DataKind_G Then
                                        nFlag_G = nFlag_G - 1
                                        If nFlag_G <= 0 Then
                                            nFlag_G = 16
                                            Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, 1, iniInfoAry(j).KOMOKU_NAME))
                                            '収集データの登録
                                            BatchAppComm.SetCollectionData(datFileName, dataKind)
                                        End If
                                        Exit For
                                    End If
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '収集データの登録
                                    BatchAppComm.SetCollectionData(datFileName, dataKind)
                                    Exit For
                                End If
                            End If
                            sUnit_No = lineInfo(j)
                            Continue For
                        Case "SYUSYU_DATE"  '収集データ
                            If Not Date.TryParse(lineInfo(j), New Date) Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                '収集データの登録
                                BatchAppComm.SetCollectionData(datFileName, dataKind)
                                Return bRtn
                            End If
                            Continue For
                    End Select
                Next

                If isHaveErr = False Then
                    '機器構成マスタチェック
                    If dataKind = DataKind_G Then
                        sModel_Code = "G"
                    Else
                        sModel_Code = "Y"
                    End If
                    nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(String.Format(strSQL, Utility.SetSglQuot(sRail_Code), _
                                                   Utility.SetSglQuot(sStation_Code), _
                                                   Utility.SetSglQuot(sCorner_Code), _
                                                   Utility.SetSglQuot(sModel_Code), sUnit_No)))
                    '  監視盤のIPアドレスから対象の改札機を抽出し、コーナコードを取得
                    If (nRtn = 0) And (dataKind = DataKind_G) Then
                        Dim code As EkCode
                        'ファイル名を解析する
                        code = UpboundDataPath.GetEkCode(datFileName)
                        Dim sSQL As String = _
                                "SELECT CORNER_CODE FROM V_MACHINE_NOW" _
                                & "  WHERE RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
                                & "    AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
                                & "    AND MONITOR_ADDRESS = (" _
                                & "    SELECT ADDRESS FROM V_MACHINE_NOW" _
                                & "      WHERE RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
                                & "        AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
                                & "        AND CORNER_CODE = '" & code.Corner.ToString & "'" _
                                & "        AND MODEL_CODE = 'W'" _
                                & "        AND UNIT_NO = '" & code.Unit.ToString & "')" _
                                & "    AND UNIT_NO = '" & sUnit_No & "'" _
                                & "    AND MODEL_CODE = 'G'"
                        Dim oCorner As Object = dbCtl.ExecuteSQLToReadScalar(sSQL)
                        If oCorner IsNot Nothing Then
                            For j = 0 To iniInfoAry.Length - 1
                                If iniInfoAry(j).FIELD_NAME = "CORNER_CODE" Then
                                    lineInfo(j) = Format(CInt(oCorner), "0000")
                                End If
                            Next
                            nRtn = 1
                        End If
                    End If

                    If nRtn = 0 Then
                        Log.Error(String.Format(RecAppConstants.ERR_MACHINE_NOVALUE, sRail_Code, sStation_Code, sCorner_Code, sUnit_No))
                    End If

                    dlineInfoLstNew.Add(lineInfo)
                End If
            Next

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '収集データの登録
            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo)
            Return bRtn
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return bRtn

    End Function

#End Region
End Class
