' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2017/08/08  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 窓処のプログラムバージョン情報を書き出す際に使用するクラス。
''' </summary>
Public Class ExProgramVersionInfoForY

    'NOTE: 書き出せない場合などに、IOExceptionをスローし得ます。
    Public Shared Sub WriteToStream(ByVal oProgram As HoldingProgram, ByVal oOutputStream As Stream, ByVal area As Integer)
        Dim len As Integer = ProgramVersionInfoUtil.RecordLengthInBytes
        Dim oBytes(len - 1) As Byte
        If oProgram IsNot Nothing Then

            'TODO: 各エリアにおいて適用される部材は本物の窓処に合わせる。
            '現状、TOICAエリアのみ、接続試験結果をもとに本物の窓処に合わせてある。

            'TODO: 下記が成立しない場合に、本物はどう対処するのか？
            If oProgram.ListHashValue IsNot Nothing Then
                ProgramVersionInfoUtil.SetFieldValueToBytes("プログラム適用リストバージョン", oProgram.ListVersion.ToString("D2"), oBytes)
                ProgramVersionInfoUtil.SetFieldValueToBytes("プログラム適用日", oProgram.ApplicableDate, oBytes)
            End If
            CopyVersionListToInfo(oProgram, oBytes, "共通部 ユーザコード")
            CopyVersionListToInfo(oProgram, oBytes, "共通部 適用エリア")
            CopyVersionListToInfo(oProgram, oBytes, "共通部 プログラム区分")
            CopyVersionListToInfo(oProgram, oBytes, "共通部 プログラム動作許可日")
            CopyVersionListToInfo(oProgram, oBytes, "共通部 プログラム全体Ver（新）")
            CopyVersionListToInfo(oProgram, oBytes, "共通部 プログラム全体Ver（現）")
            CopyVersionListToInfo(oProgram, oBytes, "共通部 予備")
            If area = 0 OrElse area = 1 OrElse area = 7 Then
                CopyVersionListToInfo(oProgram, oBytes, "在来IC判定バージョン(Suica)")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "在来IC判定バージョン(TOICA)")
            End If
            If area = 0 OrElse area = 2 Then
                CopyVersionListToInfo(oProgram, oBytes, "在来IC判定バージョン(ICOCA)")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "新幹線IC判定バージョン")
            End If
            CopyVersionListToInfo(oProgram, oBytes, "EXIC判定バージョン")
            If area = 0 OrElse area = 1 OrElse area = 7 Then
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データ世代1バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データ世代1適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データ世代2バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データ世代2適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データ名")
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データ全体ソフト型式")
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データバージョン")
                CopyVersionListToInfo(oProgram, oBytes, "Suica運賃データ作成年月日")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データ世代1バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データ世代1適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データ世代2バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データ世代2適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データ名")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データ全体ソフト型式")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データバージョン")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA運賃データ作成年月日")
            End If
            If area = 0 OrElse area = 2 Then
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データ世代1バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データ世代1適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データ世代2バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データ世代2適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データ名")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データ全体ソフト型式")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データバージョン")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA運賃データ作成年月日")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データ世代1バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データ世代1適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データ世代2バージョン")
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データ世代2適用年月日")
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データ名")
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データ全体ソフト型式")
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データバージョン")
                CopyVersionListToInfo(oProgram, oBytes, "特急料金データ作成年月日")
            End If
            CopyVersionListToInfo(oProgram, oBytes, "磁気ファームウェアバージョン")
            CopyVersionListToInfo(oProgram, oBytes, "予備")
            CopyVersionListToInfo(oProgram, oBytes, "未締切時プログラム非適用チェックフラグ")
            CopyVersionListToInfo(oProgram, oBytes, "未送有時プログラム非適用チェックフラグ")
            CopyVersionListToInfo(oProgram, oBytes, "備考")
        End If
        oOutputStream.Write(oBytes, 0, oBytes.Length)
    End Sub

    Private Shared Sub CopyVersionListToInfo(ByVal oProgram As HoldingProgram, ByVal oBytes As Byte(), ByVal sFieldName As String)
        ProgramVersionInfoUtil.SetFieldValueToBytes( _
          sFieldName, _
          ProgramVersionListUtil.GetFieldValueFromBytes(sFieldName, oProgram.VersionListData), _
          oBytes)
    End Sub

End Class
