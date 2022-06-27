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

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

Public Class UiStateClass

    '「基本」タブにあるMyTelegrapherの参照項目
    Public AutomaticComStart As Boolean
    Public CapSndTelegs As Boolean
    Public CapRcvTelegs As Boolean
    Public CapSndFiles As Boolean
    Public CapRcvFiles As Boolean

    '「電文送信」タブにあるMyTelegrapherの参照項目


    '「POST電文受信」タブにあるMyTelegrapherの参照項目
    Public StatusCodeForPassivePostSeqCodes As Dictionary(Of NkSeqCode, UShort)

    Public Sub New()
        '「基本」タブにあるMyTelegrapherの参照項目
        Me.AutomaticComStart = True
        Me.CapSndTelegs = True
        Me.CapRcvTelegs = True
        Me.CapSndFiles = False
        Me.CapRcvFiles = False

        '「電文送信」タブにあるMyTelegrapherの参照項目


        '「POST電文受信」タブにあるMyTelegrapherの参照項目
        Me.StatusCodeForPassivePostSeqCodes = New Dictionary(Of NkSeqCode, UShort)
            RegisterPathToPassivePostSeqCodes(NkSeqCode.Collection, 0)
    End Sub

    '指定データ種別のデフォルト受信ディレクトリパスを
    'Me.ApplyFileForPassivePostSeqCodesに追加する。
    Private Sub RegisterPathToPassivePostSeqCodes(ByVal seqCode As NkSeqCode, ByVal statusCode As UShort)
        Me.StatusCodeForPassivePostSeqCodes.Add(seqCode, statusCode)
    End Sub

End Class
