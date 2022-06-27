' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2013/11/07  (NES)�͘e  �t�F�[�Y�Q�Ή�
'                                   �ESNMPTrap�Ώۋy�у��[���ΏۑΉ�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports System.IO
Imports JR.ExOpmg.DBCommon
Imports System.Text

Public Class BatchAppComm

#Region "���\�b�h�iPublic�j"

    ''' <summary>
    ''' �P�O�i�����O���ڽ���Ȃ��P�U�i���̌`�ɕϊ�����
    ''' </summary>
    ''' <param name="bytDat10">�P�޲��ް�</param>
    ''' <returns>�P�U�i��������</returns>
    ''' <remarks>�P�޲��ް��̂O���ڽ�����P�U�i���\�L</remarks>
    Public Shared Function fnHexDisp(ByVal bytDat10 As Byte) As String

        '�펞�Q���ŕԂ�
        If Len(Hex(bytDat10)) <= 1 Then     '�P���Ȃ��
            fnHexDisp = "0" & Hex(bytDat10)   '�O���ڽ���Ȃ�
        Else                                '�Q���Ȃ��
            fnHexDisp = Hex(bytDat10)         '���̂܂�
        End If

    End Function

    ''' <summary>
    ''' DAT�t�@�C���̉��
    ''' </summary>
    ''' <param name="iniInfoAry">INI�t�@�C���̓��e</param>
    ''' <param name="datFileName">dat�t�@�C����</param>
    ''' <param name="clientKind"></param>
    ''' <param name="redLen">�f�[�^���T�C�Y</param>
    ''' <param name="headLen">�w�b�_���T�C�Y</param>
    ''' <param name="lineInfoLst">��͂����f�[�^</param>
    ''' <param name="dataKind">�f�[�^���</param>
    ''' <param name="isFtpData">true:FTP�f�[�^;false:�d���f�[�^</param>
    ''' <param name="isGet">true:�f�[�^��ʂɂ��l�擾;false:�l�擾���Ȃ�</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Public Shared Function GetInfoFromDataFileComm(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                                   ByVal datFileName As String, _
                                                   ByVal clientKind As String, _
                                                   ByVal redLen As Integer, _
                                                   ByVal headLen As Integer, _
                                                   ByRef lineInfoLst As List(Of String()), _
                                                   ByVal dataKind As String, _
                                                   Optional ByRef isFtpData As Boolean = False, _
                                                   Optional ByVal isGet As Boolean = False) As Boolean
        'Ver0.1 ���֐��p�����[�^��isFtpData��ǉ��iSNMPTrap�Ώۋy�у��[���ΏۑΉ��j

        '�o�^�p�̊�{���
        Dim headInfo As RecDataStructure.BaseInfo = Nothing

        '�P���R�[�h���̐��f�[�^���ꎞ�I�ɓǂݍ��ނ��߂̗̈�
        Dim bData(redLen + headLen) As Byte

        '��͂�������ԋp���邽�߂̃��X�g�𐶐��܂��͏�����
        If lineInfoLst Is Nothing Then
            lineInfoLst = New List(Of String())
        Else
            lineInfoLst.Clear()
        End If

        Dim fileStream As FileStream
        Try
            '�t�@�C���X�g���[�����擾
            fileStream = New FileStream(datFileName, FileMode.Open)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            '�t�@�C���T�C�Y���P���R�[�h���ɖ����Ȃ��ꍇ
            If fileStream.Length < (redLen + headLen) Then
                Log.Error(RecAppConstants.ERR_TOO_SHORT_FILE)

                CollectedDataTypoRecorder.Record( _
                   New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                   DbConstants.CdtKindServerError, _
                   Lexis.CdtReadingTotallyFailed.Gen(dataKind, Path.GetFileNameWithoutExtension(datFileName)))
                Return False
            End If

            '���R�[�h���擾
            Dim iRecCnt As Integer = CInt(fileStream.Length \ (redLen + headLen))

            '�J�n���R�[�hindex�擾
            Dim iStarRecIndex As Integer
            If fileStream.Length > (redLen + headLen) Then
                'FTP�Ŏ擾�����t�@�C���̏ꍇ�́A�擪�̃��R�[�h�͓ǂ܂Ȃ��B
                iStarRecIndex = 1
                fileStream.Seek(redLen + headLen, SeekOrigin.Begin)
                'Ver0.1 ADD SNMPTrap�Ώۋy�у��[���ΏۑΉ�
                isFtpData = True
            Else
                '�d���Ƃ��Ď擾�����t�@�C���̏ꍇ�́A�擪�̃��R�[�h��ǂށB
                iStarRecIndex = 0
                'Ver0.1 ADD SNMPTrap�Ώۋy�у��[���ΏۑΉ�
                isFtpData = False
            End If

            Dim skipped As Boolean = False '��͂ł��Ȃ����R�[�h�̗L��

            '�P���R�[�h�P�ʂŃf�[�^��ǂݎ��A��͂���B
            For i As Integer = iStarRecIndex To iRecCnt - 1
                fileStream.Read(bData, 0, redLen + headLen)
                headInfo = Nothing
                BinaryHeadInfoParse.GetBaseInfo(bData, clientKind, headInfo)

                If isGet = True Then
                    If (Not dataKind = "") AndAlso (Not dataKind = headInfo.DATA_KIND) Then
                        Continue For
                    End If
                End If

                '�P���R�[�h���̉�͌��ʂ��i�[���邽�߂̗̈�𐶐�
                Dim lineInfo As String() = New String(iniInfoAry.Length - 1) {}

                '�f�[�^�̉��
                If GetRecDataComm(iniInfoAry, bData, headInfo, lineInfo) = False Then
                    Log.Error(String.Format(RecAppConstants.ERR_INVALID_RECORD, i.ToString()))

                    'NOTE: �����ł͎��W�f�[�^��L�͓o�^���Ȃ��B
                    '�ʏ�̎��W�f�[�^��L�́A���̉w�R�[�h�����@��\���ɑ��݂���ۏ؂�
                    '�����ȏ�́A�o�^���Ă��A�^�ǒ[�����猩���Ȃ��\��������B
                    '�����āA�����Ŏ̂Ă����R�[�h�ɂ��ẮA���̌�ɍs���@�푶�݃`�F�b�N
                    '�̑ΏۂɂȂ�Ȃ����߁A�o�^������L���[���ŉ{���ł��Ȃ����̂ł���ꍇ
                    '�ɂ����āA����Ɂu�@�킪���݂��܂���v�Ƃ����ُ킪�o�^�����Ƃ���
                    '�킯�ł͂Ȃ��B����āA�����Ŏ̂Ă郌�R�[�h�ɂ��ẮA�ʏ��
                    '���W�f�[�^��L�ɓo�^����̂ł͂Ȃ��A�T�[�o���ُ�ɓo�^����B
                    skipped = True
                    Continue For
                End If

                lineInfoLst.Add(lineInfo)
            Next

            '�t�@�C���̒��������R�[�h���̔{���łȂ��ꍇ
            If fileStream.Length Mod (redLen + headLen) <> 0 Then
                Log.Error(RecAppConstants.ERR_FILE_ROUNDED_OFF)
                skipped = True
            End If

            If skipped Then
                CollectedDataTypoRecorder.Record( _
                   New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
                   DbConstants.CdtKindServerError, _
                   Lexis.CdtReadingPartiallyFailed.Gen(dataKind, Path.GetFileNameWithoutExtension(datFileName)))

                'TODO: �{���́A�����ɊY�������t�@�C���́ANormal�f�B���N�g���ł͂Ȃ�
                'Skipped�f�B���N�g�����Ɉړ����������iRecordToDatabase����IOError����
                '�߂肽���j�B�������A�������������ɂ́A�����o�^�n�v���Z�X��
                '�������\�b�h�̖߂�l��True��False�̓���ł���Ƃ�����肩��
                '�������Ȃ���΂Ȃ�Ȃ��B
            End If

            Return True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)

            'NOTE: ���̗ނ̉ӏ��ł́A��L�����iGetRecDataComm()�̖߂�l��False��
            '�P�[�X�́j������łȂ��AheadInfo�\���̂�String�Q�ƌ^�����o��
            'Nothing���Z�b�g����Ă��邱�Ƃ����蓾�邽�߁ASetCollectionData��
            '�Ăяo���i���W�f�[�^��L�̓o�^�j�͍s��Ȃ����ƂƂ���B
            CollectedDataTypoRecorder.Record( _
               New RecDataStructure.BaseInfo(EkConstants.ModelCodeNone, EkCode.Empty), _
               DbConstants.CdtKindServerError, _
               Lexis.CdtReadingTotallyFailed.Gen(dataKind, Path.GetFileNameWithoutExtension(datFileName)))

            Return False
        Finally
            '�t�@�C���X�g���[�������
            fileStream.Close()
        End Try

    End Function

    ''' <summary>
    ''' DAT�t�@�C���̂P���R�[�h�擾
    ''' </summary>
    ''' <param name="iniInfoAry">ini�t�@�C�����</param>
    ''' <param name="bData">�f�[�^���</param>
    ''' <param name="headInfo">�w�b�_���</param>
    ''' <param name="lineInfo">�P���R�[�h�̓��e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Public Shared Function GetRecDataComm(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                      ByVal bData() As Byte, _
                                      ByVal headInfo As RecDataStructure.BaseInfo, _
                                      ByRef lineInfo() As String) As Boolean

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0

        Dim strData As String = ""               'HEX ���� BCD�f�[�^
        Dim lBinData As Long = 0                 'binary�f�[�^
        Dim iPower As Integer = 0                '�ݏ�


        '���� �ݗ�����w���ˊ�������w�� �ϊ��Ή�
        Dim CHK_Count As Integer = 0            '�`�F�b�N���v��

        Try
            For j = 0 To iniInfoAry.Length - 1
                '�w�b�_���̃t�B�[���h�ł���ꍇ
                '���� �ݗ�����w���ˊ�������w�� �ϊ��Ή� MOD START
                '�@�i����F119�̉w���F003�̑����̉ғ��ێ炩�ُ�f�[�^���H�j
                'Select Case UCase(iniInfoAry(j).FIELD_NAME)
                '    Case "DATA_KIND" '�f�[�^���
                '        lineInfo(j) = headInfo.DATA_KIND
                '        Continue For
                '    Case "MODEL_CODE" '�@��
                '        lineInfo(j) = headInfo.MODEL_CODE
                '        Continue For
                '    Case "RAIL_SECTION_CODE"  '�T�C�o�l����R�[�h
                '        lineInfo(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE
                '        Continue For
                '    Case "STATION_ORDER_CODE"  '�T�C�o�l�w���R�[�h
                '        lineInfo(j) = headInfo.STATION_CODE.STATION_ORDER_CODE
                '        Continue For
                '    Case "CORNER_CODE"  '�R�[�i�[�R�[�h
                '        lineInfo(j) = headInfo.CORNER_CODE
                '        Continue For
                'End Select
                Select Case UCase(iniInfoAry(j).FIELD_NAME)
                    Case "DATA_KIND" '�f�[�^���
                        lineInfo(j) = headInfo.DATA_KIND
                        If (headInfo.DATA_KIND = "C3") Or (headInfo.DATA_KIND = "A6") Or (headInfo.DATA_KIND = "A7") Then
                            'C3:�ُ�f�[�^�AA6:�ُ�f�[�^�i�Ď��W�j�AA7:�ғ��f�[�^�@�Ȃ�J�E���g
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "MODEL_CODE" '�@��
                        lineInfo(j) = headInfo.MODEL_CODE
                        If (headInfo.MODEL_CODE = "Y") Then
                            'Y:�����@�Ȃ�J�E���g
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "RAIL_SECTION_CODE"  '�T�C�o�l����R�[�h
                        lineInfo(j) = headInfo.STATION_CODE.RAIL_SECTION_CODE
                        If (headInfo.STATION_CODE.RAIL_SECTION_CODE = "119") Then
                            '119:������@�Ȃ�J�E���g
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "STATION_ORDER_CODE"  '�T�C�o�l�w���R�[�h
                        lineInfo(j) = headInfo.STATION_CODE.STATION_ORDER_CODE
                        If (headInfo.STATION_CODE.STATION_ORDER_CODE = "003") Then
                            '003:������@�Ȃ�J�E���g
                            CHK_Count = CHK_Count + 1
                        End If
                        Continue For
                    Case "CORNER_CODE"  '�R�[�i�[�R�[�h
                        lineInfo(j) = headInfo.CORNER_CODE
                        Continue For
                End Select
                '���� �ݗ�����w���ˊ�������w�� �ϊ��Ή� MOD END

                If iniInfoAry(j).BYTE_LEN = 0 Then
                    lineInfo(j) = ""
                    Continue For
                End If

                '�f�[�^���̃t�B�[���h�ł���ꍇ
                Dim dataFormat As String = UCase(iniInfoAry(j).DATA_FORMAT)
                Select Case dataFormat
                    Case "HEX", "BCD"
                        strData = ""
                        If iniInfoAry(j).BIT_LEN = 0 Then
                            For k = 0 To iniInfoAry(j).BYTE_LEN - 1
                                strData = strData & fnHexDisp(bData(iniInfoAry(j).BYTE_OFFSET + k))
                            Next
                        Else
                            'bit���삪�K�v�ȏꍇ
                            If iniInfoAry(j).BYTE_LEN = 1 Then
                                strData = GetBitValueFromByte("BCD", bData(iniInfoAry(j).BYTE_OFFSET), iniInfoAry(j).BIT_OFFSET, iniInfoAry(j).BIT_LEN)
                            End If
                        End If

                        If dataFormat = "BCD" Then
                            'NOTE: ���ڂ̈Ӗ��Ɉˑ����Ȃ��i�f�[�^�t�H�[�}�b�g�ɂ݈̂ˑ�����j�`�F�b�N�́A
                            'CheckDataComm()�ł͂Ȃ��A���̃��\�b�h���ōs���Ă��܂����Ƃɂ���B
                            'NOTE: ������Ƃ��ĕێ�����Ă���l�𕶎�����Narrow�ȕʂ̌`���ɕϊ�����
                            '�ۂɁi�K�v�ɉ����āj�`�F�b�N����Ƃ����l���������邩������Ȃ����A
                            '���̂��郌�R�[�h�݂̂�o�^�ΏۊO�Ƃ��邽�߂ɂ́AList�Ƀ��R�[�h��Add���s��������
                            '�Ăь����V����List��p�ӂ��Ă���CheckDataComm()�ɂă`�F�b�N���s���̂��Ó��ł���B
                            For Each c As Char In strData
                                '�����ȊO�̕������܂܂�Ă��邩���ׂ�B
                                If c < "0"c OrElse "9"c < c Then
                                    Log.Error(String.Format(RecAppConstants.ERR_INVALID_FIELD_AS_BCD, iniInfoAry(j).KOMOKU_NAME))
                                    Return False
                                End If
                            Next c
                        End If

                        lineInfo(j) = strData

                    Case "BIN"
                        lBinData = 0
                        If iniInfoAry(j).BIT_LEN = 0 Then
                            'OPT: �ȉ��̖��ʂ́A�������e�⏈���p�x�ɂ���l����ƁA���Ȃ��肪����Ǝv����B
                            If iniInfoAry(j).PARA6.Trim.Equals("1") Then
                                '�C���e���`��
                                iPower = iniInfoAry(j).BYTE_LEN - 1
                                For k = iniInfoAry(j).BYTE_LEN - 1 To 0 Step -1
                                    lBinData = lBinData + CLng((bData(iniInfoAry(j).BYTE_OFFSET + k)) * (256 ^ iPower))
                                    iPower = iPower - 1
                                Next
                            Else
                                iPower = iniInfoAry(j).BYTE_LEN - 1
                                For k = 0 To iniInfoAry(j).BYTE_LEN - 1
                                    lBinData = lBinData + CLng((bData(iniInfoAry(j).BYTE_OFFSET + k)) * (256 ^ iPower))
                                    iPower = iPower - 1
                                Next
                            End If
                        Else
                            'bit���삪�K�v�ȏꍇ
                            If iniInfoAry(j).BYTE_LEN = 1 Then
                                strData = GetBitValueFromByte("BIN", bData(iniInfoAry(j).BYTE_OFFSET), iniInfoAry(j).BIT_OFFSET, iniInfoAry(j).BIT_LEN)
                                lBinData = Integer.Parse(strData)
                            End If

                        End If
                        lineInfo(j) = lBinData.ToString

                    Case "S-JIS"
                        '�̈�̃o�C�g����dataLen�Ɏ擾�B
                        Dim dataLen As Integer = iniInfoAry(j).BYTE_LEN

                        '�u�L���o�C�g���v�̊i�[�ʒu�iPARA3�j�ƃ����O�X�iPARA4�j����`����Ă���ꍇ�́A
                        '�u�L���o�C�g���v�̒l���擾���A����ɏ]����dataLen��ǂݑւ���B
                        If (Not iniInfoAry(j).PARA3.Equals("")) AndAlso (Not iniInfoAry(j).PARA4.Equals("")) Then
                            'TODO: ��`�t�@�C���̒l���s���ȏꍇ�́A�N������Ƀv���Z�X�I����������悢�B
                            Dim yukoByteNumOffset As Integer = Integer.Parse(iniInfoAry(j).PARA3)
                            Dim yukoByteNumLength As Integer = Integer.Parse(iniInfoAry(j).PARA4)
                            Dim yukoByteNum As UInteger
                            Select Case yukoByteNumLength
                                Case 4
                                    yukoByteNum = Utility.GetUInt32FromLeBytes4(bData, yukoByteNumOffset)
                                Case 3
                                    yukoByteNum = Utility.GetUInt32FromLeBytes3(bData, yukoByteNumOffset)
                                Case 2
                                    yukoByteNum = Utility.GetUInt16FromLeBytes2(bData, yukoByteNumOffset)
                                Case 1
                                    yukoByteNum = bData(yukoByteNumOffset)
                            End Select

                            'NOTE: �uyukoByteNum = dataLen�v�̏ꍇ��dataLen�����̂܂܎g�p����B
                            '�Ȃ��A�uyukoByteNum > dataLen�v�̏ꍇ��dataLen�����̂܂܎g�p���A
                            '�s���ȃf�[�^�Ƃ݂͂Ȃ��Ȃ����A����͎d�l�ł���B
                            If yukoByteNum < dataLen Then
                                dataLen = CInt(yukoByteNum)
                            End If
                        End If

                        '�̈悩��dataLen���̃f�[�^���擾����B
                        'NOTE: ��O�����������ꍇ�́A���R�[�h�S�ُ̂̈�Ƃ݂Ȃ��A�{���\�b�h��
                        '�ُ�I������i���W�f�[�^��L�e�[�u���ɓo�^����j�B
                        'NOTE: bData��dataLen�o�C�g�̕�������`���ꂽ�����́u�L���o�C�g���v��
                        '�i�[���Ă���i�\���Ȓ����ł���j���Ƃ́A�{���\�b�h�̌Ăь��̐Ӗ��ł���B
                        lineInfo(j) = OPMGUtility.getJisStringFromBytes(bData, iniInfoAry(j).BYTE_OFFSET, dataLen)
                End Select
            Next

            '���� �ݗ�����w���ˊ�������w�� �ϊ��Ή� ADD START
            '����w���R�[�h�u�����Ή��i����F119�A�w���F003 �� ����F070�A�w���F100�j
            If CHK_Count = 4 Then
                For j = 0 To iniInfoAry.Length - 1
                    Select Case UCase(iniInfoAry(j).FIELD_NAME)
                        Case "RAIL_SECTION_CODE"  '�T�C�o�l����R�[�h
                            lineInfo(j) = "070"
                            Continue For
                        Case "STATION_ORDER_CODE"  '�T�C�o�l�w���R�[�h
                            lineInfo(j) = "100"
                            Continue For
                    End Select
                Next
            End If
            '���� �ݗ�����w���ˊ�������w�� �ϊ��Ή� ADD END

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' ���t�`�F�b�N
    ''' </summary>
    ''' <param name="strDate">YYYYMMDDHHMMSS</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks></remarks>
    Public Shared Function CheckDate(ByVal strDate As String) As Boolean

        '���������t�H�[�}�[�g�`�F�b�N
        Dim sDate As String = strDate.Substring(0, 4) & "/" & _
                        strDate.ToString.Substring(4, 2) & "/" & _
                            strDate.Substring(6, 2) & " " & _
                            strDate.Substring(8, 2) & ":" & _
                            strDate.Substring(10, 2) & ":" & _
                            strDate.Substring(12, 2)
        If Date.TryParse(sDate, New Date) = False Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' DAT�t�@�C���̋��ʃ`�F�b�N:1���R�[�h�̃`�F�b�N
    ''' </summary>
    ''' <param name="rowIndex">�s��</param>
    ''' <param name="iniInfoAry">ini�t�@�C�����</param>
    ''' <param name="lineInfo">���R�[�h�f�[�^</param>
    ''' <param name="datFileName">�t�@�C����</param>
    ''' <param name="isCheckMachine">�@��\���}�X�^�`�F�b�N�F True�F�`�F�b�N�@False�F�`�F�b�N���Ȃ�</param>
    ''' <param name="isMachineCollect">�@��\���}�X�^�`�F�b�N�A���݂��Ȃ��ꍇ�ATrue�F���W�f�[�^��o�^ False�F���W�f�[�^��o�^���Ȃ�</param>
    ''' <param name="isMachineLog">�@��\���}�X�^�`�F�b�N�A���݂��Ȃ��ꍇ�ATrue�F���O���o�� False�F���W�f�[�^��o�^���Ȃ�</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�o�C�i���t�@�C���̊�{�w�b�_������͂���</remarks>
    Public Shared Function CheckDataComm(ByVal rowIndex As Integer, _
                                         ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                         ByVal lineInfo() As String, _
                                         ByVal datFileName As String, _
                                         Optional ByVal isCheckMachine As Boolean = True, _
                                         Optional ByVal isMachineCollect As Boolean = True, _
                                         Optional ByVal isMachineLog As Boolean = False) As Boolean

        Dim iFlag As Integer = 4
        Dim dataKind As String = "" '�f�[�^���

        Try

            For i As Integer = 0 To iniInfoAry.Length - 1
                If UCase(iniInfoAry(i).FIELD_NAME) = "DATA_KIND" Then
                    dataKind = lineInfo(i)
                    Continue For
                End If

                '�w�R�[�h�A�R�[�i�[�R�[�h�A���@�ԍ����S���`�F�b�N�ł͂Ȃ��ꍇ
                If iFlag > 0 Then
                    Select Case UCase(iniInfoAry(i).FIELD_NAME)  '�w�R�[�h�A�R�[�i�[�R�[�h�A���@�ԍ�
                        Case "RAIL_SECTION_CODE", "STATION_ORDER_CODE", "CORNER_CODE", "UNIT_NO"
                            iFlag = iFlag - 1
                            'Null�`�F�b�N�p
                            If (iniInfoAry(i).PARA2 = False) Then
                                If lineInfo(i) Is Nothing OrElse _
                                   lineInfo(i) = "" OrElse _
                                   lineInfo(i).Replace("0", "").Length <= 0 Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    SetCollectionData(datFileName, dataKind) '�t�@�C�������
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
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                            '���W�f�[�^�̓o�^
                            SetCollectionData(iniInfoAry, lineInfo)
                            Return (False)
                        Else '��ꍇ
                            'NULL�s��
                            If (iniInfoAry(i).PARA2 = False) Then
                                If lineInfo(i) Is Nothing OrElse _
                                   lineInfo(i) = "" OrElse _
                                   lineInfo(i).Replace("0", "").Length <= 0 Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    SetCollectionData(iniInfoAry, lineInfo)
                                    Return (False)
                                End If
                            End If
                        End If
                    Case "DATESTR"
                        '���������t�H�[�}�[�g�`�F�b�N
                        Dim lnDate As Long = 0

                        If lineInfo(i) IsNot Nothing AndAlso _
                           (Not lineInfo(i) = "") AndAlso _
                           OPMGUtility.checkNumber(lineInfo(i)) = False Then
                            Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                            '���W�f�[�^�̓o�^
                            SetCollectionData(datFileName, dataKind)
                            Return False
                        Else '�S���O�ꍇ
                            'NULL�s��
                            If (iniInfoAry(i).PARA2 = False) Then
                                If lineInfo(i) Is Nothing OrElse _
                                   lineInfo(i) = "" OrElse _
                                   lineInfo(i).Replace("0", "").Length <= 0 Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_NOVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    SetCollectionData(datFileName, dataKind)
                                    Return False
                                End If
                            End If
                            If lineInfo(i).Length = 14 Then
                                If CheckDate(lineInfo(i)) = False Then
                                    Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                    '���W�f�[�^�̓o�^
                                    SetCollectionData(datFileName, dataKind)
                                    Return False
                                End If
                            Else
                                Log.Error(String.Format(RecAppConstants.ERR_MSG_ERRVALUE, rowIndex.ToString, iniInfoAry(i).KOMOKU_NAME))
                                '���W�f�[�^�̓o�^
                                SetCollectionData(datFileName, dataKind)
                                Return False
                            End If

                        End If
                End Select

            Next

            '�@��\���}�X�^�`�F�b�N
            If isCheckMachine Then
                Dim sBuilder As New StringBuilder
                Dim sRail_Code As String = ""
                Dim sStation_Code As String = ""
                Dim sCorner_Code As String = ""
                Dim sUnit_No As String = ""
                Dim sModel_Code As String = ""

                '�@��\���}�X�^�`�F�b�N�pSQL��
                sBuilder.AppendLine("SELECT COUNT(1) FROM V_MACHINE_NOW WHERE 0=0 ")

                iFlag = 5
                For i As Integer = 0 To iniInfoAry.Length - 1
                    '�w�R�[�h�A�R�[�i�[�R�[�h
                    Select Case UCase(iniInfoAry(i).FIELD_NAME)
                        Case "RAIL_SECTION_CODE"
                            iFlag = iFlag - 1
                            '�@��\���}�X�^�`�F�b�N�p
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sRail_Code = lineInfo(i)

                        Case "STATION_ORDER_CODE"
                            iFlag = iFlag - 1
                            '�@��\���}�X�^�`�F�b�N�p
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sStation_Code = lineInfo(i)

                        Case "CORNER_CODE"
                            iFlag = iFlag - 1
                            '�@��\���}�X�^�`�F�b�N�p
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sCorner_Code = lineInfo(i)

                        Case "UNIT_NO"
                            iFlag = iFlag - 1
                            '�@��\���}�X�^�`�F�b�N�p
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sUnit_No = lineInfo(i)

                        Case "MODEL_CODE"
                            iFlag = iFlag - 1
                            '�@��\���}�X�^�`�F�b�N�p
                            If UCase(iniInfoAry(i).FIELD_FORMAT) = "INTEGER" Then
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & lineInfo(i))
                            Else
                                sBuilder.AppendLine(" and " & UCase(iniInfoAry(i).FIELD_NAME) & " = " & Utility.SetSglQuot(lineInfo(i)))
                            End If
                            sUnit_No = lineInfo(i)

                    End Select
                    If iFlag = 0 Then
                        Exit For
                    End If
                Next

                If iFlag = 0 Then
                    '�@��\���}�X�^�`�F�b�N
                    Dim dbCtl As DatabaseTalker = Nothing
                    Dim nRtn As Integer
                    Try
                        dbCtl = New DatabaseTalker
                        dbCtl.ConnectOpen()
                        nRtn = CInt(dbCtl.ExecuteSQLToReadScalar(sBuilder.ToString))
                        '  �Ď��Րݒ���̌ŗL����
                        '  �Ď��Ղ�IP�A�h���X����Ώۂ̉��D�@�𒊏o���A�R�[�i�R�[�h���擾
                        If (nRtn = 0) And (dataKind = "54") Then
                            Dim j As Integer
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
                    Finally
                        'DB�����
                        If dbCtl IsNot Nothing AndAlso dbCtl.IsConnect = True Then
                            dbCtl.ConnectClose()
                        End If
                        If dbCtl IsNot Nothing Then dbCtl = Nothing
                    End Try

                    If nRtn = 0 Then
                        '���݂��Ȃ��ꍇ�A���O���o�͂���
                        If isMachineLog Then
                            Log.Error(String.Format(RecAppConstants.ERR_MACHINE_NOVALUE, sRail_Code, sStation_Code, sCorner_Code, sUnit_No))
                        End If
                        '���݂��Ȃ��ꍇ�A���W�f�[�^��L��o�^����
                        'TODO: sRail_Code��sStation_Code�̕s���Łu�@�킪���݂��܂���v�ɂȂ�ꍇ�́A
                        '���W�f�[�^�̂����ɂ����o�^�����Ƃ���ŁA���ǁA�^�ǒ[���ŒT���Ă�
                        '�����݂��Ȃ��̂ł́H
                        '�������������[�U�̑z�肩��O��Ă���ꍇ���A�����q�b�g���Ȃ��킯��
                        '���邵�A�u�@��R�[�h���@��\���ɖ����ꍇ��A���������ƌ��ݓ����̊Ԃ�
                        '�ݒ肵�������𒴂��阨��������ꍇ�ɁACdtReadingPartiallyFailed��
                        '�T�[�o���ُ��o�^����v���A���{�I�ȉ��P���K�v�ł́H
                        If isMachineCollect Then
                            SetCollectionData(iniInfoAry, lineInfo, _
                                              Lexis.CdtTheUnitNotFound.Gen(sRail_Code, sStation_Code, sCorner_Code, sUnit_No), _
                                              True)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            '���W�f�[�^�̓o�^
            SetCollectionData(iniInfoAry, lineInfo)
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' DAT�t�@�C���̃f�[�^�̓o�^
    ''' </summary>
    ''' <param name="iniInfoAry">INI�t�@�C�����e</param>
    ''' <param name="dlineInfoLst">�f�[�^</param>
    ''' <param name="dbName">�e�[�u����</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�`�F�b�N�����ɂ����H�����f�[�^�ɂēo�^�������s��</remarks>
    Public Shared Function PutDataToDBCommon(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                             ByVal dlineInfoLst As List(Of String()), _
                                             ByVal dbName As String) As Boolean
        Dim bRtn As Boolean = False
        Dim sCountBuilder As New StringBuilder
        Dim sAddBuilder As New StringBuilder
        Dim sbAddItem As New StringBuilder
        Dim sbAddValue As New StringBuilder
        Dim sUpdateBuilder As New StringBuilder
        Dim sbSqlWhere As New StringBuilder
        Dim sLoginID As String = "batch"
        Dim sClient As String = "OPMGServer"
        Dim dbCtl As DatabaseTalker = Nothing
        Dim i As Integer
        Dim j As Integer


        Try
            If dlineInfoLst Is Nothing OrElse dlineInfoLst.Count = 0 Then
                Return True
            End If

            dbCtl = New DatabaseTalker
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
        Catch ex As Exception
            'DB�����
            If dbCtl IsNot Nothing AndAlso dbCtl.IsConnect = True Then
                dbCtl.ConnectClose()
            End If
            If dbCtl IsNot Nothing Then dbCtl = Nothing
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try

            For i = 0 To dlineInfoLst.Count - 1

                Dim lineInfo(iniInfoAry.Length) As String
                lineInfo = dlineInfoLst.Item(i)

                sCountBuilder = New StringBuilder
                sAddBuilder = New StringBuilder
                sbAddItem = New StringBuilder
                sbAddValue = New StringBuilder
                sUpdateBuilder = New StringBuilder
                sbSqlWhere = New StringBuilder
                sCountBuilder.AppendLine("SELECT COUNT(1) FROM " & dbName)

                sbAddItem.AppendLine("INSERT INTO " & dbName & "(INSERT_DATE,INSERT_USER_ID,INSERT_MACHINE_ID")
                sbAddItem.AppendLine(",UPDATE_DATE,UPDATE_USER_ID,UPDATE_MACHINE_ID")
                sbAddValue.AppendLine("VALUES(getdate(),")
                sbAddValue.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
                sbAddValue.AppendLine(Utility.SetSglQuot(sClient) & ",")
                sbAddValue.AppendLine("getdate(),")
                sbAddValue.AppendLine(Utility.SetSglQuot(sLoginID) & ",")
                sbAddValue.AppendLine(Utility.SetSglQuot(sClient))

                sUpdateBuilder.AppendLine("UPDATE " & dbName & " SET ")
                sUpdateBuilder.AppendLine("UPDATE_DATE=getdate(),")
                sUpdateBuilder.AppendLine("UPDATE_USER_ID=" & Utility.SetSglQuot(sLoginID) & ",")
                sUpdateBuilder.AppendLine("UPDATE_MACHINE_ID=" & Utility.SetSglQuot(sClient))

                sbSqlWhere.AppendLine(" WHERE 0 = 0 ")

                For j = 0 To iniInfoAry.Length - 1

                    'DB���Ł@�L���ȃt�B�[���h�ł͂Ȃ�
                    If UCase(iniInfoAry(j).PARA5) = "FALSE" Then
                        Continue For
                    End If

                    sbAddItem.AppendLine("," & iniInfoAry(j).FIELD_NAME)

                    If UCase(iniInfoAry(j).FIELD_FORMAT).Equals("INTEGER") Then
                        sbAddValue.AppendLine("," & lineInfo(j).ToString)
                    Else
                        sbAddValue.AppendLine("," & Utility.SetSglQuot(lineInfo(j).ToString))
                    End If

                    If iniInfoAry(j).PARA1 Then
                        If UCase(iniInfoAry(j).FIELD_FORMAT).Equals("INTEGER") Then
                            sbSqlWhere.AppendLine(" AND " & iniInfoAry(j).FIELD_NAME & "=" & lineInfo(j).ToString)
                        Else
                            sbSqlWhere.AppendLine(" AND " & iniInfoAry(j).FIELD_NAME & "=" & Utility.SetSglQuot(lineInfo(j).ToString))
                        End If
                    Else
                        If UCase(iniInfoAry(j).FIELD_FORMAT).Equals("INTEGER") Then
                            sUpdateBuilder.AppendLine("," & iniInfoAry(j).FIELD_NAME & "=" & lineInfo(j).ToString)
                        Else
                            sUpdateBuilder.AppendLine("," & iniInfoAry(j).FIELD_NAME & "=" & Utility.SetSglQuot(lineInfo(j).ToString))
                        End If
                    End If
                Next

                sCountBuilder.Append(sbSqlWhere)

                sbAddItem.AppendLine(")")
                sbAddValue.AppendLine(")")
                sAddBuilder.Append(sbAddItem)
                sAddBuilder.Append(sbAddValue)

                sUpdateBuilder.Append(sbSqlWhere)

                Dim nRtn As Integer = CInt(dbCtl.ExecuteSQLToReadScalar(sCountBuilder.ToString))
                If nRtn = 0 Then
                    dbCtl.ExecuteSQLToWrite(sAddBuilder.ToString)
                Else
                    dbCtl.ExecuteSQLToWrite(sUpdateBuilder.ToString)
                End If
            Next
            dbCtl.TransactionCommit()
            bRtn = True
        Catch ex As Exception
            Try
                dbCtl.TransactionRollBack()
            Catch ex1 As Exception
            End Try
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            'DB�����
            If dbCtl IsNot Nothing AndAlso dbCtl.IsConnect = True Then
                dbCtl.ConnectClose()
            End If
            If dbCtl IsNot Nothing Then dbCtl = Nothing

            If bRtn = False Then
                '���W�f�[�^�̓o�^
                SetCollectionData(iniInfoAry, dlineInfoLst.Item(i))
            End If
            dbCtl = Nothing
        End Try

        Return bRtn
    End Function

    ''' <summary>
    ''' �o�C�g���e
    ''' </summary>
    ''' <param name="byteData">�o�C�g���e</param>
    ''' <param name="bitOff">�r�b�g�I�t�Z�b�g</param>
    ''' <param name="bitLen">�r�b�g�����O�X</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�r�b�g�I�t�Z�b�g�A�r�b�g�����O�X�ɂ��l���擾</remarks>
    Public Shared Function GetBitValueFromByte(ByVal byteType As String, _
                                               ByVal byteData As Byte, _
                                               ByVal bitOff As Integer, _
                                               ByVal bitLen As Integer) As String
        Dim strValue As String = ""
        Dim bytData As Byte = byteData
        '����
        If bitOff > 0 Then
            bytData = bytData << bitOff
        End If
        '�E��
        bytData = bytData >> (8 - bitLen)

        Select Case UCase(byteType)
            Case "HEX", "BCD"
                strValue = fnHexDisp(bytData)
                If bitLen <= 4 Then
                    strValue = strValue.Substring(1, 1)
                End If
            Case "BIN"
                strValue = bytData.ToString
        End Select

        Return strValue

    End Function

    ''' <summary>
    ''' �o�C�g���e
    ''' </summary>
    ''' <param name="iniInfoAry">ini�t�@�C�����</param>
    ''' <param name="lineInfo">���R�[�h�f�[�^</param>
    ''' <param name="errInfo">�ُ���e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�r�b�g�I�t�Z�b�g�A�r�b�g�����O�X�ɂ��l���擾</remarks>
    Public Shared Function SetCollectionData(ByVal iniInfoAry() As RecDataStructure.DefineInfo, _
                                         ByVal lineInfo() As String, _
                                         Optional ByVal errInfo As String = "", _
                                         Optional ByVal isMachine As Boolean = False) As Boolean


        Dim iFlag As Integer = 7
        Dim baseInfo As RecDataStructure.BaseInfo = Nothing
        Dim sDataKindText As String = ""
        Try

            For i As Integer = 0 To iniInfoAry.Length - 1

                Select Case UCase(iniInfoAry(i).FIELD_NAME)
                    Case "DATA_KIND"
                        iFlag = iFlag - 1
                        baseInfo.DATA_KIND = lineInfo(i)
                    Case "RAIL_SECTION_CODE" '�w�R�[�h
                        iFlag = iFlag - 1
                        baseInfo.STATION_CODE.RAIL_SECTION_CODE = lineInfo(i)
                    Case "STATION_ORDER_CODE"
                        iFlag = iFlag - 1
                        baseInfo.STATION_CODE.STATION_ORDER_CODE = lineInfo(i)
                    Case "CORNER_CODE" '�R�[�i�[�R�[�h
                        iFlag = iFlag - 1
                        baseInfo.CORNER_CODE = lineInfo(i)
                    Case "UNIT_NO" '���@�ԍ�
                        iFlag = iFlag - 1
                        baseInfo.UNIT_NO = Integer.Parse(lineInfo(i))
                    Case "PROCESSING_TIME", "OCCUR_DATE", "SYUSYU_DATE"
                        iFlag = iFlag - 1
                        If lineInfo(i).Length > 14 Then
                            baseInfo.PROCESSING_TIME = GetDateTimeString(lineInfo(i))
                        Else
                            baseInfo.PROCESSING_TIME = lineInfo(i)
                        End If
                    Case "MODEL_CODE"
                        iFlag = iFlag - 1
                        baseInfo.MODEL_CODE = lineInfo(i)
                End Select

                If iFlag = 0 Then Exit For
            Next
            If errInfo = "" Then
                errInfo = Lexis.CdtRecordingFailed.Gen()
            End If
            If isMachine Then
                sDataKindText = DbConstants.CdtKindServerError
            Else
                sDataKindText = GetDataKindText(baseInfo.DATA_KIND)
            End If
            CollectedDataTypoRecorder.Record(baseInfo, sDataKindText, errInfo)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' �o�C�g���e
    ''' </summary>
    ''' <param name="baseInfo">�w�b�h���</param>
    ''' <param name="errInfo">�ُ���e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�r�b�g�I�t�Z�b�g�A�r�b�g�����O�X�ɂ��l���擾</remarks>
    Public Shared Function SetCollectionData(ByVal baseInfo As RecDataStructure.BaseInfo, _
                                             ByVal dataKind As String, _
                                             Optional ByVal errInfo As String = "", _
                                             Optional ByVal isMachine As Boolean = False) As Boolean

        Dim sDataKindText As String = ""
        Try
            If errInfo = "" Then
                errInfo = Lexis.CdtRecordingFailed.Gen()
            End If

            baseInfo.DATA_KIND = dataKind

            If isMachine Then
                sDataKindText = DbConstants.CdtKindServerError
            Else
                sDataKindText = GetDataKindText(baseInfo.DATA_KIND)
            End If

            CollectedDataTypoRecorder.Record(baseInfo, sDataKindText, errInfo)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' �o�C�g���e
    ''' </summary>
    ''' <param name="filePath">dat�t�@�C������</param>
    ''' <param name="errInfo">�ُ���e</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�r�b�g�I�t�Z�b�g�A�r�b�g�����O�X�ɂ��l���擾</remarks>
    Public Shared Function SetCollectionData(ByVal filePath As String, _
                                             ByVal dataKind As String, _
                                             Optional ByVal errInfo As String = "", _
                                             Optional ByVal isMachine As Boolean = False) As Boolean
        '�w�b�h��
        Dim headInfo As RecDataStructure.BaseInfo = Nothing
        Dim sDataKindText As String = ""
        Try
            Dim clientKind As String
            Dim codeInfo As EkCode = UpboundDataPath.GetEkCode(filePath)
            headInfo.DATA_KIND = dataKind
            headInfo.STATION_CODE.RAIL_SECTION_CODE = codeInfo.RailSection.ToString("D3")
            headInfo.STATION_CODE.STATION_ORDER_CODE = codeInfo.StationOrder.ToString("D3")
            headInfo.CORNER_CODE = codeInfo.Corner.ToString("D4")
            headInfo.UNIT_NO = codeInfo.Unit
            clientKind = codeInfo.Model.ToString("D2")
            headInfo.MODEL_CODE = GetModelCode(clientKind, dataKind, codeInfo.Unit)
            headInfo.PROCESSING_TIME = UpboundDataPath.GetTimestampString(filePath)

            If errInfo = "" Then
                errInfo = Lexis.CdtRecordingFailed.Gen()
            End If

            If isMachine Then
                sDataKindText = DbConstants.CdtKindServerError
            Else
                sDataKindText = GetDataKindText(headInfo.DATA_KIND)
            End If

            CollectedDataTypoRecorder.Record(headInfo, sDataKindText, errInfo)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Return False
        End Try
        Return True

    End Function

    ''' <summary>
    ''' �|�[�g�ԍ��擾
    ''' </summary>
    ''' <param name="clientKind">�N���C�A���gID</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    Public Shared Function GetPortNumber(ByVal clientKind As String) As Integer
        Select Case clientKind
            Case "02"
                Return RecServerAppBaseConfig.InputIpPortFromKanshiban
            Case "06"
                Return RecServerAppBaseConfig.InputIpPortFromTokatsu
            Case "08"
                Return RecServerAppBaseConfig.InputIpPortFromMadosho
            Case Else
                Return 0
        End Select
    End Function

    ''' <summary>
    ''' �@��擾
    ''' </summary>
    ''' <param name="clientKind">�N���C�A���gID</param>
    ''' <param name="dataKind">�f�[�^���</param>
    ''' <param name="unitNo">���@�ԍ�</param>
    ''' <returns>����:TRUE�^�ُ�:FALSE</returns>
    ''' <remarks>�r�b�g�I�t�Z�b�g�A�r�b�g�����O�X�ɂ��l���擾</remarks>
    Public Shared Function GetModelCode(ByVal clientKind As String, _
                                        ByVal dataKind As String, _
                                        Optional ByVal unitNo As Integer = 0) As String
        Dim modelCode As String = ""

        Select Case clientKind
            Case "02"
                Select Case dataKind
                    Case "A1", "A2", "A3", "A4", "A5", "A7", "A8", "55", "B1"
                        'G�F�i���D�@�j
                        modelCode = EkConstants.ModelCodeGate
                    Case "54"
                        'W�F�i�Ď��Ձj
                        modelCode = EkConstants.ModelCodeKanshiban
                    Case "A6", "C3"
                        If unitNo = 0 Then
                            'G�F�i���D�@�j
                            modelCode = EkConstants.ModelCodeGate
                        Else
                            'W�F�i�Ď��Ձj
                            modelCode = EkConstants.ModelCodeKanshiban
                        End If
                End Select

            Case "06"
                If dataKind = "89" Then
                    'Y�F�i���������@�j
                    modelCode = EkConstants.ModelCodeMadosho
                Else
                    'X�F�����^EX����
                    modelCode = EkConstants.ModelCodeTokatsu
                End If
            Case "08"
                '�O�W��Y�F�i���������@�j
                modelCode = EkConstants.ModelCodeMadosho
        End Select

        Return modelCode
    End Function

    ''' <summary>
    ''' �f�[�^��ʂɂ���āA���̂��擾����
    ''' </summary>
    ''' <param name="sDataKind">�f�[�^���</param>
    ''' <returns>�f�[�^��ʖ���</returns>
    Public Shared Function GetDataKindText(ByVal sDataKind As String) As String
        Dim sDataKindText As String = ""

        Select Case sDataKind
            Case "A1"
                sDataKindText = DbConstants.CdtKindBesshuData

            Case "A2"
                sDataKindText = DbConstants.CdtKindFuseiJoshaData

            Case "A3"
                sDataKindText = DbConstants.CdtKindKyokoToppaData

            Case "A4", "A5"
                sDataKindText = DbConstants.CdtKindFunshitsuData

            Case "A6", "C3"
                sDataKindText = DbConstants.CdtKindFaultData

            Case "A7", "A8"
                sDataKindText = DbConstants.CdtKindKadoData

            Case "B1"
                sDataKindText = DbConstants.CdtKindTrafficData

            Case "54"
                sDataKindText = DbConstants.CdtKindKsbConfig

            Case "55"
                sDataKindText = DbConstants.CdtKindConStatus

            Case "89"
                sDataKindText = DbConstants.CdtKindConStatus

            Case Else
                sDataKindText = sDataKind

        End Select

        Return sDataKindText
    End Function

    ''' <summary>
    ''' yyyy/MM/dd HH:mm:ss->yyyyMMddHHmmss
    ''' </summary>
    ''' <param name="sDataTime">����</param>
    ''' <returns>yyyyMMddHHmmss</returns>
    Public Shared Function GetDateTimeString(ByVal sDataTime As String) As String
        Dim sRtnDateTime As String
        Dim sDate() As String = sDataTime.Split(CChar(" "))

        'TODO: Now���瓾���l��Ԃ��̂́A���̃��\�b�h�̎d�l�Ȃ̂��H

        If sDate.Length >= 2 Then
            sRtnDateTime = sDate(0).Replace("/", "")

            Dim sTime() As String = sDate(1).Split(CChar(":"))
            If sTime.Length >= 3 Then
                For n As Integer = 0 To 2
                    sRtnDateTime += Format(CInt(sTime(n)), "00")
                Next
            Else
                sRtnDateTime = Now.ToString("yyyyMMddHHmmss")
            End If
        Else
            sRtnDateTime = Now.ToString("yyyyMMddHHmmss")
        End If

        Return sRtnDateTime
    End Function
#End Region

End Class
