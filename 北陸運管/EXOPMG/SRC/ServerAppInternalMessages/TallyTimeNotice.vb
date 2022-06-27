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
''' �΂m�ԒʐM�v���Z�X��Listener����Telegrapher�ւ̏W�v�����ʒm���b�Z�[�W�B
''' </summary>
Public Structure TallyTimeNotice
#Region "�ϐ�"
    Private RawBytes As Byte()
#End Region

#Region "InternalMessage�C���X�^���X�������\�b�h"
    Public Shared Function Gen() As InternalMessage
        Return New InternalMessage(ServerAppInternalMessageKind.TallyTimeNotice)
    End Function
#End Region

#Region "InternalMessage����̕ϊ����\�b�h"
    Public Shared Function Parse(ByVal msg As InternalMessage) As TallyTimeNotice
        Debug.Assert(msg.Kind = ServerAppInternalMessageKind.TallyTimeNotice)

        Dim ret As TallyTimeNotice
        ret.RawBytes = msg.RawBytes
        Return ret
    End Function
#End Region
End Structure
