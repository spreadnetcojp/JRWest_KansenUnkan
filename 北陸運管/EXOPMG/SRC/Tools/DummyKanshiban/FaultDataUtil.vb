' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/11/21  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class FaultDataUtil

    Private Class FieldRef
        Public Field As XlsField
        Public BitOffset As Integer
        Public Index As Integer

        Public Sub New(ByVal oField As XlsField, ByVal bitOfs As Integer, ByVal i As Integer)
            Field = oField
            BitOffset = bitOfs
            Index = i
        End Sub
    End Class

    Private Shared oFieldRefs As Dictionary(Of String, FieldRef)
    Private Shared totalBitCount As Integer

    Private Shared ReadOnly oFields As XlsField() = New XlsField() { _
        New XlsField(8*1, "X2", 1, " "c, "��{�w�b�_�[ �f�[�^���", "DataKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "��{�w�b�_�[ �w�R�[�h", "Station"), _
        New XlsField(8*7, "X14", 1, " "c, "��{�w�b�_�[ ��������"), _
        New XlsField(8*1, "D", 1, " "c, "��{�w�b�_�[ �R�[�i�["), _
        New XlsField(8*1, "D", 1, " "c, "��{�w�b�_�[ ���@"), _
        New XlsField(8*4, "D", 1, " "c, "��{�w�b�_�[ �V�[�P���XNo", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*1, "X2", 1, " "c, "��{�w�b�_�[ �o�[�W����"), _
        New XlsField(8*4, "D", 1, " "c, "�f�[�^�����O�X", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*8, "X16", 1, " "c, "��������"), _
        New XlsField(8*1, "X2", 1, " "c, "���@�ԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "�ʘH����", "PassDirection"), _
        New XlsField(8*4, "X8", 1, " "c, "�G���[�R�[�h", "FaultDataErrorCode"), _
        New XlsField(8*1, "X2", 1, " "c, "�ُ헚���w��"), _
        New XlsField(8*1, "X2", 1, " "c, "�ُ���"), _
        New XlsField(8*1, "X2", 1, " "c, "���Z�b�g�����v���"), _
        New XlsField(8*1, "X2", 1, " "c, "���q�ē��t���"), _
        New XlsField(8*4, "D", 1, " "c, "�ُ퍀�� �L���o�C�g��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*40, "S932", 1, " "c, "�ُ퍀�� �\���f�[�^"), _
        New XlsField(8*4, "D", 1, " "c, "�S�����\�� �L���o�C�g��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*12, "S932", 1, " "c, "�S�����\�� �\���f�[�^"), _
        New XlsField(8*4, "D", 1, " "c, "�ϕ\���� �L���o�C�g��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*540, "S932", 1, " "c, "�ϕ\���� �\���f�[�^"), _
        New XlsField(8*4, "D", 1, " "c, "���u���e �L���o�C�g��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*150, "S932", 1, " "c, "���u���e �\���f�[�^")}

    Shared Sub New()
        oFieldRefs = New Dictionary(Of String, FieldRef)
        Dim bits As Integer = 0
        For i As Integer = 0 To oFields.Length - 1
            Dim oField As XlsField = oFields(i)
            oFieldRefs.Add(oField.MetaName, New FieldRef(oField, bits, i))
            bits += oField.ElementBits * oField.ElementCount
        Next i
        totalBitCount = bits
    End Sub

    Public Shared ReadOnly Property RecordLengthInBits As Integer
        Get
            Return totalBitCount
        End Get
    End Property

    Public Shared ReadOnly Property RecordLengthInBytes As Integer
        Get
            Return (totalBitCount + 7) \ 8
        End Get
    End Property

    Public Shared ReadOnly Property Fields As XlsField()
        Get
            Return oFields
        End Get
    End Property

    Public Shared ReadOnly Property Field(ByVal sMetaName As String) As XlsField
        Get
            Return oFieldRefs(sMetaName).Field
        End Get
    End Property

    Public Shared Function FieldIndexOf(ByVal sMetaName As String) As Integer
        Return oFieldRefs(sMetaName).Index
    End Function

    Public Shared Function GetFieldValueFromBytes(ByVal sMetaName As String, ByVal oBytes As Byte()) As String
        Dim oRef As FieldRef = oFieldRefs(sMetaName)
        Return oRef.Field.CreateValueFromBytes(oBytes, oRef.BitOffset)
    End Function

    Public Shared Sub SetFieldValueToBytes(ByVal sMetaName As String, ByVal sValue As String, ByVal oBytes As Byte())
        Dim oRef As FieldRef = oFieldRefs(sMetaName)
        oRef.Field.CopyValueToBytes(sValue, oBytes, oRef.BitOffset)
    End Sub

    Public Shared Sub AdjustByteCountField(ByVal sSuperName As String, ByVal oBytes As Byte())
        Dim sDataFieldName As String = sSuperName & " �\���f�[�^"
        Dim sLenFieldName As String = sSuperName & " �L���o�C�g��"
        Dim sDataValue As String = GetFieldValueFromBytes(sDataFieldName, oBytes)
        Dim sLenValue As String = MyUtility.GetValidByteCount(Field(sDataFieldName), sDataValue).ToString()
        SetFieldValueToBytes(sLenFieldName, sLenValue, oBytes)
    End Sub

    Public Shared Function CreatePassDirectionValue(ByVal latchConfig As Byte) As String
        If latchConfig = &H0 Then
            Return "00"
        Else
            'OPT: ���������A���D�@��latchConfig��&H3�ȏ�Ƃ������Ƃ��A���^�p�ł͂��蓾�Ȃ��Ǝv����B
            Return If(latchConfig < &H3, "01", "02")
        End If
    End Function

End Class
