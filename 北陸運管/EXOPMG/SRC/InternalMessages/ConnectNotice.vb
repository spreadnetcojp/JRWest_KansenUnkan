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

Imports System.Net.Sockets

''' <summary>
''' �e�X���b�h����e��Telegrapher�ւ̐ڑ��ʒm���b�Z�[�W�B
''' </summary>
Public Structure ConnectNotice
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�A�N�Z�T"
    Public Function GetSocket() As Socket
        Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
        Dim inf As SocketInformation = DirectCast(obj, SocketInformation)
        Return New Socket(inf)
    End Function
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal sock As Socket) As InternalMessage
        Dim curProcess As System.Diagnostics.Process = System.Diagnostics.Process.GetCurrentProcess()
        Dim inf As SocketInformation = sock.DuplicateAndClose(curProcess.Id)
        curProcess.Close()
        Return New InternalMessage(InternalMessageKind.ConnectNotice, inf)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As ConnectNotice
        Debug.Assert(msg.Kind = InternalMessageKind.ConnectNotice)

        Dim ret As ConnectNotice
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure
