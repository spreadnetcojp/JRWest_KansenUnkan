' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2013/04/01  (NES)小林  在来運管向けのものをベースに作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Data.SqlClient

Public Class DatabaseTalker

#Region "定数や変数"
    Dim DBConnection As New SqlConnection           'コネクション
    Dim IsConnected As Boolean                      'コネクションの有無
    Dim HasTransaction As Boolean                   'トランザクションの有無
    Dim SQLTran As SqlTransaction                   'トランザクション
#End Region

#Region "プロパティ"
    ''' <summary>
    ''' コネクションの状態を取得する。
    ''' </summary>
    ''' <value></value>
    ''' <returns>コネクションの状態</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsConnect() As Boolean
        Get
            Return Me.IsConnected
        End Get
    End Property
#End Region

#Region "メソッド"
    ''' <summary>
    ''' コネクションを取得する。
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
    ''' コネクションをクローズする。
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
    ''' トランザクションを開始する。
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
    ''' トランザクションをコミットする。
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
    ''' トランザクションをロールバックする。
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
    ''' 値を取得するためのSQLを実行する。
    ''' </summary>
    ''' <param name="CommandString">SQL文</param>
    ''' <returns>取得した値</returns>
    ''' <remarks>
    ''' クエリでNULLを取得した場合はDBNull.Valueを、
    ''' 何も取得できなかった場合はNothingを返却する。
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
    ''' 更新、削除、挿入のためのSQLを実行する。
    ''' </summary>
    ''' <param name="CommandString">SQL文</param>
    ''' <returns>実行結果</returns>
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
    ''' 値のコレクションを取得するためのSQL文を実行する。
    ''' </summary>
    ''' <param name="CommandString"></param>
    ''' <returns>データテーブル</returns>
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
    ''' ストアドプロシージャを実行する。
    ''' </summary>
    ''' <param name="sCmd">実行文字列（引数含むOUT引数不可）</param>
    ''' <param name="bRtn">戻り値あり（intのみ）</param>
    ''' <returns>ストアドプロシージャの戻り値</returns>
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

            'ストアドの実行
            oCmd.ExecuteNonQuery()

            '結果の取得
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
