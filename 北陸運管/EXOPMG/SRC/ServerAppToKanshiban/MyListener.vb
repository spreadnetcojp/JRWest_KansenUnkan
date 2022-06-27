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
Imports System.Messaging
Imports System.Net.Sockets
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' �ΊĎ��ՒʐM�v���Z�X�̃N���C�A���g�Ǘ��N���X�B
''' </summary>
Public Class MyListener
    Inherits TelServerAppListener

#Region "�萔��ϐ�"
    '�d������
    Protected oTelegGene As EkTelegramGene

    '�d����荞�݊�
    Protected oTelegImporter As EkTelegramImporter
#End Region

#Region "�R���X�g���N�^"
    Public Sub New()
        MyBase.New(&H02, EkConstants.ModelCodeKanshiban, DbConstants.PortPurposeGeneralData, True)

        Me.oTelegGene = New EkTelegramGeneForNativeModels(Config.FtpServerRootDirPath)
        Me.oTelegImporter = New EkTelegramImporter(oTelegGene)
        Me.sCdtClientModelName = Lexis.CdtKanshiban.Gen()
        Me.sCdtPortName = Lexis.CdtGeneralDataPort.Gen()
    End Sub
#End Region

#Region "���\�b�h"
    '-------Ver0.1 ������ԕ�Ή� MOD START-----------
    Protected Overrides Function CreateTelegrapher( _
       ByVal sThreadName As String, _
       ByVal oParentMessageSock As Socket, _
       ByVal clientCode As EkCode, _
       ByVal sClientStationName As String, _
       ByVal sClientCornerName As String) As ServerTelegrapher
        Return New MyTelegrapher( _
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
          sClientCornerName)
    End Function
    '-------Ver0.1 ������ԕ�Ή� MOD END-------------

    Protected Overrides Sub ProcOnAccept(ByVal oSocket As Socket)
        Dim oRcvTeleg As EkDodgyTelegram _
           = oTelegImporter.GetTelegramFromSocket(oSocket, Config.TelegReadingLimitBaseTicks, Config.TelegReadingLimitExtraTicksPerMiB, Config.TelegLoggingMaxLengthOnRead)
        If oRcvTeleg Is Nothing Then
            Log.Info("Closing the connection...")
            oSocket.Close()
            Return
        End If

        Dim headerViolation As NakCauseCode = oRcvTeleg.GetHeaderFormatViolation()
        If headerViolation <> EkNakCauseCode.None Then
            Log.Error("Telegram with invalid HeadPart received.")
            SendNakTelegramThenDisconnect(headerViolation, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.CmdCode <> EkCmdCode.Req Then
            Log.Error("Telegram with invalid CmdCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.SubCmdCode <> EkSubCmdCode.Get Then
            Log.Error("Telegram with invalid SubCmdCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        If oRcvTeleg.ObjCode <> EkComStartReqTelegram.FormalObjCodeInKanshiban Then
            Log.Error("Telegram with invalid ObjCode received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.TelegramError, oRcvTeleg, oSocket)
            Return
        End If

        Dim oRcvComStartGetReqTeleg As New EkComStartReqTelegram(oRcvTeleg)
        Dim bodyViolation As NakCauseCode = oRcvComStartGetReqTeleg.GetBodyFormatViolation()
        If bodyViolation <> EkNakCauseCode.None Then
            Log.Error("ComStart REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(bodyViolation, oRcvComStartGetReqTeleg, oSocket)
            Return
        End If

        Dim code As EkCode = oRcvComStartGetReqTeleg.ClientCode
        Dim oClient As Client = FindClient(code)
        If oClient Is Nothing Then
            Log.Error("ComStart REQ with unregistered ClientCode (" & code.ToString(EkCodeOupFormat) & ") received.")
            SendNakTelegramThenDisconnect(EkNakCauseCode.NotPermit, oRcvComStartGetReqTeleg, oSocket)
            Return
        End If

        'NOTE: code��IP�A�h���X�Ɛ������Ă��Ȃ��ꍇ���ANotPermit�Ƃ���̂�
        '�悢��������Ȃ��B

        'NOTE: �T�d�ɉ^�p����Ȃ��i�@��ԍ��̏d�������΂��Δ�������j�ꍇ�́A
        'oClient.LineStatus��Disconnected�ɂȂ��Ă��Ȃ��Ԃ��ANotPermit��
        '����̂��悢��������Ȃ��B

        Log.Info("ComStart REQ with registered ClientCode (" & code.ToString(EkCodeOupFormat) & ") received.")

        'ACK�d����ԐM����B
        Dim oReplyTeleg As EkComStartAckTelegram = oRcvComStartGetReqTeleg.CreateAckTelegram()
        Log.Info("Sending ComStart ACK...")
        If SendReplyTelegram(oSocket, oReplyTeleg, oRcvComStartGetReqTeleg) = False Then
            Log.Info("Closing the connection...")
            oSocket.Close()
            Return
        End If

        Log.Info("Sending new socket to telegrapher [" & code.ToString(EkCodeOupFormat) & "]...")
        SendToTelegrapher(oClient, ConnectNotice.Gen(oSocket))
    End Sub
#End Region

End Class
