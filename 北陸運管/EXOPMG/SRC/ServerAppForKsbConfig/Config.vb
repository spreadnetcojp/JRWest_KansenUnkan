' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/06/01       ����  �k���E���ڊg���Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits RecServerAppBaseConfig

    '�o�^�Ώۃf�[�^�����t�@�C���̃p�X
    Public Shared FormatFilePath As String
    '----------- 0.1  �k���E���ڊg���Ή�   ADD  START------------------------
    Public Shared FormatOldFilePath As String
    '----------- 0.1  �k���E���ڊg���Ή�   ADD    END------------------------

    '�f�[�^�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const DATA_NAME As String = "KsbConfig"

    'INI�t�@�C�����ɂ�����e�ݒ荀�ڂ̃L�[
    'Private Const FOO_BAR_KEY As String = DATA_NAME & "FooBar"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̊Ď��Րݒ�f�[�^�o�^�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)

        Try
            ReadFileElem(PATH_SECTION, "KsbConfigFormatFilePath")
            FormatFilePath = LastReadValue
            '----------- 0.1  �k���E���ڊg���Ή�   ADD  START------------------------
            ReadFileElem(PATH_SECTION, "KsbConfigOldFormatFilePath")
            FormatOldFilePath = LastReadValue
            '----------- 0.1  �k���E���ڊg���Ή�   ADD    END------------------------
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class

