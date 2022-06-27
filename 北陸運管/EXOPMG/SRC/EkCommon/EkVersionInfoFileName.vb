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
''' バージョン情報のファイル名を取り扱うためのクラス。
''' </summary>
Public Class EkVersionInfoFileName

#Region "定数"
    Private Shared ReadOnly oFileNameRegx As New Regex("^[MP]_[GWY]_[0-9]{12}VER.DAT$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
#End Region

#Region "メソッド"
    ''' <summary>
    ''' ファイル名がバージョン情報のものであるか否かを判定する。
    ''' </summary>
    ''' <remarks>
    ''' GetXxxxメソッドは、このメソッドの戻り値がTrueになるファイル名を
    ''' 引数に呼び出すことを前提とする。
    ''' </remarks>
    ''' <param name="sFileName">ファイル名</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        Dim sDataApplicableModel As String = GetDataApplicableModel(sFileName)
        If sDataApplicableModel.Equals("W") AndAlso GetDataPurpose(sFileName).Equals("MST") Then Return False

        Return True
    End Function

    ''' <summary>
    ''' バージョン情報ファイル名からバージョン付与対象データの用途を取得する。
    ''' </summary>
    ''' <param name="sFileName">バージョン情報ファイル名</param>
    ''' <returns>データ用途（"MST"または"PRG"）</returns>
    Public Shared Function GetDataPurpose(ByVal sFileName As String) As String
        Select Case sFileName.Substring(0, 1).ToUpperInvariant()
            Case "M"
                Return "MST"
            Case "P"
                Return "PRG"
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' バージョン情報ファイル名からデータの適用対象機種を取得する。
    ''' </summary>
    ''' <param name="sFileName">バージョン情報ファイル名</param>
    ''' <returns>データの適用対象機種（用途がマスタの場合は"G"または"Y"、用途がプログラムの場合は"W"または"G"または"Y"）</returns>
    Public Shared Function GetDataApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(2, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' バージョン情報ファイル名から対象号機の識別コードを取得する。
    ''' </summary>
    ''' <remarks>
    ''' 戻り値のModelプロパティは常に0である。
    ''' </remarks>
    ''' <param name="sFileName">バージョン情報ファイル名</param>
    ''' <returns>対象号機の識別コード</returns>
    Public Shared Function GetDataApplicableUnit(ByVal sFileName As String) As EkCode
        Return EkCode.Parse(sFileName.Substring(4, 12), "%3R%3S%4C%2U")
    End Function

    ''' <summary>
    ''' 厳密なファイル名を生成する。
    ''' </summary>
    ''' <param name="sFileName">バージョン情報ファイル名</param>
    ''' <returns>厳密なバージョン情報ファイル名</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetDataPurpose(sFileName), GetDataApplicableModel(sFileName), GetDataApplicableUnit(sFileName))
    End Function

    ''' <summary>
    ''' バージョン情報のファイル名を生成する。
    ''' </summary>
    ''' <remarks>
    ''' 生成するファイル名の形式は本クラスの内部にカプセル化してある。
    ''' </remarks>
    ''' <param name="sDataPurpose">データの用途（"MST"または"PRG"）</param>
    ''' <param name="sDataApplicableModel">データの適用対象機種（用途がマスタの場合は"G"または"Y"、用途がプログラムの場合は"W"または"G"または"Y"）</param>
    ''' <param name="dataApplicableUnit">データの対象号機</param>
    ''' <returns>バージョン情報ファイル名</returns>
    Public Shared Function Gen( _
       ByVal sDataPurpose As String, _
       ByVal sDataApplicableModel As String, _
       ByVal dataApplicableUnit As EkCode) As String

        Select Case sDataPurpose
            Case "MST"
                Return "M_" & sDataApplicableModel & dataApplicableUnit.ToString("_%3R%3S%4C%2UVER.DAT")
            Case "PRG"
                Return "P_" & sDataApplicableModel & dataApplicableUnit.ToString("_%3R%3S%4C%2UVER.DAT")
            Case Else
                Return Nothing
        End Select
    End Function
#End Region

End Class
