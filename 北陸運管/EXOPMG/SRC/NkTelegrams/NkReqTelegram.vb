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

Imports JR.ExOpmg.Common

''' <summary>
''' REQ電文。
''' </summary>
''' <remarks>
''' あくまで、各種REQ電文クラスの実装の一部を代行するためのクラスである。
''' </remarks>
Public MustInherit Class NkReqTelegram
    Inherits NkTelegram
    Implements IReqTelegram

#Region "変数"
    Private _ReplyLimitTicks As Integer
#End Region

#Region "プロパティ"
    Private ReadOnly Property __ReplyLimitTicks() As Integer Implements IReqTelegram.ReplyLimitTicks
        Get
            Return _ReplyLimitTicks
        End Get
    End Property

    Public Property ReplyLimitTicks() As Integer
        Get
            Return _ReplyLimitTicks
        End Get

        Set(ByVal ticks As Integer)
            _ReplyLimitTicks = ticks
        End Set
    End Property
#End Region

#Region "コンストラクタ（サブクラスのコンストラクタの実装用）"
    Protected Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal cmdCode As NkCmdCode, _
       ByVal objLen As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(seqCode, cmdCode, objLen)
        Me._ReplyLimitTicks = replyLimitTicks
    End Sub

    Protected Sub New( _
       ByVal seqCode As NkSeqCode, _
       ByVal cmdCode As NkCmdCode, _
       ByVal objHeaderLen As Integer, _
       ByVal oObjFilePathList As List(Of String), _
       ByVal objFilesCombinedLen As Long, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(seqCode, cmdCode, objHeaderLen, oObjFilePathList, objFilesCombinedLen)
        Me._ReplyLimitTicks = replyLimitTicks
    End Sub

    'iTelegの実体がNkTelegramであることを前提とするメソッドです。
    '誤った使い方をした場合は、InvalidCastExceptionがスローされます。
    Protected Sub New(ByVal iTeleg As ITelegram)
        MyBase.New(iTeleg)
        If TypeOf iTeleg Is NkReqTelegram Then
            Me._ReplyLimitTicks = DirectCast(iTeleg, NkReqTelegram)._ReplyLimitTicks
        Else
            'NOTE: iTelegが受信電文（EkDodgyTelegram）の場合は、こちらのケース
            'として処理が行われるが、それは意図通りである。
            'このプロトコルでは、電文に応答受信期限に相当する情報は格納されていない
            '（だからこそ、このような専用メンバに別途設定することになっている）。
            'すなわち、ReplyLimitTicksプロパティは、REQ電文を送信する側でのみ意味を持つ。
            Me._ReplyLimitTicks = 0
        End If
    End Sub
#End Region

#Region "メソッド"
    '渡された電文がACKとして整合性があるか判定するメソッド
    Public MustOverride Function IsValidAck(ByVal iReplyTeleg As ITelegram) As Boolean Implements IReqTelegram.IsValidAck

    '渡された電文がNAKとして整合性があるか判定するメソッド
    Public Function IsValidNak(ByVal iReplyTeleg As ITelegram) As Boolean Implements IReqTelegram.IsValidNak
        Return False
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Protected MustOverride Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram Implements IReqTelegram.ParseAsAck

    '渡された電文の型をNAK電文の型に変換するメソッド
    Private Function ParseAsINak(ByVal oReplyTeleg As ITelegram) As INakTelegram Implements IReqTelegram.ParseAsNak
        Return Nothing
    End Function
#End Region

End Class
