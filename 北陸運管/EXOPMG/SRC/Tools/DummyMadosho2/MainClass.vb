' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/06/27  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Threading

Imports JR.ExOpmg.Common

''' <summary>
''' ���d���������@�������p�f�[�^�T�[�o�̃��C����������������N���X�B
''' </summary>
Public Class MainClass

    ''' <summary>
    ''' ���d���������@�������p�f�[�^�T�[�o�̃��C�������B
    ''' </summary>
    <STAThread()> _
    Public Shared Sub Main()
        Application.EnableVisualStyles()
        Dim sWorkingDir As String = System.Environment.CurrentDirectory
        Dim m As New Mutex(False, "ExOpmgDummyMadosho2")
        If m.WaitOne(0, False) Then
            Dim sLogBasePath As String = Path.Combine(sWorkingDir, "LOG")
            Log.Init(sLogBasePath, "DummyMadosho2")
            Log.Info("�v���Z�X�J�n")

            Using oForm As New MainForm()
                Try
                    '��ʕ\���iUI�p���b�Z�[�W���[�v���s�j
                    oForm.ShowDialog()
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    oForm.Close()
                End Try
            End Using

            Log.Info("�v���Z�X�I��")

            'NOTE: ������ʂ�Ȃ��Ă��A���̃X���b�h�̏��łƂƂ��ɉ�������
            '�悤�Ȃ̂ŁA�ň��̐S�z�͂Ȃ��B
            m.ReleaseMutex()
        Else
            AlertBox.Show(Lexis.MultipleInstanceNotAllowed)
        End If
    End Sub

End Class
