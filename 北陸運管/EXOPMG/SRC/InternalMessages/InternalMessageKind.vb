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
''' �������b�Z�[�W�̎�ʁB
''' </summary>
Public Class InternalMessageKind
    Public Const QuitRequest As Integer = 32

    Public Const ConnectNotice As Integer = 64
    Public Const DisconnectRequest As Integer = 65

    Public Const DownloadRequest As Integer = 96
    Public Const DownloadResponse As Integer = 97
    Public Const UploadRequest As Integer = 98
    Public Const UploadResponse As Integer = 99

    Public Const AppDefinitionBase As Integer = 1024
End Class
