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
''' �m�ԋ��ʃV�~�����[�^�p�������b�Z�[�W�̎�ʁB
''' </summary>
Public Class MyInternalMessageKind
    Inherits InternalMessageKind

    Public Const ActiveOneExecRequest As Integer = AppDefinitionBase
    Public Const ComStartExecRequest As Integer = AppDefinitionBase + 1
    Public Const InquiryExecRequest As Integer = AppDefinitionBase + 2
End Class
