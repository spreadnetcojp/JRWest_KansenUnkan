' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�ێ�c�[���j
'
'   Copyright Toshiba Solutions Corporation 2014 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2014/04/20  (NES)      �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DataAccess

Public Class FrmNotifiableErrCode

    Private Shared ReadOnly chkNumbRegx As New Regex("^[0-9]+$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
    Private Shared ReadOnly chkDataRegx As New Regex("^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    '�K�p�J�n��
    Private sApplyDate As String = Now.ToString("yyyyMMdd")     '�f�t�H���g���V�X�e�����t�Ƃ���

    '�K�p�J�n��
    Public Property ApplyDate() As String
        Get
            Return sApplyDate
        End Get
        Set(ByVal Value As String)
            sApplyDate = Value
        End Set
    End Property

    ' ������
    Private sSenku() As String
    ' �o�^������
    Private dtSenku As DataTable

    ''' <summary>
    ''' ���㏈��
    ''' </summary>
    Private Sub FrmNotifiableErrCode_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen

        Try
            Me.Cursor = Cursors.WaitCursor

            ' ini�t�@�C����������擾
            Dim i As Integer
            Dim s As String
            Dim sIniFilePath As String = Path.ChangeExtension(Application.ExecutablePath, ".ini")

            i = 0
            Do
                s = Constant.GetIni("Senku", "Senku" & i.ToString(), sIniFilePath)

                If s Is Nothing OrElse s = "" Then Exit Do

                ReDim Preserve Me.sSenku(i)

                Me.sSenku(i) = s

                i = i + 1
            Loop

            ' �w�R���{�ݒ�
            Me.setCmbEki()
            ' �@��R���{�ݒ�
            Me.setCmbModel()

            ' datagridview���^�u�L�[�Ŏ��ɔ�����
            Me.dgvErrCodeList.StandardTab = True

            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(AlertBoxAttr.OK, Lexis.FormProcAbnormalEnd)
            Me.Close()
        End Try

    End Sub

    ''' <summary>
    ''' [�w�R���{�ݒ�]
    ''' </summary>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function setCmbEki() As Boolean
        Dim dt As DataTable
        Dim oMst As StationMaster = New StationMaster
        Dim cname As String()
        Dim i, j, num As Integer

        num = Me.sSenku.Length

        If num > 0 Then
            Me.dtSenku = New DataTable
            Me.dtSenku.Columns.Add("name", GetType(String))
            Me.dtSenku.Columns.Add("code", GetType(String))
        End If

        ' DB�擾
        oMst.ApplyDate = ApplyDate
        dt = oMst.SelectTable(False, "G,W,Y")

        ' ini�t�@�C��senku���
        For i = num - 1 To 0 Step -1

            ' ���悪���݂��邩�H
            For j = 0 To dt.Rows.Count - 1
                If (dt.Rows(j).Item(0).ToString()).Substring(0, 3) = Me.sSenku(i).Substring(0, 3) Then
                    j = -1
                    Exit For
                ElseIf Me.sSenku(i).Substring(0, 3) = "999" Then
                    j = -2
                    Exit For
                End If
            Next

            ' ���݂������̏��ǉ�
            If j = -1 OrElse j = -2 Then
                cname = Me.sSenku(i).Split(","c)

                Me.dtSenku.Rows.Add(cname(2), cname(0))

                Dim s As String = "STATION_NAME = '" & cname(2) & "'"

                If dt.Select(s).Length = 0 Then
                    dt = oMst.SetSpace()
                    dt.Rows(0).Item(0) = cname(0) & cname(1)
                    dt.Rows(0).Item(1) = cname(2)
                End If
            End If
        Next

        ' �S��
        dt = oMst.SetAll()
        dt.Rows(0).Item(1) = "�S��"

        BaseSetMstDtToCmb(dt, cmbEki)
        cmbEki.SelectedIndex = 0

        If cmbEki.Items.Count <= 0 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' �@�햼�̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���@�햼�̂̈ꗗ�y�сu�󔒁v��ݒ肷��B</remarks>
    Private Function setCmbModel() As Boolean
        Dim dt As DataTable
        Dim oMst As New ModelMaster

        '�@�햼�̃R���{�{�b�N�X�p�̃f�[�^���擾����B
        'dt = oMst.SelectTable(True)
        dt = Me.SelectTable()
        If dt.Rows.Count = 0 Then
            '�@��f�[�^�擾���s
            Return False
        End If
        'dt = oMst.SetSpace()
        'dt = oMst.SetAll()
        dt = Me.SetAll(dt)

        BaseSetMstDtToCmb(dt, Me.cmbModel)
        Me.cmbModel.SelectedIndex = 0
        If Me.cmbModel.Items.Count <= 0 Then Return False

        Return True
    End Function

    ''' <summary>
    ''' �[���}�X�^�N���X���ԋp���ꂽ�f�[�^�e�[�u�����R���{�{�b�N�X�̃f�[�^�\�[�X�Ƀo�C���h���A
    ''' �\�����Ɛݒ����ݒ肷��B
    ''' </summary>
    ''' <param name="dt">�o�C���h�pDataTable(Columuns�\���͒[���}�X�^�N���X�ɏ���)</param>
    ''' <param name="cmb">�o�C���h�K�v�̂���ComboBox</param>
    Public Shared Sub BaseSetMstDtToCmb(ByVal dt As DataTable, ByRef cmb As ComboBox)

        cmb.DataSource = Nothing
        '�R���{�{�b�N�X������
        If cmb.Items.Count > 0 Then
            cmb.Items.Clear()
        End If
        'DataSource�̐ݒ�
        cmb.DataSource = dt
        '�\�������o�[�̐ݒ�
        cmb.DisplayMember = dt.Columns(1).ColumnName
        '�o�����[�����o�[�̐ݒ�
        cmb.ValueMember = dt.Columns(0).ColumnName
    End Sub

    ''' <summary>
    ''' �w��Select�������s���ADataTable�ɐݒ�ԋp����B
    ''' �I�[�v���ȊO�̎��s�G���[��OPMGException�𐶐���Throw����B
    ''' </summary>
    ''' <param name="sSql">���s����Select��</param>
    ''' <param name="dt">���s���ʂ��i�[����DataTable</param>
    ''' <returns>����:��������,-9:�I�[�v�����s</returns>
    Public Shared Function BaseSqlDataTableFill(ByVal sSql As String, ByRef dt As DataTable) As Integer
        Dim Cn As SqlClient.SqlConnection
        Dim da As SqlClient.SqlDataAdapter

        '�I�[�v��
        Try
            Log.Debug("Connecting to DB...")
            Cn = New SqlClient.SqlConnection(Utility.GetDbConnectString)
            Cn.Open()
            da = New SqlClient.SqlDataAdapter(sSql, Cn)
            da.SelectCommand.CommandTimeout = Config.DatabaseReadLimitSeconds
            dt = New System.Data.DataTable()
        Catch ex As Exception
            Log.Error("Unwelcome Exception caught.", ex)
            Return -9
        End Try

        '���s
        Dim nCnt As Integer
        Try
            Log.Debug(sSql & "...")
            da.Fill(dt)
            nCnt = dt.Rows.Count
            Cn.Dispose()
            da.Dispose()
        Catch ex As Exception
            If Not Log.LoggingDebug Then
                Log.Error(sSql & "...")
            End If
            Cn.Dispose()
            da.Dispose()
            Throw New OPMGException(ex)
        End Try

        Log.Debug(nCnt.ToString() & " record(s) read.")
        Return nCnt
    End Function

    ''' <summary>
    ''' ���j���[��ʂɖ߂�
    ''' </summary>
    Private Sub btnReturn_Click(sender As System.Object, e As System.EventArgs) Handles btnReturn.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' �����J�n
    ''' </summary>
    Private Sub btnSearch_Click(sender As System.Object, e As System.EventArgs) Handles btnSearch.Click
        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim sSql2 As String = ""
        Dim code1, code2, code3 As String
        Dim senkus As String

        Try
            Me.Cursor = Cursors.WaitCursor

            code1 = ""
            code2 = ""
            code3 = ""

            senkus = ""

            If Not (Me.cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code1 = Me.cmbEki.SelectedValue.ToString
            End If

            If code1 <> "" Then
                If code1.Substring(3, 3) = "999" Then

                    Dim rows() As DataRow = Me.dtSenku.Select("name = '" & Me.cmbEki.Text.ToString & "'")

                    For Each row As DataRow In rows
                        If senkus = "" Then
                            senkus = "'" & row.Item("code").ToString & "'"
                        Else
                            senkus = senkus & ",'" & row.Item("code").ToString & "'"
                        End If
                    Next

                End If
            End If

            If Not (Me.cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code3 = Me.cmbModel.SelectedValue.ToString
            End If

            '�f�[�^�擾����
            sSql =
             "select distinct nec.RAIL_SECTION_CODE, " & _
             "nec.STATION_ORDER_CODE, " & _
             "vmn.STATION_NAME, " & _
             "nec.MODEL_CODE, " & _
             "mdl.MODEL_NAME, " & _
             "nec.ERR_CODE " & _
             "from M_NOTIFIABLE_ERR_CODE as nec " & _
             "left join V_MACHINE_NOW as vmn " & _
             "on nec.RAIL_SECTION_CODE = vmn.RAIL_SECTION_CODE " & _
             "and nec.STATION_ORDER_CODE = vmn.STATION_ORDER_CODE " & _
             "and nec.MODEL_CODE = vmn.MODEL_CODE " & _
             "left join M_MODEL as mdl " & _
             "on nec.MODEL_CODE = mdl.MODEL_CODE"

            If code1 <> "" Then
                '���w�R���{�Łu�S�āv�ȊO�̂��̂��I������Ă���ꍇ�ł���B
                If code1.Substring(3, 3) <> "999" Then
                    '���w�R���{�ŋ�̓I�ȁi�@��\���}�X�^����擾���Ă����j�w���I������Ă���A
                    '���́i�@��\���}�X�^�ɓo�^����Ă���j�w�����u999�v�łȂ��ꍇ�ł���B
                    'NOTE: INI�t�@�C����Senku�Z�N�V�����ɉw�����u999�v�łȂ����R�[�h������
                    '����\���͂Ȃ����̂Ƃ���B
                    sSql2 = " where nec.RAIL_SECTION_CODE = '" & code1.Substring(0, 3) & "' and nec.STATION_ORDER_CODE = '" & code1.Substring(3, 3) & "'"
                Else
                    If senkus = "" Then
                        '���w�R���{�ŋ�̓I�ȁi�@��\���}�X�^����擾���Ă����j�w���I������Ă���A
                        '���́i�@��\���}�X�^�ɓo�^����Ă���j�w�����u999�v�̏ꍇ�ł���B
                        sSql2 = " where nec.RAIL_SECTION_CODE = '" & code1.Substring(0, 3) & "' and nec.STATION_ORDER_CODE = '999'"
                        Log.Warn("�@��\���}�X�^�ɉw����999�̃��R�[�h�����݂��Ă��܂��B")
                    Else
                        '���w�R���{�œ��ʂȈӖ��́iINI�t�@�C����Senku�Z�N�V��������擾���Ă����j
                        '�A�C�e�����I������Ă���ꍇ�ł���B
                        'NOTE: INI�t�@�C����Senku�Z�N�V�����ɓo�^����Ă�����̂Ɠ����̉w��
                        '�@��\���}�X�^�ɓo�^����Ă���\���͂Ȃ����̂Ƃ���B
                        sSql2 = " where nec.RAIL_SECTION_CODE in (" & senkus & ") and nec.STATION_ORDER_CODE = '999'"
                    End If
                End If
            End If

            If code3 <> "" Then
                If sSql2 = "" Then
                    sSql2 = " where"
                Else
                    sSql2 = sSql2 & " and"
                End If

                sSql2 = sSql2 & " nec.MODEL_CODE = '" & code3 & "'"
            End If

            sSql = sSql & sSql2 & " order by ERR_CODE, MODEL_CODE, RAIL_SECTION_CODE, STATION_ORDER_CODE"

            nRtn = BaseSqlDataTableFill(sSql, dt)

            'Select Case nRtn
            '   Case -9             '�c�a�I�[�v���G���[
            '       Exit Sub
            '   Case 0              '�Y���Ȃ�
            '       Exit Sub
            '   Case Is > nMaxCount     '�������擾�\����
            '       Exit Sub
            'End Select

            Me.dgvErrCodeList.Columns.Clear()

            Me.dgvErrCodeList.RowHeadersVisible = False
            Me.dgvErrCodeList.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvErrCodeList.AutoGenerateColumns = True

            If nRtn < 0 Then
                AlertBox.Show(AlertBoxAttr.OK, Lexis.DatabaseOpenErrorOccurred)
            ElseIf nRtn > 0 Then
                Me.dgvErrCodeList.DataSource = dt

                Me.dgvErrCodeList.AllowUserToAddRows = False

                Me.ReplaceUnsolvedNamesInErrCodeList()

                Dim dummy As New DataGridViewTextBoxColumn()
                dummy.DataPropertyName = "dummy"
                dummy.Name = ""
                dummy.HeaderText = ""
                Me.dgvErrCodeList.Columns.Add(dummy)

                Me.dgvErrCodeList.Columns(2).HeaderText = "�w"
                Me.dgvErrCodeList.Columns(4).HeaderText = "�@��"
                Me.dgvErrCodeList.Columns(5).HeaderText = "�G���[�R�[�h"

                Me.dgvErrCodeList.Columns(2).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvErrCodeList.Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvErrCodeList.Columns(5).SortMode = DataGridViewColumnSortMode.NotSortable

                Me.dgvErrCodeList.Columns(0).Visible = False
                Me.dgvErrCodeList.Columns(1).Visible = False
                Me.dgvErrCodeList.Columns(2).Width = 150
                Me.dgvErrCodeList.Columns(3).Visible = False
                Me.dgvErrCodeList.Columns(4).Width = 150
                Me.dgvErrCodeList.Columns(5).Width = 150
                Me.dgvErrCodeList.Columns(6).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' �ꗗ�\�����̖������̉w������������
    ''' </summary>
    Private Sub ReplaceUnsolvedNamesInErrCodeList()
        Dim lineCount As Integer = Me.dgvErrCodeList.Rows.Count
        Dim i As Integer

        For i = 0 To lineCount - 1
            If Me.dgvErrCodeList(2, i).Value Is Nothing OrElse Me.dgvErrCodeList(2, i).Value.ToString() = "" Then
                Dim senk As String = CStr(Me.dgvErrCodeList(0, i).Value)
                Dim ekjn As String = CStr(Me.dgvErrCodeList(1, i).Value)
                Dim name As String = ""
                If ekjn = "999" Then
                    name = Me.GetSenkuName(senk)
                End If
                If name = "" Then
                    name = "[" & senk & ekjn & "]"
                End If
                Me.dgvErrCodeList(2, i).Value = name
            End If
        Next
    End Sub

    ''' <summary>
    ''' ����R�[�h��ini�t�@�C���̐��於�̂ɕϊ�����
    ''' </summary>
    Private Function GetSenkuName(senkuCode As String) As String
        Dim rname As String = ""
        Dim cname As String()
        Dim num As Integer = Me.sSenku.Length
        Dim i As Integer

        For i = 0 To num - 1
            cname = Me.sSenku(i).Split(","c)

            If senkuCode = cname(0) Then
                rname = cname(2)
                Exit For
            End If
        Next

        Return rname
    End Function

    ''' <summary>
    ''' �C���|�[�g
    ''' </summary>
    Private Sub btnImport_Click(sender As System.Object, e As System.EventArgs) Handles btnImport.Click
        Dim ofd As New OpenFileDialog()

        '�_�C�A���O��\������
        If ofd.ShowDialog() = DialogResult.OK Then

            Dim fname As String = ofd.FileName

            '�u�����t�@�C���̓��e�ōX�V���Ă���낵���ł����H�v
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyImport, fname) = DialogResult.No Then
                Exit Sub
            End If

            Dim dbCtl As DatabaseTalker = New DatabaseTalker
            Dim completed As Boolean = False

            Try
                Me.Cursor = Cursors.WaitCursor

                'CSV�t�@�C����ǂݍ���
                Dim csvData As ArrayList = Common.ReadCsv(fname)
                Dim listCount As Integer = csvData.Count
                Dim i As Integer
                Dim data1, data2, data3, data4 As String

                data1 = ""
                data2 = ""
                data3 = ""
                data4 = ""

                Dim sSql As String = ""
                Dim sSql2 As String = ""
                Dim errflg As Integer = 0
                Dim enc As Encoding = Encoding.GetEncoding(932)

                dbCtl.ConnectOpen()
                dbCtl.TransactionBegin()

                sSql = "delete from M_NOTIFIABLE_ERR_CODE"

                dbCtl.ExecuteSQLToWrite(sSql)

                For i = 0 To listCount - 1
                    data1 = Common.ReadStringFromCSV(csvData, i, 0)     ' ����
                    data2 = Common.ReadStringFromCSV(csvData, i, 1)     ' �w��
                    data3 = Common.ReadStringFromCSV(csvData, i, 2)     ' �@��
                    data4 = Common.ReadStringFromCSV(csvData, i, 3)     ' �G���[�R�[�h

                    ' �o�^�f�[�^�`�F�b�N
                    If data1 = "" OrElse data2 = "" OrElse data3 = "" OrElse data4 = "" Then
                        errflg = 1
                        Exit For
                    End If
                    If data1.Length > 3 OrElse _
                       data2.Length > 3 OrElse _
                       data3.Length > 1 OrElse _
                       data4.Length > 6 Then
                        errflg = 2
                        Exit For
                    End If
                    If data1.Length <> enc.GetByteCount(data1) OrElse _
                       data2.Length <> enc.GetByteCount(data2) OrElse _
                       data3.Length <> enc.GetByteCount(data3) OrElse _
                       data4.Length <> enc.GetByteCount(data4) Then
                        errflg = 3
                        Exit For
                    End If
                    If chkNumbRegx.IsMatch(data1) = False OrElse _
                       chkNumbRegx.IsMatch(data2) = False Then
                        errflg = 4
                        Exit For
                    End If
                    If chkDataRegx.IsMatch(data4) = False Then
                        errflg = 5
                        Exit For
                    End If
                    If data3.Equals("G") = False AndAlso _
                       data3.Equals("W") = False AndAlso _
                       data3.Equals("Y") = False Then
                        errflg = 6
                        Exit For
                    End If

                    ' �����炸�C��
                    data1 = data1.PadLeft(3, "0"c)
                    data2 = data2.PadLeft(3, "0"c)
                    data4 = data4.PadLeft(6, "0"c)

                    sSql =
                      "insert into M_NOTIFIABLE_ERR_CODE ( " & _
                      "INSERT_DATE, " & _
                      "INSERT_USER_ID, " & _
                      "INSERT_MACHINE_ID, " & _
                      "UPDATE_DATE, " & _
                      "UPDATE_USER_ID, " & _
                      "UPDATE_MACHINE_ID, " & _
                      "RAIL_SECTION_CODE, " & _
                      "STATION_ORDER_CODE, " & _
                      "MODEL_CODE, " & _
                      "ERR_CODE, " & _
                      "SNMP_SEVERITY " & _
                      ") values ( " & _
                      "GETDATE(), " & _
                      "'TOOL', " & _
                      "'00', " & _
                      "GETDATE(), " & _
                      "'TOOL', " & _
                      "'00', " & _
                      "'" & data1 & "', " & _
                      "'" & data2 & "', " & _
                      "'" & data3 & "', " & _
                      "'" & data4 & "', " & _
                      "'' )"

                    dbCtl.ExecuteSQLToWrite(sSql)
                Next

                If errflg = 1 Then
                    ' �f�[�^�G���[�i���ݒ�j
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr1DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 2 Then
                    ' �f�[�^�G���[�i���I�[�o�[�j
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr2DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 3 Then
                    ' �f�[�^�G���[�i�S�p�����L��j
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr3DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 4 Then
                    ' �f�[�^�G���[�i����w���ɐ����ȊO�j
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr4DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 5 Then
                    ' �f�[�^�G���[�i�s�������j
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr5DetectedOnImport, data1, data2, data3, data4)
                ElseIf errflg = 6 Then
                    ' �f�[�^�G���[�i�@��R�[�h�ُ�j
                    dbCtl.TransactionRollBack()
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DataErr6DetectedOnImport, data1, data2, data3, data4)
                Else
                    ' �o�^�I��
                    dbCtl.TransactionCommit()
                    completed = True
                End If

            Catch ex As Exception
                dbCtl.TransactionRollBack()
                Log.Fatal("Unwelcome Exception caught.", ex)
                '�u�X�V�����Ɏ��s���܂����B�v
                AlertBox.Show(AlertBoxAttr.OK, Lexis.UpdateFailed)
            Finally
                dbCtl.ConnectClose()
                dbCtl = Nothing
                Me.Cursor = Cursors.Default
            End Try

            If completed Then
                AlertBox.Show(AlertBoxAttr.OK, Lexis.UpdateCompleted)
                Me.btnSearch_Click(sender, e)
            End If

        End If
    End Sub

    ''' <summary>
    ''' �G�N�X�|�[�g
    ''' </summary>
    Private Sub btnExport_Click(sender As System.Object, e As System.EventArgs) Handles btnExport.Click
        Dim sfd As New SaveFileDialog()
        sfd.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") & "ErrCode.csv"
        sfd.Filter = "CSV�t�@�C��(*.csv)|*.csv;*.CSV|���ׂẴt�@�C��(*.*)|*.*"

        ' �_�C�A���O��\������
        If sfd.ShowDialog() = DialogResult.OK Then

            Dim fname As String = sfd.FileName

            '�u�����t�@�C���ɕۑ����Ă���낵���ł����H�v
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyExport, fname) = DialogResult.No Then
                Exit Sub
            End If

            Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(932)
            Dim sdata As String
            Dim nRtn As Integer
            Dim dt As New DataTable
            Dim sSql As String = ""
            Dim i As Integer

            Try
                Me.Cursor = Cursors.WaitCursor

                ' �S�f�[�^�擾����
                sSql =
                 "select RAIL_SECTION_CODE, STATION_ORDER_CODE, MODEL_CODE, ERR_CODE " & _
                 "from M_NOTIFIABLE_ERR_CODE " & _
                 "order by ERR_CODE"

                nRtn = BaseSqlDataTableFill(sSql, dt)

                If nRtn < 0 Then
                    '�uDB�ڑ��Ɏ��s���܂����B�v
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.DatabaseOpenErrorOccurred)
                Else
                    Using sw As New System.IO.StreamWriter(fname, False, enc)
                        ' �w�b�_
                        sdata = "#����,�w��,�@��,�G���[�R�[�h"

                        sw.WriteLine(sdata)

                        For i = 0 To nRtn - 1
                            sdata =
                             dt.Rows(i).Item(0).ToString() & "," & _
                             dt.Rows(i).Item(1).ToString() & "," & _
                             dt.Rows(i).Item(2).ToString() & "," & _
                             dt.Rows(i).Item(3).ToString()

                            sw.WriteLine(sdata)
                        Next

                        sw.Flush()
                    End Using

                    '�u�ۑ�����������ɏI�����܂����B�v
                    AlertBox.Show(AlertBoxAttr.OK, Lexis.ExportCompleted)
                End If

            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                '�u�t�@�C���̏������݂Ɏ��s���܂����B�v
                AlertBox.Show(AlertBoxAttr.OK, Lexis.ERR_FILE_WRITE)
            Finally
                Me.Cursor = Cursors.Default
            End Try
        End If
    End Sub

    ''' <summary> DB���A�f�[�^���擾����B</summary>
    ''' <returns>�}�X�^�擾���ʊi�[�e�[�u��</returns>
    Private Function SelectTable() As DataTable
        Dim dt As DataTable = New DataTable
        Dim dbCtl As New DatabaseTalker
        Dim sSQL As String = ""

        '�e�[�u��:�@��}�X�^
        sSQL = "SELECT MODEL_CODE,MODEL_NAME  FROM M_MODEL"

        sSQL = sSQL & " WHERE FAULT_RCV_FLAG = '1'"

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

    ''' <summary>DataTable�̐擪�ɁA�u�S�@��v��ǉ�����B</summary>
    ''' <returns>�@��}�X�^���</returns>
    Private Function SetAll(dt As DataTable) As DataTable
        Dim drw As DataRow

        drw = dt.NewRow()

        Try
            drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
            drw.Item(1) = "�S�@��"
            dt.Rows.InsertAt(drw, 0)

        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            drw = Nothing
        End Try

        Return dt
    End Function

End Class
