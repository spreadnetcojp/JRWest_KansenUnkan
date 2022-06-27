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


Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.Common

''' <summary>
''' �N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B
''' </summary>
''' <remarks>�@��\���}�X�^���A�R�[�i�[�����擾���ADataTable�Ɋi�[����B
''' </remarks>
Public Class CornerMaster


    'set �S�R�[�i�[
    Private Const AllConnor As String = "�S�R�[�i�["

    '��}�X�^�擾���ʊi�[�e�[�u��
    Private dt As DataTable

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

    ''' <summary> DB���A�f�[�^���擾����B</summary>
    ''' <param name="station">�w�R�[�h�i�w��R�[�h�{�w���R�[�h�j</param>
    ''' <param name="sModel">�@��R�[�h</param>
    ''' <returns>�O���[�v�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SelectTable(ByVal station As String, ByVal sModel As String) As DataTable
        Dim sSQL As String

        Dim dbCtl As New DatabaseTalker

        Dim strModel() As String
        Dim i As Integer

        '�p�����[�^�[���`�F�b�N����
        If (System.String.IsNullOrEmpty(station)) Then

            '���O�o��
            Log.Error("����station����ł��B") '�����s��

            '��O���ďo���ɖ߂�
            Throw New DatabaseException()

        ElseIf (station.Length <> 6) Then

            '���O�o��
            Log.Error("����station��6���ł���܂���B") '�����s��

            '��O���ďo���ɖ߂�
            Throw New DatabaseException()

        End If

        '�e�[�u��:�@��\���}�X�^
        '�擾����:�@��\���}�X�^�D�R�[�i�[����
        '�擾����:�@��\���}�X�^�D�R�[�i�[�R�[�h

        sSQL = " SELECT DISTINCT CAST(CORNER_CODE AS varchar) AS CORNER_CODE,CORNER_NAME" _
             & " FROM M_MACHINE" _
             & " WHERE SETTING_START_DATE=(SELECT MAX(SETTING_START_DATE)" _
             & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')" _
             & " AND [RAIL_SECTION_CODE]+[STATION_ORDER_CODE]='" & station & "'"
        If sModel <> "" Then
            '������𕪊�
            strModel = sModel.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ") ORDER BY CORNER_CODE"
        Else
            sSQL = sSQL & " AND MODEL_CODE<>'X' ORDER BY CORNER_CODE"
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
    ''' <returns>�R�[�i�[�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SetSpace() As DataTable

        Dim drw As DataRow

        Dim i As Integer

        DtNothingToOneColumn()
        drw = dt.NewRow()

        For i = 0 To dt.Columns.Count - 1
            drw.Item(i) = ""
        Next

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    ''' <summary>DataTable�̐擪�ɁA�u�S�R�[�i�[�v��ǉ�����B</summary>
    ''' <returns>�R�[�i�[�}�X�^���</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
        drw.Item(1) = AllConnor
        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("CORNER_CODE")
                dt.Columns.Add("EXIT_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()

        MyBase.Finalize()

    End Sub
End Class