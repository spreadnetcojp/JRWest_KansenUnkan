' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)河脇  新規作成
'   0.1      2014/04/01       金沢  北陸対応　INI定義ファイルリストを取得
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Config
    Inherits RecServerAppBaseConfig

    '登録対象データ書式ファイルのパス
    '-------------------------------------------------------------
    Public Const KadoFormatFilePath_G As String = "KadoDataFormatFilePath_G"
    Public Shared KadoFormatFilePath_Y As String
    Public Const HosyuFormatFilePath As String = "HosyuDataFormatFilePath"
    Protected Const KADOINPUTPATH_SECTION As String = "Path"
    Public Shared KadoFormatFileG As New ArrayList
    Public Shared HosyuFormatFile As New ArrayList
    '-------------------------------------------------------------
    '入力データ別（プロセス別）キーに対するプレフィックス
    Private Const DATA_NAME As String = "KadoData"

    ''' <summary>INIファイルから運管サーバの稼動・保守データ登録プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        RecServerAppBaseInit(sIniFilePath, DATA_NAME)
        Dim i As Integer
        Try
            '------------------Ver0.1　北陸対応　MOD START-------------------------------
            i = 0
            Do
                ReadFileElem(KADOINPUTPATH_SECTION, KadoFormatFilePath_G + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & KADOINPUTPATH_SECTION & ", Key: " & KadoFormatFilePath_G & ")")
                    Else
                        Exit Do
                    End If
                End If
                KadoFormatFileG.Add(LastReadValue)
                i = i + 1
            Loop
            '------------------Ver0.1　北陸対応　MOD  END-------------------------------
            ReadFileElem(PATH_SECTION, "KadoDataFormatFilePath_Y")
            KadoFormatFilePath_Y = LastReadValue
            '------------------Ver0.1　北陸対応　MOD START-------------------------------
            i = 0
            Do
                ReadFileElem(KADOINPUTPATH_SECTION, HosyuFormatFilePath + "_" + CStr(i), False)
                If LastReadValue Is Nothing Then
                    If i = 0 Then
                        Throw New OPMGException("It's not defined or has too long value. (Section: " & KADOINPUTPATH_SECTION & ", Key: " & HosyuFormatFilePath & ")")
                    Else
                        Exit Do
                    End If
                End If
                HosyuFormatFile.Add(LastReadValue)
                i = i + 1
            Loop
            '------------------Ver0.1　北陸対応　MOD  END-------------------------------
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
