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

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp

''' <summary>
''' �{�v���Z�X�́A���W�����s����Ԍ��o�f�[�^�A���s�˔j���o�f�[�^�A
''' ���������o�f�[�^�AFREX������h�c���o�f�[�^����͂��A�^�p�Ǘ��T�[�o��DB�ɓo�^����B
''' </summary>
Public Class MainClass
    Inherits RecServerAppBaseMainClass

#Region "�錾�̈�iPrivate�j"

    Private Shared ERR_MSG_WTN As String = "{0}�s�ڂ̕s������Ώۋ敪������܂���"
    Private Shared ERR_Wrong_WTN As String = "{0}�s�ڂ̕s������Ώۋ敪���s���ł�"
    Private Shared ERR_MSG_TN As String = "{0}�s�ڂ̌���ԍ�������܂���"
    Private Shared ERR_Wrong_TN As String = "{0}�s�ڂ̌���ԍ����s���ł�"
    Private Shared ERR_MSG_ID As String = "{0}�s�ڂ�ID�ԍ�������܂���"

    Private Const MeisaiLength As Integer = 111              '�f�[�^����
    Private Const HeadLength As Integer = 17                 '�w�b�_����
    Private Const FuseiJoshaDataKind As String = "A2"        '�s����Ԍ��o�f�[�^�̃f�[�^���
    Private Const KyokoToppaDataKind As String = "A3"        '���s�˔j���o�f�[�^�̃f�[�^���
    Private Const FunshitsuDataKind As String = "A4"         '���������o�f�[�^�̃f�[�^���
    Private Const FrexDataKind As String = "A5"              'FREX������h�c���o�f�[�^�̃f�[�^���

    '�e�[�u����
    Private Const Fuseijyosha_TableName As String = "D_FUSEI_JOSHA_DATA"
    Private Const Kyokotopa_TableName As String = "D_KYOKO_TOPPA_DATA"
    Private Const Funshitsu_TableName As String = "D_FUNSHITSU_DATA"

    Private Shared defineInfo_FuseiJosha() As RecDataStructure.DefineInfo = Nothing  '��`���
    Private Shared defineInfo_KyokoToppa() As RecDataStructure.DefineInfo = Nothing '��`���
    Private Shared defineInfo_Funshitsu() As RecDataStructure.DefineInfo = Nothing '��`���
    Private Shared defineInfo_Frex() As RecDataStructure.DefineInfo = Nothing '��`���

    Private Shared lstFuseiJoshaData As New List(Of String())
    Private Shared lstKyokoToppaData As New List(Of String())
    Private Shared lstFunshitsuData As New List(Of String())
    Private Shared lstFrexData As New List(Of String())
#End Region

#Region "Main"
    ''' <summary>
    ''' ���׃f�[�^�o�^�v���Z�X�̃��C�������B
    ''' </summary>
    ''' <remarks>
    ''' ���׃f�[�^�o�^�v���Z�X�̃G���g���|�C���g�ł���B
    ''' </remarks>
    <STAThread()> _
    Public Shared Sub Main()
        Dim m As New Mutex(False, "ExOpmgServerAppForMeisaiData")
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

                Log.Init(sLogBasePath, "ForMeisaiData")
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

                '�s����Ԍ��o�f�[�^�̒�`�����擾����B
                defineInfo_FuseiJosha = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FuseiJoshaFormatFilePath, "FuseiJosha_001", defineInfo_FuseiJosha) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                '���s�˔j�����f�[�^�̒�`�����擾����B
                defineInfo_KyokoToppa = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.KyokoToppaFormatFilePath, "KyokoToppa_001", defineInfo_KyokoToppa) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                '���������o�f�[�^�̒�`�����擾����B
                defineInfo_Funshitsu = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FunshitsuFormatFilePath, "Funshitsu_001", defineInfo_Funshitsu) = False Then
                    AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
                    Return
                End If

                'FREX�����ID�����f�[�^�̒�`�����擾����B
                defineInfo_Frex = Nothing
                If DefineInfoShutoku.GetDefineInfo(Config.FrexFormatFilePath, "Funshitsu_001", defineInfo_Frex) = False Then
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
#End Region

#Region "Private"
    ''' <summary>
    ''' ���׃f�[�^�o�^�����B
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
            Dim lstChkData As New List(Of String())                     '�`�F�b�N�����f�[�^���
            Dim lstDBData As New List(Of String())                      'DB�ɓo�^����f�[�^���
            lstFuseiJoshaData.Clear()
            lstKyokoToppaData.Clear()
            lstFunshitsuData.Clear()
            lstFrexData.Clear()
            '���׃f�[�^�̉��
            If GetInfoFromDataFile(sFilePath, sModelCode) = False Then
                Return RecordingResult.ParseError
            End If

            '-----------------------------�s����Ԍ��o�f�[�^�捞���� Start-----------------------------
            '�`�F�b�N���s���B
            lstChkData = New List(Of String())
            If CheckData(defineInfo_FuseiJosha, lstFuseiJoshaData, FuseiJoshaDataKind, sFilePath, lstChkData) = True Then
                If lstChkData IsNot Nothing AndAlso lstChkData.Count > 0 Then
                    '�擾�����f�[�^��DB�o�^�f�[�^�ɍĉ��H����B
                    lstDBData = New List(Of String())
                    If GetDBInfoFromDataInfo(defineInfo_FuseiJosha, FuseiJoshaDataKind, lstChkData, lstDBData) = False Then
                        Return RecordingResult.ParseError
                    End If
                    'DB�Ƀf�[�^��o�^����B
                    If BatchAppComm.PutDataToDBCommon(defineInfo_FuseiJosha, lstDBData, Fuseijyosha_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------�s����Ԍ��o�f�[�^�捞���� End-----------------------------

            '-----------------------------���s�˔j���o�f�[�^�捞���� Start-----------------------------
            '�`�F�b�N���s���B
            lstDBData = New List(Of String())
            If CheckData(defineInfo_KyokoToppa, lstKyokoToppaData, KyokoToppaDataKind, sFilePath, lstDBData) = True Then
                If lstDBData IsNot Nothing AndAlso lstDBData.Count > 0 Then
                    'DB�Ƀf�[�^��o�^����B
                    If BatchAppComm.PutDataToDBCommon(defineInfo_KyokoToppa, lstDBData, Kyokotopa_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------���s�˔j���o�f�[�^�捞���� End-----------------------------

            '-----------------------------���������o�f�[�^�捞���� Start-----------------------------
            '�`�F�b�N���s���B
            lstDBData = New List(Of String())
            If CheckData(defineInfo_Funshitsu, lstFunshitsuData, FunshitsuDataKind, sFilePath, lstDBData) = True Then
                If lstDBData IsNot Nothing AndAlso lstDBData.Count > 0 Then
                    'DB�Ƀf�[�^��o�^����B
                    If BatchAppComm.PutDataToDBCommon(defineInfo_Funshitsu, lstDBData, Funshitsu_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------���������o�f�[�^�捞���� End-----------------------------

            '-----------------------------FREX�����ID���o�f�[�^�捞���� Start-----------------------------
            '�`�F�b�N���s���B
            lstChkData = New List(Of String())
            If CheckData(defineInfo_Frex, lstFrexData, FrexDataKind, sFilePath, lstChkData) = True Then
                If lstChkData IsNot Nothing AndAlso lstChkData.Count > 0 Then
                    '�擾�����f�[�^��DB�o�^�f�[�^�ɍĉ��H����B
                    lstDBData = New List(Of String())
                    If GetDBInfoFromDataInfo(defineInfo_Frex, FrexDataKind, lstChkData, lstDBData) = False Then
                        Return RecordingResult.ParseError
                    End If
                    'DB�Ƀf�[�^��o�^����B
                    If BatchAppComm.PutDataToDBCommon(defineInfo_Frex, lstDBData, Funshitsu_TableName) = False Then
                        Return RecordingResult.IOError
                    End If
                End If
            End If
            '-----------------------------FREX�����ID���o�f�[�^�捞���� End-----------------------------

            '�o�^�����������ꍇ
            Return RecordingResult.Success
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: ���׃f�[�^�Ɋւ����͎��s�̃t�@�C����ʂ́A��ɕs����Ԍ��o�f�[�^�̎�ʂƂ���B
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(FuseiJoshaDataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return RecordingResult.IOError
        End Try
    End Function

    ''' <summary>
    ''' �擾�����t�H�[�}�b�g��`���ɂăo�C�i���t�@�C������͂��A
    ''' �o�^�f�[�^�Ƃ��ă������ɕێ�����B
    ''' </summary>
    ''' <param name="sFilePath">�o�^����ׂ��f�[�^���i�[���ꂽ�t�@�C���̐�΃p�X��</param>
    ''' <param name="sModelCode">�@��R�[�h</param>
    ''' <returns>True:����/False:�ُ�</returns>
    Private Shared Function GetInfoFromDataFile(ByVal sFilePath As String, _
                                                ByVal sModelCode As String) As Boolean

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

        Try
            '�t�@�C���X�g���[�����擾
            fileStream = New FileStream(sFilePath, FileMode.Open)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            '�����`�F�b�N
            If fileStream.Length < (MeisaiLength + HeadLength) Then
                Log.Error(RecAppConstants.ERR_TOO_SHORT_FILE)
                Return False
            End If

            '���M����
            If fileStream.Length > (MeisaiLength + HeadLength) Then 'ftp�̏ꍇ
                iStarRecIndex = 1
            Else 'socket�̏ꍇ
                iStarRecIndex = 0
            End If

            '���R�[�h���擾
            If fileStream.Length Mod (MeisaiLength + HeadLength) = 0 Then
                iRecCnt = CInt(fileStream.Length / (MeisaiLength + HeadLength))
            Else
                iRecCnt = CInt(Int(fileStream.Length / (MeisaiLength + HeadLength)))
            End If

            '�ǃt�@�C��
            For i As Integer = iStarRecIndex To iRecCnt - 1

                ReDim bData(MeisaiLength + HeadLength) '1���R�[�h�̃f�[�^

                '���
                fileStream.Seek(i * (MeisaiLength + HeadLength), SeekOrigin.Begin)
                fileStream.Read(bData, 0, MeisaiLength + HeadLength)

                headInfo = Nothing
                BinaryHeadInfoParse.GetBaseInfo(bData, sModelCode, headInfo)

                If headInfo.DATA_KIND = FuseiJoshaDataKind Then
                    '�s����Ԍ����f�[�^�̉��
                    ReDim sArrRecord(defineInfo_FuseiJosha.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_FuseiJosha, bData, headInfo, sArrRecord) = False Then
                        '���W�f�[�^�̓o�^
                        BatchAppComm.SetCollectionData(headInfo, FuseiJoshaDataKind)
                        Continue For
                    End If

                    '�d���ł̍s�ڂ�ݒ肷��
                    sArrRecord(defineInfo_FuseiJosha.Length - 1) = CStr(i + 1)

                    '��͂����f�[�^��ݒ肷��
                    lstFuseiJoshaData.Add(sArrRecord)

                ElseIf headInfo.DATA_KIND = KyokoToppaDataKind Then
                    '���s�˔j���o�f�[�^�̉��
                    ReDim sArrRecord(defineInfo_KyokoToppa.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_KyokoToppa, bData, headInfo, sArrRecord) = False Then
                        '���W�f�[�^�̓o�^
                        BatchAppComm.SetCollectionData(headInfo, KyokoToppaDataKind)
                        Continue For
                    End If

                    '�d���ł̍s�ڂ�ݒ肷��
                    sArrRecord(defineInfo_KyokoToppa.Length - 1) = CStr(i + 1)

                    '��͂����f�[�^��ݒ肷��
                    lstKyokoToppaData.Add(sArrRecord)

                ElseIf headInfo.DATA_KIND = FunshitsuDataKind Then
                    '���������o�f�[�^�̉��
                    ReDim sArrRecord(defineInfo_Funshitsu.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_Funshitsu, bData, headInfo, sArrRecord) = False Then
                        '���W�f�[�^�̓o�^
                        BatchAppComm.SetCollectionData(headInfo, FunshitsuDataKind)
                        Continue For
                    End If

                    '�d���ł̍s�ڂ�ݒ肷��
                    sArrRecord(defineInfo_Funshitsu.Length - 1) = CStr(i + 1)

                    '��͂����f�[�^��ݒ肷��
                    lstFunshitsuData.Add(sArrRecord)

                ElseIf headInfo.DATA_KIND = FrexDataKind Then
                    'Frex��������o�f�[�^�̉��
                    ReDim sArrRecord(defineInfo_Frex.Length - 1)
                    If BatchAppComm.GetRecDataComm(defineInfo_Frex, bData, headInfo, sArrRecord) = False Then
                        '���W�f�[�^�̓o�^
                        BatchAppComm.SetCollectionData(headInfo, FrexDataKind)
                        Continue For
                    End If

                    '�d���ł̍s�ڂ�ݒ肷��
                    sArrRecord(defineInfo_Frex.Length - 1) = CStr(i + 1)

                    '��͂����f�[�^��ݒ肷��
                    lstFrexData.Add(sArrRecord)
                Else
                    '�f�[�^��ʂ��s�����O���o�͂���
                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, i + 1, "�f�[�^���"))
                    '���W�f�[�^�̓o�^
                    BatchAppComm.SetCollectionData(sFilePath, headInfo.DATA_KIND)
                End If
            Next
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: ���׃f�[�^�Ɋւ����͎��s�̃t�@�C����ʂ́A��ɕs����Ԍ��o�f�[�^�̎�ʂƂ���B
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(FuseiJoshaDataKind, Path.GetFileNameWithoutExtension(sFilePath)))

            Return False
        Finally
            '�t�@�C���X�g���[�������
            fileStream.Close()
        End Try

        Return True
    End Function

    ''' <summary>
    ''' �f�[�^�`�F�b�N
    ''' </summary>
    ''' <param name="defineInfo">��`���</param>
    ''' <param name="lstDataFrom">�f�[�^���</param>
    ''' <param name="sDataKind">�f�[�^���</param>
    ''' <param name="refLstData">�`�F�b�N�����f�[�^���</param>
    ''' <returns>True:���� False:�`�F�b�N�G���[</returns>
    Private Shared Function CheckData(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                      ByVal lstDataFrom As List(Of String()), _
                                      ByVal sDataKind As String, _
                                      ByVal sFileName As String, _
                                      ByRef refLstData As List(Of String())) As Boolean

        If lstDataFrom Is Nothing OrElse lstDataFrom.Count <= 0 Then Return True

        Dim bRtn As Boolean = True

        Dim sArrData(defineInfo.Length) As String
        Dim isHaveErr As Boolean     'False:�`�F�b�NOK True:�`�F�b�N�ُ�
        Dim iFlag As Integer
        Dim iLineNo As Integer
        refLstData = New List(Of String())

        For i As Integer = 0 To lstDataFrom.Count - 1

            isHaveErr = False

            '�Y�����R�[�h���擾����
            sArrData = lstDataFrom.Item(i)

            '�d���ł̍s�ڂ��擾����
            If OPMGUtility.checkNumber(sArrData(sArrData.Length - 1)) Then
                iLineNo = CInt(sArrData(sArrData.Length - 1))
            Else
                iLineNo = i + 1
            End If

            '���ʂ̃`�F�b�N���s��
            If BatchAppComm.CheckDataComm(iLineNo, defineInfo, sArrData, sFileName) = False Then
                Continue For
            End If

            '���ʂȃ`�F�b�N
            Select Case sDataKind
                Case FuseiJoshaDataKind
                    '�s����Ԍ��o�f�[�^�̃`�F�b�N
                    iFlag = 10
                    Dim iErrCnt As Integer = 0
                    For j As Integer = 0 To defineInfo.Length - 1
                        Select Case defineInfo(j).FIELD_NAME
                            Case "WRANG_TARGET_NO"
                                iFlag = iFlag - 1
                                '�s������Ώۋ敪�̃`�F�b�N
                                If OPMGUtility.checkNumber(sArrData(j)) = False Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_Wrong_WTN, iLineNo))
                                    '���W�f�[�^�o�^���s��
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                                'Null�`�F�b�N
                                If sArrData(j).Replace("0", "").Length <= 0 Then
                                    iErrCnt = iErrCnt + 1
                                End If
                        End Select
                        '�`�F�b�N�����̏ꍇ�A���~
                        If iFlag = 0 Then Exit For
                    Next
                    'Null�`�F�b�N
                    If iErrCnt = 10 Then
                        isHaveErr = True
                        Log.Error(String.Format(ERR_MSG_WTN, iLineNo))
                        '���W�f�[�^�o�^���s��
                        BatchAppComm.SetCollectionData(defineInfo, sArrData)
                        Exit For
                    End If
                Case FunshitsuDataKind
                    '���������o�f�[�^�̃`�F�b�N
                    iFlag = 2
                    For j As Integer = 0 To defineInfo.Length - 1
                        Select Case defineInfo(j).FIELD_NAME
                            Case "TICKET_NO"
                                iFlag = iFlag - 1
                                '����ԍ��̃`�F�b�N
                                If OPMGUtility.checkNumber(sArrData(j)) = False Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_Wrong_TN, iLineNo))
                                    '���W�f�[�^�o�^���s��
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                                If Integer.Parse(sArrData(j)) = 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_MSG_TN, iLineNo))
                                    '���W�f�[�^�o�^���s��
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                            Case "ID_NO"
                                iFlag = iFlag - 1
                                'ID�ԍ��̃`�F�b�N
                                If sArrData(j).Replace("0", "").Length <= 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_MSG_ID, iLineNo))
                                    '���W�f�[�^�o�^���s��
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                        End Select
                        '�`�F�b�N�����̏ꍇ�A���~
                        If iFlag = 0 Then Exit For
                    Next
                Case FrexDataKind
                    'FREX�����ID���o�f�[�^�̃`�F�b�N
                    iFlag = 1
                    For j As Integer = 0 To defineInfo.Length - 1
                        Select Case defineInfo(j).FIELD_NAME
                            Case "ID_NO"
                                iFlag = iFlag - 1
                                'ID�ԍ��̃`�F�b�N
                                If sArrData(j).Replace("0", "").Length <= 0 Then
                                    isHaveErr = True
                                    Log.Error(String.Format(ERR_MSG_ID, iLineNo))
                                    '���W�f�[�^�o�^���s��
                                    BatchAppComm.SetCollectionData(defineInfo, sArrData)
                                    Exit For
                                End If
                        End Select
                        '�`�F�b�N�����̏ꍇ�A���~
                        If iFlag = 0 Then Exit For
                    Next
            End Select

            '����̏ꍇ�A�f�[�^��ǉ�����
            If isHaveErr = False Then
                refLstData.Add(sArrData)
            End If
        Next

        Return bRtn
    End Function

    ''' <summary>
    ''' �擾�����f�[�^��DB�o�^�f�[�^�ɍĉ��H����
    ''' </summary>
    ''' <param name="defineInfo">��`���</param>
    ''' <param name="sDataKind">�f�[�^���</param>
    ''' <param name="lstGetData">�f�[�^���</param>
    ''' <param name="lstData">�����f�[�^���</param>
    ''' <returns></returns>
    Private Shared Function GetDBInfoFromDataInfo(ByVal defineInfo() As RecDataStructure.DefineInfo, _
                                           ByVal sDataKind As String, _
                                           ByVal lstGetData As List(Of String()), _
                                           ByRef lstData As List(Of String())) As Boolean

        If lstGetData Is Nothing OrElse lstGetData.Count <= 0 Then Return True

        Dim sArrRecord() As String             '���R�[�h

        '�擾�����f�[�^���c�a�f�[�^�̊i���ɓ]������B
        lstData = New List(Of String())
        If sDataKind.Equals(FuseiJoshaDataKind) Then
            Dim isWtn As Boolean = False
            Dim nWtn As Integer = 0         '�Ώۋ敪�̈ʒu
            Dim nWtnValue As Integer = 1    '�Ώۋ敪�̒l
            '�s����Ԍ��o�f�[�^
            For i As Integer = 0 To lstGetData.Count - 1
                isWtn = False
                nWtn = 0
                For j As Integer = 0 To defineInfo.Length - 1
                    '�s������NG���ڂɂ���āA�s������Ώۋ敪��ݒ肷��B
                    If defineInfo(j).FIELD_NAME = "WRANG_TARGET_NO" Then
                        '�s������NG���ڂ̌���
                        If isWtn = False Then
                            nWtn = j
                            isWtn = True
                            nWtnValue = 1
                        End If
                        '�N���A
                        ReDim sArrRecord(nWtn)

                        If CInt(lstGetData(i)(j)) <> 0 Then
                            '��{�w�b�_��ݒ肷��
                            For K As Integer = 0 To nWtn - 1
                                sArrRecord(K) = lstGetData(i)(K)
                            Next

                            '�s������Ώۋ敪��ݒ肷��
                            sArrRecord(nWtn) = CStr(nWtnValue)

                            lstData.Add(sArrRecord)
                        End If

                        '�s������Ώۋ敪�̒l����������B
                        nWtnValue = nWtnValue + 1
                    End If
                Next
            Next
        ElseIf sDataKind.Equals(FrexDataKind) Then
            'FREX������h�c���o�f�[�^:����ԍ��͢10��Ƃ���B
            For i As Integer = 0 To lstGetData.Count - 1
                For j As Integer = 0 To defineInfo.Length - 1
                    If defineInfo(j).FIELD_NAME = "TICKET_NO" Then
                        lstGetData(i)(j) = "10"
                    End If
                Next
            Next
            lstData = lstGetData
        Else
            lstData = lstGetData
        End If

        Return True
    End Function
#End Region
End Class
