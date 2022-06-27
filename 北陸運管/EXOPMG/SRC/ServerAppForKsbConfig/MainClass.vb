' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/06/01       金沢  北陸・項目拡張対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading
Imports System.Diagnostics
Imports JR.ExOpmg.ServerApp
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp.RecDataStructure
Imports System.Text

''' <summary>
''' 監視盤設定データ登録プロセス共通のメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "宣言領域（Private）"
    Private Shared iniInfoAry() As RecDataStructure.DefineInfo
    Private Const DATA_KIND As String = "54"   'データ種別
    '----------- 0.1  北陸・項目拡張対応   ADD  START------------------------
    Private Shared iniInfoOldAry() As RecDataStructure.DefineInfo
    Private Shared dataLen As Integer
    Private Const OldLen As Integer = 672
    Private Const NewLen As Integer = 864
    '----------- 0.1  北陸・項目拡張対応   ADD    END------------------------
#End Region

#Region "メソッド（Main）"
    ''' <summary>
    ''' 監視盤設定データ登録プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 監視盤設定データプロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForKsbConfig")
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

                Log.Init(sLogBasePath, "ForKsbConfig")
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

                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "KansibanSetInfo_001", iniInfoAry) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If
                '----------- 0.1  北陸・項目拡張対応   ADD  START------------------------
                If DefineInfoShutoku.GetDefineInfo(Config.FormatOldFilePath, "KansibanSetInfo_001", iniInfoOldAry) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If
                '----------- 0.1  北陸・項目拡張対応   ADD    END------------------------

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


    ''' <summary>
    '''  監視盤設定データ取込
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
            Dim dt As DataTable = Nothing
            '----------- 0.1  北陸・項目拡張対応   MOD  START------------------------
            Dim fileStream As FileStream
            Try
                'ファイルストリームを取得
                fileStream = New FileStream(sFilePath, FileMode.Open)
                dataLen = CInt(fileStream.Length) - 1
            Catch ex As Exception
                'ファイルストリームを解放
                Log.Fatal("Unwelcome Exception caught.", ex)
                Return RecordingResult.IOError
            End Try
            fileStream.Close()
            'ファイルサイズが１レコード分に満たない場合
            If dataLen = NewLen Then
                'datファイルデータ取得
                If GetInfoFromDataFileComm(iniInfoAry, sFilePath, sModelCode, dlineInfoLst) = False Then
                    Return RecordingResult.ParseError
                End If
                'チェック
                If CheckData(iniInfoAry, dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If
                'DB登録
                If BatchAppComm.PutDataToDBCommon(iniInfoAry, dlineInfoLstNew, "D_KSB_CONFIG") = False Then
                    Return RecordingResult.IOError
                End If
            ElseIf dataLen = OldLen Then
                'datファイルデータ取得
                If GetInfoFromDataFileComm(iniInfoOldAry, sFilePath, sModelCode, dlineInfoLst) = False Then
                    Return RecordingResult.ParseError
                End If
                'チェック
                If CheckData(iniInfoOldAry, dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If
                'DB登録
                If BatchAppComm.PutDataToDBCommon(iniInfoOldAry, dlineInfoLstNew, "D_KSB_CONFIG") = False Then
                    Return RecordingResult.IOError
                End If
            Else
                Return RecordingResult.IOError
            End If
            '----------- 0.1  北陸・項目拡張対応   MOD    END------------------------
            '成功した場合
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(DATA_KIND, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function
#End Region

#Region "メソッド（Private）"

    ''' <summary>
    ''' DATファイルの解析
    ''' </summary>
    ''' <param name="iniInfoAry">INIファイルの内容</param>
    ''' <param name="datFileName">datファイル名</param>
    ''' <param name="clientKind">データIndex</param>
    ''' <param name="lineInfoLst">解析したデータ</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Private Shared Function GetInfoFromDataFileComm(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByVal datFileName As String, _
                                                ByVal clientKind As String, _
                                                ByRef lineInfoLst As List(Of String())) As Boolean
        'データIndex
        Dim dataIndex As String = Nothing
        Dim info(iniInfoAry.Length - 1) As String
        Dim uNoName As String = Nothing '号機番号
        'ヘッド部
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        Dim lineInfo() As String
        '全部レコード
        Dim lineInfoLstOld As New List(Of String())
        If lineInfoLst Is Nothing Then
            lineInfoLst = New List(Of String())
        Else
            lineInfoLst.Clear()
        End If
        '----------- 0.1  北陸・項目拡張対応   MOD  START------------------------
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, datFileName, clientKind, dataLen, 1, lineInfoLstOld, DATA_KIND) = False Then
            Return False
        End If
        '----------- 0.1  北陸・項目拡張対応   MOD  START------------------------

        If lineInfoLstOld.Count <= 0 Then
            Return True
        End If

        'ファイル名の解析
        If GetBaseInfo(headInfo, datFileName) = False Then
            Return False
        End If

        ReDim lineInfo(iniInfoAry.Length - 1)
        For Each lineInfo In lineInfoLstOld

            For k As Integer = 0 To iniInfoAry.Length - 1
                If iniInfoAry(k).FIELD_NAME = "UNIT_NO" Then
                    dataIndex = lineInfo(k)  '各エリアの号機番号
                    uNoName = iniInfoAry(k).KOMOKU_NAME   '号機
                    Exit For
                End If
            Next

            For i As Integer = 0 To dataIndex.Length - 1 Step 2
                If dataIndex.Substring(i, 2) = "00" Then  '無効データ
                    Continue For
                End If
                ReDim info(iniInfoAry.Length - 1)
                For j As Integer = 0 To iniInfoAry.Length - 1
                    Select Case UCase(iniInfoAry(j).FIELD_NAME)
                        Case "RAIL_SECTION_CODE"
                            info(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE   '線区コード
                            Continue For
                        Case "STATION_ORDER_CODE"
                            info(j) = headInfo.STATION_CODE.STATION_ORDER_CODE    '駅順コード
                            Continue For
                        Case "CORNER_CODE"
                            info(j) = headInfo.CORNER_CODE    'コーナー
                            Continue For
                        Case "SYUSYU_DATE"
                            info(j) = headInfo.PROCESSING_TIME   '収集日時
                            Continue For
                        Case "MODEL_CODE" '機種
                            info(j) = "G"
                            Continue For
                    End Select
                    If CInt(iniInfoAry(j).PARA6) = 1 AndAlso
                        lineInfo(j).Length >= i + 2 Then
                        info(j) = lineInfo(j).Substring(i, 2) '号機別ステータス
                        If iniInfoAry(j).FIELD_NAME = "UNIT_NO" Then  '号機番号
                            If (OPMGUtility.checkNumber(info(j)) = True AndAlso CInt(info(j)) >= 10) _
                               OrElse OPMGUtility.checkNumber(info(j)) = False Then
                                Dim uNo As String = "&H" & info(j)
                                info(j) = CInt(uNo).ToString
                            End If
                        End If
                    ElseIf CInt(iniInfoAry(j).PARA6) = 2 AndAlso
                       lineInfo(j).Length >= i + 2 Then
                        info(j) = lineInfo(j).Substring(i + 1, 1) '通路設定
                    ElseIf CInt(iniInfoAry(j).PARA6) = 3 AndAlso
                        lineInfo(j).Length >= i + 2 Then
                        info(j) = lineInfo(j).Substring(i, 1) '通路
                    Else
                        info(j) = lineInfo(j)
                    End If
                Next
                lineInfoLst.Add(info)
            Next
        Next
        If lineInfoLst.Count <= 0 Then
            Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, "1", uNoName))

            '収集データの登録
            BatchAppComm.SetCollectionData(datFileName, DATA_KIND)
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' 基本ヘッダ部情報の解析
    ''' </summary>
    ''' <param name="infoObj">解析した結果を保存用</param> 
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Private Shared Function GetBaseInfo(ByRef infoObj As BaseInfo, _
                                       ByVal fileName As String) As Boolean
        Try
            Dim code As EkCode = UpboundDataPath.GetEkCode(fileName)
            '線区
            infoObj.STATION_CODE.RAIL_SECTION_CODE = code.RailSection.ToString("D3")
            '駅順
            infoObj.STATION_CODE.STATION_ORDER_CODE = code.StationOrder.ToString("D3")
            'コーナー
            infoObj.CORNER_CODE = code.Corner.ToString("D4")
            '処理日時
            infoObj.PROCESSING_TIME = UpboundDataPath.GetTimestamp(fileName).ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function


    ''' <summary>
    ''' 監視盤設定データのチェック
    ''' </summary>
    ''' <param name="iniInfoAry">iniファイル</param>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dlineInfoLstNew">チェック後、正確的datファイル内容</param>
    ''' <param name="datFileName">ファイル名</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Private Shared Function CheckData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                     ByVal dlineInfoLst As List(Of String()), _
                                     ByRef dlineInfoLstNew As List(Of String()), _
                                     ByVal datFileName As String) As Boolean

        If dlineInfoLst.Count <= 0 Then Return True

        Dim lineInfo(iniInfoAry.Length) As String '1レコード
        Dim iFlag As Integer = 145
        If dlineInfoLstNew Is Nothing Then
            dlineInfoLstNew = New List(Of String())
        Else
            dlineInfoLstNew.Clear()
        End If

        For j As Integer = 0 To dlineInfoLst.Count - 1

            '1レコード取得
            lineInfo = dlineInfoLst.Item(j)

            '全部フィールド
            For i As Integer = 0 To iniInfoAry.Length - 1

                If iFlag = 0 Then Exit For

                Select Case UCase(iniInfoAry(i).FIELD_NAME)
                    Case "DATA_KIND" 'データ種別
                        If (Not lineInfo(i) = DATA_KIND) Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, "データ種別"))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '収集データの登録
                            Return False
                        End If
                End Select
                '入場フリー設定１～入場フリー設定９、日跨り出場フリー設定１～日跨り出場フリー設定９
                '改札機自動設定ON、改札機自動設定OFF
                Select Case UCase(iniInfoAry(i).PARA6)

                    Case "5"    '開始　年           
                        If Integer.Parse(lineInfo(i)) > 99 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '収集データの登録
                            Return False
                        End If
                    Case "6"    '開始　月
                        If Integer.Parse(lineInfo(i)) > 12 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '収集データの登録
                            Return False
                        End If
                    Case "7"    '開始　日
                        If Integer.Parse(lineInfo(i)) > 31 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '収集データの登録
                            Return False
                        End If
                    Case "8"    '開始　時、設定（時）
                        If Integer.Parse(lineInfo(i)) > 23 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '収集データの登録
                            Return False
                        End If
                    Case "9"    '設定（分）
                        If Integer.Parse(lineInfo(i)) > 59 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '収集データの登録
                            Return False
                        End If
                End Select
            Next
            '共通のチェック
            If BatchAppComm.CheckDataComm(j + 1, iniInfoAry, lineInfo, datFileName, True, False, True) = False Then
                Continue For
            End If
            dlineInfoLstNew.Add(lineInfo)
        Next

        Return True
    End Function
#End Region
End Class
