' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2014/04/20  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text
Imports JR.ExOpmg.Common

Public Class Common

    ''' <summary>
    ''' CSV�t�@�C����ǂݍ���
    ''' </summary>
    ''' <param name="filename">CSV�t�@�C����</param>
    ''' <returns>�ǂݍ��݌��ʗp�̔z��</returns>
    Public Shared Function ReadCsv(ByVal filename As String) As ArrayList

        Dim ret As New ArrayList

        ''�e�L�X�g�t�@�C�����ǂ���
        'Try
        '    Dim bytedata As Byte() = System.IO.File.ReadAllBytes(filename)

        '    For i As Integer = 0 To bytedata.Length - 1
        '        If bytedata(i) = 0 Then
        '            AlertBox.Show(Lexis.ERR_FILE_CSV)
        '            Return ret
        '        End If
        '    Next
        'Catch ex As Exception
        '    AlertBox.Show(Lexis.ERR_FILE_READ)
        '    Throw
        'End Try

        Try
            'Shift JIS�œǂݍ��݂܂��B
            Using swText As New FileIO.TextFieldParser(filename, System.Text.Encoding.GetEncoding(932))

                '�t�B�[���h�������ŋ�؂��Ă���ݒ���s���܂��B
                swText.TextFieldType = FileIO.FieldType.Delimited

                '��؂蕶�����u,�i�J���}�j�v�ɐݒ肵�܂��B
                swText.Delimiters = New String() {","}

                '�t�B�[���h��"�ň͂݁A���s�����A��؂蕶�����܂߂邱�Ƃ� '�ł��邩��ݒ肵�܂��B
                swText.HasFieldsEnclosedInQuotes = True

                '�t�B�[���h�̑O�ォ��X�y�[�X���폜����ݒ���s���܂��B
                swText.TrimWhiteSpace = False

                While Not swText.EndOfData
                    'CSV�t�@�C���̃t�B�[���h��ǂݍ��݂܂��B
                    Dim fields As String() = swText.ReadFields()

                    '�z��ɒǉ����܂��B�R�����g������
                    If Not fields(0).StartsWith("#") And Not fields(0).StartsWith("&") Then
                        ret.Add(fields)
                    End If
                End While

            End Using
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_FILE_READ)
            Throw
        End Try
        Return ret

    End Function

    ''' <summary>
    '''CSV�t�@�C���̎w��s�l���擾����
    ''' </summary>
    ''' <param name="CsvData">CSV���e�����z��</param>
    ''' <param name="row">�s</param>
    Public Shared Function ReadStringFromCSV(ByVal CsvData As ArrayList, ByVal row As Integer) As String()

        Return CType(CsvData.Item(row), String())

    End Function

    ''' <summary>
    '''CSV�t�@�C���̎w��s�A��̒l���擾����
    ''' </summary>
    ''' <param name="CsvData">CSV���e�����z��</param>
    ''' <param name="row">�s</param>
    ''' <param name="col">��</param>
    ''' <returns>�Y���ʒu�̒l</returns>
    Public Shared Function ReadStringFromCSV(ByVal CsvData As ArrayList, ByVal row As Integer, ByVal col As Integer) As String

        Try
            Dim a As String() = CType(CsvData.Item(row), String())

            Return a(col)
        Catch ex As Exception
            Return ""
        End Try

    End Function

    Public Shared Function ReadBin(ByVal filename As String) As Byte()
        Try
            Return System.IO.File.ReadAllBytes(filename)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_FILE_READ)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �����񂪂���͈͂��ǂ����𔻒f����
    ''' </summary>
    ''' <param name="str">���f�Ώ�</param>
    ''' <param name="min">�ŏ��l</param>
    ''' <param name="max">�ő�l</param>
    ''' <returns>�͈͈ȓ��̏ꍇ�Atrue,���̑� false</returns>
    Public Shared Function IsBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String) As Boolean
        Try
            Dim r As New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If r.IsMatch(str) = False Then
                Return False
            Else
                If CLng(str) > CLng(max) Or CLng(str) < CLng(min) Then
                    Return False
                End If
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' �L���I������byte�z����擾
    ''' </summary>
    Public Shared Function GetBCDDate(ByVal str As String, ByVal name As String) As Byte()

        Try
            '���t�`�F�b�N
            Return Utility.CHARtoBCD(DateTime.Parse(Format(CInt(str), "0000/00/00")).ToString("yyyyMMdd"), 4)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �K�p���t��byte�z����擾
    ''' </summary>
    Public Shared Function GetApplyDate(ByVal str As String, ByVal name As String) As Byte()

        Try
            '���t�`�F�b�N
            Return Utility.CHARtoBCD(DateTime.Parse(str).ToString("yyyyMMdd"), 4)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �K�p���t��byte�z����擾
    ''' </summary>
    Public Shared Function GetApplyDateDEC(ByVal str As String, ByVal name As String) As Byte()

        Try
            '���t�`�F�b�N
            Return Utility.CHARtoDEC(DateTime.Parse(str).ToString("yyyyMMdd"), 8)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �K�p���t��byte�z����擾
    ''' </summary>
    Public Shared Function GetApplyDateTimeDEC(ByVal str As String, ByVal name As String) As Byte()

        Try
            '���t�`�F�b�N
            Return Utility.CHARtoDEC(DateTime.Parse(str).ToString("yyyyMMddHHmm"), 12)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �K�p���t��byte�z����擾
    ''' </summary>
    Public Shared Function GetApplyDateTimeBCD(ByVal str As String, ByVal name As String) As Byte()

        Try
            '���t�`�F�b�N
            Return Utility.CHARtoBCD(DateTime.Parse(str).ToString("yyyyMMddHHmm"), 6)
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �}�X�^�o�[�W������byte�z����擾
    ''' </summary>
    Public Shared Function GetVersion(ByVal str As String) As Byte()

        Try
            If IsBetweenAnd(str, "1", "255") Then
                Return New Byte() {Byte.Parse(str)}
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, "�}�X�^�o�[�W����")
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �w�蕶�����byte�z����擾
    ''' </summary>
    Public Shared Function GetBytesBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return New Byte() {Byte.Parse(str)}
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �w�蕶�����byte�z����擾�A�z���2byte������
    ''' </summary>
    Public Shared Function GetBytes2BetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return BitConverter.GetBytes(Short.Parse(str))
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �w�蕶�����byte�z����擾�A�z���3byte������(���g���[���^�iBig Endian�j)
    ''' </summary>
    Public Shared Function GetBytes3BetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return Utility.CHARtoBINwithBigEndian(str, 3)
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �w�蕶�����BCD byte�z����擾
    ''' </summary>
    Public Shared Function GetBCDBytesBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String, ByVal len As Integer) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return Utility.CHARtoBCD(str, len)
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' �w�蕶�����BCD byte�z����擾
    ''' </summary>
    Public Shared Function GetDECBytesBetweenAnd(ByVal str As String, ByVal min As String, ByVal max As String, ByVal name As String, ByVal len As Integer) As Byte()

        Try
            If IsBetweenAnd(str, min, max) Then
                Return Utility.CHARtoDEC(str, len)
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

    ''' <summary>
    ''' ���s�@�֖���byte�z����擾
    ''' </summary>
    Public Shared Function GetBytesKikan(ByVal str As String, ByVal name As String) As Byte()

        Dim ret As Byte() = New Byte(15) {}
        ret(0) = &H30

        If "0".CompareTo(str) = 0 Then
            Return ret
        End If

        Try
            If Encoding.GetEncoding(932).GetByteCount(str) / 2 = str.Length Then
                'SHIFT-JIS��JIS
                Dim temp As Byte() = Utility.SJtoJIS(str)
                Array.Copy(temp, ret, temp.Length)
            Else
                Throw New Exception
            End If

            Return ret
        Catch ex As Exception
            AlertBox.Show(Lexis.ERR_COMMON, name)
            Throw
        End Try

        Return Nothing
    End Function

End Class
