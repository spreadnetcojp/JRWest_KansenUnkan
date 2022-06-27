' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports System.Text

''' <summary>
''' �Ɩ��d�l�Ɋ�Â��`�F�b�N��������񋟂���N���X�B
''' </summary>
''' <remarks></remarks>
Public Class OPMGUtility

    ''' <summary>�w��o�C�g�ʒu����w��o�C�g������Byte�z������o��</summary>
    ''' <remarks>
    ''' �w��o�C�g�ʒu����w��o�C�g������Byte�z������o��
    ''' </remarks>
    ''' <param name="fromBytes">Byte�z��</param>
    ''' <param name="startIndex">�w��o�C�g�ʒu</param>
    ''' <param name="resultLen">�w��o�C�g��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function getBytesFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal resultLen As Integer) As Byte()
        If startIndex + resultLen > fromBytes.Length Then
            Log.Error("�������s���ł��B" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; resultLen:" & resultLen & _
                          "; fromBytes.Length:" & fromBytes.Length) '�����s��
            Throw New DatabaseException()
        End If

        Dim bRtn(resultLen - 1) As Byte

        For i As Integer = 0 To resultLen - 1
            bRtn.SetValue((fromBytes.GetValue(startIndex + i)), i)
        Next

        Return bRtn
    End Function

    ''' <summary>�w��o�C�g�ʒu����w��o�C�g������Byte�z���ݒ肷��</summary>
    ''' <remarks>
    '''  �w��o�C�g�ʒu����w��o�C�g������Byte�z���ݒ肷��
    ''' </remarks>
    ''' <param name="value">Byte�z��</param>
    ''' <param name="toBytes">Byte�z��</param>
    ''' <param name="startIndex">�w��o�C�g�ʒu</param>
    ''' <returns>�ݒ��o�C�g�ʒu</returns>
    Public Shared Function setBytesToBytes(ByVal value As Byte(), ByRef toBytes As Byte(), ByVal startIndex As Integer) As Integer
        If startIndex + value.Length > toBytes.Length Then
            Log.Error("�������s���ł��B" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; value.Length:" & value.Length & _
                          "; toBytes.Length:" & toBytes.Length) '�����s��
            Throw New DatabaseException()
        End If

        For i As Integer = 0 To value.Length - 1
            toBytes.SetValue((value.GetValue(i)), startIndex + i)
        Next

        Return startIndex + value.Length
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��iBIN�������j]
    ''' �ϊ����̕�����Ascii�̂ݗL���B
    ''' </summary>
    ''' <param name="fromBytes">Byte�z��</param>
    ''' <param name="startIndex">�w��o�C�g�ʒu</param>
    ''' <param name="bytesLen">Byte��</param>
    ''' <returns>�ϊ��㕶��</returns>
    Public Shared Function getAsciiStringFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal bytesLen As Integer) As String
        If startIndex + bytesLen > fromBytes.Length Then
            Log.Error("�������s���ł��B" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; bytesLen:" & bytesLen & _
                          "; fromBytes.Length:" & fromBytes.Length) '�����s��
            Throw New DatabaseException()
        End If

        Dim bTemp As Byte()
        bTemp = getBytesFromBytes(fromBytes, startIndex, bytesLen)

        Return binToAsciiString(bTemp)
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��i������BIN�j]
    ''' �ϊ����̕�����Ascii�̂ݗL���B
    ''' </summary>
    ''' <param name="value">����</param>
    ''' <param name="toBytes">Byte�z��</param>
    ''' <param name="startIndex">�w��o�C�g�ʒu</param>
    ''' <param name="len">Byte��</param>
    ''' <returns>�ݒ��o�C�g�ʒu</returns>
    Public Shared Function setAsciiStringToBytes(ByVal value As String, ByVal toBytes As Byte(), ByVal startIndex As Integer, ByVal len As Integer) As Integer
        If startIndex + len > toBytes.Length Then
            Log.Error("�������s���ł��B" & vbCrLf & _
                          "startIndex:" & startIndex & _
                          "; len:" & len & _
                          "; toBytes.Length:" & toBytes.Length) '�����s��
            Throw New DatabaseException()
        End If

        If value.Length > len Then
            Log.Error("�������s���ł��B" & vbCrLf & _
                          "value:" & value & _
                          "; len:" & len) '�����s��
            Throw New DatabaseException()
        End If

        Dim bTemp As Byte()
        bTemp = asciiStringToBin(value, len)

        Return setBytesToBytes(bTemp, toBytes, startIndex)
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��iBIN�������j]
    ''' �ϊ����̕�����Shift_JIS�̂ݗL���B
    ''' </summary>
    ''' <param name="fromBytes">Byte�z��</param>
    ''' <param name="startIndex">�w��o�C�g�ʒu</param>
    ''' <param name="bytesLen">Byte��</param>
    ''' <returns>�ϊ��㕶��</returns>
    Public Shared Function getJisStringFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal bytesLen As Integer) As String
        Dim bTemp As Byte()
        bTemp = getBytesFromBytes(fromBytes, startIndex, bytesLen)

        Return binToJisString(bTemp)
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��i������BIN�j]
    ''' �ϊ����̕�����Shift_JIS�̂ݗL���B
    ''' </summary>
    ''' <param name="value">����</param>
    ''' <param name="toBytes">Byte�z��</param>
    ''' <param name="startIndex">�w��o�C�g�ʒu</param>
    ''' <param name="len">Byte��</param>
    ''' <returns>�ݒ��o�C�g�ʒu</returns>
    Public Shared Function setJisStringToBytes(ByVal value As String, ByRef toBytes As Byte(), ByVal startIndex As Integer, ByVal len As Integer) As Integer
        Dim bTemp As Byte()
        bTemp = jisStringToBin(value, len)

        Return setBytesToBytes(bTemp, toBytes, startIndex)
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��i������BIN�j]
    ''' �ϊ����̕�����ascii�̂ݗL���B
    ''' </summary>
    ''' <param name="ASCIIpar">ascii����</param>
    ''' <param name="len">Byte��</param>
    ''' <returns>Byte�z��</returns>
    Public Shared Function asciiStringToBin(ByVal ASCIIpar As String, ByVal len As Integer) As Byte()
        If ASCIIpar.Length > len Then
            Log.Error("�������s���ł��B" & vbCrLf & _
                          "ASCIIpar:" & ASCIIpar & _
                          "; len:" & len) '�����s��
            Throw New DatabaseException()
        End If

        Dim sTemp As String
        sTemp = ASCIIpar.PadRight(len)

        Return Encoding.ASCII.GetBytes(sTemp)
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��iBIN�������j]
    ''' �ϊ����̕�����ascii�̂ݗL���B
    ''' </summary>
    ''' <param name="par">Byte�z��</param>
    ''' <returns>ascii����</returns>
    Public Shared Function binToAsciiString(ByVal par As Byte()) As String
        Return Encoding.ASCII.GetString(par).Trim
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��i������BIN�j]
    ''' �ϊ����̕�����Shift_JIS�̂ݗL���B
    ''' </summary>
    ''' <param name="jisPar">Shift_JIS����Byte�z��</param>
    ''' <param name="len">Byte��</param>
    ''' <returns>Byte�z��</returns>
    Public Shared Function jisStringToBin(ByVal JISpar As String, ByVal len As Integer) As Byte()
        Dim nLen As Integer = 0
        Dim bChar() As Char = {}
        Dim bRet() As Byte = {}

        bRet = Utility.SJtoJIS(JISpar)
        nLen = len - bRet.Length

        If nLen > 0 Then
            Array.Resize(bChar, nLen)
            nLen = bRet.Length
            Array.Resize(bRet, bRet.Length + bChar.Length)
            setBytesToBytes(System.Text.Encoding.Default.GetBytes(bChar), bRet, nLen)
        Else
            Array.Resize(bRet, len)
        End If
        Return bRet
    End Function

    ''' <summary>
    ''' [�R�[�h�ϊ��iBIN�������j]
    ''' �ϊ����̕�����Shift_JIS�̂ݗL���B
    ''' </summary>
    ''' <param name="jisPar">Byte�z��</param>
    ''' <returns>Shift_JIS����</returns>
    Public Shared Function binToJisString(ByVal jisPar As Byte()) As String
        Return System.Text.Encoding.GetEncoding("Shift_JIS").GetString(jisPar)
    End Function

    ''' <summary>
    ''' ������͉p�����ł��邩���`�F�b�N����B
    ''' </summary>
    ''' <param name="sTxtContent">�`�F�b�N����K�v�̂��镶����</param>
    ''' <returns>�����񍇖@�t���O</returns>
    ''' <remarks>�h�c�R�[�h�A�p�X���[�h��0-9�Aa-z�AA-Z�ɂ���đg�ݍ��킹�镶����Ɍ���</remarks>
    Public Shared Function checkCharacter(ByVal sTxtContent As String) As Boolean

        '���֐��̖߂�l�B
        Dim bResult As Boolean = False
        Dim cTxt As Char
        For i As Integer = 0 To sTxtContent.Length() - 1
            cTxt = sTxtContent.Chars(i)
            If (Asc(cTxt) >= 48 And Asc(cTxt) <= 57) Or (Asc(cTxt) >= 65 And Asc(cTxt) <= 90) Or (Asc(cTxt) >= 97 And Asc(cTxt) <= 122) Then
                bResult = True
            Else
                bResult = False
                Exit For
            End If
        Next

        Return bResult

    End Function

    'TODO: �폜
    ''' <summary>
    ''' ������͐����ł��邩���`�F�b�N����B
    ''' </summary>
    ''' <param name="sTxtContent">�`�F�b�N����K�v�̂��镶����</param>
    ''' <returns>�����񍇖@�t���O</returns>
    ''' <remarks>�������0-9�ɂ���đg�ݍ��킹�镶����Ɍ���</remarks>
    Public Shared Function checkNumber(ByVal sTxtContent As String) As Boolean

        '���֐��̖߂�l�B
        Dim bResult As Boolean = False
        Dim cTxt As Char
        For i As Integer = 0 To sTxtContent.Length() - 1
            cTxt = sTxtContent.Chars(i)
            If Asc(cTxt) >= 48 And Asc(cTxt) <= 57 Then
                bResult = True
            Else
                bResult = False
                Exit For
            End If
        Next

        Return bResult

    End Function

    ''''<summary>
    ''''������`�F�b�N
    ''''</summary>
    ''''<param name="CheckValue">�`�F�b�N�Ώە�����</param>
    ''''<param name="MaxLength">�ő啶����</param>
    ''''<param name="CharSize">�S�p���p�����@0:���p�S�p�@1:���p�̂݉@2:�S�p�̂݉�</param>
    ''''<param name="NoInputFlag">�����̓`�F�b�N�@true:�����͋֎~�@false:�����͉�</param>
    ''''<returns>int 0:���� -1:���������߃G���[ -2:�����̓G���[ -3:�S�p���݃G���[ -4:���p���݃G���[ </returns>
    Public Shared Function CheckString(ByVal CheckValue As String, ByVal MaxLength As Integer _
                        , ByVal CharSize As Integer, ByVal NoInputFlag As Boolean) As Integer

        Dim Encode As Encoding
        Encode = Encoding.GetEncoding("Shift_JIS")
        If True = NoInputFlag Then
            If CheckValue.Trim.Length = 0 Then
                '�����̓G���[
                Return -2
            End If
            '���p��������
            If CharSize = 1 Then
                If Not CheckValue.Length = Encode.GetByteCount(CheckValue) Then
                    '�S�p�������܂܂�Ă���
                    Return -3
                End If
            End If
            '�S�p��������
            If CharSize = 2 Then
                If Not CheckValue.Length = Encode.GetByteCount(CheckValue) / 2 Then
                    '���p�������܂܂�Ă���
                    Return -4
                End If
            End If
            '�������`�F�b�N
            If MaxLength * 2 < Encode.GetByteCount(CheckValue) Then
                '�������I�[�o�[
                Return -1
            End If
        End If
        '�`�F�b�N����
        Return 0
    End Function
End Class
