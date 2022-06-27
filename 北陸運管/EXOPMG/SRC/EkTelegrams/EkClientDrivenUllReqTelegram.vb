' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �ł���{�I�ȃN���C�A���g���ULL��REQ�d���B
''' </summary>
Public Class EkClientDrivenUllReqTelegram
    Inherits EkReqTelegram
    Implements IXllReqTelegram

#Region "�萔"
    Public Const FormalObjCodeAsOpClientFile As Byte = &H18
    Public Const FormalObjCodeAsKsbProgramVerInfo As Byte = &HAE
    Public Const FormalObjCodeAsGateMasterVerInfo As Byte = &HAF
    Public Const FormalObjCodeAsGateProgramVerInfo As Byte = &HAD
    '-------Ver0.1 ������ԕ�Ή� DEL START-----------
    'Public Const FormalObjCodeAsGateRiyoData As Byte = &HAC
    '-------Ver0.1 ������ԕ�Ή� DEL END-------------
    Public Const FormalObjCodeAsMadoMasterVerInfo As Byte = &H8B
    Public Const FormalObjCodeAsMadoProgramVerInfo As Byte = &H87
    '-------Ver0.1 ������ԕ�Ή� DEL START-----------
    'Public Const FormalObjCodeAsMadoRiyoData As Byte = &HA0
    '-------Ver0.1 ������ԕ�Ή� DEL END-------------

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
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ContinueCode = continueCode
        Me.FileName = fileName
        Me.FileHashValue = ""
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
        Me.FileHashValue = fileHashValue
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
        Return New EkClientDrivenUllReqTelegram(oNextTeleg, TransferLimitTicks)
    End Function

    '�㑱REQ�d���𐶐����郁�\�b�h
    Public Function CreateContinuousTelegram(ByVal continueCode As ContinueCode, ByVal transferLimitTicks As Integer, ByVal replyLimitTicks As Integer) As EkClientDrivenUllReqTelegram
        Return New EkClientDrivenUllReqTelegram( _
           Gene, _
           ObjCode, _
           continueCode, _
           FileName, _
           FileHashValue, _
           transferLimitTicks, _
           replyLimitTicks)
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
