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
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' ���@�}�X�^���A�l���擾���ADataTable�Ɋi�[����B
''' </summary>
''' <remarks>�N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B</remarks>

Public Class UnitMaster
    '���@�}�X�^�擾���ʊi�[�e�[�u��
    Private dt As DataTable

    '�S���@
    Private Const ALL_UNIT As String = "�S���@"

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

    ''' <summary> DB���A�w�肵�������Ɉ�v����f�[�^���擾����B</summary>
    ''' <remarks>
    '''  DB���A�w�肵�������Ɉ�v����f�[�^���擾����B
    ''' </remarks>
    ''' <param name="station">�w</param>
    ''' <param name="corner">�R�[�i</param>
    ''' <param name="model">�@��</param>
    ''' <returns>���@�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SelectTable(ByVal station As String, ByVal corner As String, ByVal model As String) As DataTable
        Dim sSQL As String
        Dim strModel() As String
        Dim i As Integer
        Dim dbCtl As DatabaseTalker

        '�p�����[�^�[���`�F�b�N����
        'If (System.String.IsNullOrEmpty(station) Or station.Length <> 8) Then
        If (System.String.IsNullOrEmpty(station) Or station.Length <> 6) Then
            Log.Error("����station��6���ł���܂���B") '�����s��
            Throw New DatabaseException()
        ElseIf (System.String.IsNullOrEmpty(corner) Or corner.Length > 4) Then
            Log.Error("����corner��4���𒴂��Ă��܂��B") '�����s��
            Throw New DatabaseException()
        ElseIf (System.String.IsNullOrEmpty(model) Or model.Length > 14) Then
            Log.Error("����model��14���𒴂��Ă��܂��B") '�����s��
            Throw New DatabaseException()

        End If

        '�e�[�u��:�@��\���}�X�^
        '�擾����:�\���p���@NO
        '�擾����:�\���p���@NAME
        sSQL = " SELECT MCHN.UNIT_NO AS INDICATION_NO," _
             & " CONVERT(CHAR(8),MCHN.UNIT_NO) AS INDICATION_NAME" _
             & " FROM M_MACHINE MCHN" _
             & " WHERE[RAIL_SECTION_CODE]+[STATION_ORDER_CODE]='" & station & "'" _
             & " AND MCHN.CORNER_CODE = '" & corner & "'" _
             & " AND MCHN.SETTING_START_DATE = (" _
                 & " SELECT MAX(SETTING_START_DATE)" _
                 & " FROM M_MACHINE " _
                 & " WHERE SETTING_START_DATE <= '" & ApplyDate & "' )"

        If model <> "" Then
            '������𕪊�
            strModel = model.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MCHN.MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MCHN.MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ") ORDER BY INDICATION_NO"
        Else
            sSQL = sSQL & " AND MODEL_CODE<>'X' ORDER BY INDICATION_NO"
        End If

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

    ''' <summary>DataTable�̐擪�ɁA�u�S���@�v��ǉ�����B</summary>
    ''' <remarks>
    '''  DataTable�̐擪�ɁA�u�S���@�v��ǉ�����B
    ''' </remarks>
    ''' <returns>�}�X�^�f�[�^�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SetAll() As DataTable

        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'DataTable��MODEL�ɁA�uTERMINAL_ALL�v�ǉ�����B
        drw.Item(0) = DBNull.Value

        'DataTable��MODEL_NAME�ɁA�u�S���@�v��ǉ�����B
        drw.Item(1) = ALL_UNIT

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    ''' <summary>DataTable�̐擪�ɁA�󔒍s��ǉ�����B</summary>
    ''' <returns>���@�}�X�^���</returns>
    Public Function SetSpace() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try

            'DataTable�̐擪�ɁA�󔒍s��ǉ�����B
            For i As Integer = 0 To dt.Columns.Count - 1
                drw.Item(i) = DBNull.Value
            Next

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
                dt.Columns.Add("INDICATION_NO")
                dt.Columns.Add("INDICATION_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
