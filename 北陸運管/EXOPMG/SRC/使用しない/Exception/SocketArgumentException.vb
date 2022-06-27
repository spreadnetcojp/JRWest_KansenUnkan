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

''' <summary>
''' ソケット操作パラメータ要因例外
''' </summary>
''' <remarks>
''' ソケットに対する処理が失敗した際、入力パラメータの範囲や書式が要因
''' である場合に生成する例外。
''' 固定の値ではなく、設定ファイル等から取得したパラメータを渡している
''' 場合にのみ、予期すべき例外であると言える。
''' ただし、そのような事情があっても、ユーザがみている状況でのみ処理が
''' 行われる場合や、アプリケーションを終了させることでアラームをあげる
''' 思想である（それ以外の方法でユーザに通知することができない）場合は、
''' この例外でもアプリケーションを終了させるべきかもしれない。
''' </remarks>
Public Class SocketArgumentException
    Inherits Exception
#Region " コンストラクタ "
    'メッセージプロパティのデフォルト値
    'NOTE: どこかからとってきたい。
    Private Const defaultMessage As String = "Socket operation failed by argument value."

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
End Class
