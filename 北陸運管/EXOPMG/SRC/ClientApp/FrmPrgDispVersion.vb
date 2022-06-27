' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2011/07/20  (NES)�͘e    �V�K�쐬
'   0.1      2014/06/03  (NES)����    �k���Ή��i�^�u�E�{�^���ʒu�ω��j
'   0.2      2015/01/13  (NES)����    �����ΏۊOPG��\���Ή�
' **********************************************************************

Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon
Imports JR.ExOpmg.DataAccess

''' <summary>�v���O�����o�[�W�����\��</summary>
''' <remarks>
''' �^�p�Ǘ��T�[�o�ɂĕێ����Ă���v���O�����o�[�W�����ƒ[���@��ŕێ����Ă���v���O�����o�[�W�������r���A���ق�����ΊY���̉w��ԐF�\������B
'''�u�w�v�{�^�����N���b�N���邱�Ƃɂ��e�w�ɑΉ�����o�[�W�����ڍ׉�ʂ�\������B
''' </remarks>
Public Class FrmPrgDispVersion
    Inherits FrmBase

#Region "�萔�̒�`"
    '�{�^���̍������`����
    Private Const BTNH As Integer = 48
    '�{�^���̕����`����
    Private Const BTNW As Integer = 152
    '�y�[�W���Ƃɕ\������{�^���̐����`����
    Private Const BTNEKI_CNT As Integer = 50
    Friend WithEvents cmbModel As System.Windows.Forms.ComboBox
    Friend WithEvents lblModel As System.Windows.Forms.Label
    '�^�u�y�[�W�̍������`����
    Private Const BTNEKI_TABH As Integer = BTNH * 10

    ''' <summary>
    ''' �l�ύX�ɂ��C�x���g������h���t���O
    ''' �iTrue:�C�x���g��~�AFalse:�C�x���g�����n�j�j
    ''' </summary>
    Private LbEventStop As Boolean

#End Region

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
    Friend WithEvents btnReturn As System.Windows.Forms.Button
    Friend WithEvents tabDspVer As System.Windows.Forms.TabControl
    Friend WithEvents btnGetData As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnReturn = New System.Windows.Forms.Button()
        Me.btnGetData = New System.Windows.Forms.Button()
        Me.tabDspVer = New System.Windows.Forms.TabControl()
        Me.cmbModel = New System.Windows.Forms.ComboBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.pnlBodyBase.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        '
        'pnlBodyBase
        '
        Me.pnlBodyBase.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.pnlBodyBase.Controls.Add(Me.cmbModel)
        Me.pnlBodyBase.Controls.Add(Me.lblModel)
        Me.pnlBodyBase.Controls.Add(Me.tabDspVer)
        Me.pnlBodyBase.Controls.Add(Me.btnReturn)
        Me.pnlBodyBase.Controls.Add(Me.btnGetData)
        '
        'lblToday
        '
        Me.lblToday.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.lblToday.Text = "2013/07/31(��)  11:43"
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnReturn.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnReturn.Location = New System.Drawing.Point(872, 584)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(128, 40)
        Me.btnReturn.TabIndex = 4
        Me.btnReturn.Text = "�I�@��"
        Me.btnReturn.UseVisualStyleBackColor = False
        '
        'btnGetData
        '
        Me.btnGetData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.btnGetData.Font = New System.Drawing.Font("�l�r �S�V�b�N", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.btnGetData.Location = New System.Drawing.Point(872, 520)
        Me.btnGetData.Name = "btnGetData"
        Me.btnGetData.Size = New System.Drawing.Size(128, 40)
        Me.btnGetData.TabIndex = 3
        Me.btnGetData.Text = "�ĕ\��"
        Me.btnGetData.UseVisualStyleBackColor = False
        '
        'tabDspVer
        '
        Me.tabDspVer.Alignment = System.Windows.Forms.TabAlignment.Bottom
        Me.tabDspVer.Font = New System.Drawing.Font("�l�r �S�V�b�N", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.tabDspVer.Location = New System.Drawing.Point(48, 52)
        Me.tabDspVer.Name = "tabDspVer"
        Me.tabDspVer.SelectedIndex = 0
        Me.tabDspVer.Size = New System.Drawing.Size(772, 515)
        Me.tabDspVer.TabIndex = 2
        '
        'cmbModel
        '
        Me.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbModel.Location = New System.Drawing.Point(90, 16)
        Me.cmbModel.Name = "cmbModel"
        Me.cmbModel.Size = New System.Drawing.Size(172, 21)
        Me.cmbModel.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.BackColor = System.Drawing.SystemColors.ControlLight
        Me.lblModel.Font = New System.Drawing.Font("�l�r �S�V�b�N", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblModel.Location = New System.Drawing.Point(50, 19)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(45, 18)
        Me.lblModel.TabIndex = 52
        Me.lblModel.Text = "�@��"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FrmPrgDispVersion
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ClientSize = New System.Drawing.Size(1014, 732)
        Me.Name = "FrmPrgDispVersion"
        Me.Text = "�^�p�[�� "
        Me.pnlBodyBase.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "��ʂ̃f�[�^����������"
    ''' <summary>��ʂ̃f�[�^����������</summary>
    ''' <remarks>
    '''�f�[�^���������A��ʂɕ\������
    ''' </remarks>
    ''' <returns>�f�[�^�����t���O�F�����iTrue�j�A���s�iFalse�j</returns>
    Public Function InitFrmData() As Boolean
        Dim bRtn As Boolean = False
        LbEventStop = True      '�C�x���g�����n�e�e

        Try
            Log.Info("Method started.")

            '�@�햼�̃R���{�{�b�N�X��ݒ肷��B
            If setCmbModel() = False Then Exit Try
            cmbModel.SelectedIndex = 0            '�f�t�H���g�\������

            '-------Ver0.1�@�k���Ή��@MOD START-----------
            '�f�[�^�擾���w�{�^���z�u����ʕ\������
            If reShowSelect() = False Then Exit Try
            '-------Ver0.1�@�k���Ή��@MOD END-----------

            LbEventStop = False '�C�x���g�����n�m

            bRtn = True
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            If bRtn Then
                Log.Info("Method ended.")
            Else
                Log.Error("Method abended.")
                AlertBox.Show(Lexis.FormProcAbnormalEnd)
            End If
        End Try
        Return bRtn
    End Function
#End Region

#Region "�t�H�[�����[�h"

    ''' <summary>�t�H�[�����[�h</summary>
    Private Sub frmPrgDispVersion_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '��ʃ^�C�g��
        lblTitle.Text = "�v���O�����o�[�W�����\��"
        lblTitle.BackColor = Config.BackgroundColor
        lblToday.BackColor = Config.BackgroundColor
        pnlBodyBase.BackColor = Config.BackgroundColor
        lblModel.BackColor = Config.BackgroundColor
        btnGetData.BackColor = Config.ButtonColor
        btnReturn.BackColor = Config.ButtonColor

    End Sub
#End Region

#Region "�R���{�{�b�N�X��ݒ肷��B"
    ''' <summary>
    ''' �@�햼�̃R���{�{�b�N�X��ݒ肷��B
    ''' </summary>
    ''' <returns>�ݒ茋�ʁF�����iTrue�j�A���s�iFalse�j</returns>
    ''' <remarks>�Ǘ����Ă���@�햼�̂̈ꗗ�y�сu�󔒁v��ݒ肷��B</remarks>
    Private Function setCmbModel() As Boolean

        Dim bRtn As Boolean = False
        Dim dt As DataTable
        Dim oMst As New ModelMaster

        Try
            '�@�햼�̃R���{�{�b�N�X�p�̃f�[�^���擾����B
            dt = oMst.SelectTable(True)
            If dt.Rows.Count = 0 Then
                '�@��f�[�^�擾���s
                Return bRtn
            End If
            dt = oMst.SetAll()

            bRtn = BaseSetMstDtToCmb(dt, cmbModel)
            cmbModel.SelectedIndex = -1
            If cmbModel.Items.Count <= 0 Then bRtn = False

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            bRtn = False
        Finally
            oMst = Nothing
            dt = Nothing
        End Try
        Return bRtn

    End Function
#End Region

#Region "�f�[�^�x�[�X�Ɛڑ����āA�Ԃ����ʏW����dt�ɓn��"

    ''' <summary>�f�[�^�x�[�X�Ɛڑ����āA�Ԃ����ʏW����dt�ɓn��</summary>
    Private Function conSql() As DataTable


        Dim sSql As String = ""
        Dim sModel As String = ""
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable
        dbCtl = New DatabaseTalker
        dt = New DataTable

        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            sModel = "G','Y','W"
        Else
            sModel = cmbModel.SelectedValue.ToString
        End If
        '-----Ver0.2�@�����ΏۊOPG��\���Ή��@�@ADD�@START---------------------------------
        Dim CmbModelSql As String = ""
        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            CmbModelSql = " AND ( PRG.MODEL_CODE='W' OR PRG.MODEL_CODE='G' OR ((PRG.MODEL_CODE='Y') AND((PRG.UPDATE_DATE<>'' AND (VERSION1<>'' OR VERSION2<>'')) OR (PRG.UPDATE_DATE='' AND VERSION3<>''))))"
        ElseIf cmbModel.SelectedValue.ToString = "Y" Then
            CmbModelSql = " AND ((PRG.MODEL_CODE='Y') AND((PRG.UPDATE_DATE<>'' AND (VERSION1<>'' OR VERSION2<>'')) OR (PRG.UPDATE_DATE='' AND VERSION3<>'')))"
        End If
        '-----Ver0.2�@�����ΏۊOPG��\���Ή��@�@ADD�@END-----------------------------------
        '-----Ver0.2�@�����ΏۊOPG��\���Ή��@�@MOD�@START---------------------------------
        sSql = "SELECT M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE,MAX(M.STS) AS FLG" _
            & "  FROM" _
            & "      (" _
            & "          SELECT STATION_NAME,MAC.RAIL_SECTION_CODE,MAC.STATION_ORDER_CODE," _
            & "              MAC.MODEL_CODE,MAC.UNIT_NO,ELEMENT_ID," _
            & "              CASE" _
            & "                  WHEN ELEMENT_ID IS NULL THEN '0'" _
            & "                  WHEN VERSION1 = '' THEN '0'" _
            & "                  WHEN (VERSION1 = VERSION3) AND (VERSION2 = '') THEN '0'" _
            & "                  WHEN (VERSION2 = VERSION3) AND (VERSION3 <> '') THEN '1'" _
            & "                  ELSE '2'" _
            & "              END AS STS,UPDATE_DATE" _
            & "          FROM" _
            & "              (" _
            & "                  SELECT STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                      CORNER_CODE,MODEL_CODE,UNIT_NO" _
            & "                  FROM" _
            & "                      V_MACHINE_NOW" _
            & "                  WHERE" _
            & "                      MODEL_CODE IN ('" & sModel & "')" _
            & "              ) AS MAC" _
            & "              LEFT OUTER JOIN" _
            & "                  (" _
            & "                      SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                          CORNER_CODE,UNIT_NO,ELEMENT_ID," _
            & "                          MAX(VERSION1) AS VERSION1," _
            & "                          MAX(VERSION2) AS VERSION2," _
            & "                          MAX(VERSION3) AS VERSION3,MAX(UPDATE_DATE) AS UPDATE_DATE" _
            & "                      FROM" _
            & "                          (" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                                  CORNER_CODE,UNIT_NO,ELEMENT_ID,ELEMENT_VERSION AS VERSION1," _
            & "                                  '' AS VERSION2,'' AS VERSION3, UPDATE_DATE" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_CUR" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,ELEMENT_VERSION AS VERSION2," _
            & "                                  '' AS VERSION3,UPDATE_DATE" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_NEW" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,'' AS VERSION2," _
            & "                                  ELEMENT_VERSION AS VERSION3,'' AS UPDATE_DATE" _
            & "                              FROM" _
            & "                                  S_PRG_VER_INFO_EXPECTED" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                          ) AS PR" _
            & "                      GROUP BY" _
            & "                          MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                          UNIT_NO,ELEMENT_ID" _
            & "                  ) AS PRG" _
            & "              ON  MAC.RAIL_SECTION_CODE = PRG.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE = PRG.STATION_ORDER_CODE" _
            & "              AND MAC.CORNER_CODE = PRG.CORNER_CODE AND MAC.MODEL_CODE = PRG.MODEL_CODE" _
            & "              AND MAC.UNIT_NO = PRG.UNIT_NO" & CmbModelSql _
            & "      ) AS M" _
            & "  GROUP BY" _
            & "      M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE" _
            & "  ORDER BY" _
            & "      M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE"
        '-----Ver0.2�@�����ΏۊOPG��\���Ή��@�@MOD�@END----------------------------------
        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
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

#Region "�o�[�W�����\����ʂ�\������"
    ''' <summary>�o�[�W�����\����ʂ�\������</summary>
    ''' <remarks>
    ''' �f�[�^�x�[�X����f�[�^���擾���Adt,�ɓn���Bdt�̃f�[�^�ɂ���ē��I��tabpage,button,���쐬����
    ''' </remarks>
    Public Function reShow() As Boolean
        Dim bRtn As Boolean = False

        'Tabpage�y�[�W���̃��[�v�ϐ�
        Dim i As Integer = 0
        '�eTabpage�Ƀ{�^�����ʂ̃��[�v�ϐ���\������B
        Dim l As Integer = 0
        '�s��P�ʂƂ��A�{�^���̃��[�v�ϐ���ǉ����A�����{�^���̕�
        Dim j As Integer = 0
        '���P�ʂƂ��A�{�^���̃��[�v�ϐ���ǉ����A�����{�^���̍���
        Dim k As Integer = 0
        '������dt�ɂă{�^�����ʂ̃��[�v�ϐ������[�v����
        Dim t As Integer = 0

        Dim tabEki As TabPage

        '�{�^���̐���
        Dim nBtnNum As Integer = 0
        'tabpage�̐���
        Dim nPage As Integer = 0
        '�eTabpage�{�^���̐���
        Dim nBtnNumPage As Integer = 0

        '�f�[�^�x�[�X���猟�o�������ʏW�����i�[����
        Dim dtDispEki As DataTable = New DataTable

        Try
            '�w�̖��́A�o�[�W��������������
            dtDispEki = Me.conSql()

            If dtDispEki.Rows.Count = 0 Then
                '���������Ɉ�v����f�[�^�͑��݂��Ȃ��B
                If LbEventStop = False Then
                    AlertBox.Show(Lexis.NoRecordsFound)
                End If
                Return bRtn
            End If

            '�{�^���̐���
            nBtnNum = dtDispEki.Rows.Count

            'tabpage�̐���
            nPage = CType(Int(nBtnNum / BTNEKI_CNT), Integer)

            If nBtnNum Mod BTNEKI_CNT <> 0 Then
                nPage = nPage + 1
            End If

            '���[�f�B���Otabpage
            '�^�u�y�[�W�𓮓I�ɐ������AtabDspVer�Ƀ��[�h����B
            For i = 0 To nPage - 1

                tabEki = New TabPage

                'tabEki�̃v���p�e�B��ݒ肷��
                tabEki.Text = getTabTitle(i, nBtnNum, nPage)

                tabEki.BorderStyle = BorderStyle.Fixed3D
                tabEki.Size = New System.Drawing.Size(764, 523)

                'tabDspVer�Ƀ��[�h����
                Me.tabDspVer.Controls.Add(tabEki)

                'i�y�[�W�ڂɃ{�^������ݒ肷��B
                If i <> nPage - 1 Or (i = nPage - 1 And nBtnNum Mod BTNEKI_CNT = 0) Then
                    nBtnNumPage = BTNEKI_CNT - 1
                    '�Ⴕ�A�Ō��tabpage�y�[�W�ł���ꍇ
                ElseIf i = nPage - 1 And nBtnNum Mod BTNEKI_CNT <> 0 Then
                    nBtnNumPage = nBtnNum Mod BTNEKI_CNT - 1
                End If

                j = 0
                k = 0

                '���[�f�B���O�{�^��
                For l = 0 To nBtnNumPage

                    Call Me.addBtnEki(tabEki, j, k, t, dtDispEki)

                    k = k + BTNH
                    t = t + 1
                    '����
                    If (k = BTNEKI_TABH) Then
                        j = j + BTNW
                        k = 0
                    End If
                Next

            Next

            dtDispEki.Dispose()
            bRtn = True
        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        Finally
            dtDispEki = Nothing
        End Try
        Return bRtn

    End Function
#End Region

#Region "tabpage��text�v���p�e�B��ݒ肷��"

    ''' <summary>tabpage��text�v���p�e�B��ݒ肷��</summary>
    ''' <param name="i">Tabpage�y�[�W���̃��[�v�ϐ�</param>
    ''' <param name="nBtnNum">�{�^���̐���</param>
    ''' <param name="nPage">tabpage�̐���</param>
    ''' <remarks>
    ''' tabpages�̃{�^�����̕\���͈͂��m�肷��
    '''  </remarks>
    Private Function getTabTitle(ByVal i As Integer, ByVal nBtnNum As Integer, ByVal nPage As Integer) As String
        Dim sStartText As String = ""
        Dim sEndText As String = ""

        '�^�u����
        sStartText = (BTNEKI_CNT * i + 1).ToString

        If (i = nPage - 1) Then
            sEndText = nBtnNum.ToString
        Else
            sEndText = (BTNEKI_CNT * (i + 1)).ToString
        End If

        'tabpage���x���ɖ{�y�[�W�̃{�^�����͈̔͂�\������B
        Return sStartText & "�`" & sEndText

    End Function
#End Region

#Region "�w�{�^����ǉ�"

    '''<summary> �w�{�^����ǉ� </summary>
    ''' <param name="tab">���[�f�B���O����{�^���̃^�u�y�[�W</param>
    ''' <param name="j">�s��P�ʂƂ��A�{�^���̃��[�v�ϐ���ǉ����A�����{�^���̕�</param>
    ''' <param name="k">���P�ʂƂ��A�{�^���̃��[�v�ϐ���ǉ����A�����{�^���̍���</param>
    ''' <param name="t">������dt�ɂă{�^�����ʂ̃��[�v�ϐ������[�v����</param>
    ''' <param name="dt">�f�[�^�x�[�X���猟�o�������ʏW�����i�[����</param>
    '''<remarks>
    ''' �{�^����V�K�쐬����B�{�^���̃v���p�e�B��ݒ肵�Atabpage�ɒǉ�����B
    ''' </remarks>
    Private Sub addBtnEki(ByVal tab As TabPage, ByVal j As Integer, ByVal k As Integer, ByVal t As Integer, ByVal dt As DataTable)

        Dim btnEki As Button

        btnEki = New Button
        btnEki.Size = New Size(BTNW, BTNH)
        btnEki.Text = dt.Rows(t).Item("STATION_NAME").ToString
        btnEki.Name = dt.Rows(t).Item("RAIL_SECTION_CODE").ToString & dt.Rows(t).Item("STATION_ORDER_CODE").ToString
        btnEki.Tag = dt.Rows(t).Item("STATION_ORDER_CODE").ToString
        btnEki.Location = New Point(j, k)
        btnEki.FlatStyle = FlatStyle.Standard

        '�{�^���w�i�F�iBackColor�j��ݒ肷��
        If (CType(dt.Rows(t).Item("FLG"), Integer) = 0) Then
            btnEki.BackColor = Color.White
        ElseIf (CType(dt.Rows(t).Item("FLG"), Integer) = 1) Then
            btnEki.BackColor = Color.Yellow
        Else
            btnEki.BackColor = Color.Red
        End If

        AddHandler btnEki.Click, AddressOf detail
        tab.Controls.Add(btnEki)

    End Sub
#End Region

#Region "�u�w��ԁv�{�^���N���b�N"
    ''' <summary>�u�w��ԁv�{�^���N���b�N</summary>
    ''' <remarks>
    ''' �w�{�^�����������ꂽ�ꍇ�̏������s��,�e�w�ɑΉ�����o�[�W�����ڍ׉�ʂ�\������B
    ''' </remarks>
    Private Sub detail(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try

            Call waitCursor(True)
            '�w�{�^������
            LogOperation(sender, e)    '�{�^���������O

            Dim oFrmPrgDispVersionDetail As New FrmPrgDispVersionDetail

            oFrmPrgDispVersionDetail.sCmbValue = cmbModel.SelectedIndex
            oFrmPrgDispVersionDetail.sBtnName = CType(sender, Button).Name.Substring(0, 3)
            oFrmPrgDispVersionDetail.sBtnTag = CType(sender, Button).Tag.ToString

            If oFrmPrgDispVersionDetail.InitFrmData() = False Then
                oFrmPrgDispVersionDetail = Nothing
                Exit Sub
            End If

            Me.Hide()
            oFrmPrgDispVersionDetail.ShowDialog()
            oFrmPrgDispVersionDetail.Dispose()
            Me.Show()

        Finally

            Call waitCursor(False)

        End Try
    End Sub
#End Region

#Region "�u�ĕ\���v�{�^���N���b�N"
    ''' <summary>�u�ĕ\���v�{�^���N���b�N</summary>
    ''' <remarks>
    ''' �u�ĕ\���v�{�^�����N���b�N���邱�Ƃɂ��A�e�w�̃o�[�W���������Ď擾���\������B
    ''' </remarks>
    Private Sub btnGetData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetData.Click
        '�ĕ\���{�^������
        LogOperation(sender, e)    '�{�^���������O

        Try

            Call waitCursor(True)
            '�ēx�̃��[�f�B���O��h�����߂�'tabcontrol1���N���A����B
            Me.tabDspVer.TabPages.Clear()

            '-------Ver0.1�@�k���Ή��@MOD START-----------
            '�f�[�^�擾���w�{�^���z�u����ʕ\������
            If reShowSelect() = False Then Exit Try
            '-------Ver0.1�@�k���Ή��@MOD END-----------

        Catch ex As DatabaseException
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.DatabaseSearchErrorOccurred) '�������s���b�Z�[�W

        Finally

            Call waitCursor(False)

        End Try

    End Sub
#End Region

#Region "�u�I���v�{�^���N���b�N"
    ''' <summary>�u�I���v�{�^���N���b�N</summary>
    ''' <remarks >
    ''' ����ʂ��I�����A�u�v���O�����Ǘ����j���[�v��ʂ�\������
    ''' </remarks >
    Private Sub btnReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReturn.Click

        '�I���{�^������
        LogOperation(sender, e)    '�{�^���������O
        Me.Close()

    End Sub
#End Region

    ''' <summary>
    ''' �@��R���{�I���C�x���g
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbModel_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbModel.SelectedIndexChanged
        If LbEventStop Then Exit Sub
        LfWaitCursor()

        Try
            Me.tabDspVer.TabPages.Clear()

            '-------Ver0.1�@�k���Ή��@MOD START-----------
            '�f�[�^�擾���w�{�^���z�u����ʕ\������
            If reShowSelect() = False Then Exit Try
            '-------Ver0.1�@�k���Ή��@MOD END-----------

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.FormProcAbnormalEnd)
        Finally
            LfWaitCursor(False)
        End Try

    End Sub
    '-------Ver0.1�@�k���Ή��@ADD START-----------
#Region "�^�u���擾"
    Private Function getTab_Name() As DataTable
        Dim sSql As String = ""
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable
        dbCtl = New DatabaseTalker
        dt = New DataTable

        sSql = " SELECT DISTINCT TAB_ORDER,TAB_NAME FROM M_TAB_BTN WHERE TAB_NAME <> '' ORDER BY TAB_ORDER,TAB_NAME "

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
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
#Region "�w�{�^�����ςɕ\��"
    'OPT ���g�p
    Private Function reshow2(ByVal dtTab As DataRow, ByVal dtBtn_Idx As DataTable) As Boolean
        Dim bRtn As Boolean = False
        'Tabpage�y�[�W���̃��[�v�ϐ�
        Dim i As Integer = 0
        '�eTabpage�Ƀ{�^�����ʂ̃��[�v�ϐ���\������B
        Dim l As Integer = 0
        '�s�ʒu
        Dim j As Integer = 0
        '��ʒu
        Dim k As Integer = 0
        '������dt�ɂă{�^�����ʂ̃��[�v�ϐ������[�v����
        Dim t As Integer = 0

        Dim tabEki As TabPage

        '�{�^���̐���
        Dim nBtnNum As Integer = 0
        '�eTabpage�{�^���̐���
        Dim nBtnNumPage As Integer = 0

        '�f�[�^�x�[�X���猟�o�������ʏW�����i�[����
        Try
            '�{�^���̐���
            nBtnNum = dtBtn_Idx.Rows.Count

            tabEki = New TabPage

            'tabEki�̃v���p�e�B��ݒ肷��
            tabEki.Text = dtTab.Item("TAB_NAME").ToString

            tabEki.BorderStyle = BorderStyle.Fixed3D
            tabEki.Size = New System.Drawing.Size(764, 523)

            'tabDspVer�Ƀ��[�h����
            Me.tabDspVer.Controls.Add(tabEki)

            '�{�^����ݒ肷��B
            For l = 0 To nBtnNum - 1
                j = getRowPosition(CType(dtBtn_Idx.Rows(l).Item("ROW_ID"), Integer))
                k = getColumnPosition(CType(dtBtn_Idx.Rows(l).Item("COLUMN_ID"), Integer))
                Call Me.addBtnEki(tabEki, k, j, l, dtBtn_Idx)
            Next

            bRtn = True

        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        End Try

        Return bRtn

    End Function
#End Region

#Region "�w�A�{�^���z�u�����擾����"
    Private Function consql2(ByVal TabOrder As Integer, ByVal TabPage As String) As DataTable
        Dim sSql As String = ""
        Dim sModel As String = ""
        Dim dbCtl As DatabaseTalker
        Dim dt As DataTable
        dbCtl = New DatabaseTalker
        dt = New DataTable

        '�u�S�@��v�I���ł����G�F���D�@�AY�F���������@�AW�F�Ď��Ղ��@��ɐݒ�
        If cmbModel.SelectedValue.ToString = ClientDaoConstants.TERMINAL_ALL Then
            sModel = "G','Y','W"
        Else
            sModel = cmbModel.SelectedValue.ToString
        End If

        sSql = "SELECT M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE,MAX(M.STS) AS FLG,TAB_NAME,ROW_ID,COLUMN_ID" _
            & "  FROM" _
            & "      (" _
            & "          SELECT STATION_NAME,MAC.RAIL_SECTION_CODE,MAC.STATION_ORDER_CODE," _
            & "              MAC.MODEL_CODE,MAC.UNIT_NO,ELEMENT_ID," _
            & "              CASE" _
            & "                  WHEN ELEMENT_ID IS NULL THEN '0'" _
            & "                  WHEN VERSION1 = '' THEN '0'" _
            & "                  WHEN (VERSION1 = VERSION3) AND (VERSION2 = '') THEN '0'" _
            & "                  WHEN (VERSION2 = VERSION3) AND (VERSION3 <> '') THEN '1'" _
            & "                  ELSE '2'" _
            & "              END AS STS," _
            & "              TRC.TAB_NAME,TRC.ROW_ID,TRC.COLUMN_ID" _
            & "          FROM" _
            & "              (" _
            & "                  SELECT STATION_NAME,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                      CORNER_CODE,MODEL_CODE,UNIT_NO" _
            & "                  FROM" _
            & "                      V_MACHINE_NOW" _
            & "                  WHERE" _
            & "                      MODEL_CODE IN ('" & sModel & "')" _
            & "              ) AS MAC" _
            & "              LEFT OUTER JOIN" _
            & "                  (" _
            & "                      SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                          CORNER_CODE,UNIT_NO,ELEMENT_ID," _
            & "                          MAX(VERSION1) AS VERSION1," _
            & "                          MAX(VERSION2) AS VERSION2," _
            & "                          MAX(VERSION3) AS VERSION3" _
            & "                      FROM" _
            & "                          (" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE," _
            & "                                  CORNER_CODE,UNIT_NO,ELEMENT_ID,ELEMENT_VERSION AS VERSION1," _
            & "                                  '' AS VERSION2,'' AS VERSION3" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_CUR" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,ELEMENT_VERSION AS VERSION2," _
            & "                                  '' AS VERSION3" _
            & "                              FROM" _
            & "                                  D_PRG_VER_INFO_NEW" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                              UNION" _
            & "                              SELECT MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                                  UNIT_NO,ELEMENT_ID,'' AS VERSION1,'' AS VERSION2," _
            & "                                  ELEMENT_VERSION AS VERSION3" _
            & "                              FROM" _
            & "                                  S_PRG_VER_INFO_EXPECTED" _
            & "                              WHERE" _
            & "                                  MODEL_CODE IN ('" & sModel & "')" _
            & "                          ) AS PR" _
            & "                      GROUP BY" _
            & "                          MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE," _
            & "                          UNIT_NO,ELEMENT_ID" _
            & "                  ) AS PRG" _
            & "              ON  MAC.RAIL_SECTION_CODE = PRG.RAIL_SECTION_CODE AND MAC.STATION_ORDER_CODE = PRG.STATION_ORDER_CODE" _
            & "              AND MAC.CORNER_CODE = PRG.CORNER_CODE AND MAC.MODEL_CODE = PRG.MODEL_CODE" _
            & "              AND MAC.UNIT_NO = PRG.UNIT_NO" _
            & "              LEFT OUTER JOIN" _
            & "                  (" _
            & "                      SELECT TAB_ORDER,TAB_NAME,ROW_ID,COLUMN_ID,RAIL_SECTION_CODE,STATION_ORDER_CODE" _
            & "                      FROM M_TAB_BTN" _
            & "                      WHERE " _
            & "                          RAIL_SECTION_CODE <> ''" _
            & "                      AND STATION_ORDER_CODE <> ''" _
            & "                  ) AS TRC" _
            & "              ON  MAC.RAIL_SECTION_CODE = TRC.RAIL_SECTION_CODE" _
            & "              AND MAC.STATION_ORDER_CODE = TRC.STATION_ORDER_CODE" _
            & "          WHERE" _
            & "              TRC.RAIL_SECTION_CODE <> ''" _
            & "          AND TRC.STATION_ORDER_CODE <> ''" _
            & "          AND TRC.TAB_ORDER = '" & TabOrder & "'" _
            & "          AND TRC.TAB_NAME = '" & TabPage & "'" _
            & "      ) AS M" _
            & "  GROUP BY" _
            & "      M.STATION_NAME,M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE,M.TAB_NAME,M.ROW_ID,M.COLUMN_ID" _
            & "  ORDER BY" _
            & "      M.RAIL_SECTION_CODE,M.STATION_ORDER_CODE"

        Try
            dbCtl.ConnectOpen()
            dt = dbCtl.ExecuteSQLToRead(sSql)
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
#Region "�s�ʒu�Z�o"
    Private Function getRowPosition(ByVal j As Integer) As Integer
        getRowPosition = (j - 1) * BTNH
    End Function
#End Region

#Region "��ʒu�Z�o"
    Private Function getColumnPosition(ByVal k As Integer) As Integer
        getColumnPosition = (k - 1) * BTNW
    End Function
#End Region

#Region "�w�{�^���z�u�ʒu������or�ς�I�����A�o�[�W�����\����ʂ�\������"
    ''' <summary>�w�{�^���z�u�ʒu��I�����o�[�W�����\����ʂ�\������</summary>
    ''' <remarks>
    ''' �����z�u�Ȃ�reShow()���Ăяo��
    ''' �ϔz�u�Ȃ�consql2()�AreShow2()���Ăяo��
    ''' </remarks>
    Public Function reShowSelect() As Boolean
        Dim bRtn As Boolean = False
        Dim dtTab As DataTable
        Dim dtBtn_Idx As DataTable
        Dim i As Integer
        Dim initflg As Boolean = False

        Try
            '�^�u�{�^���}�X�^�ɓo�^������Γo�^���e�ɏ]���ĉw�{�^����z�u����
            dtTab = getTab_Name()
            If dtTab.Rows.Count > 0 Then
                For i = 0 To dtTab.Rows.Count - 1
                    '�^�u���̉w�̃o�[�W�������A�z�u�ʒu�����擾����
                    dtBtn_Idx = consql2(Integer.Parse(dtTab.Rows(i).Item("TAB_ORDER").ToString), dtTab.Rows(i).Item("TAB_NAME").ToString)
                    If dtBtn_Idx.Rows.Count > 0 Then
                        '�w�{�^���z�u
                        If reShow2(dtTab.Rows(i), dtBtn_Idx) = False Then Exit Try
                        initflg = True
                    End If
                Next
                '�z�u�ʒu���P�����܂�Ȃ���Ύ����ŉw�{�^����z�u����
                If initflg = False Then
                    If reShow() = False Then Exit Try
                End If
            Else
                '�^�u�{�^���}�X�^�ɓo�^���Ȃ���Ύ����ŉw�{�^����z�u����
                If reShow() = False Then Exit Try
            End If

            bRtn = True

        Catch ex As DatabaseException
            Throw
        Catch ex As Exception
            Throw New DatabaseException(ex)
        End Try
        Return bRtn

    End Function
#End Region
    '-------Ver0.1�@�k���Ή��@ADD END-----------
End Class
