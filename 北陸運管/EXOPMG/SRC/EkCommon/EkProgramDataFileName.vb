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

Imports System.Text.RegularExpressions

''' <summary>
''' 駅務機器のプログラムファイル名を取り扱うためのクラス。
''' </summary>
Public Class EkProgramDataFileName

#Region "定数"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[0-9]{2}_(" & EkConstants.SpecificCodeOfKanshiban & "|" & EkConstants.SpecificCodeOfGate & "|" & EkConstants.SpecificCodeOfMadosho & ")_[0-9]{1,8}\.CAB$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Const fileNameLenForGateAndMado As Integer = 18
    Private Const fileNameLenForKanshiban As Integer = 22
#End Region

#Region "メソッド"
    ''' <summary>
    ''' ファイル名がプログラムデータのものであるか否かを判定する。
    ''' </summary>
    ''' <remarks>
    ''' GetXxxxメソッドは、このメソッドの戻り値がTrueになるファイル名を
    ''' 引数に呼び出すことを前提とする。
    ''' </remarks>
    ''' <param name="sFileName">ファイル名</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        Dim sApplicableModel As String = GetApplicableModel(sFileName)
        If sApplicableModel.Equals("W") Then
            If sFileName.Length <> fileNameLenForKanshiban Then Return False
        Else
            If sFileName.Length <> fileNameLenForGateAndMado Then Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' プログラムファイル名からデータの種別（"WPG"または"GPG"または"YPG"）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' 取得する文字列の形式は本クラスの内部にカプセル化してある。
    ''' </remarks>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>データの種別（"WPG"等、適用リストファイル名中と同様の抽象データ種別名）</returns>
    Public Shared Function GetKind(ByVal sFileName As String) As String
        Dim sApplicableModel As String = GetApplicableModel(sFileName)
        Return sApplicableModel & "PG"
    End Function

    ''' <summary>
    ''' プログラムファイル名からデータのサブ種別（エリアNo）を取得する。
    ''' </summary>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>データのサブ種別（エリアNo）</returns>
    Public Shared Function GetSubKindAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetSubKind(sFileName))
    End Function

    ''' <summary>
    ''' プログラムファイル名からデータのサブ種別（エリアNo）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' ２桁の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>データのサブ種別（エリアNo）</returns>
    Public Shared Function GetSubKind(ByVal sFileName As String) As String
        Return sFileName.Substring(0, 2)
    End Function

    ''' <summary>
    ''' プログラムファイル名からデータの適用対象機種（"W"または"G"または"Y"）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' 取得する文字列の形式は本クラスの内部にカプセル化してある。
    ''' </remarks>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>データの適用対象機種（"W"等、適用リストファイル名中と同様の抽象データ機種名）</returns>
    Public Shared Function GetApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(4, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' プログラムファイル名からデータの適用対象製品コードを取得する。
    ''' </summary>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>データの適用対象製品コード</returns>
    Public Shared Function GetApplicableSpecificModel(ByVal sFileName As String) As String
        Return sFileName.Substring(3, 6).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' プログラムファイル名からデータのバージョン（代表バージョン）を取得する。
    ''' </summary>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>データのバージョン（代表バージョン）</returns>
    Public Shared Function GetVersionAsInt(ByVal sFileName As String) As Integer
        Dim nextSepPos As Integer = sFileName.IndexOf("."c, 11)
        Return Integer.Parse(sFileName.Substring(10, nextSepPos - 10))
    End Function

    ''' <summary>
    ''' プログラムファイル名からデータのバージョン（代表バージョン）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' 適用対象機種によって８桁または４桁の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>データのバージョン（代表バージョン）</returns>
    Public Shared Function GetVersion(ByVal sFileName As String) As String
        Dim intValue As Integer = GetVersionAsInt(sFileName)
        Dim sApplicableModel As String = GetApplicableModel(sFileName)
        Return intValue.ToString(EkConstants.ProgramDataVersionFormatOfModels(sApplicableModel))
    End Function

    ''' <summary>
    ''' 厳密なファイル名を生成する。
    ''' </summary>
    ''' <param name="sFileName">プログラムファイル名</param>
    ''' <returns>厳密なプログラムファイル名</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetSubKind(sFileName), GetApplicableModel(sFileName), GetVersion(sFileName))
    End Function

    ''' <summary>
    ''' プログラムのファイル名を生成する。
    ''' </summary>
    ''' <remarks>
    ''' 生成するファイル名の形式は本クラスの内部にカプセル化してある。
    ''' </remarks>
    ''' <param name="sSubKind">データのサブ種別（エリアNo）</param>
    ''' <param name="sApplicableModel">データの適用対象機種（"W"または"G"または"Y"）</param>
    ''' <param name="sVersion">データのバージョン（代表バージョン）</param>
    ''' <returns>プログラムファイル名</returns>
    Public Shared Function Gen( _
       ByVal sSubKind As String, _
       ByVal sApplicableModel As String, _
       ByVal sVersion As String) As String

        Return sSubKind & "_" & EkConstants.SpecificCodeOfModels(sApplicableModel) & "_" & sVersion & ".CAB"
    End Function
#End Region

End Class
