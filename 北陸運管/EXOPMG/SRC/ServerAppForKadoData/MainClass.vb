' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/06/01  　　金沢   北陸対応
'   0.2      2015/05/25  　　金沢   稼動保守データ欠落対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading
Imports System.Text
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' 本プロセスは、収集した稼動・保守データを解析し、運用管理サーバのDBに登録する。
''' </summary>
''' <remarks></remarks>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "宣言領域（Private）"

    ''' <summary>
    ''' 稼動テーブル名
    ''' </summary>
    Private Const Kadou_TableName As String = "D_KADO_DATA"

    ''' <summary>
    ''' 保守テーブル名
    ''' </summary>
    Private Const Hosyu_TableName As String = "D_HOSYU_DATA"

    ''' <summary>
    ''' 改札機データ種別
    ''' </summary>
    Private Const DataKind_G As String = "A7"

    ''' <summary>
    ''' 窓口処理機データ種別
    ''' </summary>
    Private Const DataKind_Y As String = "B7"

    ''' <summary>
    ''' 稼動データ種別
    ''' </summary>
    Private Const Kado_DataKind As String = "A7"

    ''' <summary>
    ''' 保守データ種別
    ''' </summary>
    Private Const Hosyu_DataKind As String = "A8"
    '-------Ver0.1　北陸対応　ADD START-----------
    ''' <summary>
    ''' グループ番号
    ''' </summary>
    Private Shared GrpNo As Integer = 0
    '-------Ver0.1　北陸対応　ADD END-----------

#End Region

#Region "Main"

    ''' <summary>
    ''' 稼動・保守データプロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 稼動・保守データプロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForKadoData")
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

                Log.Init(sLogBasePath, "ForKadoData")
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

#Region "メソッド（Private）"

    ''' <summary>
    '''  稼動・保守データ
    ''' </summary>
    ''' <param name="sFilePath">登録するべきデータが格納されたファイルの絶対パス名</param>
    ''' <returns>登録の結果</returns>
    ''' <remarks>
    ''' データ登録スレッドで呼び出される。
    ''' </remarks>
    Private Shared Function RecordToDatabase(ByVal sFilePath As String) As RecordingResult
        Try
            Dim modelCode As Integer = UpboundDataPath.GetEkCode(sFilePath).Model '機種コード

            Dim sModelCode As String = Format(modelCode, "00")
            Dim kadoDefineInfo() As RecDataStructure.DefineInfo = Nothing  '定義情報
            Dim hosyuDefineInfo() As RecDataStructure.DefineInfo = Nothing '定義情報
            Dim lstDataNew As New List(Of String())                        '処理したデータ情報

            Dim lstKadoData As New List(Of String())                        'データ情報
            Dim lstHosyuData As New List(Of String())                       'データ情報
            Dim dataKind(0) As Byte
            '-------Ver0.1　北陸対応　ADD START-----------
            'ファイル名から線区駅順コード取得
            Dim ekiCode As String = UpboundDataPath.GetEkCode(sFilePath).RailSection.ToString("D3") _
                                    & UpboundDataPath.GetEkCode(sFilePath).StationOrder.ToString("D3")
            '線区駅順コードを条件にグループ番号取得
            If GetGroupNo(ekiCode) = False Then
                Return RecordingResult.IOError
            End If
            '-------Ver0.1　北陸対応　ADD END-----------
            'データ種別を取得
            Using fs As New FileStream(sFilePath, FileMode.Open)
                fs.Read(dataKind, 0, 1)
            End Using

            'OPT: 以下、GetDefineInfoはMainメソッドにて一度だけ行う方がよいが、
            'xxxDefineInfoがImmutableでないため、毎回つくりなおしているという
            '話もあり、対応するなら注意しなければならない。

            If Hex(dataKind(0)) = DataKind_G Then  '改札機
                '-------Ver0.1　北陸対応　MOD START-----------
                '稼動定義情報を取得する。
                If DefineInfoShutoku.GetDefineInfo(Config.KadoFormatFileG(GrpNo).ToString, "KADO", kadoDefineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If

                '保守定義情報を取得する。
                If DefineInfoShutoku.GetDefineInfo(Config.HosyuFormatFile(GrpNo).ToString, "HOSYU", hosyuDefineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If
                '-------Ver0.1　北陸対応　MOD END-----------
                '-----------------------------稼動処理 Start-----------------------------
                'DATファイルデータ取得
                If GetInfoFromDataFile(kadoDefineInfo, sFilePath, sModelCode, Kado_DataKind, lstKadoData, True) = False Then
                    Return RecordingResult.ParseError
                End If

                'チェック
                If CheckData(kadoDefineInfo, lstKadoData, lstDataNew, sFilePath, Kado_DataKind) = False Then
                    Return RecordingResult.IOError
                End If

                'DB登録
                If BatchAppComm.PutDataToDBCommon(kadoDefineInfo, lstDataNew, Kadou_TableName) = False Then
                    Return RecordingResult.IOError
                End If
                '-----------------------------稼動処理 End  -----------------------------

                '-----------------------------保守処理 Start-----------------------------
                'DATファイルデータ取得
                If GetInfoFromDataFile(hosyuDefineInfo, sFilePath, sModelCode, Hosyu_DataKind, lstHosyuData) = False Then
                    Return RecordingResult.ParseError
                End If

                'チェック
                lstDataNew = New List(Of String())
                If CheckData(hosyuDefineInfo, lstHosyuData, lstDataNew, sFilePath, Hosyu_DataKind) = False Then
                    Return RecordingResult.IOError
                End If

                '保守データを再加工
                lstHosyuData = New List(Of String())
                If GetDBInfoFromDataInfo(hosyuDefineInfo, sFilePath, sModelCode, lstDataNew, lstHosyuData) = False Then
                    Return RecordingResult.ParseError
                End If

                'DB登録
                If BatchAppComm.PutDataToDBCommon(hosyuDefineInfo, lstHosyuData, Hosyu_TableName) = False Then
                    Return RecordingResult.IOError
                End If
                '-----------------------------保守処理 End  -----------------------------
            ElseIf Hex(dataKind(0)) = DataKind_Y Then     '窓口処理機
                '稼動定義情報を取得する。
                If DefineInfoShutoku.GetDefineInfo(Config.KadoFormatFilePath_Y, "KADO", kadoDefineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If

                '-----------------------------稼動処理 Start-----------------------------
                'DATファイルデータ取得
                If GetInfoFromDataFile(kadoDefineInfo, sFilePath, sModelCode, Kado_DataKind, lstKadoData, True) = False Then
                    Return RecordingResult.ParseError
                End If

                'チェック
                If CheckData(kadoDefineInfo, lstKadoData, lstDataNew, sFilePath, Kado_DataKind) = False Then
                    Return RecordingResult.IOError
                End If

                'DB登録
                If BatchAppComm.PutDataToDBCommon(kadoDefineInfo, lstDataNew, Kadou_TableName) = False Then
                    Return RecordingResult.IOError
                End If
                '-----------------------------稼動処理 End  -----------------------------
            End If

            '成功した場合
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Kado_DataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' 稼動・保守データの解析
    ''' </summary>
    ''' <param name="defineInfo">INIファイル内容</param>
    ''' <param name="sFilePath">データファイル名</param>
    ''' <param name="sModelCode">機種コード</param>
    ''' <param name="sDataKind">データ種別</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>取得した電文フォーマット定義情報にて稼動・保守データを解析する</remarks>
    Private Shared Function GetInfoFromDataFile(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                                ByVal sFilePath As String, _
                                                ByVal sModelCode As String, _
                                                ByVal sDataKind As String, _
                                                ByRef lstData As List(Of String()), _
                                                Optional ByVal isCheckDataKind As Boolean = False) As Boolean
        Dim nHeadSize As Integer = 17
        Dim nDataSize As Integer = 2188

        Dim fileStream As FileStream = Nothing
        Dim iStarRecIndex As Integer = 0 '開始レコードindex
        'レコード数
        Dim iRecCnt As Integer = 0
        'データ部
        Dim bData() As Byte
        'ヘッド部
        Dim headInfo As RecDataStructure.BaseInfo = Nothing
        '１レコード
        Dim sArrRecord() As String

        lstData = New List(Of String())

        Try
            'ファイルストリームを取得
            fileStream = New FileStream(sFilePath, FileMode.Open)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            'レコードサイズチェック
            If fileStream.Length < (nDataSize + nHeadSize) Then
                Log.Error(RecAppConstants.ERR_TOO_SHORT_FILE)
                Return False
            End If

            '送信方式
            If fileStream.Length > (nDataSize + nHeadSize) Then 'ftpの場合
                iStarRecIndex = 1
            Else 'socketの場合
                iStarRecIndex = 0
            End If

            'レコード数取得
            If fileStream.Length Mod (nDataSize + nHeadSize) = 0 Then
                iRecCnt = CInt(fileStream.Length / (nDataSize + nHeadSize))
            Else
                iRecCnt = CInt(Int(fileStream.Length / (nDataSize + nHeadSize)))
            End If

            'レコード毎処理
            For i As Integer = iStarRecIndex To iRecCnt - 1

                ReDim bData(nDataSize + nHeadSize) '1レコードのデータ

                'ファイル内レコード位置
                fileStream.Seek(i * (nDataSize + nHeadSize), SeekOrigin.Begin)
                fileStream.Read(bData, 0, nDataSize + nHeadSize)

                headInfo = Nothing
                BinaryHeadInfoParse.GetBaseInfo(bData, sModelCode, headInfo)

                'データ種別のチェックを行う
                If isCheckDataKind = True _
                   AndAlso headInfo.DATA_KIND <> Kado_DataKind _
                   AndAlso headInfo.DATA_KIND <> Hosyu_DataKind Then
                    'データ種別の不正ログを出力する
                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, "データ種別"))
                    '収集データの登録
                    BatchAppComm.SetCollectionData(sFilePath, headInfo.DATA_KIND)
                    Continue For
                End If

                '解析データを取得する
                If headInfo.DATA_KIND = sDataKind Then
                    ReDim sArrRecord(defineInfo.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo, bData, headInfo, sArrRecord) = False Then
                        '収集データの登録
                        BatchAppComm.SetCollectionData(headInfo, sDataKind)
                        Continue For
                    End If

                    '電文での行目を設定する
                    sArrRecord(defineInfo.Length - 1) = CStr(i + 1)

                    '解析したデータを設定する
                    lstData.Add(sArrRecord)
                End If
            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: 稼動・保守データに関する解析失敗のファイル種別は、常に稼動データの種別とする。
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Kado_DataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return False
        Finally
            'ファイルストリームを解放
            fileStream.Close()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' 稼動・保守データのチェック
    ''' </summary>
    ''' <param name="iniInfoAry">iniファイル</param>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dlineInfoLstNew">datファイル内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>解析処理による取得データをチェックする</remarks>
    Private Shared Function CheckData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String, _
                                      ByVal sDataKind As String) As Boolean
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:エラー;false:エラーない
        Dim strDate As String
        Dim iLineNo As Integer

        dlineInfoLstNew = New List(Of String())

        For i = 0 To dlineInfoLst.Count - 1

            isHaveErr = False

            lineInfo = dlineInfoLst.Item(i)

            '電文での行目を取得する
            If OPMGUtility.checkNumber(lineInfo(lineInfo.Length - 1)) Then
                iLineNo = CInt(lineInfo(lineInfo.Length - 1))
            Else
                iLineNo = i + 1
            End If

            '共通のチェックを行う
            If BatchAppComm.CheckDataComm(iLineNo, iniInfoAry, lineInfo, datFileName) = False Then
                Continue For
            End If

            '特別なチェック
            If sDataKind = Kado_DataKind Then
                For j = 0 To iniInfoAry.Length - 1
                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "KAI_INSPECT_TIME", "SYU_INSPECT_TIME"     '改札側点検日時  集札側点検日時
                            If lineInfo(j).Substring(0, 14) <> "00000000000000" Then
                                strDate = lineInfo(j).Substring(0, 4) & "/" & _
                                     lineInfo(j).Substring(4, 2) & "/" & _
                                     lineInfo(j).Substring(6, 2) & " " & _
                                     lineInfo(j).Substring(8, 2) & ":" & _
                                     lineInfo(j).Substring(10, 2) & ":" & _
                                     lineInfo(j).Substring(12, 2)

                                If Not Date.TryParse(strDate, New Date) Then
                                    lineInfo(j) = "00000000000000"
                                End If
                            End If
                    End Select
                Next
            End If

            If isHaveErr = False Then
                dlineInfoLstNew.Add(lineInfo)
            End If
        Next

        Return True

    End Function

    ''' <summary>
    ''' 取得したデータをDB登録データに再加工する
    ''' </summary>
    ''' <param name="hosyuDefineInfo">定義情報</param>
    ''' <param name="sFilePath">ファイルパース</param>
    ''' <param name="sModelCode">機種コード</param>
    ''' <param name="lstGetData">データ情報</param>
    ''' <param name="lstHosyuData">再加工データ</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>チェックした保守データを再加工する</remarks>
    Private Shared Function GetDBInfoFromDataInfo(ByVal hosyuDefineInfo() As RecDataStructure.DefineInfo, _
                                                  ByVal sFilePath As String, _
                                                  ByVal sModelCode As String, _
                                                  ByVal lstGetData As List(Of String()), _
                                                  ByRef lstHosyuData As List(Of String())) As Boolean
        '主キー情報
        Dim sRAIL_SECTION_CODE As String = ""
        Dim sSTATION_ORDER_CODE As String = ""
        Dim sCORNER_CODE As String = ""
        Dim sMODEL_CODE As String = ""
        Dim sUNIT_NO As String = ""
        Dim sPROCESSING_TIME As String = ""
        Dim sCOLLECT_START_TIME As String = ""
        Dim sCOLLECT_END_TIME As String = ""

        Dim nKeyFlag As Integer
        '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
        Dim tKeyFlg As Boolean
        '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
        Dim sArrInfo As String()
        Dim kadouDefineInfo() As RecDataStructure.DefineInfo = Nothing
        Dim lstKadouNewData As New List(Of String())
        Dim lstKadouALLData As New List(Of String())
        lstHosyuData = New List(Of String())
        '-------Ver0.1　北陸対応　ADD START-----------
        'ファイル名から線区駅順コード取得
        Dim ekiCode As String = UpboundDataPath.GetEkCode(sFilePath).RailSection.ToString("D3") _
                                & UpboundDataPath.GetEkCode(sFilePath).StationOrder.ToString("D3")
        '線区駅順コードを条件にグループ番号取得
        If GetGroupNo(ekiCode) = False Then
            Return False
        End If
        '-------Ver0.1　北陸対応　ADD END-----------
        '-------Ver0.1　北陸対応　MOD START-----------
        '稼動定義情報を取得する。
        If DefineInfoShutoku.GetDefineInfo(Config.KadoFormatFileG(GrpNo).ToString, "KADO_002", kadouDefineInfo) = False Then
            AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
            Return False
        End If
        '-------Ver0.1　北陸対応　MOD END-----------
        'DATファイルデータ取得
        If GetInfoFromDataFile(kadouDefineInfo, sFilePath, sModelCode, Kado_DataKind, lstKadouALLData) = False Then
            Return False
        End If

        'チェック
        If CheckDataNoMsg(kadouDefineInfo, lstKadouALLData, lstKadouNewData, sFilePath, Kado_DataKind) = False Then
            Return False
        End If

        '稼動と保守データのペアチェックを行う
        Dim lstChkKadoData As New List(Of String())
        Dim lstChkHosyuData As New List(Of String())
        For i As Integer = 0 To lstKadouNewData.Count - 1
            lstChkKadoData.Add(lstKadouNewData(i))
        Next
        Call CheckPair(lstChkKadoData, lstGetData, hosyuDefineInfo, kadouDefineInfo, lstChkHosyuData)

        If lstChkHosyuData.Count <= 0 Then
            Return True
        End If

        '保守データを再加工する
        For iHosyu As Integer = 0 To lstChkHosyuData.Count - 1
            sArrInfo = lstChkHosyuData(iHosyu)
            nKeyFlag = 6
            '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
            '処理日時判定フラグを初期化
            tKeyFlg = False
            '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
            '保守定義情報により保守の主キー値を取得する
            For j As Integer = 0 To hosyuDefineInfo.Length - 1
                Select Case hosyuDefineInfo(j).FIELD_NAME
                    Case "RAIL_SECTION_CODE"
                        sRAIL_SECTION_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "STATION_ORDER_CODE"
                        sSTATION_ORDER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "CORNER_CODE"
                        sCORNER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "MODEL_CODE"
                        sMODEL_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "UNIT_NO"
                        sUNIT_NO = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "PROCESSING_TIME"
                        sPROCESSING_TIME = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                End Select
                If nKeyFlag = 0 Then
                    Exit For
                End If
            Next

            '稼動データ
            For iKadou As Integer = 0 To lstKadouNewData.Count - 1
                nKeyFlag = 6
                Dim nKadouFlag As Integer = 6
                '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
                '処理日時判定フラグを初期化
                tKeyFlg = False
                '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
                '稼動定義情報により保守の主キー値と稼動の主キー値は一致するかどうかを判断する
                For j As Integer = 0 To kadouDefineInfo.Length - 1
                    Select Case kadouDefineInfo(j).FIELD_NAME
                        Case "RAIL_SECTION_CODE"
                            If sRAIL_SECTION_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1
                        Case "STATION_ORDER_CODE"
                            If sSTATION_ORDER_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "CORNER_CODE"
                            If sCORNER_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "MODEL_CODE"
                            If sMODEL_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "UNIT_NO"
                            If sUNIT_NO.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "PROCESSING_TIME"
                            '-----Ver0.2　稼動保守データ欠落対応　MOD　START----------------------
                            '処理日時の年月日のみ比較する
                            If sPROCESSING_TIME.Substring(0, 8).Equals(lstKadouNewData(iKadou)(j).Substring(0, 8)) Then
                                nKeyFlag = nKeyFlag - 1
                            Else
                                '処理日時判定フラグをセット
                                tKeyFlg = True
                            End If
                            '-----Ver0.2　稼動保守データ欠落対応　MOD　END----------------------
                            nKadouFlag = nKadouFlag - 1

                    End Select
                    If nKadouFlag = 0 Then
                        Exit For
                    End If
                Next

                ''該当稼動データに主キーと該当保守データの主キーは一致
                '-----Ver0.2　稼動保守データ欠落対応　MOD　START----------------------
                If nKeyFlag = 0 Or (nKeyFlag = 1 And tKeyFlg = True) Then
                    'Dim nSetFlag As Integer = 301 '384
                    For j As Integer = 0 To hosyuDefineInfo.Length - 1
                        If (hosyuDefineInfo(j).COMMENT.Contains("COLLECT_START_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("COLLECT_END_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("KAI_INSPECT_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("SYU_INSPECT_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("KAI_SERIAL_NO")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("SYU_SERIAL_NO")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("ITEM")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("KAI_SENSOR_")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("SYU_SENSOR_")) Then
                            '保守データの項目値に稼動データの対応項目値を設定する
                            For n As Integer = 0 To kadouDefineInfo.Length - 1
                                If kadouDefineInfo(n).FIELD_NAME = hosyuDefineInfo(j).COMMENT Then
                                    sArrInfo(j) = lstKadouNewData(iKadou)(n)
                                    'nSetFlag = nSetFlag - 1
                                    Exit For
                                End If
                            Next
                            'If nSetFlag = 0 Then
                            '    lstHosyuData.Add(sArrInfo)
                            '    Exit For
                            'End If
                        End If
                    Next
                    lstHosyuData.Add(sArrInfo)
                    Exit For
                End If
                '-----Ver0.2　稼動保守データ欠落対応　MOD　START----------------------
            Next
        Next


        Return True
    End Function

    ''' <summary>
    ''' 稼動と保守データのペアチェックを行う
    ''' </summary>
    ''' <param name="lstKadoData">稼動データ</param>
    ''' <param name="lstHosyuData">保守データ</param>
    ''' <param name="hosyuDefineInfo">保守定義情報</param>
    ''' <param name="kadoDefineInfo">稼動定義情報</param>
    ''' <param name="lstRtnHosyuData">保守データ</param>
    ''' <returns>True：処理OK False：チェックNG</returns>
    ''' <remarks>ペアセットとなってない場合、収集データ登録を行う</remarks>
    Private Shared Function CheckPair(ByVal lstKadoData As List(Of String()), _
                                      ByVal lstHosyuData As List(Of String()), _
                                      ByVal hosyuDefineInfo() As RecDataStructure.DefineInfo, _
                                      ByVal kadoDefineInfo() As RecDataStructure.DefineInfo, _
                                      ByRef lstRtnHosyuData As List(Of String())) As Boolean
        Dim nKeyFlag As Integer
        Dim sArrInfo As String()
        '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
        Dim tKeyFlg As Boolean  '処理日時判定フラグ
        '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
        '主キー情報
        Dim sRAIL_SECTION_CODE As String = ""
        Dim sSTATION_ORDER_CODE As String = ""
        Dim sCORNER_CODE As String = ""
        Dim sMODEL_CODE As String = ""
        Dim sUNIT_NO As String = ""
        Dim sPROCESSING_TIME As String = ""
        Dim sCOLLECT_START_TIME As String = ""
        Dim sCOLLECT_END_TIME As String = ""

        If lstHosyuData.Count <= 0 Then
            '保守データが無い場合、稼動データをもって収集データ登録を行う
            Call InsertCollectionDataPair(kadoDefineInfo, lstKadoData)
            Return True
        End If

        If lstKadoData.Count <= 0 Then
            '稼動データが無い場合、保守データをもって収集データ登録を行う
            Call InsertCollectionDataPair(hosyuDefineInfo, lstHosyuData)
            Return True
        End If

        If lstKadoData.Count > lstHosyuData.Count Then
            For iHosyu As Integer = lstHosyuData.Count - 1 To 0 Step -1
                sArrInfo = lstHosyuData(iHosyu)
                nKeyFlag = 6
                '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
                '処理日時判定フラグを初期化
                tKeyFlg = False
                '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
                '保守定義情報により保守の主キー値を取得する
                For j As Integer = 0 To hosyuDefineInfo.Length - 1
                    Select Case hosyuDefineInfo(j).FIELD_NAME
                        Case "RAIL_SECTION_CODE"
                            sRAIL_SECTION_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "STATION_ORDER_CODE"
                            sSTATION_ORDER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "CORNER_CODE"
                            sCORNER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "MODEL_CODE"
                            sMODEL_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "UNIT_NO"
                            sUNIT_NO = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "PROCESSING_TIME"
                            sPROCESSING_TIME = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                    End Select
                    If nKeyFlag = 0 Then
                        Exit For
                    End If
                Next

                '稼動データ
                For iKadou As Integer = lstKadoData.Count - 1 To 0 Step -1
                    nKeyFlag = 6
                    '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
                    '処理日時判定フラグを初期化
                    tKeyFlg = False
                    '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
                    Dim nKadouFlag As Integer = 6
                    '稼動定義情報により保守の主キー値と稼動の主キー値は一致するかどうかを判断する
                    For j As Integer = 0 To kadoDefineInfo.Length - 1
                        Select Case kadoDefineInfo(j).FIELD_NAME
                            Case "RAIL_SECTION_CODE"
                                If sRAIL_SECTION_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1
                            Case "STATION_ORDER_CODE"
                                If sSTATION_ORDER_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "CORNER_CODE"
                                If sCORNER_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "MODEL_CODE"
                                If sMODEL_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "UNIT_NO"
                                If sUNIT_NO.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "PROCESSING_TIME"
                                '-----Ver0.2　稼動保守データ欠落対応　MOD　START----------------------
                                '処理日時の年月日のみ比較する
                                If sPROCESSING_TIME.Substring(0, 8).Equals(lstKadoData(iKadou)(j).Substring(0, 8)) Then
                                    nKeyFlag = nKeyFlag - 1
                                Else
                                    '処理日時判定フラグをTRUE
                                    tKeyFlg = True
                                End If
                                '-----Ver0.2　稼動保守データ欠落対応　MOD　END----------------------
                                nKadouFlag = nKadouFlag - 1

                        End Select
                        If nKadouFlag = 0 Then
                            Exit For
                        End If
                    Next
                    '該当稼動データに主キーと該当保守データの主キーは一致
                    '処理日時のみ不一致の場合でもデータ登録する
                    If nKeyFlag = 0 Or (nKeyFlag = 1 And tKeyFlg = True) Then
                        For j As Integer = 0 To kadoDefineInfo.Length - 1
                            '保守データの項目値に稼動データの対応項目値を設定する
                            For n As Integer = 0 To hosyuDefineInfo.Length - 1
                                If hosyuDefineInfo(n).COMMENT = kadoDefineInfo(j).FIELD_NAME Then
                                    lstHosyuData(iHosyu)(n) = lstKadoData(iKadou)(j)
                                    Exit For
                                End If
                            Next
                        Next

                        lstRtnHosyuData.Add(lstHosyuData(iHosyu))
                        '主キー一致する場合、データクリア
                        If nKeyFlag = 0 And tKeyFlg = False Then
                            lstHosyuData.RemoveAt(iHosyu)
                            lstKadoData.RemoveAt(iKadou)
                        End If
                    End If
                Next
            Next
        Else
            For iKadou As Integer = lstKadoData.Count - 1 To 0 Step -1
                sArrInfo = lstKadoData(iKadou)
                nKeyFlag = 6
                '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
                '処理日時判定フラグを初期化
                tKeyFlg = False
                '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
                '保守定義情報により保守の主キー値を取得する
                For j As Integer = 0 To kadoDefineInfo.Length - 1
                    Select Case kadoDefineInfo(j).FIELD_NAME
                        Case "RAIL_SECTION_CODE"
                            sRAIL_SECTION_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "STATION_ORDER_CODE"
                            sSTATION_ORDER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "CORNER_CODE"
                            sCORNER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "MODEL_CODE"
                            sMODEL_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "UNIT_NO"
                            sUNIT_NO = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "PROCESSING_TIME"
                            sPROCESSING_TIME = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                    End Select
                    If nKeyFlag = 0 Then
                        Exit For
                    End If
                Next

                'データ
                For iHosyu As Integer = lstHosyuData.Count - 1 To 0 Step -1
                    nKeyFlag = 6
                    Dim nKadouFlag As Integer = 6
                    '-----Ver0.2　稼動保守データ欠落対応　ADD　START----------------------
                    '処理日時判定フラグを初期化
                    tKeyFlg = False
                    '-----Ver0.2　稼動保守データ欠落対応　ADD　END----------------------
                    '稼動定義情報により保守の主キー値と稼動の主キー値は一致するかどうかを判断する
                    For j As Integer = 0 To hosyuDefineInfo.Length - 1
                        Select Case hosyuDefineInfo(j).FIELD_NAME
                            Case "RAIL_SECTION_CODE"
                                If sRAIL_SECTION_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1
                            Case "STATION_ORDER_CODE"
                                If sSTATION_ORDER_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "CORNER_CODE"
                                If sCORNER_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "MODEL_CODE"
                                If sMODEL_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "UNIT_NO"
                                If sUNIT_NO.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "PROCESSING_TIME"
                                '-----Ver0.2　稼動保守データ欠落対応　MOD　START----------------------
                                '処理日時の年月日のみ比較する
                                If sPROCESSING_TIME.Substring(0, 8).Equals(lstHosyuData(iHosyu)(j).Substring(0, 8)) Then
                                    nKeyFlag = nKeyFlag - 1
                                Else
                                    tKeyFlg = True
                                End If
                                '-----Ver0.2　稼動保守データ欠落対応　MOD　END----------------------
                                nKadouFlag = nKadouFlag - 1

                        End Select
                        If nKadouFlag = 0 Then
                            Exit For
                        End If
                    Next

                    '該当稼動データに主キーと該当保守データの主キーは一致
                    '処理日時のみ不一致の場合でもデータ登録する
                    If nKeyFlag = 0 Or (nKeyFlag = 1 And tKeyFlg = True) Then
                        For j As Integer = 0 To kadoDefineInfo.Length - 1
                            '保守データの項目値に稼動データの対応項目値を設定する
                            For n As Integer = 0 To hosyuDefineInfo.Length - 1
                                If hosyuDefineInfo(n).COMMENT = kadoDefineInfo(j).FIELD_NAME Then
                                    lstHosyuData(iHosyu)(n) = sArrInfo(j)
                                    Exit For
                                End If
                            Next
                        Next
                        lstRtnHosyuData.Add(lstHosyuData(iHosyu))
                        '主キー一致する場合、データクリア
                        If nKeyFlag = 0 And tKeyFlg = False Then
                            lstHosyuData.RemoveAt(iHosyu)
                            lstKadoData.RemoveAt(iKadou)
                        End If
                    End If
                Next
            Next
        End If

        'ペアセットとなってない場合、収集データ登録を行う
        Call InsertCollectionDataPair(hosyuDefineInfo, lstHosyuData)
        Call InsertCollectionDataPair(kadoDefineInfo, lstKadoData)

        Return True
    End Function

    ''' <summary>
    ''' 収集データ登録
    ''' </summary>
    ''' <param name="defineInfo">定義情報</param>
    ''' <param name="lstData">データ</param>
    ''' <remarks>ペアセットとなってない場合、収集データ登録を行う</remarks>
    Private Shared Sub InsertCollectionDataPair(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                                ByVal lstData As List(Of String()))
        Dim nKeyFlag As Integer
        Dim sArrInfo As String()

        '主キー情報
        Dim sRAIL_SECTION_CODE As String = ""
        Dim sSTATION_ORDER_CODE As String = ""
        Dim sCORNER_CODE As String = ""
        Dim sMODEL_CODE As String = ""
        Dim sUNIT_NO As String = ""
        Dim sPROCESSING_TIME As String = ""
        Dim sCOLLECT_START_TIME As String = ""
        Dim sCOLLECT_END_TIME As String = ""

        For i As Integer = 0 To lstData.Count - 1
            sArrInfo = lstData(i)
            nKeyFlag = 6
            '保守定義情報により保守の主キー値を取得する
            For j As Integer = 0 To defineInfo.Length - 1
                Select Case defineInfo(j).FIELD_NAME
                    Case "RAIL_SECTION_CODE"
                        sRAIL_SECTION_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "STATION_ORDER_CODE"
                        sSTATION_ORDER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "CORNER_CODE"
                        sCORNER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "MODEL_CODE"
                        sMODEL_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "UNIT_NO"
                        sUNIT_NO = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "PROCESSING_TIME"
                        sPROCESSING_TIME = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                End Select
                If nKeyFlag = 0 Then
                    Exit For
                End If
            Next

            '基本ヘッダ情報を設定する
            Dim baseInfo As RecDataStructure.BaseInfo = Nothing
            With baseInfo
                .STATION_CODE.RAIL_SECTION_CODE = sRAIL_SECTION_CODE
                .STATION_CODE.STATION_ORDER_CODE = sSTATION_ORDER_CODE
                .CORNER_CODE = sCORNER_CODE
                .MODEL_CODE = sMODEL_CODE
                .UNIT_NO = CInt(sUNIT_NO)
                .PROCESSING_TIME = sPROCESSING_TIME
                .DATA_KIND = DbConstants.CdtKindKadoData
            End With

            '収集データ登録を行う
            CollectedDataTypoRecorder.Record(baseInfo, DbConstants.CdtKindKadoData, _
                                             Lexis.CdtUnpairedKadoDataDetected.Gen(baseInfo.UNIT_NO))
        Next
    End Sub

    ''' <summary>
    ''' 稼動・保守データのチェック
    ''' </summary>
    ''' <param name="iniInfoAry">iniファイル</param>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dlineInfoLstNew">datファイル内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>解析処理による取得データをチェックする</remarks>
    Private Shared Function CheckDataNoMsg(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String, _
                                      ByVal sDataKind As String) As Boolean
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim strDate As String = ""

        dlineInfoLstNew = New List(Of String())

        For i = 0 To dlineInfoLst.Count - 1

            lineInfo = dlineInfoLst.Item(i)

            '共通のチェックを行う
            If CheckDataCommNoMsg(iniInfoAry, lineInfo, datFileName) = False Then
                Continue For
            End If

            '特別なチェック
            If sDataKind = Kado_DataKind Then
                For j = 0 To iniInfoAry.Length - 1
                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "KAI_INSPECT_TIME", "SYU_INSPECT_TIME"     '改札側点検日時  集札側点検日時
                            '-------Ver0.1　北陸対応　ADD START-----------
                            If lineInfo(j).Substring(0, 14) <> "00000000000000" Then
                                strDate = lineInfo(j).Substring(0, 4) & "/" & _
                                     lineInfo(j).Substring(4, 2) & "/" & _
                                     lineInfo(j).Substring(6, 2) & " " & _
                                     lineInfo(j).Substring(8, 2) & ":" & _
                                     lineInfo(j).Substring(10, 2) & ":" & _
                                     lineInfo(j).Substring(12, 2)

                                If Not Date.TryParse(strDate, New Date) Then
                                    lineInfo(j) = "00000000000000"
                                End If
                            End If
                            '-------Ver0.1　北陸対応　ADD END-----------
                    End Select
                Next
            End If

            dlineInfoLstNew.Add(lineInfo)
        Next

        Return True

    End Function

    ''' <summary>
    ''' DATファイルの共通チェック:1レコードのチェック
    ''' </summary>
    ''' <param name="iniInfoAry">iniファイル情報</param>
    ''' <param name="lineInfo">レコードデータ</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Public Shared Function CheckDataCommNoMsg(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                              ByVal lineInfo() As String, _
                                              ByVal datFileName As String) As Boolean

        Dim iFlag As Integer = 4
        Dim dataKind As String = "" 'データ種別

        Try

            For i As Integer = 0 To iniInfoAry.Length - 1
                If UCase(iniInfoAry(i).FIELD_NAME) = "DATA_KIND" Then
                    dataKind = lineInfo(i) 'OPT: 使わないので不要
                    Continue For
                End If

                '駅コード、コーナーコード、号機番号が全部チェックではない場合
                If iFlag > 0 Then
                    Select Case UCase(iniInfoAry(i).FIELD_NAME)  '駅コード、コーナーコード、号機番号
                        Case "RAIL_SECTION_CODE", "STATION_ORDER_CODE", "CORNER_CODE", "UNIT_NO"
                            iFlag = iFlag - 1

                            If (iniInfoAry(i).PARA2 = False) Then
                                If Integer.Parse(lineInfo(i)) = 0 Then
                                    Return False
                                End If
                            End If

                            Continue For
                    End Select
                End If

                'キー 且 NULL不可
                Select Case UCase(iniInfoAry(i).FIELD_FORMAT)
                    Case "INTEGER"
                        '不正場合
                        If lineInfo(i) IsNot Nothing AndAlso _
                          (Not lineInfo(i) = "") AndAlso _
                          OPMGUtility.checkNumber(lineInfo(i)) = False Then
                            Return (False)
                        Else '空場合
                            'NULL不可
                            If (iniInfoAry(i).PARA2 = False) Then
                                If Integer.Parse(lineInfo(i)) = 0 Then
                                    Return (False)
                                End If
                            End If

                        End If
                    Case "DATESTR"
                        '処理日時フォーマートチェック
                        Dim lnDate As Long = 0

                        If OPMGUtility.checkNumber(lineInfo(i)) = False Then
                            Return False
                        Else '全部０場合
                            'NULL不可
                            If (iniInfoAry(i).PARA2 = False) Then
                                If Long.Parse(lineInfo(i)) = 0 Then
                                    Return False
                                End If
                            End If
                            If lineInfo(i).Length = 14 Then
                                If BatchAppComm.CheckDate(lineInfo(i)) = False Then
                                    Return False
                                End If
                            Else
                                Return False
                            End If

                        End If
                End Select

            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '収集データの登録
            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo)
            Return False
        End Try

        Return True

    End Function
    '--------------Ver0.1　北陸対応　ADD START-------------------
    ''' <summary>グループ番号取得</summary>
    ''' <returns>グループ番号</returns>
    ''' <param name="ekiNo">線区駅順コード</param>
    ''' <remarks>線区駅順コードを条件にグループ番号取得</remarks>
    Private Shared Function GetGroupNo(ByVal ekiNo As String) As Boolean
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable

        sSQL = " SELECT M_BRANCH_OFFICE.GROUP_NO " _
            & " FROM M_BRANCH_OFFICE, V_MACHINE_NOW as m1 " _
            & " WHERE m1.RAIL_SECTION_CODE+m1.STATION_ORDER_CODE= " & ekiNo _
            & " and m1.BRANCH_OFFICE_CODE =M_BRANCH_OFFICE.CODE " _
             & "GROUP BY M_BRANCH_OFFICE.GROUP_NO "

        dbCtl = New DatabaseTalker

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)
            If dt Is Nothing Then
                Return False
            Else
                GrpNo = CInt(dt(0).Item("GROUP_NO"))
                Return True
            End If
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try
    End Function
    '---------------Ver0.1　北陸対応　ADD END-----------------
#End Region

End Class