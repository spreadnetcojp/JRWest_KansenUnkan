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
''' MultiplexEkimuSim�ɑ΂���O������̑��상�b�Z�[�W��Body�̌^�B
''' </summary>
Public Structure ExtSimFuncMessageBody
    Public MachineId As String
    Public Verb As String
    Public Params As Object()
End Structure
