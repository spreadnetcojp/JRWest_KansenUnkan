' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' �e�X���b�h����Server�nTelegrapher�ւ̃N���C�A���g���ύX�ʒm���b�Z�[�W�B
''' </summary>
Public Structure NameChangeNotice
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property ExtendPart() As NameChangeNoticeExtendPart
        Get
            Dim obj As Object = InternalMessage.Parse(RawBytes).GetExtendObject()
            Return DirectCast(obj, NameChangeNoticeExtendPart)
        End Get
    End Property
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen(ByVal extend As NameChangeNoticeExtendPart) As InternalMessage
        Return New InternalMessage(ServerAppInternalMessageKind.NameChangeNotice, extend)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As NameChangeNotice
        Debug.Assert(msg.Kind = ServerAppInternalMessageKind.NameChangeNotice)

        Dim ret As NameChangeNotice
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure

<Serializable()> Public Class NameChangeNoticeExtendPart
    Public StationName As String
    Public CornerName As String
End Class
