' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Public Class ServerAppForm

    Private Sub ServerAppForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '��ʃT�C�Y��ݒ肷��B
        Me.Size = New Size(ServerAppBaseConfig.FormWidth, ServerAppBaseConfig.FormHeight)
        '��ʕ\���ʒu��ݒ肷��B
        Me.Location = New Point(ServerAppBaseConfig.FormPosX, ServerAppBaseConfig.FormPosY)
        '��ʃ^�C�g����ݒ肷��B
        Me.Text = ServerAppBaseConfig.FormTitle
    End Sub

End Class
