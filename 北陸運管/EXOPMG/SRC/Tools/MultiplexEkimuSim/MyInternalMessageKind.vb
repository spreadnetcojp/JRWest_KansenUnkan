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
''' ���d�w���@�틤�ʃV�~�����[�^�p�������b�Z�[�W�̎�ʁB
''' </summary>
Public Class MyInternalMessageKind
    Inherits InternalMessageKind

    Public Const ConnectRequest As Integer = AppDefinitionBase
    Public Const ActiveOneExecRequest As Integer = AppDefinitionBase + 1
    Public Const ActiveUllExecRequest As Integer = AppDefinitionBase + 2
    Public Const ComStartExecRequest As Integer = AppDefinitionBase + 3
    Public Const TimeDataGetExecRequest As Integer = AppDefinitionBase + 4
    Public Const ScenarioStartRequest As Integer = AppDefinitionBase + 5
    Public Const ScenarioStopRequest As Integer = AppDefinitionBase + 6
    Public Const AppFuncEndNotice As Integer = AppDefinitionBase + 7
End Class
