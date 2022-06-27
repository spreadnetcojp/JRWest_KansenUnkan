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
''' �e�X���b�h����MyTelegrapher�ւ̔\���IULL�V�[�P���X���{�v�����b�Z�[�W�B
''' </summary>
Public Structure ActiveUllExecRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As ActiveUllExecRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, ActiveUllExecRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As ActiveUllExecRequestExtendPart) As InternalMessage
        Return New InternalMessage(MyInternalMessageKind.ActiveUllExecRequest, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As ActiveUllExecRequest
        Debug.Assert(msg.Kind = MyInternalMessageKind.ActiveUllExecRequest)

        Dim ret As ActiveUllExecRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class ActiveUllExecRequestExtendPart
    Public ObjCode As Integer
    Public TransferFileName As String
    Public ApplyFilePath As String
    Public ApplyFileHashValue As String
    Public TransferLimitTicks As Integer
    Public ReplyLimitTicksOnStart As Integer
    Public ReplyLimitTicksOnFinish As Integer
    Public RetryIntervalTicks As Integer
    Public MaxRetryCountToForget As Integer
    Public MaxRetryCountToCare As Integer
    Public DeleteApplyFileIfCompleted As Boolean
    Public ApplyFileMustExists As Boolean
End Class
