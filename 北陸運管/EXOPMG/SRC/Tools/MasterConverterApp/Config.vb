' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/03/01  (NES)����  �V�K�쐬
'   0.2      2014/06/12  (NES)�c��  �k���Ή�
'                                   �E�c�[��Ver�̃^�C�g���\���Ή�
'                                   �E�}�X�^�ʃp�^�[���ԍ��`�F�b�N�Ή�
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

    'Ver0.2 ADD START  �k���Ή�
    Public Shared MachineKind As String
    Public Shared VerNoSet As String
    Public Shared LimitPatterns As ArrayList = New ArrayList()
    'Ver0.2 ADD END    �k���Ή�

    'INI�t�@�C�����̃Z�N�V������
    Protected Const LOGGING_SECTION As String = "Logging"
    Protected Const STORAGE_LIFE_SECTION As String = "StorageLife"
    'Ver0.2 ADD START  �k���Ή�
    Protected Const CREDENTIAL_SECTION As String = "Credential"
    Protected Const MST_INPUT_CHECK_SECTION As String = "MstInputCheck"
    'Ver0.2 ADD END    �k���Ή�

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    Private Const LOG_KINDS_MASK_KEY As String = "LogKindsMask"
    Private Const LOGS_KEEPING_DAYS_KEY As String = "LogsKeepingDays"
    'Ver0.2 ADD START  �k���Ή�
    Private Const MACHINE_KIND_KEY As String = "MachineKind"
    Private Const VER_N_OSET As String = "VerNoSet"
    Private Const LIMI_TPATTERNS_KEY As String = "LimitPattern_"
    'Ver0.2 ADD END    �k���Ή�

    ''' <summary>INI�t�@�C������}�X�^�ϊ��c�[���ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath)

        Dim i As Integer
        'Ver0.2 ADD START  �k���Ή�
        Dim workString() As String
        Dim subList As ArrayList
        'Ver0.2 ADD END    �k���Ή�

        Try
            ReadFileElem(LOGGING_SECTION, LOG_KINDS_MASK_KEY)
            LogKindsMask = Integer.Parse(LastReadValue)

            ReadFileElem(STORAGE_LIFE_SECTION, LOGS_KEEPING_DAYS_KEY)
            LogsKeepingDays = Integer.Parse(LastReadValue)


            'Ver0.2 ADD START  �k���Ή�
            '�c�[���^�C�g��
            Try
                ReadFileElem(CREDENTIAL_SECTION, MACHINE_KIND_KEY)
                MachineKind = LastReadValue
                If MachineKind.Length = 0 Then
                    MachineKind = "�w���@��}�X�^�ϊ�"
                End If
            Catch ex As Exception
                MachineKind = "�w���@��}�X�^�ϊ�"
            End Try
            '�o�[�W����
            Try
                ReadFileElem(CREDENTIAL_SECTION, VER_N_OSET)
                VerNoSet = LastReadValue
            Catch ex As Exception
                VerNoSet = ""   '�o�[�W�����w�肪������΃o�[�W������\��
            End Try


            '�}�X�^�ʂ̃p�^�[���ԍ��`�F�b�N�\�ǂݍ���
            For i = 0 To 99
                Try
                    ReadFileElem(MST_INPUT_CHECK_SECTION, LIMI_TPATTERNS_KEY & i)
                    workString = Split(LastReadValue, ",")
                    subList = New ArrayList()
                    subList.Add(workString(0))
                    subList.Add(Integer.Parse(workString(1)))
                    subList.Add(Integer.Parse(workString(2)))
                    LimitPatterns.Add(subList)
                Catch ex As Exception
                    Exit For
                End Try
            Next
            'Ver0.2 ADD END    �k���Ή�
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
