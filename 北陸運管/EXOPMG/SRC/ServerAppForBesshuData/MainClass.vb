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
Imports System.Diagnostics
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports System.Text
Imports JR.ExOpmg.ServerApp

''' <summary>
''' �ʏW�D�f�[�^�o�^�v���Z�X���ʂ̃��C����������������N���X�B
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "�錾�̈�iPrivate�j"

    Private Const DataLength As Integer = 111              '�f�[�^����
    Private Const HeadLength As Integer = 17                 '�w�b�_����
    Private Const DATA_KIND As String = "A1"                 '�f�[�^���

    Private Shared iniInfoAry() As RecDataStructure.DefineInfo

#End Region

#Region "���\�b�h�iMain�j"
    ''' <summary>
    '''  �ʏW�D�f�[�^�o�^�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    '''  �ʏW�D�f�[�^�o�^�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForBesshuData")
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

                Log.Init(sLogBasePath, "ForBesshuData")
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

                If DefineInfoShutoku.GetDefineInfo(Config.FormatFilePath, "BesshuData_001", iniInfoAry) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

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
    '''   �ʏW�D�f�[�^�捞
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

            Dim dlineInfoLst As New List(Of String())
            Dim dlineInfoLstNew As New List(Of String())

            'dat�t�@�C���f�[�^�擾
            If GetInfoFromDataFile(sFilePath, sModelCode, DATA_KIND, dlineInfoLst) = False Then
                Return RecordingResult.ParseError
            End If
            '�`�F�b�N
            If CheckData(dlineInfoLst, dlineInfoLstNew, sFilePath) = False Then
                Return RecordingResult.IOError
            End If
            'DB�o�^
            If BatchAppComm.PutDataToDBCommon(iniInfoAry, dlineInfoLstNew, "D_BESSHU_DATA") = False Then
                Return RecordingResult.IOError
            End If

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

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' DAT�t�@�C���̉��
    ''' </summary>
    ''' <param name="sFilePath">�o�^����ׂ��f�[�^���i�[���ꂽ�t�@�C���̐�΃p�X��</param>
    ''' <param name="sModelCode">�@��R�[�h</param>
    ''' <param name="sDataKind">�f�[�^���</param>
    ''' <param name="lineInfoLst">�擾�������</param>
    ''' <returns>True:����/False:�ُ�</returns>
    Public Shared Function GetInfoFromDataFile(ByVal sFilePath As String, _
                                               ByVal sModelCode As String, _
                                               ByVal sDataKind As String, _
                                               ByRef lineInfoLst As List(Of String())) As Boolean
        Dim info() As String                            '�P���R�[�h
        Dim isWtn As Boolean = False
        Dim nWtn As Integer = 0
        Dim nCtn As Integer = 0
        Dim ticketCnt As String = "" '���o����
        Dim nTnt As Integer = 0   '�s��

        '�S�����R�[�h
        Dim lineInfoLstOld As New List(Of String())
        If lineInfoLst Is Nothing Then
            lineInfoLst = New List(Of String())
        Else
            lineInfoLst.Clear()
        End If

        '�f�[�^���擾����B
        lineInfoLst = New List(Of String())
        If BatchAppComm.GetInfoFromDataFileComm(iniInfoAry, sFilePath, sModelCode, _
                                                   DataLength, HeadLength, lineInfoLstOld, sDataKind) = False Then
            Return False
        End If

        For i As Integer = 0 To lineInfoLstOld.Count - 1
            isWtn = False
            nWtn = 0
            For j As Integer = 0 To iniInfoAry.Length - 1
                '���o������ݒ肷��
                If iniInfoAry(j).FIELD_NAME = "TICKET_CNT" Then
                    If isWtn = False Then
                        nWtn = j
                        isWtn = True
                    End If
                    ticketCnt = lineInfoLstOld(i)(j)
                    If OPMGUtility.checkNumber(ticketCnt) = False Then
                        Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, CStr(i + 1), iniInfoAry(j).KOMOKU_NAME))
                        BatchAppComm.SetCollectionData(sFilePath, DATA_KIND) '�t�@�C�������
                        Exit For
                    End If
                    If Integer.Parse(ticketCnt) = 0 Then
                        Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, CStr(i + 1), iniInfoAry(j).KOMOKU_NAME))
                        BatchAppComm.SetCollectionData(iniInfoAry, lineInfoLstOld(i)) '�t�@�C�������
                        Exit For
                    ElseIf CInt(ticketCnt) > 4 Then
                        ticketCnt = CStr(4)
                    End If
                    nTnt = 1
                End If

                '1�s��
                If iniInfoAry(j).FIELD_NAME = "TICKET_NO" Then
                    '���o����
                    If nTnt > CInt(ticketCnt) Then
                        Exit For
                    Else
                        '�N���A
                        ReDim info(nWtn + 5)
                        '��{�w�b�_��ݒ肷��
                        For k As Integer = 0 To nWtn - 1
                            info(k) = lineInfoLstOld(i)(k)
                        Next
                        '���ڂ�ݒ肷��
                        For nCtn = nWtn To nWtn + 5
                            '���o����
                            If iniInfoAry(nCtn).FIELD_NAME = "TICKET_CNT" Then
                                info(nCtn) = CStr(nTnt)
                                Continue For
                            End If
                            '�s��
                            If iniInfoAry(nCtn).FIELD_NAME = "BESSYU_CNT" Then
                                info(nCtn) = CStr(i + 1)
                                Continue For
                            End If
                            info(nCtn) = lineInfoLstOld(i)((nTnt - 1) * 6 + nCtn)
                        Next
                        lineInfoLst.Add(info)
                        nTnt += 1
                    End If
                End If
            Next
        Next
        Return True
    End Function

    ''' <summary>
    ''' �ʏW�D�f�[�^�̃`�F�b�N
    ''' </summary>
    ''' <param name="dlineInfoLst">dat�t�@�C�����e</param>
    ''' <param name="dlineInfoLstNew">�`�F�b�N��A���m�Idat�t�@�C�����e</param>
    '''  <param name="datFileName">�f�[�^FileName</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Public Shared Function CheckData(ByVal dlineInfoLst As List(Of String()), _
                                     ByRef dlineInfoLstNew As List(Of String()), _
                                     ByVal datFileName As String) As Boolean

        Dim bRtn As Boolean = True
        Dim i As Integer = 0
        Dim lineInfo(iniInfoAry.Length) As String
        Dim isHaveErr As Boolean = False      'true:�G���[������;false:�G���[���Ȃ�
        Dim intData As Integer = 0
        dlineInfoLstNew = New List(Of String())

        '�S�����R�[�h
        For i = 0 To dlineInfoLst.Count - 1

            '1���R�[�h�擾
            lineInfo = dlineInfoLst.Item(i)

            isHaveErr = False

            For j As Integer = 0 To iniInfoAry.Length - 1

                Select Case iniInfoAry(j).FIELD_NAME
                    Case "DATA_KIND" '�f�[�^���
                        If (Not lineInfo(j) = DATA_KIND) Then
                            isHaveErr = True
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, (i + 1).ToString, "�f�[�^���"))
                            BatchAppComm.SetCollectionData(datFileName, DATA_KIND) '�t�@�C�������
                            Exit For
                        End If
                    Case "TICKET"  '��������
                        If Integer.TryParse(lineInfo(j), intData) = False Then
                            lineInfo(i) = CStr(0)
                            Continue For
                        End If
                End Select
            Next

            If isHaveErr = False Then
                '���ʂ̃`�F�b�N
                If BatchAppComm.CheckDataComm(i + 1, iniInfoAry, lineInfo, datFileName) = False Then
                    Continue For
                End If

                dlineInfoLstNew.Add(lineInfo)
            End If
        Next
        Return True

    End Function

#End Region

End Class
