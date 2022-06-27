' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
' **********************************************************************
Imports AdvanceSoftware.VBReport7

'���[�v���r���[�N���X
Public Class PrintViewer

    '�v���r���[��`
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