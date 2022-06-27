' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
' **********************************************************************
Option Strict On
Option Explicit On

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' �N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B
''' �ʘH�����̏���DataTable�Ɋi�[����B
''' </summary>
Public Class DirectionMaster

    ''' <summary>
    ''' �ʘH�������(�����Œ�)�擾���ʊi�[�e�[�u��
    ''' </summary>
    Private dt As DataTable

    ''' <summary>
    ''' �ʘH����:���D
    ''' </summary>
    Private ReadOnly LcstKaisatu As String = "���D"

    ''' <summary>
    ''' �ʘH����:�W�D
    ''' </summary>
    Private ReadOnly LcstSyusatu As String = "�W�D"

    ''' <summary>�ݒ�f�[�^��ԋp����</summary>
    ''' <returns>�ʘH�������</returns>
    Public Function SelectTable() As DataTable

        Dim drw As DataRow
        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker

        dbCtl = New DatabaseTalker

        sSQL = " SELECT FLG" _
             & " , '" & LcstKaisatu & "' name " _
             & "  FROM M_PASSAGE" _
             & " WHERE NAME like '%" & LcstKaisatu & "%'" _
             & " UNION " _
             & " SELECT FLG" _
             & " , '" & LcstSyusatu & "' name " _
             & "  FROM M_PASSAGE" _
             & " WHERE NAME like '%" & LcstSyusatu & "%'" _
             & " ORDER BY FLG"

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSQL)

            drw = dt.NewRow()
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL : drw.Item(1) = "������"
            dt.Rows.InsertAt(drw, 0)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            Throw New OPMGException(ex)
        Finally
            dbCtl.ConnectClose()
            dbCtl = Nothing
        End Try

        Return dt
    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("CODE")
                dt.Columns.Add("NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
