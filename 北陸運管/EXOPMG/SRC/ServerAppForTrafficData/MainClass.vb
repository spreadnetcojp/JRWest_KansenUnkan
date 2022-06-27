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
Imports JR.ExOpmg.ServerApp

''' <summary>
''' ���ԑѕʏ�~�f�[�^�捞
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass
#Region "�錾�̈�iPrivate�j"
    ''' <summary>
    ''' ���ԑѕʏ�~�f�[�^���
    ''' </summary>
    Private Const Tim_DataKind As String = "B1"

    ''' <summary>
    ''' ���ԑуe�[�u����
    ''' </summary>
    Private Const Tim_TableName As String = "D_TRAFFIC_DATA"

    ''' <summary>
    ''' ���v���o��Ґ��ُ�
    ''' </summary>
    Private Shared ERR_MSG_ERRVALUE As String = "{0}�s�ڂ̓���Ґ��F{1}�@�o��Ґ��F{2} ���v���o��Ґ��F{3}�@�Z�o���ʂƈقȂ�܂��B"

#End Region

#Region "���\�b�h�iMain�j"
    ''' <summary>
    ''' ���ԑѕʏ�~�f�[�^�捞�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' ���ԑѕʏ�~�f�[�^�捞�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForTrafficData")
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

                Log.Init(sLogBasePath, "ForTrafficData")
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
    '''  ���ԑѕʏ�~�f�[�^�捞
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

            Dim dlineInfoLst As List(Of String()) = Nothing
            Dim dlineInfoLstNew As List(Of String()) = Nothing
            Dim dataInfoLst As List(Of String()) = Nothing

            'OPT: �ȉ��AGetDefineInfo��Main���\�b�h�ɂĈ�x�����s�������悢���A
            'iniInfoAry��Immutable�łȂ����߁A�������Ȃ����Ă���Ƃ���
            '�b������A�Ή�����Ȃ璍�ӂ��Ȃ���΂Ȃ�Ȃ��B

            Dim iniInfoAry() As RecDataStructure.DefineInfo = Nothing

            '��`�����擾����
            If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "TrafficData_001", iniInfoAry) = False Then
                AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                Return RecordingResult.IOError
            End If

            'dat�t�@�C���f�[�^�擾
            If GetInfoFromDataFile(sFilePath, sModelCode, iniInfoAry, dlineInfoLst) = False Then
                Return RecordingResult.ParseError
            End If

            '�f�[�^�`�F�b�N
            If CheckData(dlineInfoLst, dlineInfoLstNew, sFilePath, iniInfoAry) = False Then
                Return RecordingResult.IOError
            End If

            '�f�[�^���
            If GetDBInfoFromDataInfo(dlineInfoLstNew, dataInfoLst, iniInfoAry) = False Then
                Return RecordingResult.IOError
            End If

            'DB�o�^
            If BatchAppComm.PutDataToDBCommon(iniInfoAry, dataInfoLst, Tim_TableName) = False Then
                Return RecordingResult.IOError
            End If

            '���������ꍇ
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(Tim_DataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' ���ԑѕʏ�~�f�[�^�̉��
    ''' </summary>
    ''' <param name="sFilePath">�o�^����ׂ��f�[�^���i�[���ꂽ�t�@�C���̐�΃p�X��</param>
    ''' <param name="sClientKind">�N���C�A���g���</param>
    ''' <param name="iniInfoAry">Ini��`���</param>
    ''' <param name="dlineInfoLst">�f�[�^���X�g</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�擾�����d���t�H�[�}�b�g��`���ɂĎ��ԑѕʏ�~�f�[�^����͂���</remarks>
    Private Shared Function GetInfoFromDataFile(ByVal sFilePath As String, _
                                                ByVal sClientKind As String, _
                                                ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByRef dlineInfoLst As List(Of String())) As Boolean
        Dim nHeadSize As Integer = 17
        Dim nDataSize As Integer = 433

        'dat�t�@�C���f�[�^�擾
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, sFilePath, sClientKind, nDataSize, nHeadSize, dlineInfoLst, Tim_DataKind) = False Then
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' ���ԑѕʏ�~�f�[�^�̃`�F�b�N
    ''' </summary>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dlineInfoLstNew">�`�F�b�N��A���m�Idat�t�@�C�����e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C�����`�F�b�N����</remarks>
    Private Shared Function CheckData(ByVal dlineInfoLst As List(Of String()), _
                                      ByRef dlineInfoLstNew As List(Of String()), _
                                      ByVal datFileName As String, _
                                      ByVal iniInfoAry() As RecDataStructure.DefineInfo) As Boolean

        '�@��\���}�X�^SQL
        Dim strSQL As String = "SELECT COUNT(1) FROM V_MACHINE_NOW WHERE RAIL_SECTION_CODE = {0} AND STATION_ORDER_CODE = {1} AND CORNER_CODE = {2}"
        Dim lineInfo(iniInfoAry.Length) As String
        Dim lineInfoNew(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False 'true:�G���[������;false:�G���[���Ȃ�
        Dim dbCtl As New DatabaseTalker
        Dim nRtn As Integer
        Dim ErrNo As Integer
        dlineInfoLstNew = New List(Of String())

        Dim sRail_Code As String = ""
        Dim sStation_Code As String = ""
        Dim sCorner_Code1 As String = ""
        Dim sCorner_Code2 As String = ""

        Try
            dbCtl.ConnectOpen()
            '�S�����R�[�h
            For i As Integer = 0 To dlineInfoLst.Count - 1

                '1���R�[�h�擾
                lineInfo = dlineInfoLst.Item(i)

                '������
                isHaveErr = False
                sRail_Code = ""
                sStation_Code = ""
                sCorner_Code1 = ""
                sCorner_Code2 = ""

                '���ʂ̃`�F�b�N
                If BatchAppComm.CheckDataComm(i + 1, iniInfoAry, lineInfo, datFileName, False) = False Then
                    Continue For
                End If

                'OPT: �ȉ��A�ulineInfo(j).ToString�v�́uToString�v�͖��炩�ɕs�v�B

                '�S���t�B�[���h
                For j As Integer = 0 To iniInfoAry.Length - 1
                    Select Case iniInfoAry(j).FIELD_NAME
                        Case "DATA_KIND" '�f�[�^���
                            If Not lineInfo(j).Equals(Tim_DataKind) Then
                                isHaveErr = True
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (i + 1).ToString, iniInfoAry(j).KOMOKU_NAME))
                                BatchAppComm.SetCollectionData(datFileName, Tim_DataKind) '���W�f�[�^�o�^
                                Exit For
                            End If
                        Case "CORNER_CODE1" '�R�[�i�[�R�[�h
                            If lineInfo(j).Replace("0", "").Length > 0 Then
                                sCorner_Code1 = lineInfo(j).ToString
                            Else
                                sCorner_Code1 = ""
                                ErrNo = j
                            End If

                        Case "CORNER_CODE2" '�R�[�i�[�R�[�h
                            If lineInfo(j).Replace("0", "").Length > 0 Then
                                sCorner_Code2 = lineInfo(j).ToString
                            Else
                                sCorner_Code2 = ""
                            End If
                        Case "STATION_IN" '���v���o��Ґ�
                            If Long.Parse(lineInfo(j)) + Long.Parse(lineInfo(j + 1)) <> Long.Parse(lineInfo(j + 2)) Then
                                Log.Error(String.Format(ERR_MSG_ERRVALUE, Convert.ToString(i + 1), lineInfo(j), lineInfo(j + 1), lineInfo(j + 2)))
                                lineInfo(j + 2) = "0"
                            End If

                        Case "TICKET_NO"
                            lineInfo(j) = "99"

                        Case "DATE"
                            If lineInfo(j).Length >= 8 Then
                                lineInfo(j) = lineInfo(j).Substring(0, 4) & "/" & lineInfo(j).Substring(4, 2) & "/" & lineInfo(j).Substring(6, 2)
                            End If

                        Case "TIME_ZONE"
                            If lineInfo(j).Length >= 4 Then
                                lineInfo(j) = lineInfo(j).Substring(0, 2) & ":" & lineInfo(j).Substring(2, 2)
                            End If

                        Case "RAIL_SECTION_CODE"
                            sRail_Code = lineInfo(j).ToString

                        Case "STATION_ORDER_CODE"
                            sStation_Code = lineInfo(j).ToString

                        Case "UNIT_NO"
                            lineInfo(j) = "0"
                    End Select
                Next

                If sCorner_Code1 = "" And sCorner_Code2 = "" Then
                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, (i + 1).ToString, iniInfoAry(ErrNo).KOMOKU_NAME))
                    BatchAppComm.SetCollectionData(datFileName, Tim_DataKind) '���W�f�[�^�o�^
                    isHaveErr = True
                End If

                '�@��\���}�X�^�`�F�b�N
                If Not sRail_Code = "" AndAlso Not sStation_Code = "" Then
                    '�R�[�i�[1�`�F�b�N
                    If Not sCorner_Code1 = "" Then
                        '�@��\���}�X�^�`�F�b�N�pSQL��
                        nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(String.Format(strSQL, Utility.SetSglQuot(sRail_Code), _
                                                                      Utility.SetSglQuot(sStation_Code), _
                                                                      Utility.SetSglQuot(sCorner_Code1))))
                        If nRtn = 0 Then
                            '���W�f�[�^�o�^���s��
                            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo, _
                                              Lexis.CdtTheCornerNotFound.Gen(sRail_Code, sStation_Code, sCorner_Code1), _
                                              True)
                        End If
                    End If

                    '�R�[�i�[2�`�F�b�N
                    If Not sCorner_Code2 = "" Then
                        '�@��\���}�X�^�`�F�b�N�pSQL��
                        nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(String.Format(strSQL, Utility.SetSglQuot(sRail_Code), _
                                                                      Utility.SetSglQuot(sStation_Code), _
                                                                      Utility.SetSglQuot(sCorner_Code2))))
                        If nRtn = 0 Then
                            '���W�f�[�^�o�^���s��
                            BatchAppComm.SetCollectionData(iniInfoAry, lineInfo, _
                                              Lexis.CdtTheCornerNotFound.Gen(sRail_Code, sStation_Code, sCorner_Code2), _
                                              True)
                        End If
                    End If
                End If

                If isHaveErr = False Then
                    dlineInfoLstNew.Add(lineInfo)
                End If
            Next
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return True

    End Function

    ''' <summary>
    ''' �f�[�^����
    ''' </summary>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dataInfoLst">������̃f�[�^</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>
    ''' �f�[�^����
    ''' </remarks>
    Private Shared Function GetDBInfoFromDataInfo(ByVal dlineInfoLst As List(Of String()), _
                                                  ByRef dataInfoLst As List(Of String()), _
                                                  ByVal iniInfoAry() As RecDataStructure.DefineInfo) As Boolean
        If dlineInfoLst Is Nothing OrElse dlineInfoLst.Count = 0 Then
            Return True
        End If

        Dim TICKET_NO As Integer = 1
        Dim dataInfo() As String
        Dim dlineInfo() As String

        If dataInfoLst Is Nothing Then
            dataInfoLst = New List(Of String())
        Else
            dataInfoLst.Clear()
        End If

        '�f�[�^�ĉ��H
        For i As Integer = 0 To dlineInfoLst.Count - 1
            dataInfo = dlineInfoLst.Item(i)

            Dim sCurCorner As String = ""
            Dim sCurStationIn As String = ""
            Dim sCurStationOut As String = ""
            Dim sCurStationSum As String = ""
            Dim nCurTicketNo As Integer
            Dim nFlag As Integer = 0

            For j As Integer = 0 To iniInfoAry.Length - 1
                Select Case iniInfoAry(j).FIELD_NAME
                    Case "CORNER_CODE1"
                        sCurCorner = Format(CInt(dataInfo(j)), "000")
                        nCurTicketNo = 1
                    Case "CORNER_CODE2"
                        '�R�[�i�[���Z�b�g���ꂽ�ꍇ�̂݌���J�E���g�A�b�v����
                        If dataInfo(j).Replace("0", "").Length > 0 Then
                            sCurCorner = Format(CInt(dataInfo(j)), "000")
                            nCurTicketNo = 1
                        Else
                            nFlag = 0
                            Exit For
                        End If
                    Case "STATION_IN"
                        sCurStationIn = dataInfo(j)
                        nFlag = nFlag + 1
                    Case "STATION_OUT"
                        sCurStationOut = dataInfo(j)
                        nFlag = nFlag + 1
                    Case "STATION_SUM"
                        sCurStationSum = dataInfo(j)
                        nFlag = nFlag + 1
                End Select

                '�f�[�^��ǉ�����B
                If nFlag = 3 Then
                    ReDim dlineInfo(13)
                    For k As Integer = 0 To 8
                        dlineInfo(k) = dataInfo(k)
                    Next
                    dlineInfo(9) = sCurCorner
                    dlineInfo(10) = CStr(nCurTicketNo)
                    dlineInfo(11) = sCurStationIn
                    dlineInfo(12) = sCurStationOut
                    dlineInfo(13) = sCurStationSum

                    If dlineInfo(9).Replace("0", "").Length > 0 Then
                        dataInfoLst.Add(dlineInfo)
                    End If

                    '�N���A
                    nFlag = 0

                    '����
                    nCurTicketNo = nCurTicketNo + 1
                End If
            Next
        Next

        '�R�[�i�[�ύX
        For j As Integer = 0 To iniInfoAry.Length - 1
            Select Case iniInfoAry(j).FIELD_NAME
                Case "CORNER_CODE1", "CORNER_CODE2"
                    iniInfoAry(j).FIELD_NAME = "CORNER_CODE"
            End Select
        Next
        Return True

    End Function

#End Region

End Class
