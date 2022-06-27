' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  OPMGMdlCmnPublic���������č쐬
' **********************************************************************
Option Strict On
Option Explicit On

''' <summary>
''' ���L�Ϗ��i�[�p���W���[��
''' </summary>
Public Class GlobalVariables

#Region "�萔"
    Friend Shared ReadOnly LockObject As New Object()
#End Region

#Region "�ϐ�"
    'NOTE: ���̎Q�ƌ^�ϐ��𒼐ړǂݏ�������ۂ́A
    '���O��SyncLock LockObject������ōs�����ƁB
    Friend Shared SysUserId As String = "SYS"
#End Region

#Region "���J�v���p�e�B"
    Public Shared Property UserId() As String
        Get
            Dim sRetVal As String
            SyncLock GlobalVariables.LockObject
                sRetVal = SysUserId
            End SyncLock
            Return sRetVal
        End Get

        Set(ByVal sVal As String)
            If String.IsNullOrEmpty(sVal) Then
                sVal = "SYS"
            End If
            SyncLock GlobalVariables.LockObject
                SysUserId = sVal
            End SyncLock
        End Set
    End Property
#End Region

End Class
