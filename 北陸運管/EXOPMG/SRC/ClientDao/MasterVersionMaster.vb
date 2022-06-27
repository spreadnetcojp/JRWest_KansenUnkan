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

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' �o�^�ς̃}�X�^���o���^�}�X�^�K�p���X�g���o�����A�l���擾���ADataTable�Ɋi�[����B
''' </summary>
''' <remarks>�N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B</remarks>
Public Class MasterVersionMaster

    '�}�X�^�^�}�X�^�K�p���X�g�̃o�[�W�����擾���ʊi�[�e�[�u��
    Private dt As DataTable

    ''' <summary>
    ''' DB���A�f�[�^���擾����B
    ''' </summary>
    ''' <param name="model">�@��</param>
    ''' <param name="master">�}�X�^</param>
    ''' <param name="kbn">�f�[�^���</param>
    ''' <param name="pattern">�p�^�[��</param>
    ''' <returns>�}�X�^�^�}�X�^�K�p���X�g�̃o�[�W�����擾���ʊi�[�e�[�u��</returns>
    ''' <remarks>DB���A�w�肵�������Ɉ�v����f�[�^���擾����B</remarks>
    Public Function SelectTable(ByVal model As String, ByVal master As String, ByVal kbn As String, ByVal pattern As String) As DataTable
        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker

        '�e�[�u��:�}�X�^�^�}�X�^�K�p���X�g�̌��o��
        '�擾����:�t�@�C���敪
        '�擾����:�o�[�W����
        sSQL = "SELECT" _
             & "    DISTINCT LST.KBN," _
             & "    CASE" _
             & "        WHEN LST.KBN = 'DAT' THEN LST.DATA_VERSION" _
             & "        ELSE LST.LIST_VERSION" _
             & "    END AS VER" _
             & " FROM" _
             & "    (" _
             & "        SELECT" _
             & "            MODEL_CODE,DATA_KIND,DATA_SUB_KIND,DATA_VERSION" _
             & "        FROM" _
             & "            S_MST_DATA_HEADLINE" _
             & "    ) AS MST," _
             & "    (" _
             & "        SELECT MODEL_CODE,'" & kbn & "' AS KBN,DATA_KIND,DATA_SUB_KIND," _
             & "            DATA_VERSION,LIST_VERSION" _
             & "        FROM" _
             & "            S_MST_LIST_HEADLINE" _
             & "    ) AS LST" _
             & " WHERE" _
             & "     MST.MODEL_CODE = LST.MODEL_CODE AND MST.DATA_KIND = LST.DATA_KIND" _
             & " AND MST.DATA_SUB_KIND = LST.DATA_SUB_KIND AND MST.DATA_VERSION = LST.DATA_VERSION" _
             & " AND LST.MODEL_CODE = '" & model & "' AND LST.DATA_KIND = '" & master & "'" _
             & " AND LST.DATA_SUB_KIND = '" & pattern & "'"

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
    ''' <returns>�}�X�^�o�[�W�����}�X�^</returns>
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

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("KBN")
                dt.Columns.Add("VERSION")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
