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
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary> マスタデータマスタより、値を取得し、DataTableに格納する。 </summary>
''' <remarks>
''' クライアント画面のコンポーネント(ComboBox,ListBox)に設定するマスタデータを取得する。
''' </remarks>

Public Class MasterMaster
    'マスタデータマスタ取得結果格納テーブル
    Private dt As DataTable

    ''' <summary>DBより、指定した引数に一致するデータを取得する。</summary>
    ''' <remarks>
    '''  DBより、指定した引数に一致するデータを取得する。
    ''' </remarks>
    ''' <param name="model">機種コード</param>
    ''' <param name="bkbn">True:適用リスト名称取得、False:マスタ名称取得</param>
    ''' <returns>マスタデータマスタ取得結果格納テーブル</returns>
    Public Function SelectTable(ByVal model As String, Optional ByVal bkbn As Boolean = False) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker
        Dim strModel() As String
        Dim i As Integer

        'テーブル:マスタデータマスタ,機種別マスタ設定
        '取得項目:マスタ種別
        '取得項目:マスタ名称
        sSQL = "SELECT DATA_KIND,NAME FROM(SELECT DATA_KIND,NAME,MST_NO,row_number()" _
            & " over(partition by MST_NO order by DATA_KIND,NAME) AS RANK FROM M_MST_NAME WHERE USE_FLG='1'"
        If bkbn Then
            sSQL = sSQL & " AND FILE_KBN='LST'"
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
            sSQL = sSQL & ")) AS DA WHERE RANK='1'"
        Else
            sSQL = sSQL & " ) AS DA WHERE RANK='1'"
        End If

        dbCtl = New DatabaseTalker()

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
    ''' <summary>
    ''' DBより、指定した引数に一致するデータを取得する
    ''' </summary>
    ''' <param name="model">機種コード</param>
    ''' <returns>マスタデータマスタ取得結果格納テーブル</returns>
    ''' <remarks>DBより、指定した引数に一致するデータを取得する</remarks>
    Public Function SelectTable2(ByVal model As String) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker

        'パラメーターをチェックする
        If System.String.IsNullOrEmpty(model) Or model.Length > 14 Then
            'TODO: そもそもこの14桁は妥当な仕様なのか？
            Log.Error("引数modelが14桁を超えています。") '引数不正
            Throw New DatabaseException()
        End If

        'テーブル:マスタデータマスタ,機種別マスタ設定
        '取得項目:ファイル区分＋マスタ種別
        '取得項目:マスタ名称
        sSQL = "SELECT FILE_KBN+DATA_KIND AS KIND, NAME FROM M_MST_NAME WHERE" _
             & " MODEL_CODE='" & model & "' AND USE_FLG='1' ORDER BY MST_NO"

        dbCtl = New DatabaseTalker()

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
    ''' <summary>
    ''' DBより、指定した引数に一致するデータを取得する
    ''' </summary>
    ''' <param name="model">機種コード</param>
    ''' <returns>マスタデータマスタ取得結果格納テーブル</returns>
    ''' <remarks>DBより、指定した引数に一致するデータを取得する</remarks>
    Public Function SelectTableShort(ByVal model As String) As DataTable
        Dim sSQL As String
        Dim dbCtl As DatabaseTalker

        'パラメーターをチェックする
        If System.String.IsNullOrEmpty(model) Or model.Length > 14 Then
            Log.Error("引数modelが14桁を超えています。") '引数不正
            Throw New DatabaseException()
        End If

        'テーブル:マスタデータマスタ,機種別マスタ設定
        '取得項目:マスタ種別+マスタ略名
        '取得項目:マスタ名称
        sSQL = "SELECT DATA_KIND+SHORT_NAME AS KIND, NAME FROM M_MST_NAME WHERE" _
             & " FILE_KBN='DAT' AND MODEL_CODE='" & model & "' AND USE_FLG='1' ORDER BY MST_NO"

        dbCtl = New DatabaseTalker()

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
    ''' <remarks>
    '''  DataTableの先頭に、空白行を追加する。
    ''' </remarks>
    ''' <returns>マスタデータマスタ取得結果格納テーブル</returns>
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

    ''' <summary>DataTableの先頭に、「全マスタ」を追加する。</summary>
    ''' <remarks>
    '''  DataTableの先頭に、「全マスタ」を追加する。
    ''' </remarks>
    ''' <returns>マスタデータマスタ取得結果格納テーブル</returns>
    Public Function SetAll() As DataTable
        Dim drw As DataRow

        DtNothingToOneColumn()
        drw = dt.NewRow()
        drw.Item(0) = ClientDaoConstants.TERMINAL_ALL
        drw.Item(1) = "全マスタ"
        dt.Rows.InsertAt(drw, 0)

        Return dt
    End Function

    Private Sub DtNothingToOneColumn()
        Try
            If dt Is Nothing Then
                dt = New DataTable()
                dt.Columns.Add("DATA_KIND")
                dt.Columns.Add("NAME")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
