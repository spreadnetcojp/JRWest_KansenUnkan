'NOTE: 未使用
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
''' 運用系上りデータの基本ヘッダ部をファイルに書き込むクラス。
''' </summary>
Public Class ExVersionInfoFileHeader

    Public Const Length As Integer = 17

    'NOTE: 書き出せない場合などに、IOExceptionをスローし得ます。
    Public Shared Sub WriteToStream(ByVal dataKind As Byte, ByVal machineCode As EkCode, ByVal procDate As DateTime, ByVal version As Byte, ByVal oOutputStream As Stream)
        Dim RawBytes(Length - 1) As Byte
        Dim pos As Integer = 0
        RawBytes(pos) = dataKind
        pos += 1
        RawBytes(pos) = CType(machineCode.RailSection, Byte)
        pos += 1
        RawBytes(pos) = CType(machineCode.StationOrder, Byte)
        pos += 1
        Utility.CHARtoBCD(procDate.ToString("yyyyMMddHHmmss"), 7).CopyTo(RawBytes, pos)
        pos += 7
        RawBytes(pos) = CType(machineCode.Corner, Byte)
        pos += 1
        RawBytes(pos) = CType(machineCode.Unit, Byte)
        pos += 1
        Utility.CopyUInt32ToLeBytes4(0, RawBytes, pos)
        pos += 4
        RawBytes(pos) = version
        pos += 1
        oOutputStream.Write(RawBytes, 0, RawBytes.Length)
    End Sub

End Class
