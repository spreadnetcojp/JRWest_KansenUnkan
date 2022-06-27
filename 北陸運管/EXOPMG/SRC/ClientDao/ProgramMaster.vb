' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2011/07/20  (NES)河脇    新規作成
' **********************************************************************

Option Strict On
Option Explicit On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>駅グループマスタより、値を取得し、DataTableに格納する。</summary>
''' <remarks>クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。</remarks>
Public Class ProgramMaster

    '全プログラム
    Private Const ALL_MODE As String = "全プログラム"
    'グループマスタ取得結果格納テーブル
    Private dt As DataTable

    ''' <summary>DBより、データを取得する。</summary>
    ''' <param name="model">機種コード</param>
    ''' <param name="bkbn">True:適用リスト名称取得、False:マスタ名称取得</param>
    ''' <returns>プログラムマスタ情報</returns>
    Public Function SelectTable(ByVal model As String, Optional ByVal bkbn As Boolean = False) As DataTable

        Dim sSQL As String = ""
        Dim strModel() As String
        Dim i As Integer
        Dim dbCtl As DatabaseTalker

        If System.String.IsNullOrEmpty(model) Then
            Log.Error("引数modelが空です。") '引数不正
            Throw New DatabaseException()
        End If

        If model.Length > 14 Then
            'TODO: そもそもこの14桁は妥当なのか？
            Log.Error("引数modelが14桁を超えています。") '引数不正
            Throw New DatabaseException()
        End If
        '本メソッドの実行により、DataTable dtは初期化される。
        dt = New DataTable
        dbCtl = New DatabaseTalker

        sSQL = "SELECT FILE_KBN+DATA_KIND AS KIND,NAME FROM M_PRG_NAME WHERE USE_FLG='1' "
        If bkbn Then

        Else
            sSQL = sSQL & " AND FILE_KBN='DAT'"
        End If

        If model <> "" Then
            '文字列を分割
            strModel = model.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ")"
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

    ''' <summary>DBより、データを取得する。</summary>
    ''' <param name="model">機種コード</param>
    ''' <returns>プログラムマスタ情報</returns>
    Public Function SelectTable2(ByVal model As String) As DataTable

        Dim sSQL As String = ""
        Dim strModel() As String
        Dim i As Integer
        Dim dbCtl As DatabaseTalker

        If System.String.IsNullOrEmpty(model) Then
            Log.Error("引数modelが空です。") '引数不正
            Throw New DatabaseException()
        End If

        If model.Length > 14 Then
            'TODO: そもそも
            Log.Error("引数modelが14桁を超えています。") '引数不正
            Throw New DatabaseException()
        End If

        '本メソッドの実行により、DataTable dtは初期化される。
        dt = New DataTable
        dbCtl = New DatabaseTalker

        sSQL = "SELECT MODEL_CODE AS KIND,NAME FROM M_PRG_NAME WHERE USE_FLG='1' AND FILE_KBN='DAT' "

        If model <> "" Then
            '文字列を分割
            strModel = model.Split(CChar(","))
            For i = 0 To strModel.Length - 1
                If i = 0 Then
                    sSQL = sSQL & " AND (MODEL_CODE='" & strModel(i) & "' "
                Else
                    sSQL = sSQL & " OR MODEL_CODE='" & strModel(i) & "' "
                End If
            Next
            sSQL = sSQL & ")"
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

    ''' <summary>DataTableの先頭に、空白行を追加する。</summary>
    ''' <returns>プログラムマスタ取得結果格納テーブル</returns>
    Public Function SetSpace() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'DataTableの先頭に、空白行を追加する。

        For i As Integer = 0 To dt.Columns.Count - 1
            drw.Item(i) = ""
        Next

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function

    '''<summary>DataTableの先頭に、「全プログラム」を追加する。</summary>
    '''<returns>プログラムマスタ情報</returns>
    Public Function SetAll() As DataTable

        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()

        'DataTableのMODELに、「TERMINAL_ALL」追加する。
        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL

        'DataTableのMODEL_NAMEに、「全プログラム」を追加する。
        drw.Item(1) = ALL_MODE

        dt.Rows.InsertAt(drw, 0)

        Return dt

    End Function


    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("PRG_KIND")
                dt.Columns.Add("PRG_NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
