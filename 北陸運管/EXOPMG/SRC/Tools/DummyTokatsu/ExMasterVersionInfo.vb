' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  EkMasterVersion.vb�����Ƃɍ쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �}�X�^�o�[�W�������������o���ۂɎg�p����N���X�B
''' </summary>
Public Class ExMasterVersionInfo

    Public Const Length As Integer = (1 + 2) * 50

    Public Shared ReadOnly Kinds() As String = { _
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

    'NOTE: �����o���Ȃ��ꍇ�ȂǂɁAIOException���X���[�����܂��B
    Public Shared Sub WriteToStream(ByVal oMasters As Dictionary(Of String, HoldingMaster), ByVal oOutputStream As Stream)
        Dim RawBytes(Length - 1) As Byte
        Dim pos As Integer = 0
        For i As Integer = 0 To Kinds.Length - 1
            Dim oMaster As HoldingMaster = Nothing
            If Kinds(i).Length <> 0 AndAlso oMasters.TryGetValue(Kinds(i), oMaster) = True Then
                Utility.CopyIntToBcdBytes(oMaster.DataSubKind, RawBytes, pos, 1)
                pos += 1
                Utility.CopyIntToBcdBytes(oMaster.DataVersion, RawBytes, pos, 2)
                pos += 2
            Else
                Utility.CopyIntToBcdBytes(0, RawBytes, pos, 1)
                pos += 1
                Utility.CopyIntToBcdBytes(0, RawBytes, pos, 2)
                pos += 2
            End If
        Next i
        oOutputStream.Write(RawBytes, 0, RawBytes.Length)
    End Sub

End Class
