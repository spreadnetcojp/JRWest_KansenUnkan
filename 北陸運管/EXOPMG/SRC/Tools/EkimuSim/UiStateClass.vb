' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Runtime.Serialization

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

<DataContract> Public Class UiStateClass

    '�u��{�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public AutomaticComStart As Boolean
    <DataMember> Public CapSndTelegs As Boolean
    <DataMember> Public CapRcvTelegs As Boolean
    <DataMember> Public CapSndFiles As Boolean
    <DataMember> Public CapRcvFiles As Boolean

    '�u�d�����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���


    '�u�\���IULL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���

    '�u�d�����M�v�^�u�ɂ��鍀��
    <DataMember> Public ActiveOneApplyFilePath As String
    <DataMember> Public ActiveOneReplyLimitTicks As Integer
    <DataMember> Public ActiveOneExecIntervalTicks As Integer

    '�u�\���IULL�v�^�u�ɂ��鍀��
    <DataMember> Public ActiveUllObjCode As String
    <DataMember> Public ActiveUllTransferFilePath As String
    <DataMember> Public ActiveUllTransferLimitTicks As Integer
    <DataMember> Public ActiveUllReplyLimitTicks As Integer
    <DataMember> Public ActiveUllExecIntervalTicks As Integer

    '�uGET�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public ApplyFileForPassiveGetObjCodes As Dictionary(Of Byte, String)
    <DataMember> Public ForceReplyNakToPassiveGetReq As Boolean
    <DataMember> Public NakCauseNumberToPassiveGetReq As Integer
    <DataMember> Public NakCauseTextToPassiveGetReq As String

    '�u�󓮓IULL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public ApplyFileForPassiveUllObjCodes As Dictionary(Of Byte, String)
    <DataMember> Public ForceReplyNakToPassiveUllStartReq As Boolean
    <DataMember> Public NakCauseNumberToPassiveUllStartReq As Integer
    <DataMember> Public NakCauseTextToPassiveUllStartReq As String
    <DataMember> Public PassiveUllTransferLimitTicks As Integer
    <DataMember> Public PassiveUllFinishReplyLimitTicks As Integer

    '�uPOST�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public SomethingForPassivePostObjCodes As Dictionary(Of Byte, String)
    <DataMember> Public ForceReplyNakToPassivePostReq As Boolean
    <DataMember> Public NakCauseNumberToPassivePostReq As Integer
    <DataMember> Public NakCauseTextToPassivePostReq As String

    '�u�󓮓IDLL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public SomethingForPassiveDllObjCodes As Dictionary(Of Byte, String)
    <DataMember> Public ForceReplyNakToPassiveDllStartReq As Boolean
    <DataMember> Public NakCauseNumberToPassiveDllStartReq As Integer
    <DataMember> Public NakCauseTextToPassiveDllStartReq As String
    <DataMember> Public PassiveDllTransferLimitTicks As Integer
    <DataMember> Public PassiveDllFinishReplyLimitTicks As Integer
    <DataMember> Public SimulateStoringOnPassiveDllComplete As Boolean
    <DataMember> Public PassiveDllResultantVersionOfSlot1 As Integer
    <DataMember> Public PassiveDllResultantVersionOfSlot2 As Integer
    <DataMember> Public PassiveDllResultantFlagOfFull As Integer

    '�u�V�i���I�v�^�u�ɂ��鍀��
    <DataMember> Public ScenarioFilePath As String
    <DataMember> Public ScenarioExecIntervalTicks As Integer

    Public Sub New()
        '�u��{�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.AutomaticComStart = True
        Me.CapSndTelegs = True
        Me.CapRcvTelegs = True
        Me.CapSndFiles = False
        Me.CapRcvFiles = False

        '�u�d�����M�v�^�u�ɂ��鍀��
        Me.ActiveOneApplyFilePath = ""
        Me.ActiveOneReplyLimitTicks = 60000
        Me.ActiveOneExecIntervalTicks = 0

        '�u�\���IULL�v�^�u�ɂ��鍀��
        Me.ActiveUllObjCode = ""
        Me.ActiveUllTransferFilePath = ""
        Me.ActiveUllTransferLimitTicks = 0
        Me.ActiveUllReplyLimitTicks = 60000
        Me.ActiveUllExecIntervalTicks = 0

        '�uGET�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.ApplyFileForPassiveGetObjCodes = New Dictionary(Of Byte, String)
        Select Case Config.AplProtocol
            Case EkAplProtocol.Tokatsu
                RegisterPathToPassiveGetObjCodes(EkWatchdogReqTelegram.FormalObjCodeInTokatsu)
                RegisterPathToPassiveGetObjCodes(EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus)
            Case EkAplProtocol.Kanshiban
                RegisterPathToPassiveGetObjCodes(EkWatchdogReqTelegram.FormalObjCodeInKanshiban)
            Case EkAplProtocol.Kanshiban2
                RegisterPathToPassiveGetObjCodes(EkWatchdogReqTelegram.FormalObjCodeInKanshiban)
            Case EkAplProtocol.Madosho
                RegisterPathToPassiveGetObjCodes(EkWatchdogReqTelegram.FormalObjCodeInMadosho)
            Case EkAplProtocol.Madosho2
                RegisterPathToPassiveGetObjCodes(EkWatchdogReqTelegram.FormalObjCodeInMadosho)
        End Select
        Me.ForceReplyNakToPassiveGetReq = False
        Me.NakCauseNumberToPassiveGetReq = 101
        Me.NakCauseTextToPassiveGetReq = "BUSY"

        '�u�󓮓IULL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.ApplyFileForPassiveUllObjCodes = New Dictionary(Of Byte, String)
        Select Case Config.AplProtocol
            Case EkAplProtocol.Kanshiban
                RegisterPathToPassiveUllObjCodes(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateBesshuData)
                RegisterPathToPassiveUllObjCodes(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateMeisaiData)
                RegisterPathToPassiveUllObjCodes(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateKadoData)
                RegisterPathToPassiveUllObjCodes(EkServerDrivenUllReqTelegram.FormalObjCodeAsKsbGateFaultData)
                If Config.SelfCompany = EkCompany.JRWest Then
                    RegisterPathToPassiveUllObjCodes(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateTrafficData)
                End If
            Case EkAplProtocol.Madosho
                RegisterPathToPassiveUllObjCodes(EkServerDrivenUllReqTelegram.FormalObjCodeAsMadoKadoData)
                RegisterPathToPassiveUllObjCodes(EkServerDrivenUllReqTelegram.FormalObjCodeAsMadoFaultData)
        End Select
        Me.ForceReplyNakToPassiveUllStartReq = False
        Me.NakCauseNumberToPassiveUllStartReq = 101
        Me.NakCauseTextToPassiveUllStartReq = "BUSY"
        Me.PassiveUllTransferLimitTicks = 0
        Me.PassiveUllFinishReplyLimitTicks = 60000

        '�uPOST�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.SomethingForPassivePostObjCodes = New Dictionary(Of Byte, String)
        Select Case Config.AplProtocol
        End Select
        Me.ForceReplyNakToPassivePostReq = False
        Me.NakCauseNumberToPassivePostReq = 101
        Me.NakCauseTextToPassivePostReq = "BUSY"

        '�u�󓮓IDLL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.SomethingForPassiveDllObjCodes = New Dictionary(Of Byte, String)
        Select Case Config.AplProtocol
            Case EkAplProtocol.Kanshiban
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsGateMasterSuite)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsGateMasterList)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsGateProgramSuite)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsGateProgramList)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsKsbProgramSuite)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsKsbProgramList)
            Case EkAplProtocol.Tokatsu
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsMadoMasterSuite)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsMadoMasterList)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsMadoProgramSuite)
                RegisterPathToPassiveDllObjCodes(EkMasProDllReqTelegram.FormalObjCodeAsMadoProgramList)
        End Select
        Me.ForceReplyNakToPassiveDllStartReq = False
        Me.NakCauseNumberToPassiveDllStartReq = 101
        Me.NakCauseTextToPassiveDllStartReq = "BUSY"
        Me.PassiveDllTransferLimitTicks = 0
        Me.PassiveDllFinishReplyLimitTicks = 60000
        Me.SimulateStoringOnPassiveDllComplete = True
        Me.PassiveDllResultantVersionOfSlot1 = 0
        Me.PassiveDllResultantVersionOfSlot2 = 0
        Me.PassiveDllResultantFlagOfFull = 0

        '�u�V�i���I�v�^�u�ɂ��鍀��
        Me.ScenarioFilePath = ""
        Me.ScenarioExecIntervalTicks = 0
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g���M�t�@�C���p�X��
    'Me.ApplyFileForPassiveGetObjCodes�ɒǉ�����B
    Private Sub RegisterPathToPassiveGetObjCodes(ByVal objCode As Byte)
        Me.ApplyFileForPassiveGetObjCodes.Add( _
           objCode, _
           Path.Combine(Config.DefaultApplyDataDirPath, objCode.ToString("X2") & ".DAT"))
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g���M�t�@�C���p�X��
    'Me.ApplyFileForPassiveUllObjCodes�ɒǉ�����B
    Private Sub RegisterPathToPassiveUllObjCodes(ByVal objCode As Byte)
        Me.ApplyFileForPassiveUllObjCodes.Add( _
           objCode, _
           Path.Combine(Config.DefaultApplyDataDirPath, objCode.ToString("X2") & ".DAT"))
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g��M�f�B���N�g���p�X��
    'Me.SomethingForPassivePostObjCodes�ɒǉ�����B
    Private Sub RegisterPathToPassivePostObjCodes(ByVal objCode As Byte)
        Me.SomethingForPassivePostObjCodes.Add(objCode, Nothing)
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g��M�f�B���N�g���p�X��
    'Me.SomethingForPassiveDllObjCodes�ɒǉ�����B
    Private Sub RegisterPathToPassiveDllObjCodes(ByVal objCode As Byte)
        Me.SomethingForPassiveDllObjCodes.Add(objCode, Nothing)
    End Sub

End Class
