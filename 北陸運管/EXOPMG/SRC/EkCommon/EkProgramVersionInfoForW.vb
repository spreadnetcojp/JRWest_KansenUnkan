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

Public Class EkProgramVersionInfoReaderForW
    Inherits EkProgramVersionInfoReader

    Public Sub New()
        Me.VersionPos = 20
        Me.VersionLen = 8
        Me.DispNamePos = 36
        Me.DispNameLen = 64
    End Sub

End Class
