' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2013/12/10  (NES)����  �o�[�W�������(TOICA,ICOCA)�ǉ��Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

''' <summary>
''' �����v���O�����o�[�W�������̃��R�[�h�B
''' </summary>
Public Structure EkMadoProgramVersionInfoElement
    Public Name As String
    Public Value As String
    Public IsVersion As Boolean
End Structure

''' <summary>
''' �����̃v���O�����o�[�W��������ǂݏo���N���X�B
''' </summary>
Public Class EkMadoProgramVersionInfoReader

#Region "�����N���X��"
    Structure SourceFormat
        Public Length As Integer
        Public Encoding As String
        Public IsVersion As Boolean
        Public Name As String

        Public Sub New( _
           ByVal length As Integer, _
           ByVal sEncoding As String, _
           ByVal isVersion As Boolean, _
           ByVal sName As String)

            Me.Length = length
            Me.Encoding = sEncoding
            Me.IsVersion = isVersion
            Me.Name = sName
        End Sub
    End Structure
#End Region

#Region "�萔"
    'NOTE: ������O���t�@�C������ǂݍ��ނ悤�ɂ���΁A�����v���O�����\����
    '�ύX�ɑ΂��A�^�ǂ̎����ύX�Ȃ��őΉ��ł���悤�ɂȂ�B
    '�������A�^�ǂ̎������ꎩ�̂��X�N���v�g����ɋ߂��i��r�I�ȈՂɋL�q�ł��A
    '�R���p�C������y�ł���j��A�Ǝ������̃t�@�C�����L�q���������肪
    '���o����₷���̂ŁA���Ƃ������Ȃ��B
    'NOTE: 6�o�C�gBCD�̍쐬�N�����́A�{���Ƀp�b�N�hBCD�Ȃ̂��^��ł��邪�A
    '�z��ƈ���Ă��ϊ��ŗ�O���������邱�ƂȂǂ͂Ȃ��͂��ł���A
    '���̂Ƃ���\���ɗp���邱�Ƃ��Ȃ����߁A�p�b�N�hBCD�Ƃ݂Ȃ���
    '�ϊ�����悤�ɂ��Ă���B
    'Ver0.1 MOD START TOICA,ICOCA�̉^���f�[�^�Ή�
    Private Shared ReadOnly aSourceFormats() As SourceFormat = { _
        New SourceFormat(1,  "",      False, "�Ώۃ��[�U�R�[�h"), _
        New SourceFormat(1,  "",      False, "�K�p�G���A�R�[�h"), _
        New SourceFormat(1,  "",      False, "�v���O�����敪"), _
        New SourceFormat(4,  "BCD",   False, "�v���O�������싖��"), _
        New SourceFormat(4,  "BCD",   True,  "DLL�S�̃o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "DLL�K�p�o�[�W����"), _
        New SourceFormat(15, "",      False, "�\��"), _
        New SourceFormat(2,  "BCD",   True,  "�ݗ�IC����o�[�W����(Suica)"), _
        New SourceFormat(2,  "BCD",   True,  "�ݗ�IC����o�[�W����(TOICA)"), _
        New SourceFormat(2,  "BCD",   True,  "�ݗ�IC����o�[�W����(ICOCA)"), _
        New SourceFormat(2,  "BCD",   True,  "�V����IC����o�[�W����"), _
        New SourceFormat(2,  "BCD",   True,  "EXIC����o�[�W����"), _
        New SourceFormat(2,  "ASCII", True,  "Suica�^���f�[�^����1�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "Suica�^���f�[�^����1�K�p�N����"), _
        New SourceFormat(2,  "ASCII", True,  "Suica�^���f�[�^����2�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "Suica�^���f�[�^����2�K�p�N����"), _
        New SourceFormat(15, "ASCII", False, "Suica�^���f�[�^��"), _
        New SourceFormat(3,  "ASCII", False, "Suica�^���f�[�^�S�̃\�t�g�^��"), _
        New SourceFormat(2,  "ASCII", True,  "Suica�^���f�[�^�o�[�W����"), _
        New SourceFormat(6,  "BCD",   False, "Suica�^���f�[�^�쐬�N����"), _
        New SourceFormat(2,  "ASCII", True,  "TOICA�^���f�[�^����1�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "TOICA�^���f�[�^����1�K�p�N����"), _
        New SourceFormat(2,  "ASCII", True,  "TOICA�^���f�[�^����2�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "TOICA�^���f�[�^����2�K�p�N����"), _
        New SourceFormat(15, "ASCII", False, "TOICA�^���f�[�^��"), _
        New SourceFormat(3,  "ASCII", False, "TOICA�^���f�[�^�S�̃\�t�g�^��"), _
        New SourceFormat(2,  "ASCII", True,  "TOICA�^���f�[�^�o�[�W����"), _
        New SourceFormat(6,  "BCD",   False, "TOICA�^���f�[�^�쐬�N����"), _
        New SourceFormat(2,  "ASCII", True,  "ICOCA�^���f�[�^����1�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "ICOCA�^���f�[�^����1�K�p�N����"), _
        New SourceFormat(2,  "ASCII", True,  "ICOCA�^���f�[�^����2�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "ICOCA�^���f�[�^����2�K�p�N����"), _
        New SourceFormat(15, "ASCII", False, "ICOCA�^���f�[�^��"), _
        New SourceFormat(3,  "ASCII", False, "ICOCA�^���f�[�^�S�̃\�t�g�^��"), _
        New SourceFormat(2,  "ASCII", True,  "ICOCA�^���f�[�^�o�[�W����"), _
        New SourceFormat(6,  "BCD",   False, "ICOCA�^���f�[�^�쐬�N����"), _
        New SourceFormat(2,  "ASCII", True,  "���}�����f�[�^����1�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "���}�����f�[�^����1�K�p�N����"), _
        New SourceFormat(2,  "ASCII", True,  "���}�����f�[�^����2�o�[�W����"), _
        New SourceFormat(4,  "BCD",   False, "���}�����f�[�^����2�K�p�N����"), _
        New SourceFormat(15, "ASCII", False, "���}�����f�[�^��"), _
        New SourceFormat(3,  "ASCII", False, "���}�����f�[�^�S�̃\�t�g�^��"), _
        New SourceFormat(2,  "ASCII", True,  "���}�����f�[�^�o�[�W����"), _
        New SourceFormat(6,  "BCD",   False, "���}�����f�[�^�쐬�N����"), _
        New SourceFormat(2,  "BCD",   True,  "���C�t�@�[���E�F�A�o�[�W����"), _
        New SourceFormat(14, "",      False, "�\��"), _
        New SourceFormat(1,  "",      False, "�����؎��v���O������K�p�`�F�b�N�t���O"), _
        New SourceFormat(1,  "",      False, "�����L���v���O������K�p�`�F�b�N�t���O"), _
        New SourceFormat(46, "",      False, "���l")}

    Private Const _Length As Integer = 256
    'Ver0.1 MOD END TOICA,ICOCA�̉^���f�[�^�Ή�
#End Region

#Region "���\�b�h"
    'NOTE: �t�@�C���̒������Z���ꍇ�Ȃǂɂ́AIOException���X���[���܂��B
    'NOTE: ���߂��s�\�ȏꍇ�́AFormatException���X���[���܂��B
    Public Shared Function GetElementsFromStream(ByVal oInputStream As Stream) As EkMadoProgramVersionInfoElement()
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
        Dim aInfoElements As EkMadoProgramVersionInfoElement() = New EkMadoProgramVersionInfoElement(aSourceFormats.Length - 1) {}
        For i As Integer = 0 To aSourceFormats.Length - 1
            aInfoElements(i).Name = aSourceFormats(i).Name
            aInfoElements(i).IsVersion = aSourceFormats(i).IsVersion

            Dim len As Integer = aSourceFormats(i).Length
            Select Case aSourceFormats(i).Encoding
                Case "BCD"
                    'NOTE: �ȉ��́A�������ꂽ���ނ̃o�[�W�������i�v���O�����o�[�W�������X�g�j
                    '�ɂ����āABCD���ڂ�0�`9�ȊO���Z�b�g����Ă��邽�߁A��������e���邽�߂�
                    '�R�����g�A�E�g���Ă���B
                    'If Not Utility.IsBcdBytes(RawBytes, pos, len) Then
                    '    Throw New FormatException("Element #" & i.ToString() & " is invalid (not BCD bytes).")
                    'End If

                    aInfoElements(i).Value = BitConverter.ToString(RawBytes, pos, len).Replace("-", "")

                    '0x00�Ŗ��߂��Ă���ꍇ�͓��ꈵ���i�o�[�W���������j�Ƃ���B
                    If aSourceFormats(i).IsVersion Then
                        Dim bitSum As Byte = 0
                        For p As Integer = pos To pos + len - 1
                            bitSum = BitSum Or RawBytes(p)
                        Next
                        If bitSum = 0 Then
                            aInfoElements(i).Value = ""
                        End If
                    End If

                Case "ASCII"
                    If Not Utility.IsVisibleAsciiBytes(RawBytes, pos, len) Then
                        Throw New FormatException("Element #" & i.ToString() & " is invalid (not visible ASCII bytes).")
                    End If

                    aInfoElements(i).Value = Encoding.UTF8.GetString(RawBytes, pos, len).TrimEnd(Chr(0))

                Case Else
                    Debug.Assert(aSourceFormats(i).IsVersion = False)
            End Select

            pos += len
        Next
        Return aInfoElements
    End Function
#End Region

End Class
