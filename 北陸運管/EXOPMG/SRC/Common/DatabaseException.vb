Imports JR.ExOpmg.Common

Public Class DatabaseException
    Inherits Exception

    'メッセージプロパティのデフォルト値
    'NOTE: どこかからとってきたい。
    Private Const defaultMessage As String = "Some method fails in database access."

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

End Class
