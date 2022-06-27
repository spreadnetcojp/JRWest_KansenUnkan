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
''' 駅務機器のマスタファイル名を取り扱うためのクラス。
''' </summary>
Public Class EkMasterDataFileName

#Region "定数"
    Private Shared ReadOnly oFileNameRegx As New Regex("^PR_[A-Z]{3}[0-9]{2}_[GY]_[0-9]{3}_[0-9]{8}\.bin$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
#End Region

#Region "メソッド"
    ''' <summary>
    ''' ファイル名がマスタデータのものであるか否かを判定する。
    ''' </summary>
    ''' <remarks>
    ''' GetXxxxメソッドは、このメソッドの戻り値がTrueになるファイル名を
    ''' 引数に呼び出すことを前提とする。
    ''' </remarks>
    ''' <param name="sFileName">ファイル名</param>
    ''' <returns>判定結果</returns>
    Public Shared Function IsValid(ByVal sFileName As String) As Boolean
        If Not oFileNameRegx.IsMatch(sFileName) Then Return False

        'NOTE: このメソッドではKindの値はチェックしない。
        'このメソッドを呼び出した後、GetKindの戻り値が
        'DBに登録されているか別途チェックすることを前提としている。

        Dim version As Integer = GetVersionAsInt(sFileName)
        If version < 1 OrElse version > 255 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' マスタファイル名からデータの種別を取得する。
    ''' </summary>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>データの種別（"DSH"等）</returns>
    Public Shared Function GetKind(ByVal sFileName As String) As String
        Return sFileName.Substring(3, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' マスタファイル名からデータのサブ種別（パターンNo）を取得する。
    ''' </summary>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>データのサブ種別（パターンNo）</returns>
    Public Shared Function GetSubKindAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetSubKind(sFileName))
    End Function

    ''' <summary>
    ''' マスタファイル名からデータのサブ種別（パターンNo）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' ２桁の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>データのサブ種別（パターンNo）</returns>
    Public Shared Function GetSubKind(ByVal sFileName As String) As String
        Return sFileName.Substring(6, 2)
    End Function

    ''' <summary>
    ''' マスタファイル名からデータの適用対象機種（"G"または"Y"）を取得する。
    ''' </summary>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>データの適用対象機種（"G"または"Y"）</returns>
    Public Shared Function GetApplicableModel(ByVal sFileName As String) As String
        Return sFileName.Substring(9, 1).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' マスタファイル名からデータのバージョン（マスタバージョン）を取得する。
    ''' </summary>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>データのバージョン（マスタバージョン）</returns>
    Public Shared Function GetVersionAsInt(ByVal sFileName As String) As Integer
        Return Integer.Parse(GetVersion(sFileName))
    End Function

    ''' <summary>
    ''' マスタファイル名からデータのバージョン（マスタバージョン）を取得する。
    ''' </summary>
    ''' <remarks>
    ''' ３桁の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>データのバージョン（マスタバージョン）</returns>
    Public Shared Function GetVersion(ByVal sFileName As String) As String
        Return sFileName.Substring(11, 3)
    End Function

    ''' <summary>
    ''' マスタファイル名からユーザ定義メモ値を取得する。
    ''' </summary>
    ''' <remarks>
    ''' ８桁の文字列として取得する。
    ''' </remarks>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>ユーザ定義メモ値</returns>
    Public Shared Function GetUserMemo(ByVal sFileName As String) As String
        Return sFileName.Substring(15, 8)
    End Function

    ''' <summary>
    ''' 厳密なファイル名を生成する。
    ''' </summary>
    ''' <param name="sFileName">マスタファイル名</param>
    ''' <returns>厳密なマスタファイル名</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetKind(sFileName), GetSubKind(sFileName), GetApplicableModel(sFileName), GetVersion(sFileName), GetUserMemo(sFileName))
    End Function

    ''' <summary>
    ''' マスタのファイル名を生成する。
    ''' </summary>
    ''' <remarks>
    ''' 生成するファイル名の形式は本クラスの内部にカプセル化してある。
    ''' </remarks>
    ''' <param name="sKind">データの種別（"DSH"等）</param>
    ''' <param name="sSubKind">データのサブ種別（パターンNo）</param>
    ''' <param name="sApplicableModel">データの適用対象機種（"G"または"Y"）</param>
    ''' <param name="sVersion">データのバージョン（マスタバージョン）</param>
    ''' <param name="sUserMemo">ユーザ定義メモ値</param>
    ''' <returns>マスタファイル名</returns>
    Public Shared Function Gen( _
       ByVal sKind As String, _
       ByVal sSubKind As String, _
       ByVal sApplicableModel As String, _
       ByVal sVersion As String, _
       ByVal sUserMemo As String) As String

        Return "PR_" & sKind & sSubKind & "_" & sApplicableModel & "_" & sVersion & "_" & sUserMemo & ".bin"
    End Function
#End Region

End Class
