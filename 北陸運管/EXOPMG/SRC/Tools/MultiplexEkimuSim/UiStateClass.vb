' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/01/14  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Runtime.Serialization

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

<DataContract> Public Class UiStateClass

    '�u�ڑ��ؒf�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public AutomaticComStart As Boolean

    '�u�d�����M�v�^�u�ɂ��鍀��
    <DataMember> Public ActiveOneApplyFilePath As String
    <DataMember> Public ActiveOneReplyLimitTicks As Integer
    <DataMember> Public ActiveOneExecIntervalTicks As Integer

    '�u�\���IULL�v�^�u�ɂ��鍀��
    <DataMember> Public ActiveUllObjCode As String
    <DataMember> Public ActiveUllTransferName As String
    <DataMember> Public ActiveUllApplyFilePath As String
    <DataMember> Public ActiveUllTransferLimitTicks As Integer
    <DataMember> Public ActiveUllStartReplyLimitTicks As Integer
    <DataMember> Public ActiveUllFinishReplyLimitTicks As Integer
    <DataMember> Public ActiveUllExecIntervalTicks As Integer

    '�uGET�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public PassiveGetObjCodesApplyFiles As Dictionary(Of Byte, String)
    <DataMember> Public PassiveGetForceReplyNak As Boolean
    <DataMember> Public PassiveGetNakCauseNumber As Integer
    <DataMember> Public PassiveGetNakCauseText As String

    '�u�󓮓IULL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public PassiveUllObjCodesApplyFiles As Dictionary(Of Byte, String)
    <DataMember> Public PassiveUllForceReplyNak As Boolean
    <DataMember> Public PassiveUllNakCauseNumber As Integer
    <DataMember> Public PassiveUllNakCauseText As String
    <DataMember> Public PassiveUllTransferLimitTicks As Integer
    <DataMember> Public PassiveUllReplyLimitTicks As Integer

    '�uPOST�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public PassivePostObjCodes As Dictionary(Of Byte, String)
    <DataMember> Public PassivePostForceReplyNak As Boolean
    <DataMember> Public PassivePostNakCauseNumber As Integer
    <DataMember> Public PassivePostNakCauseText As String

    '�u�󓮓IDLL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public PassiveDllObjCodes As Dictionary(Of Byte, String)
    <DataMember> Public PassiveDllForceReplyNak As Boolean
    <DataMember> Public PassiveDllNakCauseNumber As Integer
    <DataMember> Public PassiveDllNakCauseText As String
    <DataMember> Public PassiveDllTransferLimitTicks As Integer
    <DataMember> Public PassiveDllReplyLimitTicks As Integer
    <DataMember> Public PassiveDllSimulateStoring As Boolean
    <DataMember> Public PassiveDllResultantVersionOfSlot1 As Integer
    <DataMember> Public PassiveDllResultantVersionOfSlot2 As Integer
    <DataMember> Public PassiveDllResultantFlagOfFull As Integer

    '�u�V�i���I�v�^�u�ɂ��鍀��
    <DataMember> Public ScenarioFilePath As String
    <DataMember> Public ScenarioStartTimeSpecified As Boolean
    <DataMember> Public ScenarioStartTime As DateTime

    '�u�L���v�`���v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
    <DataMember> Public CapSndTelegs As Boolean
    <DataMember> Public CapRcvTelegs As Boolean
    <DataMember> Public CapSndFiles As Boolean
    <DataMember> Public CapRcvFiles As Boolean

    '���O�\���t�B���^�̗���
    <DataMember> Public LogDispFilterHistory As List(Of String)

    Public Sub New()
        '�u�ڑ��ؒf�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.AutomaticComStart = True

        '�u�d�����M�v�^�u�ɂ��鍀��
        Me.ActiveOneApplyFilePath = ""
        Me.ActiveOneReplyLimitTicks = 60000
        Me.ActiveOneExecIntervalTicks = 0

        '�u�\���IULL�v�^�u�ɂ��鍀��
        Me.ActiveUllObjCode = ""
        Me.ActiveUllTransferName = ""
        Me.ActiveUllApplyFilePath = ""
        Me.ActiveUllTransferLimitTicks = 0
        Me.ActiveUllStartReplyLimitTicks = 60000
        Me.ActiveUllFinishReplyLimitTicks = 60000
        Me.ActiveUllExecIntervalTicks = 0

        '�uGET�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.PassiveGetObjCodesApplyFiles = New Dictionary(Of Byte, String)
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
        Me.PassiveGetForceReplyNak = False
        Me.PassiveGetNakCauseNumber = 101
        Me.PassiveGetNakCauseText = "BUSY"

        '�u�󓮓IULL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.PassiveUllObjCodesApplyFiles = New Dictionary(Of Byte, String)
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
        Me.PassiveUllForceReplyNak = False
        Me.PassiveUllNakCauseNumber = 101
        Me.PassiveUllNakCauseText = "BUSY"
        Me.PassiveUllTransferLimitTicks = 0
        Me.PassiveUllReplyLimitTicks = 60000

        '�uPOST�d����M�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.PassivePostObjCodes = New Dictionary(Of Byte, String)
        Select Case Config.AplProtocol
        End Select
        Me.PassivePostForceReplyNak = False
        Me.PassivePostNakCauseNumber = 101
        Me.PassivePostNakCauseText = "BUSY"

        '�u�󓮓IDLL�v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.PassiveDllObjCodes = New Dictionary(Of Byte, String)
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
        Me.PassiveDllForceReplyNak = False
        Me.PassiveDllNakCauseNumber = 101
        Me.PassiveDllNakCauseText = "BUSY"
        Me.PassiveDllTransferLimitTicks = 0
        Me.PassiveDllReplyLimitTicks = 60000
        Me.PassiveDllSimulateStoring = True
        Me.PassiveDllResultantVersionOfSlot1 = 0
        Me.PassiveDllResultantVersionOfSlot2 = 0
        Me.PassiveDllResultantFlagOfFull = 0

        '�u�V�i���I�v�^�u�ɂ��鍀��
        Me.ScenarioFilePath = ""
        Me.ScenarioStartTimeSpecified = False
        Me.ScenarioStartTime = DateTime.Now

        '�u�L���v�`���v�^�u�ɂ���MyTelegrapher�̎Q�ƍ���
        Me.CapSndTelegs = False
        Me.CapRcvTelegs = False
        Me.CapSndFiles = False
        Me.CapRcvFiles = False

        '���O�\���t�B���^�̗���
        Me.LogDispFilterHistory = New List(Of String)
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g���M�t�@�C���p�X��
    'Me.PassiveGetObjCodesApplyFiles�ɒǉ�����B
    Private Sub RegisterPathToPassiveGetObjCodes(ByVal objCode As Byte)
        Me.PassiveGetObjCodesApplyFiles.Add( _
           objCode, _
           Path.Combine(Config.DefaultApplyDataDirPath, objCode.ToString("X2") & ".DAT"))
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g���M�t�@�C���p�X��
    'Me.PassiveUllObjCodesApplyFiles�ɒǉ�����B
    Private Sub RegisterPathToPassiveUllObjCodes(ByVal objCode As Byte)
        Me.PassiveUllObjCodesApplyFiles.Add( _
           objCode, _
           Path.Combine(Config.DefaultApplyDataDirPath, objCode.ToString("X2") & ".DAT"))
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g��M�f�B���N�g���p�X��
    'Me.PassivePostObjCodes�ɒǉ�����B
    Private Sub RegisterPathToPassivePostObjCodes(ByVal objCode As Byte)
        Me.PassivePostObjCodes.Add(objCode, Nothing)
    End Sub

    '�w��f�[�^��ʂ̃f�t�H���g��M�f�B���N�g���p�X��
    'Me.PassiveDllObjCodes�ɒǉ�����B
    Private Sub RegisterPathToPassiveDllObjCodes(ByVal objCode As Byte)
        Me.PassiveDllObjCodes.Add(objCode, Nothing)
    End Sub

End Class
