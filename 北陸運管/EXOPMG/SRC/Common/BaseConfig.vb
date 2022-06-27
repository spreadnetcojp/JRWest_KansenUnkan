' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2014/04/01  (NES)河脇  北陸対応：INIファイルの可変キー項目対応
'   0.2      2017/04/10  (NES)小林  次世代車補対応にて、GetFileSectionKeys
'                                   GetFileSectionAsDictionary、
'                                   GetFileSectionAsDataTableを追加
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Data
Imports System.Runtime.InteropServices
Imports System.Text

''' <summary>
''' 定数コンテナの基本クラス
''' </summary>
Public Class BaseConfig
    'データベース接続用情報
    Public Shared DatabaseServerName As String
    Public Shared DatabaseName As String
    Public Shared DatabaseUserName As String
    Public Shared DatabasePassword As String

    'データベース関連タイマ値
    Public Shared DatabaseReadLimitSeconds As Integer
    Public Shared DatabaseWriteLimitSeconds As Integer

    'INIファイル内のセクション名
    Protected Const DATABASE_SECTION As String = "Database"

    'INIファイル内における各設定項目のキー
    Private Const DATABASE_SERVER_NAME_KEY As String = "ServerName"
    Private Const DATABASE_NAME_KEY As String = "Name"
    Private Const DATABASE_USER_NAME_KEY As String = "UserName"
    Private Const DATABASE_PASSWORD_KEY As String = "Password"
    Private Const DATABASE_READ_LIMIT_KEY As String = "ReadLimitSeconds"
    Private Const DATABASE_WRITE_LIMIT_KEY As String = "WriteLimitSeconds"

    Protected Shared IniFileParh As String
    Protected Shared LastReadSection As String = ""
    Protected Shared LastReadKey As String = ""
    Protected Shared LastReadValue As String = ""

    Private Declare Ansi Function GetPrivateProfileStringToBytes Lib "KERNEL32.DLL" _
       Alias "GetPrivateProfileStringA" ( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String, _
        <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.U1)> ByVal lpReturnedString As Byte(), _
        ByVal nSize As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String _
      ) As Integer

    ''' <summary>
    ''' INIファイルから指定項目の設定値を読み取る。
    ''' </summary>
    ''' <param name="sectionName">セクションの名称</param>
    ''' <param name="keyName">キー</param>
    ''' <param name="ValueCheck">Value値チェック有無 True：有、False：無</param>
    ''' <remarks>Constant.GetIniを使用する。</remarks>
    Protected Shared Sub ReadFileElem(ByVal sectionName As String, ByVal keyName As String, Optional ByVal ValueCheck As Boolean = True)
        ' --- Ver0.1 北陸対応：INIファイルの可変キー項目対応 MOD
        'Protected Shared Sub ReadFileElem(ByVal sectionName As String, ByVal keyName As String)
        If (String.IsNullOrEmpty(sectionName) OrElse String.IsNullOrEmpty(keyName)) Then
            Throw New OPMGException("Invalid parameter.")
        End If

        LastReadSection = sectionName
        LastReadKey = keyName

        LastReadValue = Constant.GetIni(sectionName, keyName, IniFileParh)
        ' --- Ver0.1 北陸対応：INIファイルの可変キー項目対応 MOD
        'If LastReadValue Is Nothing Then
        If (LastReadValue Is Nothing) And ValueCheck Then
            Throw New OPMGException("It's not defined or has too long value. (Section: " & sectionName & ", Key: " & keyName & ")")
        End If
    End Sub

    ''' <summary>
    ''' INIファイルから指定セクションに含まれる全てのキーを読み取り、String配列として返却する。
    ''' </summary>
    ''' <param name="sectionName">セクションの名称</param>
    Protected Shared Function GetFileSectionKeys(ByVal sectionName As String) As String()
        Try
            'セクション内の全キーをヌル区切りでバイト列内に取得する。
            Dim bytes(65535) As Byte
            Dim validLengthOfBytes As Integer = _
               GetPrivateProfileStringToBytes(sectionName, Nothing, "[]_", bytes, bytes.Length, IniFileParh)
            If validLengthOfBytes = 0 Then
                'INIファイルや所定セクションは存在し、キーが１つもない場合である。
                Return New String(-1) {}
            End If
            If validLengthOfBytes = CUInt(bytes.Length - 2) Then
                '全キーがバッファに入りきらなかった可能性がある場合である。
                Throw New OPMGException("The [" & sectionName & "] section might contain too many keys.")
            End If

            'バイト列をStringに変換する。
            Dim sNullSeparatedKeys As String = Encoding.Default.GetString(bytes, 0, validLengthOfBytes - 1)
            If sNullSeparatedKeys.Equals("[]") Then
                'INIファイルまたは所定セクションが存在しない場合である。
                Throw New OPMGException("The [" & sectionName & "] section not found.")
            End If

            '各キーを要素とするString配列を作成し、返却する。
            Return sNullSeparatedKeys.Split(Chr(0))

        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            'NOTE: GetPrivateProfileStringToBytes()やEncoding.Default.GetString()で例外がスローされた場合を想定。
            Throw New OPMGException("Something may be wrong. (Section: " & sectionName & ")", ex)
        End Try
    End Function

    ''' <summary>
    ''' INIファイルから指定セクションに含まれる全てのキーと設定値を読み取り、Dictionaryとして返却する。
    ''' </summary>
    ''' <param name="sectionName">セクションの名称</param>
    ''' <remarks>Constant.GetIniを使用する。</remarks>
    Protected Shared Function GetFileSectionAsDictionary(ByVal sectionName As String) As Dictionary(Of String, String)
        Dim keyNames As String() = GetFileSectionKeys(sectionName)
        Dim dic As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Try
            For Each keyName As String In keyNames
                ReadFileElem(sectionName, keyName)
                dic.Add(keyName, LastReadValue)
            Next keyName
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("Something may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try

        Return dic
    End Function

    ''' <summary>
    ''' INIファイルから指定セクションに含まれる全てのキーと設定値を読み取り、DataTableとして返却する。
    ''' </summary>
    ''' <param name="sectionName">セクションの名称</param>
    ''' <remarks>Constant.GetIniを使用する。</remarks>
    Protected Shared Function GetFileSectionAsDataTable(ByVal sectionName As String, Optional ByVal addEmptyRow As Boolean = False) As DataTable
        Dim keyNames As String() = GetFileSectionKeys(sectionName)
        Dim dt As New DataTable()
        dt.Columns.Add("Key", GetType(String))
        dt.Columns.Add("Value", GetType(String))
        If addEmptyRow Then
            Dim row As DataRow = dt.NewRow()
            row("Key") = ""
            row("Value") = ""
            dt.Rows.Add(row)
        End If

        Try
            For Each keyName As String In keyNames
                ReadFileElem(sectionName, keyName)
                Dim row As DataRow = dt.NewRow()
                row("Key") = keyName
                row("Value") = LastReadValue
                dt.Rows.Add(row)
            Next keyName
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("Something may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try

        Return dt
    End Function

    ''' <summary>INIファイルからCommonライブラリに必須の設定値を取り込む。</summary>
    Protected Shared Sub BaseInit(ByVal sIniFilePath As String)
        IniFileParh = sIniFilePath
        Try
            ReadFileElem(DATABASE_SECTION, DATABASE_SERVER_NAME_KEY)
            DatabaseServerName = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_NAME_KEY)
            DatabaseName = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_USER_NAME_KEY)
            DatabaseUserName = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_PASSWORD_KEY)
            DatabasePassword = LastReadValue

            ReadFileElem(DATABASE_SECTION, DATABASE_READ_LIMIT_KEY)
            DatabaseReadLimitSeconds = Integer.Parse(LastReadValue)

            ReadFileElem(DATABASE_SECTION, DATABASE_WRITE_LIMIT_KEY)
            DatabaseWriteLimitSeconds = Integer.Parse(LastReadValue)
        Catch ex As OPMGException
            Throw
        Catch ex As Exception
            Throw New OPMGException("The value may be wrong. (Section: " & LastReadSection & ", Key: " & LastReadKey & ", Value: " & LastReadValue & ")", ex)
        End Try
    End Sub

End Class
