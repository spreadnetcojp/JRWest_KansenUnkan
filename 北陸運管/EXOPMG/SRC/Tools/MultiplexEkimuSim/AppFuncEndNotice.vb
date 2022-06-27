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

Imports JR.ExOpmg.Common

''' <summary>
''' �e�X���b�h����MyTelegrapher�ւ̊O���֐��Ăяo���I���ʒm���b�Z�[�W�B
''' </summary>
Public Structure AppFuncEndNotice
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As AppFuncEndNoticeExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, AppFuncEndNoticeExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As AppFuncEndNoticeExtendPart) As InternalMessage
        Return New InternalMessage(MyInternalMessageKind.AppFuncEndNotice, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As AppFuncEndNotice
        Debug.Assert(msg.Kind = MyInternalMessageKind.AppFuncEndNotice)

        Dim ret As AppFuncEndNotice
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class AppFuncEndNoticeExtendPart
    Public CorrelationId As String
    Public Completed As Boolean
    Public Result As String
End Class
