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
''' 駅務機器からサーバ要求ULLで収集するデータのファイル名を取り扱うためのクラス。
''' </summary>
Public Class EkScheduledDataFileName

#Region "定数"
    Private Shared ReadOnly oFileNameRegx As New Regex("^SK_[A-Z]{3}.DAT$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
#End Region

#Region "メソッド"
    ''' <summary>
    ''' ファイル名がサーバ要求ULLで収集するデータのものであるか否かを判定する。
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
        '正しいものであるか別途チェックすることを前提としている。
        Return True
    End Function

    ''' <summary>
    ''' サーバ要求ULLで収集するファイル名からデータの種別を取得する。
    ''' </summary>
    ''' <param name="sFileName">サーバ要求ULLで収集するファイル名</param>
    ''' <returns>データの種別（"DSH"等）</returns>
    Public Shared Function GetKind(ByVal sFileName As String) As String
        Return sFileName.Substring(3, 3).ToUpperInvariant()
    End Function

    ''' <summary>
    ''' 厳密なファイル名を生成する。
    ''' </summary>
    ''' <param name="sFileName">サーバ要求ULLで収集するファイル名</param>
    ''' <returns>サーバ要求ULLで収集する厳密なファイル名</returns>
    Public Shared Function Normalize(ByVal sFileName As String) As String
        Return Gen(GetKind(sFileName))
    End Function

    ''' <summary>
    ''' サーバ要求ULLで収集するデータのファイル名を生成する。
    ''' </summary>
    ''' <remarks>
    ''' 生成するファイル名の形式は本クラスの内部にカプセル化してある。
    ''' </remarks>
    ''' <param name="sKind">データの種別（"KDO"等）</param>
    ''' <returns>サーバ要求ULLで収集するファイル名</returns>
    Public Shared Function Gen(ByVal sKind As String) As String
        Return "SK_" & sKind & ".DAT"
    End Function
#End Region

End Class
