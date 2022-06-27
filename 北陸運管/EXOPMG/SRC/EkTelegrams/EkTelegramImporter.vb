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
Imports System.Net.Sockets

Imports JR.ExOpmg.Common

''' <summary>
''' 外部から電文を取り込むクラス。
''' </summary>
Public Class EkTelegramImporter
    Implements ITelegramImporter

#Region "変数"
    Protected Gene As EkTelegramGene
#End Region

#Region "コンストラクタ"
    Public Sub New(ByVal oGene As EkTelegramGene)
        Me.Gene = oGene
    End Sub
#End Region

#Region "メソッド"
    'バイト配列からの電文取得メソッド
    'NOTE: バイト列が電文として完全に不正である（所定箇所に記載されている
    'レングスが規定値に満たない、あるいは規定より大きい）ために処理できない
    '場合や、ヘッダ部に相当するバイト数を読み取れないまたは、ヘッダ部に記載
    'された分のバイト数を読み取れない場合は、発生事象を内部で記録し、
    'Nothingを返却する。
    Public Function GetTelegramFromBytes( _
       ByVal aBytes As Byte()) As EkDodgyTelegram

        Dim minReceiveSize As Integer = Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
        If aBytes.Length < minReceiveSize Then
            Log.Error("The bytes is too short as EkTelegram.")
            Return Nothing
        End If

        Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(aBytes, Gene.ObjSizePos)
        If objSize > Gene.GetObjSizeByRawLen(Integer.MaxValue) Then
            Log.Error("ObjSize of the telegram is too large.")
            Return Nothing
        End If

        Dim telegLen As Integer = Gene.GetRawLenByObjSize(objSize)
        If telegLen > aBytes.Length Then
            Log.Error("Telegram size based on the ObjSize is greater than the bytes.")
            Return Nothing
        End If

        Dim aTelegBytes As Byte() = New Byte(telegLen - 1) {}
        Buffer.BlockCopy(aBytes, 0, aTelegBytes, 0, telegLen)

        Return New EkDodgyTelegram(Gene, aTelegBytes)
    End Function

    'ストリームからの電文取得メソッド
    'NOTE: バイト列が電文として完全に不正である（所定箇所に記載されている
    'レングスが規定値に満たない、あるいは規定より大きい）ために処理できない
    '場合は、発生事象を内部で記録し、Nothingを返却する。
    'また、oStreamが終端に達して、ヘッダ部に相当するバイト数を読み取れない
    'または、ヘッダ部に記載された分のバイト数を読み取れない場合も、
    '発生事象を内部で記録し、Nothingを返却する。
    'なお、このメソッドは、oStreamが指すインスタンスのReadメソッドが
    'スローし得る全ての例外をスローし得る。呼び元は、それらの例外のうち、
    'プログラム内部の不整合でしか発生し得ない例外のみを予期せぬ問題
    'として扱うべきである。たとえば、ソフトウェアでハンドリング可能な
    'ハードウェアの異常が何らかのExceptionとしてスローされることがあり、
    'それをアプリケーションで処理する必要がある（たとえば、他の処理を
    '継続する必要があるあるいは、すぐに落ちるのではなく、何かしら行う
    '必要がある）なら、それは予期すべき例外であり、選別可能な方法で
    'Catchしなければならない。
    Public Function GetTelegramFromStream( _
       ByVal oStream As Stream) As EkDodgyTelegram

        Dim minReceiveSize As Integer = Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
        Debug.Assert(Gene.MinAllocSize >= minReceiveSize)
        Debug.Assert(Gene.MaxReceiveSize > Gene.MinAllocSize)

        Dim telegLen As Integer = minReceiveSize
        Dim aBytes As Byte() = New Byte(Gene.MinAllocSize - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False
        Do
            Dim rcvlen As Integer = oStream.Read(aBytes, offset, telegLen - offset)
            If rcvlen = 0 Then
                Log.Error("End of stream detected.")
                Return Nothing
            End If

            offset = offset + rcvlen
            If offset = telegLen Then
                If isReceivedMinSize Then
                    Exit Do
                End If

                Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(aBytes, Gene.ObjSizePos)
                If objSize > Gene.GetObjSizeByRawLen(Integer.MaxValue) Then
                    Log.Error("ObjSize of the telegram is too large.")
                    Return Nothing
                End If

                telegLen = Gene.GetRawLenByObjSize(objSize)
                If telegLen <= minReceiveSize Then
                    If telegLen < minReceiveSize Then
                        Log.Error("ObjSize of the telegram is too small.")
                        Return Nothing
                    End If
                    Exit Do
                End If

                If telegLen > Gene.MinAllocSize Then
                    If telegLen > Gene.MaxReceiveSize Then
                        Log.Error("Telegram size based on the ObjSize is greater than my buffer.")
                        Return Nothing
                    End If
                    Array.Resize(aBytes, telegLen)
                End If

                isReceivedMinSize = True
            End If
        Loop

        Return New EkDodgyTelegram(Gene, aBytes)
    End Function

    'ソケットからの電文取得メソッド
    'NOTE: timeoutBaseTicksに0または-1を指定すると無期限待機となる。
    'NOTE: バイト列が電文として完全に不正である（所定箇所に記載されている
    'レングスが規定値に満たない、あるいは規定より大きい）ために処理できない
    '場合や、指定時間内にヘッダ部に相当するバイト数を読み取れないまたは、
    'ヘッダ部に記載された分のバイト数を読み取れない場合、電文の途中で
    '相手装置から終端を告げられた場合、外部要因の可能性がある
    'SocketExceptionが発生した場合など、コネクション終了に持ち込むべきで
    'ある（プログラムの異常と扱うべきでない）ケースでは、発生事象を内部で
    '記録し、Nothingを返却する。
    Public Function GetTelegramFromSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As EkDodgyTelegram

        Dim minReceiveSize As Integer = Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
        Debug.Assert(Gene.MinAllocSize >= minReceiveSize)
        Debug.Assert(Gene.MaxReceiveSize > Gene.MinAllocSize)

        Dim telegLen As Integer = minReceiveSize
        Dim aBytes As Byte() = New Byte(Gene.MinAllocSize - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False

        Debug.Assert(oSocket.Blocking)
        Dim oTimer As TickTimer = Nothing
        Dim systemTick As Long
        If timeoutBaseTicks > 0 Then
            oTimer = New TickTimer(timeoutBaseTicks)
            systemTick = TickTimer.GetSystemTick()
            oTimer.Start(systemTick)
        Else
            oSocket.ReceiveTimeout = 0
        End If
        Try
            Do
                If timeoutBaseTicks > 0 Then
                    Dim ticks As Long = oTimer.GetTicksToTimeout(systemTick)
                    If ticks < 1 Then
                        LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                        Log.Error("I'm through waiting for all bytes of the telegram to read.")
                        Return Nothing
                    End If
                    oSocket.ReceiveTimeout = CInt(ticks)
                End If

                Dim rcvlen As Integer = oSocket.Receive(aBytes, offset, telegLen - offset, SocketFlags.None)
                If rcvlen = 0 Then
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("Connection closed by peer.")
                    Return Nothing
                End If

                offset = offset + rcvlen
                If offset = telegLen Then
                    If isReceivedMinSize Then
                        Exit Do
                    End If

                    Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(aBytes, Gene.ObjSizePos)
                    If objSize > Gene.GetObjSizeByRawLen(Integer.MaxValue) Then
                        LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                        Log.Error("ObjSize of the telegram is too large.")
                        Return Nothing
                    End If

                    telegLen = Gene.GetRawLenByObjSize(objSize)
                    If telegLen <= minReceiveSize Then
                        If telegLen < minReceiveSize Then
                            LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                            Log.Error("ObjSize of the telegram is too small.")
                            Return Nothing
                        End If
                        Exit Do
                    End If

                    If telegLen > Gene.MinAllocSize Then
                        If telegLen > Gene.MaxReceiveSize Then
                            LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                            Log.Error("Telegram size based on the ObjSize is greater than my buffer.")
                            Return Nothing
                        End If
                        Array.Resize(aBytes, telegLen)
                    End If

                    If timeoutBaseTicks > 0 Then
                        'NOTE: oTimer.Renew()に与える時間が負値になる可能性もあるが、
                        'ループの先頭でタイムアウトと判定されるはずであるため、
                        'ここでの判定は省略する。
                        systemTick = TickTimer.GetSystemTick()
                        Dim remainingTicks As Long = oTimer.GetTicksToTimeout(systemTick)
                        oTimer.Renew(remainingTicks + (CLng(timeoutExtraTicksPerMiB) * objSize) \ 1048576)
                        oTimer.Start(systemTick)
                    End If

                    isReceivedMinSize = True
                End If

                systemTick = TickTimer.GetSystemTick()
            Loop
        Catch ex As SocketException
            Select Case ex.ErrorCode
                '指定した時間内に書き込みできなかった場合（WSAETIMEDOUT）
                'TODO: これじゃない気も（Socketクラスの実装次第？）
                Case 10060
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("I'm through waiting for all bytes of the telegram to read.", ex)
                    Return Nothing

                '発生したなら、アプリの不具合が要因である可能性が濃厚であるため、
                'アプリを終了させて早めに（テスト中に）気付かせた方がよいエラー
                'NOTE: 外部と通信を行うためのソケットを複数のスレッドから操作することは
                'あり得ない（呼び元のバグである）という前提で、10036（WSAEINPROGRESS）
                'もここにある。
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Throw

                'Receive()において、装置外要因や装置内の状況で発生しそうであるため、
                'アプリを終了させるわけにはいかないと思われるエラー
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("SocketException caught.", ex)
                    Return Nothing

                '発生し得ないはずであるが、将来どうなるかわからないため、
                'アプリを終了させない方が無難と思われるエラー
                Case Else
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("Surprising SocketException caught.", ex)
                    Return Nothing
            End Select
        End Try

        If Not Gene.IsCrcIndicatingOkay(aBytes) Then
            LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
            Log.Error("CRC Error detected.")
            Return Nothing
        End If

        LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
        Return New EkDodgyTelegram(Gene, aBytes)
    End Function

    Private Shared Sub LogReceivedBytes(ByVal aBytes As Byte(), ByVal validLen As Integer, ByVal loggingMaxLen As Integer)
        If loggingMaxLen > 0 Then
            If validLen <= 0 Then
                Log.Info("No byte received.")
            Else
                Dim loggingLen As Integer = validLen
                If loggingLen > loggingMaxLen Then loggingLen = loggingMaxLen
                Log.Info(validLen.ToString() & " bytes received.", aBytes, 0, loggingLen)
            End If
        End If
    End Sub

    'ソケットからの電文取得メソッド
    Private Function GetITelegramFromSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As ITelegram Implements ITelegramImporter.GetTelegramFromSocket

        Return GetTelegramFromSocket(oSocket, timeoutBaseTicks, timeoutExtraTicksPerMiB, telegLoggingMaxLength)
    End Function
#End Region

End Class
