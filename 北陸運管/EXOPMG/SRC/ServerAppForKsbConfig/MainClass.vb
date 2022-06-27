' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/06/01       ����  �k���E���ڊg���Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading
Imports System.Diagnostics
Imports JR.ExOpmg.ServerApp
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp.RecDataStructure
Imports System.Text

''' <summary>
''' �Ď��Րݒ�f�[�^�o�^�v���Z�X���ʂ̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "�錾�̈�iPrivate�j"
    Private Shared iniInfoAry() As RecDataStructure.DefineInfo
    Private Const DATA_KIND As String = "54"   '�f�[�^���
    '----------- 0.1  �k���E���ڊg���Ή�   ADD  START------------------------
    Private Shared iniInfoOldAry() As RecDataStructure.DefineInfo
    Private Shared dataLen As Integer
    Private Const OldLen As Integer = 672
    Private Const NewLen As Integer = 864
    '----------- 0.1  �k���E���ڊg���Ή�   ADD    END------------------------
#End Region

#Region "���\�b�h�iMain�j"
    ''' <summary>
    ''' �Ď��Րݒ�f�[�^�o�^�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' �Ď��Րݒ�f�[�^�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForKsbConfig")
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

                Log.Init(sLogBasePath, "ForKsbConfig")
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

                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "KansibanSetInfo_001", iniInfoAry) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If
                '----------- 0.1  �k���E���ڊg���Ή�   ADD  START------------------------
                If DefineInfoShutoku.GetDefineInfo(Config.FormatOldFilePath, "KansibanSetInfo_001", iniInfoOldAry) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If
                '----------- 0.1  �k���E���ڊg���Ή�   ADD    END------------------------

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


    ''' <summary>
    '''  �Ď��Րݒ�f�[�^�捞
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
            Dim dt As DataTable = Nothing
            '----------- 0.1  �k���E���ڊg���Ή�   MOD  START------------------------
            Dim fileStream As FileStream
            Try
                '�t�@�C���X�g���[�����擾
                fileStream = New FileStream(sFilePath, FileMode.Open)
                dataLen = CInt(fileStream.Length) - 1
            Catch ex As Exception
                '�t�@�C���X�g���[�������
                Log.Fatal("Unwelcome Exception caught.", ex)
                Return RecordingResult.IOError
            End Try
            fileStream.Close()
            '�t�@�C���T�C�Y���P���R�[�h���ɖ����Ȃ��ꍇ
            If dataLen = NewLen Then
                'dat�t�@�C���f�[�^�擾
                If GetInfoFromDataFileComm(iniInfoAry, sFilePath, sModelCode, dlineInfoLst) = False Then
                    Return RecordingResult.ParseError
                End If
                '�`�F�b�N
                If CheckData(iniInfoAry, dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If
                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(iniInfoAry, dlineInfoLstNew, "D_KSB_CONFIG") = False Then
                    Return RecordingResult.IOError
                End If
            ElseIf dataLen = OldLen Then
                'dat�t�@�C���f�[�^�擾
                If GetInfoFromDataFileComm(iniInfoOldAry, sFilePath, sModelCode, dlineInfoLst) = False Then
                    Return RecordingResult.ParseError
                End If
                '�`�F�b�N
                If CheckData(iniInfoOldAry, dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                    Return RecordingResult.IOError
                End If
                'DB�o�^
                If BatchAppComm.PutDataToDBCommon(iniInfoOldAry, dlineInfoLstNew, "D_KSB_CONFIG") = False Then
                    Return RecordingResult.IOError
                End If
            Else
                Return RecordingResult.IOError
            End If
            '----------- 0.1  �k���E���ڊg���Ή�   MOD    END------------------------
            '���������ꍇ
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(DATA_KIND, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function
#End Region

#Region "���\�b�h�iPrivate�j"

    ''' <summary>
    ''' DAT�t�@�C���̉��
    ''' </summary>
    ''' <param name="iniInfoAry">INI�t�@�C���̓��e</param>
    ''' <param name="datFileName">dat�t�@�C����</param>
    ''' <param name="clientKind">�f�[�^Index</param>
    ''' <param name="lineInfoLst">��͂����f�[�^</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Private Shared Function GetInfoFromDataFileComm(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                ByVal datFileName As String, _
                                                ByVal clientKind As String, _
                                                ByRef lineInfoLst As List(Of String())) As Boolean
        '�f�[�^Index
        Dim dataIndex As String = Nothing
        Dim info(iniInfoAry.Length - 1) As String
        Dim uNoName As String = Nothing '���@�ԍ�
        '�w�b�h��
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        Dim lineInfo() As String
        '�S�����R�[�h
        Dim lineInfoLstOld As New List(Of String())
        If lineInfoLst Is Nothing Then
            lineInfoLst = New List(Of String())
        Else
            lineInfoLst.Clear()
        End If
        '----------- 0.1  �k���E���ڊg���Ή�   MOD  START------------------------
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, datFileName, clientKind, dataLen, 1, lineInfoLstOld, DATA_KIND) = False Then
            Return False
        End If
        '----------- 0.1  �k���E���ڊg���Ή�   MOD  START------------------------

        If lineInfoLstOld.Count <= 0 Then
            Return True
        End If

        '�t�@�C�����̉��
        If GetBaseInfo(headInfo, datFileName) = False Then
            Return False
        End If

        ReDim lineInfo(iniInfoAry.Length - 1)
        For Each lineInfo In lineInfoLstOld

            For k As Integer = 0 To iniInfoAry.Length - 1
                If iniInfoAry(k).FIELD_NAME = "UNIT_NO" Then
                    dataIndex = lineInfo(k)  '�e�G���A�̍��@�ԍ�
                    uNoName = iniInfoAry(k).KOMOKU_NAME   '���@
                    Exit For
                End If
            Next

            For i As Integer = 0 To dataIndex.Length - 1 Step 2
                If dataIndex.Substring(i, 2) = "00" Then  '�����f�[�^
                    Continue For
                End If
                ReDim info(iniInfoAry.Length - 1)
                For j As Integer = 0 To iniInfoAry.Length - 1
                    Select Case UCase(iniInfoAry(j).FIELD_NAME)
                        Case "RAIL_SECTION_CODE"
                            info(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE   '����R�[�h
                            Continue For
                        Case "STATION_ORDER_CODE"
                            info(j) = headInfo.STATION_CODE.STATION_ORDER_CODE    '�w���R�[�h
                            Continue For
                        Case "CORNER_CODE"
                            info(j) = headInfo.CORNER_CODE    '�R�[�i�[
                            Continue For
                        Case "SYUSYU_DATE"
                            info(j) = headInfo.PROCESSING_TIME   '���W����
                            Continue For
                        Case "MODEL_CODE" '�@��
                            info(j) = "G"
                            Continue For
                    End Select
                    If CInt(iniInfoAry(j).PARA6) = 1 AndAlso
                        lineInfo(j).Length >= i + 2 Then
                        info(j) = lineInfo(j).Substring(i, 2) '���@�ʃX�e�[�^�X
                        If iniInfoAry(j).FIELD_NAME = "UNIT_NO" Then  '���@�ԍ�
                            If (OPMGUtility.checkNumber(info(j)) = True AndAlso CInt(info(j)) >= 10) _
                               OrElse OPMGUtility.checkNumber(info(j)) = False Then
                                Dim uNo As String = "&H" & info(j)
                                info(j) = CInt(uNo).ToString
                            End If
                        End If
                    ElseIf CInt(iniInfoAry(j).PARA6) = 2 AndAlso
                       lineInfo(j).Length >= i + 2 Then
                        info(j) = lineInfo(j).Substring(i + 1, 1) '�ʘH�ݒ�
                    ElseIf CInt(iniInfoAry(j).PARA6) = 3 AndAlso
                        lineInfo(j).Length >= i + 2 Then
                        info(j) = lineInfo(j).Substring(i, 1) '�ʘH
                    Else
                        info(j) = lineInfo(j)
                    End If
                Next
                lineInfoLst.Add(info)
            Next
        Next
        If lineInfoLst.Count <= 0 Then
            Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, "1", uNoName))

            '���W�f�[�^�̓o�^
            BatchAppComm.SetCollectionData(datFileName, DATA_KIND)
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' ��{�w�b�_�����̉��
    ''' </summary>
    ''' <param name="infoObj">��͂������ʂ�ۑ��p</param> 
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Private Shared Function GetBaseInfo(ByRef infoObj As BaseInfo, _
                                       ByVal fileName As String) As Boolean
        Try
            Dim code As EkCode = UpboundDataPath.GetEkCode(fileName)
            '����
            infoObj.STATION_CODE.RAIL_SECTION_CODE = code.RailSection.ToString("D3")
            '�w��
            infoObj.STATION_CODE.STATION_ORDER_CODE = code.StationOrder.ToString("D3")
            '�R�[�i�[
            infoObj.CORNER_CODE = code.Corner.ToString("D4")
            '��������
            infoObj.PROCESSING_TIME = UpboundDataPath.GetTimestamp(fileName).ToString()

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function


    ''' <summary>
    ''' �Ď��Րݒ�f�[�^�̃`�F�b�N
    ''' </summary>
    ''' <param name="iniInfoAry">ini�t�@�C��</param>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dlineInfoLstNew">�`�F�b�N��A���m�Idat�t�@�C�����e</param>
    ''' <param name="datFileName">�t�@�C����</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Private Shared Function CheckData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                     ByVal dlineInfoLst As List(Of String()), _
                                     ByRef dlineInfoLstNew As List(Of String()), _
                                     ByVal datFileName As String) As Boolean

        If dlineInfoLst.Count <= 0 Then Return True

        Dim lineInfo(iniInfoAry.Length) As String '1���R�[�h
        Dim iFlag As Integer = 145
        If dlineInfoLstNew Is Nothing Then
            dlineInfoLstNew = New List(Of String())
        Else
            dlineInfoLstNew.Clear()
        End If

        For j As Integer = 0 To dlineInfoLst.Count - 1

            '1���R�[�h�擾
            lineInfo = dlineInfoLst.Item(j)

            '�S���t�B�[���h
            For i As Integer = 0 To iniInfoAry.Length - 1

                If iFlag = 0 Then Exit For

                Select Case UCase(iniInfoAry(i).FIELD_NAME)
                    Case "DATA_KIND" '�f�[�^���
                        If (Not lineInfo(i) = DATA_KIND) Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, "�f�[�^���"))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '���W�f�[�^�̓o�^
                            Return False
                        End If
                End Select
                '����t���[�ݒ�P�`����t���[�ݒ�X�A���ׂ�o��t���[�ݒ�P�`���ׂ�o��t���[�ݒ�X
                '���D�@�����ݒ�ON�A���D�@�����ݒ�OFF
                Select Case UCase(iniInfoAry(i).PARA6)

                    Case "5"    '�J�n�@�N           
                        If Integer.Parse(lineInfo(i)) > 99 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '���W�f�[�^�̓o�^
                            Return False
                        End If
                    Case "6"    '�J�n�@��
                        If Integer.Parse(lineInfo(i)) > 12 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '���W�f�[�^�̓o�^
                            Return False
                        End If
                    Case "7"    '�J�n�@��
                        If Integer.Parse(lineInfo(i)) > 31 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '���W�f�[�^�̓o�^
                            Return False
                        End If
                    Case "8"    '�J�n�@���A�ݒ�i���j
                        If Integer.Parse(lineInfo(i)) > 23 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '���W�f�[�^�̓o�^
                            Return False
                        End If
                    Case "9"    '�ݒ�i���j
                        If Integer.Parse(lineInfo(i)) > 59 Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (j + 1).ToString, iniInfoAry(i).KOMOKU_NAME))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '���W�f�[�^�̓o�^
                            Return False
                        End If
                End Select
            Next
            '���ʂ̃`�F�b�N
            If BatchAppComm.CheckDataComm(j + 1, iniInfoAry, lineInfo, datFileName, True, False, True) = False Then
                Continue For
            End If
            dlineInfoLstNew.Add(lineInfo)
        Next

        Return True
    End Function
#End Region
End Class
