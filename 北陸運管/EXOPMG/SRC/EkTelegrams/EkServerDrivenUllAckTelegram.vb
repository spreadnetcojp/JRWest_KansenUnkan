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
''' �ł���{�I�ȃT�[�o���ULL��ACK�d���B
''' </summary>
Public Class EkServerDrivenUllAckTelegram
    Inherits EkTelegram
    Implements IXllTelegram

#Region "�萔"
    Private Const ContinueCodePos As Integer = 0
    Private Const ContinueCodeLen As Integer = 1
    Private Const FileHashValuePos As Integer = ContinueCodePos + ContinueCodeLen
    Private Const FileHashValueLen As Integer = 32
    Private Const ObjDetailLen As Integer = FileHashValuePos + FileHashValueLen
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
            If EkServerDrivenUllReqTelegram.oContinueCodeTable.TryGetValue(RawBytes(GetRawPos(ContinueCodePos)), code) = False Then
                code = ContinueCode.None
            End If
            Return code
        End Get

        Set(ByVal code As ContinueCode)
            RawBytes(GetRawPos(ContinueCodePos)) = EkServerDrivenUllReqTelegram.oRawContinueCodeTable(code)
        End Set
    End Property

    Public ReadOnly Property RawContinueCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(ContinueCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(ContinueCodePos), ret, 0, ContinueCodeLen)
            Return ret
        End Get
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
#End Region

#Region "�R���X�g���N�^"
    'String�^��xxx��XxxLen�����ȉ���ASCII�L�����N�^�ō\������镶����ł��邱�Ƃ��O��ł��B
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal fileHashValue As String)

        MyBase.New(oGene, EkCmdCode.Ack, EkSubCmdCode.Get, objCode, ObjDetailLen)
        Me.ContinueCode = continueCode
        Me.FileHashValue = fileHashValue
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

        'NOTE: �n�b�V���l�́A��̔�r�Ń`�F�b�N�����͂��ł��邽�߁A�����ł�
        '�`�F�b�N�͊ɂ߂ɂ���i������ɕϊ��\�ł��肳������΂悢�j�B
        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen) Then
            Log.Error("FileHashValue is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        '�����ȍ~�A�v���p�e�B�ɃA�N�Z�X�\�B

        Return EkNakCauseCode.None
    End Function
#End Region

End Class
