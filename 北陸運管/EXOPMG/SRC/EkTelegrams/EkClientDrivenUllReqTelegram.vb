' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  新規作成
'   0.1      2017/04/10  (NES)小林  次世代車補対応
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' 最も基本的なクライアント主体ULLのREQ電文。
''' </summary>
Public Class EkClientDrivenUllReqTelegram
    Inherits EkReqTelegram
    Implements IXllReqTelegram

#Region "定数"
    Public Const FormalObjCodeAsOpClientFile As Byte = &H18
    Public Const FormalObjCodeAsKsbProgramVerInfo As Byte = &HAE
    Public Const FormalObjCodeAsGateMasterVerInfo As Byte = &HAF
    Public Const FormalObjCodeAsGateProgramVerInfo As Byte = &HAD
    '-------Ver0.1 次世代車補対応 DEL START-----------
    'Public Const FormalObjCodeAsGateRiyoData As Byte = &HAC
    '-------Ver0.1 次世代車補対応 DEL END-------------
    Public Const FormalObjCodeAsMadoMasterVerInfo As Byte = &H8B
    Public Const FormalObjCodeAsMadoProgramVerInfo As Byte = &H87
    '-------Ver0.1 次世代車補対応 DEL START-----------
    'Public Const FormalObjCodeAsMadoRiyoData As Byte = &HA0
    '-------Ver0.1 次世代車補対応 DEL END-------------

    Friend Shared ReadOnly oRawContinueCodeTable As New Dictionary(Of ContinueCode, Byte) From { _
       {ContinueCode.Start, &H1}, _
       {ContinueCode.Finish, &H2}, _
       {ContinueCode.Abort, &H10}}
    Friend Shared ReadOnly oContinueCodeTable As New Dictionary(Of Byte, ContinueCode) From { _
       {&H1, ContinueCode.Start}, _
       {&H2, ContinueCode.Finish}, _
       {&H10, ContinueCode.Abort}}

    Private Const ContinueCodePos As Integer = 0
    Private Const ContinueCodeLen As Integer = 1
    Private Const FileNamePos As Integer = ContinueCodePos + ContinueCodeLen
    Private Const FileNameLen As Integer = 80
    Private Const FileHashValuePos As Integer = FileNamePos + FileNameLen
    Private Const FileHashValueLen As Integer = 32
    Private Const ObjDetailLen As Integer = FileHashValuePos + FileHashValueLen
#End Region

#Region "変数"
    Private _TransferLimitTicks As Integer
#End Region

#Region "プロパティ"
    Private ReadOnly Property __ContinueCode() As ContinueCode Implements IXllTelegram.ContinueCode
        Get
            Return ContinueCode
        End Get
    End Property

    Public Property ContinueCode() As ContinueCode
        Get
            Dim code As ContinueCode
            If oContinueCodeTable.TryGetValue(RawBytes(GetRawPos(ContinueCodePos)), code) = False Then
                code = ContinueCode.None
            End If
            Return code
        End Get

        Set(ByVal code As ContinueCode)
            RawBytes(GetRawPos(ContinueCodePos)) = oRawContinueCodeTable(code)
        End Set
    End Property

    Public ReadOnly Property RawContinueCode() As Byte()
        Get
            Dim ret As Byte() = New Byte(ContinueCodeLen - 1) {}
            Buffer.BlockCopy(RawBytes, GetRawPos(ContinueCodePos), ret, 0, ContinueCodeLen)
            Return ret
        End Get
    End Property

    Public Property FileName() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(FileNamePos), FileNameLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal fileName As String)
            Array.Clear(RawBytes, GetRawPos(FileNamePos), FileNameLen)
            Encoding.UTF8.GetBytes(fileName, 0, fileName.Length, RawBytes, GetRawPos(FileNamePos))
        End Set
    End Property

    Public Property FileHashValue() As String
        Get
            Return Encoding.UTF8.GetString(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen).TrimEnd(Chr(0))
        End Get

        Set(ByVal fileHashValue As String)
            Array.Clear(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen)
            Encoding.UTF8.GetBytes(fileHashValue, 0, fileHashValue.Length, RawBytes, GetRawPos(FileHashValuePos))
        End Set
    End Property

    Public ReadOnly Property TransferListBase() As String Implements IXllReqTelegram.TransferListBase
        Get
            Return GetXllBasePath()
        End Get
    End Property

    Public ReadOnly Property TransferList() As List(Of String) Implements IXllReqTelegram.TransferList
        Get
            Dim oList As New List(Of String)(2)
            oList.Add(FileName)
            Return oList
        End Get
    End Property

    Private ReadOnly Property __TransferLimitTicks() As Integer Implements IXllReqTelegram.TransferLimitTicks
        Get
            Return _TransferLimitTicks
        End Get
    End Property

    Public Property TransferLimitTicks() As Integer
        Get
            Return _TransferLimitTicks
        End Get

        Set(ByVal ticks As Integer)
            _TransferLimitTicks = ticks
        End Set
    End Property

    Public ReadOnly Property IsHashValueReady() As Boolean Implements IXllReqTelegram.IsHashValueReady
        Get
            Return FileHashValue.Length <> 0
        End Get
    End Property

    Public ReadOnly Property IsHashValueIndicatingOkay() As Boolean Implements IXllReqTelegram.IsHashValueIndicatingOkay
        Get
            Dim sPath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), FileName)
            Dim sHashValue As String
            Try
                sHashValue = Utility.CalculateMD5(sPath)
            Catch ex As Exception
                Log.Error("Some Exception caught.", ex)
                Return False
            End Try
            Return StringComparer.OrdinalIgnoreCase.Compare(sHashValue, FileHashValue) = 0
        End Get
    End Property
#End Region

#Region "コンストラクタ"
    'String型のxxxはXxxLen文字以下のASCIIキャラクタで構成される文字列であることが前提です。
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal fileName As String, _
       ByVal transferLimitTicks As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ContinueCode = continueCode
        Me.FileName = fileName
        Me.FileHashValue = ""
        Me._TransferLimitTicks = transferLimitTicks
    End Sub

    'String型のxxxはXxxLen文字以下のASCIIキャラクタで構成される文字列であることが前提です。
    Public Sub New( _
       ByVal oGene As EkTelegramGene, _
       ByVal objCode As Integer, _
       ByVal continueCode As ContinueCode, _
       ByVal fileName As String, _
       ByVal fileHashValue As String, _
       ByVal transferLimitTicks As Integer, _
       ByVal replyLimitTicks As Integer)

        MyBase.New(oGene, EkCmdCode.Req, EkSubCmdCode.Get, objCode, ObjDetailLen, replyLimitTicks)
        Me.ContinueCode = continueCode
        Me.FileName = fileName
        Me.FileHashValue = fileHashValue
        Me._TransferLimitTicks = transferLimitTicks
    End Sub

    Public Sub New( _
       ByVal oTeleg As ITelegram, _
       ByVal transferLimitTicks As Integer)

        MyBase.New(oTeleg)
        Me._TransferLimitTicks = transferLimitTicks
    End Sub
#End Region

#Region "メソッド"
    'ボディ部の書式違反をチェックするメソッド
    Public Overrides Function GetBodyFormatViolation() As NakCauseCode
        If GetObjDetailLen() <> ObjDetailLen Then
            Log.Error("ObjSize is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(FileNamePos), FileNameLen) Then
            Log.Error("FileName is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        'NOTE: ハッシュ値は、後の比較でチェックされるはずであるため、ここでの
        'チェックは緩めにする（文字列に変換可能でありさえすればよい）。
        If Not Utility.IsVisibleAsciiBytes(RawBytes, GetRawPos(FileHashValuePos), FileHashValueLen) Then
            Log.Error("FileHashValue is invalid.")
            Return EkNakCauseCode.TelegramError
        End If

        'ここ以降、プロパティにアクセス可能。

        If Not Utility.IsValidVirtualPath(FileName) Then
            Log.Error("FileName is invalid (illegal path).")
            Return EkNakCauseCode.TelegramError
        End If

        Return EkNakCauseCode.None
    End Function

    'ACK電文を生成するメソッド
    Private Function CreateIAckTelegram() As IXllTelegram Implements IXllReqTelegram.CreateAckTelegram
        Return New EkClientDrivenUllAckTelegram(Gene, ObjCode, ContinueCode, FileHashValue)
    End Function

    'ACK電文を生成するメソッド
    Public Function CreateAckTelegram() As EkClientDrivenUllAckTelegram
        Return New EkClientDrivenUllAckTelegram(Gene, ObjCode, ContinueCode, FileHashValue)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Protected Overrides Function ParseAsIAck(ByVal oReplyTeleg As ITelegram) As ITelegram
        Return New EkClientDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Private Function ParseAsIXllAck(ByVal oReplyTeleg As ITelegram) As IXllTelegram Implements IXllReqTelegram.ParseAsAck
        Return New EkClientDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '渡された電文の型をACK電文の型に変換するメソッド
    Public Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As EkClientDrivenUllAckTelegram
        Return New EkClientDrivenUllAckTelegram(oReplyTeleg)
    End Function

    '渡された電文の型を同一型に変換するメソッド
    Public Function ParseAsSameKind(ByVal oNextTeleg As ITelegram) As IXllReqTelegram Implements IXllReqTelegram.ParseAsSameKind
        Return New EkClientDrivenUllReqTelegram(oNextTeleg, TransferLimitTicks)
    End Function

    '後続REQ電文を生成するメソッド
    Public Function CreateContinuousTelegram(ByVal continueCode As ContinueCode, ByVal transferLimitTicks As Integer, ByVal replyLimitTicks As Integer) As EkClientDrivenUllReqTelegram
        Return New EkClientDrivenUllReqTelegram( _
           Gene, _
           ObjCode, _
           continueCode, _
           FileName, _
           FileHashValue, _
           transferLimitTicks, _
           replyLimitTicks)
    End Function

    '渡された同一型電文のObjDetail部が同一のファイル転送を示しているか判定するメソッド
    Function IsContinuousWith(ByVal oXllReqTeleg As IXllReqTelegram) As Boolean Implements IXllReqTelegram.IsContinuousWith
        Dim oRealTeleg As EkClientDrivenUllReqTelegram = DirectCast(oXllReqTeleg, EkClientDrivenUllReqTelegram)
        If FileName <> oRealTeleg.FileName Then Return False
        'NOTE: FileHashValueは比較しないことにしている。
        Return True
    End Function

    'ACK電文からハッシュ値やファイル転送期限を取り込むメソッド
    Public Sub ImportFileDependentValueFromAck(ByVal oReplyTeleg As IXllTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromAck
        Dim oRealReplyTeleg As EkClientDrivenUllAckTelegram = DirectCast(oReplyTeleg, EkClientDrivenUllAckTelegram)
        FileHashValue = oRealReplyTeleg.FileHashValue
        'NOTE: このシーケンスでは、このメソッドが呼ばれることはないはずであるが、
        '実装可能であるため、とりあえず実装している。
        'なお、どのようなプロトコルであっても、このシーケンスのACK電文に
        'ファイル転送期限に相当する情報が設定されていることはあり得ないため、
        'ファイル転送期限の取り込みは行わない。
    End Sub

    '同一型電文からハッシュ値やファイル転送期限を取り込むメソッド
    Public Sub ImportFileDependentValueFromSameKind(ByVal oPreviousTeleg As IXllReqTelegram) Implements IXllReqTelegram.ImportFileDependentValueFromSameKind
        Dim oRealPreviousTeleg As EkClientDrivenUllReqTelegram = DirectCast(oPreviousTeleg, EkClientDrivenUllReqTelegram)
        FileHashValue = oRealPreviousTeleg.FileHashValue
        _TransferLimitTicks = oRealPreviousTeleg._TransferLimitTicks
    End Sub

    'HashValue部に値をセットするメソッド
    Public Sub UpdateHashValue() Implements IXllReqTelegram.UpdateHashValue
        Dim sPath As String = Utility.CombinePathWithVirtualPath(GetXllBasePath(), FileName)
        Try
            FileHashValue = Utility.CalculateMD5(sPath)
        Catch ex As Exception
            Log.Error("Some Exception caught.", ex)
            'NOTE: 以下のようにMD5としてあり得ない値にすることで、
            '相手に異常な電文と判断してもらう。
            'NOTE: 本来は、このメソッドが呼ばれる前に、正しくアクセス可能な
            'ファイルを設置しておくことは、アプリの責務であり、
            '例外はここでキャッチするべきではないかもしれないが、
            'ハードウェアの障害等で発生した異常で、いきなり落ちるのも
            '微妙であるため、とりあえず、このようにしておく。
            FileHashValue = ""
        End Try
    End Sub
#End Region

End Class
