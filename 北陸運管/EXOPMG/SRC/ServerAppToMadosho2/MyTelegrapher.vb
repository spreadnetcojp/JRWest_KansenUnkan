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

'-------Ver0.1 次世代車補対応 MOD START-----------
''' <summary>
''' 窓処と利用データ用コネクションで電文の送受信を行うクラス。
''' </summary>
Public Class MyTelegrapher
    Inherits TelServerAppTelegrapher

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
          Lexis.Madosho2LineErrorAlertMailSubject, _
          Lexis.Madosho2LineErrorAlertMailBody)
        Me.formalObjCodeOfWatchdog = EkWatchdogReqTelegram.FormalObjCodeInMadosho
        Me.oRiyoDataUllSpecOfObjCodes = Config.RiyoDataUllSpecOfObjCodes

        'アクセスする予定のディレクトリについて、無ければ作成しておく。
        'NOTE: 基底クラスが作成するものや、必ずサブディレクトリの作成から
        '行うことになるものについては、対象外とする。
        Directory.CreateDirectory(sRiyoDataInputDirPath)
        Directory.CreateDirectory(sRiyoDataRejectDirPath)
    End Sub
    '-------Ver0.1 次世代車補対応 MOD END-------------
#End Region

End Class
'-------Ver0.1 次世代車補対応 MOD END-------------
