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
Public Class EkTelegram
    Implements ITelegram

#Region "定数"
    Private Shared ReadOnly aCmdCodeREQ() As Byte = {Asc("R"), Asc("E"), Asc("Q")}
    Private Shared ReadOnly aCmdCodeACK() As Byte = {Asc("A"), Asc("C"), Asc("K")}
    Private Shared ReadOnly aCmdCodeNAK() As Byte = {Asc("N"), Asc("A"), Asc("K")}
    Private Shared ReadOnly aSubCmdCodeGET() As Byte = {Asc("G"), Asc("E"), Asc("T"), 0}
    Private Shared ReadOnly aSubCmdCodePOST() As Byte = {Asc("P"), Asc("O"), Asc("S"), Asc("T")}

    Private Shared ReadOnly oRawCmdCodeTable As New Dictionary(Of EkCmdCode, Byte()) From { _
       {EkCmdCode.Req, aCmdCodeREQ}, _
       {EkCmdCode.Ack, aCmdCodeACK}, _
       {EkCmdCode.Nak, aCmdCodeNAK}}
    Private Shared ReadOnly oCmdCodeTable As New Dictionary(Of UInteger, EkCmdCode) From { _
       {Utility.GetUInt32FromLeBytes3(aCmdCodeREQ, 0), EkCmdCode.Req}, _
       {Utility.GetUInt32FromLeBytes3(aCmdCodeACK, 0), EkCmdCode.Ack}, _
       {Utility.GetUInt32FromLeBytes3(aCmdCodeNAK, 0), EkCmdCode.Nak}}

    Private Shared ReadOnly oRawSubCmdCodeTable As New Dictionary(Of EkSubCmdCode, Byte()) From { _
       {EkSubCmdCode.Get, aSubCmdCodeGET}, _
       {EkSubCmdCode.Post, aSubCmdCodePOST}}
    Private Shared ReadOnly oSubCmdCodeTable As New Dictionary(Of UInteger, EkSubCmdCode) From { _
       {Utility.GetUInt32FromLeBytes4(aSubCmdCodeGET, 0), EkSubCmdCode.Get}, _
       {Utility.GetUInt32FromLeBytes4(aSubCmdCodePOST, 0), EkSubCmdCode.Post}}

    Private Shared ReadOnly oCmdKindTable As New Dictionary(Of UInteger, CmdKind) From { _
       {Utility.GetUInt32FromLeBytes3(aCmdCodeREQ, 0), CmdKind.Req}, _
       {Utility.GetUInt32FromLeBytes3(aCmdCodeACK, 0), CmdKind.Ack}, _
       {Utility.GetUInt32FromLeBytes3(aCmdCodeNAK, 0), CmdKind.Nak}}
#End Region

#Region "変数"
    Protected Gene As EkTelegramGene
    Protected RawBytes As Byte()
#End Region

#Region "プロパティ"
    Public Property CmdCode() As EkCmdCode
        Get
            Dim code As EkCmdCode
            If oCmdCodeTable.TryGetValue(Utility.GetUInt32FromLeBytes3(RawBytes, Gene.CmdCodePos), code) = False Then
                code = EkCmdCode.None
            End If
            Return code
        End Get

        Set(ByVal code As EkCmdCode)
            Dim rawCmdCode As Byte() = oRawCmdCodeTable(code)
            Buffer.BlockCopy(rawCmdCode, 0, RawBytes, Gene.CmdCodePos, rawCmdCode.Length)
        End Set
    End Property

    Public Property SubCmdCode() As EkSubCmdCode
        Get
            Dim code As EkSubCmdCode
            If oSubCmdCodeTable.TryGetValue(Utility.GetUInt32FromLeBytes4(RawBytes, Gene.SubCmdCodePos), code) = False Then
                code = EkSubCmdCode.None
            End If
            Return code
        End Get

        Set(ByVal code As EkSubCmdCode)
            Dim rawSubCmdCode As Byte() = oRawSubCmdCodeTable(code)
            Buffer.BlockCopy(rawSubCmdCode, 0, RawBytes, Gene.SubCmdCodePos, rawSubCmdCode.Length)
        End Set
    End Property

    Public Property ReqNumber() As Integer
        Get
            Return Utility.GetIntFromDecimalAsciiBytes(RawBytes, Gene.ReqNumberPos, Gene.ReqNumberLen)
        End Get

        Set(ByVal reqNumber As Integer)
            Utility.CopyIntToDecimalAsciiBytes(reqNumber, RawBytes, Gene.ReqNumberPos, Gene.ReqNumberLen)
        End Set
    End Property

    Public Property ClientCode() As EkCode
        Get
            Dim code As EkCode
            code.Model = Utility.GetIntFromDecimalAsciiBytes(RawBytes, Gene.ClientModelCodePos, Gene.ClientModelCodeLen)
            code.RailSection = Utility.GetIntFromDecimalAsciiBytes(RawBytes, Gene.ClientRailSectionCodePos, Gene.ClientRailSectionCodeLen)
            code.StationOrder = Utility.GetIntFromDecimalAsciiBytes(RawBytes, Gene.ClientStationOrderCodePos, Gene.ClientStationOrderCodeLen)
            code.Corner = Utility.GetIntFromDecimalAsciiBytes(RawBytes, Gene.ClientCornerCodePos, Gene.ClientCornerCodeLen)
            code.Unit = Utility.GetIntFromDecimalAsciiBytes(RawBytes, Gene.ClientUnitCodePos, Gene.ClientUnitCodeLen)
            Return code
        End Get

        Set(ByVal clientCode As EkCode)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.Model, RawBytes, Gene.ClientModelCodePos, Gene.ClientModelCodeLen)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.RailSection, RawBytes, Gene.ClientRailSectionCodePos, Gene.ClientRailSectionCodeLen)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.StationOrder, RawBytes, Gene.ClientStationOrderCodePos, Gene.ClientStationOrderCodeLen)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.Corner, RawBytes, Gene.ClientCornerCodePos, Gene.ClientCornerCodeLen)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.Unit, RawBytes, Gene.ClientUnitCodePos, Gene.ClientUnitCodeLen)
        End Set
    End Property

    Public Property SendTime() As DateTime
        Get
            Dim sSendTime As String = Encoding.UTF8.GetString(RawBytes, Gene.SendTimePos, Gene.SendTimeLen)
            Return DateTime.ParseExact(sSendTime, Gene.SendTimeFormat, CultureInfo.InvariantCulture)
        End Get

        Set(ByVal sendTime As DateTime)
            Dim sSendTime As String = sendTime.ToString(Gene.SendTimeFormat)
            Encoding.UTF8.GetBytes(sSendTime, 0, Gene.SendTimeLen, RawBytes, Gene.SendTimePos)
        End Set
    End Property

    Public Property ObjCode() As Integer
        Get
            Return RawBytes(Gene.ObjCodePos)
        End Get

        Set(ByVal objCode As Integer)
            RawBytes(Gene.ObjCodePos) = CType(objCode, Byte)
        End Set
    End Property

    Public Property RawCmdCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(Gene.CmdCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, Gene.CmdCodePos, ret, 0, Gene.CmdCodeLen)
            Return ret
        End Get

        Set(ByVal rawCmdCode As Byte())
            Buffer.BlockCopy(rawCmdCode, 0, RawBytes, Gene.CmdCodePos, Gene.CmdCodeLen)
        End Set
    End Property

    Public Property RawSubCmdCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(Gene.SubCmdCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, Gene.SubCmdCodePos, ret, 0, Gene.SubCmdCodeLen)
            Return ret
        End Get

        Set(ByVal rawSubCmdCode As Byte())
            Buffer.BlockCopy(rawSubCmdCode, 0, RawBytes, Gene.SubCmdCodePos, Gene.SubCmdCodeLen)
        End Set
    End Property

    Public Property RawReqNumber() As Byte()
        Get
            Dim ret As Byte() = New Byte(Gene.ReqNumberLen - 1) {}
            Buffer.BlockCopy(RawBytes, Gene.ReqNumberPos, ret, 0, Gene.ReqNumberLen)
            Return ret
        End Get

        Set(ByVal rawReqNumber As Byte())
            Buffer.BlockCopy(rawReqNumber, 0, RawBytes, Gene.ReqNumberPos, Gene.ReqNumberLen)
        End Set
    End Property

    Public Property RawClientCode() As Byte()
        Get
            Dim pos As Integer = Gene.ClientModelCodePos
            Dim len As Integer = Gene.ClientModelCodeLen + Gene.ClientRailSectionCodeLen + Gene.ClientStationOrderCodeLen + Gene.ClientCornerCodeLen + Gene.ClientUnitCodeLen
            Dim ret As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, pos, ret, 0, len)
            Return ret
        End Get

        Set(ByVal rawClientCode As Byte())
            Dim pos As Integer = Gene.ClientModelCodePos
            Dim len As Integer = Gene.ClientModelCodeLen + Gene.ClientRailSectionCodeLen + Gene.ClientStationOrderCodeLen + Gene.ClientCornerCodeLen + Gene.ClientUnitCodeLen
            Buffer.BlockCopy(rawClientCode, 0, RawBytes, pos, len)
        End Set
    End Property

    Public Property RawSendTime() As Byte()
        Get
            Dim ret As Byte() = New Byte(Gene.SendTimeLen - 1) {}
            Buffer.BlockCopy(RawBytes, Gene.SendTimePos, ret, 0, Gene.SendTimeLen)
            Return ret
        End Get

        Set(ByVal rawSendTime As Byte())
            Buffer.BlockCopy(rawSendTime, 0, RawBytes, Gene.SendTimePos, Gene.SendTimeLen)
        End Set
    End Property

    Public Property RawObjCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(Gene.ObjCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, Gene.ObjCodePos, ret, 0, Gene.ObjCodeLen)
            Return ret
        End Get

        Set(ByVal rawObjCode As Byte())
            Buffer.BlockCopy(rawObjCode, 0, RawBytes, Gene.ObjCodePos, Gene.ObjCodeLen)
        End Set
    End Property

    Public Property ObjSize() As UInteger
        Get
            Return Utility.GetUInt32FromLeBytes4(RawBytes, Gene.ObjSizePos)
        End Get

        Set(ByVal objSize As UInteger)
            Utility.CopyUInt32ToLeBytes4(objSize, RawBytes, Gene.ObjSizePos)
        End Set
    End Property

    Public ReadOnly Property CmdKind() As CmdKind Implements ITelegram.CmdKind
        Get
            Dim kind As CmdKind
            If oCmdKindTable.TryGetValue(Utility.GetUInt32FromLeBytes3(RawBytes, Gene.CmdCodePos), kind) = False Then
                kind = CmdKind.None
            End If
            Return kind
        End Get
    End Property
#End Region

#Region "コンストラクタ（サブクラスのコンストラクタの実装用）"
    Protected Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal cmdCode As EkCmdCode, _
       ByVal subCmdCode As EkSubCmdCode, _
       ByVal objCode As Integer, _
       ByVal objDetailLen As Integer)

        Me.Gene = oGene

        Dim objSize As UInteger = Gene.GetObjSizeByObjDetailLen(objDetailLen)
        Dim telegSize As Integer = Gene.GetRawLenByObjSize(objSize)
        Me.RawBytes = New Byte(telegSize - 1) {}

        Buffer.BlockCopy(oRawCmdCodeTable(cmdCode), 0, Me.RawBytes, Gene.CmdCodePos, Gene.CmdCodeLen)
        Buffer.BlockCopy(oRawSubCmdCodeTable(subCmdCode), 0, Me.RawBytes, Gene.SubCmdCodePos, Gene.SubCmdCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ReqNumberPos, Gene.ReqNumberLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientModelCodePos, Gene.ClientModelCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientRailSectionCodePos, Gene.ClientRailSectionCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientStationOrderCodePos, Gene.ClientStationOrderCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientCornerCodePos, Gene.ClientCornerCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientUnitCodePos, Gene.ClientUnitCodeLen)
        Me.SendTime = DateTime.MinValue
        Utility.CopyUInt32ToLeBytes4(objSize, Me.RawBytes, Gene.ObjSizePos)
        Me.RawBytes(Gene.ObjCodePos) = CType(objCode, Byte)
    End Sub

    Protected Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal cmdCode As EkCmdCode, _
       ByVal aRawSubCmdCode As Byte(), _
       ByVal aRawObjCode As Byte(), _
       ByVal objDetailLen As Integer)

        Me.Gene = oGene

        Dim objSize As UInteger = Gene.GetObjSizeByObjDetailLen(objDetailLen)
        Dim telegSize As Integer = Gene.GetRawLenByObjSize(objSize)
        Me.RawBytes = New Byte(telegSize - 1) {}

        Buffer.BlockCopy(oRawCmdCodeTable(cmdCode), 0, Me.RawBytes, Gene.CmdCodePos, Gene.CmdCodeLen)
        Buffer.BlockCopy(aRawSubCmdCode, 0, Me.RawBytes, Gene.SubCmdCodePos, Gene.SubCmdCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ReqNumberPos, Gene.ReqNumberLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientModelCodePos, Gene.ClientModelCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientRailSectionCodePos, Gene.ClientRailSectionCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientStationOrderCodePos, Gene.ClientStationOrderCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientCornerCodePos, Gene.ClientCornerCodeLen)
        Utility.FillBytes(CByte(Asc("0")), Me.RawBytes, Gene.ClientUnitCodePos, Gene.ClientUnitCodeLen)
        Me.SendTime = DateTime.MinValue
        Utility.CopyUInt32ToLeBytes4(objSize, Me.RawBytes, Gene.ObjSizePos)
        Buffer.BlockCopy(aRawObjCode, 0, Me.RawBytes, Gene.ObjCodePos, Gene.ObjCodeLen)
    End Sub

    Protected Sub New(ByVal oGene As EkTelegramGene, ByVal aRawBytes As Byte())
        Me.Gene = oGene
        Me.RawBytes = aRawBytes
    End Sub

    'iTelegの実体がEkTelegramであることを前提とするメソッドです。
    '誤った使い方をした場合は、InvalidCastExceptionがスローされます。
    Protected Sub New(ByVal oGene As EkTelegramGene, ByVal iTeleg As ITelegram)
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.Gene.GetType() Is oGene.GetType() Then
            Me.Gene = oGene
            Me.RawBytes = oTeleg.RawBytes
        Else
            '現在のところ、書式の異なる電文の内容を相互にコピーする必要性が
            'ないのでエラーとする。
            Debug.Fail("This case has not been supported yet.")
        End If
    End Sub

    'iTelegの実体がEkTelegramであることを前提とするメソッドです。
    '誤った使い方をした場合は、InvalidCastExceptionがスローされます。
    Protected Sub New(ByVal iTeleg As ITelegram)
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        Me.Gene = oTeleg.Gene
        Me.RawBytes = oTeleg.RawBytes
    End Sub
#End Region

#Region "サブクラス実装用メソッド"
    Protected Function GetRawPos(ByVal posByObjDetail As Integer) As Integer
        Return Gene.ObjDetailPos + posByObjDetail
    End Function

    Protected Function GetObjDetailLen() As Integer
        Return Gene.GetObjDetailLenByObjSize(Utility.GetUInt32FromLeBytes4(RawBytes, Gene.ObjSizePos))
    End Function

    Protected Function GetXllBasePath() As String
        Return Gene.XllBasePath
    End Function
#End Region

#Region "メソッド"
    'NAK電文を生成するメソッド
    Private Function CreateINakTelegram(ByVal causeCode As NakCauseCode) As INakTelegram Implements ITelegram.CreateNakTelegram
        Return New EkNakTelegram(Gene, Me, causeCode)
    End Function

    'NAK電文を生成するメソッド
    Public Function CreateNakTelegram(ByVal causeCode As NakCauseCode) As EkNakTelegram
        Return New EkNakTelegram(Gene, Me, causeCode)
    End Function

    'NAK電文を生成するメソッド
    Public Function CreateNakTelegram(ByVal causeNumber As Integer, ByVal sCauseText As String) As EkNakTelegram
        Return New EkNakTelegram(Gene, Me, causeNumber, sCauseText)
    End Function

    'ヘッダ部の書式違反をチェックするメソッド
    Public Function GetHeaderFormatViolation() As NakCauseCode Implements ITelegram.GetHeaderFormatViolation
        If Not Utility.IsDecimalAsciiBytes(RawBytes, Gene.ReqNumberPos, Gene.ReqNumberLen) Then
            Log.Error("ReqNumber is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, Gene.ClientModelCodePos, Gene.ClientModelCodeLen) Then
            Log.Error("ClientModelCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, Gene.ClientRailSectionCodePos, Gene.ClientRailSectionCodeLen) Then
            Log.Error("ClientRailSectionCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, Gene.ClientStationOrderCodePos, Gene.ClientStationOrderCodeLen) Then
            Log.Error("ClientStationOrderCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, Gene.ClientCornerCodePos, Gene.ClientCornerCodeLen) Then
            Log.Error("ClientCornerCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, Gene.ClientUnitCodePos, Gene.ClientUnitCodeLen) Then
            Log.Error("ClientUnitCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytesFixed(RawBytes, Gene.SendTimePos, Gene.SendTimeLen) Then
            Log.Error("SendTime is invalid (not decimal ASCII bytes).")
            Return EkNakCauseCode.TelegramError
        End If

        Dim sSendTime As String = Encoding.UTF8.GetString(RawBytes, Gene.SendTimePos, Gene.SendTimeLen)
        Dim oSendTime As DateTime
        If DateTime.TryParseExact(sSendTime, Gene.SendTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, oSendTime) = False Then
            Log.Error("SendTime is invalid (not a time).")
            Return EkNakCauseCode.TelegramError
        End If

        'ここ以降、プロパティにアクセス可能。

        Return EkNakCauseCode.None
    End Function

    'ボディ部の書式違反をチェックするメソッド
    'NOTE: ダミーです。殆どのサブクラスがオーバライドする想定です。
    Public Overridable Function GetBodyFormatViolation() As NakCauseCode Implements ITelegram.GetBodyFormatViolation
        Return EkNakCauseCode.None
    End Function

    '渡された電文の種類が同じであるか判定するメソッド
    Public Function IsSameKindWith(ByVal iTeleg As ITelegram) As Boolean Implements ITelegram.IsSameKindWith
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If Me.CmdCode <> oTeleg.CmdCode Then Return False
        If Me.SubCmdCode <> oTeleg.SubCmdCode Then Return False
        If Me.ObjCode <> oTeleg.ObjCode Then Return False
        Return True
    End Function

    'バイト列生成メソッド
    Public Function GetBytes() As Byte()
        Dim telegLen As Integer = Gene.GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(RawBytes, Gene.ObjSizePos))
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
        Dim telegLen As Integer = Gene.GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(RawBytes, Gene.ObjSizePos))
        oStream.Write(RawBytes, 0, telegLen)
    End Sub

    'ソケットへの出力メソッド
    Public Function WriteToSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As Boolean Implements ITelegram.WriteToSocket

        Dim sSendTime As String = DateTime.Now.ToString(Gene.SendTimeFormat)
        Encoding.UTF8.GetBytes(sSendTime, 0, Gene.SendTimeLen, RawBytes, Gene.SendTimePos)

        Gene.UpdateCrc(RawBytes)

        Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(RawBytes, Gene.ObjSizePos)
        Dim telegLen As Integer = Gene.GetRawLenByObjSize(objSize)
        LogBytesToSend(RawBytes, telegLen, telegLoggingMaxLength)

        Debug.Assert(oSocket.Blocking)
        Dim allTicks As Integer = 0
        If timeoutBaseTicks > 0 Then
            allTicks = CInt(timeoutBaseTicks + (CLng(timeoutExtraTicksPerMiB) * objSize) \ 1048576)
        End If

        Try
            oSocket.SendTimeout = allTicks
            oSocket.Send(RawBytes, telegLen, SocketFlags.None)
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

    Private Shared Sub LogBytesToSend(ByVal aBytes As Byte(), ByVal validLen As Integer, ByVal loggingMaxLen As Integer)
        If loggingMaxLen > 0 Then
            If validLen <= 0 Then
                Log.Info("Sending no byte...")
            Else
                Dim loggingLen As Integer = validLen
                If loggingLen > loggingMaxLen Then loggingLen = loggingMaxLen
                Log.Info("Sending " & validLen.ToString() & " bytes...", aBytes, 0, loggingLen)
            End If
        End If
    End Sub
#End Region

End Class

'コマンドコード
Public Enum EkCmdCode As Integer
    None
    Req
    Ack
    Nak
End Enum

'サブコマンドコード
Public Enum EkSubCmdCode As Integer
    None
    [Get]
    Post
End Enum
