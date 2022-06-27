' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2015/01/09  (NES)����  �����Ɩ��O�F�؃��O���W�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization

Imports JR.ExOpmg.Common

''' <summary>
''' �^�ǃT�[�o�Ƒ����̊Ԃ̃��O�i���샍�O�E�Ɩ��O�F�؃��O�j���tREQ�d���B
''' </summary>
Public Class EkMadoLogPostReqTelegram
    Inherits EkReqTelegram

#Region "�萔"
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  MOD START-----------
    Public Const FormalObjCodeAsMadoLog As Byte = &HC4
    Public Const FormalObjCodeAsMadoCertLog As Byte = &HC5
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  MOD END-----------

    Private Const LogSendTimeFormat As String = "yyyyMMddHHmmss"
    Private Const LogSendTimePos As Integer = 0
    Private Const LogSendTimeLen As Integer = 14
    Private Const LogDataSizePos As Integer = LogSendTimePos + LogSendTimeLen
    Private Const LogDataSizeLen As Integer = 4
    Private Const LogDataSumValuePos As Integer = LogDataSizePos + LogDataSizeLen
    Private Const LogDataSumValueLen As Integer = 4
    Private Const LogDataPos As Integer = LogDataSumValuePos + LogDataSumValueLen
#End Region

#Region "�v���p�e�B"
    Public Property LogSendTime() As DateTime
        Get
            Dim yyyymmdd As Integer = Utility.GetIntFromUnpackedBcdBytes(RawBytes, GetRawPos(LogSendTimePos), 8)
            Dim hhmmss As Integer = Utility.GetIntFromUnpackedBcdBytes(RawBytes, GetRawPos(LogSendTimePos + 8), 6)
            Dim sLogSendTime As String = yyyymmdd.ToString("D8") & hhmmss.ToString("D6")
            Return DateTime.ParseExact(sLogSendTime, LogSendTimeFormat, CultureInfo.InvariantCulture)
        End Get

        Set(ByVal oLogSendTime As DateTime)
            Dim sLogSendTime As String = oLogSendTime.ToString(LogSendTimeFormat)
            Dim yyyymmdd As Integer = Integer.Parse(sLogSendTime.SubString(0, 8))
            Dim hhmmss As Integer = Integer.Parse(sLogSendTime.SubString(8, 6))
            Utility.CopyIntToUnpackedBcdBytes(yyyymmdd, RawBytes, GetRawPos(LogSendTimePos), 8)
            Utility.CopyIntToUnpackedBcdBytes(hhmmss, RawBytes, GetRawPos(LogSendTimePos + 8), 6)
        End Set
    End Property

    Public Property LogDataSize() As UInteger
        Get
            Return Utility.GetUInt32FromLeBytes4(RawBytes, GetRawPos(LogDataSizePos))
        End Get

        Set(ByVal size As UInteger)
            Utility.CopyUInt32ToLeBytes4(size, RawBytes, GetRawPos(LogDataSizePos))
        End Set
    End Property

    Public Property LogDataSumValue() As UInteger
        Get
            Return Utility.GetUInt32FromLeBytes4(RawBytes, GetRawPos(LogDataSumValuePos))
        End Get

        Set(ByVal size As UInteger)
            Utility.CopyUInt32ToLeBytes4(size, RawBytes, GetRawPos(LogDataSumValuePos))
        End Set
    End Property

    Public ReadOnly Property LogData() As Byte()
        Get
            Dim len As Integer = GetObjDetailLen() - LogDataPos
            Dim aBytes As Byte() = New Byte(len - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(LogDataPos), aBytes, 0, len)
            Return aBytes
        End Get
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal logSendTime As DateTime, _
       ByVal logDataSize As UInteger, _
       ByVal logDataSumValue As UInteger, _
       ByVal aLogData As Byte(), _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Post, objCode, LogDataPos + aLogData.Length, replyLimitTicks)
        Me.LogSendTime = logSendTime
        Me.LogDataSize = logDataSize
        Me.LogDataSumValue = logDataSumValue
        Buffer.BlockCopy(aLogData, 0, Me.RawBytes, GetRawPos(LogDataPos), aLogData.Length)
    End Sub

    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal logSendTime As DateTime, _
       ByVal aLogData As Byte(), _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Post, objCode, LogDataPos + aLogData.Length, replyLimitTicks)
        Me.LogSendTime = logSendTime
        Me.LogDataSize = CUInt(aLogData.Length)
        Me.LogDataSumValue = CalculateSumValue(aLogData, 0, aLogData.Length)
        Buffer.BlockCopy(aLogData, 0, Me.RawBytes, GetRawPos(LogDataPos), aLogData.Length)
    End Sub

    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal aLogData As Byte(), _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Post, objCode, LogDataPos + aLogData.Length, replyLimitTicks)
        Me.LogSendTime = DateTime.Now
        Me.LogDataSize = CUInt(aLogData.Length)
        Me.LogDataSumValue = CalculateSumValue(aLogData, 0, aLogData.Length)
        Buffer.BlockCopy(aLogData, 0, Me.RawBytes, GetRawPos(LogDataPos), aLogData.Length)
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If GetObjDetailLen() < LogDataPos Then
            Log.Error("ObjSize is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsUnpackedBcdBytes(RawBytes, GetRawPos(LogSendTimePos), LogSendTimeLen) Then
            Log.Error("LogSendTime is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        Dim yyyymmdd As Integer = Utility.GetIntFromUnpackedBcdBytes(RawBytes, GetRawPos(LogSendTimePos), 8)
        Dim hhmmss As Integer = Utility.GetIntFromUnpackedBcdBytes(RawBytes, GetRawPos(LogSendTimePos + 8), 6)
        Dim sLogSendTime As String = yyyymmdd.ToString("D8") & hhmmss.ToString("D6")
        Dim oLogSendTime As DateTime
        If DateTime.TryParseExact(sLogSendTime, LogSendTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, oLogSendTime) = False Then
            Log.Error("LogSendTime is invalid (not a time).")
            Return EkNakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As EkMadoLogPostAckTelegram
        'TODO: �K�v�ł���΁ALogData���̃f�[�^�t�H�[�}�b�g�ُ�����肷��B
        'TODO: �K�v�ł���΁Alen���C���^�t�F�[�X�d�l����𒴂���ꍇ��
        '���R�[�h���̔{���łȂ��ꍇ���Acode��1�Ƃ���B
        Dim code As Integer = 0
        Dim len As Integer = GetObjDetailLen() - LogDataPos
        If len <> LogDataSize Then
            code = 1
        Else
            If CalculateSumValue(RawBytes, GetRawPos(LogDataPos), len) <> LogDataSumValue Then
                code = 2
            End If
        End If

        Return New EkMadoLogPostAckTelegram(Gene, ObjCode, code)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkMadoLogPostAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Shadows Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkMadoLogPostAckTelegram
        Return New EkMadoLogPostAckTelegram(oReplyTeleg)
    End Function

    Private Shared Function CalculateSumValue(ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer) As UInteger
        Dim endPos As Integer = pos + len - 1
        Dim sum As Long = 0
        For i As Integer = pos To endPos
            sum += aBytes(i)
            sum = sum And &HFFFFFFFF
        Next i
        Return CUInt(sum)
    End Function
#End Region

End Class
