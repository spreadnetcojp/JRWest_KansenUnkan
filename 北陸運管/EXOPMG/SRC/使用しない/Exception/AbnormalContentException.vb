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
''' 異常コンテンツ例外
''' </summary>
''' <remarks>
''' ファイルストリームやソケットやバイト配列からデータを取得するメソッドに
''' おいて、データの異常で処理が継続できない場合に生成する例外。
''' 格納されているものが変わらない限り、何度取得しようとしても同じ結果に
''' なることが推測される。少なくともファイルストリームやソケットを
''' 一度閉じなければならないことは間違いない。
''' </remarks>
Public Class AbnormalContentException
    Inherits Exception
#Region " コンストラクタ "
    'メッセージプロパティのデフォルト値
    'NOTE: どこかからとってきたい。
    Private Const defaultMessage As String = "Content error detected."

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
