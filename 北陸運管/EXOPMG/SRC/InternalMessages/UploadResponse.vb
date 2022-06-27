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
''' IXllWorker�����X���b�h����ClientTelegrapher�ւ̃A�b�v���[�h�������b�Z�[�W�B
''' </summary>
Public Structure UploadResponse
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property Result() As UploadResult
        Get
            Return DirectCast(InternalMessage.Parse(RawBytes).GetExtendInteger1(), UploadResult)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal result As UploadResult) As InternalMessage
        Return New InternalMessage(InternalMessageKind.UploadResponse, result, 0)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As UploadResponse
        Debug.Assert(msg.Kind = InternalMessageKind.UploadResponse)

        Dim ret As UploadResponse
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

Public Enum UploadResult As Integer
    Finished
    Aborted
End Enum
