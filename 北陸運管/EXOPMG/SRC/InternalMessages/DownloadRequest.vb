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
''' ClientTelegrapher����IXllWorker�����X���b�h�ւ̃_�E�����[�h�v�����b�Z�[�W�B
''' </summary>
Public Structure DownloadRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As DownloadRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, DownloadRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As DownloadRequestExtendPart) As InternalMessage
        Return New InternalMessage(InternalMessageKind.DownloadRequest, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As DownloadRequest
        Debug.Assert(msg.Kind = InternalMessageKind.DownloadRequest)

        Dim ret As DownloadRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class DownloadRequestExtendPart
    '�]���Ώۃt�@�C�����̃x�[�X�Ƃ��郍�[�J���p�X
    Public TransferListBase As String

    '�]���Ώۃt�@�C�����̈ꗗ
    Public TransferList As List(Of String)
End Class
