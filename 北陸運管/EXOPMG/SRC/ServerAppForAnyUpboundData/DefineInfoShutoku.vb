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

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.ServerApp.RecDataStructure

''' <summary>
''' �t�@�C�����ɂđ��M���N���C�A���g������͂��A�ꎞ�ێ�����B
''' </summary>
Public Class DefineInfoShutoku

#Region "�錾�̈�iPrivate�j"

    ''' <summary>
    ''' �l�^�C�v�̃`�F�b�N
    ''' </summary>
    Public Shared DataTypeError As String = "{0}�^�C�v�G���[�B"

    ''' <summary>
    ''' �P�s�ڂ̃f�[�^���ڐ��̃`�F�b�N
    ''' </summary>
    Public Shared DataNumError As String = "��`���{0}�s�ڂ̃f�[�^���ڐ����s���ł��B"

#End Region

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' ��`���̎擾
    ''' </summary>
    ''' <param name="fileName">INI�t�@�C����</param>
    ''' <param name="sectionName">�Z�N�V������</param> 
    ''' <param name="infoObj">�擾�������ʂ�ۑ��p</param> 
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>INI�t�@�C�����ɂēd���t�H�[�}�b�g��`�����擾���A�ꎞ�ێ�����</remarks>
    Public Shared Function GetDefineInfo(ByVal fileName As String, _
                                         ByVal sectionName As String, _
                                         ByRef infoObj() As DefineInfo) As Boolean
        Dim bRtn As Boolean = False
        Dim i As Integer = 0
        Dim strDefInfo As String = ""
        Dim strData() As String
        Try

            'INI�t�@�C���̑��݃`�F�b�N
            If File.Exists(fileName) = False Then
                Log.Error(String.Format(RecAppConstants.ERR_INI_FILE_NOT_FOUND, fileName))
                Return False
            End If

            For i = 1 To 9999
                strDefInfo = Constant.GetIni(sectionName, Format(i, "0000"), fileName)
                If strDefInfo <> "" Then
                    strData = strDefInfo.Split(CChar(","))

                    If strData.Length < 15 Then
                        Log.Error(String.Format(DataNumError, i))
                        Return bRtn
                    End If

                    'TODO: OPT: AAAAAA ���������A���̃��\�b�h���P�t�@�C����o�^���邽�тɌĂ΂��
                    '���Ǝ��̂����{�I��NG�ł��邪�A����͂��܂�ɂ�...
                    '�Ƃ肠�����u() As RecDataStructure.DefineInfo�v��grep���āA
                    '�u As List(Of RecDataStructure.DefineInfo)�v�ɒu�������A
                    '���̃��\�b�h�ł́A�P�񂾂��uinfoObj = New List(Of DefineInfo)�v����
                    '�����́uNew DefineInfo�v�ƁA�����Add�ɂ��邾���ł��A�t�B�[���h����
                    '�K�敪��DefineInfo�̃R�s�[���������Ȃ������A�Ȃ�ڂ��܂��ɂȂ肻���H
                    '����Ƃ��A�̈�m�ۂ̃R�X�g���������ׂ��A���̃��[�v�̑O��sectionName����
                    '����S�Ă�Key���擾���A�P�񂾂��z���New���s�����Ƃɂ��邩�H
                    '�������A�̈�m�ۂ̃R�X�g�������o������A�o�^�v���Z�X�p�̂��̃��C�u������
                    '�e�t�B�[���h�̂�����Ƃ����ϊ����s�������ł��A�q�[�v����̖��ʂȊm�ۂ�
                    '���S����s���Ă��邵...
                    ReDim Preserve infoObj(i - 1)

                    '���ږ��́F���{�ꖼ�̂��擾�B�G���[���b�Z�[�W�Ɏg�p�B
                    infoObj(i - 1).KOMOKU_NAME = strData(0)

                    '�R�}���h�F�����擾���邪�A�b��g�p���܂���B
                    infoObj(i - 1).COMMENT = strData(1)

                    '�o�C�g�I�t�Z�b�g: �Y�����ڂ̃o�C�g�I�t�Z�b�g
                    Dim sByteOffset As String = strData(2)
                    If IsNumeric(sByteOffset) Then
                        infoObj(i - 1).BYTE_OFFSET = Convert.ToInt32(sByteOffset)
                    Else
                        Log.Error(String.Format(DataTypeError, "�o�C�g�I�t�Z�b�g"))
                        Return bRtn
                    End If

                    '�o�C�g�����O�X: �Y�����ڂ̃o�C�g�����O�X
                    Dim sByteLen As String = strData(3)
                    If IsNumeric(sByteLen) Then
                        infoObj(i - 1).BYTE_LEN = Integer.Parse(sByteLen)
                    Else
                        Log.Error(String.Format(DataTypeError, "�o�C�g�����O�X"))
                        Return bRtn
                    End If


                    '�r�b�g�I�t�Z�b�g: �Y�����ڂ̃r�b�g�I�t�Z�b�g
                    Dim sBitOffset As String = strData(4)
                    If IsNumeric(sBitOffset) Then
                        infoObj(i - 1).BIT_OFFSET = Integer.Parse(sBitOffset)
                    Else
                        Log.Error(String.Format(DataTypeError, "�r�b�g�I�t�Z�b�g"))
                        Return bRtn
                    End If

                    '�r�b�g�����O�X: �Y�����ڂ̃r�b�g�����O�X
                    Dim sBitLen As String = strData(5)
                    If IsNumeric(sBitLen) Then
                        infoObj(i - 1).BIT_LEN = Integer.Parse(sBitLen)
                    Else
                        Log.Error(String.Format(DataTypeError, "�r�b�g�����O�X"))
                        Return bRtn
                    End If

                    '�f�[�^�`��:  �Y�����ڂ�BIN�܂���BCD
                    infoObj(i - 1).DATA_FORMAT = strData(6)

                    '�t�B�[���h��: �o�^�Ώۂc�a�t�B�[���h
                    infoObj(i - 1).FIELD_NAME = strData(7)

                    '�t�B�[���h�`��: �o�^���̌^
                    infoObj(i - 1).FIELD_FORMAT = strData(8)

                    '��L�[
                    If UCase(strData(9)).Equals("TRUE") Then
                        infoObj(i - 1).PARA1 = True
                    Else
                        infoObj(i - 1).PARA1 = False
                    End If

                    'IS NULL
                    If UCase(strData(10)).Equals("TRUE") Then
                        infoObj(i - 1).PARA2 = True
                    Else
                        infoObj(i - 1).PARA2 = False
                    End If

                    '�p�����[�^�[
                    infoObj(i - 1).PARA3 = strData(11)

                    '�p�����[�^�[
                    infoObj(i - 1).PARA4 = strData(12)

                    '�p�����[�^�[
                    infoObj(i - 1).PARA5 = strData(13)

                    '�p�����[�^�[
                    infoObj(i - 1).PARA6 = strData(14)
                Else
                    Exit For
                End If
            Next

            bRtn = True
        Catch ex As Exception
            Log.Error(RecAppConstants.ERR_BAD_INI_FILE)
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        End Try

        '��`�F�b�N
        If bRtn AndAlso infoObj Is Nothing OrElse infoObj.Length <= 0 Then
            Log.Error(RecAppConstants.ERR_BAD_INI_FILE)
            bRtn = False
        End If

        Return bRtn

    End Function

#End Region

End Class