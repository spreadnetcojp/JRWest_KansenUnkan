' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2014/01/13  (NES)金沢  窓処業務前認証ログ収集対応
'   0.2      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net.Sockets

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 窓処と電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits TelServerAppTelegrapher

#Region "定数や変数"
    'このクラス用の運管指定データULLの仕様
    Protected Shared oScheduledUllSpecDictionary As New Dictionary(Of String, TelServerAppScheduledUllSpec)

    'このクラス用のPOST電文受信の仕様
    Protected Shared oByteArrayPassivePostSpecDictionary As New Dictionary(Of Byte, TelServerAppByteArrayPassivePostSpec)
#End Region

#Region "コンストラクタ"
    '-------Ver0.2 次世代車補対応 MOD START-----------
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

        'アクセスする予定のディレクトリについて、無ければ作成しておく。
        'NOTE: 基底クラスが作成するものや、必ずサブディレクトリの作成から
        '行うことになるものについては、対象外とする。
        Directory.CreateDirectory(Config.InputDirPathForApps("ForFaultData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForFaultData"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForKadoData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForKadoData"))
        Directory.CreateDirectory(Config.MadoLogDirPath)
        '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
        Directory.CreateDirectory(Config.MadoCertLogDirPath)
        '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------
    End Sub
    '-------Ver0.2 次世代車補対応 MOD END-------------

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

#Region "イベント処理メソッド"
    Protected Overrides Function ProcOnPassiveOneReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As EkTelegram = DirectCast(iRcvTeleg, EkTelegram)

        '-------Ver0.1  窓処業務前認証ログ収集対応  MOD START-----------
        If oRcvTeleg.SubCmdCode = EkSubCmdCode.Post Then
            Select Case oRcvTeleg.ObjCode
                Case EkMadoLogPostReqTelegram.FormalObjCodeAsMadoLog
                    Return ProcOnMadoLogPostReqTelegramReceive(oRcvTeleg)
                Case EkMadoLogPostReqTelegram.FormalObjCodeAsMadoCertLog
                    Return ProcOnMadoCertLogPostReqTelegramReceive(oRcvTeleg)
            End Select
        End If
        '-------Ver0.1  窓処業務前認証ログ収集対応  MOD END-----------

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

        '窓処操作ログ管理ディレクトリ上の未使用ファイル名を取得する。
        Dim sDstPath As String = UpboundDataPath.Gen(Config.MadoLogDirPath, clientCode, DateTime.Now)

        If UpboundDataPath.GetBranchNumber(sDstPath) <= Config.MadoLogMaxBranchNumber Then
            '一時作業用ディレクトリでファイル化する。
            Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
            Try
                Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                    Dim aBytes As Byte() = oRcvTeleg.LogData
                    oStream.Write(aBytes, 0, aBytes.Length)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                'NOTE: 一応、ランタイムな条件次第で発生する例外もあるので、
                'どうするのがベストかよく考えた方がよい。
                Abort()
            End Try

            '作成したファイルを窓処操作ログ管理ディレクトリに移動する。
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

    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD START-----------
    Protected Overridable Function ProcOnMadoCertLogPostReqTelegramReceive(ByVal iRcvTeleg As ITelegram) As Boolean
        Dim oRcvTeleg As New EkMadoLogPostReqTelegram(iRcvTeleg)
        Dim violation As NakCauseCode = oRcvTeleg.GetBodyFormatViolation()
        If violation <> EkNakCauseCode.None Then
            Log.Error("MadoCertLogPost REQ with invalid BodyPart received.")
            SendNakTelegramThenDisconnect(violation, oRcvTeleg)
            Return True
        End If

        Log.Info("MadoCertLogPost REQ received.")

        '窓処業務前認証ログ管理ディレクトリ上の未使用ファイル名を取得する。
        Dim sDstPath As String = UpboundDataPath.Gen(Config.MadoCertLogDirPath, clientCode, DateTime.Now)

        If UpboundDataPath.GetBranchNumber(sDstPath) <= Config.MadoCertLogMaxBranchNumber Then
            '一時作業用ディレクトリでファイル化する。
            Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
            Try
                Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                    Dim aBytes As Byte() = oRcvTeleg.LogData
                    oStream.Write(aBytes, 0, aBytes.Length)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                'NOTE: 一応、ランタイムな条件次第で発生する例外もあるので、
                'どうするのがベストかよく考えた方がよい。
                Abort()
            End Try

            '作成したファイルを窓処業務前認証ログ管理ディレクトリに移動する。
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
    '-------Ver0.1  窓処業務前認証ログ収集対応  ADD END-----------
#End Region

End Class
