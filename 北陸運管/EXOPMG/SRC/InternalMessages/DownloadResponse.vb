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
''' IXllWorker�����X���b�h����ClientTelegrapher�ւ̃_�E�����[�h�������b�Z�[�W�B
''' </summary>
Public Structure DownloadResponse
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property Result() As DownloadResult
        Get
            Return DirectCast(InternalMessage.Parse(RawBytes).GetExtendInteger1(), DownloadResult)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal result As DownloadResult) As InternalMessage
        Return New InternalMessage(InternalMessageKind.DownloadResponse, result, 0)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As DownloadResponse
        Debug.Assert(msg.Kind = InternalMessageKind.DownloadResponse)

        Dim ret As DownloadResponse
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

Public Enum DownloadResult As Integer
    Finished
    Aborted
End Enum
