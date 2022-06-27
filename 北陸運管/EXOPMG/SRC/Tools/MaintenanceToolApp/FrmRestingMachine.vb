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

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DataAccess

Public Class FrmRestingMachine

    ' �C�x���g�����������ꍇ�ɖ������邩�ۂ�
    ' �iTrue:�C�x���g�𖳎�����AFalse:�C�x���g���n���h�����O����j
    Private dontHandleEvent As Boolean

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

    ' ����������R�[�h
    Private sSenkuCode As String = ""
    ' �������w���R�[�h
    Private sEkijunCode As String = ""
    ' �������R�[�i�[�R�[�h
    Private sCornerCode As String = ""
    ' �������@��R�[�h
    Private sKisyuCode As String = ""

    ''' <summary>
    ''' ���㏈��
    ''' </summary>
    Private Sub FrmRestingMachine_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Try
            Me.Cursor = Cursors.WaitCursor

            '�C�x���g�����J�n
            dontHandleEvent = True
            ' �w�R���{�ݒ�
            Me.setCmbEki()
            ' �R�[�i�[�R���{�ݒ�
            Me.setCmbMado(Me.cmbEki.SelectedValue.ToString)
            ' �@��R���{�ݒ�
            Me.setCmbModel()
            '�C�x���g�����I��
            dontHandleEvent = False

            ' datagridview���^�u�L�[�Ŏ��ɔ�����
            Me.dgvGokiList.StandardTab = True

            Me.Cursor = Cursors.Default
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(AlertBoxAttr.OK, Lexis.FormProcAbnormalEnd)
            Me.Close()
        End Try

    End Sub

    ''' <summary>
    ''' �w�R���{�ύX
    ''' </summary>
    Private Sub cmbEki_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbEki.SelectedIndexChanged
        If dontHandleEvent Then Exit Sub
        Try
            Me.Cursor = Cursors.WaitCursor

            ' �R�[�i�[�R���{�ݒ�
            Me.setCmbMado(Me.cmbEki.SelectedValue.ToString)

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(AlertBoxAttr.OK, Lexis.FormProcAbnormalEnd)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' [�w�R���{�ݒ�]
    ''' </summary>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function setCmbEki() As Boolean
        Dim dt As DataTable
        Dim oMst As StationMaster

        oMst = New StationMaster
        oMst.ApplyDate = ApplyDate
        dt = oMst.SelectTable(False, "G,Y")
        dt = oMst.SetAll()
        BaseSetMstDtToCmb(dt, cmbEki)
        cmbEki.SelectedIndex = 0
        If cmbEki.Items.Count <= 0 Then Return False
        Return True
    End Function

    ''' <summary>
    ''' [�R�[�i�[�R���{�ݒ�]
    ''' </summary>
    ''' <param name="Station">�w�R�[�h</param>
    ''' <returns>True:�����AFalse:���s</returns>
    Private Function setCmbMado(ByVal Station As String) As Boolean
        Dim dt As DataTable
        Dim oMst As CornerMaster

        oMst = New CornerMaster
        oMst.ApplyDate = ApplyDate
        If String.IsNullOrEmpty(Station) Then
            Station = ""
        End If
        If Station <> "" And Station <> ClientDaoConstants.TERMINAL_ALL Then
            dt = oMst.SelectTable(Station, "G,Y")
        End If
        dt = oMst.SetAll()
        BaseSetMstDtToCmb(dt, Me.cmbCorner)
        Me.cmbCorner.SelectedIndex = 0
        If Me.cmbCorner.Items.Count <= 0 Then Return False
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
        dt = oMst.SelectTable(False)
        If dt.Rows.Count = 0 Then
            '�@��f�[�^�擾���s
            Return False
        End If
        'dt = oMst.SetSpace()
        dt = oMst.SetAll()

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
    ''' <param name="cmb">�o�C���h���ComboBox</param>
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
        If dontHandleEvent Then Exit Sub

        Dim nRtn As Integer
        Dim dt As New DataTable
        Dim sSql As String = ""
        Dim sSql2 As String = ""
        Dim code1, code2, code3 As String

        Try
            Me.Cursor = Cursors.WaitCursor

            code1 = ""
            code2 = ""
            code3 = ""

            If Not (Me.cmbEki.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code1 = Me.cmbEki.SelectedValue.ToString
            End If

            If Not (Me.cmbCorner.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code2 = Me.cmbCorner.SelectedValue.ToString
            End If

            If Not (Me.cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL) Then
                code3 = Me.cmbModel.SelectedValue.ToString
            End If

            '�f�[�^�擾����
            sSql =
               "select (case when mrm.MODEL_CODE is Null then convert(bit, 'true') else convert(bit, 'false') end) as kado_chk, " & _
               "vmn.BRANCH_OFFICE_CODE as BRANCH_OFFICE_CODE, " & _
               "vmn.RAIL_SECTION_CODE as RAIL_SECTION_CODE, " & _
               "vmn.STATION_ORDER_CODE as STATION_ORDER_CODE, " & _
               "vmn.STATION_NAME as STATION_NAME, " & _
               "vmn.CORNER_CODE as CORNER_CODE, " & _
               "vmn.CORNER_NAME as CORNER_NAME, " & _
               "vmn.MODEL_CODE as MODEL_CODE, " & _
               "vmn.MODEL_NAME as MODEL_NAME, " & _
               "vmn.UNIT_NO as UNIT_NO " & _
               "from V_MACHINE_NOW as vmn " & _
               "left join M_RESTING_MACHINE as mrm " & _
               "on vmn.RAIL_SECTION_CODE = mrm.RAIL_SECTION_CODE " & _
               "and vmn.STATION_ORDER_CODE = mrm.STATION_ORDER_CODE " & _
               "and vmn.CORNER_CODE = mrm.CORNER_CODE " & _
               "and vmn.MODEL_CODE = mrm.MODEL_CODE " & _
               "and vmn.UNIT_NO = mrm.UNIT_NO"

            ' �w��
            If code1 <> "" Then
                sSql2 = " where vmn.RAIL_SECTION_CODE = '" & code1.Substring(0, 3) & "' and vmn.STATION_ORDER_CODE = '" & code1.Substring(3, 3) & "'"
            End If

            ' �R�[�i�[
            If code2 <> "" Then
                If sSql2 = "" Then
                    sSql2 = " where"
                Else
                    sSql2 = sSql2 & " and"
                End If

                sSql2 = sSql2 & " vmn.CORNER_CODE = '" & code2 & "'"
            End If

            ' �@��
            If sSql2 = "" Then
                sSql2 = " where"
            Else
                sSql2 = sSql2 & " and"
            End If

            If code3 <> "" Then
                sSql2 = sSql2 & " vmn.MODEL_CODE = '" & code3 & "'"
            Else
                ' �S�@��
                Dim j, p As Integer
                p = Me.cmbModel.SelectedIndex
                For j = 1 To Me.cmbModel.Items.Count - 1
                    Me.cmbModel.SelectedIndex = j
                    If j = 1 Then
                        sSql2 = sSql2 & " vmn.MODEL_CODE in ('" & Me.cmbModel.SelectedValue.ToString() & "'"
                    Else
                        sSql2 = sSql2 & ", '" & Me.cmbModel.SelectedValue.ToString() & "'"
                    End If
                Next
                sSql2 = sSql2 & ")"
                Me.cmbModel.SelectedIndex = p
            End If

            sSql = sSql & sSql2 & " order by vmn.BRANCH_OFFICE_CODE, vmn.RAIL_SECTION_CODE, vmn.STATION_ORDER_CODE, vmn.CORNER_CODE, vmn.MODEL_CODE, vmn.UNIT_NO"

            nRtn = BaseSqlDataTableFill(sSql, dt)

            'Select Case nRtn
            '   Case -9             '�c�a�I�[�v���G���[
            '       Exit Sub
            '   Case 0              '�Y���Ȃ�
            '       Exit Sub
            '   Case Is > nMaxCount     '�������擾�\����
            '       Exit Sub
            'End Select

            Me.dgvGokiList.Columns.Clear()

            Me.dgvGokiList.RowHeadersVisible = False
            Me.dgvGokiList.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            Me.dgvGokiList.AutoGenerateColumns = True

            If nRtn < 0 Then
                AlertBox.Show(AlertBoxAttr.OK, Lexis.DatabaseOpenErrorOccurred)
            ElseIf nRtn > 0 Then
                Me.dgvGokiList.DataSource = dt

                Me.dgvGokiList.AllowUserToAddRows = False

                Dim dummy As New DataGridViewTextBoxColumn()
                dummy.DataPropertyName = "dummy"
                dummy.Name = ""
                dummy.HeaderText = ""
                Me.dgvGokiList.Columns.Add(dummy)

                Me.dgvGokiList.Columns(0).HeaderText = "�ғ�"
                Me.dgvGokiList.Columns(4).HeaderText = "�w"
                Me.dgvGokiList.Columns(6).HeaderText = "�R�[�i�["
                Me.dgvGokiList.Columns(8).HeaderText = "�@��"
                Me.dgvGokiList.Columns(9).HeaderText = "���@"

                Me.dgvGokiList.Columns(0).ReadOnly = False
                Me.dgvGokiList.Columns(4).ReadOnly = True
                Me.dgvGokiList.Columns(6).ReadOnly = True
                Me.dgvGokiList.Columns(8).ReadOnly = True
                Me.dgvGokiList.Columns(9).ReadOnly = True

                Me.dgvGokiList.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(4).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(6).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(8).SortMode = DataGridViewColumnSortMode.NotSortable
                Me.dgvGokiList.Columns(9).SortMode = DataGridViewColumnSortMode.NotSortable

                Me.dgvGokiList.Columns(0).Width = 40
                Me.dgvGokiList.Columns(1).Visible = False
                Me.dgvGokiList.Columns(2).Visible = False
                Me.dgvGokiList.Columns(3).Visible = False
                Me.dgvGokiList.Columns(4).Width = 150
                Me.dgvGokiList.Columns(5).Visible = False
                Me.dgvGokiList.Columns(6).Width = 150
                Me.dgvGokiList.Columns(7).Visible = False
                Me.dgvGokiList.Columns(8).Width = 150
                Me.dgvGokiList.Columns(9).Width = 40
                Me.dgvGokiList.Columns(9).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                Me.dgvGokiList.Columns(10).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            End If

            ' �������̏���ۑ�
            If code1 <> "" Then
                Me.sSenkuCode = code1.Substring(0, 3)
                Me.sEkijunCode = code1.Substring(3, 3)
            Else
                Me.sSenkuCode = ""
                Me.sEkijunCode = ""
            End If
            Me.sCornerCode = code2
            Me.sKisyuCode = code3

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)        '�������s���O
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnEnter_Click(sender As System.Object, e As System.EventArgs) Handles btnEnter.Click
        Dim line_count As Integer
        Dim i As Integer
        Dim code1, code2, code3, code4, code5 As String
        Dim sSql As String = ""
        Dim sSql2 As String = ""
        Dim dbCtl As DatabaseTalker
        Dim completed As Boolean = False

        dbCtl = New DatabaseTalker

        Try
            '�u�X�V���Ă���낵���ł����H�v
            If AlertBox.Show(AlertBoxAttr.YesNo, Lexis.ReallyUpdate) = DialogResult.No Then
                Exit Sub
            End If

            Me.Cursor = Cursors.WaitCursor

            dbCtl.ConnectOpen()

            dbCtl.TransactionBegin()

            line_count = Me.dgvGokiList.RowCount

            If line_count > 0 Then
                sSql = "delete from M_RESTING_MACHINE"

                If Me.sSenkuCode <> "" Then
                    sSql2 = " where RAIL_SECTION_CODE = '" & Me.sSenkuCode & "' and STATION_ORDER_CODE = '" & Me.sEkijunCode & "'"
                End If

                If Me.sCornerCode <> "" Then
                    If sSql2 = "" Then
                        sSql2 = " where"
                    Else
                        sSql2 = sSql2 & " and"
                    End If

                    sSql2 = sSql2 & " CORNER_CODE = '" & Me.sCornerCode & "'"
                End If

                If Me.sKisyuCode <> "" Then
                    If sSql2 = "" Then
                        sSql2 = " where"
                    Else
                        sSql2 = sSql2 & " and"
                    End If

                    sSql2 = sSql2 & " MODEL_CODE = '" & Me.sKisyuCode & "'"
                End If

                sSql = sSql & sSql2

                dbCtl.ExecuteSQLToWrite(sSql)
            End If

            For i = 0 To line_count - 1 Step 1
                If CBool(Me.dgvGokiList(0, i).Value) = False Then
                    code1 = CStr(Me.dgvGokiList(2, i).Value)
                    code2 = CStr(Me.dgvGokiList(3, i).Value)
                    code3 = CStr(Me.dgvGokiList(5, i).Value)
                    code4 = CStr(Me.dgvGokiList(7, i).Value)
                    code5 = CStr(Me.dgvGokiList(9, i).Value)

                    'Me.saveRestingMachine(code1, code2, code3, code4, code5)

                    sSql =
                     "insert into M_RESTING_MACHINE ( " & _
                     "INSERT_DATE, " & _
                     "INSERT_USER_ID, " & _
                     "INSERT_MACHINE_ID, " & _
                     "UPDATE_DATE, " & _
                     "UPDATE_USER_ID, " & _
                     "UPDATE_MACHINE_ID, " & _
                     "RAIL_SECTION_CODE, " & _
                     "STATION_ORDER_CODE, " & _
                     "CORNER_CODE, " & _
                     "MODEL_CODE, " & _
                     "UNIT_NO " & _
                     ") values ( " & _
                     "GETDATE(), " & _
                     "'TOOL', " & _
                     "'00', " & _
                     "GETDATE(), " & _
                     "'TOOL', " & _
                     "'00', " & _
                     "'" & code1 & "', " & _
                     "'" & code2 & "', " & _
                     "'" & code3 & "', " & _
                     "'" & code4 & "', " & _
                     "'" & code5 & "' )"

                    dbCtl.ExecuteSQLToWrite(sSql)
                End If
            Next

            dbCtl.TransactionCommit()
            completed = True

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
            '�u�X�V����������ɏI�����܂����B�v
            AlertBox.Show(AlertBoxAttr.OK, Lexis.UpdateCompleted)
        End If
    End Sub

End Class
