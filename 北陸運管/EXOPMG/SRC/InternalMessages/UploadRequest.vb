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

''' <summary>
''' ClientTelegrapher����IXllWorker�����X���b�h�ւ̃A�b�v���[�h�v�����b�Z�[�W�B
''' </summary>
Public Structure UploadRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As UploadRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, UploadRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As UploadRequestExtendPart) As InternalMessage
        Return New InternalMessage(InternalMessageKind.UploadRequest, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As UploadRequest
        Debug.Assert(msg.Kind = InternalMessageKind.UploadRequest)

        Dim ret As UploadRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class UploadRequestExtendPart
    '�]���Ώۃt�@�C�����̃x�[�X�Ƃ��郍�[�J���p�X
    Public TransferListBase As String

    '�]���Ώۃt�@�C�����̈ꗗ
    Public TransferList As List(Of String)
End Class
