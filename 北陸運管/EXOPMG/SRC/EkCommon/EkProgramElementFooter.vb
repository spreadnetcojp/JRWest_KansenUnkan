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

Imports System.IO
Imports System.Text

''' <summary>
''' 監視盤プログラムと改札機プログラムの要素ファイルのフッタを抽象化したクラス。
''' </summary>
Public MustInherit Class EkProgramElementFooter

#Region "定数"
    Public Const Length As Integer = 96
    Protected VersionPos As Integer
    Protected VersionLen As Integer
    Protected DispNamePos As Integer
    Protected DispNameLen As Integer
#End Region

#Region "変数"
    Protected RawBytes(Length - 1) As Byte
#End Region

#Region "プロパティ"
    Public Overridable Property Version() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, VersionPos, VersionLen).TrimEnd(Chr(&H20))
        End Get

        Set(ByVal sVersion As String)
            Utility.FillBytes(&H20, RawBytes, VersionPos, VersionLen)
            Encoding.UTF8.GetBytes(sVersion, 0, sVersion.Length, RawBytes, VersionPos)
        End Set
    End Property

    Public Overridable Property DispName() As String
        Get
            Return Encoding.GetEncoding(932).GetString(RawBytes, DispNamePos, DispNameLen).TrimEnd(Chr(&H20))
        End Get

        Set(ByVal sDispName As String)
            Utility.FillBytes(&H20, RawBytes, DispNamePos, DispNameLen)
            Encoding.GetEncoding(932).GetBytes(sDispName, 0, sDispName.Length, RawBytes, DispNamePos)
        End Set
    End Property
#End Region

#Region "メソッド"
    'NOTE: sFooteredFilePathにファイルがない場合や、ファイルの長さが短い場合などには、
    'IOExceptionをスローします。
    Protected Sub New(ByVal sFooteredFilePath As String)
        Using oInputStream As New FileStream(sFooteredFilePath, FileMode.Open, FileAccess.Read)
            oInputStream.Seek(-Length, SeekOrigin.End)
            Dim pos As Integer = 0
            Do
                Dim readSize As Integer = oInputStream.Read(RawBytes, pos, Length - pos)
                If readSize = 0 Then Exit Do
                pos += readSize
            Loop
        End Using
    End Sub

    Public Overridable Function GetFormatViolation() As String
        If Not Utility.IsVisibleAsciiBytesFixed(RawBytes, VersionPos, VersionLen) Then
            Return "Version is invalid (not visible ASCII bytes)."
        End If

        Try
            'NOTE: プロパティのゲッタに副作用があってはならない（コンパイラは
            'そのように想定してよい）などの規定があるなら、オミットされる
            '可能性があるが、さすがにそのような規定はないものと想定している。
            Dim sDispName As String = DispName
        Catch ex As DecoderFallbackException
            Return "DispName is invalid."
        End Try

        Return Nothing
    End Function
#End Region

End Class
