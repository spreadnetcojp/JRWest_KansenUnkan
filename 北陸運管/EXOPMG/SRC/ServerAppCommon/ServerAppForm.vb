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

Public Class ServerAppForm

    Private Sub ServerAppForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '画面サイズを設定する。
        Me.Size = New Size(ServerAppBaseConfig.FormWidth, ServerAppBaseConfig.FormHeight)
        '画面表示位置を設定する。
        Me.Location = New Point(ServerAppBaseConfig.FormPosX, ServerAppBaseConfig.FormPosY)
        '画面タイトルを設定する。
        Me.Text = ServerAppBaseConfig.FormTitle
    End Sub

End Class
