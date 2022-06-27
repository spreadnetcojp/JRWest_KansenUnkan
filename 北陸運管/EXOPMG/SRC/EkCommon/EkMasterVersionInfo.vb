' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2014/06/01       ����  �V�K�}�X�^�ǉ��Ή�
'   0.2      2017/05/22  (NES)�͘e  �|�C���g�|�X�g�y�C�Ή�
'                                     �}�X�^�ǉ��i������Ԏ��ԁA�|�X�g�y�C�G���A�}�X�^�j
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

''' <summary>
''' �}�X�^�o�[�W�������̃��R�[�h�B
''' </summary>
Public Structure EkMasterVersionInfoElement
    Public Kind As String
    Public SubKind As String
    Public Version As String
End Structure

''' <summary>
''' �}�X�^�o�[�W��������ǂݏo���N���X�B
''' </summary>
Public Class EkMasterVersionInfoReader

#Region "�萔"
    '----------- 0.1  �V�K�}�X�^�ǉ��Ή�   ADD  START------------------------
    '----------- 0.2  �|�C���g�|�X�g�y�C�Ή�   MOD  START------------------------
    Private Shared ReadOnly aKinds() As String = { _
        "KEN",
        "DLY",
        "PAY",
        "",
        "ICD",
        "",
        "LOS",
        "DSC",
        "HLD",
        "EXP",
        "FRX",
        "ICH",
        "FJW",
        "IJW",
        "FJC",
        "IJC",
        "FJR",
        "DSH",
        "LST",
        "IJE",
        "CYC",
        "STP",
        "PNO",
        "FRC",
        "",
        "DUS",
        "NSI",
        "NTO",
        "NIC",
        "NJW",
        "",
        "FSK",
        "IUZ",
        "KSZ",
        "IUK",
        "SWK",
        "HIR",
        "PPA",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        ""}
    '----------- 0.2  �|�C���g�|�X�g�y�C�Ή�   MOD    END------------------------
    '----------- 0.1  �V�K�}�X�^�ǉ��Ή�   ADD    END------------------------
    Private Const _Length As Integer = (1 + 2) * 50
#End Region

#Region "���\�b�h"
    'NOTE: �t�@�C���̒������Z���ꍇ�Ȃǂɂ́AIOException���X���[���܂��B
    'NOTE: ���߂��s�\�ȏꍇ�́AFormatException���X���[���܂��B
    Public Shared Function GetElementsFromStream(ByVal oInputStream As Stream) As EkMasterVersionInfoElement()
        Dim RawBytes(_Length - 1) As Byte
        Dim pos As Integer = 0

        Do
            Dim readLimit As Integer = _Length - pos
            If readLimit = 0 Then Exit Do
            Dim readSize As Integer = oInputStream.Read(RawBytes, pos, readLimit)
            If readSize = 0 Then
                Throw New EndOfStreamException()
            End If
            pos += readSize
        Loop

        pos = 0
        Dim aInfoElements As EkMasterVersionInfoElement() = New EkMasterVersionInfoElement(aKinds.Length - 1) {}
        For i As Integer = 0 To aKinds.Length - 1
            aInfoElements(i).Kind = aKinds(i)

            If Not Utility.IsBcdBytes(RawBytes, pos, 1) Then
                Throw New FormatException("PatternNumber of Element #" & i.ToString() & " is invalid (not BCD bytes).")
            End If

            Dim intSubKind As Integer = Utility.GetIntFromBcdBytes(RawBytes, pos, 1)
            aInfoElements(i).SubKind = intSubKind.ToString("D2")
            pos += 1

            If Not Utility.IsBcdBytes(RawBytes, pos, 2) Then
                Throw New FormatException("VersionNumber of Element #" & i.ToString() & " is invalid (not BCD bytes).")
            End If

            Dim intVersion As Integer = Utility.GetIntFromBcdBytes(RawBytes, pos, 2)
            aInfoElements(i).Version = intVersion.ToString("D3")
            pos += 2
        Next
        Return aInfoElements
    End Function
#End Region

End Class
