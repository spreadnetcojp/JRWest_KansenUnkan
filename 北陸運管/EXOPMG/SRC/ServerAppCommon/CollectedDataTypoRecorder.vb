' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2013/06/18  (NES)小林  秒内に発生する別内容の異常に対応、
'                                   レースコンディション除去
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.ServerApp.RecDataStructure

''' <summary>
''' 収集データに検出した異常を登録するためのクラス。
''' </summary>
Public Class CollectedDataTypoRecorder

#Region "定数や変数"
    Private Const UserId As String = "System"
    Private Const MachineId As String = "Server"
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 収集データより検出した異常をDBに登録する。
    ''' </summary>
    ''' <param name="infoObj">駅務機器から受信するデータの基本情報</param>
    ''' <param name="dataKind">データ種別</param> 
    ''' <param name="errInfo">異常内容</param> 
    ''' <returns>True:成功、False:失敗</returns>
    Public Shared Function Record(ByVal infoObj As BaseInfo, ByVal dataKind As String, ByVal errInfo As String) As Boolean
        'NOTE: infoObj.STATION_CODE.RAIL_SECTION_CODEや
        'infoObj.STATION_CODE.STATION_ORDER_CODEの桁数については、
        'それらの妥当性がチェックされる前に、他の項目の不正によって
        '本メソッドが呼び出されることを想定し、ここでゼロパディングを
        '行う（運管端末からみえなくなる可能性を極力減らす）。
        'コーナーコード等が数字でない場合は、本メソッドは呼び出されない
        'はずであるが、呼び出されたとしてもログを出力することで対応する。
        Dim sSQL As String = _
           "MERGE INTO D_COLLECTED_DATA_TYPO AS Target" _
           & " USING (SELECT '" & infoObj.STATION_CODE.RAIL_SECTION_CODE.PadLeft(3, "0"c) & "' RAIL_SECTION_CODE," _
                         & " '" & infoObj.STATION_CODE.STATION_ORDER_CODE.PadLeft(3, "0"c) & "' STATION_ORDER_CODE," _
                         & " " & infoObj.CORNER_CODE & " CORNER_CODE," _
                         & " '" & infoObj.MODEL_CODE & "' MODEL_CODE," _
                         & " " & infoObj.UNIT_NO & " UNIT_NO," _
                         & " '" & dataKind & "' DATA_KIND," _
                         & " '" & infoObj.PROCESSING_TIME & "' PROCESSING_TIME," _
                         & " '" & errInfo & "' ERROR_INFO) AS Source" _
           & " ON (Target.RAIL_SECTION_CODE = Source.RAIL_SECTION_CODE" _
            & " AND Target.STATION_ORDER_CODE = Source.STATION_ORDER_CODE" _
            & " AND Target.CORNER_CODE = Source.CORNER_CODE" _
            & " AND Target.MODEL_CODE = Source.MODEL_CODE" _
            & " AND Target.UNIT_NO = Source.UNIT_NO" _
            & " AND Target.DATA_KIND = Source.DATA_KIND" _
            & " AND Target.PROCESSING_TIME = Source.PROCESSING_TIME" _
            & " AND Target.ERROR_INFO = Source.ERROR_INFO)" _
           & " WHEN MATCHED THEN" _
            & " UPDATE" _
             & " SET Target.UPDATE_DATE = GETDATE()," _
                 & " Target.UPDATE_USER_ID = '" & UserId & "'," _
                 & " Target.UPDATE_MACHINE_ID = '" & MachineId & "'" _
           & " WHEN NOT MATCHED THEN" _
            & " INSERT (INSERT_DATE," _
                    & " INSERT_USER_ID," _
                    & " INSERT_MACHINE_ID," _
                    & " UPDATE_DATE," _
                    & " UPDATE_USER_ID," _
                    & " UPDATE_MACHINE_ID," _
                    & " RAIL_SECTION_CODE," _
                    & " STATION_ORDER_CODE," _
                    & " CORNER_CODE," _
                    & " MODEL_CODE," _
                    & " UNIT_NO," _
                    & " DATA_KIND," _
                    & " PROCESSING_TIME," _
                    & " ERROR_INFO)" _
            & " VALUES (GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " GETDATE()," _
                    & " '" & UserId & "'," _
                    & " '" & MachineId & "'," _
                    & " Source.RAIL_SECTION_CODE," _
                    & " Source.STATION_ORDER_CODE," _
                    & " Source.CORNER_CODE," _
                    & " Source.MODEL_CODE," _
                    & " Source.UNIT_NO," _
                    & " Source.DATA_KIND," _
                    & " Source.PROCESSING_TIME," _
                    & " Source.ERROR_INFO);"

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            dbCtl.ExecuteSQLToWrite(sSQL)
            dbCtl.TransactionCommit()
            Return True

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            dbCtl.TransactionRollBack()
            Return False

        Finally
            dbCtl.ConnectClose()
        End Try
    End Function
#End Region

End Class
