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
''' Client�nTelegrapher����e�X���b�h�ւ̃}�X�^/�v���O����ULL�������b�Z�[�W�B
''' </summary>
Public Structure MasProUllResponse
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property Result() As MasProUllResult
        Get
            Return DirectCast(InternalMessage.Parse(RawBytes).GetExtendInteger1(), MasProUllResult)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal result As MasProUllResult) As InternalMessage
        Return New InternalMessage(ClientAppInternalMessageKind.MasProUllResponse, result, 0)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As MasProUllResponse
        Debug.Assert(msg.Kind = ClientAppInternalMessageKind.MasProUllResponse)

        Dim ret As MasProUllResponse
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

Public Enum MasProUllResult As Integer
    Completed
    Failed
    FailedByBusy
    FailedByInvalidContent
    FailedByUnknownLight
End Enum
