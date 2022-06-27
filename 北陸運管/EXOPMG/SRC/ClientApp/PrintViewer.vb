' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
' **********************************************************************
Imports AdvanceSoftware.VBReport7

'帳票プレビュークラス
Public Class PrintViewer

    'プレビュー定義
    Public Sub GetDocument(ByVal doc As Document, Optional ByVal fname As String = "")
        viewerControl2.Clear()
        viewerControl2.ShowToolBar = True
        viewerControl2.ViewZoom = 100
        viewerControl2.Document = doc
        If fname <> "" Then
            viewerControl2.SaveFileName = fname
        Else
            viewerControl2.SaveFileName = doc.ViewData(0).sheetName
        End If
        Me.Text = doc.ViewData(0).sheetName
    End Sub

    Private Sub buttonClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles buttonClose.Click
        Me.Close()
    End Sub
End Class