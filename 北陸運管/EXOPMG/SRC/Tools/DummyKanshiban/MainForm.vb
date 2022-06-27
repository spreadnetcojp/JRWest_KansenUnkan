' **********************************************************************
'   システム名：新幹線自動改札システム（運用管理サーバ／端末）
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   変更履歴:
'   Ver      日付        担当       コメント
'   0.0      2015/02/16  (NES)小林  新規作成
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Messaging
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.Telegramming

#Const AcceptsSameNameMasOfSameHashValue = True
#Const AcceptsSameNameProOfSameHashValue = True

Public Class MainForm
    Protected OptionalWriter As LogToOptionalDelegate
    Protected oLogDispStorage As DataTable
    Protected oLogDispBinder As BindingSource
    Protected oLogDispFilterEditDialog As LogDispFilterEditDialog = Nothing

    Protected Structure MasProId
        Public DataSubKind As Integer
        Public DataVersion As Integer
        Public DataHashValue As String

        Public Sub New(ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal sDataHashValue As String)
            Me.DataSubKind = dataSubKind
            Me.DataVersion = dataVersion
            Me.DataHashValue = sDataHashValue
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj.GetType() IsNot GetType(MasProId) Then Return False
            Dim oId As MasProId = CType(obj, MasProId)
            If DataSubKind <> oId.DataSubKind Then Return False
            If DataVersion <> oId.DataVersion Then Return False
            If StringComparer.OrdinalIgnoreCase.Compare(DataHashValue, oId.DataHashValue) <> 0 Then Return False
            Return True
        End Function

        Public Shared Operator =(ByVal a As MasProId, ByVal b As MasProId) As Boolean
            Return a.Equals(b)
        End Operator

        Public Shared Operator <>(ByVal a As MasProId, ByVal b As MasProId) As Boolean
            Return Not a.Equals(b)
        End Operator
    End Structure

    Protected Structure GateProgramContent
        Dim RunnableDate As String
        Dim ModuleInfos As ProgramModuleInfo()
        Dim ArchiveCatalog As String
        Dim VersionListData As Byte()
    End Structure

    Protected Structure KsbProgramContent
        Dim RunnableDate As String
        Dim ArchiveCatalog As String
        Dim VersionListData As Byte()
    End Structure

    Protected Const MachineDirFormat As String = "%3R%3S_%4C_%2U"
    Protected Const MachineDirPattern As String = "??????_????_??"
    Protected Shared ReadOnly MachineDirRegx As New Regex("^[0-9]{6}_[0-9]{4}_[0-9]{2}$", RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Public UiState As UiStateClass
    Protected TelegGene As EkTelegramGene
    Protected TelegImporter As EkTelegramImporter
    Protected Table1 As DataTable
    Protected Table2 As DataTable
    Protected Friend WithEvents InputQueue As MessageQueue = Nothing
    Protected Friend MasProDataFormDic As Dictionary(Of String, Form)
    Protected Friend MasProListFormDic As Dictionary(Of String, Form)
    Protected Friend FaultDataFormDic As Dictionary(Of String, FaultDataForm)
    Protected Friend KadoDataFormDic As Dictionary(Of String, KadoDataForm)

    Protected Rand As Random

    Protected Shared Function GetMachineProfileTable(ByVal sPath As String) As DataTable
        Dim dt As New DataTable("MachineProfile")
        Dim n As Integer = Config.MachineProfileFieldNames.Length
        For i As Integer = 0 To n - 1
            dt.Columns.Add(Config.MachineProfileFieldNames(i), Config.FieldNamesTypes(Config.MachineProfileFieldNames(i)))
        Next i

        Using sr As StreamReader = New StreamReader(sPath, Encoding.Default)
            Dim line As String = sr.ReadLine()
            Dim lineCount As Integer = 1
            While line IsNot Nothing
                Dim columns As String() = line.Split(","c)
                If columns.Count <> n Then
                    Throw New OPMGException("機器構成の" & lineCount.ToString() & "行目のカラム数が不正です。")
                End If

                dt.Rows.Add(columns)

                line = sr.ReadLine()
                lineCount += 1
            End While
        End Using

        Return dt
    End Function

    Protected Shared Function GetMachineProfile(ByVal oProfileTableRow As DataRow) As Object()
        Dim oProfile As Object() = New Object(Config.MachineProfileFieldNames.Length - 1) {}
        For i As Integer = 0 To Config.MachineProfileFieldNames.Length - 1
            oProfile(i) = oProfileTableRow(i)
        Next i
        Return oProfile
    End Function

    Protected Shared Function GetMachineId(ByVal oProfileTableRow As DataRow) As String
        Dim sId As String = _
           oProfileTableRow.Field(Of String)("MODEL_CODE") & "_" & _
           oProfileTableRow.Field(Of String)("RAIL_SECTION_CODE") & _
           oProfileTableRow.Field(Of String)("STATION_ORDER_CODE") & "_" & _
           oProfileTableRow.Field(Of Integer)("CORNER_CODE").ToString("D4") & "_" & _
           oProfileTableRow.Field(Of Integer)("UNIT_NO").ToString("D2")
        Return sId
    End Function

    Public Shared Function GetMachineId(ByVal sModel As String, ByVal sStation As String, ByVal sCorner As String, ByVal sUnit As String) As String
        Return sModel & "_" & sStation & "_" & sCorner & "_" & sUnit
    End Function

    Public Shared Function GetMachineId(ByVal sMachineDirName As String) As String
        Return Config.ModelSym & "_" & sMachineDirName
    End Function

    Public Shared Function GetMachineDirNameOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2)
    End Function

    Public Shared Function GetModelOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(0, 1)
    End Function

    Public Shared Function GetHypStationOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2, 3) & "-" & sMachineId.Substring(5, 3)
    End Function

    Public Shared Function GetStationOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(2, 6)
    End Function

    Public Shared Function GetCornerOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(9, 4)
    End Function

    Public Shared Function GetUnitOf(ByVal sMachineId As String) As String
        Return sMachineId.Substring(14, 2)
    End Function

    Public Shared Function GetEkCodeOf(ByVal sMachineId As String) As EkCode
        Return EkCode.Parse(sMachineId.Substring(2), MachineDirFormat)
    End Function

    Protected Shared Sub DeleteFiles(ByVal sMachineId As String, ByVal sDirPath As String, ByVal sFileNamePattern As String, Optional ByVal oFileNameRegx As Regex = Nothing)
        Try
            For Each sFilePath As String In Directory.GetFiles(sDirPath, sFileNamePattern)
                If oFileNameRegx IsNot Nothing Then
                    Dim sFileName As String = Path.GetFileName(sFilePath)
                    If oFileNameRegx.IsMatch(sFileName) Then
                        File.Delete(sFilePath)
                        Log.Info(sMachineId, "ファイル [" & sFilePath & "] を削除しました。")
                    End If
                Else
                    File.Delete(sFilePath)
                    Log.Info(sMachineId, "ファイル [" & sFilePath & "] を削除しました。")
                End If
            Next sFilePath
        Catch ex As Exception
            Log.Error(sMachineId, "Exception caught.", ex)
        End Try
    End Sub

    Protected Shared Function GetDummyModuleInfos() As ProgramModuleInfo()
        Dim oInfos As ProgramModuleInfo() = New ProgramModuleInfo(ExConstants.GateProgramModuleNamesInCab.Length - 1) {}
        For i As Integer = 0 To ExConstants.GateProgramModuleNamesInCab.Length - 1
            oInfos(i).Elements = New ProgramElementInfo(-1) {}
        Next i
        Return oInfos
    End Function

    Protected Shared Function ExtractGateProgramCab(ByVal sFilePath As String, ByVal sTempDirPath As String) As GateProgramContent
        Dim ret As GateProgramContent
        Utility.DeleteTemporalDirectory(sTempDirPath)
        Directory.CreateDirectory(sTempDirPath)
        Try
            'CABを展開する。
            Using oProcess As New System.Diagnostics.Process()
                oProcess.StartInfo.FileName = Path.Combine(Application.StartupPath, "TsbCab.exe")
                oProcess.StartInfo.Arguments = "-x """ & sFilePath & """ """ & sTempDirPath & "\"""
                oProcess.StartInfo.UseShellExecute = False
                oProcess.StartInfo.RedirectStandardInput = True
                oProcess.StartInfo.CreateNoWindow = True
                oProcess.Start()
                Dim oStreamWriter As StreamWriter = oProcess.StandardInput
                oStreamWriter.WriteLine("")
                oStreamWriter.Close()
                oProcess.WaitForExit()
            End Using

            'プログラムバージョンリストを解析する。
            Dim sVerListPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.GateProgramVersionListPathInCab)
            Try
                Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                    'ファイルのレングスを取得する。
                    Dim len As Integer = CInt(oInputStream.Length)
                    If len < ProgramVersionListUtil.RecordLengthInBytes Then
                        Throw New OPMGException("バージョンリストのサイズが異常です。")
                    End If
                    'ファイルを読み込む。
                    ret.VersionListData = New Byte(len - 1) {}
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(ret.VersionListData, pos, len - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Throw New OPMGException("バージョンリストの読み込みで異常が発生しました。", ex)
            End Try

            ret.RunnableDate = ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム動作許可日", ret.VersionListData)
            Dim oRunnableDate As DateTime
            If DateTime.TryParseExact(ret.RunnableDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oRunnableDate) = False Then
                Throw New OPMGException("バージョンリストに記載された動作許可日が異常です。" & vbCrLf & "動作許可日: " & ret.RunnableDate)
            End If

            '全てのプログラムグループのベースパスと、
            '各グループのディレクトリ名の配列を生成する。
            'TODO: 監視盤CABを改札機CABと同じ方法で処理する場合は
            'Configに監視盤のProgramGroupに関するフィールドを用意し、
            'その参照をここで下記変数にセットすること。
            Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.GateProgramModuleBasePathInCab)

            'CAB内の所定ディレクトリを順に処理する。
            ret.ModuleInfos = New ProgramModuleInfo(ExConstants.GateProgramModuleNamesInCab.Length - 1) {}
            For i As Integer = 0 To ExConstants.GateProgramModuleNamesInCab.Length - 1
                Dim oElems As New List(Of ProgramElementInfo)
                Dim sModName As String = ExConstants.GateProgramModuleNamesInCab(i)
                Dim sModDirPath As String = Path.Combine(sBaseDirPath, sModName)
                Dim sLine As String

                'ディレクトリ内にある見出しファイルを解析する。
                'NOTE: このアプリでは、CABそのものを持ち続けたくないために、監視盤が受信したこの時点で、
                'モジュールの情報を抽出する。そして、CABに異常があれば、抽出の過程で行き詰るため、
                '図らずも監視盤へのDLLシーケンスの段階で異常を伝えることになる。
                'しかし、本物の監視盤がそうであるとは限らない。
                Using oReader As StreamReader _
                   = New StreamReader(Path.Combine(sModDirPath, ExConstants.GateProgramModuleCatalogFileNameInCab), Encoding.GetEncoding(932))

                    '見出しファイルの各行を処理する。
                    Dim lineNumber As Integer = 1
                    sLine = oReader.ReadLine()
                    While sLine IsNot Nothing
                        If Not sLine.StartsWith("/", StringComparison.Ordinal) Then
                            '見出しファイルの非コメント行からバージョン管理対象となるファイルの名前を取得する。
                            Dim sElementFileName As String = sLine.Substring(2, 16).TrimEnd(Chr(&H20))
                            If Not Path.GetFileName(sElementFileName).Equals(sElementFileName, StringComparison.OrdinalIgnoreCase) Then
                                Throw New OPMGException("[" & Path.Combine(sModName, ExConstants.GateProgramModuleCatalogFileNameInCab) & "] " & lineNumber.ToString() & "行目のファイル名 [" & sElementFileName  & "] が不正です。")
                            End If

                            'ファイルのフッタを読み出す。
                            Dim sElementFilePath As String = Path.Combine(sModDirPath, sElementFileName)
                            Dim oFooter As ExProgramElementFooterForG
                            Try
                                oFooter = New ExProgramElementFooterForG(sElementFilePath)
                            Catch ex As Exception
                                Throw New OPMGException("[" & Path.Combine(sModName, sElementFileName) & "] のフッタ読み込みで異常が発生しました。", ex)
                            End Try

                            '読み出したフッタの書式をチェックする。
                            Dim sFooterViolation As String = oFooter.GetFormatViolation()
                            If sFooterViolation IsNot Nothing Then
                                Throw New OPMGException("[" & Path.Combine(sModName, sElementFileName) & "] のフッタ書式が異常です。" & vbCrLf & sFooterViolation)
                            End If

                            Dim elem As ProgramElementInfo
                            elem.FileName = sElementFileName
                            elem.DispData = oFooter.Data
                            oElems.Add(elem)
                        End If

                        sLine = oReader.ReadLine()
                        lineNumber += 1
                    End While
                End Using
                ret.ModuleInfos(i).Elements = oElems.ToArray()
            Next i
        Finally
            Utility.DeleteTemporalDirectory(sTempDirPath)
        End Try

        'CAB内のファイル一覧を取得する。
        Using oProcess As New System.Diagnostics.Process()
            'NOTE: TsbCab -l は、コマンド引数に渡すCABファイルのパスに
            '多バイト文字が含まれているとクラッシュするようであるため、
            'WorkingDirectoryを当該ファイルのあるディレクトリにすることで、
            'コマンド引数にはファイル名のみを渡すことにする。
            oProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(sFilePath)
            oProcess.StartInfo.FileName = Path.Combine(Application.StartupPath, "TsbCab.exe")
            oProcess.StartInfo.Arguments = "-l """ & Path.GetFileName(sFilePath) & """"
            oProcess.StartInfo.UseShellExecute = False
            oProcess.StartInfo.RedirectStandardInput = True
            oProcess.StartInfo.RedirectStandardOutput = True
            oProcess.StartInfo.CreateNoWindow = True
            oProcess.Start()
            Dim oStreamWriter As StreamWriter = oProcess.StandardInput
            oStreamWriter.WriteLine("")
            ret.ArchiveCatalog = oProcess.StandardOutput.ReadToEnd()
            oStreamWriter.Close()
            oProcess.WaitForExit()
        End Using

        Return ret
    End Function

    Protected Shared Function ExtractKsbProgramCab(ByVal sFilePath As String, ByVal sTempDirPath As String) As KsbProgramContent
        Dim ret As KsbProgramContent
        Utility.DeleteTemporalDirectory(sTempDirPath)
        Directory.CreateDirectory(sTempDirPath)
        Try
            'CABを展開する。
            Using oProcess As New System.Diagnostics.Process()
                oProcess.StartInfo.FileName = Path.Combine(Application.StartupPath, "TsbCab.exe")
                oProcess.StartInfo.Arguments = "-x """ & sFilePath & """ """ & sTempDirPath & "\"""
                oProcess.StartInfo.UseShellExecute = False
                oProcess.StartInfo.RedirectStandardInput = True
                oProcess.StartInfo.CreateNoWindow = True
                oProcess.Start()
                Dim oStreamWriter As StreamWriter = oProcess.StandardInput
                oStreamWriter.WriteLine("")
                oStreamWriter.Close()
                oProcess.WaitForExit()
            End Using

            'プログラムバージョンリストを解析する。
            Dim sVerListPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.KsbProgramVersionListPathInCab)
            Try
                Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                    'ファイルのレングスを取得する。
                    Dim len As Integer = CInt(oInputStream.Length)
                    If len < ProgramVersionListUtil.RecordLengthInBytes Then
                        Throw New OPMGException("バージョンリストのサイズが異常です。")
                    End If
                    'ファイルを読み込む。
                    ret.VersionListData = New Byte(len - 1) {}
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(ret.VersionListData, pos, len - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Throw New OPMGException("バージョンリストの読み込みで異常が発生しました。", ex)
            End Try

            ret.RunnableDate = ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム動作許可日", ret.VersionListData)
            Dim oRunnableDate As DateTime
            If DateTime.TryParseExact(ret.RunnableDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oRunnableDate) = False Then
                Throw New OPMGException("バージョンリストに記載された動作許可日が異常です。" & vbCrLf & "動作許可日: " & ret.RunnableDate)
            End If
        Finally
            Utility.DeleteTemporalDirectory(sTempDirPath)
        End Try

        'CAB内のファイル一覧を取得する。
        Using oProcess As New System.Diagnostics.Process()
            'NOTE: TsbCab -l は、コマンド引数に渡すCABファイルのパスに
            '多バイト文字が含まれているとクラッシュするようであるため、
            'WorkingDirectoryを当該ファイルのあるディレクトリにすることで、
            'コマンド引数にはファイル名のみを渡すことにする。
            oProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(sFilePath)
            oProcess.StartInfo.FileName = Path.Combine(Application.StartupPath, "TsbCab.exe")
            oProcess.StartInfo.Arguments = "-l """ & Path.GetFileName(sFilePath) & """"
            oProcess.StartInfo.UseShellExecute = False
            oProcess.StartInfo.RedirectStandardInput = True
            oProcess.StartInfo.RedirectStandardOutput = True
            oProcess.StartInfo.CreateNoWindow = True
            oProcess.Start()
            Dim oStreamWriter As StreamWriter = oProcess.StandardInput
            oStreamWriter.WriteLine("")
            ret.ArchiveCatalog = oProcess.StandardOutput.ReadToEnd()
            oStreamWriter.Close()
            oProcess.WaitForExit()
        End Using

        Return ret
    End Function

    'NOTE: ログ出力毎に呼ばれるので、これの中でログを出力してはならない。
    Protected Sub BeginFetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)
        Try
            'OPT: 上記が守られる限りはデッドロックもないと思われるので、
            'BeginInvoke()ではなく、Invoke()でもよいかもしれない。
            'Invoke()ならば、メッセージキューがあふれる心配もない。
            BeginInvoke( _
                OptionalWriter, _
                New Object() {number, sSecondName, sDateTime, sKind, sClassName, sMethodName, sText})
        Catch ex As Exception
            'NOTE: このControlが破棄された後にこのメソッドが呼び出される万が一の場合を想定している。
            'この後の（このデリゲートに依存しない）処理を通常通り行うよう、例外は握りつぶす。
        End Try
    End Sub

    'NOTE: ログ出力毎に呼ばれるので、これの中でログを出力してはならない。
    Protected Sub FetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)

        If LogDispCheckBox.Checked Then
            If oLogDispStorage.Rows.Count > Config.LogDispMaxRowsCount Then
                oLogDispStorage.Rows.Remove(oLogDispStorage.Rows(0))
            End If

            Dim oRow As DataRow = oLogDispStorage.NewRow()
            oRow(0) = sDateTime
            oRow(1) = sSecondName
            oRow(2) = sKind
            oRow(3) = sText
            oLogDispStorage.Rows.Add(oRow)

            Dim nDispRows As Integer = LogDispGrid.Rows.Count
            If nDispRows <> 0 Then
                LogDispGrid.FirstDisplayedScrollingRowIndex = nDispRows - 1
            End If
        End If
    End Sub

    Private Sub LogDispGrid_CellFormatting(sender As System.Object, e As DataGridViewCellFormattingEventArgs) Handles LogDispGrid.CellFormatting
        If e.ColumnIndex = 3 Then
            Dim dgv As DataGridView = DirectCast(sender, DataGridView)
            Dim k As String = DirectCast(dgv.Rows(e.RowIndex).Cells(2).Value, String)
            Select Case k
                Case "[INFO]"
                    e.CellStyle.ForeColor = Color.RoyalBlue
                    e.CellStyle.SelectionForeColor = Color.RoyalBlue
                Case "[WARN]"
                    e.CellStyle.ForeColor = Color.Fuchsia
                    e.CellStyle.SelectionForeColor = Color.Fuchsia
                Case "[ERROR]"
                    e.CellStyle.ForeColor = Color.Red
                    e.CellStyle.SelectionForeColor = Color.Red
                Case "[FATAL]"
                    e.CellStyle.ForeColor = Color.DarkOrange
                    e.CellStyle.SelectionForeColor = Color.DarkOrange
                Case Else
                    e.CellStyle.ForeColor = Color.DarkGray
                    e.CellStyle.SelectionForeColor = Color.DarkGray
            End Select
        End If
    End Sub

    Private Sub LogDispClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogDispClearButton.Click
        oLogDispStorage.Clear()
    End Sub

    Private Sub LogDispFilterEditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogDispFilterEditButton.Click
        If oLogDispFilterEditDialog Is Nothing Then
            oLogDispFilterEditDialog = New LogDispFilterEditDialog(UiState.LogDispFilterHistory, oLogDispStorage)
        End If

        Dim onError As Boolean = False
        Do
            If oLogDispFilterEditDialog.ShowDialog(onError) <> DialogResult.OK Then Return
            Dim sNewFilter As String = oLogDispFilterEditDialog.FilterValue
            Try
                oLogDispBinder.Filter = sNewFilter
                Dim nDispRows As Integer = LogDispGrid.Rows.Count
                If nDispRows <> 0 Then
                    LogDispGrid.FirstDisplayedScrollingRowIndex = nDispRows - 1
                End If
            Catch ex As Exception
                AlertBox.Show(Lexis.LogDispFilterIsInvalid)
                onError = True
                Continue Do
            End Try

            UiState.LogDispFilterHistory.Remove(sNewFilter)
            UiState.LogDispFilterHistory.Insert(0, sNewFilter)
            While UiState.LogDispFilterHistory.Count > Config.LogDispFilterMaxHisCount
                UiState.LogDispFilterHistory.RemoveAt(UiState.LogDispFilterHistory.Count - 1)
            End While

            LogDispFilter.Text = sNewFilter
            Exit Do
        Loop
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)

        OptionalWriter = New LogToOptionalDelegate(AddressOf Me.FetchLog)

        oLogDispStorage = New DataTable()
        'oLogDispStorage.Columns.Add("Time", GetType(DateTime))
        oLogDispStorage.Columns.Add("Time", GetType(String))
        oLogDispStorage.Columns.Add("Source", GetType(String))
        oLogDispStorage.Columns.Add("Kind", GetType(String))
        oLogDispStorage.Columns.Add("Message", GetType(String))

        oLogDispBinder = New BindingSource()
        oLogDispBinder.DataSource = oLogDispStorage

        LogDispGrid.DefaultCellStyle.SelectionForeColor = LogDispGrid.DefaultCellStyle.ForeColor
        LogDispGrid.DefaultCellStyle.SelectionBackColor = Color.LightGray
        LogDispGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        LogDispGrid.AutoGenerateColumns = True
        LogDispGrid.DataSource = oLogDispBinder

        'LogDispGrid.Columns(0).DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss.fff"
        LogDispGrid.Columns(2).Visible = False
        LogDispGrid.Columns(3).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        LogDispGrid.Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable
        LogDispGrid.Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable
        LogDispGrid.Columns(3).SortMode = DataGridViewColumnSortMode.NotSortable
        LogDispGrid.Columns(0).Width = MyUtility.GetTextWidth("9999/99/99 99:99:99.999", LogDispGrid.Font)
        LogDispGrid.Columns(1).Width = MyUtility.GetTextWidth("W_999999_9999_99", LogDispGrid.Font)
        LogDispGrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
    End Sub

    Protected Overrides Sub OnShown(ByVal e As EventArgs)
        MyBase.OnShown(e)

        Log.SetOptionalWriter(New LogToOptionalDelegate(AddressOf Me.BeginFetchLog))

        Dim sWorkingDir As String = System.Environment.CurrentDirectory
        Dim sIniFilePath As String = Path.ChangeExtension(Application.ExecutablePath, ".ini")
        sIniFilePath = Path.Combine(sWorkingDir, Path.GetFileName(sIniFilePath))
        Try
            Lexis.Init(sIniFilePath)
            Config.Init(sIniFilePath)
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.SomeErrorOccurredOnReadingConfigFile)
            Me.Close()
            Return
        End Try

        Log.SetKindsMask(Config.LogKindsMask)

        LocalConnectionProvider.Init()

        Dim oSerializer As New DataContractSerializer(GetType(UiStateClass))
        Dim sStateFileUri As String = Path.ChangeExtension(Path.GetFileName(Application.ExecutablePath), ".xml")
        sStateFileUri = sStateFileUri.Insert(sStateFileUri.Length - 4, "State")
        Try
            Using xr As XmlReader = XmlReader.Create(sStateFileUri)
                UiState = DirectCast(oSerializer.ReadObject(xr), UiStateClass)
            End Using
        Catch ex As FileNotFoundException
            Log.Info("Initializing UiState...")
            UiState = New UiStateClass()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.UiStateDeserializeFailed)
            Me.Close()
            Return
        End Try

        'Lexisから生成した文言やConfigやUiStateの値を各コントロールに反映する。

        If Config.ClearLogDispFilterHisOnBoot Then
            UiState.LogDispFilterHistory.Clear()
        End If

        For i As Integer = 0 To Config.LogDispFilterInitialHis.Rows.Count - 1
            Dim s As String = Config.LogDispFilterInitialHis.Rows(i).Field(Of String)("Value")
            UiState.LogDispFilterHistory.Remove(s)
            UiState.LogDispFilterHistory.Insert(i, s)
        Next i

        If UiState.LogDispFilterHistory.Count <> 0 Then
            oLogDispBinder.Filter = UiState.LogDispFilterHistory(0)
            LogDispFilter.Text = UiState.LogDispFilterHistory(0)
            Dim nDispRows As Integer = LogDispGrid.Rows.Count
            If nDispRows <> 0 Then
                LogDispGrid.FirstDisplayedScrollingRowIndex = nDispRows - 1
            End If
        End If

        LogDispGrid.Columns(0).HeaderText = Lexis.LogDispTimeColumnTitle.Gen()
        LogDispGrid.Columns(1).HeaderText = Lexis.LogDispSourceColumnTitle.Gen()
        LogDispGrid.Columns(3).HeaderText = Lexis.LogDispMessageColumnTitle.Gen()

        If Config.LogDispMessageColumnWidth > 0 Then
            LogDispGrid.Columns(3).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            LogDispGrid.Columns(3).Width = Config.LogDispMessageColumnWidth
        End If

        Me.Text = Lexis.FormTitle.Gen()

        TelegGene = New EkTelegramGeneForNativeModels("")
        TelegImporter = New EkTelegramImporter(TelegGene)

        InitTable1()
        InitTable2WithoutFilter()
        TuneTable2FilterToTable1Selection()
        TableSplitContainer.SplitterDistance _
           = DataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) _
            + SystemInformation.VerticalScrollBarWidth _
            + SystemInformation.BorderSize.Width * 2 _
            + TableSplitContainer.SplitterWidth - 1

        MasProDataFormDic = New Dictionary(Of String, Form)
        MasProListFormDic = New Dictionary(Of String, Form)
        FaultDataFormDic = New Dictionary(Of String, FaultDataForm)
        KadoDataFormDic = New Dictionary(Of String, KadoDataForm)

        Rand = New Random(1)

        Try
            If Not MessageQueue.Exists(Config.SelfMqPath) Then
                InputQueue = MessageQueue.Create(Config.SelfMqPath)
            Else
                InputQueue = New MessageQueue(Config.SelfMqPath)
            End If
            InputQueue.MessageReadPropertyFilter.ClearAll()
            InputQueue.MessageReadPropertyFilter.Body = True
            InputQueue.MessageReadPropertyFilter.Id = True
            InputQueue.MessageReadPropertyFilter.ResponseQueue = True
            InputQueue.Formatter = New XmlMessageFormatter(New Type() {GetType(ExtAppFuncMessageBody)})
            InputQueue.Purge()
            InputQueue.SynchronizingObject = Me
            InputQueue.BeginReceive()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            AlertBox.Show(Lexis.MessageQueueServiceNotAvailable)
            Me.Close()
            Return
        End Try

        SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
        If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
            Me.Close()
            Return
        End If

        Do
            Dim aDirectoryInfo As DirectoryInfo()
            Try
                Dim oDirInfo As New DirectoryInfo(Path.Combine(SimWorkingDirDialog.SelectedPath, Config.ModelPathInSimWorkingDir))
                aDirectoryInfo = oDirInfo.GetDirectories(MachineDirPattern)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                AlertBox.Show(Lexis.InvalidDirectorySpecified)
                Exit Do
            End Try

            For Each oDirectoryInfo As DirectoryInfo In aDirectoryInfo
                If Not MachineDirRegx.IsMatch(oDirectoryInfo.Name) Then Continue For
                Try
                    FetchMachineProfileFromFile(oDirectoryInfo.FullName)
                Catch ex As Exception
                    Log.Error("Exception caught.", ex)
                End Try
            Next oDirectoryInfo
        Loop While False
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        If oLogDispFilterEditDialog IsNot Nothing Then
            oLogDispFilterEditDialog.Dispose()
            oLogDispFilterEditDialog = Nothing
        End If

        If InputQueue IsNot Nothing Then
            'NOTE: 最後に呼び出したBeginReceiveに対応するReceiveCompletedイベントは、
            '下記によるInputQueue.readHandleのようなもののCloseによって、
            'それ以降、発生することは無くなる想定である。
            'InputQueue.SynchronizingObjectをNothingにしておけばよさそうにも
            '思えるが、このプロパティはスレッドセーフではなさそうであるため、
            '既にBeginReceiveを行ってしまっているこの時点では変更するべきではない。
            InputQueue.Dispose()
        End If

        If Config.SelfMqPath IsNot Nothing Then
            Try
                'NOTE: SelfMqPathが長すぎる場合は、MessageQueue.Exists()は
                '実際に当該パスにキューが存在していても、Falseを返却
                'するようである。一方、SelfMqPathが長すぎる場合も、
                'メッセージキューサービスがインストールされている限りは、
                'MessageQueue.Createが成功してしまう。
                'よって、SelfMqPathが長すぎる場合に、起動時に作成してしまった
                'キューの削除を試みるには、ここでのMessageQueue.Existsによる
                '判断を省略するしかない。なお、削除を試みたところで、
                'SelfMqPathが長すぎることを理由にMessageQueue.Delete()から
                'MessageQueueExceptionがスローされ、削除は失敗するが、
                'そのMessageQueueErrorCodeプロパティによって、キューが残って
                'しまうことが分かるため、その警告を出すことができる。
                'If MessageQueue.Exists(Config.SelfMqPath) Then
                '    MessageQueue.Delete(Config.SelfMqPath)
                'End If

                MessageQueue.Delete(Config.SelfMqPath)

            Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.FormatNameBufferTooSmall
                'NOTE: 本来は、アプリケーションがMessageQueueErrorCodeを
                '参照するべきではないが、MessageQueueクラスの挙動が
                '酷すぎるためやむを得ない。MessageQueueクラスの挙動が
                '改善されたら、除去すること。
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.MessageQueueDeleteFailed)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If

        If UiState IsNot Nothing Then
            'NOTE: このケースでは、右辺の各コントロールに、少なくとも起動時のファイルから
            'ロードした値はセット済みの想定である。

            'TODO: コントロールの状態を保存することにした場合は、
            'ここで各コントロールの値をUiStateに反映する。

            Dim oSerializer As New DataContractSerializer(GetType(UiStateClass))
            Dim sStateFileUri As String = Path.ChangeExtension(Path.GetFileName(Application.ExecutablePath), ".xml")
            sStateFileUri = sStateFileUri.Insert(sStateFileUri.Length - 4, "State")
            Try
                Dim ws As New XmlWriterSettings()
                ws.NewLineHandling = NewLineHandling.Entitize
                Using xw As XmlWriter = XmlWriter.Create(sStateFileUri, ws)
                    oSerializer.WriteObject(xw, UiState)
                End Using
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.UiStateSerializeFailed)
            End Try
        End If

        LocalConnectionProvider.Dispose()

        Log.SetOptionalWriter(Nothing)

        MyBase.OnFormClosed(e)
    End Sub

    Protected Sub AddExtraColumnsToTable2()
        If ConStatusRadioButton.Checked Then
            Table2.Columns.Add("PWR_FROM_KSB", GetType(Byte))
            Table2.Columns.Add("MCP_FROM_KSB", GetType(Byte))
            Table2.Columns.Add("ICM_FROM_MCP", GetType(Byte))
            Table2.Columns.Add("DLS_FROM_MCP", GetType(Byte))
            Table2.Columns.Add("DLS_FROM_ICM", GetType(Byte))
            Table2.Columns.Add("EXS_FROM_ICM", GetType(Byte))
        End If

        If MasStatusRadioButton.Checked Then
            Table2.Columns.Add("SLOT", GetType(String))
            For Each sKind As String In ExConstants.GateMastersSubObjCodes.Keys
                Table2.Columns.Add(sKind & "_DataSubKind", GetType(Integer))
                Table2.Columns.Add(sKind & "_DataVersion", GetType(Integer))
                Table2.Columns.Add(sKind & "_ListVersion", GetType(Integer))
                Table2.Columns.Add(sKind & "_DataAcceptDate", GetType(DateTime))
                Table2.Columns.Add(sKind & "_ListAcceptDate", GetType(DateTime))
                Table2.Columns.Add(sKind & "_DataDeliverDate", GetType(DateTime))
                Table2.Columns.Add(sKind & "_DataHashValue", GetType(String))
                Table2.Columns.Add(sKind & "_ListHashValue", GetType(String))
            Next sKind
        End If

        If ProStatusRadioButton.Checked Then
            Table2.Columns.Add("SLOT", GetType(String))
            Table2.Columns.Add("GPG_DataSubKind", GetType(Integer))
            Table2.Columns.Add("GPG_DataVersion", GetType(Integer))
            Table2.Columns.Add("GPG_ListVersion", GetType(Integer))
            Table2.Columns.Add("GPG_DataAcceptDate", GetType(DateTime))
            Table2.Columns.Add("GPG_ListAcceptDate", GetType(DateTime))
            Table2.Columns.Add("GPG_DataDeliverDate", GetType(DateTime))
            Table2.Columns.Add("GPG_ListDeliverDate", GetType(DateTime))
            Table2.Columns.Add("GPG_RunnableDate", GetType(String))
            Table2.Columns.Add("GPG_ApplicableDate", GetType(String))
            Table2.Columns.Add("GPG_ApplyDate", GetType(DateTime))
            Table2.Columns.Add("GPG_DataHashValue", GetType(String))
            Table2.Columns.Add("GPG_ListHashValue", GetType(String))
        End If

        If KsbProStatusRadioButton.Checked Then
            Table2.Columns.Add("SLOT", GetType(String))
            Table2.Columns.Add("WPG_DataSubKind", GetType(Integer))
            Table2.Columns.Add("WPG_DataVersion", GetType(Integer))
            Table2.Columns.Add("WPG_ListVersion", GetType(Integer))
            Table2.Columns.Add("WPG_DataAcceptDate", GetType(DateTime))
            Table2.Columns.Add("WPG_ListAcceptDate", GetType(DateTime))
            Table2.Columns.Add("WPG_DataDeliverDate", GetType(DateTime))
            Table2.Columns.Add("WPG_ListDeliverDate", GetType(DateTime))
            Table2.Columns.Add("WPG_RunnableDate", GetType(String))
            Table2.Columns.Add("WPG_ApplicableDate", GetType(String))
            Table2.Columns.Add("WPG_ApplyDate", GetType(DateTime))
            Table2.Columns.Add("WPG_DataHashValue", GetType(String))
            Table2.Columns.Add("WPG_ListHashValue", GetType(String))
        End If

        If UpboundProcStateRadioButton.Checked Then
            Table2.Columns.Add("LATCH_CONF", GetType(Byte))
            Table2.Columns.Add("FAULT_SEQ_NO", GetType(UInteger))
            Table2.Columns.Add("FAULT_DATE", GetType(DateTime))
            Table2.Columns.Add("KADO_SEQ_NO", GetType(UInteger))
            Table2.Columns.Add("KADO_DATE", GetType(DateTime))
            Table2.Columns.Add("HOSYU_SEQ_NO", GetType(UInteger))
            Table2.Columns.Add("HOSYU_DATE", GetType(DateTime))
        End If
    End Sub

    Protected Sub InitExtraComboColumnViewOfTable2(ByVal sName As String, ByVal sHeaderText As String, ByVal sSampleText As String, ByVal sMenuHeaderText As String, ByVal sMenuSampleText As String, ByVal oMenuTable As DataTable)
        Dim oColumns As DataGridViewColumnCollection = DataGridView2.Columns

        Dim oCodeColumn As DataGridViewColumn = oColumns(sName)
        oCodeColumn.HeaderText = sHeaderText
        oCodeColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        oCodeColumn.Width = MyUtility.GetTextWidth(sSampleText, DataGridView2.Font)

        Dim oMenuColumn As New DataGridViewComboBoxColumn()
        oMenuColumn.DataPropertyName = sName
        oMenuColumn.Name = sName & "_MENU"
        oMenuColumn.DataSource = oMenuTable
        oMenuColumn.ValueMember = "Key"
        oMenuColumn.DisplayMember = "Value"
        oMenuColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        oMenuColumn.FlatStyle = FlatStyle.Flat
        oMenuColumn.HeaderText = sMenuHeaderText
        oMenuColumn.Width = MyUtility.GetTextWidth(sMenuSampleText, DataGridView2.Font)
        oColumns.Insert(oCodeColumn.Index + 1, oMenuColumn)
    End Sub

    Protected Sub InitExtraColumnsViewOfTable2()
        If ConStatusRadioButton.Checked Then
            InitExtraComboColumnViewOfTable2("PWR_FROM_KSB", "電源状態 by監視 (X)", "FF..", "電源状態 by監視", "○○状態 by○○..", Config.MenuTableOfPwrStatusFromKsb)
            InitExtraComboColumnViewOfTable2("MCP_FROM_KSB", "主制状態 by監視 (X)", "FF..", "主制状態 by監視", "○○状態 by○○..", Config.MenuTableOfMcpStatusFromKsb)
            InitExtraComboColumnViewOfTable2("ICM_FROM_MCP", "ICU状態 by主制 (X)", "FF..", "ICU状態 by主制", "○○状態 by○○..", Config.MenuTableOfIcmStatusFromMcp)
            InitExtraComboColumnViewOfTable2("DLS_FROM_MCP", "配サ状態 by主制 (X)", "FF..", "配サ状態 by主制", "○○状態 by○○..", Config.MenuTableOfDlsStatusFromMcp)
            InitExtraComboColumnViewOfTable2("DLS_FROM_ICM", "配サ状態 byICU (X)", "FF..", "配サ状態 byICU", "○○状態 by○○..", Config.MenuTableOfDlsStatusFromIcm)
            InitExtraComboColumnViewOfTable2("EXS_FROM_ICM", "統サ状態 byICU (X)", "FF..", "統サ状態 byICU", "○○状態 by○○..", Config.MenuTableOfExsStatusFromIcm)
        End If

        If MasStatusRadioButton.Checked Then
            DataGridView2.Columns("SLOT").ReadOnly = True
            DataGridView2.Columns("SLOT").Frozen = True
            DataGridView2.Columns("SLOT").HeaderText = "世代"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("配信待ち(9)",  DataGridView2.Font)
            Dim pnWidth As Integer = MyUtility.GetTextWidth("000.", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("000.", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("000.", DataGridView2.Font)
            Dim tmWidth As Integer = MyUtility.GetTextWidth("9999/99/99 99:99..", DataGridView2.Font)
            Dim hvWidth As Integer = MyUtility.GetTextWidth("AAAAAAAAAAAA...", DataGridView2.Font)
            For Each sKind As String In ExConstants.GateMastersSubObjCodes.Keys
                DataGridView2.Columns(sKind & "_DataSubKind").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataVersion").ReadOnly = True
                DataGridView2.Columns(sKind & "_ListVersion").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataAcceptDate").ReadOnly = True
                DataGridView2.Columns(sKind & "_ListAcceptDate").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataDeliverDate").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataHashValue").ReadOnly = True
                DataGridView2.Columns(sKind & "_ListHashValue").ReadOnly = True
                DataGridView2.Columns(sKind & "_DataSubKind").HeaderText = "パターンNo (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataVersion").HeaderText = "マスタVer (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListVersion").HeaderText = "リストVer (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataAcceptDate").HeaderText = "データ監着 (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListAcceptDate").HeaderText = "リスト監着 (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataDeliverDate").HeaderText = "データ改着 (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataHashValue").HeaderText = "データ概値 (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListHashValue").HeaderText = "リスト概値 (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataSubKind").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                DataGridView2.Columns(sKind & "_DataVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                DataGridView2.Columns(sKind & "_ListVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                'DataGridView2.Columns(sKind & "_DataAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
                'DataGridView2.Columns(sKind & "_ListAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
                'DataGridView2.Columns(sKind & "_DataDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
                DataGridView2.Columns(sKind & "_DataSubKind").Width = pnWidth
                DataGridView2.Columns(sKind & "_DataVersion").Width = dvWidth
                DataGridView2.Columns(sKind & "_ListVersion").Width = lvWidth
                DataGridView2.Columns(sKind & "_DataAcceptDate").Width = tmWidth
                DataGridView2.Columns(sKind & "_ListAcceptDate").Width = tmWidth
                DataGridView2.Columns(sKind & "_DataDeliverDate").Width = tmWidth
                DataGridView2.Columns(sKind & "_DataHashValue").Width = hvWidth
                DataGridView2.Columns(sKind & "_ListHashValue").Width = hvWidth
            Next sKind
        End If

        If ProStatusRadioButton.Checked Then
            DataGridView2.Columns("SLOT").ReadOnly = True
            DataGridView2.Columns("SLOT").Frozen = True
            DataGridView2.Columns("SLOT").HeaderText = "世代"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("配信待ち(9)", DataGridView2.Font)
            Dim anWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim adWidth As Integer = MyUtility.GetTextWidth("○○○○○.", DataGridView2.Font)
            Dim rdWidth As Integer = MyUtility.GetTextWidth("○○○○○.", DataGridView2.Font)
            Dim tmWidth As Integer = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)
            Dim hvWidth As Integer = MyUtility.GetTextWidth("AAAAAAAAAAAA...", DataGridView2.Font)
            DataGridView2.Columns("GPG_DataSubKind").ReadOnly = True
            DataGridView2.Columns("GPG_DataVersion").ReadOnly = True
            DataGridView2.Columns("GPG_ListVersion").ReadOnly = True
            DataGridView2.Columns("GPG_DataAcceptDate").ReadOnly = True
            DataGridView2.Columns("GPG_ListAcceptDate").ReadOnly = True
            DataGridView2.Columns("GPG_DataDeliverDate").ReadOnly = True
            DataGridView2.Columns("GPG_ListDeliverDate").ReadOnly = True
            DataGridView2.Columns("GPG_RunnableDate").ReadOnly = True
            DataGridView2.Columns("GPG_ApplicableDate").ReadOnly = True
            DataGridView2.Columns("GPG_ApplyDate").ReadOnly = True
            DataGridView2.Columns("GPG_DataHashValue").ReadOnly = True
            DataGridView2.Columns("GPG_ListHashValue").ReadOnly = True
            DataGridView2.Columns("GPG_DataSubKind").HeaderText = "エリアNo"
            DataGridView2.Columns("GPG_DataVersion").HeaderText = "代表Ver"
            DataGridView2.Columns("GPG_ListVersion").HeaderText = "リストVer"
            DataGridView2.Columns("GPG_DataAcceptDate").HeaderText = "データ監着日時"
            DataGridView2.Columns("GPG_ListAcceptDate").HeaderText = "リスト監着日時"
            DataGridView2.Columns("GPG_DataDeliverDate").HeaderText = "データ改着日時"
            DataGridView2.Columns("GPG_ListDeliverDate").HeaderText = "リスト改着日時"
            DataGridView2.Columns("GPG_RunnableDate").HeaderText = "動作許可日"
            DataGridView2.Columns("GPG_ApplicableDate").HeaderText = "適用日"
            DataGridView2.Columns("GPG_ApplyDate").HeaderText = "適用完了日時"
            DataGridView2.Columns("GPG_DataHashValue").HeaderText = "データハッシュ値"
            DataGridView2.Columns("GPG_ListHashValue").HeaderText = "リストハッシュ値"
            DataGridView2.Columns("GPG_DataSubKind").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("GPG_DataVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("GPG_ListVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            'DataGridView2.Columns("GPG_DataAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("GPG_ListAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("GPG_DataDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("GPG_ListDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("GPG_ApplyDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("GPG_DataSubKind").Width = anWidth
            DataGridView2.Columns("GPG_DataVersion").Width = dvWidth
            DataGridView2.Columns("GPG_ListVersion").Width = lvWidth
            DataGridView2.Columns("GPG_DataAcceptDate").Width = tmWidth
            DataGridView2.Columns("GPG_ListAcceptDate").Width = tmWidth
            DataGridView2.Columns("GPG_DataDeliverDate").Width = tmWidth
            DataGridView2.Columns("GPG_ListDeliverDate").Width = tmWidth
            DataGridView2.Columns("GPG_RunnableDate").Width = rdWidth
            DataGridView2.Columns("GPG_ApplicableDate").Width = adWidth
            DataGridView2.Columns("GPG_ApplyDate").Width = tmWidth
            DataGridView2.Columns("GPG_DataHashValue").Width = hvWidth
            DataGridView2.Columns("GPG_ListHashValue").Width = hvWidth
        End If

        If KsbProStatusRadioButton.Checked Then
            DataGridView2.Columns("SLOT").ReadOnly = True
            DataGridView2.Columns("SLOT").Frozen = True
            DataGridView2.Columns("SLOT").HeaderText = "世代"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("配信待ち(9)", DataGridView2.Font)
            Dim anWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("○○○Abc", DataGridView2.Font)
            Dim adWidth As Integer = MyUtility.GetTextWidth("○○○○○.", DataGridView2.Font)
            Dim rdWidth As Integer = MyUtility.GetTextWidth("○○○○○.", DataGridView2.Font)
            Dim tmWidth As Integer = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)
            Dim hvWidth As Integer = MyUtility.GetTextWidth("AAAAAAAAAAAA...", DataGridView2.Font)
            DataGridView2.Columns("WPG_DataSubKind").ReadOnly = True
            DataGridView2.Columns("WPG_DataVersion").ReadOnly = True
            DataGridView2.Columns("WPG_ListVersion").ReadOnly = True
            DataGridView2.Columns("WPG_DataAcceptDate").ReadOnly = True
            DataGridView2.Columns("WPG_ListAcceptDate").ReadOnly = True
            DataGridView2.Columns("WPG_DataDeliverDate").ReadOnly = True
            DataGridView2.Columns("WPG_ListDeliverDate").ReadOnly = True
            DataGridView2.Columns("WPG_RunnableDate").ReadOnly = True
            DataGridView2.Columns("WPG_ApplicableDate").ReadOnly = True
            DataGridView2.Columns("WPG_ApplyDate").ReadOnly = True
            DataGridView2.Columns("WPG_DataHashValue").ReadOnly = True
            DataGridView2.Columns("WPG_ListHashValue").ReadOnly = True
            DataGridView2.Columns("WPG_DataSubKind").HeaderText = "エリアNo"
            DataGridView2.Columns("WPG_DataVersion").HeaderText = "代表Ver"
            DataGridView2.Columns("WPG_ListVersion").HeaderText = "リストVer"
            DataGridView2.Columns("WPG_DataAcceptDate").HeaderText = "データ受信日時"
            DataGridView2.Columns("WPG_ListAcceptDate").HeaderText = "リスト受信日時"
            DataGridView2.Columns("WPG_DataDeliverDate").HeaderText = "データDL完了日時"
            DataGridView2.Columns("WPG_ListDeliverDate").HeaderText = "リストDL完了日時"
            DataGridView2.Columns("WPG_RunnableDate").HeaderText = "動作許可日"
            DataGridView2.Columns("WPG_ApplicableDate").HeaderText = "適用日"
            DataGridView2.Columns("WPG_ApplyDate").HeaderText = "適用完了日時"
            DataGridView2.Columns("WPG_DataHashValue").HeaderText = "データハッシュ値"
            DataGridView2.Columns("WPG_ListHashValue").HeaderText = "リストハッシュ値"
            DataGridView2.Columns("WPG_DataSubKind").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("WPG_DataVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("WPG_ListVersion").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            'DataGridView2.Columns("WPG_DataAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("WPG_ListAcceptDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("WPG_DataDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("WPG_ListDeliverDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            'DataGridView2.Columns("WPG_ApplyDate").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("WPG_DataSubKind").Width = anWidth
            DataGridView2.Columns("WPG_DataVersion").Width = dvWidth
            DataGridView2.Columns("WPG_ListVersion").Width = lvWidth
            DataGridView2.Columns("WPG_DataAcceptDate").Width = tmWidth
            DataGridView2.Columns("WPG_ListAcceptDate").Width = tmWidth
            DataGridView2.Columns("WPG_DataDeliverDate").Width = tmWidth
            DataGridView2.Columns("WPG_ListDeliverDate").Width = tmWidth
            DataGridView2.Columns("WPG_RunnableDate").Width = rdWidth
            DataGridView2.Columns("WPG_ApplicableDate").Width = adWidth
            DataGridView2.Columns("WPG_ApplyDate").Width = tmWidth
            DataGridView2.Columns("WPG_DataHashValue").Width = hvWidth
            DataGridView2.Columns("WPG_ListHashValue").Width = hvWidth
        End If

        If UpboundProcStateRadioButton.Checked Then
            InitExtraComboColumnViewOfTable2("LATCH_CONF", "ラッチ形態(X)", "FF..", "ラッチ形態", "ラッチ外出札...▼", Config.MenuTableOfLatchConf)
            DataGridView2.Columns("LATCH_CONF").Frozen = True
            DataGridView2.Columns("LATCH_CONF_MENU").Frozen = True

            DataGridView2.Columns("FAULT_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("FAULT_SEQ_NO").HeaderText = "最終異常SEQ.No"
            DataGridView2.Columns("FAULT_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("FAULT_SEQ_NO").Width = MyUtility.GetTextWidth("最終異常SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("FAULT_DATE").ReadOnly = True
            DataGridView2.Columns("FAULT_DATE").HeaderText = "最終異常処理日時"
            'DataGridView2.Columns("FAULT_DATE").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("FAULT_DATE").Width = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)

            DataGridView2.Columns("KADO_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("KADO_SEQ_NO").HeaderText = "最終稼動SEQ.No"
            DataGridView2.Columns("KADO_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("KADO_SEQ_NO").Width = MyUtility.GetTextWidth("最終稼動SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("KADO_DATE").ReadOnly = True
            DataGridView2.Columns("KADO_DATE").HeaderText = "最終稼動処理日時"
            'DataGridView2.Columns("KADO_DATE").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("KADO_DATE").Width = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)

            DataGridView2.Columns("HOSYU_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("HOSYU_SEQ_NO").HeaderText = "最終保守SEQ.No"
            DataGridView2.Columns("HOSYU_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("HOSYU_SEQ_NO").Width = MyUtility.GetTextWidth("最終保守SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("HOSYU_DATE").ReadOnly = True
            DataGridView2.Columns("HOSYU_DATE").HeaderText = "最終保守処理日時"
            'DataGridView2.Columns("HOSYU_DATE").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("HOSYU_DATE").Width = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)
        End If
End Sub

    Protected Function GetMonitorMachineRowCountForTable2(ByVal oMachine As Machine) As Integer
        If ConStatusRadioButton.Checked Then
            Return 0
        End If

        If MasStatusRadioButton.Checked Then
            Return 2
        End If

        If ProStatusRadioButton.Checked Then
            Return 2
        End If

        If KsbProStatusRadioButton.Checked Then
            Return oMachine.PendingKsbPrograms.Count + 2
        End If

        If UpboundProcStateRadioButton.Checked Then
            Return 1
        End If

        Return 0
    End Function

    Protected Sub SetProfileValueToTable2MonitorMachineRow(ByVal oTargetRow As DataRow, ByVal sMachineId As String, ByVal oMachine As Machine)
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If sFieldName = "MACHINE_ID" Then
                oTargetRow(sFieldName) = sMachineId
            ElseIf sFieldName = "TERM_MACHINE_ID" Then
                oTargetRow(sFieldName) = ""
            Else
                oTargetRow(sFieldName) = oMachine.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
            End If
        Next i
    End Sub

    Protected Sub SetExtraValueToTable2MonitorMachineRow(ByVal oTargetRow As DataRow, ByVal oMachine As Machine, ByVal index As Integer, ByVal count As Integer)
        If MasStatusRadioButton.Checked Then
            Dim listIndex As Integer = count - index - 1
            oTargetRow("SLOT") = "保持(" & (listIndex + 1).ToString() & ")"
            For Each sKind As String In ExConstants.GateMastersSubObjCodes.Keys
                Dim oMas As HoldingMaster = Nothing
                Dim oHoldingMasters As HoldingMaster() = Nothing
                If oMachine.HoldingMasters.TryGetValue(sKind, oHoldingMasters) = True Then
                    oMas = oHoldingMasters(listIndex)
                End If

                If oMas IsNot Nothing Then
                    oTargetRow(sKind & "_DataSubKind") = oMas.DataSubKind
                    oTargetRow(sKind & "_DataVersion") = oMas.DataVersion
                    oTargetRow(sKind & "_DataAcceptDate") = oMas.DataAcceptDate
                    oTargetRow(sKind & "_DataHashValue") = oMas.DataHashValue
                Else
                    oTargetRow(sKind & "_DataSubKind") = DbNull.Value
                    oTargetRow(sKind & "_DataVersion") = DbNull.Value
                    oTargetRow(sKind & "_DataAcceptDate") = DbNull.Value
                    oTargetRow(sKind & "_DataHashValue") = DbNull.Value
                End If

                If oMas IsNot Nothing AndAlso oMas.ListHashValue IsNot Nothing Then
                    oTargetRow(sKind & "_ListVersion") = oMas.ListVersion
                    oTargetRow(sKind & "_ListAcceptDate") = oMas.ListAcceptDate
                    oTargetRow(sKind & "_ListHashValue") = oMas.ListHashValue
                Else
                    oTargetRow(sKind & "_ListVersion") = DbNull.Value
                    oTargetRow(sKind & "_ListAcceptDate") = DbNull.Value
                    oTargetRow(sKind & "_ListHashValue") = DbNull.Value
                End If

                oTargetRow(sKind & "_DataDeliverDate") = DbNull.Value
            Next sKind
        End If

        If ProStatusRadioButton.Checked Then
            Dim listIndex As Integer = count - index - 1
            Dim oPro As HoldingProgram = oMachine.HoldingPrograms(listIndex)
            oTargetRow("SLOT") = "保持(" & (listIndex + 1).ToString() & ")"

            If oPro IsNot Nothing Then
                oTargetRow("GPG_DataSubKind") = oPro.DataSubKind
                oTargetRow("GPG_DataVersion") = oPro.DataVersion
                oTargetRow("GPG_DataAcceptDate") = oPro.DataAcceptDate
                oTargetRow("GPG_RunnableDate") = oPro.RunnableDate
                oTargetRow("GPG_DataHashValue") = oPro.DataHashValue
            Else
                oTargetRow("GPG_DataSubKind") = DbNull.Value
                oTargetRow("GPG_DataVersion") = DbNull.Value
                oTargetRow("GPG_DataAcceptDate") = DbNull.Value
                oTargetRow("GPG_RunnableDate") = DbNull.Value
                oTargetRow("GPG_DataHashValue") = DbNull.Value
            End If

            If oPro IsNot Nothing AndAlso oPro.ListHashValue IsNot Nothing Then
                oTargetRow("GPG_ListVersion") = oPro.ListVersion
                oTargetRow("GPG_ListAcceptDate") = oPro.ListAcceptDate
                oTargetRow("GPG_ListHashValue") = oPro.ListHashValue
            Else
                oTargetRow("GPG_ListVersion") = DbNull.Value
                oTargetRow("GPG_ListAcceptDate") = DbNull.Value
                oTargetRow("GPG_ListHashValue") = DbNull.Value
            End If

            oTargetRow("GPG_DataDeliverDate") = DbNull.Value
            oTargetRow("GPG_ListDeliverDate") = DbNull.Value
            oTargetRow("GPG_ApplicableDate") = DbNull.Value
            oTargetRow("GPG_ApplyDate") = DbNull.Value
        End If

        If KsbProStatusRadioButton.Checked Then
            If index < count - 2 Then
                Dim listIndex As Integer = count - 2 - index - 1
                Dim oPro As PendingKsbProgram = oMachine.PendingKsbPrograms(listIndex)
                oTargetRow("SLOT") = "配信待ち(" & (listIndex + 1).ToString() & ")"

                If oPro.DataVersion <> 0 Then
                    oTargetRow("WPG_DataSubKind") = oPro.DataSubKind
                    oTargetRow("WPG_DataVersion") = oPro.DataVersion
                    oTargetRow("WPG_DataAcceptDate") = oPro.DataAcceptDate
                    oTargetRow("WPG_RunnableDate") = oPro.RunnableDate
                    oTargetRow("WPG_DataHashValue") = oPro.DataHashValue
                Else
                    oTargetRow("WPG_DataSubKind") = DbNull.Value
                    oTargetRow("WPG_DataVersion") = DbNull.Value
                    oTargetRow("WPG_DataAcceptDate") = DbNull.Value
                    oTargetRow("WPG_RunnableDate") = DbNull.Value
                    oTargetRow("WPG_DataHashValue") = DbNull.Value
                End If

                If oPro.DataVersion <> 0 AndAlso oPro.ListHashValue IsNot Nothing Then
                    oTargetRow("WPG_ListVersion") = oPro.ListVersion
                    oTargetRow("WPG_ListAcceptDate") = oPro.ListAcceptDate
                    oTargetRow("WPG_ApplicableDate") = oPro.ApplicableDate
                    oTargetRow("WPG_ListHashValue") = oPro.ListHashValue
                Else
                    oTargetRow("WPG_ListVersion") = DbNull.Value
                    oTargetRow("WPG_ListAcceptDate") = DbNull.Value
                    oTargetRow("WPG_ApplicableDate") = DbNull.Value
                    oTargetRow("WPG_ListHashValue") = DbNull.Value
                End If

                oTargetRow("WPG_DataDeliverDate") = DbNull.Value
                oTargetRow("WPG_ListDeliverDate") = DbNull.Value
                oTargetRow("WPG_ApplyDate") = DbNull.Value
            Else
                Dim listIndex As Integer = 1 - index + (count - 2)
                Dim oPro As HoldingKsbProgram = oMachine.HoldingKsbPrograms(listIndex)
                oTargetRow("SLOT") = If(listIndex = 1, "適用待ち", "適用中")

                If oPro IsNot Nothing Then
                    oTargetRow("WPG_DataSubKind") = oPro.DataSubKind
                    oTargetRow("WPG_DataVersion") = oPro.DataVersion
                    oTargetRow("WPG_DataAcceptDate") = oPro.DataAcceptDate
                    oTargetRow("WPG_DataDeliverDate") = oPro.DataDeliverDate
                    oTargetRow("WPG_RunnableDate") = oPro.RunnableDate
                    oTargetRow("WPG_ApplyDate") = If(listIndex = 1, DbNull.Value, DirectCast(oPro.ApplyDate, Object))
                    oTargetRow("WPG_DataHashValue") = oPro.DataHashValue
                Else
                    oTargetRow("WPG_DataSubKind") = DbNull.Value
                    oTargetRow("WPG_DataVersion") = DbNull.Value
                    oTargetRow("WPG_DataAcceptDate") = DbNull.Value
                    oTargetRow("WPG_DataDeliverDate") = DbNull.Value
                    oTargetRow("WPG_RunnableDate") = DbNull.Value
                    oTargetRow("WPG_ApplyDate") = DbNull.Value
                    oTargetRow("WPG_DataHashValue") = DbNull.Value
                End If

                If oPro IsNot Nothing AndAlso oPro.ListHashValue IsNot Nothing Then
                    oTargetRow("WPG_ListVersion") = oPro.ListVersion
                    oTargetRow("WPG_ListAcceptDate") = oPro.ListAcceptDate
                    oTargetRow("WPG_ListDeliverDate") = oPro.ListDeliverDate
                    oTargetRow("WPG_ApplicableDate") = oPro.ApplicableDate
                    oTargetRow("WPG_ListHashValue") = oPro.ListHashValue
                Else
                    oTargetRow("WPG_ListVersion") = DbNull.Value
                    oTargetRow("WPG_ListAcceptDate") = DbNull.Value
                    oTargetRow("WPG_ListDeliverDate") = DbNull.Value
                    oTargetRow("WPG_ApplicableDate") = DbNull.Value
                    oTargetRow("WPG_ListHashValue") = DbNull.Value
                End If
            End If
        End If

        If UpboundProcStateRadioButton.Checked Then
            oTargetRow("LATCH_CONF") = oMachine.LatchConf
            oTargetRow("FAULT_SEQ_NO") = oMachine.FaultSeqNumber
            oTargetRow("FAULT_DATE") = oMachine.FaultDate
        End If
    End Sub

    Protected Function GetTermMachineRowCountForTable2(ByVal oMachine As TermMachine) As Integer
        If ConStatusRadioButton.Checked Then
            Return 1
        End If

        If MasStatusRadioButton.Checked Then
            Dim n As Integer = 0
            For Each oPendingMasters As LinkedList(Of PendingMaster) In oMachine.PendingMasters.Values
                If n < oPendingMasters.Count Then
                    n = oPendingMasters.Count
                End If
            Next oPendingMasters
            Return n + 1
        End If

        If ProStatusRadioButton.Checked Then
            Return oMachine.PendingPrograms.Count + 2
        End If

        If KsbProStatusRadioButton.Checked Then
            Return 0
        End If

        If UpboundProcStateRadioButton.Checked Then
            Return 1
        End If

        Return 0
    End Function

    Protected Sub SetProfileValueToTable2TermMachineRow(ByVal oTargetRow As DataRow, ByVal sMonitorMachineId As String, ByVal sTermMachineId As String, ByVal oTermMachine As TermMachine)
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If sFieldName = "MACHINE_ID" Then
                oTargetRow(sFieldName) = sMonitorMachineId
            ElseIf sFieldName = "TERM_MACHINE_ID" Then
                oTargetRow(sFieldName) = sTermMachineId
            Else
                oTargetRow(sFieldName) = oTermMachine.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
            End If
        Next i
    End Sub

    Protected Sub SetExtraValueToTable2TermMachineRow(ByVal oTargetRow As DataRow, ByVal oMachine As TermMachine, ByVal index As Integer, ByVal count As Integer)
        If ConStatusRadioButton.Checked Then
            oTargetRow("PWR_FROM_KSB") = oMachine.PwrStatusFromKsb
            oTargetRow("MCP_FROM_KSB") = oMachine.McpStatusFromKsb
            oTargetRow("ICM_FROM_MCP") = oMachine.IcmStatusFromMcp
            oTargetRow("DLS_FROM_MCP") = oMachine.DlsStatusFromMcp
            oTargetRow("DLS_FROM_ICM") = oMachine.DlsStatusFromIcm
            oTargetRow("EXS_FROM_ICM") = oMachine.ExsStatusFromIcm
        End If

        If MasStatusRadioButton.Checked Then
            If index < count - 1 Then
                Dim listIndex As Integer = count - 1 - index - 1
                oTargetRow("SLOT") = "配信待ち(" & (listIndex + 1).ToString() & ")"
                For Each sKind As String In ExConstants.GateMastersSubObjCodes.Keys
                    Dim oMas As PendingMaster = Nothing
                    Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                    If oMachine.PendingMasters.TryGetValue(sKind, oPendingMasters) = True AndAlso
                       listIndex < oPendingMasters.Count Then
                        oMas = oPendingMasters(listIndex)
                    End If

                    If oMas IsNot Nothing Then
                        oTargetRow(sKind & "_DataSubKind") = oMas.DataSubKind
                        oTargetRow(sKind & "_DataVersion") = oMas.DataVersion
                        oTargetRow(sKind & "_DataAcceptDate") = oMas.DataAcceptDate
                        oTargetRow(sKind & "_DataHashValue") = oMas.DataHashValue
                    Else
                        oTargetRow(sKind & "_DataSubKind") = DbNull.Value
                        oTargetRow(sKind & "_DataVersion") = DbNull.Value
                        oTargetRow(sKind & "_DataAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_DataHashValue") = DbNull.Value
                    End If

                    If oMas IsNot Nothing AndAlso oMas.ListHashValue IsNot Nothing Then
                        oTargetRow(sKind & "_ListVersion") = oMas.ListVersion
                        oTargetRow(sKind & "_ListAcceptDate") = oMas.ListAcceptDate
                        oTargetRow(sKind & "_ListHashValue") = oMas.ListHashValue
                    Else
                        oTargetRow(sKind & "_ListVersion") = DbNull.Value
                        oTargetRow(sKind & "_ListAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_ListHashValue") = DbNull.Value
                    End If

                    oTargetRow(sKind & "_DataDeliverDate") = DbNull.Value
                Next sKind
            Else
                oTargetRow("SLOT") = "適用中"
                For Each sKind As String In ExConstants.GateMastersSubObjCodes.Keys
                    Dim oMas As HoldingMaster = Nothing
                    oMachine.HoldingMasters.TryGetValue(sKind, oMas)

                    If oMas IsNot Nothing Then
                        oTargetRow(sKind & "_DataSubKind") = oMas.DataSubKind
                        oTargetRow(sKind & "_DataVersion") = oMas.DataVersion
                        oTargetRow(sKind & "_DataAcceptDate") = oMas.DataAcceptDate
                        oTargetRow(sKind & "_DataDeliverDate") = oMas.DataDeliverDate
                        oTargetRow(sKind & "_DataHashValue") = oMas.DataHashValue
                    Else
                        oTargetRow(sKind & "_DataSubKind") = DbNull.Value
                        oTargetRow(sKind & "_DataVersion") = DbNull.Value
                        oTargetRow(sKind & "_DataAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_DataDeliverDate") = DbNull.Value
                        oTargetRow(sKind & "_DataHashValue") = DbNull.Value
                    End If

                    If oMas IsNot Nothing AndAlso oMas.ListHashValue IsNot Nothing Then
                        oTargetRow(sKind & "_ListVersion") = oMas.ListVersion
                        oTargetRow(sKind & "_ListAcceptDate") = oMas.ListAcceptDate
                        oTargetRow(sKind & "_ListHashValue") = oMas.ListHashValue
                    Else
                        oTargetRow(sKind & "_ListVersion") = DbNull.Value
                        oTargetRow(sKind & "_ListAcceptDate") = DbNull.Value
                        oTargetRow(sKind & "_ListHashValue") = DbNull.Value
                    End If
                Next sKind
            End If
        End If

        If ProStatusRadioButton.Checked Then
            If index < count - 2 Then
                Dim listIndex As Integer = count - 2 - index - 1
                Dim oPro As PendingProgram = oMachine.PendingPrograms(listIndex)
                oTargetRow("SLOT") = "配信待ち(" & (listIndex + 1).ToString() & ")"

                oTargetRow("GPG_DataSubKind") = oPro.DataSubKind
                oTargetRow("GPG_DataVersion") = oPro.DataVersion
                oTargetRow("GPG_DataAcceptDate") = oPro.DataAcceptDate
                oTargetRow("GPG_DataDeliverDate") = DbNull.Value
                oTargetRow("GPG_RunnableDate") = oPro.RunnableDate
                oTargetRow("GPG_ApplyDate") = DbNull.Value
                oTargetRow("GPG_DataHashValue") = oPro.DataHashValue
                oTargetRow("GPG_ListDeliverDate") = DbNull.Value

                If oPro.ListHashValue IsNot Nothing Then
                    oTargetRow("GPG_ListVersion") = oPro.ListVersion
                    oTargetRow("GPG_ListAcceptDate") = oPro.ListAcceptDate
                    oTargetRow("GPG_ApplicableDate") = oPro.ApplicableDate
                    oTargetRow("GPG_ListHashValue") = oPro.ListHashValue
                Else
                    oTargetRow("GPG_ListVersion") = DbNull.Value
                    oTargetRow("GPG_ListAcceptDate") = DbNull.Value
                    oTargetRow("GPG_ApplicableDate") = DbNull.Value
                    oTargetRow("GPG_ListHashValue") = DbNull.Value
                End If
            Else
                Dim listIndex As Integer = 1 - index + (count - 2)
                Dim oPro As HoldingProgram = oMachine.HoldingPrograms(listIndex)
                oTargetRow("SLOT") = If(listIndex = 1, "適用待ち", "適用中")

                If oPro IsNot Nothing Then
                    oTargetRow("GPG_DataSubKind") = oPro.DataSubKind
                    oTargetRow("GPG_DataVersion") = oPro.DataVersion
                    oTargetRow("GPG_DataAcceptDate") = oPro.DataAcceptDate
                    oTargetRow("GPG_DataDeliverDate") = oPro.DataDeliverDate
                    oTargetRow("GPG_RunnableDate") = oPro.RunnableDate
                    oTargetRow("GPG_ApplyDate") = If(listIndex = 1, DbNull.Value, DirectCast(oPro.ApplyDate, Object))
                    oTargetRow("GPG_DataHashValue") = oPro.DataHashValue
                Else
                    oTargetRow("GPG_DataSubKind") = DbNull.Value
                    oTargetRow("GPG_DataVersion") = DbNull.Value
                    oTargetRow("GPG_DataAcceptDate") = DbNull.Value
                    oTargetRow("GPG_DataDeliverDate") = DbNull.Value
                    oTargetRow("GPG_RunnableDate") = DbNull.Value
                    oTargetRow("GPG_ApplyDate") = DbNull.Value
                    oTargetRow("GPG_DataHashValue") = DbNull.Value
                End If

                If oPro IsNot Nothing AndAlso oPro.ListHashValue IsNot Nothing Then
                    oTargetRow("GPG_ListVersion") = oPro.ListVersion
                    oTargetRow("GPG_ListAcceptDate") = oPro.ListAcceptDate
                    oTargetRow("GPG_ListDeliverDate") = oPro.ListDeliverDate
                    oTargetRow("GPG_ApplicableDate") = oPro.ApplicableDate
                    oTargetRow("GPG_ListHashValue") = oPro.ListHashValue
                Else
                    oTargetRow("GPG_ListVersion") = DbNull.Value
                    oTargetRow("GPG_ListAcceptDate") = DbNull.Value
                    oTargetRow("GPG_ListDeliverDate") = DbNull.Value
                    oTargetRow("GPG_ApplicableDate") = DbNull.Value
                    oTargetRow("GPG_ListHashValue") = DbNull.Value
                End If
            End If
        End If

        If UpboundProcStateRadioButton.Checked Then
            oTargetRow("LATCH_CONF") = oMachine.LatchConf
            oTargetRow("FAULT_SEQ_NO") = oMachine.FaultSeqNumber
            oTargetRow("FAULT_DATE") = oMachine.FaultDate
            oTargetRow("KADO_SEQ_NO") = oMachine.KadoSeqNumber(0)
            oTargetRow("KADO_DATE") = oMachine.KadoDate(0)
            oTargetRow("HOSYU_SEQ_NO") = oMachine.KadoSeqNumber(1)
            oTargetRow("HOSYU_DATE") = oMachine.KadoDate(1)
        End If
    End Sub

    Protected Sub FetchStateFromTable2Row(ByVal oRow As DataRow)
        Dim sMachineId As String = oRow.Field(Of String)("MACHINE_ID")
        Dim oMachine As Machine = UiState.Machines(sMachineId)

        If ConStatusRadioButton.Checked Then
            Dim sTermMachineId As String = oRow.Field(Of String)("TERM_MACHINE_ID")
            Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
            oTerm.PwrStatusFromKsb = oRow.Field(Of Byte)("PWR_FROM_KSB")
            oTerm.McpStatusFromKsb = oRow.Field(Of Byte)("MCP_FROM_KSB")
            oTerm.IcmStatusFromMcp = oRow.Field(Of Byte)("ICM_FROM_MCP")
            oTerm.DlsStatusFromMcp = oRow.Field(Of Byte)("DLS_FROM_MCP")
            oTerm.DlsStatusFromIcm = oRow.Field(Of Byte)("DLS_FROM_ICM")
            oTerm.ExsStatusFromIcm = oRow.Field(Of Byte)("EXS_FROM_ICM")
        End If

        If UpboundProcStateRadioButton.Checked Then
            Dim sTermMachineId As String = oRow.Field(Of String)("TERM_MACHINE_ID")
            If sTermMachineId.Length = 0 Then
                oMachine.LatchConf = oRow.Field(Of Byte)("LATCH_CONF")
            Else
                Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                oTerm.LatchConf = oRow.Field(Of Byte)("LATCH_CONF")
            End If
        End If
    End Sub

    Protected Sub InitTable1()
        '空のデータテーブルを作成し、フィールド名を設定する。
        Table1 = New DataTable()
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            Table1.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i

        'UiState.Machinesの基本情報をデータテーブルに展開する。
        For Each oMachineEntry As KeyValuePair(Of String, Machine) In UiState.Machines
            Dim oRow As DataRow = Table1.NewRow()
            For i As Integer = 0 To Config.Table1FieldNames.Length - 1
                Dim sFieldName As String = Config.Table1FieldNames(i)
                If sFieldName = "MACHINE_ID" Then
                    oRow(sFieldName) = oMachineEntry.Key
                ElseIf sFieldName = "LAST_CONFIRMED" Then
                    oRow(sFieldName) = oMachineEntry.Value.LastConfirmed
                Else
                    oRow(sFieldName) = oMachineEntry.Value.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
                End If
            Next i
            Table1.Rows.Add(oRow)
        Next oMachineEntry

        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)

        Dim oDataView As DataView = New DataView(Table1)
        oDataView.Sort = "MACHINE_ID ASC"

        DataGridView1.SuspendLayout()
        DataGridView1.Columns.Clear()
        DataGridView1.AutoGenerateColumns = True
        DataGridView1.DataSource = oDataView
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            If Array.IndexOf(Config.Table1VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView1.Columns(i).HeaderText = Config.Table1VisibleFieldNamesTitles(sFieldName)
                DataGridView1.Columns(i).Width = MyUtility.GetTextWidth(Config.Table1VisibleFieldNamesCanonicalValues(sFieldName), DataGridView1.Font)
                DataGridView1.Columns(i).ReadOnly = True
                DataGridView1.Columns(i).Visible = ((Config.Table1VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
                If Config.FieldNamesTypes(sFieldName) Is GetType(DateTime) Then
                    DataGridView1.Columns(i).DefaultCellStyle.Format = Config.DateTimeFormatInGui
                ElseIf Config.FieldNamesTypes(sFieldName) IsNot GetType(String) Then
                    DataGridView1.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Else
                DataGridView1.Columns(i).Visible = False
            End If
        Next i
        DataGridView1.ResumeLayout()
    End Sub

    Protected Sub InitTable2WithoutFilter()
        '空のデータテーブルを作成し、フィールド名を設定する。
        Table2 = New DataTable()
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            Table2.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i
        AddExtraColumnsToTable2()

        'UiState.Machinesの基本情報をデータテーブルに展開する。
        For Each oMachineEntry As KeyValuePair(Of String, Machine) In UiState.Machines
            With Nothing
                Dim n As Integer = GetMonitorMachineRowCountForTable2(oMachineEntry.Value)
                For index As Integer = 0 To n - 1
                    Dim oRow As DataRow = Table2.NewRow()
                    SetProfileValueToTable2MonitorMachineRow(oRow, oMachineEntry.Key, oMachineEntry.Value)
                    SetExtraValueToTable2MonitorMachineRow(oRow, oMachineEntry.Value, index, n)
                    Table2.Rows.Add(oRow)
                Next index
            End With

            For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMachineEntry.Value.TermMachines
                Dim n As Integer = GetTermMachineRowCountForTable2(oTermEntry.Value)
                For index As Integer = 0 To n - 1
                    Dim oRow As DataRow = Table2.NewRow()
                    SetProfileValueToTable2TermMachineRow(oRow, oMachineEntry.Key, oTermEntry.Key, oTermEntry.Value)
                    SetExtraValueToTable2TermMachineRow(oRow, oTermEntry.Value, index, n)
                    Table2.Rows.Add(oRow)
                Next index
            Next oTermEntry
        Next oMachineEntry

        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)

        Dim oDataView As DataView = New DataView(Table2)
        oDataView.Sort = "MACHINE_ID ASC, TERM_MACHINE_ID ASC"

        DataGridView2.SuspendLayout()
        DataGridView2.Columns.Clear()
        DataGridView2.AutoGenerateColumns = True
        DataGridView2.DataSource = oDataView
        DataGridView2.AutoGenerateColumns = False
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If Array.IndexOf(Config.Table2VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView2.Columns(i).HeaderText = Config.Table2VisibleFieldNamesTitles(sFieldName)
                DataGridView2.Columns(i).Width = MyUtility.GetTextWidth(Config.Table2VisibleFieldNamesCanonicalValues(sFieldName), DataGridView2.Font)
                DataGridView2.Columns(i).ReadOnly = True
                DataGridView2.Columns(i).Frozen = True
                DataGridView2.Columns(i).Visible = ((Config.Table2VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
                If Config.FieldNamesTypes(sFieldName) Is GetType(DateTime) Then
                    'DataGridView2.Columns(i).DefaultCellStyle.Format = Config.DateTimeFormatInGui
                ElseIf Config.FieldNamesTypes(sFieldName) IsNot GetType(String) Then
                    DataGridView2.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                End If
            Else
                DataGridView2.Columns(i).Visible = False
                DataGridView2.Columns(i).Frozen = True
            End If
        Next i
        InitExtraColumnsViewOfTable2()
        DataGridView2.ResumeLayout()
    End Sub

    Protected Sub TuneTable2FilterToTable1Selection()
        Dim oBuilder As New StringBuilder()
        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                If oBuilder.Length <> 0 Then
                    oBuilder.Append(" Or ")
                End If
                oBuilder.Append("MACHINE_ID = '" & DirectCast(gridRow.Cells(idxColumn).Value, String) & "'")
            End If
        Next gridRow
        DirectCast(DataGridView2.DataSource, DataView).RowFilter = oBuilder.ToString()
        DataGridView2.Columns("MACHINE_ID").Visible = False
    End Sub

    Protected Sub UpdateTable2OnMonitorStateChanged(ByVal sMonitorMachineId As String, ByVal oMonitorMachine As Machine)
        Dim nextRowCount As Integer = GetMonitorMachineRowCountForTable2(oMonitorMachine)
        Dim oCurRows As DataRow() = Table2.Select("MACHINE_ID = '" & sMonitorMachineId & "' AND TERM_MACHINE_ID = ''")

        DataGridView2.SuspendLayout()
        If oCurRows.Length >= nextRowCount Then
            For index As Integer = 0 To nextRowCount - 1
                SetExtraValueToTable2MonitorMachineRow(oCurRows(index), oMonitorMachine, index, nextRowCount)
            Next index
            For index As Integer = nextRowCount To oCurRows.Length - 1
                oCurRows(index).Delete()
            Next index
        Else
            For index As Integer = 0 To oCurRows.Length - 1
                SetExtraValueToTable2MonitorMachineRow(oCurRows(index), oMonitorMachine, index, nextRowCount)
            Next index
            For index As Integer = oCurRows.Length To nextRowCount - 1
                Dim oRow As DataRow = Table2.NewRow()
                SetProfileValueToTable2MonitorMachineRow(oRow, sMonitorMachineId, oMonitorMachine)
                SetExtraValueToTable2MonitorMachineRow(oRow, oMonitorMachine, index, nextRowCount)
                Table2.Rows.Add(oRow)
            Next index
        End If
        DataGridView2.ResumeLayout()
    End Sub

    Protected Sub UpdateTable2OnTermStateChanged(ByVal sMonitorMachineId As String, ByVal sTermMachineId As String, ByVal oTermMachine As TermMachine)
        Dim nextRowCount As Integer = GetTermMachineRowCountForTable2(oTermMachine)
        Dim oCurRows As DataRow() = Table2.Select("TERM_MACHINE_ID = '" & sTermMachineId & "'")

        DataGridView2.SuspendLayout()
        If oCurRows.Length >= nextRowCount Then
            For index As Integer = 0 To nextRowCount - 1
                SetExtraValueToTable2TermMachineRow(oCurRows(index), oTermMachine, index, nextRowCount)
            Next index
            For index As Integer = nextRowCount To oCurRows.Length - 1
                oCurRows(index).Delete()
            Next index
        Else
            For index As Integer = 0 To oCurRows.Length - 1
                SetExtraValueToTable2TermMachineRow(oCurRows(index), oTermMachine, index, nextRowCount)
            Next index
            For index As Integer = oCurRows.Length To nextRowCount - 1
                Dim oRow As DataRow = Table2.NewRow()
                SetProfileValueToTable2TermMachineRow(oRow, sMonitorMachineId, sTermMachineId, oTermMachine)
                SetExtraValueToTable2TermMachineRow(oRow, oTermMachine, index, nextRowCount)
                Table2.Rows.Add(oRow)
            Next index
        End If
        DataGridView2.ResumeLayout()
    End Sub

    Protected Sub FetchMachineProfileFromFile(ByVal sMachineDirPath As String)
        Dim sMachineDirName As String = Path.GetFileName(sMachineDirPath)
        Dim sMachineId As String = GetMachineId(sMachineDirName)
        Dim oMachine As Machine = Nothing
        Dim oTable1Row As DataRow
        Dim d As DateTime = DateTime.Now
        Dim newMachineDetected As Boolean = False

        If UiState.Machines.TryGetValue(sMachineId, oMachine) = True Then
            oTable1Row = Table1.Select("MACHINE_ID = '" & sMachineId & "'")(0)
        Else
            Log.Info("新しい監視機器 [" & sMachineId & "] を検出しました。")
            oMachine = New Machine()
            oMachine.LatchConf = CByte(&H0)
            oTable1Row = Table1.NewRow()
            oTable1Row("MACHINE_ID") = sMachineId
            newMachineDetected = True
        End If

        oMachine.LastConfirmed = d
        oTable1Row("LAST_CONFIRMED") = oMachine.LastConfirmed

        With Nothing
            Dim sFile As String = Path.Combine(sMachineDirPath, "#Machine.csv")
            Dim t As DateTime = File.GetLastWriteTime(sFile)
            If oMachine.Profile Is Nothing OrElse t <> oMachine.ProfileTimestamp Then
                Dim oProfileTable As DataTable = GetMachineProfileTable(sFile)
                If oProfileTable.Rows.Count <> 1 Then
                    Throw New OPMGException("機器構成ファイルの行数が異常です。")
                End If
                If GetMachineId(oProfileTable.Rows(0)) <> sMachineId Then
                    Throw New OPMGException("機器構成ファイルの内容と作業ディレクトリ名に不整合があります。")
                End If
                oMachine.Profile = GetMachineProfile(oProfileTable.Rows(0))
                oMachine.ProfileTimestamp = t
                For i As Integer = 0 To Config.Table1FieldNames.Length - 1
                    Dim sFieldName As String = Config.Table1FieldNames(i)
                    If sFieldName <> "MACHINE_ID" AndAlso _
                       sFieldName <> "LAST_CONFIRMED" Then
                        oTable1Row(sFieldName) = oMachine.Profile(Config.MachineProfileFieldNamesIndices(sFieldName))
                    End If
                Next i
            End If
        End With

        If newMachineDetected Then
            Try
                InitMonitorUpboundData(sMachineId, oMachine)
            Catch ex As Exception
                Log.Fatal("Unwelcome Exception caught.", ex)
            End Try
        End If

        With Nothing
            Dim sFile As String = Path.Combine(sMachineDirPath, "#TermMachine.csv")
            Dim t As DateTime = File.GetLastWriteTime(sFile)
            If oMachine.TermMachines Is Nothing OrElse t <> oMachine.TermMachinesProfileTimestamp Then
                Dim oProfileTable As DataTable = GetMachineProfileTable(sFile)
                Dim oTermMachines As New Dictionary(Of String, TermMachine)(oProfileTable.Rows.Count)

                Dim residueCount As Integer = 0
                For Each oProfileRow As DataRow In oProfileTable.Rows
                    Dim sTermId As String = GetMachineId(oProfileRow)
                    Dim oTerm As TermMachine = Nothing
                    If oMachine.TermMachines.TryGetValue(sTermId, oTerm) = True Then
                        oTerm.Profile = GetMachineProfile(oProfileRow)
                        oTermMachines.Add(sTermId, oTerm)
                        residueCount += 1
                        Dim oTable2Rows As DataRow() = Table2.Select("TERM_MACHINE_ID = '" & sTermId & "'")
                        For Each oRow As DataRow In oTable2Rows
                            SetProfileValueToTable2TermMachineRow(oRow, sMachineId, sTermId, oTerm)
                        Next oRow
                    Else
                        Log.Info("新しい端末機器 [" & sTermId & "] を検出しました。")
                        oTerm = New TermMachine()
                        oTerm.Profile = GetMachineProfile(oProfileRow)
                        oTerm.LatchConf = If(oProfileRow.Field(Of String)("CORNER_NAME").Contains("乗換"), CByte(&H2), CByte(&H1))

                        Dim oExemplar As HoldingProgram = Nothing
                        If oMachine.HoldingPrograms(0) IsNot Nothing Then
                            'NOTE: 監視盤は一度改札機用プログラムを受け入れると、
                            'それを全ての改札機に適用するか、改造中止で捨てるかしない限り、
                            'それと異なるものは受け入れない。すなわち、改札機に監視盤が
                            '保持しているものと異なるものが適用されている状態というのは、
                            '想定外の状態であると言える。よって、監視盤の旧世代面に
                            'ものが存在する場合は、それが投入されている体の改札機を
                            '追加する。
                            oExemplar = oMachine.HoldingPrograms(0)
                        ElseIf oMachine.HoldingPrograms(1) IsNot Nothing Then
                            'NOTE: 監視盤は一度改札機用プログラムを受け入れると、
                            'それを全ての改札機に適用するか、改造中止で捨てるかしない限り、
                            'それと異なるものは受け入れない。すなわち、改札機に監視盤が
                            '保持しているものと異なるものが適用されている状態というのは、
                            '想定外の状態であると言える。よって、監視盤の新世代面に
                            'ものが存在する場合は、それが投入されている体の改札機を
                            '追加する。
                            oExemplar = oMachine.HoldingPrograms(1)
                        ElseIf oMachine.TermMachines.Count <> 0 Then
                            'NOTE: １つの監視盤の配下にある改札機のプログラムは、バージョンが
                            '揃っていることが基本である（揃っていないと、運管からの配信が不可能に
                            'なるケースもある）ため、既存の改札機と同じプログラムがインストール
                            'された体の改札機を追加する。
                            oExemplar = oMachine.TermMachines.Values(0).HoldingPrograms(0)
                        End If

                        oTerm.HoldingPrograms(0) = New HoldingProgram()
                        oTerm.HoldingPrograms(0).DataDeliverDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).ListDeliverDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).ApplyDate = Config.UnknownTime
                        If oExemplar IsNot Nothing Then
                            'NOTE: これは「保守機能による直接投入」の一種であるが、
                            'この後の配信でDL完了通知が発生しないのはあまりにも酷なので、
                            'ダミーの適用リストも投入しておく。
                            oTerm.HoldingPrograms(0).DataAcceptDate = oExemplar.DataAcceptDate
                            oTerm.HoldingPrograms(0).ListAcceptDate = oExemplar.ListAcceptDate
                            oTerm.HoldingPrograms(0).DataSubKind = oExemplar.DataSubKind
                            oTerm.HoldingPrograms(0).DataVersion = oExemplar.DataVersion
                            oTerm.HoldingPrograms(0).ListVersion = oExemplar.ListVersion
                            oTerm.HoldingPrograms(0).RunnableDate = oExemplar.RunnableDate
                            oTerm.HoldingPrograms(0).ApplicableDate = oExemplar.ApplicableDate
                            oTerm.HoldingPrograms(0).ModuleInfos = oExemplar.ModuleInfos
                            oTerm.HoldingPrograms(0).ArchiveCatalog = oExemplar.ArchiveCatalog
                            oTerm.HoldingPrograms(0).VersionListData = oExemplar.VersionListData
                            oTerm.HoldingPrograms(0).ListContent = oExemplar.ListContent
                            oTerm.HoldingPrograms(0).DataHashValue = oExemplar.DataHashValue
                            oTerm.HoldingPrograms(0).ListHashValue = oExemplar.ListHashValue
                        Else
                            oTerm.HoldingPrograms(0).DataAcceptDate = Config.UnknownTime
                            oTerm.HoldingPrograms(0).ListAcceptDate = Config.UnknownTime
                            oTerm.HoldingPrograms(0).DataSubKind = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                            oTerm.HoldingPrograms(0).DataVersion = 0
                            oTerm.HoldingPrograms(0).ListVersion = 0
                            oTerm.HoldingPrograms(0).RunnableDate = "00000000"
                            oTerm.HoldingPrograms(0).ApplicableDate = "00000000"
                            oTerm.HoldingPrograms(0).ModuleInfos = GetDummyModuleInfos()
                            oTerm.HoldingPrograms(0).ArchiveCatalog = ""
                            oTerm.HoldingPrograms(0).VersionListData = New Byte(ProgramVersionListUtil.RecordLengthInBytes - 1) {}
                            oTerm.HoldingPrograms(0).ListContent = ""
                            oTerm.HoldingPrograms(0).DataHashValue = Config.UnknownHashValue
                            oTerm.HoldingPrograms(0).ListHashValue = Config.UnknownHashValue
                        End If

                        Try
                            InitTermUpboundData(sMachineId, sTermId, oTerm)
                        Catch ex As Exception
                            Log.Fatal("Unwelcome Exception caught.", ex)
                        End Try

                        oTermMachines.Add(sTermId, oTerm)
                        Dim n As Integer = GetTermMachineRowCountForTable2(oTerm)
                        For index As Integer = 0 To n - 1
                            Dim oRow As DataRow = Table2.NewRow()
                            SetProfileValueToTable2TermMachineRow(oRow, sMachineId, sTermId, oTerm)
                            SetExtraValueToTable2TermMachineRow(oRow, oTerm, index, n)
                            Table2.Rows.Add(oRow)
                        Next index
                    End If
                Next oProfileRow

                If residueCount < oMachine.TermMachines.Count Then
                    For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMachine.TermMachines
                        If Not oTermMachines.ContainsKey(oTermEntry.Key) Then
                            Log.Warn("削除された端末 [" & oTermEntry.Key & "] の状態情報をクリアします。")
                            Dim oRows As DataRow() = Table2.Select("TERM_MACHINE_ID = '" & oTermEntry.Key & "'")
                            For Each oRow As DataRow In oRows
                                oRow.Delete()
                            Next oRow
                        End If
                    Next oTermEntry
                End If

                oMachine.TermMachines = oTermMachines
                oMachine.TermMachinesProfileTimestamp = t
            End If
        End With

        'NOTE: 端末機器の構成が変化しても、Table2の監視機器の行に、関連する項目は無い想定である。
        'よって、Table2の監視機器の行については、監視機器自体が追加されたケースでのみ、ケアする。
        If newMachineDetected Then
            'NOTE: これは「保守機能による直接投入」の一種であるが、
            'この後の配信でDL完了通知が発生しないのはあまりにも酷なので、
            'ダミーの適用リストも投入しておく。
            oMachine.HoldingKsbPrograms(0) = New HoldingKsbProgram()
            oMachine.HoldingKsbPrograms(0).DataAcceptDate = Config.UnknownTime
            oMachine.HoldingKsbPrograms(0).ListAcceptDate = Config.UnknownTime
            oMachine.HoldingKsbPrograms(0).DataDeliverDate = Config.UnknownTime
            oMachine.HoldingKsbPrograms(0).ListDeliverDate = Config.UnknownTime
            oMachine.HoldingKsbPrograms(0).ApplyDate = Config.UnknownTime
            oMachine.HoldingKsbPrograms(0).DataSubKind = DirectCast(oMachine.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
            oMachine.HoldingKsbPrograms(0).DataVersion = 0
            oMachine.HoldingKsbPrograms(0).ListVersion = 0
            oMachine.HoldingKsbPrograms(0).RunnableDate = "00000000"
            oMachine.HoldingKsbPrograms(0).ApplicableDate = "00000000"
            oMachine.HoldingKsbPrograms(0).ArchiveCatalog = ""
            oMachine.HoldingKsbPrograms(0).VersionListData = New Byte(ProgramVersionListUtil.RecordLengthInBytes - 1) {}
            oMachine.HoldingKsbPrograms(0).ListContent = ""
            oMachine.HoldingKsbPrograms(0).DataHashValue = Config.UnknownHashValue
            oMachine.HoldingKsbPrograms(0).ListHashValue = Config.UnknownHashValue
            UiState.Machines.Add(sMachineId, oMachine)
            Table1.Rows.Add(oTable1Row)
            Dim n As Integer = GetMonitorMachineRowCountForTable2(oMachine)
            For index As Integer = 0 To n - 1
                Dim oRow As DataRow = Table2.NewRow()
                SetProfileValueToTable2MonitorMachineRow(oRow, sMachineId, oMachine)
                SetExtraValueToTable2MonitorMachineRow(oRow, oMachine, index, n)
                Table2.Rows.Add(oRow)
            Next index
        End If
    End Sub

    Protected Sub InitMonitorUpboundData(ByVal sMonitorMachineId As String, ByVal oMonitor As Machine)
        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))

        oMonitor.FaultSeqNumber = 0UI
        oMonitor.FaultDate = Config.EmptyTime

        With Nothing
            Dim sFileName As String = "#FaultDataForPassiveUll.dat"
            Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
            File.Delete(sFilePath)
        End With

        With Nothing
            Dim sFileName As String = "#KadoData.dat"
            Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
            File.Delete(sFilePath)
        End With

        For Each oTerm As TermMachine In oMonitor.TermMachines.Values
            For k As Integer = 0 To 1
                oTerm.KadoSlot(k) = 0
            Next k
        Next oTerm
    End Sub

    Protected Sub InitTermUpboundData(ByVal sMonitorMachineId As String, ByVal sTermMachineId As String, ByVal oTerm As TermMachine)
        Dim isHokurikuMode As Boolean = GetStationOf(sTermMachineId).StartsWith("073")
        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))

        oTerm.FaultSeqNumber = 0UI
        oTerm.FaultDate = Config.EmptyTime

        For k As Integer = 0 To 1
            oTerm.KadoSeqNumber(k) = 0UI
            oTerm.KadoDate(k) = Config.EmptyTime
        Next k

        '稼動保守データ管理ファイルの当該レコードを初期化する。
        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes(0)
        Dim termEkCode As EkCode = GetEkCodeOf(sTermMachineId)

        Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
            Dim fileLen As Long = oOutputStream.Length
            Dim slotCount As Integer = If(fileLen < recLen, 1, CInt(fileLen \ recLen))

            For k As Integer = 0 To 1
                Dim oBytes As Byte() = New Byte(recLen - 1) {}
                If isHokurikuMode Then
                    KadoDataUtil073.InitBaseHeaderFields(k, termEkCode, now, 0UI, oBytes)
                    KadoDataUtil073.InitCommonPartFields(k, termEkCode, now, oBytes)
                Else
                    KadoDataUtil.InitBaseHeaderFields(k, termEkCode, now, 0UI, oBytes)
                    KadoDataUtil.InitCommonPartFields(k, termEkCode, now, oBytes)
                End If

                If oTerm.KadoSlot(k) = 0 Then
                    oTerm.KadoSlot(k) = slotCount
                    slotCount += 1
                End If

                'OPT: ここの実行は k = 1 の場合に限定してよい。
                'ファイルサイズが０の場合にここを省略しても、次のSeekによって、ファイルサイズが大きくなるはずである。
                oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                ExUpboundFileHeader.WriteToStream(&HA7, slotCount - 1, recLen, now, oOutputStream)

                oOutputStream.Seek(recLen * oTerm.KadoSlot(k), SeekOrigin.Begin)
                oOutputStream.Write(oBytes, 0, oBytes.Length)
            Next k
        End Using
    End Sub

    Private Sub ViewModeRadioButtons_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
       Handles ConStatusRadioButton.CheckedChanged, MasStatusRadioButton.CheckedChanged, _
               ProStatusRadioButton.CheckedChanged, KsbProStatusRadioButton.CheckedChanged, _
               KsbConfigRadioButton.CheckedChanged, UpboundProcStateRadioButton.CheckedChanged
        If DirectCast(sender, RadioButton).Checked AndAlso Table2 IsNot Nothing Then
            Dim sFilter As String = DirectCast(DataGridView2.DataSource, DataView).RowFilter
            InitTable2WithoutFilter()
            DirectCast(DataGridView2.DataSource, DataView).RowFilter = sFilter
        End If
    End Sub

    Private Sub SymbolizeCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SymbolizeCheckBox.CheckedChanged
        Dim visibleFieldKind As Integer = If(SymbolizeCheckBox.Checked, 2, 1)

        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()

        DataGridView1.SuspendLayout()
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            If Array.IndexOf(Config.Table1VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView1.Columns(i).Visible = ((Config.Table1VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
            Else
                DataGridView1.Columns(i).Visible = False
            End If
        Next i
        DataGridView1.ResumeLayout()

        DataGridView2.SuspendLayout()
        Dim curRow As Integer = -1
        Dim curCol As Integer = -1
        If DataGridView2.CurrentCell IsNot Nothing Then
            curRow = DataGridView2.CurrentCell.RowIndex
            curCol = DataGridView2.CurrentCell.ColumnIndex
        End If
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            If Array.IndexOf(Config.Table2VisibleFieldNames, sFieldName) <> -1 Then
                DataGridView2.Columns(i).Visible = ((Config.Table2VisibleFieldNamesKinds(sFieldName) And visibleFieldKind) <> 0)
            Else
                DataGridView2.Columns(i).Visible = False
            End If
        Next i
        If curCol <> -1 AndAlso Not DataGridView2.Columns(curCol).Visible Then
            If visibleFieldKind = 1 Then
                Do
                    curCol -= 1
                    If DataGridView2.Columns(curCol).Visible Then
                        DataGridView2.CurrentCell = DataGridView2.Rows(curRow).Cells(curCol)
                        Exit Do
                    End If
                Loop While curCol > 0
            ElseIf visibleFieldKind = 2 Then
                Do
                    curCol += 1
                    If DataGridView2.Columns(curCol).Visible Then
                        DataGridView2.CurrentCell = DataGridView2.Rows(curRow).Cells(curCol)
                        Exit Do
                    End If
                Loop While curCol < Config.Table2FieldNames.Length
            End If
        End If
        DataGridView2.ResumeLayout()

        TableSplitContainer.SplitterDistance _
           = DataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) _
            + SystemInformation.VerticalScrollBarWidth _
            + SystemInformation.BorderSize.Width * 2 _
            + TableSplitContainer.SplitterWidth - 1

        SplitContainer1.Panel1.ResumeLayout()
        SplitContainer1.Panel2.ResumeLayout()
        SplitContainer1.ResumeLayout()
    End Sub

    Private Sub DataGridView1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridView1.SelectionChanged
        If Table2 IsNot Nothing Then
            TuneTable2FilterToTable1Selection()
        End If
    End Sub

    Private Sub DataGridView1_UserDeletingRow(ByVal sender As System.Object, ByVal e As DataGridViewRowCancelEventArgs) Handles DataGridView1.UserDeletingRow
        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                UiState.Machines.Remove(sMachineId)
                Dim oRows As DataRow() = Table2.Select("MACHINE_ID = '" & sMachineId & "'")
                For Each oRow As DataRow In oRows
                    oRow.Delete()
                Next oRow
            End If
        Next gridRow
    End Sub

    Private Sub DataGridView1_UserDeletedRow(ByVal sender As System.Object, ByVal e As DataGridViewRowEventArgs) Handles DataGridView1.UserDeletedRow
        Table1.AcceptChanges()
    End Sub

    Private Sub DataGridView2_CellFormatting(ByVal sender As System.Object, ByVal e As DataGridViewCellFormattingEventArgs) Handles DataGridView2.CellFormatting
        If TypeOf DataGridView2.Columns(e.ColumnIndex) Is DataGridViewTextBoxColumn Then
            Dim oType As Type = e.Value.GetType()
            If oType Is GetType(Byte) Then
                e.Value = DirectCast(e.Value, Byte).ToString("X2")
                e.FormattingApplied = True
            ElseIf oType Is GetType(DateTime) Then
                Dim t As DateTime = DirectCast(e.Value, DateTime)
                If t.Equals(Config.EmptyTime) Then
                    e.Value = Lexis.EmptyTime.Gen()
                ElseIf t.Equals(Config.UnknownTime) Then
                    e.Value = Lexis.UnknownTime.Gen()
                Else
                    e.Value = t.ToString(Config.DateTimeFormatInGui)
                End If
                e.FormattingApplied = True
            End If
        End If
    End Sub

    Private Sub DataGridView2_CellParsing(ByVal sender As System.Object, ByVal e As DataGridViewCellParsingEventArgs) Handles DataGridView2.CellParsing
        If TypeOf DataGridView2.Columns(e.ColumnIndex) Is DataGridViewTextBoxColumn Then
            If e.DesiredType Is GetType(Byte) Then
                Try
                    e.Value = Byte.Parse(DirectCast(e.Value, String), NumberStyles.HexNumber)
                    e.ParsingApplied = True
                Catch ex As Exception
                    e.ParsingApplied = False
                End Try
            End If
        End If
    End Sub

    Private Sub DataGridView2_CellValueChanged(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles DataGridView2.CellValueChanged
        If e.RowIndex < 0 Then Return
        Dim oView As DataRowView = DirectCast(DataGridView2.Rows(e.RowIndex).DataBoundItem, DataRowView)
        oView.Row.AcceptChanges()
        FetchStateFromTable2Row(oView.Row)
    End Sub

    Private Sub DataGridView2_CellDoubleClick(ByVal sender As System.Object, ByVal e As DataGridViewCellEventArgs) Handles DataGridView2.CellDoubleClick
        If e.RowIndex < 0 Then Return
        If Not DataGridView2.Columns(e.ColumnIndex).ReadOnly Then Return

        If MasStatusRadioButton.Checked OrElse ProStatusRadioButton.Checked OrElse KsbProStatusRadioButton.Checked Then
            Dim sColName As String = DataGridView2.Columns(e.ColumnIndex).DataPropertyName
            If Config.MachineProfileFieldNamesIndices.ContainsKey(sColName) Then Return
            If sColName = "SLOT" Then Return
            Dim sDataKind As String = sColName.Substring(0, 3)

            Dim oView As DataRowView = DirectCast(DataGridView2.Rows(e.RowIndex).DataBoundItem, DataRowView)
            If oView.Row.IsNull(sDataKind & "_DataSubKind") Then Return
            Dim sMachineId As String = oView.Row.Field(Of String)("MACHINE_ID")
            Dim dataSubKind As Integer = oView.Row.Field(Of Integer)(sDataKind & "_DataSubKind")
            Dim dataVersion As Integer = oView.Row.Field(Of Integer)(sDataKind & "_DataVersion")

            Dim sColType As String = sColName.Substring(4)
            If sColType = "ListVersion" OrElse sColType = "ListAcceptDate" OrElse sColType = "ListDeliverDate" OrElse _
               sColType = "ApplicableDate"  OrElse sColType = "ListHashValue" Then
                If oView.Row(sDataKind & "_ListHashValue").GetType() IsNot GetType(String) Then Return

                Dim listVersion As Integer = oView.Row.Field(Of Integer)(sDataKind & "_ListVersion")
                Dim listAcceptDate As DateTime = oView.Row.Field(Of DateTime)(sDataKind & "_ListAcceptDate")
                Dim sListHashValue As String = oView.Row.Field(Of String)(sDataKind & "_ListHashValue")
                Dim sKey As String = sMachineId & "." & sDataKind & "." & dataSubKind.ToString() & "." & dataVersion.ToString() & "."  & listVersion.ToString() & "." & listAcceptDate.ToString("yyyyMMddHHmmssfff") & "." & sListHashValue
                Dim oForm As Form = Nothing
                If MasProListFormDic.TryGetValue(sKey, oForm) = True Then
                    oForm.Activate()
                Else
                    Dim sListContent As String = Nothing
                    Dim oMachine As Machine = UiState.Machines(sMachineId)
                    If MasStatusRadioButton.Checked Then
                        Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                        If sTermMachineId.Length = 0 Then
                            'NOTE: ダブルクリックされたのが監視機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                            '監視機器の行は、保持(1)と保持(2)のみであるため、上記の条件が成立しているなら、oMachine.HoldingMastersには
                            'sDataKindをキーとする要素が必ず存在している。
                            For Each oMas As HoldingMaster In oMachine.HoldingMasters(sDataKind)
                                If oMas IsNot Nothing AndAlso _
                                   oMas.DataSubKind = dataSubKind AndAlso _
                                   oMas.DataVersion = dataVersion AndAlso _
                                   oMas.ListVersion = listVersion AndAlso _
                                   oMas.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oMas.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oMas.ListContent
                                    Exit For
                                End If
                            Next oMas
                        Else
                            'NOTE: ダブルクリックされたのが端末機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                            '端末機器には、適用中の行と配信待ち(n)の行があるため、上記の条件が成立していても、（ダブルクリックされた
                            'のが配信待ち(n)の行なら）oTerm.HoldingMastersにsDataKindをキーとする要素が存在しているとは限らないし、
                            '（ダブルクリックされたのが適用中の行なら）oTerm.PendingMastersにsDataKindをキーとする要素が存在している
                            'とは限らない。
                            Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                            With Nothing
                                Dim oMas As HoldingMaster = Nothing
                                If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = True AndAlso _
                                   oMas.DataSubKind = dataSubKind AndAlso _
                                   oMas.DataVersion = dataVersion AndAlso _
                                   oMas.ListVersion = listVersion AndAlso _
                                   oMas.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oMas.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oMas.ListContent
                                End If
                            End With
                            If sListContent Is Nothing Then
                                Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                                If oTerm.PendingMasters.TryGetValue(sDataKind, oPendingMasters) = True Then
                                    For Each oMas As PendingMaster In oPendingMasters
                                        If oMas IsNot Nothing AndAlso _
                                           oMas.DataSubKind = dataSubKind AndAlso _
                                           oMas.DataVersion = dataVersion AndAlso _
                                           oMas.ListVersion = listVersion AndAlso _
                                           oMas.ListAcceptDate = listAcceptDate AndAlso _
                                           StringComparer.OrdinalIgnoreCase.Compare(oMas.ListHashValue, sListHashValue) = 0 Then
                                            sListContent = oMas.ListContent
                                            Exit For
                                        End If
                                    Next oMas
                                End If
                            End If
                        End If
                    ElseIf ProStatusRadioButton.Checked Then
                        Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                        If sTermMachineId.Length = 0 Then
                            For Each oPro As HoldingProgram In oMachine.HoldingPrograms
                                If oPro IsNot Nothing AndAlso _
                                   oPro.DataSubKind = dataSubKind AndAlso _
                                   oPro.DataVersion = dataVersion AndAlso _
                                   oPro.ListVersion = listVersion AndAlso _
                                   oPro.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oPro.ListContent
                                    Exit For
                                End If
                            Next oPro
                        Else
                            Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                            For Each oPro As HoldingProgram In oTerm.HoldingPrograms
                                If oPro IsNot Nothing AndAlso _
                                   oPro.DataSubKind = dataSubKind AndAlso _
                                   oPro.DataVersion = dataVersion AndAlso _
                                   oPro.ListVersion = listVersion AndAlso _
                                   oPro.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oPro.ListContent
                                    Exit For
                                End If
                            Next oPro
                            If sListContent Is Nothing Then
                                For Each oPro As PendingProgram In oTerm.PendingPrograms
                                    If oPro IsNot Nothing AndAlso _
                                       oPro.DataSubKind = dataSubKind AndAlso _
                                       oPro.DataVersion = dataVersion AndAlso _
                                       oPro.ListVersion = listVersion AndAlso _
                                       oPro.ListAcceptDate = listAcceptDate AndAlso _
                                       StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                        sListContent = oPro.ListContent
                                        Exit For
                                    End If
                                Next oPro
                            End If
                        End If
                    ElseIf KsbProStatusRadioButton.Checked Then
                        For Each oPro As HoldingKsbProgram In oMachine.HoldingKsbPrograms
                            If oPro IsNot Nothing AndAlso _
                               oPro.DataSubKind = dataSubKind AndAlso _
                               oPro.DataVersion = dataVersion AndAlso _
                               oPro.ListVersion = listVersion AndAlso _
                               oPro.ListAcceptDate = listAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                sListContent = oPro.ListContent
                                Exit For
                            End If
                        Next oPro
                        If sListContent Is Nothing Then
                            For Each oPro As PendingKsbProgram In oMachine.PendingKsbPrograms
                                If oPro IsNot Nothing AndAlso _
                                   oPro.DataSubKind = dataSubKind AndAlso _
                                   oPro.DataVersion = dataVersion AndAlso _
                                   oPro.ListVersion = listVersion AndAlso _
                                   oPro.ListAcceptDate = listAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oPro.ListHashValue, sListHashValue) = 0 Then
                                    sListContent = oPro.ListContent
                                    Exit For
                                End If
                            Next oPro
                        End If
                    End If
                    If sListContent IsNot Nothing Then
                        oForm = New ApplicableListForm(sMachineId, sDataKind, dataSubKind, dataVersion, listVersion, listAcceptDate, sListHashValue, sListContent, sKey, Me)
                        MasProListFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                End If
            Else
                Dim dataAcceptDate As DateTime = oView.Row.Field(Of DateTime)(sDataKind & "_DataAcceptDate")
                Dim sDataHashValue As String = oView.Row.Field(Of String)(sDataKind & "_DataHashValue")
                Dim sKey As String = sMachineId & "." & sDataKind & "." & dataSubKind.ToString() & "." & dataVersion.ToString() & "." & dataAcceptDate.ToString("yyyyMMddHHmmssfff") & "." & sDataHashValue

                Dim oForm As Form = Nothing
                If MasProDataFormDic.TryGetValue(sKey, oForm) = True Then
                    oForm.Activate()
                ElseIf MasStatusRadioButton.Checked Then
                    Dim oDataFooter As Byte() = Nothing
                    Dim oMachine As Machine = UiState.Machines(sMachineId)
                    Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                    If sTermMachineId.Length = 0 Then
                        'NOTE: ダブルクリックされたのが監視機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                        '監視機器の行は、保持(1)と保持(2)のみであるため、上記の条件が成立しているなら、oMachine.HoldingMastersには
                        'sDataKindをキーとする要素が必ず存在している。
                        For Each oMas As HoldingMaster In oMachine.HoldingMasters(sDataKind)
                            If oMas IsNot Nothing AndAlso _
                               oMas.DataSubKind = dataSubKind AndAlso _
                               oMas.DataVersion = dataVersion AndAlso _
                               oMas.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oMas.DataHashValue, sDataHashValue) = 0 Then
                                oDataFooter = oMas.DataFooter
                                Exit For
                            End If
                        Next oMas
                    Else
                        'NOTE: ダブルクリックされたのが端末機器の行であり、その行のsDataKindに関する列には値が存在している場合である。
                        '端末機器には、適用中の行と配信待ち(n)の行があるため、上記の条件が成立していても、（ダブルクリックされた
                        'のが配信待ち(n)の行なら）oTerm.HoldingMastersにsDataKindをキーとする要素が存在しているとは限らないし、
                        '（ダブルクリックされたのが適用中の行なら）oTerm.PendingMastersにsDataKindをキーとする要素が存在している
                        'とは限らない。
                        Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                        With Nothing
                            Dim oMas As HoldingMaster = Nothing
                            If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = True AndAlso _
                               oMas.DataSubKind = dataSubKind AndAlso _
                               oMas.DataVersion = dataVersion AndAlso _
                               oMas.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oMas.DataHashValue, sDataHashValue) = 0 Then
                                oDataFooter = oMas.DataFooter
                            End If
                        End With
                        If oDataFooter Is Nothing Then
                            Dim oPendingMasters As LinkedList(Of PendingMaster) = Nothing
                            If oTerm.PendingMasters.TryGetValue(sDataKind, oPendingMasters) = True Then
                                For Each oMas As PendingMaster In oPendingMasters
                                    If oMas IsNot Nothing AndAlso _
                                       oMas.DataSubKind = dataSubKind AndAlso _
                                       oMas.DataVersion = dataVersion AndAlso _
                                       oMas.DataAcceptDate = dataAcceptDate AndAlso _
                                       StringComparer.OrdinalIgnoreCase.Compare(oMas.DataHashValue, sDataHashValue) = 0 Then
                                        oDataFooter = oMas.DataFooter
                                        Exit For
                                    End If
                                Next oMas
                            End If
                        End If
                    End If
                    If oDataFooter IsNot Nothing Then
                        oForm = New GateMasDataForm(sMachineId, sDataKind, dataSubKind, dataVersion, dataAcceptDate, sDataHashValue, oDataFooter, sKey, Me)
                        MasProDataFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                ElseIf ProStatusRadioButton.Checked Then
                    Dim oModuleInfos As ProgramModuleInfo() = Nothing
                    Dim sArchiveCatalog As String = Nothing
                    Dim oVersionListData As Byte() = Nothing
                    Dim oMachine As Machine = UiState.Machines(sMachineId)
                    Dim sTermMachineId As String = oView.Row.Field(Of String)("TERM_MACHINE_ID")
                    If sTermMachineId.Length = 0 Then
                        For Each oPro As HoldingProgram In oMachine.HoldingPrograms
                            If oPro IsNot Nothing AndAlso _
                               oPro.DataSubKind = dataSubKind AndAlso _
                               oPro.DataVersion = dataVersion AndAlso _
                               oPro.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                                oModuleInfos = oPro.ModuleInfos
                                sArchiveCatalog = oPro.ArchiveCatalog
                                oVersionListData = oPro.VersionListData
                                Exit For
                            End If
                        Next oPro
                    Else
                        Dim oTerm As TermMachine = oMachine.TermMachines(sTermMachineId)
                        For Each oPro As HoldingProgram In oTerm.HoldingPrograms
                            If oPro IsNot Nothing AndAlso _
                               oPro.DataSubKind = dataSubKind AndAlso _
                               oPro.DataVersion = dataVersion AndAlso _
                               oPro.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                                oModuleInfos = oPro.ModuleInfos
                                sArchiveCatalog = oPro.ArchiveCatalog
                                oVersionListData = oPro.VersionListData
                                Exit For
                            End If
                        Next oPro
                        If oVersionListData Is Nothing Then
                            For Each oPro As PendingProgram In oTerm.PendingPrograms
                                If oPro IsNot Nothing AndAlso _
                                   oPro.DataSubKind = dataSubKind AndAlso _
                                   oPro.DataVersion = dataVersion AndAlso _
                                   oPro.DataAcceptDate = dataAcceptDate AndAlso _
                                   StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                                    oModuleInfos = oPro.ModuleInfos
                                    sArchiveCatalog = oPro.ArchiveCatalog
                                    oVersionListData = oPro.VersionListData
                                    Exit For
                                End If
                            Next oPro
                        End If
                    End If
                    If oVersionListData IsNot Nothing Then
                        oForm = New GateProDataForm(sMachineId, sDataKind, dataSubKind, dataVersion, dataAcceptDate, sDataHashValue, oModuleInfos, sArchiveCatalog, oVersionListData, sKey, Me)
                        MasProDataFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                ElseIf KsbProStatusRadioButton.Checked Then
                    Dim sArchiveCatalog As String = Nothing
                    Dim oVersionListData As Byte() = Nothing
                    Dim oMachine As Machine = UiState.Machines(sMachineId)
                    For Each oPro As HoldingKsbProgram In oMachine.HoldingKsbPrograms
                        If oPro IsNot Nothing AndAlso _
                           oPro.DataSubKind = dataSubKind AndAlso _
                           oPro.DataVersion = dataVersion AndAlso _
                           oPro.DataAcceptDate = dataAcceptDate AndAlso _
                           StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                            sArchiveCatalog = oPro.ArchiveCatalog
                            oVersionListData = oPro.VersionListData
                            Exit For
                        End If
                    Next oPro
                    If oVersionListData Is Nothing Then
                        For Each oPro As PendingKsbProgram In oMachine.PendingKsbPrograms
                            If oPro IsNot Nothing AndAlso _
                               oPro.DataSubKind = dataSubKind AndAlso _
                               oPro.DataVersion = dataVersion AndAlso _
                               oPro.DataAcceptDate = dataAcceptDate AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oPro.DataHashValue, sDataHashValue) = 0 Then
                                sArchiveCatalog = oPro.ArchiveCatalog
                                oVersionListData = oPro.VersionListData
                                Exit For
                            End If
                        Next oPro
                    End If
                    If oVersionListData IsNot Nothing Then
                        oForm = New KsbProDataForm(sMachineId, sDataKind, dataSubKind, dataVersion, dataAcceptDate, sDataHashValue, sArchiveCatalog, oVersionListData, sKey, Me)
                        MasProDataFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                End If
            End If
        ElseIf UpboundProcStateRadioButton.Checked Then
            Dim oView As DataRowView = DirectCast(DataGridView2.Rows(e.RowIndex).DataBoundItem, DataRowView)
            Dim oRow As DataRow = oView.Row
            Dim sMonitorMachineId As String = oRow.Field(Of String)("MACHINE_ID")
            Dim sTermMachineId As String = oRow.Field(Of String)("TERM_MACHINE_ID")
            Dim sColName As String = DataGridView2.Columns(e.ColumnIndex).DataPropertyName
            Select Case sColName
                Case "FAULT_SEQ_NO", "FAULT_DATE"
                    Dim oForm As FaultDataForm = Nothing
                    Dim sTargetMachineId As String = If(sTermMachineId.Length = 0, sMonitorMachineId, sTermMachineId)
                    Dim sKey As String = sMonitorMachineId & sTargetMachineId
                    If FaultDataFormDic.TryGetValue(sKey, oForm) = True Then
                        oForm.Activate()
                    Else
                        oForm = New FaultDataForm(sMonitorMachineId, sTargetMachineId, Me)
                        FaultDataFormDic.Add(sKey, oForm)
                        oForm.Show()
                    End If
                Case "KADO_SEQ_NO", "KADO_DATE", "HOSYU_SEQ_NO", "HOSYU_DATE"
                    If sTermMachineId.Length = 0 Then Return

                    Dim oForm As KadoDataForm = Nothing
                    If KadoDataFormDic.TryGetValue(sMonitorMachineId & sTermMachineId, oForm) = True Then
                        oForm.Activate()
                    Else
                        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
                            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
                            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                                Return
                            End If
                        End If

                        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
                        Dim sMonitorMachineDir As String
                        Try
                            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
                            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                            If Not Directory.Exists(sMonitorMachineDir) Then
                                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                                Return
                            End If
                        Catch ex As Exception
                            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
                            Return
                        End Try

                        Dim sFileName As String = "#KadoData.dat"
                        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)

                        oForm = New KadoDataForm(sMonitorMachineId, sTermMachineId, sFilePath, Me)
                        KadoDataFormDic.Add(sMonitorMachineId & sTermMachineId, oForm)
                        oForm.Show()
                    End If
           End Select
        End If
    End Sub

    Private Sub MachineProfileFetchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MachineProfileFetchButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
        End If

        If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        Dim aDirectoryInfo As DirectoryInfo()
        Try
            Dim oDirInfo As New DirectoryInfo(Path.Combine(SimWorkingDirDialog.SelectedPath, Config.ModelPathInSimWorkingDir))
            aDirectoryInfo = oDirInfo.GetDirectories(MachineDirPattern)
        Catch ex As Exception
            Log.Error("Exception caught.", ex)
            AlertBox.Show(Lexis.InvalidDirectorySpecified)
            Return
        End Try

        For Each oDirectoryInfo As DirectoryInfo In aDirectoryInfo
            If Not MachineDirRegx.IsMatch(oDirectoryInfo.Name) Then Continue For
            Try
                FetchMachineProfileFromFile(oDirectoryInfo.FullName)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        Next oDirectoryInfo

        AlertBox.Show(Lexis.MachineProfileFetchFinished)
    End Sub

    Private Sub MasClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasClearButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] と配下の全端末から、マスタおよびマスタ適用リストを削除します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                If ClearGateMas(sContextDir) = True Then
                    Dim oVerInfoParams As Object() = { _
                        &HAF, _
                        "M_G_%T3R%T3S%T4C%T2UVER.DAT", _
                        Path.Combine(sContextDir, "GateMasVerInfo_%T3R%T3S%T4C%T2U.dat"), _
                        "", _
                        0, _
                        60000, _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
                End If
            End If
        Next gridRow
    End Sub

    Private Sub MasDeliverButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MasDeliverButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] より、配信待ちの全マスタを配信します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                If DeliverGateMas(sContextDir) = True Then
                    Dim oDlReflectParams As Object() = { _
                        Path.Combine(sMonitorMachineDir, "#GateMasDlReflectReq_%T3R%T3S%T4C%T2U_*.dat"), _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

                    Dim oVerInfoParams As Object() = { _
                        &HAF, _
                        "M_G_%T3R%T3S%T4C%T2UVER.DAT", _
                        Path.Combine(sContextDir, "GateMasVerInfo_%T3R%T3S%T4C%T2U.dat"), _
                        "", _
                        0, _
                        60000, _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
                End If
            End If
        Next gridRow
    End Sub

    Private Sub ProDirectInstallButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProDirectInstallButton.Click
        Dim oDialog As New OpenFileDialog
        oDialog.Filter = "CABファイル|*.cab"
        oDialog.FileName = ""
        oDialog.ReadOnlyChecked = True
        oDialog.Title = "投入するプログラムを選択してください。"
        If oDialog.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")

        'NOTE: 以下、少し汚いが、複数機器が選択されている場合の速度性能を優先して、
        'InstallGateProgramDirectlyの中ではなく、呼び元でCABの解析を行うことにしている。

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(oDialog.FileName)
        Catch ex As Exception
            Log.Error("プログラム本体のハッシュ値算出で例外が発生しました。", ex)
            Return
        End Try

        Dim content As GateProgramContent
        Try
            Dim sMonitorMachineId As String = DirectCast(DataGridView1.CurrentRow.Cells(idxColumn).Value, String)
            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error("代表機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            content = ExtractGateProgramCab(oDialog.FileName, Path.Combine(sContextDir, "GatePro"))
        Catch ex As Exception
            Log.Error("プログラム本体の解析で例外が発生しました。", ex)
            Return
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("バージョンリストからのエリアNoの抽出で例外が発生しました。", ex)
            Return
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("バージョンリストからの代表Verの抽出で例外が発生しました。", ex)
            Return
        End Try

        'TODO: 自動改札機システムでは、監視盤配下の改札機は、全て同一エリアに所属しているはずなので、
        'ここで、部材のエリアと監視盤が管理している改札機エリアの整合性チェックを行うことも可能と思われる。
        'もし実機がチェックを行うなら、それに合わせた方がよい。
        'おそらく、自動改札機のHW自体は、どのエリアの改札機プログラムもインストール可能であり、
        '直接投入においてまでそれを妨げることは無いと思われるが。

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("プログラムの動作許可日が未来日に設定されています。" & content.RunnableDate & "まで適用できませんのでご注意ください。")
        End If

        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] から、改札機プログラムを直接投入します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                InstallGateProgramDirectly(sContextDir, subKind, version, content, sHashValue)

                'Dim oDlReflectParams As Object() = { _
                '    Path.Combine(sMonitorMachineDir, "#GateProDlReflectReq_%T3R%T3S%T4C%T2U_*.dat"), _
                '    60000, _
                '    60000, _
                '    0, _
                '    3, _
                '    True}
                'SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

                Dim oVerInfoParams As Object() = { _
                    &HAD, _
                    "P_G_%T3R%T3S%T4C%T2UVER.DAT", _
                    Path.Combine(sContextDir, "GateProVerInfo_%T3R%T3S%T4C%T2U.dat"), _
                    "", _
                    0, _
                    60000, _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub ProDeliverButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProDeliverButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] より、配信待ちの全改札機プログラムを配信します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                If DeliverGatePro(sContextDir) = True Then
                    Dim oDlReflectParams As Object() = { _
                        Path.Combine(sMonitorMachineDir, "#GateProDlReflectReq_%T3R%T3S%T4C%T2U_*.dat"), _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

                    Dim oVerInfoParams As Object() = { _
                        &HAD, _
                        "P_G_%T3R%T3S%T4C%T2UVER.DAT", _
                        Path.Combine(sContextDir, "GateProVerInfo_%T3R%T3S%T4C%T2U.dat"), _
                        "", _
                        0, _
                        60000, _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
                End If
            End If
        Next gridRow
    End Sub

    Private Sub ProApplyButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProApplyButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] 配下の全端末において、適用待ちのプログラムを適用します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                If ApplyGatePro(sContextDir) = True Then
                    Dim oVerInfoParams As Object() = { _
                        &HAD, _
                        "P_G_%T3R%T3S%T4C%T2UVER.DAT", _
                        Path.Combine(sContextDir, "GateProVerInfo_%T3R%T3S%T4C%T2U.dat"), _
                        "", _
                        0, _
                        60000, _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
                End If
            End If
        Next gridRow
    End Sub

    Private Sub KsbProDirectInstallButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KsbProDirectInstallButton.Click
        Dim oDialog As New OpenFileDialog
        oDialog.Filter = "CABファイル|*.cab"
        oDialog.FileName = ""
        oDialog.ReadOnlyChecked = True
        oDialog.Title = "投入するプログラムを選択してください。"
        If oDialog.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")

        'NOTE: 以下、少し汚いが、複数機器が選択されている場合の速度性能を優先して、
        'InstallKsbProgramDirectlyの中ではなく、呼び元でCABの解析を行うことにしている。

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(oDialog.FileName)
        Catch ex As Exception
            Log.Error("プログラム本体のハッシュ値算出で例外が発生しました。", ex)
            Return
        End Try

        Dim content As KsbProgramContent
        Try
            Dim sMonitorMachineId As String = DirectCast(DataGridView1.CurrentRow.Cells(idxColumn).Value, String)
            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error("代表機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            content = ExtractKsbProgramCab(oDialog.FileName, Path.Combine(sContextDir, "KsbPro"))
        Catch ex As Exception
            Log.Error("プログラム本体の解析で例外が発生しました。", ex)
            Return
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("バージョンリストからのエリアNoの抽出で例外が発生しました。", ex)
            Return
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("バージョンリストからの代表Verの抽出で例外が発生しました。", ex)
            Return
        End Try

        'TODO: ここで、部材のエリアと監視盤が認識している監視盤エリアの整合性チェックを行うことも可能と思われる。
        'もし実機がチェックを行うなら、それに合わせた方がよい。

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("プログラムの動作許可日が未来日に設定されています。" & content.RunnableDate & "まで適用できませんのでご注意ください。")
        End If

        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] から、監視盤プログラムを直接投入します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                InstallKsbProgramDirectly(sContextDir, subKind, version, content, sHashValue)

                'Dim oDlReflectParams As Object() = { _
                '    Path.Combine(sMonitorMachineDir, "#KsbProDlReflectReq_%3R%3S%4C%2U_*.dat"), _
                '    60000, _
                '    60000, _
                '    0, _
                '    3, _
                '    True}
                'SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

                Dim oVerInfoParams As Object() = { _
                    &HAE, _
                    "P_W_%3R%3S%4C%2UVER.DAT", _
                    Path.Combine(sContextDir, "KsbProVerInfo_%3R%3S%4C%2U.dat"), _
                    "", _
                    0, _
                    60000, _
                    60000, _
                    60000, _
                    0, _
                    3, _
                    True}
                SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub KsbProDeliverButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KsbProDeliverButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] より、配信待ちの監視盤プログラムを配信します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                If DeliverKsbPro(sContextDir) = True Then
                    Dim oDlReflectParams As Object() = { _
                        Path.Combine(sMonitorMachineDir, "#KsbProDlReflectReq_%3R%3S%4C%2U_*.dat"), _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveOne", oDlReflectParams, sSimWorkingDir, sMonitorMachineId)

                    Dim oVerInfoParams As Object() = { _
                        &HAE, _
                        "P_W_%3R%3S%4C%2UVER.DAT", _
                        Path.Combine(sContextDir, "KsbProVerInfo_%3R%3S%4C%2U.dat"), _
                        "", _
                        0, _
                        60000, _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
                End If
            End If
        Next gridRow
    End Sub

    Private Sub KsbProApplyButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KsbProApplyButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] において、適用待ちのプログラムを適用します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                If ApplyKsbPro(sContextDir) = True Then
                    Dim oVerInfoParams As Object() = { _
                        &HAE, _
                        "P_W_%3R%3S%4C%2UVER.DAT", _
                        Path.Combine(sContextDir, "KsbProVerInfo_%3R%3S%4C%2U.dat"), _
                        "", _
                        0, _
                        60000, _
                        60000, _
                        60000, _
                        0, _
                        3, _
                        True}
                    SendSimFuncMessage("TryActiveUll", oVerInfoParams, sSimWorkingDir, sMonitorMachineId)
                End If
            End If
        Next gridRow
    End Sub

    Private Sub ConStatusSendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConStatusSendButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(sModelDir)

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "選択中の監視機器 [" & sMonitorMachineId & "] より、接続状態を送信します...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                    Continue For
                End If

                Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
                Directory.CreateDirectory(sContextDir)

                If CreateConStatus(sContextDir) = True Then
                    Dim oConStatusParams As Object() = { _
                        Path.Combine(sContextDir, "ConStatusPostReq.dat"), _
                        60000, _
                        0, _
                        0, _
                        0, _
                        True}
                    SendSimFuncMessage("ActiveOne", oConStatusParams, sSimWorkingDir, sMonitorMachineId)
                End If
            End If
        Next gridRow
    End Sub

    Private Sub UpboundDataClearButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpboundDataClearButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                ClearUpboundData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub RandFaultDataStoreButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RandFaultDataStoreButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                StoreRandFaultData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub RandFaultDataSendButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RandFaultDataSendButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                SendRandFaultData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub KadoDataRandUpdateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KadoDataRandUpdateButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                UpdateKadoDataRandomly(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub KadoDataCommitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KadoDataCommitButton.Click
        If SimWorkingDirDialog.SelectedPath.Length = 0 Then
            SimWorkingDirDialog.SelectedPath = Environment.CurrentDirectory
            If SimWorkingDirDialog.ShowDialog() <> DialogResult.OK Then
                Return
            End If
        End If

        Dim idxColumn As Integer = Array.IndexOf(Config.Table1FieldNames, "MACHINE_ID")
        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                CommitKadoData(sMonitorMachineId)
            End If
        Next gridRow
    End Sub

    Private Sub InputQueue_ReceiveCompleted(ByVal sender As System.Object, ByVal e As System.Messaging.ReceiveCompletedEventArgs) Handles InputQueue.ReceiveCompleted
        Dim sTargetMachineId As String = Nothing
        Dim isProcCompleted As Boolean = False
        Dim sResult As String = Nothing
        Try
            Dim bd As ExtAppFuncMessageBody = DirectCast(e.Message.Body, ExtAppFuncMessageBody)
            Dim sContextDir As String = bd.WorkingDirectory

            If String.IsNullOrEmpty(sContextDir) OrElse String.IsNullOrEmpty(bd.Func) Then
                Log.Error("不正な要求を受信しました。" & vbCrLf & "WorkingDirectory: " & If(sContextDir Is Nothing, "Nothing", "[" & sContextDir & "]") & vbCrLf & "Func: " & If(bd.Func Is Nothing, "Nothing",  "[" & bd.Func & "]"))
            End If

            Try
                Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
                sTargetMachineId = GetMachineId(Path.GetFileName(sMachineDir))
                FetchMachineProfileFromFile(sMachineDir)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                Log.Error("不正な要求を受信しました。" & vbCrLf & "WorkingDirectory: [" & sContextDir & "]" & vbCrLf & "Func: [" & bd.Func & "]")
                Exit Try
            End Try

            Log.Info(sTargetMachineId, "[" & sContextDir & "] に対する要求 [" & bd.Func & "] を処理します...")
            Select Case bd.Func.ToUpperInvariant()
                'TODO: bd.Argsの件数（0の場合はNothingであること）をチェックする。
                Case "CreateConStatus".ToUpperInvariant()
                    isProcCompleted = CreateConStatus(sContextDir)
                Case "CreateGateMasVerInfo".ToUpperInvariant()
                    isProcCompleted = CreateGateMasVerInfo(sContextDir)
                Case "CreateGateProVerInfo".ToUpperInvariant()
                    isProcCompleted = CreateGateProVerInfo(sContextDir)
                Case "CreateKsbProVerInfo".ToUpperInvariant()
                    isProcCompleted = CreateKsbProVerInfo(sContextDir)
                Case "ClearGateMas".ToUpperInvariant()
                    isProcCompleted = ClearGateMas(sContextDir)
                Case "AcceptGateMas".ToUpperInvariant()
                    isProcCompleted = AcceptGateMas(sContextDir, sResult)
                Case "DeliverGateMas".ToUpperInvariant()
                    isProcCompleted = DeliverGateMas(sContextDir)
                Case "DirectInstallGatePro".ToUpperInvariant()
                    isProcCompleted = DirectInstallGatePro(sContextDir, bd.Args(0))
                Case "AcceptGatePro".ToUpperInvariant()
                    isProcCompleted = AcceptGatePro(sContextDir, sResult)
                Case "DeliverGatePro".ToUpperInvariant()
                    isProcCompleted = DeliverGatePro(sContextDir)
                Case "ApplyGatePro".ToUpperInvariant()
                    isProcCompleted = ApplyGatePro(sContextDir)
                Case "DirectInstallKsbPro".ToUpperInvariant()
                    isProcCompleted = DirectInstallKsbPro(sContextDir, bd.Args(0))
                Case "AcceptKsbPro".ToUpperInvariant()
                    isProcCompleted = AcceptKsbPro(sContextDir, sResult)
                Case "DeliverKsbPro".ToUpperInvariant()
                    isProcCompleted = DeliverKsbPro(sContextDir)
                Case "ApplyKsbPro".ToUpperInvariant()
                    isProcCompleted = ApplyKsbPro(sContextDir)
                Case "ClearUpboundData".ToUpperInvariant()
                    isProcCompleted = ClearUpboundData(sTargetMachineId)
                Case "StoreRandFaultData".ToUpperInvariant()
                    isProcCompleted = StoreRandFaultData(sTargetMachineId)
                Case "SendRandFaultData".ToUpperInvariant()
                    isProcCompleted = SendRandFaultData(sTargetMachineId)
                Case "UpdateKadoDataRandomly".ToUpperInvariant()
                    isProcCompleted = UpdateKadoDataRandomly(sTargetMachineId)
                Case "CommitKadoData".ToUpperInvariant()
                    isProcCompleted = CommitKadoData(sTargetMachineId)
                Case Else
                    Log.Error(sTargetMachineId, "未知の要求です。")
                    isProcCompleted = False
            End Select
        Catch ex As Exception
            Log.Error(sTargetMachineId, "Exception caught.", ex)
        Finally
            SendResponseMessage(e.Message, isProcCompleted, sResult, sTargetMachineId)
        End Try

        Try
            InputQueue.BeginReceive()
        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
        End Try
    End Sub

    Protected Function CreateStringOfContinuousPassiveDllReq(ByVal slot1Ver As Integer, ByVal slot2Ver As Integer, ByVal fullFlag As Integer, ByVal sContinueCode As String) As String
        Return sContinueCode & ";" & slot1Ver.ToString() & ";" & slot2Ver.ToString() & ";" & fullFlag.ToString()
    End Function

    Protected Sub CreateFileOfGateMasVerInfo(ByVal sMonitorMachineId As String, ByVal sTermId As String, ByVal oTermMachine As TermMachine, ByVal sContextDir As String)
        Dim sFileName As String = _
           "GateMasVerInfo_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & _
           ".dat"
        Dim sFilePath As String = Path.Combine(sContextDir, sFileName)
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            Dim t As DateTime = DateTime.Now
            ExVersionInfoFileHeader.WriteToStream(&HAF, GetEkCodeOf(sTermId), t, 1, oOutputStream)
            ExMasterVersionInfo.WriteToStream(oTermMachine.HoldingMasters, oOutputStream)
        End Using
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を作成しました。")
    End Sub

    Protected Sub CreateFileOfGateProVerInfo(ByVal sMonitorMachineId As String, ByVal sTermId As String, ByVal oTermMachine As TermMachine, ByVal sContextDir As String)
        'NOTE: 改札機に必要な適用リストは、これから適用する（受信は済んだが
        'まだ適用していない）プログラムに対する適用リストだけであると思われる。
        'よって、oTermMachine.HoldingPrograms(1)が存在しない場合は、
        '適用リストを無しとする（リストバージョン0をセットする）。
        'TODO: 本物の改札機（監視盤？）が何をセットするのか確認すべき。
        'Dim listVer As Integer = oTermMachine.HoldingPrograms(0).ListVersion
        Dim listVer As Integer = 0
        If oTermMachine.HoldingPrograms(1) IsNot Nothing Then
            'NOTE: 直接投入の直後でも、ListVersionには特定の値（0）がセットされており、
            'それをそのまま送信することにする。
            listVer = oTermMachine.HoldingPrograms(1).ListVersion
        End If

        Dim sFileName As String = _
           "GateProVerInfo_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & _
           ".dat"
        Dim sFilePath As String = Path.Combine(sContextDir, sFileName)
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            Dim t As DateTime = DateTime.Now
            ExVersionInfoFileHeader.WriteToStream(&HAD, GetEkCodeOf(sTermId), t, 1, oOutputStream)
            oOutputStream.Write(New Byte(0) {CType(listVer, Byte)}, 0, 1)
            ExProgramVersionInfoForG.WriteToStream(oTermMachine.HoldingPrograms(0), oOutputStream)
            ExProgramVersionInfoForG.WriteToStream(oTermMachine.HoldingPrograms(1), oOutputStream)
        End Using
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を作成しました。")
    End Sub

    Protected Sub CreateFileOfKsbProVerInfo(ByVal sMachineId As String, ByVal oMachine As Machine, ByVal sContextDir As String)
        'NOTE: 監視盤が自身の管理に必要とする適用リストは、これから適用する（受信は済んだが
        'まだ適用していない）プログラムに対する適用リストだけであると思われる。
        'よって、oMachine.HoldingKsbPrograms(1)が存在しない場合は、
        '適用リストを無しとする（リストバージョン0をセットする）。
        'TODO: 本物の改札機（監視盤？）が何をセットするのか確認すべき。
        'Dim listVer As Integer = oMachine.HoldingKsbPrograms(0).ListVersion
        Dim listVer As Integer = 0
        If oMachine.HoldingKsbPrograms(1) IsNot Nothing Then
            'NOTE: 直接投入の直後でも、ListVersionには特定の値（0）がセットされており、
            'それをそのまま送信することにする。
            listVer = oMachine.HoldingKsbPrograms(1).ListVersion
        End If

        Dim sFileName As String = _
           "KsbProVerInfo_" & _
           GetStationOf(sMachineId) & GetCornerOf(sMachineId) & GetUnitOf(sMachineId) & _
           ".dat"
        Dim sFilePath As String = Path.Combine(sContextDir, sFileName)
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            Dim t As DateTime = DateTime.Now
            ExVersionInfoFileHeader.WriteToStream(&HAE, GetEkCodeOf(sMachineId), t, 1, oOutputStream)
            oOutputStream.Write(New Byte(0) {CType(listVer, Byte)}, 0, 1)
            ExProgramVersionInfoForW.WriteToStream(oMachine.HoldingKsbPrograms(0), oOutputStream)
            ExProgramVersionInfoForW.WriteToStream(oMachine.HoldingKsbPrograms(1), oOutputStream)
        End Using
        Log.Info(sMachineId, "ファイル [" & sFilePath & "] を作成しました。")
    End Sub

    Protected Sub CreateFileOfGateMasDlReflectReq( _
       ByVal objCode As Byte, _
       ByVal subObjCode As Byte, _
       ByVal subKind As Integer, _
       ByVal version As Integer, _
       ByVal deliveryResult As Byte, _
       ByVal sMonitorMachineId As String, _
       ByVal sTermId As String, _
       ByVal sMachineDir As String)
        Dim oTeleg As New EkMasProDlReflectReqTelegram(TelegGene, objCode, subObjCode, subKind, version, GetEkCodeOf(sTermId), deliveryResult, 0)
        Dim sOddFileName As String = _
           "#GateMasDlReflectReq_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & "_"
        Dim sOddFilePath As String = Path.Combine(sMachineDir, sOddFileName)

        Dim branchNum As Integer = -1
        Do
            branchNum += 1
            Dim sFilePath As String = sOddFilePath & branchNum.ToString() & ".dat"
            If File.Exists(sFilePath) Then Continue Do

            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oTeleg.WriteToStream(oOutputStream)
            End Using
            Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を配信結果 [" & deliveryResult.ToString("X2") & "] で作成しました。")
            Exit Do
        Loop
    End Sub

    Protected Sub CreateFileOfGateProDlReflectReq( _
       ByVal objCode As Byte, _
       ByVal version As Integer, _
       ByVal deliveryResult As Byte, _
       ByVal sMonitorMachineId As String, _
       ByVal sTermId As String, _
       ByVal sMachineDir As String)
        Dim oTeleg As New EkMasProDlReflectReqTelegram(TelegGene, objCode, 0, 0, version, GetEkCodeOf(sTermId), deliveryResult, 0)
        Dim sOddFileName As String = _
           "#GateProDlReflectReq_" & _
           GetStationOf(sTermId) & GetCornerOf(sTermId) & GetUnitOf(sTermId) & "_"
        Dim sOddFilePath As String = Path.Combine(sMachineDir, sOddFileName)

        Dim branchNum As Integer = -1
        Do
            branchNum += 1
            Dim sFilePath As String = sOddFilePath & branchNum.ToString() & ".dat"
            If File.Exists(sFilePath) Then Continue Do

            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oTeleg.WriteToStream(oOutputStream)
            End Using
            Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を配信結果 [" & deliveryResult.ToString("X2") & "] で作成しました。")
            Exit Do
        Loop
    End Sub

    Protected Sub CreateFileOfKsbProDlReflectReq( _
       ByVal objCode As Byte, _
       ByVal version As Integer, _
       ByVal deliveryResult As Byte, _
       ByVal sMachineId As String, _
       ByVal sMachineDir As String)
        Dim oTeleg As New EkMasProDlReflectReqTelegram(TelegGene, objCode, 0, 0, version, GetEkCodeOf(sMachineId), deliveryResult, 0)
        Dim sOddFileName As String = _
           "#KsbProDlReflectReq_" & _
           GetStationOf(sMachineId) & GetCornerOf(sMachineId) & GetUnitOf(sMachineId) & "_"
        Dim sOddFilePath As String = Path.Combine(sMachineDir, sOddFileName)

        Dim branchNum As Integer = -1
        Do
            branchNum += 1
            Dim sFilePath As String = sOddFilePath & branchNum.ToString() & ".dat"
            If File.Exists(sFilePath) Then Continue Do

            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oTeleg.WriteToStream(oOutputStream)
            End Using
            Log.Info(sMachineId, "ファイル [" & sFilePath & "] を配信結果 [" & deliveryResult.ToString("X2") & "] で作成しました。")
            Exit Do
        Loop
    End Sub

    Protected Function CreateFileOfFaultDataPostReq( _
       ByVal oContents As Byte(), _
       ByVal sMachineDir As String, _
       ByVal sMonitorMachineId As String) As String
        Dim oTeleg As New EkByteArrayPostReqTelegram(TelegGene, EkByteArrayPostReqTelegram.FormalObjCodeAsKsbGateFaultData, oContents, 0)
        Dim sOddFileName As String = "#FaultDataPostReq_"
        Dim sOddFilePath As String = Path.Combine(sMachineDir, sOddFileName)

        Dim sFilePath As String
        Dim branchNum As Integer = -1
        Do
            branchNum += 1
            sFilePath = sOddFilePath & branchNum.ToString() & ".dat"
        Loop While File.Exists(sFilePath)

        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
                oTeleg.WriteToStream(oOutputStream)
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "ファイル [" & sFilePath & "] の作成が失敗しました。", ex)
            Return Nothing
        End Try
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を作成しました。")
        Return sFilePath
    End Function

    Protected Function CreateConStatus(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachinesに設定されている接続状態をもとに、
        'sContextDirにConStatus.datを作成する。

        Dim oTerms(15) As TermMachine
        Dim termIndex As Integer = 0
        For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
            oTerms(termIndex) = oTerm
            termIndex += 1
        Next oTerm

        Dim oBytes(112) As Byte
        Dim pos As Integer = 0

        oBytes(pos) = &H55
        pos += 1
        For Each oTerm As TermMachine In oTerms
            If oTerm IsNot Nothing Then
                oBytes(pos) = CType(oTerm.Profile(Config.MachineProfileFieldNamesIndices("UNIT_NO")), Byte)
            End If
            pos += 1
        Next oTerm
        For Each oTerm As TermMachine In oTerms
            If oTerm IsNot Nothing Then
                oBytes(pos) = oTerm.PwrStatusFromKsb
            End If
            pos += 1
        Next oTerm
        For Each oTerm As TermMachine In oTerms
            If oTerm IsNot Nothing Then
                oBytes(pos) = oTerm.DlsStatusFromMcp
            End If
            pos += 1
        Next oTerm
        For Each oTerm As TermMachine In oTerms
            If oTerm IsNot Nothing Then
                oBytes(pos) = oTerm.DlsStatusFromIcm
            End If
            pos += 1
        Next oTerm
        For Each oTerm As TermMachine In oTerms
            If oTerm IsNot Nothing Then
                oBytes(pos) = oTerm.ExsStatusFromIcm
            End If
            pos += 1
        Next oTerm
        For Each oTerm As TermMachine In oTerms
            If oTerm IsNot Nothing Then
                oBytes(pos) = oTerm.IcmStatusFromMcp
            End If
            pos += 1
        Next oTerm
        For Each oTerm As TermMachine In oTerms
            If oTerm IsNot Nothing Then
                oBytes(pos) = oTerm.McpStatusFromKsb
            End If
            pos += 1
        Next oTerm

        Dim sFileName As String = "ConStatusPostReq.dat"
        Dim sFilePath As String = Path.Combine(sContextDir, sFileName)
        Dim oTeleg As New EkByteArrayPostReqTelegram(TelegGene, &H55, oBytes, 0)
        Using oOutputStream As New FileStream(sFilePath, FileMode.Create, FileAccess.Write)
            oTeleg.WriteToStream(oOutputStream)
        End Using
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を作成しました。")

        Return True
    End Function

    Protected Function CreateGateMasVerInfo(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachinesに設定されているマスタ保持状態をもとに、
        'sContextDirに号機別のGateMasVerInfo_RRRSSSCCCCUU.datを作成する。
        'また、過去のものがあれば消す。

        DeleteFiles(sMonitorMachineId, sContextDir, "GateMasVerInfo_*.dat")

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            CreateFileOfGateMasVerInfo(sMonitorMachineId, oTermEntry.Key, oTermEntry.Value, sContextDir)
        Next oTermEntry

        Return True
    End Function

    Protected Function CreateGateProVerInfo(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachinesに設定されている改札機向けプログラム保持状態をもとに、
        'sContextDirに号機別のGateProVerInfo_RRRSSSCCCCUU.datを作成する。
        'また、過去のものがあれば消す。

        DeleteFiles(sMonitorMachineId, sContextDir, "GateProVerInfo_*.dat")

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            CreateFileOfGateProVerInfo(sMonitorMachineId, oTermEntry.Key, oTermEntry.Value, sContextDir)
        Next oTermEntry

        Return True
    End Function

    Protected Function CreateKsbProVerInfo(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachineに設定されている監視盤向けプログラム保持状態をもとに、
        'sContextDirに号機別のKsbProVerInfo_RRRSSSCCCCUU.datを作成する。
        'また、過去のものがあれば消す。

        DeleteFiles(sMonitorMachineId, sContextDir, "KsbProVerInfo_*.dat")

        CreateFileOfKsbProVerInfo(sMonitorMachineId, oMonitorMachine, sContextDir)

        Return True
    End Function

    Protected Function ClearGateMas(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        DeleteFiles(sMonitorMachineId, sContextDir, "GateMasVerInfo_*.dat")

        oMonitorMachine.HoldingMasters.Clear()
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)

        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim oTerm As TermMachine = oTermEntry.Value
            oTerm.HoldingMasters.Clear()
            oTerm.PendingMasters.Clear()
            CreateFileOfGateMasVerInfo(sMonitorMachineId, oTermEntry.Key, oTerm, sContextDir)
            UpdateTable2OnTermStateChanged(sMachineDir, oTermEntry.Key, oTerm)
        Next oTermEntry

        Return True
    End Function

    'NOTE: 本物の監視盤は、改札機とオフラインであっても、改札機が保持しているマスタと
    '同じバージョンの適用リストを運管サーバから受信すれば、監視盤自身が「適用済み」の
    'DL完了通知を作成し、運管サーバに送信する。しかし、このアプリでは、DL完了通知を
    '送信するタイミングを任意にしたいので、そのようなことは行わない。
    'また、それゆえに、改札機の保持しているマスタのバージョンがv1、改札機とオフライン
    'になっている監視盤にキューイングされている（当該改札機に送信するべき）マスタの
    'バージョンがv2の状況で、運管サーバからv1の適用リストを受信した際、特殊な配慮を
    'せずとも、「適用済み」のDL完了通知を生成してしまうようなことはない。
    'なお、本物の監視盤は、監視盤内部に「オンラインになっている改札機が保持している
    'はずのマスタ」を仮想的に管理することで、その状況で「適用済み」は生成しないように
    'なっているかもしれないし、そうではなく「適用済み」を生成してしまうかもしれない。
    'どちらであるかは、接続試験を実施した限りでは不明であった。
    Protected Function AcceptGateMas(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDirの#PassiveDllReq.datが示すファイルをもとに
        '監視盤のマスタ保持状態（oMonitorMachine.HoldingMasters）を更新し、
        'sContextDirにExtOutput.datを作成する。
        'ただし、本物の監視盤と同じように、監視盤に格納場所がない
        '（既に格納しているものを捨てることもできない）場合は、
        '監視盤のマスタ保持状態を更新せずに、ContinueCodeが
        'FinishWithoutStoringのExtOutput.datを作成する。

        'NOTE: ContinueCodeがFinishのExtOutput.datを作成した場合は、
        'DL完了通知も作成しなければならない。これについては、
        'マスタ適用リストに記載された改札機(t)のマスタ保持状態
        '（oMonitorMachine.TermMachines(t).HoldingMasters）をこの場で
        '更新することにした上で、DL完了通知もこの場でsContextDirに
        '作成するのが簡単であるが、改札機まで届いていない期間も
        '再現したいので、このアプリにDeliverGateMas処理を用意し、
        'シミュレータ本体からその処理を要求された際に、改札機の
        'マスタ保持状態を更新する方式とする。
        'この方式の場合、１回のDeliverGateMasで、これまでに受信した
        '複数の適用リストの分のDL完了通知を作成する仕様にしても
        '違和感がないはず。そして、そのような仕様にすれば、
        'シナリオは、PassiveDllをハンドリングした文脈とは別の文脈で、
        'DeliverGateMasの要求およびDL完了通知のTryActiveOneを行う
        'こともできるようになる。また、このアプリも、作成する
        'DL完了通知を文脈別に管理する必要がなくなる。
        'ただし、マスタ適用リストをキューに保存しておく必要がある。
        '号機ごとにDL完了のタイミングを変えたいのであれば、
        'このキューは号機ごとに用意するべきである。
        'マスタ種別ごとにDL完了のタイミングを変えたいのであれば、
        'このキューはマスタ種別ごとに用意するべきである。
        'とりあえず正攻法で、DeliverGateMas処理を用意し、号機別
        '種別別のキューも用意する方式を実装しておく。

        'NOTE: 「最終受信物（データ本体）を配信完了していない場合に、マスタバージョン
        'やパターン番号がそれと異なる受信物（データ本体や適用リスト）を受け入れない」
        'ように監視盤部分を作ったとしても、キューには、マスタバージョンやパターン番号
        'が同一のものだけが入るとは限らない。具体的には、前半にマスタバージョンと
        'パターン番号が１世代前のものが（１つまたは複数）入り、後半にマスタバージョン
        'とパターン番号が最新のものが（１つまたは複数）入る場合があるはずである。
        '蛇足であるが、この場合、監視盤自身が後半のものを受け入れていることから、前半の
        'ものと同じマスタバージョンおよびパターン番号が付与されたデータ本体を全改札機に
        '配信済みであると言える。つまり、各キューにおける前半の情報は、当該改札機に
        '対してデータ本体を配信完了させた後に、同じマスタバージョン・同じパターン番号の
        'データ本体または適用リストを受信した際に、できたものであると言い切れる。
        'いずれにせよ、このような状態が生じ得るのなら、「最終受信物（データ本体）を配信
        '完了していない場合に、マスタバージョンやパターン番号がそれと異なる受信物
        '（データ本体や適用リスト）を受け入れない」ように監視盤部分を作り込む意味がない
        'ように思えるかもしれないが、（初期の）監視盤はこのような感じであったかもしれず、
        'それは単純に監視盤の仕様である（仕様であった）と思われる。
        'また、少なくともマスタについては、同一改札機を指定した同一パターン番号かつ
        '同一マスタバージョンのDLLが複数回ある場合に、１回目にDL完了通知を作ったら、
        'それ以上のDL完了通知を作っても運管の表示が変わらないゆえ、
        '２つ目以降の情報を保持する必要がないように思えるかもしれない。
        'そして、そうするならば、先述した監視盤の仕様を導入することで、１つの改札機に
        'ついてキューイングすべき情報は最大で１つだけになる（しかも、そのマスタバージョン
        'やパターン番号は、監視盤が最後に受信したものと同じになるはずである）から、
        'キューなど必要なくなるように思えるかもしれない。
        'しかし、運管の表示が変わらないといっても、全てのDLLに対応するDL完了通知を
        '作る（２回目からは「適用済み」のDL完了通知を作る）ことは決まりごとであるため、
        '本当にそれを守ろうとしたら、キューイングは必須である。
        'なお、号機ごとマスタ種別ごとに、配信を保留しているもののマスタバージョン
        'およびパターン番号を２世代分記憶し、さらにそれぞれの回数も記憶しておけば、
        '事足りるはずであるが、柔軟性と単純さを優先し、可変容量のキューで管理する
        'ことにする。

        Dim d As DateTime = DateTime.Now

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "電文書式に不正があります。")
            Return False
        End If

        Log.Info(sMonitorMachineId, "適用リストのファイル名は [" & oReqTeleg.ListFileName & "] です。")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "マスタ本体のファイル名は [" & oReqTeleg.DataFileName & "] です。")
        Else
            Log.Info(sMonitorMachineId, "マスタ本体のファイル名はありません。")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "適用リストのファイル名が不正です。")
            Return False
        End If

        Dim sApplicableModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim dataSubKind As Integer = EkMasProListFileName.GetDataSubKindAsInt(sListFileName)
        Dim dataVersion As Integer = EkMasProListFileName.GetDataVersionAsInt(sListFileName)
        Dim listVersion As Integer = EkMasProListFileName.GetListVersionAsInt(sListFileName)

        If oReqTeleg.DataFileName.Length <> 0 Then
            Dim sDataFileName As String = Path.GetFileName(oReqTeleg.DataFileName)
            If Not EkMasterDataFileName.IsValid(sDataFileName) Then
                Log.Error(sMonitorMachineId, "マスタ本体のファイル名が不正です。")
                Return False
            End If
            If EkMasterDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "ファイル名（適用先機種）に不整合があります。")
                Return False
            End If
            If EkMasterDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "ファイル名（マスタ種別）に不整合があります。")
                Return False
            End If
            If EkMasterDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "ファイル名（パターンNo）に不整合があります。")
                Return False
            End If
            If EkMasterDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "ファイル名（マスタVer）に不整合があります。")
                Return False
            End If
        End If

        'NOTE: 基本的に、oReqTelegのSubObjCodeとListFileNameの整合性は、シナリオ側
        'で保証する想定である。また、DataFileNameが空でない場合のListFileNameとの
        '整合性についても、シナリオ側で保証する想定である。シナリオ側でなら、
        '整合性のないREQに対し、NAKを返信するように設定できるためである。
        'ただし、たとえ、シナリオ側でチェックしないとしても、そもそも、整合性のない
        '配信を行えてしまえたなら、それは運管システムの不具合のはずであるから、
        'シナリオが実施結果NGで終了する方がよい。以上のことから、ここでも
        '整合性のチェックを行い、整合性がない場合は、否定応答を返却する。
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "ファイル名（適用先機種）が不正です。")
            Return False
        End If
        If Not ExConstants.GateMastersSubObjCodes.ContainsKey(sDataKind) Then
            Log.Error(sMonitorMachineId, "ファイル名（マスタ種別）が不正です。")
            Return False
        End If
        If oReqTeleg.SubObjCode <> ExConstants.GateMastersSubObjCodes(sDataKind) Then
            Log.Error(sMonitorMachineId, "電文のサブ種別がファイル名（マスタ種別）と整合していません。")
            Return False
        End If

        '監視盤が保持している当該種別マスタのパターン番号とバージョンを取得する。
        'NOTE: 監視盤が種別sDataKindのマスタを保持していないときは、
        'oHoldingMastersはNothingになり、holding0Versionやholding1Versionは0になる。
        'NOTE: EkMasProListFileName.IsValid()でのチェックによって、
        'dataVersionが0になることはあり得ないため、上記ケースでは、
        'holding0Versionおよびholding1Versionが、dataVersionと一致
        'することはあり得ない。
        Dim oHoldingMasters As HoldingMaster() = Nothing
        Dim holding0SubKind As Integer = 0
        Dim holding0Version As Integer = 0
        Dim holding1SubKind As Integer = 0
        Dim holding1Version As Integer = 0
        If oMonitorMachine.HoldingMasters.TryGetValue(sDataKind, oHoldingMasters) = True Then
            If oHoldingMasters(0) IsNot Nothing Then
                holding0SubKind = oHoldingMasters(0).DataSubKind
                holding0Version = oHoldingMasters(0).DataVersion
            End If
            If oHoldingMasters(1) IsNot Nothing Then
                holding1SubKind = oHoldingMasters(1).DataSubKind
                holding1Version = oHoldingMasters(1).DataVersion
            End If
        End If

        Dim oAreas As New HashSet(Of Integer)
        For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
            Dim ar As Integer = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
            oAreas.Add(ar)
        Next oTerm
        If oAreas.Count > 1 Then
            Log.Warn(sMonitorMachineId, "当該機器には異なるエリアNoの端末が混在しているため、正常な判断を行うことができません。ご注意ください。")
        End If

        Dim isDataKindAcceptable As Boolean = False
        For Each ar As Integer In oAreas
            Dim oAreaSpec As ExAreaSpec = Nothing
            If ExConstants.GateAreasSpecs.TryGetValue(ar, oAreaSpec) = True AndAlso _
               oAreaSpec.KsbReadyGateMasters.Contains(sDataKind) Then
                isDataKindAcceptable = True
            End If
        Next ar
        If Not isDataKindAcceptable Then
            Log.Error(sMonitorMachineId, "この種別のマスタは、当該機器の端末エリアNoでは受け付けることができません。")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End If

        'OPT: 現状の配信方法だと、マスタDLLシーケンスで適用リストは必ず受信する上、
        'それを契機に改札機に配信を行う場合は、過去に受信した同一名の適用リストの
        '有無によらず、受信した適用リストを使うことになるため、
        'sListHashValueやlistAcceptDateは必要ない。
        'oReqTeleg.ListFileHashValueやdを直接参照すればよい。

        Dim sDataHashValue As String = Nothing
        Dim sListHashValue As String = Nothing
        Dim holdingListVersion As Integer
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            Debug.Assert(oHoldingMasters IsNot Nothing)
            sDataHashValue = oHoldingMasters(0).DataHashValue
            sListHashValue = oHoldingMasters(0).ListHashValue
            holdingListVersion = oHoldingMasters(0).ListVersion
        ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
            Debug.Assert(oHoldingMasters IsNot Nothing)
            sDataHashValue = oHoldingMasters(1).DataHashValue
            sListHashValue = oHoldingMasters(1).ListHashValue
            holdingListVersion = oHoldingMasters(1).ListVersion
        End If

        Dim isDataUpdated As Boolean = False
        If oReqTeleg.DataFileName.Length = 0 Then
            '監視盤が保持していないマスタに関して、適用リストのみを送り付けられた場合は、
            'ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            'NOTE: そのケースで本物の監視盤がどのような反応を示すかは、分かっていない。
            'NOTE: この条件は、下記と同等である。
            ' (holding0SubKind <> dataSubKind OrElse holding0Version <> dataVersion) AndAlso _
            ' (holding1SubKind <> dataSubKind OrElse holding1Version <> dataVersion)
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "適用リストに紐づくマスタ本体がありません。")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            If sDataHashValue IsNot Nothing Then
                If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.DataFileHashValue, sDataHashValue) <> 0 Then
                    Log.Warn(sMonitorMachineId, "現在保持しているマスタと同一名ですが、内容（※ハッシュ値）が違うため、新しいマスタとして処理します。")
                    isDataUpdated = True
                End If
            End If
            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
        End If

        '受信した適用リストの内容を監視盤が保持しているものと比較する。
        '適用リストは個々に意味があり、バージョンも簡単に一巡するため、
        '内容が一致していないなら、新しい適用リストとして扱う。
        '同様の理由から、内容が一致していたとしても、何らかのDL完了通知を
        '発生させるのが然るべき姿であると思われるが、本物の監視盤は捨てる
        'ように見受けられるため、捨てる（isNewListをFalse にする）ことにする。
        'なお、受け付けた配信を必ず完了させる保証がなければ、このように捨てる
        'というのは有害である（前回受け付けた配信に対して、実際の配信を諦めた場合、
        'たとえ「配信異常」のDL完了通知を運管に送信していたとしても、
        '同じ適用リストによる今回の要求が捨てられれば、ユーザは
        '意味不明であると感じるかもしれないし、そもそも、捨てられた
        'のだということに気付かずに待ち続けるかもしれない）。
        'NOTE: 本物の監視盤は、ハッシュ値ではなく、適用リストの内容そのものを
        '比較すると思われる。しかし、万が一そうでなく、かつ、ハッシュ値の
        '偶然の一致により不当に捨てることになる場合に、そのことがリハーサル等で
        '判明しないのは危険であるため、シミュレータではハッシュ値を比較することにする。
        Dim isNewList As Boolean = True
        If sListHashValue IsNot Nothing AndAlso holdingListVersion = listVersion Then
            If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.ListFileHashValue, sListHashValue) <> 0 Then
                Log.Warn(sMonitorMachineId, "現在保持している適用リストと同一名ですが、内容（※ハッシュ値）が違うため、新しい適用リストとして処理します。")
            Else
                Log.Warn(sMonitorMachineId, "現在保持している適用リストと同一名で内容（※ハッシュ値）も同一であるため、これに基づく端末への配信は発生しません。")
                isNewList = False
            End If
        End If

        sListHashValue = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        '配下のいずれかの改札機に対して配信待ちのもの（DL完了通知未作成のもの）がある場合、
        'または、配下の全改札機の適用中バージョンが監視盤の保持(1)または保持(2)のいずれか
        '一方に揃っていない状況で、運管から新しいバージョンのDLLを要求された場合は、
        'ContinueCode.FinishWithoutStoringのREQ電文を作成する。
        'NOTE: 配下の改札機が「適用しているもの」または「適用することになるもの」
        '（=「適用済み」などのDL完了通知の作成が必要になるもの）が、監視盤の保持して
        'いるもの（２つ）に限定される状況を維持することが目標である。
        '監視盤は運管サーバにDL完了通知を送信できない状況において、何らかの方法で
        'DL完了通知の送信を保留する必要があるが、このようなルールにすることで、
        '保留するDL完了通知を改札機単位で最大２つに限定する（パターン番号とバージョン
        'が同じものは、配信結果値に優先順位をつけるなどして１つに畳み込む）ことも
        '可能になるのかもしれない。また、改札機にあるものを監視盤が必ず保持するなら、
        '配下の（一部の）改札機が保持しているものとパターン番号およびバージョンが
        '同一（= 同一名）でありながら内容が異なるものを受信した場合に、そのことを
        '検出し、受け入れを拒否することができる（結果として、同一名でありながら
        '内容の異なるマスタが、配下の改札機に混在する状況を防ぐことができる）。
        'TODO: 本物の監視盤に合わせたい。
        'NOTE: 本物の監視盤は、たとえDL完了通知を運管に送信できていない状況であったと
        'しても、その内容が全て「適用済み」で済む（もはやマスタそのものを保持しておく
        '必要がない）なら、投機的に受け入れを行うなど、複雑な動作をする可能性もある。
        '逆に、ここでは、保持(1)をみない（保持(2)を適用していない改札機がある状況で
        '保持(2)と違うものを受信した場合に、必ず受け入れを拒否する）ようにすることで、
        '保留しているDL完了通知が「保持(2)の受け入れ前に再受け入れしていた保持(1)に
        '対する適用済み」か「保持(2)に対する正常や適用済み」になる（いつでも保持(1)を
        '捨てることができる）ように制限し、保留しているDL完了通知の有無に関係なく、
        '（保持(2)のものが全改札機に適用されてさえいれば、それを保持(1)に移して）
        '新たなものを受け入れることができるよう、単純化している可能性もある。
        'その場合、たとえ適用リストのみの受信であっても、それが保持(2)のバージョンと
        '一致しない場合は、このメソッドの終わりに（保持(2)のものを保持(1)に移動して
        'から）受信したものを保持(2)に入れる（マスタ本体について、もともと保持(1)に
        'あったものを保持(2)に移した体とする）ことになる。マスタ本体の受け入れ日時が
        '後のものではなく、後から配信を要求されたものを保持(2)に格納するわけである。
        'これならば、新たなバージョンのマスタを受信した際も、保持(1)に入れるか
        '保持(2)に入れるかで面倒な判定（改札機に適用されていないのはどちらであるかの
        '判定）をせずに済む。しかし、これだと、監視盤設計者から聞いた「監視盤が保持
        'するマスタの世代１と世代２には、どちらが新しいといった決まりはない」という
        '思想と違う気がする。もしかすると、接続試験初期の頃の監視盤は、そのような感じ
        'だった気もするが、新しいマスタを一部の改札機に配信した状態において、マスタ
        'バージョンが世代１な適用リストを受け入れなくなるため、そのマスタに誤りが
        'あっても、全ての改札機に配信してからでないと、マスタのバージョン戻しが
        'できない...ということになり、あり得ない仕様という感じもする。
        'NOTE: そもそも、本物の監視盤は、現在保持しているものと同じマスタバージョンの
        '適用リストを受信した場合であっても、過去に受信した同一種別適用リスト（マスタ
        'バージョンは同一とは限らない）に対応するDL完了通知を送信していない場合は、
        'ContinueCode.FinishWithoutStoringのREQ電文を作成するかもしれない（複数の
        'DL完了通知をキューイングしたくない等の理由で）。
        'NOTE: このアプリでは、監視盤の保持(1)か保持(2)が空いている場合は、たとえ配下の
        '改札機に未配信のものがあろうが、配下の改札機の適用バージョンが揃ってなかろうが、
        'そこに受け入れを行う。たとえば、保持(1)に何か入っていて、保持(2)が空いている
        '場合に、「保持(1)と同じものを適用している改札機」と「何も適用していない改札機」
        'が混在している状況であっても、新たなものを１つだけ受け入れるようになっている。
        'しかし、本物の監視盤はそうではないかもしれない。本物の監視盤は、改札機がマスタ
        'を保持していない状態を、バージョン0を保持している状態とみなし、それ以上の混在
        'を許さない可能性がある。ただ、合理的な理由でそうなっているとは考えにくいし、
        '運用の妨げにもなるので、とりあえず、このアプリでは、そのような制限はかけず、
        '然るべき動作をさせることにする。
        'NOTE: 本物の監視盤は、改札機に適用されているもの（適用されるはずのもの）が、
        '監視盤の保持している世代１とも世代２とも異なる場合は、新たなマスタを受信した
        '場合であっても、その世代に受け入れを可能にする（もしくは、適用リストなしでも
        '自らの判断で改札機に配信を行い、それが完了するまでの期間だけ、受け入れを拒否
        'する）など、特別な配慮をしている可能性もある。
        'NOTE: 本物の監視盤は、パターン番号を比較しないかもしれない。

        Dim acceptableSlot As Integer = -1
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            acceptableSlot = 0
        ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
            acceptableSlot = 1
        ElseIf oHoldingMasters Is Nothing OrElse oHoldingMasters(0) Is Nothing Then
            acceptableSlot = 0
        ElseIf oHoldingMasters(1) Is Nothing Then
            acceptableSlot = 1
        End If

        If acceptableSlot = -1 OrElse isDataUpdated Then
            'OPT: isDataUpdatedがTrueの状況では、運管サーバが適用リストに記載された端末と関係のない
            '監視盤に配信を行ったということがない限り、Update前のDataが必ずいずれかのTermMachineの
            'PendingMastersかHoldingMastersに収納されているはずであり、検索を行うまでもなく
            'ここでFinishWithoutStoringのDL完了通知を発生させることができるはずである。

            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                Dim oQueue As LinkedList(Of PendingMaster) = Nothing
                If oTerm.PendingMasters.TryGetValue(sDataKind, oQueue) = True AndAlso _
                   oQueue.Count <> 0 Then
                    'TODO: この電文にパターン番号を格納する場所がないということは、
                    '本物の監視盤や改札機は、パターン番号が異なるものであれば、
                    '種別やバージョンが同一であっても、同時に保持できるようになって
                    'いるのかもしれない。ただし、プログラム配信の事情（監視盤と
                    'その配下の改札機のエリア番号が、配信ごとに変化しないこと）を
                    '優先して、仕様が決められているだけもしれない。
                    Log.Error(sMonitorMachineId, "当該種別のマスタについて、配信待ち（DL完了通知未作成）の端末がある状況で、新たなものを受信しました。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                    Return True
                End If
            Next oTerm

            If isDataUpdated Then
                Debug.Assert(acceptableSlot <> -1)
                'TODO: 本物の監視盤は、既に「全部の」端末に配信した状況であれば、
                'それと同一名・別内容のマスタであっても、受け入れそうな気もする。
                For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                    Dim oMas As HoldingMaster = Nothing
                    If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = True AndAlso _
                       oMas.DataSubKind = dataSubKind AndAlso oMas.DataVersion <> DataVersion Then
                        Log.Error(sMonitorMachineId, "当該種別・当該バージョンのマスタについて、既に一部の端末に配信した状況で、内容（※ハッシュ値）の異なるものを受信しました。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                        Return True
                    End If
                Next oTerm
            End If
        End If

        If acceptableSlot = -1 Then
            '監視盤の保持(2)と同一のものが全ての改札機に適用されている場合は、監視盤の保持(1)に受け入れ可とする。
            acceptableSlot = 0
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                'NOTE: ここが実行されるケースでは、「oHoldingMasters(1) Is Nothing」はあり得ないため、
                '「holding1Version = 0」であるとしても、それは、監視盤がバージョン0のマスタを保持
                'しているということである。そして、万が一「oMas.DataVersion = 0」であるとすれば、
                'それは、改札機がバージョン0のマスタを保持しているということであり、監視盤と改札機の
                'バージョンが一致しているとみなしてよい。
                Dim oMas As HoldingMaster = Nothing
                If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = False OrElse _
                   holding1SubKind <> oMas.DataSubKind OrElse holding1Version <> oMas.DataVersion Then
                    acceptableSlot = -1
                    Exit For
                End If
            Next oTerm
        End If

        If acceptableSlot = -1 Then
            '監視盤の保持(1)と同一のものが全ての改札機に適用されている場合は、監視盤の保持(2)に受け入れ可とする。
            acceptableSlot = 1
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                'NOTE: ここが実行されるケースでは、「oHoldingMasters(0) Is Nothing」はあり得ないため、
                '「holding0Version = 0」であるとしても、それは、監視盤がバージョン0のマスタを保持
                'しているということである。そして、万が一「oMas.DataVersion = 0」であるとすれば、
                'それは、改札機がバージョン0のマスタを保持しているということであり、監視盤と改札機の
                'バージョンが一致しているとみなしてよい。
                Dim oMas As HoldingMaster = Nothing
                If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = False OrElse _
                   holding0SubKind <> oMas.DataSubKind OrElse holding0Version <> oMas.DataVersion Then
                    acceptableSlot = -1
                    Exit For
                End If
            Next oTerm
        End If

        If acceptableSlot = -1 Then
            Log.Error(sMonitorMachineId, "当該種別のマスタについて、新たな格納場所が無い（保持している一方のマスタを全端末に適用していない）状況で、新たなものを受信しました。")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
            Return True
        End If

        '適用リストの内容を解析する。
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, Integer)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            'とりあえず、全て読み取る。
            'OPT: 「適用リストの内容を表示する機能」を追加した際に、手っ取り早く
            '実装するために、このように二度読みすることになっている。
            '非効率であり、改善の余地がある。
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '１行ずつ読み取る。
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '以下、適用リストの内容をチェックし、問題がある場合は、
                'ContinueCode.FinishWithoutStoringのREQ電文を作成する。

                'ヘッダ部の１行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストが空です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の１行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "適用リスト1行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '作成年月日をチェックする。
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された作成年月日が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'リストVerをチェックする。
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたリストVerがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストに2行目がありません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "適用リスト2行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'パターンNoをチェックする。
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたパターンNoがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'マスタVerをチェックする。
                If Not dataVersion.ToString("D3").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたマスタVerがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '機種コードをチェックする。
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された機種がファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '適用リストの３行目以降から、oMonitorMachineに関係する号機を抽出する。
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '読み込んだ行を列に分割する。
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 3 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の項目数が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'サイバネ線区駅順コードの書式をチェックする。
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の駅コードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'コーナーコードの書式をチェックする。
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目のコーナーコードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '号機番号の書式をチェックする。
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の号機番号が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '行の重複をチェックする。
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目が既出の行と重複しています。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '行の内容を一時保存する。
                    oListedMachines.Add(sLineKey, 0)

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "適用リストの読取りで例外が発生しました。", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End Try

        Dim dataAcceptDate As DateTime
        Dim oDataFooter As Byte()
      #If AcceptsSameNameMasOfSameHashValue Then
        If oReqTeleg.DataFileName.Length <> 0 Then
            dataAcceptDate = d

            'マスタ本体のフッタ情報を読取る。
            oDataFooter = New Byte(ExMasterDataFooter.Length - 1) {}
            Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
            Try
                Using oInputStream As New FileStream(sDataFilePath, FileMode.Open, FileAccess.Read)
                    oInputStream.Seek(-ExMasterDataFooter.Length, SeekOrigin.End)
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(oDataFooter, pos, ExMasterDataFooter.Length - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "マスタ本体のフッタ情報の読取りで例外が発生しました。", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            '読取ったフッタ情報に問題がある場合は、
            'ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            Dim footerView As New ExMasterDataFooter(oDataFooter)
            Dim sViolation As String = footerView.GetFormatViolation()
            If sViolation IsNot nothing Then
                Log.Error(sMonitorMachineId, "マスタ本体のフッタ情報が異常です。" & vbCrLf & sViolation)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
                Debug.Assert(oHoldingMasters IsNot Nothing)
                dataAcceptDate = oHoldingMasters(0).DataAcceptDate
                oDataFooter = oHoldingMasters(0).DataFooter
            Else
                Debug.Assert(holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion)
                Debug.Assert(oHoldingMasters IsNot Nothing)
                dataAcceptDate = oHoldingMasters(1).DataAcceptDate
                oDataFooter = oHoldingMasters(1).DataFooter
            End If
        End If
      #Else
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            Debug.Assert(oHoldingMasters IsNot Nothing)
            dataAcceptDate = oHoldingMasters(0).DataAcceptDate
        ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
            Debug.Assert(oHoldingMasters IsNot Nothing)
            dataAcceptDate = oHoldingMasters(1).DataAcceptDate
        Else
            dataAcceptDate = d

            'マスタ本体のフッタ情報を読取る。
            oDataFooter = New Byte(ExMasterDataFooter.Length - 1) {}
            Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
            Try
                Using oInputStream As New FileStream(sDataFilePath, FileMode.Open, FileAccess.Read)
                    oInputStream.Seek(-ExMasterDataFooter.Length, SeekOrigin.End)
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(oDataFooter, pos, ExMasterDataFooter.Length - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "マスタ本体のフッタ情報の読取りで例外が発生しました。", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            '読取ったフッタ情報に問題がある場合は、
            'ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            Dim footerView As New ExMasterDataFooter(oDataFooter)
            Dim sViolation As String = footerView.GetFormatViolation()
            If sViolation IsNot nothing Then
                Log.Error(sMonitorMachineId, "マスタ本体のフッタ情報が異常です。" & vbCrLf & sViolation)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        End If
      #End If

        If isNewList Then
            '一時保存していた行が示す各機器に、配信のための情報をキューイングする。
            Dim targetTermCount As Integer = 0
            For Each sName As String In oListedMachines.Keys
                '行がoMonitorMachineに関係する場合
                Dim oTerm As TermMachine = Nothing
                If oMonitorMachine.TermMachines.TryGetValue(sName, oTerm) = True Then
                    Log.Debug(sMonitorMachineId, "適用リストに記載された端末 [" & sName & "] の行をキューイングします。")
                    Dim oQueue As LinkedList(Of PendingMaster) = Nothing
                    If oTerm.PendingMasters.TryGetValue(sDataKind, oQueue) = False Then
                        oQueue = New LinkedList(Of PendingMaster)()
                        oTerm.PendingMasters.Add(sDataKind, oQueue)
                    End If
                    Dim oPenMas As New PendingMaster()
                    oPenMas.DataSubKind = dataSubKind
                    oPenMas.DataVersion = dataVersion
                    oPenMas.ListVersion = listVersion
                    oPenMas.DataAcceptDate = dataAcceptDate
                    oPenMas.DataFooter = oDataFooter
                    oPenMas.DataHashValue = sDataHashValue
                    oPenMas.ListAcceptDate = listAcceptDate
                    oPenMas.ListContent = sListContent
                    oPenMas.ListHashValue = sListHashValue
                    oQueue.AddLast(oPenMas)
                    UpdateTable2OnTermStateChanged(sMonitorMachineId, sName, oTerm)
                    targetTermCount += 1
                End If
            Next sName
            Log.Debug(sMonitorMachineId, "適用リストに記載された" & oListedMachines.Count.ToString() & "台のうち、" & targetTermCount.ToString() & "台が当該機器の端末でした。")

            'NOTE: 下記のケースで、本物の監視盤がどのような反応を示すかは、よくわからない。
            'しかし、このような部材の情報をoMonitorMachine.HoldingMasters()に格納するわけにはいかない。
            '配下のTermMachineのPendingMastersに登録していないということは、次に運管から受け入れ可能と
            'するバージョンが、それに制限される保証がないということである。
            '実害の有無は微妙であるが、受け入れ許可するマスタをQに制限しないにもかかわらず、
            'QがoMonitorMachine.HoldingMasters()に登録されている状態というのは、紛らわしすぎる。
            If targetTermCount = 0 Then
                Log.Error(sMonitorMachineId, "配信を生み出さない適用リストを受信しました。")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        End If

        If oReqTeleg.DataFileName.Length <> 0 Then
            '監視盤のマスタ保持状態を更新する。

            If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
                Log.Info(sMonitorMachineId, "保持(1)にある同一パターン・同一バージョンのマスタを上書きします。")
            ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
                Log.Info(sMonitorMachineId, "保持(2)にある同一パターン・同一バージョンのマスタを上書きします。")
            ElseIf oHoldingMasters Is Nothing OrElse oHoldingMasters(0) Is Nothing Then
                Log.Info(sMonitorMachineId, "保持(1)が空いているので、この空きを使って新たなものを受け入れます。本物の監視盤がこのように動作するとは限りません。")
            ElseIf oHoldingMasters(1) Is Nothing Then
                Log.Info(sMonitorMachineId, "保持(2)が空いているので、この空きを使って新たなものを受け入れます。本物の監視盤がこのように動作するとは限りません。")
            ElseIf acceptableSlot = 0 Then
                Log.Info(sMonitorMachineId, "新たなものを受け入れるために、配信待ちでなく端末にも適用していない保持(1)のマスタを削除します。")
            ElseIf acceptableSlot = 1 Then
                Log.Info(sMonitorMachineId, "新たなものを受け入れるために、配信待ちでなく端末にも適用していない保持(2)のマスタを削除します。")
            End If

            If oHoldingMasters IsNot Nothing Then
                '世代２に何かを保持している状況であるにもかかわらず、
                'これまでと違うものをいきなり世代１に受け入れる場合は、
                '世代２にあるものを世代１に移して、世代２に受け入れる。
                'TODO: 本物の監視盤に合わせたい。本物の監視盤は
                'このように凝ったことはしないかもしれない。
                If acceptableSlot = 0 AndAlso _
                   oHoldingMasters(1) IsNot Nothing AndAlso _
                  (holding0SubKind <> dataSubKind OrElse holding0Version <> dataVersion) Then
                    Log.Info(sMonitorMachineId, "保持(2)にあるマスタを保持(1)に移動し、保持(2)に受け入れます。本物の監視盤がこのように動作するとは限りません。")
                    oHoldingMasters(0) = oHoldingMasters(1)
                    acceptableSlot = 1
                End If
            Else
                oHoldingMasters = New HoldingMaster(1) {}
                oMonitorMachine.HoldingMasters.Add(sDataKind, oHoldingMasters)
            End If
            oHoldingMasters(acceptableSlot) = New HoldingMaster()
            Debug.Assert(dataAcceptDate = d)
            Debug.Assert(listAcceptDate = d)
            oHoldingMasters(acceptableSlot).DataSubKind = dataSubKind
            oHoldingMasters(acceptableSlot).DataVersion = dataVersion
            oHoldingMasters(acceptableSlot).ListVersion = listVersion
            oHoldingMasters(acceptableSlot).DataAcceptDate = dataAcceptDate
            oHoldingMasters(acceptableSlot).DataFooter = oDataFooter
            oHoldingMasters(acceptableSlot).DataHashValue = sDataHashValue
            oHoldingMasters(acceptableSlot).ListAcceptDate = listAcceptDate
            oHoldingMasters(acceptableSlot).ListContent = sListContent
            oHoldingMasters(acceptableSlot).ListHashValue = sListHashValue
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        Else
            Debug.Assert(oHoldingMasters(acceptableSlot).DataSubKind = dataSubKind)
            Debug.Assert(oHoldingMasters(acceptableSlot).DataVersion = dataVersion)
            Debug.Assert(listAcceptDate = d)
            oHoldingMasters(acceptableSlot).ListVersion = listVersion
            oHoldingMasters(acceptableSlot).ListAcceptDate = listAcceptDate
            oHoldingMasters(acceptableSlot).ListContent = sListContent
            oHoldingMasters(acceptableSlot).ListHashValue = sListHashValue
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        End If
        Log.Info(sMonitorMachineId, "受け入れが完了しました。")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverGateMas(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '全改札機のキューから全てのマスタ適用リストを取り出し、
        'マスタ適用リストごとに、当該改札機(t)のマスタ保持状態
        '（oMonitorMachine.TermMachines(t).HoldingMasters）を
        '更新し、sMachineDirにマスタ適用リスト別・改札機別の
        '#GateMasDlReflectReq_RRRSSSCCCCUU_N.dat（Nは0～）を作成する。
        'また、マスタ保持状態を更新した改札機については、
        'sContextDirにGateMasVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、GateMasDlReflectReq_RRRSSSCCCCUU_N.datと
        'GateMasVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの、今回更新
        'していない改札機のもの）を削除する。

        'NOTE: これらのファイル名のRRRSSSCCCCUUは端末の機器コードで
        'あるが、これらのファイルが全端末分（シナリオ内で
        '「%T3R%T3S%T4C%T2U」と記述した場合に複製される全行分）
        '作成されるとは限らない。
        'よって、シナリオは、当該ファイルを送信する際、
        'ActiveOneではなく、TryActiveOneを使用する。

        DeleteFiles(sMonitorMachineId, sContextDir, "GateMasVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now

        '全端末について処理を行う。
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            Dim oTerm As TermMachine = oTermEntry.Value

            Dim existsAnyProc As Boolean = False
            For Each oPendingList As LinkedList(Of PendingMaster) In oTerm.PendingMasters.Values
                If oPendingList.Count <> 0 Then
                    existsAnyProc = True
                    Exit For
                End If
            Next oPendingList
            If Not existsAnyProc Then
                Log.Debug(sMonitorMachineId, "端末 [" & sTermMachineId & "] に対するマスタ配信はありません。")
                Continue For
            ElseIf oTerm.McpStatusFromKsb <> &H0 Then
                Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] については、主制状態が正常以外に設定されているため、配信処理を保留します。")
                Continue For
            End If

            Dim isHoldingMasUpdated As Boolean = False

            'マスタ種別ごとに処理を行う。
            For Each oKindEntry As KeyValuePair(Of String, LinkedList(Of PendingMaster)) In oTerm.PendingMasters

                '当該監視盤が保持している当該種別マスタのパターン番号とバージョンを取得する。
                'NOTE: 当該監視盤が種別oKindEntry.Keyのマスタを保持していないときは、
                'oHoldingMastersはNothingになり、holding0Versionやholding1Versionは0になる。
                Dim oHoldingMasters As HoldingMaster() = Nothing
                Dim holding0SubKind As Integer = 0
                Dim holding0Version As Integer = 0
                Dim holding1SubKind As Integer = 0
                Dim holding1Version As Integer = 0
                If oMonitorMachine.HoldingMasters.TryGetValue(oKindEntry.Key, oHoldingMasters) = True Then
                    If oHoldingMasters(0) IsNot Nothing Then
                        holding0SubKind = oHoldingMasters(0).DataSubKind
                        holding0Version = oHoldingMasters(0).DataVersion
                    End If
                    If oHoldingMasters(1) IsNot Nothing Then
                        holding1SubKind = oHoldingMasters(1).DataSubKind
                        holding1Version = oHoldingMasters(1).DataVersion
                    End If
                End If

                'TODO: 本物の監視盤は、最後にキューイングしたものしか改札機に送信しないかもしれない。
                'それは仕方ないとしても、最後にキューイングした要求に対応するDL完了通知しか発生させない可能性も
                'ある（要求されたことを行えないなら「異常」のDL完了通知を発生させれば済むにもかかわらず）。
                'もしそうだとしたら、運管に対する働きが実機とシミュレータで違う... ということになってしまうので、
                'シミュレータでも、最後にキューイングされているもの以外は読み捨てた方がよいかもしれない。
                'なお、たとえそうするにしても、TermMachineクラスのPendingMastersは必要である。
                'シミュレータの機能として、改札機に未配信のものをユーザに示す必要があるためである。

                '配信処理に使っていない全適用リストについて処理を行う。
                For Each oPenMas As PendingMaster In oKindEntry.Value
                    'NOTE: 実体のない（ListHashValue Is Nothing の）適用リストで配信が行われる可能性は想定しない。
                    Log.Info(sMonitorMachineId, "適用リスト [" & oPenMas.ListVersion.ToString() & "] に基づき、端末 [" & sTermMachineId & "] に対する種別 [" & oKindEntry.Key & "] パターンNo [" & oPenMas.DataSubKind.ToString() & "] マスタVer [" & oPenMas.DataVersion.ToString() & "] のマスタ配信処理を行います...")

                    '配信結果（「正常」または「適用済み」）を決める。
                    'TODO: 本物の監視盤に合わせたい。
                    'プロトコル仕様で決められているわけではないが、本物の監視盤は、
                    'たとえパターン番号やマスタバージョンが一致していたとしても、
                    '（改札機にあるものが、監視盤から受け取ったものではない等で）
                    'データの内容が不一致であれば、改札機に配信しなおして、
                    '配信結果を「正常」としそうな気がするので、そうしてあるが、
                    '実は違うかもしれない。
                    'TODO: 運管サーバの実装は、当初伝えらていた駅務機器の仕様に合わせてあり、
                    'この実装と整合していない。具体的には「たとえパターン番号の異なるマスタが
                    '監視機器までDLLされても、マスタバージョンが同一である限り、端末機器は
                    'それを監視機器から取得しない」という仕様に合わせ、マスタバージョンが
                    '同一である限りは、端末機器の受信状態を「配信中」に変更しない。
                    'へたに「配信中」のレコードを作成して、それが残るよりはよいが、
                    '窓処の場合はどうなのかも含め、実際の理想形を確認するべきである。
                    Dim deliveryResult As Byte = &H0
                    Dim isOutOfArea As Boolean = False

                    If deliveryResult = &H0 Then
                        Dim ar As Integer = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                        Dim oAreaSpec As ExAreaSpec = Nothing
                        If ExConstants.GateAreasSpecs.TryGetValue(ar, oAreaSpec) = False OrElse _
                           Not oAreaSpec.GateReadyGateMasters.Contains(oKindEntry.Key) Then
                            'NOTE: 配信しないのに「適用済み」は不適切に思えるが、実際にこのように動作する。
                            Log.Error(sMonitorMachineId, "この種別のマスタはエリア [" & ar.ToString() &"] の端末には配信できません。")
                            deliveryResult = &HF
                            isOutOfArea = True
                        End If
                    End If

                    If deliveryResult = &H0 Then
                        Dim oMas As HoldingMaster = Nothing
                        If oTerm.HoldingMasters.TryGetValue(oKindEntry.Key, oMas) = True AndAlso _
                           oMas.DataSubKind = oPenMas.DataSubKind AndAlso _
                           oMas.DataVersion = oPenMas.DataVersion AndAlso _
                           StringComparer.OrdinalIgnoreCase.Compare(oMas.DataHashValue, oPenMas.DataHashValue) = 0 Then
                            '改札機が保持しているものと同じものを配信することになる場合は、
                            '配信結果を「適用済み」とする。
                            Log.Warn(sMonitorMachineId, "当該端末に対しては当該マスタを適用済み（※配信済み）です。再配信は行いません。")
                            deliveryResult = &HF
                        End If
                    End If

                    If deliveryResult = &H0 Then
                        If (oPenMas.DataSubKind <> holding0SubKind OrElse oPenMas.DataVersion <> holding0Version) AndAlso _
                           (oPenMas.DataSubKind <> holding1SubKind OrElse oPenMas.DataVersion <> holding1Version) Then
                            Log.Error(sMonitorMachineId, "想定外の状況です。配信しなければならないマスタが既に監視盤にありません。")
                            deliveryResult = &H5 'NOTE: 適当なコードがないので、とりあえず正常以外にしておく。
                        End If
                    End If

                    '改札機のマスタ保持状態を更新する。
                    If deliveryResult = &H0 OrElse isOutOfArea Then
                        'NOTE: 改札機は適用リストを保持しないが、どの適用リストの指示によって
                        '当該改札機にマスタ本体の配信が行われたかが分かる方がよいので、
                        '適用リストバージョンもセットすることにする。
                        Dim oNewMas As New HoldingMaster()
                        oNewMas.DataSubKind = oPenMas.DataSubKind
                        oNewMas.DataVersion = oPenMas.DataVersion
                        oNewMas.ListVersion = oPenMas.ListVersion
                        oNewMas.DataAcceptDate = oPenMas.DataAcceptDate
                        oNewMas.DataDeliverDate = d
                        oNewMas.DataFooter = oPenMas.DataFooter
                        oNewMas.DataHashValue = oPenMas.DataHashValue
                        oNewMas.ListAcceptDate = oPenMas.ListAcceptDate
                        oNewMas.ListContent = oPenMas.ListContent
                        oNewMas.ListHashValue = oPenMas.ListHashValue
                        oTerm.HoldingMasters(oKindEntry.Key) = oNewMas
                        isHoldingMasUpdated = True
                        If deliveryResult = &H0 Then
                            Log.Info(sMonitorMachineId, "当該端末に対して当該マスタの配信を行いました。")
                        Else
                            Log.Warn(sMonitorMachineId, "当該端末に対して当該マスタの配信を行いました。これは自動改札機システムの生成するバージョン情報を再現させるための特別措置ですので、ご注意ください。")
                        End If
                    End If

                    '#GateMasDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
                    CreateFileOfGateMasDlReflectReq( _
                       &H47, _
                       ExConstants.GateMastersSubObjCodes(oKindEntry.Key), _
                       oPenMas.DataSubKind, _
                       oPenMas.DataVersion, _
                       deliveryResult, _
                       sMonitorMachineId, _
                       sTermMachineId, _
                       sMachineDir)
                Next oPenMas
            Next oKindEntry
            oTerm.PendingMasters.Clear()

            If isHoldingMasUpdated Then
                CreateFileOfGateMasVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
            End If

            UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
        Next oTermEntry

        Return True
    End Function

    Protected Function DirectInstallGatePro(ByVal sContextDir As String, ByVal sFilePath As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        'Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(sFilePath)
        Catch ex As Exception
            Log.Error("プログラム本体のハッシュ値算出で例外が発生しました。", ex)
            Return False
        End Try

        Dim content As GateProgramContent
        Try
            content = ExtractGateProgramCab(sFilePath, Path.Combine(sContextDir, "GatePro"))
        Catch ex As Exception
            Log.Error("プログラム本体の解析で例外が発生しました。", ex)
            Return False
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("バージョンリストからのエリアNoの抽出で例外が発生しました。", ex)
            Return False
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("バージョンリストからの代表Verの抽出で例外が発生しました。", ex)
            Return False
        End Try

        'TODO: 自動改札機システムでは、監視盤配下の改札機は、全て同一エリアに所属しているはずなので、
        'ここで、部材のエリアと監視盤が管理している改札機エリアの整合性チェックを行うことも可能と思われる。
        'もし実機がチェックを行うなら、それに合わせた方がよい。
        'おそらく、自動改札機のHW自体は、どのエリアの改札機プログラムもインストール可能であり、
        '直接投入においてまでそれを妨げることは無いと思われるが。

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("プログラムの動作許可日が未来日に設定されています。" & content.RunnableDate & "まで適用できませんのでご注意ください。")
        End If

        InstallGateProgramDirectly(sContextDir, subKind, version, content, sHashValue)

        Return True
    End Function

    Protected Function AcceptGatePro(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDirの#PassiveDllReq.datが示すファイルをもとに
        '監視盤の改札機向けプログラム保持状態（oMonitorMachine.HoldingPrograms）を更新し、
        'sContextDirにExtOutput.datを作成する。
        'ただし、本物の監視盤と同じように、何れかの改札機の保持バージョン
        'が、監視盤の保持バージョン（前回受信バージョン）と異なる
        '場合は、監視盤の改札機向けプログラム保持状態を更新せずに、
        'ContinueCodeがFinishWithoutStoringのExtOutput.datを作成する。

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "電文書式に不正があります。")
            Return False
        End If

        Log.Info(sMonitorMachineId, "適用リストのファイル名は [" & oReqTeleg.ListFileName & "] です。")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "プログラム本体のファイル名は [" & oReqTeleg.DataFileName & "] です。")
        Else
            Log.Info(sMonitorMachineId, "プログラム本体のファイル名はありません。")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "適用リストのファイル名が不正です。")
            Return False
        End If

        Dim sApplicableModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim dataSubKind As Integer = EkMasProListFileName.GetDataSubKindAsInt(sListFileName)
        Dim dataVersion As Integer = EkMasProListFileName.GetDataVersionAsInt(sListFileName)
        Dim listVersion As Integer = EkMasProListFileName.GetListVersionAsInt(sListFileName)

        If oReqTeleg.DataFileName.Length <> 0 Then
            Dim sDataFileName As String = Path.GetFileName(oReqTeleg.DataFileName)
            If Not EkProgramDataFileName.IsValid(sDataFileName) Then
                Log.Error(sMonitorMachineId, "プログラム本体のファイル名が不正です。")
                Return False
            End If
            If EkProgramDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "ファイル名（適用先機種）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "ファイル名（プログラム種別）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "ファイル名（エリアNo）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "ファイル名（代表Ver）に不整合があります。")
                Return False
            End If
        End If

        'NOTE: 基本的に、oReqTelegのSubObjCodeとListFileNameの整合性は、シナリオ側
        'で保証する想定である。また、DataFileNameが空でない場合のListFileNameとの
        '整合性についても、シナリオ側で保証する想定である。シナリオ側でなら、
        '整合性のないREQに対し、NAKを返信するように設定できるためである。
        'ただし、たとえ、シナリオ側でチェックしないとしても、そもそも、整合性のない
        '配信を行えてしまえたなら、それは運管システムの不具合のはずであるから、
        'シナリオが実施結果NGで終了する方がよい。以上のことから、ここでも
        '整合性のチェックを行い、整合性がない場合は、否定応答を返却する。
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "ファイル名（適用先機種）が不正です。")
            Return False
        End If
        If sDataKind <> "GPG" Then
            Log.Error(sMonitorMachineId, "ファイル名（プログラム種別）が不正です。")
            Return False
        End If
        If oReqTeleg.SubObjCode <> &H0 Then
            Log.Error(sMonitorMachineId, "電文のサブ種別が不正です。")
            Return False
        End If

        'NOTE: 監視盤が改札機向けプログラムを保持していないときは、
        'holding0Versionやholding1Versionは0になる。
        'NOTE: EkMasProListFileName.IsValid()でのチェックによって、
        'dataVersionが0になることはあり得ないため、上記ケースでは、
        'holding0Versionおよびholding1Versionが、dataVersionと一致
        'することはあり得ない。
        Dim holding0SubKind As Integer = 0
        Dim holding0Version As Integer = 0
        Dim holding1SubKind As Integer = 0
        Dim holding1Version As Integer = 0
        If oMonitorMachine.HoldingPrograms(0) IsNot Nothing Then
            holding0SubKind = oMonitorMachine.HoldingPrograms(0).DataSubKind
            holding0Version = oMonitorMachine.HoldingPrograms(0).DataVersion
        End If
        If oMonitorMachine.HoldingPrograms(1) IsNot Nothing Then
            holding1SubKind = oMonitorMachine.HoldingPrograms(1).DataSubKind
            holding1Version = oMonitorMachine.HoldingPrograms(1).DataVersion
        End If

        Dim sDataHashValue As String = Nothing
        Dim sListHashValue As String = Nothing
        Dim holdingListVersion As Integer
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingPrograms(0) IsNot Nothing)
            sDataHashValue = oMonitorMachine.HoldingPrograms(0).DataHashValue
            sListHashValue = oMonitorMachine.HoldingPrograms(0).ListHashValue
            holdingListVersion = oMonitorMachine.HoldingPrograms(0).ListVersion
        ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingPrograms(1) IsNot Nothing)
            sDataHashValue = oMonitorMachine.HoldingPrograms(1).DataHashValue
            sListHashValue = oMonitorMachine.HoldingPrograms(1).ListHashValue
            holdingListVersion = oMonitorMachine.HoldingPrograms(1).ListVersion
        End If

        'Dim isDataUpdated As Boolean = False
        If oReqTeleg.DataFileName.Length = 0 Then
            '監視盤が保持していないバージョンの改札機向けプログラムに関して、適用リストのみを
            '送り付けられた場合は、ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            'NOTE: そのケースで本物の監視盤がどのような反応を示すかは、分かっていない。
            'NOTE: この条件は、下記と同等である。
            ' (holding0SubKind <> dataSubKind OrElse holding0Version <> dataVersion) AndAlso _
            ' (holding1SubKind <> dataSubKind OrElse holding1Version <> dataVersion)
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "適用リストに紐づくプログラム本体がありません。")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            If sDataHashValue IsNot Nothing Then
                'NOTE: 強制配信が不可能にならないよう、受信データが同一名であっても
                '同一内容であれば、この後で保持データを上書きする想定である。
                If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.DataFileHashValue, sDataHashValue) <> 0 Then
                    Log.Warn(sMonitorMachineId, "現在保持しているプログラムと同一名ですが、内容（※ハッシュ値）が違うため、新しいプログラムとして処理します。")
                    'isDataUpdated = True
                End If
            End If
            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
        End If


        '受信した適用リストの内容を監視盤が保持しているものと比較する。
        '適用リストは個々に意味があり、バージョンも簡単に一巡するため、
        '内容が一致していないなら、新しい適用リストとして扱う。
        '同様の理由から、内容が一致していたとしても、何らかのDL完了通知を
        '発生させるのが然るべき姿であると思われるが、本物の監視盤は捨てる
        'ように見受けられるため、捨てる（isNewListをFalse にする）ことにする。
        'なお、受け付けた配信を必ず完了させる保証がなければ、このように捨てる
        'というのは有害である（前回受け付けた配信に対して、実際の配信を諦めた場合、
        'たとえ「配信異常」のDL完了通知を運管に送信していたとしても、
        '同じ適用リストによる今回の要求が捨てられれば、ユーザは
        '意味不明であると感じるかもしれないし、そもそも、捨てられた
        'のだということに気付かずに待ち続けるかもしれない）。
        '→ このような適用リストを捨ててしまうと、適用リストに関する
        '「適用済み」のDL完了通知を発生させることができなくなるため、
        'この段階では捨てないことにする。この段階で捨てないといっても、
        'DeliverGateProにおいて、listDeliveryResultが&HFになることで
        'データ本体を端末へ配信する効力を失うため、適用リストのDL完了を
        '発生させるということを除いて、マスタ適用リストの場合と大差はない。
        'NOTE: 本物の監視盤は、ハッシュ値ではなく、適用リストの内容そのものを
        '比較すると思われる。しかし、万が一そうでなく、かつ、ハッシュ値の
        '偶然の一致により不当に捨てることになる場合に、そのことがリハーサル等で
        '判明しないのは危険であるため、シミュレータではハッシュ値を比較することにする。
        'Dim isNewList As Boolean = True
        If sListHashValue IsNot Nothing AndAlso holdingListVersion = listVersion Then
            If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.ListFileHashValue, sListHashValue) <> 0 Then
                Log.Warn(sMonitorMachineId, "現在保持している適用リストと同一名ですが、内容（※ハッシュ値）が違うため、新しい適用リストとして処理します。")
            Else
                Log.Warn(sMonitorMachineId, "現在保持している適用リストと同一名で内容（※ハッシュ値）も同一であるため、これに基づく端末への配信は発生しません。")
                'isNewList = False
            End If
        End If

        sListHashValue = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        'NOTE: 以下の制御は、完全な推測であり、本物の監視盤と違う可能性がある。
        'しかし、このように号機別の判断を経て、全体の判断をしない限り、
        'ケースが発散してしまい、妥当な制御は不可能と思われる。
        With Nothing
            '配下の改札機ごとに、次に受信可能なバージョンを調べて、
            '制限があるなら、このリストに追加する。
            Dim oRestrictions As New List(Of MasProId)
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                Dim appliedPro As New MasProId(oTerm.HoldingPrograms(0).DataSubKind, oTerm.HoldingPrograms(0).DataVersion, oTerm.HoldingPrograms(0).DataHashValue)

                '適用中のものと違うもの（DL完了通知が適用済み以外になるもの）が
                '適用待ちになっている場合は、それと同じものだけを受け入れ可能とみなす。
                If oTerm.HoldingPrograms(1) IsNot Nothing Then
                    Dim reservedPro As New MasProId(oTerm.HoldingPrograms(1).DataSubKind, oTerm.HoldingPrograms(1).DataVersion, oTerm.HoldingPrograms(1).DataHashValue)
                    If reservedPro <> appliedPro Then
                        oRestrictions.Add(reservedPro)
                        'NOTE: 本当は、配信でPendingされているプログラムもチェックした方がよいが、
                        '不正な媒体投入等がない限りは、reservedProと同じであるはずであるため、
                        '省略する。
                        Continue For
                    End If
                End If

                '適用中のものと違うもの（DL完了通知が適用済み以外になるもの）が
                '配信待ちになっている場合は、それと同じものだけを受け入れ可能とみなす。
                'TODO: ApplicableDateが"99999999"のPendingProgramが存在する場合は、
                'それ以降のPendingProgramをみるべきかもしれない。
                For Each oPenPro As PendingProgram In oTerm.PendingPrograms
                    Dim pendingPro As New MasProId(oPenPro.DataSubKind, oPenPro.DataVersion, oPenPro.DataHashValue)
                    If pendingPro <> appliedPro Then
                        oRestrictions.Add(pendingPro)
                        'NOTE: 本当は、後方にPendingされているプログラムもチェックした方がよいが、
                        '不正な媒体投入等がない限りは、pendingProと同じであるはずであるため、
                        '省略する。
                        Exit For
                    End If
                Next oPenPro
            Next oTerm

            '受信したプログラムが、いずれかの改札機にとって受け入れ可能なものと違う
            '場合は、ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            'NOTE: 監視盤が保持できる改札機向けプログラムは、「配下の全改札機共通で」
            '２世代となっているため、このような残念な制御となる。この制御によって、
            '配下の全改札機について、配信待ちプログラムや適用待ちプログラムは、必ず
            '同じものになる。また、配下の改札機で適用中となるプログラムは、多くて
            '２種類に制限される。
            If oReqTeleg.DataFileName.Length = 0 Then
                For Each pro As MasProId In oRestrictions
                    If dataSubKind <> pro.DataSubKind OrElse dataVersion <> pro.DataVersion
                        Log.Error(sMonitorMachineId, "先行して受け入れたプログラムを全端末に適用する（または捨てる）まで、新たなプログラムの受け入れはできません。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                        Return True
                    End If
                Next pro
            Else
                For Each pro As MasProId In oRestrictions
                    If dataSubKind <> pro.DataSubKind OrElse dataVersion <> pro.DataVersion OrElse _
                       StringComparer.OrdinalIgnoreCase.Compare(sDataHashValue, pro.DataHashValue) <> 0 Then
                        Log.Error(sMonitorMachineId, "先行して受け入れたプログラムを全端末に適用する（または捨てる）まで、新たなプログラムの受け入れはできません。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                        Return True
                    End If
                Next pro
            End If
        End With

        'NOTE: 適用リストに記載されたいずれかの管理対象号機への配信が
        '完了していない状態で、マスタ本体が添付されたDLLを要求された場合は、
        'たとえ、そのパターン番号やマスタバージョンが、監視盤の保持している
        '前回受信バージョンと同一であっても、それを受け入れるべきではない
        '（監視盤の保持しているものを差し替えるべきではない）と思われる。
        'なぜなら、「新たなマスタ本体を受け取る前に要求された配信は、
        'その時点で監視盤が保持していたマスタ本体を以て行うべき」と
        '考えられるためである。しかし、プログラムの場合は、
        '先行DLLされたCABと（バージョンが同じで）内容が違うCABを
        '受け入れないようにしているので、そのような制御は不要である。

        '適用リストの内容を解析する。
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, String)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            'とりあえず、全て読み取る。
            'OPT: 「適用リストの内容を表示する機能」を追加した際に、手っ取り早く
            '実装するために、このように二度読みすることになっている。
            '非効率であり、改善の余地がある。
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '１行ずつ読み取る。
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '以下、適用リストの内容をチェックし、問題がある場合は、
                'ContinueCode.FinishWithoutStoringのREQ電文を作成する。

                'ヘッダ部の１行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストが空です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の１行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "適用リスト1行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '作成年月日をチェックする。
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された作成年月日が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'リストVerをチェックする。
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたリストVerがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストに2行目がありません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "適用リスト2行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'エリアNoをチェックする。
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたエリアNoがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '代表Verをチェックする。
                If Not dataVersion.ToString("D8").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された代表Verがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '機種コードをチェックする。
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された機種がファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '適用リストの３行目以降から、oMonitorMachineに関係する号機を抽出する。
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '読み込んだ行を列に分割する。
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 4 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の項目数が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'サイバネ線区駅順コードの書式をチェックする。
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の駅コードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'コーナーコードの書式をチェックする。
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目のコーナーコードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '号機番号の書式をチェックする。
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の号機番号が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '適用日のレングスをチェックする。
                    If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '適用日がブランクでない場合、値をチェックする。
                    If aColumns(3).Length = 8 Then
                       If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                            Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If

                        Dim applicableDate As DateTime
                        If Not aColumns(3).Equals("99999999") AndAlso _
                           DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, applicableDate) = False Then
                            Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    '行の重複をチェックする。
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目が既出の行と重複しています。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '行の内容を一時保存する。
                    oListedMachines.Add(sLineKey, aColumns(3))

                    '行がoMonitorMachineに関係する場合
                    Dim oTerm As TermMachine = Nothing
                    If oMonitorMachine.TermMachines.TryGetValue(sLineKey, oTerm) = True Then
                        'エリア番号をチェックする。
                        'NOTE: 改札機のプログラムにエリア番号0が指定されることはない（異常事態）という前提である。
                        If dataSubKind <> DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer) Then
                            Log.Error(sMonitorMachineId, "適用リストに記載された端末 [" & sLineKey & "] の所属エリアが、適用リストの対象エリアと異なります。")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "適用リストの読取りで例外が発生しました。", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End Try

        Dim dataAcceptDate As DateTime
        Dim sRunnableDate As String
        Dim oModuleInfos As ProgramModuleInfo()
        Dim sArchiveCatalog As String
        Dim oVersionListData As Byte()

      #If AcceptsSameNameProOfSameHashValue Then
        'NOTE: 運管から監視盤への強制配信に対応した監視盤は、恐らくこのような動作をすると思われる。
        'TODO: そもそも本物の監視盤がプログラムの強制配信に対応しているのか確認した方がよい。
        If oReqTeleg.DataFileName.Length <> 0 Then
            dataAcceptDate = d

            Dim content As GateProgramContent
            Try
                Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
                content = ExtractGateProgramCab(sDataFilePath, Path.Combine(sContextDir, "GatePro"))
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "プログラム本体の解析で例外が発生しました。", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の監視盤の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載されたエリアNoがファイル名と整合していませんが、処理を強行します。")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の監視盤の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載された代表Verがファイル名と整合していませんが、処理を強行します。")
            End If

            sRunnableDate = content.RunnableDate
            oModuleInfos = content.ModuleInfos
            sArchiveCatalog = content.ArchiveCatalog
            oVersionListData = content.VersionListData
        Else
            If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
                Debug.Assert(oMonitorMachine.HoldingPrograms(0) IsNot Nothing)
                dataAcceptDate = oMonitorMachine.HoldingPrograms(0).DataAcceptDate
                sRunnableDate = oMonitorMachine.HoldingPrograms(0).RunnableDate
                oModuleInfos = oMonitorMachine.HoldingPrograms(0).ModuleInfos
                sArchiveCatalog = oMonitorMachine.HoldingPrograms(0).ArchiveCatalog
                oVersionListData = oMonitorMachine.HoldingPrograms(0).VersionListData
            Else
                Debug.Assert(holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion)
                Debug.Assert(oMonitorMachine.HoldingPrograms(1) IsNot Nothing)
                dataAcceptDate = oMonitorMachine.HoldingPrograms(1).DataAcceptDate
                sRunnableDate = oMonitorMachine.HoldingPrograms(1).RunnableDate
                oModuleInfos = oMonitorMachine.HoldingPrograms(1).ModuleInfos
                sArchiveCatalog = oMonitorMachine.HoldingPrograms(1).ArchiveCatalog
                oVersionListData = oMonitorMachine.HoldingPrograms(1).VersionListData
            End If
        End If
      #Else
        'NOTE: 運管から監視盤への強制配信に対応していなかった頃の監視盤は、
        '恐らくこのような動作をしていたと思われる。
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingPrograms(0) IsNot Nothing)
            dataAcceptDate = oMonitorMachine.HoldingPrograms(0).DataAcceptDate
            sRunnableDate = oMonitorMachine.HoldingPrograms(0).RunnableDate
            oModuleInfos = oMonitorMachine.HoldingPrograms(0).ModuleInfos
            sArchiveCatalog = oMonitorMachine.HoldingPrograms(0).ArchiveCatalog
            oVersionListData = oMonitorMachine.HoldingPrograms(0).VersionListData
        ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingPrograms(1) IsNot Nothing)
            dataAcceptDate = oMonitorMachine.HoldingPrograms(1).DataAcceptDate
            sRunnableDate = oMonitorMachine.HoldingPrograms(1).RunnableDate
            oModuleInfos = oMonitorMachine.HoldingPrograms(1).ModuleInfos
            sArchiveCatalog = oMonitorMachine.HoldingPrograms(1).ArchiveCatalog
            oVersionListData = oMonitorMachine.HoldingPrograms(1).VersionListData
        Else
            dataAcceptDate = d

            Debug.Assert(oReqTeleg.DataFileName.Length <> 0)
            Dim content As GateProgramContent
            Try
                Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
                content = ExtractGateProgramCab(sDataFilePath, Path.Combine(sContextDir, "GatePro"))
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "プログラム本体の解析で例外が発生しました。", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の監視盤の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載されたエリアNoがファイル名と整合していませんが、処理を強行します。")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の監視盤の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載された代表Verがファイル名と整合していませんが、処理を強行します。")
            End If

            sRunnableDate = content.RunnableDate
            oModuleInfos = content.ModuleInfos
            sArchiveCatalog = content.ArchiveCatalog
            oVersionListData = content.VersionListData
        End If
      #End If

        'If isNewList Then
        If True Then
            '一時保存していた行が示す各機器に、配信のための情報をキューイングする。
            Dim targetTermCount As Integer = 0
            Dim targetTermFullCount As Integer = 0
            For Each oApplyEntry As KeyValuePair(Of String, String) In oListedMachines
                '行がoMonitorMachineに関係する場合
                Dim oTerm As TermMachine = Nothing
                If oMonitorMachine.TermMachines.TryGetValue(oApplyEntry.Key, oTerm) = True Then
                    '適用日が現在の運用日付と同じか未来あるいは「19000101」か「99999999」の場合のみ、
                    '配信が（このアプリの場合は、DL完了通知が）必要とみなす。
                    'NOTE: 本物の監視盤は、この条件に該当していない行について、「適用済み」のDL完了通知を
                    '送り付けてきたような気がする（そのために、運管側は、既に「正常」になっている場合は
                    '「適用済み」のDL完了通知を無視しなければならなくなった）。
                    'よって、最新の監視盤は、そもそも適用日が過去日の行であっても、当該行の改札機に
                    '当該プログラムを未配信であれば、配信してしまうのかもしれない。
                    'もしそうだとすると、かなり問題である。
                    '運管は、適用日が過去日の行は、適用日がブランクの行と同じ扱いにすることになっている。
                    'それゆえに、適用リストにそのような行しかなければ、監視盤に対して配信しない。
                    'また、DLLシーケンスが完了した際（監視盤まで配信が完了した際）も、そのような適用日が
                    '記載されている改札機については、配信状態を「配信中」にはしない。
                    '運管の動作はI/F仕様（ツール仕様書の別紙6）に完全に合致している。
                    'TODO: 本物の監視盤が「適用済み」を送り付けてくる件について、システム試験では、それを
                    '無視するように運管側を改造し、監視盤チームの考える仕様通りということでOKとしたが、
                    '監視盤の実装がどうなっているのか、システムとして問題がないのか、システム試験で
                    '実施していないケース（適用日が過去日の行の改札機に対して未配信だったケース等）に
                    'ついても、運管の動作も含めて、検証するべきである。
                    If oApplyEntry.Value.Length = 8 AndAlso _
                      (oApplyEntry.Value.Equals("19000101") OrElse _
                       String.CompareOrdinal(oApplyEntry.Value, sServiceDate) >= 0) Then
                        Log.Debug(sMonitorMachineId, "適用リストに記載された端末 [" & oApplyEntry.Key & "] 適用日 [" & oApplyEntry.Value & "] の行をキューイングします。")
                        Dim oPenPro As New PendingProgram()
                        oPenPro.DataSubKind = dataSubKind
                        oPenPro.DataVersion = dataVersion
                        oPenPro.ListVersion = listVersion
                        oPenPro.DataAcceptDate = dataAcceptDate
                        oPenPro.RunnableDate = sRunnableDate
                        oPenPro.ModuleInfos = oModuleInfos
                        oPenPro.ArchiveCatalog = sArchiveCatalog
                        oPenPro.VersionListData = oVersionListData
                        oPenPro.DataHashValue = sDataHashValue
                        oPenPro.ListAcceptDate = listAcceptDate
                        oPenPro.ApplicableDate = oApplyEntry.Value
                        oPenPro.ListContent = sListContent
                        oPenPro.ListHashValue = sListHashValue
                        oTerm.PendingPrograms.AddLast(oPenPro)
                        UpdateTable2OnTermStateChanged(sMonitorMachineId, oApplyEntry.Key, oTerm)
                        targetTermCount += 1
                    Else
                        Log.Debug(sMonitorMachineId, "適用リストに記載された端末 [" & oApplyEntry.Key & "] 適用日 [" & oApplyEntry.Value & "] の行は除外します。")
                    End If
                    targetTermFullCount += 1
                End If
            Next oApplyEntry
            Log.Debug(sMonitorMachineId, "適用リストに記載された" & oListedMachines.Count.ToString() & "台のうち、" & targetTermFullCount.ToString() & "台が当該機器の端末でした。そのうち" & targetTermCount.ToString() & "台の適用日が有効でした。")

            'NOTE: 下記のケースで、本物の監視盤がどのような反応を示すかは、よくわからない。
            'しかし、このような部材の情報をoMonitorMachine.HoldingPrograms(1)に格納するわけにはいかない。
            '配下のTermMachineのPendingProgramsに登録していないということは、次に運管から受け入れ可能と
            'するバージョンが、それに制限される保証がないということである。
            '実害の有無は微妙であるが、受け入れ許可するプログラムをQに制限しないにもかかわらず、
            'QがoMonitorMachine.HoldingPrograms(1)に登録されている状態というのは、紛らわしすぎる。
            If targetTermCount = 0 Then
                Log.Error(sMonitorMachineId, "配信を生み出さない適用リストを受信しました。")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        End If

        '監視盤の改札機向けプログラム保持状態を更新する。
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingPrograms(0) IsNot Nothing)
            'NOTE: このケースでは、想定ではoMonitorMachine.HoldingPrograms(1)が空のはずである。
            '今回受信したプログラムPと同じものがHoldingPrograms(0)に格納されている
            'ということは、配下にある全改札機がそれを適用していることになる。
            '仮にHoldingPrograms(1)に何かを格納しているのであれば、前述のPの適用後、
            'Pと違うバージョンのプログラムを受け入れている（一部号機に先行適用されていたり、
            '適用待ちになっていたり、配信待ちになっている）ということであり、
            '今回受信したPは、受け入れ拒否しているはずである。
            If oMonitorMachine.HoldingPrograms(1) IsNot Nothing Then
                Log.Warn(sMonitorMachineId, "端末への適用が済んでいない新世代プログラムを保持しているにもかかわらず、旧世代プログラムの適用リスト受け入れを決定しました。想定外の状況ですが、処理を強行します。")
            End If
          #If AcceptsSameNameProOfSameHashValue Then
            If oReqTeleg.DataFileName.Length <> 0 Then
                Log.Info(sMonitorMachineId, "保持(1)にある同一エリアNo・同一代表Verのプログラムを上書きします。")
                Debug.Assert(dataAcceptDate = d)
                oMonitorMachine.HoldingPrograms(0).DataAcceptDate = dataAcceptDate
                oMonitorMachine.HoldingPrograms(0).RunnableDate = sRunnableDate
                oMonitorMachine.HoldingPrograms(0).ModuleInfos = oModuleInfos
                oMonitorMachine.HoldingPrograms(0).ArchiveCatalog = sArchiveCatalog
                oMonitorMachine.HoldingPrograms(0).VersionListData = oVersionListData
                oMonitorMachine.HoldingPrograms(0).DataHashValue = sDataHashValue
            End If
          #End If
            Debug.Assert(listAcceptDate = d)
            oMonitorMachine.HoldingPrograms(0).ListVersion = listVersion
            oMonitorMachine.HoldingPrograms(0).ListAcceptDate = listAcceptDate
            oMonitorMachine.HoldingPrograms(0).ListContent = sListContent
            oMonitorMachine.HoldingPrograms(0).ListHashValue = sListHashValue
        ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingPrograms(1) IsNot Nothing)
          #If AcceptsSameNameProOfSameHashValue Then
            If oReqTeleg.DataFileName.Length <> 0 Then
                Log.Info(sMonitorMachineId, "保持(2)にある同一エリアNo・同一代表Verのプログラムを上書きします。")
                Debug.Assert(dataAcceptDate = d)
                oMonitorMachine.HoldingPrograms(1).DataAcceptDate = dataAcceptDate
                oMonitorMachine.HoldingPrograms(1).RunnableDate = sRunnableDate
                oMonitorMachine.HoldingPrograms(1).ModuleInfos = oModuleInfos
                oMonitorMachine.HoldingPrograms(1).ArchiveCatalog = sArchiveCatalog
                oMonitorMachine.HoldingPrograms(1).VersionListData = oVersionListData
                oMonitorMachine.HoldingPrograms(1).DataHashValue = sDataHashValue
            End If
          #End If
            Debug.Assert(listAcceptDate = d)
            oMonitorMachine.HoldingPrograms(1).ListVersion = listVersion
            oMonitorMachine.HoldingPrograms(1).ListAcceptDate = listAcceptDate
            oMonitorMachine.HoldingPrograms(1).ListContent = sListContent
            oMonitorMachine.HoldingPrograms(1).ListHashValue = sListHashValue
        Else
            'NOTE: このケースでは、想定ではoMonitorMachine.HoldingPrograms(1)が空のはずである。
            If oMonitorMachine.HoldingPrograms(1) IsNot Nothing Then
                Log.Warn(sMonitorMachineId, "端末への適用が済んでいない新世代プログラムを保持しているにもかかわらず、別の新世代プログラムの受け入れを決定しました。想定外の状況ですが、処理を強行します。")
            End If
            Log.Info(sMonitorMachineId, "新たなプログラムを保持(2)に受け入れます。")
            oMonitorMachine.HoldingPrograms(1) = New HoldingProgram()
            Debug.Assert(dataAcceptDate = d)
            Debug.Assert(listAcceptDate = d)
            oMonitorMachine.HoldingPrograms(1).DataSubKind = dataSubKind
            oMonitorMachine.HoldingPrograms(1).DataVersion = dataVersion
            oMonitorMachine.HoldingPrograms(1).ListVersion = listVersion
            oMonitorMachine.HoldingPrograms(1).DataAcceptDate = dataAcceptDate
            oMonitorMachine.HoldingPrograms(1).RunnableDate = sRunnableDate
            oMonitorMachine.HoldingPrograms(1).ModuleInfos = oModuleInfos
            oMonitorMachine.HoldingPrograms(1).ArchiveCatalog = sArchiveCatalog
            oMonitorMachine.HoldingPrograms(1).VersionListData = oVersionListData
            oMonitorMachine.HoldingPrograms(1).DataHashValue = sDataHashValue
            oMonitorMachine.HoldingPrograms(1).ListAcceptDate = listAcceptDate
            oMonitorMachine.HoldingPrograms(1).ApplicableDate = Nothing
            oMonitorMachine.HoldingPrograms(1).ListContent = sListContent
            oMonitorMachine.HoldingPrograms(1).ListHashValue = sListHashValue
        End If
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        Log.Info(sMonitorMachineId, "受け入れが完了しました。")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverGatePro(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '全改札機のキューから全ての改札機向けプログラム適用リストを取り出し、
        '適用リストごとに、当該改札機(t)のプログラム保持状態
        '（oMonitorMachine.TermMachines(t).HoldingPrograms）を
        '更新し、sMachineDirに適用リスト別・改札機別の
        '#GateProDlReflectReq_RRRSSSCCCCUU_N.dat（Nは0～）を作成する。
        'また、プログラム保持状態を更新した改札機については、
        'sContextDirにGateProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、GateProDlReflectReq_RRRSSSCCCCUU_N.datと
        'GateProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの、今回更新
        'していない改札機のもの）を削除する。

        'NOTE: これらのファイル名のRRRSSSCCCCUUは端末の機器コードで
        'あるが、これらのファイルが全端末分（シナリオ内で
        '「%T3R%T3S%T4C%T2U」と記述した場合に複製される全行分）
        '作成されるとは限らない。
        'よって、シナリオは、当該ファイルを送信する際、
        'ActiveOneではなく、TryActiveOneを使用する。

        DeleteFiles(sMonitorMachineId, sContextDir, "GateProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now

        '全端末について処理を行う。
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            Dim oTerm As TermMachine = oTermEntry.Value

            If oTerm.PendingPrograms.Count = 0 Then
                Log.Debug(sMonitorMachineId, "端末 [" & sTermMachineId & "] に対するプログラム配信はありません。")
                Continue For
            ElseIf oTerm.McpStatusFromKsb <> &H0 Then
                Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] については、主制状態が正常以外に設定されているため、配信処理を保留します。")
                Continue For
            End If

            Dim isHoldingProUpdated As Boolean = False

            'TODO: 本物の監視盤は、最後にキューイングしたものしか改札機に送信しないかもしれない。
            'それは仕方ないとしても、最後にキューイングした要求に対応するDL完了通知しか発生させない可能性も
            'ある（要求されたことを行えないなら「異常」のDL完了通知を発生させれば済むにもかかわらず）。
            'もしそうだとしたら、運管に対する働きが実機とシミュレータで違う... ということになってしまうので、
            'シミュレータでも、最後にキューイングされているもの以外は読み捨てた方がよいかもしれない。
            'なお、たとえそうするにしても、TermMachineクラスのPendingProgramsは必要である。
            'シミュレータの機能として、改札機に未配信のものをユーザに示す必要があるためである。

            '配信処理に使っていない全適用リストについて処理を行う。
            For Each oPenPro As PendingProgram In oTerm.PendingPrograms
                'NOTE: 実体のない（ListHashValue Is Nothing の）適用リストで配信が行われる可能性は想定しない。
                Log.Info(sMonitorMachineId, "適用リスト [" & oPenPro.ListVersion.ToString() & "] に基づき、端末 [" & sTermMachineId & "] に対するエリアNo [" & oPenPro.DataSubKind.ToString() & "] 代表Ver [" & oPenPro.DataVersion.ToString() & "] のプログラム配信処理を行います...")

                If oPenPro.ApplicableDate.Equals("99999999") Then
                    Log.Info(sMonitorMachineId, "※当該端末に対する要求は改造中止要求です。")
                End If

                '当該端末に対して最後に配信した適用リストの情報を取得する。
                Dim latestDataSubKind As Integer = oTerm.HoldingPrograms(0).DataSubKind
                Dim latestDataVersion As Integer = oTerm.HoldingPrograms(0).DataVersion
                Dim latestListVersion As Integer = oTerm.HoldingPrograms(0).ListVersion
                Dim latestListHashValue As String = oTerm.HoldingPrograms(0).ListHashValue
                If oTerm.HoldingPrograms(1) IsNot Nothing Then
                    latestDataSubKind = oTerm.HoldingPrograms(1).DataSubKind
                    latestDataVersion = oTerm.HoldingPrograms(1).DataVersion
                    latestListVersion = oTerm.HoldingPrograms(1).ListVersion
                    latestListHashValue = oTerm.HoldingPrograms(1).ListHashValue
                End If

                '適用リストの配信結果（「正常」または「異常」「適用済み」）を決める。
                'TODO: 本物の監視盤は適用リストバージョンしか比較しないかもしれない。
                '最新の（適用済み対応の）監視盤では適用リストの内容を比較すると信じたい。
                'NOTE: そもそも本物の監視盤は、適用リストの「適用済み」を判断するのに、
                '現在適用中のプログラムを適用した際に用いた適用リストと比較するのかもしれない。
                '適用リストの場合、プログラム本体と異なり、改札機が待機面に保持していたり
                '監視盤が改札機への配信待ちにしているものが、同一代表バージョンでありながら、
                '何種類もあり得るわけだから、さすがにそのような仕様ではないと思われるが...
                'NOTE: このアプリでは、本物の監視盤に合わせて、適用リストに対する「適用済み」も
                '生成できるようにしてはいるが、そもそも、適用リストに対する「適用済み」という
                '発想自体、変である。適用リストには名前などなく、個別に（運管からDLLの要求
                'ごとに）意味があるものなのだから、わざわざ比較して「適用済み」などとせずに、
                '有意な適用日が指定されている改札機には必ず送り付ける方が、理に適っている。
                Dim listDeliveryResult As Byte = &H0

                If latestListHashValue IsNot Nothing AndAlso _
                   oPenPro.DataSubKind = latestDataSubKind AndAlso _
                   oPenPro.DataVersion = latestDataVersion AndAlso _
                   oPenPro.ListVersion = latestListVersion AndAlso _
                   StringComparer.OrdinalIgnoreCase.Compare(oPenPro.ListHashValue, latestListHashValue) = 0 Then
                    '「改札機が適用待ちの部材と一緒に保持している適用リスト」と同じものを配信する
                    'ことになる場合は、配信結果を「適用済み」とする。
                    'NOTE: そのケースでは、改造中止要求の適用リストに対しても「適用済み（改造中止済み？）」
                    'で済ましてしまうが、本物の監視盤もそうであるかは不明。そもそも、前回の配信時に
                    '改造中止をしているとしたら、待機面から消えているはずなので、普通にはあり得ない
                    'ケースと思われる。
                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        Log.Warn(sMonitorMachineId, "当該端末に対しては当該適用リストを配信済みです。適用リストの再配信を行いませんので、改造中止も行いません。")
                    Else
                        Log.Warn(sMonitorMachineId, "当該端末に対しては当該適用リストを配信済みです。適用リストの再配信は行いません。適用リストに基づくプログラム本体の配信も行いません。")
                    End If
                    listDeliveryResult = &HF
                End If

                If listDeliveryResult = &H0 Then
                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        'NOTE: oPenProのバージョンのプログラムが既に適用中になっているケースも
                        '以下のケース（無効な改造中止要求）に当てはまるはずである。
                        'NOTE: 「oTerm.HoldingPrograms(1) Is Nothing」でない場合においては、
                        '万が一「oTerm.HoldingPrograms(1).DataVersion = 0」であるとしても、
                        'それは、端末がバージョン0のプログラムを保持しているということである。
                        'よって、oPenPro.DataVersionも0であり、エリア番号も一致するなら、
                        'oPenProは有効な改造中止であり、以下の条件が偽になってよい。
                        If oTerm.HoldingPrograms(1) Is Nothing OrElse _
                           oTerm.HoldingPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                           oTerm.HoldingPrograms(1).DataVersion <> oPenPro.DataVersion Then
                            'NOTE: 本物の監視盤がこのように厳密な動作をするのかは、不明である。
                            Log.Error(sMonitorMachineId, "無効な改造中止要求です。当該端末において当該プログラムが適用待ちになっていません。")
                            listDeliveryResult = &H1
                        End If
                    End If
                End If

                'NOTE: 適用リストが適用済みの場合や、改造中止要求の適用リストが無効な場合は、
                'プログラム本体のDL完了通知は発生させない。それらのケースでは、
                'プログラム本体は配信対象でないはずであり、問題ないはず。
                '改造中止の行を含む適用リストがプログラム本体とともにDLLされるケースは想定しない。
                'TODO: 本物の監視盤＆改札機がどう動くかは分からない。
                If listDeliveryResult = &H0 Then
                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        'NOTE: 適用日が「99999999」の行の改札機については、プログラム本体のDL完了通知は
                        '（適用済みなども含めて）生成しないことにする。本物の監視盤がどうなのかは不明。
                        'TODO: 運管において、ある改札機に対するあるバージョンのプログラムの初回の配信指示で、
                        '適用リストに「99999999」を記載してしまったり、「99999999」が記載された適用リストで
                        '配信を行う際に「プログラム+プログラム適用リスト 強制配信」にチェックを入れて
                        'しまったりすると、プログラム本体に関する当該改札機の受信状態が「配信中」になり、
                        'それがそのまま残ってしまうと思われる。これについては、適用日「99999999」が指定
                        'された改札機について「配信中」のレコードを作成しないように、そして、できること
                        'なら「99999999」が記載された適用リストで「プログラム+プログラム適用リスト 強制配信」
                        'を指定できないように、運管の実装を改善するべきである。

                        '改札機のプログラム保持状態を更新する。
                        'NOTE: 本物の監視盤は、たとえ一部の改札機の改造中止であっても、
                        '監視盤の保持バージョンまで変わっていたかもしれない...
                        oTerm.HoldingPrograms(1) = Nothing
                        isHoldingProUpdated = True
                        Log.Info(sMonitorMachineId, "当該端末に対して改造中止を行いました。")
                    Else
                        'NOTE: たとえ適用リストに関するDL完了通知が「適用済み」であったとしても、
                        'プログラム本体のDL完了通知も生成する（適用リストに関する「適用済み」など
                        'という概念が持ち込まれたことで、違和感があるかもしれないが、運管からの
                        'DLL要求には、適用リストのバージョンなどに関係なく、個別に意味がある）。
                        'また、たとえ適用リストが適用済み（= 実際は、単なる送信済み）であったとしても、
                        'その適用リストにおいて、当該プログラム未適用の改札機に有意な適用日が記載されて
                        'いれば、プログラム本体については「適用済み」ではなく「正常」のDL完了通知を
                        '生成する。
                        'TODO: 本物の監視盤は、適用リストに関するDL完了通知が「適用済み」である場合に、
                        'プログラム本体のDL完了通知（おそらく「適用済み」）を生成しないかもしれない。
                        'その状況では、運管における当該改札機の当該プログラムの受信状態も「配信中」
                        'ではなく「正常」等になっていると思われるが、本当にその保証があるのか
                        '検証した方がよい。
                        'TODO: このアプリでは、良心に基づいて比較しているが、本物の監視盤が
                        'CABの内容を比較して、不一致の場合に再配信を行うかは不明である。

                        'プログラム本体の配信結果（「正常」または「適用済み」）を決める。
                        Dim dataDeliveryResult As Byte = &H0

                        'Dim sServiceDate As String = EkServiceDate.GenString(d)
                        'If String.CompareOrdinal(oPenPro.ApplicableDate, sServiceDate) < 0 Then
                        'End If

                        'NOTE: 本物の改札機＆監視盤はハッシュ値の比較を行わないかもしれない。
                        'ただし、そもそも、あるバージョンのプログラムが既に改札機に入っている（入ることが確定している）状況では、
                        'それと同じバージョンで内容の異なるプログラムを監視盤が受け入れること自体がないはずなので、
                        '比較しても動作に違いはないと思われる。

                        If oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                           oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion AndAlso _
                           StringComparer.OrdinalIgnoreCase.Compare(oTerm.HoldingPrograms(0).DataHashValue, oPenPro.DataHashValue) = 0 Then
                            'TODO: 改札機が適用中のものと同じバージョンのプログラムを改札機に配信することはできないはずなので、
                            'ハッシュ値が異なる場合でも異常扱いにするべきかもしれないが、それをするなら、そもそも
                            'AcceptGateProにて監視盤自身への受け入れを防止するべきである。
                            'NOTE: 適用日前のものを改札機が適用しているはずはないし、適用日を過ぎたものを改札機に配信しようとする
                            'はずもない。しかし、改札機が適用中のものと同バージョンのプログラムを、適用日当日に、
                            '監視盤が受信したケースや、適用日前に監視盤が受信し、改札機に配信しないまま適用日が過ぎた
                            'ケースなどは、あり得る。後者は本関数内で適用日と運用日を比較して、別の異常扱いにすることも
                            '可能であるが、前者はそうはいかない。
                            'TODO: とりあえず配信結果を「適用済み」とするが、本物の監視盤に合わせた方がよい。
                            Log.Warn(sMonitorMachineId, "当該端末には同バージョンのプログラムを適用済みです。プログラム本体の再配信は行いません。")
                            dataDeliveryResult = &HF
                        End If

                        If dataDeliveryResult = &H0 Then
                            If oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                               oTerm.HoldingPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                               oTerm.HoldingPrograms(1).DataVersion = oPenPro.DataVersion AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oTerm.HoldingPrograms(1).DataHashValue, oPenPro.DataHashValue) = 0 Then
                                '改札機が適用待ちにしているものと同じものの配信指示があった場合は、
                                '配信結果を「適用済み」とする。
                                Log.Warn(sMonitorMachineId, "当該端末に対しては当該プログラムを適用済み（※配信済み）です。プログラム本体の再配信は行いません。")
                                dataDeliveryResult = &HF
                            End If
                        End If

                        If dataDeliveryResult = &H0 Then
                            If Not oPenPro.ApplicableDate.Equals("19000101") AndAlso _
                               String.CompareOrdinal(oPenPro.ApplicableDate, oPenPro.RunnableDate) < 0 Then
                                Log.Error(sMonitorMachineId, "プログラムの動作許可日が適用日以降に設定されています。配信は行いません。")
                                dataDeliveryResult = &HC
                                listDeliveryResult = &H1 'TODO: 適用リストの配信結果は３種類しかない。本物は適用リストを配信するのかもしれない。
                            End If
                        End If

                        '改札機のプログラム保持状態を更新する。
                        If dataDeliveryResult = &H0 Then
                            If oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                              (oTerm.HoldingPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                               oTerm.HoldingPrograms(1).DataVersion <> oPenPro.DataVersion OrElse _
                               StringComparer.OrdinalIgnoreCase.Compare(oTerm.HoldingPrograms(1).DataHashValue, oPenPro.DataHashValue) <> 0) Then
                                Log.Warn(sMonitorMachineId, "新世代プログラムが適用待ちになっている端末に対する、別の新世代プログラムの配信です。想定外の状況ですが配信を強行します。")
                            End If
                            Debug.Assert(listDeliveryResult = &H0)
                            Dim oPro As New HoldingProgram()
                            oPro.DataSubKind = oPenPro.DataSubKind
                            oPro.DataVersion = oPenPro.DataVersion
                            oPro.ListVersion = oPenPro.ListVersion
                            oPro.DataAcceptDate = oPenPro.DataAcceptDate
                            oPro.DataDeliverDate = d
                            oPro.RunnableDate = oPenPro.RunnableDate
                            oPro.ModuleInfos = oPenPro.ModuleInfos
                            oPro.ArchiveCatalog = oPenPro.ArchiveCatalog
                            oPro.VersionListData = oPenPro.VersionListData
                            oPro.DataHashValue = oPenPro.DataHashValue
                            oPro.ListAcceptDate = oPenPro.ListAcceptDate
                            oPro.ListDeliverDate = d
                            oPro.ApplicableDate = oPenPro.ApplicableDate
                            oPro.ListContent = oPenPro.ListContent
                            oPro.ListHashValue = oPenPro.ListHashValue
                            oTerm.HoldingPrograms(1) = oPro
                            isHoldingProUpdated = True
                            Log.Info(sMonitorMachineId, "当該端末の待機面に対して当該プログラム本体の配信を行いました。")
                            Log.Info(sMonitorMachineId, "当該端末の待機面に対して当該適用リストの配信を行いました。")
                        ElseIf listDeliveryResult = &H0 Then
                            If oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                               oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion Then
                                oTerm.HoldingPrograms(0).ListVersion = oPenPro.ListVersion
                                oTerm.HoldingPrograms(0).ListAcceptDate = oPenPro.ListAcceptDate
                                oTerm.HoldingPrograms(0).ListDeliverDate = d
                                oTerm.HoldingPrograms(0).ApplicableDate = oPenPro.ApplicableDate
                                oTerm.HoldingPrograms(0).ListContent = oPenPro.ListContent
                                oTerm.HoldingPrograms(0).ListHashValue = oPenPro.ListHashValue
                                'TODO: oTerm.HoldingPrograms(0).ListVersionを更新しても、
                                '基本的に改札機プログラムバージョン情報に変化は無いはずなので、
                                '以下は行わない方がよいかもしれない。
                                'TODO: このアプリでは、待機面にある適用リストにこそ意味がある
                                'ものとして、改札機プログラムバージョン情報にセットしているが、
                                'そもそも本物の改札機システムがどうであるかはわからない。
                                isHoldingProUpdated = True
                                Log.Warn(sMonitorMachineId, "当該端末の適用面に対して当該適用リストの配信を行いました。この適用日は意味を持ちませんので注意してください。")
                            ElseIf oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                                   oTerm.HoldingPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                                   oTerm.HoldingPrograms(1).DataVersion = oPenPro.DataVersion Then
                                oTerm.HoldingPrograms(1).ListVersion = oPenPro.ListVersion
                                oTerm.HoldingPrograms(1).ListAcceptDate = oPenPro.ListAcceptDate
                                oTerm.HoldingPrograms(1).ListDeliverDate = d
                                oTerm.HoldingPrograms(1).ApplicableDate = oPenPro.ApplicableDate
                                oTerm.HoldingPrograms(1).ListContent = oPenPro.ListContent
                                oTerm.HoldingPrograms(1).ListHashValue = oPenPro.ListHashValue
                                isHoldingProUpdated = True
                                Log.Info(sMonitorMachineId, "当該端末の待機面に対して当該適用リストの配信を行いました。")
                            Else
                                'TODO: よく検証しないとあり得るケースかどうか分からない。
                                Log.Error(sMonitorMachineId, "当該端末において、当該適用リストに紐づくプログラム本体がありません。適用リストの配信は行いません。")
                                listDeliveryResult = &H1
                            End If
                        End If

                        'プログラム本体に関する#GateProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
                        CreateFileOfGateProDlReflectReq( _
                           &H21, _
                           oPenPro.DataVersion, _
                           dataDeliveryResult, _
                           sMonitorMachineId, _
                           sTermMachineId, _
                           sMachineDir)
                    End If
                End If

                'NOTE: この配信の前に直接投入を実施した場合など、改札機に適用リストが
                '存在しない場合は、下記を行わない。
                '実物の改札機システムの挙動を（良し悪しに関係なく）忠実に再現する。
                If latestListHashValue IsNot Nothing Then
                    '適用リストに関する#GateProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
                    CreateFileOfGateProDlReflectReq( _
                       &H48, _
                       oPenPro.ListVersion, _
                       listDeliveryResult, _
                       sMonitorMachineId, _
                       sTermMachineId, _
                       sMachineDir)
                Else
                    Log.Warn(sMonitorMachineId, "当該端末が適用リストを保持していなかったため、適用リストのDL完了通知は作成しませんでした。これは自動改札機システムの制限事項です。")
                End If
            Next oPenPro
            oTerm.PendingPrograms.Clear()

            If isHoldingProUpdated Then
                CreateFileOfGateProVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
            End If

            UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
        Next oTermEntry

        Return True
    End Function

    Protected Function ApplyGatePro(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '全改札機について、待機面にプログラムを保持しているかチェックし、
        'その適用日が運用日以前であれば、適用面に移動する。
        '全ての改札機の適用中バージョンが同一になり、
        'それが監視盤において、新世代面に格納されている場合は、
        '旧世代面に移動する。
        'また、プログラム保持状態を更新した改札機については、
        'sContextDirにGateProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、GateProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回更新していない改札機のもの）を削除する。

        'NOTE: これらのファイル名のRRRSSSCCCCUUは端末の機器コードで
        'あるが、これらのファイルが全端末分（シナリオ内で
        '「%T3R%T3S%T4C%T2U」と記述した場合に複製される全行分）
        '作成されるとは限らない。
        'よって、シナリオは、当該ファイルを送信する際、
        'ActiveOneではなく、TryActiveOneを使用する。

        DeleteFiles(sMonitorMachineId, sContextDir, "GateProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        '全端末について処理を行う。
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            Dim oTerm As TermMachine = oTermEntry.Value
            If oTerm.HoldingPrograms(1) Is Nothing Then
                Log.Debug(sMonitorMachineId, "端末 [" & sTermMachineId & "] には、適用待ちのプログラムがありません。")
            ElseIf oTerm.HoldingPrograms(1).ListHashValue IsNot Nothing AndAlso _
                   String.CompareOrdinal(oTerm.HoldingPrograms(1).ApplicableDate, sServiceDate) > 0 Then
                Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] には、適用待ちのプログラムがありますが、適用日前であるため、適用しません。")
            ElseIf String.CompareOrdinal(oTerm.HoldingPrograms(1).RunnableDate, sServiceDate) > 0 Then
                Log.Warn(sMonitorMachineId, "端末 [" & sTermMachineId & "] には、適用待ちのプログラムがありますが、動作許可日前であるため、適用しません。")
            Else
                oTerm.HoldingPrograms(0) = oTerm.HoldingPrograms(1)
                oTerm.HoldingPrograms(0).ApplyDate = d
                oTerm.HoldingPrograms(1) = Nothing
                Log.Info(sMonitorMachineId, "端末 [" & sTermMachineId & "] において、適用待ちのプログラムを適用しました。")
                CreateFileOfGateProVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
            End If
        Next oTermEntry

        'TODO: 全改札機の適用中バージョンが揃ったにもかかわらず、
        '監視盤において、それが新世代面にも旧世代面にも保持できていない
        'ケースや、既に旧世代面に移動済みでありながら新世代面に
        '何か存在しているケースなど、（不正な媒体投入を行うなどしない限り）
        'あり得ないケースの場合は、警告を出した上で、監視盤の状態を補正する
        'べきかもしれない。
        If oMonitorMachine.HoldingPrograms(1) IsNot Nothing Then
            Dim isAppliedToAllTerm As Boolean = True
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                If oTerm.HoldingPrograms(0).DataSubKind <> oMonitorMachine.HoldingPrograms(1).DataSubKind OrElse _
                   oTerm.HoldingPrograms(0).DataVersion <> oMonitorMachine.HoldingPrograms(1).DataVersion Then
                    isAppliedToAllTerm = False
                    Exit For
                End If
            Next oTerm
            If isAppliedToAllTerm Then
                oMonitorMachine.HoldingPrograms(0) = oMonitorMachine.HoldingPrograms(1)
                oMonitorMachine.HoldingPrograms(1) = Nothing
                Log.Info(sMonitorMachineId, "保持(2)のプログラムが全端末に適用されたため、保持(1)に移動しました。")
                UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
            End If
        End If

        Return True
    End Function

    Protected Function DirectInstallKsbPro(ByVal sContextDir As String, ByVal sFilePath As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        'Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(sFilePath)
        Catch ex As Exception
            Log.Error("プログラム本体のハッシュ値算出で例外が発生しました。", ex)
            Return False
        End Try

        Dim content As KsbProgramContent
        Try
            content = ExtractKsbProgramCab(sFilePath, Path.Combine(sContextDir, "KsbPro"))
        Catch ex As Exception
            Log.Error("プログラム本体の解析で例外が発生しました。", ex)
            Return False
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("バージョンリストからのエリアNoの抽出で例外が発生しました。", ex)
            Return False
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("バージョンリストからの代表Verの抽出で例外が発生しました。", ex)
            Return False
        End Try

        'TODO: ここで、部材のエリアと監視盤が認識している監視盤エリアの整合性チェックを行うことも可能と思われる。
        'もし実機がチェックを行うなら、それに合わせた方がよい。

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("プログラムの動作許可日が未来日に設定されています。" & content.RunnableDate & "まで適用できませんのでご注意ください。")
        End If

        InstallKsbProgramDirectly(sContextDir, subKind, version, content, sHashValue)

        Return True
    End Function

    Protected Function AcceptKsbPro(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDirの#PassiveDllReq.datが示すファイルを
        'oMonitorMachine.PendingKsbProgramsにキューイングし、
        'sContextDirにExtOutput.datを作成する。
        'ただし、受け入れ不可能な場合は、キューイングせずに、
        'ContinueCodeがFinishWithoutStoringのExtOutput.datを作成する。

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "電文書式に不正があります。")
            Return False
        End If

        Log.Info(sMonitorMachineId, "適用リストのファイル名は [" & oReqTeleg.ListFileName & "] です。")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "プログラム本体のファイル名は [" & oReqTeleg.DataFileName & "] です。")
        Else
            Log.Info(sMonitorMachineId, "プログラム本体のファイル名はありません。")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "適用リストのファイル名が不正です。")
            Return False
        End If

        Dim sApplicableModel As String = EkMasProListFileName.GetDataApplicableModel(sListFileName)
        Dim sDataKind As String = EkMasProListFileName.GetDataKind(sListFileName)
        Dim dataSubKind As Integer = EkMasProListFileName.GetDataSubKindAsInt(sListFileName)
        Dim dataVersion As Integer = EkMasProListFileName.GetDataVersionAsInt(sListFileName)
        Dim listVersion As Integer = EkMasProListFileName.GetListVersionAsInt(sListFileName)

        If oReqTeleg.DataFileName.Length <> 0 Then
            Dim sDataFileName As String = Path.GetFileName(oReqTeleg.DataFileName)
            If Not EkProgramDataFileName.IsValid(sDataFileName) Then
                Log.Error(sMonitorMachineId, "プログラム本体のファイル名が不正です。")
                Return False
            End If
            If EkProgramDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "ファイル名（適用先機種）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "ファイル名（プログラム種別）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "ファイル名（エリアNo）に不整合があります。")
                Return False
            End If
            If EkProgramDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "ファイル名（代表Ver）に不整合があります。")
                Return False
            End If
        End If

        'NOTE: 基本的に、oReqTelegのSubObjCodeとListFileNameの整合性は、シナリオ側
        'で保証する想定である。また、DataFileNameが空でない場合のListFileNameとの
        '整合性についても、シナリオ側で保証する想定である。シナリオ側でなら、
        '整合性のないREQに対し、NAKを返信するように設定できるためである。
        'ただし、たとえ、シナリオ側でチェックしないとしても、そもそも、整合性のない
        '配信を行えてしまえたなら、それは運管システムの不具合のはずであるから、
        'シナリオが実施結果NGで終了する方がよい。以上のことから、ここでも
        '整合性のチェックを行い、整合性がない場合は、否定応答を返却する。
        If sApplicableModel <> Config.ModelSym Then
            Log.Error(sMonitorMachineId, "ファイル名（適用先機種）が不正です。")
            Return False
        End If
        If sDataKind <> "WPG" Then
            Log.Error(sMonitorMachineId, "ファイル名（プログラム種別）が不正です。")
            Return False
        End If
        If oReqTeleg.SubObjCode <> &H0 Then
            Log.Error(sMonitorMachineId, "電文のサブ種別が不正です。")
            Return False
        End If

        'NOTE: プログラムを保持していないときは、
        'holding0Versionやholding1Versionは0になる。
        'NOTE: EkMasProListFileName.IsValid()でのチェックによって、
        'dataVersionが0になることはあり得ないため、上記ケースでは、
        'holding0Versionおよびholding1Versionが、dataVersionと一致
        'することはあり得ない。
        Dim holding0SubKind As Integer = 0
        Dim holding0Version As Integer = 0
        Dim holding1SubKind As Integer = 0
        Dim holding1Version As Integer = 0
        If oMonitorMachine.HoldingKsbPrograms(0) IsNot Nothing Then
            holding0SubKind = oMonitorMachine.HoldingKsbPrograms(0).DataSubKind
            holding0Version = oMonitorMachine.HoldingKsbPrograms(0).DataVersion
        End If
        If oMonitorMachine.HoldingKsbPrograms(1) IsNot Nothing Then
            holding1SubKind = oMonitorMachine.HoldingKsbPrograms(1).DataSubKind
            holding1Version = oMonitorMachine.HoldingKsbPrograms(1).DataVersion
        End If

        Dim sDataHashValue As String = Nothing
        Dim dataAcceptDate As DateTime
        Dim sRunnableDate As String = Nothing
        Dim sArchiveCatalog As String = Nothing
        Dim oVersionListData As Byte() = Nothing

        '適用リストのみを受信した（マスタ本体を受信しない）場合のために、
        '当該監視盤が最後に受信した（受信した適用リストに紐づく）
        'マスタ本体の情報を取得しておく。
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingKsbPrograms(0) IsNot Nothing)
            sDataHashValue = oMonitorMachine.HoldingKsbPrograms(0).DataHashValue
            dataAcceptDate = oMonitorMachine.HoldingKsbPrograms(0).DataAcceptDate
            sRunnableDate = oMonitorMachine.HoldingKsbPrograms(0).RunnableDate
            sArchiveCatalog = oMonitorMachine.HoldingKsbPrograms(0).ArchiveCatalog
            oVersionListData = oMonitorMachine.HoldingKsbPrograms(0).VersionListData
        End If
        If holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingKsbPrograms(1) IsNot Nothing)
            sDataHashValue = oMonitorMachine.HoldingKsbPrograms(1).DataHashValue
            dataAcceptDate = oMonitorMachine.HoldingKsbPrograms(1).DataAcceptDate
            sRunnableDate = oMonitorMachine.HoldingKsbPrograms(1).RunnableDate
            sArchiveCatalog = oMonitorMachine.HoldingKsbPrograms(1).ArchiveCatalog
            oVersionListData = oMonitorMachine.HoldingKsbPrograms(1).VersionListData
        End If
        For Each oPenPro As PendingKsbProgram In oMonitorMachine.PendingKsbPrograms
            If oPenPro.DataSubKind = dataSubKind AndAlso oPenPro.DataVersion = dataVersion Then
                sDataHashValue = oPenPro.DataHashValue
                dataAcceptDate = oPenPro.DataAcceptDate
                sRunnableDate = oPenPro.RunnableDate
                sArchiveCatalog = oPenPro.ArchiveCatalog
                oVersionListData = oPenPro.VersionListData
            End If
        Next oPenPro

        If oReqTeleg.DataFileName.Length = 0 Then
            '保持もキューイングもしていないバージョンのプログラムに関して、適用リストのみを
            '送り付けられた場合は、ContinueCode.FinishWithoutStoringのREQ電文を作成する。
            'NOTE: そのケースで本物の監視盤がどのような反応を示すかは、分かっていない。
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "適用リストに紐づくプログラム本体がありません。")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            '保持しているバージョンのプログラムに関して、CAB本体を
            '送り付けられた場合は、保持しているものと内容比較を行う。
            '内容が異なる場合も、ログで警告するだけにする。
            'NOTE: そのケースで本物の監視盤がどのような反応を示すかは、分かっていない。
            If sDataHashValue IsNot Nothing Then
                If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.DataFileHashValue, sDataHashValue) <> 0 Then
                    Log.Warn(sMonitorMachineId, "先行して受け入れた同一名プログラムと内容（※ハッシュ値）が違いますが、受け入れを強行します。")
                End If
            End If
        End If

        'TODO: 本物の監視盤において、監視盤プログラム適用リストにも改札機マスタ適用リストと同様の
        '制限事項（適用リストバージョンが保持しているものと同一である場合は捨てる等）があるなら、
        'isNewListを用意するなど、同様の実装にする。
        Dim sListHashValue As String = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        'NOTE: 以下の制御は、fullFlagが1のFinishWithoutStoringを返すケースを作るために
        '用意したものである。本物の監視盤では、待機面への配信を保留する機能はなく、
        '受信したら、速やかに（次に受信する前に）待機面に配信する（コピーする）と
        '思われるため、そもそも、このような制御はあり得ないはず。
        If oMonitorMachine.PendingKsbPrograms.Count <> 0 Then
            Log.Error(sMonitorMachineId, "先行して受け入れたものを待機面に移動するまで、新たなプログラムの受け入れはできません。")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
            Return True
        End If

        '適用リストの内容を解析する。
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, String)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            'とりあえず、全て読み取る。
            'OPT: 「適用リストの内容を表示する機能」を追加した際に、手っ取り早く
            '実装するために、このように二度読みすることになっている。
            '非効率であり、改善の余地がある。
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '１行ずつ読み取る。
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '以下、適用リストの内容をチェックし、問題がある場合は、
                'ContinueCode.FinishWithoutStoringのREQ電文を作成する。

                'ヘッダ部の１行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストが空です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の１行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "適用リスト1行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '作成年月日をチェックする。
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された作成年月日が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'リストVerをチェックする。
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたリストVerがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を読み込む。
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "適用リストに2行目がありません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'ヘッダ部の２行目を列に分割する。
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "適用リスト2行目の項目数が不正です。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                'エリアNoをチェックする。
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載されたエリアNoがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '代表Verをチェックする。
                If Not dataVersion.ToString("D8").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された代表Verがファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '機種コードをチェックする。
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "適用リストに記載された機種がファイル名と整合していません。")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '適用リストの３行目以降から、oMonitorMachineに相当する号機を抽出する。
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '読み込んだ行を列に分割する。
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 4 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の項目数が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'サイバネ線区駅順コードの書式をチェックする。
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の駅コードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    'コーナーコードの書式をチェックする。
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目のコーナーコードが不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '号機番号の書式をチェックする。
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の号機番号が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '適用日のレングスをチェックする。
                    If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '適用日がブランクでない場合、値をチェックする。
                    If aColumns(3).Length = 8 Then
                       If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                            Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If

                        Dim applicableDate As DateTime
                        If Not aColumns(3).Equals("99999999") AndAlso _
                           DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, applicableDate) = False Then
                            Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目の適用日が不正です。")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    '行の重複をチェックする。
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "適用リスト" & lineNumber.ToString() & "行目が既出の行と重複しています。")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '行の内容を一時保存する。
                    oListedMachines.Add(sLineKey, aColumns(3))

                    '行がoMonitorMachineに相当する場合
                    If sLineKey = sMonitorMachineId Then
                        'エリア番号をチェックする。
                        'TODO: エリア0の部材であれば所属エリアが0以外の監視盤にも適用できるようにしてあるが、
                        'そもそも監視盤の所属エリアが0以外であること自体が異常なので、やめた方がよいかもしれない。
                        If dataSubKind <> 0 AndAlso _
                           dataSubKind <> DirectCast(oMonitorMachine.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer) Then
                            Log.Error(sMonitorMachineId, "適用リストに記載された機器 [" & sLineKey & "] の所属エリアが、適用リストの対象エリアと異なります。")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "適用リストの読取りで例外が発生しました。", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End Try

        If oReqTeleg.DataFileName.Length <> 0 Then
            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
            dataAcceptDate = d

            Dim content As KsbProgramContent
            Try
                Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
                content = ExtractKsbProgramCab(sDataFilePath, Path.Combine(sContextDir, "KsbPro"))
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "プログラム本体の解析で例外が発生しました。", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 適用エリア", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の監視盤の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載されたエリアNoがファイル名と整合していませんが、処理を強行します。")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("共通部 プログラム全体Ver（新）", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CABの中身を確認できる方が親切なので、このまま処理を強行する。
                'TODO: 本物の監視盤の動作に合わせた方がよい？
                Log.Warn(sMonitorMachineId, "バージョンリストに記載された代表Verがファイル名と整合していませんが、処理を強行します。")
            End If

            sRunnableDate = content.RunnableDate
            sArchiveCatalog = content.ArchiveCatalog
            oVersionListData = content.VersionListData
        End If

        '配信のための情報をキューイングする。
        Dim targetCount As Integer = 0
        Dim targetFullCount As Integer = 0
        For Each oApplyEntry As KeyValuePair(Of String, String) In oListedMachines
            '行がoMonitorMachineに相当する場合
            If oApplyEntry.Key = sMonitorMachineId Then
                '適用日が現在の運用日付と同じか未来あるいは「19000101」か「99999999」の場合のみ、
                '配信が（このアプリの場合は、DL完了通知が）必要とみなす。
                'TODO: 本物の監視盤は、自身の行がこの条件に該当していない場合に、「適用済み」のDL完了通知を
                '送り付けてくるかもしれない（改札機向けプログラムについては、そうであったようにみえる）。
                'よって、最新の監視盤は、そもそも適用日が過去日の行であっても、当該プログラムが待機面に
                'なければ（かつ未適用であれば？）、待機面にコピーしてしまうかもしれない。
                '一方、運管は、適用日が過去日の行は、適用日がブランクの行と同じ扱いにすることになっている。
                'それゆえに、適用リストにそのような行しかなければ、監視盤に対して送信しないので、
                '適用リストを受信した監視盤において、自身の行が上記条件に該当しないということ自体、
                '考えにくいことではある。ただ、万が一を考えると、実際にどうなのか検証した方がよい。
                'なお、運管の動作はI/F仕様（ツール仕様書の別紙6）に完全に合致している。
                If oApplyEntry.Value.Length = 8 AndAlso _
                  (oApplyEntry.Value.Equals("19000101") OrElse _
                   String.CompareOrdinal(oApplyEntry.Value, sServiceDate) >= 0) Then
                    Log.Debug(sMonitorMachineId, "適用リストに記載された機器 [" & oApplyEntry.Key & "] 適用日 [" & oApplyEntry.Value & "] の行をキューイングします。")
                    Dim oPenPro As New PendingKsbProgram()
                    oPenPro.DataSubKind = dataSubKind
                    oPenPro.DataVersion = dataVersion
                    oPenPro.ListVersion = listVersion
                    oPenPro.DataAcceptDate = dataAcceptDate
                    oPenPro.RunnableDate = sRunnableDate
                    oPenPro.ArchiveCatalog = sArchiveCatalog
                    oPenPro.VersionListData = oVersionListData
                    oPenPro.DataHashValue = sDataHashValue
                    oPenPro.ListAcceptDate = listAcceptDate
                    oPenPro.ApplicableDate = oApplyEntry.Value
                    oPenPro.ListContent = sListContent
                    oPenPro.ListHashValue = sListHashValue
                    oMonitorMachine.PendingKsbPrograms.AddLast(oPenPro)
                    targetCount += 1
                Else
                    Log.Debug(sMonitorMachineId, "適用リストに記載された機器 [" & oApplyEntry.Key & "] 適用日 [" & oApplyEntry.Value & "] の行は除外します。")
                End If
                targetFullCount += 1
            End If
        Next oApplyEntry
        Log.Debug(sMonitorMachineId, "適用リストに記載された" & oListedMachines.Count.ToString() & "台のうち、" & targetFullCount.ToString() & "台が当該機器でした。そのうち" & targetCount.ToString() & "台の適用日が有効でした。")

        'NOTE: 下記のケースで、本物の監視盤がどのような反応を示すかは、よくわからない。
        If targetCount = 0 Then
            Log.Error(sMonitorMachineId, "配信を生み出さない適用リストを受信しました。")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End If

        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        Log.Info(sMonitorMachineId, "受け入れが完了しました。")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverKsbPro(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'キューから全てのプログラム適用リストを取り出し、
        '適用リストごとに、プログラム保持状態
        '（oMonitorMachine.HoldingKsbPrograms）を
        '更新し、sMachineDirに適用リスト別の
        '#KsbProDlReflectReq_RRRSSSCCCCUU_N.dat（Nは0～）を作成する。
        'また、プログラム保持状態を更新した場合は、
        'sContextDirにKsbProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、KsbProDlReflectReq_RRRSSSCCCCUU_N.datと
        'KsbProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの）を削除する。

        DeleteFiles(sMonitorMachineId, sContextDir, "KsbProVerInfo_*.dat")

        If oMonitorMachine.PendingKsbPrograms.Count = 0 Then
            Log.Debug(sMonitorMachineId, "当該機器に対するプログラム配信はありません。")
            Return True
        End If

        Dim d As DateTime = DateTime.Now
        Dim isHoldingProUpdated As Boolean = False

        '配信処理に使っていない全適用リストについて処理を行う。
        For Each oPenPro As PendingKsbProgram In oMonitorMachine.PendingKsbPrograms
            'NOTE: 実体のない（ListHashValue Is Nothing の）適用リストで配信が行われる可能性は想定しない。
            Log.Info(sMonitorMachineId, "適用リスト [" & oPenPro.ListVersion.ToString() & "] に基づき、当該機器内でエリアNo [" & oPenPro.DataSubKind.ToString() & "] 代表Ver [" & oPenPro.DataVersion.ToString() & "] のプログラム配信処理を行います...")

            If oPenPro.ApplicableDate.Equals("99999999") Then
                Log.Info(sMonitorMachineId, "※当該機器に対する要求は改造中止要求です。")
            End If

            '最後に配信した（待機面に移動した）適用リストの情報を取得する。
            Dim latestDataSubKind As Integer = oMonitorMachine.HoldingKsbPrograms(0).DataSubKind
            Dim latestDataVersion As Integer = oMonitorMachine.HoldingKsbPrograms(0).DataVersion
            Dim latestListVersion As Integer = oMonitorMachine.HoldingKsbPrograms(0).ListVersion
            Dim latestListHashValue As String = oMonitorMachine.HoldingKsbPrograms(0).ListHashValue
            If oMonitorMachine.HoldingKsbPrograms(1) IsNot Nothing Then
                latestDataSubKind = oMonitorMachine.HoldingKsbPrograms(1).DataSubKind
                latestDataVersion = oMonitorMachine.HoldingKsbPrograms(1).DataVersion
                latestListVersion = oMonitorMachine.HoldingKsbPrograms(1).ListVersion
                latestListHashValue = oMonitorMachine.HoldingKsbPrograms(1).ListHashValue
            End If

            '適用リストの配信結果（「正常」または「異常」「適用済み」）を決める。
            'TODO: 本物の監視盤は適用リストバージョンしか比較しないかもしれない。
            '最新の（適用済み対応の）監視盤では適用リストの内容を比較すると信じたい。
            'NOTE: そもそも本物の監視盤は、適用リストの「適用済み」を判断するのに、
            '最後に受け入れたものではなく、現在適用中のプログラムを適用した際に用いた
            '適用リストと比較するのかもしれない。それだと、最後に受け入れたものが未適用で、
            'それが今回受信したものと同一である場合に、「正常」のDL完了通知の後、
            '「適用済み」ではなく、再度「正常」のDL完了通知を出すことになるわけであり、
            'さすがにそのような仕様にはしないと思われるが...
            Dim listDeliveryResult As Byte = &H0

            If latestListHashValue IsNot Nothing AndAlso _
               oPenPro.DataSubKind = latestDataSubKind AndAlso _
               oPenPro.DataVersion = latestDataVersion AndAlso _
               oPenPro.ListVersion = latestListVersion AndAlso _
               StringComparer.OrdinalIgnoreCase.Compare(oPenPro.ListHashValue, latestListHashValue) = 0 Then
                '待機面に保持している適用リストと同じものを待機面に配信することになる場合は、
                '配信結果を「適用済み」とする。
                'NOTE: そのケースでは、改造中止要求の適用リストに対しても「適用済み（改造中止済み？）」
                'で済ましてしまうが、本物の監視盤もそうであるかは不明。そもそも、前回の配信時に
                '改造中止をしているとしたら、待機面から消えているはずなので、普通にはあり得ない
                'ケースと思われる。
                If oPenPro.ApplicableDate.Equals("99999999") Then
                    Log.Warn(sMonitorMachineId, "当該機器に対しては当該適用リストを配信済みです。適用リストの再配信を行いませんので、改造中止も行いません。")
                Else
                    Log.Warn(sMonitorMachineId, "当該機器に対しては当該適用リストを配信済みです。適用リストの再配信は行いません。適用リストに基づくプログラム本体の配信も行いません。")
                End If
                listDeliveryResult = &HF
            End If

            If listDeliveryResult = &H0 Then
                If oPenPro.ApplicableDate.Equals("99999999") Then
                    'NOTE: oPenProのバージョンのプログラムが既に適用中になっているケースも
                    '以下のケース（無効な改造中止要求）に当てはまるはずである。
                    'NOTE: 「oMonitorMachine.HoldingKsbPrograms(1) Is Nothing」でない場合においては、
                    '万が一「oMonitorMachine.HoldingKsbPrograms(1).DataVersion = 0」であるとしても、
                    'それは、バージョン0のプログラムを保持しているということである。
                    'よって、oPenPro.DataVersionも0であり、エリア番号も一致するなら、
                    'oPenProは有効な改造中止であり、以下の条件が偽になってよい。
                    If oMonitorMachine.HoldingKsbPrograms(1) Is Nothing OrElse _
                       oMonitorMachine.HoldingKsbPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                       oMonitorMachine.HoldingKsbPrograms(1).DataVersion <> oPenPro.DataVersion Then
                        'NOTE: 本物の監視盤がこのように厳密な動作をするのかは、不明である。
                        Log.Error(sMonitorMachineId, "無効な改造中止要求です。当該機器において当該プログラムが適用待ちになっていません。")
                        listDeliveryResult = &H1
                    End If
                End If
            End If

            'NOTE: 適用リストが適用済みの場合や、改造中止要求の適用リストが無効な場合は、
            'プログラム本体のDL完了通知は発生させない。それらのケースでは、
            'プログラム本体は配信対象でないはずであり、問題ないはず。
            '改造中止の行を含む適用リストがプログラム本体とともにDLLされるケースは想定しない。
            'TODO: 本物の監視盤がどう動くかは分からない。
            If listDeliveryResult = &H0 Then
                If oPenPro.ApplicableDate.Equals("99999999") Then
                    'NOTE: 適用日が「99999999」の場合は、プログラム本体のDL完了通知は（適用済みなども含めて）
                    '生成しないことにする。本物の監視盤がどうなのかは不明。
                    'TODO: 運管において、ある監視盤に対するあるバージョンのプログラムの初回の配信指示で、
                    '適用リストに「99999999」を記載してしまったり、「99999999」が記載された適用リストで
                    '配信を行う際に「プログラム+プログラム適用リスト 強制配信」にチェックを入れて
                    'しまったりすると、プログラム本体に関する当該監視盤の受信状態が「配信中」になり、
                    'それがそのまま残ってしまうと思われる。これについては、適用日「99999999」が指定
                    'された監視盤について「配信中」のレコードを作成しないように、そして、できること
                    'なら「99999999」が記載された適用リストで「プログラム+プログラム適用リスト 強制配信」
                    'を指定できないように、運管の実装を改善するべきである。

                    '監視盤プログラム保持状態を更新する。
                    oMonitorMachine.HoldingKsbPrograms(1) = Nothing
                    isHoldingProUpdated = True
                    Log.Info(sMonitorMachineId, "当該機器に対して改造中止を行いました。")
                Else
                    'NOTE: たとえ適用リストに関するDL完了通知が「適用済み」であったとしても、
                    'プログラム本体のDL完了通知も生成する（適用リストに関する「適用済み」など
                    'という概念が持ち込まれたことで、違和感があるかもしれないが、運管からの
                    'DLL要求には、適用リストのバージョンなどに関係なく、個別に意味がある）。
                    'また、たとえ適用リストが適用済み（= 実際は、単なる送信済み）であったとしても、
                    'その適用リストに有意な適用日が記載されており、当該プログラムが未適用である
                    'なら、プログラム本体については「適用済み」ではなく「正常」のDL完了通知を
                    '生成する。
                    'TODO: 本物の監視盤は、適用リストに関するDL完了通知が「適用済み」である場合に、
                    'プログラム本体のDL完了通知（おそらく「適用済み」）を生成しないかもしれない。
                    'その状況では、運管における当該監視盤のプログラムの受信状態も「配信中」
                    'ではなく「正常」等になっていると思われるが、本当にその保証があるのか
                    '検証した方がよい。
                    'TODO: このアプリでは、良心に基づいて比較しているが、本物の監視盤が
                    'CABの内容を比較して、不一致の場合に待機面への再コピーを行うかは不明である。

                    'プログラム本体の配信結果（「正常」または「適用済み」）を決める。
                    Dim dataDeliveryResult As Byte = &H0

                    'TODO: DeliverGateProと同じように、HoldingKsbPrograms(0)をチェックする必要はないか？
                    If oMonitorMachine.HoldingKsbPrograms(1) IsNot Nothing AndAlso _
                       oMonitorMachine.HoldingKsbPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                       oMonitorMachine.HoldingKsbPrograms(1).DataVersion = oPenPro.DataVersion AndAlso _
                       StringComparer.OrdinalIgnoreCase.Compare(oMonitorMachine.HoldingKsbPrograms(1).DataHashValue, oPenPro.DataHashValue) = 0 Then
                        '適用中のものと同じものを配信することになる場合は、
                        '配信結果を「適用済み」とする。
                        Log.Warn(sMonitorMachineId, "当該機器に対しては当該プログラムを適用済み（※配信済み）です。プログラム本体の再配信は行いません。")
                        dataDeliveryResult = &HF
                    End If

                    If dataDeliveryResult = &H0 Then
                        If Not oPenPro.ApplicableDate.Equals("19000101") AndAlso _
                           String.CompareOrdinal(oPenPro.ApplicableDate, oPenPro.RunnableDate) < 0 Then
                            Log.Error(sMonitorMachineId, "プログラムの動作許可日が適用日以降に設定されています。配信は行いません。")
                            dataDeliveryResult = &HC
                            listDeliveryResult = &H1 'TODO: 適用リストの配信結果は３種類しかない。本物は適用リストを配信するのかもしれない。
                        End If
                    End If

                    '監視盤プログラム保持状態を更新する。
                    If dataDeliveryResult = &H0 Then
                        Debug.Assert(listDeliveryResult = &H0)
                        Dim oPro As New HoldingKsbProgram()
                        oPro.DataSubKind = oPenPro.DataSubKind
                        oPro.DataVersion = oPenPro.DataVersion
                        oPro.ListVersion = oPenPro.ListVersion
                        oPro.DataAcceptDate = oPenPro.DataAcceptDate
                        oPro.DataDeliverDate = d
                        oPro.RunnableDate = oPenPro.RunnableDate
                        oPro.ArchiveCatalog = oPenPro.ArchiveCatalog
                        oPro.VersionListData = oPenPro.VersionListData
                        oPro.DataHashValue = oPenPro.DataHashValue
                        oPro.ListAcceptDate = oPenPro.ListAcceptDate
                        oPro.ListDeliverDate = d
                        oPro.ApplicableDate = oPenPro.ApplicableDate
                        oPro.ListContent = oPenPro.ListContent
                        oPro.ListHashValue = oPenPro.ListHashValue
                        oMonitorMachine.HoldingKsbPrograms(1) = oPro
                        isHoldingProUpdated = True
                        Log.Info(sMonitorMachineId, "当該機器の待機面に対して当該プログラム本体の配信を行いました。")
                        Log.Info(sMonitorMachineId, "当該機器の待機面に対して当該適用リストの配信を行いました。")
                    ElseIf listDeliveryResult = &H0 Then
                        If oMonitorMachine.HoldingKsbPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                           oMonitorMachine.HoldingKsbPrograms(0).DataVersion = oPenPro.DataVersion Then
                            oMonitorMachine.HoldingKsbPrograms(0).ListVersion = oPenPro.ListVersion
                            oMonitorMachine.HoldingKsbPrograms(0).ListAcceptDate = oPenPro.ListAcceptDate
                            oMonitorMachine.HoldingKsbPrograms(0).ListDeliverDate = d
                            oMonitorMachine.HoldingKsbPrograms(0).ApplicableDate = oPenPro.ApplicableDate
                            oMonitorMachine.HoldingKsbPrograms(0).ListContent = oPenPro.ListContent
                            oMonitorMachine.HoldingKsbPrograms(0).ListHashValue = oPenPro.ListHashValue
                            'TODO: oMonitorMachine.HoldingKsbPrograms(0).ListVersionを更新しても、
                            '基本的に監視盤プログラムバージョン情報に変化は無いはずなので、
                            '以下は行わない方がよいかもしれない。
                            'TODO: このアプリでは、待機面にある適用リストにこそ意味がある
                            'ものとして、監視盤プログラムバージョン情報にセットしているが、
                            'そもそも本物の監視盤がどうであるかはわからない。
                            isHoldingProUpdated = True
                            Log.Warn(sMonitorMachineId, "当該機器の適用面に対して当該適用リストの配信を行いました。この適用日は意味を持ちませんので注意してください。")
                        ElseIf oMonitorMachine.HoldingKsbPrograms(1) IsNot Nothing AndAlso _
                               oMonitorMachine.HoldingKsbPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                               oMonitorMachine.HoldingKsbPrograms(1).DataVersion = oPenPro.DataVersion Then
                            oMonitorMachine.HoldingKsbPrograms(1).ListVersion = oPenPro.ListVersion
                            oMonitorMachine.HoldingKsbPrograms(1).ListAcceptDate = oPenPro.ListAcceptDate
                            oMonitorMachine.HoldingKsbPrograms(1).ListDeliverDate = d
                            oMonitorMachine.HoldingKsbPrograms(1).ApplicableDate = oPenPro.ApplicableDate
                            oMonitorMachine.HoldingKsbPrograms(1).ListContent = oPenPro.ListContent
                            oMonitorMachine.HoldingKsbPrograms(1).ListHashValue = oPenPro.ListHashValue
                            isHoldingProUpdated = True
                            Log.Info(sMonitorMachineId, "当該機器の待機面に対して当該適用リストの配信を行いました。")
                        Else
                            'TODO: よく検証しないとあり得るケースかどうか分からない。
                            Log.Error(sMonitorMachineId, "当該機器において、当該適用リストに紐づくプログラム本体がありません。適用リストの配信は行いません。")
                            listDeliveryResult = &H1
                        End If
                    End If

                    'プログラム本体に関する#KsbProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
                    CreateFileOfKsbProDlReflectReq( _
                       &H22, _
                       oPenPro.DataVersion, _
                       dataDeliveryResult, _
                       sMonitorMachineId, _
                       sMachineDir)
                End If
            End If

            'NOTE: この配信の前に直接投入を実施した場合など、監視盤に適用リストが
            '存在しない場合は、下記を行わない。
            '実物の改札機システムの挙動を（良し悪しに関係なく）忠実に再現する。
            If latestListHashValue IsNot Nothing Then
                '適用リストに関する#KsbProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
                CreateFileOfKsbProDlReflectReq( _
                   &H49, _
                   oPenPro.ListVersion, _
                   listDeliveryResult, _
                   sMonitorMachineId, _
                   sMachineDir)
            Else
                Log.Warn(sMonitorMachineId, "当該機器が適用リストを保持していなかったため、適用リストのDL完了通知は作成しませんでした。これは自動改札機システムの制限事項です。")
            End If
        Next oPenPro
        oMonitorMachine.PendingKsbPrograms.Clear()

        If isHoldingProUpdated Then
            CreateFileOfKsbProVerInfo(sMonitorMachineId, oMonitorMachine, sContextDir)
        End If

        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)

        Return True
    End Function

    Protected Function ApplyKsbPro(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '待機面にプログラムを保持しているかチェックし、
        'その適用日が運用日以前であれば、適用面に移動する。
        'また、プログラム保持状態を更新した場合は、
        'sContextDirにKsbProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、KsbProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のものを削除する。

        DeleteFiles(sMonitorMachineId, sContextDir, "KsbProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        If oMonitorMachine.HoldingKsbPrograms(1) Is Nothing Then
            Log.Debug(sMonitorMachineId, "当該機器には、適用待ちのプログラムがありません。")
        ElseIf oMonitorMachine.HoldingKsbPrograms(1).ListHashValue IsNot Nothing AndAlso _
               String.CompareOrdinal(oMonitorMachine.HoldingKsbPrograms(1).ApplicableDate, sServiceDate) > 0 Then
            Log.Warn(sMonitorMachineId, "当該機器には、適用待ちのプログラムがありますが、適用日前であるため、適用しません。")
        ElseIf String.CompareOrdinal(oMonitorMachine.HoldingKsbPrograms(1).RunnableDate, sServiceDate) > 0 Then
            Log.Warn(sMonitorMachineId, "当該機器には、適用待ちのプログラムがありますが、動作許可日前であるため、適用しません。")
        Else
            oMonitorMachine.HoldingKsbPrograms(0) = oMonitorMachine.HoldingKsbPrograms(1)
            oMonitorMachine.HoldingKsbPrograms(0).ApplyDate = d
            oMonitorMachine.HoldingKsbPrograms(1) = Nothing
            Log.Info(sMonitorMachineId, "当該機器において、適用待ちのプログラムを適用しました。")
            CreateFileOfKsbProVerInfo(sMonitorMachineId, oMonitorMachine, sContextDir)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        End If

        Return True
    End Function

    Public Function ClearUpboundData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "機器の保持する上りデータをクリアします...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Try
            Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)
            InitMonitorUpboundData(sMonitorMachineId, oMonitorMachine)
            For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
                InitTermUpboundData(sMonitorMachineId, oTermEntry.Key, oTermEntry.Value)
                UpdateTable2OnTermStateChanged(sMonitorMachineDir, oTermEntry.Key, oTermEntry.Value)
            Next oTermEntry
            Log.Info(sMonitorMachineId, "クリアが完了しました。")
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True
    End Function

    Public Function StoreRandFaultData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "ランダム異常データを生成し再収集用に蓄積します...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachineまたはoMonitorMachine.TermMachinesに設定されている情報と乱数をもとに異常データを生成し、
        'sMonitorMachineDirの#FaultDataForPassiveUll.datに追記する。

        Dim termCount As Integer = oMonitorMachine.TermMachines.Count

        'NOTE: 収集周期（12時間）あたり最大300人（平均150人）の利用者が１つの改札機で問題を起こす想定である。
        'TODO: ラッシュ時の東京駅などはもっと多いかもしれないし、
        '全駅平均で考えればもっと少ないと思われるため、試験内容に応じて
        '調整可能にした方がよい。データグリッドに「人口密度」的な項目（値を
        '編集可能）を用意するなど。
        Dim recCount As Integer = Rand.Next(0, termCount * 300 + 300)

        Dim oTermEntries(termCount - 1) As KeyValuePair(Of String, TermMachine)
        CType(oMonitorMachine.TermMachines, ICollection(Of KeyValuePair(Of String, TermMachine))).CopyTo(oTermEntries, 0)

        'Dim oMoniEntries(UiState.Machines.Count - 1) As KeyValuePair(Of String, Machine)
        'CType(UiState.Machines, ICollection(Of KeyValuePair(Of String, Machine))).CopyTo(oMoniEntries, 0)

        Dim now As DateTime = DateTime.Now
        Dim prevTime As DateTime = oMonitorMachine.FaultDate
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oTermEntries
            If oTermEntry.Value.FaultDate > prevTime Then
                prevTime = oTermEntry.Value.FaultDate
            End If
        Next oTermEntry
        Dim span As Double = (now - prevTime).TotalSeconds

        Dim sFileName As String = "#FaultDataForPassiveUll.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                Dim fileLen As Long = oOutputStream.Length
                Dim recLen As Integer = FaultDataUtil.RecordLengthInBytes
                If fileLen < recLen Then
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                    ExUpboundFileHeader.WriteToStream(&HB6, recCount, recLen, now, oOutputStream)
                Else
                    Dim totalRecCount As Integer = CInt((fileLen \ recLen) - 1) + recCount
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                    ExUpboundFileHeader.WriteToStream(&HB6, totalRecCount, recLen, now, oOutputStream)
                    oOutputStream.Seek(0, SeekOrigin.End)
                End If

                For i As Integer = 1 To recCount
                    Dim oBytes(recLen - 1) As Byte

                    Dim t As DateTime = prevTime.AddSeconds(span * i / recCount)
                    Dim termIndex As Integer = Rand.Next(-1, termCount)
                    If termIndex = -1 Then
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー データ種別", "A6", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 駅コード", GetHypStationOf(sMonitorMachineId), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 処理日時", t.ToString("yyyyMMddHHmmss"), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー コーナー", GetCornerOf(sMonitorMachineId), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 号機", GetUnitOf(sMonitorMachineId), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー シーケンスNo", "0", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー バージョン", "01", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("データレングス", "780", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("発生日時", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("号機番号", "00", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("通路方向", FaultDataUtil.CreatePassDirectionValue(oMonitorMachine.LatchConf), oBytes)

                        'Dim errorcdIndex As Integer = Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)
                        'FaultDataUtil.SetFieldValueToBytes("エラーコード", Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
                        'FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("異常項目 表示データ"), Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

                        Dim sErrorCode As String = Config.KsbFaultDataErrorCodeItems.Rows(Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
                        FaultDataUtil.SetFieldValueToBytes("エラーコード", sErrorCode, oBytes)

                        Dim sErrorText As String = Nothing
                        If Config.KsbFaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", sErrorText, oBytes)
                        End If
                        If Config.KsbFaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("４文字表示 表示データ", sErrorText, oBytes)
                        End If
                        If Config.KsbFaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("可変表示部 表示データ", sErrorText, oBytes)
                        End If
                        If Config.KsbFaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("処置内容 表示データ", sErrorText, oBytes)
                        End If
                    Else
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー データ種別", "A6", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 駅コード", GetHypStationOf(oTermEntries(termIndex).Key), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 処理日時", t.ToString("yyyyMMddHHmmss"), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー コーナー", GetCornerOf(oTermEntries(termIndex).Key), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 号機", "0", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー シーケンスNo", "0", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("基本ヘッダー バージョン", "01", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("データレングス", "780", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("発生日時", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("号機番号", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("通路方向", FaultDataUtil.CreatePassDirectionValue(oTermEntries(termIndex).Value.LatchConf), oBytes)

                        'Dim errorcdIndex As Integer = Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)
                        'FaultDataUtil.SetFieldValueToBytes("エラーコード", Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
                        'FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("異常項目 表示データ"), Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

                        Dim sErrorCode As String = Config.FaultDataErrorCodeItems.Rows(Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
                        FaultDataUtil.SetFieldValueToBytes("エラーコード", sErrorCode, oBytes)

                        Dim sErrorText As String = Nothing
                        If Config.FaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", sErrorText, oBytes)
                        End If
                        If Config.FaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("４文字表示 表示データ", sErrorText, oBytes)
                        End If
                        If Config.FaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("可変表示部 表示データ", sErrorText, oBytes)
                        End If
                        If Config.FaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("処置内容 表示データ", sErrorText, oBytes)
                        End If
                    End If

                    FaultDataUtil.AdjustByteCountField("異常項目", oBytes)
                    FaultDataUtil.AdjustByteCountField("４文字表示", oBytes)
                    FaultDataUtil.AdjustByteCountField("可変表示部", oBytes)
                    FaultDataUtil.AdjustByteCountField("処置内容", oBytes)

                    oOutputStream.Write(oBytes, 0, oBytes.Length)

                    If termIndex = -1 Then
                        oMonitorMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
                        oMonitorMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                    Else
                        oTermEntries(termIndex).Value.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
                        oTermEntries(termIndex).Value.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                    End If
                Next i
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "ファイル [" & sFilePath & "] へのレコード追加が失敗しました。", ex)
            Return False
        End Try
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] に [" & recCount.ToString() & "] レコードを追加しました。")

        'NOTE: 監視機器や個々の端末の行について、何度も更新することになる可能性が高いため、
        'ここで監視機器と全端末の行を一度だけ更新することにしている。
        UpdateTable2OnMonitorStateChanged(sMonitorMachineDir, oMonitorMachine)
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            UpdateTable2OnTermStateChanged(sMonitorMachineDir, oTermEntry.Key, oTermEntry.Value)
        Next oTermEntry

        Return True
    End Function

    Public Function SendRandFaultData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "ランダム異常データを生成し即時送信します...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachinesに設定されている情報と乱数をもとに異常データを生成し、
        'シミュレータ本体に送信させる。

        Dim termCount As Integer = oMonitorMachine.TermMachines.Count

        Dim oTermEntries(termCount - 1) As KeyValuePair(Of String, TermMachine)
        CType(oMonitorMachine.TermMachines, ICollection(Of KeyValuePair(Of String, TermMachine))).CopyTo(oTermEntries, 0)

        Dim oBytes(FaultDataUtil.RecordLengthInBytes - 1) As Byte

        Dim t As DateTime = DateTime.Now
        Dim termIndex As Integer = Rand.Next(-1, termCount)
        If termIndex = -1 Then
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー データ種別", "A6", oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 駅コード", GetHypStationOf(sMonitorMachineId), oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 処理日時", t.ToString("yyyyMMddHHmmss"), oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー コーナー", GetCornerOf(sMonitorMachineId), oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 号機", GetUnitOf(sMonitorMachineId), oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー シーケンスNo", "0", oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー バージョン", "01", oBytes)
            FaultDataUtil.SetFieldValueToBytes("データレングス", "780", oBytes)
            FaultDataUtil.SetFieldValueToBytes("発生日時", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
            FaultDataUtil.SetFieldValueToBytes("号機番号", "00", oBytes)
            FaultDataUtil.SetFieldValueToBytes("通路方向", FaultDataUtil.CreatePassDirectionValue(oMonitorMachine.LatchConf), oBytes)

            'Dim errorcdIndex As Integer = Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)
            'FaultDataUtil.SetFieldValueToBytes("エラーコード", Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
            'FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("異常項目 表示データ"), Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

            Dim sErrorCode As String = Config.KsbFaultDataErrorCodeItems.Rows(Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
            FaultDataUtil.SetFieldValueToBytes("エラーコード", sErrorCode, oBytes)

            Dim sErrorText As String = Nothing
            If Config.KsbFaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", sErrorText, oBytes)
            End If
            If Config.KsbFaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("４文字表示 表示データ", sErrorText, oBytes)
            End If
            If Config.KsbFaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("可変表示部 表示データ", sErrorText, oBytes)
            End If
            If Config.KsbFaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("処置内容 表示データ", sErrorText, oBytes)
            End If
        Else
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー データ種別", "A6", oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 駅コード", GetHypStationOf(oTermEntries(termIndex).Key), oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 処理日時", t.ToString("yyyyMMddHHmmss"), oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー コーナー", GetCornerOf(oTermEntries(termIndex).Key), oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー 号機", "0", oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー シーケンスNo", "0", oBytes)
            FaultDataUtil.SetFieldValueToBytes("基本ヘッダー バージョン", "01", oBytes)
            FaultDataUtil.SetFieldValueToBytes("データレングス", "780", oBytes)
            FaultDataUtil.SetFieldValueToBytes("発生日時", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
            FaultDataUtil.SetFieldValueToBytes("号機番号", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
            FaultDataUtil.SetFieldValueToBytes("通路方向", FaultDataUtil.CreatePassDirectionValue(oTermEntries(termIndex).Value.LatchConf), oBytes)

            'Dim errorcdIndex As Integer = Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)
            'FaultDataUtil.SetFieldValueToBytes("エラーコード", Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
            'FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("異常項目 表示データ"), Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

            Dim sErrorCode As String = Config.FaultDataErrorCodeItems.Rows(Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
            FaultDataUtil.SetFieldValueToBytes("エラーコード", sErrorCode, oBytes)

            Dim sErrorText As String = Nothing
            If Config.FaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("異常項目 表示データ", sErrorText, oBytes)
            End If
            If Config.FaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("４文字表示 表示データ", sErrorText, oBytes)
            End If
            If Config.FaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("可変表示部 表示データ", sErrorText, oBytes)
            End If
            If Config.FaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("処置内容 表示データ", sErrorText, oBytes)
            End If
        End If

        FaultDataUtil.AdjustByteCountField("異常項目", oBytes)
        FaultDataUtil.AdjustByteCountField("４文字表示", oBytes)
        FaultDataUtil.AdjustByteCountField("可変表示部", oBytes)
        FaultDataUtil.AdjustByteCountField("処置内容", oBytes)

        If termIndex = -1 Then
            oMonitorMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
            oMonitorMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineDir, oMonitorMachine)
        Else
            oTermEntries(termIndex).Value.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
            oTermEntries(termIndex).Value.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnTermStateChanged(sMonitorMachineDir, oTermEntries(termIndex).Key, oTermEntries(termIndex).Value)
        End If

        Dim sFilePath As String = CreateFileOfFaultDataPostReq(oBytes, sMonitorMachineDir, sMonitorMachineId)
        If sFilePath Is Nothing Then Return False

        Dim oFaultDataParams As Object() = { _
            sFilePath, _
            60000, _
            60000, _
            0, _
            3, _
            True}
        Return SendSimFuncMessage("ActiveOne", oFaultDataParams, sSimWorkingDir, sMonitorMachineId)
    End Function

    Public Function StoreFaultData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()) As Boolean
        Log.Info(sMonitorMachineId, "機器 [" & sSourceMachineId & "] の異常データを再収集用に蓄積します...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#FaultDataForPassiveUll.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                Dim fileLen As Long = oOutputStream.Length
                Dim recLen As Integer = FaultDataUtil.RecordLengthInBytes
                If fileLen < recLen Then
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                    ExUpboundFileHeader.WriteToStream(&HB6, 1, recLen, now, oOutputStream)
                Else
                    Dim totalRecCount As Integer = CInt((fileLen \ recLen) - 1) + 1
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                    ExUpboundFileHeader.WriteToStream(&HB6, totalRecCount, recLen, now, oOutputStream)
                    oOutputStream.Seek(0, SeekOrigin.End)
                End If

                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "ファイル [" & sFilePath & "] へのレコード追加が失敗しました。", ex)
            Return False
        End Try
        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] にレコードを追加しました。")

        If sSourceMachineId = sMonitorMachineId Then
            Dim oMachine As Machine = UiState.Machines(sMonitorMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMachine)
        Else
            Dim oMachine As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnTermStateChanged(sMonitorMachineId, sSourceMachineId, oMachine)
        End If
        Return True
    End Function

    Public Function SendFaultData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()) As Boolean
        Log.Info(sMonitorMachineId, "機器 [" & sSourceMachineId & "] の異常データを即時送信します...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        If sSourceMachineId = sMonitorMachineId Then
            Dim oMachine As Machine = UiState.Machines(sMonitorMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMachine)
        Else
            Dim oMachine As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー シーケンスNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnTermStateChanged(sMonitorMachineId, sSourceMachineId, oMachine)
        End If

        Dim sFilePath As String = CreateFileOfFaultDataPostReq(oBytes, sMonitorMachineDir, sMonitorMachineId)
        If sFilePath Is Nothing Then Return False

        Dim oFaultDataParams As Object() = { _
            sFilePath, _
            60000, _
            60000, _
            0, _
            3, _
            True}
        Return SendSimFuncMessage("ActiveOne", oFaultDataParams, sSimWorkingDir, sMonitorMachineId)
    End Function

    Public Function UpdateKadoDataRandomly(ByVal sMonitorMachineId As String) As Boolean
        Dim isHokurikuMode As Boolean = GetStationOf(sMonitorMachineId).StartsWith("073")
        Log.Info(sMonitorMachineId, "稼動保守データをランダムに更新します...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        'sMonitorMachineDirの#KadoData.datの全レコードを更新する。
        Dim now As DateTime = DateTime.Now
        Dim yesterday As DateTime = now.AddDays(-1).Date
        Dim sYesterday As String = yesterday.ToString("yyyyMMddHHmmss")
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes(0)
        Dim oBytes As Byte()() = {New Byte(recLen - 1) {}, New Byte(recLen - 1) {}}
        Try
            Using oStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
                Dim fileLen As Long = oStream.Length

                If fileLen < recLen * 3 OrElse (fileLen - recLen) Mod (recLen * 2) <> 0 Then
                    Log.Fatal(sMonitorMachineId, "ファイル [" & sFilePath & "] のサイズが異常です。")
                    Return False
                End If

                Dim recCount As Integer = CInt((fileLen \ recLen) - 1)
                oStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                ExUpboundFileHeader.WriteToStream(&HA7, recCount, recLen, now, oStream)

                For Each oTerm As TermMachine In UiState.Machines(sMonitorMachineId).TermMachines.Values
                    For k As Integer = 0 To 1
                        oStream.Seek(recLen * oTerm.KadoSlot(k), SeekOrigin.Begin)
                        Dim pos As Integer = 0
                        Dim len As Integer = recLen
                        While pos < len
                            Dim readSize As Integer = oStream.Read(oBytes(k), pos, len - pos)
                            If readSize = 0 Then Exit While  'OPT: 念のためにチェックしているが、ファイルが排他されている限り、あり得ないはずであり、不要。
                            pos += readSize
                        End While
                    Next k

                    For k As Integer = 0 To 1
                        If isHokurikuMode Then
                            KadoDataUtil073.SetFieldValueToBytes(k, "基本ヘッダー 処理日時", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                            KadoDataUtil073.SetFieldValueToBytes(k, "基本ヘッダー シーケンスNo", MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber(k)).ToString(), oBytes(k))
                            KadoDataUtil073.SetFieldValueToBytes(k, "共通部 集計終了(収集)日時", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                        Else
                            KadoDataUtil.SetFieldValueToBytes(k, "基本ヘッダー 処理日時", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                            KadoDataUtil.SetFieldValueToBytes(k, "基本ヘッダー シーケンスNo", MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber(k)).ToString(), oBytes(k))
                            KadoDataUtil.SetFieldValueToBytes(k, "共通部 集計終了(収集)日時", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                        End If
                    Next k

                    If Rand.Next(0, 3) = 0 Then
                        If isHokurikuMode Then
                            Dim sOldDate As String = KadoDataUtil073.GetFieldValueFromBytes(0, "共通部 改札側搬送部点検日時", oBytes(0))
                            If sOldDate = "00000000000000" OrElse sOldDate < sYesterday Then
                                Dim sNewDate As String = yesterday.AddSeconds(Rand.Next(0, 24 * 60 * 60)).ToString("yyyyMMddHHmmss")
                                For k As Integer = 0 To 1
                                    KadoDataUtil073.SetFieldValueToBytes(k, "共通部 改札側搬送部点検日時", sNewDate, oBytes(k))
                                    KadoDataUtil073.SetFieldValueToBytes(k, "共通部 集札側搬送部点検日時", sNewDate, oBytes(k))
                                Next k
                            End If
                        Else
                            Dim sOldDate As String = KadoDataUtil.GetFieldValueFromBytes(0, "共通部 改札側搬送部点検日時", oBytes(0))
                            If sOldDate = "00000000000000" OrElse sOldDate < sYesterday Then
                                Dim sNewDate As String = yesterday.AddSeconds(Rand.Next(0, 24 * 60 * 60)).ToString("yyyyMMddHHmmss")
                                For k As Integer = 0 To 1
                                    KadoDataUtil.SetFieldValueToBytes(k, "共通部 改札側搬送部点検日時", sNewDate, oBytes(k))
                                    KadoDataUtil.SetFieldValueToBytes(k, "共通部 集札側搬送部点検日時", sNewDate, oBytes(k))
                                Next k
                            End If
                        End If
                    End If

                    For k As Integer = 0 To 1
                        If isHokurikuMode Then
                            For Each oField As XlsField In KadoDataUtil073.Fields(k)
                                If oField.MetaName.StartsWith("集計") AndAlso oField.MetaName.Substring(6) <> "（空き）" Then
                                    Dim oldValue As Long = Long.Parse(KadoDataUtil073.GetFieldValueFromBytes(k, oField.MetaName, oBytes(k)))
                                    Dim newValue As Long = oldValue + Rand.Next(0, 100)
                                    If newValue > UInteger.MaxValue Then
                                        newValue = UInteger.MaxValue
                                    End If
                                    KadoDataUtil073.SetFieldValueToBytes(k, oField.MetaName, newValue.ToString(), oBytes(k))
                                End If
                            Next oField
                        Else
                            For Each oField As XlsField In KadoDataUtil.Fields(k)
                                If oField.MetaName.StartsWith("集計") AndAlso oField.MetaName.Substring(6) <> "（空き）" Then
                                    Dim oldValue As Long = Long.Parse(KadoDataUtil.GetFieldValueFromBytes(k, oField.MetaName, oBytes(k)))
                                    Dim newValue As Long = oldValue + Rand.Next(0, 100)
                                    If newValue > UInteger.MaxValue Then
                                        newValue = UInteger.MaxValue
                                    End If
                                    KadoDataUtil.SetFieldValueToBytes(k, oField.MetaName, newValue.ToString(), oBytes(k))
                                End If
                            Next oField
                        End If
                    Next k

                    If isHokurikuMode Then
                        KadoDataUtil073.UpdateSummaryFields(oBytes)
                    Else
                        KadoDataUtil.UpdateSummaryFields(oBytes)
                    End If

                    For k As Integer = 0 To 1
                        oStream.Seek(recLen * oTerm.KadoSlot(k), SeekOrigin.Begin)
                        oStream.Write(oBytes(k), 0, oBytes(k).Length)
                    Next k

                    'NOTE: UiStateとグリッドの更新は稼動保守データ収集完了時に行う。
                Next oTerm
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "ファイル [" & sFilePath & "] の更新が失敗しました。", ex)
            Return False
        End Try

        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を更新しました。")
        Return True
    End Function

    Public Function UpdateKadoData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()()) As Boolean
        Log.Info(sMonitorMachineId, "機器 [" & sSourceMachineId & "] の稼動保守データを更新します...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oTerm As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)

        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes(0)

        Try
            Using oOutputStream As New FileStream(sFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                Dim fileLen As Long = oOutputStream.Length
                Dim slotCount As Integer = If(fileLen < recLen, 1, CInt(fileLen \ recLen))

                For k As Integer = 0 To 1
                    'OPT: 下記のケースはあり得ないはずであり、救う必要もない。
                    If oTerm.KadoSlot(k) = 0 Then
                        oTerm.KadoSlot(k) = slotCount
                        slotCount += 1
                    End If

                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: 不要かもしれない。
                    ExUpboundFileHeader.WriteToStream(&HA7, slotCount - 1, recLen, now, oOutputStream)

                    oOutputStream.Seek(recLen * oTerm.KadoSlot(k), SeekOrigin.Begin)
                    oOutputStream.Write(oBytes(k), 0, oBytes(k).Length)
                Next k
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "ファイル [" & sFilePath & "] の更新が失敗しました。", ex)
            Return False
        End Try

        'NOTE: UiStateとグリッドの更新は稼動保守データ収集完了時に行う。

        Log.Info(sMonitorMachineId, "ファイル [" & sFilePath & "] を更新しました。")
        Return True
    End Function

    Public Function CommitKadoData(ByVal sMonitorMachineId As String) As Boolean
        Dim isHokurikuMode As Boolean = GetStationOf(sMonitorMachineId).StartsWith("073")
        Log.Info(sMonitorMachineId, "稼動保守データの収集完了を反映します...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "監視機器のディレクトリ [" & sMonitorMachineDir & "] がみつかりませんでした。")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        Dim now As DateTime = DateTime.Now
        Dim sFileName As String = "#KadoData.dat"
        Dim sFilePath As String = Path.Combine(sMonitorMachineDir, sFileName)
        Dim recLen As Integer = KadoDataUtil.RecordLengthInBytes(0)
        Dim oBytes As Byte() = New Byte(recLen - 1) {}
        Try
            Using oInputStream As New FileStream(sFilePath, FileMode.Open, FileAccess.Read)
                Dim fileLen As Long = oInputStream.Length

                If fileLen < recLen * 3 OrElse (fileLen - recLen) Mod (recLen * 2) <> 0 Then
                    Log.Fatal(sMonitorMachineId, "ファイル [" & sFilePath & "] のサイズが異常です。")
                    Return False
                End If

                Dim recCount As Integer = CInt((fileLen \ recLen) - 1)
                oInputStream.Seek(recLen, SeekOrigin.Begin)

                Dim oTerms(recCount - 1) As TermMachine
                Dim oTermKeys(recCount - 1) As String
                Dim kinds(recCount - 1) As Integer
                For Each oTermEntry As KeyValuePair(Of String, TermMachine) In UiState.Machines(sMonitorMachineId).TermMachines
                    Dim oTerm As TermMachine = oTermEntry.Value
                    For k As Integer = 0 To 1
                        If oTerm.KadoSlot(k) >= 1 AndAlso oTerm.KadoSlot(k) <= recCount Then
                            oTerms(oTerm.KadoSlot(k) - 1) = oTerm
                            oTermKeys(oTerm.KadoSlot(k) - 1) = oTermEntry.Key
                            kinds(oTerm.KadoSlot(k) - 1) = k
                        End If
                    Next k
                Next oTermEntry

                For recIndex As Integer = 0 To recCount - 1
                    Dim pos As Integer = 0
                    Dim len As Integer = recLen
                    While pos < len
                        Dim readSize As Integer = oInputStream.Read(oBytes, pos, len - pos)
                        If readSize = 0 Then Exit While  'OPT: 念のためにチェックしているが、ファイルが排他されている限り、あり得ないはずであり、不要。
                        pos += readSize
                    End While

                    Dim oTerm As TermMachine = oTerms(recIndex)
                    If oTerm IsNot Nothing Then
                        Dim k As Integer = kinds(recIndex)
                        If isHokurikuMode Then
                            oTerm.KadoSeqNumber(k) = UInteger.Parse(KadoDataUtil073.GetFieldValueFromBytes(k, "基本ヘッダー シーケンスNo", oBytes))
                            oTerm.KadoDate(k) = DateTime.ParseExact(KadoDataUtil073.GetFieldValueFromBytes(k, "基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                        Else
                            oTerm.KadoSeqNumber(k) = UInteger.Parse(KadoDataUtil.GetFieldValueFromBytes(k, "基本ヘッダー シーケンスNo", oBytes))
                            oTerm.KadoDate(k) = DateTime.ParseExact(KadoDataUtil.GetFieldValueFromBytes(k, "基本ヘッダー 処理日時", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                        End If
                        UpdateTable2OnTermStateChanged(sMonitorMachineId, oTermKeys(recIndex), oTerm)  'OPT: 後で一度だけ行う方がよい。
                    End If
                Next recIndex
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "異常が発生しました。", ex)
            Return False
        End Try

        Log.Info(sMonitorMachineId, "反映しました。")
        Return True
    End Function

    Protected Function SendResponseMessage(ByVal oReceivedMessage As Message, ByVal isCompleted As Boolean, ByVal sResult As String, ByVal sMonitorMachineId As String) As Boolean
        Try
            Dim bd As ExtAppFuncMessageBody = DirectCast(oReceivedMessage.Body, ExtAppFuncMessageBody)
            bd.Func = ""
            bd.Args = Nothing
            bd.Completed = isCompleted
            bd.Result = sResult
            Dim oResponseMessage As New Message()
            oResponseMessage.CorrelationId = oReceivedMessage.Id
            oResponseMessage.Body = bd
            oReceivedMessage.ResponseQueue.Send(oResponseMessage)
            If sResult Is Nothing Then
                Log.Info(sMonitorMachineId, "シミュレータ本体へ応答 [" & isCompleted.ToString() & "] を返信しました。")
            Else
                Log.Info(sMonitorMachineId, "シミュレータ本体へ応答 [" & isCompleted.ToString() & "][" & sResult & "] を返信しました。")
            End If
            Return True
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try
    End Function

    Protected Function SendSimFuncMessage(ByVal sVerb As String, ByVal oParams As Object(), ByVal sWorkingDir As String, ByVal sMonitorMachineId As String) As Boolean
        Try
            Dim oOutMessage As New Message()
            Using oTargetQueue As New MessageQueue(Config.TargetMqPath & "@" & sWorkingDir.Replace("\", "/"))
                Dim bd As ExtSimFuncMessageBody
                bd.MachineId = GetMachineDirNameOf(sMonitorMachineId)
                bd.Verb = sVerb
                bd.Params = oParams
                oOutMessage.Body = bd
                oOutMessage.AppSpecific = 1
                oTargetQueue.Send(oOutMessage)
            End Using
            Log.Info(sMonitorMachineId, "シミュレータ本体へ要求 [" & sVerb & "] を行いました。")
            Return True
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try
    End Function

    Protected Sub InstallGateProgramDirectly(ByVal sContextDir As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal content As GateProgramContent, ByVal sDataHashValue As String)
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '指定された監視盤の改札機プログラム保持状況および、当該監視盤から
        '各端末への改札機プログラム配信保留状況を初期化し、
        '指定された改札機プログラムを当該監視盤に投入する（保持させる）。
        'また、その監視盤配下の全改札機の改札機プログラム保持状況も初期化し、
        '指定された改札機プログラムをそれらの待機面に投入し、
        'sContextDirにGateProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、GateProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの、今回更新
        'していない改札機のもの）を削除する。

        DeleteFiles(sMonitorMachineId, sContextDir, "GateProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now

        oMonitorMachine.HoldingPrograms(0) = Nothing
        oMonitorMachine.HoldingPrograms(1) = New HoldingProgram()
        oMonitorMachine.HoldingPrograms(1).DataSubKind = dataSubKind
        oMonitorMachine.HoldingPrograms(1).DataVersion = dataVersion
        oMonitorMachine.HoldingPrograms(1).DataAcceptDate = d
        oMonitorMachine.HoldingPrograms(1).RunnableDate = content.RunnableDate
        oMonitorMachine.HoldingPrograms(1).ModuleInfos = content.ModuleInfos
        oMonitorMachine.HoldingPrograms(1).ArchiveCatalog = content.ArchiveCatalog
        oMonitorMachine.HoldingPrograms(1).VersionListData = content.VersionListData
        oMonitorMachine.HoldingPrograms(1).DataHashValue = sDataHashValue
        oMonitorMachine.HoldingPrograms(1).ListVersion = 0
        oMonitorMachine.HoldingPrograms(1).ListAcceptDate = Config.EmptyTime
        oMonitorMachine.HoldingPrograms(1).ApplicableDate = Nothing
        oMonitorMachine.HoldingPrograms(1).ListContent = Nothing
        oMonitorMachine.HoldingPrograms(1).ListHashValue = Nothing

        '全端末について処理を行う。
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim oTerm As TermMachine = oTermEntry.Value

            oTerm.PendingPrograms.Clear()

            'TODO: 下記のような状況の場合、実物のシステムではどうなるのか？
            'If oTerm.McpStatusFromKsb <> &H0 Then
            '    Log.Warn(sMonitorMachineId, "端末 [" & oTermEntry.Key & "] については、主制状態が正常以外に設定されているため、配信処理を保留します。")
            '    Continue For
            'End If

            Dim oPro As New HoldingProgram()
            oPro.DataSubKind = dataSubKind
            oPro.DataVersion = dataVersion
            oPro.DataAcceptDate = d
            oPro.DataDeliverDate = d
            oPro.RunnableDate = content.RunnableDate
            oPro.ModuleInfos = content.ModuleInfos
            oPro.ArchiveCatalog = content.ArchiveCatalog
            oPro.VersionListData = content.VersionListData
            oPro.DataHashValue = sDataHashValue
            oPro.ListVersion = 0
            oPro.ListAcceptDate = Config.EmptyTime
            oPro.ListDeliverDate = Config.EmptyTime
            oPro.ApplicableDate = Nothing
            oPro.ListContent = Nothing
            oPro.ListHashValue = Nothing
            oTerm.HoldingPrograms(1) = oPro
            Log.Info(sMonitorMachineId, "端末 [" & oTermEntry.Key & "] の待機面に対して改札機プログラムを直接投入しました。")

            'プログラム本体に関する#GateProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
            'TODO: 直接投入の直後に運管から配信を行った際のおかしな挙動から、これは無いと推測しているが、記憶が定かでない。
            '実機が発生させるなら、運管的にも発生させて構わないので、実機に合わせるべき。
            'CreateFileOfGateProDlReflectReq( _
            '   &H21, _
            '   dataVersion, _
            '   &H0, _
            '   sMonitorMachineId, _
            '   oTermEntry.Key, _
            '   sMachineDir)

            CreateFileOfGateProVerInfo(sMonitorMachineId, oTermEntry.Key, oTerm, sContextDir)
            UpdateTable2OnTermStateChanged(sMachineDir, oTermEntry.Key, oTerm)
        Next oTermEntry

        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

    Protected Sub InstallKsbProgramDirectly(ByVal sContextDir As String, ByVal dataSubKind As Integer, ByVal dataVersion As Integer, ByVal content As KsbProgramContent, ByVal sDataHashValue As String)
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '指定された監視盤内部への監視盤プログラム配信保留状況および、
        '指定された監視盤内部の監視盤プログラム保持状況を初期化する、
        'さらに、指定された監視盤プログラムを当該監視盤内部の待機面に投入し、
        'sContextDirにKsbProVerInfo_RRRSSSCCCCUU.datを作成する。
        'なお、KsbProVerInfo_RRRSSSCCCCUU.datについては、
        '過去のもの（今回の配信と無関係なもの）を削除する。

        DeleteFiles(sMonitorMachineId, sContextDir, "KsbProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now

        oMonitorMachine.PendingKsbPrograms.Clear()

        oMonitorMachine.HoldingKsbPrograms(0) = Nothing
        oMonitorMachine.HoldingKsbPrograms(1) = New HoldingKsbProgram()
        oMonitorMachine.HoldingKsbPrograms(1).DataSubKind = dataSubKind
        oMonitorMachine.HoldingKsbPrograms(1).DataVersion = dataVersion
        oMonitorMachine.HoldingKsbPrograms(1).DataAcceptDate = d
        oMonitorMachine.HoldingKsbPrograms(1).DataDeliverDate = d
        oMonitorMachine.HoldingKsbPrograms(1).RunnableDate = content.RunnableDate
        oMonitorMachine.HoldingKsbPrograms(1).ArchiveCatalog = content.ArchiveCatalog
        oMonitorMachine.HoldingKsbPrograms(1).VersionListData = content.VersionListData
        oMonitorMachine.HoldingKsbPrograms(1).DataHashValue = sDataHashValue
        oMonitorMachine.HoldingKsbPrograms(1).ListVersion = 0
        oMonitorMachine.HoldingKsbPrograms(1).ListAcceptDate = Config.EmptyTime
        oMonitorMachine.HoldingKsbPrograms(1).ListDeliverDate = Config.EmptyTime
        oMonitorMachine.HoldingKsbPrograms(1).ApplicableDate = Nothing
        oMonitorMachine.HoldingKsbPrograms(1).ListContent = Nothing
        oMonitorMachine.HoldingKsbPrograms(1).ListHashValue = Nothing

        Log.Info(sMonitorMachineId, "機器 [" & sMonitorMachineId & "] の待機面に対して監視盤プログラムを直接投入しました。")

        'プログラム本体に関する#KsbProDlReflectReq_RRRSSSCCCCUU_N.datを作成する。
        'TODO: 直接投入の直後に運管から配信を行った際のおかしな挙動から、これは無いと推測しているが、記憶が定かでない。
        '実機が発生させるなら、運管的にも発生させて構わないので、実機に合わせるべき。
        'CreateFileOfKsbProDlReflectReq( _
        '   &H22, _
        '   dataVersion, _
        '   &H0, _
        '   sMonitorMachineId, _
        '   sMachineDir)

        CreateFileOfKsbProVerInfo(sMonitorMachineId, oMonitorMachine, sContextDir)
        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
    End Sub

End Class
