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
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �����f�[�^�擾ACK�d���B
''' </summary>
Public Class EkTimeDataGetAckTelegram
    Inherits EkTelegram

#Region "�萔"
    Private Const TimeDataFormat As String = "yyyyMMddHHmmssfff"
    Private Const TimeDataPos As Integer = 0
    Private Const TimeDataLen As Integer = 17
    Private Const ObjDetailLen As Integer = TimeDataLen
#End Region

#Region "�v���p�e�B"
    Public Property TimeData() As DateTime
        Get
            Dim sTimeData As String = Encoding.UTF8.GetString(RawBytes, GetRawPos(TimeDataPos), TimeDataLen)
            Return DateTime.ParseExact(sTimeData, TimeDataFormat, CultureInfo.InvariantCulture)
        End Get

        Set(ByVal oTimeData As DateTime)
            Dim sTimeData As String = oTimeData.ToString(TimeDataFormat)
            Encoding.UTF8.GetBytes(sTimeData, 0, TimeDataLen, RawBytes, GetRawPos(TimeDataPos))
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^"
    'oTimeData��TimeDataLen�����ȉ���ASCII�L�����N�^�ō\������镶����ł��邱�Ƃ��O��ł��B
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal oTimeData As DateTime)
        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Get, objCode, ObjDetailLen)
        Me.TimeData = oTimeData
    End Sub

    Public Sub New(ByVal oTeleg As ITelegram)
        MyBase.New(oTeleg)
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If GetObjDetailLen() <> ObjDetailLen Then
            Log.Error("ObjSize is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytesFixed(RawBytes, GetRawPos(TimeDataPos), TimeDataLen) Then
            Log.Error("TimeData is invalid (not decimal ASCII bytes).")
            Return EkNakCauseCode.TelegramError
        End If

        Dim sTimeData As String = Encoding.UTF8.GetString(RawBytes, GetRawPos(TimeDataPos), TimeDataLen)
        Dim oTimeData As DateTime
        If DateTime.TryParseExact(sTimeData, TimeDataFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, oTimeData) = False Then
            Log.Error("TimeData is invalid (not a time).")
            Return EkNakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function
#End Region

End Class
