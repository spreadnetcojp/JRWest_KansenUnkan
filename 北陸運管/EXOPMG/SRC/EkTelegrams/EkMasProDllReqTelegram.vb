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

Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �}�X�^�t�@�C���܂��̓v���O�����t�@�C����DLL����ۂ̐�pREQ�d���B
''' </summary>
Public Class EkMasProDllReqTelegram
    Inherits EkReqTelegram
    Implements IXllReqTelegram

#Region "�萔"
    Public Const FormalObjCodeAsGateMasterSuite As Byte = &H40
    Public Const FormalObjCodeAsGateMasterList As Byte = &H43
    Public Const FormalObjCodeAsGateProgramSuite As Byte = &H61
    Public Const FormalObjCodeAsGateProgramList As Byte = &H41
    Public Const FormalObjCodeAsMadoMasterSuite As Byte = &H70
    Public Const FormalObjCodeAsMadoMasterList As Byte = &H73
    Public Const FormalObjCodeAsMadoProgramSuite As Byte = &H81
    Public Const FormalObjCodeAsMadoProgramList As Byte = &H71
    Public Const FormalObjCodeAsKsbProgramSuite As Byte = &H62
    Public Const FormalObjCodeAsKsbProgramList As Byte = &H42

    Friend Shared ReadOnly oRawContinueCodeTable As New Dictionary(Of ContinueCode, Byte) From { _
       {ContinueCode.Start, &H1}, _
       {ContinueCode.Finish, &H2}, _
       {ContinueCode.FinishWithoutStoring, &H3}, _
       {ContinueCode.Abort, &H10}}
    Friend Shared ReadOnly oContinueCodeTable As New Dictionary(Of Byte, ContinueCode) From { _
       {&H1, ContinueCode.Start}, _
       {&H2, ContinueCode.Finish}, _
       {&H3, ContinueCode.FinishWithoutStoring}, _
       {&H10, ContinueCode.Abort}}

    Private Const SubObjCodePos As Integer = 0
    Private Const SubObjCodeLen As Integer = 1
    Private Const ReservedArea1Pos As Integer = SubObjCodePos + SubObjCodeLen
    Private Const ReservedArea1Len As Integer = 4
    Private Const ContinueCodePos As Integer = ReservedArea1Pos + ReservedArea1Len
    Private Const ContinueCodeLen As Integer = 1
    Private Const DataFileNamePos As Integer = ContinueCodePos + ContinueCodeLen
    Private Const DataFileNameLen As Integer = 80
    Private Const DataFileHashValuePos As Integer = DataFileNamePos + DataFileNameLen
    Private Const DataFileHashValueLen As Integer = 32
    Private Const ListFileNamePos As Integer = DataFileHashValuePos + DataFileHashValueLen
    Private Const ListFileNameLen As Integer = 80
    Private Const ListFileHashValuePos As Integer = ListFileNamePos + ListFileNameLen
    Private Const ListFileHashValueLen As Integer = 32
    Private Const ResultantVersionOfSlot1Pos As Integer = ListFileHashValuePos + ListFileHashValueLen
    Private Const ResultantVersionOfSlot1Len As Integer = 4
    Private Const ResultantVersionOfSlot2Pos As Integer = ResultantVersionOfSlot1Pos + ResultantVersionOfSlot1Len
    Private Const ResultantVersionOfSlot2Len As Integer = 4
    Private Const ResultantFlagOfFullPos As Integer = ResultantVersionOfSlot2Pos + ResultantVersionOfSlot2Len
    Private Const ResultantFlagOfFullLen As Integer = 1
    Private Const ObjDetailLen As Integer = ResultantFlagOfFullPos + ResultantFlagOfFullLen
#End Region

#Region "�ϐ�"
    Private _TransferLimitTicks As Integer
#End Region

#Region "�v���p�e�B"
    Private ReadOnly Property __ContinueCode() As ContinueCode Implements IXllTelegram.ContinueCode
        Get
            Return ContinueCode
        End Get
    End Property

    Public Property ContinueCode() As ContinueCode
        Get
            Dim code As ContinueCode
            If oContinueCodeTable.TryGetValue(RawBytes(GetRawPos(ContinueCodePos)), code) = False Then
                code = ContinueCode.None
            End If
            Return code
        End Get

        Set(ByVal code As ContinueCode)
            RawBytes(GetRawPos(ContinueCodePos)) = oRawContinueCodeTable(code)
        End Set
    End Property

    Public ReadOnly Property RawContinueCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(ContinueCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(ContinueCodePos), ret, 0, ContinueCodeLen)
            Return ret
        End Get
    End Property

    Public Property SubObjCode() As Integer
        Get
            Return RawBytes(GetRawPos(SubObjCodePos))
        End Get

        Set(ByVal subObjCode As Integer)
            RawBytes(GetRawPos(SubObjCodePos)) = CType(subObjCode, Byte)
        End Set
    End Property

    Public Property DataFileName() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(DataFileNamePos), DataFileNameLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal dataFileName As String)
            Array.Clear(RawBytes, GetRawPos(DataFileNamePos), DataFileNameLen)
            Encoding.UTF8.GetBytes(dataFileName, 0, dataFileName.Length, RawBytes, GetRawPos(DataFileNamePos))
        End Set
    End Property

    Public Property DataFileHashValue() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(DataFileHashValuePos), DataFileHashValueLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal dataFileHashValue As String)
            Array.Clear(RawBytes, GetRawPos(DataFileHashValuePos), DataFileHashValueLen)
            Encoding.UTF8.GetBytes(dataFileHashValue, 0, dataFileHashValue.Length, RawBytes, GetRawPos(DataFileHashValuePos))
        End Set
    End Property

    Public Property ListFileName() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(ListFileNamePos), ListFileNameLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal listFileName As String)
            Array.Clear(RawBytes, GetRawPos(ListFileNamePos), ListFileNameLen)
            Encoding.UTF8.GetBytes(listFileName, 0, listFileName.Length, RawBytes, GetRawPos(ListFileNamePos))
        End Set
    End Property

    Public Property ListFileHashValue() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(ListFileHashValuePos), ListFileHashValueLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal listFileHashValue As String)
            Array.Clear(RawBytes, GetRawPos(ListFileHashValuePos), ListFileHashValueLen)
            Encoding.UTF8.GetBytes(listFileHashValue, 0, listFileHashValue.Length, RawBytes, GetRawPos(ListFileHashValuePos))
        End Set
    End Property

    Public Property ResultantVersionOfSlot1() As Integer
        Get
            Return Utility.GetIntFromBcdBytes(RawBytes, GetRawPos(ResultantVersionOfSlot1Pos), ResultantVersionOfSlot1Len)
        End Get

        Set(ByVal resultantVersionOfSlot1 As Integer)
            Utility.CopyIntToBcdBytes(resultantVersionOfSlot1, RawBytes, GetRawPos(ResultantVersionOfSlot1Pos), ResultantVersionOfSlot1Len)
        End Set
    End Property

    Public Property ResultantVersionOfSlot2() As Integer
        Get
            Return Utility.GetIntFromBcdBytes(RawBytes, GetRawPos(ResultantVersionOfSlot2Pos), ResultantVersionOfSlot2Len)
        End Get

        Set(ByVal resultantVersionOfSlot2 As Integer)
            Utility.CopyIntToBcdBytes(resultantVersionOfSlot2, RawBytes, GetRawPos(ResultantVersionOfSlot2Pos), ResultantVersionOfSlot2Len)
        End Set
    End Property

    Public Property ResultantFlagOfFull() As Integer
        Get
            Return RawBytes(GetRawPos(ResultantFlagOfFullPos))
        End Get

        Set(ByVal resultantFlagOfFull As Integer)
            RawBytes(GetRawPos(ResultantFlagOfFullPos)) = CType(resultantFlagOfFull, Byte)
        End Set
    End Property

    Public ReadOnly Property TransferListBase() As String Implements IXllReqTelegram.TransferListBase
        Get
            Return GetXllBasePath()
        End Get
    End Property

    Public ReadOnly Property TransferList() As List(Of String) Implements IXllReqTelegram.TransferList
        Get
            Dim oList As New List(Of String)(2)

            If ObjCode = FormalObjCodeAsGateMasterSuite OrElse _
               ObjCode = FormalObjCodeAsGateProgramSuite OrElse _
               ObjCode = FormalObjCodeAsMadoMasterSuite OrElse _
               ObjCode = FormalObjCodeAsMadoProgramSuite OrElse _
               ObjCode = FormalObjCodeAsKsbProgramSuite Then
                oList.Add(DataFileName)
            End If

            oList.Add(ListFileName)

            Return oList
        End Get
    End Property

    Private ReadOnly Property __TransferLimitTicks() As Integer Implements IXllReqTelegram.TransferLimitTicks
        Get
            Return _TransferLimitTicks
        End Get
    End Property

    Public Property TransferLimitTicks() As Integer
        Get
            Return _TransferLimitTicks
        End Get

        Set(ByVal ticks As Integer)
            _TransferLimitTicks = ticks
        End Set
    End Property

    Public ReadOnly Property IsHashValueReady() As Boolean Implements IXllReqTelegram.IsHashValueReady
        Get
            Return DataFileHashValue.Length <> 0 AndAlso ListFileHashValue.Length <> 0
        End Get
    End Property

    Public ReadOnly Property IsHashValueIndicatingOkay() As Boolean Implements IXllReqTelegram.IsHashValueIndicatingOkay
        Get

            Dim sListFilePath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), ListFileName)
            Dim sListFileHashValue As String
            Try
                sListFileHashValue = Utility.CalculateMD5(sListFilePath)
            Catch ex As Exception
                Log.Error("Some Exception caught.", ex)
                Return False
            End Try
            If StringComparer.OrdinalIgnoreCase.Compare(sListFileHashValue, ListFileHashValue) <> 0 Then Return False

            If ObjCode = FormalObjCodeAsGateMasterSuite OrElse _
               ObjCode = FormalObjCodeAsGateProgramSuite OrElse _
               ObjCode = FormalObjCodeAsMadoMasterSuite OrElse _
               ObjCode = FormalObjCodeAsMadoProgramSuite OrElse _
               ObjCode = FormalObjCodeAsKsbProgramSuite Then
                Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), DataFileName)
                Dim sDataFileHashValue As String
                Try
                    sDataFileHashValue = Utility.CalculateMD5(sDataFilePath)
                Catch ex As Exception
                    Log.Error("Some Exception caught.", ex)
                    Return False
                End Try
                If StringComparer.OrdinalIgnoreCase.Compare(sDataFileHashValue, DataFileHashValue) <> 0 Then Return False
            End If

            Return True
        End Get
    End Property
#End Region

#Region "�R���X�g���N�^"
    'String�^��xxx��XxxLen�����ȉ���ASCII�L�����N�^�ō\������镶����ł��邱�Ƃ��O��ł��B
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal subObjCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal dataFileName As String, _
       ByVal listFileName As String, _
       ByVal resultantVersionOfSlot1 As Integer, _
       ByVal resultantVersionOfSlot2 As Integer, _
       ByVal resultantFlagOfFull As Integer, _
       ByVal transferLimitTicks As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.SubObjCode = subObjCode
        Me.ContinueCode = continueCode
        Me.DataFileName = dataFileName
        Me.DataFileHashValue = ""
        Me.ListFileName = listFileName
        Me.ListFileHashValue = ""
        Me.ResultantVersionOfSlot1 = resultantVersionOfSlot1
        Me.ResultantVersionOfSlot2 = resultantVersionOfSlot2
        Me.ResultantFlagOfFull = resultantFlagOfFull
        Me._TransferLimitTicks = transferLimitTicks
    End Sub

    'String�^��xxx��XxxLen�����ȉ���ASCII�L�����N�^�ō\������镶����ł��邱�Ƃ��O��ł��B
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal subObjCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal dataFileName As String, _
       ByVal dataFileHashValue As String, _
       ByVal listFileName As String, _
       ByVal listFileHashValue As String, _
       ByVal resultantVersionOfSlot1 As Integer, _
       ByVal resultantVersionOfSlot2 As Integer, _
       ByVal resultantFlagOfFull As Integer, _
       ByVal transferLimitTicks As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.SubObjCode = subObjCode
        Me.ContinueCode = continueCode
        Me.DataFileName = dataFileName
        Me.DataFileHashValue = dataFileHashValue
        Me.ListFileName = listFileName
        Me.ListFileHashValue = listFileHashValue
        Me.ResultantVersionOfSlot1 = resultantVersionOfSlot1
        Me.ResultantVersionOfSlot2 = resultantVersionOfSlot2
        Me.ResultantFlagOfFull = resultantFlagOfFull
        Me._TransferLimitTicks = transferLimitTicks
    End Sub

    Public Sub New( _
       ByVal oTeleg As ITelegram, _
       ByVal transferLimitTicks As Integer)

        MyBase.New(oTeleg)
        Me._TransferLimitTicks = transferLimitTicks
    End Sub
#End Region

#Region "���\�b�h"
    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If GetObjDetailLen() <> ObjDetailLen Then
            Log.Error("ObjSize is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(DataFileNamePos), DataFileNameLen) Then
            Log.Error("DataFileName is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        'NOTE: �n�b�V���l�́A��̔�r�Ń`�F�b�N�����͂��ł��邽�߁A�����ł�
        '�`�F�b�N�͊ɂ߂ɂ���i������ɕϊ��\�ł��肳������΂悢�j�B
        '�܂��A���̍��ڂɊւ��ẮA�K�p���X�g�݂̂�]������ꍇ��0x00��
        '�t�B�������́A����0�̕���������e���˂΂Ȃ炸�A���̈Ӗ��ł�
        'IsHexadecimalAsciiBytes�Ń`�F�b�N���Ă͂Ȃ�Ȃ��B
        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(DataFileHashValuePos), DataFileHashValueLen) Then
            Log.Error("DataFileHashValue is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(ListFileNamePos), ListFileNameLen) Then
            Log.Error("ListFileName is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        'NOTE: �n�b�V���l�́A��̔�r�Ń`�F�b�N�����͂��ł��邽�߁A�����ł�
        '�`�F�b�N�͊ɂ߂ɂ���i������ɕϊ��\�ł��肳������΂悢�j�B
        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(ListFileHashValuePos), ListFileHashValueLen) Then
            Log.Error("ListFileHashValue is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsBcdBytes(RawBytes, GetRawPos(ResultantVersionOfSlot1Pos), ResultantVersionOfSlot1Len) Then
            Log.Error("ResultantVersionOfSlot1 is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsBcdBytes(RawBytes, GetRawPos(ResultantVersionOfSlot2Pos), ResultantVersionOfSlot2Len) Then
            Log.Error("ResultantVersionOfSlot2 is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        If ObjCode = FormalObjCodeAsGateMasterSuite OrElse _
           ObjCode = FormalObjCodeAsGateProgramSuite OrElse _
           ObjCode = FormalObjCodeAsMadoMasterSuite OrElse _
           ObjCode = FormalObjCodeAsMadoProgramSuite OrElse _
           ObjCode = FormalObjCodeAsKsbProgramSuite Then
            If Not Utility.IsValidVirtualPath(DataFileName) Then
                Log.Error("DataFileName is invalid (illegal path).")
                Return EkNakCauseCode.TelegramError
            End If
        Else
            If Not DataFileName.Equals("") Then
                Log.Error("DataFileName is invalid (not 0x00).")
                Return EkNakCauseCode.TelegramError
            End If
        End If

        If Not Utility.IsValidVirtualPath(ListFileName) Then
            Log.Error("ListFileName is invalid (illegal path).")
            Return EkNakCauseCode.TelegramError
        End If

        Return EkNakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Private Function CreateIAckTelegram() As IXllTelegram Implements IXllReqTelegram.CreateAckTelegram
        Return New EkMasProDllAckTelegram(Gene, ObjCode, ContinueCode)
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As EkMasProDllAckTelegram
        Return New EkMasProDllAckTelegram(Gene, ObjCode, ContinueCode)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkMasProDllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Private Function ParseAsIXllAck(ByVal oReplyTeleg As ITelegram) As IXllTelegram Implements IXllReqTelegram.ParseAsAck
        Return New EkMasProDllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkMasProDllAckTelegram
        Return New EkMasProDllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^�𓯈�^�ɕϊ����郁�\�b�h
    Public Function ParseAsSameKind(ByVal oNextTeleg As ITelegram) As IXllReqTelegram Implements IXllReqTelegram.ParseAsSameKind
        Return New EkMasProDllReqTelegram(oNextTeleg, TransferLimitTicks)
    End Function

    '�㑱REQ�d���𐶐����郁�\�b�h
    Public Function CreateContinuousTelegram(ByVal continueCode As ContinueCode, ByVal resultantVersionOfSlot1 As Integer, ByVal resultantVersionOfSlot2 As Integer, ByVal resultantFlagOfFull As Integer, ByVal transferLimitTicks As Integer, ByVal replyLimitTicks As Integer) As EkMasProDllReqTelegram
        Return New EkMasProDllReqTelegram( _
           Gene, _
           ObjCode, _
           SubObjCode, _
           continueCode, _
           DataFileName, _
           DataFileHashValue, _
           ListFileName, _
           ListFileHashValue, _
           resultantVersionOfSlot1, _
           resultantVersionOfSlot2, _
           resultantFlagOfFull, _
           transferLimitTicks, _
           replyLimitTicks)
    End Function

    '�n���ꂽ����^�d����ObjDetail��������̃t�@�C���]���������Ă��邩���肷�郁�\�b�h
    Function IsContinuousWith(ByVal oXllReqTeleg As IXllReqTelegram) As Boolean Implements IXllReqTelegram.IsContinuousWith
        Dim oRealTeleg As EkMasProDllReqTelegram = DirectCast(oXllReqTeleg, EkMasProDllReqTelegram)
        If SubObjCode <> oRealTeleg.SubObjCode Then Return False
        If DataFileName <> oRealTeleg.DataFileName Then Return False
        If ListFileName <> oRealTeleg.ListFileName Then Return False
        'NOTE: �T�[�o�����DLL�V�[�P���X�ɂ����āA�t�@�C���]����ɃN���C�A���g����
        '���M�����REQ�d���́A�t�@�C�����e�����Ƀn�b�V���l���Čv�Z����Ă���B
        '�܂�A����V�[�P���X���ł����Ă��A�n�b�V���l����v����Ƃ͌���Ȃ��B
        '����āA�n�b�V���l�ɂ��Ă͔�r���Ȃ��B
        Return True
    End Function

    'ACK�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    Public Sub ImportFileDependentValueFromAck(ByVal oReplyTeleg As IXllTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromAck
        Debug.Fail("This case is impermissible.")
    End Sub

    '����^�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    Public Sub ImportFileDependentValueFromSameKind(ByVal oPreviousTeleg As IXllReqTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromSameKind
        Dim oRealPreviousTeleg As EkMasProDllReqTelegram = DirectCast(oPreviousTeleg, EkMasProDllReqTelegram)
        DataFileHashValue = oRealPreviousTeleg.DataFileHashValue
        ListFileHashValue = oRealPreviousTeleg.ListFileHashValue
        _TransferLimitTicks = oRealPreviousTeleg._TransferLimitTicks
    End Sub

    'HashValue���ɒl���Z�b�g���郁�\�b�h
    Public Sub UpdateHashValue() Implements IXllReqTelegram.UpdateHashValue
        Dim sListFilePath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), ListFileName)
        Try
            ListFileHashValue = Utility.CalculateMD5(sListFilePath)
        Catch ex As Exception
            Log.Error("Some Exception caught.", ex)
            'NOTE: �ȉ��̂悤��MD5�Ƃ��Ă��蓾�Ȃ��l�ɂ��邱�ƂŁA
            '����Ɉُ�ȓd���Ɣ��f���Ă��炤�B
            'NOTE: �{���́A���̃��\�b�h���Ă΂��O�ɁA�������A�N�Z�X�\��
            '�t�@�C����ݒu���Ă������Ƃ́A�A�v���̐Ӗ��ł���A
            '��O�͂����ŃL���b�`����ׂ��ł͂Ȃ���������Ȃ����A
            '�n�[�h�E�F�A�̏�Q���Ŕ��������ُ�ŁA�����Ȃ藎����̂�
            '�����ł��邽�߁A�Ƃ肠�����A���̂悤�ɂ��Ă����B
            ListFileHashValue = ""
        End Try

        If ObjCode = FormalObjCodeAsGateMasterSuite OrElse _
           ObjCode = FormalObjCodeAsGateProgramSuite OrElse _
           ObjCode = FormalObjCodeAsMadoMasterSuite OrElse _
           ObjCode = FormalObjCodeAsMadoProgramSuite OrElse _
           ObjCode = FormalObjCodeAsKsbProgramSuite Then
            Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), DataFileName)
            Try
                DataFileHashValue = Utility.CalculateMD5(sDataFilePath)
            Catch ex As Exception
                Log.Error("Some Exception caught.", ex)
                'NOTE: �ȉ��̂悤��MD5�Ƃ��Ă��蓾�Ȃ��l�ɂ��邱�ƂŁA
                '����Ɉُ�ȓd���Ɣ��f���Ă��炤�B
                'NOTE: �{���́A���̃��\�b�h���Ă΂��O�ɁA�������A�N�Z�X�\��
                '�t�@�C����ݒu���Ă������Ƃ́A�A�v���̐Ӗ��ł���A
                '��O�͂����ŃL���b�`����ׂ��ł͂Ȃ���������Ȃ����A
                '�n�[�h�E�F�A�̏�Q���Ŕ��������ُ�ŁA�����Ȃ藎����̂�
                '�����ł��邽�߁A�Ƃ肠�����A���̂悤�ɂ��Ă����B
                DataFileHashValue = ""
            End Try
        End If
    End Sub
#End Region

End Class
