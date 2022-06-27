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
''' �e�X���b�h����Client�nTelegrapher�ւ̃}�X�^/�v���O����ULL�v�����b�Z�[�W�B
''' </summary>
Public Structure MasProUllRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property FileName() As String
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, String)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal fileName As String) As InternalMessage
        Return New InternalMessage(ClientAppInternalMessageKind.MasProUllRequest, fileName)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As MasProUllRequest
        Debug.Assert(msg.Kind = ClientAppInternalMessageKind.MasProUllRequest)

        Dim ret As MasProUllRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure
