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
''' 監視盤のプログラムバージョン情報を書き出す際に使用するクラス。
''' </summary>
Public Class ExProgramVersionInfoForW

    Private Const FolderName As String = ""
    Private Const FileName As String = ""
    Private Const ProductName As String = "EW7200"
    Private Const CreationDate As String = ""
    Private Const Reserved As String = ""
    Private Const DispName As String = "監視盤アプリケーション"

    'NOTE: 書き出せない場合などに、IOExceptionをスローし得ます。
    Public Shared Sub WriteToStream(ByVal oProgram As HoldingKsbProgram, ByVal oOutputStream As Stream)
        Dim len As Integer = (8 + 2) + (12 + 96)

        Dim RawBytes(len - 1) As Byte
        Dim pos As Integer = 0

        Utility.FillBytes(&H20, RawBytes, pos, 8)
        Encoding.UTF8.GetBytes(FolderName, 0, FolderName.Length, RawBytes, pos)
        pos += 8

        Utility.CopyUInt16ToLeBytes2(CType(1, UShort), RawBytes, pos)
        pos += 2

        Utility.FillBytes(&H20, RawBytes, pos, 12)
        Encoding.UTF8.GetBytes(FileName, 0, FileName.Length, RawBytes, pos)
        pos += 12

        Utility.FillBytes(&H20, RawBytes, pos, 8)
        Encoding.UTF8.GetBytes(ProductName, 0, ProductName.Length, RawBytes, pos)
        pos += 8

        If oProgram Is Nothing OrElse oProgram.DataVersion = 0 Then
            Utility.FillBytes(&H0, RawBytes, pos, 8)
        Else
            Encoding.UTF8.GetBytes(oProgram.DataVersion.ToString("D8"), 0, 8, RawBytes, pos)
        End If
        pos += 8

        Utility.FillBytes(&H20, RawBytes, pos, 6)
        Encoding.UTF8.GetBytes(CreationDate, 0, CreationDate.Length, RawBytes, pos)
        pos += 6

        Utility.FillBytes(&H20, RawBytes, pos, 2)
        Encoding.UTF8.GetBytes(Reserved, 0, Reserved.Length, RawBytes, pos)
        pos += 2

        Utility.FillBytes(&H20, RawBytes, pos, 64)
        Encoding.GetEncoding(932).GetBytes(DispName, 0, DispName.Length, RawBytes, pos)
        pos += 64

        Utility.CopyUInt32ToLeBytes4(CType(0, UInteger), RawBytes, pos)
        pos += 4

        Utility.CopyUInt32ToLeBytes4(CType(0, UInteger), RawBytes, pos)
        pos += 4

        oOutputStream.Write(RawBytes, 0, RawBytes.Length)
    End Sub

End Class
