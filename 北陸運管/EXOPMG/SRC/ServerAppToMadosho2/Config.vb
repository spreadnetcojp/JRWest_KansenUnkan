' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports JR.ExOpmg.Common

Public Class Config
    Inherits TelServerAppBaseConfig

    '-------Ver0.1 ������ԕ�Ή� MOD START-----------
    '�������p�f�[�^�Z�N�V�����̓��e
    Public Shared RiyoDataUllSpecOfObjCodes As New Dictionary(Of Byte, TelServerAppRiyoDataUllSpec)

    '�v���Z�X�ʃL�[�ɑ΂���v���t�B�b�N�X
    Private Const MODEL_NAME As String = "Madosho2"

    ''' <summary>INI�t�@�C������^�ǃT�[�o�̑Α������p�f�[�^�ʐM�v���Z�X�ɕK�{�̑S�ݒ�l����荞�ށB</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            RiyoDataUllSpecOfObjCodes = New Dictionary(Of Byte, TelServerAppRiyoDataUllSpec)
            Dim oTempDic As Dictionary(Of String, String) = GetFileSectionAsDictionary("MadoRiyoData")
            For Each oEntry As KeyValuePair(Of String, String) In oTempDic
                LastReadKey = oEntry.Key
                LastReadValue = oEntry.Value
                Dim code As Byte = Byte.Parse(LastReadKey, NumberStyles.HexNumber)
                Dim oElems As String() = LastReadValue.Split(","c)
                Dim oSpec As New TelServerAppRiyoDataUllSpec(oElems(0), oElems(1), Integer.Parse(oElems(2)), Integer.Parse(oElems(3)))
                RiyoDataUllSpecOfObjCodes.Add(code, oSpec)
            Next oEntry
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub
    '-------Ver0.1 ������ԕ�Ή� MOD END-------------

    Public Shared Sub Dispose()
        TelServerAppBaseDispose()
    End Sub

End Class
