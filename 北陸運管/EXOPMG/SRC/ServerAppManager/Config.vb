' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/06/07  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '�풓�v���Z�X�̒�~���e����
    Public Shared ResidentAppPendingLimitTicks As Integer

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const APP_ID As String = "Manager"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const RESIDENT_APP_PENDING_LIMIT_KEY As String = "ResidentAppPendingLimitTicks"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̃v���Z�X�}�l�[�W���ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Try
            ReadFileElem(TIME_INFO_SECTION, RESIDENT_APP_PENDING_LIMIT_KEY)
            ResidentAppPendingLimitTicks = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    Public Shared Sub Dispose()
        ServerAppBaseDispose()
    End Sub

End Class
