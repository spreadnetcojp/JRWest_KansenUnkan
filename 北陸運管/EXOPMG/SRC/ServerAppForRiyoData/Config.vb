' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '�o�^�X���b�h��~���e����
    Public Shared RecorderPendingLimitTicks As Integer

    '�o�^���s����
    Public Shared RecordingIntervalTicks As Integer

    '�P�g�����U�N�V�����œo�^����ő�t�@�C����
    Public Shared RecordingFileCountAtOnce As Integer

    '�ǂݏo���Ώۃ��b�Z�[�W�L���[�̖��O
    Public Shared MyMqPath As String

    '���p�f�[�^�̃t�H�[�}�b�g�t�@�C����SQL�t�@�C���̊i�[�ꏊ
    Public Shared RiyoDataImporterFilesBasePath As String

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const APP_ID As String = "ForRiyoData"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̗��p�f�[�^�o�^�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Try
            ReadFileElem(TIME_INFO_SECTION, APP_ID & "RecorderPendingLimitTicks")
            RecorderPendingLimitTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, APP_ID & "RecordingIntervalTicks")
            RecordingIntervalTicks = Integer.Parse(LastReadValue)

            ReadFileElem(TIME_INFO_SECTION, APP_ID & "RecordingFileCountAtOnce")
            RecordingFileCountAtOnce = Integer.Parse(LastReadValue)

            ReadFileElem(MQ_SECTION, APP_ID & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(PATH_SECTION, "RiyoDataImporterFilesBasePath")
            RiyoDataImporterFilesBasePath = LastReadValue

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
