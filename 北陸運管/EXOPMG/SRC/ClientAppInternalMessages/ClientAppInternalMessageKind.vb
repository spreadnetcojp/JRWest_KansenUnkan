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
''' �^�ǒ[���v���Z�X�p�������b�Z�[�W�̎�ʁB
''' </summary>
Public Class ClientAppInternalMessageKind
    Inherits InternalMessageKind

    Public Const MasProUllRequest As Integer = AppDefinitionBase
    Public Const MasProUllResponse As Integer = AppDefinitionBase + 1
    Public Const MasProDllInvokeRequest As Integer = AppDefinitionBase + 2
    Public Const MasProDllInvokeResponse As Integer = AppDefinitionBase + 3
End Class
