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
''' MultiplexEkimuSim����O���풓�v���Z�X�ւ̗v�����b�Z�[�W�����
''' ���̉������b�Z�[�W��Body�̌^�B
''' </summary>
Public Structure ExtAppFuncMessageBody
    Public WorkingDirectory As String
    Public Func As String
    Public Args As String()
    Public Completed As Boolean
    Public Result As String
End Structure
