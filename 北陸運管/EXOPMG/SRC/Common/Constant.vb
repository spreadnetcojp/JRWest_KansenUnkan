' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  車改サーバでの改善をフィードバック
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Runtime.InteropServices
Imports System.Text

''' <summary>
''' 設定値入出力クラス
''' </summary>
Public Class Constant

#Region "プライベートフィールド"
    Private Const BUFFER_LEN As Integer = 256
#End Region

#Region "API宣言"
    <DllImport("KERNEL32.DLL", CharSet:=CharSet.Auto)> _
    Private Shared Function GetPrivateProfileString( _
       ByVal lpAppName As String, _
       ByVal lpKeyName As String, _
       ByVal lpDefault As String, _
       ByVal lpReturnedString As System.Text.StringBuilder, _
       ByVal nSize As Integer, _
       ByVal lpFileName As String) As Integer
    End Function
    <DllImport("KERNEL32.DLL")> _
    Private Shared Function WritePrivateProfileString( _
       ByVal lpAppName As String, _
       ByVal lpKeyName As String, _
       ByVal lpString As String, _
       ByVal lpFileName As String) As Integer
    End Function
#End Region

#Region "環境変数取得"
    ''' <summary>
    ''' [環境変数取得]
    ''' </summary>
    ''' <remarks>
    ''' 環境変数が定義されていない場合はNothingを返却する。
    ''' 定義値が0バイトの文字列の場合、0文字のStringを返却する。
    ''' 引数の不正等があった場合は例外（OPMGException以外）を生成する。
    ''' </remarks>
    ''' <param name="sName">環境変数名</param>
    ''' <returns>取得値</returns>
    Public Shared Function GetEnv(ByVal sName As String) As String
        'NOTE: OSの対応有無や引数に依存して発生する例外はそのままThrowする。
        Dim sRtn As String
        sRtn = System.Environment.GetEnvironmentVariable(sName, EnvironmentVariableTarget.Machine)
        If sRtn = Nothing Then
            sRtn = System.Environment.GetEnvironmentVariable(sName, EnvironmentVariableTarget.Process)
        End If
        If sRtn = Nothing Then
            sRtn = System.Environment.GetEnvironmentVariable(sName, EnvironmentVariableTarget.Machine)
        End If
        Return sRtn
    End Function
#End Region

#Region "設定情報取得"
    ''' <summary>
    ''' [設定情報取得]
    ''' 指定INIファイルから設定情報を読み出す。
    ''' </summary>
    ''' <remarks>
    ''' 指定INIファイル、指定セクション、指定キーのいずれかが存在しない場合はNothingを返却する。
    ''' 設定値が長すぎる場合はNothingを返却する。
    ''' 設定値が0バイトの文字列の場合、0文字のStringを返却する。
    ''' 引数の不正等があった場合は例外（OPMGException以外）を生成する。
    ''' </remarks>
    ''' <param name="SectionName">セクション名</param>
    ''' <param name="KeyName">キー名</param>
    ''' <param name="FileFullName">INIファイル絶対パス名</param>
    ''' <returns>取得値</returns>
    Public Shared Function GetIni(ByVal SectionName As String, ByVal KeyName As String, ByVal FileFullName As String) As String
        Dim sb As StringBuilder = New StringBuilder(BUFFER_LEN)
        'NOTE: API内からSEH例外が送出された際は、CLRが何らかのExceptionを生成する想定。
        'そのケースは設定ファイルに依存しないプログラムのバグであるため、
        'このメソッドの呼び元にそのままスローする。
        GetPrivateProfileString(SectionName, KeyName, vbLf, sb, BUFFER_LEN, FileFullName)

        Dim s As String = sb.ToString()
        '指定のファイルまたは指定の設定項目が存在しない場合はNothingを返却。
        If s.Equals(vbLf) Then Return Nothing
        'APIに指定したバッファに入りきらなかった可能性がある場合はNothingを返却。
        If s.Length >= BUFFER_LEN Then Return Nothing
        Return s
    End Function
#End Region

#Region "設定情報書込"
    ''' <summary>
    ''' [設定情報書込]
    ''' 指定INIファイルに設定情報を書き込む。
    ''' </summary>
    ''' <param name="SectionName">セクション名</param>
    ''' <param name="KeyName">キー名</param>
    ''' <param name="FileFullName">INIファイル絶対パス名</param>
    ''' <param name="Value">設定値</param>
    ''' <returns>True:成功,False:失敗</returns>
    Public Shared Function SetIni(ByVal SectionName As String, ByVal KeyName As String, ByVal FileFullName As String, ByVal Value As String) As Boolean
        Try
            Dim sDir As String = System.IO.Path.GetDirectoryName(FileFullName)
            If Not System.IO.Directory.Exists(sDir) Then    'フォルダがない場合作成する
                System.IO.Directory.CreateDirectory(sDir)
            End If
            If WritePrivateProfileString(SectionName, KeyName, Value, FileFullName) = 0 Then
                Throw New System.ArgumentException("WritePrivateProfileString(" & SectionName & ", " & KeyName & ", " & Value & ", " & FileFullName & ") failed.")
            End If
            Return True
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            Return False
        End Try
    End Function
#End Region

End Class
