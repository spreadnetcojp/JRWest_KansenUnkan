' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net.Sockets

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �����Ɠd���̑���M���s���N���X�B
''' </summary>
Public Class MyTelegrapher
    Inherits TelServerAppTelegrapher

#Region "�萔��ϐ�"
    '���̃N���X�p�̃}�X�^/�v���O�����ꎮDLL�̎d�l
    Protected Shared oMasProSuiteDllSpecDictionary As New Dictionary(Of String, TelServerAppMasProDllSpec)

    '���̃N���X�p�̃}�X�^/�v���O�����K�p���X�gDLL�̎d�l
    Protected Shared oMasProListDllSpecDictionary As New Dictionary(Of String, TelServerAppMasProDllSpec)

    '���̃N���X�p�̃}�X�^/�v���O����DL�����ʒm�̎d�l
    Protected Shared oMasProDlReflectSpecDictionary As New Dictionary(Of UShort, TelServerAppMasProDlReflectSpec)

    '���̃N���X�p�̃o�[�W�������ULL�̎d�l
    Protected Shared oVersionInfoUllSpecDictionary As New Dictionary(Of Byte, TelServerAppVersionInfoUllSpec)

    '�ڑ���Ԏ擾���{�^�C�}
    Protected oConStatusGetTimer As TickTimer
#End Region

#Region "�R���X�g���N�^"
    '-------Ver0.1 ������ԕ�Ή� MOD START-----------
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal oTelegImporter As EkTelegramImporter, _
       ByVal oTelegGene As EkTelegramGene, _
       ByVal clientCode As EkCode, _
       ByVal sClientModel As String, _
       ByVal sPortPurpose As String, _
       ByVal sCdtClientModelName As String, _
       ByVal sCdtPortName As String, _
       ByVal sClientStationName As String, _
       ByVal sClientCornerName As String)

        MyBase.New( _
          sThreadName, _
          oParentMessageSock, _
          oTelegImporter, _
          oTelegGene, _
          clientCode, _
          sClientModel, _
          sPortPurpose, _
          sCdtClientModelName, _
          sCdtPortName, _
          sClientStationName, _
          sClientCornerName, _
          Lexis.TokatsuLineErrorAlertMailSubject, _
          Lexis.TokatsuLineErrorAlertMailBody)
        Me.formalObjCodeOfWatchdog = EkWatchdogReqTelegram.FormalObjCodeInTokatsu
        Me.formalObjCodeOfTimeDataGet = EkTimeDataGetReqTelegram.FormalObjCodeInTokatsu

        SyncLock oMasProSuiteDllSpecDictionary
            If oMasProSuiteDllSpecDictionary.Count = 0 Then
                AddItemsToMasProSuiteDllSpecDictionary()
            End If
        End SyncLock
        Me.oMasProSuiteDllSpecOfDataKinds = oMasProSuiteDllSpecDictionary

        SyncLock oMasProListDllSpecDictionary
            If oMasProListDllSpecDictionary.Count = 0 Then
                AddItemsToMasProListDllSpecDictionary()
            End If
        End SyncLock
        Me.oMasProListDllSpecOfDataKinds = oMasProListDllSpecDictionary

        SyncLock oMasProDlReflectSpecDictionary
            If oMasProDlReflectSpecDictionary.Count = 0 Then
                AddItemsToMasProDlReflectSpecDictionary()
            End If
        End SyncLock
        Me.oMasProDlReflectSpecOfCplxObjCodes = oMasProDlReflectSpecDictionary

        SyncLock oVersionInfoUllSpecDictionary
            If oVersionInfoUllSpecDictionary.Count = 0 Then
                AddItemsToVersionInfoUllSpecDictionary()
            End If
        End SyncLock
        Me.oVersionInfoUllSpecOfObjCodes = oVersionInfoUllSpecDictionary

        Me.oConStatusGetTimer = New TickTimer(Config.TktConStatusGetIntervalTicks)

        '�A�N�Z�X����\��̃f�B���N�g���ɂ��āA������΍쐬���Ă����B
        'NOTE: ���N���X���쐬������̂�A�K���T�u�f�B���N�g���̍쐬����
        '�s�����ƂɂȂ���̂ɂ��ẮA�ΏۊO�Ƃ���B
        Directory.CreateDirectory(Config.InputDirPathForApps("ForConStatus"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForConStatus"))
    End Sub
    '-------Ver0.1 ������ԕ�Ή� MOD END-------------

    Protected Overridable Sub AddItemsToMasProSuiteDllSpecDictionary()
        Dim masCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsMadoMasterSuite)
        Dim masTranLim As Integer = Config.MadoMasterSuiteDllTransferLimitTicks
        Dim masStartLim As Integer = Config.MadoMasterSuiteDllStartReplyLimitTicks
        Dim masRetryItv As Integer = Config.MadoMasterSuiteDllRetryIntervalTicks
        Dim masRetryCntF As Integer = 0
        Dim masRetryCntC As Integer = Config.MadoMasterSuiteDllMaxRetryCountToCare
        Dim proCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsMadoProgramSuite)
        Dim proTranLim As Integer = Config.MadoProgramSuiteDllTransferLimitTicks
        Dim proStartLim As Integer = Config.MadoProgramSuiteDllStartReplyLimitTicks
        Dim proRetryItv As Integer = Config.MadoProgramSuiteDllRetryIntervalTicks
        Dim proRetryCntF As Integer = 0
        Dim proRetryCntC As Integer = Config.MadoProgramSuiteDllMaxRetryCountToCare

        With oMasProSuiteDllSpecDictionary
            .Add("FJW", New TelServerAppMasProDllSpec(masCode, &H3E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJW", New TelServerAppMasProDllSpec(masCode, &H43, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJC", New TelServerAppMasProDllSpec(masCode, &H4E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJC", New TelServerAppMasProDllSpec(masCode, &H4F, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJR", New TelServerAppMasProDllSpec(masCode, &H50, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJE", New TelServerAppMasProDllSpec(masCode, &H56, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("LST", New TelServerAppMasProDllSpec(masCode, &H4D, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("KEN", New TelServerAppMasProDllSpec(masCode, &H59, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("ICD", New TelServerAppMasProDllSpec(masCode, &H55, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DLY", New TelServerAppMasProDllSpec(masCode, &H41, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("ICH", New TelServerAppMasProDllSpec(masCode, &H44, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("CYC", New TelServerAppMasProDllSpec(masCode, &H64, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NSI", New TelServerAppMasProDllSpec(masCode, &H70, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NTO", New TelServerAppMasProDllSpec(masCode, &H71, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NIC", New TelServerAppMasProDllSpec(masCode, &H72, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NJW", New TelServerAppMasProDllSpec(masCode, &H73, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("YPG", New TelServerAppMasProDllSpec(proCode, &H0, proTranLim, proStartLim, proRetryItv, proRetryCntF, proRetryCntC))
        End With
    End Sub

    Protected Overridable Sub AddItemsToMasProListDllSpecDictionary()
        Dim masCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsMadoMasterList)
        Dim masTranLim As Integer = Config.MadoMasterListDllTransferLimitTicks
        Dim masStartLim As Integer = Config.MadoMasterListDllStartReplyLimitTicks
        Dim masRetryItv As Integer = Config.MadoMasterListDllRetryIntervalTicks
        Dim masRetryCntF As Integer = 0
        Dim masRetryCntC As Integer = Config.MadoMasterListDllMaxRetryCountToCare
        Dim proCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsMadoProgramList)
        Dim proTranLim As Integer = Config.MadoProgramListDllTransferLimitTicks
        Dim proStartLim As Integer = Config.MadoProgramListDllStartReplyLimitTicks
        Dim proRetryItv As Integer = Config.MadoProgramListDllRetryIntervalTicks
        Dim proRetryCntF As Integer = 0
        Dim proRetryCntC As Integer = Config.MadoProgramListDllMaxRetryCountToCare

        With oMasProListDllSpecDictionary
            .Add("FJW", New TelServerAppMasProDllSpec(masCode, &H3E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJW", New TelServerAppMasProDllSpec(masCode, &H43, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJC", New TelServerAppMasProDllSpec(masCode, &H4E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJC", New TelServerAppMasProDllSpec(masCode, &H4F, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJR", New TelServerAppMasProDllSpec(masCode, &H50, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJE", New TelServerAppMasProDllSpec(masCode, &H56, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("LST", New TelServerAppMasProDllSpec(masCode, &H4D, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("KEN", New TelServerAppMasProDllSpec(masCode, &H59, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("ICD", New TelServerAppMasProDllSpec(masCode, &H55, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DLY", New TelServerAppMasProDllSpec(masCode, &H41, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("ICH", New TelServerAppMasProDllSpec(masCode, &H44, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("CYC", New TelServerAppMasProDllSpec(masCode, &H64, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NSI", New TelServerAppMasProDllSpec(masCode, &H70, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NTO", New TelServerAppMasProDllSpec(masCode, &H71, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NIC", New TelServerAppMasProDllSpec(masCode, &H72, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NJW", New TelServerAppMasProDllSpec(masCode, &H73, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("YPG", New TelServerAppMasProDllSpec(proCode, &H0, proTranLim, proStartLim, proRetryItv, proRetryCntF, proRetryCntC))
        End With
    End Sub

    Protected Overridable Sub AddItemsToMasProDlReflectSpecDictionary()
        Dim objCodeMasData As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsMadoMasterData
        Dim objCodeProData As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsMadoProgramData
        Dim objCodeProList As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsMadoProgramList
        Dim modelMado As String = EkConstants.ModelCodeMadosho
        Dim filePurpData As String = EkConstants.FilePurposeData
        Dim filePurpList As String = EkConstants.FilePurposeList
        Dim dataPurpMas As String = EkConstants.DataPurposeMaster
        Dim dataPurpPro As String = EkConstants.DataPurposeProgram

        With oMasProDlReflectSpecDictionary
            .Add(GenCplxObjCode(objCodeMasData, &H0), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, Nothing)) 'NOTE: �_�~�[
            .Add(GenCplxObjCode(objCodeMasData, &H3E), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "FJW"))
            .Add(GenCplxObjCode(objCodeMasData, &H43), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "IJW"))
            .Add(GenCplxObjCode(objCodeMasData, &H4E), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "FJC"))
            .Add(GenCplxObjCode(objCodeMasData, &H4F), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "IJC"))
            .Add(GenCplxObjCode(objCodeMasData, &H50), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "FJR"))
            .Add(GenCplxObjCode(objCodeMasData, &H56), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "IJE"))
            .Add(GenCplxObjCode(objCodeMasData, &H4D), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "LST"))
            .Add(GenCplxObjCode(objCodeMasData, &H59), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "KEN"))
            .Add(GenCplxObjCode(objCodeMasData, &H55), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "ICD"))
            .Add(GenCplxObjCode(objCodeMasData, &H41), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "DLY"))
            .Add(GenCplxObjCode(objCodeMasData, &H44), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "ICH"))
            .Add(GenCplxObjCode(objCodeMasData, &H64), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "CYC"))
            .Add(GenCplxObjCode(objCodeMasData, &H70), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "NSI"))
            .Add(GenCplxObjCode(objCodeMasData, &H71), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "NTO"))
            .Add(GenCplxObjCode(objCodeMasData, &H72), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "NIC"))
            .Add(GenCplxObjCode(objCodeMasData, &H73), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, "NJW"))
            .Add(GenCplxObjCode(objCodeProData, &H0), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpPro, "YPG"))
            .Add(GenCplxObjCode(objCodeProList, &H0), New TelServerAppMasProDlReflectSpec(modelMado, filePurpList, dataPurpPro, "YPG"))
        End With
    End Sub

    Protected Overridable Sub AddItemsToVersionInfoUllSpecDictionary()
        Dim objCodeMas As Byte = CByte(EkClientDrivenUllReqTelegram.FormalObjCodeAsMadoMasterVerInfo)
        Dim objCodePro As Byte = CByte(EkClientDrivenUllReqTelegram.FormalObjCodeAsMadoProgramVerInfo)
        Dim modelMado As String = EkConstants.ModelCodeMadosho
        Dim dataPurpMas As String = EkConstants.DataPurposeMaster
        Dim dataPurpPro As String = EkConstants.DataPurposeProgram
        Dim masTranLim As Integer = Config.MadoMasterVersionInfoUllTransferLimitTicks
        Dim proTranLim As Integer = Config.MadoProgramVersionInfoUllTransferLimitTicks

        With oVersionInfoUllSpecDictionary
            .Add(objCodeMas, New TelServerAppVersionInfoUllSpec(modelMado, dataPurpMas, Nothing, masTranLim))
            .Add(objCodePro, New TelServerAppVersionInfoUllSpec(modelMado, dataPurpPro, Nothing, proTranLim))
        End With
    End Sub
#End Region

#Region "�C�x���g�������\�b�h"
    '�e�X���b�h����R�l�N�V�������󂯎�����ꍇ�i�ʐM��Ԃ̕ω����t�b�N���邽�߂̃��\�b�h�j
    Protected Overrides Sub ProcOnConnectionAppear()
        MyBase.ProcOnConnectionAppear()

        RegisterConStatusGet()
    End Sub

    Protected Overrides Function ProcOnTimeout(ByVal oTimer As TickTimer) As Boolean
        If oTimer Is oConStatusGetTimer Then
            Return ProcOnConStatusGetTime()
        End If

        Return MyBase.ProcOnTimeout(oTimer)
    End Function

    Protected Overridable Function ProcOnConStatusGetTime() As Boolean
        Log.Info("ConStatusGet time comes.")

        RegisterConStatusGet()
        Return True
    End Function

    '�\���I�P���V�[�P���X�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Info("ConStatusGet completed.")

            Dim oAckTeleg As EkByteArrayGetAckTelegram = DirectCast(iAckTeleg, EkByteArrayGetAckTelegram)

            Dim sDstPath As String = UpboundDataPath.Gen(Config.InputDirPathForApps("ForConStatus"), clientCode, DateTime.Now)
            If UpboundDataPath.GetBranchNumber(sDstPath) <= Config.MaxBranchNumberForApps("ForConStatus") Then
                '�ꎞ��Ɨp�f�B���N�g���Ńt�@�C��������B
                Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
                Try
                    Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                        Dim aBytes As Byte() = oAckTeleg.ByteArray
                        oStream.Write(aBytes, 0, aBytes.Length)
                    End Using
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    'NOTE: �ꉞ�A�����^�C���ȏ�������Ŕ��������O������̂ŁA
                    '�ǂ�����̂��x�X�g���悭�l���������悢�B
                    Abort()
                End Try

                '�쐬�����t�@�C����o�^�v���Z�X���ǂݎ��p�X�Ɉړ�����B
                File.Move(sTmpPath, sDstPath)

                '�o�^�v���Z�X�ɒʒm����B
                Config.MessageQueueForApps("ForConStatus").Send(New ExtFileCreationNotice())
            Else
                Log.Warn("Ignored.")
            End If

            '����̎擾�Ɍ����ă^�C�}���Z�b�g����B
            RegisterTimer(oConStatusGetTimer, TickTimer.GetSystemTick())
        Else
            MyBase.ProcOnActiveOneComplete(iReqTeleg, iAckTeleg)
        End If
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ��łȂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Warn("ConStatusGet skipped.")

            '����̎擾�Ɍ����ă^�C�}���Z�b�g����B
            RegisterTimer(oConStatusGetTimer, TickTimer.GetSystemTick())
        Else
            MyBase.ProcOnActiveOneRetryOverToForget(iReqTeleg, iNakTeleg)
        End If
    End Sub

    '�\���I�P���V�[�P���X�ňُ�Ƃ݂Ȃ��ׂ����g���C�I�[�o�[�����������ꍇ
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Error("ConStatusGet failed by retry over.")

            'NOTE: �@��ڑ���Ԃ����W�ł��Ă��Ȃ����Ƃ́A�@��ڑ���Ԋm�F��ʂɂ�
            '�ŏI���W�������L�[�Ƀ\�[�g����Δ��f�ł��邽�߁A���W�f�[�^��L�e�[�u��
            '�ւ̓o�^�͍s��Ȃ��i���s�@�Ɠ����d�l�j�B

            '����̎擾�Ɍ����ă^�C�}���Z�b�g����B
            RegisterTimer(oConStatusGetTimer, TickTimer.GetSystemTick())
        Else
            MyBase.ProcOnActiveOneRetryOverToCare(iReqTeleg, iNakTeleg)
        End If
    End Sub

    '�\���I�P���V�[�P���X�̍Œ���L���[�C���O���ꂽ�\���I�P���V�[�P���X�̎��{�O�ɒʐM�ُ�����o�����ꍇ
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Error("ConStatusGet failed by telegramming error.")

            'NOTE: �@��ڑ���Ԃ����W�ł��Ă��Ȃ����Ƃ́A�@��ڑ���Ԋm�F��ʂɂ�
            '�ŏI���W�������L�[�Ƀ\�[�g����Δ��f�ł��邽�߁A���W�f�[�^��L�e�[�u��
            '�ւ̓o�^�͍s��Ȃ��i���s�@�Ɠ����d�l�j�B

            'NOTE: ���̏ꍇ�́A���̌�ŃR�l�N�V�������ؒf�����i�����͊���
            '�ؒf����Ă���j���߁AoConStatusGetTimer�̃Z�b�g�͖��p�ł���B
        Else
            MyBase.ProcOnActiveOneAnonyError(iReqTeleg)
        End If
    End Sub
#End Region

#Region "�C�x���g���������p���\�b�h"
    Protected Overrides Sub UnregisterConnectionDependentTimers()
        MyBase.UnregisterConnectionDependentTimers()

        UnregisterTimer(oConStatusGetTimer)
    End Sub

    Protected Sub RegisterConStatusGet()
        Dim oReqTeleg As New EkByteArrayGetReqTelegram( _
           oTelegGene, _
           EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus,
           Config.TktConStatusGetReplyLimitTicks)

        RegisterActiveOne(oReqTeleg, _
           Config.TktConStatusGetRetryIntervalTicks, _
           Config.TktConStatusGetMaxRetryCountToForget + 1, _
           Config.TktConStatusGetMaxRetryCountToCare + 1, _
           "ConStatusGet")
    End Sub
#End Region

End Class
