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

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �����ؖ��t�@�C�����X�V����N���X�B
''' </summary>
Public Class ServerAppPulser

#Region "���\�b�h"
    Public Shared Sub Pulse()
        Try
            Dim aBytes(14 - 1) As Byte
            Dim sTime As String = DateTime.Now.ToString("yyyyMMddHHmmss")
            Encoding.UTF8.GetBytes(sTime, 0, 14, aBytes, 0)

            Dim sFilePath As String = Path.Combine(ServerAppBaseConfig.ResidentAppPulseDirPath, ServerAppBaseConfig.AppIdentifier)
            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oOutputStream.Write(aBytes, 0, 14)
            End Using
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub
#End Region

End Class
