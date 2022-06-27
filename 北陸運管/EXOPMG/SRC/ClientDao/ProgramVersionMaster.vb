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
''' �v���O�����o�[�W�����}�X�^���A�l���擾���ADataTable�Ɋi�[����B
''' </summary>
''' <remarks>�N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B</remarks>
Public Class ProgramVersionMaster

    '�v���O�����o�[�W�����擾����
    Private dt As DataTable

#Region "DB���A�w�肵�������Ɉ�v����f�[�^���擾����B"
    ''' <summary>
    ''' DB���A�w�肵�������Ɉ�v����f�[�^���擾����B
    ''' </summary>
    ''' <param name="sModel">�@��R�[�h+�@��^�C�v</param>
    ''' <param name="sArea">�G���A</param>
    ''' <param name="kbn">�f�[�^���</param>
    ''' <param name="sProgram">�v���O�������</param>
    ''' <returns>�v���O�����o�[�W�����擾����</returns>
    Public Function SelectTable(ByVal sModel As String, ByVal sArea As String, ByVal kbn As String, ByVal sProgram As String) As DataTable
        Dim sSQL As String = ""
        Dim dbCtl As DatabaseTalker

        '�e�[�u��:�v���O�����^�v���O�����K�p���X�g�̌��o��
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
             & "            S_PRG_DATA_HEADLINE" _
             & "    ) AS PRG," _
             & "    (" _
             & "        SELECT" _
             & "            MODEL_CODE,'" & kbn & "' AS KBN,DATA_KIND,DATA_SUB_KIND," _
             & "            DATA_VERSION,LIST_VERSION" _
             & "        FROM" _
             & "            S_PRG_LIST_HEADLINE" _
             & "    ) AS LST" _
             & " WHERE" _
             & "     PRG.MODEL_CODE = LST.MODEL_CODE AND PRG.DATA_KIND = LST.DATA_KIND" _
             & " AND PRG.DATA_SUB_KIND = LST.DATA_SUB_KIND AND PRG.DATA_VERSION = LST.DATA_VERSION" _
             & " AND LST.MODEL_CODE = '" & sModel & "' AND LST.DATA_KIND = '" & sProgram & "'" _
             & " AND LST.DATA_SUB_KIND = '" & sArea & "'"

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
#End Region

#Region "DataTable�̐擪�ɁA�󔒍s��ǉ�����B"
    ''' <summary>
    ''' DataTable�̐擪�ɁA�󔒍s��ǉ�����B
    ''' </summary>
    ''' <returns>�}�X�^�p�^�[���}�X�^���</returns>
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

#End Region

End Class
