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
Public Class NkTelegram
    Implements ITelegram

#Region "�萔"
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

#Region "�ϐ�"
    Protected RawBytes As Byte()
    Protected ObjFilePathList As List(Of String)
    Protected ObjFilesCombinedLen As UInteger
#End Region

#Region "�v���p�e�B"
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

#Region "�R���X�g���N�^�i�T�u�N���X�̃R���X�g���N�^�̎����p�j"
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

    'iTeleg�̎��̂�NkTelegram�ł��邱�Ƃ�O��Ƃ��郁�\�b�h�ł��B
    '������g�����������ꍇ�́AInvalidCastException���X���[����܂��B
    Protected Sub New(ByVal iTeleg As ITelegram)
        Dim oTeleg As NkTelegram = DirectCast(iTeleg, NkTelegram)
        Me.RawBytes = oTeleg.RawBytes
        Me.ObjFilePathList = oTeleg.ObjFilePathList
        Me.ObjFilesCombinedLen = oTeleg.ObjFilesCombinedLen
    End Sub
#End Region

#Region "���\�b�h"
    'NAK�d���𐶐����郁�\�b�h
    Private Function CreateINakTelegram(ByVal causeCode As NakCauseCode) As INakTelegram Implements ITelegram.CreateNakTelegram
        Return Nothing
    End Function

    '�w�b�_���̏����ᔽ���`�F�b�N���郁�\�b�h
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

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return NakCauseCode.None
    End Function

    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    'NOTE: �_�~�[�ł��B�w�ǂ̃T�u�N���X���I�[�o���C�h����z��ł��B
    Public Overridable Function GetBodyFormatViolation() As NakCauseCode Implements ITelegram.GetBodyFormatViolation
        Return NakCauseCode.None
    End Function

    '�n���ꂽ�d���̎�ނ������ł��邩���肷�郁�\�b�h
    Public Function IsSameKindWith(ByVal iTeleg As ITelegram) As Boolean Implements ITelegram.IsSameKindWith
        Dim oTeleg As NkTelegram = DirectCast(iTeleg, NkTelegram)
        If Me.SeqCode <> oTeleg.SeqCode Then Return False
        If Me.CmdCode <> oTeleg.CmdCode Then Return False
        Return True
    End Function

    '�o�C�g�񐶐����\�b�h
    'NOTE: �f�[�^�����t�@�C���Ŏw�肳��Ă���ꍇ��Nothing��ԋp����B
    Public Function GetBytes() As Byte()
        If ObjFilePathList IsNot Nothing Then Return Nothing
        Dim telegLen As Integer = ObjPos + CInt(Utility.GetUInt32FromLeBytes4(RawBytes, ObjSizePos))
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
        Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(RawBytes, ObjSizePos)
        Dim rawBytesValidLen As Integer = ObjPos
        If ObjFilePathList Is Nothing Then
            rawBytesValidLen += CInt(objSize)
        Else
            rawBytesValidLen += CInt(objSize - ObjFilesCombinedLen)
        End If

        oStream.Write(RawBytes, 0, rawBytesValidLen)
        If ObjFilePathList IsNot Nothing Then
            Dim bufferLen As Integer = 16384  'NOTE: �`���[�j���O�̗]�n����
            Dim aBuffer(bufferLen - 1) As Byte
            For listIndex As Integer = 0 To ObjFilePathList.Count - 1
                Using oInputStream As New FileStream(ObjFilePathList(listIndex), FileMode.Open, FileAccess.Read)
                    'NOTE: .NET Framework 4�ȏオ�O��ɂȂ�����AStream.CopyTo���g���ׂ��B
                    'NOTE: WriteToSocket���\�b�h�Ɠ������AObjFilesCombinedLen��
                    '�������āA�t�@�C���̏I�[�܂ł�ǂݎ���ď������ށB
                    '����́AObjFilesCombinedLen�ɐݒ肳��Ă���l�ƃt�@�C���̍��v�T�C�Y��
                    '���������Ƃ�O��ɂ��������ł���B
                    '�܂�A���̓d���C���X�^���X���쐬����ۂ�ObjFilesCombinedLen�̎Z�o
                    '���_����ObjFilePathList�������t�@�C���̓��e��ύX���Ă͂Ȃ�Ȃ��B
                    Do
                        Dim readSize As Integer = oInputStream.Read(aBuffer, 0, bufferLen)
                        If readSize = 0 Then Exit Do
                        oStream.Write(aBuffer, 0, readSize)
                    Loop
                End Using
            Next listIndex
        End If
    End Sub

    '�\�P�b�g�ւ̏o�̓��\�b�h�i�ʂ̃\�P�b�g�̓ǂݎ��Ď����j
    'NOTE: timeoutBaseTicks��0�܂���-1���w�肷��Ɩ������ҋ@�ƂȂ�B
    'NOTE: oInterruptSocketList�ɓo�^����Ă���\�P�b�g���ǂݎ��\�ɂȂ����ۂ́A
    'oSocket�ւ̏������݂𒆎~����BoInterruptSocketList�ɂ́A�A�v���P�[�V�����̐݌v
    '�ɉ����āA���X�j���O�\�P�b�g��ΐe�X���b�h�ʐM�p�\�P�b�g��o�^���Ă����Ƃ悢�B
    '�O�҂�o�^����ꍇ�A�Ăь��̃X���b�h�����X�j���O�\�P�b�g���Ǘ����邱�Ƃ��O��ɂȂ�B
    '�@��\���̕ω��⃆�[�U�ɂ��I�����삪�������ꍇ�ł��A���̓d���̑��M�𒆎~������
    '�Ȃ��Ȃ�A�O�҂݂̂�o�^���Ă������ƂŁA�Ή����\�ł���B
    '�������A�����d����M�҂��̊Ԃ����l�̔z�����K�v�ɂȂ邵�A�ʐM���肩��̐V���Ȑڑ���
    '���҂ł��Ȃ��P�[�X������Ȃ�΁A���̃��\�b�h����K�������o����悤�ɂ��邽�߂ɂ́A
    '�������łȂ��^�C�}�l���w�肷�邱�Ƃ��K�{�ɂȂ邽�߁A���܂�悢�Ή��Ƃ͌����Ȃ��B
    '����𓥂܂���ƁA�V���ȃR�l�N�V������e�X���b�h���烁�b�Z�[�W�Ŏ󂯎��ꍇ��
    '���炸�A��҂̓o�^�͂قڕK�{�ƌ�����B�������A�����郁�b�Z�[�W�𓯈�̃\�P�b�g��
    '��M����̂ł���΁B�����郁�b�Z�[�W�̎�M���ɁA���̓d���̑��M��r���ł�߂�
    '�i�܂�A�d���p�̃R�l�N�V�������p���g�p�s�\�ȏ�Ԃŕ�������j���ƂɂȂ�̂ŁA
    '���ӂ��K�v�ł���B�K�v�ɉ����āA�e�탁�b�Z�[�W���i�ً}�x���ƂɁj�قȂ�\�P�b�g��
    '��M����悤�ɂ�����A�e�X���b�h���A�ʐM��Ԃ��݂āA�I���v�����b�Z�[�W�𑗐M����
    '���ۂ������߂�ȂǁA�A�v�����x���ł̍l���͔������Ȃ��B
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
        Dim bufferLen As Integer = 16384  'NOTE: �`���[�j���O�̗]�n����
        Dim aBuffer(bufferLen - 1) As Byte
        Dim bufferValidLen As Integer = bufferLen
        Dim nextSendPosInBuffer As Integer = bufferLen
        Dim nextReadPosInRawBytes As Integer = 0
        Dim isAllBytesRead As Boolean = False

        Try
            Do
                'aBuffer���瑗�M�ς݃o�C�g����������B�܂��A�ł������i�ǂݏo��
                '���̂��ǂ����ɂ������j�AaBuffer�Ƀo�C�g���l�ߍ��ށB
                'NOTE: ���̂悤�ɁA�u���b�N���ꂽ�ہAaBuffer�S�̂𖢑��M�o�C�g��
                '�������Ă����̂́A�u���b�N�����񐔂��ŏ������邽�߂ł���B
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

                'NOTE: �����ł́AnextSendPosInBuffer���K��0�ł���B
                '�܂��AbufferValidLen��0�̏ꍇ�i��L��Do-Loop����x���Ō�܂�
                '���s����Ă��Ȃ��ꍇ�j�AisAllBytesRead�͕K��True�ł���B
                '����āA���L�̏����́unextSendPosInBuffer = bufferValidLen
                'AndAlso isAllBytesRead�v�Ɠ����ł���B
                If bufferValidLen = 0 Then Exit Do

                oCheckWriteList.Clear()
                oCheckWriteList.Add(oSocket)

                Try
                    'NOTE: �{���́AallTicks���u0�v�̃P�[�X�ł́ASocket.Select��
                    '�u-1�v��n���悤�ɂ��āA�u�������ҋ@�v���������B
                    '�������A.NET Framework 3.5��Socket.Select�ɂ̓o�O������A
                    '�u-1�v���w�肵���ꍇ�ɑ������A����悤�ł��邽�߁A
                    '�ł��邾���������ԁiInteger.MaxValue�j���w�肵�������t��
                    '�̑ҋ@�ɂ��Ă����B
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

                    'NOTE: �{���́A���̃��\�b�h�̌Ăь����A�v�f��0�̃��X�g��n���Ȃ��悤�z�����ׂ��B
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
                        'WOULD_BLOCK���������邩aBuffer�̗L���o�C�g��S�ď������ނ܂ŁA
                        '�\�P�b�g�ւ̏������݂��J��Ԃ��B
                        While nextSendPosInBuffer < bufferValidLen
                            nextSendPosInBuffer _
                               += oSocket.Send( _
                                  aBuffer, _
                                  nextSendPosInBuffer, _
                                  bufferValidLen - nextSendPosInBuffer, _
                                  SocketFlags.None)
                        End While

                        'aBuffer�̗L���o�C�g��S�ď������񂾏ꍇ�̂݁A���������s�����B
                        '����ȏ�ǂݍ��ނ��̂��Ȃ��ꍇ�́A���\�b�h���I������B
                        If isAllBytesRead Then Exit Do
                    End If

                    'NOTE: aBuffer�ɖ����M�o�C�g�������Ȃ����ꍇ�����łȂ��A
                    'Socket.Select���^�C���A�E�g�����ꍇ���A������ʂ邱�ƂɂȂ�B
                    '��҂̏ꍇ�́A�����ő����ɂ��̃��\�b�h���I�����Ă��悢���A
                    '�ЂƂ܂����[�v�̐擪�ɖ߂�����ASocket.Select�̎�O��
                    '���̃��\�b�h���I�����邱�Ƃɂ���B
                Catch ex As SocketException
                    Select Case ex.ErrorCode
                        '�������݂��u���b�N���ꂽ�ꍇ�iWSAEWOULDBLOCK�j�܂��́A
                        '�w�肵�����ԓ��ɏ������݂ł��Ȃ������ꍇ�iWSAETIMEDOUT�j
                        Case 10035, 10060
                            Continue Do

                        '���������Ȃ�A�A�v���̕s����v���ł���\�����Z���ł��邽�߁A
                        '�A�v�����I�������đ��߂Ɂi�e�X�g���Ɂj�C�t�����������悢�G���[
                        'NOTE: �O���ƒʐM���s�����߂̃\�P�b�g�𕡐��̃X���b�h���瑀�삷�邱�Ƃ�
                        '���蓾�Ȃ��i�Ăь��̃o�O�ł���j�Ƃ����O��ŁA10036�iWSAEINPROGRESS�j
                        '�������ɂ���B
                        Case 10009, 10013, 10014, 10022, 10036, 10037, 10038, _
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
            Loop
        Finally
            oSocket.Blocking = wasSocketBlocking
            If oInputStream IsNot Nothing Then
                oInputStream.Close()
            End If
        End Try
        Return True
    End Function

    '�\�P�b�g�ւ̏o�̓��\�b�h
    'NOTE: timeoutBaseTicks��0�܂���-1���w�肷��Ɩ������ҋ@�ƂȂ�B
    'NOTE: �f�[�^�����t�@�C���Ŏw�肳��Ă���ꍇ�����M�\�ł��邪�A���̂悤�ȑ傫�ȓd���𑗐M����
    '�ꍇ���A���A�̌_�@�͓d������M�p�\�P�b�g�ւ̏������ݐ����E���s���^�C���A�E�g�����ł���A����
    '���ۂ��_�@�ɕ��A���邱�Ƃ͂ł��Ȃ��i�^�C���A�E�g�l�̓t�@�C���̑傫���ɉ����ē��I�ɎZ�o�\�j�B
    '�T�[�o���ő��M������݂����A�N���C�A���g����̍ăR�l�N�g�ŕ��A�������̂ł���΁A
    'WriteToSocketInterruptible���\�b�h���g�p���邱�ƁB
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

'�V�[�P���X���ʃR�[�h
Public Enum NkSeqCode As Integer
    None
    Collection
    Delivery
    Test
End Enum

'�R�}���h���ʃR�[�h
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
