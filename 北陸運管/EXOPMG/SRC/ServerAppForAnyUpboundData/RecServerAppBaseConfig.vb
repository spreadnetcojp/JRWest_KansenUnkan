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

Public Class RecServerAppBaseConfig
    Inherits ServerAppBaseConfig

    '�ǂݏo���Ώۂ̃��b�Z�[�W�L���[
    Public Shared MyMqPath As String

    '�Ď��Ղ���̃f�[�^�̎�M�|�[�g�ԍ�
    Public Shared InputIpPortFromKanshiban As Integer

    '��������̃f�[�^�̎�M�|�[�g�ԍ�
    Public Shared InputIpPortFromTokatsu As Integer

    '��������̃f�[�^�̎�M�|�[�g�ԍ�
    Public Shared InputIpPortFromMadosho As Integer

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const KANSHIBAN_PORT_KEY As String = "ToKanshibanTelegConnectionPort"
    Private Const TOKATSU_PORT_KEY As String = "ToTokatsuTelegConnectionPort"
    Private Const MADOSHO_PORT_KEY As String = "ToMadoshoTelegConnectionPort"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̓o�^�n�v���Z�X�ɕK�{�̐ݒ�l����荞�ށB</summary>
    Public Shared Sub RecServerAppBaseInit(ByVal sIniFilePath As String, ByVal sDataName As String)
        Dim sAppIdentifier As String = "For" & sDataName
        ServerAppBaseInit(sIniFilePath, sAppIdentifier)

        Try
            ReadFileElem(MQ_SECTION, sAppIdentifier & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(NETWORK_SECTION, KANSHIBAN_PORT_KEY)
            InputIpPortFromKanshiban = Integer.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, TOKATSU_PORT_KEY)
            InputIpPortFromTokatsu = Integer.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, MADOSHO_PORT_KEY)
            InputIpPortFromMadosho = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    'NOTE: �Ăяo���Ȃ��Ă����Ȃ��B
    Public Shared Sub Dispose()
        ServerAppBaseDispose()
    End Sub

End Class
