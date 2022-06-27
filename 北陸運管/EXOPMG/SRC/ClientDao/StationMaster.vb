' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)�͘e  �V�K�쐬
'   0.1      2014/04/01  (NES)�͘e  �k���Ή��F�O���[�v�{�x�ЃR�[�h���w���\��
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' �@��\���}�X�^���A�w�����擾���ADataTable�Ɋi�[����B
''' </summary>
''' <remarks>�N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B</remarks>
Public Class StationMaster

    '�w�}�X�^�擾���ʊi�[�e�[�u��
    Private dt As DataTable

    Private Const ALL_STATION As String = "�S�w"

    '�K�p�J�n��
    Private sApplyDate As String = ""
    '�K�p�J�n��
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

    ''' <summary>�v���p�e�BApplyDate�ɁA�{���̓��t(YYYYMMDD)���Z�b�g����</summary>
    Public Sub New()
        ApplyDate = Now.ToString("yyyyMMdd")
    End Sub

    ''' <summary>DB���A�w�肵�������Ɉ�v����f�[�^���擾����B</summary>
    ''' <param name="sFlg">�x�Вǉ��L���@False�F���ATrue�F�L</param>
    ''' <param name="sModel">�Ώۋ@��</param>
    ''' <param name="GroupSortFlg">�w�R�[�h�̃O���[�vNo,�x�ЃR�[�h�t�^�L���@False�F���ATrue�F�L</param>
    ''' <returns>�w�}�X�^���</returns>
    Public Function SelectTable(ByVal sFlg As Boolean, ByVal sModel As String, Optional ByVal GroupSortFlg As Boolean = False) As DataTable
        ' --- Ver0.1 �O���[�v�{�x�ЃR�[�h���w���\�� MOD
        'Public Function SelectTable(ByVal sFlg As Boolean, ByVal sModel As String) As DataTable
        Dim sSQL As String = ""
        Dim sSQLsub As String = ""
        Dim dbCtl As DatabaseTalker
        Dim strModel() As String
        Dim i As Integer

        dbCtl = New DatabaseTalker

        If sModel <> "" Then
            '������𕪊�
            strModel = sModel.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    ' --- Ver0.1 �O���[�v�{�x�ЃR�[�h���w���\�� MOD START
                    'sSQLsub = " AND (MODEL_CODE='" & strModel(i) & "' "
                    sSQLsub = " (MODEL_CODE='" & strModel(i) & "' "
                    ' --- Ver0.1 �O���[�v�{�x�ЃR�[�h���w���\�� MOD END
                Else
                    sSQLsub = sSQLsub & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQLsub = sSQLsub & ")"
        End If

        Try
            ' --- Ver0.1 �O���[�v�{�x�ЃR�[�h���w���\�� MOD START
            'If sFlg Then
            '    sSQL = " SELECT '000'+OFFICE.BRANCH_OFFICE_CODE AS STATION_CODE," _
            '         & " M_BRANCH_OFFICE.NAME AS STATION_NAME" _
            '         & " FROM (SELECT DISTINCT BRANCH_OFFICE_CODE" _
            '         & " FROM M_MACHINE WHERE SETTING_START_DATE=(SELECT MAX(SETTING_START_DATE)" _
            '         & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')" _
            '         & sSQLsub & ") AS OFFICE,M_BRANCH_OFFICE" _
            '         & " WHERE OFFICE.BRANCH_OFFICE_CODE=M_BRANCH_OFFICE.CODE" _
            '         & " UNION"
            'End If

            'sSQL = sSQL & " SELECT DISTINCT RAIL_SECTION_CODE+STATION_ORDER_CODE AS STATION_CODE" _
            ' & " ,STATION_NAME" _
            ' & " FROM M_MACHINE WHERE SETTING_START_DATE=(SELECT MAX(SETTING_START_DATE) " _
            ' & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')" _
            ' & sSQLsub & " ORDER BY STATION_CODE"
            sSQL = sSQL + "SELECT"
            If GroupSortFlg Then
                ' �w���R�[�h�ɃO���[�v�{�x�ЃR�[�h��t�^
                sSQL = sSQL + "   LTRIM(GROUP_NO2+BRANCH_OFFICE_CODE+RAIL_SECTION_CODE+STATION_ORDER_CODE) AS STATION_CODE,"
            Else
                sSQL = sSQL + "   LTRIM(RAIL_SECTION_CODE+STATION_ORDER_CODE) AS STATION_CODE, "
            End If
            sSQL = sSQL + "   STATION_NAME"
            sSQL = sSQL + " FROM("
            ' �x�Ђ̒ǉ��L��
            If sFlg Then
                sSQL = sSQL + "   SELECT"
                sSQL = sSQL + "     '0' AS GROUP_NO,'000' AS BRANCH_OFFICE_CODE,"
                sSQL = sSQL + "     '000' AS RAIL_SECTION_CODE,"
                sSQL = sSQL + "     V_MACHINE_NOW.BRANCH_OFFICE_CODE AS STATION_ORDER_CODE,"
                sSQL = sSQL + "     M_BRANCH_OFFICE.NAME AS STATION_NAME,"
                sSQL = sSQL + "     CONVERT(varchar,M_BRANCH_OFFICE.GROUP_NO) AS GROUP_NO2"
                sSQL = sSQL + "   FROM"
                sSQL = sSQL + "     V_MACHINE_NOW,M_BRANCH_OFFICE"
                sSQL = sSQL + "   WHERE"
                sSQL = sSQL + "     V_MACHINE_NOW.BRANCH_OFFICE_CODE=M_BRANCH_OFFICE.CODE"
                If sSQLsub <> "" Then
                    sSQL = sSQL + "     AND " + sSQLsub
                End If
                sSQL = sSQL + "   UNION"
            End If
            sSQL = sSQL + "   SELECT"
            sSQL = sSQL + "     CONVERT(varchar,GROUP_NO)AS GROUP_NO,BRANCH_OFFICE_CODE,"
            sSQL = sSQL + "     RAIL_SECTION_CODE,STATION_ORDER_CODE,STATION_NAME,"
            sSQL = sSQL + "     CONVERT(varchar,GROUP_NO)AS GROUP_NO2"
            sSQL = sSQL + "   FROM"
            sSQL = sSQL + "     V_MACHINE_NOW"
            If sSQLsub <> "" Then
                sSQL = sSQL + "   WHERE " + sSQLsub
            End If
            sSQL = sSQL + "   GROUP BY"
            sSQL = sSQL + "     GROUP_NO,BRANCH_OFFICE_CODE,RAIL_SECTION_CODE,"
            sSQL = sSQL + "     STATION_ORDER_CODE,STATION_NAME"
            sSQL = sSQL + " ) AS DAT"
            ' --- Ver0.1 �O���[�v�{�x�ЃR�[�h���w���\�� MOD END

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
    ''' <returns>�w�}�X�^���</returns>
    Public Function SetSpace() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try
            For i As Integer = 0 To dt.Columns.Count - 1
                drw.Item(i) = ""
            Next
            dt.Rows.InsertAt(drw, 0)
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function

    ''' <summary>DataTable�̐擪�ɁA�u�S�w�v��ǉ�����B</summary>
    ''' <returns>�w�}�X�^���</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
            drw.Item(1) = ALL_STATION
            dt.Rows.InsertAt(drw, 0)
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("NONMONITOR_STATION_CODE")
                dt.Columns.Add("NONMONITOR_STATION_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class