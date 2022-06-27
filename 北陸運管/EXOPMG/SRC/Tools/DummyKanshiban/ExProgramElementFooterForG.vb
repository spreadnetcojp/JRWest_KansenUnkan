' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/02/16  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

''' <summary>
''' 改札機プログラムの要素ファイルのフッタ。
''' </summary>
Public Class ExProgramElementFooterForG
    Inherits EkProgramElementFooterForG

    Public Property Data() As Byte()
        Get
            Dim oBytes As Byte() = New Byte(Length - 1) {}
            Buffer.BlockCopy(RawBytes, 0, oBytes, 0, Length)
            Return oBytes
        End Get

        Set(ByVal oBytes As Byte())
            Buffer.BlockCopy(oBytes, 0, RawBytes, 0, Length)
        End Set
    End Property

    'NOTE: sFooteredFilePathにファイルがない場合や、ファイルの長さが短い場合などには、
    'IOExceptionをスローします。
    Public Sub New(ByVal sFooteredFilePath As String)
        MyBase.New(sFooteredFilePath)
    End Sub

End Class
