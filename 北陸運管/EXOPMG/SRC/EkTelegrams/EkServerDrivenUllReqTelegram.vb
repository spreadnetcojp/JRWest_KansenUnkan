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
''' �ł���{�I�ȃT�[�o���ULL��REQ�d���B
''' </summary>
Public Class EkServerDrivenUllReqTelegram
    Inherits EkReqTelegram
    Implements IXllReqTelegram

#Region "�萔"
    Public Const FormalObjCodeAsGateBesshuData As Byte = &HA1
    Public Const FormalObjCodeAsGateMeisaiData As Byte = &HA2
    Public Const FormalObjCodeAsGateKadoData As Byte = &HA7
    Public Const FormalObjCodeAsGateTrafficData As Byte = &HB1
    Public Const FormalObjCodeAsKsbGateFaultData As Byte = &HB6
    Public Const FormalObjCodeAsMadoKadoData As Byte = &HB7
    Public Const FormalObjCodeAsMadoFaultData As Byte = &HB8

    Friend Shared ReadOnly oRawContinueCodeTable As New Dictionary(Of ContinueCode, Byte) From { _
       {ContinueCode.Start, &H1}, _
       {ContinueCode.Finish, &H2}, _
       {ContinueCode.Abort, &H10}}
    Friend Shared ReadOnly oContinueCodeTable As New Dictionary(Of Byte, ContinueCode) From { _
       {&H1, ContinueCode.Start}, _
       {&H2, ContinueCode.Finish}, _
       {&H10, ContinueCode.Abort}}

    Private Const ContinueCodePos As Integer = 0
    Private Const ContinueCodeLen As Integer = 1
    Private Const FileNamePos As Integer = ContinueCodePos + ContinueCodeLen
    Private Const FileNameLen As Integer = 80
    Private Const ObjDetailLen As Integer = FileNamePos + FileNameLen
#End Region

#Region "�ϐ�"
    Private _FileHashValue As String
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

    Public Property FileName() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(FileNamePos), FileNameLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal fileName As String)
            Array.Clear(RawBytes, GetRawPos(FileNamePos), FileNameLen)
            Encoding.UTF8.GetBytes(fileName, 0, fileName.Length, RawBytes, GetRawPos(FileNamePos))
        End Set
    End Property

    Public Property FileHashValue() As String
        Get
            Return _FileHashValue
        End Get

        Set(ByVal fileHashValue As String)
            _FileHashValue = fileHashValue
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
            oList.Add(FileName)
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
            Return _FileHashValue.Length <> 0
        End Get
    End Property

    Public ReadOnly Property IsHashValueIndicatingOkay() As Boolean Implements IXllReqTelegram.IsHashValueIndicatingOkay
        Get
            Dim sPath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), FileName)
            Dim sHashValue As String
            Try
                sHashValue = Utility.CalculateMD5(sPath)
            Catch ex As Exception
                Log.Error("Some Exception caught.", ex)
                Return False
            End Try
            Return StringComparer.OrdinalIgnoreCase.Compare(sHashValue, _FileHashValue) = 0
        End Get
    End Property
#End Region

#Region "�R���X�g���N�^"
    'String�^��xxx��XxxLen�����ȉ���ASCII�L�����N�^�ō\������镶����ł��邱�Ƃ��O��ł��B
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal fileName As String, _
       ByVal transferLimitTicks As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ContinueCode = continueCode
        Me.FileName = fileName
        Me._FileHashValue = ""
        Me._TransferLimitTicks = transferLimitTicks
    End Sub

    'String�^��xxx��XxxLen�����ȉ���ASCII�L�����N�^�ō\������镶����ł��邱�Ƃ��O��ł��B
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal fileName As String, _
       ByVal fileHashValue As String, _
       ByVal transferLimitTicks As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ContinueCode = continueCode
        Me.FileName = fileName
        Me._FileHashValue = fileHashValue
        Me._TransferLimitTicks = transferLimitTicks
    End Sub

    Public Sub New( _
       ByVal oTeleg As ITelegram, _
       ByVal transferLimitTicks As Integer)

        MyBase.New(oTeleg)
        Me._FileHashValue = ""
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

        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(FileNamePos), FileNameLen) Then
            Log.Error("FileName is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        If Not Utility.IsValidVirtualPath(FileName) Then
            Log.Error("FileName is invalid (illegal path).")
            Return EkNakCauseCode.TelegramError
        End If

        Return EkNakCauseCode.None
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Private Function CreateIAckTelegram() As IXllTelegram Implements IXllReqTelegram.CreateAckTelegram
        Return New EkServerDrivenUllAckTelegram(Gene, ObjCode, ContinueCode, _FileHashValue)
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As EkServerDrivenUllAckTelegram
        Return New EkServerDrivenUllAckTelegram(Gene, ObjCode, ContinueCode, _FileHashValue)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkServerDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Private Function ParseAsIXllAck(ByVal oReplyTeleg As ITelegram) As IXllTelegram Implements IXllReqTelegram.ParseAsAck
        Return New EkServerDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkServerDrivenUllAckTelegram
        Return New EkServerDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^�𓯈�^�ɕϊ����郁�\�b�h
    Public Function ParseAsSameKind(ByVal oNextTeleg As ITelegram) As IXllReqTelegram Implements IXllReqTelegram.ParseAsSameKind
        Return New EkServerDrivenUllReqTelegram(oNextTeleg, TransferLimitTicks)
    End Function

    '�㑱REQ�d���𐶐����郁�\�b�h
    Public Function CreateContinuousTelegram(ByVal continueCode As ContinueCode, ByVal transferLimitTicks As Integer, ByVal replyLimitTicks As Integer) As EkServerDrivenUllReqTelegram
        Return New EkServerDrivenUllReqTelegram( _
           Gene, _
           ObjCode, _
           continueCode, _
           FileName, _
           _FileHashValue, _
           transferLimitTicks, _
           replyLimitTicks)
    End Function

    '�n���ꂽ����^�d����ObjDetail��������̃t�@�C���]���������Ă��邩���肷�郁�\�b�h
    Function IsContinuousWith(ByVal oXllReqTeleg As IXllReqTelegram) As Boolean Implements IXllReqTelegram.IsContinuousWith
        Dim oRealTeleg As EkServerDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkServerDrivenUllReqTelegram)
        If FileName <> oRealTeleg.FileName Then Return False
        'NOTE: �T�[�o����J�n����ULL�V�[�P���X�ł́A
        'REQ�d�����̂Ƀn�b�V���l���i�[���鍀�ڂ������B
        '����āA�n�b�V���l�͔�r���Ȃ��B
        Return True
    End Function

    'ACK�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    Public Sub ImportFileDependentValueFromAck(ByVal oReplyTeleg As IXllTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromAck
        Dim oRealReplyTeleg As EkServerDrivenUllAckTelegram = DirectCast(oReplyTeleg, EkServerDrivenUllAckTelegram)
        _FileHashValue = oRealReplyTeleg.FileHashValue
        'NOTE: �t�@�C���]��������d���œ`���邱�Ƃ��ł���v���g�R���̏ꍇ�A
        '���̃V�[�P���X�ł́AACK�d���ɓ]�������ɑ��������񂪊i�[�����
        '�͂��ł���B�������A�w���@��n�v���g�R���ł́A���̂悤�ȏ���
        'ACK�d�����ɑ��݂����A�ŏ���REQ�d���̃R���X�g���N�^�Ŏw�肷�邱�Ƃ�
        '�Ȃ��Ă���B����āA�����ł́A�]�������̎�荞�݂͍s��Ȃ��B
    End Sub

    '����^�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    Public Sub ImportFileDependentValueFromSameKind(ByVal oPreviousTeleg As IXllReqTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromSameKind
        Dim oRealPreviousTeleg As EkServerDrivenUllReqTelegram = DirectCast(oPreviousTeleg, EkServerDrivenUllReqTelegram)
        _FileHashValue = oRealPreviousTeleg._FileHashValue
        _TransferLimitTicks = oRealPreviousTeleg._TransferLimitTicks
    End Sub

    'HashValue���ɒl���Z�b�g���郁�\�b�h
    Public Sub UpdateHashValue() Implements IXllReqTelegram.UpdateHashValue
        Dim sPath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), FileName)
        Try
            _FileHashValue = Utility.CalculateMD5(sPath)
        Catch ex As Exception
            Log.Error("Some Exception caught.", ex)
            'NOTE: �ȉ��̂悤��MD5�Ƃ��Ă��蓾�Ȃ��l�ɂ��邱�ƂŁA
            '��������Ƃɂ���ACK�d���𑊎�Ɉُ�Ɣ��f���Ă��炤�B
            'NOTE: �{���́A���̃��\�b�h���Ă΂��O�ɁA�������A�N�Z�X�\��
            '�t�@�C����ݒu���Ă������Ƃ́A�A�v���̐Ӗ��ł���A
            '��O�͂����ŃL���b�`����ׂ��ł͂Ȃ���������Ȃ����A
            '�n�[�h�E�F�A�̏�Q���Ŕ��������ُ�ŁA�����Ȃ藎����̂�
            '�����ł��邽�߁A�Ƃ肠�����A���̂悤�ɂ��Ă����B
            _FileHashValue = ""
        End Try
    End Sub
#End Region

End Class
