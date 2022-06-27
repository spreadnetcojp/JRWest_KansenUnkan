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
''' �@��}�X�^���A�l���擾���ADataTable�Ɋi�[����B
''' </summary>
''' <remarks>�N���C�A���g��ʂ̃R���|�[�l���g(ComboBox,ListBox)�ɐݒ肷��}�X�^�f�[�^���擾����B</remarks>
Public Class ModelMaster

    '�S�@��
    Private Const ALL_MODE As String = "�S�@��"

    '�@��}�X�^�擾���ʊi�[�e�[�u��
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

#Region "DB���A�f�[�^���擾����B"
    ''' <summary> DB���A�f�[�^���擾����B</summary>
    ''' <param name="bGetSend">true:PRG���M�Ώێ擾�Afalse:�}�X�^���M�Ώ�</param>
    ''' <returns>�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SelectTable(Optional ByVal bGetSend As Boolean = False) As DataTable

        '�{���\�b�h�̎��s�ɂ��ADataTable dt�͏����������B
        dt = New DataTable

        Dim dbCtl As New DatabaseTalker

        Dim sSQL As String = ""

        '�e�[�u��:�@��}�X�^
        '�擾����:�@��}�X�^�D�@��R�[�h
        '�擾����:�@��}�X�^�D�@�햼
        sSQL = "SELECT MODEL_CODE,MODEL_NAME  FROM M_MODEL"

        If bGetSend Then
            sSQL = sSQL & " WHERE PRG_SND_FLAG = '1'"
        Else
            sSQL = sSQL & " WHERE MST_SND_FLAG = '1'"
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
#End Region

#Region "DB���A�f�[�^���擾����B"
    ''' <summary> DB���A�f�[�^���擾����B</summary>
    ''' <param name="sStation">�w�R�[�h�i�w��R�[�h�{�w���R�[�h�j</param>
    ''' <param name="sCorner">�R�[�i�[�R�[�h</param>
    ''' <param name="bKadoReceive">true:�ғ��f�[�^��M�Ώێ擾</param>
    ''' <param name="bFaultReceive">true:�ُ�f�[�^��M�Ώێ擾</param>
    ''' <returns>�O���[�v�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Public Function SelectTable(ByVal sStation As String, ByVal sCorner As String, _
                                ByVal bKadoReceive As Boolean, Optional ByVal bFaultReceive As Boolean = False) As DataTable

        '�p�����[�^�[���`�F�b�N����
        If String.IsNullOrEmpty(sStation) Then
            Log.Error("����sStation����ł��B") '�����s��
            Throw New DatabaseException()
        End If

        If sStation.Length <> 6 Then
            Log.Error("����sStation��6���ł���܂���B") '�����s��
            Throw New DatabaseException()
        End If

        If String.IsNullOrEmpty(sCorner) Then
            Log.Error("����sCorner����ł��B") '�����s��
            Throw New DatabaseException()
        End If

        '�{���\�b�h�̎��s�ɂ��ADataTable dt�͏����������B
        dt = New DataTable

        Dim dbCtl As DatabaseTalker

        Dim sSQL As String = ""

        dbCtl = New DatabaseTalker

        Try
            '�e�[�u��:�@��}�X�^
            '�e�[�u��:�@��\���}�X�^
            '�擾����:�@��}�X�^�D�@��R�[�h
            '�擾����:�@��}�X�^�D�@�햼
            sSQL = "SELECT DISTINCT MOD.MODEL_CODE AS MODEL, MOD.MODEL_NAME" _
                & " FROM M_MACHINE MAC,M_MODEL MOD" _
                & " WHERE MAC.MODEL_CODE = MOD.MODEL_CODE" _
                & " AND MAC.RAIL_SECTION_CODE+MAC.STATION_ORDER_CODE='" & sStation & "'" _
                & " AND MAC.CORNER_CODE = '" & sCorner & "'" _
                & " AND MAC.SETTING_START_DATE = (SELECT MAX(SETTING_START_DATE)" _
                & " FROM M_MACHINE WHERE SETTING_START_DATE <= '" & ApplyDate & "')"

            If bFaultReceive Then
                sSQL = sSQL & " AND MOD.FAULT_RCV_FLAG = '1'"
            Else
                If bKadoReceive Then
                    sSQL = sSQL & " AND MOD.KADO_RCV_FLAG = '1'"
                End If
            End If

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
    ''' <summary>DataTable�̐擪�ɁA�󔒍s��ǉ�����B</summary>
    ''' <returns>�@��}�X�^���</returns>
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
#End Region

#Region "DataTable�̐擪�ɁA�u�S�@��v��ǉ�����B"
    ''' <summary>DataTable�̐擪�ɁA�u�S�@��v��ǉ�����B</summary>
    ''' <returns>�@��}�X�^���</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        Try
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
            drw.Item(1) = ALL_MODE
            dt.Rows.InsertAt(drw, 0)

        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function
#End Region

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("MODEL")
                dt.Columns.Add("MODEL_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

#Region "�f�B�X�R���X�g���N�^����"
    ''' <summary>
    ''' �f�B�X�R���X�g���N�^����
    ''' </summary>
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#End Region

End Class
