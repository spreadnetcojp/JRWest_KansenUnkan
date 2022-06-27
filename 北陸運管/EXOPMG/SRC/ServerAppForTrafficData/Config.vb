' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits RecServerAppBaseConfig

    '�o�^�Ώۃf�[�^�����t�@�C���̃p�X
    Public Shared FormatFilePath As String

    '���̓f�[�^�ʁi�v���Z�X�ʁj�L�[�ɑ΂���v���t�B�b�N�X
    Private Const DATA_NAME As String = "TrafficData"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̎��ԑѕʏ�~�f�[�^�o�^�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)

        Try
            ReadFileElem(PATH_SECTION, "TrafficDataFormatFilePath")
            FormatFilePath = LastReadValue

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
