' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応にて、Debug、Info、Warnに
'                                   例外情報を受け取るオーバーロードを追加
' **********************************************************************
Option Strict On
Option Explicit On

Imports System.Reflection
Imports System.Text
Imports System.Threading

''' <summary>
''' ログ拡張出力デリゲート
''' </summary>
Public Delegate Sub LogToOptionalDelegate( _
   ByVal number As Long, _
   ByVal sSecondName As String, _
   ByVal sDateTime As String, _
   ByVal sKind As String, _
   ByVal sClassName As String, _
   ByVal sMethodName As String, _
   ByVal sText As String)

'NOTE: このクラスのメソッドは、BaseConfig.Init()実行前に呼び出されることを想定
'しなければならない。即ち、このクラスのメソッドやそこから呼び出されるメソッドは
'BaseConfigを参照してはならない。

'NOTE: このクラスのメソッドは、BaseLexis.Init()実行前に呼び出されることを想定
'しなければならない。即ち、このクラスのメソッドやそこから呼び出されるメソッドは
'BaseLexisを参照するべきではない。
'そもそも、このクラスが内部に記述しているような「全ログに共通の文字列」に
'関して、外部設定で置き換え可能にするなどの要望はないと思われるが、
'Lexisを使ってそれを実現するのであれば、Lexis.Init()の実行完了前に出力される
'ログ（たとえばLexis.Init()自身が当該文言の置き換え前に出力するログ）は、
'置き換わらないということを考慮するべきである。
'同一ライブラリ内とはいえ、どうみても単方向の依存関係にあるべきクラス同士が
'相互依存するのはよくないわけであり、このクラス自身にSetReplacementFormat()
'のようなメソッドを用意する（利用側はLog.Init()に続き、それを呼び出す）方が
'間違いなく無難である。

''' <summary>
''' ロガー
''' </summary>
Public Class Log
#Region "内部クラス等"
    ''' <summary>
    ''' ログ種別
    ''' </summary>
    Private Enum LogKind As Integer
        Debug = 0
        Info
        Warn
        [Error]
        Fatal
        Extra
    End Enum
#End Region

#Region "定数"
    Private Shared ReadOnly aTextForLogKind As String() = {"[DEBUG]", "[INFO]", "[WARN]", "[ERROR]", "[FATAL]"}
    Private Const sTimestampFormat As String = "yyyy/MM/dd HH:mm:ss.fff"
    Private Const sSep As String = ","
    Private Const sQuot As String = Chr(&H22)
    Private Const sFileExtension As String = ".csv"
    Private Const maxBranchNumber As Integer = 99
#End Region

#Region "変数"
    Private Shared sDirName As String = Nothing
    Private Shared sFirstName As String = Nothing
    Private Shared seqNumber As Integer = -1
    Private Shared kindsMask As Integer = &HFF
    Private Shared oLogToOptionalDelegate As LogToOptionalDelegate = Nothing
#End Region


#Region "プロパティ"
    Public Shared ReadOnly Property LoggingDebug() As Boolean
        Get
            Return (kindsMask And 1) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingInfo() As Boolean
        Get
            Return (kindsMask And 2) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingWarn() As Boolean
        Get
            Return (kindsMask And 4) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingError() As Boolean
        Get
            Return (kindsMask And 8) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingFatal() As Boolean
        Get
            Return (kindsMask And 16) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingExtra() As Boolean
        Get
            Return (kindsMask And 32) <> 0
        End Get
    End Property
#End Region

#Region "初期化メソッド"
    'NOTE: 別のスレッドが記録メソッドを呼び出さないことが保証できる時点で、実行してください。
    Public Shared Sub Init(ByVal sBasePath As String, ByVal sArgFirstName As String)
        sDirName = sBasePath
        sFirstName = sArgFirstName
        seqNumber = -1
    End Sub

    'NOTE: 別のスレッドが記録メソッドを呼び出さないことが保証できる時点で、実行してください。
    Public Shared Sub SetKindsMask(ByVal value As Integer)
        kindsMask = value
    End Sub

    'NOTE: 別のスレッドが記録メソッドを呼び出さないことが保証できる時点で、実行してください。
    Public Shared Sub SetOptionalWriter(ByVal oArgLogToOptionalDelegate As LogToOptionalDelegate)
        oLogToOptionalDelegate = oArgLogToOptionalDelegate
    End Sub
#End Region

#Region "記録メソッド"
    ''' <summary>
    ''' デバッグ用情報を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Debug(ByVal sText As String)
        Try
            Put(LogKind.Debug, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' デバッグ用情報をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Debug(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Debug, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' デバッグ用情報を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Debug(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Debug, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 一般情報を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Info(ByVal sText As String)
        Try
            Put(LogKind.Info, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 一般情報をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Info(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Info, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 一般情報を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Info(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Info, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 警告を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Warn(ByVal sText As String)
        Try
            Put(LogKind.Warn, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 警告をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Warn(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Warn, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 警告を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Warn(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Warn, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 異常を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub [Error](ByVal sText As String)
        Try
            Put(LogKind.Error, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 異常をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub [Error](ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Error, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 異常を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub [Error](ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Error, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 深刻な異常を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Fatal(ByVal sText As String)
        Try
            Put(LogKind.Fatal, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 深刻な異常をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Fatal(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Fatal, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 深刻な異常を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Fatal(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Fatal, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名でデバッグ用情報を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Debug(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Debug, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名でデバッグ用情報をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Debug(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Debug, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名でデバッグ用情報を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Debug(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Debug, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で一般情報を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Info(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Info, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で一般情報をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Info(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Info, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で一般情報を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Info(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Info, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で警告を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Warn(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Warn, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で警告をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Warn(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Warn, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で警告を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Warn(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Warn, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で異常を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub [Error](ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Error, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で異常をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub [Error](ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Error, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で異常を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub [Error](ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Error, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で深刻な異常を記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Fatal(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Fatal, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で深刻な異常をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Fatal(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Fatal, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 任意スレッド名で深刻な異常を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Fatal(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Fatal, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 追加情報を記録する。
    ''' </summary>
    ''' <param name="sExtraName">追加情報名</param>
    ''' <param name="sText">記録文言</param>
    Public Shared Sub Extra(ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String)
        Try
            Put(LogKind.Extra, sExtraName, oCaller, sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 追加情報をバイナリデータとともに記録する。
    ''' </summary>
    ''' <param name="sExtraName">追加情報名</param>
    ''' <param name="sText">記録文言</param>
    ''' <param name="aBytes">バイナリデータ格納配列</param>
    ''' <param name="pos">バイナリデータの位置</param>
    ''' <param name="len">バイナリデータの長さ</param>
    Public Shared Sub Extra(ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Extra, sExtraName, oCaller, sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub

    ''' <summary>
    ''' 追加情報を例外の情報とともに記録する。
    ''' </summary>
    ''' <param name="sExtraName">追加情報名</param>
    ''' <param name="sText">記録文言</param>
    ''' <param name="exception">記録例外</param>
    Public Shared Sub Extra(ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Extra, sExtraName, oCaller, sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw しない
        End Try
    End Sub
#End Region

#Region "プライベートメソッド"
    Private Shared Function GetSeqNumber() As Long
        'NOTE: 現状ではGlobalVariables.LockObjectについてのSyncLockの中でのみ
        '実行することになっているので、Interlockedクラスを使う必要はない。
        Dim num As Integer = Interlocked.Increment(seqNumber)
        If num >= 0 Then
            Return num
        Else
            Return CLng(num + 1) + UInt32.MaxValue
        End If
    End Function

    Private Shared Sub Put(ByVal kind As LogKind, ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String)
        If (kindsMask And 1 << kind) = 0 Then Return

        Dim sClassName As String = oCaller.DeclaringType.ToString()
        Dim sMethodName As String = oCaller.Name

        'OPT: sCurThreadNameについては、LogKind.Extraの場合もスレッド名とし、
        '各種別のログをどのようなセカンド名のファイルに書き込むかは、Init()
        'か何かで設定する方がよい。
        Dim oCurThread As Thread = Thread.CurrentThread
        Dim sCurThreadName As String = If(sExtraName IsNot Nothing, sExtraName, oCurThread.Name)
        Dim sKind As String = If(kind = LogKind.Extra, "[" & sExtraName & "]", aTextForLogKind(kind))

        'NOTE: oLogToOptionalDelegate()に引数で渡した参照は、
        'oLogToOptionalDelegate()内部で別のスレッドに
        '渡してかまわない。たとえ複数のスレッドに渡したとしても
        'これらは全て変更され得ないオブジェクト（String）を
        '指しているため、特に配慮は不要である。

        'NOTE: 以下のSyncLockにより、同一プロセス内の別スレッドが出力する２つの
        'レコードについて、タイムスタンプの前後関係とseqNumberの前後関係は必ず
        '同じになる。複数のスレッドが書き込む同一ファイル内だけでなく、
        'プロセス内の全ログファイルをマージ（連結後、seqNumberでソート）した
        '場合の各レコードについても、これらの前後関係は同じになる。

        'OPT: 現状、以下のSyncLockは、参照型変数GlobalVariables.SysUserIdに
        'ついて、一部分が書き変わった状態で読み取りを行わないことや、
        'seqNumberの前後関係と実際の出力行の前後関係を一致させることに加え、
        'このプロセス内の別スレッドによる「枝番前までが同一名のファイルへの
        'レコード追加」を無期限で待つことも兼ねて記述している。
        'しかし、以下の理由により、タイムスタンプの前後関係とseqNumberの
        '前後関係を同じにすることだけを目的とする方がよいかもれない。
        '(1) 出力先リソースに関する排他制御は、WriteToFile()内かその呼び出し
        '  の範囲で別途行うべきである。なお、単なる期限付きのファイルロックで
        '  実現すると、他のプロセスがロックし続けている場合に（期限一杯まで
        '  待つことになり）パフォーマンスが落ちるだろうし、このプロセス内の
        '  別スレッドがログ出力のための処理でロックしている場合も、その処理が
        '  期限内に終わってくれなければ同様のことになり、無駄に枝番もできて
        '  しまうはずである。しかし、ファイルロックとは別の仕組みで、枝番なし
        '  のファイル単位で無期限待機を行うようにすれば済む話である。
        '(2) １つのファイルがロックされているなどによりWriteToFile()がブロック
        '  されただけで、そのファイルと関係のないスレッドまでブロックされかね
        '  ないのは微妙すぎる。
        '(3) seqNumberの前後関係と出力行の前後関係など、後でseqNumberをキーに
        '  ソートすれば一致するわけであるし、かなりどうでもよい。
        '以上の理由から、やはりWriteToFile()の処理の大部分（GlobalVariables.
        'SysUserIdの読み取り以外）はSyncLockの外に出した方がよい。

        'OPT: プールスレッドに名前が無いことが保証されているなら、後半の条件は不要。
        'そもそも、フレームワークがワーカースレッドに実行させるような処理の一部を
        'アプリで実装する（そこでログ出力を行う）ことは無い前提でよいかと...。
        If (sCurThreadName IsNot Nothing) AndAlso (Not oCurThread.IsThreadPoolThread) Then
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sText)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sText)
                End If
            End SyncLock
        Else
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sText)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sText)
                End If
            End SyncLock
        End If
    End Sub

    Private Shared Sub Put(ByVal kind As LogKind, ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        If (kindsMask And 1 << kind) = 0 Then Return

        Dim sClassName As String = oCaller.DeclaringType.ToString()
        Dim sMethodName As String = oCaller.Name

        'OPT: sCurThreadNameについては、LogKind.Extraの場合もスレッド名とし、
        '各種別のログをどのようなセカンド名のファイルに書き込むかは、Init()
        'か何かで設定する方がよい。
        Dim oCurThread As Thread = Thread.CurrentThread
        Dim sCurThreadName As String = If(sExtraName IsNot Nothing, sExtraName, oCurThread.Name)
        Dim sKind As String = If(kind = LogKind.Extra, "[" & sExtraName & "]", aTextForLogKind(kind))

        'NOTE: oLogToOptionalDelegate()に引数で渡した参照は、
        'oLogToOptionalDelegate()内部で別のスレッドに
        '渡してかまわない。たとえ複数のスレッドに渡したとしても
        'これらは全て変更され得ないオブジェクト（String）を
        '指しているため、特に配慮は不要である。

        'NOTE: 以下のSyncLockにより、同一プロセス内の別スレッドが出力する２つの
        'レコードについて、タイムスタンプの前後関係とseqNumberの前後関係は必ず
        '同じになる。複数のスレッドが書き込む同一ファイル内だけでなく、
        'プロセス内の全ログファイルをマージ（連結後、seqNumberでソート）した
        '場合の各レコードについても、これらの前後関係は同じになる。

        'OPT: 現状、以下のSyncLockは、参照型変数GlobalVariables.SysUserIdに
        'ついて、一部分が書き変わった状態で読み取りを行わないことや、
        'seqNumberの前後関係と実際の出力行の前後関係を一致させることに加え、
        'このプロセス内の別スレッドによる「枝番前までが同一名のファイルへの
        'レコード追加」を無期限で待つことも兼ねて記述している。
        'しかし、以下の理由により、タイムスタンプの前後関係とseqNumberの
        '前後関係を同じにすることだけを目的とする方がよいかもれない。
        '(1) 出力先リソースに関する排他制御は、WriteToFile()内かその呼び出し
        '  の範囲で別途行うべきである。なお、単なる期限付きのファイルロックで
        '  実現すると、他のプロセスがロックし続けている場合に（期限一杯まで
        '  待つことになり）パフォーマンスが落ちるだろうし、このプロセス内の
        '  別スレッドがログ出力のための処理でロックしている場合も、その処理が
        '  期限内に終わってくれなければ同様のことになり、無駄に枝番もできて
        '  しまうはずである。しかし、ファイルロックとは別の仕組みで、枝番なし
        '  のファイル単位で無期限待機を行うようにすれば済む話である。
        '(2) １つのファイルがロックされているなどによりWriteToFile()がブロック
        '  されただけで、そのファイルと関係のないスレッドまでブロックされかね
        '  ないのは微妙すぎる。
        '(3) seqNumberの前後関係と出力行の前後関係など、後でseqNumberをキーに
        '  ソートすれば一致するわけであるし、かなりどうでもよい。
        '以上の理由から、やはりWriteToFile()の処理の大部分（GlobalVariables.
        'SysUserIdの読み取り以外）はSyncLockの外に出した方がよい。

        Dim s As String = sText & vbCrLf & BitConverter.ToString(aBytes, pos, len)

        'OPT: プールスレッドに名前が無いことが保証されているなら、後半の条件は不要。
        'そもそも、フレームワークがワーカースレッドに実行させるような処理の一部を
        'アプリで実装する（そこでログ出力を行う）ことは無い前提でよいのでは。
        If (sCurThreadName IsNot Nothing) AndAlso (Not oCurThread.IsThreadPoolThread) Then
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, s)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, s)
                End If
            End SyncLock
        Else
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, "Someone", sCurTime, sKind, sClassName, sMethodName, s)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, "Someone", sCurTime, sKind, sClassName, sMethodName, s)
                End If
            End SyncLock
        End If
    End Sub

    Private Shared Sub Put(ByVal kind As LogKind, ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal ex As Exception)
        If (kindsMask And 1 << kind) = 0 Then Return

        Dim sClassName As String = oCaller.DeclaringType.ToString()
        Dim sMethodName As String = oCaller.Name

        'OPT: sCurThreadNameについては、LogKind.Extraの場合もスレッド名とし、
        '各種別のログをどのようなセカンド名のファイルに書き込むかは、Init()
        'か何かで設定する方がよい。
        Dim oCurThread As Thread = Thread.CurrentThread
        Dim sCurThreadName As String = If(sExtraName IsNot Nothing, sExtraName, oCurThread.Name)
        Dim sKind As String = If(kind = LogKind.Extra, "[" & sExtraName & "]", aTextForLogKind(kind))

        'NOTE: oLogToOptionalDelegate()に引数で渡した参照は、
        'oLogToOptionalDelegate()内部で別のスレッドに
        '渡してかまわない。たとえ複数のスレッドに渡したとしても
        'これらは全て変更され得ないオブジェクト（String）を
        '指しているため、特に配慮は不要である。

        'NOTE: 以下のSyncLockにより、同一プロセス内の別スレッドが出力する２つの
        'レコードについて、タイムスタンプの前後関係とseqNumberの前後関係は必ず
        '同じになる。複数のスレッドが書き込む同一ファイル内だけでなく、
        'プロセス内の全ログファイルをマージ（連結後、seqNumberでソート）した
        '場合の各レコードについても、これらの前後関係は同じになる。

        'OPT: 現状、以下のSyncLockは、参照型変数GlobalVariables.SysUserIdに
        'ついて、一部分が書き変わった状態で読み取りを行わないことや、
        'seqNumberの前後関係と実際の出力行の前後関係を一致させることに加え、
        'このプロセス内の別スレッドによる「枝番前までが同一名のファイルへの
        'レコード追加」を無期限で待つことも兼ねて記述している。
        'しかし、以下の理由により、タイムスタンプの前後関係とseqNumberの
        '前後関係を同じにすることだけを目的とする方がよいかもれない。
        '(1) 出力先リソースに関する排他制御は、WriteToFile()内かその呼び出し
        '  の範囲で別途行うべきである。なお、単なる期限付きのファイルロックで
        '  実現すると、他のプロセスがロックし続けている場合に（期限一杯まで
        '  待つことになり）パフォーマンスが落ちるだろうし、このプロセス内の
        '  別スレッドがログ出力のための処理でロックしている場合も、その処理が
        '  期限内に終わってくれなければ同様のことになり、無駄に枝番もできて
        '  しまうはずである。しかし、ファイルロックとは別の仕組みで、枝番なし
        '  のファイル単位で無期限待機を行うようにすれば済む話である。
        '(2) １つのファイルがロックされているなどによりWriteToFile()がブロック
        '  されただけで、そのファイルと関係のないスレッドまでブロックされかね
        '  ないのは微妙すぎる。
        '(3) seqNumberの前後関係と出力行の前後関係など、後でseqNumberをキーに
        '  ソートすれば一致するわけであるし、かなりどうでもよい。
        '以上の理由から、やはりWriteToFile()の処理の大部分（GlobalVariables.
        'SysUserIdの読み取り以外）はSyncLockの外に出した方がよい。

        Dim sb As New StringBuilder(sText & vbCrLf)
        While ex IsNot Nothing
            sb.AppendLine(ex.GetType().ToString())
            sb.AppendLine(ex.Message)
            sb.AppendLine(ex.StackTrace)
            ex = ex.InnerException
            If ex IsNot Nothing Then
                sb.AppendLine("The exception has an inner exception...")
            End If
        End While

        'OPT: プールスレッドに名前が無いことが保証されているなら、後半の条件は不要。
        'そもそも、フレームワークがワーカースレッドに実行させるような処理の一部を
        'アプリで実装する（そこでログ出力を行う）ことは無い前提でよいのでは。
        If (sCurThreadName IsNot Nothing) AndAlso (Not oCurThread.IsThreadPoolThread) Then
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                End If
            End SyncLock
        Else
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                End If
            End SyncLock
        End If
    End Sub

    Private Shared Sub WriteToFile( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)

        Dim sPath As String = sDirName
        'sPath = System.IO.Path.Combine(sDirName, sKind)  'ログ種別のフォルダを追加

        'フォルダ作成(ないときだけ)
        If Not System.IO.Directory.Exists(sPath) Then
            'NOTE: 従来機から継承した仕様であるが、以下は微妙である。
            'イベントログへの書き込みができないクライアントへの対策として、
            'Utility.WriteLogToEvent()に「イベントログに書き込む代わりに
            '起動時からオープンしてあるファイルに書き込む」等のフォールバックが
            'あるとよい。起動時のファイルオープンに失敗する可能性はあるが、
            'その場合は、クライアントにおける起動時の１回だけということで、
            'メッセージボックスを表示しても煩わしさはないはず。
            If Utility.MakeFolder(sPath) = False Then Return
        End If

        sPath = System.IO.Path.Combine(sPath, Format(Now, "yyyyMMdd") & "-" & sFirstName & "-" & sSecondName)

        Dim sUser As String = "[" & GlobalVariables.SysUserId & "]"
        sText = sText.Replace(sQuot, sQuot & sQuot)
        Dim sMessage As String = _
           number.ToString("D10") & sSep & sDateTime & sSep & sKind & sSep _
           & sUser & sSep & sClassName & sSep & sMethodName & sSep _
           & sQuot & sText & sQuot

        Dim sTryPath As String = sPath & sFileExtension
        For i As Integer = 0 To maxBranchNumber
            Dim swFile As System.IO.StreamWriter = Nothing
            Try
                If Not i = 0 Then
                    sTryPath = sPath & "-" & i.ToString() & sFileExtension
                End If
                swFile = New System.IO.StreamWriter(sTryPath, True, Encoding.Default)
                swFile.WriteLine(sMessage)
                swFile.Flush()
                Return
            Catch ex As System.IO.DirectoryNotFoundException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.DriveNotFoundException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.FileNotFoundException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.InternalBufferOverflowException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.InvalidDataException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.PathTooLongException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.IOException
                If i = maxBranchNumber Then
                    Utility.WriteLogToEvent(EventLogEntryType.FailureAudit, ex.Message, Utility.ClsName(), Utility.MethodName())
                    Return
                End If
            Catch ex As Exception
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Finally
                If swFile IsNot Nothing Then
                    swFile.Close()
                End If
            End Try
        Next i
    End Sub
#End Region
End Class
