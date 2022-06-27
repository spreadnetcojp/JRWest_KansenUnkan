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
''' ソケット操作非恒久的要因例外
''' </summary>
''' <remarks>
''' ソケットに対する処理が失敗した際、要因が非恒久的（他のソケットによる
''' 競合リソースの使用等）である可能性もある場合に生成する例外。
''' ユーザがみている状況でのみ処理が行われる場合や、アプリケーションを
''' 終了させることでアラームをあげる思想である場合をのぞき、
''' この例外でアプリケーションを終了させるべきではない（ただし、ほとんどの
''' アプリケーションにおいて、何かしらの手段でユーザに状況を通知することは
''' 必須と思われる）。
''' </remarks>
Public Class SocketImpermanentException
    Inherits Exception
#Region " コンストラクタ "
    'メッセージプロパティのデフォルト値
    'NOTE: どこかからとってきたい。
    Private Const defaultMessage As String = "Socket operation failed by impermanent cause."

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
