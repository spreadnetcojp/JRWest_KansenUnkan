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
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary> �}�X�^�f�[�^�}�X�^���A�l���擾���ADataTable�Ɋi�[����B </summary>
''' <remarks>
''' �N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B
''' </remarks>

Public Class MasterMaster
    '�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��
    Private dt As DataTable

    ''' <summary>DB���A�w�肵�������Ɉ�v����f�[�^���擾����B</summary>
    ''' <remarks>
    '''  DB���A�w�肵�������Ɉ�v����f�[�^���擾����B
    ''' </remarks>
    ''' <param name="model">�@��R�[�h</param>
    ''' <param name="bkbn">True:�K�p���X�g���̎擾�AFalse:�}�X�^���̎擾</param>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SelectTable(ByVal model As String, Optional ByVal bkbn As Boolean = False) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker
        Dim strModel() As String
        Dim i As Integer

        '�e�[�u��:�}�X�^�f�[�^�}�X�^,�@��ʃ}�X�^�ݒ�
        '�擾����:�}�X�^���
        '�擾����:�}�X�^����
        sSQL = "SELECT DATA_KIND,NAME FROM(SELECT DATA_KIND,NAME,MST_NO,row_number()" _
            & " over(partition by MST_NO order by DATA_KIND,NAME) AS RANK FROM M_MST_NAME WHERE USE_FLG='1'"
        If bkbn Then
            sSQL = sSQL & " AND FILE_KBN='LST'"
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
            sSQL = sSQL & ")) AS DA WHERE RANK='1'"
        Else
            sSQL = sSQL & " ) AS DA WHERE RANK='1'"
        End If

        dbCtl = New DatabaseTalker()

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
    ''' <summary>
    ''' DB���A�w�肵�������Ɉ�v����f�[�^���擾����
    ''' </summary>
    ''' <param name="model">�@��R�[�h</param>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��</returns>
    ''' <remarks>DB���A�w�肵�������Ɉ�v����f�[�^���擾����</remarks>
    Public Function SelectTable2(ByVal model As String) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker

        '�p�����[�^�[���`�F�b�N����
        If System.String.IsNullOrEmpty(model) Or model.Length > 14 Then
            'TODO: ������������14���͑Ó��Ȏd�l�Ȃ̂��H
            Log.Error("����model��14���𒴂��Ă��܂��B") '�����s��
            Throw New DatabaseException()
        End If

        '�e�[�u��:�}�X�^�f�[�^�}�X�^,�@��ʃ}�X�^�ݒ�
        '�擾����:�t�@�C���敪�{�}�X�^���
        '�擾����:�}�X�^����
        sSQL = "SELECT FILE_KBN+DATA_KIND AS KIND, NAME FROM M_MST_NAME WHERE" _
             & " MODEL_CODE='" & model & "' AND USE_FLG='1' ORDER BY MST_NO"

        dbCtl = New DatabaseTalker()

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
    ''' <summary>
    ''' DB���A�w�肵�������Ɉ�v����f�[�^���擾����
    ''' </summary>
    ''' <param name="model">�@��R�[�h</param>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��</returns>
    ''' <remarks>DB���A�w�肵�������Ɉ�v����f�[�^���擾����</remarks>
    Public Function SelectTableShort(ByVal model As String) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker

        '�p�����[�^�[���`�F�b�N����
        If System.String.IsNullOrEmpty(model) Or model.Length > 14 Then
            Log.Error("����model��14���𒴂��Ă��܂��B") '�����s��
            Throw New DatabaseException()
        End If

        '�e�[�u��:�}�X�^�f�[�^�}�X�^,�@��ʃ}�X�^�ݒ�
        '�擾����:�}�X�^���+�}�X�^����
        '�擾����:�}�X�^����
        sSQL = "SELECT DATA_KIND+SHORT_NAME AS KIND, NAME FROM M_MST_NAME WHERE" _
             & " FILE_KBN='DAT' AND MODEL_CODE='" & model & "' AND USE_FLG='1' ORDER BY MST_NO"

        dbCtl = New DatabaseTalker()

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
    ''' <remarks>
    '''  DataTable�̐擪�ɁA�󔒍s��ǉ�����B
    ''' </remarks>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��</returns>
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

    ''' <summary>DataTable�̐擪�ɁA�u�S�}�X�^�v��ǉ�����B</summary>
    ''' <remarks>
    '''  DataTable�̐擪�ɁA�u�S�}�X�^�v��ǉ�����B
    ''' </remarks>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()
        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
        drw.Item(1) = "�S�}�X�^"
        dt.Rows.InsertAt(drw, 0)

        Return dt
    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("DATA_KIND")
                dt.Columns.Add("NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
