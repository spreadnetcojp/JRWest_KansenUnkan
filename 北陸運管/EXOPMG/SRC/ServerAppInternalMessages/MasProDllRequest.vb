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
''' �e�X���b�h����Server�nTelegrapher�ւ̃}�X�^/�v���O����DLL�v�����b�Z�[�W�B
''' </summary>
Public Structure MasProDllRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As MasProDllRequestExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, MasProDllRequestExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As MasProDllRequestExtendPart) As InternalMessage
        Return New InternalMessage(ServerAppInternalMessageKind.MasProDllRequest, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As MasProDllRequest
        Debug.Assert(msg.Kind = ServerAppInternalMessageKind.MasProDllRequest)

        Dim ret As MasProDllRequest
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class MasProDllRequestExtendPart
    Public DataFileName As String
    Public DataFileHashValue As String
    Public ListFileName As String
    Public ListFileHashValue As String
End Class
