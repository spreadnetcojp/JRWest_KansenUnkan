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

Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' 文言属性
''' </summary>
Public Enum SentenceAttr As Integer
    None
    Information
    Warning
    [Error]
    Question
End Enum

'NOTE: このクラスのメソッドは、BaseConfig.Init()実行前に呼び出されることを想定
'しなければならない。即ち、このクラスのメソッドやそこから呼び出されるメソッドは
'BaseConfigを参照してはならない。

''' <summary>
''' 置換可能な書式文言
''' </summary>
Public Structure Sentence
    '正しい書式指定項目のみにマッチする正規表現
    Private Shared ReadOnly oFormatItemRegx As New Regex("\{[0-9]+(\,[+-]{0,1}[0-9]){0,1}(:[^{}]+){0,1}\}", RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    '正しいまたは誤った書式指定項目にマッチする正規表現
    Private Shared ReadOnly oPseudoFormatItemRegx As New Regex("\{[^{}]*\}", RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Public Attr As SentenceAttr
    Friend RawValue As String
    Friend FormatItemCount As Integer

    '書式指定項目の件数（実際のところは、最大のindex + 1）を返却する。
    'NOTE: 渡された文字列が複合書式指定文字列として問題がある場合はArgumentExceptionをスローする。
    Public Shared Function CountFormatItems(ByVal s As String, ByVal isReplacement As Boolean) As Integer
        '文字列から"{{"と"}}"を除去した上で、書式指定項目を抜き出す。
        s = s.Replace("{{", "A").Replace("}}", "Z")
        Dim oMatches As MatchCollection = oFormatItemRegx.Matches(s)

        If oPseudoFormatItemRegx.Matches(s).Count <> oMatches.Count Then
            '書き間違いとみなせる書式指定項目が存在する場合
            Throw New ArgumentException("The string contains pseudo format item.")
        End If

        '書式指定項目の最大のindexを調べる。
        'また、同一indexの書式指定項目が存在しないかチェックしつつ、
        'isReadyの各要素に当該indexの書式指定項目が存在するか否かをセットする。
        Dim isReady(99) As Boolean
        Dim maxIndex As Integer = -1
        For Each oMatch As Match In oMatches
            Dim indexAsDouble As Double = Val(oMatch.Value.Substring(1))
            If indexAsDouble > 99 Then
                '書式指定項目のindexが大きすぎる場合
                Throw New ArgumentException("The string contains invalid format item [" & oMatch.Value & "]. Its index is too large.")
            End If
            Dim index As Integer = CInt(indexAsDouble)
            If isReady(index) Then
                '同一indexの書式指定項目が存在する場合
                Throw New ArgumentException("The string contains invalid format item [" & oMatch.Value & "]. Its index is duplicative.")
            End If
            isReady(index) = True
            If index > maxIndex Then maxIndex = index
        Next oMatch

        'オリジナルの文字列（ソースコードに記述したもの）の場合は、
        '書式指定項目のindexが歯抜け状態でないかチェックする。
        'NOTE: 設定次第で表示したくない引数もあるかもしれないため、
        '置き換え文字列の場合は、このチェックはやめておくことにしている。
        'NOTE: オリジナルの文字列（ソースコードに記述したもの）については、
        '引数に対応する書式指定項目を全て記述することが設計の前提である
        '（少なくとも最後の引数に対応する項目は記述しておかなければならない）
        'ため、このチェックが有害になることはない。
        If Not isReplacement Then
            For index As Integer = 0 To maxIndex
                If Not isReady(index) Then
                    Throw New ArgumentException("The string should contain a format item whose index is [" & index.ToString() & "].")
                End If
            Next index
        End If

        Return maxIndex + 1
    End Function

    Private Sub Init(ByVal sRawValue As String, ByVal attr As SentenceAttr, ByVal isReplacement As Boolean)
        Dim count As Integer = CountFormatItems(sRawValue, isReplacement)
        Me.FormatItemCount = count
        Me.RawValue = Utility.TranslateClangLiteralToDosText(sRawValue)
        Me.Attr = attr
    End Sub

    Public Sub New(ByVal sRawValue As String, ByVal attr As SentenceAttr, Optional ByVal isReplacement As Boolean = False)
        Init(sRawValue, attr, isReplacement)
    End Sub

    Public Sub New(ByVal sRawValue As String,Optional ByVal isReplacement As Boolean = False)
        Init(sRawValue, SentenceAttr.None, isReplacement)
    End Sub

    Public Sub New(ByVal sRawValue As String, ByVal sAttr As String,Optional ByVal isReplacement As Boolean = False)
        Init(sRawValue, DirectCast([Enum].Parse(GetType(SentenceAttr), sAttr), SentenceAttr), isReplacement)
    End Sub

    Public Function Gen(ByVal ParamArray args As Object()) As String
        Return String.Format(RawValue, args)
    End Function
End Structure

''' <summary>
''' 文言コンテナの基本クラス
''' </summary>
Public Class BaseLexis
    Public Shared NoneTitle As New Sentence("")
    Public Shared InformationTitle As New Sentence("通知")
    Public Shared WarningTitle As New Sentence("警告")
    Public Shared ErrorTitle As New Sentence("エラー")
    Public Shared QuestionTitle As New Sentence("確認")
    Public Shared UnforeseenErrorOccurred As New Sentence("予期せぬ異常が発生しました。", SentenceAttr.Error)

    Private Const sSection As String = "Lexis"
    Private Const sAttrSuffix As String = "_Attr"
    Private Const targetBindingFlags As BindingFlags = _
       BindingFlags.Static Or _
       BindingFlags.Public Or _
       BindingFlags.NonPublic Or _
       BindingFlags.FlattenHierarchy

    Private Declare Ansi Function GetPrivateProfileString Lib "KERNEL32.DLL" _
       Alias "GetPrivateProfileStringA" ( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpReturnedString As StringBuilder, _
        ByVal nSize As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String _
      ) As Integer

    Private Declare Ansi Function GetPrivateProfileStringToBytes Lib "KERNEL32.DLL" _
       Alias "GetPrivateProfileStringA" ( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String, _
        <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.U1)> ByVal lpReturnedString As Byte(), _
        ByVal nSize As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String _
      ) As Integer

    ''' <summary>INIファイルの内容を取り込む。</summary>
    ''' <remarks>
    ''' INIファイルの内容を取り込む。
    ''' </remarks>
    Public Shared Sub BaseInit(ByVal sIniFilePath As String, ByVal t As Type)
        'クラスtの静的な公開Sentence型フィールド全てについて、
        'メンバが正しく初期化されていることをチェックする。
        'NOTE: 以下を行うことの直接の意味は、これらのフィールドの
        'コンストラクタをこの場で強制的に実行させることにある。
        '下記を行わないと、INIファイルのLexisセクションにキーが１つも
        '存在しない場合に、このメソッド内でクラスtにアクセスする
        'ことが無くなってしまう。つまり、メッセージボックスを表示する
        'などでtのメンバにアクセスするときまで、tのフィールドの
        'コンストラクタが実行されない（文字列に不正があっても
        '起動時にわからない）ということになってしまう。
        Dim aFields As FieldInfo() = t.GetFields(targetBindingFlags)
        For Each oField As FieldInfo In aFields
            Dim val As Object = oField.GetValue(Nothing)
            If val.GetType() Is GetType(Sentence) Then
                Dim value As Sentence = DirectCast(val, Sentence)
                If value.RawValue Is Nothing Then
                    'NOTE: 実際にコンストラクタが途中で失敗している（Throwで抜けている）場合は、
                    'ここは実行されないはずである。
                    Throw New OPMGException(t.ToString() & "." & oField.Name & " refers to nothing.")
                End If
            End If
        Next oField

        'INIファイルの所定セクション内の全キーをヌル区切りでバイト列内に取得する。
        Dim aBytes(16384) As Byte
        Dim validLengthOfBytes As Integer = _
           GetPrivateProfileStringToBytes(sSection, Nothing, "[]_", aBytes, aBytes.Length, sIniFilePath)
        If validLengthOfBytes = 0 Then
            'INIファイルや所定セクションは存在し、キーが１つもない場合である。
            Return
        End If

        'バイト列をStringに変換後、各キーを要素とするString配列を作成する。
        Dim sNullSeparatedKeys As String = Encoding.Default.GetString(aBytes, 0, validLengthOfBytes - 1)
        If sNullSeparatedKeys.Equals("[]") Then
            'INIファイルまたは所定セクションが存在しない場合である。
            Throw New OPMGException("The [" & sSection & "] section not found.")
        End If
        Dim aKeys As String() = sNullSeparatedKeys.Split(Chr(0))

        For Each sKey As String In aKeys
            Dim sFieldName As String
            Dim isAttrKey As Boolean
            If sKey.EndsWith(sAttrSuffix) Then
                sFieldName = sKey.Substring(0, sKey.Length - sAttrSuffix.Length)
                isAttrKey = True
            Else
                sFieldName = sKey
                isAttrKey = False
            End If

            Dim oField As FieldInfo = t.GetField(sFieldName, targetBindingFlags Or BindingFlags.IgnoreCase)
            If oField Is Nothing Then
                '余計なキー（クラスtに該当フィールドの無いキー）が記述されている場合である。
                Throw New OPMGException("The [" & t.ToString() & "] does not have a field named [" & sFieldName & "].")
            End If

            Dim val As Object = oField.GetValue(Nothing)
            If val.GetType() IsNot GetType(Sentence) Then
                Throw New OPMGException("[" & t.ToString() & "." & sFieldName & "] is not a Sentence.")
            End If
            Dim value As Sentence = DirectCast(val, Sentence)
            If isAttrKey Then
                Try
                    Dim sb As StringBuilder = New StringBuilder(1024)
                    GetPrivateProfileString(sSection, sKey, "", sb, sb.Capacity, sIniFilePath)
                    value.Attr = DirectCast([Enum].Parse(GetType(SentenceAttr), sb.ToString()), SentenceAttr)
                    oField.SetValue(Nothing, value)
                Catch ex As Exception
                    Throw New OPMGException("Some error detected around [" & sKey & "].", ex)
                End Try
            Else
                Dim newValue As Sentence

                Try
                    'NOTE: キーに設定された実際の値がデフォルト用と同じ値である
                    'か、キーの一覧を取得してからキーに設定された値を取得する
                    'までの間にINIファイルからキーが削除されない限り、
                    'デフォルト用の値が取得されることはない。後者の場合は
                    'エラーとして扱いたいが、前者であっても後者と区別がつかない
                    'ため、どのみちTranslateClangLiteralToDosText()で例外が発生
                    'することになる不正なエスケープシーケンスをデフォルト値に
                    'している。そして、後者の場合のエラー検出も、この例外の
                    '検出に委ねることにしている。
                    Dim sb As StringBuilder = New StringBuilder(1024)
                    GetPrivateProfileString(sSection, sKey, "\a", sb, sb.Capacity, sIniFilePath)
                    newValue = New Sentence(sb.ToString(), True)
                Catch ex As Exception
                    Throw New OPMGException("Some error detected around [" & sKey & "].", ex)
                End Try

                If newValue.FormatItemCount > value.FormatItemCount Then
                    Throw New OPMGException("The value of [" & sKey & "] is disportionate to the original string.")
                End If

                newValue.Attr = value.Attr
                oField.SetValue(Nothing, newValue)
            End If
        Next sKey
    End Sub
End Class
