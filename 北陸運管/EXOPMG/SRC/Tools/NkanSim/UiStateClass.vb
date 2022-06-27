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

Imports System.IO

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

Public Class UiStateClass

    '�u��{�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    Public AutomaticComStart As Boolean
    Public CapSndTelegs As Boolean
    Public CapRcvTelegs As Boolean
    Public CapSndFiles As Boolean
    Public CapRcvFiles As Boolean

    '�u�d�����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���


    '�uPOST�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    Public StatusCodeForPassivePostSeqCodes As Dictionary(Of NkSeqCode, UShort)

    Public Sub New()
        '�u��{�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.AutomaticComStart = True
        Me.CapSndTelegs = True
        Me.CapRcvTelegs = True
        Me.CapSndFiles = False
        Me.CapRcvFiles = False

        '�u�d�����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���


        '�uPOST�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.StatusCodeForPassivePostSeqCodes = New Dictionary(Of NkSeqCode, UShort)
            RegisterPathToPassivePostSeqCodes(NkSeqCode.Collection, 0)
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g��M�f�B���N�g���p�X��
    'Me.ApplyFileForPassivePostSeqCodes�ɒǉ�����B
    Private Sub RegisterPathToPassivePostSeqCodes(ByVal seqCode As NkSeqCode, ByVal statusCode As UShort)
        Me.StatusCodeForPassivePostSeqCodes.Add(seqCode, statusCode)
    End Sub

End Class
