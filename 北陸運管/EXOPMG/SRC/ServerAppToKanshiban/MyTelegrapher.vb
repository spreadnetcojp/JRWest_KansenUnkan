' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2014/06/01       金沢  新規マスタ追加対応
'   0.2      2017/04/10  (NES)小林  次世代車補対応
'   0.3      2017/05/22  (NES)河脇  ポイントポストペイ対応
'                                     マスタ追加（昼特区間時間、ポストペイエリアマスタ）
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Net.Sockets

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 監視盤と電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits TelServerAppTelegrapher

#Region "定数や変数"
    'このクラス用のマスタ/プログラム一式DLLの仕様
    Protected Shared oMasProSuiteDllSpecDictionary As New Dictionary(Of String, TelServerAppMasProDllSpec)

    'このクラス用のマスタ/プログラム適用リストDLLの仕様
    Protected Shared oMasProListDllSpecDictionary As New Dictionary(Of String, TelServerAppMasProDllSpec)

    'このクラス用の運管指定データULLの仕様
    Protected Shared oScheduledUllSpecDictionary As New Dictionary(Of String, TelServerAppScheduledUllSpec)

    'このクラス用のマスタ/プログラムDL完了通知の仕様
    Protected Shared oMasProDlReflectSpecDictionary As New Dictionary(Of UShort, TelServerAppMasProDlReflectSpec)

    'このクラス用のPOST電文受信の仕様
    Protected Shared oByteArrayPassivePostSpecDictionary As New Dictionary(Of Byte, TelServerAppByteArrayPassivePostSpec)

    'このクラス用のバージョン情報ULLの仕様
    Protected Shared oVersionInfoUllSpecDictionary As New Dictionary(Of Byte, TelServerAppVersionInfoUllSpec)
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
          Lexis.KanshibanLineErrorAlertMailSubject, _
          Lexis.KanshibanLineErrorAlertMailBody)
        Me.formalObjCodeOfWatchdog = EkWatchdogReqTelegram.FormalObjCodeInKanshiban
        Me.formalObjCodeOfTimeDataGet = EkTimeDataGetReqTelegram.FormalObjCodeInKanshiban

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

        SyncLock oScheduledUllSpecDictionary
            If oScheduledUllSpecDictionary.Count = 0 Then
                AddItemsToScheduledUllSpecDictionary()
            End If
        End SyncLock
        Me.oScheduledUllSpecOfDataKinds = oScheduledUllSpecDictionary

        SyncLock oMasProDlReflectSpecDictionary
            If oMasProDlReflectSpecDictionary.Count = 0 Then
                AddItemsToMasProDlReflectSpecDictionary()
            End If
        End SyncLock
        Me.oMasProDlReflectSpecOfCplxObjCodes = oMasProDlReflectSpecDictionary

        SyncLock oByteArrayPassivePostSpecDictionary
            If oByteArrayPassivePostSpecDictionary.Count = 0 Then
                AddItemsToByteArrayPassivePostSpecDictionary()
            End If
        End SyncLock
        Me.oByteArrayPassivePostSpecOfObjCodes = oByteArrayPassivePostSpecDictionary

        SyncLock oVersionInfoUllSpecDictionary
            If oVersionInfoUllSpecDictionary.Count = 0 Then
                AddItemsToVersionInfoUllSpecDictionary()
            End If
        End SyncLock
        Me.oVersionInfoUllSpecOfObjCodes = oVersionInfoUllSpecDictionary

        'アクセスする予定のディレクトリについて、無ければ作成しておく。
        'NOTE: 基底クラスが作成するものや、必ずサブディレクトリの作成から
        '行うことになるものについては、対象外とする。
        Directory.CreateDirectory(Config.InputDirPathForApps("ForConStatus"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForConStatus"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForKsbConfig"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForKsbConfig"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForBesshuData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForBesshuData"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForMeisaiData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForMeisaiData"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForFaultData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForFaultData"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForKadoData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForKadoData"))
        Directory.CreateDirectory(Config.InputDirPathForApps("ForTrafficData"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForTrafficData"))
    End Sub
    '-------Ver0.2 次世代車補対応 MOD END-------------

    Protected Overridable Sub AddItemsToMasProSuiteDllSpecDictionary()
        Dim masCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsGateMasterSuite)
        Dim masTranLim As Integer = Config.GateMasterSuiteDllTransferLimitTicks
        Dim masStartLim As Integer = Config.GateMasterSuiteDllStartReplyLimitTicks
        Dim masRetryItv As Integer = Config.GateMasterSuiteDllRetryIntervalTicks
        Dim masRetryCntF As Integer = 0
        Dim masRetryCntC As Integer = Config.GateMasterSuiteDllMaxRetryCountToCare
        Dim proCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsGateProgramSuite)
        Dim proTranLim As Integer = Config.GateProgramSuiteDllTransferLimitTicks
        Dim proStartLim As Integer = Config.GateProgramSuiteDllStartReplyLimitTicks
        Dim proRetryItv As Integer = Config.GateProgramSuiteDllRetryIntervalTicks
        Dim proRetryCntF As Integer = 0
        Dim proRetryCntC As Integer = Config.GateProgramSuiteDllMaxRetryCountToCare
        Dim wproCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsKsbProgramSuite)
        Dim wproTranLim As Integer = Config.KsbProgramSuiteDllTransferLimitTicks
        Dim wproStartLim As Integer = Config.KsbProgramSuiteDllStartReplyLimitTicks
        Dim wproRetryItv As Integer = Config.KsbProgramSuiteDllRetryIntervalTicks
        Dim wproRetryCntF As Integer = 0
        Dim wproRetryCntC As Integer = Config.KsbProgramSuiteDllMaxRetryCountToCare

        With oMasProSuiteDllSpecDictionary
            .Add("DSH", New TelServerAppMasProDllSpec(masCode, &H47, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("LOS", New TelServerAppMasProDllSpec(masCode, &H48, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DSC", New TelServerAppMasProDllSpec(masCode, &H49, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("HLD", New TelServerAppMasProDllSpec(masCode, &H4A, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("EXP", New TelServerAppMasProDllSpec(masCode, &H4B, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FRX", New TelServerAppMasProDllSpec(masCode, &H4C, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("LST", New TelServerAppMasProDllSpec(masCode, &H4D, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJW", New TelServerAppMasProDllSpec(masCode, &H3E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJW", New TelServerAppMasProDllSpec(masCode, &H43, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJC", New TelServerAppMasProDllSpec(masCode, &H4E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJC", New TelServerAppMasProDllSpec(masCode, &H4F, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJR", New TelServerAppMasProDllSpec(masCode, &H50, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJE", New TelServerAppMasProDllSpec(masCode, &H56, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("KEN", New TelServerAppMasProDllSpec(masCode, &H59, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DLY", New TelServerAppMasProDllSpec(masCode, &H41, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("ICH", New TelServerAppMasProDllSpec(masCode, &H44, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("PAY", New TelServerAppMasProDllSpec(masCode, &H42, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("CYC", New TelServerAppMasProDllSpec(masCode, &H64, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("STP", New TelServerAppMasProDllSpec(masCode, &H63, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("PNO", New TelServerAppMasProDllSpec(masCode, &H62, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FRC", New TelServerAppMasProDllSpec(masCode, &H61, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DUS", New TelServerAppMasProDllSpec(masCode, &H66, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NSI", New TelServerAppMasProDllSpec(masCode, &H70, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NTO", New TelServerAppMasProDllSpec(masCode, &H71, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NIC", New TelServerAppMasProDllSpec(masCode, &H72, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NJW", New TelServerAppMasProDllSpec(masCode, &H73, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            '----------- 0.1  新規マスタ追加対応   ADD  START------------------------
            .Add("FSK", New TelServerAppMasProDllSpec(masCode, &H80, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IUZ", New TelServerAppMasProDllSpec(masCode, &H84, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("KSZ", New TelServerAppMasProDllSpec(masCode, &H85, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IUK", New TelServerAppMasProDllSpec(masCode, &H86, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("SWK", New TelServerAppMasProDllSpec(masCode, &H87, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            '----------- 0.3  ポイントポストペイ対応   ADD  START------------------------
            .Add("HIR", New TelServerAppMasProDllSpec(masCode, &H8A, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("PPA", New TelServerAppMasProDllSpec(masCode, &H89, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            '----------- 0.3  ポイントポストペイ対応   ADD    END------------------------
            '----------- 0.1  新規マスタ追加対応   ADD    END------------------------
            .Add("GPG", New TelServerAppMasProDllSpec(proCode, &H0, proTranLim, proStartLim, proRetryItv, proRetryCntF, proRetryCntC))
            .Add("WPG", New TelServerAppMasProDllSpec(wproCode, &H0, wproTranLim, wproStartLim, wproRetryItv, wproRetryCntF, wproRetryCntC))
        End With
    End Sub

    Protected Overridable Sub AddItemsToMasProListDllSpecDictionary()
        Dim masCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsGateMasterList)
        Dim masTranLim As Integer = Config.GateMasterListDllTransferLimitTicks
        Dim masStartLim As Integer = Config.GateMasterListDllStartReplyLimitTicks
        Dim masRetryItv As Integer = Config.GateMasterListDllRetryIntervalTicks
        Dim masRetryCntF As Integer = 0
        Dim masRetryCntC As Integer = Config.GateMasterListDllMaxRetryCountToCare
        Dim proCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsGateProgramList)
        Dim proTranLim As Integer = Config.GateProgramListDllTransferLimitTicks
        Dim proStartLim As Integer = Config.GateProgramListDllStartReplyLimitTicks
        Dim proRetryItv As Integer = Config.GateProgramListDllRetryIntervalTicks
        Dim proRetryCntF As Integer = 0
        Dim proRetryCntC As Integer = Config.GateProgramListDllMaxRetryCountToCare
        Dim wproCode As Byte = CByte(EkMasProDllReqTelegram.FormalObjCodeAsKsbProgramList)
        Dim wproTranLim As Integer = Config.KsbProgramListDllTransferLimitTicks
        Dim wproStartLim As Integer = Config.KsbProgramListDllStartReplyLimitTicks
        Dim wproRetryItv As Integer = Config.KsbProgramListDllRetryIntervalTicks
        Dim wproRetryCntF As Integer = 0
        Dim wproRetryCntC As Integer = Config.KsbProgramListDllMaxRetryCountToCare

        With oMasProListDllSpecDictionary
            .Add("DSH", New TelServerAppMasProDllSpec(masCode, &H47, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("LOS", New TelServerAppMasProDllSpec(masCode, &H48, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DSC", New TelServerAppMasProDllSpec(masCode, &H49, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("HLD", New TelServerAppMasProDllSpec(masCode, &H4A, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("EXP", New TelServerAppMasProDllSpec(masCode, &H4B, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FRX", New TelServerAppMasProDllSpec(masCode, &H4C, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("LST", New TelServerAppMasProDllSpec(masCode, &H4D, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJW", New TelServerAppMasProDllSpec(masCode, &H3E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJW", New TelServerAppMasProDllSpec(masCode, &H43, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJC", New TelServerAppMasProDllSpec(masCode, &H4E, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJC", New TelServerAppMasProDllSpec(masCode, &H4F, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FJR", New TelServerAppMasProDllSpec(masCode, &H50, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IJE", New TelServerAppMasProDllSpec(masCode, &H56, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("KEN", New TelServerAppMasProDllSpec(masCode, &H59, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DLY", New TelServerAppMasProDllSpec(masCode, &H41, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("ICH", New TelServerAppMasProDllSpec(masCode, &H44, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("PAY", New TelServerAppMasProDllSpec(masCode, &H42, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("CYC", New TelServerAppMasProDllSpec(masCode, &H64, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("STP", New TelServerAppMasProDllSpec(masCode, &H63, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("PNO", New TelServerAppMasProDllSpec(masCode, &H62, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("FRC", New TelServerAppMasProDllSpec(masCode, &H61, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("DUS", New TelServerAppMasProDllSpec(masCode, &H66, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NSI", New TelServerAppMasProDllSpec(masCode, &H70, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NTO", New TelServerAppMasProDllSpec(masCode, &H71, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NIC", New TelServerAppMasProDllSpec(masCode, &H72, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("NJW", New TelServerAppMasProDllSpec(masCode, &H73, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            '----------- 0.1  新規マスタ追加対応   ADD  START------------------------
            .Add("FSK", New TelServerAppMasProDllSpec(masCode, &H80, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IUZ", New TelServerAppMasProDllSpec(masCode, &H84, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("KSZ", New TelServerAppMasProDllSpec(masCode, &H85, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("IUK", New TelServerAppMasProDllSpec(masCode, &H86, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("SWK", New TelServerAppMasProDllSpec(masCode, &H87, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            '----------- 0.3  ポイントポストペイ対応   ADD  START------------------------
            .Add("HIR", New TelServerAppMasProDllSpec(masCode, &H8A, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            .Add("PPA", New TelServerAppMasProDllSpec(masCode, &H89, masTranLim, masStartLim, masRetryItv, masRetryCntF, masRetryCntC))
            '----------- 0.3  ポイントポストペイ対応   ADD    END------------------------
            '----------- 0.1  新規マスタ追加対応   ADD    END------------------------
            .Add("GPG", New TelServerAppMasProDllSpec(proCode, &H0, proTranLim, proStartLim, proRetryItv, proRetryCntF, proRetryCntC))
            .Add("WPG", New TelServerAppMasProDllSpec(wproCode, &H0, wproTranLim, wproStartLim, wproRetryItv, wproRetryCntF, wproRetryCntC))
        End With
    End Sub

    Protected Overridable Sub AddItemsToScheduledUllSpecDictionary()
        AddBesshuDataItemsToScheduledUllSpecDictionary()
        AddMeisaiDataItemsToScheduledUllSpecDictionary()
        AddFaultDataItemsToScheduledUllSpecDictionary()
        AddKadoDataItemsToScheduledUllSpecDictionary()
        AddTrafficDataItemsToScheduledUllSpecDictionary()
    End Sub

    Protected Overridable Sub AddBesshuDataItemsToScheduledUllSpecDictionary()
        Dim objCode As Byte = CByte(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateBesshuData)
        Dim tranLim As Integer = Config.GateBesshuDataUllTransferLimitTicks
        Dim startLim As Integer = Config.GateBesshuDataUllStartReplyLimitTicks
        Dim retryItv As Integer = Config.GateBesshuDataUllRetryIntervalTicks
        Dim retryCntF As Integer = Config.GateBesshuDataUllMaxRetryCountToForget
        Dim retryCntC As Integer = Config.GateBesshuDataUllMaxRetryCountToCare
        Dim sAppId As String = "ForBesshuData"

        With oScheduledUllSpecDictionary
            .Add("BSY", New TelServerAppScheduledUllSpec(objCode, tranLim, startLim, retryItv, retryCntF, retryCntC, sAppId))
        End With
    End Sub

    Protected Overridable Sub AddMeisaiDataItemsToScheduledUllSpecDictionary()
        Dim objCode As Byte = CByte(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateMeisaiData)
        Dim tranLim As Integer = Config.GateMeisaiDataUllTransferLimitTicks
        Dim startLim As Integer = Config.GateMeisaiDataUllStartReplyLimitTicks
        Dim retryItv As Integer = Config.GateMeisaiDataUllRetryIntervalTicks
        Dim retryCntF As Integer = Config.GateMeisaiDataUllMaxRetryCountToForget
        Dim retryCntC As Integer = Config.GateMeisaiDataUllMaxRetryCountToCare
        Dim sAppId As String = "ForMeisaiData"

        With oScheduledUllSpecDictionary
            .Add("MEI", New TelServerAppScheduledUllSpec(objCode, tranLim, startLim, retryItv, retryCntF, retryCntC, sAppId))
        End With
    End Sub

    Protected Overridable Sub AddFaultDataItemsToScheduledUllSpecDictionary()
        Dim objCode As Byte = CByte(EkServerDrivenUllReqTelegram.FormalObjCodeAsKsbGateFaultData)
        Dim tranLim As Integer = Config.KsbGateFaultDataUllTransferLimitTicks
        Dim startLim As Integer = Config.KsbGateFaultDataUllStartReplyLimitTicks
        Dim retryItv As Integer = Config.KsbGateFaultDataUllRetryIntervalTicks
        Dim retryCntF As Integer = Config.KsbGateFaultDataUllMaxRetryCountToForget
        Dim retryCntC As Integer = Config.KsbGateFaultDataUllMaxRetryCountToCare
        Dim sAppId As String = "ForFaultData"

        With oScheduledUllSpecDictionary
            .Add("ERR", New TelServerAppScheduledUllSpec(objCode, tranLim, startLim, retryItv, retryCntF, retryCntC, sAppId))
        End With
    End Sub

    Protected Overridable Sub AddKadoDataItemsToScheduledUllSpecDictionary()
        Dim objCode As Byte = CByte(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateKadoData)
        Dim tranLim As Integer = Config.GateKadoDataUllTransferLimitTicks
        Dim startLim As Integer = Config.GateKadoDataUllStartReplyLimitTicks
        Dim retryItv As Integer = Config.GateKadoDataUllRetryIntervalTicks
        Dim retryCntF As Integer = Config.GateKadoDataUllMaxRetryCountToForget
        Dim retryCntC As Integer = Config.GateKadoDataUllMaxRetryCountToCare
        Dim sAppId As String = "ForKadoData"

        With oScheduledUllSpecDictionary
            .Add("KDO", New TelServerAppScheduledUllSpec(objCode, tranLim, startLim, retryItv, retryCntF, retryCntC, sAppId))
        End With
    End Sub

    Protected Overridable Sub AddTrafficDataItemsToScheduledUllSpecDictionary()
        Dim objCode As Byte = CByte(EkServerDrivenUllReqTelegram.FormalObjCodeAsGateTrafficData)
        Dim tranLim As Integer = Config.GateTrafficDataUllTransferLimitTicks
        Dim startLim As Integer = Config.GateTrafficDataUllStartReplyLimitTicks
        Dim retryItv As Integer = Config.GateTrafficDataUllRetryIntervalTicks
        Dim retryCntF As Integer = Config.GateTrafficDataUllMaxRetryCountToForget
        Dim retryCntC As Integer = Config.GateTrafficDataUllMaxRetryCountToCare
        Dim sAppId As String = "ForTrafficData"

        With oScheduledUllSpecDictionary
            .Add("TIM", New TelServerAppScheduledUllSpec(objCode, tranLim, startLim, retryItv, retryCntF, retryCntC, sAppId))
        End With
    End Sub

    Protected Overridable Sub AddItemsToMasProDlReflectSpecDictionary()
        Dim objCodeMasData As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsGateMasterData
        Dim objCodeProData As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsGateProgramData
        Dim objCodeProList As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsGateProgramList
        Dim objCodeWProData As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsKsbProgramData
        Dim objCodeWProList As Integer = EkMasProDlReflectReqTelegram.FormalObjCodeAsKsbProgramList
        Dim modelGate As String = EkConstants.ModelCodeGate
        Dim modelKsb As String = EkConstants.ModelCodeKanshiban
        Dim filePurpData As String = EkConstants.FilePurposeData
        Dim filePurpList As String = EkConstants.FilePurposeList
        Dim dataPurpMas As String = EkConstants.DataPurposeMaster
        Dim dataPurpPro As String = EkConstants.DataPurposeProgram

        With oMasProDlReflectSpecDictionary
            .Add(GenCplxObjCode(objCodeMasData, &H0), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, Nothing)) 'NOTE: ダミー
            .Add(GenCplxObjCode(objCodeMasData, &H47), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "DSH"))
            .Add(GenCplxObjCode(objCodeMasData, &H48), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "LOS"))
            .Add(GenCplxObjCode(objCodeMasData, &H49), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "DSC"))
            .Add(GenCplxObjCode(objCodeMasData, &H4A), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "HLD"))
            .Add(GenCplxObjCode(objCodeMasData, &H4B), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "EXP"))
            .Add(GenCplxObjCode(objCodeMasData, &H4C), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "FRX"))
            .Add(GenCplxObjCode(objCodeMasData, &H4D), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "LST"))
            .Add(GenCplxObjCode(objCodeMasData, &H3E), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "FJW"))
            .Add(GenCplxObjCode(objCodeMasData, &H43), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "IJW"))
            .Add(GenCplxObjCode(objCodeMasData, &H4E), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "FJC"))
            .Add(GenCplxObjCode(objCodeMasData, &H4F), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "IJC"))
            .Add(GenCplxObjCode(objCodeMasData, &H50), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "FJR"))
            .Add(GenCplxObjCode(objCodeMasData, &H56), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "IJE"))
            .Add(GenCplxObjCode(objCodeMasData, &H59), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "KEN"))
            .Add(GenCplxObjCode(objCodeMasData, &H41), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "DLY"))
            .Add(GenCplxObjCode(objCodeMasData, &H44), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "ICH"))
            .Add(GenCplxObjCode(objCodeMasData, &H42), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "PAY"))
            .Add(GenCplxObjCode(objCodeMasData, &H64), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "CYC"))
            .Add(GenCplxObjCode(objCodeMasData, &H63), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "STP"))
            .Add(GenCplxObjCode(objCodeMasData, &H62), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "PNO"))
            .Add(GenCplxObjCode(objCodeMasData, &H61), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "FRC"))
            .Add(GenCplxObjCode(objCodeMasData, &H66), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "DUS"))
            .Add(GenCplxObjCode(objCodeMasData, &H70), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "NSI"))
            .Add(GenCplxObjCode(objCodeMasData, &H71), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "NTO"))
            .Add(GenCplxObjCode(objCodeMasData, &H72), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "NIC"))
            .Add(GenCplxObjCode(objCodeMasData, &H73), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "NJW"))
            '----------- 0.1  新規マスタ追加対応   ADD  START------------------------
            .Add(GenCplxObjCode(objCodeMasData, &H80), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "FSK"))
            .Add(GenCplxObjCode(objCodeMasData, &H84), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "IUZ"))
            .Add(GenCplxObjCode(objCodeMasData, &H85), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "KSZ"))
            .Add(GenCplxObjCode(objCodeMasData, &H86), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "IUK"))
            .Add(GenCplxObjCode(objCodeMasData, &H87), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "SWK"))
            '----------- 0.3  ポイントポストペイ対応   ADD  START------------------------
            .Add(GenCplxObjCode(objCodeMasData, &H8A), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "HIR"))
            .Add(GenCplxObjCode(objCodeMasData, &H89), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpMas, "PPA"))
            '----------- 0.3  ポイントポストペイ対応   ADD    END------------------------
            '----------- 0.1  新規マスタ追加対応   ADD    END------------------------
            .Add(GenCplxObjCode(objCodeProData, &H0), New TelServerAppMasProDlReflectSpec(modelGate, filePurpData, dataPurpPro, "GPG"))
            .Add(GenCplxObjCode(objCodeProList, &H0), New TelServerAppMasProDlReflectSpec(modelGate, filePurpList, dataPurpPro, "GPG"))
            .Add(GenCplxObjCode(objCodeWProData, &H0), New TelServerAppMasProDlReflectSpec(modelKsb, filePurpData, dataPurpPro, "WPG"))
            .Add(GenCplxObjCode(objCodeWProList, &H0), New TelServerAppMasProDlReflectSpec(modelKsb, filePurpList, dataPurpPro, "WPG"))
        End With
    End Sub

    Protected Overridable Sub AddItemsToByteArrayPassivePostSpecDictionary()
        With oByteArrayPassivePostSpecDictionary
            .Add(CByte(EkByteArrayPostReqTelegram.FormalObjCodeAsKsbGateFaultData), New TelServerAppByteArrayPassivePostSpec("ForFaultData"))
            .Add(CByte(EkByteArrayPostReqTelegram.FormalObjCodeAsKsbConfig), New TelServerAppByteArrayPassivePostSpec("ForKsbConfig"))
            .Add(CByte(EkByteArrayPostReqTelegram.FormalObjCodeAsKsbConStatus), New TelServerAppByteArrayPassivePostSpec("ForConStatus"))
        End With
    End Sub

    Protected Overridable Sub AddItemsToVersionInfoUllSpecDictionary()
        Dim objCodeMas As Byte = CByte(EkClientDrivenUllReqTelegram.FormalObjCodeAsGateMasterVerInfo)
        Dim objCodePro As Byte = CByte(EkClientDrivenUllReqTelegram.FormalObjCodeAsGateProgramVerInfo)
        Dim objCodeWPro As Byte = CByte(EkClientDrivenUllReqTelegram.FormalObjCodeAsKsbProgramVerInfo)
        Dim modelGate As String = EkConstants.ModelCodeGate
        Dim modelKsb As String = EkConstants.ModelCodeKanshiban
        Dim dataPurpMas As String = EkConstants.DataPurposeMaster
        Dim dataPurpPro As String = EkConstants.DataPurposeProgram
        Dim proGroupTitles As String() = Config.GateProgramGroupTitles
        Dim wproGroupTitles As String() = New String() {""}
        Dim masTranLim As Integer = Config.GateMasterVersionInfoUllTransferLimitTicks
        Dim proTranLim As Integer = Config.GateProgramVersionInfoUllTransferLimitTicks
        Dim wproTranLim As Integer = Config.KsbProgramVersionInfoUllTransferLimitTicks

        With oVersionInfoUllSpecDictionary
            .Add(objCodeMas, New TelServerAppVersionInfoUllSpec(modelGate, dataPurpMas, Nothing, masTranLim))
            .Add(objCodePro, New TelServerAppVersionInfoUllSpec(modelGate, dataPurpPro, proGroupTitles, proTranLim))
            .Add(objCodeWPro, New TelServerAppVersionInfoUllSpec(modelKsb, dataPurpPro, wproGroupTitles, wproTranLim))
        End With
    End Sub
#End Region

End Class
