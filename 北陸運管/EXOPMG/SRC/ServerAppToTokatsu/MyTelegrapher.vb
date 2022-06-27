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
Imports System.Net.Sockets

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Telegramming

''' <summary>
''' 統括と電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits TelServerAppTelegrapher

#Region "定数や変数"
    'このクラス用のマスタ/プログラム一式DLLの仕様
    Protected Shared oMasProSuiteDllSpecDictionary As New Dictionary(Of String, TelServerAppMasProDllSpec)

    'このクラス用のマスタ/プログラム適用リストDLLの仕様
    Protected Shared oMasProListDllSpecDictionary As New Dictionary(Of String, TelServerAppMasProDllSpec)

    'このクラス用のマスタ/プログラムDL完了通知の仕様
    Protected Shared oMasProDlReflectSpecDictionary As New Dictionary(Of UShort, TelServerAppMasProDlReflectSpec)

    'このクラス用のバージョン情報ULLの仕様
    Protected Shared oVersionInfoUllSpecDictionary As New Dictionary(Of Byte, TelServerAppVersionInfoUllSpec)

    '接続状態取得実施タイマ
    Protected oConStatusGetTimer As TickTimer
#End Region

#Region "コンストラクタ"
    '-------Ver0.1 次世代車補対応 MOD START-----------
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

        'アクセスする予定のディレクトリについて、無ければ作成しておく。
        'NOTE: 基底クラスが作成するものや、必ずサブディレクトリの作成から
        '行うことになるものについては、対象外とする。
        Directory.CreateDirectory(Config.InputDirPathForApps("ForConStatus"))
        Directory.CreateDirectory(Config.RejectDirPathForApps("ForConStatus"))
    End Sub
    '-------Ver0.1 次世代車補対応 MOD END-------------

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
            .Add(GenCplxObjCode(objCodeMasData, &H0), New TelServerAppMasProDlReflectSpec(modelMado, filePurpData, dataPurpMas, Nothing)) 'NOTE: ダミー
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

#Region "イベント処理メソッド"
    '親スレッドからコネクションを受け取った場合（通信状態の変化をフックするためのメソッド）
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

    '能動的単発シーケンスが成功した場合
    Protected Overrides Sub ProcOnActiveOneComplete(ByVal iReqTeleg As IReqTelegram, ByVal iAckTeleg As ITelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Info("ConStatusGet completed.")

            Dim oAckTeleg As EkByteArrayGetAckTelegram = DirectCast(iAckTeleg, EkByteArrayGetAckTelegram)

            Dim sDstPath As String = UpboundDataPath.Gen(Config.InputDirPathForApps("ForConStatus"), clientCode, DateTime.Now)
            If UpboundDataPath.GetBranchNumber(sDstPath) <= Config.MaxBranchNumberForApps("ForConStatus") Then
                '一時作業用ディレクトリでファイル化する。
                Dim sTmpPath As String = Path.Combine(sTempDirPath, sTempFileName)
                Try
                    Using oStream As New FileStream(sTmpPath, FileMode.Create, FileAccess.Write)
                        Dim aBytes As Byte() = oAckTeleg.ByteArray
                        oStream.Write(aBytes, 0, aBytes.Length)
                    End Using
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    'NOTE: 一応、ランタイムな条件次第で発生する例外もあるので、
                    'どうするのがベストかよく考えた方がよい。
                    Abort()
                End Try

                '作成したファイルを登録プロセスが読み取るパスに移動する。
                File.Move(sTmpPath, sDstPath)

                '登録プロセスに通知する。
                Config.MessageQueueForApps("ForConStatus").Send(New ExtFileCreationNotice())
            Else
                Log.Warn("Ignored.")
            End If

            '次回の取得に向けてタイマをセットする。
            RegisterTimer(oConStatusGetTimer, TickTimer.GetSystemTick())
        Else
            MyBase.ProcOnActiveOneComplete(iReqTeleg, iAckTeleg)
        End If
    End Sub

    '能動的単発シーケンスで異常とみなすべきでないリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToForget(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Warn("ConStatusGet skipped.")

            '次回の取得に向けてタイマをセットする。
            RegisterTimer(oConStatusGetTimer, TickTimer.GetSystemTick())
        Else
            MyBase.ProcOnActiveOneRetryOverToForget(iReqTeleg, iNakTeleg)
        End If
    End Sub

    '能動的単発シーケンスで異常とみなすべきリトライオーバーが発生した場合
    Protected Overrides Sub ProcOnActiveOneRetryOverToCare(ByVal iReqTeleg As IReqTelegram, ByVal iNakTeleg As INakTelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Error("ConStatusGet failed by retry over.")

            'NOTE: 機器接続状態が収集できていないことは、機器接続状態確認画面にて
            '最終収集日時をキーにソートすれば判断できるため、収集データ誤記テーブル
            'への登録は行わない（現行機と同じ仕様）。

            '次回の取得に向けてタイマをセットする。
            RegisterTimer(oConStatusGetTimer, TickTimer.GetSystemTick())
        Else
            MyBase.ProcOnActiveOneRetryOverToCare(iReqTeleg, iNakTeleg)
        End If
    End Sub

    '能動的単発シーケンスの最中やキューイングされた能動的単発シーケンスの実施前に通信異常を検出した場合
    Protected Overrides Sub ProcOnActiveOneAnonyError(ByVal iReqTeleg As IReqTelegram)
        Dim oReqTelegram As EkReqTelegram = DirectCast(iReqTeleg, EkReqTelegram)
        If oReqTelegram.SubCmdCode = EkSubCmdCode.Get AndAlso _
           oReqTelegram.ObjCode = EkByteArrayGetReqTelegram.FormalObjCodeAsTktConStatus Then
            Debug.Assert(oReqTelegram.GetType() Is GetType(EkByteArrayGetReqTelegram))
            Log.Error("ConStatusGet failed by telegramming error.")

            'NOTE: 機器接続状態が収集できていないことは、機器接続状態確認画面にて
            '最終収集日時をキーにソートすれば判断できるため、収集データ誤記テーブル
            'への登録は行わない（現行機と同じ仕様）。

            'NOTE: この場合は、この後でコネクションが切断される（或いは既に
            '切断されている）ため、oConStatusGetTimerのセットは無用である。
        Else
            MyBase.ProcOnActiveOneAnonyError(iReqTeleg)
        End If
    End Sub
#End Region

#Region "イベント処理実装用メソッド"
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
