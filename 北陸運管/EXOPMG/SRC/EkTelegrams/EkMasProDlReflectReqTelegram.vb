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

Imports JR.ExOpmg.Common

''' <summary>
''' �}�X�^�t�@�C���܂��̓v���O�����t�@�C����DL�����ʒmREQ�d���B
''' </summary>
Public Class EkMasProDlReflectReqTelegram
    Inherits EkReqTelegram

#Region "�萔"
    Public Const FormalObjCodeAsGateMasterData As Byte = &H47
    Public Const FormalObjCodeAsGateProgramData As Byte = &H21
    Public Const FormalObjCodeAsGateProgramList As Byte = &H48
    Public Const FormalObjCodeAsMadoMasterData As Byte = &H74
    Public Const FormalObjCodeAsMadoProgramData As Byte = &H91
    Public Const FormalObjCodeAsMadoProgramList As Byte = &H75
    Public Const FormalObjCodeAsKsbProgramData As Byte = &H22
    Public Const FormalObjCodeAsKsbProgramList As Byte = &H49

    Private Const SubObjCodePos As Integer = 0
    Private Const SubObjCodeLen As Integer = 1
    Private Const PatternNumberPos As Integer = SubObjCodePos + SubObjCodeLen
    Private Const PatternNumberLen As Integer = 1
    Private Const VersionNumberPos As Integer = PatternNumberPos + PatternNumberLen
    Private Const VersionNumberLen As Integer = 4
    Private Const ReservedArea1Pos As Integer = VersionNumberPos + VersionNumberLen
    Private Const ReservedArea1Len As Integer = 4
    Private Const EatClientRailSectionCodePos As Integer = ReservedArea1Pos + ReservedArea1Len
    Private Const EatClientRailSectionCodeLen As Integer = 3
    Private Const EatClientStationOrderCodePos As Integer = EatClientRailSectionCodePos + EatClientRailSectionCodeLen
    Private Const EatClientStationOrderCodeLen As Integer = 3
    Private Const EatClientCornerCodePos As Integer = EatClientStationOrderCodePos + EatClientStationOrderCodeLen
    Private Const EatClientCornerCodeLen As Integer = 4
    Private Const EatClientUnitCodePos As Integer = EatClientCornerCodePos + EatClientCornerCodeLen
    Private Const EatClientUnitCodeLen As Integer = 1
    Private Const EatResultPos As Integer = EatClientUnitCodePos + EatClientUnitCodeLen
    Private Const EatResultLen As Integer = 1
    Private Const ObjDetailLen As Integer = EatResultPos + EatResultLen
#End Region

#Region "�v���p�e�B"
    Public Property SubObjCode() As Integer
        Get
            Return RawBytes(GetRawPos(SubObjCodePos))
        End Get

        Set(ByVal subObjCode As Integer)
            RawBytes(GetRawPos(SubObjCodePos)) = CType(subObjCode, Byte)
        End Set
    End Property

    Public Property PatternNumber() As Integer
        Get
            Return Utility.GetIntFromBcdBytes(RawBytes, GetRawPos(PatternNumberPos), PatternNumberLen)
        End Get

        Set(ByVal patternNumber As Integer)
            Utility.CopyIntToBcdBytes(patternNumber, RawBytes, GetRawPos(PatternNumberPos), PatternNumberLen)
        End Set
    End Property

    Public Property VersionNumber() As Integer
        Get
            Return Utility.GetIntFromBcdBytes(RawBytes, GetRawPos(VersionNumberPos), VersionNumberLen)
        End Get

        Set(ByVal versionNumber As Integer)
            Utility.CopyIntToBcdBytes(versionNumber, RawBytes, GetRawPos(VersionNumberPos), VersionNumberLen)
        End Set
    End Property

    Public Property EatClientCode() As EkCode
        Get
            Dim code As EkCode
            code.RailSection = Utility.GetIntFromDecimalAsciiBytes(RawBytes, GetRawPos(EatClientRailSectionCodePos), EatClientRailSectionCodeLen)
            code.StationOrder = Utility.GetIntFromDecimalAsciiBytes(RawBytes, GetRawPos(EatClientStationOrderCodePos), EatClientStationOrderCodeLen)
            code.Corner = Utility.GetIntFromDecimalAsciiBytes(RawBytes, GetRawPos(EatClientCornerCodePos), EatClientCornerCodeLen)
            code.Unit = RawBytes(GetRawPos(EatClientUnitCodePos))
            Return code
        End Get

        Set(ByVal clientCode As EkCode)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.RailSection, RawBytes, GetRawPos(EatClientRailSectionCodePos), EatClientRailSectionCodeLen)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.StationOrder, RawBytes, GetRawPos(EatClientStationOrderCodePos), EatClientStationOrderCodeLen)
            Utility.CopyIntToDecimalAsciiBytes(clientCode.Corner, RawBytes, GetRawPos(EatClientCornerCodePos), EatClientCornerCodeLen)
            RawBytes(GetRawPos(EatClientUnitCodePos)) = CByte(clientCode.Unit)
        End Set
    End Property

    Public Property EatResult() As Integer
        Get
            Return RawBytes(GetRawPos(EatResultPos))
        End Get

        Set(ByVal result As Integer)
            RawBytes(GetRawPos(EatResultPos)) = CByte(result)
        End Set
    End Property
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal subObjCode As Integer, _
       ByVal patternNumber As Integer, _
       ByVal versionNumber As Integer, _
       ByVal eatClientCode As EkCode, _
       ByVal eatResult As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.SubObjCode = subObjCode
        Me.PatternNumber = patternNumber
        Me.VersionNumber = versionNumber
        Me.EatClientCode = eatClientCode
        Me.EatResult = eatResult
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

        If Not Utility.IsBcdBytes(RawBytes, GetRawPos(PatternNumberPos), PatternNumberLen) Then
            Log.Error("PatternNumber is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsBcdBytes(RawBytes, GetRawPos(VersionNumberPos), VersionNumberLen) Then
            Log.Error("VersionNumber is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, GetRawPos(EatClientRailSectionCodePos), EatClientRailSectionCodeLen) Then
            Log.Error("EatClientRailSectionCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, GetRawPos(EatClientStationOrderCodePos), EatClientStationOrderCodeLen) Then
            Log.Error("EatClientStationOrderCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsDecimalAsciiBytes(RawBytes, GetRawPos(EatClientCornerCodePos), EatClientCornerCodeLen) Then
            Log.Error("EatClientCornerCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If RawBytes(GetRawPos(EatClientUnitCodePos)) > 99 Then
            Log.Error("EatClientUnitCode is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As EkMasProDlReflectAckTelegram
        Return New EkMasProDlReflectAckTelegram(Gene, ObjCode)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkMasProDlReflectAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkMasProDlReflectAckTelegram
        Return New EkMasProDlReflectAckTelegram(oReplyTeleg)
    End Function
#End Region

End Class
