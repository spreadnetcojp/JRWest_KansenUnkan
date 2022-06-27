' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/11/07  (NES)河脇  フェーズ２対応
'                                   ・SNMPTrap対象及びメール対象対応
'   0.2      2014/06/01       金沢  北陸異常メール対応
'   0.3      2014/06/01  (NES)河脇  異常メール詳細追加対応
'   0.4      2017/04/10  (NES)小林  次世代車補対応にて、メール文言の
'                                   号機番号をIntegerで入力する（差し替え
'                                   時に書式を指定可能とする）ように統一
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Text
Imports System.Threading
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp

''' <summary>
''' 異常データのDB登録
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "宣言領域（Private）"
    Private Structure MailSource
        Public RailSectionCode As String
        Public StationOrderCode As String
        Public CornerCode As String
        Public ModelCode As String
        Public UnitNo As Integer
        Public OccurDate As String
        Public ErrCode As String
        Public ErrItem As String
        'Ver0.3 ADD
        Public Dtl_Info As String
        'Ver0.3 MOD
        Public Sub New( _
           ByVal sRailSectionCode As String, _
           ByVal sStationOrderCode As String, _
           ByVal sCornerCode As String, _
           ByVal sModelCode As String, _
           ByVal unitNo As Integer, _
           ByVal sOccurDate As String, _
           ByVal sErrCode As String, _
           ByVal sErrItem As String, _
           ByVal sDtl_Info As String)
            Me.RailSectionCode = sRailSectionCode
            Me.StationOrderCode = sStationOrderCode
            Me.CornerCode = sCornerCode
            Me.ModelCode = sModelCode
            Me.UnitNo = unitNo
            Me.OccurDate = sOccurDate
            Me.ErrCode = sErrCode
            Me.ErrItem = sErrItem
            'Ver0.3 ADD
            Me.Dtl_Info = sDtl_Info
        End Sub
    End Structure
    Private Shared iniInfoAry() As RecDataStructure.DefineInfo
    Private Shared mailStartMinutesInDay As Integer
    Private Shared mailEndMinutesInDay As Integer
    Private Shared oMailQueue As Queue(Of MailSource)
    Private Shared oMailEvent As ManualResetEvent
    'Ver0.1 ADD
    Private Shared isFtpData As Boolean     'Ftpデータ有無
#End Region

#Region "メソッド（Main）"
    ''' <summary>
    ''' 異常データ取込プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 異常データ取込プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForFaultData")
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

                Log.Init(sLogBasePath, "ForFaultData")
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

                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "FaultData_001", iniInfoAry) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                If Not Config.MailSmtpServerName.Equals("") Then
                    '有効時間帯の開始時刻と終了時刻を（0時0分からの経過分の形式で）
                    'mailStartMinutesInDayとmailEndMinutesInDayに算出しておく。
                    'その際、mailStartMinutesInDay <= mailEndMinutesInDayになるよう、
                    '必要に応じてmailEndMinutesInDayには補正をかけておく。
                    'NOTE: mailStartMinutesInDay == mailEndMinutesInDayは正当な設定
                    'であり、有効時間帯がその１分間だけであることを意味する。
                    mailStartMinutesInDay = Config.MailStartHour * 60 + Config.MailStartMinute
                    mailEndMinutesInDay = Config.MailEndHour * 60 + Config.MailEndMinute
                    If mailStartMinutesInDay > mailEndMinutesInDay Then
                        mailEndMinutesInDay += 24 * 60
                    End If

                    'NOTE: oMailEventの参照先オブジェクトが不要になるのは、
                    'どのみちプロセスが終了するときである。よって、
                    'そのDisposeは、その際のガーベージコレクションに委ねる。
                    oMailQueue = New Queue(Of MailSource)
                    oMailEvent = New ManualResetEvent(False)
                    Dim oMailerThread As New Thread(AddressOf MainClass.MailingLoop)
                    oMailerThread.IsBackground = True
                    oMailerThread.Name = "Mailer"
                    oMailerThread.Start()
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
    '''  異常データ取込
    ''' </summary>
    ''' <param name="sFilePath">登録するべきデータが格納されたファイルの絶対パス名</param>
    ''' <returns>登録の結果</returns>
    ''' <remarks>
    ''' データ登録スレッドで呼び出される。
    ''' </remarks>
    Private Shared Function RecordToDatabase(ByVal sFilePath As String) As RecordingResult
        Dim modelCode As Integer = UpboundDataPath.GetEkCode(sFilePath).Model
        Dim sModelCode As String = Format(modelCode, "00")
        Dim dlineInfoLst As List(Of String()) = Nothing
        Dim dlineInfoLstNew As List(Of String()) = Nothing
        Dim recResultFinal As RecordingResult
        Try

            'datファイルデータ取得
            If GetInfoFromDataFileComm(sFilePath, sModelCode, dlineInfoLst) = False Then
                Return RecordingResult.ParseError
            End If

            'チェック
            If CheckData(dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                'NOTE: 現在のところここが実行されることはあり得ないが、
                'CheckDataメソッドがDBを参照するように改造されれば、
                'ここが実行されることもあるはず。そして、ここが実行されるのは、
                'データの書式に不正がある場合ではなく、DBのアクセスなどに
                '失敗した（ランタイムの異常が発生した）場合である。
                'その場合、メールの送信やSNMP通知を行える可能性もあるが、
                '行えない可能性の方が高い（それらの場合もDBを参照する）
                '上、どのみちdlineInfoLstNewには全ての正常レコードが登録
                'されていない（もしくは、不正なレコードが残っている）かも
                'しれない。よって、メールの送信やSNMP通知は諦めて、
                'ここでメソッドを終了することにする（実際、DBへの登録も
                '諦めているので、それは一貫性のあるつくりである）。
                Return RecordingResult.IOError
            End If

            'DB登録
            If BatchAppComm.PutDataToDBCommon(iniInfoAry, dlineInfoLstNew, "D_FAULT_DATA") = True Then
                recResultFinal = RecordingResult.Success
            Else
                recResultFinal = RecordingResult.IOError
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen("A6", Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try

        'Ver0.1 ADD 電文データのときのみ
        If Not isFtpData Then
            'メールやSNMPによる通知
            MailOrTrapIfNeeded(sModelCode, iniInfoAry, dlineInfoLstNew)
            'Ver0.1 ADD
        End If

        Return recResultFinal
    End Function
#End Region

#Region "メソッド（Private）"
    ''' <summary>
    ''' DATファイルの解析
    ''' </summary>
    ''' <param name="datFileName">datファイル名</param>
    ''' <param name="clientKind"></param>
    '''  <param name="lineInfoLst">解析したデータ</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Private Shared Function GetInfoFromDataFileComm(ByVal datFileName As String, _
                                                ByVal clientKind As String, _
                                                ByRef lineInfoLst As List(Of String())) As Boolean
        'Ver0.1 MOD パラメータ(isFtpData)追加
        'If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, datFileName, clientKind, 780, 17, lineInfoLst, "A6") = False Then
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, datFileName, clientKind, 780, 17, lineInfoLst, "A6", isFtpData) = False Then
            Return False
        End If

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim jIndex As Integer = 0
        Dim iFlag As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim iUnitNoW As Integer = 0 '監視盤場合
        Dim iUnitNoG As Integer = 0 '監視盤以外場合

        '全部レコード
        For i = 0 To lineInfoLst.Count - 1

            '1レコード取得
            lineInfo = lineInfoLst.Item(i)

            iFlag = 2
            '全部フィールド
            For j = 0 To iniInfoAry.Length - 1
                Select Case iniInfoAry(j).FIELD_NAME
                    Case "UNIT_NO" '号機
                        iFlag = iFlag - 1
                        jIndex = j
                        iUnitNoW = Integer.Parse(lineInfo(j))
                    Case "UNIT_NOG" '号機番号
                        iFlag = iFlag - 1
                        iUnitNoG = Integer.Parse(lineInfo(j))
                End Select
                If iFlag = 0 Then Exit For
            Next

            If iUnitNoW = 0 Then '監視盤以外場合
                lineInfo(jIndex) = iUnitNoG.ToString
            End If
        Next

        Return True

    End Function

    ''' <summary>
    ''' 異常データのチェック
    ''' </summary>
    ''' <param name="dlineInfoLst">datファイル内容</param>
    ''' <param name="dlineInfoLstNew">チェック後、正確的datファイル内容</param>
    ''' <returns>正常:TRUE／異常:FALSE</returns>
    ''' <remarks>バイナリファイルの基本ヘッダ部を解析する</remarks>
    Private Shared Function CheckData(ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String) As Boolean

        Dim iFlag As Integer = 0

        Dim i As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim lineInfoNew(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:エラーがある;false:エラーがない

        dlineInfoLstNew = New List(Of String())

        '全部レコード
        For i = 0 To dlineInfoLst.Count - 1

            '1レコード取得
            lineInfo = dlineInfoLst.Item(i)

            '初期化
            isHaveErr = False
            iFlag = 2

            '全部フィールド
            For j As Integer = 0 To iniInfoAry.Length - 1

                If iFlag = 0 Then Exit For

                Select Case iniInfoAry(j).FIELD_NAME
                    Case "DATA_KIND" 'データ種別
                        iFlag = iFlag - 1
                        If (Not lineInfo(j) = "A6") AndAlso (Not lineInfo(j) = "C3") Then
                            isHaveErr = True
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (i + 1).ToString, "データ種別"))
                            BatchAppComm.SetCollectionData(datFileName, "A6") 'ファイル名解析
                            Exit For
                        End If
                    Case "ERR_CODE" 'エラーコード
                        iFlag = iFlag - 1
                        If lineInfo(j) = "000000" Then
                            isHaveErr = True
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, (i + 1).ToString, iniInfoAry(j).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo)
                            Exit For
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

    ''' <summary>
    ''' SNMP通知の実施およびメール送信スレッドへの異常データメール送信依頼を行う。
    ''' </summary>
    ''' <param name="sModelNumber">プロトコル形式の機種コード</param>
    ''' <param name="iniInfoAry">INIファイル内容</param>
    ''' <param name="dlineInfoLst">データ</param>
    Public Shared Sub MailOrTrapIfNeeded(ByVal sModelNumber As String, _
                                         ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                         ByVal dlineInfoLst As List(Of String()))

        Dim portNumber As Integer = BatchAppComm.GetPortNumber(sModelNumber)
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            For i As Integer = 0 To dlineInfoLst.Count - 1
                Dim sRailSectionCode As String = Nothing
                Dim sStationOrderCode As String = Nothing
                Dim sCornerCode As String = Nothing
                Dim unitNo As Integer
                Dim sOccurDate As String = Nothing
                Dim sModelCode As String = Nothing
                Dim sErrType As String = Nothing
                Dim sActStep As String = Nothing
                Dim sErrCode As String = Nothing
                Dim sErrItem As String = ""
                'Ver0.3 ADD
                Dim sDtl_Info As String = ""
                For j As Integer = 0 To iniInfoAry.Length - 1
                    'OPT: iniInfoAryの情報源は運管サーバ自身の定義ファイルなので、
                    '大文字化は不要である。定義ファイルをケースインセンシティブに
                    'したいのであれば、何度もやらずにGetDefineInfo()の中で
                    '一度だけ実施しておけばよい。
                    Select Case iniInfoAry(j).FIELD_NAME.ToUpperInvariant()
                        Case "RAIL_SECTION_CODE"
                            sRailSectionCode = dlineInfoLst.Item(i)(j)
                        Case "STATION_ORDER_CODE"
                            sStationOrderCode = dlineInfoLst.Item(i)(j)
                        Case "CORNER_CODE"
                            sCornerCode = dlineInfoLst.Item(i)(j)
                        Case "UNIT_NO"
                            unitNo = Integer.Parse(dlineInfoLst.Item(i)(j))
                        Case "OCCUR_DATE"
                            sOccurDate = dlineInfoLst.Item(i)(j)
                        Case "MODEL_CODE"
                            sModelCode = dlineInfoLst.Item(i)(j)
                        Case "ERROR_TYPE"
                            sErrType = dlineInfoLst.Item(i)(j)
                        Case "ACT_STEP"
                            sActStep = dlineInfoLst.Item(i)(j)
                        Case "ERR_CODE"
                            sErrCode = dlineInfoLst.Item(i)(j)
                        Case "ERR_ITEM"
                            sErrItem = dlineInfoLst.Item(i)(j)
                            'Ver0.3 ADD START
                        Case "DTL_INFO"
                            sDtl_Info = dlineInfoLst.Item(i)(j)
                            'Ver0.3 ADD END
                    End Select
                Next
                '----------- 0.2  北陸異常メール通知対応   MOD  START------------------------
                Dim sSQL As String = _
                   "SELECT SNMP_SEVERITY" _
                   & " FROM M_NOTIFIABLE_ERR_CODE" _
                   & " WHERE ERR_CODE = '" & sErrCode & "'" _
                   & " AND MODEL_CODE = '" & sModelCode & "'" _
                   & " AND ((STATION_ORDER_CODE = '999' AND RAIL_SECTION_CODE = '999')" _
                    & " OR (STATION_ORDER_CODE = '999' AND RAIL_SECTION_CODE = '" & sRailSectionCode & "')" _
                    & " OR (STATION_ORDER_CODE = '" & sStationOrderCode & "' AND RAIL_SECTION_CODE = '" & sRailSectionCode & "'))"
                '----------- 0.2  北陸異常メール通知対応   MOD  　　END------------------------
                Dim oSeverity As Object = dbCtl.ExecuteSQLToReadScalar(sSQL)
                If oSeverity IsNot Nothing Then
                    'NOTE: sModelは、M_NOTIFIABLE_ERR_CODEテーブルに登録されていることから、
                    'SnmpAppNumberForWarningFaultOfModelsやSnmpAppNumberForCriticalFaultOfModelsに
                    '登録されている機種であるものとみなす。
                    Dim appNumber As Integer = 0
                    Select Case CStr(oSeverity)
                        Case DbConstants.SnmpSeverityWarning
                            appNumber = Config.SnmpAppNumberForWarningFaultOfModels(sModelCode)
                        Case DbConstants.SnmpSeverityCritical
                            appNumber = Config.SnmpAppNumberForCriticalFaultOfModels(sModelCode)
                    End Select

                    If appNumber <> 0 Then
                        'TODO: 異常データの発生源が窓処の場合に、sErrTypeとsActStepが0になることを
                        '前提にしている。もし、そうでないなら、SNMP_CALLライブラリの仕様に合わせて
                        '最後の引数は、窓処用に特別に編集しなければならない。
                        SnmpTrap.Act( _
                           appNumber, _
                           sModelCode, _
                           sRailSectionCode, _
                           sStationOrderCode, _
                           sCornerCode, _
                           unitNo, _
                           portNumber, _
                           sErrType & sActStep & sErrCode)
                    End If

                    If Not Config.MailSmtpServerName.Equals("") Then
                        'mailStartMinutesInDay以上になるように補正した
                        '現在時刻を（0時0分からの経過分の形式で）求める。
                        Dim now As DateTime = DateTime.Now
                        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
                        If mailStartMinutesInDay > nowMinutesInDay Then
                            nowMinutesInDay += 24 * 60
                        End If

                        '有効時間帯の場合のみメール送信を行う。
                        If nowMinutesInDay <= mailEndMinutesInDay Then
                            'Ver0.3 MOD
                            Dim mailSource As New MailSource(sRailSectionCode, sStationOrderCode, sCornerCode, sModelCode, unitNo, sOccurDate, sErrCode, sErrItem, sDtl_Info)
                            SyncLock oMailQueue
                                oMailQueue.Enqueue(mailSource)
                            End SyncLock
                            oMailEvent.Set()
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try

    End Sub

    ''' <summary>
    ''' メール送信スレッドのメインループ。
    ''' </summary>
    Private Shared Sub MailingLoop()

        Dim oSmtpClient As New SmtpClient()
        oSmtpClient.Host = Config.MailSmtpServerName
        oSmtpClient.Port = Config.MailSmtpPort
        oSmtpClient.Credentials = New NetworkCredential(Config.MailSmtpUserName, Config.MailSmtpPassword)
        oSmtpClient.Timeout = Config.MailSendLimitTicks

        Do
            oMailEvent.WaitOne()

            'NOTE: oMailEvent.Reset()の後でキューイングされているものを
            '全て処理するため、このタイミングで新たなものをキューイング
            'され、oMailEvent.Set()されても構わない。
            oMailEvent.Reset()
            Do
                Dim isSendFailedOnce As Boolean = False

                Dim mailSource As MailSource
                SyncLock oMailQueue
                    If oMailQueue.Count = 0 Then Exit Do
                    mailSource = oMailQueue.Dequeue()
                End SyncLock

                Using oMail As New MailMessage()
                    Dim sMailBody As String
                    Try
                        'メールヘッダのFROMや宛先を編集。
                        oMail.From =  New MailAddress(Config.MailFromAddr)
                        For i As Integer = 0 To Config.MailToAddrs.Length - 1
                            If Not String.IsNullOrEmpty(Config.MailToAddrs(i)) Then
                                 oMail.To.Add(Config.MailToAddrs(i))
                            End If
                        Next
                        For i As Integer = 0 To Config.MailCcAddrs.Length - 1
                            If Not String.IsNullOrEmpty(Config.MailCcAddrs(i)) Then
                                 oMail.CC.Add(Config.MailCcAddrs(i))
                            End If
                        Next
                        For i As Integer = 0 To Config.MailBccAddrs.Length - 1
                            If Not String.IsNullOrEmpty(Config.MailBccAddrs(i)) Then
                                 oMail.Bcc.Add(Config.MailBccAddrs(i))
                            End If
                        Next

                        'メールの件名を編集。
                        Dim sStation As String
                        Dim sCorner As String
                        Dim sModel As String
                        Dim table As DataTable = SelectNames(mailSource.RailSectionCode, mailSource.StationOrderCode, mailSource.CornerCode, mailSource.ModelCode)
                        If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                            sStation = table.Rows(0).Field(Of String)("STATION_NAME")
                            sCorner = table.Rows(0).Field(Of String)("CORNER_NAME")
                            sModel = table.Rows(0).Field(Of String)("MODEL_NAME")
                        Else
                            sStation = mailSource.RailSectionCode & mailSource.StationOrderCode
                            sCorner = mailSource.CornerCode
                            sModel = mailSource.ModelCode
                        End If
                        Dim oSubjectEncoding As Encoding = Encoding.GetEncoding(Config.MailSubjectEncoding)
                        oMail.Subject = String.Format( _
                           "=?{0}?B?{1}?=", _
                           oSubjectEncoding.BodyName, _
                           Convert.ToBase64String(oSubjectEncoding.GetBytes(Lexis.FaultDataMailSubject.Gen(sStation, sCorner, sModel, mailSource.UnitNo)), Base64FormattingOptions.None))

                        'メール本文を編集。
                        Dim occurDate As DateTime = DateTime.ParseExact(mailSource.OccurDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                        'Ver0.3 MOD START
                        If mailSource.ModelCode = "G" Then
                            sMailBody = Lexis.FaultDataMailBody.Gen(occurDate.ToString(Lexis.DateTimeFormatInFaultDataMailBody.Gen()), mailSource.ErrCode, mailSource.ErrItem & vbCrLf & mailSource.Dtl_Info)
                        Else
                            sMailBody = Lexis.FaultDataMailBody.Gen(occurDate.ToString(Lexis.DateTimeFormatInFaultDataMailBody.Gen()), mailSource.ErrCode, mailSource.ErrItem)
                        End If
                        'Ver0.3 MOD END
                        Dim oAltView As AlternateView = _
                           AlternateView.CreateAlternateViewFromString( _
                              sMailBody, _
                              Encoding.GetEncoding(Config.MailBodyEncoding), _
                              MediaTypeNames.Text.Plain)
                        oAltView.TransferEncoding = Config.MailTransferEncoding
                        oMail.AlternateViews.Add(oAltView)
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                        Continue Do
                    End Try

                    If Config.MailSendFailureSpreads AndAlso isSendFailedOnce Then
                        Log.Error("送信待ちメールの送信を中止しました。")
                        Log.Error("送信中止メール内容:" & vbCrLf & oMail.Subject & vbCrLf & sMailBody)
                        Continue Do
                    End If

                    Try
                        'メールを送信。
                        oSmtpClient.Send(oMail)
                        Log.Info("メールを送信しました。")
                    Catch ex As SmtpFailedRecipientsException
                        Log.Error("Exception caught.", ex)
                        Log.Error("送信失敗メール内容:" & vbCrLf & oMail.Subject & vbCrLf & sMailBody)
                    Catch ex As SmtpException
                        Log.Error("Exception caught.", ex)
                        Log.Error("送信失敗メール内容:" & vbCrLf & oMail.Subject & vbCrLf & sMailBody)
                        isSendFailedOnce = True
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                    End Try
                End Using
            Loop
        Loop

    End Sub

    Private Shared Function SelectNames(ByVal sRailSectionCode As String, _
                                        ByVal sStationOrderCode As String, _
                                        ByVal sCornerCode As String, _
                                        ByVal sModelCode As String) As DataTable

        Dim sSQL As String = _
           "SELECT TOP 1 STATION_NAME, CORNER_NAME, MODEL_NAME" _
           & " FROM M_MACHINE" _
           & " WHERE RAIL_SECTION_CODE = '" & sRailSectionCode & "'" _
           & " AND STATION_ORDER_CODE = '" & sStationOrderCode & "'" _
           & " AND CORNER_CODE = " & sCornerCode _
           & " AND MODEL_CODE = '" & sModelCode & "'" _
           & " AND SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & EkServiceDate.GenString() & "')"

        Dim table As DataTable = Nothing
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            table = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            dbCtl.ConnectClose()
        End Try
        Return table

    End Function
#End Region

End Class
