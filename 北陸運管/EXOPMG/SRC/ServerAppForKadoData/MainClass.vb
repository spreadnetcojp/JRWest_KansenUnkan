' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/06/01  �@�@����   �k���Ή�
'   0.2      2015/05/25  �@�@����   �ғ��ێ�f�[�^�����Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading
Imports System.Text
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' �{�v���Z�X�́A���W�����ғ��E�ێ�f�[�^����͂��A�^�p�Ǘ��T�[�o��DB�ɓo�^����B
''' </summary>
''' <remarks></remarks>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "�錾�̈�iPrivate�j"

    ''' <summary>
    ''' �ғ��e�[�u����
    ''' </summary>
    Private Const Kadou_TableName As String = "D_KADO_DATA"

    ''' <summary>
    ''' �ێ�e�[�u����
    ''' </summary>
    Private Const Hosyu_TableName As String = "D_HOSYU_DATA"

    ''' <summary>
    ''' ���D�@�f�[�^���
    ''' </summary>
    Private Const DataKind_G As String = "A7"

    ''' <summary>
    ''' ���������@�f�[�^���
    ''' </summary>
    Private Const DataKind_Y As String = "B7"

    ''' <summary>
    ''' �ғ��f�[�^���
    ''' </summary>
    Private Const Kado_DataKind As String = "A7"

    ''' <summary>
    ''' �ێ�f�[�^���
    ''' </summary>
    Private Const Hosyu_DataKind As String = "A8"
    '-------Ver0.1�@�k���Ή��@ADD START-----------
    ''' <summary>
    ''' �O���[�v�ԍ�
    ''' </summary>
    Private Shared GrpNo As Integer = 0
    '-------Ver0.1�@�k���Ή��@ADD END-----------

#End Region

#Region "Main"

    ''' <summary>
    ''' �ғ��E�ێ�f�[�^�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �ғ��E�ێ�f�[�^�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForKadoData")
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

                Log.Init(sLogBasePath, "ForKadoData")
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

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    '''  �ғ��E�ێ�f�[�^
    ''' </summary>
    ''' <param name="sFilePath">�o�^����ׂ��f�[�^���i�[���ꂽ�t�@�C���̐�΃p�X��</param>
    ''' <returns>�o�^�̌���</returns>
    ''' <remarks>
    ''' �f�[�^�o�^�X���b�h�ŌĂяo�����B
    ''' </remarks>
    Private Shared Function RecordToDatabase(ByVal sFilePath As String) As RecordingResult
        Try
            Dim modelCode As Integer = UpboundDataPath.GetEkCode(sFilePath).Model '�@��R�[�h

            Dim sModelCode As String = Format(modelCode, "00")
            Dim kadoDefineInfo() As RecDataStructure.DefineInfo = Nothing  '��`���
            Dim hosyuDefineInfo() As RecDataStructure.DefineInfo = Nothing '��`���
            Dim lstDataNew As New List(Of String())                        '���������f�[�^���

            Dim lstKadoData As New List(Of String())                        '�f�[�^���
            Dim lstHosyuData As New List(Of String())                       '�f�[�^���
            Dim dataKind(0) As Byte
            '-------Ver0.1�@�k���Ή��@ADD START-----------
            '�t�@�C�����������w���R�[�h�擾
            Dim ekiCode As String = UpboundDataPath.GetEkCode(sFilePath).RailSection.ToString("D3") _
                                    & UpboundDataPath.GetEkCode(sFilePath).StationOrder.ToString("D3")
            '����w���R�[�h�������ɃO���[�v�ԍ��擾
            If GetGroupNo(ekiCode) = False Then
                Return RecordingResult.IOError
            End If
            '-------Ver0.1�@�k���Ή��@ADD END-----------
            '�f�[�^��ʂ��擾
            Using fs As New FileStream(sFilePath, FileMode.Open)
                fs.Read(dataKind, 0, 1)
            End Using

            'OPT: �ȉ��AGetDefineInfo��Main���\�b�h�ɂĈ�x�����s�������悢���A
            'xxxDefineInfo��Immutable�łȂ����߁A�������Ȃ����Ă���Ƃ���
            '�b������A�Ή�����Ȃ璍�ӂ��Ȃ���΂Ȃ�Ȃ��B

            If Hex(dataKind(0)) = DataKind_G Then  '���D�@
                '-------Ver0.1�@�k���Ή��@MOD START-----------
                '�ғ���`�����擾����B
                If DefineInfoShutoku.GetDefineInfo(Config.KadoFormatFileG(GrpNo).ToString, "KADO", kadoDefineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If

                '�ێ��`�����擾����B
                If DefineInfoShutoku.GetDefineInfo(Config.HosyuFormatFile(GrpNo).ToString, "HOSYU", hosyuDefineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If
                '-------Ver0.1�@�k���Ή��@MOD END-----------
                '-----------------------------�ғ����� Start-----------------------------
                'DAT�t�@�C���f�[�^�擾
                If GetInfoFromDataFile(kadoDefineInfo, sFilePath, sModelCode, Kado_DataKind, lstKadoData, True) = False Then
                    Return RecordingResult.ParseError
                End If

                '�`�F�b�N
                If CheckData(kadoDefineInfo, lstKadoData, lstDataNew, sFilePath, Kado_DataKind) = False Then
                    Return RecordingResult.IOError
                End If

                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(kadoDefineInfo, lstDataNew, Kadou_TableName) = False Then
                    Return RecordingResult.IOError
                End If
                '-----------------------------�ғ����� End  -----------------------------

                '-----------------------------�ێ珈�� Start-----------------------------
                'DAT�t�@�C���f�[�^�擾
                If GetInfoFromDataFile(hosyuDefineInfo, sFilePath, sModelCode, Hosyu_DataKind, lstHosyuData) = False Then
                    Return RecordingResult.ParseError
                End If

                '�`�F�b�N
                lstDataNew = New List(Of String())
                If CheckData(hosyuDefineInfo, lstHosyuData, lstDataNew, sFilePath, Hosyu_DataKind) = False Then
                    Return RecordingResult.IOError
                End If

                '�ێ�f�[�^���ĉ��H
                lstHosyuData = New List(Of String())
                If GetDBInfoFromDataInfo(hosyuDefineInfo, sFilePath, sModelCode, lstDataNew, lstHosyuData) = False Then
                    Return RecordingResult.ParseError
                End If

                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(hosyuDefineInfo, lstHosyuData, Hosyu_TableName) = False Then
                    Return RecordingResult.IOError
                End If
                '-----------------------------�ێ珈�� End  -----------------------------
            ElseIf Hex(dataKind(0)) = DataKind_Y Then     '���������@
                '�ғ���`�����擾����B
                If DefineInfoShutoku.GetDefineInfo(Config.KadoFormatFilePath_Y, "KADO", kadoDefineInfo) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return RecordingResult.IOError
                End If

                '-----------------------------�ғ����� Start-----------------------------
                'DAT�t�@�C���f�[�^�擾
                If GetInfoFromDataFile(kadoDefineInfo, sFilePath, sModelCode, Kado_DataKind, lstKadoData, True) = False Then
                    Return RecordingResult.ParseError
                End If

                '�`�F�b�N
                If CheckData(kadoDefineInfo, lstKadoData, lstDataNew, sFilePath, Kado_DataKind) = False Then
                    Return RecordingResult.IOError
                End If

                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(kadoDefineInfo, lstDataNew, Kadou_TableName) = False Then
                    Return RecordingResult.IOError
                End If
                '-----------------------------�ғ����� End  -----------------------------
            End If

            '���������ꍇ
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Kado_DataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' �ғ��E�ێ�f�[�^�̉��
    ''' </summary>
    ''' <param name="defineInfo">INI�t�@�C�����e</param>
    ''' <param name="sFilePath">�f�[�^�t�@�C����</param>
    ''' <param name="sModelCode">�@��R�[�h</param>
    ''' <param name="sDataKind">�f�[�^���</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�擾�����d���t�H�[�}�b�g��`���ɂĉғ��E�ێ�f�[�^����͂���</remarks>
    Private Shared Function GetInfoFromDataFile(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                                ByVal sFilePath As String, _
                                                ByVal sModelCode As String, _
                                                ByVal sDataKind As String, _
                                                ByRef lstData As List(Of String()), _
                                                Optional ByVal isCheckDataKind As Boolean = False) As Boolean
        Dim nHeadSize As Integer = 17
        Dim nDataSize As Integer = 2188

        Dim fileStream As FileStream = Nothing
        Dim iStarRecIndex As Integer = 0 '�J�n���R�[�hindex
        '���R�[�h��
        Dim iRecCnt As Integer = 0
        '�f�[�^��
        Dim bData() As Byte
        '�w�b�h��
        Dim headInfo As RecDataStructure.BaseInfo = Nothing
        '�P���R�[�h
        Dim sArrRecord() As String

        lstData = New List(Of String())

        Try
            '�t�@�C���X�g���[�����擾
            fileStream = New FileStream(sFilePath, FileMode.Open)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            '���R�[�h�T�C�Y�`�F�b�N
            If fileStream.Length < (nDataSize + nHeadSize) Then
                Log.Error(RecAppConstants.ERR_TOO_SHORT_FILE)
                Return False
            End If

            '���M����
            If fileStream.Length > (nDataSize + nHeadSize) Then 'ftp�̏ꍇ
                iStarRecIndex = 1
            Else 'socket�̏ꍇ
                iStarRecIndex = 0
            End If

            '���R�[�h���擾
            If fileStream.Length Mod (nDataSize + nHeadSize) = 0 Then
                iRecCnt = CInt(fileStream.Length / (nDataSize + nHeadSize))
            Else
                iRecCnt = CInt(Int(fileStream.Length / (nDataSize + nHeadSize)))
            End If

            '���R�[�h������
            For i As Integer = iStarRecIndex To iRecCnt - 1

                ReDim bData(nDataSize + nHeadSize) '1���R�[�h�̃f�[�^

                '�t�@�C�������R�[�h�ʒu
                fileStream.Seek(i * (nDataSize + nHeadSize), SeekOrigin.Begin)
                fileStream.Read(bData, 0, nDataSize + nHeadSize)

                headInfo = Nothing
                BinaryHeadInfoParse.GetBaseInfo(bData, sModelCode, headInfo)

                '�f�[�^��ʂ̃`�F�b�N���s��
                If isCheckDataKind = True _
                   AndAlso headInfo.DATA_KIND <> Kado_DataKind _
                   AndAlso headInfo.DATA_KIND <> Hosyu_DataKind Then
                    '�f�[�^��ʂ̕s�����O���o�͂���
                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, "�f�[�^���"))
                    '���W�f�[�^�̓o�^
                    BatchAppComm.SetCollectionData(sFilePath, headInfo.DATA_KIND)
                    Continue For
                End If

                '��̓f�[�^���擾����
                If headInfo.DATA_KIND = sDataKind Then
                    ReDim sArrRecord(defineInfo.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo, bData, headInfo, sArrRecord) = False Then
                        '���W�f�[�^�̓o�^
                        BatchAppComm.SetCollectionData(headInfo, sDataKind)
                        Continue For
                    End If

                    '�d���ł̍s�ڂ�ݒ肷��
                    sArrRecord(defineInfo.Length - 1) = CStr(i + 1)

                    '��͂����f�[�^��ݒ肷��
                    lstData.Add(sArrRecord)
                End If
            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: �ғ��E�ێ�f�[�^�Ɋւ����͎��s�̃t�@�C����ʂ́A��ɉғ��f�[�^�̎�ʂƂ���B
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Kado_DataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return False
        Finally
            '�t�@�C���X�g���[�������
            fileStream.Close()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' �ғ��E�ێ�f�[�^�̃`�F�b�N
    ''' </summary>
    ''' <param name="iniInfoAry">ini�t�@�C��</param>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dlineInfoLstNew">dat�t�@�C�����e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>��͏����ɂ��擾�f�[�^���`�F�b�N����</remarks>
    Private Shared Function CheckData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String, _
                                      ByVal sDataKind As String) As Boolean
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:�G���[;false:�G���[�Ȃ�
        Dim strDate As String
        Dim iLineNo As Integer

        dlineInfoLstNew = New List(Of String())

        For i = 0 To dlineInfoLst.Count - 1

            isHaveErr = False

            lineInfo = dlineInfoLst.Item(i)

            '�d���ł̍s�ڂ��擾����
            If OPMGUtility.checkNumber(lineInfo(lineInfo.Length - 1)) Then
                iLineNo = CInt(lineInfo(lineInfo.Length - 1))
            Else
                iLineNo = i + 1
            End If

            '���ʂ̃`�F�b�N���s��
            If BatchAppComm.CheckDataComm(iLineNo, iniInfoAry, lineInfo, datFileName) = False Then
                Continue For
            End If

            '���ʂȃ`�F�b�N
            If sDataKind = Kado_DataKind Then
                For j = 0 To iniInfoAry.Length - 1
                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "KAI_INSPECT_TIME", "SYU_INSPECT_TIME"     '���D���_������  �W�D���_������
                            If lineInfo(j).Substring(0, 14) <> "00000000000000" Then
                                strDate = lineInfo(j).Substring(0, 4) & "/" & _
                                     lineInfo(j).Substring(4, 2) & "/" & _
                                     lineInfo(j).Substring(6, 2) & " " & _
                                     lineInfo(j).Substring(8, 2) & ":" & _
                                     lineInfo(j).Substring(10, 2) & ":" & _
                                     lineInfo(j).Substring(12, 2)

                                If Not Date.TryParse(strDate, New Date) Then
                                    lineInfo(j) = "00000000000000"
                                End If
                            End If
                    End Select
                Next
            End If

            If isHaveErr = False Then
                dlineInfoLstNew.Add(lineInfo)
            End If
        Next

        Return True

    End Function

    ''' <summary>
    ''' �擾�����f�[�^��DB�o�^�f�[�^�ɍĉ��H����
    ''' </summary>
    ''' <param name="hosyuDefineInfo">��`���</param>
    ''' <param name="sFilePath">�t�@�C���p�[�X</param>
    ''' <param name="sModelCode">�@��R�[�h</param>
    ''' <param name="lstGetData">�f�[�^���</param>
    ''' <param name="lstHosyuData">�ĉ��H�f�[�^</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�`�F�b�N�����ێ�f�[�^���ĉ��H����</remarks>
    Private Shared Function GetDBInfoFromDataInfo(ByVal hosyuDefineInfo() As RecDataStructure.DefineInfo, _
                                                  ByVal sFilePath As String, _
                                                  ByVal sModelCode As String, _
                                                  ByVal lstGetData As List(Of String()), _
                                                  ByRef lstHosyuData As List(Of String())) As Boolean
        '��L�[���
        Dim sRAIL_SECTION_CODE As String = ""
        Dim sSTATION_ORDER_CODE As String = ""
        Dim sCORNER_CODE As String = ""
        Dim sMODEL_CODE As String = ""
        Dim sUNIT_NO As String = ""
        Dim sPROCESSING_TIME As String = ""
        Dim sCOLLECT_START_TIME As String = ""
        Dim sCOLLECT_END_TIME As String = ""

        Dim nKeyFlag As Integer
        '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
        Dim tKeyFlg As Boolean
        '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
        Dim sArrInfo As String()
        Dim kadouDefineInfo() As RecDataStructure.DefineInfo = Nothing
        Dim lstKadouNewData As New List(Of String())
        Dim lstKadouALLData As New List(Of String())
        lstHosyuData = New List(Of String())
        '-------Ver0.1�@�k���Ή��@ADD START-----------
        '�t�@�C�����������w���R�[�h�擾
        Dim ekiCode As String = UpboundDataPath.GetEkCode(sFilePath).RailSection.ToString("D3") _
                                & UpboundDataPath.GetEkCode(sFilePath).StationOrder.ToString("D3")
        '����w���R�[�h�������ɃO���[�v�ԍ��擾
        If GetGroupNo(ekiCode) = False Then
            Return False
        End If
        '-------Ver0.1�@�k���Ή��@ADD END-----------
        '-------Ver0.1�@�k���Ή��@MOD START-----------
        '�ғ���`�����擾����B
        If DefineInfoShutoku.GetDefineInfo(Config.KadoFormatFileG(GrpNo).ToString, "KADO_002", kadouDefineInfo) = False Then
            AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
            Return False
        End If
        '-------Ver0.1�@�k���Ή��@MOD END-----------
        'DAT�t�@�C���f�[�^�擾
        If GetInfoFromDataFile(kadouDefineInfo, sFilePath, sModelCode, Kado_DataKind, lstKadouALLData) = False Then
            Return False
        End If

        '�`�F�b�N
        If CheckDataNoMsg(kadouDefineInfo, lstKadouALLData, lstKadouNewData, sFilePath, Kado_DataKind) = False Then
            Return False
        End If

        '�ғ��ƕێ�f�[�^�̃y�A�`�F�b�N���s��
        Dim lstChkKadoData As New List(Of String())
        Dim lstChkHosyuData As New List(Of String())
        For i As Integer = 0 To lstKadouNewData.Count - 1
            lstChkKadoData.Add(lstKadouNewData(i))
        Next
        Call CheckPair(lstChkKadoData, lstGetData, hosyuDefineInfo, kadouDefineInfo, lstChkHosyuData)

        If lstChkHosyuData.Count <= 0 Then
            Return True
        End If

        '�ێ�f�[�^���ĉ��H����
        For iHosyu As Integer = 0 To lstChkHosyuData.Count - 1
            sArrInfo = lstChkHosyuData(iHosyu)
            nKeyFlag = 6
            '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
            '������������t���O��������
            tKeyFlg = False
            '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
            '�ێ��`���ɂ��ێ�̎�L�[�l���擾����
            For j As Integer = 0 To hosyuDefineInfo.Length - 1
                Select Case hosyuDefineInfo(j).FIELD_NAME
                    Case "RAIL_SECTION_CODE"
                        sRAIL_SECTION_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "STATION_ORDER_CODE"
                        sSTATION_ORDER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "CORNER_CODE"
                        sCORNER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "MODEL_CODE"
                        sMODEL_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "UNIT_NO"
                        sUNIT_NO = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "PROCESSING_TIME"
                        sPROCESSING_TIME = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                End Select
                If nKeyFlag = 0 Then
                    Exit For
                End If
            Next

            '�ғ��f�[�^
            For iKadou As Integer = 0 To lstKadouNewData.Count - 1
                nKeyFlag = 6
                Dim nKadouFlag As Integer = 6
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
                '������������t���O��������
                tKeyFlg = False
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
                '�ғ���`���ɂ��ێ�̎�L�[�l�Ɖғ��̎�L�[�l�͈�v���邩�ǂ����𔻒f����
                For j As Integer = 0 To kadouDefineInfo.Length - 1
                    Select Case kadouDefineInfo(j).FIELD_NAME
                        Case "RAIL_SECTION_CODE"
                            If sRAIL_SECTION_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1
                        Case "STATION_ORDER_CODE"
                            If sSTATION_ORDER_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "CORNER_CODE"
                            If sCORNER_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "MODEL_CODE"
                            If sMODEL_CODE.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "UNIT_NO"
                            If sUNIT_NO.Equals(lstKadouNewData(iKadou)(j)) Then
                                nKeyFlag = nKeyFlag - 1
                            End If
                            nKadouFlag = nKadouFlag - 1

                        Case "PROCESSING_TIME"
                            '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@START----------------------
                            '���������̔N�����̂ݔ�r����
                            If sPROCESSING_TIME.Substring(0, 8).Equals(lstKadouNewData(iKadou)(j).Substring(0, 8)) Then
                                nKeyFlag = nKeyFlag - 1
                            Else
                                '������������t���O���Z�b�g
                                tKeyFlg = True
                            End If
                            '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@END----------------------
                            nKadouFlag = nKadouFlag - 1

                    End Select
                    If nKadouFlag = 0 Then
                        Exit For
                    End If
                Next

                ''�Y���ғ��f�[�^�Ɏ�L�[�ƊY���ێ�f�[�^�̎�L�[�͈�v
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@START----------------------
                If nKeyFlag = 0 Or (nKeyFlag = 1 And tKeyFlg = True) Then
                    'Dim nSetFlag As Integer = 301 '384
                    For j As Integer = 0 To hosyuDefineInfo.Length - 1
                        If (hosyuDefineInfo(j).COMMENT.Contains("COLLECT_START_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("COLLECT_END_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("KAI_INSPECT_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("SYU_INSPECT_TIME")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("KAI_SERIAL_NO")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("SYU_SERIAL_NO")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("ITEM")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("KAI_SENSOR_")) _
                           OrElse (hosyuDefineInfo(j).COMMENT.Contains("SYU_SENSOR_")) Then
                            '�ێ�f�[�^�̍��ڒl�ɉғ��f�[�^�̑Ή����ڒl��ݒ肷��
                            For n As Integer = 0 To kadouDefineInfo.Length - 1
                                If kadouDefineInfo(n).FIELD_NAME = hosyuDefineInfo(j).COMMENT Then
                                    sArrInfo(j) = lstKadouNewData(iKadou)(n)
                                    'nSetFlag = nSetFlag - 1
                                    Exit For
                                End If
                            Next
                            'If nSetFlag = 0 Then
                            '    lstHosyuData.Add(sArrInfo)
                            '    Exit For
                            'End If
                        End If
                    Next
                    lstHosyuData.Add(sArrInfo)
                    Exit For
                End If
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@START----------------------
            Next
        Next


        Return True
    End Function

    ''' <summary>
    ''' �ғ��ƕێ�f�[�^�̃y�A�`�F�b�N���s��
    ''' </summary>
    ''' <param name="lstKadoData">�ғ��f�[�^</param>
    ''' <param name="lstHosyuData">�ێ�f�[�^</param>
    ''' <param name="hosyuDefineInfo">�ێ��`���</param>
    ''' <param name="kadoDefineInfo">�ғ���`���</param>
    ''' <param name="lstRtnHosyuData">�ێ�f�[�^</param>
    ''' <returns>True�F����OK False�F�`�F�b�NNG</returns>
    ''' <remarks>�y�A�Z�b�g�ƂȂ��ĂȂ��ꍇ�A���W�f�[�^�o�^���s��</remarks>
    Private Shared Function CheckPair(ByVal lstKadoData As List(Of String()), _
                                      ByVal lstHosyuData As List(Of String()), _
                                      ByVal hosyuDefineInfo() As RecDataStructure.DefineInfo, _
                                      ByVal kadoDefineInfo() As RecDataStructure.DefineInfo, _
                                      ByRef lstRtnHosyuData As List(Of String())) As Boolean
        Dim nKeyFlag As Integer
        Dim sArrInfo As String()
        '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
        Dim tKeyFlg As Boolean  '������������t���O
        '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
        '��L�[���
        Dim sRAIL_SECTION_CODE As String = ""
        Dim sSTATION_ORDER_CODE As String = ""
        Dim sCORNER_CODE As String = ""
        Dim sMODEL_CODE As String = ""
        Dim sUNIT_NO As String = ""
        Dim sPROCESSING_TIME As String = ""
        Dim sCOLLECT_START_TIME As String = ""
        Dim sCOLLECT_END_TIME As String = ""

        If lstHosyuData.Count <= 0 Then
            '�ێ�f�[�^�������ꍇ�A�ғ��f�[�^�������Ď��W�f�[�^�o�^���s��
            Call InsertCollectionDataPair(kadoDefineInfo, lstKadoData)
            Return True
        End If

        If lstKadoData.Count <= 0 Then
            '�ғ��f�[�^�������ꍇ�A�ێ�f�[�^�������Ď��W�f�[�^�o�^���s��
            Call InsertCollectionDataPair(hosyuDefineInfo, lstHosyuData)
            Return True
        End If

        If lstKadoData.Count > lstHosyuData.Count Then
            For iHosyu As Integer = lstHosyuData.Count - 1 To 0 Step -1
                sArrInfo = lstHosyuData(iHosyu)
                nKeyFlag = 6
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
                '������������t���O��������
                tKeyFlg = False
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
                '�ێ��`���ɂ��ێ�̎�L�[�l���擾����
                For j As Integer = 0 To hosyuDefineInfo.Length - 1
                    Select Case hosyuDefineInfo(j).FIELD_NAME
                        Case "RAIL_SECTION_CODE"
                            sRAIL_SECTION_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "STATION_ORDER_CODE"
                            sSTATION_ORDER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "CORNER_CODE"
                            sCORNER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "MODEL_CODE"
                            sMODEL_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "UNIT_NO"
                            sUNIT_NO = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "PROCESSING_TIME"
                            sPROCESSING_TIME = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                    End Select
                    If nKeyFlag = 0 Then
                        Exit For
                    End If
                Next

                '�ғ��f�[�^
                For iKadou As Integer = lstKadoData.Count - 1 To 0 Step -1
                    nKeyFlag = 6
                    '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
                    '������������t���O��������
                    tKeyFlg = False
                    '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
                    Dim nKadouFlag As Integer = 6
                    '�ғ���`���ɂ��ێ�̎�L�[�l�Ɖғ��̎�L�[�l�͈�v���邩�ǂ����𔻒f����
                    For j As Integer = 0 To kadoDefineInfo.Length - 1
                        Select Case kadoDefineInfo(j).FIELD_NAME
                            Case "RAIL_SECTION_CODE"
                                If sRAIL_SECTION_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1
                            Case "STATION_ORDER_CODE"
                                If sSTATION_ORDER_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "CORNER_CODE"
                                If sCORNER_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "MODEL_CODE"
                                If sMODEL_CODE.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "UNIT_NO"
                                If sUNIT_NO.Equals(lstKadoData(iKadou)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "PROCESSING_TIME"
                                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@START----------------------
                                '���������̔N�����̂ݔ�r����
                                If sPROCESSING_TIME.Substring(0, 8).Equals(lstKadoData(iKadou)(j).Substring(0, 8)) Then
                                    nKeyFlag = nKeyFlag - 1
                                Else
                                    '������������t���O��TRUE
                                    tKeyFlg = True
                                End If
                                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@END----------------------
                                nKadouFlag = nKadouFlag - 1

                        End Select
                        If nKadouFlag = 0 Then
                            Exit For
                        End If
                    Next
                    '�Y���ғ��f�[�^�Ɏ�L�[�ƊY���ێ�f�[�^�̎�L�[�͈�v
                    '���������̂ݕs��v�̏ꍇ�ł��f�[�^�o�^����
                    If nKeyFlag = 0 Or (nKeyFlag = 1 And tKeyFlg = True) Then
                        For j As Integer = 0 To kadoDefineInfo.Length - 1
                            '�ێ�f�[�^�̍��ڒl�ɉғ��f�[�^�̑Ή����ڒl��ݒ肷��
                            For n As Integer = 0 To hosyuDefineInfo.Length - 1
                                If hosyuDefineInfo(n).COMMENT = kadoDefineInfo(j).FIELD_NAME Then
                                    lstHosyuData(iHosyu)(n) = lstKadoData(iKadou)(j)
                                    Exit For
                                End If
                            Next
                        Next

                        lstRtnHosyuData.Add(lstHosyuData(iHosyu))
                        '��L�[��v����ꍇ�A�f�[�^�N���A
                        If nKeyFlag = 0 And tKeyFlg = False Then
                            lstHosyuData.RemoveAt(iHosyu)
                            lstKadoData.RemoveAt(iKadou)
                        End If
                    End If
                Next
            Next
        Else
            For iKadou As Integer = lstKadoData.Count - 1 To 0 Step -1
                sArrInfo = lstKadoData(iKadou)
                nKeyFlag = 6
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
                '������������t���O��������
                tKeyFlg = False
                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
                '�ێ��`���ɂ��ێ�̎�L�[�l���擾����
                For j As Integer = 0 To kadoDefineInfo.Length - 1
                    Select Case kadoDefineInfo(j).FIELD_NAME
                        Case "RAIL_SECTION_CODE"
                            sRAIL_SECTION_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "STATION_ORDER_CODE"
                            sSTATION_ORDER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "CORNER_CODE"
                            sCORNER_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "MODEL_CODE"
                            sMODEL_CODE = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "UNIT_NO"
                            sUNIT_NO = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                        Case "PROCESSING_TIME"
                            sPROCESSING_TIME = sArrInfo(j)
                            nKeyFlag = nKeyFlag - 1

                    End Select
                    If nKeyFlag = 0 Then
                        Exit For
                    End If
                Next

                '�f�[�^
                For iHosyu As Integer = lstHosyuData.Count - 1 To 0 Step -1
                    nKeyFlag = 6
                    Dim nKadouFlag As Integer = 6
                    '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@START----------------------
                    '������������t���O��������
                    tKeyFlg = False
                    '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@ADD�@END----------------------
                    '�ғ���`���ɂ��ێ�̎�L�[�l�Ɖғ��̎�L�[�l�͈�v���邩�ǂ����𔻒f����
                    For j As Integer = 0 To hosyuDefineInfo.Length - 1
                        Select Case hosyuDefineInfo(j).FIELD_NAME
                            Case "RAIL_SECTION_CODE"
                                If sRAIL_SECTION_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1
                            Case "STATION_ORDER_CODE"
                                If sSTATION_ORDER_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "CORNER_CODE"
                                If sCORNER_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "MODEL_CODE"
                                If sMODEL_CODE.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "UNIT_NO"
                                If sUNIT_NO.Equals(lstHosyuData(iHosyu)(j)) Then
                                    nKeyFlag = nKeyFlag - 1
                                End If
                                nKadouFlag = nKadouFlag - 1

                            Case "PROCESSING_TIME"
                                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@START----------------------
                                '���������̔N�����̂ݔ�r����
                                If sPROCESSING_TIME.Substring(0, 8).Equals(lstHosyuData(iHosyu)(j).Substring(0, 8)) Then
                                    nKeyFlag = nKeyFlag - 1
                                Else
                                    tKeyFlg = True
                                End If
                                '-----Ver0.2�@�ғ��ێ�f�[�^�����Ή��@MOD�@END----------------------
                                nKadouFlag = nKadouFlag - 1

                        End Select
                        If nKadouFlag = 0 Then
                            Exit For
                        End If
                    Next

                    '�Y���ғ��f�[�^�Ɏ�L�[�ƊY���ێ�f�[�^�̎�L�[�͈�v
                    '���������̂ݕs��v�̏ꍇ�ł��f�[�^�o�^����
                    If nKeyFlag = 0 Or (nKeyFlag = 1 And tKeyFlg = True) Then
                        For j As Integer = 0 To kadoDefineInfo.Length - 1
                            '�ێ�f�[�^�̍��ڒl�ɉғ��f�[�^�̑Ή����ڒl��ݒ肷��
                            For n As Integer = 0 To hosyuDefineInfo.Length - 1
                                If hosyuDefineInfo(n).COMMENT = kadoDefineInfo(j).FIELD_NAME Then
                                    lstHosyuData(iHosyu)(n) = sArrInfo(j)
                                    Exit For
                                End If
                            Next
                        Next
                        lstRtnHosyuData.Add(lstHosyuData(iHosyu))
                        '��L�[��v����ꍇ�A�f�[�^�N���A
                        If nKeyFlag = 0 And tKeyFlg = False Then
                            lstHosyuData.RemoveAt(iHosyu)
                            lstKadoData.RemoveAt(iKadou)
                        End If
                    End If
                Next
            Next
        End If

        '�y�A�Z�b�g�ƂȂ��ĂȂ��ꍇ�A���W�f�[�^�o�^���s��
        Call InsertCollectionDataPair(hosyuDefineInfo, lstHosyuData)
        Call InsertCollectionDataPair(kadoDefineInfo, lstKadoData)

        Return True
    End Function

    ''' <summary>
    ''' ���W�f�[�^�o�^
    ''' </summary>
    ''' <param name="defineInfo">��`���</param>
    ''' <param name="lstData">�f�[�^</param>
    ''' <remarks>�y�A�Z�b�g�ƂȂ��ĂȂ��ꍇ�A���W�f�[�^�o�^���s��</remarks>
    Private Shared Sub InsertCollectionDataPair(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                                ByVal lstData As List(Of String()))
        Dim nKeyFlag As Integer
        Dim sArrInfo As String()

        '��L�[���
        Dim sRAIL_SECTION_CODE As String = ""
        Dim sSTATION_ORDER_CODE As String = ""
        Dim sCORNER_CODE As String = ""
        Dim sMODEL_CODE As String = ""
        Dim sUNIT_NO As String = ""
        Dim sPROCESSING_TIME As String = ""
        Dim sCOLLECT_START_TIME As String = ""
        Dim sCOLLECT_END_TIME As String = ""

        For i As Integer = 0 To lstData.Count - 1
            sArrInfo = lstData(i)
            nKeyFlag = 6
            '�ێ��`���ɂ��ێ�̎�L�[�l���擾����
            For j As Integer = 0 To defineInfo.Length - 1
                Select Case defineInfo(j).FIELD_NAME
                    Case "RAIL_SECTION_CODE"
                        sRAIL_SECTION_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "STATION_ORDER_CODE"
                        sSTATION_ORDER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "CORNER_CODE"
                        sCORNER_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "MODEL_CODE"
                        sMODEL_CODE = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "UNIT_NO"
                        sUNIT_NO = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                    Case "PROCESSING_TIME"
                        sPROCESSING_TIME = sArrInfo(j)
                        nKeyFlag = nKeyFlag - 1

                End Select
                If nKeyFlag = 0 Then
                    Exit For
                End If
            Next

            '��{�w�b�_����ݒ肷��
            Dim baseInfo As RecDataStructure.BaseInfo = Nothing
            With baseInfo
                .STATION_CODE.RAIL_SECTION_CODE = sRAIL_SECTION_CODE
                .STATION_CODE.STATION_ORDER_CODE = sSTATION_ORDER_CODE
                .CORNER_CODE = sCORNER_CODE
                .MODEL_CODE = sMODEL_CODE
                .UNIT_NO = CInt(sUNIT_NO)
                .PROCESSING_TIME = sPROCESSING_TIME
                .DATA_KIND = DbConstants.CdtKindKadoData
            End With

            '���W�f�[�^�o�^���s��
            CollectedDataTypoRecorder.Record(baseInfo, DbConstants.CdtKindKadoData, _
                                             Lexis.CdtUnpairedKadoDataDetected.Gen(baseInfo.UNIT_NO))
        Next
    End Sub

    ''' <summary>
    ''' �ғ��E�ێ�f�[�^�̃`�F�b�N
    ''' </summary>
    ''' <param name="iniInfoAry">ini�t�@�C��</param>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dlineInfoLstNew">dat�t�@�C�����e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>��͏����ɂ��擾�f�[�^���`�F�b�N����</remarks>
    Private Shared Function CheckDataNoMsg(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String, _
                                      ByVal sDataKind As String) As Boolean
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim strDate As String = ""

        dlineInfoLstNew = New List(Of String())

        For i = 0 To dlineInfoLst.Count - 1

            lineInfo = dlineInfoLst.Item(i)

            '���ʂ̃`�F�b�N���s��
            If CheckDataCommNoMsg(iniInfoAry, lineInfo, datFileName) = False Then
                Continue For
            End If

            '���ʂȃ`�F�b�N
            If sDataKind = Kado_DataKind Then
                For j = 0 To iniInfoAry.Length - 1
                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "KAI_INSPECT_TIME", "SYU_INSPECT_TIME"     '���D���_������  �W�D���_������
                            '-------Ver0.1�@�k���Ή��@ADD START-----------
                            If lineInfo(j).Substring(0, 14) <> "00000000000000" Then
                                strDate = lineInfo(j).Substring(0, 4) & "/" & _
                                     lineInfo(j).Substring(4, 2) & "/" & _
                                     lineInfo(j).Substring(6, 2) & " " & _
                                     lineInfo(j).Substring(8, 2) & ":" & _
                                     lineInfo(j).Substring(10, 2) & ":" & _
                                     lineInfo(j).Substring(12, 2)

                                If Not Date.TryParse(strDate, New Date) Then
                                    lineInfo(j) = "00000000000000"
                                End If
                            End If
                            '-------Ver0.1�@�k���Ή��@ADD END-----------
                    End Select
                Next
            End If

            dlineInfoLstNew.Add(lineInfo)
        Next

        Return True

    End Function

    ''' <summary>
    ''' DAT�t�@�C���̋��ʃ`�F�b�N:1���R�[�h�̃`�F�b�N
    ''' </summary>
    ''' <param name="iniInfoAry">ini�t�@�C�����</param>
    ''' <param name="lineInfo">���R�[�h�f�[�^</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Public Shared Function CheckDataCommNoMsg(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                              ByVal lineInfo() As String, _
                                              ByVal datFileName As String) As Boolean

        Dim iFlag As Integer = 4
        Dim dataKind As String = "" '�f�[�^���

        Try

            For i As Integer = 0 To iniInfoAry.Length - 1
                If UCase(iniInfoAry(i).FIELD_NAME) = "DATA_KIND" Then
                    dataKind = lineInfo(i) 'OPT: �g��Ȃ��̂ŕs�v
                    Continue For
                End If

                '�w�R�[�h�A�R�[�i�[�R�[�h�A���@�ԍ����S���`�F�b�N�ł͂Ȃ��ꍇ
                If iFlag > 0 Then
                    Select Case UCase(iniInfoAry(i).FIELD_NAME)  '�w�R�[�h�A�R�[�i�[�R�[�h�A���@�ԍ�
                        Case "RAIL_SECTION_CODE", "STATION_ORDER_CODE", "CORNER_CODE", "UNIT_NO"
                            iFlag = iFlag - 1

                            If (iniInfoAry(i).PARA2 = False) Then
                                If Integer.Parse(lineInfo(i)) = 0 Then
                                    Return False
                                End If
                            End If

                            Continue For
                    End Select
                End If

                '�L�[ �� NULL�s��
                Select Case UCase(iniInfoAry(i).FIELD_FORMAT)
                    Case "INTEGER"
                        '�s���ꍇ
                        If lineInfo(i) IsNot Nothing AndAlso _
                          (Not lineInfo(i) = "") AndAlso _
                          OPMGUtility.checkNumber(lineInfo(i)) = False Then
                            Return (False)
                        Else '��ꍇ
                            'NULL�s��
                            If (iniInfoAry(i).PARA2 = False) Then
                                If Integer.Parse(lineInfo(i)) = 0 Then
                                    Return (False)
                                End If
                            End If

                        End If
                    Case "DATESTR"
                        '���������t�H�[�}�[�g�`�F�b�N
                        Dim lnDate As Long = 0

                        If OPMGUtility.checkNumber(lineInfo(i)) = False Then
                            Return False
                        Else '�S���O�ꍇ
                            'NULL�s��
                            If (iniInfoAry(i).PARA2 = False) Then
                                If Long.Parse(lineInfo(i)) = 0 Then
                                    Return False
                                End If
                            End If
                            If lineInfo(i).Length = 14 Then
                                If BatchAppComm.CheckDate(lineInfo(i)) = False Then
                                    Return False
                                End If
                            Else
                                Return False
                            End If

                        End If
                End Select

            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            '���W�f�[�^�̓o�^
            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo)
            Return False
        End Try

        Return True

    End Function
    '--------------Ver0.1�@�k���Ή��@ADD START-------------------
    ''' <summary>�O���[�v�ԍ��擾</summary>
    ''' <returns>�O���[�v�ԍ�</returns>
    ''' <param name="ekiNo">����w���R�[�h</param>
    ''' <remarks>����w���R�[�h�������ɃO���[�v�ԍ��擾</remarks>
    Private Shared Function GetGroupNo(ByVal ekiNo As String) As Boolean
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable

        sSQL = " SELECT M_BRANCH_OFFICE.GROUP_NO " _
            & " FROM M_BRANCH_OFFICE, V_MACHINE_NOW as m1 " _
            & " WHERE m1.RAIL_SECTION_CODE+m1.STATION_ORDER_CODE= " & ekiNo _
            & " and m1.BRANCH_OFFICE_CODE =M_BRANCH_OFFICE.CODE " _
             & "GROUP BY M_BRANCH_OFFICE.GROUP_NO "

        dbCtl = New DatabaseTalker

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)
            If dt Is Nothing Then
                Return False
            Else
                GrpNo = CInt(dt(0).Item("GROUP_NO"))
                Return True
            End If
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try
    End Function
    '---------------Ver0.1�@�k���Ή��@ADD END-----------------
#End Region

End Class