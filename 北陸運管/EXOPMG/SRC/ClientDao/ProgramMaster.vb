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

Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>�w�O���[�v�}�X�^���A�l���擾���ADataTable�Ɋi�[����B</summary>
''' <remarks>�N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B</remarks>
Public Class ProgramMaster

    '�S�v���O����
    Private Const ALL_MODE As String = "�S�v���O����"
    '�O���[�v�}�X�^�擾���ʊi�[�e�[�u��
    Private dt As DataTable

    ''' <summary>DB���A�f�[�^���擾����B</summary>
    ''' <param name="model">�@��R�[�h</param>
    ''' <param name="bkbn">True:�K�p���X�g���̎擾�AFalse:�}�X�^���̎擾</param>
    ''' <returns>�v���O�����}�X�^���</returns>
    Public Function SelectTable(ByVal model As String, Optional ByVal bkbn As Boolean = False) As DataTable

        Dim sSQL As String = ""
        Dim strModel() As String
        Dim i As Integer
        Dim dbCtl As DatabaseTalker

        If System.String.IsNullOrEmpty(model) Then
            Log.Error("����model����ł��B") '�����s��
            Throw New DatabaseException()
        End If

        If model.Length > 14 Then
            'TODO: ������������14���͑Ó��Ȃ̂��H
            Log.Error("����model��14���𒴂��Ă��܂��B") '�����s��
            Throw New DatabaseException()
        End If
        '�{���\�b�h�̎��s�ɂ��ADataTable dt�͏����������B
        dt = New DataTable
        dbCtl = New DatabaseTalker

        sSQL = "SELECT FILE_KBN+DATA_KIND AS KIND,NAME FROM M_PRG_NAME WHERE USE_FLG='1' "
        If bkbn Then

        Else
            sSQL = sSQL & " AND FILE_KBN='DAT'"
        End If

        If model <> "" Then
            '������𕪊�
            strModel = model.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ")"
        End If

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dt
    End Function

    ''' <summary>DB���A�f�[�^���擾����B</summary>
    ''' <param name="model">�@��R�[�h</param>
    ''' <returns>�v���O�����}�X�^���</returns>
    Public Function SelectTable2(ByVal model As String) As DataTable

        Dim sSQL As String = ""
        Dim strModel() As String
        Dim i As Integer
        Dim dbCtl As DatabaseTalker

        If System.String.IsNullOrEmpty(model) Then
            Log.Error("����model����ł��B") '�����s��
            Throw New DatabaseException()
        End If

        If model.Length > 14 Then
            'TODO: ��������
            Log.Error("����model��14���𒴂��Ă��܂��B") '�����s��
            Throw New DatabaseException()
        End If

        '�{���\�b�h�̎��s�ɂ��ADataTable dt�͏����������B
        dt = New DataTable
        dbCtl = New DatabaseTalker

        sSQL = "SELECT MODEL_CODE AS KIND,NAME FROM M_PRG_NAME WHERE USE_FLG='1' AND FILE_KBN='DAT' "

        If model <> "" Then
            '������𕪊�
            strModel = model.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ")"
        End If

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dt
    End Function

    ''' <summary>DataTable�̐擪�ɁA�󔒍s��ǉ�����B</summary>
    ''' <returns>�v���O�����}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SetSpace() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'DataTable�̐擪�ɁA�󔒍s��ǉ�����B

        For i As Integer = 0 To dt.Columns.Count - 1
            drw.Item(i) = ""
        Next

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    '''<summary>DataTable�̐擪�ɁA�u�S�v���O�����v��ǉ�����B</summary>
    '''<returns>�v���O�����}�X�^���</returns>
    Public Function SetAll() As DataTable

        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'DataTable��MODEL�ɁA�uTERMINAL_ALL�v�ǉ�����B
        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL

        'DataTable��MODEL_NAME�ɁA�u�S�v���O�����v��ǉ�����B
        drw.Item(1) = ALL_MODE

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function


    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("PRG_KIND")
                dt.Columns.Add("PRG_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
