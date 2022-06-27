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
Imports System.Net.Cache
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' ファイル転送の全手続きを裏で（生成元と非同期に）行うクラス。
''' </summary>
''' <remarks>
''' ファイル転送の方法はFTPである。
''' </remarks>
Public Class FtpWorker
    Inherits Looper
    Implements IXllWorker

#Region "定数や変数"
    'サーバ側ファイルのベースURI
    Private oForeignBaseUri As Uri

    'サーバの資格情報
    Private oCredential As NetworkCredential

    '最初の要求送信開始からデータコネクションが確立するまでの
    '期限（ファイルのダウンロード時やアップロード時）または、
    '最初の要求送信開始から最後の応答を受信完了するまでの期限
    '（制御コネクションだけですべてが終わるメソッドの場合）および、
    'アップロード後のFTP状態取得の期限。
    Private requestLimitTicks As Integer

    'ログアウトの試行期限
    'NOTE: 親スレッドは、たとえCancelTransfer()を実行したとしても
    '応答メッセージ受信までには、これだけ（+α）の時間が掛り得る
    'ことを想定しなければならない。
    'つまり、ClientTelegrapherのactiveXllWorkerPendingLimitTicks
    'やpassiveXllWorkerPendingLimitTicksには、この設定値よりも
    '十分に大きな値を設定するべきである。
    'なお、QuitRequestメッセージ送信後も、これだけ（+α）の時間は、
    'このスレッドが残り得る。
    Private logoutLimitTicks As Integer

    '転送停止許容時間
    Private transferStallLimitTicks As Integer

    'パッシブモードを使用するか否か
    Private usePassiveMode As Boolean

    '親スレッドからの要求を実行するごとにログアウトするか否か
    Private logoutEachTime As Boolean

    '転送データの読み込み（書き出し）用バッファ
    Private aBuffer As Byte()

    '転送キャンセルの通知待ちオブジェクト
    Private oCancelEvent As ManualResetEvent

    'ログインしているか否か
    Private isLoggedIn As Boolean
#End Region

#Region "コンストラクタ"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal sForeignBaseUri As String, _
       ByVal oCredential As NetworkCredential, _
       ByVal requestLimitTicks As Integer, _
       ByVal logoutLimitTicks As Integer, _
       ByVal transferStallLimitTicks As Integer, _
       ByVal usePassiveMode As Boolean, _
       ByVal logoutEachTime As Boolean, _
       Optional ByVal bufferLength As Integer = 1024)
        'NOTE: このメソッドは親スレッドで実行されることになる。そして、
        'ここで（親スレッドで）初期化した変数は、MyBase.Startメソッドを実行して
        '以降、子スレッドで参照されることになる。しかし、MyBase.Startメソッドが
        'メモリバリアとなるため、初期化は単純代入等で済まして問題ない。

        MyBase.New(sThreadName, oParentMessageSock)
        Me.oForeignBaseUri = New Uri(sForeignBaseUri)
        Me.oCredential = oCredential
        Me.requestLimitTicks = requestLimitTicks
        Me.logoutLimitTicks = logoutLimitTicks
        Me.transferStallLimitTicks = transferStallLimitTicks
        Me.usePassiveMode = usePassiveMode
        Me.logoutEachTime = logoutEachTime
        Me.aBuffer = New Byte(bufferLength - 1) {}
        Me.oCancelEvent = New ManualResetEvent(False)
        Me.isLoggedIn = False
    End Sub
#End Region

#Region "親スレッド用メソッド"
    Private ReadOnly Property __ThreadState() As ThreadState Implements IXllWorker.ThreadState
        Get
            Return ThreadState
        End Get
    End Property

    Public Overrides Sub Start() Implements IXllWorker.Start
        MyBase.Start()
    End Sub

    Public Overrides Sub Abort() Implements IXllWorker.Abort
        MyBase.Abort()
    End Sub

    Public Sub PrepareTransfer() Implements IXllWorker.PrepareTransfer
        oCancelEvent.Reset()
    End Sub

    Public Sub CancelTransfer() Implements IXllWorker.CancelTransfer
        oCancelEvent.Set()
    End Sub
#End Region

#Region "メソッド"
    Protected Overrides Function ProcOnSockReadable(ByVal oSock As Socket) As Boolean
        Debug.Assert(oSock Is oParentMessageSock)
        Dim oRcvMsg As InternalMessage = InternalMessage.GetInstanceFromSocket(oSock)
        Select Case oRcvMsg.Kind
            Case InternalMessageKind.DownloadRequest
                Return ProcOnDownloadRequestReceive(oRcvMsg)

            Case InternalMessageKind.UploadRequest
                Return ProcOnUploadRequestReceive(oRcvMsg)

            Case InternalMessageKind.QuitRequest
                Return ProcOnQuitRequestReceive(oRcvMsg)

            Case Else
                Debug.Fail("This case is impermissible.")
                Return True
        End Select
    End Function

    'TODO: StreamのBeginWrite()は停止する可能性はないか？
 
    Protected Overridable Function ProcOnDownloadRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        'NOTE: 以下のForの内側ではWebException以外の例外も発生する可能性はある。
        'たとえば、FileStreamのコンストラクタに渡す文字列に不正がある場合などである。
        'しかし、それらが発生しないようにするのは、IXllWorkerの呼び元の責務とする。
        'そして、そういった不特定箇所で例外が発生した後、少なくともこのスレッドの
        '状態を健全な状態まで戻すことは非現実的である。よって、そのような例外を
        'ここで捕捉して、ログアウトやアップロード（中止）応答送信を行うのではなく、
        'そのような例外はProcOnUnhandledExceptionで処理し、対親スレッド通信用ソケット
        'をクローズすることで、親スレッドに通信スレッドの破損を検出させることにする。
        Dim isOK As Boolean = True
        Dim oExt As DownloadRequestExtendPart = DownloadRequest.Parse(oRcvMsg).ExtendPart
        Dim lastIndex As Integer = oExt.TransferList.Count - 1
        For i As Integer = 0 To lastIndex
            Dim oForeignUri As New Uri(oForeignBaseUri, oExt.TransferList(i))
            Dim sLocalPath As String = Utility.CombinePathWithVirtualPath(oExt.TransferListBase, oExt.TransferList(i))
            Directory.CreateDirectory(Path.GetDirectoryName(sLocalPath))

            Dim oFtpReq As FtpWebRequest = DirectCast(WebRequest.Create(oForeignUri), FtpWebRequest)
            oFtpReq.Credentials = oCredential
            oFtpReq.Method = WebRequestMethods.Ftp.DownloadFile
            oFtpReq.UseBinary = True
            oFtpReq.UsePassive = usePassiveMode
            oFtpReq.CachePolicy = New RequestCachePolicy(RequestCacheLevel.BypassCache)
            oFtpReq.Proxy = Nothing
            oFtpReq.Timeout = Timeout.Infinite
            oFtpReq.ReadWriteTimeout = Timeout.Infinite
            If logoutEachTime Then
                oFtpReq.KeepAlive = False
            Else
                oFtpReq.KeepAlive = If(i = lastIndex, False, True)
            End If

            'NOTE: oFtpReq.EndGetResponse()におけるリソース確保の後、その
            '戻り値となるべきリファレンスをoFtpResにセットするまでの間に
            '例外が発生すれば、確保されたリソースを解放することはできない
            'が、下記の前提があるため、問題ない。
            'まず、EndGetResponse自身は、リソースを確保した後、外部要因の
            '異常（通信異常等）を示す例外をスローする場合は、必ずリソースの
            '解放を行うはずである（EndGetResponseの責務である）。
            '次に、EndGetResponse()から戻った後、戻り値をoFtpResにセット
            '完了するまでの間も、このスレッド自身の処理では、少なくとも
            'バグ以外の要因で例外がスローされることは無いはずである。
            '最後に、他のスレッドがこのスレッドに対するAbort()を実行する
            'ことで、oFtpReq.EndGetResponse()におけるリソース確保の後、その
            '戻り値となるべきリファレンスがoFtpResにセットされるまでの間に、
            'ThreadAbortExceptionが発生するケースについては、仮にあった
            'としても、リソースの解放が行われる必要はない。プロセスの存続を
            '前提としたThread.Abort()の利用自体が、避けるべきことであり
            'そのような利用方法があるなら、それを修正するべきである。
            '少なくとも、Thread.Abort()は、内部要因の異常（バグ）に対応する
            'ためのものであり、外部要因で生じる処理のキャンセルを実現する
            'ために使用してはならない。
            Dim oFtpRes As FtpWebResponse = Nothing
            Dim oResStream As Stream = Nothing
            Dim oFileStream As FileStream = Nothing
            Try
                Log.Info("Requesting " & oForeignUri.AbsoluteUri & " to get...")

                isLoggedIn = True
                Dim oBegResult As IAsyncResult = oFtpReq.BeginGetResponse(Nothing, Nothing)
                Dim oBegResultAsyncWaitHandle As WaitHandle = oBegResult.AsyncWaitHandle
                Try
                    Dim aWaitHandles() As WaitHandle _
                       = {oBegResultAsyncWaitHandle, oCancelEvent}

                    Dim index As Integer = WaitHandle.WaitAny(aWaitHandles, requestLimitTicks)
                    If index = WaitHandle.WaitTimeout Then
                        Log.Error("Request limit time comes.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If
                    If index = 1 Then
                        Log.Info("Canceled by manager.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If

                    'NOTE: FtpWebRequestインスタンス（FtpWebRequestへの
                    'DirectCastが成功したインスタンス）からGetResponse()で
                    '取得したインスタンスは必ずFtpWebResponseであるため、
                    '下記のDirectCastで例外が発生する可能性は想定しない。
                    oFtpRes = DirectCast(oFtpReq.EndGetResponse(oBegResult), FtpWebResponse)
                    Log.Info("Request succeeded.")
                    Log.Info("ftp status: " & oFtpRes.StatusDescription)
                Finally
                    oBegResultAsyncWaitHandle.Close()
                End Try

                Log.Info("Transferring the file...")
                oResStream = oFtpRes.GetResponseStream()
                oFileStream = New FileStream(sLocalPath, FileMode.Create, FileAccess.Write)
                Do
                    Dim oBegReadResult As IAsyncResult _
                       = oResStream.BeginRead(aBuffer, 0, aBuffer.Length, Nothing, Nothing)
                    Dim oBegReadResultAsyncWaitHandle As WaitHandle _
                       = oBegReadResult.AsyncWaitHandle
                    Try
                        Dim aReadWaitHandles() As WaitHandle _
                           = {oBegReadResultAsyncWaitHandle, oCancelEvent}

                        Dim readableIndex As Integer = WaitHandle.WaitAny(aReadWaitHandles, transferStallLimitTicks)
                        If readableIndex = WaitHandle.WaitTimeout Then
                            Log.Error("Transfer stall limit time comes.")
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If
                        If readableIndex = 1 Then
                            Log.Info("Canceled by manager.")
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If

                        Dim readSize As Integer = oResStream.EndRead(oBegReadResult)
                        If readSize = 0 Then Exit Do

                        oFileStream.Write(aBuffer, 0, readSize)
                    Finally
                        oBegReadResultAsyncWaitHandle.Close()
                    End Try
                Loop

                Log.Info("Transfer finished.")
                Log.Info("ftp status: " & oFtpRes.StatusDescription)

            Catch ex As WebException
                Log.Error("WebException caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If ex.Response IsNot Nothing Then
                    Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                    Log.Error("ftp status: " & oExFtpRes.StatusDescription)
                End If

                'ログインを実施していないことが確実と言えるケースでは、
                'Logoutメソッドの実行は無意味なので省略する（フレーム
                'ワークがセッションを継続していない状況においてLogout
                'メソッドを実行すると、無駄にログインしてからPWDを実施
                'し、そのセッションからログアウトすることになる）。
                'OPT: 確実と言えるケースは他にもあるかもしれない。
                If i = 0 AndAlso _
                   oFtpRes Is Nothing AndAlso _
                   ex.Status = WebExceptionStatus.ConnectFailure Then
                    isLoggedIn = False
                End If

                Exit For
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If oFtpRes IsNot Nothing Then
                    Log.Error("ftp status: " & oFtpRes.StatusDescription)
                End If

                Exit For
            Finally
                If oFileStream IsNot Nothing Then
                    oFileStream.Close()
                End If

                If oResStream IsNot Nothing Then
                    Try
                        oResStream.Close()
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        isOK = False
                    End Try
                End If

                If oFtpRes IsNot Nothing Then
                    'NOTE: oFtpRes.Close()が失敗した場合は、以下で
                    'isLoggedInを変更せずに、Exit Forする。
                    Try
                        oFtpRes.Close()

                        'NOTE: FTPの開始から転送の中で例外が発生した場合でも、
                        'oFtpRes.Close()が成功すれば、ここは実行されるが、
                        'そのような異常系でも、Not KeepAliveなoFtpReqから得た
                        'oFtpResについて、oFtpRes.Close()が成功すれば、
                        '必ずログアウト状態になる（既にセッションが異常終了
                        'しているか、そうでなければoFtpRes.Close()で正常に
                        '終了する）という想定である。
                        '即ち、ここで下記の条件が成立する場合は確実に
                        'ログアウト状態のはずであり、Logoutメソッドの
                        '実行は無意味なので省略する（フレームワークが
                        'セッションを継続していない状況においてLogout
                        'メソッドを実行すると、無駄にログインしてから
                        'PWDを実施し、そのセッションからログアウトする
                        'ことになる）。
                        If Not oFtpReq.KeepAlive Then
                            isLoggedIn = False
                        End If
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        isOK = False
                    End Try
                End If
            End Try
            If Not isOK Then Exit For
        Next

        If logoutEachTime AndAlso isLoggedIn Then
            Logout()
        End If

        If isOk Then
            DownloadResponse.Gen(DownloadResult.Finished).WriteToSocket(oParentMessageSock)
        Else
            DownloadResponse.Gen(DownloadResult.Aborted).WriteToSocket(oParentMessageSock)
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnUploadRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        'NOTE: 以下のForの内側ではWebException以外の例外も発生する可能性はある。
        'たとえば、FileStreamのコンストラクタに渡す文字列に不正がある場合などである。
        'しかし、それらが発生しないようにするのは、IXllWorkerの呼び元の責務とする。
        'そして、そういった不特定箇所で例外が発生した後、少なくともこのスレッドの
        '状態を健全な状態まで戻すことは非現実的である。よって、そのような例外を
        'ここで捕捉して、ログアウトやアップロード（中止）応答送信を行うのではなく、
        'そのような例外はProcOnUnhandledExceptionで処理し、対親スレッド通信用ソケット
        'をクローズすることで、親スレッドに通信スレッドの破損を検出させることにする。
        'なお、運管端末プロセスの場合、そのようなとき、親スレッドは、通信スレッドの
        '再起動を実施することなる。本当なら、プロセス全体を再起動したいところであるが、
        'その判断は、メッセージボックスの表示をみたユーザに委ねる。
        Dim isOK As Boolean = True
        Dim oExt As UploadRequestExtendPart = UploadRequest.Parse(oRcvMsg).ExtendPart
        Dim lastIndex As Integer = oExt.TransferList.Count - 1
        For i As Integer = 0 To lastIndex
            Dim oForeignUri As New Uri(oForeignBaseUri, oExt.TransferList(i))
            Dim sLocalPath As String = Utility.CombinePathWithVirtualPath(oExt.TransferListBase, oExt.TransferList(i))
            Dim oFtpReq As FtpWebRequest = DirectCast(WebRequest.Create(oForeignUri), FtpWebRequest)
            oFtpReq.Credentials = oCredential
            oFtpReq.Method = WebRequestMethods.Ftp.UploadFile
            oFtpReq.UseBinary = True
            oFtpReq.UsePassive = usePassiveMode
            oFtpReq.CachePolicy = New RequestCachePolicy(RequestCacheLevel.BypassCache)
            oFtpReq.Proxy = Nothing
            oFtpReq.Timeout = Timeout.Infinite
            oFtpReq.ReadWriteTimeout = Timeout.Infinite
            If logoutEachTime Then
                oFtpReq.KeepAlive = False
            Else
                oFtpReq.KeepAlive = If(i = lastIndex, False, True)
            End If

            'NOTE: oReqStreamは、WebExceptionがスローされた際に、
            'どの処理でスローされたのかを判断するだけのために、
            'このレベルで宣言している。
            Dim oReqStream As Stream = Nothing
            Dim oFileStream As FileStream = Nothing
            Try
                oFileStream = New FileStream(sLocalPath, FileMode.Open, FileAccess.Read)

                Log.Info("Requesting " & oForeignUri.AbsoluteUri & " to put...")

                isLoggedIn = True
                Dim oBegResult As IAsyncResult = oFtpReq.BeginGetRequestStream(Nothing, Nothing)
                Dim oBegResultAsyncWaitHandle As WaitHandle = oBegResult.AsyncWaitHandle
                Try
                    Dim aWaitHandles() As WaitHandle _
                       = {oBegResultAsyncWaitHandle, oCancelEvent}

                    Dim index As Integer = WaitHandle.WaitAny(aWaitHandles, requestLimitTicks)
                    If index = WaitHandle.WaitTimeout Then
                        Log.Error("Request limit time comes.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If
                    If index = 1 Then
                        Log.Info("Canceled by manager.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If

                    oReqStream = oFtpReq.EndGetRequestStream(oBegResult)
                    Log.Info("Request succeeded.")
                Finally
                    oBegResultAsyncWaitHandle.Close()
                End Try

                Log.Info("Transferring the file...")
                Do
                    Dim readSize As Integer = oFileStream.Read(aBuffer, 0, aBuffer.Length)
                    If readSize = 0 Then Exit Do

                    Dim oBegWriteResult As IAsyncResult _
                       = oReqStream.BeginWrite(aBuffer, 0, readSize, Nothing, Nothing)
                    Dim oBegWriteResultAsyncWaitHandle As WaitHandle _
                       = oBegWriteResult.AsyncWaitHandle
                    Try
                        Dim aWriteWaitHandles() As WaitHandle _
                           = {oBegWriteResultAsyncWaitHandle, oCancelEvent}

                        Dim writableIndex As Integer = WaitHandle.WaitAny(aWriteWaitHandles, transferStallLimitTicks)
                        If writableIndex = WaitHandle.WaitTimeout Then
                            Log.Error("Transfer stall limit time comes.")
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If
                        If writableIndex = 1 Then
                            Log.Info("Canceled by manager.")
                            'NOTE: 最後まで書き込まないでよいなら、下記のAbort()か
                            'Stream自体のClose()により、oReqStream.EndWrite(oBegWriteResult)に
                            '相当する処理は不要になるという想定である。
                            oFtpReq.Abort()
                            isOK = False
                            Exit For
                        End If

                        oReqStream.EndWrite(oBegWriteResult)
                    Finally
                        oBegWriteResultAsyncWaitHandle.Close()
                    End Try
                Loop
                Log.Info("Transfer finished.")
            Catch ex As WebException
                Log.Error("WebException caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If ex.Response IsNot Nothing Then
                    Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                    Log.Error("ftp status: " & oExFtpRes.StatusDescription)
                End If

                'ログインを実施していないことが確実と言えるケースでは、
                'Logoutメソッドの実行は無意味なので省略する（フレーム
                'ワークがセッションを継続していない状況においてLogout
                'メソッドを実行すると、無駄にログインしてからPWDを実施
                'し、そのセッションからログアウトすることになる）。
                'OPT: 確実と言えるケースは他にもあるかもしれない。
                If i = 0 AndAlso _
                   oReqStream Is Nothing AndAlso _
                   ex.Status = WebExceptionStatus.ConnectFailure Then
                    isLoggedIn = False
                End If

                Exit For
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                oFtpReq.Abort()
                isOK = False
                Exit For
            Finally
                If oFileStream IsNot Nothing Then
                    oFileStream.Close()
                End If

                If oReqStream IsNot Nothing Then
                    Try
                        oReqStream.Close()
                    Catch ex As Exception
                        Log.Error("Exception caught.", ex)
                        isOK = False
                    End Try
                End If
            End Try
            If Not isOK Then Exit For

            'OPT: 最後のファイルについては、以下でNot KeepAliveな
            'FtpWebRequestからFtpWebResponseを作成し、それを閉じる
            'ことで、ログアウトすることを意図しているが、
            '少なくとも最後のファイル以外は、以下を行う必要は
            'ない（パフォーマンスを考慮するなら行うべきでない）
            '感じがする。
            Dim oFtpRes As FtpWebResponse = Nothing
            Try
                Log.Info("Requesting ftp status...")
                Dim oBegResult As IAsyncResult = oFtpReq.BeginGetResponse(Nothing, Nothing)
                Dim oBegResultAsyncWaitHandle As WaitHandle = oBegResult.AsyncWaitHandle
                Try
                    Dim aWaitHandles() As WaitHandle _
                       = {oBegResultAsyncWaitHandle, oCancelEvent}

                    'NOTE: 最後の待機操作なので、わざわざrequestLimitTicksを指定する
                    'ことにメリットはないように思えるが、親スレッドがタイマをかけて
                    'くれている（いつかCancelを行う）とは限らないため、
                    'requestLimitTicksを指定することにする。
                    Dim index As Integer = WaitHandle.WaitAny(aWaitHandles, requestLimitTicks)
                    If index = WaitHandle.WaitTimeout Then
                        Log.Error("Request limit time comes.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If
                    If index = 1 Then
                        Log.Info("Canceled by manager.")
                        oFtpReq.Abort()
                        isOK = False
                        Exit For
                    End If

                    'NOTE: FtpWebRequestインスタンス（FtpWebRequestへの
                    'DirectCastが成功したインスタンス）からGetResponse()で
                    '取得したインスタンスは必ずFtpWebResponseであるため、
                    '下記のDirectCastで例外が発生する可能性は想定しない。
                    oFtpRes = DirectCast(oFtpReq.EndGetResponse(oBegResult), FtpWebResponse)
                    Log.Info("Request succeeded.")
                    Log.Info("ftp status: " & oFtpRes.StatusDescription)
                Finally
                    oBegResultAsyncWaitHandle.Close()
                End Try
            Catch ex As WebException
                Log.Error("WebException caught.", ex)
                oFtpReq.Abort()
                isOK = False

                If ex.Response IsNot Nothing Then
                    Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                    Log.Error("ftp status: " & oExFtpRes.StatusDescription)
                End If

                Exit For
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                oFtpReq.Abort()
                isOK = False

                'NOTE: この状況ではoFtpResはNothingであると思われるし、
                '万が一Nothingでないとしても、Close済みであると思われる
                'ため、oFtpRes.StatusDescriptionのログ出力は、
                '常に行わない。

                Exit For
            Finally
                If oFtpRes IsNot Nothing Then
                    'NOTE: oFtpRes.Close()が失敗した場合は、以下で
                    'isLoggedInを変更せずに、Exit Forする。
                    Try
                        oFtpRes.Close()

                        'NOTE: FTPの開始から転送の中で例外が発生した場合でも、
                        'oFtpRes.Close()が成功すれば、ここは実行されるが、
                        'そのような異常系でも、Not KeepAliveなoFtpReqから得た
                        'oFtpResについて、oFtpRes.Close()が成功すれば、
                        '必ずログアウト状態になる（既にセッションが異常終了
                        'しているか、そうでなければoFtpRes.Close()で正常に
                        '終了する）という想定である。
                        '即ち、ここで下記の条件が成立する場合は確実に
                        'ログアウト状態のはずであり、Logoutメソッドの
                        '実行は無意味なので省略する（フレームワークが
                        'セッションを継続していない状況においてLogout
                        'メソッドを実行すると、無駄にログインしてから
                        'PWDを実施し、そのセッションからログアウトする
                        'ことになる）。
                        If Not oFtpReq.KeepAlive Then
                            isLoggedIn = False
                        End If
                    Catch ex As Exception
                        Log.Fatal("Unwelcome Exception caught.", ex)
                        isOK = False
                    End Try
                End If
            End Try
            If Not isOK Then Exit For
        Next

        If logoutEachTime AndAlso isLoggedIn Then
            Logout()
        End If

        If isOk Then
            UploadResponse.Gen(UploadResult.Finished).WriteToSocket(oParentMessageSock)
        Else
            UploadResponse.Gen(UploadResult.Aborted).WriteToSocket(oParentMessageSock)
        End If

        Return True
    End Function

    Protected Overridable Function ProcOnQuitRequestReceive(ByVal oRcvMsg As InternalMessage) As Boolean
        Log.Info("Quit requested by manager.")

        If isLoggedIn Then
            Logout()
        End If

        If oParentMessageSock IsNot Nothing Then
            UnregisterSocket(oParentMessageSock)
            oParentMessageSock.Close()
            oParentMessageSock = Nothing
        End If

        If oCancelEvent IsNot Nothing Then
            oCancelEvent.Close()
        End If

        Return False
    End Function

    Protected Overrides Sub ProcOnUnhandledException(ByVal ex As Exception)
        'NOTE: 親スレッドなどからAbortを呼ばれた場合もここが実行される
        'ため、ここでの動作は予測できないと考えるべきである。
        'よって、サーバ側のリソースを考えると、Logout()を行っておきたい
        'ところであるが、それは諦める。
        'たしかにサーバ側のことは心配になるが、このスレッド自身の例外で
        'ここに到達することは、どこかの実装に問題がない限り、あり得ない
        'はずであるし、サーバ側も短いタイマでFTPのセッションを回収する
        '設定になっているはずであり、おそらく問題にはならない。
        'それが問題になる前に、端末側で予期せぬ例外が発生すること自体が
        '問題になり、修正されるはずである。

        'NOTE: これは、IXllWorker実装クラスの責務とする。
        '親スレッドは、このクローズによって、異常を検知する。
        If oParentMessageSock IsNot Nothing Then
            UnregisterSocket(oParentMessageSock)
            oParentMessageSock.Close()
            oParentMessageSock = Nothing
        End If

        If oCancelEvent IsNot Nothing Then
            oCancelEvent.Close()
        End If
    End Sub

    Private Sub Logout()
        Dim oFtpReq As FtpWebRequest = DirectCast(WebRequest.Create(oForeignBaseUri), FtpWebRequest)
        oFtpReq.Credentials = oCredential
        oFtpReq.Method = WebRequestMethods.Ftp.PrintWorkingDirectory
        oFtpReq.KeepAlive = False
        oFtpReq.Timeout = logoutLimitTicks
        oFtpReq.Proxy = Nothing
        Try
            Dim oRes As WebResponse = oFtpReq.GetResponse()
            oRes.Close()
        Catch ex As WebException
            'NOTE: このメソッド自体は失敗しているが、
            'ここが実行されるということは、そもそもログインも失敗しているか、
            'ログインが成功した後に回線が切れたケースと思われるので、
            'このメソッドが成功しなくても、問題はないと思われる。
            Log.Error("WebException caught.", ex)
            oFtpReq.Abort()

            If ex.Response IsNot Nothing Then
                Dim oExFtpRes As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
                Log.Error("ftp status: " & oExFtpRes.StatusDescription)
            End If
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            oFtpReq.Abort()
        End Try

        isLoggedIn = False
    End Sub

#End Region

End Class
