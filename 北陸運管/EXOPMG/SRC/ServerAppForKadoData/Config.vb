' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/04/01       ����  �k���Ή��@INI��`�t�@�C�����X�g���擾
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits RecServerAppBaseConfig

    '�o�^�Ώۃf�[�^�����t�@�C���̃p�X
    '-------------------------------------------------------------
    Public Const KadoFormatFilePath_G As String = "KadoDataFormatFilePath_G"
    Public Shared KadoFormatFilePath_Y As String
    Public Const HosyuFormatFilePath As String = "HosyuDataFormatFilePath"
    Protected Const KADOINPUTPATH_SECTION As String = "Path"
    Public Shared KadoFormatFileG As New ArrayList
    Public Shared HosyuFormatFile As New ArrayList
    '-------------------------------------------------------------
    '���̓f�[�^�ʁi�v���Z�X�ʁj�L�[�ɑ΂���v���t�B�b�N�X
    Private Const DATA_NAME As String = "KadoData"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̉ғ��E�ێ�f�[�^�o�^�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)
        Dim i As Integer
        Try
            '------------------Ver0.1�@�k���Ή��@MOD START-------------------------------
            i = 0
            Do
                ReadFileElem(KADOINPUTPATH_SECTION, KadoFormatFilePath_G + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & KADOINPUTPATH_SECTION & ", Key: " & KadoFormatFilePath_G & ")")
                    Else
                        Exit Do
                    End If
                End If
                KadoFormatFileG.Add(LastReadValue)
                i = i + 1
            Loop
            '------------------Ver0.1�@�k���Ή��@MOD  END-------------------------------
            ReadFileElem(PATH_SECTION, "KadoDataFormatFilePath_Y")
            KadoFormatFilePath_Y = LastReadValue
            '------------------Ver0.1�@�k���Ή��@MOD START-------------------------------
            i = 0
            Do
                ReadFileElem(KADOINPUTPATH_SECTION, HosyuFormatFilePath + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & KADOINPUTPATH_SECTION & ", Key: " & HosyuFormatFilePath & ")")
                    Else
                        Exit Do
                    End If
                End If
                HosyuFormatFile.Add(LastReadValue)
                i = i + 1
            Loop
            '------------------Ver0.1�@�k���Ή��@MOD  END-------------------------------
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
