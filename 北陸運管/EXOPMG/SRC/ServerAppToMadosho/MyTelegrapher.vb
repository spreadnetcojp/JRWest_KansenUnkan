' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2014/01/13  (NES)����  �����Ɩ��O�F�؃��O���W�Ή�
'   0.2      2017/04/10  (NES)����  ������ԕ�Ή�
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
    '���̃N���X�p�̉^�ǎw��f�[�^ULL�̎d�l
    Protected Shared oScheduledUllSpecDictionary As New Dictionary(Of String, TelServerAppScheduledUllSpec)

    '���̃N���X�p��POST�d����M�̎d�l
    Protected Shared oByteArrayPassivePostSpecDictionary As New Dictionary(Of Byte, TelServerAppByteArrayPassivePostSpec)
#End Region

#Region "�R���X�g���N�^"
    '-------Ver0.2 ������ԕ�Ή� MOD START-----------
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
          Lexis.MadoshoLineErrorAlertMailSubject, _
          Lexis.MadoshoLineErrorAlertMailBody)
        Me.formalObjCodeOfWatchdog = EkWatchdogReqTelegram.FormalObjCodeInMadosho

        SyncLock oScheduledUllSpecDictionary
            If oScheduledUllSpecDictionary.Count = 0 Then
                AddItemsToScheduledUllSpecDictionary()
            End If
        End SyncLock
        Me.oScheduledUllSpecOfDataKinds = oScheduledUllSpecDictionary

        SyncLock oByteArrayPassivePostSpecDictionary
            If oByteArrayPassivePostSpecDictionary.Count = 0 Then
                AddItemsToByteArrayPassivePostSpecDictionary()
            End If
        End SyncLock
        Me.oByteArrayPassivePostSpecOfObjCodes = oByteArrayPassivePostSpecDictionary

        '�A�N�Z�X����\��̃f�B���N�g���ɂ��āA������΍쐬���Ă����B
        'NOTE: ���N���X���쐬������̂�A�K���T�u�f�B���N�g���̍쐬����
        '�s�����ƂɂȂ���̂ɂ��ẮA�ΏۊO�Ƃ���B
        Directory.CreateDirectory(Config.InputDirPathForApps("ForFaultData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForFaultData"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForKadoData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForKadoData"))
        Directory.CreateDirectory(Config.MadoLogDirPath)
        '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
        Directory.CreateDirectory(Config.MadoCertLogDirPath)
        '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------
    End Sub
    '-------Ver0.2 ������ԕ�Ή� MOD END-------------

    Protected Overridable Sub AddItemsToScheduledUllSpecDictionary()
        AddFaultDataItemsToScheduledUllSpecDictionary()
        AddKadoDataItemsToScheduledUllSpecDictionary()
    End Sub

    Protected Overridable Sub AddFaultDataItemsToScheduledUllSpecDictionary()
        Dim objCode As Byte = CByte(EkServerDrivenUllReqTelegram.FormalObjCodeAsMadoFaultData)
        Dim tranLim As Integer = Config.MadoFaultDataUllTransferLimitTicks
        Dim startLim As Integer = Config.MadoFaultDataUllStartReplyLimitTicks
        Dim retryItv As Integer = Config.MadoFaultDataUllRetryIntervalTicks
        Dim retryCntF As Integer = Config.MadoFaultDataUllMaxRetryCountToForget
        Dim retryCntC As Integer = Config.MadoFaultDataUllMaxRetryCountToCare
        Dim sAppId As String = "ForFaultData"

        With oScheduledUllSpecDictionary
            .Add("ERR", New TelServerAppScheduledUllSpec(objCode, tranLim, startLim, retryItv, retryCntF, retryCntC, sAppId))
        End With
    End Sub

    Protected Overridable Sub AddKadoDataItemsToScheduledUllSpecDictionary()
        Dim objCode As Byte = CByte(EkServerDrivenUllReqTelegram.FormalObjCodeAsMadoKadoData)
        Dim tranLim As Integer = Config.MadoKadoDataUllTransferLimitTicks
        Dim startLim As Integer = Config.MadoKadoDataUllStartReplyLimitTicks
        Dim retryItv As Integer = Config.MadoKadoDataUllRetryIntervalTicks
        Dim retryCntF As Integer = Config.MadoKadoDataUllMaxRetryCountToForget
        Dim retryCntC As Integer = Config.MadoKadoDataUllMaxRetryCountToCare
        Dim sAppId As String = "ForKadoData"

        With oScheduledUllSpecDictionary
            .Add("KDO", New TelServerAppScheduledUllSpec(objCode, tranLim, startLim, retryItv, retryCntF, retryCntC, sAppId))
        End With
    End Sub

    Protected Overridable Sub AddItemsToByteArrayPassivePostSpecDictionary()
        With oByteArrayPassivePostSpecDictionary
            .Add(CByte(EkByteArrayPostReqTelegram.FormalObjCodeAsMadoFaultData), New TelServerAppByteArrayPassivePostSpec("ForFaultData"))
        End With
    End Sub
#End Region

#Region "�C�x���g�������\�b�h"
    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)

        '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  MOD START-----------
        If oRcvTeleg.SubCmdCode = EkSubCmdCode.Post Then
            Select Case oRcvTeleg.ObjCode
                Case EkMadoLogPostReqTelegram.FormalObjCodeAsMadoLog
                    Return ProcOnMadoLogPostReqTelegramReceive(oRcvTeleg)
                Case EkMadoLogPostReqTelegram.FormalObjCodeAsMadoCertLog
                    Return ProcOnMadoCertLogPostReqTelegramReceive(oRcvTeleg)
            End Select
        End If
        '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  MOD END-----------

        Return MyBase.ProcOnPassiveOneReqTelegramReceive(iRcvTeleg)
    End Function

    Protected Overridable Function ProcOnMadoLogPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkMadoLogPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("MadoLogPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("MadoLogPost REQ received.")

        '�������샍�O�Ǘ��f�B���N�g����̖��g�p�t�@�C�������擾����B
        Dim sDstPath As String = UpboundDataPath.Gen(Config.MadoLogDirPath, clientCode, DateTime.Now)

        If UpboundDataPath.GetBranchNumber(sDstPath) <= Config.MadoLogMaxBranchNumber Then
            '�ꎞ��Ɨp�f�B���N�g���Ńt�@�C��������B
            Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
            Try
                Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                    Dim aBytes As Byte() = oRcvTeleg.LogData
                    oStream.Write(aBytes, 0, aBytes.Length)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                'NOTE: �ꉞ�A�����^�C���ȏ�������Ŕ��������O������̂ŁA
                '�ǂ�����̂��x�X�g���悭�l���������悢�B
                Abort()
            End Try

            '�쐬�����t�@�C���𑋏����샍�O�Ǘ��f�B���N�g���Ɉړ�����B
            File.Move(sTmpPath, sDstPath)
        Else
            Log.Warn("Ignored.")
        End If

        Dim oReplyTeleg As EkMadoLogPostAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending MadoLogPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function

    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD START-----------
    Protected Overridable Function ProcOnMadoCertLogPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkMadoLogPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("MadoCertLogPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("MadoCertLogPost REQ received.")

        '�����Ɩ��O�F�؃��O�Ǘ��f�B���N�g����̖��g�p�t�@�C�������擾����B
        Dim sDstPath As String = UpboundDataPath.Gen(Config.MadoCertLogDirPath, clientCode, DateTime.Now)

        If UpboundDataPath.GetBranchNumber(sDstPath) <= Config.MadoCertLogMaxBranchNumber Then
            '�ꎞ��Ɨp�f�B���N�g���Ńt�@�C��������B
            Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
            Try
                Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                    Dim aBytes As Byte() = oRcvTeleg.LogData
                    oStream.Write(aBytes, 0, aBytes.Length)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                'NOTE: �ꉞ�A�����^�C���ȏ�������Ŕ��������O������̂ŁA
                '�ǂ�����̂��x�X�g���悭�l���������悢�B
                Abort()
            End Try

            '�쐬�����t�@�C���𑋏��Ɩ��O�F�؃��O�Ǘ��f�B���N�g���Ɉړ�����B
            File.Move(sTmpPath, sDstPath)
        Else
            Log.Warn("Ignored.")
        End If

        Dim oReplyTeleg As EkMadoLogPostAckTelegram = oRcvTeleg.CreateAckTelegram()
        Log.Info("Sending MadoCertLogPost ACK...")
        If SendReplyTelegram(oReplyTeleg, oRcvTeleg) = False Then
            Disconnect()
            Return True
        End If

        Return True
    End Function
    '-------Ver0.1  �����Ɩ��O�F�؃��O���W�Ή�  ADD END-----------
#End Region

End Class
