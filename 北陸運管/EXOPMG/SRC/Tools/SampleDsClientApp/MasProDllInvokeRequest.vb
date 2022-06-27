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
''' �e�X���b�h����Client�nTelegrapher�ւ̔z�M�w���v�����b�Z�[�W�B
''' </summary>
Public Structure MasProDllInvokeRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As MasProDllInvokeRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, MasProDllInvokeRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As MasProDllInvokeRequestExtendPart) As InternalMessage
        Return New InternalMessage(ClientAppInternalMessageKind.MasProDllInvokeRequest, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As MasProDllInvokeRequest
        Debug.Assert(msg.Kind = ClientAppInternalMessageKind.MasProDllInvokeRequest)

        Dim ret As MasProDllInvokeRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class MasProDllInvokeRequestExtendPart
    Public ListFileName As String
    Public ForcingFlag As Boolean
End Class
