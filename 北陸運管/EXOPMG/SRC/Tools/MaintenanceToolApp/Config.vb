' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2014/04/20  (NES)      �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits BaseConfig

    '�L�����O���
    Public Shared LogKindsMask As Integer

    '���O��ێ��������
    Public Shared LogsKeepingDays As Integer

    '���u��ʁi�E�B���h�E�^�C�g���ɕ\�����閼�́j
    Public Shared MachineKind As String

    '�E�B���h�E�^�C�g���ɕ\������o�[�W�����ԍ�
    Public Shared VerNoSet As String

    'INI�t�@�C�����̃Z�N�V������
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const STORAGE_LIFE_SECTION As String = "StorageLife"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const LOGS_KEEPING_DAYS_KEY As String = "LogsKeepingDays"
    Private Const MACHINE_KIND_KEY As String = "MachineKind"
    Private Const VER_NO_SET_KEY As String = "VerNoSet"

    ''' <summary>INI�t�@�C������}�X�^�ϊ��c�[���ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Try
            ReadFileElem(CREDENTIAL_SECTION, MACHINE_KIND_KEY)
            MachineKind = LastReadValue

            ReadFileElem(CREDENTIAL_SECTION, VER_NO_SET_KEY)
            VerNoSet = LastReadValue

            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, LOGS_KEEPING_DAYS_KEY)
            LogsKeepingDays = Integer.Parse(LastReadValue)

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
