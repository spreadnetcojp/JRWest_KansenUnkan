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
''' �e�X���b�h����e��Telegrapher�ւ̐ؒf�v�����b�Z�[�W�B
''' </summary>
Public Structure DisconnectRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen() As InternalMessage
        Return New InternalMessage(InternalMessageKind.DisconnectRequest)
    End Function
#End Region
End Structure
