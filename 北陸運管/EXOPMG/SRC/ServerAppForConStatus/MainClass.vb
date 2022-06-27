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
Imports System.Threading
Imports System.Text

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' �@��ڑ���ԓo�^�v���Z�X�̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "�錾�̈�iPrivate�j"

    ''' <summary>
    ''' �e�[�u����
    ''' </summary>
    Private Const ConStatus_TableName As String = "D_CON_STATUS"

    ''' <summary>
    ''' ���D�@�f�[�^���
    ''' </summary>
    Private Const DataKind_G As String = "55"

    ''' <summary>
    ''' ���������@�f�[�^���
    ''' </summary>
    Private Const DataKind_Y As String = "89"

#End Region

#Region "Main"

    ''' <summary>
    ''' �@��ڑ���ԓo�^�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �@��ڑ���ԓo�^�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForConStatus")
        If m.WaitOne(0, False) Then
            Try
                Dim sLogBasePath As String = Constant.GetEnv(REG_LOG)
                If sLogBasePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_LOG)
                    Return
                End If

                Dim sIniFilePath As String = Constant.GetEnv(REG_SERVER_INI)
                If sIniFilePath Is Nothing Then
                    AlertBox.Show(Lexis.EnvVarNotFound, REG_SERVER_INI)
                    Return
                End If

                Log.Init(sLogBasePath, "ForConStatus")
                Log.Info("�v���Z�X�J�n")

                Try
                    Lexis.Init(sIniFilePath)
                    Config.Init(sIniFilePath)
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End Try

                Log.SetKindsMask(Config.LogKindsMask)

                RecServerAppBaseMain(AddressOf RecordToDatabase)

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            Finally
                Config.Dispose()
                Log.Info("�v���Z�X�I��")

                'NOTE: ������ʂ�Ȃ��Ă��A���̃X���b�h�̏��łƂƂ��ɉ�������
                '�悤�Ȃ̂ŁA�ň��̐S�z�͂Ȃ��B
                m.ReleaseMutex()

                Application.Exit()
            End Try
        End If
    End Sub

#End Region

#Region "Private"

    ''' <summary>
    ''' �@��ڑ���ԓo�^�����B
    ''' </summary>
    ''' <param name="sFilePath">�o�^����ׂ��f�[�^���i�[���ꂽ�t�@�C���̐�΃p�X��</param>
    ''' <returns>�o�^�̌���</returns>
    ''' <remarks>
    ''' �f�[�^�o�^�X���b�h�ŌĂяo�����B
    ''' </remarks>
    Private Shared Function RecordToDatabase(ByVal sFilePath As String) As RecordingResult

        Dim defineInfo() As RecDataStructure.DefineInfo = Nothing '��`���
        Dim lstData As New List(Of String())                        '�f�[�^���
        Dim lstDataNew As New List(Of String())                     '���������f�[�^���

        Dim dataKind(0) As Byte
        Dim fileNameInfo As RecDataStructure.BaseInfo = Nothing
        Dim code As EkCode

        Try
            '�f�[�^��ʂ��擾
            Using fs As New FileStream(sFilePath, FileMode.Open)
                fs.Read(dataKind, 0, 1)
            End Using

            If Hex(dataKind(0)) = DataKind_G Then  '���D�@�ڑ���ԊĎ�
                fileNameInfo = Nothing
                '�t�@�C��������͂���
                code = UpboundDataPath.GetEkCode(sFilePath)
                '�t�@�C�����������擾
                fileNameInfo.STATION_CODE.RAIL_SECTION_CODE = code.RailSection.ToString("D3")
                '�t�@�C��������w���擾
                fileNameInfo.STATION_CODE.STATION_ORDER_CODE = code.StationOrder.ToString("D3")
                '�t�@�C��������R�[�i�[�擾
                fileNameInfo.CORNER_CODE = code.Corner.ToString("D4")
                '�t�@�C����������W�����擾 
                fileNameInfo.PROCESSING_TIME = UpboundDataPath.GetTimestamp(sFilePath).ToString()

                'OPT: defineInfo���R�p�ӂ��āAGetDefineInfo��Main���\�b�h�ɂ�
                '��x�����s�������悢���A�R��ނ̐ݒ�l������Ƃ����ȑO��
                'defineInfo��Immutable�łȂ����߁A�������Ȃ����Ă���Ƃ���
                '�b������A�Ή�����Ȃ璍�ӂ��Ȃ���΂Ȃ�Ȃ��B

                '��`�����擾����B
                defineInfo = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath_G, "ConStatus", defineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If

                '�f�[�^���擾����B
                lstData = New List(Of String())
                If MainClass.GetInfoFromDataFile(defineInfo, sFilePath, DataKind_G, lstData, fileNameInfo) = False Then
                    Return RecordingResult.ParseError
                End If

                '�`�F�b�N���s���B
                lstDataNew = New List(Of String())
                If MainClass.CheckData(defineInfo, lstData, lstDataNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If

                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(defineInfo, lstDataNew, ConStatus_TableName) = False Then
                    Return RecordingResult.IOError
                End If
            ElseIf Hex(dataKind(0)) = DataKind_Y Then     '���������@�ڑ���ԊĎ�
                fileNameInfo = Nothing
                '�t�@�C����������W�����擾
                fileNameInfo.PROCESSING_TIME = UpboundDataPath.GetTimestamp(sFilePath).ToString()

                '��`�����擾����B
                defineInfo = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath_Y, "ConStatus", defineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If
                '�f�[�^���擾����B
                lstData = New List(Of String())
                If MainClass.GetInfoFromDataFile(defineInfo, sFilePath, DataKind_Y, lstData, fileNameInfo) = False Then
                    Return RecordingResult.ParseError
                End If

                '�`�F�b�N
                lstDataNew = New List(Of String())
                If MainClass.CheckData(defineInfo, lstData, lstDataNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If

                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(defineInfo, lstDataNew, ConStatus_TableName) = False Then
                    Return RecordingResult.IOError
                End If

                '�@�킪X�ꍇ
                '��`�����擾����B
                defineInfo = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath_X, "ConStatus", defineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If
                '�f�[�^���擾����B
                lstData = New List(Of String())
                If MainClass.GetInfoFromFileName(defineInfo, sFilePath, lstData) = False Then
                    Return RecordingResult.ParseError
                End If

                '�`�F�b�N
                lstDataNew = New List(Of String())
                If MainClass.CheckData(defineInfo, lstData, lstDataNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If

                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(defineInfo, lstDataNew, ConStatus_TableName) = False Then
                    Return RecordingResult.IOError
                End If
            Else
                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, 1, "�f�[�^���"))
                '���W�f�[�^�̓o�^
                BatchAppComm.SetCollectionData(sFilePath, DataKind_G)
            End If

            '�o�^�����������ꍇ
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Hex(dataKind(0)), Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' ���������@�ڑ���ԊĎ��A�@�킪X�ꍇ�A�f�[�^�̉��
    ''' </summary>
    ''' <param name="iniInfoAry">INI�t�@�C�����e</param>
    ''' <param name="fileName">�f�[�^�t�@�C����</param>
    ''' <param name="dlineInfoLst">�f�[�^���X�g</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�擾�����d���t�H�[�}�b�g��`���ɂċ@��ڑ���ԃf�[�^����͂���</remarks>
    Private Shared Function GetInfoFromFileName(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByVal fileName As String, _
                                                ByRef dlineInfoLst As List(Of String())) As Boolean
        Dim bRtn As Boolean = False
        Dim nHeadSize_Y As Integer = 4
        Dim fs As FileStream = Nothing
        Dim bData() As Byte
        Dim j As Integer
        Dim dataInfo() As String
        Dim code As EkCode
        Dim sDataKind As String = "89" '�f�[�^���

        '�w�b�h��
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        Try
            fs = New FileStream(fileName, FileMode.Open)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return bRtn
        End Try

        Try
            If dlineInfoLst Is Nothing Then
                dlineInfoLst = New List(Of String())
            Else
                dlineInfoLst.Clear()
            End If

            ReDim bData(nHeadSize_Y)

            '�o�C�i���f�[�^�擾
            fs.Read(bData, 0, nHeadSize_Y)

            ReDim dataInfo(iniInfoAry.Length - 1)

            headInfo.DATA_KIND = "89"
            '�@��
            headInfo.MODEL_CODE = "X"

            code = UpboundDataPath.GetEkCode(fileName)
            For j = 0 To iniInfoAry.Length - 1
                '�w�b�h�ꍇ
                Select Case UCase(iniInfoAry(j).FIELD_NAME)
                    Case "MODEL_CODE" '�@��
                        dataInfo(j) = headInfo.MODEL_CODE
                    Case "RAIL_SECTION_CODE"    '�T�C�o�l����R�[�h
                        dataInfo(j) = code.RailSection.ToString("D3")
                        headInfo.STATION_CODE.RAIL_SECTION_CODE = dataInfo(j)
                    Case "STATION_ORDER_CODE"   '�T�C�o�l�w���R�[�h
                        dataInfo(j) = code.StationOrder.ToString("D3")
                        headInfo.STATION_CODE.STATION_ORDER_CODE = dataInfo(j)
                    Case "CORNER_CODE"  '�R�[�i�[�R�[�h
                        dataInfo(j) = code.Corner.ToString("D4")
                        headInfo.CORNER_CODE = dataInfo(j)
                    Case "SYUSYU_DATE"  '���W����
                        dataInfo(j) = UpboundDataPath.GetTimestamp(fileName).ToString
                    Case "IDCENTERCONNECT"  '����
                        dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET).ToString
                    Case "UNIT_NO"
                        dataInfo(j) = code.Unit.ToString()
                        headInfo.UNIT_NO = CInt(dataInfo(j))
                End Select
            Next
            dlineInfoLst.Add(dataInfo)

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(sDataKind, Path.GetFileNameWithoutExtension(fileName)))

            Return bRtn
        Finally
            fs.Close()
        End Try

        Return bRtn
    End Function

    ''' <summary>
    ''' �@��ڑ���ԃf�[�^�̉��
    ''' </summary>
    ''' <param name="iniInfoAry">INI�t�@�C�����e</param>
    ''' <param name="fileName">�f�[�^�t�@�C����</param>
    ''' <param name="sDataKind">�f�[�^���</param>
    ''' <param name="dlineInfoLst">�f�[�^���X�g</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�擾�����d���t�H�[�}�b�g��`���ɂċ@��ڑ���ԃf�[�^����͂���</remarks>
    Private Shared Function GetInfoFromDataFile(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByVal fileName As String, _
                                                ByVal sDataKind As String, _
                                                ByRef dlineInfoLst As List(Of String()), _
                                                ByVal fileNameInfo As RecDataStructure.BaseInfo) As Boolean
        Dim bRtn As Boolean = False
        Dim nHeadSize_Y As Integer = 4
        Dim nDataSize_Y As Integer = 15
        Dim nDataSize_G As Integer = 113
        Dim fs As FileStream = Nothing
        Dim bData() As Byte
        Dim i As Integer
        Dim j As Integer
        Dim dataInfo() As String
        Dim iDataCnt As Integer = 0
        Dim strCode As String = ""

        '�w�b�h��
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        Try
            fs = New FileStream(fileName, FileMode.Open)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return bRtn
        End Try

        Try

            If dlineInfoLst Is Nothing Then
                dlineInfoLst = New List(Of String())
            Else
                dlineInfoLst.Clear()
            End If

            If sDataKind = DataKind_G Then     '���D�@�ڑ���ԊĎ�

                ReDim bData(nDataSize_G)
                '�o�C�i���f�[�^�擾
                fs.Read(bData, 0, nDataSize_G)

                iDataCnt = 16

                For i = 0 To iDataCnt - 1
                    ReDim dataInfo(iniInfoAry.Length - 1)
                    headInfo = Nothing
                    headInfo.STATION_CODE.RAIL_SECTION_CODE = fileNameInfo.STATION_CODE.RAIL_SECTION_CODE
                    headInfo.STATION_CODE.STATION_ORDER_CODE = fileNameInfo.STATION_CODE.STATION_ORDER_CODE
                    headInfo.CORNER_CODE = fileNameInfo.CORNER_CODE
                    headInfo.MODEL_CODE = "G"

                    For j = 0 To iniInfoAry.Length - 1
                        '�w�b�h�ꍇ()
                        Select Case UCase(iniInfoAry(j).FIELD_NAME)
                            Case "DATA_KIND" '�f�[�^���
                                dataInfo(j) = Hex(bData(iniInfoAry(j).BYTE_OFFSET))
                                headInfo.DATA_KIND = dataInfo(j)
                            Case "MODEL_CODE" '�@��
                                dataInfo(j) = headInfo.MODEL_CODE
                            Case "RAIL_SECTION_CODE"  '�T�C�o�l����R�[�h
                                dataInfo(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE
                            Case "STATION_ORDER_CODE"  '�T�C�o�l�w���R�[�h
                                dataInfo(j) = headInfo.STATION_CODE.STATION_ORDER_CODE
                            Case "CORNER_CODE"  '�R�[�i�[�R�[�h
                                dataInfo(j) = headInfo.CORNER_CODE
                            Case "SYUSYU_DATE"  '���W����
                                dataInfo(j) = fileNameInfo.PROCESSING_TIME
                            Case "UNIT_NO"
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i).ToString
                                headInfo.UNIT_NO = CInt(dataInfo(j))
                            Case Else
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i).ToString
                        End Select
                    Next

                    dlineInfoLst.Add(dataInfo)
                Next

            ElseIf sDataKind = DataKind_Y Then    '���������@�ڑ���ԊĎ�
                Dim isChkErr As Boolean = False

                '1���R�[�h�̃f�[�^�𐔎擾
                iDataCnt = CInt(Int((fs.Length - nHeadSize_Y) / nDataSize_Y))

                ReDim bData(nDataSize_Y * iDataCnt + nHeadSize_Y)

                '�o�C�i���f�[�^�擾
                fs.Read(bData, 0, nDataSize_Y * iDataCnt + nHeadSize_Y)

                For i = 0 To iDataCnt - 1
                    ReDim dataInfo(iniInfoAry.Length - 1)

                    isChkErr = False
                    headInfo = Nothing
                    '�@��
                    headInfo.MODEL_CODE = "Y"

                    For j = 0 To iniInfoAry.Length - 1
                        '�w�b�h�ꍇ
                        Select Case UCase(iniInfoAry(j).FIELD_NAME)
                            Case "DATA_KIND" '�f�[�^���
                                dataInfo(j) = Hex(bData(iniInfoAry(j).BYTE_OFFSET))
                                headInfo.DATA_KIND = dataInfo(j)
                                Continue For
                            Case "MODEL_CODE" '�@��
                                dataInfo(j) = headInfo.MODEL_CODE
                                Continue For
                            Case "RAIL_SECTION_CODE"    '�T�C�o�l����R�[�h
                                strCode = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y, _
                                                                                iniInfoAry(j).BYTE_LEN)
                                '�����`�F�b�N���s��
                                If OPMGUtility.checkNumber(strCode) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    BatchAppComm.SetCollectionData(fileName, DataKind_Y)
                                    isChkErr = True
                                    Exit For
                                End If
                                dataInfo(j) = Format(CInt(strCode), "000")
                                headInfo.STATION_CODE.RAIL_SECTION_CODE = dataInfo(j)
                                Continue For
                            Case "STATION_ORDER_CODE"   '�T�C�o�l�w���R�[�h
                                strCode = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y, _
                                                                                iniInfoAry(j).BYTE_LEN)
                                '�����`�F�b�N���s��
                                If OPMGUtility.checkNumber(strCode) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    BatchAppComm.SetCollectionData(fileName, DataKind_Y)
                                    isChkErr = True
                                    Exit For
                                End If
                                dataInfo(j) = Format(CInt(strCode), "000")
                                headInfo.STATION_CODE.STATION_ORDER_CODE = dataInfo(j)
                                Continue For
                            Case "CORNER_CODE"  '�R�[�i�[�R�[�h
                                strCode = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y, _
                                                                                iniInfoAry(j).BYTE_LEN)
                                '�����`�F�b�N���s��
                                If OPMGUtility.checkNumber(strCode) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    BatchAppComm.SetCollectionData(fileName, DataKind_Y)
                                    isChkErr = True
                                    Exit For
                                End If
                                dataInfo(j) = Format(CInt(strCode), "0000")
                                headInfo.CORNER_CODE = dataInfo(j)
                                Continue For
                            Case "SYUSYU_DATE"  '���W����
                                dataInfo(j) = fileNameInfo.PROCESSING_TIME
                                Continue For
                            Case "IDCENTERCONNECT"  '����
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET).ToString
                            Case "UNIT_NO"
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y).ToString
                                headInfo.UNIT_NO = CInt(dataInfo(j))
                            Case Else
                                dataInfo(j) = bData(iniInfoAry(j).BYTE_OFFSET + i * nDataSize_Y).ToString
                        End Select
                    Next

                    If isChkErr = False Then
                        dlineInfoLst.Add(dataInfo)
                    End If
                Next
            End If

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(sDataKind, Path.GetFileNameWithoutExtension(fileName)))

            Return bRtn
        Finally
            fs.Close()
        End Try

        Return bRtn
    End Function

    ''' <summary>
    ''' �@��ڑ���ԃf�[�^�̃`�F�b�N
    ''' </summary>
    ''' <param name="iniInfoAry">ini�t�@�C��</param>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dlineInfoLstNew">dat�t�@�C�����e</param>
    ''' <param name="datFileName">�f�[�^�t�@�C����</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>��͏����ɂ��擾�f�[�^���`�F�b�N����</remarks>
    Private Shared Function CheckData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String) As Boolean
        Dim bRtn As Boolean = False
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:�G���[;false:�G���[�Ȃ�
        Dim dataKind As String = ""

        '�@��\���}�X�^SQL
        Dim strSQL As String = "SELECT COUNT(1) FROM V_MACHINE_NOW WHERE RAIL_SECTION_CODE = {0} AND STATION_ORDER_CODE = {1} AND CORNER_CODE = {2} AND MODEL_CODE = {3} AND UNIT_NO = {4}"
        Dim dbCtl As DatabaseTalker = Nothing
        Dim sRail_Code As String = ""
        Dim sStation_Code As String = ""
        Dim sCorner_Code As String = ""
        Dim sModel_Code As String = ""
        Dim sUnit_No As String = ""
        Dim nRtn As Integer
        Dim nFlag_G As Integer = 16 '���@�`�F�b�N�p

        Try
            dlineInfoLstNew = New List(Of String())
            dbCtl = New DatabaseTalker
            dbCtl.ConnectOpen()

            For i = 0 To dlineInfoLst.Count - 1

                isHaveErr = False

                lineInfo = dlineInfoLst.Item(i)

                For j = 0 To iniInfoAry.Length - 1

                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "DATA_KIND"    '�f�[�^���
                            dataKind = lineInfo(j)
                            Continue For
                        Case "RAIL_SECTION_CODE", _
                             "STATION_ORDER_CODE", _
                             "CORNER_CODE"         '�T�C�o�l����R�[�h,�T�C�o�l�w���R�[�h,�R�[�i�[�R�[�h
                            If OPMGUtility.checkNumber(lineInfo(j)) = False Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                '���W�f�[�^�̓o�^
                                BatchAppComm.SetCollectionData(datFileName, dataKind)
                                If dataKind = DataKind_G Then
                                    Return bRtn
                                End If
                                Exit For
                            Else
                                If CLng(lineInfo(j)) = 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    BatchAppComm.SetCollectionData(datFileName, dataKind)
                                    If dataKind = DataKind_G Then
                                        Return bRtn
                                    End If
                                    Exit For
                                End If
                            End If
                            Select Case iniInfoAry(j).FIELD_NAME
                                Case "RAIL_SECTION_CODE"
                                    sRail_Code = lineInfo(j)

                                Case "STATION_ORDER_CODE"
                                    sStation_Code = lineInfo(j)

                                Case "CORNER_CODE"
                                    sCorner_Code = lineInfo(j)
                            End Select
                            Continue For
                        Case "UNIT_NO"  '���@�ԍ�
                            If OPMGUtility.checkNumber(lineInfo(j)) = False Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                '���W�f�[�^�̓o�^
                                BatchAppComm.SetCollectionData(datFileName, dataKind)
                                Exit For
                            Else
                                If CInt(lineInfo(j)) = 0 Then
                                    isHaveErr = True
                                    If dataKind = DataKind_G Then
                                        nFlag_G = nFlag_G - 1
                                        If nFlag_G <= 0 Then
                                            nFlag_G = 16
                                            Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, 1, iniInfoAry(j).KOMOKU_NAME))
                                            '���W�f�[�^�̓o�^
                                            BatchAppComm.SetCollectionData(datFileName, dataKind)
                                        End If
                                        Exit For
                                    End If
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    BatchAppComm.SetCollectionData(datFileName, dataKind)
                                    Exit For
                                End If
                            End If
                            sUnit_No = lineInfo(j)
                            Continue For
                        Case "SYUSYU_DATE"  '���W�f�[�^
                            If Not Date.TryParse(lineInfo(j), New Date) Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, iniInfoAry(j).KOMOKU_NAME))
                                '���W�f�[�^�̓o�^
                                BatchAppComm.SetCollectionData(datFileName, dataKind)
                                Return bRtn
                            End If
                            Continue For
                    End Select
                Next

                If isHaveErr = False Then
                    '�@��\���}�X�^�`�F�b�N
                    If dataKind = DataKind_G Then
                        sModel_Code = "G"
                    Else
                        sModel_Code = "Y"
                    End If
                    nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(String.Format(strSQL, Utility.SetSglQuot(sRail_Code), _
                                                   Utility.SetSglQuot(sStation_Code), _
                                                   Utility.SetSglQuot(sCorner_Code), _
                                                   Utility.SetSglQuot(sModel_Code), sUnit_No)))
                    '  �Ď��Ղ�IP�A�h���X����Ώۂ̉��D�@�𒊏o���A�R�[�i�R�[�h���擾
                    If (nRtn = 0) And (dataKind = DataKind_G) Then
                        Dim code As EkCode
                        '�t�@�C��������͂���
                        code = UpboundDataPath.GetEkCode(datFileName)
                        Dim sSQL As String = _
                                "SELECT CORNER_CODE FROM V_MACHINE_NOW" _
                                & "  WHERE RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
                                & "    AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
                                & "    AND MONITOR_ADDRESS = (" _
                                & "    SELECT ADDRESS FROM V_MACHINE_NOW" _
                                & "      WHERE RAIL_SECTION_CODE = '" & code.RailSection.ToString("D3") & "'" _
                                & "        AND STATION_ORDER_CODE = '" & code.StationOrder.ToString("D3") & "'" _
                                & "        AND CORNER_CODE = '" & code.Corner.ToString & "'" _
                                & "        AND MODEL_CODE = 'W'" _
                                & "        AND UNIT_NO = '" & code.Unit.ToString & "')" _
                                & "    AND UNIT_NO = '" & sUnit_No & "'" _
                                & "    AND MODEL_CODE = 'G'"
                        Dim oCorner As Object = dbCtl.ExecuteSQLToReadScalar(sSQL)
                        If oCorner IsNot Nothing Then
                            For j = 0 To iniInfoAry.Length - 1
                                If iniInfoAry(j).FIELD_NAME = "CORNER_CODE" Then
                                    lineInfo(j) = Format(CInt(oCorner), "0000")
                                End If
                            Next
                            nRtn = 1
                        End If
                    End If

                    If nRtn = 0 Then
                        Log.Error(String.Format(RecAppConstants.ERR_MACHINE_NOVALUE, sRail_Code, sStation_Code, sCorner_Code, sUnit_No))
                    End If

                    dlineInfoLstNew.Add(lineInfo)
                End If
            Next

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '���W�f�[�^�̓o�^
            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo)
            Return bRtn
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return bRtn

    End Function

#End Region
End Class
