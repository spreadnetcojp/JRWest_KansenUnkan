' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/08/08  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class ProgramVersionListUtil

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
        New XlsField(8*1, "X2", 1, " "c, "���ʕ� ���[�U�R�[�h", "CompanyCode"), _
        New XlsField(8*1, "X2", 1, " "c, "���ʕ� �K�p�G���A", "IcArea"), _
        New XlsField(8*1, "X2", 1, " "c, "���ʕ� �v���O�����敪", "ProgramDistribution"), _
        New XlsField(8*4, "X8", 1, " "c, "���ʕ� �v���O�������싖��"), _
        New XlsField(8*1, "X2", 4, " "c, "���ʕ� �v���O�����S��Ver�i�V�j"), _
        New XlsField(8*1, "X2", 4, " "c, "���ʕ� �v���O�����S��Ver�i���j"), _
        New XlsField(8*1, "X2", 15, " "c, "���ʕ� �\��"), _
        New XlsField(8*2, "X4", 1, " "c, "�ݗ�IC����o�[�W����(Suica)"), _
        New XlsField(8*2, "X4", 1, " "c, "�ݗ�IC����o�[�W����(TOICA)"), _
        New XlsField(8*2, "X4", 1, " "c, "�ݗ�IC����o�[�W����(ICOCA)"), _
        New XlsField(8*2, "X4", 1, " "c, "�V����IC����o�[�W����"), _
        New XlsField(8*2, "X4", 1, " "c, "EXIC����o�[�W����"), _
        New XlsField(8*2, "A", 1, " "c, "Suica�^���f�[�^����1�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "Suica�^���f�[�^����1�K�p�N����"), _
        New XlsField(8*2, "A", 1, " "c, "Suica�^���f�[�^����2�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "Suica�^���f�[�^����2�K�p�N����"), _
        New XlsField(8*15, "A", 1, " "c, "Suica�^���f�[�^��"), _
        New XlsField(8*3, "A", 1, " "c, "Suica�^���f�[�^�S�̃\�t�g�^��"), _
        New XlsField(8*2, "A", 1, " "c, "Suica�^���f�[�^�o�[�W����"), _
        New XlsField(8*6, "X12", 1, " "c, "Suica�^���f�[�^�쐬�N����"), _
        New XlsField(8*2, "A", 1, " "c, "TOICA�^���f�[�^����1�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "TOICA�^���f�[�^����1�K�p�N����"), _
        New XlsField(8*2, "A", 1, " "c, "TOICA�^���f�[�^����2�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "TOICA�^���f�[�^����2�K�p�N����"), _
        New XlsField(8*15, "A", 1, " "c, "TOICA�^���f�[�^��"), _
        New XlsField(8*3, "A", 1, " "c, "TOICA�^���f�[�^�S�̃\�t�g�^��"), _
        New XlsField(8*2, "A", 1, " "c, "TOICA�^���f�[�^�o�[�W����"), _
        New XlsField(8*6, "X12", 1, " "c, "TOICA�^���f�[�^�쐬�N����"), _
        New XlsField(8*2, "A", 1, " "c, "ICOCA�^���f�[�^����1�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "ICOCA�^���f�[�^����1�K�p�N����"), _
        New XlsField(8*2, "A", 1, " "c, "ICOCA�^���f�[�^����2�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "ICOCA�^���f�[�^����2�K�p�N����"), _
        New XlsField(8*15, "A", 1, " "c, "ICOCA�^���f�[�^��"), _
        New XlsField(8*3, "A", 1, " "c, "ICOCA�^���f�[�^�S�̃\�t�g�^��"), _
        New XlsField(8*2, "A", 1, " "c, "ICOCA�^���f�[�^�o�[�W����"), _
        New XlsField(8*6, "X12", 1, " "c, "ICOCA�^���f�[�^�쐬�N����"), _
        New XlsField(8*2, "A", 1, " "c, "���}�����f�[�^����1�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "���}�����f�[�^����1�K�p�N����"), _
        New XlsField(8*2, "A", 1, " "c, "���}�����f�[�^����2�o�[�W����"), _
        New XlsField(8*4, "X8", 1, " "c, "���}�����f�[�^����2�K�p�N����"), _
        New XlsField(8*15, "A", 1, " "c, "���}�����f�[�^��"), _
        New XlsField(8*3, "A", 1, " "c, "���}�����f�[�^�S�̃\�t�g�^��"), _
        New XlsField(8*2, "A", 1, " "c, "���}�����f�[�^�o�[�W����"), _
        New XlsField(8*6, "X12", 1, " "c, "���}�����f�[�^�쐬�N����"), _
        New XlsField(8*2, "X4", 1, " "c, "���C�t�@�[���E�F�A�o�[�W����"), _
        New XlsField(8*1, "X2", 14, " "c, "�\��"), _
        New XlsField(8*1, "X2", 1, " "c, "�����؎��v���O������K�p�`�F�b�N�t���O"), _
        New XlsField(8*1, "X2", 1, " "c, "�����L���v���O������K�p�`�F�b�N�t���O"), _
        New XlsField(8*1, "X2", 46, " "c, "���l")}

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

End Class
