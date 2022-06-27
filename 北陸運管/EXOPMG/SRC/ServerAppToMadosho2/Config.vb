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

Imports System.Globalization
Imports JR.ExOpmg.Common

Public Class Config
    Inherits TelServerAppBaseConfig

    '-------Ver0.1 次世代車補対応 MOD START-----------
    '窓処利用データセクションの内容
    Public Shared RiyoDataUllSpecOfObjCodes As New Dictionary(Of Byte, TelServerAppRiyoDataUllSpec)

    'プロセス別キーに対するプレフィックス
    Private Const MODEL_NAME As String = "Madosho2"

    ''' <summary>INIファイルから運管サーバの対窓処利用データ通信プロセスに必須の全設定値を取り込む。</summary>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        TelServerAppBaseInit(sIniFilePath, MODEL_NAME, True)

        Dim sAppIdentifier As String = "To" & MODEL_NAME
        Try
            RiyoDataUllSpecOfObjCodes = New Dictionary(Of Byte, TelServerAppRiyoDataUllSpec)
            Dim oTempDic As Dictionary(Of String, String) = GetFileSectionAsDictionary("MadoRiyoData")
            For Each oEntry As KeyValuePair(Of String, String) In oTempDic
                LastReadKey = oEntry.Key
                LastReadValue = oEntry.Value
                Dim code As Byte = Byte.Parse(LastReadKey, NumberStyles.HexNumber)
                Dim oElems As String() = LastReadValue.Split(","c)
                Dim oSpec As New TelServerAppRiyoDataUllSpec(oElems(0), oElems(1), Integer.Parse(oElems(2)), Integer.Parse(oElems(3)))
                RiyoDataUllSpecOfObjCodes.Add(code, oSpec)
            Next oEntry
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub
    '-------Ver0.1 次世代車補対応 MOD END-------------

    Public Shared Sub Dispose()
        TelServerAppBaseDispose()
    End Sub

End Class
