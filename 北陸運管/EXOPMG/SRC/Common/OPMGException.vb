' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  在来運管向けのものをベースに作成
' **********************************************************************
Option Strict On
Option Explicit On

Imports System.Text

''' <summary>
''' 【例外クラス】
''' </summary>
Public Class OPMGException
    Inherits Exception

    'メッセージプロパティのデフォルト値
    'NOTE: どこかからとってきたい。
    Private Const defaultMessage As String = "Some method fails in OPMG library."

#Region " コンストラクタ "
    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    Public Sub New()
        MyBase.New(defaultMessage)
    End Sub

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <param name="message">エラーメッセージ</param>
    ''' <remarks>
    ''' 任意のエラーメッセージを指定する場合のコンストラクタ。
    ''' </remarks>
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <param name="innerException">現在の例外の原因である例外</param>
    Public Sub New(ByVal innerException As Exception)
        MyBase.New(defaultMessage, innerException)
    End Sub

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <param name="innerException">現在の例外の原因である例外</param>
    ''' <param name="message">エラーメッセージ</param>
    ''' <remarks>
    ''' 任意のエラーメッセージを指定する場合のコンストラクタ。
    ''' </remarks>
    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub
#End Region

#Region " +s DetailHeader()  詳細ログのヘッダ行作成 "
    ''' <summary>
    ''' 詳細ログのヘッダ行作成
    ''' </summary>
    ''' <param name="placeName">発生場所</param>
    ''' <returns>詳細ログのヘッダ行</returns>
    ''' <remarks></remarks>
    Public Shared Function DetailHeader(ByVal placeName As String) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} fails.", placeName)
        Return sb.ToString()
    End Function
#End Region
#Region " +s DetailNull()  詳細ログのNothing出力 "
    ''' <summary>
    ''' 詳細ログのNothing出力
    ''' </summary>
    ''' <param name="objName">オブジェクト名</param>
    ''' <returns>詳細ログのNothing出力</returns>
    ''' <remarks>
    ''' オブジェクトがNothingである場合の出力を作成する。
    ''' </remarks>
    Public Shared Function DetailNull(ByVal objName As String) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} is Nothing.", objName)
        Return sb.ToString()
    End Function
#End Region

#Region " +s DetailNotNull()  詳細ログのNot Nothing出力 "
    ''' <summary>
    ''' 詳細ログのNot Nothing出力
    ''' </summary>
    ''' <param name="objName">オブジェクト名</param>
    ''' <returns>詳細ログのNot Nothing出力</returns>
    ''' <remarks>
    ''' オブジェクトがNothingでない場合の出力を作成する。
    ''' </remarks>
    Public Shared Function DetailNotNull(ByVal objName As String) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} is Something.", objName)
        Return sb.ToString()
    End Function
#End Region

#Region " +s DetailNullOrNotNull()  詳細ログのNothingまたはNot Nothing出力 "
    ''' <summary>
    ''' 詳細ログのNothingまたはNot Nothing出力
    ''' </summary>
    ''' <param name="objName">オブジェクト名</param>
    ''' <param name="objValue">オブジェクトへの参照</param>
    ''' <returns>詳細ログのNothingまたはNot Nothing出力</returns>
    ''' <remarks>
    ''' オブジェクトへの参照がNothingの場合、DetailNull()の結果を、
    ''' オブジェクトへの参照がNot Nothingの場合、DetailNotNull()の結果を返す。
    ''' </remarks>
    Public Shared Function DetailNullOrNotNull(ByVal objName As String, ByVal objValue As Object) As String
        Dim r$ = ""
        If IsNothing(objValue) Then
            r = DetailNull(objName)
        Else
            r = DetailNotNull(objName)
        End If
        Return r
    End Function
#End Region

#Region " +s DetailException()  詳細ログの例外出力 "
    ''' <summary>
    ''' 詳細ログの例外出力
    ''' </summary>
    ''' <param name="actionName">ログ作成中の操作</param>
    ''' <param name="exp">Catchした例外</param>
    ''' <returns>詳細ログの例外出力</returns>
    ''' <remarks>
    ''' 詳細ログ出力文字列作成中に発生した例外を出力する。
    ''' </remarks>
    Public Shared Function DetailException(ByVal actionName$, ByVal exp As Exception) As String
        Dim sb As New StringBuilder
        sb.AppendFormat("{0} is indeterminable. ({1})", actionName, exp.Message)
        Return sb.ToString()
    End Function
#End Region

End Class
