' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Net.Sockets
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �d���B
''' </summary>
''' <remarks>
''' �����܂ŁA�e��d���N���X�̎����̈ꕔ���s���邽�߂̃N���X�ł���B
''' ����āA���̃N���X���̂̃C���X�^���X���쐬���邱�Ƃ͕s�\�ł���B
''' </remarks>
Public Class EkTelegram
    Implements ITelegram

#Region "�萔"
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

#Region "�ϐ�"
    Protected Gene As EkTelegramGene
    Protected RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
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

#Region "�R���X�g���N�^�i�T�u�N���X�̃R���X�g���N�^�̎����p�j"
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

    'iTeleg�̎��̂�EkTelegram�ł��邱�Ƃ�O��Ƃ��郁�\�b�h�ł��B
    '������g�����������ꍇ�́AInvalidCastException���X���[����܂��B
    Protected Sub New(ByVal oGene As EkTelegramGene, ByVal iTeleg As ITelegram)
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If oTeleg.Gene.GetType() Is oGene.GetType() Then
            Me.Gene = oGene
            Me.RawBytes = oTeleg.RawBytes
        Else
            '���݂̂Ƃ���A�����̈قȂ�d���̓��e�𑊌݂ɃR�s�[����K�v����
            '�Ȃ��̂ŃG���[�Ƃ���B
            Debug.Fail("This case has not been supported yet.")
        End If
    End Sub

    'iTeleg�̎��̂�EkTelegram�ł��邱�Ƃ�O��Ƃ��郁�\�b�h�ł��B
    '������g�����������ꍇ�́AInvalidCastException���X���[����܂��B
    Protected Sub New(ByVal iTeleg As ITelegram)
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        Me.Gene = oTeleg.Gene
        Me.RawBytes = oTeleg.RawBytes
    End Sub
#End Region

#Region "�T�u�N���X�����p���\�b�h"
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

#Region "���\�b�h"
    'NAK�d���𐶐����郁�\�b�h
    Private Function CreateINakTelegram(ByVal causeCode As NakCauseCode) As INakTelegram Implements ITelegram.CreateNakTelegram
        Return New EkNakTelegram(Gene, Me, causeCode)
    End Function

    'NAK�d���𐶐����郁�\�b�h
    Public Function CreateNakTelegram(ByVal causeCode As NakCauseCode) As EkNakTelegram
        Return New EkNakTelegram(Gene, Me, causeCode)
    End Function

    'NAK�d���𐶐����郁�\�b�h
    Public Function CreateNakTelegram(ByVal causeNumber As Integer, ByVal sCauseText As String) As EkNakTelegram
        Return New EkNakTelegram(Gene, Me, causeNumber, sCauseText)
    End Function

    '�w�b�_���̏����ᔽ���`�F�b�N���郁�\�b�h
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

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function

    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    'NOTE: �_�~�[�ł��B�w�ǂ̃T�u�N���X���I�[�o���C�h����z��ł��B
    Public Overridable Function GetBodyFormatViolation() As NakCauseCode Implements ITelegram.GetBodyFormatViolation
        Return EkNakCauseCode.None
    End Function

    '�n���ꂽ�d���̎�ނ������ł��邩���肷�郁�\�b�h
    Public Function IsSameKindWith(ByVal iTeleg As ITelegram) As Boolean Implements ITelegram.IsSameKindWith
        Dim oTeleg As EkTelegram = DirectCast(iTeleg, EkTelegram)
        If Me.CmdCode <> oTeleg.CmdCode Then Return False
        If Me.SubCmdCode <> oTeleg.SubCmdCode Then Return False
        If Me.ObjCode <> oTeleg.ObjCode Then Return False
        Return True
    End Function

    '�o�C�g�񐶐����\�b�h
    Public Function GetBytes() As Byte()
        Dim telegLen As Integer = Gene.GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(RawBytes, Gene.ObjSizePos))
        Dim aBytes As Byte() = New Byte(telegLen - 1) {}
        Buffer.BlockCopy(RawBytes, 0, aBytes, 0, telegLen)
        Return aBytes
    End Function

    '�X�g���[���ւ̏o�̓��\�b�h
    'NOTE: ���̃��\�b�h�́AoStream���w���C���X�^���X��Write���\�b�h��
    '�X���[������S�Ă̗�O���X���[������B�Ăь��́A�����̗�O�̂����A
    '�v���O���������̕s�����ł������������Ȃ���O�݂̂�\�����ʖ��
    '�Ƃ��Ĉ����ׂ��ł���B���Ƃ��΁A�\�t�g�E�F�A�Ńn���h�����O�\��
    '�n�[�h�E�F�A�ُ̈킪���炩��Exception�Ƃ��ăX���[����邱�Ƃ�����A
    '������A�v���P�[�V�����ŏ�������K�v������i���Ƃ��΁A���̏�����
    '�p������K�v�����邠�邢�́A�����ɗ�����̂ł͂Ȃ��A��������s��
    '�K�v������j�Ȃ�A����͗\�����ׂ���O�ł���A�I�ʉ\�ȕ��@��
    'Catch���Ȃ���΂Ȃ�Ȃ��B
    Public Sub WriteToStream(ByVal oStream As Stream)
        Dim telegLen As Integer = Gene.GetRawLenByObjSize(Utility.GetUInt32FromLeBytes4(RawBytes, Gene.ObjSizePos))
        oStream.Write(RawBytes, 0, telegLen)
    End Sub

    '�\�P�b�g�ւ̏o�̓��\�b�h
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
                '�w�肵�����ԓ��ɏ������݂ł��Ȃ������ꍇ�iWSAETIMEDOUT�j
                'TODO: ���ꂶ��Ȃ��C���iSocket�N���X�̎�������H�j
                Case 10060
                    Log.Error("I'm through waiting for all bytes of the telegram to write.", ex)
                    Return False

                '���������Ȃ�A�A�v���̕s����v���ł���\�����Z���ł��邽�߁A
                '�A�v�����I�������đ��߂Ɂi�e�X�g���Ɂj�C�t�����������悢�G���[
                'NOTE: �O���ƒʐM���s�����߂̃\�P�b�g�𕡐��̃X���b�h���瑀�삷�邱�Ƃ�
                '���蓾�Ȃ��i�Ăь��̃o�O�ł���j�Ƃ����O��ŁA10036�iWSAEINPROGRESS�j
                '�������ɂ���B
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    Throw

                'Send()�ɂ����āA���u�O�v���⑕�u���̏󋵂Ŕ����������ł��邽�߁A
                '�A�v�����I��������킯�ɂ͂����Ȃ��Ǝv����G���[
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    Log.Error("SocketException caught.", ex)
                    Return False

                '���������Ȃ��͂��ł��邪�A�����ǂ��Ȃ邩�킩��Ȃ����߁A
                '�A�v�����I�������Ȃ���������Ǝv����G���[
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

'�R�}���h�R�[�h
Public Enum EkCmdCode As Integer
    None
    Req
    Ack
    Nak
End Enum

'�T�u�R�}���h�R�[�h
Public Enum EkSubCmdCode As Integer
    None
    [Get]
    Post
End Enum
