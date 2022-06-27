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

Imports System.Net.Sockets

Imports JR.ExOpmg.Common

''' <summary>
''' 種別等が不明の電文。
''' </summary>
Public Class NkDodgyTelegram
    Inherits NkTelegram

    Friend Sub New(ByVal aRawBytes As Byte())
        MyBase.New(aRawBytes, Nothing, 0)
    End Sub

    'NOTE: このクラスのインスタンスからGetBodyFormatViolation()を行うことは無意味であり、
    '明らかに誤りであるため、誤って使用されたことが判るよう、あえて実装しています。
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        Debug.Fail("The caller of ITelegram.GetBodyFormatViolation() may be wrong.")
        Return NakCauseCode.None
    End Function

End Class
