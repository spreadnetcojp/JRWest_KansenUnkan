' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' ���D�@�̃v���O�����o�[�W�������������o���ۂɎg�p����N���X�B
''' </summary>
Public Class ExProgramVersionInfoForG

    'NOTE: �����o���Ȃ��ꍇ�ȂǂɁAIOException���X���[�����܂��B
    Public Shared Sub WriteToStream(ByVal oProgram As HoldingProgram, ByVal oOutputStream As Stream)
        If oProgram IsNot Nothing Then
            Debug.Assert(oProgram.ModuleInfos.Length = 5)

            Dim len As Integer = (8 + 2) * 5
            For Each oModuleInfo As ProgramModuleInfo In oProgram.ModuleInfos
                len += (12 + 96) * oModuleInfo.Elements.Length
            Next oModuleInfo

            Dim RawBytes(len - 1) As Byte
            Dim pos As Integer = 0

            For i As Integer = 0 To oProgram.ModuleInfos.Length - 1
                Array.Clear(RawBytes, pos, 8)
                Dim sFolderName As String = ExConstants.GateProgramModuleNamesInVersionInfo(i)
                Debug.Assert(sFolderName.Length <= 8)
                Encoding.UTF8.GetBytes(sFolderName, 0, sFolderName.Length, RawBytes, pos)
                pos += 8

                Utility.CopyUInt16ToLeBytes2(CType(oProgram.ModuleInfos(i).Elements.Length, UShort), RawBytes, pos)
                pos += 2
            Next i

            For Each oModuleInfo As ProgramModuleInfo In oProgram.ModuleInfos
                For Each oElement As ProgramElementInfo In oModuleInfo.Elements
                    Array.Clear(RawBytes, pos, 12)
                    Debug.Assert(oElement.FileName.Length <= 12)
                    Encoding.UTF8.GetBytes(oElement.FileName, 0, oElement.FileName.Length, RawBytes, pos)
                    pos += 12

                    Debug.Assert(oElement.DispData.Length = 96)
                    Buffer.BlockCopy(oElement.DispData, 0, RawBytes, pos, 96)
                    pos += 96
                Next oElement
            Next oModuleInfo

            oOutputStream.Write(RawBytes, 0, RawBytes.Length)
        Else
            Dim len As Integer = (8 + 2) * 5
            Dim RawBytes(len - 1) As Byte
            oOutputStream.Write(RawBytes, 0, RawBytes.Length)
        End If
    End Sub

End Class
