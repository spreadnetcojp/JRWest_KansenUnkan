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
Imports System.Diagnostics
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports System.Text
Imports JR.ExOpmg.ServerApp

''' <summary>
''' 別集札データ登録プロセス共通のメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "宣言領域（Private）"

    Private Const DataLength As Integer = 111              'データ桁数
    Private Const HeadLength As Integer = 17                 'ヘッダ桁数
    Private Const DATA_KIND As String = "A1"                 'データ種別

    Private Shared iniInfoAry() As RecDataStructure.DefineInfo

#End Region

#Region "メソッド（Main）"
    ''' <summary>
    '''  別集札データ登録プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    '''  別集札データ登録プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForBesshuData")
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

                Log.Init(sLogBasePath, "ForBesshuData")
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

                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "BesshuData_001", iniInfoAry) = False Then
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

    ''' <summary>
    '''   別集札データ取込
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

            Dim dlineInfoLst As New List(Of String())
            Dim dlineInfoLstNew As New List(Of String())

            'datファイルデータ取得
            If GetInfoFromDataFile(sFilePath, sModelCode, DATA_KIND, dlineInfoLst) = False Then
                Return RecordingResult.ParseError
            End If
            'チェック
            If CheckData(dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                Return RecordingResult.IOError
            End If
            'DB登録
            If BatchAppComm.PutDataToDBCommon(iniInfoAry, dlineInfoLstNew, "D_BESSHU_DATA") = False Then
                Return RecordingResult.IOError
            End If

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

#Region "メソッド（Public）"

    ''' <summary>
    ''' DATファイルの解析
    ''' </summary>
    ''' <param name="sFilePath">登録するべきデータが格納されたファイルの絶対パス名</param>
    ''' <param name="sModelCode">機種コード</param>
    ''' <param name="sDataKind">データ種別</param>
    ''' <param name="lineInfoLst">取得した情報</param>
    ''' <returns>True:正常/False:異常</returns>
    Public Shared Function GetInfoFromDataFile(ByVal sFilePath As String, _
                                               ByVal sModelCode As String, _
                                               ByVal sDataKind As String, _
                                               ByRef lineInfoLst As List(Of String())) As Boolean
        Dim info() As String                            '１レコード
        Dim isWtn As Boolean = False
        Dim nWtn As Integer = 0
        Dim nCtn As Integer = 0
        Dim ticketCnt As String = "" '放出枚数
        Dim nTnt As Integer = 0   '行目

        '全部レコード
        Dim lineInfoLstOld As New List(Of String())
        If lineInfoLst Is Nothing Then
            lineInfoLst = New List(Of String())
        Else
            lineInfoLst.Clear()
        End If

        'データを取得する。
        lineInfoLst = New List(Of String())
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, sFilePath, sModelCode, _
                                                   DataLength, HeadLength, lineInfoLstOld, sDataKind) = False Then
            Return False
        End If

        For i As Integer = 0 To lineInfoLstOld.Count - 1
            isWtn = False
            nWtn = 0
            For j As Integer = 0 To iniInfoAry.Length - 1
                '放出枚数を設定する
                If iniInfoAry(j).FIELD_NAME = "TICKET_CNT" Then
                    If isWtn = False Then
                        nWtn = j
                        isWtn = True
                    End If
                    ticketCnt = lineInfoLstOld(i)(j)
                    If OPMGUtility.checkNumber(ticketCnt) = False Then
                        Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, CStr(i + 1), iniInfoAry(j).KOMOKU_NAME))
                        BatchAppComm.SetCollectionData(sFilePath, DATA_KIND) 'ファイル名解析
                        Exit For
                    End If
                    If Integer.Parse(ticketCnt) = 0 Then
                        Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, CStr(i + 1), iniInfoAry(j).KOMOKU_NAME))
                        BatchAppComm.SetCollectionData(iniInfoAry, lineInfoLstOld(i)) 'ファイル名解析
                        Exit For
                    ElseIf CInt(ticketCnt) > 4 Then
                        ticketCnt = CStr(4)
                    End If
                    nTnt = 1
                End If

                '1行目
                If iniInfoAry(j).FIELD_NAME = "TICKET_NO" Then
                    '放出枚数
                    If nTnt > CInt(ticketCnt) Then
                        Exit For
                    Else
                        'クリア
                        ReDim info(nWtn + 5)
                        '基本ヘッダを設定する
                        For k As Integer = 0 To nWtn - 1
                            info(k) = lineInfoLstOld(i)(k)
                        Next
                        '枚目を設定する
                        For nCtn = nWtn To nWtn + 5
                            '放出枚数
                            If iniInfoAry(nCtn).FIELD_NAME = "TICKET_CNT" Then
                                info(nCtn) = CStr(nTnt)
                                Continue For
                            End If
                            '行目
                            If iniInfoAry(nCtn).FIELD_NAME = "BESSYU_CNT" Then
                                info(nCtn) = CStr(i + 1)
                                Continue For
                            End If
                            info(nCtn) = lineInfoLstOld(i)((nTnt - 1) * 6 + nCtn)
                        Next
                        lineInfoLst.Add(info)
                        nTnt += 1
                    End If
                End If
            Next
        Next
        Return True
    End Function

    ''' <summary>
    ''' 別集札データのチェック
    ''' </summary>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dlineInfoLstNew">チェック後、正確的datファイル内容</param>
    '''  <param name="datFileName">データFileName</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Public Shared Function CheckData(ByVal dlineInfoLst As List(Of String()), _
                                     ByRef dlineInfoLstNew As List(Of String()), _
                                     ByVal datFileName As String) As Boolean

        Dim bRtn As Boolean = True
        Dim i As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False      'true:エラーがある;false:エラーがない
        Dim intData As Integer = 0
        dlineInfoLstNew = New List(Of String())

        '全部レコード
        For i = 0 To dlineInfoLst.Count - 1

            '1レコード取得
            lineInfo = dlineInfoLst.Item(i)

            isHaveErr = False

            For j As Integer = 0 To iniInfoAry.Length - 1

                Select Case iniInfoAry(j).FIELD_NAME
                    Case "DATA_KIND" 'データ種別
                        If (Not lineInfo(j) = DATA_KIND) Then
                            isHaveErr = True
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (i + 1).ToString, "データ種別"))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) 'ファイル名解析
                            Exit For
                        End If
                    Case "TICKET"  '投入枚数
                        If Integer.TryParse(lineInfo(j), intData) = False Then
                            lineInfo(i) = CStr(0)
                            Continue For
                        End If
                End Select
            Next

            If isHaveErr = False Then
                '共通のチェック
                If BatchAppComm.CheckDataComm(i + 1, iniInfoAry, lineInfo, datFileName) = False Then
                    Continue For
                End If

                dlineInfoLstNew.Add(lineInfo)
            End If
        Next
        Return True

    End Function

#End Region

End Class
