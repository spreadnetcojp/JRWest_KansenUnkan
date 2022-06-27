' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

Public Class OpClientUtil
    Private Shared oTelegramGene As EkTelegramGene
    Private Shared oChildSteerSock As Socket
    Private Shared oTelegrapher As OpClientTelegrapher
    Private Shared sPermittedPathInFtp As String

    Public Shared Sub StartTelegrapher()
        Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, Config.PermittedPathInFtp)
        Log.Info("Sweeping directory [" & sFtpBase & "]...")
        Utility.DeleteTemporalDirectory(sFtpBase)

        oTelegramGene = New EkTelegramGeneForNativeModels(Config.FtpWorkingDirPath)
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New OpClientTelegrapher("Telegrapher", oMessageSockForTelegrapher, oTelegramGene)

        Log.Info("Starting the telegrapher...")
        oTelegrapher.Start()

        sPermittedPathInFtp = Nothing
    End Sub

    Public Shared Sub QuitTelegrapher()
        If oTelegrapher IsNot Nothing Then
            Log.Info("Sending quit request to the telegrapher...")
            If QuitRequest.Gen().WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
                Log.Fatal("The telegrapher seems broken.")
            End If

            Log.Info("Waiting for the telegrapher to quit...")
            If oTelegrapher.Join(Config.TelegrapherPendingLimitTicks) = False Then
                Log.Fatal("The telegrapher seems broken.")
                oTelegrapher.Abort()
            End If
        End If

        If oChildSteerSock IsNot Nothing Then
            oChildSteerSock.Close()
        End If

        oTelegramGene = Nothing
        oChildSteerSock = Nothing
        oTelegrapher = Nothing
        sPermittedPathInFtp = Nothing
    End Sub

    ''' <summary>
    ''' [壊れたTelegrapherに関するリソースの回収とリスタート]
    ''' </summary>
    Public Shared Sub RestartBrokenTelegrapher()
        '子スレッドのAbort応答期限
        Const TelegrapherAbortLimitTicks As Integer = 5000

        Log.Info("Renewing the telegrapher...")
        oChildSteerSock.Close()

        If oTelegrapher.ThreadState <> System.Threading.ThreadState.Stopped Then
            oTelegrapher.Abort()

            'NOTE: Abort()の結果、Telegrapherは例外をキャッチしてログを
            '出力する可能性がある。また、こちらがAbort()から戻ってきた時点で、
            '既に例外処理が開始されていることは最低限保証されていてほしいが、
            'msdnをみた感じだといまいち不明であるため、スレッドが終了状態に
            'ならない限りは、通信相手に関するその他のグローバルな情報もまだ更新
            'する可能性があると考えるべきである。よって、できる限り終了を待って
            'から、新たなTelegrapherをスタートさせる。
            If oTelegrapher.Join(TelegrapherAbortLimitTicks) = False Then
                Log.Warn("The telegrapher may refuse to abort.")
            End If
        End If

        sPermittedPathInFtp = Nothing

        'NOTE: アプリケーションを再起動することで除去できるし、
        '障害解析のヒントになる可能性もあるので、FTPの
        '一時作業用ディレクトリはそのままにしておく。

        'oChildSteerSockに関して、現在の参照先を切り離し、新たなLocalConnectionの一端を参照させる。
        'oTelegrapherに関して、現在の参照先を切り離し、新たなTelegrapherを参照させる。
        Dim oMessageSockForTelegrapher As Socket = Nothing
        LocalConnectionProvider.CreateSockets(oChildSteerSock, oMessageSockForTelegrapher)
        oTelegrapher = New OpClientTelegrapher("Telegrapher", oMessageSockForTelegrapher, oTelegramGene)

        Log.Info("Starting the telegrapher...")
        oTelegrapher.Start()
    End Sub

    'NOTE: 設計段階でコネクションキープを想定していたため、以下の実装は微妙である。
    'もし、このままコネクションキープにしないのであれば、ちょっと複雑になるが、
    'コネクト完了待ちや、配信結果待ち、切断待ちの間は、そのための状態を
    '管理した上で、メッセージループを継続する方がよい。
    '業務的な処理は完全にできなくするにしても、ウィンドウの移動など
    'はできた方がよいので、最悪、Telegrapherとの一連のやりとりを
    'プールスレッドに任せて、画面から抜けるときなどのみに、その終了を
    '待てばよい。

    Public Shared Function Connect() As Boolean
        Dim sServerName As String = Config.ServerIpAddr & "." & Config.IpPortForTelegConnection.ToString()
        Log.Info("Outgoing to [" & sServerName & "]...")

        Dim oTelegSock As Socket
        Try
            oTelegSock = SockUtil.Connect(Config.ServerIpAddr, Config.IpPortForTelegConnection)
        Catch ex As OPMGException
            Log.Error("Exception caught.", ex)
            Return False
        End Try

        Dim oLocalEndPoint As IPEndPoint = DirectCast(oTelegSock.LocalEndPoint, IPEndPoint)
        Dim sClientName As String = oLocalEndPoint.Address.ToString() & "." & oLocalEndPoint.Port.ToString()
        Log.Info("Connection established by [" & sClientName & "] to [" & sServerName & "].")

        sPermittedPathInFtp = Path.Combine(Config.PermittedPathInFtp, sClientName)

        'FTPで使う一時作業用ディレクトリを初期化する。
        Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, sPermittedPathInFtp)
        Log.Info("Initializing directory [" & sFtpBase & "]...")
        Utility.DeleteTemporalDirectory(sFtpBase)
        Directory.CreateDirectory(sFtpBase)

        Log.Info("Sending new socket to the telegrapher...")
        oTelegrapher.LineStatus = LineStatus.Connected
        If ConnectNotice.Gen(oTelegSock).WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Return True
    End Function

    Public Shared Sub Disconnect()
        Log.Info("Sending disconnect request to the telegrapher...")
        If DisconnectRequest.Gen().WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Dim oTimer As New TickTimer(Config.TelegrapherPendingLimitTicks)
        oTimer.Start(TickTimer.GetSystemTick())
        While oTelegrapher.LineStatus <> LineStatus.Disconnected
            If oTelegrapher.ThreadState = System.Threading.ThreadState.Stopped Then
                Log.Fatal("The telegrapher seems broken.")
                RestartBrokenTelegrapher()
                Throw New OPMGException()
            End If
            If oTimer.GetTicksToTimeout(TickTimer.GetSystemTick()) <= 0 Then
                Log.Fatal("The telegrapher seems broken.")
                RestartBrokenTelegrapher()
                Throw New OPMGException()
            End If
            System.Threading.Thread.Sleep(100)
        End While

        'FTPで使った一時作業用ディレクトリを片付ける。
        Dim sFtpBase As String = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, sPermittedPathInFtp)
        sPermittedPathInFtp = Nothing
        Log.Info("Sweeping directory [" & sFtpBase & "]...")
        Utility.DeleteTemporalDirectory(sFtpBase)
    End Sub

    Public Shared Function UllMasProFile(ByVal sFilePath As String) As MasProUllResult
        Dim sFileNameInFtp As String _
           = Path.Combine(sPermittedPathInFtp, Path.GetFileName(sFilePath))

        Dim sDestPath As String _
           = Utility.CombinePathWithVirtualPath(Config.FtpWorkingDirPath, sFileNameInFtp)

        File.Copy(sFilePath, sDestPath, True)

        Log.Info("Sending MasProUllRequest to the telegrapher...")
        If MasProUllRequest.Gen(sFileNameInFtp).WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Log.Info("Waiting for MasProUllResponse from the telegrapher...")
        'NOTE: 通信スレッドで異常が発生した場合、通信スレッドがoChildSteerSockの対端を必ずクローズする想定。
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oChildSteerSock, Config.TelegrapherUllLimitTicks)
        If (Not oRcvMsg.HasValue) OrElse (oRcvMsg.Kind <> ClientAppInternalMessageKind.MasProUllResponse) Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Return MasProUllResponse.Parse(oRcvMsg).Result
    End Function

    Public Shared Function InvokeMasProDll(ByVal sListFileName As String, ByVal forcingFlag As Boolean) As MasProDllInvokeResult
        Dim oExt As New MasProDllInvokeRequestExtendPart()
        oExt.ListFileName = sListFileName
        oExt.ForcingFlag = forcingFlag
        Log.Info("Sending MasProDllInvokeRequest to the telegrapher...")
        If MasProDllInvokeRequest.Gen(oExt).WriteToSocket(oChildSteerSock, Config.TelegrapherPendingLimitTicks) = False Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Log.Info("Waiting for MasProDllInvokeResponse from the telegrapher...")
        'NOTE: 通信スレッドで異常が発生した場合、通信スレッドがoChildSteerSockの対端を必ずクローズする想定。
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oChildSteerSock, Config.TelegrapherDllInvokeLimitTicks)
        If (Not oRcvMsg.HasValue) OrElse (oRcvMsg.Kind <> ClientAppInternalMessageKind.MasProDllInvokeResponse) Then
            Log.Fatal("The telegrapher seems broken.")
            RestartBrokenTelegrapher()
            Throw New OPMGException()
        End If

        Return MasProDllInvokeResponse.Parse(oRcvMsg).Result
    End Function
End Class
