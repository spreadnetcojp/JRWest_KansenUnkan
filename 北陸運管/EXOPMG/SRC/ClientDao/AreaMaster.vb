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

''' <summary>
''' �G���A�}�X�^���A�l���擾���ADataTable�Ɋi�[����B
''' </summary>
''' <remarks>�N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B</remarks>
Public Class AreaMaster

    '�G���A�}�X�^�擾���ʊi�[�e�[�u��
    Private dt As DataTable

    'set �S�p�^�[��
    Private Const AllArea As String = "�S�K�p�G���A"

    ''' <summary> DB���A�f�[�^���擾����B</summary>
    ''' <remarks>
    '''  DB���A�w�肵�������Ɉ�v����f�[�^���擾����B
    ''' </remarks>
    ''' <param name="model">�@��</param>
    ''' <returns>�G���A�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SelectTable(ByVal model As String) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker

        sSQL = "SELECT AREA_NO, AREA_NAME FROM M_AREA_DATA" _
             & " WHERE MODEL_CODE='" & model & "'"

        dbCtl = New DatabaseTalker

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
    ''' <returns>�G���A�}�X�^�擾���ʊi�[�e�[�u��</returns>
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

    ''' <summary>DataTable�̐擪�ɁA�u�S�G���A�v��ǉ�����B</summary>
    ''' <returns>�G���A�}�X�^���</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'English DataRow��AREA_NO�ɁA�uTERMINAL_ALL�v�ǉ�����B
        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL

        'English DataRow��AREA_NAME�ɁA�uTERMINAL_ALL�v�ǉ�����B
        drw.Item(1) = AllArea

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("AREA_NO")
                dt.Columns.Add("AREA_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
