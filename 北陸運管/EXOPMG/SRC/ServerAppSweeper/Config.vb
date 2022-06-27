' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2015/01/09  (NES)����  �����Ɩ��O�F�؃��O���W�Ή�
'   0.2      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Net.Mime
Imports JR.ExOpmg.Common

Public Class Config
    Inherits ServerAppBaseConfig

    '�������샍�O�Ǘ��f�B���N�g���̃p�X
    Public Shared MadoLogDirPath As String

    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
    '�����Ɩ��O�F�؃��O�Ǘ��f�B���N�g���̃p�X
    Public Shared MadoCertLogDirPath As String
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------

    '�z�M�p�}�X�^�̕ێ����㐔
    Public Shared MasterDataKeepingGenerations As Integer

    '�z�M�p�v���O�����̕ێ����㐔
    Public Shared ProgramDataKeepingGenerations As Integer

    '�ʏW�D�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared BesshuDataVisibleDays As Integer

    '�s����Ԍ����o�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared FuseiJoshaDataVisibleDays As Integer

    '���s�˔j���o�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared KyokoToppaDataVisibleDays As Integer

    '���������o�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared FunshitsuDataVisibleDays As Integer

    '�ُ�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared FaultDataVisibleDays As Integer

    '�ғ��f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared KadoDataVisibleDays As Integer

    '�ێ�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared HosyuDataVisibleDays As Integer

    '���ԑѕʏ�~�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared TrafficDataVisibleDays As Integer

    '���W�f�[�^��L���f�[�^�x�[�X��ŕێ��������
    Public Shared CollectedDataTypoVisibleDays As Integer

    '-------Ver0.2 ������ԕ�Ή� ADD START-----------
    '���p�f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared RiyoDataVisibleDays As Integer

    '�V�����w�茔����f�[�^���f�[�^�x�[�X��ŕێ��������
    Public Shared ShiteiDataVisibleDays As Integer
    '-------Ver0.2 ������ԕ�Ή� ADD END-------------

    '�@��ڑ���Ԃ��f�B���N�g����ŕێ��������
    Public Shared ConStatusKeepingDaysInRejectDir As Integer
    Public Shared ConStatusKeepingDaysInTrashDir As Integer
    Public Shared ConStatusKeepingDaysInQuarantineDir As Integer

    '�Ď��Րݒ�����f�B���N�g����ŕێ��������
    Public Shared KsbConfigKeepingDaysInRejectDir As Integer
    Public Shared KsbConfigKeepingDaysInTrashDir As Integer
    Public Shared KsbConfigKeepingDaysInQuarantineDir As Integer

    '�ʏW�D�f�[�^���f�B���N�g����ŕێ��������
    Public Shared BesshuDataKeepingDaysInRejectDir As Integer
    Public Shared BesshuDataKeepingDaysInTrashDir As Integer
    Public Shared BesshuDataKeepingDaysInQuarantineDir As Integer

    '���׌n�f�[�^���f�B���N�g����ŕێ��������
    Public Shared MeisaiDataKeepingDaysInRejectDir As Integer
    Public Shared MeisaiDataKeepingDaysInTrashDir As Integer
    Public Shared MeisaiDataKeepingDaysInQuarantineDir As Integer

    '�ُ�f�[�^���f�B���N�g����ŕێ��������
    Public Shared FaultDataKeepingDaysInRejectDir As Integer
    Public Shared FaultDataKeepingDaysInTrashDir As Integer
    Public Shared FaultDataKeepingDaysInQuarantineDir As Integer

    '�ғ��E�ێ�f�[�^���f�B���N�g����ŕێ��������
    Public Shared KadoDataKeepingDaysInRejectDir As Integer
    Public Shared KadoDataKeepingDaysInTrashDir As Integer
    Public Shared KadoDataKeepingDaysInQuarantineDir As Integer

    '���ԑѕʏ�~�f�[�^���f�B���N�g����ŕێ��������
    Public Shared TrafficDataKeepingDaysInRejectDir As Integer
    Public Shared TrafficDataKeepingDaysInTrashDir As Integer
    Public Shared TrafficDataKeepingDaysInQuarantineDir As Integer

    '���p�f�[�^���f�B���N�g����ŕێ��������
    Public Shared RiyoDataKeepingDaysInRejectDir As Integer
    Public Shared RiyoDataKeepingDaysInTrashDir As Integer

    '�������샍�O���f�B���N�g����ŕێ��������
    Public Shared MadoLogsKeepingDays As Integer

    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
    '�����Ɩ��O�F�؃��O���f�B���N�g����ŕێ��������
    Public Shared MadoCertLogsKeepingDays As Integer
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------

    '�^�ǃT�[�o���g�̃��O���f�B���N�g����ŕێ��������
    Public Shared LogsKeepingDays As Integer

    'INI�t�@�C�����̃Z�N�V������
    Protected Const STORAGE_LIFE_SECTION As String = "StorageLife"

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const APP_ID As String = "Sweeper"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̐􂢑ւ��v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        ServerAppBaseInit(sIniFilePath, APP_ID)

        Try
            ReadFileElem(PATH_SECTION, "MadoLogDirPath")
            MadoLogDirPath = LastReadValue

            '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
            ReadFileElem(PATH_SECTION, "MadoCertLogDirPath")
            MadoCertLogDirPath = LastReadValue
            '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------

            ReadFileElem(STORAGE_LIFE_SECTION, "MasterDataKeepingGenerations")
            MasterDataKeepingGenerations = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "ProgramDataKeepingGenerations")
            ProgramDataKeepingGenerations = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataVisibleDays")
            BesshuDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FuseiJoshaDataVisibleDays")
            FuseiJoshaDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KyokoToppaDataVisibleDays")
            KyokoToppaDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FunshitsuDataVisibleDays")
            FunshitsuDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataVisibleDays")
            FaultDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataVisibleDays")
            KadoDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "HosyuDataVisibleDays")
            HosyuDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataVisibleDays")
            TrafficDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "CollectedDataTypoVisibleDays")
            CollectedDataTypoVisibleDays = Integer.Parse(LastReadValue)

            '-------Ver0.2 ������ԕ�Ή� ADD START-----------
            ReadFileElem(STORAGE_LIFE_SECTION, "RiyoDataVisibleDays")
            RiyoDataVisibleDays = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "ShiteiDataVisibleDays")
            ShiteiDataVisibleDays = Integer.Parse(LastReadValue)
            '-------Ver0.2 ������ԕ�Ή� ADD END-------------

            ReadFileElem(STORAGE_LIFE_SECTION, "ConStatusKeepingDaysInRejectDir")
            ConStatusKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "ConStatusKeepingDaysInTrashDir")
            ConStatusKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "ConStatusKeepingDaysInQuarantineDir")
            ConStatusKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KsbConfigKeepingDaysInRejectDir")
            KsbConfigKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KsbConfigKeepingDaysInTrashDir")
            KsbConfigKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KsbConfigKeepingDaysInQuarantineDir")
            KsbConfigKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataKeepingDaysInRejectDir")
            BesshuDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataKeepingDaysInTrashDir")
            BesshuDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "BesshuDataKeepingDaysInQuarantineDir")
            BesshuDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "MeisaiDataKeepingDaysInRejectDir")
            MeisaiDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "MeisaiDataKeepingDaysInTrashDir")
            MeisaiDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "MeisaiDataKeepingDaysInQuarantineDir")
            MeisaiDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataKeepingDaysInRejectDir")
            FaultDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataKeepingDaysInTrashDir")
            FaultDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "FaultDataKeepingDaysInQuarantineDir")
            FaultDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataKeepingDaysInRejectDir")
            KadoDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataKeepingDaysInTrashDir")
            KadoDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "KadoDataKeepingDaysInQuarantineDir")
            KadoDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataKeepingDaysInRejectDir")
            TrafficDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataKeepingDaysInTrashDir")
            TrafficDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "TrafficDataKeepingDaysInQuarantineDir")
            TrafficDataKeepingDaysInQuarantineDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "RiyoDataKeepingDaysInRejectDir")
            RiyoDataKeepingDaysInRejectDir = Integer.Parse(LastReadValue)
            ReadFileElem(STORAGE_LIFE_SECTION, "RiyoDataKeepingDaysInTrashDir")
            RiyoDataKeepingDaysInTrashDir = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, "MadoLogsKeepingDays")
            MadoLogsKeepingDays = Integer.Parse(LastReadValue)

            '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
            ReadFileElem(STORAGE_LIFE_SECTION, "MadoCertLogsKeepingDays")
            MadoCertLogsKeepingDays = Integer.Parse(LastReadValue)
            '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------

            ReadFileElem(STORAGE_LIFE_SECTION, "LogsKeepingDays")
            LogsKeepingDays = Integer.Parse(LastReadValue)
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
