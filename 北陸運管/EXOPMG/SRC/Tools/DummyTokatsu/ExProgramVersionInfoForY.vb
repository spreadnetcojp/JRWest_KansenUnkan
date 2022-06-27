' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/08/08  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

''' <summary>
''' �����̃v���O�����o�[�W�������������o���ۂɎg�p����N���X�B
''' </summary>
Public Class ExProgramVersionInfoForY

    'NOTE: �����o���Ȃ��ꍇ�ȂǂɁAIOException���X���[�����܂��B
    Public Shared Sub WriteToStream(ByVal oProgram As HoldingProgram, ByVal oOutputStream As Stream, ByVal area As Integer)
        Dim len As Integer = ProgramVersionInfoUtil.RecordLengthInBytes
        Dim oBytes(len - 1) As Byte
        If oProgram IsNot Nothing Then

            'TODO: �e�G���A�ɂ����ēK�p����镔�ނ͖{���̑����ɍ��킹��B
            '����ATOICA�G���A�̂݁A�ڑ��������ʂ����Ƃɖ{���̑����ɍ��킹�Ă���B

            'TODO: ���L���������Ȃ��ꍇ�ɁA�{���͂ǂ��Ώ�����̂��H
            If oProgram.ListHashValue IsNot Nothing Then
                ProgramVersionInfoUtil.SetFieldValueToBytes("�v���O�����K�p���X�g�o�[�W����", oProgram.ListVersion.ToString("D2"), oBytes)
                ProgramVersionInfoUtil.SetFieldValueToBytes("�v���O�����K�p��", oProgram.ApplicableDate, oBytes)
            End If
            CopyVersionListToInfo(oProgram, oBytes, "���ʕ� ���[�U�R�[�h")
            CopyVersionListToInfo(oProgram, oBytes, "���ʕ� �K�p�G���A")
            CopyVersionListToInfo(oProgram, oBytes, "���ʕ� �v���O�����敪")
            CopyVersionListToInfo(oProgram, oBytes, "���ʕ� �v���O�������싖��")
            CopyVersionListToInfo(oProgram, oBytes, "���ʕ� �v���O�����S��Ver�i�V�j")
            CopyVersionListToInfo(oProgram, oBytes, "���ʕ� �v���O�����S��Ver�i���j")
            CopyVersionListToInfo(oProgram, oBytes, "���ʕ� �\��")
            If area = 0 OrElse area = 1 OrElse area = 7 Then
                CopyVersionListToInfo(oProgram, oBytes, "�ݗ�IC����o�[�W����(Suica)")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "�ݗ�IC����o�[�W����(TOICA)")
            End If
            If area = 0 OrElse area = 2 Then
                CopyVersionListToInfo(oProgram, oBytes, "�ݗ�IC����o�[�W����(ICOCA)")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "�V����IC����o�[�W����")
            End If
            CopyVersionListToInfo(oProgram, oBytes, "EXIC����o�[�W����")
            If area = 0 OrElse area = 1 OrElse area = 7 Then
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^����1�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^����1�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^����2�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^����2�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^��")
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^�S�̃\�t�g�^��")
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "Suica�^���f�[�^�쐬�N����")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^����1�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^����1�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^����2�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^����2�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^��")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^�S�̃\�t�g�^��")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "TOICA�^���f�[�^�쐬�N����")
            End If
            If area = 0 OrElse area = 2 Then
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^����1�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^����1�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^����2�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^����2�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^��")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^�S�̃\�t�g�^��")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "ICOCA�^���f�[�^�쐬�N����")
            End If
            If area = 0 OrElse area = 3 Then
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^����1�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^����1�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^����2�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^����2�K�p�N����")
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^��")
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^�S�̃\�t�g�^��")
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^�o�[�W����")
                CopyVersionListToInfo(oProgram, oBytes, "���}�����f�[�^�쐬�N����")
            End If
            CopyVersionListToInfo(oProgram, oBytes, "���C�t�@�[���E�F�A�o�[�W����")
            CopyVersionListToInfo(oProgram, oBytes, "�\��")
            CopyVersionListToInfo(oProgram, oBytes, "�����؎��v���O������K�p�`�F�b�N�t���O")
            CopyVersionListToInfo(oProgram, oBytes, "�����L���v���O������K�p�`�F�b�N�t���O")
            CopyVersionListToInfo(oProgram, oBytes, "���l")
        End If
        oOutputStream.Write(oBytes, 0, oBytes.Length)
    End Sub

    Private Shared Sub CopyVersionListToInfo(ByVal oProgram As HoldingProgram, ByVal oBytes As Byte(), ByVal sFieldName As String)
        ProgramVersionInfoUtil.SetFieldValueToBytes( _
          sFieldName, _
          ProgramVersionListUtil.GetFieldValueFromBytes(sFieldName, oProgram.VersionListData), _
          oBytes)
    End Sub

End Class
