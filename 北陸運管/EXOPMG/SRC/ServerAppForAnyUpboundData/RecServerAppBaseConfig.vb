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

Imports JR.ExOpmg.Common

Public Class RecServerAppBaseConfig
    Inherits ServerAppBaseConfig

    '読み出し対象のメッセージキュー
    Public Shared MyMqPath As String

    '監視盤からのデータの受信ポート番号
    Public Shared InputIpPortFromKanshiban As Integer

    '統括からのデータの受信ポート番号
    Public Shared InputIpPortFromTokatsu As Integer

    '窓処からのデータの受信ポート番号
    Public Shared InputIpPortFromMadosho As Integer

    'INIファイル内における各設定項目のキー
    Private Const KANSHIBAN_PORT_KEY As String = "ToKanshibanTelegConnectionPort"
    Private Const TOKATSU_PORT_KEY As String = "ToTokatsuTelegConnectionPort"
    Private Const MADOSHO_PORT_KEY As String = "ToMadoshoTelegConnectionPort"

    ''' <summary>INIファイルから運管サーバの登録系プロセスに必須の設定値を取り込む。</summary>
    Public Shared Sub RecServerAppBaseInit(ByVal sIniFilePath As String, ByVal sDataName As String)
        Dim sAppIdentifier As String = "For" & sDataName
        ServerAppBaseInit(sIniFilePath, sAppIdentifier)

        Try
            ReadFileElem(MQ_SECTION, sAppIdentifier & MQ_PATH_KEY)
            MyMqPath = LastReadValue

            ReadFileElem(NETWORK_SECTION, KANSHIBAN_PORT_KEY)
            InputIpPortFromKanshiban = Integer.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, TOKATSU_PORT_KEY)
            InputIpPortFromTokatsu = Integer.Parse(LastReadValue)

            ReadFileElem(NETWORK_SECTION, MADOSHO_PORT_KEY)
            InputIpPortFromMadosho = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

    'NOTE: 呼び出さなくても問題ない。
    Public Shared Sub Dispose()
        ServerAppBaseDispose()
    End Sub

End Class
