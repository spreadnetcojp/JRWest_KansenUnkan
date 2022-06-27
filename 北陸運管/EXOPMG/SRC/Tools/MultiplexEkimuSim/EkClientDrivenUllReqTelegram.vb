' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/01/14  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �ł���{�I�ȃN���C�A���g���ULL��REQ�d���i�I���W�i���t�@�C�����i�[�@�\���j�B
''' </summary>
Public Class EkClientDrivenUllReqTelegram
    Inherits EkReqTelegram
    Implements IXllReqTelegram

#Region "�萔"
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
    Private Const FileHashValuePos As Integer = FileNamePos + FileNameLen
    Private Const FileHashValueLen As Integer = 32
    Private Const ObjDetailLen As Integer = FileHashValuePos + FileHashValueLen
#End Region

#Region "�ϐ�"
    Private _TransferLimitTicks As Integer
    Private _AltReplyLimitTicks As Integer
    Private _OriginalFilePath As String
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
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal fileHashValue As String)
            Array.Clear(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen)
            Encoding.UTF8.GetBytes(fileHashValue, 0, fileHashValue.Length, RawBytes, GetRawPos(FileHashValuePos))
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

    Public Property AltReplyLimitTicks() As Integer
        Get
            Return _AltReplyLimitTicks
        End Get

        Set(ByVal ticks As Integer)
            _AltReplyLimitTicks = ticks
        End Set
    End Property

    Public Property OriginalFilePath() As String
        Get
            Return _OriginalFilePath
        End Get

        Set(ByVal filePath As String)
            _OriginalFilePath = filePath
        End Set
    End Property

    Public ReadOnly Property IsHashValueReady() As Boolean Implements IXllReqTelegram.IsHashValueReady
        Get
            Return FileHashValue.Length <> 0
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
            Return StringComparer.OrdinalIgnoreCase.Compare(sHashValue, FileHashValue) = 0
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
       ByVal replyLimitTicks As Integer, _
       ByVal altReplyLimitTicks As Integer, _
       ByVal originalFilePath As String)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ContinueCode = continueCode
        Me.FileName = fileName
        Me.FileHashValue = ""
        Me._TransferLimitTicks = transferLimitTicks
        Me._AltReplyLimitTicks = altReplyLimitTicks
        Me._OriginalFilePath = originalFilePath
    End Sub

    'String�^��xxx��XxxLen�����ȉ���ASCII�L�����N�^�ō\������镶����ł��邱�Ƃ��O��ł��B
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal fileName As String, _
       ByVal fileHashValue As String, _
       ByVal transferLimitTicks As Integer, _
       ByVal replyLimitTicks As Integer, _
       ByVal altReplyLimitTicks As Integer, _
       ByVal originalFilePath As String)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ContinueCode = continueCode
        Me.FileName = fileName
        Me.FileHashValue = fileHashValue
        Me._TransferLimitTicks = transferLimitTicks
        Me._AltReplyLimitTicks = altReplyLimitTicks
        Me._OriginalFilePath = originalFilePath
    End Sub

    'NOTE: �T�[�o���ɂ����āA���{���̃V�[�P���X�Ɩ��֌W�Ȏ�M�d���iEkDodgyTelegram�^�j��
    '�{�N���X�ɕϊ����邱�Ƃɓ��������R���X�g���N�^�ł���B
    Public Sub New( _
       ByVal oTeleg As ITelegram, _
       ByVal transferLimitTicks As Integer)

        MyBase.New(oTeleg)
        Me._TransferLimitTicks = transferLimitTicks
        Me._AltReplyLimitTicks = 0
        Me._OriginalFilePath = Nothing
    End Sub

    Public Sub New(ByVal iTeleg As ITelegram)
        MyBase.New(iTeleg)
        If TypeOf iTeleg Is EkClientDrivenUllReqTelegram Then
            Me._TransferLimitTicks = DirectCast(iTeleg, EkClientDrivenUllReqTelegram)._TransferLimitTicks
            Me._AltReplyLimitTicks = DirectCast(iTeleg, EkClientDrivenUllReqTelegram)._AltReplyLimitTicks
            Me._OriginalFilePath = DirectCast(iTeleg, EkClientDrivenUllReqTelegram)._OriginalFilePath
        Else
            'NOTE: iTeleg����M�d���iEkDodgyTelegram�j�̏ꍇ�́A������̃P�[�X
            '�Ƃ��ď������s���邪�A����͈Ӑ}�ʂ�ł���B
            '���̃v���g�R���ł́A�d���̃o�C�g��ɕԐM�����i�T���j��I���W�i���t�@�C�����ɑ�������
            '���͊i�[����Ȃ��i�����炱���A���̂悤�Ȑ�p�����o�ɕʓr�ݒ肷�邱�ƂɂȂ��Ă���j�B
            '���Ȃ킿�A�����v���p�e�B�́AImportFileDependentValueFromSameKind()�ŃR�s�[�����
            'TransferLimitTicks�������AREQ�d���𑗐M���鑤�ł݈̂Ӗ������B
            Me._TransferLimitTicks = 0
            Me._AltReplyLimitTicks = 0
            Me._OriginalFilePath = Nothing
        End If
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

        'NOTE: �n�b�V���l�́A��̔�r�Ń`�F�b�N�����͂��ł��邽�߁A�����ł�
        '�`�F�b�N�͊ɂ߂ɂ���i������ɕϊ��\�ł��肳������΂悢�j�B
        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen) Then
            Log.Error("FileHashValue is invalid.")
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
        Return New EkClientDrivenUllAckTelegram(Gene, ObjCode, ContinueCode, FileHashValue)
    End Function

    'ACK�d���𐶐����郁�\�b�h
    Public Function CreateAckTelegram() As EkClientDrivenUllAckTelegram
        Return New EkClientDrivenUllAckTelegram(Gene, ObjCode, ContinueCode, FileHashValue)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkClientDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Private Function ParseAsIXllAck(ByVal oReplyTeleg As ITelegram) As IXllTelegram Implements IXllReqTelegram.ParseAsAck
        Return New EkClientDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkClientDrivenUllAckTelegram
        Return New EkClientDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '�n���ꂽ�d���̌^�𓯈�^�ɕϊ����郁�\�b�h
    Public Function ParseAsSameKind(ByVal oNextTeleg As ITelegram) As IXllReqTelegram Implements IXllReqTelegram.ParseAsSameKind
        Return New EkClientDrivenUllReqTelegram(oNextTeleg)
    End Function

    '�㑱REQ�d���𐶐����郁�\�b�h
    Public Function CreateContinuousTelegram(ByVal continueCode As ContinueCode, ByVal transferLimitTicks As Integer, ByVal replyLimitTicks As Integer, ByVal altReplyLimitTicks As Integer, ByVal originalFilePath As String) As EkClientDrivenUllReqTelegram
        Return New EkClientDrivenUllReqTelegram( _
           Gene, _
           ObjCode, _
           continueCode, _
           FileName, _
           FileHashValue, _
           transferLimitTicks, _
           replyLimitTicks, _
           altReplyLimitTicks, _
           originalFilePath)
    End Function

    '�n���ꂽ����^�d����ObjDetail��������̃t�@�C���]���������Ă��邩���肷�郁�\�b�h
    Function IsContinuousWith(ByVal oXllReqTeleg As IXllReqTelegram) As Boolean Implements IXllReqTelegram.IsContinuousWith
        Dim oRealTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)
        If FileName <> oRealTeleg.FileName Then Return False
        'NOTE: FileHashValue�͔�r���Ȃ����Ƃɂ��Ă���B
        Return True
    End Function

    'ACK�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    Public Sub ImportFileDependentValueFromAck(ByVal oReplyTeleg As IXllTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromAck
        Dim oRealReplyTeleg As EkClientDrivenUllAckTelegram = DirectCast(oReplyTeleg, EkClientDrivenUllAckTelegram)
        FileHashValue = oRealReplyTeleg.FileHashValue
        'NOTE: ���̃V�[�P���X�ł́A���̃��\�b�h���Ă΂�邱�Ƃ͂Ȃ��͂��ł��邪�A
        '�����\�ł��邽�߁A�Ƃ肠�����������Ă���B
        '�Ȃ��A�ǂ̂悤�ȃv���g�R���ł����Ă��A���̃V�[�P���X��ACK�d����
        '�t�@�C���]�������ɑ��������񂪐ݒ肳��Ă��邱�Ƃ͂��蓾�Ȃ����߁A
        '�t�@�C���]�������̎�荞�݂͍s��Ȃ��B
    End Sub

    '����^�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    Public Sub ImportFileDependentValueFromSameKind(ByVal oPreviousTeleg As IXllReqTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromSameKind
        Dim oRealPreviousTeleg As EkClientDrivenUllReqTelegram = DirectCast(oPreviousTeleg, EkClientDrivenUllReqTelegram)
        FileHashValue = oRealPreviousTeleg.FileHashValue
        _TransferLimitTicks = oRealPreviousTeleg._TransferLimitTicks
    End Sub

    'HashValue���ɒl���Z�b�g���郁�\�b�h
    Public Sub UpdateHashValue() Implements IXllReqTelegram.UpdateHashValue
        Dim sPath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), FileName)
        Try
            FileHashValue = Utility.CalculateMD5(sPath)
        Catch ex As Exception
            Log.Error("Some Exception caught.", ex)
            'NOTE: �ȉ��̂悤��MD5�Ƃ��Ă��蓾�Ȃ��l�ɂ��邱�ƂŁA
            '����Ɉُ�ȓd���Ɣ��f���Ă��炤�B
            'NOTE: �{���́A���̃��\�b�h���Ă΂��O�ɁA�������A�N�Z�X�\��
            '�t�@�C����ݒu���Ă������Ƃ́A�A�v���̐Ӗ��ł���A
            '��O�͂����ŃL���b�`����ׂ��ł͂Ȃ���������Ȃ����A
            '�n�[�h�E�F�A�̏�Q���Ŕ��������ُ�ŁA�����Ȃ藎����̂�
            '�����ł��邽�߁A�Ƃ肠�����A���̂悤�ɂ��Ă����B
            FileHashValue = ""
        End Try
    End Sub
#End Region

End Class
