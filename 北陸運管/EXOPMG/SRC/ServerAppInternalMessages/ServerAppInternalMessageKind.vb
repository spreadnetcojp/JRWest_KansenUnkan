' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή��ɂ�NameChangeNotice��ǉ�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' �^�ǃT�[�o�v���Z�X�p�������b�Z�[�W�̎�ʁB
''' </summary>
Public Class ServerAppInternalMessageKind
    Inherits InternalMessageKind

    Public Const MasProDllRequest As Integer = AppDefinitionBase
    Public Const MasProDllResponse As Integer = AppDefinitionBase + 1
    Public Const ScheduledUllRequest As Integer = AppDefinitionBase + 2
    Public Const ScheduledUllResponse As Integer = AppDefinitionBase + 3
    Public Const TallyTimeNotice As Integer = AppDefinitionBase + 4
    Public Const NameChangeNotice As Integer = AppDefinitionBase + 5
End Class
