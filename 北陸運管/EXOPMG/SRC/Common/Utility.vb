' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX�����F
'   Ver      ���t        �S��       �R�����g
'   0.0      2006/07/07             �V�K�쐬
'   0.1      2006/11/15  muneyuki   CHARtoBIN�ACHARtoBIN �ύX BigEndian��LittleEndian
'                                   CHARtoBINwithBigEndian�ABINtoCHARwithBigEndian �ǉ�
'   0.2      2011/10/20  NES(�͘e)  INI����̂c�a�ڑ�������擾��ǉ�
'   0.3      2013/04/01  (NES)����  �^�ǒ[���Ɖ^�ǃT�[�o�ŕ��򂷂�ނ̐��i�ˑ������������A
'                                   CopyIntToBcdBytes�`DeleteTemporalDirectory��ǉ�
' **********************************************************************
Option Strict On
Option Explicit On

''' <summary>
''' ���[�e�B���e�B
''' </summary>
Public Class Utility

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< ���\�b�h

#Region "�R�[�h�ϊ��i������BIN�j(���g���[���^�iBig Endian�j�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i������BIN�j(���g���[���^�iBig Endian�j�j]
    ''' �ϊ����̕����͐��l�̂ݗL���B
    ''' </summary>
    ''' <param name="BaseChar">�ϊ��O������</param>
    ''' <param name="ByteLength">�ϊ����Byte��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function CHARtoBINwithBigEndian(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim nConv As UInt64
        Try
            If String.IsNullOrEmpty(BaseChar) Then BaseChar = "0"
            nConv = System.UInt64.Parse(BaseChar)
            bRtn = System.BitConverter.GetBytes(nConv)
            Array.Resize(bRtn, ByteLength)  '�T�C�Y�ύX
            Array.Reverse(bRtn)             '�v�f�����]
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iBIN�������j(���g���[���^�iBig Endian�j�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iBIN�������j(���g���[���^�iBig Endian�j�j]
    ''' �ϊ���̕����͐��l�̂ݗL���B
    ''' </summary>
    ''' <param name="BaseByte">�ϊ��OByte�z��</param>
    ''' <returns>�ϊ��㕶����</returns>
    Public Shared Function BINtoCHARwithBigEndian(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Dim n As UInt64
        Dim nRtn As UInt64
        Try
            Array.Reverse(BaseByte)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
        Try
            For i = 0 To BaseByte.Length - 1
                n = CType(BaseByte(i).ToString, UInt64)
                If i <> 0 Then
                    If n <> 0 Then
                        n = CType((256 ^ i) * n, UInt64)
                    End If
                End If
                nRtn = nRtn + n
            Next
            sRtn = CType(nRtn, String)
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        Finally
            Array.Reverse(BaseByte)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��i������BIN�j(�C���e���^�iLittle Endian�j�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i������BIN�j(�C���e���^�iLittle Endian�j�j]
    ''' �ϊ����̕����͐��l�̂ݗL���B
    ''' </summary>
    ''' <param name="BaseChar">�ϊ��O������</param>
    ''' <param name="ByteLength">�ϊ����Byte��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function CHARtoBIN(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim nConv As UInt64
        Try
            If String.IsNullOrEmpty(BaseChar) Then BaseChar = "0"
            nConv = System.UInt64.Parse(BaseChar)
            bRtn = System.BitConverter.GetBytes(nConv)
            Array.Resize(bRtn, ByteLength)  '�T�C�Y�ύX

            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iBIN�������j(�C���e���^�iLittle Endian�j�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iBIN�������j(�C���e���^�iLittle Endian�j�j]
    ''' �ϊ���̕����͐��l�̂ݗL���B
    ''' </summary>
    ''' <param name="BaseByte">�ϊ��OByte�z��</param>
    ''' <returns>�ϊ��㕶����</returns>
    Public Shared Function BINtoCHAR(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Dim n As UInt64
        Dim nRtn As UInt64

        Try
            For i = 0 To BaseByte.Length - 1
                n = CType(BaseByte(i).ToString, UInt64)
                If i <> 0 Then
                    If n <> 0 Then
                        n = CType((256 ^ i) * n, UInt64)
                    End If
                End If
                nRtn = nRtn + n
            Next
            sRtn = CType(nRtn, String)
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        Finally

        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��i������DEC�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i������DEC�j]
    ''' �ϊ��O�̕����͐��l�̂ݗL���B
    ''' �w��Byte�����Ȃ��ꍇ�́A���[�����߂����{�B
    ''' </summary>
    ''' <param name="BaseChar">�ϊ��O������</param>
    ''' <param name="ByteLength">�ϊ����Byte��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function CHARtoDEC(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim i As Integer
        Dim sInf As String
        Dim sSet As String
        Dim sErr As String = ""
        Try
            If BaseChar.Length > ByteLength Then
                sErr = "�ϊ��O�̕������w��ԋpByte�w�萔�𒴂��Ă��܂��B" & vbCrLf & _
                       "CHARtoDEC[BaseChar=" & BaseChar & "][ByteLength=" & ByteLength.ToString & "]"
                Throw New System.ArgumentException(sErr)
            End If
            ReDim bRtn(ByteLength - 1)
            sInf = BaseChar.PadLeft(ByteLength, CType("0", Char))   '���[������
            For i = 0 To ByteLength - 1
                sSet = sInf.Substring(0, 1)
                'bRtn(i) = CType("&H00" + sSet, Byte)
                bRtn(i) = System.Convert.ToByte(sSet, 16)
                sInf = sInf.Substring(1)
            Next
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iDEC�������j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iDEC�������j]
    ''' �ϊ���̕����͐��l�̂ݗL���B
    ''' </summary>
    ''' <param name="BaseByte">�ϊ��OByte�z��</param>
    ''' <returns>�ϊ��㕶����</returns>
    Public Shared Function DECtoCHAR(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Try
            For i = 0 To BaseByte.Length - 1
                'sRtn = sRtn + Hex(BaseByte(i)).PadLeft(2, CType("0", Char))
                sRtn = sRtn + Hex(BaseByte(i)).PadLeft(2, CType("0", Char)).Substring(1, 1)
            Next
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��i������BCD�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i������BCD�j]
    ''' �ϊ��O�̕����͐��l�̂ݗL���B
    ''' �w��Byte�����Ȃ��ꍇ�́A���[�����߂����{�B
    ''' </summary>
    ''' <param name="BaseChar">�ϊ��O������</param>
    ''' <param name="ByteLength">�ϊ����Byte��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function CHARtoBCD(ByVal BaseChar As String, ByVal ByteLength As Integer) As Byte()
        Dim bRtn() As Byte
        Dim i As Integer
        Dim sInf As String
        Dim sSet As String
        Dim sErr As String = ""
        Try
            If BaseChar.Length > ByteLength * 2 Then
                sErr = "�ϊ��O�̕������w��ԋpByte�w�萔�𒴂��Ă��܂��B" & vbCrLf & _
                       "CHARtoBCD[BaseChar=" & BaseChar & "][ByteLength=" & ByteLength.ToString & "]"
                Throw New System.ArgumentException(sErr)
            End If
            ReDim bRtn(ByteLength - 1)
            sInf = BaseChar.PadLeft(ByteLength * 2, CType("0", Char))   '���[������
            For i = 0 To ByteLength - 1
                sSet = sInf.Substring(0, 2)
                'bRtn(i) = CType("&H00" + sSet, Byte)
                bRtn(i) = System.Convert.ToByte(sSet, 16)
                sInf = sInf.Substring(2)
            Next
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iBCD�������j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iBCD�������j]
    ''' �ϊ���̕����͐��l�̂ݗL���B
    ''' </summary>
    ''' <param name="BaseByte">�ϊ��OByte�z��</param>
    ''' <returns>�ϊ��㕶����</returns>
    Public Shared Function BCDtoCHAR(ByVal BaseByte() As Byte) As String
        Dim sRtn As String = ""
        Dim i As Integer
        Try
            For i = 0 To BaseByte.Length - 1
                sRtn = sRtn + Hex(BaseByte(i)).PadLeft(2, CType("0", Char))
            Next
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iBIN��BCD�j[BINtoCHAR��CHARtoBCD]"
    ''' <summary>
    ''' [�R�[�h�ϊ��iBIN��BCD�j]
    ''' �����Ƃ��āA���R�[�h�ϊ���BINtoCHAR��CHARtoBCD�����{�B
    ''' [�����g�p���\�b�h�FBINtoCHAR,CHARtoBCD]
    ''' </summary>
    ''' <param name="BaseByte">�ϊ��OByte�z��</param>
    ''' <param name="ByteLength">�ϊ����Byte��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function BINtoBCD(ByVal BaseByte() As Byte, ByVal ByteLength As Integer) As Byte()
        Try
            Return CHARtoBCD(BINtoCHAR(BaseByte), ByteLength)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iBCD��BIN�j[BCDtoCHAR��CHARtoBIN]"
    ''' <summary>
    ''' [�R�[�h�ϊ��iBCD��BIN�j]
    ''' �����Ƃ��āA���R�[�h�ϊ���BCDtoCHAR��CHARtoBIN�����{�B
    ''' [�����g�p���\�b�h�FBCDtoCHAR,CHARtoBIN]
    ''' </summary>
    ''' <param name="BaseByte">�ϊ��OByte�z��</param>
    ''' <param name="ByteLength">�ϊ����Byte��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function BCDtoBIN(ByVal BaseByte() As Byte, ByVal ByteLength As Integer) As Byte()
        Try
            Return CHARtoBIN(BCDtoCHAR(BaseByte), ByteLength)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iS-JIS��JIS�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iS-JIS��JIS�j]
    ''' </summary>
    ''' <param name="BaseChar">�ϊ��O������</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function SJtoJIS(ByVal BaseChar As String) As Byte()
        Dim bRtn() As Byte
        Dim bInf() As Byte
        Dim nStartPos As Integer
        Dim nEndPos As Integer
        Dim i As Integer
        Dim n As Integer = 0
        Try
            bInf = System.Text.Encoding.GetEncoding(50220).GetBytes(BaseChar)
            If bInf.Length >= 8 Then
                If ((Hex(bInf(0)) = "1B") And _
                    (Hex(bInf(1)) = "24") And _
                    (Hex(bInf(2)) = "42")) Then
                    nStartPos = 3
                Else
                    nStartPos = 0
                End If
                If ((Hex(bInf(bInf.Length - 3)) = "1B") And _
                    (Hex(bInf(bInf.Length - 2)) = "28") And _
                    (Hex(bInf(bInf.Length - 1)) = "42")) Then
                    nEndPos = bInf.Length - 4
                Else
                    nEndPos = bInf.Length - 1
                End If
                ReDim bRtn(nEndPos - nStartPos)
                For i = nStartPos To nEndPos
                    bRtn(n) = bInf(i)
                    n += 1
                Next i
                Return bRtn
            Else
                bRtn = bInf
            End If
            Return bRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�R�[�h�ϊ��iJIS��S-JIS�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iJIS��S-JIS�j]
    ''' </summary>
    ''' <param name="BaseChar">�ϊ��O������</param>
    ''' <returns>�ϊ��㕶����</returns>
    Public Shared Function JIStoSJ(ByVal BaseChar As String) As String
        Dim sRtn As String = BaseChar
        sRtn = BaseChar
        Try
            Dim bytesData As Byte() = System.Text.Encoding.GetEncoding(932).GetBytes(sRtn)
            sRtn = System.Text.Encoding.GetEncoding(50220).GetString(bytesData)
            Return sRtn
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�c�a�ڑ�������擾"
    ''' <summary>
    ''' [�c�a�ڑ�������擾]
    ''' �ݒ�����c�a�ڑ������擾����B
    ''' </summary>
    ''' <returns>�c�a�ڑ�������擾</returns>
    Public Shared Function GetDbConnectString() As String
        Try
            Return String.Format("Server={0};Database={1};UID={2};PWD={3}", _
                                 BaseConfig.DatabaseServerName, _
                                 BaseConfig.DatabaseName, _
                                 BaseConfig.DatabaseUserName, _
                                 BaseConfig.DatabasePassword)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�C�x���g���O�o��"
    ''' <summary>
    ''' [�C�x���g���O�o��]
    ''' ���[�J�� �R���s���[�^�̃A�v���P�[�V�������O�ɏo�͂���B
    ''' �o�͓��e�̐擪�Ɍďo�������o�͂���B
    ''' ���s�}�[�N�Ɖ��s�R�[�h�͔��p�X�y�[�X�P���ɕϊ�����B
    ''' </summary>
    ''' <param name="EntType">���</param>
    ''' <param name="Detail">�t�����</param>
    ''' <param name="Souce_Name">�ďo���N���X��</param>
    ''' <param name="Method_Name">�ďo�����\�b�h��</param>
    Public Shared Sub WriteLogToEvent(ByVal EntType As EventLogEntryType, ByVal Detail As String, ByVal Souce_Name As String, ByVal Method_Name As String)
        Try
            WriteLogToEventCore(EntType, Detail, Souce_Name, Method_Name)
        Catch ex As Exception
            '�������Ȃ�
        End Try
    End Sub
    ''' <summary>
    ''' [�C�x���g���O�o��]
    ''' ���[�J�� �R���s���[�^�̃A�v���P�[�V�������O�ɏo�͂���B
    ''' �o�͓��e�̐擪�Ɍďo�������o�͂���B
    ''' ���s�}�[�N�Ɖ��s�R�[�h�͔��p�X�y�[�X�P���ɕϊ�����B
    ''' </summary>
    ''' <param name="EntType">���</param>
    ''' <param name="Detail">�t�����</param>
    ''' <param name="Souce_Name">�ďo���N���X��</param>
    ''' <param name="Method_Name">�ďo�����\�b�h��</param>
    Private Shared Sub WriteLogToEventCore(ByVal EntType As EventLogEntryType, ByVal Detail As String, ByVal Souce_Name As String, ByVal Method_Name As String)
        Try
            Detail = Detail.Replace(vbCrLf, Space(1))
            Detail = String.Format("{0}[EVENT={1}]", Detail, Method_Name)

            '���[�J�� �R���s���[�^�̃A�v���P�[�V�������O�ɏo��
            System.Diagnostics.EventLog.WriteEntry(Souce_Name, Detail, EntType)
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try
    End Sub
#End Region

    ' <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< ���\�b�h�i�c�[���n�j

#Region "�t�H���_����"
    ''' <summary>
    ''' [�t�H���_����]
    ''' </summary>
    ''' <param name="sFolderPath">�Ώۃp�X</param>
    ''' <returns>True:����,False:���s(�쐬�ł��Ȃ��p�X�w�蓙)</returns>
    Public Shared Function MakeFolder(ByVal sFolderPath As String) As Boolean
        Try
            System.IO.Directory.CreateDirectory(sFolderPath)
            Return True
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Return False
        End Try
    End Function
#End Region

#Region "Null�����u��"
    ''' <summary>
    ''' [Null�����u��]
    ''' </summary>
    ''' <param name="oValue">�`�F�b�N����l�iDB��Select���ʂ̒l���i�[�����t�B�[���h��񓙁j</param>
    ''' <param name="sConvStr">Null���A�u�����镶����</param>
    ''' <returns>�u������������</returns>
    Public Shared Function CNull(ByVal oValue As Object, ByVal sConvStr As String) As String
        Try
            Return CNullCore(oValue, sConvStr)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
    ''' <summary>
    ''' [Null�����u��]
    ''' </summary>
    ''' <param name="oValue">�`�F�b�N����l�iDB��Select���ʂ̒l���i�[�����t�B�[���h��񓙁j</param>
    ''' <param name="sConvStr">Null���A�u�����镶����</param>
    ''' <returns>�u������������</returns>
    Private Shared Function CNullCore(ByVal oValue As Object, ByVal sConvStr As String) As String
        Dim sStr As String = ""
        Try
            If IsDBNull(oValue) Then
                sStr = sConvStr
            Else
                sStr = CStr(oValue)
            End If
            Return sStr
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "�V���O���N�H�e�[�V�����t��"
    ''' <summary>
    ''' [�V���O���N�H�e�[�V�����t��]
    ''' ��FSetSglQuot("a'b''cd",",") �� "'a''b''''cd',"
    ''' </summary>
    ''' <param name="sValue">�`�F�b�N����l</param>
    ''' <param name="sLstStr">�ŏI�ʒu�ɕt������l</param>
    ''' <returns>�t������������</returns>
    Public Shared Function SetSglQuot(ByVal sValue As String, ByVal sLstStr As String) As String
        Try
            Return SetSglQuotCore(sValue, sLstStr)
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
    ''' <summary>
    ''' [�V���O���N�H�e�[�V�����t��]
    ''' ��FSetSglQuot("a'b''cd") �� "'a''b''''cd'"
    ''' </summary>
    ''' <param name="sValue">�`�F�b�N����l</param>
    ''' <returns>�t������������</returns>
    Public Shared Function SetSglQuot(ByVal sValue As String) As String
        Try
            Return SetSglQuotCore(sValue, "")
        Catch ex As Exception
            WriteLogToEvent(EventLogEntryType.Error, ex.Message, ClsName(), MethodName())
            Throw New OPMGException(ex)
        End Try
    End Function
    ''' <summary>
    ''' [�V���O���N�H�e�[�V�����t��]
    ''' ��FSetSglQuot("a'b''cd",",") �� "'a''b''''cd',"
    ''' </summary>
    ''' <param name="sValue">�`�F�b�N����l</param>
    ''' <param name="sLstStr">�ŏI�ʒu�ɕt������l</param>
    ''' <returns>�t������������</returns>
    Private Shared Function SetSglQuotCore(ByVal sValue As String, ByVal sLstStr As String) As String
        Dim sStr As String = ""
        Try
            sStr = sValue.Replace("'", "''")
            sStr = "'" & sStr & "'" & sLstStr
            Return sStr
        Catch ex As Exception
            Throw New OPMGException(ex)
        End Try
    End Function
#End Region

#Region "���\�b�h���擾"
    ''' <summary>
    ''' [���\�b�h���擾]
    ''' �Ăяo�������\�b�h���̖��O��"���\�b�h��"�̏����ŕԋp���܂��B
    ''' </summary>
    Public Shared Function MethodName() As String
        Return New StackTrace(0, True).GetFrame(1).GetMethod().Name
    End Function
#End Region

#Region "�N���X���擾"
    ''' <summary>
    ''' [�N���X���擾]
    ''' �Ăяo�������\�b�h������������N���X�̖��O��"���O���.�N���X��"�ŕԋp���܂��B
    ''' </summary>
    Public Shared Function ClsName() As String
        Return New StackTrace(0, True).GetFrame(1).GetMethod().DeclaringType.ToString
    End Function
#End Region

#Region "�����_�ȉ��ۂߏ����֐�"
    ''' <summary>
    ''' �����_�ȉ��ۂߏ����֐�
    ''' </summary>
    ''' <param name="nValue">�ۂߑΏےl</param>
    ''' <param name="nRoundKbn">�ۂߋ敪(0:�؎̂�,1:�l�̌ܓ�,2:�؏グ,else:�؎̂�)</param>
    ''' <param name="nRoundPos">�����ȉ��ۂ߈ʒu(max5 5�ȏ��5)</param>
    ''' <returns>�ۂߌ��ʒl</returns>
    ''' <remarks></remarks>
    Public Shared Function RoundValue(ByVal nValue As Double, ByVal nRoundKbn As Integer, ByVal nRoundPos As Integer) As Double
        Dim bMinus As Boolean
        Dim nVal As Double
        Dim nRetVal As Double

        If nValue = 0 Then
            '�l0�͏����Ȃ�
            Return nValue
        ElseIf nValue < 0 Then
            '�}�C�i�X�ޔ�
            nVal = nValue * -1
            bMinus = True
        Else
            nVal = nValue
            bMinus = False
        End If

        If nVal = System.Math.Floor(nVal) Then
            '�����Ȃ�ۂ߂Ȃ�
            nRetVal = nVal
        Else
            '���ȉ��ۂ�
            '�ۂ߈ʒu�̍ő�͏�����T��
            If nRoundPos > 5 Then
                nRoundPos = 5
            End If

            '�ۂ߈ʒu�Ő����ɂȂ�悤�ɂ���i�ۂ߈ʒu0�͂��̂܂܁j
            If (10 ^ nRoundPos) > 0 Then
                nVal = nVal * (10 ^ nRoundPos)
            End If

            '--�ۂߋ敪�ɂ��ۂߏ���
            If nRoundKbn = 0 Then
                '�؂�̂�
                nRetVal = System.Math.Floor(nVal)
            ElseIf nRoundKbn = 1 Then
                '�l�̌ܓ�
                nRetVal = System.Math.Floor(nVal + 0.5)
            ElseIf nRoundKbn = 2 Then
                '�؂�グ
                If nVal <> System.Math.Floor(nVal) Then
                    nRetVal = System.Math.Floor(nVal) + 1
                Else
                    nRetVal = System.Math.Floor(nVal)
                End If
            Else
                '�؂�̂�
                nRetVal = System.Math.Floor(nVal)
            End If

            '�ۂ߈ʒu�����Ƃɂ��ǂ��i�ۂ߈ʒu0�͂��̂܂܁j
            If (10 ^ nRoundPos) > 0 Then
                nRetVal = nRetVal / (10 ^ nRoundPos)
            End If
        End If


        '�}�C�i�X���A
        If bMinus Then
            nRetVal = nRetVal * -1
        End If

        Return nRetVal

    End Function

#End Region

#Region "�w��o�C�g�ʒu����w��o�C�g������Byte�z������o��"
    ''' <summary>�w��o�C�g�ʒu����w��o�C�g������Byte�z������o��</summary>
    ''' <remarks>
    ''' �w��o�C�g�ʒu����w��o�C�g������Byte�z������o��
    ''' </remarks>
    ''' <param name="fromBytes">Byte�z��</param>
    ''' <param name="startIndex">�w��o�C�g�ʒu</param>
    ''' <param name="resultLen">�w��o�C�g��</param>
    ''' <returns>�ϊ���Byte�z��</returns>
    Public Shared Function GetBytesFromBytes(ByVal fromBytes As Byte(), ByVal startIndex As Integer, ByVal resultLen As Integer) As Byte()
        Dim bRtn(resultLen - 1) As Byte

        For i As Integer = 0 To resultLen - 1
            bRtn.SetValue((fromBytes.GetValue(startIndex + i)), i)
        Next

        Return bRtn
    End Function
#End Region

#Region "�R�[�h�ϊ��i����Integer��BCD�����j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i����Integer��BCD�����j]
    ''' </summary>
    ''' <param name="src">�ϊ���Integer�l</param>
    ''' <param name="dst">�������ݐ�Byte�z��</param>
    ''' <param name="pos">�������ݐ�Byte�z����̏������݈ʒu</param>
    ''' <param name="len">��������Byte��</param>
    Public Shared Sub CopyIntToBcdBytes(ByVal src As Integer, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        pos = pos + len
        For i As Integer = 1 To len
            Dim nextSrc As Integer = src \ 10
            Dim bin As Integer = src - nextSrc * 10
            src = nextSrc

            nextSrc = src \ 10
            bin = bin Or (src - nextSrc * 10) << 4
            src = nextSrc

            dst(pos - i) = CByte(bin)
        Next
    End Sub
#End Region

#Region "�R�[�h�ϊ��iBCD����������Integer�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iBCD����������Integer�j]
    ''' </summary>
    ''' <param name="src">�ϊ���Byte�z��</param>
    ''' <param name="pos">�ϊ���Byte�z����̎擾�ʒu</param>
    ''' <param name="len">�ϊ���Byte��</param>
    ''' <returns>�ϊ���Integer</returns>
    Public Shared Function GetIntFromBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            ret = ret * 10 + (bcd >> 4)
            ret = ret * 10 + (bcd And &H0f)
        Next
        Return ret
    End Function
#End Region

#Region "�R�[�h����iBCD�����j"
    ''' <summary>
    ''' [�R�[�h����iBCD�����j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            If (bcd >> 4) > 9 OrElse (bcd And &H0f) > 9 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h�ϊ��i����Integer���A���p�b�N�^BCD�����j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i����Integer���A���p�b�N�^BCD�����j]
    ''' </summary>
    ''' <param name="src">�ϊ���Integer�l</param>
    ''' <param name="dst">�������ݐ�Byte�z��</param>
    ''' <param name="pos">�������ݐ�Byte�z����̏������݈ʒu</param>
    ''' <param name="len">��������Byte��</param>
    Public Shared Sub CopyIntToUnpackedBcdBytes(ByVal src As Integer, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        pos = pos + len
        For i As Integer = 1 To len
            Dim nextSrc As Integer = src \ 10
            Dim bin As Integer = src - nextSrc * 10
            src = nextSrc
            dst(pos - i) = CByte(bin)
        Next
    End Sub
#End Region

#Region "�R�[�h�ϊ��i�A���p�b�N�^BCD����������Integer�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i�A���p�b�N�^BCD����������Integer�j]
    ''' </summary>
    ''' <param name="src">�ϊ���Byte�z��</param>
    ''' <param name="pos">�ϊ���Byte�z����̎擾�ʒu</param>
    ''' <param name="len">�ϊ���Byte��</param>
    ''' <returns>�ϊ���Integer</returns>
    Public Shared Function GetIntFromUnpackedBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            ret = ret * 10 + bcd
        Next
        Return ret
    End Function
#End Region

#Region "�R�[�h����i�A���p�b�N�^BCD�����j"
    ''' <summary>
    ''' [�R�[�h����i�A���p�b�N�^BCD�����j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsUnpackedBcdBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim bcd As Integer = src(i)
            If bcd > 9 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h�ϊ��i����Integer��ASCII�`��10�i�����j"
    ''' <summary>
    ''' [�R�[�h�ϊ��i����Integer��ASCII�`��10�i�����j]
    ''' </summary>
    ''' <param name="src">�ϊ���Integer�l</param>
    ''' <param name="dst">�������ݐ�Byte�z��</param>
    ''' <param name="pos">�������ݐ�Byte�z����̏������݈ʒu</param>
    ''' <param name="len">��������Byte��</param>
    Public Shared Sub CopyIntToDecimalAsciiBytes(ByVal src As Integer, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        pos = pos + len
        For i As Integer = 1 To len
            Dim nextSrc As Integer = src \ 10
            dst(pos - i) = CByte(&H30 + (src - nextSrc * 10))
            src = nextSrc
        Next
    End Sub
#End Region

#Region "�R�[�h�ϊ��iASCII�`��10�i����������Integer�j"
    ''' <summary>
    ''' [�R�[�h�ϊ��iASCII�`��10�i����������Integer�j]
    ''' </summary>
    ''' <param name="src">�ϊ���Byte�z��</param>
    ''' <param name="pos">�ϊ���Byte�z����̎擾�ʒu</param>
    ''' <param name="len">�ϊ���Byte��</param>
    ''' <returns>�ϊ���Integer</returns>
    Public Shared Function GetIntFromDecimalAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            ret = ret * 10 + (src(i) - &H30)
        Next
        Return ret
    End Function
#End Region

#Region "�R�[�h����iASCII�`��10�i���� �㔼�k���������e�j"
    ''' <summary>
    ''' [�R�[�h����iASCII�`��10�i���� �㔼�k���������e�j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsDecimalAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c <> &H00 Then Return False  '�����ł��k�������ł��Ȃ��ꍇ
            If i = pos Then Return False  '�擪�̕������k�������̏ꍇ

            '�k���������o�����Ĉȍ~�́A���L�̏����Ŕ��肷��B
            For j As Integer = i + 1 To (pos + len - 1)
                If src(j) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h����iASCII�`��10�i���� �k�������s���j"
    ''' <summary>
    ''' [�R�[�h����iASCII�`��10�i���� �k�������s���j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsDecimalAsciiBytesFixed(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c < &H30 OrElse c > &H39 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h����iASCII�`��16�i���� �㔼�k���������e�j"
    ''' <summary>
    ''' [�R�[�h����iASCII�`��16�i���� �㔼�k���������e�j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsHexadecimalAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c >= &H41 AndAlso c <= &H46 Then Continue For
            If c >= &H61 AndAlso c <= &H66 Then Continue For
            If c <> &H00 Then Return False  '16�i�����ł��k�������ł��Ȃ��ꍇ
            If i = pos Then Return False  '�擪�̕������k�������̏ꍇ

            '�k���������o�����Ĉȍ~�́A���L�̏����Ŕ��肷��B
            For j As Integer = i + 1 To (pos + len - 1)
                If src(j) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h����iASCII�`��16�i���� �k�������s���j"
    ''' <summary>
    ''' [�R�[�h����iASCII�`��16�i���� �k�������s���j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsHexadecimalAsciiBytesFixed(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c >= &H41 AndAlso c <= &H46 Then Continue For
            If c >= &H61 AndAlso c <= &H66 Then Continue For
            Return False
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h����iASCII�`���������� �㔼�k���������e�j"
    ''' <summary>
    ''' [�R�[�h����iASCII�`���������� �㔼�k���������e�j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsVisibleAsciiBytes(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c >= &H20 AndAlso c <= &H7E Then Continue For
            If c <> &H00 Then Return False  '�������ł��k�������ł��Ȃ��ꍇ

            '�k���������o�����Ĉȍ~�́A���L�̏����Ŕ��肷��B
            For j As Integer = i + 1 To (pos + len - 1)
                If src(j) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h����iASCII�`���������� �k�������s���j"
    ''' <summary>
    ''' [�R�[�h����iASCII�`���������� �k�������s���j]
    ''' </summary>
    ''' <param name="src">����Byte�z��</param>
    ''' <param name="pos">����Byte�z����̔���J�n�ʒu</param>
    ''' <param name="len">����Byte��</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsVisibleAsciiBytesFixed(ByVal src As Byte(), ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Byte = src(i)
            If c < &H20 OrElse c > &H7E Then Return False
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h�ϊ��iString�^10�i����������Integer�j"
    'NOTE: String.Substring()���q�[�v�̑��쓙���s��Ȃ��i�����j�Ȃ炢��Ȃ��B
    ''' <summary>
    ''' [�R�[�h�ϊ��iString�^10�i����������Integer�j]
    ''' </summary>
    ''' <param name="src">�ϊ���String</param>
    ''' <param name="pos">�ϊ���String���̎擾�ʒu</param>
    ''' <param name="len">�ϊ���������</param>
    ''' <returns>�ϊ���Integer</returns>
    Public Shared Function GetIntFromDecimalString(ByVal src As String, ByVal pos As Integer, ByVal len As Integer) As Integer
        Dim ret As Integer = 0
        For i As Integer = pos To (pos + len - 1)
            ret = ret * 10 + Val(src.Chars(i))
        Next
        Return ret
    End Function
#End Region

#Region "�R�[�h����iString�^10�i���� �㔼�k���������e�j"
    ''' <summary>
    ''' [�R�[�h����iString�^10�i���� �㔼�k���������e�j]
    ''' </summary>
    ''' <param name="src">����String</param>
    ''' <param name="pos">����String���̔���J�n�ʒu</param>
    ''' <param name="len">���蕶����</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsDecimalString(ByVal src As String, ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Integer = AscW(src.Chars(i))
            If c >= &H30 AndAlso c <= &H39 Then Continue For
            If c <> &H00 Then Return False '�����ł��k�������ł��Ȃ��ꍇ
            If i = pos Then Return False '�擪�̕������k�������̏ꍇ

            '�k���������o�����Ĉȍ~�́A���L�̏����Ŕ��肷��B
            For j As Integer = i + 1 To (pos + len - 1)
                If AscW(src.Chars(j)) <> &H00 Then Return False
            Next
            Return True
        Next
        Return True
    End Function
#End Region

#Region "�R�[�h����iString�^10�i���� �k�������s���j"
    ''' <summary>
    ''' [�R�[�h����iString�^10�i���� �k�������s���j]
    ''' </summary>
    ''' <param name="src">����String</param>
    ''' <param name="pos">����String����̔���J�n�ʒu</param>
    ''' <param name="len">���蕶����</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsDecimalStringFixed(ByVal src As String, ByVal pos As Integer, ByVal len As Integer) As Boolean
        For i As Integer = pos To (pos + len - 1)
            Dim c As Integer = AscW(src.Chars(i))
            If c < &H30 OrElse c > &H39 Then Return False
        Next
        Return True
    End Function
#End Region

#Region "�t�B��"
    ''' <summary>
    ''' [�t�B��]
    ''' </summary>
    ''' <param name="val">�t�B���l</param>
    ''' <param name="dst">�t�B���Ώ�Byte�z��</param>
    ''' <param name="pos">�t�B���J�n�ʒu</param>
    ''' <param name="len">�t�B��Byte��</param>
    Public Shared Sub FillBytes(ByVal val As Byte, ByVal dst As Byte(), ByVal pos As Integer, ByVal len As Integer)
        For i As Integer = pos To (pos + len - 1)
            dst(i) = val
        Next
    End Sub
#End Region

#Region "�o�C�g�I�[�_�ۏ�BIN�ϊ�"
    Public Shared Function GetUInt32FromLeBytes4(ByVal aBytes As Byte(), ByVal offset As Integer) As UInteger
        Return CUInt(aBytes(offset)) Or CUInt(aBytes(offset + 1)) << 8 Or CUInt(aBytes(offset + 2)) << 16 Or CUInt(aBytes(offset + 3)) << 24
    End Function

    Public Shared Function GetUInt32FromLeBytes3(ByVal aBytes As Byte(), ByVal offset As Integer) As UInteger
        Return CUInt(aBytes(offset)) Or CUInt(aBytes(offset + 1)) << 8 Or CUInt(aBytes(offset + 2)) << 16
    End Function

    Public Shared Function GetUInt16FromLeBytes2(ByVal aBytes As Byte(), ByVal offset As Integer) As UShort
        Return CUShort(aBytes(offset)) Or CUShort(aBytes(offset + 1)) << 8
    End Function

    Public Shared Sub CopyUInt32ToLeBytes4(ByVal src As UInteger, ByVal aBytes As Byte(), ByVal offset As Integer)
        aBytes(offset) = CByte(src And &HFF)
        aBytes(offset + 1) = CByte(src >> 8 And &HFF)
        aBytes(offset + 2) = CByte(src >> 16 And &HFF)
        aBytes(offset + 3) = CByte(src >> 24 And &HFF)
    End Sub

    Public Shared Sub CopyUInt16ToLeBytes2(ByVal src As UShort, ByVal aBytes As Byte(), ByVal offset As Integer)
        aBytes(offset) = CByte(src And &Hff)
        aBytes(offset + 1) = CByte(src >> 8 And &Hff)
    End Sub
#End Region

#Region "CRC-16�Z�o"
    Public Shared Function CalculateCRC16(ByVal buf As Byte(), ByVal pos As Integer, ByVal len As Integer) As UShort
        Const polynomial As UInteger = &H1800500
        Dim sum As UInteger = 0

        For i As Integer = pos To (pos + len - 1)
            sum = sum Or buf(i)
            For j As Integer = 0 To 7
                sum = sum << 1
                If (sum And &H1000000) <> 0 Then
                    sum = sum Xor polynomial
                End If
            Next
        Next

        For i As Integer = 0 To 1  'last 2 byte zero
            sum = CUInt(sum Or &H00)
            For j As Integer = 0 To 7
                sum = sum << 1
                If (sum And &H1000000) <> 0 Then
                    sum = sum Xor polynomial
                End If
            Next
        Next

        Return CUShort((sum >> 8) And &H0000ffff)
    End Function
#End Region

#Region "MD5�Z�o"
    'NOTE: �t�@�C�����J���Ȃ��܂��͓ǂ߂Ȃ��ꍇ�́A���炩�̗�O���X���[���܂��B
    Public Shared Function CalculateMD5(ByVal sFilePath As String) As String
        Dim aHashValue As Byte()
        Using oStream As New System.IO.FileStream(sFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read)
            Dim oHasher As New System.Security.Cryptography.MD5CryptoServiceProvider()
            aHashValue = oHasher.ComputeHash(oStream)
        End Using
        Return System.BitConverter.ToString(aHashValue).Replace("-", "")
    End Function
#End Region

#Region "C���ꃊ�e�����`��������̖|��i���s�R�[�h�C�Ӂj"
    ''' <summary>
    ''' [C���ꃊ�e�����`��������̖|��i���s�R�[�h�C�Ӂj]
    ''' </summary>
    ''' <param name="sLiteral">C���ꃊ�e�����`��������</param>
    ''' <returns>�|���̕�����</returns>
    Public Shared Function TranslateClangLiteral(ByVal sLiteral As String) As String
        Dim lastIndex As Integer = sLiteral.Length - 1
        Dim i As Integer = 0
        Dim aDst(lastIndex) As Char
        Dim dstIndex As Integer = 0
        While i <= lastIndex
            Dim c As Char = sLiteral.Chars(i)
            If c.Equals("\"c) Then
                i += 1
                If i > lastIndex Then
                    Throw New ArgumentException("The argument contains bad escape sequence (solitary back slash).")
                End If
                Dim c2 As Char = sLiteral.Chars(i)
                If c2.Equals("\"c) Then
                    aDst(dstIndex) = "\"c
                ElseIf c2.Equals("n"c) Then
                    aDst(dstIndex) = ControlChars.Lf
                ElseIf c2.Equals("r"c) Then
                    aDst(dstIndex) = ControlChars.Cr
                Else
                    Throw New ArgumentException("The argument contains indecipherable escape sequence.")
                End If
            Else
                aDst(dstIndex) = c
            End If
            dstIndex += 1
            i += 1
        End While
        Return New String(aDst, 0, dstIndex)
    End Function
#End Region

#Region "C���ꃊ�e�����`��������̖|��i���s�R�[�hCRLF�Œ�j"
    ''' <summary>
    ''' [C���ꃊ�e�����`��������̖|��i���s�R�[�hCRLF�Œ�j]
    ''' </summary>
    ''' <param name="sLiteral">C���ꃊ�e�����`��������</param>
    ''' <returns>�|���̕�����</returns>
    Public Shared Function TranslateClangLiteralToDosText(ByVal sLiteral As String) As String
        Dim lastIndex As Integer = sLiteral.Length - 1
        Dim i As Integer = 0
        Dim aDst(lastIndex) As Char
        Dim dstIndex As Integer = 0
        While i <= lastIndex
            Dim c As Char = sLiteral.Chars(i)
            If c.Equals("\"c) Then
                i += 1
                If i > lastIndex Then
                    Throw New ArgumentException("The argument contains bad escape sequence (solitary back slash).")
                End If
                Dim c2 As Char = sLiteral.Chars(i)
                If c2.Equals("\"c) Then
                    aDst(dstIndex) = "\"c
                ElseIf c2.Equals("n"c) Then
                    aDst(dstIndex) = ControlChars.Cr
                    dstIndex += 1
                    aDst(dstIndex) = ControlChars.Lf
                Else
                    Throw New ArgumentException("The argument contains indecipherable escape sequence.")
                End If
            Else
                aDst(dstIndex) = c
            End If
            dstIndex += 1
            i += 1
        End While
        Return New String(aDst, 0, dstIndex)
    End Function
#End Region

#Region "�t�@�C���p�X�֘A"
    ''' <summary>
    ''' [���z�p�X�̗L�����𔻒�]
    ''' </summary>
    ''' <param name="sVirtualPath">���z�p�X</param>
    ''' <returns>���茋��</returns>
    ''' <remarks>
    ''' ���z�p�X�Ƃ́A���z�I�ȃt�@�C���V�X�e���ɂ������΂܂��͑��΃p�X�̂��Ƃł���B
    ''' </remarks>
    Public Shared Function IsValidVirtualPath(ByVal sVirtualPath As String) As Boolean
        '�h���C�u�w�肪����p�X�́A���[�J���t�@�C���V�X�e���̃p�X��
        '�����ł��Ȃ��̂ŁA���z�p�X�Ƃ��Ė����Ƃ݂Ȃ��B
        If sVirtualPath.Contains(":") Then
            Return False
        End If

        '�u\\�v��u//�v�ł͂��܂�p�X�́A���[�J���t�@�C���V�X�e���̃p�X��
        '�����ł��Ȃ��i��������Ƃ��ɁA�ŏ��́u\�v���Ƃ��Ă��A��΃p�X��
        '�݂��Ă��܂��A���ł���j�̂ŁA���z�p�X�Ƃ��Ė����Ƃ݂Ȃ��B
        If sVirtualPath.Length >= 2 Then
            If sVirtualPath.Chars(1) = System.IO.Path.DirectorySeparatorChar OrElse _
               sVirtualPath.Chars(1) = System.IO.Path.AltDirectorySeparatorChar Then
                Return False
            End If
        End If

        '�p�X��ʂƂ��Ė����ȃp�X�́A���z�p�X�Ƃ��Ă������Ƃ݂Ȃ��B
        Try
            System.IO.Path.GetDirectoryName(sVirtualPath)
        Catch ex As Exception
            Return False
        End Try

        '�u\..�v���܂ރp�X�́A�e�f�B���N�g���i���z�t�@�C���V�X�e���̊O���j��
        '�w�����˂Ȃ��Ƃ����Ӗ��ŕs���ȉ\���͂��邪�A�����͉\�Ȃ̂ŁA
        '���z�p�X�Ƃ��Ă������Ƃ݂͂Ȃ��Ȃ��B
        'NOTE: �A�N�Z�X���ꂽ���Ȃ��f�B���N�g�����w���Ă��Ȃ����́A
        'IsAncestPath(�A�N�Z�X���p�X, ������̃p�X)�Ń`�F�b�N���邱�ƁB

        Return True
    End Function

    ''' <summary>
    ''' [���z�p�X���p�X�Ɍ���]
    ''' </summary>
    ''' <param name="sPath">�p�X</param>
    ''' <param name="sVirtualPath">���z�p�X</param>
    ''' <returns>���������p�X</returns>
    ''' <remarks>
    ''' ���z�p�X�Ƃ́A���z�I�ȃt�@�C���V�X�e���ɂ������΂܂��͑��΃p�X�̂��Ƃł���B
    ''' ���z�I�ȃt�@�C���V�X�e���ɂ�������̂ł����Ă��A���΃p�X�ł��邱�Ƃ��m���ȏꍇ�́A
    ''' ���̃��\�b�h���g���K�v�͂Ȃ��ASystem.IO.Path.Combine()���g���΂悢�B
    ''' �߂�l�̗p�r�ɂ���ẮAsVirtualPath���L���ȉ��z�p�X�ł��邱�Ƃ�
    ''' IsValidVirtualPath()�Ń`�F�b�N���Ă������Ƃ𐄏�����B
    ''' sVirtualPath���L���ȉ��z�p�X�łȂ��ꍇ�A���̃��\�b�h�̖߂�l��
    ''' sVirtualPath�ɂȂ�댯��������B
    ''' </remarks>
    Public Shared Function CombinePathWithVirtualPath(ByVal sPath As String, ByVal sVirtualPath As String) As String
        If sVirtualPath.Chars(0) = System.IO.Path.DirectorySeparatorChar OrElse _
           sVirtualPath.Chars(0) = System.IO.Path.AltDirectorySeparatorChar Then
            sVirtualPath = sVirtualPath.Remove(0, 1)
        End If
        Return System.IO.Path.Combine(sPath, sVirtualPath)
    End Function

    ''' <summary>
    ''' [�e�܂��͐�c�f�B���N�g��������]
    ''' </summary>
    ''' <param name="sSuperPath">����Ώۃp�X�i���K���ς݃t���p�X�j</param>
    ''' <param name="sSubPath">��r�Ώۃp�X�i���K���ς݃t���p�X�j</param>
    ''' <returns>���茋��</returns>
    Public Shared Function IsAncestPath(ByVal sSuperPath As String, ByVal sSubPath As String) As Boolean
        Dim i As Integer = 0
        Dim max As Integer = (sSubPath.Length + 1) \ 2
        sSuperPath = sSuperPath.ToUpperInvariant()
        sSubPath = sSubPath.ToUpperInvariant()
        Try
            sSubPath = System.IO.Path.GetDirectoryName(sSubPath)
            While sSubPath IsNot Nothing AndAlso i < max
                If sSubPath.Equals(sSuperPath) Then Return True
                sSubPath = System.IO.Path.GetDirectoryName(sSubPath)
            End While
        Catch ex As System.IO.IOException
            Return False
        Catch ex As ArgumentException
            Return False
        End Try
        Return False
    End Function
#End Region

#Region "�w��f�B���N�g�����̑S�T�u�f�B���N�g���E�S�t�@�C���Ɏw�葮����ǉ�"
    Public Shared Sub AddAttributesToDirectoryDescendants(ByVal sDirPath As String, ByVal attrs As System.IO.FileAttributes)
        Dim aSubDirs As String() = System.IO.Directory.GetDirectories(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sSubDir As String In aSubDirs
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sSubDir)
            Dim newAttrs As System.IO.FileAttributes = curAttrs Or attrs
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sSubDir, newAttrs)
            End If
        Next sSubDir

        Dim aFiles As String() = System.IO.Directory.GetFiles(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sFile As String In aFiles
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sFile)
            Dim newAttrs As System.IO.FileAttributes = curAttrs Or attrs
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sFile, newAttrs)
            End If
        Next sFile
    End Sub
#End Region

#Region "�w��f�B���N�g�����̑S�T�u�f�B���N�g���E�S�t�@�C������w�葮��������"
    Public Shared Sub RemoveAttributesFromDirectoryDescendants(ByVal sDirPath As String, ByVal attrs As System.IO.FileAttributes)
        Dim aDirs As String() = System.IO.Directory.GetDirectories(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sDir As String In aDirs
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sDir)
            Dim newAttrs As System.IO.FileAttributes = curAttrs And (Not attrs)
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sDir, newAttrs)
            End If
        Next sDir

        Dim aFiles As String() = System.IO.Directory.GetFiles(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sFile As String In aFiles
            Dim curAttrs As System.IO.FileAttributes = System.IO.File.GetAttributes(sFile)
            Dim newAttrs As System.IO.FileAttributes = curAttrs And (Not attrs)
            If newAttrs <> curAttrs Then
                System.IO.File.SetAttributes(sFile, newAttrs)
            End If
        Next sFile
    End Sub
#End Region

#Region "�ꎞ��Ɨp�f�B���N�g���̍폜"
    ''' <summary>
    ''' [�ꎞ��Ɨp�f�B���N�g���̍폜]
    ''' </summary>
    ''' <param name="sDirPath">�f�B���N�g���̃p�X</param>
    ''' <remarks>
    ''' �w�肳�ꂽ�f�B���N�g�������݂��Ȃ��ꍇ�͉������Ȃ��B
    ''' �f�B���N�g�����̃A�C�e���ɓǂݎ���p�������t�^����Ă���ꍇ��
    ''' �܂Ƃ߂č폜����B
    ''' �f�B���N�g����f�B���N�g�����̃A�C�e�����쐬�E�폜�E�ύX����̂́A
    ''' ���݂̃v���Z�X�̌Ăь��̃X���b�h�i�܂��͂���ƃV�[�P���V������
    ''' ���삷��X���b�h�j�݂̂ł��邱�Ƃ��O��ł���B
    ''' ���ꂪ����Ȃ��ꍇ��A�f�B���N�g���Ɠ����̃t�@�C�������݂��Ă���
    ''' �ꍇ��A�w��̃f�B���N�g�����̂ɓǂݎ���p�������t�^����Ă���
    ''' �ꍇ�́A���炩�̗�O�X���[������B
    ''' </remarks>
    Public Shared Sub DeleteTemporalDirectory(ByVal sDirPath As String)
        If System.IO.Directory.Exists(sDirPath) Then
            RemoveAttributesFromDirectoryDescendants(sDirPath, System.IO.FileAttributes.ReadOnly)
            System.IO.Directory.Delete(sDirPath, True)
        End If
    End Sub
#End Region

#Region "�f�B���N�g���̏�����"
    ''' <summary>
    ''' [�f�B���N�g���̏�����]
    ''' </summary>
    ''' <param name="sDirPath">�f�B���N�g���̃p�X</param>
    ''' <remarks>
    ''' �w�肳�ꂽ�f�B���N�g�����ł������ŋ�ɂ���B
    ''' �������̊֌W�ō폜�ł��Ȃ����̂������Ă��A���̍폜�݂̂���߂āA
    ''' ���̃T�u�f�B���N�g����t�@�C���̍폜�͎��s����B
    ''' �폜�ł��Ȃ����̂�����ꍇ��Log�N���X���g���ċL�^���邽�߁A
    ''' Log�N���X�̃��\�b�h�́A�{���\�b�h���g�p���Ă͂Ȃ�Ȃ��B
    ''' </remarks>
    Public Shared Sub CleanUpDirectory(ByVal sDirPath As String)
        'OPT: �ň��̏ꍇ�Ƀf�B���N�g�����̊K��I�[�_�̏������s�����ƂɂȂ邽�߁A
        '�����ɖ�肪���邩������Ȃ��B�����I�ɖ�肪����Ȃ�A�ċA�Ăяo����
        '���O�ōs���Ăł������ʂ��ŏ�������ׂ��ł���B

        Dim aSubDirs As String() = System.IO.Directory.GetDirectories(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sSubDir As String In aSubDirs
            If System.IO.Directory.Exists(sSubDir) Then
                Try
                    System.IO.Directory.Delete(sSubDir, True)
                Catch ex As Exception
                    'NOTE: �폜�ł��Ȃ��t�@�C����폜�ł��Ȃ��T�u�f�B���N�g�����P�ł�����΁A
                    'sSubDir�����ɖ������́i�폜�����݂Ă��Ȃ��j�t�@�C����T�u�f�B���N�g����
                    '�c�����܂܎���sSubDir�̏����Ɉڍs���邱�ƂɂȂ�B
                    '�������A�������̃T�u�f�B���N�g���ō폜�ł�����̂́A���̃��[�v�̎��ȍ~��
                    'sSubDir�̏����ō폜���邱�ƂɂȂ�B�܂��A�������̃t�@�C���ō폜�ł������
                    '�́A���̌��sFile�̃��[�v�ō폜���邱�ƂɂȂ�B
                    '���ʂƂ��āA�c��̂́A���ꎩ�̂��폜�ł��Ȃ��悤�ɂȂ��Ă���t�@�C����
                    '�f�B���N�g���ƁA�������i�[�����ŕK�v�ȍŏ����̃f�B���N�g�������ł���B
                End Try
            End If
        Next sSubDir

        Dim aFiles As String() = System.IO.Directory.GetFiles(sDirPath, "*", System.IO.SearchOption.AllDirectories)
        For Each sFile As String In aFiles
            Try
                System.IO.File.Delete(sFile)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        Next sFile
    End Sub
#End Region

End Class
