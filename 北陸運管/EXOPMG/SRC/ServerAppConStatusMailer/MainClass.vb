' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2013/12/10  (NES)小林  統括状態情報の追加対応
'   0.2      2017/04/10  (NES)小林  次世代車補対応にて、メール文言の
'                                   号機番号をIntegerで入力する（差し替え
'                                   時に書式を指定可能とする）ように統一
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' 機器接続状態メール生成プロセスのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass
    Inherits ServerAppBaseMainClass

#Region "定数や変数"
    'メインウィンドウ
    Private Shared oMainForm As ServerAppForm

    '実作業スレッドへの終了要求フラグ
    Private Shared quitWorker As Integer
#End Region

    ''' <summary>
    ''' 機器接続状態メール生成プロセスのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 機器接続状態メール生成プロセスのエントリポイントである。
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppConStatusMailer")
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

                Log.Init(sLogBasePath, "ConStatusMailer")
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

                'メッセージループがアイドル状態になる前（かつ、定期的にそれを行う
                'スレッドを起動する前）に、生存証明ファイルを更新しておく。
                Directory.CreateDirectory(Config.ResidentAppPulseDirPath)
                ServerAppPulser.Pulse()

                oMainForm = New ServerAppForm()

                '実作業スレッドを開始する。
                Dim oWorkerThread As New Thread(AddressOf MainClass.WorkingLoop)
                Log.Info("Starting the worker thread...")
                quitWorker = 0
                oWorkerThread.Name = "Worker"
                oWorkerThread.Start()

                'ウインドウプロシージャを実行する。
                'NOTE: このメソッドから例外がスローされることはない。
                ServerAppBaseMain(oMainForm)

                Try
                    '実作業スレッドに終了を要求する。
                    Log.Info("Sending quit request to the worker thread...")
                    Thread.VolatileWrite(quitWorker, 1)

                    'NOTE: 以下で実作業スレッドが終了しない場合、
                    '実作業スレッドは生存証明を行わないはずであり、
                    '状況への対処はプロセスマネージャで行われる想定である。

                    '実作業スレッドの終了を待つ。
                    Log.Info("Waiting for the worker thread to quit...")
                    oWorkerThread.Join()
                    Log.Info("The worker thread has quit.")
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oWorkerThread.Abort()
                End Try
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                If oMainForm IsNot Nothing Then
                    oMainForm.Dispose()
                End If
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
    ''' 実作業スレッドのメイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 定期的にDBから機器接続状態を取得し、異常があれば、
    ''' メールに記述して送信する。
    ''' </remarks>
    Private Shared Sub WorkingLoop()
        Try
            Log.Info("The worker thread started.")

            Dim oSmtpClient As SmtpClient = Nothing
            If Not Config.MailSmtpServerName.Equals("") Then
                oSmtpClient = New SmtpClient()
                oSmtpClient.Host = Config.MailSmtpServerName
                oSmtpClient.Port = Config.MailSmtpPort
                oSmtpClient.Credentials = New NetworkCredential(Config.MailSmtpUserName, Config.MailSmtpPassword)
                oSmtpClient.Timeout = Config.MailSendLimitTicks
            End If

            '前回検索日時（正規化済み）
            Dim lastSearchTime As DateTime = Normalize(DateTime.Now.AddMilliseconds(-Config.MailSendDelayTicks))

            '有効時間帯の開始時刻と終了時刻を（0時0分からの経過分の形式で）
            'mailStartMinutesInDayとmailEndMinutesInDayに算出しておく。
            'その際、mailStartMinutesInDay <= mailEndMinutesInDayになるよう、
            '必要に応じてmailEndMinutesInDayには補正をかけておく。
            'NOTE: mailStartMinutesInDay == mailEndMinutesInDayは正当な設定
            'であり、有効時間帯がその１分間だけであることを意味する。
            Dim mailStartMinutesInDay As Integer = Config.MailStartHour * 60 + Config.MailStartMinute
            Dim mailEndMinutesInDay As Integer = Config.MailEndHour * 60 + Config.MailEndMinute
            If mailStartMinutesInDay > mailEndMinutesInDay Then
                mailEndMinutesInDay += 24 * 60
            End If

            Dim oDiagnosisTimer As New TickTimer(Config.SelfDiagnosisIntervalTicks)
            oDiagnosisTimer.Start(TickTimer.GetSystemTick())

            While Thread.VolatileRead(quitWorker) = 0
                Dim systemTick As Long = TickTimer.GetSystemTick()

                If oDiagnosisTimer.GetTicksToTimeout(systemTick) <= 0 Then
                    oDiagnosisTimer.Start(systemTick)
                    ServerAppPulser.Pulse()
                End If

                If oSmtpClient IsNot Nothing Then
                    Dim now As DateTime = Normalize(DateTime.Now.AddMilliseconds(-Config.MailSendDelayTicks))
                    If now > lastSearchTime Then
                        'mailStartMinutesInDay以上になるように補正した
                        '現在時刻を（0時0分からの経過分の形式で）求める。
                        Dim nowMinutesInDay As Integer = now.Hour * 60 + now.Minute
                        If mailStartMinutesInDay > nowMinutesInDay Then
                            nowMinutesInDay += 24 * 60
                        End If

                        '有効時間帯のみ送信を行う。
                        If nowMinutesInDay <= mailEndMinutesInDay Then
                            SearchAndSend(lastSearchTime, now, oSmtpClient)
                        End If

                        lastSearchTime = now
                    ElseIf now < lastSearchTime Then
                        'システム日時が2周期以上戻された場合は、
                        '最終実施日時を正規化したシステム日時に合わせる。
                        Dim span As TimeSpan = lastSearchTime - now
                        Dim cycles As Integer = span.Minutes \ Config.MailSendCycle
                        If cycles > 1 Then
                            Log.Warn("The system time goes back into the past.")
                            lastSearchTime = now
                        End If
                    End If
                End If

                Thread.Sleep(Config.PollIntervalTicks)
            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP発生（または収集データ誤記テーブルへの登録）は、
            'プロセスマネージャが行うので、ここでは不要である。

            oMainForm.Invoke(New MethodInvoker(AddressOf oMainForm.Close))
        End Try
    End Sub

    Private Shared Function Normalize(ByVal time As DateTime) As DateTime
        Dim minutes As Integer = (time.Hour * 60 + time.Minute) - (Config.MailStartHour * 60 + Config.MailStartMinute)
        If minutes < 0 Then
            minutes += 24 * 60
            minutes = (minutes \ Config.MailSendCycle) * Config.MailSendCycle
            minutes -= 24 * 60
        Else
            minutes = (minutes \ Config.MailSendCycle) * Config.MailSendCycle
        End If
        Dim originTime As New DateTime(time.Year, time.Month, time.Day, Config.MailStartHour, Config.MailStartMinute, 0)
        Return originTime.AddMinutes(minutes)
    End Function

    ''' <summary>
    ''' サーチおよびメール生成処理。
    ''' </summary>
    Private Shared Sub SearchAndSend(ByVal prevTime As DateTime, ByVal curTime As DateTime, ByVal oSmtpClient As SmtpClient)
        Dim oMailBody As New StringBuilder()
        Dim sServiceDate As String = EkServiceDate.GenString()
        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()

            Dim agents As DataRowCollection
            Dim writeCount As Integer

            '機器構成マスタから、現在運用されているべき監視盤の一覧（休止号機は除外）を取得する。
            agents = SelectUnitsInService(dbCtl, EkConstants.ModelCodeKanshiban, sServiceDate).Rows

            'メール本文内の改札機状態情報セクションを編集。
            '機器構成から取得した監視盤単位で処理を行う。
            writeCount = 0
            For Each agent As DataRow In agents
                Dim sAgentStationName As String = agent.Field(Of String)("STATION_NAME")
                Dim sAgentCornerName As String = agent.Field(Of String)("CORNER_NAME")
                Dim agentUnitNumber As Integer = agent.Field(Of Integer)("UNIT_NO")

                '当該監視盤と接続していないまたはprevTime以降に接続した場合は、
                '今回の監視期間において運管サーバ⇔監視盤の通信異常があった旨を記載し、
                'この監視盤配下の情報は、それ以上記載しない。
                Dim oConnectDate As Object = SelectDirectConnectDate(dbCtl, EkConstants.ModelCodeKanshiban, agent, DbConstants.PortPurposeGeneralData)
                If oConnectDate Is Nothing OrElse _
                   CType(oConnectDate, DateTime) >= prevTime Then
                    'NOTE: 駅名とコーナー名だけで監視盤を特定することができるのか
                    '不明である（運用に依存する）ため、デフォルトのメール文言では、
                    'コーナー名の後に「監視盤」と断りを入れたうえで、号機番号も
                    '記載することにしている。
                    'TODO: 逆に、当該監視盤が担当する他のコーナーについても、
                    '運管との通信異常が発生してる旨を表示しなくてよいのか？
                    'メールを受信したユーザは、機器構成と照らし合わせた上で、
                    '関係する改札機で異常が発生している可能性を推察してくれるのか？
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.KsbLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.KsbOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '機器構成マスタから、当該監視盤配下で現在運用されているべき
                '改札機の一覧（休止号機は除外）を機器接続状態の列つきで取得する。
                'また、一覧には、TRUST列も付加されるものとする。
                'この列は、機器接続状態が登録されていない場合や
                '機器接続状態の収集日が古い場合（監視盤と接続はできていても、
                '監視盤から改札機接続状態の通知が無い場合）に、NULLになる。
                Dim units As DataRowCollection = SelectGateUnitsWithConStatusInService(dbCtl, EkConstants.ModelCodeGate, agent, sServiceDate, curTime).Rows

                'TRUST列がNULLになっている機器が配下に１つでもある監視盤については、
                '全ての機器接続状態が信頼できない旨を記載し、この監視盤配下の情報は、
                'それ以上記載しない。
                Dim isTrustAll As Boolean = True
                For Each unit As DataRow In units
                    If unit.Field(Of String)("TRUST") Is Nothing Then
                        isTrustAll = False
                        Exit For
                    End If
                Next unit
                If Not isTrustAll Then
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.KsbLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.KsbOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '機器構成から取得した改札機単位で処理を行う。
                For Each unit As DataRow In units
                    Dim sStationName As String = unit.Field(Of String)("STATION_NAME")
                    Dim sCornerName As String = unit.Field(Of String)("CORNER_NAME")
                    Dim unitNumber As Integer = unit.Field(Of Integer)("UNIT_NO")

                    '改札機電源が×なら、その旨を記載し、この改札機の情報は、
                    'それ以上記載しない。
                    If unit.Field(Of Integer)("KAIDENGEN") = 2 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                        oMailBody.AppendLine(Lexis.GatePowerErrorInConStatusMailBody.Gen())
                        Continue For
                    End If

                    '監視盤－主制御部が×なら、その旨を記載し、この改札機の情報は、
                    'それ以上記載しない。
                    If unit.Field(Of Integer)("KANSICONNECT") = 1 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                        oMailBody.AppendLine(Lexis.GateMainKsbErrorInConStatusMailBody.Gen())
                        Continue For
                    End If

                    '主制御部－ICUが×なら、その旨を記載し、この改札機の情報は、
                    'それ以上記載しない。
                    If unit.Field(Of Integer)("SHUSECONNECT") = 1 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                        oMailBody.AppendLine(Lexis.GateMainIcuErrorInConStatusMailBody.Gen())
                        Continue For
                    End If

                    'その他の×があれば、同一行にその旨を記載する。
                    If unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("HAISINICMCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 Then
                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.GatePartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.GateLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))

                        Dim fieldPos As Integer = 0
                        If unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 Then
                            oMailBody.Append(Lexis.GateMainDsvErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If
                        If unit.Field(Of Integer)("HAISINICMCONNECT") = 1 Then
                            If fieldPos <> 0 Then
                                oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                            End If
                            oMailBody.Append(Lexis.GateIcuDsvErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If
                        If unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 Then
                            If fieldPos <> 0 Then
                                oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                            End If
                            oMailBody.Append(Lexis.GateIcuTktErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If

                        oMailBody.AppendLine()
                    End If
                Next unit
            Next agent

            'Ver0,1 ADD START 統括状態情報の追加対応
            '機器構成マスタから、現在運用されているべき統括の
            '一覧（休止号機は除外）を機器接続状態の列つきで取得する。
            '一覧には、TRUST列も付加されるものとする。
            'この列は、機器接続状態が登録されていない場合や
            '機器接続状態の収集日が古い場合に、NULLになる。
            agents = SelectTktUnitsWithConStatusInService(dbCtl, EkConstants.ModelCodeTokatsu, sServiceDate, curTime).Rows

            'メール本文内の統括状態情報セクションを編集。
            '機器構成から取得した統括単位で処理を行う。
            writeCount = 0
            For Each agent As DataRow In agents
                Dim sStationName As String = agent.Field(Of String)("STATION_NAME")
                Dim sCornerName As String = agent.Field(Of String)("CORNER_NAME")
                Dim unitNumber As Integer = agent.Field(Of Integer)("UNIT_NO")

                'TRUST列がNULLになっている統括については、
                '機器接続状態が信頼できない旨を記載し、
                'それ以上の情報は記載しない。
                If agent.Field(Of String)("TRUST") Is Nothing Then
                    'NOTE: 駅名とコーナー名だけで統括を特定することができるのか
                    '不明である（運用に依存する）ため、デフォルトのメール文言では、
                    'コーナー名の後に「統括」と断りを入れたうえで、号機番号も
                    '記載することにしている。
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.TktPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '当該統括と接続していないまたはprevTime以降に接続した場合は、
                '今回の監視期間において運管サーバ⇔統括の通信異常があった旨を記載し、
                'この統括の情報は、それ以上記載しない。
                Dim oConnectDate As Object = SelectDirectConnectDate(dbCtl, EkConstants.ModelCodeTokatsu, agent, DbConstants.PortPurposeGeneralData)
                If oConnectDate Is Nothing OrElse _
                   CType(oConnectDate, DateTime) >= prevTime Then
                    'NOTE: 駅名とコーナー名だけで統括を特定することができるのか
                    '不明である（運用に依存する）ため、デフォルトのメール文言では、
                    'コーナー名の後に「統括」と断りを入れたうえで、号機番号も
                    '記載することにしている。
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.TktPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                If agent.Field(Of Integer)("IDCENTERCONNECT") = 1 Then
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.TktPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))
                    oMailBody.AppendLine(Lexis.TktIdcErrorInConStatusMailBody.Gen())
                End If
            Next agent
            'Ver0,1 ADD END 統括状態情報の追加対応

            '機器構成マスタから、現在運用されているべき統括の一覧（休止号機は除外）を取得する。
            agents = SelectUnitsInService(dbCtl, EkConstants.ModelCodeTokatsu, sServiceDate).Rows

            'メール本文内の窓処状態情報セクションを編集。
            '機器構成から取得した統括単位で処理を行う。
            writeCount = 0
            For Each agent As DataRow In agents
                Dim sAgentStationName As String = agent.Field(Of String)("STATION_NAME")
                Dim sAgentCornerName As String = agent.Field(Of String)("CORNER_NAME")
                Dim agentUnitNumber As Integer = agent.Field(Of Integer)("UNIT_NO")

                '当該統括と接続していないまたはprevTime以降に接続した場合は、
                '今回の監視期間において運管サーバ⇔統括の通信異常があった旨を記載し、
                'この統括配下の情報は、それ以上記載しない。
                Dim oConnectDate As Object = SelectDirectConnectDate(dbCtl, EkConstants.ModelCodeTokatsu, agent, DbConstants.PortPurposeGeneralData)
                If oConnectDate Is Nothing OrElse _
                   CType(oConnectDate, DateTime) >= prevTime Then
                    'NOTE: 駅名とコーナー名だけで統括を特定することができるのか
                    '不明である（運用に依存する）ため、デフォルトのメール文言では、
                    'コーナー名の後に「統括」と断りを入れたうえで、号機番号も
                    '記載することにしている。
                    'TODO: 逆に、当該統括が担当する他のコーナーについても、
                    '運管との通信異常が発生してる旨を表示しなくてよいのか？
                    'メールを受信したユーザは、機器構成と照らし合わせた上で、
                    '関係する窓処で異常が発生している可能性を推察してくれるのか？
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.MadoPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '機器構成マスタから、当該統括配下で現在運用されているべき
                '窓処の一覧（休止号機は除外）を機器接続状態の列つきで取得する。
                'また、一覧には、TRUST列も付加されるものとする。
                'この列は、機器接続状態が登録されていない場合や
                '機器接続状態の収集日が古い場合に、NULLになる。
                Dim units As DataRowCollection = SelectMadoUnitsWithConStatusInService(dbCtl, EkConstants.ModelCodeMadosho, agent, sServiceDate, curTime).Rows

                'TRUST列がNULLになっている機器が配下に１つでもある統括については、
                '全ての機器接続状態が信頼できない旨を記載し、この統括配下の情報は、
                'それ以上記載しない。
                Dim isTrustAll As Boolean = True
                For Each unit As DataRow In units
                    If unit.Field(Of String)("TRUST") Is Nothing Then
                        isTrustAll = False
                        Exit For
                    End If
                Next unit
                If Not isTrustAll Then
                    writeCount += 1
                    If writeCount = 1 Then
                        oMailBody.AppendLine(Lexis.MadoPartTitleInConStatusMailBody.Gen())
                    End If
                    oMailBody.Append(Lexis.TktLabelInConStatusMailBody.Gen(sAgentStationName, sAgentCornerName, agentUnitNumber))
                    oMailBody.AppendLine(Lexis.TktOpmgErrorInConStatusMailBody.Gen())
                    Continue For
                End If

                '機器構成から取得した窓処単位で処理を行う。
                For Each unit As DataRow In units
                    If unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("EXTOKATUDLCONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("KANSICONNECT") = 1 OrElse _
                       unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 Then

                        Dim sStationName As String = unit.Field(Of String)("STATION_NAME")
                        Dim sCornerName As String = unit.Field(Of String)("CORNER_NAME")
                        Dim unitNumber As Integer = unit.Field(Of Integer)("UNIT_NO")

                        writeCount += 1
                        If writeCount = 1 Then
                            oMailBody.AppendLine(Lexis.MadoPartTitleInConStatusMailBody.Gen())
                        End If
                        oMailBody.Append(Lexis.MadoLabelInConStatusMailBody.Gen(sStationName, sCornerName, unitNumber))

                        Dim fieldPos As Integer = 0
                        If unit.Field(Of Integer)("EXTOKATUCONNECT") = 1 Then
                            oMailBody.Append(Lexis.MadoTktIdErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        End If
                        If unit.Field(Of Integer)("EXTOKATUDLCONNECT") = 1 Then
                            If fieldPos <> 0 Then
                                oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                            End If
                            oMailBody.Append(Lexis.MadoTktDlErrorInConStatusMailBody.Gen())
                            fieldPos += 1
                        Else
                            If unit.Field(Of Integer)("KANSICONNECT") = 1 Then
                                If fieldPos <> 0 Then
                                    oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                                End If
                                oMailBody.Append(Lexis.MadoKsbErrorInConStatusMailBody.Gen())
                                fieldPos += 1
                            End If
                            If unit.Field(Of Integer)("HAISINSYUCONNECT") = 1 Then
                                If fieldPos <> 0 Then
                                    oMailBody.Append(Lexis.ErrorSeparatorInConStatusMailBody.Gen())
                                End If
                                oMailBody.Append(Lexis.MadoDsvErrorInConStatusMailBody.Gen())
                                fieldPos += 1
                            End If
                        End If

                        oMailBody.AppendLine()
                    End If
                Next unit
            Next agent

            'NOTE: 運管と窓処のコネクションについては、現行機に
            '表示の機能がないことから、ここでも通知は不要らしい。
            '（現行機は、常時接続でない故に判定対象外に
            'なっているのだと思われるが...）

            'NOTE: 利用データ用のコネクションについても、メール機能が
            '有効なJR西日本では、重要ではないため、メールによる通知は
            '不要であるものとする。

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return
        Finally
            dbCtl.ConnectClose()
        End Try

        If oMailBody.Length <> 0 Then
            Using oMail As New MailMessage()
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
                    Dim oSubjectEncoding As Encoding = Encoding.GetEncoding(Config.MailSubjectEncoding)
                    Dim sTimestamp As String = curTime.ToString(Lexis.DateTimeFormatInConStatusMailSubject.Gen())
                    oMail.Subject = String.Format( _
                       "=?{0}?B?{1}?=", _
                       oSubjectEncoding.BodyName, _
                       Convert.ToBase64String(oSubjectEncoding.GetBytes(Lexis.ConStatusMailSubject.Gen(sTimestamp)), Base64FormattingOptions.None))

                    'メール本文を編集。
                    Dim oAltView As AlternateView = _
                       AlternateView.CreateAlternateViewFromString( _
                          oMailBody.ToString(), _
                          Encoding.GetEncoding(Config.MailBodyEncoding), _
                          MediaTypeNames.Text.Plain)
                    oAltView.TransferEncoding = Config.MailTransferEncoding
                    oMail.AlternateViews.Add(oAltView)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    Return
                End Try

                Try
                    'メールを送信。
                    oSmtpClient.Send(oMail)
                    Log.Info("メールを送信しました。")
                Catch ex As SmtpException
                    Log.Error("Exception caught.", ex)
                    Log.Error("送信失敗メール内容:" & vbCrLf & oMail.Subject & vbCrLf & oMailBody.ToString())
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                End Try
            End Using
        End If
    End Sub

    Private Shared Function SelectUnitsInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal sServiceDate As String) As DataTable
        Dim sSQL As String = _
           "SELECT M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.ADDRESS, M.STATION_NAME, M.CORNER_NAME" _
           & " FROM M_MACHINE M" _
           & " WHERE M.MODEL_CODE = '" & sModel & "'" _
           & " AND M.ADDRESS <> ''" _
           & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                        & " FROM M_MACHINE" _
                                        & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
           & " AND NOT EXISTS (SELECT *" _
                              & " FROM M_RESTING_MACHINE R" _
                              & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                              & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                              & " AND M.CORNER_CODE = R.CORNER_CODE" _
                              & " AND M.MODEL_CODE = R.MODEL_CODE" _
                              & " AND M.UNIT_NO = R.UNIT_NO)"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function

    'Ver0,1 ADD START 統括状態情報の追加対応
    Private Shared Function SelectTktUnitsWithConStatusInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal sServiceDate As String, ByVal curTime As DateTime) As DataTable
        Dim sTrustLimitTime As String = curTime.AddMilliseconds(-Config.MadoConStatusTrustLimitTicks).ToString("yyyy/MM/dd HH:mm:ss.fff")
        Dim sSQL As String = _
           "SELECT T.*, S.MODEL_CODE AS TRUST, S.IDCENTERCONNECT" _
           & " FROM (" _
             & "SELECT M.MODEL_CODE, M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.STATION_NAME, M.CORNER_NAME" _
             & " FROM M_MACHINE M" _
             & " WHERE M.MODEL_CODE = '" & sModel & "'" _
             & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                          & " FROM M_MACHINE" _
                                          & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
             & " AND NOT EXISTS (SELECT *" _
                                & " FROM M_RESTING_MACHINE R" _
                                & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                                & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                                & " AND M.CORNER_CODE = R.CORNER_CODE" _
                                & " AND M.MODEL_CODE = R.MODEL_CODE" _
                                & " AND M.UNIT_NO = R.UNIT_NO)) AS T" _
           & " LEFT JOIN (" _
             & "SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                   & " IDCENTERCONNECT" _
               & " FROM D_CON_STATUS" _
               & " WHERE SYUSYU_DATE >= '" & sTrustLimitTime & "') AS S" _
           & " ON T.MODEL_CODE = S.MODEL_CODE" _
           & " AND T.RAIL_SECTION_CODE = S.RAIL_SECTION_CODE" _
           & " AND T.STATION_ORDER_CODE = S.STATION_ORDER_CODE" _
           & " AND T.CORNER_CODE = S.CORNER_CODE" _
           & " AND T.UNIT_NO = S.UNIT_NO"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function
    'Ver0,1 ADD END 統括状態情報の追加対応

    Private Shared Function SelectGateUnitsWithConStatusInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal agent As DataRow, ByVal sServiceDate As String, ByVal curTime As DateTime) As DataTable
        Dim sTrustLimitTime As String = curTime.AddMilliseconds(-Config.GateConStatusTrustLimitTicks).ToString("yyyy/MM/dd HH:mm:ss.fff")
        Dim sSQL As String = _
           "SELECT T.*, S.MODEL_CODE AS TRUST, S.KAIDENGEN, S.KANSICONNECT, S.SHUSECONNECT, S.HAISINSYUCONNECT, S.HAISINICMCONNECT, S.EXTOKATUCONNECT" _
           & " FROM (" _
             & "SELECT M.MODEL_CODE, M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.STATION_NAME, M.CORNER_NAME" _
             & " FROM M_MACHINE M" _
             & " WHERE M.MODEL_CODE = '" & sModel & "'" _
             & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                          & " FROM M_MACHINE" _
                                          & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
             & " AND M.MONITOR_ADDRESS = '" & agent.Field(Of String)("ADDRESS") & "'" _
             & " AND NOT EXISTS (SELECT *" _
                                & " FROM M_RESTING_MACHINE R" _
                                & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                                & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                                & " AND M.CORNER_CODE = R.CORNER_CODE" _
                                & " AND M.MODEL_CODE = R.MODEL_CODE" _
                                & " AND M.UNIT_NO = R.UNIT_NO)) AS T" _
           & " LEFT JOIN (" _
             & "SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                   & " KAIDENGEN, KANSICONNECT, SHUSECONNECT, HAISINSYUCONNECT, HAISINICMCONNECT, EXTOKATUCONNECT" _
               & " FROM D_CON_STATUS" _
               & " WHERE SYUSYU_DATE >= '" & sTrustLimitTime & "') AS S" _
           & " ON T.MODEL_CODE = S.MODEL_CODE" _
           & " AND T.RAIL_SECTION_CODE = S.RAIL_SECTION_CODE" _
           & " AND T.STATION_ORDER_CODE = S.STATION_ORDER_CODE" _
           & " AND T.CORNER_CODE = S.CORNER_CODE" _
           & " AND T.UNIT_NO = S.UNIT_NO"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function

    Private Shared Function SelectMadoUnitsWithConStatusInService(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal agent As DataRow, ByVal sServiceDate As String, ByVal curTime As DateTime) As DataTable
        Dim sTrustLimitTime As String = curTime.AddMilliseconds(-Config.MadoConStatusTrustLimitTicks).ToString("yyyy/MM/dd HH:mm:ss.fff")
        Dim sSQL As String = _
           "SELECT T.*, S.MODEL_CODE AS TRUST, S.KANSICONNECT, S.HAISINSYUCONNECT, S.EXTOKATUDLCONNECT, S.EXTOKATUCONNECT" _
           & " FROM (" _
             & "SELECT M.MODEL_CODE, M.RAIL_SECTION_CODE, M.STATION_ORDER_CODE, M.CORNER_CODE, M.UNIT_NO, M.STATION_NAME, M.CORNER_NAME" _
             & " FROM M_MACHINE M" _
             & " WHERE M.MODEL_CODE = '" & sModel & "'" _
             & " AND M.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                                          & " FROM M_MACHINE" _
                                          & " WHERE SETTING_START_DATE <= '" & sServiceDate & "')" _
             & " AND M.MONITOR_ADDRESS = '" & agent.Field(Of String)("ADDRESS") & "'" _
             & " AND NOT EXISTS (SELECT *" _
                                & " FROM M_RESTING_MACHINE R" _
                                & " WHERE M.RAIL_SECTION_CODE = R.RAIL_SECTION_CODE" _
                                & " AND M.STATION_ORDER_CODE = R.STATION_ORDER_CODE" _
                                & " AND M.CORNER_CODE = R.CORNER_CODE" _
                                & " AND M.MODEL_CODE = R.MODEL_CODE" _
                                & " AND M.UNIT_NO = R.UNIT_NO)) AS T" _
           & " LEFT JOIN (" _
             & "SELECT MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_CODE, UNIT_NO," _
                   & " KANSICONNECT, HAISINSYUCONNECT, EXTOKATUDLCONNECT, EXTOKATUCONNECT" _
               & " FROM D_CON_STATUS" _
               & " WHERE SYUSYU_DATE >= '" & sTrustLimitTime & "') AS S" _
           & " ON T.MODEL_CODE = S.MODEL_CODE" _
           & " AND T.RAIL_SECTION_CODE = S.RAIL_SECTION_CODE" _
           & " AND T.STATION_ORDER_CODE = S.STATION_ORDER_CODE" _
           & " AND T.CORNER_CODE = S.CORNER_CODE" _
           & " AND T.UNIT_NO = S.UNIT_NO"
        Return dbCtl.ExecuteSQLToRead(sSQL)
    End Function

    Private Shared Function SelectDirectConnectDate(ByVal dbCtl As DatabaseTalker, ByVal sModel As String, ByVal agent As DataRow, ByVal sPortPurpose As String) As Object
        Dim sSQL As String = _
           "SELECT CONNECT_DATE" _
           & " FROM S_DIRECT_CON_STATUS" _
           & " WHERE MODEL_CODE = '" & sModel & "'" _
           & " AND RAIL_SECTION_CODE = '" & agent.Field(Of String)("RAIL_SECTION_CODE") & "'" _
           & " AND STATION_ORDER_CODE = '" & agent.Field(Of String)("STATION_ORDER_CODE") & "'" _
           & " AND CORNER_CODE = " & agent.Field(Of Integer)("CORNER_CODE").ToString() _
           & " AND UNIT_NO = " & agent.Field(Of Integer)("UNIT_NO").ToString() _
           & " AND PORT_KBN = '" & sPortPurpose & "'"
        Return dbCtl.ExecuteSQLToReadScalar(sSQL)
    End Function

End Class
