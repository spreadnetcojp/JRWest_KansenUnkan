' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/01/14  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' 多重駅務機器シミュレータのメイン処理を実装するクラス。
''' </summary>
Public Class MainClass

    ''' <summary>
    ''' 多重駅務機器シミュレータのメイン処理。
    ''' </summary>
    <STAThread()> _
    Public Shared Sub Main()
        Application.EnableVisualStyles()
        Dim sWorkingDir As String = Environment.CurrentDirectory
        Dim m As New Mutex(False, "ExOpmgMultiplexEkimuSim@" & sWorkingDir.ToUpperInvariant().Replace("\", "/"))
        If m.WaitOne(0, False) Then
            Dim sLogBasePath As String = Path.Combine(sWorkingDir, "LOG")
            Log.Init(sLogBasePath, "MultiplexEkimuSim")
            Log.Info("プロセス開始")

            Using oForm As New MainForm()
                Try
                    '画面表示（UI用メッセージループ実行）
                    oForm.ShowDialog()
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oForm.Close()
                End Try
            End Using

            Log.Info("プロセス終了")

            'NOTE: ここを通らなくても、このスレッドの消滅とともに解放される
            'ようなので、最悪の心配はない。
            m.ReleaseMutex()
        Else
            AlertBox.Show(Lexis.DoNotExecInSameWorkingDir)
        End If
    End Sub

End Class
