' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �ݗ��^�ǌ����̂��̂��x�[�X�ɍ쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Data.SqlClient

Public Class DatabaseTalker

#Region "�萔��ϐ�"
    Dim DBConnection As New SqlConnection           '�R�l�N�V����
    Dim IsConnected As Boolean                      '�R�l�N�V�����̗L��
    Dim HasTransaction As Boolean                   '�g�����U�N�V�����̗L��
    Dim SQLTran As SqlTransaction                   '�g�����U�N�V����
#End Region

#Region "�v���p�e�B"
    ''' <summary>
    ''' �R�l�N�V�����̏�Ԃ��擾����B
    ''' </summary>
    ''' <value></value>
    ''' <returns>�R�l�N�V�����̏��</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsConnect() As Boolean
        Get
            Return Me.IsConnected
        End Get
    End Property
#End Region

#Region "���\�b�h"
    ''' <summary>
    ''' �R�l�N�V�������擾����B
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ConnectOpen()
        If Me.DBConnection.State = ConnectionState.Open Then
            Exit Sub
        End If

        Log.Debug("Connecting to DB...")

        Dim ServerName As String = BaseConfig.DatabaseServerName
        Dim DatabaseName As String = BaseConfig.DatabaseName
        Dim User As String = BaseConfig.DatabaseUserName
        Dim Password As String = BaseConfig.DatabasePassword

        If Trim(DatabaseName) <> "" And Trim(User) <> "" And Trim(ServerName) <> "" Then
            With Me.DBConnection
                .ConnectionString = String.Format("Server={0};Database={1};UID={2};PWD={3}", ServerName, DatabaseName, User, Password)
                Try
                    .Open()
                    Me.IsConnected = True
                Catch ex As Exception
                    Log.Error(String.Format("Connecting to DB failed." & vbCrLf & "Server is [{0}]. Database is [{1}]. UID is [{2}]. PWD is [{3}].", ServerName, DatabaseName, User, Password))
                    Me.IsConnected = False
                    Throw New DatabaseException(ex)
                End Try
            End With
        Else
            Log.Error(String.Format("Connecting to DB canceled." & vbCrLf & "Server is [{0}]. Database is [{1}]. UID is [{2}]. PWD is [{3}].", ServerName, DatabaseName, User, Password))
            Me.IsConnected = False
        End If
    End Sub

    ''' <summary>
    ''' �R�l�N�V�������N���[�Y����B
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ConnectClose()
        Try
            If IsConnected Then
                'Try to close
                DBConnection.Close()
                Me.IsConnected = False
            End If
        Catch ex As Exception
            If DBConnection.State = ConnectionState.Open Then
                'keep the last state
                Me.IsConnected = True
            End If
            Throw New DatabaseException(ex)
        End Try
    End Sub

    ''' <summary>
    ''' �g�����U�N�V�������J�n����B
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransactionBegin()
        'Begin a new Transaction
        Log.Info("Start transaction...")

        Try
            SQLTran = DBConnection.BeginTransaction
            HasTransaction = True
        Catch ex As Exception
            Throw New DatabaseException(ex)
        End Try
    End Sub

    ''' <summary>
    ''' �g�����U�N�V�������R�~�b�g����B
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransactionCommit()
        'Commit a Transaction
        Try
            SQLTran.Commit()
            HasTransaction = False
        Catch ex As Exception
            Throw New DatabaseException(ex)
        End Try

        Log.Info("Transaction completed.")
    End Sub

    ''' <summary>
    ''' �g�����U�N�V���������[���o�b�N����B
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub TransactionRollBack()
        If Not HasTransaction Then
            Log.Warn("I have no transaction to roll back.")
            Return
        End If

        'Transaction RollBack
        Try
            SQLTran.Rollback()
            HasTransaction = False
        Catch ex As InvalidOperationException
            Log.Warn("The connection is terminated or the transaction has already been rolled back.", ex)
            HasTransaction = False
            Return
        Catch ex As Exception
            Throw New DatabaseException(ex)
        End Try

        Log.Info("Transaction rolled back.")
    End Sub

    ''' <summary>
    ''' �l���擾���邽�߂�SQL�����s����B
    ''' </summary>
    ''' <param name="CommandString">SQL��</param>
    ''' <returns>�擾�����l</returns>
    ''' <remarks>
    ''' �N�G����NULL���擾�����ꍇ��DBNull.Value���A
    ''' �����擾�ł��Ȃ������ꍇ��Nothing��ԋp����B
    ''' </remarks>
    Public Function ExecuteSQLToReadScalar(ByVal CommandString As String) As Object
        Dim cmdSqlCommand As New SqlCommand   'SQL Command
        Dim obj As Object

        Log.Debug(CommandString & "...")

        If Not IsConnected Then
            If Not Log.LoggingDebug Then
                Log.Error(CommandString & "...")
            End If
            Throw New DatabaseException("It's necessary to connect to DB before execute SQL.")
        End If

        'Only the connection has opened, execute the CommandString
        Try
            'Init cmdSqlCommand and Execute CommandString
            With cmdSqlCommand
                .Connection = DBConnection
                .CommandTimeout = BaseConfig.DatabaseReadLimitSeconds
                .CommandType = CommandType.Text
                .CommandText = CommandString
                If Me.HasTransaction Then
                    'Have Transaction
                    .Transaction = Me.SQLTran
                End If
                obj = .ExecuteScalar
            End With
        Catch ex As Exception
            If Not Log.LoggingDebug Then
                Log.Error(CommandString & "...")
            End If
            Throw New DatabaseException(ex)
        End Try

        If obj Is Nothing Then
            Log.Debug("No record read.")
        Else
            Log.Debug("A value [" & obj.ToString() & "] as [" & obj.GetType().ToString() & "] read.")
        End If
        Return obj
    End Function

    ''' <summary>
    ''' �X�V�A�폜�A�}���̂��߂�SQL�����s����B
    ''' </summary>
    ''' <param name="CommandString">SQL��</param>
    ''' <returns>���s����</returns>
    ''' <remarks></remarks>
    Public Function ExecuteSQLToWrite(ByVal CommandString As String) As Integer
        Dim cmdSqlCommand As New SqlCommand   'SQL Command
        Dim nRet As Integer

        Log.Debug(CommandString & "...")

        If Not IsConnected Then
            If Not Log.LoggingDebug Then
                Log.Error(CommandString & "...")
            End If
            Throw New DatabaseException("It's necessary to connect to DB before execute SQL.")
        End If

        'Only the connection has opened, execute the CommandString
        Try
            'Init cmdSqlCommand and Execute CommandString
            With cmdSqlCommand
                .Connection = Me.DBConnection
                .CommandTimeout = BaseConfig.DatabaseWriteLimitSeconds
                .CommandType = CommandType.Text
                .CommandText = CommandString
                If Me.HasTransaction Then
                    'Have Transaction
                    .Transaction = Me.SQLTran
                End If
                nRet = .ExecuteNonQuery
            End With
        Catch ex As Exception
            If Not Log.LoggingDebug Then
                Log.Error(CommandString & "...")
            End If
            Throw New DatabaseException(ex)
        End Try

        Log.Debug(nRet.ToString() & " record(s) will be written.")
        Return nRet
    End Function

    ''' <summary>
    ''' �l�̃R���N�V�������擾���邽�߂�SQL�������s����B
    ''' </summary>
    ''' <param name="CommandString"></param>
    ''' <returns>�f�[�^�e�[�u��</returns>
    ''' <remarks></remarks>
    Public Function ExecuteSQLToRead(ByVal CommandString As String) As DataTable
        Dim cmdSqlCommand As New SqlCommand   'SQL Command
        Dim daAdapter As SqlDataAdapter       'SqlDataAdapter
        Dim dt As New DataTable

        Log.Debug(CommandString & "...")

        If Not IsConnected Then
            If Not Log.LoggingDebug Then
                Log.Error(CommandString & "...")
            End If
            Throw New DatabaseException("It's necessary to connect to DB before execute SQL.")
        End If

        Try
            'Init cmdSqlCommand
            With cmdSqlCommand
                .Connection = Me.DBConnection
                .CommandType = CommandType.Text
                .CommandText = CommandString
                .CommandTimeout = BaseConfig.DatabaseReadLimitSeconds
                If Me.HasTransaction Then
                    'Have Transaction
                    .Transaction = Me.SQLTran
                End If
            End With

            'Init daAdapter
            daAdapter = New SqlDataAdapter(cmdSqlCommand)

            'Execute CommandString and fill result to p_strTableName
            daAdapter.Fill(dt)
        Catch ex As Exception
            If Not Log.LoggingDebug Then
                Log.Error(CommandString & "...")
            End If
            Throw New DatabaseException(ex)
        End Try

        Log.Debug(dt.Rows.Count.ToString() & " record(s) read.")
        Return dt
    End Function

    ''' <summary>
    ''' �X�g�A�h�v���V�[�W�������s����B
    ''' </summary>
    ''' <param name="sCmd">���s������i�����܂�OUT�����s�j</param>
    ''' <param name="bRtn">�߂�l����iint�̂݁j</param>
    ''' <returns>�X�g�A�h�v���V�[�W���̖߂�l</returns>
    ''' <remarks></remarks>
    Public Function ExecuteStoredProcToWrite(ByVal sCmd As String, Optional ByVal bRtn As Boolean = True) As Integer
        Dim oCmd As New SqlCommand
        Dim oParmRet As SqlParameter
        Dim CommandString As String = ""
        Dim nRet As Integer = 0
        Try
            CommandString = "exec "
            If bRtn Then
                CommandString = CommandString & "@PO_RTN = "
            End If
            CommandString = CommandString & sCmd

            Log.Debug(CommandString & "...")

            oCmd.Connection = Me.DBConnection
            oCmd.CommandType = CommandType.Text
            oCmd.CommandText = CommandString
            oCmd.CommandTimeout = BaseConfig.DatabaseWriteLimitSeconds
            If bRtn Then
                oParmRet = oCmd.Parameters.Add(New SqlParameter("PO_RTN", SqlDbType.Int))
                oParmRet.Direction = ParameterDirection.Output
            End If

            '�X�g�A�h�̎��s
            oCmd.ExecuteNonQuery()

            '���ʂ̎擾
            If bRtn Then
                Return CInt(Utility.CNull(oCmd.Parameters("PO_RTN").Value, "0"))
            Else
                Return 0
            End If

        Catch ex As Exception
            If Not Log.LoggingDebug Then
                Log.Error(CommandString & "...")
            End If
            Throw New DatabaseException(ex)
        Finally
            oParmRet = Nothing
            oCmd.Dispose()
            oCmd = Nothing
        End Try
    End Function
#End Region

End Class
