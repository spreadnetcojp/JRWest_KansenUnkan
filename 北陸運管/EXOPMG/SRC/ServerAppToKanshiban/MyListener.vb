' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応
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
''' 対監視盤通信プロセスのクライアント管理クラス。
''' </summary>
Public Class MyListener
    Inherits TelServerAppListener

#Region "定数や変数"
    '電文書式
    Protected oTelegGene As EkTelegramGene

    '電文取り込み器
    Protected oTelegImporter As EkTelegramImporter
#End Region

#Region "コンストラクタ"
    Public Sub New()
        MyBase.New(&H02, EkConstants.ModelCodeKanshiban, DbConstants.PortPurposeGeneralData, True)

        Me.oTelegGene = New EkTelegramGeneForNativeModels(Config.FtpServerRootDirPath)
        Me.oTelegImporter = New EkTelegramImporter(oTelegGene)
        Me.sCdtClientModelName = Lexis.CdtKanshiban.Gen()
        Me.sCdtPortName = Lexis.CdtGeneralDataPort.Gen()
    End Sub
#End Region

#Region "メソッド"
    '-------Ver0.1 次世代車補対応 MOD START-----------
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
    '-------Ver0.1 次世代車補対応 MOD END-------------

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

        'NOTE: codeがIPアドレスと整合していない場合も、NotPermitとするのが
        'よいかもしれない。

        'NOTE: 慎重に運用されない（機器番号の重複がしばしば発生する）場合は、
        'oClient.LineStatusがDisconnectedになっていない間も、NotPermitと
        'するのがよいかもしれない。

        Log.Info("ComStart REQ with registered ClientCode (" & code.ToString(EkCodeOupFormat) & ") received.")

        'ACK電文を返信する。
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
