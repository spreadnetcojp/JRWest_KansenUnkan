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

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 保守系データのファイルヘッダ部を書き込むクラス。
''' </summary>
Public Class ExUpboundFileHeader

    'NOTE: 書き込めない場合などに、IOExceptionをスローし得ます。
    Public Shared Sub WriteToStream(ByVal dataKind As Byte, ByVal recCount As Integer, ByVal recLength As Integer, ByVal creationDate As DateTime, ByVal oOutputStream As Stream)
        Dim RawBytes(recLength - 1) As Byte
        Dim pos As Integer = 0
        RawBytes(pos) = dataKind
        pos += 1
        Utility.CopyUInt16ToLeBytes2(CType(recCount, UShort), RawBytes, pos)
        pos += 2
        Utility.CHARtoBCD(creationDate.ToString("yyyyMMddHHmmss"), 7).CopyTo(RawBytes, pos)
        pos += 7
        oOutputStream.Write(RawBytes, 0, RawBytes.Length)
    End Sub

End Class
