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

'NOTE: 現在のところ、使用していない。

''' <summary>
''' 監視盤プログラムの要素ファイルのフッタ。
''' </summary>
Public Class EkProgramElementFooterForW
    Inherits EkProgramElementFooter

    'NOTE: sFooteredFilePathにファイルがない場合や、ファイルの長さが短い場合などには、
    'IOExceptionをスローします。
    Public Sub New(ByVal sFooteredFilePath As String)
        MyBase.New(sFooteredFilePath)

        Me.VersionPos = 8
        Me.VersionLen = 8
        Me.DispNamePos = 24
        Me.DispNameLen = 64
    End Sub

End Class
