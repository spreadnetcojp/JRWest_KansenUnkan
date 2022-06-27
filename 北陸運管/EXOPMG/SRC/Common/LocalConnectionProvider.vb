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
Option Strict On
Option Explicit On

Imports System.Net
Imports System.Net.Sockets

''' <summary>
''' ���[�J���ڑ������N���X
''' </summary>
Public Class LocalConnectionProvider

#Region "�萔��ϐ�"
    '�r���p�I�u�W�F�N�g
    'NOTE: �����̃X���b�h�ɂ��oListenerSock�̕ύX�ƎQ�Ƃ�A�����̃X���b�h�ɂ��
    '�R�l�N�V�����̐����iConnect�̎��{�j����A����iConnect�̖߂�l�j�ɑΉ�
    '����T�[�o���ʐM�p�\�P�b�g�̎��o���iAccept�̌Ăяo�������j�܂ł̏���
    '���m��r���I�ɍs�킹�邽�߂̂��̂ł���B
    '�Ȃ��A�{���I�ɂ́AoListenerSock���Q�Ƃ��꓾��iCreateSockets���\�b�h��
    '���s���꓾��j���Ԃ�oListenerSock�̕ύX�iInit��Dispose�̎��s�j���s��Ȃ��悤��
    '���邱�Ƃ́A�Ăяo�����̐Ӗ��ł���B
    Private Shared ReadOnly oListenerLockObject As New Object()

    '���X�j���O�\�P�b�g
    Private Shared oListenerSock As Socket
#End Region

#Region " +s Init()  ������"
    ''' <summary>
    ''' �N���X�̏�����
    ''' </summary>
    ''' <remarks>
    ''' �N���X���g�p�\�ɂ���B
    ''' </remarks>
    Public Shared Sub Init()
        SyncLock oListenerLockObject
            If oListenerSock IsNot Nothing Then oListenerSock.Close()  '�{���̓G���[�Ƃ��Ă悢�B
            oListenerSock = SockUtil.StartLocalListener(0)
        End SyncLock
    End Sub
#End Region

#Region " +s Dispose()  �j��"
    ''' <summary>
    ''' �N���X�̔j��
    ''' </summary>
    ''' <remarks>
    ''' �N���X��j���ɂ���B
    ''' </remarks>
    Public Shared Sub Dispose()
        SyncLock oListenerLockObject
            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
                oListenerSock = Nothing
            End If
        End SyncLock
    End Sub
#End Region

#Region " +s CreateSockets()  �\�P�b�g�쐬"
    ''' <summary>
    ''' �\�P�b�g�쐬
    ''' </summary>
    ''' <param name="oSock1">Socket</param>
    ''' <param name="oSock2">Socket</param>
    ''' <remarks>
    ''' ���[�J���ڑ��𐶐����A���[�J���ʐM�p�\�P�b�g���擾����B
    ''' </remarks>
    Public Shared Sub CreateSockets(ByRef oSock1 As Socket, ByRef oSock2 As Socket)
        Dim oSock1t As Socket = Nothing
        Dim oSock2t As Socket = Nothing

        SyncLock oListenerLockObject
            Try
                Dim portNo As Integer = DirectCast(oListenerSock.LocalEndPoint, IPEndPoint).Port
                oSock2t = SockUtil.ConnectToLocal(portNo)
                oSock1t = SockUtil.Accept(oListenerSock)
            Catch ex As Exception
                If oSock2t IsNot Nothing Then
                    oSock2t.Close()
                End If
                If oSock1t IsNot Nothing Then
                    oSock1t.Close()
                End If
                'OPT: �ꎞ�I�Ƀ��\�[�X������Ȃ������ł���\�����l����ƁA
                '�߂�l�Œʒm���������悢��������Ȃ��B
                Throw
            End Try
        End SyncLock

        oSock1 = oSock1t
        oSock2 = oSock2t
    End Sub
#End Region

End Class
