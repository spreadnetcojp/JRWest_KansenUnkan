' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/01/14  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' �e�X���b�h����MyTelegrapher�ւ̐ڑ��v�����b�Z�[�W�B
''' </summary>
Public Structure ConnectRequest
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen() As InternalMessage
        Return New InternalMessage(MyInternalMessageKind.ConnectRequest)
    End Function
#End Region
End Structure
