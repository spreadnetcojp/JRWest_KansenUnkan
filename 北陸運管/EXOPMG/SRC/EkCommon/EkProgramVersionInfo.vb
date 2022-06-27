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

''' <summary>
''' �Ď��Ճv���O�����o�[�W�������Ɖ��D�@�v���O�����o�[�W�������̃O���[�v���o���B
''' </summary>
Public Structure EkProgramVersionInfoElementGroupHeader
    Public ElementCount As Integer
End Structure

''' <summary>
''' �Ď��Ճv���O�����o�[�W�������Ɖ��D�@�v���O�����o�[�W�������̃��R�[�h�B
''' </summary>
Public Structure EkProgramVersionInfoElement
    Public FileName As String
    Public Version As String
    Public DispName As String
End Structure

''' <summary>
''' �Ď��Ճv���O�����o�[�W�������Ɖ��D�@�v���O�����o�[�W��������ǂݏo�����ۃN���X�B
''' </summary>
Public MustInherit Class EkProgramVersionInfoReader

#Region "�萔"
    Protected Const GroupHeaderLen As Integer = 10
    Protected Const ElementCountPos As Integer = 8
    Protected Const ElementLen As Integer = 108
    Protected Const FileNamePos As Integer = 0
    Protected Const FileNameLen As Integer = 12
    Protected VersionPos As Integer
    Protected VersionLen As Integer
    Protected DispNamePos As Integer
    Protected DispNameLen As Integer
#End Region

#Region "���\�b�h"
    'NOTE: �t�@�C���̒������Z���ꍇ�Ȃǂɂ́AIOException���X���[���܂��B
    'NOTE: ���߂��s�\�ȏꍇ�́AFormatException���X���[���܂��B
    Public Overridable Function GetOneGroupHeaderFromStream(ByVal oInputStream As Stream) As EkProgramVersionInfoElementGroupHeader
        Dim pos As Integer = 0
        Dim RawBytes(GroupHeaderLen - 1) As Byte
        Do
            Dim readLimit As Integer = GroupHeaderLen - pos
            If readLimit = 0 Then Exit Do
            Dim readSize As Integer = oInputStream.Read(RawBytes, pos, readLimit)
            If readSize = 0 Then
                Throw New EndOfStreamException()
            End If
            pos += readSize
        Loop

        Dim ret As New EkProgramVersionInfoElementGroupHeader()
        ret.ElementCount = Utility.GetUInt16FromLeBytes2(RawBytes, ElementCountPos)
        Return ret
    End Function

    'NOTE: �t�@�C���̒������Z���ꍇ�Ȃǂɂ́AIOException���X���[���܂��B
    'NOTE: ���߂��s�\�ȏꍇ�́AFormatException���X���[���܂��B
    Public Overridable Function GetOneGroupElementsFromStream(ByVal oInputStream As Stream, ByVal groupHeader As EkProgramVersionInfoElementGroupHeader) As EkProgramVersionInfoElement()
        Dim numElems As Integer = groupHeader.ElementCount
        Dim Length As Integer = numElems * ElementLen

        Dim pos As Integer = 0
        Dim RawBytes(Length - 1) As Byte
        Do
            Dim readLimit As Integer = Length - pos
            If readLimit = 0 Then Exit Do
            Dim readSize As Integer = oInputStream.Read(RawBytes, pos, readLimit)
            If readSize = 0 Then
                Throw New EndOfStreamException()
            End If
            pos += readSize
        Loop

        pos = 0
        Dim aInfoElements As EkProgramVersionInfoElement() = New EkProgramVersionInfoElement(numElems - 1) {}
        For i As Integer = 0 To numElems - 1
            '�t�@�C�����𔲂��o���B
            'NOTE: �v���O�����o�[�W������񂻂̂��̂ɂ�����d�l�́uJIS�v�ƂȂ��Ă��邪�A
            '��r�ΏۂƂȂ�FILELIST.TXT�̎d�l�́uASCII�v�ł��邽�߁AASCII�Œ�`����Ȃ�
            '�����������Ă��邱�Ƃ͂��蓾�Ȃ��i�w���@�푤�̏����Ɉُ킪����j���̂Ƃ݂Ȃ��B
            If Not Utility.IsVisibleAsciiBytes(RawBytes, pos + FileNamePos, FileNameLen) Then
                Throw New FormatException("FileName of Element #" & i.ToString() & " is invalid (not visible ASCII bytes).")
            End If
            'TODO: �]���@����M�������D�@�v���O�����o�[�W�������̎��f�[�^����A
            '���̗̈�̗]��̓k�������Ŗ��߂���Ƒz�肵�Ď������Ă���B�����A
            '0x20�Ŗ��߂���Ȃ�u.TrimEnd(Chr(&H20))�v�ɕύX���邱�ƁB
            aInfoElements(i).FileName = Encoding.UTF8.GetString(RawBytes, pos + FileNamePos, FileNameLen).TrimEnd(Chr(0))

            '�o�[�W�����𔲂��o���B
            If Not Utility.IsVisibleAsciiBytes(RawBytes, pos + VersionPos, VersionLen) Then
                Throw New FormatException("Version of Element #" & i.ToString() & " is invalid (not visible ASCII bytes).")
            End If
            aInfoElements(i).Version = Encoding.UTF8.GetString(RawBytes, pos + VersionPos, VersionLen).TrimEnd(Chr(0))

            '�\�����𔲂��o���B
            Try
                aInfoElements(i).DispName = Encoding.GetEncoding(932).GetString(RawBytes, pos + DispNamePos, DispNameLen).TrimEnd(Chr(&H20))
            Catch ex As DecoderFallbackException
                Throw New FormatException("DispName of Element #" & i.ToString() & " is invalid.")
            End Try

            pos += ElementLen
        Next
        Return aInfoElements
    End Function
#End Region

End Class
