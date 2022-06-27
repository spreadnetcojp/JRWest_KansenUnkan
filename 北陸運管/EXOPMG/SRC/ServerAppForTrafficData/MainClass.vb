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
Imports JR.ExOpmg.ServerApp

''' <summary>
''' 時間帯別乗降データ取込
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass
#Region "宣言領域（Private）"
    ''' <summary>
    ''' 時間帯別乗降データ種別
    ''' </summary>
    Private Const Tim_DataKind As String = "B1"

    ''' <summary>
    ''' 時間帯テーブル名
    ''' </summary>
    Private Const Tim_TableName As String = "D_TRAFFIC_DATA"

    ''' <summary>
    ''' 合計入出場者数異常
    ''' </summary>
    Private Shared ERR_MSG_ERRVALUE As String = "{0}行目の入場者数：{1}　出場者数：{2} 合計入出場者数：{3}　算出結果と異なります。"

#End Region

#Region "メソッド（Main）"
    ''' <summary>
    ''' 時間帯別乗降データ取込プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 時間帯別乗降データ取込プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForTrafficData")
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

                Log.Init(sLogBasePath, "ForTrafficData")
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
    '''  時間帯別乗降データ取込
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

            Dim dlineInfoLst As List(Of String()) = Nothing
            Dim dlineInfoLstNew As List(Of String()) = Nothing
            Dim dataInfoLst As List(Of String()) = Nothing

            'OPT: 以下、GetDefineInfoはMainメソッドにて一度だけ行う方がよいが、
            'iniInfoAryがImmutableでないため、毎回つくりなおしているという
            '話もあり、対応するなら注意しなければならない。

            Dim iniInfoAry() As RecDataStructure.DefineInfo = Nothing

            '定義情報を取得する
            If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "TrafficData_001", iniInfoAry) = False Then
                AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                Return RecordingResult.IOError
            End If

            'datファイルデータ取得
            If GetInfoFromDataFile(sFilePath, sModelCode, iniInfoAry, dlineInfoLst) = False Then
                Return RecordingResult.ParseError
            End If

            'データチェック
            If CheckData(dlineInfoLst, dlineInfoLstNew, sFilePath, iniInfoAry) = False Then
                Return RecordingResult.IOError
            End If

            'データ解析
            If GetDBInfoFromDataInfo(dlineInfoLstNew, dataInfoLst, iniInfoAry) = False Then
                Return RecordingResult.IOError
            End If

            'DB登録
            If BatchAppComm.PutDataToDBCommon(iniInfoAry, dataInfoLst, Tim_TableName) = False Then
                Return RecordingResult.IOError
            End If

            '成功した場合
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Tim_DataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' 時間帯別乗降データの解析
    ''' </summary>
    ''' <param name="sFilePath">登録するべきデータが格納されたファイルの絶対パス名</param>
    ''' <param name="sClientKind">クライアント種別</param>
    ''' <param name="iniInfoAry">Ini定義情報</param>
    ''' <param name="dlineInfoLst">データリスト</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>取得した電文フォーマット定義情報にて時間帯別乗降データを解析する</remarks>
    Private Shared Function GetInfoFromDataFile(ByVal sFilePath As String, _
                                                ByVal sClientKind As String, _
                                                ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByRef dlineInfoLst As List(Of String())) As Boolean
        Dim nHeadSize As Integer = 17
        Dim nDataSize As Integer = 433

        'datファイルデータ取得
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, sFilePath, sClientKind, nDataSize, nHeadSize, dlineInfoLst, Tim_DataKind) = False Then
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' 時間帯別乗降データのチェック
    ''' </summary>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dlineInfoLstNew">チェック後、正確的datファイル内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルをチェックする</remarks>
    Private Shared Function CheckData(ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String, _
                                      ByVal iniInfoAry() As RecDataStructure.DefineInfo) As Boolean

        '機器構成マスタSQL
        Dim strSQL As String = "SELECT COUNT(1) FROM V_MACHINE_NOW WHERE RAIL_SECTION_CODE = {0} AND STATION_ORDER_CODE = {1} AND CORNER_CODE = {2}"
        Dim lineInfo(iniInfoAry.Length) As String
        Dim lineInfoNew(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:エラーがある;false:エラーがない
        Dim dbCtl As New DatabaseTalker
        Dim nRtn As Integer
        Dim ErrNo As Integer
        dlineInfoLstNew = New List(Of String())

        Dim sRail_Code As String = ""
        Dim sStation_Code As String = ""
        Dim sCorner_Code1 As String = ""
        Dim sCorner_Code2 As String = ""

        Try
            dbCtl.ConnectOpen()
            '全部レコード
            For i As Integer = 0 To dlineInfoLst.Count - 1

                '1レコード取得
                lineInfo = dlineInfoLst.Item(i)

                '初期化
                isHaveErr = False
                sRail_Code = ""
                sStation_Code = ""
                sCorner_Code1 = ""
                sCorner_Code2 = ""

                '共通のチェック
                If BatchAppComm.CheckDataComm(i + 1, iniInfoAry, lineInfo, datFileName, False) = False Then
                    Continue For
                End If

                'OPT: 以下、「lineInfo(j).ToString」の「ToString」は明らかに不要。

                '全部フィールド
                For j As Integer = 0 To iniInfoAry.Length - 1
                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "DATA_KIND" 'データ種別
                            If Not lineInfo(j).Equals(Tim_DataKind) Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (i + 1).ToString, iniInfoAry(j).KOMOKU_NAME))
                                BatchAppComm.SetCollectionData(datFileName, Tim_DataKind) '収集データ登録
                                Exit For
                            End If
                        Case "CORNER_CODE1" 'コーナーコード
                            If lineInfo(j).Replace("0", "").Length > 0 Then
                                sCorner_Code1 = lineInfo(j).ToString
                            Else
                                sCorner_Code1 = ""
                                ErrNo = j
                            End If

                        Case "CORNER_CODE2" 'コーナーコード
                            If lineInfo(j).Replace("0", "").Length > 0 Then
                                sCorner_Code2 = lineInfo(j).ToString
                            Else
                                sCorner_Code2 = ""
                            End If
                        Case "STATION_IN" '合計入出場者数
                            If Long.Parse(lineInfo(j)) + Long.Parse(lineInfo(j + 1)) <> Long.Parse(lineInfo(j + 2)) Then
                                Log.Error(String.Format(ERR_MSG_ERRVALUE, Convert.ToString(i + 1), lineInfo(j), lineInfo(j + 1), lineInfo(j + 2)))
                                lineInfo(j + 2) = "0"
                            End If

                        Case "TICKET_NO"
                            lineInfo(j) = "99"

                        Case "DATE"
                            If lineInfo(j).Length >= 8 Then
                                lineInfo(j) = lineInfo(j).Substring(0, 4) & "/" & lineInfo(j).Substring(4, 2) & "/" & lineInfo(j).Substring(6, 2)
                            End If

                        Case "TIME_ZONE"
                            If lineInfo(j).Length >= 4 Then
                                lineInfo(j) = lineInfo(j).Substring(0, 2) & ":" & lineInfo(j).Substring(2, 2)
                            End If

                        Case "RAIL_SECTION_CODE"
                            sRail_Code = lineInfo(j).ToString

                        Case "STATION_ORDER_CODE"
                            sStation_Code = lineInfo(j).ToString

                        Case "UNIT_NO"
                            lineInfo(j) = "0"
                    End Select
                Next

                If sCorner_Code1 = "" And sCorner_Code2 = "" Then
                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, (i + 1).ToString, iniInfoAry(ErrNo).KOMOKU_NAME))
                    BatchAppComm.SetCollectionData(datFileName, Tim_DataKind) '収集データ登録
                    isHaveErr = True
                End If

                '機器構成マスタチェック
                If Not sRail_Code = "" AndAlso Not sStation_Code = "" Then
                    'コーナー1チェック
                    If Not sCorner_Code1 = "" Then
                        '機器構成マスタチェック用SQL文
                        nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(String.Format(strSQL, Utility.SetSglQuot(sRail_Code), _
                                                                      Utility.SetSglQuot(sStation_Code), _
                                                                      Utility.SetSglQuot(sCorner_Code1))))
                        If nRtn = 0 Then
                            '収集データ登録を行う
                            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo, _
                                              Lexis.CdtTheCornerNotFound.Gen(sRail_Code, sStation_Code, sCorner_Code1), _
                                              True)
                        End If
                    End If

                    'コーナー2チェック
                    If Not sCorner_Code2 = "" Then
                        '機器構成マスタチェック用SQL文
                        nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(String.Format(strSQL, Utility.SetSglQuot(sRail_Code), _
                                                                      Utility.SetSglQuot(sStation_Code), _
                                                                      Utility.SetSglQuot(sCorner_Code2))))
                        If nRtn = 0 Then
                            '収集データ登録を行う
                            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo, _
                                              Lexis.CdtTheCornerNotFound.Gen(sRail_Code, sStation_Code, sCorner_Code2), _
                                              True)
                        End If
                    End If
                End If

                If isHaveErr = False Then
                    dlineInfoLstNew.Add(lineInfo)
                End If
            Next
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return True

    End Function

    ''' <summary>
    ''' データ処理
    ''' </summary>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dataInfoLst">処理後のデータ</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>
    ''' データ処理
    ''' </remarks>
    Private Shared Function GetDBInfoFromDataInfo(ByVal dlineInfoLst As List(Of String()), _
                                                  ByRef dataInfoLst As List(Of String()), _
                                                  ByVal iniInfoAry() As RecDataStructure.DefineInfo) As Boolean
        If dlineInfoLst Is Nothing OrElse dlineInfoLst.Count = 0 Then
            Return True
        End If

        Dim TICKET_NO As Integer = 1
        Dim dataInfo() As String
        Dim dlineInfo() As String

        If dataInfoLst Is Nothing Then
            dataInfoLst = New List(Of String())
        Else
            dataInfoLst.Clear()
        End If

        'データ再加工
        For i As Integer = 0 To dlineInfoLst.Count - 1
            dataInfo = dlineInfoLst.Item(i)

            Dim sCurCorner As String = ""
            Dim sCurStationIn As String = ""
            Dim sCurStationOut As String = ""
            Dim sCurStationSum As String = ""
            Dim nCurTicketNo As Integer
            Dim nFlag As Integer = 0

            For j As Integer = 0 To iniInfoAry.Length - 1
                Select Case iniInfoAry(j).FIELD_NAME
                    Case "CORNER_CODE1"
                        sCurCorner = Format(CInt(dataInfo(j)), "000")
                        nCurTicketNo = 1
                    Case "CORNER_CODE2"
                        'コーナーがセットされた場合のみ券種カウントアップする
                        If dataInfo(j).Replace("0", "").Length > 0 Then
                            sCurCorner = Format(CInt(dataInfo(j)), "000")
                            nCurTicketNo = 1
                        Else
                            nFlag = 0
                            Exit For
                        End If
                    Case "STATION_IN"
                        sCurStationIn = dataInfo(j)
                        nFlag = nFlag + 1
                    Case "STATION_OUT"
                        sCurStationOut = dataInfo(j)
                        nFlag = nFlag + 1
                    Case "STATION_SUM"
                        sCurStationSum = dataInfo(j)
                        nFlag = nFlag + 1
                End Select

                'データを追加する。
                If nFlag = 3 Then
                    ReDim dlineInfo(13)
                    For k As Integer = 0 To 8
                        dlineInfo(k) = dataInfo(k)
                    Next
                    dlineInfo(9) = sCurCorner
                    dlineInfo(10) = CStr(nCurTicketNo)
                    dlineInfo(11) = sCurStationIn
                    dlineInfo(12) = sCurStationOut
                    dlineInfo(13) = sCurStationSum

                    If dlineInfo(9).Replace("0", "").Length > 0 Then
                        dataInfoLst.Add(dlineInfo)
                    End If

                    'クリア
                    nFlag = 0

                    '券種
                    nCurTicketNo = nCurTicketNo + 1
                End If
            Next
        Next

        'コーナー変更
        For j As Integer = 0 To iniInfoAry.Length - 1
            Select Case iniInfoAry(j).FIELD_NAME
                Case "CORNER_CODE1", "CORNER_CODE2"
                    iniInfoAry(j).FIELD_NAME = "CORNER_CODE"
            End Select
        Next
        Return True

    End Function

#End Region

End Class
