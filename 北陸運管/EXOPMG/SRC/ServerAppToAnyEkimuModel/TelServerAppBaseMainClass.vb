' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' 通信プロセス共通のメイン処理を実装するクラス。
''' </summary>
Public Class TelServerAppBaseMainClass
    Inherits ServerAppBaseMainClass

#Region "定数や変数"
    'メインウィンドウ
    Protected Friend Shared oMainForm As ServerAppForm
#End Region

#Region "メソッド"
    ''' <summary>
    ''' 通信プロセスの共通メイン処理。
    ''' </summary>
    ''' <remarks>
    ''' 各通信プロセスのメイン処理から呼び出す。
    ''' </remarks>
    Protected Shared Sub TelServerAppBaseMain(ByVal oListener As TelServerAppListener)
        Try
            'メッセージループがアイドル状態になる前（かつ、定期的にそれを行う
            'スレッドを起動する前）に、生存証明ファイルを更新しておく。
            Directory.CreateDirectory(TelServerAppBaseConfig.ResidentAppPulseDirPath)
            ServerAppPulser.Pulse()

            oMainForm = New ServerAppForm()

            '通信管理スレッドを開始する。
            Log.Info("Starting the listener thread...")
            oListener.Start()

            'ウインドウプロシージャを実行する。
            'NOTE: このメソッドから例外がスローされることはない。
            ServerAppBaseMain(oMainForm)

            Try
                '通信管理スレッドに終了を要求する。
                Log.Info("Sending quit request to the listener thread...")
                oListener.Quit()

                'NOTE: 以下で通信管理スレッドが終了しない場合、
                '通信管理スレッドは生存証明を行わないはずであり、
                '状況への対処はプロセスマネージャで行われる想定である。

                '通信管理スレッドの終了を待つ。
                Log.Info("Waiting for the listener thread to quit...")
                oListener.Join()
                Log.Info("The listener thread has quit.")
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                oListener.Abort()
            End Try
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        Finally
            If oMainForm IsNot Nothing Then
                oMainForm.Dispose()
            End If
        End Try
    End Sub
#End Region

End Class
