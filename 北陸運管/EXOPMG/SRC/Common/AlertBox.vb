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

Imports System.Windows.Forms

''' <summary>
''' 代替メッセージボックスに指定するボタン情報
''' </summary>
Public Enum AlertBoxAttr As Integer
    OK
    OKCancel
    CancelOK
    YesNo
    NoYes
    YesNoCancel
    NoYesCancel
    CancelYesNo
    AbortRetryIgnore
    RetryAbortIgnore
    IgnoreAbortRetry
    RetryCancel
    CancelRetry
End Enum

''' <summary>
''' 代替メッセージボックス
''' </summary>
Public Class AlertBox

#Region "公開メソッド"
    ''' <summary>
    ''' メッセージボックスを表示する。
    ''' </summary>
    ''' <param name="attr">ボタン情報</param>
    ''' <param name="message">0個以上の書式項目を含んだ書式文言</param>
    ''' <param name="args">0個以上の書式設定対象オブジェクトを含んだ Object配列</param>
    ''' <returns>押されたボタンの値(System.Windows.Forms.DialogResult)</returns>
    Public Shared Function Show(ByVal attr As AlertBoxAttr, ByVal message As Sentence, ByVal ParamArray args As Object()) As DialogResult
        Return ShowCore(attr, message, args)
    End Function

    ''' <summary>
    ''' メッセージボックスを表示する。
    ''' </summary>
    ''' <param name="message">0個以上の書式項目を含んだ書式文言</param>
    ''' <param name="args">0個以上の書式設定対象オブジェクトを含んだ Object配列</param>
    ''' <returns>押されたボタンの値(System.Windows.Forms.DialogResult)</returns>
    Public Shared Function Show(ByVal message As Sentence, ByVal ParamArray args As Object()) As DialogResult
        Return ShowCore(AlertBoxAttr.OK, message, args)
    End Function
#End Region

#Region "非公開メソッド"
    ''' <summary>
    ''' メッセージボックスを表示する。
    ''' </summary>
    ''' <param name="attr">ボタン情報</param>
    ''' <param name="message">0個以上の書式項目を含んだ書式文言</param>
    ''' <param name="args">0個以上の書式設定対象オブジェクトを含んだ Object配列</param>
    ''' <returns>押されたボタンの値(System.Windows.Forms.DialogResult)</returns>
    Private Shared Function ShowCore(ByVal attr As AlertBoxAttr, ByVal message As Sentence, ByVal ParamArray args As Object()) As DialogResult
        Try
            Dim sMsg As String = message.Gen(args)

            Dim sTitle As String
            Dim icon As MessageBoxIcon
            Select Case message.Attr
                Case SentenceAttr.None
                    sTitle = BaseLexis.NoneTitle.Gen()
                    icon = MessageBoxIcon.None
                Case SentenceAttr.Information
                    sTitle = BaseLexis.InformationTitle.Gen()
                    icon = MessageBoxIcon.Information
                Case SentenceAttr.Warning
                    sTitle = BaseLexis.WarningTitle.Gen()
                    icon = MessageBoxIcon.Warning
                Case SentenceAttr.Error
                    sTitle = BaseLexis.ErrorTitle.Gen()
                    icon = MessageBoxIcon.Error
                Case SentenceAttr.Question
                    sTitle = BaseLexis.QuestionTitle.Gen()
                    icon = MessageBoxIcon.Question
                Case Else
                    sTitle = ""
                    icon = MessageBoxIcon.None
            End Select

            Dim buttons As MessageBoxButtons
            Select Case attr
                Case AlertBoxAttr.OK
                     buttons = MessageBoxButtons.OK
                Case AlertBoxAttr.OKCancel, AlertBoxAttr.CancelOK
                     buttons = MessageBoxButtons.OKCancel
                Case AlertBoxAttr.YesNo, AlertBoxAttr.NoYes
                     buttons = MessageBoxButtons.YesNo
                Case AlertBoxAttr.YesNoCancel, AlertBoxAttr.NoYesCancel, AlertBoxAttr.CancelYesNo
                     buttons = MessageBoxButtons.YesNoCancel
                Case AlertBoxAttr.AbortRetryIgnore, AlertBoxAttr.RetryAbortIgnore, AlertBoxAttr.IgnoreAbortRetry
                     buttons = MessageBoxButtons.AbortRetryIgnore
                Case AlertBoxAttr.RetryCancel, AlertBoxAttr.CancelRetry
                     buttons = MessageBoxButtons.RetryCancel
                Case Else
                     buttons = MessageBoxButtons.OK
            End Select

            Dim defaultBotton As MessageBoxDefaultButton
            Select Case attr
                Case AlertBoxAttr.OK, _
                     AlertBoxAttr.OKCancel, _
                     AlertBoxAttr.YesNo, _
                     AlertBoxAttr.YesNoCancel, _
                     AlertBoxAttr.AbortRetryIgnore, _
                     AlertBoxAttr.RetryCancel
                    defaultBotton = MessageBoxDefaultButton.Button1

                Case AlertBoxAttr.CancelOK, _
                     AlertBoxAttr.NoYes, _
                     AlertBoxAttr.NoYesCancel, _
                     AlertBoxAttr.RetryAbortIgnore, _
                     AlertBoxAttr.CancelRetry
                    defaultBotton = MessageBoxDefaultButton.Button2

                Case AlertBoxAttr.CancelYesNo, _
                     AlertBoxAttr.IgnoreAbortRetry
                    defaultBotton = MessageBoxDefaultButton.Button3

                Case Else
                    defaultBotton = MessageBoxDefaultButton.Button1
            End Select

            Return MessageBox.Show(sMsg, sTitle, buttons, icon, defaultBotton)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            Return MessageBox.Show(ex.Message & vbCrLf & _
                                      "[" & Utility.ClsName() & "." & Utility.MethodName() & "]", _
                                   "深刻な異常", _
                                   MessageBoxButtons.OK, _
                                   MessageBoxIcon.Error, _
                                   MessageBoxDefaultButton.Button1)
        End Try
    End Function
#End Region

End Class
