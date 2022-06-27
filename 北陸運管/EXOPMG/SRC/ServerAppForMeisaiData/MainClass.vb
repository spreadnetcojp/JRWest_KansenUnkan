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

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp

''' <summary>
''' 本プロセスは、収集した不正乗車検出データ、強行突破検出データ、
''' 紛失券検出データ、FREX定期券ＩＤ検出データを解析し、運用管理サーバのDBに登録する。
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "宣言領域（Private）"

    Private Shared ERR_MSG_WTN As String = "{0}行目の不正判定対象区分がありません"
    Private Shared ERR_Wrong_WTN As String = "{0}行目の不正判定対象区分が不正です"
    Private Shared ERR_MSG_TN As String = "{0}行目の券種番号がありません"
    Private Shared ERR_Wrong_TN As String = "{0}行目の券種番号が不正です"
    Private Shared ERR_MSG_ID As String = "{0}行目のID番号がありません"

    Private Const MeisaiLength As Integer = 111              'データ桁数
    Private Const HeadLength As Integer = 17                 'ヘッダ桁数
    Private Const FuseiJoshaDataKind As String = "A2"        '不正乗車検出データのデータ種別
    Private Const KyokoToppaDataKind As String = "A3"        '強行突破検出データのデータ種別
    Private Const FunshitsuDataKind As String = "A4"         '紛失券検出データのデータ種別
    Private Const FrexDataKind As String = "A5"              'FREX定期券ＩＤ検出データのデータ種別

    'テーブル名
    Private Const Fuseijyosha_TableName As String = "D_FUSEI_JOSHA_DATA"
    Private Const Kyokotopa_TableName As String = "D_KYOKO_TOPPA_DATA"
    Private Const Funshitsu_TableName As String = "D_FUNSHITSU_DATA"

    Private Shared defineInfo_FuseiJosha() As RecDataStructure.DefineInfo = Nothing  '定義情報
    Private Shared defineInfo_KyokoToppa() As RecDataStructure.DefineInfo = Nothing '定義情報
    Private Shared defineInfo_Funshitsu() As RecDataStructure.DefineInfo = Nothing '定義情報
    Private Shared defineInfo_Frex() As RecDataStructure.DefineInfo = Nothing '定義情報

    Private Shared lstFuseiJoshaData As New List(Of String())
    Private Shared lstKyokoToppaData As New List(Of String())
    Private Shared lstFunshitsuData As New List(Of String())
    Private Shared lstFrexData As New List(Of String())
#End Region

#Region "Main"
    ''' <summary>
    ''' 明細データ登録プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 明細データ登録プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForMeisaiData")
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

                Log.Init(sLogBasePath, "ForMeisaiData")
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

                '不正乗車検出データの定義情報を取得する。
                defineInfo_FuseiJosha = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FuseiJoshaFormatFilePath, "FuseiJosha_001", defineInfo_FuseiJosha) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                '強行突破検索データの定義情報を取得する。
                defineInfo_KyokoToppa = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.KyokoToppaFormatFilePath, "KyokoToppa_001", defineInfo_KyokoToppa) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                '紛失券検出データの定義情報を取得する。
                defineInfo_Funshitsu = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FunshitsuFormatFilePath, "Funshitsu_001", defineInfo_Funshitsu) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                'FREX定期券ID検索データの定義情報を取得する。
                defineInfo_Frex = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FrexFormatFilePath, "Funshitsu_001", defineInfo_Frex) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

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
    ''' 明細データ登録処理。
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
            Dim lstChkData As New List(Of String())                     'チェックしたデータ情報
            Dim lstDBData As New List(Of String())                      'DBに登録するデータ情報
            lstFuseiJoshaData.Clear()
            lstKyokoToppaData.Clear()
            lstFunshitsuData.Clear()
            lstFrexData.Clear()
            '明細データの解析
            If GetInfoFromDataFile(sFilePath, sModelCode) = False Then
                Return RecordingResult.ParseError
            End If

            '-----------------------------不正乗車検出データ取込処理 Start-----------------------------
            'チェックを行う。
            lstChkData = New List(Of String())
            If CheckData(defineInfo_FuseiJosha, lstFuseiJoshaData, FuseiJoshaDataKind, sFilePath, lstChkData) = True Then
                If lstChkData IsNot Nothing AndAlso lstChkData.Count > 0 Then
                    '取得したデータをDB登録データに再加工する。
                    lstDBData = New List(Of String())
                    If GetDBInfoFromDataInfo(defineInfo_FuseiJosha, FuseiJoshaDataKind, lstChkData, lstDBData) = False Then
                        Return RecordingResult.ParseError
                    End If
                    'DBにデータを登録する。
                    If BatchAppComm.PutDataToDBCommon(defineInfo_FuseiJosha, lstDBData, Fuseijyosha_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------不正乗車検出データ取込処理 End-----------------------------

            '-----------------------------強行突破検出データ取込処理 Start-----------------------------
            'チェックを行う。
            lstDBData = New List(Of String())
            If CheckData(defineInfo_KyokoToppa, lstKyokoToppaData, KyokoToppaDataKind, sFilePath, lstDBData) = True Then
                If lstDBData IsNot Nothing AndAlso lstDBData.Count > 0 Then
                    'DBにデータを登録する。
                    If BatchAppComm.PutDataToDBCommon(defineInfo_KyokoToppa, lstDBData, Kyokotopa_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------強行突破検出データ取込処理 End-----------------------------

            '-----------------------------紛失券検出データ取込処理 Start-----------------------------
            'チェックを行う。
            lstDBData = New List(Of String())
            If CheckData(defineInfo_Funshitsu, lstFunshitsuData, FunshitsuDataKind, sFilePath, lstDBData) = True Then
                If lstDBData IsNot Nothing AndAlso lstDBData.Count > 0 Then
                    'DBにデータを登録する。
                    If BatchAppComm.PutDataToDBCommon(defineInfo_Funshitsu, lstDBData, Funshitsu_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------紛失券検出データ取込処理 End-----------------------------

            '-----------------------------FREX定期券ID検出データ取込処理 Start-----------------------------
            'チェックを行う。
            lstChkData = New List(Of String())
            If CheckData(defineInfo_Frex, lstFrexData, FrexDataKind, sFilePath, lstChkData) = True Then
                If lstChkData IsNot Nothing AndAlso lstChkData.Count > 0 Then
                    '取得したデータをDB登録データに再加工する。
                    lstDBData = New List(Of String())
                    If GetDBInfoFromDataInfo(defineInfo_Frex, FrexDataKind, lstChkData, lstDBData) = False Then
                        Return RecordingResult.ParseError
                    End If
                    'DBにデータを登録する。
                    If BatchAppComm.PutDataToDBCommon(defineInfo_Frex, lstDBData, Funshitsu_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------FREX定期券ID検出データ取込処理 End-----------------------------

            '登録が成功した場合
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: 明細データに関する解析失敗のファイル種別は、常に不正乗車検出データの種別とする。
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(FuseiJoshaDataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' 取得したフォーマット定義情報にてバイナリファイルを解析し、
    ''' 登録データとしてメモリに保持する。
    ''' </summary>
    ''' <param name="sFilePath">登録するべきデータが格納されたファイルの絶対パス名</param>
    ''' <param name="sModelCode">機種コード</param>
    ''' <returns>True:正常/False:異常</returns>
    Private Shared Function GetInfoFromDataFile(ByVal sFilePath As String, _
                                                ByVal sModelCode As String) As Boolean

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

        Try
            'ファイルストリームを取得
            fileStream = New FileStream(sFilePath, FileMode.Open)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            '桁数チェック
            If fileStream.Length < (MeisaiLength + HeadLength) Then
                Log.Error(RecAppConstants.ERR_TOO_SHORT_FILE)
                Return False
            End If

            '送信方式
            If fileStream.Length > (MeisaiLength + HeadLength) Then 'ftpの場合
                iStarRecIndex = 1
            Else 'socketの場合
                iStarRecIndex = 0
            End If

            'レコード数取得
            If fileStream.Length Mod (MeisaiLength + HeadLength) = 0 Then
                iRecCnt = CInt(fileStream.Length / (MeisaiLength + HeadLength))
            Else
                iRecCnt = CInt(Int(fileStream.Length / (MeisaiLength + HeadLength)))
            End If

            '読ファイル
            For i As Integer = iStarRecIndex To iRecCnt - 1

                ReDim bData(MeisaiLength + HeadLength) '1レコードのデータ

                '定位
                fileStream.Seek(i * (MeisaiLength + HeadLength), SeekOrigin.Begin)
                fileStream.Read(bData, 0, MeisaiLength + HeadLength)

                headInfo = Nothing
                BinaryHeadInfoParse.GetBaseInfo(bData, sModelCode, headInfo)

                If headInfo.DATA_KIND = FuseiJoshaDataKind Then
                    '不正乗車検索データの解析
                    ReDim sArrRecord(defineInfo_FuseiJosha.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_FuseiJosha, bData, headInfo, sArrRecord) = False Then
                        '収集データの登録
                        BatchAppComm.SetCollectionData(headInfo, FuseiJoshaDataKind)
                        Continue For
                    End If

                    '電文での行目を設定する
                    sArrRecord(defineInfo_FuseiJosha.Length - 1) = CStr(i + 1)

                    '解析したデータを設定する
                    lstFuseiJoshaData.Add(sArrRecord)

                ElseIf headInfo.DATA_KIND = KyokoToppaDataKind Then
                    '強行突破検出データの解析
                    ReDim sArrRecord(defineInfo_KyokoToppa.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_KyokoToppa, bData, headInfo, sArrRecord) = False Then
                        '収集データの登録
                        BatchAppComm.SetCollectionData(headInfo, KyokoToppaDataKind)
                        Continue For
                    End If

                    '電文での行目を設定する
                    sArrRecord(defineInfo_KyokoToppa.Length - 1) = CStr(i + 1)

                    '解析したデータを設定する
                    lstKyokoToppaData.Add(sArrRecord)

                ElseIf headInfo.DATA_KIND = FunshitsuDataKind Then
                    '紛失券検出データの解析
                    ReDim sArrRecord(defineInfo_Funshitsu.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_Funshitsu, bData, headInfo, sArrRecord) = False Then
                        '収集データの登録
                        BatchAppComm.SetCollectionData(headInfo, FunshitsuDataKind)
                        Continue For
                    End If

                    '電文での行目を設定する
                    sArrRecord(defineInfo_Funshitsu.Length - 1) = CStr(i + 1)

                    '解析したデータを設定する
                    lstFunshitsuData.Add(sArrRecord)

                ElseIf headInfo.DATA_KIND = FrexDataKind Then
                    'Frex定期券検出データの解析
                    ReDim sArrRecord(defineInfo_Frex.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_Frex, bData, headInfo, sArrRecord) = False Then
                        '収集データの登録
                        BatchAppComm.SetCollectionData(headInfo, FrexDataKind)
                        Continue For
                    End If

                    '電文での行目を設定する
                    sArrRecord(defineInfo_Frex.Length - 1) = CStr(i + 1)

                    '解析したデータを設定する
                    lstFrexData.Add(sArrRecord)
                Else
                    'データ種別が不正ログを出力する
                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, "データ種別"))
                    '収集データの登録
                    BatchAppComm.SetCollectionData(sFilePath, headInfo.DATA_KIND)
                End If
            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: 明細データに関する解析失敗のファイル種別は、常に不正乗車検出データの種別とする。
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(FuseiJoshaDataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return False
        Finally
            'ファイルストリームを解放
            fileStream.Close()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' データチェック
    ''' </summary>
    ''' <param name="defineInfo">定義情報</param>
    ''' <param name="lstDataFrom">データ情報</param>
    ''' <param name="sDataKind">データ種別</param>
    ''' <param name="refLstData">チェックしたデータ情報</param>
    ''' <returns>True:正常 False:チェックエラー</returns>
    Private Shared Function CheckData(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                      ByVal lstDataFrom As List(Of String()), _
                                      ByVal sDataKind As String, _
                                      ByVal sFileName As String, _
                                      ByRef refLstData As List(Of String())) As Boolean

        If lstDataFrom Is Nothing OrElse lstDataFrom.Count <= 0 Then Return True

        Dim bRtn As Boolean = True

        Dim sArrData(defineInfo.Length) As String
        Dim isHaveErr As Boolean     'False:チェックOK True:チェック異常
        Dim iFlag As Integer
        Dim iLineNo As Integer
        refLstData = New List(Of String())

        For i As Integer = 0 To lstDataFrom.Count - 1

            isHaveErr = False

            '該当レコードを取得する
            sArrData = lstDataFrom.Item(i)

            '電文での行目を取得する
            If OPMGUtility.checkNumber(sArrData(sArrData.Length - 1)) Then
                iLineNo = CInt(sArrData(sArrData.Length - 1))
            Else
                iLineNo = i + 1
            End If

            '共通のチェックを行う
            If BatchAppComm.CheckDataComm(iLineNo, defineInfo, sArrData, sFileName) = False Then
                Continue For
            End If

            '特別なチェック
            Select Case sDataKind
                Case FuseiJoshaDataKind
                    '不正乗車検出データのチェック
                    iFlag = 10
                    Dim iErrCnt As Integer = 0
                    For j As Integer = 0 To defineInfo.Length - 1
                        Select Case defineInfo(j).FIELD_NAME
                            Case "WRANG_TARGET_NO"
                                iFlag = iFlag - 1
                                '不正判定対象区分のチェック
                                If OPMGUtility.checkNumber(sArrData(j)) = False Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_Wrong_WTN, iLineNo))
                                    '収集データ登録を行う
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                                'Nullチェック
                                If sArrData(j).Replace("0", "").Length <= 0 Then
                                    iErrCnt = iErrCnt + 1
                                End If
                        End Select
                        'チェックしたの場合、中止
                        If iFlag = 0 Then Exit For
                    Next
                    'Nullチェック
                    If iErrCnt = 10 Then
                        isHaveErr = True
                        Log.Error(String.Format(ERR_MSG_WTN, iLineNo))
                        '収集データ登録を行う
                        BatchAppComm.SetCollectionData(defineInfo, sArrData)
                        Exit For
                    End If
                Case FunshitsuDataKind
                    '紛失券検出データのチェック
                    iFlag = 2
                    For j As Integer = 0 To defineInfo.Length - 1
                        Select Case defineInfo(j).FIELD_NAME
                            Case "TICKET_NO"
                                iFlag = iFlag - 1
                                '券種番号のチェック
                                If OPMGUtility.checkNumber(sArrData(j)) = False Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_Wrong_TN, iLineNo))
                                    '収集データ登録を行う
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                                If Integer.Parse(sArrData(j)) = 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_MSG_TN, iLineNo))
                                    '収集データ登録を行う
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                            Case "ID_NO"
                                iFlag = iFlag - 1
                                'ID番号のチェック
                                If sArrData(j).Replace("0", "").Length <= 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_MSG_ID, iLineNo))
                                    '収集データ登録を行う
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                        End Select
                        'チェックしたの場合、中止
                        If iFlag = 0 Then Exit For
                    Next
                Case FrexDataKind
                    'FREX定期券ID検出データのチェック
                    iFlag = 1
                    For j As Integer = 0 To defineInfo.Length - 1
                        Select Case defineInfo(j).FIELD_NAME
                            Case "ID_NO"
                                iFlag = iFlag - 1
                                'ID番号のチェック
                                If sArrData(j).Replace("0", "").Length <= 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_MSG_ID, iLineNo))
                                    '収集データ登録を行う
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                        End Select
                        'チェックしたの場合、中止
                        If iFlag = 0 Then Exit For
                    Next
            End Select

            '正常の場合、データを追加する
            If isHaveErr = False Then
                refLstData.Add(sArrData)
            End If
        Next

        Return bRtn
    End Function

    ''' <summary>
    ''' 取得したデータをDB登録データに再加工する
    ''' </summary>
    ''' <param name="defineInfo">定義情報</param>
    ''' <param name="sDataKind">データ種別</param>
    ''' <param name="lstGetData">データ情報</param>
    ''' <param name="lstData">処理データ情報</param>
    ''' <returns></returns>
    Private Shared Function GetDBInfoFromDataInfo(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                           ByVal sDataKind As String, _
                                           ByVal lstGetData As List(Of String()), _
                                           ByRef lstData As List(Of String())) As Boolean

        If lstGetData Is Nothing OrElse lstGetData.Count <= 0 Then Return True

        Dim sArrRecord() As String             'レコード

        '取得したデータをＤＢデータの格式に転換する。
        lstData = New List(Of String())
        If sDataKind.Equals(FuseiJoshaDataKind) Then
            Dim isWtn As Boolean = False
            Dim nWtn As Integer = 0         '対象区分の位置
            Dim nWtnValue As Integer = 1    '対象区分の値
            '不正乗車検出データ
            For i As Integer = 0 To lstGetData.Count - 1
                isWtn = False
                nWtn = 0
                For j As Integer = 0 To defineInfo.Length - 1
                    '不正判定NG項目によって、不正判定対象区分を設定する。
                    If defineInfo(j).FIELD_NAME = "WRANG_TARGET_NO" Then
                        '不正判定NG項目の桁数
                        If isWtn = False Then
                            nWtn = j
                            isWtn = True
                            nWtnValue = 1
                        End If
                        'クリア
                        ReDim sArrRecord(nWtn)

                        If CInt(lstGetData(i)(j)) <> 0 Then
                            '基本ヘッダを設定する
                            For K As Integer = 0 To nWtn - 1
                                sArrRecord(K) = lstGetData(i)(K)
                            Next

                            '不正判定対象区分を設定する
                            sArrRecord(nWtn) = CStr(nWtnValue)

                            lstData.Add(sArrRecord)
                        End If

                        '不正判定対象区分の値を処理する。
                        nWtnValue = nWtnValue + 1
                    End If
                Next
            Next
        ElseIf sDataKind.Equals(FrexDataKind) Then
            'FREX定期券ＩＤ検出データ:券種番号は｢10｣とする。
            For i As Integer = 0 To lstGetData.Count - 1
                For j As Integer = 0 To defineInfo.Length - 1
                    If defineInfo(j).FIELD_NAME = "TICKET_NO" Then
                        lstGetData(i)(j) = "10"
                    End If
                Next
            Next
            lstData = lstGetData
        Else
            lstData = lstGetData
        End If

        Return True
    End Function
#End Region
End Class
