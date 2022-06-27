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

Imports System.Globalization
Imports System.IO
Imports System.Net.Sockets
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 電文。
''' </summary>
''' <remarks>
''' あくまで、各種電文クラスの実装の一部を代行するためのクラスである。
''' よって、このクラス自体のインスタンスを作成することは不可能である。
''' </remarks>
Public Class NkTelegram
    Implements ITelegram

#Region "定数"
    Friend Const SendTimeFormat As String = "yyyyMMddHHmmss"

    Friend Const SeqCodeLen As Integer = 2
    Friend Const CmdCodeLen As Integer = 2
    Friend Const ObjSizeLen As Integer = 4
    Friend Const SrcRailSectionCodeLen As Integer = 1
    Friend Const SrcStationOrderCodeLen As Integer = 1
    Friend Const DstRailSectionCodeLen As Integer = 1
    Friend Const DstStationOrderCodeLen As Integer = 1
    Friend Const SendTimeLen As Integer = 14
    Friend Const ReservedAreaLen As Integer = 6

    Friend Const SeqCodePos As Integer = 0
    Friend Const CmdCodePos As Integer = SeqCodePos + SeqCodeLen
    Friend Const ObjSizePos As Integer = CmdCodePos + CmdCodeLen
    Friend Const SrcRailSectionCodePos As Integer = ObjSizePos + ObjSizeLen
    Friend Const SrcStationOrderCodePos As Integer = SrcRailSectionCodePos + SrcRailSectionCodeLen
    Friend Const DstRailSectionCodePos As Integer = SrcStationOrderCodePos + SrcStationOrderCodeLen
    Friend Const DstStationOrderCodePos As Integer = DstRailSectionCodePos + DstRailSectionCodeLen
    Friend Const SendTimePos As Integer = DstStationOrderCodePos + DstStationOrderCodeLen
    Friend Const ReservedAreaPos As Integer = SendTimePos + SendTimeLen
    Friend Const ObjPos As Integer = ReservedAreaPos + ReservedAreaLen

    Private Shared ReadOnly aSeqCodeCollection() As Byte = {Asc("1"), Asc("0")}
    Private Shared ReadOnly aSeqCodeDelivery() As Byte = {Asc("2"), Asc("0")}
    Private Shared ReadOnly aSeqCodeTest() As Byte = {Asc("3"), Asc("0")}

    Private Shared ReadOnly oRawSeqCodeTable As New Dictionary(Of NkSeqCode, Byte()) From { _
       {NkSeqCode.Collection, aSeqCodeCollection}, _
       {NkSeqCode.Delivery, aSeqCodeDelivery}, _
       {NkSeqCode.Test, aSeqCodeTest}}
    Private Shared ReadOnly oSeqCodeTable As New Dictionary(Of UShort, NkSeqCode) From { _
       {Utility.GetUInt16FromLeBytes2(aSeqCodeCollection, 0), NkSeqCode.Collection}, _
       {Utility.GetUInt16FromLeBytes2(aSeqCodeDelivery, 0), NkSeqCode.Delivery}, _
       {Utility.GetUInt16FromLeBytes2(aSeqCodeTest, 0), NkSeqCode.Test}}

    Private Shared ReadOnly aCmdCodeComStartREQ() As Byte = {Asc("0"), Asc("1")}
    Private Shared ReadOnly aCmdCodeComStartACK() As Byte = {Asc("0"), Asc("2")}
    Private Shared ReadOnly aCmdCodeInquiryREQ() As Byte = {Asc("1"), Asc("0")}
    Private Shared ReadOnly aCmdCodeInquiryACK() As Byte = {Asc("4"), Asc("0")}
    Private Shared ReadOnly aCmdCodeDataPostREQ() As Byte = {Asc("2"), Asc("0")}
    Private Shared ReadOnly aCmdCodeDataPostACK() As Byte = {Asc("3"), Asc("0")}
    Private Shared ReadOnly aCmdCodeComStopREQ() As Byte = {Asc("5"), Asc("0")}
    Private Shared ReadOnly aCmdCodeComStopACK() As Byte = {Asc("5"), Asc("8")}

    Private Shared ReadOnly oRawCmdCodeTable As New Dictionary(Of NkCmdCode, Byte()) From { _
       {NkCmdCode.ComStartReq, aCmdCodeComStartREQ}, _
       {NkCmdCode.ComStartAck, aCmdCodeComStartACK}, _
       {NkCmdCode.InquiryReq, aCmdCodeInquiryREQ}, _
       {NkCmdCode.InquiryAck, aCmdCodeInquiryACK}, _
       {NkCmdCode.DataPostReq, aCmdCodeDataPostREQ}, _
       {NkCmdCode.DataPostAck, aCmdCodeDataPostACK}, _
       {NkCmdCode.ComStopReq, aCmdCodeComStopREQ}, _
       {NkCmdCode.ComStopAck, aCmdCodeComStopACK}}
    Private Shared ReadOnly oCmdCodeTable As New Dictionary(Of UShort, NkCmdCode) From { _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStartREQ, 0), NkCmdCode.ComStartReq}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStartACK, 0), NkCmdCode.ComStartAck}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeInquiryREQ, 0), NkCmdCode.InquiryReq}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeInquiryACK, 0), NkCmdCode.InquiryAck}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeDataPostREQ, 0), NkCmdCode.DataPostReq}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeDataPostACK, 0), NkCmdCode.DataPostAck}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStopREQ, 0), NkCmdCode.ComStopReq}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStopACK, 0), NkCmdCode.ComStopAck}}

    Private Shared ReadOnly oCmdKindTable As New Dictionary(Of UShort, CmdKind) From { _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStartREQ, 0), CmdKind.Req}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStartACK, 0), CmdKind.Ack}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeInquiryREQ, 0), CmdKind.Req}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeInquiryACK, 0), CmdKind.Ack}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeDataPostREQ, 0), CmdKind.Req}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeDataPostACK, 0), CmdKind.Ack}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStopREQ, 0), CmdKind.Req}, _
       {Utility.GetUInt16FromLeBytes2(aCmdCodeComStopACK, 0), CmdKind.Ack}}
#End Region

#Region "変数"
    Protected RawBytes As Byte()
    Protected ObjFilePathList As List(Of String)
    Protected ObjFilesCombinedLen As UInteger
#End Region

#Region "プロパティ"
    Public Property SeqCode() As NkSeqCode
        Get
            Dim code As NkSeqCode
            If oSeqCodeTable.TryGetValue(Utility.GetUInt16FromLeBytes2(RawBytes, SeqCodePos), code) = False Then
                code = NkSeqCode.None
            End If
            Return code
        End Get

        Set(ByVal code As NkSeqCode)
            Dim rawSeqCode As Byte() = oRawSeqCodeTable(code)
            Buffer.BlockCopy(rawSeqCode, 0, RawBytes, SeqCodePos, rawSeqCode.Length)
        End Set
    End Property

    Public Property CmdCode() As NkCmdCode
        Get
            Dim code As NkCmdCode
            If oCmdCodeTable.TryGetValue(Utility.GetUInt16FromLeBytes2(RawBytes, CmdCodePos), code) = False Then
                code = NkCmdCode.None
            End If
            Return code
        End Get

        Set(ByVal code As NkCmdCode)
            Dim rawCmdCode As Byte() = oRawCmdCodeTable(code)
            Buffer.BlockCopy(rawCmdCode, 0, RawBytes, CmdCodePos, rawCmdCode.Length)
        End Set
    End Property

    Public Property ObjSize() As UInteger
        Get
            Return Utility.GetUInt32FromLeBytes4(RawBytes, ObjSizePos)
        End Get

        Set(ByVal objSize As UInteger)
            Utility.CopyUInt32ToLeBytes4(objSize, RawBytes, ObjSizePos)
        End Set
    End Property

    Public Property SrcEkCode() As EkCode
        Get
            Dim code As EkCode
            code.RailSection = RawBytes(SrcRailSectionCodePos)
            code.StationOrder = RawBytes(SrcStationOrderCodePos)
            Return code
        End Get

        Set(ByVal code As EkCode)
            RawBytes(SrcRailSectionCodePos) = CByte(code.RailSection)
            RawBytes(SrcStationOrderCodePos) = CByte(code.StationOrder)
        End Set
    End Property

    Public Property DstEkCode() As EkCode
        Get
            Dim code As EkCode
            code.RailSection = RawBytes(DstRailSectionCodePos)
            code.StationOrder = RawBytes(DstStationOrderCodePos)
            Return code
        End Get

        Set(ByVal code As EkCode)
            RawBytes(DstRailSectionCodePos) = CByte(code.RailSection)
            RawBytes(DstStationOrderCodePos) = CByte(code.StationOrder)
        End Set
    End Property

    Public Property SendTime() As DateTime
        Get
            Dim sSendTime As String = Encoding.UTF8.GetString(RawBytes, SendTimePos, SendTimeLen)
            Return DateTime.ParseExact(sSendTime, SendTimeFormat, CultureInfo.InvariantCulture)
        End Get

        Set(ByVal sendTime As DateTime)
            Dim sSendTime As String = sendTime.ToString(SendTimeFormat)
            Encoding.UTF8.GetBytes(sSendTime, 0, SendTimeLen, RawBytes, SendTimePos)
        End Set
    End Property

    Public Property RawSeqCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(SeqCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, SeqCodePos, ret, 0, SeqCodeLen)
            Return ret
        End Get

        Set(ByVal rawSeqCode As Byte())
            Buffer.BlockCopy(rawSeqCode, 0, RawBytes, SeqCodePos, SeqCodeLen)
        End Set
    End Property

    Public Property RawCmdCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(CmdCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, CmdCodePos, ret, 0, CmdCodeLen)
            Return ret
        End Get

        Set(ByVal rawCmdCode As Byte())
            Buffer.BlockCopy(rawCmdCode, 0, RawBytes, CmdCodePos, CmdCodeLen)
        End Set
    End Property

    Public Property RawSrcEkCode() As Byte()
        Get
            Dim pos As Integer = SrcRailSectionCodePos
            Dim len As Integer = SrcRailSectionCodeLen + SrcStationOrderCodeLen
            Dim ret As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, pos, ret, 0, len)
            Return ret
        End Get

        Set(ByVal rawCode As Byte())
            Dim pos As Integer = SrcRailSectionCodePos
            Dim len As Integer =  SrcRailSectionCodeLen + SrcStationOrderCodeLen
            Buffer.BlockCopy(rawCode, 0, RawBytes, pos, len)
        End Set
    End Property

    Public Property RawDstEkCode() As Byte()
        Get
            Dim pos As Integer = DstRailSectionCodePos
            Dim len As Integer = DstRailSectionCodeLen + DstStationOrderCodeLen
            Dim ret As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, pos, ret, 0, len)
            Return ret
        End Get

        Set(ByVal rawCode As Byte())
            Dim pos As Integer = DstRailSectionCodePos
            Dim len As Integer = DstRailSectionCodeLen + DstStationOrderCodeLen
            Buffer.BlockCopy(rawCode, 0, RawBytes, pos, len)
        End Set
    End Property

    Public Property RawSendTime() As Byte()
        Get
            Dim ret As Byte() = New Byte(SendTimeLen - 1) {}
            Buffer.BlockCopy(RawBytes, SendTimePos, ret, 0, SendTimeLen)
            Return ret
        End Get

        Set(ByVal rawSendTime As Byte())
            Buffer.BlockCopy(rawSendTime, 0, RawBytes, SendTimePos, SendTimeLen)
        End Set
    End Property

    Public ReadOnly Property CmdKind() As CmdKind Implements ITelegram.CmdKind
        Get
            Dim kind As CmdKind
            If oCmdKindTable.TryGetValue(Utility.GetUInt16FromLeBytes2(RawBytes, CmdCodePos), kind) = False Then
                kind = CmdKind.None
            End If
            Return kind
        End Get
    End Property
#End Region

#Region "コンストラクタ（サブクラスのコンストラクタの実装用）"
    Protected Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal cmdCode As NkCmdCode, _
       ByVal objLen As Integer)

        Dim objSize As UInteger = CUInt(objLen)
        Me.RawBytes = New Byte(ObjPos + objLen - 1) {}
        Me.ObjFilePathList = Nothing
        Buffer.BlockCopy(oRawSeqCodeTable(seqCode), 0, Me.RawBytes, SeqCodePos, SeqCodeLen)
        Buffer.BlockCopy(oRawCmdCodeTable(cmdCode), 0, Me.RawBytes, CmdCodePos, CmdCodeLen)
        Me.SendTime = DateTime.MinValue
        Utility.CopyUInt32ToLeBytes4(objSize, Me.RawBytes, ObjSizePos)
    End Sub

    Protected Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal cmdCode As NkCmdCode, _
       ByVal objHeaderLen As Integer, _
       ByVal oObjFilePathList As List(Of String), _
       ByVal objFilesCombinedLen As Long)

        Dim objSize As UInteger = CUInt(objHeaderLen + objFilesCombinedLen)
        Me.RawBytes = New Byte(ObjPos + objHeaderLen - 1) {}
        Me.ObjFilePathList = oObjFilePathList
        Me.ObjFilesCombinedLen = CUInt(objFilesCombinedLen)
        Buffer.BlockCopy(oRawSeqCodeTable(seqCode), 0, Me.RawBytes, SeqCodePos, SeqCodeLen)
        Buffer.BlockCopy(oRawCmdCodeTable(cmdCode), 0, Me.RawBytes, CmdCodePos, CmdCodeLen)
        Me.SendTime = DateTime.MinValue
        Utility.CopyUInt32ToLeBytes4(objSize, Me.RawBytes, ObjSizePos)
    End Sub

    Protected Sub New(ByVal aRawBytes As Byte(), ByVal oObjFilePathList As List(Of String), ByVal objFilesCombinedLen As Long)
        Me.RawBytes = aRawBytes
        Me.ObjFilePathList = oObjFilePathList
        Me.ObjFilesCombinedLen = CUInt(objFilesCombinedLen)
    End Sub

    'iTelegの実体がNkTelegramであることを前提とするメソッドです。
    '誤った使い方をした場合は、InvalidCastExceptionがスローされます。
    Protected Sub New(ByVal iTeleg As ITelegram)
        Dim oTeleg As NkTelegram = DirectCast(iTeleg, NkTelegram)
        Me.RawBytes = oTeleg.RawBytes
        Me.ObjFilePathList = oTeleg.ObjFilePathList
        Me.ObjFilesCombinedLen = oTeleg.ObjFilesCombinedLen
    End Sub
#End Region

#Region "メソッド"
    'NAK電文を生成するメソッド
    Private Function CreateINakTelegram(ByVal causeCode As NakCauseCode) As INakTelegram Implements ITelegram.CreateNakTelegram
        Return Nothing
    End Function

    'ヘッダ部の書式違反をチェックするメソッド
    Public Function GetHeaderFormatViolation() As NakCauseCode Implements ITelegram.GetHeaderFormatViolation
        If Not Utility.IsDecimalAsciiBytesFixed(RawBytes, SendTimePos, SendTimeLen) Then
            Log.Error("SendTime is invalid (not decimal ASCII bytes).")
            Return NakCauseCode.TelegramError
        End If

        Dim sSendTime As String = Encoding.UTF8.GetString(RawBytes, SendTimePos, SendTimeLen)
        Dim oSendTime As DateTime
        If DateTime.TryParseExact(sSendTime, SendTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, oSendTime) = False Then
            Log.Error("SendTime is invalid (not a time).")
            Return NakCauseCode.TelegramError
        End If

        'ここ以降、プロパティにアクセス可能。

        Return NakCauseCode.None
    End Function

    'ボディ部の書式違反をチェックするメソッド
    'NOTE: ダミーです。殆どのサブクラスがオーバライドする想定です。
    Public Overridable Function GetBodyFormatViolation() As NakCauseCode Implements ITelegram.GetBodyFormatViolation
        Return NakCauseCode.None
    End Function

    '渡された電文の種類が同じであるか判定するメソッド
    Public Function IsSameKindWith(ByVal iTeleg As ITelegram) As Boolean Implements ITelegram.IsSameKindWith
        Dim oTeleg As NkTelegram = DirectCast(iTeleg, NkTelegram)
        If Me.SeqCode <> oTeleg.SeqCode Then Return False
        If Me.CmdCode <> oTeleg.CmdCode Then Return False
        Return True
    End Function

    'バイト列生成メソッド
    'NOTE: データ部がファイルで指定されている場合はNothingを返却する。
    Public Function GetBytes() As Byte()
        If ObjFilePathList IsNot Nothing Then Return Nothing
        Dim telegLen As Integer = ObjPos + CInt(Utility.GetUInt32FromLeBytes4(RawBytes, ObjSizePos))
        Dim aBytes As Byte() = New Byte(telegLen - 1) {}
        Buffer.BlockCopy(RawBytes, 0, aBytes, 0, telegLen)
        Return aBytes
    End Function

    'ストリームへの出力メソッド
    'NOTE: このメソッドは、oStreamが指すインスタンスのWriteメソッドが
    'スローし得る全ての例外をスローし得る。呼び元は、それらの例外のうち、
    'プログラム内部の不整合でしか発生し得ない例外のみを予期せぬ問題
    'として扱うべきである。たとえば、ソフトウェアでハンドリング可能な
    'ハードウェアの異常が何らかのExceptionとしてスローされることがあり、
    'それをアプリケーションで処理する必要がある（たとえば、他の処理を
    '継続する必要があるあるいは、すぐに落ちるのではなく、何かしら行う
    '必要がある）なら、それは予期すべき例外であり、選別可能な方法で
    'Catchしなければならない。
    Public Sub WriteToStream(ByVal oStream As Stream)
        Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(RawBytes, ObjSizePos)
        Dim rawBytesValidLen As Integer = ObjPos
        If ObjFilePathList Is Nothing Then
            rawBytesValidLen += CInt(objSize)
        Else
            rawBytesValidLen += CInt(objSize - ObjFilesCombinedLen)
        End If

        oStream.Write(RawBytes, 0, rawBytesValidLen)
        If ObjFilePathList IsNot Nothing Then
            Dim bufferLen As Integer = 16384  'NOTE: チューニングの余地あり
            Dim aBuffer(bufferLen - 1) As Byte
            For listIndex As Integer = 0 To ObjFilePathList.Count - 1
                Using oInputStream As New FileStream(ObjFilePathList(listIndex), FileMode.Open, FileAccess.Read)
                    'NOTE: .NET Framework 4以上が前提になったら、Stream.CopyToを使うべき。
                    'NOTE: WriteToSocketメソッドと同じく、ObjFilesCombinedLenは
                    '無視して、ファイルの終端までを読み取って書き込む。
                    'これは、ObjFilesCombinedLenに設定されている値とファイルの合計サイズが
                    '等しいことを前提にした実装である。
                    'つまり、この電文インスタンスを作成する際のObjFilesCombinedLenの算出
                    '時点からObjFilePathListが示すファイルの内容を変更してはならない。
                    Do
                        Dim readSize As Integer = oInputStream.Read(aBuffer, 0, bufferLen)
                        If readSize = 0 Then Exit Do
                        oStream.Write(aBuffer, 0, readSize)
                    Loop
                End Using
            Next listIndex
        End If
    End Sub

    'ソケットへの出力メソッド（別のソケットの読み取り監視つき）
    'NOTE: timeoutBaseTicksに0または-1を指定すると無期限待機となる。
    'NOTE: oInterruptSocketListに登録されているソケットが読み取り可能になった際は、
    'oSocketへの書き込みを中止する。oInterruptSocketListには、アプリケーションの設計
    'に応じて、リスニングソケットや対親スレッド通信用ソケットを登録しておくとよい。
    '前者を登録する場合、呼び元のスレッドがリスニングソケットも管理することが前提になる。
    '機器構成の変化やユーザによる終了操作があった場合でも、この電文の送信を中止したく
    'ないなら、前者のみを登録しておくことで、対応が可能である。
    'ただし、応答電文受信待ちの間も同様の配慮が必要になるし、通信相手からの新たな接続が
    '期待できないケースがあるならば、このメソッドから必ず抜け出せるようにするためには、
    '無期限でないタイマ値を指定することが必須になるため、あまりよい対応とは言えない。
    'それを踏まえると、新たなコネクションを親スレッドからメッセージで受け取る場合に
    '限らず、後者の登録はほぼ必須と言える。ただし、あらゆるメッセージを同一のソケットで
    '受信するのであれば。あらゆるメッセージの受信時に、この電文の送信を途中でやめる
    '（つまり、電文用のコネクションを継続使用不可能な状態で放棄する）ことになるので、
    '注意が必要である。必要に応じて、各種メッセージを（緊急度ごとに）異なるソケットで
    '受信するようにしたり、親スレッドが、通信状態をみて、終了要求メッセージを送信する
    'か否かを決めるなど、アプリレベルでの考慮は避けられない。
    Public Function WriteToSocketInterruptible( _
       ByVal oSocket As Socket, _
       ByVal oInterruptSocketList As IList, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As Boolean

        Dim oCheckWriteList As New ArrayList()

        Dim sSendTime As String = DateTime.Now.ToString(SendTimeFormat)
        Encoding.UTF8.GetBytes(sSendTime, 0, SendTimeLen, RawBytes, SendTimePos)

        Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(RawBytes, ObjSizePos)
        Dim rawBytesValidLen As Integer = ObjPos
        If ObjFilePathList Is Nothing Then
            rawBytesValidLen += CInt(objSize)
        Else
            rawBytesValidLen += CInt(objSize - ObjFilesCombinedLen)
        End If
        LogBytesToSend(RawBytes, rawBytesValidLen, telegLoggingMaxLength, CLng(ObjPos) + objSize)

        Dim allTicks As Integer = 0
        If timeoutBaseTicks > 0 Then
            allTicks = CInt(timeoutBaseTicks + (CLng(timeoutExtraTicksPerMiB) * objSize) \ 1048576)
        End If

        Dim oTimer As TickTimer = Nothing
        If allTicks > 0 Then
            oTimer = New TickTimer(allTicks)
            oTimer.Start(TickTimer.GetSystemTick())
        End If

        Dim wasSocketBlocking As Boolean = oSocket.Blocking
        oSocket.Blocking = False
        oSocket.SendTimeout = 0

        Dim listIndex As Integer = 0
        Dim oInputStream As FileStream = Nothing
        Dim bufferLen As Integer = 16384  'NOTE: チューニングの余地あり
        Dim aBuffer(bufferLen - 1) As Byte
        Dim bufferValidLen As Integer = bufferLen
        Dim nextSendPosInBuffer As Integer = bufferLen
        Dim nextReadPosInRawBytes As Integer = 0
        Dim isAllBytesRead As Boolean = False

        Try
            Do
                'aBufferから送信済みバイトを除去する。また、できる限り（読み出す
                'ものがどこかにある限り）、aBufferにバイトを詰め込む。
                'NOTE: このように、ブロックされた際、aBuffer全体を未送信バイトで
                '満たしておくのは、ブロックされる回数を最小化するためである。
                If nextSendPosInBuffer <> 0 Then
                    bufferValidLen = bufferValidLen - nextSendPosInBuffer
                    If bufferValidLen <> 0 Then
                        Array.Copy(aBuffer, nextSendPosInBuffer, aBuffer, 0, bufferValidLen)
                    End If
                    nextSendPosInBuffer = 0

                    Do
                        If nextReadPosInRawBytes < rawBytesValidLen Then
                            Dim rawBytesRestLen As Integer = rawBytesValidLen - nextReadPosInRawBytes
                            Dim bufferRestLen As Integer = bufferLen - bufferValidLen
                            Dim copyLen As Integer = If(rawBytesRestLen < bufferRestLen, rawBytesRestLen, bufferRestLen)
                            Buffer.BlockCopy(RawBytes, nextReadPosInRawBytes, aBuffer, bufferValidLen, copyLen)
                            nextReadPosInRawBytes += copyLen
                            bufferValidLen += copyLen
                        Else
                            If ObjFilePathList Is Nothing Then
                                isAllBytesRead = True
                                Exit Do
                            End If

                            If oInputStream Is Nothing Then
                                If listIndex >= ObjFilePathList.Count Then
                                    isAllBytesRead = True
                                    Exit Do
                                End If
                                oInputStream = New FileStream(ObjFilePathList(listIndex), FileMode.Open, FileAccess.Read)
                                listIndex += 1
                            End If

                            Dim readLen As Integer = oInputStream.Read(aBuffer, bufferValidLen, bufferLen - bufferValidLen)
                            If readLen = 0 Then
                                oInputStream.Close()
                                oInputStream = Nothing
                            End If

                            bufferValidLen += readLen
                        End If
                    Loop Until bufferValidLen = bufferLen
                End If

                'NOTE: ここでは、nextSendPosInBufferが必ず0である。
                'また、bufferValidLenが0の場合（上記のDo-Loopが一度も最後まで
                '実行されていない場合）、isAllBytesReadは必ずTrueである。
                'よって、下記の条件は「nextSendPosInBuffer = bufferValidLen
                'AndAlso isAllBytesRead」と同等である。
                If bufferValidLen = 0 Then Exit Do

                oCheckWriteList.Clear()
                oCheckWriteList.Add(oSocket)

                Try
                    'NOTE: 本当は、allTicksが「0」のケースでは、Socket.Selectに
                    '「-1」を渡すようにして、「無期限待機」をしたい。
                    'しかし、.NET Framework 3.5のSocket.Selectにはバグがあり、
                    '「-1」を指定した場合に即時復帰するようであるため、
                    'できるだけ長い時間（Integer.MaxValue）を指定した期限付き
                    'の待機にしておく。
                    Dim microSeconds As Integer = Integer.MaxValue
                    If allTicks > 0 Then
                        Dim restTicks As Long = oTimer.GetTicksToTimeout(TickTimer.GetSystemTick())
                        If restTicks < 1 Then
                            Log.Error("I'm through waiting for all bytes of the telegram to write.")
                            Return False
                        End If
                        If restTicks <= Integer.MaxValue \ 1000 Then
                            microSeconds = CInt(restTicks * 1000)
                        End If
                    End If

                    'NOTE: 本当は、このメソッドの呼び元が、要素数0のリストを渡さないよう配慮すべき。
                    If oInterruptSocketList.Count <> 0 Then
                        Socket.Select(oInterruptSocketList, oCheckWriteList, Nothing, microSeconds)
                        If oInterruptSocketList.Count > 0 Then
                            Log.Error("Interrupted.")
                            Return False
                        End If
                    Else
                        Socket.Select(Nothing, oCheckWriteList, Nothing, microSeconds)
                    End If

                    If oCheckWriteList.Count > 0 Then
                        'WOULD_BLOCKが発生するかaBufferの有効バイトを全て書き込むまで、
                        'ソケットへの書き込みを繰り返す。
                        While nextSendPosInBuffer < bufferValidLen
                            nextSendPosInBuffer _
                               += oSocket.Send( _
                                  aBuffer, _
                                  nextSendPosInBuffer, _
                                  bufferValidLen - nextSendPosInBuffer, _
                                  SocketFlags.None)
                        End While

                        'aBufferの有効バイトを全て書き込んだ場合のみ、ここが実行される。
                        'これ以上読み込むものがない場合は、メソッドを終了する。
                        If isAllBytesRead Then Exit Do
                    End If

                    'NOTE: aBufferに未送信バイトが無くなった場合だけでなく、
                    'Socket.Selectがタイムアウトした場合も、ここを通ることになる。
                    '後者の場合は、ここで即座にこのメソッドを終了してもよいが、
                    'ひとまずループの先頭に戻った後、Socket.Selectの手前で
                    'このメソッドを終了することにする。
                Catch ex As SocketException
                    Select Case ex.ErrorCode
                        '書き込みがブロックされた場合（WSAEWOULDBLOCK）または、
                        '指定した時間内に書き込みできなかった場合（WSAETIMEDOUT）
                        Case 10035, 10060
                            Continue Do

                        '発生したなら、アプリの不具合が要因である可能性が濃厚であるため、
                        'アプリを終了させて早めに（テスト中に）気付かせた方がよいエラー
                        'NOTE: 外部と通信を行うためのソケットを複数のスレッドから操作することは
                        'あり得ない（呼び元のバグである）という前提で、10036（WSAEINPROGRESS）
                        'もここにある。
                        Case 10009, 10013, 10014, 10022, 10036, 10037, 10038, _
                             10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                             10047, 10048, 10049, 10056, 10092, 10093
                            Throw

                        'Send()において、装置外要因や装置内の状況で発生しそうであるため、
                        'アプリを終了させるわけにはいかないと思われるエラー
                        Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                             10058, 10061, 10064, 10065, 10101
                            Log.Error("SocketException caught.", ex)
                            Return False

                        '発生し得ないはずであるが、将来どうなるかわからないため、
                        'アプリを終了させない方が無難と思われるエラー
                        Case Else
                            Log.Error("Surprising SocketException caught.", ex)
                            Return False
                    End Select
                End Try
            Loop
        Finally
            oSocket.Blocking = wasSocketBlocking
            If oInputStream IsNot Nothing Then
                oInputStream.Close()
            End If
        End Try
        Return True
    End Function

    'ソケットへの出力メソッド
    'NOTE: timeoutBaseTicksに0または-1を指定すると無期限待機となる。
    'NOTE: データ部がファイルで指定されている場合も送信可能であるが、そのような大きな電文を送信する
    '場合も、復帰の契機は電文送受信用ソケットへの書き込み成功・失敗かタイムアウトだけであり、他の
    '事象を契機に復帰することはできない（タイムアウト値はファイルの大きさに応じて動的に算出可能）。
    'サーバ側で送信期限を設けず、クライアントからの再コネクトで復帰したいのであれば、
    'WriteToSocketInterruptibleメソッドを使用すること。
    Public Function WriteToSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As Boolean Implements ITelegram.WriteToSocket

        Dim sSendTime As String = DateTime.Now.ToString(SendTimeFormat)
        Encoding.UTF8.GetBytes(sSendTime, 0, SendTimeLen, RawBytes, SendTimePos)

        Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(RawBytes, ObjSizePos)
        Dim rawBytesValidLen As Integer = ObjPos
        If ObjFilePathList Is Nothing Then
            rawBytesValidLen += CInt(objSize)
        Else
            rawBytesValidLen += CInt(objSize - ObjFilesCombinedLen)
        End If
        LogBytesToSend(RawBytes, rawBytesValidLen, telegLoggingMaxLength, CLng(ObjPos) + objSize)

        Debug.Assert(oSocket.Blocking)
        Dim allTicks As Integer = 0
        If timeoutBaseTicks > 0 Then
            allTicks = CInt(timeoutBaseTicks + (CLng(timeoutExtraTicksPerMiB) * objSize) \ 1048576)
        End If

        Try
            If ObjFilePathList Is Nothing Then
                oSocket.SendTimeout = allTicks
                oSocket.Send(RawBytes, rawBytesValidLen, SocketFlags.None)
            Else
                Dim aPreBuffer As Byte() = New Byte(rawBytesValidLen - 1) {}
                Buffer.BlockCopy(RawBytes, 0, aPreBuffer, 0, rawBytesValidLen)

                Dim oTimer As TickTimer = Nothing
                If allTicks > 0 Then
                    oTimer = New TickTimer(allTicks)
                    oTimer.Start(TickTimer.GetSystemTick())
                End If

                For listIndex As Integer = 0 To ObjFilePathList.Count - 1
                    Dim restTicks As Integer = 0
                    If allTicks > 0 Then
                        restTicks = CInt(oTimer.GetTicksToTimeout(TickTimer.GetSystemTick()))
                        If restTicks < 1 Then
                            Log.Error("I'm through waiting for all bytes of the telegram to write.")
                            Return False
                        End If
                    End If
                    oSocket.SendTimeout = restTicks
                    oSocket.SendFile(ObjFilePathList(listIndex), aPreBuffer, Nothing, TransmitFileOptions.UseDefaultWorkerThread)
                    aPreBuffer = Nothing
                Next listIndex
            End If
        Catch ex As SocketException
            Select Case ex.ErrorCode
                '指定した時間内に書き込みできなかった場合（WSAETIMEDOUT）
                'TODO: これじゃない気も（Socketクラスの実装次第？）
                Case 10060
                    Log.Error("I'm through waiting for all bytes of the telegram to write.", ex)
                    Return False

                '発生したなら、アプリの不具合が要因である可能性が濃厚であるため、
                'アプリを終了させて早めに（テスト中に）気付かせた方がよいエラー
                'NOTE: 外部と通信を行うためのソケットを複数のスレッドから操作することは
                'あり得ない（呼び元のバグである）という前提で、10036（WSAEINPROGRESS）
                'もここにある。
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    Throw

                'Send()において、装置外要因や装置内の状況で発生しそうであるため、
                'アプリを終了させるわけにはいかないと思われるエラー
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    Log.Error("SocketException caught.", ex)
                    Return False

                '発生し得ないはずであるが、将来どうなるかわからないため、
                'アプリを終了させない方が無難と思われるエラー
                Case Else
                    Log.Error("Surprising SocketException caught.", ex)
                    Return False
            End Select
        End Try
        Return True
    End Function

    Private Shared Sub LogBytesToSend(ByVal aBytes As Byte(), ByVal validLen As Integer, ByVal loggingMaxLen As Integer, ByVal telegLen As Long)
        If loggingMaxLen > 0 Then
            If validLen <= 0 Then
                Log.Info("Sending no byte...")
            Else
                Dim loggingLen As Integer = validLen
                If loggingLen > loggingMaxLen Then loggingLen = loggingMaxLen
                Log.Info("Sending " & telegLen.ToString() & " bytes...", aBytes, 0, loggingLen)
            End If
        End If
    End Sub
#End Region

End Class

'シーケンス識別コード
Public Enum NkSeqCode As Integer
    None
    Collection
    Delivery
    Test
End Enum

'コマンド識別コード
Public Enum NkCmdCode As Integer
    None
    ComStartReq
    ComStartAck
    InquiryReq
    InquiryAck
    DataPostReq
    DataPostAck
    ComStopReq
    ComStopAck
End Enum
