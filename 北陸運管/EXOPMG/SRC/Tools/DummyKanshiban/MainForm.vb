' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  �V�K�쐬
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
                    Throw New OPMGException("�@��\����" & lineCount.ToString() & "�s�ڂ̃J���������s���ł��B")
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
                        Log.Info(sMachineId, "�t�@�C�� [" & sFilePath & "] ���폜���܂����B")
                    End If
                Else
                    File.Delete(sFilePath)
                    Log.Info(sMachineId, "�t�@�C�� [" & sFilePath & "] ���폜���܂����B")
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
            'CAB��W�J����B
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

            '�v���O�����o�[�W�������X�g����͂���B
            Dim sVerListPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.GateProgramVersionListPathInCab)
            Try
                Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                    '�t�@�C���̃����O�X���擾����B
                    Dim len As Integer = CInt(oInputStream.Length)
                    If len < ProgramVersionListUtil.RecordLengthInBytes Then
                        Throw New OPMGException("�o�[�W�������X�g�̃T�C�Y���ُ�ł��B")
                    End If
                    '�t�@�C����ǂݍ��ށB
                    ret.VersionListData = New Byte(len - 1) {}
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(ret.VersionListData, pos, len - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Throw New OPMGException("�o�[�W�������X�g�̓ǂݍ��݂ňُ킪�������܂����B", ex)
            End Try

            ret.RunnableDate = ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�������싖��", ret.VersionListData)
            Dim oRunnableDate As DateTime
            If DateTime.TryParseExact(ret.RunnableDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oRunnableDate) = False Then
                Throw New OPMGException("�o�[�W�������X�g�ɋL�ڂ��ꂽ���싖�����ُ�ł��B" & vbCrLf & "���싖��: " & ret.RunnableDate)
            End If

            '�S�Ẵv���O�����O���[�v�̃x�[�X�p�X�ƁA
            '�e�O���[�v�̃f�B���N�g�����̔z��𐶐�����B
            'TODO: �Ď���CAB�����D�@CAB�Ɠ������@�ŏ�������ꍇ��
            'Config�ɊĎ��Ղ�ProgramGroup�Ɋւ���t�B�[���h��p�ӂ��A
            '���̎Q�Ƃ������ŉ��L�ϐ��ɃZ�b�g���邱�ƁB
            Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.GateProgramModuleBasePathInCab)

            'CAB���̏���f�B���N�g�������ɏ�������B
            ret.ModuleInfos = New ProgramModuleInfo(ExConstants.GateProgramModuleNamesInCab.Length - 1) {}
            For i As Integer = 0 To ExConstants.GateProgramModuleNamesInCab.Length - 1
                Dim oElems As New List(Of ProgramElementInfo)
                Dim sModName As String = ExConstants.GateProgramModuleNamesInCab(i)
                Dim sModDirPath As String = Path.Combine(sBaseDirPath, sModName)
                Dim sLine As String

                '�f�B���N�g�����ɂ��錩�o���t�@�C������͂���B
                'NOTE: ���̃A�v���ł́ACAB���̂��̂��������������Ȃ����߂ɁA�Ď��Ղ���M�������̎��_�ŁA
                '���W���[���̏��𒊏o����B�����āACAB�Ɉُ킪����΁A���o�̉ߒ��ōs���l�邽�߁A
                '�}�炸���Ď��Ղւ�DLL�V�[�P���X�̒i�K�ňُ��`���邱�ƂɂȂ�B
                '�������A�{���̊Ď��Ղ������ł���Ƃ͌���Ȃ��B
                Using oReader As StreamReader _
                   = New StreamReader(Path.Combine(sModDirPath, ExConstants.GateProgramModuleCatalogFileNameInCab), Encoding.GetEncoding(932))

                    '���o���t�@�C���̊e�s����������B
                    Dim lineNumber As Integer = 1
                    sLine = oReader.ReadLine()
                    While sLine IsNot Nothing
                        If Not sLine.StartsWith("/", StringComparison.Ordinal) Then
                            '���o���t�@�C���̔�R�����g�s����o�[�W�����Ǘ��ΏۂƂȂ�t�@�C���̖��O���擾����B
                            Dim sElementFileName As String = sLine.Substring(2, 16).TrimEnd(Chr(&H20))
                            If Not Path.GetFileName(sElementFileName).Equals(sElementFileName, StringComparison.OrdinalIgnoreCase) Then
                                Throw New OPMGException("[" & Path.Combine(sModName, ExConstants.GateProgramModuleCatalogFileNameInCab) & "] " & lineNumber.ToString() & "�s�ڂ̃t�@�C���� [" & sElementFileName  & "] ���s���ł��B")
                            End If

                            '�t�@�C���̃t�b�^��ǂݏo���B
                            Dim sElementFilePath As String = Path.Combine(sModDirPath, sElementFileName)
                            Dim oFooter As ExProgramElementFooterForG
                            Try
                                oFooter = New ExProgramElementFooterForG(sElementFilePath)
                            Catch ex As Exception
                                Throw New OPMGException("[" & Path.Combine(sModName, sElementFileName) & "] �̃t�b�^�ǂݍ��݂ňُ킪�������܂����B", ex)
                            End Try

                            '�ǂݏo�����t�b�^�̏������`�F�b�N����B
                            Dim sFooterViolation As String = oFooter.GetFormatViolation()
                            If sFooterViolation IsNot Nothing Then
                                Throw New OPMGException("[" & Path.Combine(sModName, sElementFileName) & "] �̃t�b�^�������ُ�ł��B" & vbCrLf & sFooterViolation)
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

        'CAB���̃t�@�C���ꗗ���擾����B
        Using oProcess As New System.Diagnostics.Process()
            'NOTE: TsbCab -l �́A�R�}���h�����ɓn��CAB�t�@�C���̃p�X��
            '���o�C�g�������܂܂�Ă���ƃN���b�V������悤�ł��邽�߁A
            'WorkingDirectory�𓖊Y�t�@�C���̂���f�B���N�g���ɂ��邱�ƂŁA
            '�R�}���h�����ɂ̓t�@�C�����݂̂�n�����Ƃɂ���B
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
            'CAB��W�J����B
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

            '�v���O�����o�[�W�������X�g����͂���B
            Dim sVerListPath As String = Utility.CombinePathWithVirtualPath(sTempDirPath, ExConstants.KsbProgramVersionListPathInCab)
            Try
                Using oInputStream As New FileStream(sVerListPath, FileMode.Open, FileAccess.Read)
                    '�t�@�C���̃����O�X���擾����B
                    Dim len As Integer = CInt(oInputStream.Length)
                    If len < ProgramVersionListUtil.RecordLengthInBytes Then
                        Throw New OPMGException("�o�[�W�������X�g�̃T�C�Y���ُ�ł��B")
                    End If
                    '�t�@�C����ǂݍ��ށB
                    ret.VersionListData = New Byte(len - 1) {}
                    Dim pos As Integer = 0
                    Do
                        Dim readSize As Integer = oInputStream.Read(ret.VersionListData, pos, len - pos)
                        If readSize = 0 Then Exit Do
                        pos += readSize
                    Loop
                End Using
            Catch ex As Exception
                Throw New OPMGException("�o�[�W�������X�g�̓ǂݍ��݂ňُ킪�������܂����B", ex)
            End Try

            ret.RunnableDate = ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�������싖��", ret.VersionListData)
            Dim oRunnableDate As DateTime
            If DateTime.TryParseExact(ret.RunnableDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, oRunnableDate) = False Then
                Throw New OPMGException("�o�[�W�������X�g�ɋL�ڂ��ꂽ���싖�����ُ�ł��B" & vbCrLf & "���싖��: " & ret.RunnableDate)
            End If
        Finally
            Utility.DeleteTemporalDirectory(sTempDirPath)
        End Try

        'CAB���̃t�@�C���ꗗ���擾����B
        Using oProcess As New System.Diagnostics.Process()
            'NOTE: TsbCab -l �́A�R�}���h�����ɓn��CAB�t�@�C���̃p�X��
            '���o�C�g�������܂܂�Ă���ƃN���b�V������悤�ł��邽�߁A
            'WorkingDirectory�𓖊Y�t�@�C���̂���f�B���N�g���ɂ��邱�ƂŁA
            '�R�}���h�����ɂ̓t�@�C�����݂̂�n�����Ƃɂ���B
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

    'NOTE: ���O�o�͖��ɌĂ΂��̂ŁA����̒��Ń��O���o�͂��Ă͂Ȃ�Ȃ��B
    Protected Sub BeginFetchLog( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)
        Try
            'OPT: ��L����������̓f�b�h���b�N���Ȃ��Ǝv����̂ŁA
            'BeginInvoke()�ł͂Ȃ��AInvoke()�ł��悢��������Ȃ��B
            'Invoke()�Ȃ�΁A���b�Z�[�W�L���[�����ӂ��S�z���Ȃ��B
            BeginInvoke( _
                OptionalWriter, _
                New Object() {number, sSecondName, sDateTime, sKind, sClassName, sMethodName, sText})
        Catch ex As Exception
            'NOTE: ����Control���j�����ꂽ��ɂ��̃��\�b�h���Ăяo����閜����̏ꍇ��z�肵�Ă���B
            '���̌�́i���̃f���Q�[�g�Ɉˑ����Ȃ��j������ʏ�ʂ�s���悤�A��O�͈���Ԃ��B
        End Try
    End Sub

    'NOTE: ���O�o�͖��ɌĂ΂��̂ŁA����̒��Ń��O���o�͂��Ă͂Ȃ�Ȃ��B
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

        'Lexis���琶������������Config��UiState�̒l���e�R���g���[���ɔ��f����B

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
            'NOTE: �Ō�ɌĂяo����BeginReceive�ɑΉ�����ReceiveCompleted�C�x���g�́A
            '���L�ɂ��InputQueue.readHandle�̂悤�Ȃ��̂�Close�ɂ���āA
            '����ȍ~�A�������邱�Ƃ͖����Ȃ�z��ł���B
            'InputQueue.SynchronizingObject��Nothing�ɂ��Ă����΂悳�����ɂ�
            '�v���邪�A���̃v���p�e�B�̓X���b�h�Z�[�t�ł͂Ȃ������ł��邽�߁A
            '����BeginReceive���s���Ă��܂��Ă��邱�̎��_�ł͕ύX����ׂ��ł͂Ȃ��B
            InputQueue.Dispose()
        End If

        If Config.SelfMqPath IsNot Nothing Then
            Try
                'NOTE: SelfMqPath����������ꍇ�́AMessageQueue.Exists()��
                '���ۂɓ��Y�p�X�ɃL���[�����݂��Ă��Ă��AFalse��ԋp
                '����悤�ł���B����ASelfMqPath����������ꍇ���A
                '���b�Z�[�W�L���[�T�[�r�X���C���X�g�[������Ă������́A
                'MessageQueue.Create���������Ă��܂��B
                '����āASelfMqPath����������ꍇ�ɁA�N�����ɍ쐬���Ă��܂���
                '�L���[�̍폜�����݂�ɂ́A�����ł�MessageQueue.Exists�ɂ��
                '���f���ȗ����邵���Ȃ��B�Ȃ��A�폜�����݂��Ƃ���ŁA
                'SelfMqPath���������邱�Ƃ𗝗R��MessageQueue.Delete()����
                'MessageQueueException���X���[����A�폜�͎��s���邪�A
                '����MessageQueueErrorCode�v���p�e�B�ɂ���āA�L���[���c����
                '���܂����Ƃ������邽�߁A���̌x�����o�����Ƃ��ł���B
                'If MessageQueue.Exists(Config.SelfMqPath) Then
                '    MessageQueue.Delete(Config.SelfMqPath)
                'End If

                MessageQueue.Delete(Config.SelfMqPath)

            Catch ex As MessageQueueException When ex.MessageQueueErrorCode = MessageQueueErrorCode.FormatNameBufferTooSmall
                'NOTE: �{���́A�A�v���P�[�V������MessageQueueErrorCode��
                '�Q�Ƃ���ׂ��ł͂Ȃ����AMessageQueue�N���X�̋�����
                '�������邽�߂�ނ𓾂Ȃ��BMessageQueue�N���X�̋�����
                '���P���ꂽ��A�������邱�ƁB
                Log.Fatal("Unwelcome Exception caught.", ex)
                AlertBox.Show(Lexis.MessageQueueDeleteFailed)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
            End Try
        End If

        If UiState IsNot Nothing Then
            'NOTE: ���̃P�[�X�ł́A�E�ӂ̊e�R���g���[���ɁA���Ȃ��Ƃ��N�����̃t�@�C������
            '���[�h�����l�̓Z�b�g�ς݂̑z��ł���B

            'TODO: �R���g���[���̏�Ԃ�ۑ����邱�Ƃɂ����ꍇ�́A
            '�����Ŋe�R���g���[���̒l��UiState�ɔ��f����B

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
            InitExtraComboColumnViewOfTable2("PWR_FROM_KSB", "�d����� by�Ď� (X)", "FF..", "�d����� by�Ď�", "������� by����..", Config.MenuTableOfPwrStatusFromKsb)
            InitExtraComboColumnViewOfTable2("MCP_FROM_KSB", "�吧��� by�Ď� (X)", "FF..", "�吧��� by�Ď�", "������� by����..", Config.MenuTableOfMcpStatusFromKsb)
            InitExtraComboColumnViewOfTable2("ICM_FROM_MCP", "ICU��� by�吧 (X)", "FF..", "ICU��� by�吧", "������� by����..", Config.MenuTableOfIcmStatusFromMcp)
            InitExtraComboColumnViewOfTable2("DLS_FROM_MCP", "�z�T��� by�吧 (X)", "FF..", "�z�T��� by�吧", "������� by����..", Config.MenuTableOfDlsStatusFromMcp)
            InitExtraComboColumnViewOfTable2("DLS_FROM_ICM", "�z�T��� byICU (X)", "FF..", "�z�T��� byICU", "������� by����..", Config.MenuTableOfDlsStatusFromIcm)
            InitExtraComboColumnViewOfTable2("EXS_FROM_ICM", "���T��� byICU (X)", "FF..", "���T��� byICU", "������� by����..", Config.MenuTableOfExsStatusFromIcm)
        End If

        If MasStatusRadioButton.Checked Then
            DataGridView2.Columns("SLOT").ReadOnly = True
            DataGridView2.Columns("SLOT").Frozen = True
            DataGridView2.Columns("SLOT").HeaderText = "����"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("�z�M�҂�(9)",  DataGridView2.Font)
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
                DataGridView2.Columns(sKind & "_DataSubKind").HeaderText = "�p�^�[��No (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataVersion").HeaderText = "�}�X�^Ver (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListVersion").HeaderText = "���X�gVer (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataAcceptDate").HeaderText = "�f�[�^�Ē� (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListAcceptDate").HeaderText = "���X�g�Ē� (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataDeliverDate").HeaderText = "�f�[�^���� (" & sKind &")"
                DataGridView2.Columns(sKind & "_DataHashValue").HeaderText = "�f�[�^�T�l (" & sKind &")"
                DataGridView2.Columns(sKind & "_ListHashValue").HeaderText = "���X�g�T�l (" & sKind &")"
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
            DataGridView2.Columns("SLOT").HeaderText = "����"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("�z�M�҂�(9)", DataGridView2.Font)
            Dim anWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim adWidth As Integer = MyUtility.GetTextWidth("����������.", DataGridView2.Font)
            Dim rdWidth As Integer = MyUtility.GetTextWidth("����������.", DataGridView2.Font)
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
            DataGridView2.Columns("GPG_DataSubKind").HeaderText = "�G���ANo"
            DataGridView2.Columns("GPG_DataVersion").HeaderText = "��\Ver"
            DataGridView2.Columns("GPG_ListVersion").HeaderText = "���X�gVer"
            DataGridView2.Columns("GPG_DataAcceptDate").HeaderText = "�f�[�^�Ē�����"
            DataGridView2.Columns("GPG_ListAcceptDate").HeaderText = "���X�g�Ē�����"
            DataGridView2.Columns("GPG_DataDeliverDate").HeaderText = "�f�[�^��������"
            DataGridView2.Columns("GPG_ListDeliverDate").HeaderText = "���X�g��������"
            DataGridView2.Columns("GPG_RunnableDate").HeaderText = "���싖��"
            DataGridView2.Columns("GPG_ApplicableDate").HeaderText = "�K�p��"
            DataGridView2.Columns("GPG_ApplyDate").HeaderText = "�K�p��������"
            DataGridView2.Columns("GPG_DataHashValue").HeaderText = "�f�[�^�n�b�V���l"
            DataGridView2.Columns("GPG_ListHashValue").HeaderText = "���X�g�n�b�V���l"
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
            DataGridView2.Columns("SLOT").HeaderText = "����"
            DataGridView2.Columns("SLOT").Width = MyUtility.GetTextWidth("�z�M�҂�(9)", DataGridView2.Font)
            Dim anWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim dvWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim lvWidth As Integer = MyUtility.GetTextWidth("������Abc", DataGridView2.Font)
            Dim adWidth As Integer = MyUtility.GetTextWidth("����������.", DataGridView2.Font)
            Dim rdWidth As Integer = MyUtility.GetTextWidth("����������.", DataGridView2.Font)
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
            DataGridView2.Columns("WPG_DataSubKind").HeaderText = "�G���ANo"
            DataGridView2.Columns("WPG_DataVersion").HeaderText = "��\Ver"
            DataGridView2.Columns("WPG_ListVersion").HeaderText = "���X�gVer"
            DataGridView2.Columns("WPG_DataAcceptDate").HeaderText = "�f�[�^��M����"
            DataGridView2.Columns("WPG_ListAcceptDate").HeaderText = "���X�g��M����"
            DataGridView2.Columns("WPG_DataDeliverDate").HeaderText = "�f�[�^DL��������"
            DataGridView2.Columns("WPG_ListDeliverDate").HeaderText = "���X�gDL��������"
            DataGridView2.Columns("WPG_RunnableDate").HeaderText = "���싖��"
            DataGridView2.Columns("WPG_ApplicableDate").HeaderText = "�K�p��"
            DataGridView2.Columns("WPG_ApplyDate").HeaderText = "�K�p��������"
            DataGridView2.Columns("WPG_DataHashValue").HeaderText = "�f�[�^�n�b�V���l"
            DataGridView2.Columns("WPG_ListHashValue").HeaderText = "���X�g�n�b�V���l"
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
            InitExtraComboColumnViewOfTable2("LATCH_CONF", "���b�`�`��(X)", "FF..", "���b�`�`��", "���b�`�O�o�D...��", Config.MenuTableOfLatchConf)
            DataGridView2.Columns("LATCH_CONF").Frozen = True
            DataGridView2.Columns("LATCH_CONF_MENU").Frozen = True

            DataGridView2.Columns("FAULT_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("FAULT_SEQ_NO").HeaderText = "�ŏI�ُ�SEQ.No"
            DataGridView2.Columns("FAULT_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("FAULT_SEQ_NO").Width = MyUtility.GetTextWidth("�ŏI�ُ�SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("FAULT_DATE").ReadOnly = True
            DataGridView2.Columns("FAULT_DATE").HeaderText = "�ŏI�ُ폈������"
            'DataGridView2.Columns("FAULT_DATE").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("FAULT_DATE").Width = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)

            DataGridView2.Columns("KADO_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("KADO_SEQ_NO").HeaderText = "�ŏI�ғ�SEQ.No"
            DataGridView2.Columns("KADO_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("KADO_SEQ_NO").Width = MyUtility.GetTextWidth("�ŏI�ғ�SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("KADO_DATE").ReadOnly = True
            DataGridView2.Columns("KADO_DATE").HeaderText = "�ŏI�ғ���������"
            'DataGridView2.Columns("KADO_DATE").DefaultCellStyle.Format = Config.DateTimeFormatInGui
            DataGridView2.Columns("KADO_DATE").Width = MyUtility.GetTextWidth("9999/99/99 99:99:99", DataGridView2.Font)

            DataGridView2.Columns("HOSYU_SEQ_NO").ReadOnly = True
            DataGridView2.Columns("HOSYU_SEQ_NO").HeaderText = "�ŏI�ێ�SEQ.No"
            DataGridView2.Columns("HOSYU_SEQ_NO").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            DataGridView2.Columns("HOSYU_SEQ_NO").Width = MyUtility.GetTextWidth("�ŏI�ێ�SEQ.No..", DataGridView2.Font)
            DataGridView2.Columns("HOSYU_DATE").ReadOnly = True
            DataGridView2.Columns("HOSYU_DATE").HeaderText = "�ŏI�ێ珈������"
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
            oTargetRow("SLOT") = "�ێ�(" & (listIndex + 1).ToString() & ")"
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
            oTargetRow("SLOT") = "�ێ�(" & (listIndex + 1).ToString() & ")"

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
                oTargetRow("SLOT") = "�z�M�҂�(" & (listIndex + 1).ToString() & ")"

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
                oTargetRow("SLOT") = If(listIndex = 1, "�K�p�҂�", "�K�p��")

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
                oTargetRow("SLOT") = "�z�M�҂�(" & (listIndex + 1).ToString() & ")"
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
                oTargetRow("SLOT") = "�K�p��"
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
                oTargetRow("SLOT") = "�z�M�҂�(" & (listIndex + 1).ToString() & ")"

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
                oTargetRow("SLOT") = If(listIndex = 1, "�K�p�҂�", "�K�p��")

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
        '��̃f�[�^�e�[�u�����쐬���A�t�B�[���h����ݒ肷��B
        Table1 = New DataTable()
        For i As Integer = 0 To Config.Table1FieldNames.Length - 1
            Dim sFieldName As String = Config.Table1FieldNames(i)
            Table1.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i

        'UiState.Machines�̊�{�����f�[�^�e�[�u���ɓW�J����B
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
        '��̃f�[�^�e�[�u�����쐬���A�t�B�[���h����ݒ肷��B
        Table2 = New DataTable()
        For i As Integer = 0 To Config.Table2FieldNames.Length - 1
            Dim sFieldName As String = Config.Table2FieldNames(i)
            Table2.Columns.Add(sFieldName, Config.FieldNamesTypes(sFieldName))
        Next i
        AddExtraColumnsToTable2()

        'UiState.Machines�̊�{�����f�[�^�e�[�u���ɓW�J����B
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
            Log.Info("�V�����Ď��@�� [" & sMachineId & "] �����o���܂����B")
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
                    Throw New OPMGException("�@��\���t�@�C���̍s�����ُ�ł��B")
                End If
                If GetMachineId(oProfileTable.Rows(0)) <> sMachineId Then
                    Throw New OPMGException("�@��\���t�@�C���̓��e�ƍ�ƃf�B���N�g�����ɕs����������܂��B")
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
                        Log.Info("�V�����[���@�� [" & sTermId & "] �����o���܂����B")
                        oTerm = New TermMachine()
                        oTerm.Profile = GetMachineProfile(oProfileRow)
                        oTerm.LatchConf = If(oProfileRow.Field(Of String)("CORNER_NAME").Contains("�抷"), CByte(&H2), CByte(&H1))

                        Dim oExemplar As HoldingProgram = Nothing
                        If oMachine.HoldingPrograms(0) IsNot Nothing Then
                            'NOTE: �Ď��Ղ͈�x���D�@�p�v���O�������󂯓����ƁA
                            '�����S�Ẳ��D�@�ɓK�p���邩�A�������~�Ŏ̂Ă邩���Ȃ�����A
                            '����ƈقȂ���͎̂󂯓���Ȃ��B���Ȃ킿�A���D�@�ɊĎ��Ղ�
                            '�ێ����Ă�����̂ƈقȂ���̂��K�p����Ă����ԂƂ����̂́A
                            '�z��O�̏�Ԃł���ƌ�����B����āA�Ď��Ղ̋�����ʂ�
                            '���̂����݂���ꍇ�́A���ꂪ��������Ă���̂̉��D�@��
                            '�ǉ�����B
                            oExemplar = oMachine.HoldingPrograms(0)
                        ElseIf oMachine.HoldingPrograms(1) IsNot Nothing Then
                            'NOTE: �Ď��Ղ͈�x���D�@�p�v���O�������󂯓����ƁA
                            '�����S�Ẳ��D�@�ɓK�p���邩�A�������~�Ŏ̂Ă邩���Ȃ�����A
                            '����ƈقȂ���͎̂󂯓���Ȃ��B���Ȃ킿�A���D�@�ɊĎ��Ղ�
                            '�ێ����Ă�����̂ƈقȂ���̂��K�p����Ă����ԂƂ����̂́A
                            '�z��O�̏�Ԃł���ƌ�����B����āA�Ď��Ղ̐V����ʂ�
                            '���̂����݂���ꍇ�́A���ꂪ��������Ă���̂̉��D�@��
                            '�ǉ�����B
                            oExemplar = oMachine.HoldingPrograms(1)
                        ElseIf oMachine.TermMachines.Count <> 0 Then
                            'NOTE: �P�̊Ď��Ղ̔z���ɂ�����D�@�̃v���O�����́A�o�[�W������
                            '�����Ă��邱�Ƃ���{�ł���i�����Ă��Ȃ��ƁA�^�ǂ���̔z�M���s�\��
                            '�Ȃ�P�[�X������j���߁A�����̉��D�@�Ɠ����v���O�������C���X�g�[��
                            '���ꂽ�̂̉��D�@��ǉ�����B
                            oExemplar = oMachine.TermMachines.Values(0).HoldingPrograms(0)
                        End If

                        oTerm.HoldingPrograms(0) = New HoldingProgram()
                        oTerm.HoldingPrograms(0).DataDeliverDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).ListDeliverDate = Config.UnknownTime
                        oTerm.HoldingPrograms(0).ApplyDate = Config.UnknownTime
                        If oExemplar IsNot Nothing Then
                            'NOTE: ����́u�ێ�@�\�ɂ�钼�ړ����v�̈��ł��邪�A
                            '���̌�̔z�M��DL�����ʒm���������Ȃ��̂͂��܂�ɂ����Ȃ̂ŁA
                            '�_�~�[�̓K�p���X�g���������Ă����B
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
                            Log.Warn("�폜���ꂽ�[�� [" & oTermEntry.Key & "] �̏�ԏ����N���A���܂��B")
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

        'NOTE: �[���@��̍\�����ω����Ă��ATable2�̊Ď��@��̍s�ɁA�֘A���鍀�ڂ͖����z��ł���B
        '����āATable2�̊Ď��@��̍s�ɂ��ẮA�Ď��@�펩�̂��ǉ����ꂽ�P�[�X�ł̂݁A�P�A����B
        If newMachineDetected Then
            'NOTE: ����́u�ێ�@�\�ɂ�钼�ړ����v�̈��ł��邪�A
            '���̌�̔z�M��DL�����ʒm���������Ȃ��̂͂��܂�ɂ����Ȃ̂ŁA
            '�_�~�[�̓K�p���X�g���������Ă����B
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

        '�ғ��ێ�f�[�^�Ǘ��t�@�C���̓��Y���R�[�h������������B
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

                'OPT: �����̎��s�� k = 1 �̏ꍇ�Ɍ��肵�Ă悢�B
                '�t�@�C���T�C�Y���O�̏ꍇ�ɂ������ȗ����Ă��A����Seek�ɂ���āA�t�@�C���T�C�Y���傫���Ȃ�͂��ł���B
                oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
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
                            'NOTE: �_�u���N���b�N���ꂽ�̂��Ď��@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                            '�Ď��@��̍s�́A�ێ�(1)�ƕێ�(2)�݂̂ł��邽�߁A��L�̏������������Ă���Ȃ�AoMachine.HoldingMasters�ɂ�
                            'sDataKind���L�[�Ƃ���v�f���K�����݂��Ă���B
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
                            'NOTE: �_�u���N���b�N���ꂽ�̂��[���@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                            '�[���@��ɂ́A�K�p���̍s�Ɣz�M�҂�(n)�̍s�����邽�߁A��L�̏������������Ă��Ă��A�i�_�u���N���b�N���ꂽ
                            '�̂��z�M�҂�(n)�̍s�Ȃ�joTerm.HoldingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���Ƃ͌���Ȃ����A
                            '�i�_�u���N���b�N���ꂽ�̂��K�p���̍s�Ȃ�joTerm.PendingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���
                            '�Ƃ͌���Ȃ��B
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
                        'NOTE: �_�u���N���b�N���ꂽ�̂��Ď��@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                        '�Ď��@��̍s�́A�ێ�(1)�ƕێ�(2)�݂̂ł��邽�߁A��L�̏������������Ă���Ȃ�AoMachine.HoldingMasters�ɂ�
                        'sDataKind���L�[�Ƃ���v�f���K�����݂��Ă���B
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
                        'NOTE: �_�u���N���b�N���ꂽ�̂��[���@��̍s�ł���A���̍s��sDataKind�Ɋւ����ɂ͒l�����݂��Ă���ꍇ�ł���B
                        '�[���@��ɂ́A�K�p���̍s�Ɣz�M�҂�(n)�̍s�����邽�߁A��L�̏������������Ă��Ă��A�i�_�u���N���b�N���ꂽ
                        '�̂��z�M�҂�(n)�̍s�Ȃ�joTerm.HoldingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���Ƃ͌���Ȃ����A
                        '�i�_�u���N���b�N���ꂽ�̂��K�p���̍s�Ȃ�joTerm.PendingMasters��sDataKind���L�[�Ƃ���v�f�����݂��Ă���
                        '�Ƃ͌���Ȃ��B
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
                                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] �Ɣz���̑S�[������A�}�X�^����у}�X�^�K�p���X�g���폜���܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] ���A�z�M�҂��̑S�}�X�^��z�M���܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
        oDialog.Filter = "CAB�t�@�C��|*.cab"
        oDialog.FileName = ""
        oDialog.ReadOnlyChecked = True
        oDialog.Title = "��������v���O������I�����Ă��������B"
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

        'NOTE: �ȉ��A�����������A�����@�킪�I������Ă���ꍇ�̑��x���\��D�悵�āA
        'InstallGateProgramDirectly�̒��ł͂Ȃ��A�Ăь���CAB�̉�͂��s�����Ƃɂ��Ă���B

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(oDialog.FileName)
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̃n�b�V���l�Z�o�ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim content As GateProgramContent
        Try
            Dim sMonitorMachineId As String = DirectCast(DataGridView1.CurrentRow.Cells(idxColumn).Value, String)
            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error("��\�@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            content = ExtractGateProgramCab(oDialog.FileName, Path.Combine(sContextDir, "GatePro"))
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̃G���ANo�̒��o�ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̑�\Ver�̒��o�ŗ�O���������܂����B", ex)
            Return
        End Try

        'TODO: �������D�@�V�X�e���ł́A�Ď��Քz���̉��D�@�́A�S�ē���G���A�ɏ������Ă���͂��Ȃ̂ŁA
        '�����ŁA���ނ̃G���A�ƊĎ��Ղ��Ǘ����Ă�����D�@�G���A�̐������`�F�b�N���s�����Ƃ��\�Ǝv����B
        '�������@���`�F�b�N���s���Ȃ�A����ɍ��킹�������悢�B
        '�����炭�A�������D�@��HW���̂́A�ǂ̃G���A�̉��D�@�v���O�������C���X�g�[���\�ł���A
        '���ړ����ɂ����Ă܂ł����W���邱�Ƃ͖����Ǝv���邪�B

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("�v���O�����̓��싖�����������ɐݒ肳��Ă��܂��B" & content.RunnableDate & "�܂œK�p�ł��܂���̂ł����ӂ��������B")
        End If

        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] ����A���D�@�v���O�����𒼐ړ������܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] ���A�z�M�҂��̑S���D�@�v���O������z�M���܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] �z���̑S�[���ɂ����āA�K�p�҂��̃v���O������K�p���܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
        oDialog.Filter = "CAB�t�@�C��|*.cab"
        oDialog.FileName = ""
        oDialog.ReadOnlyChecked = True
        oDialog.Title = "��������v���O������I�����Ă��������B"
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

        'NOTE: �ȉ��A�����������A�����@�킪�I������Ă���ꍇ�̑��x���\��D�悵�āA
        'InstallKsbProgramDirectly�̒��ł͂Ȃ��A�Ăь���CAB�̉�͂��s�����Ƃɂ��Ă���B

        Dim sHashValue As String
        Try
            sHashValue = Utility.CalculateMD5(oDialog.FileName)
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̃n�b�V���l�Z�o�ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim content As KsbProgramContent
        Try
            Dim sMonitorMachineId As String = DirectCast(DataGridView1.CurrentRow.Cells(idxColumn).Value, String)
            Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error("��\�@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return
            End If

            Dim sContextDir As String = Path.Combine(sMonitorMachineDir, Config.UserContextDirName)
            Directory.CreateDirectory(sContextDir)

            content = ExtractKsbProgramCab(oDialog.FileName, Path.Combine(sContextDir, "KsbPro"))
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̃G���ANo�̒��o�ŗ�O���������܂����B", ex)
            Return
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̑�\Ver�̒��o�ŗ�O���������܂����B", ex)
            Return
        End Try

        'TODO: �����ŁA���ނ̃G���A�ƊĎ��Ղ��F�����Ă���Ď��ՃG���A�̐������`�F�b�N���s�����Ƃ��\�Ǝv����B
        '�������@���`�F�b�N���s���Ȃ�A����ɍ��킹�������悢�B

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("�v���O�����̓��싖�����������ɐݒ肳��Ă��܂��B" & content.RunnableDate & "�܂œK�p�ł��܂���̂ł����ӂ��������B")
        End If

        For Each gridRow As DataGridViewRow In DataGridView1.Rows
            If gridRow.Selected Then
                Dim sMonitorMachineId As String = DirectCast(gridRow.Cells(idxColumn).Value, String)
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] ����A�Ď��Ճv���O�����𒼐ړ������܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] ���A�z�M�҂��̊Ď��Ճv���O������z�M���܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] �ɂ����āA�K�p�҂��̃v���O������K�p���܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Info(sMonitorMachineId, "�I�𒆂̊Ď��@�� [" & sMonitorMachineId & "] ���A�ڑ���Ԃ𑗐M���܂�...")

                Dim sMonitorMachineDir As String = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
                If Not Directory.Exists(sMonitorMachineDir) Then
                    Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                Log.Error("�s���ȗv������M���܂����B" & vbCrLf & "WorkingDirectory: " & If(sContextDir Is Nothing, "Nothing", "[" & sContextDir & "]") & vbCrLf & "Func: " & If(bd.Func Is Nothing, "Nothing",  "[" & bd.Func & "]"))
            End If

            Try
                Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
                sTargetMachineId = GetMachineId(Path.GetFileName(sMachineDir))
                FetchMachineProfileFromFile(sMachineDir)
            Catch ex As Exception
                Log.Error("Exception caught.", ex)
                Log.Error("�s���ȗv������M���܂����B" & vbCrLf & "WorkingDirectory: [" & sContextDir & "]" & vbCrLf & "Func: [" & bd.Func & "]")
                Exit Try
            End Try

            Log.Info(sTargetMachineId, "[" & sContextDir & "] �ɑ΂���v�� [" & bd.Func & "] ���������܂�...")
            Select Case bd.Func.ToUpperInvariant()
                'TODO: bd.Args�̌����i0�̏ꍇ��Nothing�ł��邱�Ɓj���`�F�b�N����B
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
                    Log.Error(sTargetMachineId, "���m�̗v���ł��B")
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
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
    End Sub

    Protected Sub CreateFileOfGateProVerInfo(ByVal sMonitorMachineId As String, ByVal sTermId As String, ByVal oTermMachine As TermMachine, ByVal sContextDir As String)
        'NOTE: ���D�@�ɕK�v�ȓK�p���X�g�́A���ꂩ��K�p����i��M�͍ς񂾂�
        '�܂��K�p���Ă��Ȃ��j�v���O�����ɑ΂���K�p���X�g�����ł���Ǝv����B
        '����āAoTermMachine.HoldingPrograms(1)�����݂��Ȃ��ꍇ�́A
        '�K�p���X�g�𖳂��Ƃ���i���X�g�o�[�W����0���Z�b�g����j�B
        'TODO: �{���̉��D�@�i�Ď��ՁH�j�������Z�b�g����̂��m�F���ׂ��B
        'Dim listVer As Integer = oTermMachine.HoldingPrograms(0).ListVersion
        Dim listVer As Integer = 0
        If oTermMachine.HoldingPrograms(1) IsNot Nothing Then
            'NOTE: ���ړ����̒���ł��AListVersion�ɂ͓���̒l�i0�j���Z�b�g����Ă���A
            '��������̂܂ܑ��M���邱�Ƃɂ���B
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
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
    End Sub

    Protected Sub CreateFileOfKsbProVerInfo(ByVal sMachineId As String, ByVal oMachine As Machine, ByVal sContextDir As String)
        'NOTE: �Ď��Ղ����g�̊Ǘ��ɕK�v�Ƃ���K�p���X�g�́A���ꂩ��K�p����i��M�͍ς񂾂�
        '�܂��K�p���Ă��Ȃ��j�v���O�����ɑ΂���K�p���X�g�����ł���Ǝv����B
        '����āAoMachine.HoldingKsbPrograms(1)�����݂��Ȃ��ꍇ�́A
        '�K�p���X�g�𖳂��Ƃ���i���X�g�o�[�W����0���Z�b�g����j�B
        'TODO: �{���̉��D�@�i�Ď��ՁH�j�������Z�b�g����̂��m�F���ׂ��B
        'Dim listVer As Integer = oMachine.HoldingKsbPrograms(0).ListVersion
        Dim listVer As Integer = 0
        If oMachine.HoldingKsbPrograms(1) IsNot Nothing Then
            'NOTE: ���ړ����̒���ł��AListVersion�ɂ͓���̒l�i0�j���Z�b�g����Ă���A
            '��������̂܂ܑ��M���邱�Ƃɂ���B
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
        Log.Info(sMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
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
            Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ��z�M���� [" & deliveryResult.ToString("X2") & "] �ō쐬���܂����B")
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
            Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ��z�M���� [" & deliveryResult.ToString("X2") & "] �ō쐬���܂����B")
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
            Log.Info(sMachineId, "�t�@�C�� [" & sFilePath & "] ��z�M���� [" & deliveryResult.ToString("X2") & "] �ō쐬���܂����B")
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
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̍쐬�����s���܂����B", ex)
            Return Nothing
        End Try
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")
        Return sFilePath
    End Function

    Protected Function CreateConStatus(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă���ڑ���Ԃ����ƂɁA
        'sContextDir��ConStatus.dat���쐬����B

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
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���쐬���܂����B")

        Return True
    End Function

    Protected Function CreateGateMasVerInfo(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă���}�X�^�ێ���Ԃ����ƂɁA
        'sContextDir�ɍ��@�ʂ�GateMasVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�܂��A�ߋ��̂��̂�����Ώ����B

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

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă�����D�@�����v���O�����ێ���Ԃ����ƂɁA
        'sContextDir�ɍ��@�ʂ�GateProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�܂��A�ߋ��̂��̂�����Ώ����B

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

        'oMonitorMachine�ɐݒ肳��Ă���Ď��Ռ����v���O�����ێ���Ԃ����ƂɁA
        'sContextDir�ɍ��@�ʂ�KsbProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�܂��A�ߋ��̂��̂�����Ώ����B

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

    'NOTE: �{���̊Ď��Ղ́A���D�@�ƃI�t���C���ł����Ă��A���D�@���ێ����Ă���}�X�^��
    '�����o�[�W�����̓K�p���X�g���^�ǃT�[�o�����M����΁A�Ď��Վ��g���u�K�p�ς݁v��
    'DL�����ʒm���쐬���A�^�ǃT�[�o�ɑ��M����B�������A���̃A�v���ł́ADL�����ʒm��
    '���M����^�C�~���O��C�ӂɂ������̂ŁA���̂悤�Ȃ��Ƃ͍s��Ȃ��B
    '�܂��A����䂦�ɁA���D�@�̕ێ����Ă���}�X�^�̃o�[�W������v1�A���D�@�ƃI�t���C��
    '�ɂȂ��Ă���Ď��ՂɃL���[�C���O����Ă���i���Y���D�@�ɑ��M����ׂ��j�}�X�^��
    '�o�[�W������v2�̏󋵂ŁA�^�ǃT�[�o����v1�̓K�p���X�g����M�����ہA����Ȕz����
    '�����Ƃ��A�u�K�p�ς݁v��DL�����ʒm�𐶐����Ă��܂��悤�Ȃ��Ƃ͂Ȃ��B
    '�Ȃ��A�{���̊Ď��Ղ́A�Ď��Փ����Ɂu�I�����C���ɂȂ��Ă�����D�@���ێ����Ă���
    '�͂��̃}�X�^�v�����z�I�ɊǗ����邱�ƂŁA���̏󋵂Łu�K�p�ς݁v�͐������Ȃ��悤��
    '�Ȃ��Ă��邩������Ȃ����A�����ł͂Ȃ��u�K�p�ς݁v�𐶐����Ă��܂���������Ȃ��B
    '�ǂ���ł��邩�́A�ڑ����������{��������ł͕s���ł������B
    Protected Function AcceptGateMas(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDir��#PassiveDllReq.dat�������t�@�C�������Ƃ�
        '�Ď��Ղ̃}�X�^�ێ���ԁioMonitorMachine.HoldingMasters�j���X�V���A
        'sContextDir��ExtOutput.dat���쐬����B
        '�������A�{���̊Ď��ՂƓ����悤�ɁA�Ď��ՂɊi�[�ꏊ���Ȃ�
        '�i���Ɋi�[���Ă�����̂��̂Ă邱�Ƃ��ł��Ȃ��j�ꍇ�́A
        '�Ď��Ղ̃}�X�^�ێ���Ԃ��X�V�����ɁAContinueCode��
        'FinishWithoutStoring��ExtOutput.dat���쐬����B

        'NOTE: ContinueCode��Finish��ExtOutput.dat���쐬�����ꍇ�́A
        'DL�����ʒm���쐬���Ȃ���΂Ȃ�Ȃ��B����ɂ��ẮA
        '�}�X�^�K�p���X�g�ɋL�ڂ��ꂽ���D�@(t)�̃}�X�^�ێ����
        '�ioMonitorMachine.TermMachines(t).HoldingMasters�j�����̏��
        '�X�V���邱�Ƃɂ�����ŁADL�����ʒm�����̏��sContextDir��
        '�쐬����̂��ȒP�ł��邪�A���D�@�܂œ͂��Ă��Ȃ����Ԃ�
        '�Č��������̂ŁA���̃A�v����DeliverGateMas������p�ӂ��A
        '�V�~�����[�^�{�̂��炻�̏�����v�����ꂽ�ۂɁA���D�@��
        '�}�X�^�ێ���Ԃ��X�V��������Ƃ���B
        '���̕����̏ꍇ�A�P���DeliverGateMas�ŁA����܂łɎ�M����
        '�����̓K�p���X�g�̕���DL�����ʒm���쐬����d�l�ɂ��Ă�
        '��a�����Ȃ��͂��B�����āA���̂悤�Ȏd�l�ɂ���΁A
        '�V�i���I�́APassiveDll���n���h�����O���������Ƃ͕ʂ̕����ŁA
        'DeliverGateMas�̗v�������DL�����ʒm��TryActiveOne���s��
        '���Ƃ��ł���悤�ɂȂ�B�܂��A���̃A�v�����A�쐬����
        'DL�����ʒm�𕶖��ʂɊǗ�����K�v���Ȃ��Ȃ�B
        '�������A�}�X�^�K�p���X�g���L���[�ɕۑ����Ă����K�v������B
        '���@���Ƃ�DL�����̃^�C�~���O��ς������̂ł���΁A
        '���̃L���[�͍��@���Ƃɗp�ӂ���ׂ��ł���B
        '�}�X�^��ʂ��Ƃ�DL�����̃^�C�~���O��ς������̂ł���΁A
        '���̃L���[�̓}�X�^��ʂ��Ƃɗp�ӂ���ׂ��ł���B
        '�Ƃ肠�������U�@�ŁADeliverGateMas������p�ӂ��A���@��
        '��ʕʂ̃L���[���p�ӂ���������������Ă����B

        'NOTE: �u�ŏI��M���i�f�[�^�{�́j��z�M�������Ă��Ȃ��ꍇ�ɁA�}�X�^�o�[�W����
        '��p�^�[���ԍ�������ƈقȂ��M���i�f�[�^�{�̂�K�p���X�g�j���󂯓���Ȃ��v
        '�悤�ɊĎ��Օ�����������Ƃ��Ă��A�L���[�ɂ́A�}�X�^�o�[�W������p�^�[���ԍ�
        '������̂��̂���������Ƃ͌���Ȃ��B��̓I�ɂ́A�O���Ƀ}�X�^�o�[�W������
        '�p�^�[���ԍ����P����O�̂��̂��i�P�܂��͕����j����A�㔼�Ƀ}�X�^�o�[�W����
        '�ƃp�^�[���ԍ����ŐV�̂��̂��i�P�܂��͕����j����ꍇ������͂��ł���B
        '�֑��ł��邪�A���̏ꍇ�A�Ď��Վ��g���㔼�̂��̂��󂯓���Ă��邱�Ƃ���A�O����
        '���̂Ɠ����}�X�^�o�[�W��������уp�^�[���ԍ����t�^���ꂽ�f�[�^�{�̂�S���D�@��
        '�z�M�ς݂ł���ƌ�����B�܂�A�e�L���[�ɂ�����O���̏��́A���Y���D�@��
        '�΂��ăf�[�^�{�̂�z�M������������ɁA�����}�X�^�o�[�W�����E�����p�^�[���ԍ���
        '�f�[�^�{�̂܂��͓K�p���X�g����M�����ۂɁA�ł������̂ł���ƌ����؂��B
        '������ɂ���A���̂悤�ȏ�Ԃ���������̂Ȃ�A�u�ŏI��M���i�f�[�^�{�́j��z�M
        '�������Ă��Ȃ��ꍇ�ɁA�}�X�^�o�[�W������p�^�[���ԍ�������ƈقȂ��M��
        '�i�f�[�^�{�̂�K�p���X�g�j���󂯓���Ȃ��v�悤�ɊĎ��Օ�������荞�ވӖ����Ȃ�
        '�悤�Ɏv���邩������Ȃ����A�i�����́j�Ď��Ղ͂��̂悤�Ȋ����ł������������ꂸ�A
        '����͒P���ɊĎ��Ղ̎d�l�ł���i�d�l�ł������j�Ǝv����B
        '�܂��A���Ȃ��Ƃ��}�X�^�ɂ��ẮA������D�@���w�肵������p�^�[���ԍ�����
        '����}�X�^�o�[�W������DLL�������񂠂�ꍇ�ɁA�P��ڂ�DL�����ʒm���������A
        '����ȏ��DL�����ʒm������Ă��^�ǂ̕\�����ς��Ȃ��䂦�A
        '�Q�ڈȍ~�̏���ێ�����K�v���Ȃ��悤�Ɏv���邩������Ȃ��B
        '�����āA��������Ȃ�΁A��q�����Ď��Ղ̎d�l�𓱓����邱�ƂŁA�P�̉��D�@��
        '���ăL���[�C���O���ׂ����͍ő�łP�����ɂȂ�i�������A���̃}�X�^�o�[�W����
        '��p�^�[���ԍ��́A�Ď��Ղ��Ō�Ɏ�M�������̂Ɠ����ɂȂ�͂��ł���j����A
        '�L���[�ȂǕK�v�Ȃ��Ȃ�悤�Ɏv���邩������Ȃ��B
        '�������A�^�ǂ̕\�����ς��Ȃ��Ƃ����Ă��A�S�Ă�DLL�ɑΉ�����DL�����ʒm��
        '���i�Q��ڂ���́u�K�p�ς݁v��DL�����ʒm�����j���Ƃ͌��܂育�Ƃł��邽�߁A
        '�{���ɂ������낤�Ƃ�����A�L���[�C���O�͕K�{�ł���B
        '�Ȃ��A���@���ƃ}�X�^��ʂ��ƂɁA�z�M��ۗ����Ă�����̂̃}�X�^�o�[�W����
        '����уp�^�[���ԍ����Q���㕪�L�����A����ɂ��ꂼ��̉񐔂��L�����Ă����΁A
        '�������͂��ł��邪�A�_��ƒP������D�悵�A�ϗe�ʂ̃L���[�ŊǗ�����
        '���Ƃɂ���B

        Dim d As DateTime = DateTime.Now

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "�d�������ɕs��������܂��B")
            Return False
        End If

        Log.Info(sMonitorMachineId, "�K�p���X�g�̃t�@�C������ [" & oReqTeleg.ListFileName & "] �ł��B")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "�}�X�^�{�̂̃t�@�C������ [" & oReqTeleg.DataFileName & "] �ł��B")
        Else
            Log.Info(sMonitorMachineId, "�}�X�^�{�̂̃t�@�C�����͂���܂���B")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "�K�p���X�g�̃t�@�C�������s���ł��B")
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
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�@�C�������s���ł��B")
                Return False
            End If
            If EkMasterDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j�ɕs����������܂��B")
                Return False
            End If
            If EkMasterDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�}�X�^��ʁj�ɕs����������܂��B")
                Return False
            End If
            If EkMasterDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�p�^�[��No�j�ɕs����������܂��B")
                Return False
            End If
            If EkMasterDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�}�X�^Ver�j�ɕs����������܂��B")
                Return False
            End If
        End If

        'NOTE: ��{�I�ɁAoReqTeleg��SubObjCode��ListFileName�̐������́A�V�i���I��
        '�ŕۏ؂���z��ł���B�܂��ADataFileName����łȂ��ꍇ��ListFileName�Ƃ�
        '�������ɂ��Ă��A�V�i���I���ŕۏ؂���z��ł���B�V�i���I���łȂ�A
        '�������̂Ȃ�REQ�ɑ΂��ANAK��ԐM����悤�ɐݒ�ł��邽�߂ł���B
        '�������A���Ƃ��A�V�i���I���Ń`�F�b�N���Ȃ��Ƃ��Ă��A���������A�������̂Ȃ�
        '�z�M���s���Ă��܂����Ȃ�A����͉^�ǃV�X�e���̕s��̂͂��ł��邩��A
        '�V�i���I�����{����NG�ŏI����������悢�B�ȏ�̂��Ƃ���A�����ł�
        '�������̃`�F�b�N���s���A���������Ȃ��ꍇ�́A�ے艞����ԋp����B
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j���s���ł��B")
            Return False
        End If
        If Not ExConstants.GateMastersSubObjCodes.ContainsKey(sDataKind) Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�}�X�^��ʁj���s���ł��B")
            Return False
        End If
        If oReqTeleg.SubObjCode <> ExConstants.GateMastersSubObjCodes(sDataKind) Then
            Log.Error(sMonitorMachineId, "�d���̃T�u��ʂ��t�@�C�����i�}�X�^��ʁj�Ɛ������Ă��܂���B")
            Return False
        End If

        '�Ď��Ղ��ێ����Ă��铖�Y��ʃ}�X�^�̃p�^�[���ԍ��ƃo�[�W�������擾����B
        'NOTE: �Ď��Ղ����sDataKind�̃}�X�^��ێ����Ă��Ȃ��Ƃ��́A
        'oHoldingMasters��Nothing�ɂȂ�Aholding0Version��holding1Version��0�ɂȂ�B
        'NOTE: EkMasProListFileName.IsValid()�ł̃`�F�b�N�ɂ���āA
        'dataVersion��0�ɂȂ邱�Ƃ͂��蓾�Ȃ����߁A��L�P�[�X�ł́A
        'holding0Version�����holding1Version���AdataVersion�ƈ�v
        '���邱�Ƃ͂��蓾�Ȃ��B
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
            Log.Warn(sMonitorMachineId, "���Y�@��ɂ͈قȂ�G���ANo�̒[�������݂��Ă��邽�߁A����Ȕ��f���s�����Ƃ��ł��܂���B�����ӂ��������B")
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
            Log.Error(sMonitorMachineId, "���̎�ʂ̃}�X�^�́A���Y�@��̒[���G���ANo�ł͎󂯕t���邱�Ƃ��ł��܂���B")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End If

        'OPT: ����̔z�M���@���ƁA�}�X�^DLL�V�[�P���X�œK�p���X�g�͕K����M�����A
        '������_�@�ɉ��D�@�ɔz�M���s���ꍇ�́A�ߋ��Ɏ�M�������ꖼ�̓K�p���X�g��
        '�L���ɂ�炸�A��M�����K�p���X�g���g�����ƂɂȂ邽�߁A
        'sListHashValue��listAcceptDate�͕K�v�Ȃ��B
        'oReqTeleg.ListFileHashValue��d�𒼐ڎQ�Ƃ���΂悢�B

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
            '�Ď��Ղ��ێ����Ă��Ȃ��}�X�^�Ɋւ��āA�K�p���X�g�݂̂𑗂�t����ꂽ�ꍇ�́A
            'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            'NOTE: ���̃P�[�X�Ŗ{���̊Ď��Ղ��ǂ̂悤�Ȕ������������́A�������Ă��Ȃ��B
            'NOTE: ���̏����́A���L�Ɠ����ł���B
            ' (holding0SubKind <> dataSubKind OrElse holding0Version <> dataVersion) AndAlso _
            ' (holding1SubKind <> dataSubKind OrElse holding1Version <> dataVersion)
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "�K�p���X�g�ɕR�Â��}�X�^�{�̂�����܂���B")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            If sDataHashValue IsNot Nothing Then
                If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.DataFileHashValue, sDataHashValue) <> 0 Then
                    Log.Warn(sMonitorMachineId, "���ݕێ����Ă���}�X�^�Ɠ��ꖼ�ł����A���e�i���n�b�V���l�j���Ⴄ���߁A�V�����}�X�^�Ƃ��ď������܂��B")
                    isDataUpdated = True
                End If
            End If
            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
        End If

        '��M�����K�p���X�g�̓��e���Ď��Ղ��ێ����Ă�����̂Ɣ�r����B
        '�K�p���X�g�͌X�ɈӖ�������A�o�[�W�������ȒP�Ɉꏄ���邽�߁A
        '���e����v���Ă��Ȃ��Ȃ�A�V�����K�p���X�g�Ƃ��Ĉ����B
        '���l�̗��R����A���e����v���Ă����Ƃ��Ă��A���炩��DL�����ʒm��
        '����������̂��R��ׂ��p�ł���Ǝv���邪�A�{���̊Ď��Ղ͎̂Ă�
        '�悤�Ɍ��󂯂��邽�߁A�̂Ă�iisNewList��False �ɂ���j���Ƃɂ���B
        '�Ȃ��A�󂯕t�����z�M��K������������ۏ؂��Ȃ���΁A���̂悤�Ɏ̂Ă�
        '�Ƃ����̂͗L�Q�ł���i�O��󂯕t�����z�M�ɑ΂��āA���ۂ̔z�M����߂��ꍇ�A
        '���Ƃ��u�z�M�ُ�v��DL�����ʒm���^�ǂɑ��M���Ă����Ƃ��Ă��A
        '�����K�p���X�g�ɂ�鍡��̗v�����̂Ă���΁A���[�U��
        '�Ӗ��s���ł���Ɗ����邩������Ȃ����A���������A�̂Ă�ꂽ
        '�̂��Ƃ������ƂɋC�t�����ɑ҂������邩������Ȃ��j�B
        'NOTE: �{���̊Ď��Ղ́A�n�b�V���l�ł͂Ȃ��A�K�p���X�g�̓��e���̂��̂�
        '��r����Ǝv����B�������A�����ꂻ���łȂ��A���A�n�b�V���l��
        '���R�̈�v�ɂ��s���Ɏ̂Ă邱�ƂɂȂ�ꍇ�ɁA���̂��Ƃ����n�[�T������
        '�������Ȃ��̂͊댯�ł��邽�߁A�V�~�����[�^�ł̓n�b�V���l���r���邱�Ƃɂ���B
        Dim isNewList As Boolean = True
        If sListHashValue IsNot Nothing AndAlso holdingListVersion = listVersion Then
            If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.ListFileHashValue, sListHashValue) <> 0 Then
                Log.Warn(sMonitorMachineId, "���ݕێ����Ă���K�p���X�g�Ɠ��ꖼ�ł����A���e�i���n�b�V���l�j���Ⴄ���߁A�V�����K�p���X�g�Ƃ��ď������܂��B")
            Else
                Log.Warn(sMonitorMachineId, "���ݕێ����Ă���K�p���X�g�Ɠ��ꖼ�œ��e�i���n�b�V���l�j������ł��邽�߁A����Ɋ�Â��[���ւ̔z�M�͔������܂���B")
                isNewList = False
            End If
        End If

        sListHashValue = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        '�z���̂����ꂩ�̉��D�@�ɑ΂��Ĕz�M�҂��̂��́iDL�����ʒm���쐬�̂��́j������ꍇ�A
        '�܂��́A�z���̑S���D�@�̓K�p���o�[�W�������Ď��Ղ̕ێ�(1)�܂��͕ێ�(2)�̂����ꂩ
        '����ɑ����Ă��Ȃ��󋵂ŁA�^�ǂ���V�����o�[�W������DLL��v�����ꂽ�ꍇ�́A
        'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
        'NOTE: �z���̉��D�@���u�K�p���Ă�����́v�܂��́u�K�p���邱�ƂɂȂ���́v
        '�i=�u�K�p�ς݁v�Ȃǂ�DL�����ʒm�̍쐬���K�v�ɂȂ���́j���A�Ď��Ղ̕ێ�����
        '������́i�Q�j�Ɍ��肳���󋵂��ێ����邱�Ƃ��ڕW�ł���B
        '�Ď��Ղ͉^�ǃT�[�o��DL�����ʒm�𑗐M�ł��Ȃ��󋵂ɂ����āA���炩�̕��@��
        'DL�����ʒm�̑��M��ۗ�����K�v�����邪�A���̂悤�ȃ��[���ɂ��邱�ƂŁA
        '�ۗ�����DL�����ʒm�����D�@�P�ʂōő�Q�Ɍ��肷��i�p�^�[���ԍ��ƃo�[�W����
        '���������̂́A�z�M���ʒl�ɗD�揇�ʂ�����Ȃǂ��ĂP�ɏ�ݍ��ށj���Ƃ�
        '�\�ɂȂ�̂�������Ȃ��B�܂��A���D�@�ɂ�����̂��Ď��Ղ��K���ێ�����Ȃ�A
        '�z���́i�ꕔ�́j���D�@���ێ����Ă�����̂ƃp�^�[���ԍ�����уo�[�W������
        '����i= ���ꖼ�j�ł���Ȃ�����e���قȂ���̂���M�����ꍇ�ɁA���̂��Ƃ�
        '���o���A�󂯓�������ۂ��邱�Ƃ��ł���i���ʂƂ��āA���ꖼ�ł���Ȃ���
        '���e�̈قȂ�}�X�^���A�z���̉��D�@�ɍ��݂���󋵂�h�����Ƃ��ł���j�B
        'TODO: �{���̊Ď��Ղɍ��킹�����B
        'NOTE: �{���̊Ď��Ղ́A���Ƃ�DL�����ʒm���^�ǂɑ��M�ł��Ă��Ȃ��󋵂ł�������
        '���Ă��A���̓��e���S�āu�K�p�ς݁v�ōςށi���͂�}�X�^���̂��̂�ێ����Ă���
        '�K�v���Ȃ��j�Ȃ�A���@�I�Ɏ󂯓�����s���ȂǁA���G�ȓ��������\��������B
        '�t�ɁA�����ł́A�ێ�(1)���݂Ȃ��i�ێ�(2)��K�p���Ă��Ȃ����D�@������󋵂�
        '�ێ�(2)�ƈႤ���̂���M�����ꍇ�ɁA�K���󂯓�������ۂ���j�悤�ɂ��邱�ƂŁA
        '�ۗ����Ă���DL�����ʒm���u�ێ�(2)�̎󂯓���O�ɍĎ󂯓��ꂵ�Ă����ێ�(1)��
        '�΂���K�p�ς݁v���u�ێ�(2)�ɑ΂��鐳���K�p�ς݁v�ɂȂ�i���ł��ێ�(1)��
        '�̂Ă邱�Ƃ��ł���j�悤�ɐ������A�ۗ����Ă���DL�����ʒm�̗L���Ɋ֌W�Ȃ��A
        '�i�ێ�(2)�̂��̂��S���D�@�ɓK�p����Ă�������΁A�����ێ�(1)�Ɉڂ��āj
        '�V���Ȃ��̂��󂯓���邱�Ƃ��ł���悤�A�P�������Ă���\��������B
        '���̏ꍇ�A���Ƃ��K�p���X�g�݂̂̎�M�ł����Ă��A���ꂪ�ێ�(2)�̃o�[�W������
        '��v���Ȃ��ꍇ�́A���̃��\�b�h�̏I���Ɂi�ێ�(2)�̂��̂�ێ�(1)�Ɉړ�����
        '����j��M�������̂�ێ�(2)�ɓ����i�}�X�^�{�̂ɂ��āA���Ƃ��ƕێ�(1)��
        '���������̂�ێ�(2)�Ɉڂ����̂Ƃ���j���ƂɂȂ�B�}�X�^�{�̂̎󂯓��������
        '��̂��̂ł͂Ȃ��A�ォ��z�M��v�����ꂽ���̂�ێ�(2)�Ɋi�[����킯�ł���B
        '����Ȃ�΁A�V���ȃo�[�W�����̃}�X�^����M�����ۂ��A�ێ�(1)�ɓ���邩
        '�ێ�(2)�ɓ���邩�Ŗʓ|�Ȕ���i���D�@�ɓK�p����Ă��Ȃ��̂͂ǂ���ł��邩��
        '����j�������ɍςށB�������A���ꂾ�ƁA�Ď��Ր݌v�҂��畷�����u�Ď��Ղ��ێ�
        '����}�X�^�̐���P�Ɛ���Q�ɂ́A�ǂ��炪�V�����Ƃ��������܂�͂Ȃ��v�Ƃ���
        '�v�z�ƈႤ�C������B����������ƁA�ڑ����������̍��̊Ď��Ղ́A���̂悤�Ȋ���
        '�������C�����邪�A�V�����}�X�^���ꕔ�̉��D�@�ɔz�M������Ԃɂ����āA�}�X�^
        '�o�[�W����������P�ȓK�p���X�g���󂯓���Ȃ��Ȃ邽�߁A���̃}�X�^�Ɍ�肪
        '�����Ă��A�S�Ẳ��D�@�ɔz�M���Ă���łȂ��ƁA�}�X�^�̃o�[�W�����߂���
        '�ł��Ȃ�...�Ƃ������ƂɂȂ�A���蓾�Ȃ��d�l�Ƃ�������������B
        'NOTE: ���������A�{���̊Ď��Ղ́A���ݕێ����Ă�����̂Ɠ����}�X�^�o�[�W������
        '�K�p���X�g����M�����ꍇ�ł����Ă��A�ߋ��Ɏ�M���������ʓK�p���X�g�i�}�X�^
        '�o�[�W�����͓���Ƃ͌���Ȃ��j�ɑΉ�����DL�����ʒm�𑗐M���Ă��Ȃ��ꍇ�́A
        'ContinueCode.FinishWithoutStoring��REQ�d�����쐬���邩������Ȃ��i������
        'DL�����ʒm���L���[�C���O�������Ȃ����̗��R�Łj�B
        'NOTE: ���̃A�v���ł́A�Ď��Ղ̕ێ�(1)���ێ�(2)���󂢂Ă���ꍇ�́A���Ƃ��z����
        '���D�@�ɖ��z�M�̂��̂����낤���A�z���̉��D�@�̓K�p�o�[�W�����������ĂȂ��낤���A
        '�����Ɏ󂯓�����s���B���Ƃ��΁A�ێ�(1)�ɉ��������Ă��āA�ێ�(2)���󂢂Ă���
        '�ꍇ�ɁA�u�ێ�(1)�Ɠ������̂�K�p���Ă�����D�@�v�Ɓu�����K�p���Ă��Ȃ����D�@�v
        '�����݂��Ă���󋵂ł����Ă��A�V���Ȃ��̂��P�����󂯓����悤�ɂȂ��Ă���B
        '�������A�{���̊Ď��Ղ͂����ł͂Ȃ���������Ȃ��B�{���̊Ď��Ղ́A���D�@���}�X�^
        '��ێ����Ă��Ȃ���Ԃ��A�o�[�W����0��ێ����Ă����ԂƂ݂Ȃ��A����ȏ�̍���
        '�������Ȃ��\��������B�����A�����I�ȗ��R�ł����Ȃ��Ă���Ƃ͍l���ɂ������A
        '�^�p�̖W���ɂ��Ȃ�̂ŁA�Ƃ肠�����A���̃A�v���ł́A���̂悤�Ȑ����͂������A
        '�R��ׂ�����������邱�Ƃɂ���B
        'NOTE: �{���̊Ď��Ղ́A���D�@�ɓK�p����Ă�����́i�K�p�����͂��̂��́j���A
        '�Ď��Ղ̕ێ����Ă��鐢��P�Ƃ�����Q�Ƃ��قȂ�ꍇ�́A�V���ȃ}�X�^����M����
        '�ꍇ�ł����Ă��A���̐���Ɏ󂯓�����\�ɂ���i�������́A�K�p���X�g�Ȃ��ł�
        '����̔��f�ŉ��D�@�ɔz�M���s���A���ꂪ��������܂ł̊��Ԃ����A�󂯓��������
        '����j�ȂǁA���ʂȔz�������Ă���\��������B
        'NOTE: �{���̊Ď��Ղ́A�p�^�[���ԍ����r���Ȃ���������Ȃ��B

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
            'OPT: isDataUpdated��True�̏󋵂ł́A�^�ǃT�[�o���K�p���X�g�ɋL�ڂ��ꂽ�[���Ɗ֌W�̂Ȃ�
            '�Ď��Ղɔz�M���s�����Ƃ������Ƃ��Ȃ�����AUpdate�O��Data���K�������ꂩ��TermMachine��
            'PendingMasters��HoldingMasters�Ɏ��[����Ă���͂��ł���A�������s���܂ł��Ȃ�
            '������FinishWithoutStoring��DL�����ʒm�𔭐������邱�Ƃ��ł���͂��ł���B

            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                Dim oQueue As LinkedList(Of PendingMaster) = Nothing
                If oTerm.PendingMasters.TryGetValue(sDataKind, oQueue) = True AndAlso _
                   oQueue.Count <> 0 Then
                    'TODO: ���̓d���Ƀp�^�[���ԍ����i�[����ꏊ���Ȃ��Ƃ������Ƃ́A
                    '�{���̊Ď��Ղ���D�@�́A�p�^�[���ԍ����قȂ���̂ł���΁A
                    '��ʂ�o�[�W����������ł����Ă��A�����ɕێ��ł���悤�ɂȂ���
                    '����̂�������Ȃ��B�������A�v���O�����z�M�̎���i�Ď��Ղ�
                    '���̔z���̉��D�@�̃G���A�ԍ����A�z�M���Ƃɕω����Ȃ����Ɓj��
                    '�D�悵�āA�d�l�����߂��Ă��邾��������Ȃ��B
                    Log.Error(sMonitorMachineId, "���Y��ʂ̃}�X�^�ɂ��āA�z�M�҂��iDL�����ʒm���쐬�j�̒[��������󋵂ŁA�V���Ȃ��̂���M���܂����B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                    Return True
                End If
            Next oTerm

            If isDataUpdated Then
                Debug.Assert(acceptableSlot <> -1)
                'TODO: �{���̊Ď��Ղ́A���Ɂu�S���́v�[���ɔz�M�����󋵂ł���΁A
                '����Ɠ��ꖼ�E�ʓ��e�̃}�X�^�ł����Ă��A�󂯓��ꂻ���ȋC������B
                For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                    Dim oMas As HoldingMaster = Nothing
                    If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = True AndAlso _
                       oMas.DataSubKind = dataSubKind AndAlso oMas.DataVersion <> DataVersion Then
                        Log.Error(sMonitorMachineId, "���Y��ʁE���Y�o�[�W�����̃}�X�^�ɂ��āA���Ɉꕔ�̒[���ɔz�M�����󋵂ŁA���e�i���n�b�V���l�j�̈قȂ���̂���M���܂����B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                        Return True
                    End If
                Next oTerm
            End If
        End If

        If acceptableSlot = -1 Then
            '�Ď��Ղ̕ێ�(2)�Ɠ���̂��̂��S�Ẳ��D�@�ɓK�p����Ă���ꍇ�́A�Ď��Ղ̕ێ�(1)�Ɏ󂯓���Ƃ���B
            acceptableSlot = 0
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                'NOTE: ���������s�����P�[�X�ł́A�uoHoldingMasters(1) Is Nothing�v�͂��蓾�Ȃ����߁A
                '�uholding1Version = 0�v�ł���Ƃ��Ă��A����́A�Ď��Ղ��o�[�W����0�̃}�X�^��ێ�
                '���Ă���Ƃ������Ƃł���B�����āA������uoMas.DataVersion = 0�v�ł���Ƃ���΁A
                '����́A���D�@���o�[�W����0�̃}�X�^��ێ����Ă���Ƃ������Ƃł���A�Ď��ՂƉ��D�@��
                '�o�[�W��������v���Ă���Ƃ݂Ȃ��Ă悢�B
                Dim oMas As HoldingMaster = Nothing
                If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = False OrElse _
                   holding1SubKind <> oMas.DataSubKind OrElse holding1Version <> oMas.DataVersion Then
                    acceptableSlot = -1
                    Exit For
                End If
            Next oTerm
        End If

        If acceptableSlot = -1 Then
            '�Ď��Ղ̕ێ�(1)�Ɠ���̂��̂��S�Ẳ��D�@�ɓK�p����Ă���ꍇ�́A�Ď��Ղ̕ێ�(2)�Ɏ󂯓���Ƃ���B
            acceptableSlot = 1
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                'NOTE: ���������s�����P�[�X�ł́A�uoHoldingMasters(0) Is Nothing�v�͂��蓾�Ȃ����߁A
                '�uholding0Version = 0�v�ł���Ƃ��Ă��A����́A�Ď��Ղ��o�[�W����0�̃}�X�^��ێ�
                '���Ă���Ƃ������Ƃł���B�����āA������uoMas.DataVersion = 0�v�ł���Ƃ���΁A
                '����́A���D�@���o�[�W����0�̃}�X�^��ێ����Ă���Ƃ������Ƃł���A�Ď��ՂƉ��D�@��
                '�o�[�W��������v���Ă���Ƃ݂Ȃ��Ă悢�B
                Dim oMas As HoldingMaster = Nothing
                If oTerm.HoldingMasters.TryGetValue(sDataKind, oMas) = False OrElse _
                   holding0SubKind <> oMas.DataSubKind OrElse holding0Version <> oMas.DataVersion Then
                    acceptableSlot = -1
                    Exit For
                End If
            Next oTerm
        End If

        If acceptableSlot = -1 Then
            Log.Error(sMonitorMachineId, "���Y��ʂ̃}�X�^�ɂ��āA�V���Ȋi�[�ꏊ�������i�ێ����Ă������̃}�X�^��S�[���ɓK�p���Ă��Ȃ��j�󋵂ŁA�V���Ȃ��̂���M���܂����B")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
            Return True
        End If

        '�K�p���X�g�̓��e����͂���B
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, Integer)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            '�Ƃ肠�����A�S�ēǂݎ��B
            'OPT: �u�K�p���X�g�̓��e��\������@�\�v��ǉ������ۂɁA�����葁��
            '�������邽�߂ɁA���̂悤�ɓ�x�ǂ݂��邱�ƂɂȂ��Ă���B
            '������ł���A���P�̗]�n������B
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '�P�s���ǂݎ��B
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '�ȉ��A�K�p���X�g�̓��e���`�F�b�N���A��肪����ꍇ�́A
                'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B

                '�w�b�_���̂P�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g����ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂P�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g1�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�쐬�N�������`�F�b�N����B
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�쐬�N�������s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '���X�gVer���`�F�b�N����B
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ���X�gVer���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g��2�s�ڂ�����܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g2�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�p�^�[��No���`�F�b�N����B
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�p�^�[��No���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�}�X�^Ver���`�F�b�N����B
                If Not dataVersion.ToString("D3").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�}�X�^Ver���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�@��R�[�h���`�F�b�N����B
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�킪�t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�K�p���X�g�̂R�s�ڈȍ~����AoMonitorMachine�Ɋ֌W���鍆�@�𒊏o����B
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '�ǂݍ��񂾍s���ɕ�������B
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 3 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��ڐ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�T�C�o�l����w���R�[�h�̏������`�F�b�N����B
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̉w�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�R�[�i�[�R�[�h�̏������`�F�b�N����B
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̃R�[�i�[�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '���@�ԍ��̏������`�F�b�N����B
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��@�ԍ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�s�̏d�����`�F�b�N����B
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ����o�̍s�Əd�����Ă��܂��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�s�̓��e���ꎞ�ۑ�����B
                    oListedMachines.Add(sLineKey, 0)

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�K�p���X�g�̓ǎ��ŗ�O���������܂����B", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End Try

        Dim dataAcceptDate As DateTime
        Dim oDataFooter As Byte()
      #If AcceptsSameNameMasOfSameHashValue Then
        If oReqTeleg.DataFileName.Length <> 0 Then
            dataAcceptDate = d

            '�}�X�^�{�̂̃t�b�^����ǎ��B
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
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�b�^���̓ǎ��ŗ�O���������܂����B", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            '�ǎ�����t�b�^���ɖ�肪����ꍇ�́A
            'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            Dim footerView As New ExMasterDataFooter(oDataFooter)
            Dim sViolation As String = footerView.GetFormatViolation()
            If sViolation IsNot nothing Then
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�b�^��񂪈ُ�ł��B" & vbCrLf & sViolation)
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

            '�}�X�^�{�̂̃t�b�^����ǎ��B
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
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�b�^���̓ǎ��ŗ�O���������܂����B", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            '�ǎ�����t�b�^���ɖ�肪����ꍇ�́A
            'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            Dim footerView As New ExMasterDataFooter(oDataFooter)
            Dim sViolation As String = footerView.GetFormatViolation()
            If sViolation IsNot nothing Then
                Log.Error(sMonitorMachineId, "�}�X�^�{�̂̃t�b�^��񂪈ُ�ł��B" & vbCrLf & sViolation)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        End If
      #End If

        If isNewList Then
            '�ꎞ�ۑ����Ă����s�������e�@��ɁA�z�M�̂��߂̏����L���[�C���O����B
            Dim targetTermCount As Integer = 0
            For Each sName As String In oListedMachines.Keys
                '�s��oMonitorMachine�Ɋ֌W����ꍇ
                Dim oTerm As TermMachine = Nothing
                If oMonitorMachine.TermMachines.TryGetValue(sName, oTerm) = True Then
                    Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & sName & "] �̍s���L���[�C���O���܂��B")
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
            Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ" & oListedMachines.Count.ToString() & "��̂����A" & targetTermCount.ToString() & "�䂪���Y�@��̒[���ł����B")

            'NOTE: ���L�̃P�[�X�ŁA�{���̊Ď��Ղ��ǂ̂悤�Ȕ������������́A�悭�킩��Ȃ��B
            '�������A���̂悤�ȕ��ނ̏���oMonitorMachine.HoldingMasters()�Ɋi�[����킯�ɂ͂����Ȃ��B
            '�z����TermMachine��PendingMasters�ɓo�^���Ă��Ȃ��Ƃ������Ƃ́A���ɉ^�ǂ���󂯓���\��
            '����o�[�W�������A����ɐ��������ۏ؂��Ȃ��Ƃ������Ƃł���B
            '���Q�̗L���͔����ł��邪�A�󂯓��ꋖ����}�X�^��Q�ɐ������Ȃ��ɂ�������炸�A
            'Q��oMonitorMachine.HoldingMasters()�ɓo�^����Ă����ԂƂ����̂́A����킵������B
            If targetTermCount = 0 Then
                Log.Error(sMonitorMachineId, "�z�M�𐶂ݏo���Ȃ��K�p���X�g����M���܂����B")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        End If

        If oReqTeleg.DataFileName.Length <> 0 Then
            '�Ď��Ղ̃}�X�^�ێ���Ԃ��X�V����B

            If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
                Log.Info(sMonitorMachineId, "�ێ�(1)�ɂ��铯��p�^�[���E����o�[�W�����̃}�X�^���㏑�����܂��B")
            ElseIf holding1SubKind = dataSubKind AndAlso holding1Version = dataVersion Then
                Log.Info(sMonitorMachineId, "�ێ�(2)�ɂ��铯��p�^�[���E����o�[�W�����̃}�X�^���㏑�����܂��B")
            ElseIf oHoldingMasters Is Nothing OrElse oHoldingMasters(0) Is Nothing Then
                Log.Info(sMonitorMachineId, "�ێ�(1)���󂢂Ă���̂ŁA���̋󂫂��g���ĐV���Ȃ��̂��󂯓���܂��B�{���̊Ď��Ղ����̂悤�ɓ��삷��Ƃ͌���܂���B")
            ElseIf oHoldingMasters(1) Is Nothing Then
                Log.Info(sMonitorMachineId, "�ێ�(2)���󂢂Ă���̂ŁA���̋󂫂��g���ĐV���Ȃ��̂��󂯓���܂��B�{���̊Ď��Ղ����̂悤�ɓ��삷��Ƃ͌���܂���B")
            ElseIf acceptableSlot = 0 Then
                Log.Info(sMonitorMachineId, "�V���Ȃ��̂��󂯓���邽�߂ɁA�z�M�҂��łȂ��[���ɂ��K�p���Ă��Ȃ��ێ�(1)�̃}�X�^���폜���܂��B")
            ElseIf acceptableSlot = 1 Then
                Log.Info(sMonitorMachineId, "�V���Ȃ��̂��󂯓���邽�߂ɁA�z�M�҂��łȂ��[���ɂ��K�p���Ă��Ȃ��ێ�(2)�̃}�X�^���폜���܂��B")
            End If

            If oHoldingMasters IsNot Nothing Then
                '����Q�ɉ�����ێ����Ă���󋵂ł���ɂ�������炸�A
                '����܂łƈႤ���̂������Ȃ萢��P�Ɏ󂯓����ꍇ�́A
                '����Q�ɂ�����̂𐢑�P�Ɉڂ��āA����Q�Ɏ󂯓����B
                'TODO: �{���̊Ď��Ղɍ��킹�����B�{���̊Ď��Ղ�
                '���̂悤�ɋÂ������Ƃ͂��Ȃ���������Ȃ��B
                If acceptableSlot = 0 AndAlso _
                   oHoldingMasters(1) IsNot Nothing AndAlso _
                  (holding0SubKind <> dataSubKind OrElse holding0Version <> dataVersion) Then
                    Log.Info(sMonitorMachineId, "�ێ�(2)�ɂ���}�X�^��ێ�(1)�Ɉړ����A�ێ�(2)�Ɏ󂯓���܂��B�{���̊Ď��Ղ����̂悤�ɓ��삷��Ƃ͌���܂���B")
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
        Log.Info(sMonitorMachineId, "�󂯓��ꂪ�������܂����B")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverGateMas(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '�S���D�@�̃L���[����S�Ẵ}�X�^�K�p���X�g�����o���A
        '�}�X�^�K�p���X�g���ƂɁA���Y���D�@(t)�̃}�X�^�ێ����
        '�ioMonitorMachine.TermMachines(t).HoldingMasters�j��
        '�X�V���AsMachineDir�Ƀ}�X�^�K�p���X�g�ʁE���D�@�ʂ�
        '#GateMasDlReflectReq_RRRSSSCCCCUU_N.dat�iN��0�`�j���쐬����B
        '�܂��A�}�X�^�ێ���Ԃ��X�V�������D�@�ɂ��ẮA
        'sContextDir��GateMasVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AGateMasDlReflectReq_RRRSSSCCCCUU_N.dat��
        'GateMasVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́A����X�V
        '���Ă��Ȃ����D�@�̂��́j���폜����B

        'NOTE: �����̃t�@�C������RRRSSSCCCCUU�͒[���̋@��R�[�h��
        '���邪�A�����̃t�@�C�����S�[�����i�V�i���I����
        '�u%T3R%T3S%T4C%T2U�v�ƋL�q�����ꍇ�ɕ��������S�s���j
        '�쐬�����Ƃ͌���Ȃ��B
        '����āA�V�i���I�́A���Y�t�@�C���𑗐M����ہA
        'ActiveOne�ł͂Ȃ��ATryActiveOne���g�p����B

        DeleteFiles(sMonitorMachineId, sContextDir, "GateMasVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now

        '�S�[���ɂ��ď������s���B
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
                Log.Debug(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɑ΂���}�X�^�z�M�͂���܂���B")
                Continue For
            ElseIf oTerm.McpStatusFromKsb <> &H0 Then
                Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ��ẮA�吧��Ԃ�����ȊO�ɐݒ肳��Ă��邽�߁A�z�M������ۗ����܂��B")
                Continue For
            End If

            Dim isHoldingMasUpdated As Boolean = False

            '�}�X�^��ʂ��Ƃɏ������s���B
            For Each oKindEntry As KeyValuePair(Of String, LinkedList(Of PendingMaster)) In oTerm.PendingMasters

                '���Y�Ď��Ղ��ێ����Ă��铖�Y��ʃ}�X�^�̃p�^�[���ԍ��ƃo�[�W�������擾����B
                'NOTE: ���Y�Ď��Ղ����oKindEntry.Key�̃}�X�^��ێ����Ă��Ȃ��Ƃ��́A
                'oHoldingMasters��Nothing�ɂȂ�Aholding0Version��holding1Version��0�ɂȂ�B
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

                'TODO: �{���̊Ď��Ղ́A�Ō�ɃL���[�C���O�������̂������D�@�ɑ��M���Ȃ���������Ȃ��B
                '����͎d���Ȃ��Ƃ��Ă��A�Ō�ɃL���[�C���O�����v���ɑΉ�����DL�����ʒm�������������Ȃ��\����
                '����i�v�����ꂽ���Ƃ��s���Ȃ��Ȃ�u�ُ�v��DL�����ʒm�𔭐�������΍ςނɂ�������炸�j�B
                '�����������Ƃ�����A�^�ǂɑ΂��铭�������@�ƃV�~�����[�^�ňႤ... �Ƃ������ƂɂȂ��Ă��܂��̂ŁA
                '�V�~�����[�^�ł��A�Ō�ɃL���[�C���O����Ă�����̈ȊO�͓ǂݎ̂Ă������悢��������Ȃ��B
                '�Ȃ��A���Ƃ���������ɂ��Ă��ATermMachine�N���X��PendingMasters�͕K�v�ł���B
                '�V�~�����[�^�̋@�\�Ƃ��āA���D�@�ɖ��z�M�̂��̂����[�U�Ɏ����K�v�����邽�߂ł���B

                '�z�M�����Ɏg���Ă��Ȃ��S�K�p���X�g�ɂ��ď������s���B
                For Each oPenMas As PendingMaster In oKindEntry.Value
                    'NOTE: ���̂̂Ȃ��iListHashValue Is Nothing �́j�K�p���X�g�Ŕz�M���s����\���͑z�肵�Ȃ��B
                    Log.Info(sMonitorMachineId, "�K�p���X�g [" & oPenMas.ListVersion.ToString() & "] �Ɋ�Â��A�[�� [" & sTermMachineId & "] �ɑ΂����� [" & oKindEntry.Key & "] �p�^�[��No [" & oPenMas.DataSubKind.ToString() & "] �}�X�^Ver [" & oPenMas.DataVersion.ToString() & "] �̃}�X�^�z�M�������s���܂�...")

                    '�z�M���ʁi�u����v�܂��́u�K�p�ς݁v�j�����߂�B
                    'TODO: �{���̊Ď��Ղɍ��킹�����B
                    '�v���g�R���d�l�Ō��߂��Ă���킯�ł͂Ȃ����A�{���̊Ď��Ղ́A
                    '���Ƃ��p�^�[���ԍ���}�X�^�o�[�W��������v���Ă����Ƃ��Ă��A
                    '�i���D�@�ɂ�����̂��A�Ď��Ղ���󂯎�������̂ł͂Ȃ����Łj
                    '�f�[�^�̓��e���s��v�ł���΁A���D�@�ɔz�M���Ȃ����āA
                    '�z�M���ʂ��u����v�Ƃ������ȋC������̂ŁA�������Ă��邪�A
                    '���͈Ⴄ��������Ȃ��B
                    'TODO: �^�ǃT�[�o�̎����́A�����`����Ă����w���@��̎d�l�ɍ��킹�Ă���A
                    '���̎����Ɛ������Ă��Ȃ��B��̓I�ɂ́u���Ƃ��p�^�[���ԍ��̈قȂ�}�X�^��
                    '�Ď��@��܂�DLL����Ă��A�}�X�^�o�[�W����������ł������A�[���@���
                    '������Ď��@�킩��擾���Ȃ��v�Ƃ����d�l�ɍ��킹�A�}�X�^�o�[�W������
                    '����ł������́A�[���@��̎�M��Ԃ��u�z�M���v�ɕύX���Ȃ��B
                    '�ւ��Ɂu�z�M���v�̃��R�[�h���쐬���āA���ꂪ�c����͂悢���A
                    '�����̏ꍇ�͂ǂ��Ȃ̂����܂߁A���ۂ̗��z�`���m�F����ׂ��ł���B
                    Dim deliveryResult As Byte = &H0
                    Dim isOutOfArea As Boolean = False

                    If deliveryResult = &H0 Then
                        Dim ar As Integer = DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer)
                        Dim oAreaSpec As ExAreaSpec = Nothing
                        If ExConstants.GateAreasSpecs.TryGetValue(ar, oAreaSpec) = False OrElse _
                           Not oAreaSpec.GateReadyGateMasters.Contains(oKindEntry.Key) Then
                            'NOTE: �z�M���Ȃ��̂Ɂu�K�p�ς݁v�͕s�K�؂Ɏv���邪�A���ۂɂ��̂悤�ɓ��삷��B
                            Log.Error(sMonitorMachineId, "���̎�ʂ̃}�X�^�̓G���A [" & ar.ToString() &"] �̒[���ɂ͔z�M�ł��܂���B")
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
                            '���D�@���ێ����Ă�����̂Ɠ������̂�z�M���邱�ƂɂȂ�ꍇ�́A
                            '�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                            Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�}�X�^��K�p�ς݁i���z�M�ς݁j�ł��B�Ĕz�M�͍s���܂���B")
                            deliveryResult = &HF
                        End If
                    End If

                    If deliveryResult = &H0 Then
                        If (oPenMas.DataSubKind <> holding0SubKind OrElse oPenMas.DataVersion <> holding0Version) AndAlso _
                           (oPenMas.DataSubKind <> holding1SubKind OrElse oPenMas.DataVersion <> holding1Version) Then
                            Log.Error(sMonitorMachineId, "�z��O�̏󋵂ł��B�z�M���Ȃ���΂Ȃ�Ȃ��}�X�^�����ɊĎ��Ղɂ���܂���B")
                            deliveryResult = &H5 'NOTE: �K���ȃR�[�h���Ȃ��̂ŁA�Ƃ肠��������ȊO�ɂ��Ă����B
                        End If
                    End If

                    '���D�@�̃}�X�^�ێ���Ԃ��X�V����B
                    If deliveryResult = &H0 OrElse isOutOfArea Then
                        'NOTE: ���D�@�͓K�p���X�g��ێ����Ȃ����A�ǂ̓K�p���X�g�̎w���ɂ����
                        '���Y���D�@�Ƀ}�X�^�{�̂̔z�M���s��ꂽ��������������悢�̂ŁA
                        '�K�p���X�g�o�[�W�������Z�b�g���邱�Ƃɂ���B
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
                            Log.Info(sMonitorMachineId, "���Y�[���ɑ΂��ē��Y�}�X�^�̔z�M���s���܂����B")
                        Else
                            Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��ē��Y�}�X�^�̔z�M���s���܂����B����͎������D�@�V�X�e���̐�������o�[�W���������Č������邽�߂̓��ʑ[�u�ł��̂ŁA�����ӂ��������B")
                        End If
                    End If

                    '#GateMasDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
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
            Log.Error("�v���O�����{�̂̃n�b�V���l�Z�o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim content As GateProgramContent
        Try
            content = ExtractGateProgramCab(sFilePath, Path.Combine(sContextDir, "GatePro"))
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̃G���ANo�̒��o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̑�\Ver�̒��o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        'TODO: �������D�@�V�X�e���ł́A�Ď��Քz���̉��D�@�́A�S�ē���G���A�ɏ������Ă���͂��Ȃ̂ŁA
        '�����ŁA���ނ̃G���A�ƊĎ��Ղ��Ǘ����Ă�����D�@�G���A�̐������`�F�b�N���s�����Ƃ��\�Ǝv����B
        '�������@���`�F�b�N���s���Ȃ�A����ɍ��킹�������悢�B
        '�����炭�A�������D�@��HW���̂́A�ǂ̃G���A�̉��D�@�v���O�������C���X�g�[���\�ł���A
        '���ړ����ɂ����Ă܂ł����W���邱�Ƃ͖����Ǝv���邪�B

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("�v���O�����̓��싖�����������ɐݒ肳��Ă��܂��B" & content.RunnableDate & "�܂œK�p�ł��܂���̂ł����ӂ��������B")
        End If

        InstallGateProgramDirectly(sContextDir, subKind, version, content, sHashValue)

        Return True
    End Function

    Protected Function AcceptGatePro(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDir��#PassiveDllReq.dat�������t�@�C�������Ƃ�
        '�Ď��Ղ̉��D�@�����v���O�����ێ���ԁioMonitorMachine.HoldingPrograms�j���X�V���A
        'sContextDir��ExtOutput.dat���쐬����B
        '�������A�{���̊Ď��ՂƓ����悤�ɁA���ꂩ�̉��D�@�̕ێ��o�[�W����
        '���A�Ď��Ղ̕ێ��o�[�W�����i�O���M�o�[�W�����j�ƈقȂ�
        '�ꍇ�́A�Ď��Ղ̉��D�@�����v���O�����ێ���Ԃ��X�V�����ɁA
        'ContinueCode��FinishWithoutStoring��ExtOutput.dat���쐬����B

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "�d�������ɕs��������܂��B")
            Return False
        End If

        Log.Info(sMonitorMachineId, "�K�p���X�g�̃t�@�C������ [" & oReqTeleg.ListFileName & "] �ł��B")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "�v���O�����{�̂̃t�@�C������ [" & oReqTeleg.DataFileName & "] �ł��B")
        Else
            Log.Info(sMonitorMachineId, "�v���O�����{�̂̃t�@�C�����͂���܂���B")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "�K�p���X�g�̃t�@�C�������s���ł��B")
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
                Log.Error(sMonitorMachineId, "�v���O�����{�̂̃t�@�C�������s���ł��B")
                Return False
            End If
            If EkProgramDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�v���O������ʁj�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�G���ANo�j�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i��\Ver�j�ɕs����������܂��B")
                Return False
            End If
        End If

        'NOTE: ��{�I�ɁAoReqTeleg��SubObjCode��ListFileName�̐������́A�V�i���I��
        '�ŕۏ؂���z��ł���B�܂��ADataFileName����łȂ��ꍇ��ListFileName�Ƃ�
        '�������ɂ��Ă��A�V�i���I���ŕۏ؂���z��ł���B�V�i���I���łȂ�A
        '�������̂Ȃ�REQ�ɑ΂��ANAK��ԐM����悤�ɐݒ�ł��邽�߂ł���B
        '�������A���Ƃ��A�V�i���I���Ń`�F�b�N���Ȃ��Ƃ��Ă��A���������A�������̂Ȃ�
        '�z�M���s���Ă��܂����Ȃ�A����͉^�ǃV�X�e���̕s��̂͂��ł��邩��A
        '�V�i���I�����{����NG�ŏI����������悢�B�ȏ�̂��Ƃ���A�����ł�
        '�������̃`�F�b�N���s���A���������Ȃ��ꍇ�́A�ے艞����ԋp����B
        If sApplicableModel <> Config.TermModelSym Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j���s���ł��B")
            Return False
        End If
        If sDataKind <> "GPG" Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�v���O������ʁj���s���ł��B")
            Return False
        End If
        If oReqTeleg.SubObjCode <> &H0 Then
            Log.Error(sMonitorMachineId, "�d���̃T�u��ʂ��s���ł��B")
            Return False
        End If

        'NOTE: �Ď��Ղ����D�@�����v���O������ێ����Ă��Ȃ��Ƃ��́A
        'holding0Version��holding1Version��0�ɂȂ�B
        'NOTE: EkMasProListFileName.IsValid()�ł̃`�F�b�N�ɂ���āA
        'dataVersion��0�ɂȂ邱�Ƃ͂��蓾�Ȃ����߁A��L�P�[�X�ł́A
        'holding0Version�����holding1Version���AdataVersion�ƈ�v
        '���邱�Ƃ͂��蓾�Ȃ��B
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
            '�Ď��Ղ��ێ����Ă��Ȃ��o�[�W�����̉��D�@�����v���O�����Ɋւ��āA�K�p���X�g�݂̂�
            '����t����ꂽ�ꍇ�́AContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            'NOTE: ���̃P�[�X�Ŗ{���̊Ď��Ղ��ǂ̂悤�Ȕ������������́A�������Ă��Ȃ��B
            'NOTE: ���̏����́A���L�Ɠ����ł���B
            ' (holding0SubKind <> dataSubKind OrElse holding0Version <> dataVersion) AndAlso _
            ' (holding1SubKind <> dataSubKind OrElse holding1Version <> dataVersion)
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "�K�p���X�g�ɕR�Â��v���O�����{�̂�����܂���B")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            If sDataHashValue IsNot Nothing Then
                'NOTE: �����z�M���s�\�ɂȂ�Ȃ��悤�A��M�f�[�^�����ꖼ�ł����Ă�
                '������e�ł���΁A���̌�ŕێ��f�[�^���㏑������z��ł���B
                If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.DataFileHashValue, sDataHashValue) <> 0 Then
                    Log.Warn(sMonitorMachineId, "���ݕێ����Ă���v���O�����Ɠ��ꖼ�ł����A���e�i���n�b�V���l�j���Ⴄ���߁A�V�����v���O�����Ƃ��ď������܂��B")
                    'isDataUpdated = True
                End If
            End If
            sDataHashValue = oReqTeleg.DataFileHashValue.ToUpperInvariant()
        End If


        '��M�����K�p���X�g�̓��e���Ď��Ղ��ێ����Ă�����̂Ɣ�r����B
        '�K�p���X�g�͌X�ɈӖ�������A�o�[�W�������ȒP�Ɉꏄ���邽�߁A
        '���e����v���Ă��Ȃ��Ȃ�A�V�����K�p���X�g�Ƃ��Ĉ����B
        '���l�̗��R����A���e����v���Ă����Ƃ��Ă��A���炩��DL�����ʒm��
        '����������̂��R��ׂ��p�ł���Ǝv���邪�A�{���̊Ď��Ղ͎̂Ă�
        '�悤�Ɍ��󂯂��邽�߁A�̂Ă�iisNewList��False �ɂ���j���Ƃɂ���B
        '�Ȃ��A�󂯕t�����z�M��K������������ۏ؂��Ȃ���΁A���̂悤�Ɏ̂Ă�
        '�Ƃ����̂͗L�Q�ł���i�O��󂯕t�����z�M�ɑ΂��āA���ۂ̔z�M����߂��ꍇ�A
        '���Ƃ��u�z�M�ُ�v��DL�����ʒm���^�ǂɑ��M���Ă����Ƃ��Ă��A
        '�����K�p���X�g�ɂ�鍡��̗v�����̂Ă���΁A���[�U��
        '�Ӗ��s���ł���Ɗ����邩������Ȃ����A���������A�̂Ă�ꂽ
        '�̂��Ƃ������ƂɋC�t�����ɑ҂������邩������Ȃ��j�B
        '�� ���̂悤�ȓK�p���X�g���̂ĂĂ��܂��ƁA�K�p���X�g�Ɋւ���
        '�u�K�p�ς݁v��DL�����ʒm�𔭐������邱�Ƃ��ł��Ȃ��Ȃ邽�߁A
        '���̒i�K�ł͎̂ĂȂ����Ƃɂ���B���̒i�K�Ŏ̂ĂȂ��Ƃ����Ă��A
        'DeliverGatePro�ɂ����āAlistDeliveryResult��&HF�ɂȂ邱�Ƃ�
        '�f�[�^�{�̂�[���֔z�M������͂��������߁A�K�p���X�g��DL������
        '����������Ƃ������Ƃ������āA�}�X�^�K�p���X�g�̏ꍇ�Ƒ卷�͂Ȃ��B
        'NOTE: �{���̊Ď��Ղ́A�n�b�V���l�ł͂Ȃ��A�K�p���X�g�̓��e���̂��̂�
        '��r����Ǝv����B�������A�����ꂻ���łȂ��A���A�n�b�V���l��
        '���R�̈�v�ɂ��s���Ɏ̂Ă邱�ƂɂȂ�ꍇ�ɁA���̂��Ƃ����n�[�T������
        '�������Ȃ��̂͊댯�ł��邽�߁A�V�~�����[�^�ł̓n�b�V���l���r���邱�Ƃɂ���B
        'Dim isNewList As Boolean = True
        If sListHashValue IsNot Nothing AndAlso holdingListVersion = listVersion Then
            If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.ListFileHashValue, sListHashValue) <> 0 Then
                Log.Warn(sMonitorMachineId, "���ݕێ����Ă���K�p���X�g�Ɠ��ꖼ�ł����A���e�i���n�b�V���l�j���Ⴄ���߁A�V�����K�p���X�g�Ƃ��ď������܂��B")
            Else
                Log.Warn(sMonitorMachineId, "���ݕێ����Ă���K�p���X�g�Ɠ��ꖼ�œ��e�i���n�b�V���l�j������ł��邽�߁A����Ɋ�Â��[���ւ̔z�M�͔������܂���B")
                'isNewList = False
            End If
        End If

        sListHashValue = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        'NOTE: �ȉ��̐���́A���S�Ȑ����ł���A�{���̊Ď��ՂƈႤ�\��������B
        '�������A���̂悤�ɍ��@�ʂ̔��f���o�āA�S�̂̔��f�����Ȃ�����A
        '�P�[�X�����U���Ă��܂��A�Ó��Ȑ���͕s�\�Ǝv����B
        With Nothing
            '�z���̉��D�@���ƂɁA���Ɏ�M�\�ȃo�[�W�����𒲂ׂāA
            '����������Ȃ�A���̃��X�g�ɒǉ�����B
            Dim oRestrictions As New List(Of MasProId)
            For Each oTerm As TermMachine In oMonitorMachine.TermMachines.Values
                Dim appliedPro As New MasProId(oTerm.HoldingPrograms(0).DataSubKind, oTerm.HoldingPrograms(0).DataVersion, oTerm.HoldingPrograms(0).DataHashValue)

                '�K�p���̂��̂ƈႤ���́iDL�����ʒm���K�p�ς݈ȊO�ɂȂ���́j��
                '�K�p�҂��ɂȂ��Ă���ꍇ�́A����Ɠ������̂������󂯓���\�Ƃ݂Ȃ��B
                If oTerm.HoldingPrograms(1) IsNot Nothing Then
                    Dim reservedPro As New MasProId(oTerm.HoldingPrograms(1).DataSubKind, oTerm.HoldingPrograms(1).DataVersion, oTerm.HoldingPrograms(1).DataHashValue)
                    If reservedPro <> appliedPro Then
                        oRestrictions.Add(reservedPro)
                        'NOTE: �{���́A�z�M��Pending����Ă���v���O�������`�F�b�N���������悢���A
                        '�s���Ȕ}�̓��������Ȃ�����́AreservedPro�Ɠ����ł���͂��ł��邽�߁A
                        '�ȗ�����B
                        Continue For
                    End If
                End If

                '�K�p���̂��̂ƈႤ���́iDL�����ʒm���K�p�ς݈ȊO�ɂȂ���́j��
                '�z�M�҂��ɂȂ��Ă���ꍇ�́A����Ɠ������̂������󂯓���\�Ƃ݂Ȃ��B
                'TODO: ApplicableDate��"99999999"��PendingProgram�����݂���ꍇ�́A
                '����ȍ~��PendingProgram���݂�ׂ���������Ȃ��B
                For Each oPenPro As PendingProgram In oTerm.PendingPrograms
                    Dim pendingPro As New MasProId(oPenPro.DataSubKind, oPenPro.DataVersion, oPenPro.DataHashValue)
                    If pendingPro <> appliedPro Then
                        oRestrictions.Add(pendingPro)
                        'NOTE: �{���́A�����Pending����Ă���v���O�������`�F�b�N���������悢���A
                        '�s���Ȕ}�̓��������Ȃ�����́ApendingPro�Ɠ����ł���͂��ł��邽�߁A
                        '�ȗ�����B
                        Exit For
                    End If
                Next oPenPro
            Next oTerm

            '��M�����v���O�������A�����ꂩ�̉��D�@�ɂƂ��Ď󂯓���\�Ȃ��̂ƈႤ
            '�ꍇ�́AContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            'NOTE: �Ď��Ղ��ێ��ł�����D�@�����v���O�����́A�u�z���̑S���D�@���ʂŁv
            '�Q����ƂȂ��Ă��邽�߁A���̂悤�Ȏc�O�Ȑ���ƂȂ�B���̐���ɂ���āA
            '�z���̑S���D�@�ɂ��āA�z�M�҂��v���O������K�p�҂��v���O�����́A�K��
            '�������̂ɂȂ�B�܂��A�z���̉��D�@�œK�p���ƂȂ�v���O�����́A������
            '�Q��ނɐ��������B
            If oReqTeleg.DataFileName.Length = 0 Then
                For Each pro As MasProId In oRestrictions
                    If dataSubKind <> pro.DataSubKind OrElse dataVersion <> pro.DataVersion
                        Log.Error(sMonitorMachineId, "��s���Ď󂯓��ꂽ�v���O������S�[���ɓK�p����i�܂��͎̂Ă�j�܂ŁA�V���ȃv���O�����̎󂯓���͂ł��܂���B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                        Return True
                    End If
                Next pro
            Else
                For Each pro As MasProId In oRestrictions
                    If dataSubKind <> pro.DataSubKind OrElse dataVersion <> pro.DataVersion OrElse _
                       StringComparer.OrdinalIgnoreCase.Compare(sDataHashValue, pro.DataHashValue) <> 0 Then
                        Log.Error(sMonitorMachineId, "��s���Ď󂯓��ꂽ�v���O������S�[���ɓK�p����i�܂��͎̂Ă�j�܂ŁA�V���ȃv���O�����̎󂯓���͂ł��܂���B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
                        Return True
                    End If
                Next pro
            End If
        End With

        'NOTE: �K�p���X�g�ɋL�ڂ��ꂽ�����ꂩ�̊Ǘ��Ώۍ��@�ւ̔z�M��
        '�������Ă��Ȃ���ԂŁA�}�X�^�{�̂��Y�t���ꂽDLL��v�����ꂽ�ꍇ�́A
        '���Ƃ��A���̃p�^�[���ԍ���}�X�^�o�[�W�������A�Ď��Ղ̕ێ����Ă���
        '�O���M�o�[�W�����Ɠ���ł����Ă��A������󂯓����ׂ��ł͂Ȃ�
        '�i�Ď��Ղ̕ێ����Ă�����̂������ւ���ׂ��ł͂Ȃ��j�Ǝv����B
        '�Ȃ��Ȃ�A�u�V���ȃ}�X�^�{�̂��󂯎��O�ɗv�����ꂽ�z�M�́A
        '���̎��_�ŊĎ��Ղ��ێ����Ă����}�X�^�{�̂��Ȃčs���ׂ��v��
        '�l�����邽�߂ł���B�������A�v���O�����̏ꍇ�́A
        '��sDLL���ꂽCAB�Ɓi�o�[�W�����������Łj���e���ႤCAB��
        '�󂯓���Ȃ��悤�ɂ��Ă���̂ŁA���̂悤�Ȑ���͕s�v�ł���B

        '�K�p���X�g�̓��e����͂���B
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, String)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            '�Ƃ肠�����A�S�ēǂݎ��B
            'OPT: �u�K�p���X�g�̓��e��\������@�\�v��ǉ������ۂɁA�����葁��
            '�������邽�߂ɁA���̂悤�ɓ�x�ǂ݂��邱�ƂɂȂ��Ă���B
            '������ł���A���P�̗]�n������B
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '�P�s���ǂݎ��B
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '�ȉ��A�K�p���X�g�̓��e���`�F�b�N���A��肪����ꍇ�́A
                'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B

                '�w�b�_���̂P�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g����ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂P�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g1�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�쐬�N�������`�F�b�N����B
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�쐬�N�������s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '���X�gVer���`�F�b�N����B
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ���X�gVer���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g��2�s�ڂ�����܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g2�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�G���ANo���`�F�b�N����B
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�G���ANo���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '��\Ver���`�F�b�N����B
                If Not dataVersion.ToString("D8").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ��\Ver���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�@��R�[�h���`�F�b�N����B
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�킪�t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�K�p���X�g�̂R�s�ڈȍ~����AoMonitorMachine�Ɋ֌W���鍆�@�𒊏o����B
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '�ǂݍ��񂾍s���ɕ�������B
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 4 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��ڐ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�T�C�o�l����w���R�[�h�̏������`�F�b�N����B
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̉w�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�R�[�i�[�R�[�h�̏������`�F�b�N����B
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̃R�[�i�[�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '���@�ԍ��̏������`�F�b�N����B
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��@�ԍ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�K�p���̃����O�X���`�F�b�N����B
                    If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�K�p�����u�����N�łȂ��ꍇ�A�l���`�F�b�N����B
                    If aColumns(3).Length = 8 Then
                       If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If

                        Dim applicableDate As DateTime
                        If Not aColumns(3).Equals("99999999") AndAlso _
                           DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, applicableDate) = False Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    '�s�̏d�����`�F�b�N����B
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ����o�̍s�Əd�����Ă��܂��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�s�̓��e���ꎞ�ۑ�����B
                    oListedMachines.Add(sLineKey, aColumns(3))

                    '�s��oMonitorMachine�Ɋ֌W����ꍇ
                    Dim oTerm As TermMachine = Nothing
                    If oMonitorMachine.TermMachines.TryGetValue(sLineKey, oTerm) = True Then
                        '�G���A�ԍ����`�F�b�N����B
                        'NOTE: ���D�@�̃v���O�����ɃG���A�ԍ�0���w�肳��邱�Ƃ͂Ȃ��i�ُ펖�ԁj�Ƃ����O��ł���B
                        If dataSubKind <> DirectCast(oTerm.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer) Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & sLineKey & "] �̏����G���A���A�K�p���X�g�̑ΏۃG���A�ƈقȂ�܂��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�K�p���X�g�̓ǎ��ŗ�O���������܂����B", ex)
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End Try

        Dim dataAcceptDate As DateTime
        Dim sRunnableDate As String
        Dim oModuleInfos As ProgramModuleInfo()
        Dim sArchiveCatalog As String
        Dim oVersionListData As Byte()

      #If AcceptsSameNameProOfSameHashValue Then
        'NOTE: �^�ǂ���Ď��Ղւ̋����z�M�ɑΉ������Ď��Ղ́A���炭���̂悤�ȓ��������Ǝv����B
        'TODO: ���������{���̊Ď��Ղ��v���O�����̋����z�M�ɑΉ����Ă���̂��m�F���������悢�B
        If oReqTeleg.DataFileName.Length <> 0 Then
            dataAcceptDate = d

            Dim content As GateProgramContent
            Try
                Dim sDataFilePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.DataFileName)
                content = ExtractGateProgramCab(sDataFilePath, Path.Combine(sContextDir, "GatePro"))
            Catch ex As Exception
                Log.Error(sMonitorMachineId, "�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̊Ď��Ղ̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ�G���ANo���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̊Ď��Ղ̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ��\Ver���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
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
        'NOTE: �^�ǂ���Ď��Ղւ̋����z�M�ɑΉ����Ă��Ȃ��������̊Ď��Ղ́A
        '���炭���̂悤�ȓ�������Ă����Ǝv����B
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
                Log.Error(sMonitorMachineId, "�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̊Ď��Ղ̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ�G���ANo���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̊Ď��Ղ̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ��\Ver���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
            End If

            sRunnableDate = content.RunnableDate
            oModuleInfos = content.ModuleInfos
            sArchiveCatalog = content.ArchiveCatalog
            oVersionListData = content.VersionListData
        End If
      #End If

        'If isNewList Then
        If True Then
            '�ꎞ�ۑ����Ă����s�������e�@��ɁA�z�M�̂��߂̏����L���[�C���O����B
            Dim targetTermCount As Integer = 0
            Dim targetTermFullCount As Integer = 0
            For Each oApplyEntry As KeyValuePair(Of String, String) In oListedMachines
                '�s��oMonitorMachine�Ɋ֌W����ꍇ
                Dim oTerm As TermMachine = Nothing
                If oMonitorMachine.TermMachines.TryGetValue(oApplyEntry.Key, oTerm) = True Then
                    '�K�p�������݂̉^�p���t�Ɠ������������邢�́u19000101�v���u99999999�v�̏ꍇ�̂݁A
                    '�z�M���i���̃A�v���̏ꍇ�́ADL�����ʒm���j�K�v�Ƃ݂Ȃ��B
                    'NOTE: �{���̊Ď��Ղ́A���̏����ɊY�����Ă��Ȃ��s�ɂ��āA�u�K�p�ς݁v��DL�����ʒm��
                    '����t���Ă����悤�ȋC������i���̂��߂ɁA�^�Ǒ��́A���Ɂu����v�ɂȂ��Ă���ꍇ��
                    '�u�K�p�ς݁v��DL�����ʒm�𖳎����Ȃ���΂Ȃ�Ȃ��Ȃ����j�B
                    '����āA�ŐV�̊Ď��Ղ́A���������K�p�����ߋ����̍s�ł����Ă��A���Y�s�̉��D�@��
                    '���Y�v���O�����𖢔z�M�ł���΁A�z�M���Ă��܂��̂�������Ȃ��B
                    '�����������Ƃ���ƁA���Ȃ���ł���B
                    '�^�ǂ́A�K�p�����ߋ����̍s�́A�K�p�����u�����N�̍s�Ɠ��������ɂ��邱�ƂɂȂ��Ă���B
                    '����䂦�ɁA�K�p���X�g�ɂ��̂悤�ȍs�����Ȃ���΁A�Ď��Ղɑ΂��Ĕz�M���Ȃ��B
                    '�܂��ADLL�V�[�P���X�����������ہi�Ď��Ղ܂Ŕz�M�����������ہj���A���̂悤�ȓK�p����
                    '�L�ڂ���Ă�����D�@�ɂ��ẮA�z�M��Ԃ��u�z�M���v�ɂ͂��Ȃ��B
                    '�^�ǂ̓����I/F�d�l�i�c�[���d�l���̕ʎ�6�j�Ɋ��S�ɍ��v���Ă���B
                    'TODO: �{���̊Ď��Ղ��u�K�p�ς݁v�𑗂�t���Ă��錏�ɂ��āA�V�X�e�������ł́A�����
                    '��������悤�ɉ^�Ǒ����������A�Ď��Ճ`�[���̍l����d�l�ʂ�Ƃ������Ƃ�OK�Ƃ������A
                    '�Ď��Ղ̎������ǂ��Ȃ��Ă���̂��A�V�X�e���Ƃ��Ė�肪�Ȃ��̂��A�V�X�e��������
                    '���{���Ă��Ȃ��P�[�X�i�K�p�����ߋ����̍s�̉��D�@�ɑ΂��Ė��z�M�������P�[�X���j��
                    '���Ă��A�^�ǂ̓�����܂߂āA���؂���ׂ��ł���B
                    If oApplyEntry.Value.Length = 8 AndAlso _
                      (oApplyEntry.Value.Equals("19000101") OrElse _
                       String.CompareOrdinal(oApplyEntry.Value, sServiceDate) >= 0) Then
                        Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & oApplyEntry.Key & "] �K�p�� [" & oApplyEntry.Value & "] �̍s���L���[�C���O���܂��B")
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
                        Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�[�� [" & oApplyEntry.Key & "] �K�p�� [" & oApplyEntry.Value & "] �̍s�͏��O���܂��B")
                    End If
                    targetTermFullCount += 1
                End If
            Next oApplyEntry
            Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ" & oListedMachines.Count.ToString() & "��̂����A" & targetTermFullCount.ToString() & "�䂪���Y�@��̒[���ł����B���̂���" & targetTermCount.ToString() & "��̓K�p�����L���ł����B")

            'NOTE: ���L�̃P�[�X�ŁA�{���̊Ď��Ղ��ǂ̂悤�Ȕ������������́A�悭�킩��Ȃ��B
            '�������A���̂悤�ȕ��ނ̏���oMonitorMachine.HoldingPrograms(1)�Ɋi�[����킯�ɂ͂����Ȃ��B
            '�z����TermMachine��PendingPrograms�ɓo�^���Ă��Ȃ��Ƃ������Ƃ́A���ɉ^�ǂ���󂯓���\��
            '����o�[�W�������A����ɐ��������ۏ؂��Ȃ��Ƃ������Ƃł���B
            '���Q�̗L���͔����ł��邪�A�󂯓��ꋖ����v���O������Q�ɐ������Ȃ��ɂ�������炸�A
            'Q��oMonitorMachine.HoldingPrograms(1)�ɓo�^����Ă����ԂƂ����̂́A����킵������B
            If targetTermCount = 0 Then
                Log.Error(sMonitorMachineId, "�z�M�𐶂ݏo���Ȃ��K�p���X�g����M���܂����B")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        End If

        '�Ď��Ղ̉��D�@�����v���O�����ێ���Ԃ��X�V����B
        If holding0SubKind = dataSubKind AndAlso holding0Version = dataVersion Then
            Debug.Assert(oMonitorMachine.HoldingPrograms(0) IsNot Nothing)
            'NOTE: ���̃P�[�X�ł́A�z��ł�oMonitorMachine.HoldingPrograms(1)����̂͂��ł���B
            '�����M�����v���O����P�Ɠ������̂�HoldingPrograms(0)�Ɋi�[����Ă���
            '�Ƃ������Ƃ́A�z���ɂ���S���D�@�������K�p���Ă��邱�ƂɂȂ�B
            '����HoldingPrograms(1)�ɉ������i�[���Ă���̂ł���΁A�O�q��P�̓K�p��A
            'P�ƈႤ�o�[�W�����̃v���O�������󂯓���Ă���i�ꕔ���@�ɐ�s�K�p����Ă�����A
            '�K�p�҂��ɂȂ��Ă�����A�z�M�҂��ɂȂ��Ă���j�Ƃ������Ƃł���A
            '�����M����P�́A�󂯓��ꋑ�ۂ��Ă���͂��ł���B
            If oMonitorMachine.HoldingPrograms(1) IsNot Nothing Then
                Log.Warn(sMonitorMachineId, "�[���ւ̓K�p���ς�ł��Ȃ��V����v���O������ێ����Ă���ɂ�������炸�A������v���O�����̓K�p���X�g�󂯓�������肵�܂����B�z��O�̏󋵂ł����A���������s���܂��B")
            End If
          #If AcceptsSameNameProOfSameHashValue Then
            If oReqTeleg.DataFileName.Length <> 0 Then
                Log.Info(sMonitorMachineId, "�ێ�(1)�ɂ��铯��G���ANo�E�����\Ver�̃v���O�������㏑�����܂��B")
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
                Log.Info(sMonitorMachineId, "�ێ�(2)�ɂ��铯��G���ANo�E�����\Ver�̃v���O�������㏑�����܂��B")
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
            'NOTE: ���̃P�[�X�ł́A�z��ł�oMonitorMachine.HoldingPrograms(1)����̂͂��ł���B
            If oMonitorMachine.HoldingPrograms(1) IsNot Nothing Then
                Log.Warn(sMonitorMachineId, "�[���ւ̓K�p���ς�ł��Ȃ��V����v���O������ێ����Ă���ɂ�������炸�A�ʂ̐V����v���O�����̎󂯓�������肵�܂����B�z��O�̏󋵂ł����A���������s���܂��B")
            End If
            Log.Info(sMonitorMachineId, "�V���ȃv���O������ێ�(2)�Ɏ󂯓���܂��B")
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
        Log.Info(sMonitorMachineId, "�󂯓��ꂪ�������܂����B")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverGatePro(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '�S���D�@�̃L���[����S�Ẳ��D�@�����v���O�����K�p���X�g�����o���A
        '�K�p���X�g���ƂɁA���Y���D�@(t)�̃v���O�����ێ����
        '�ioMonitorMachine.TermMachines(t).HoldingPrograms�j��
        '�X�V���AsMachineDir�ɓK�p���X�g�ʁE���D�@�ʂ�
        '#GateProDlReflectReq_RRRSSSCCCCUU_N.dat�iN��0�`�j���쐬����B
        '�܂��A�v���O�����ێ���Ԃ��X�V�������D�@�ɂ��ẮA
        'sContextDir��GateProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AGateProDlReflectReq_RRRSSSCCCCUU_N.dat��
        'GateProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́A����X�V
        '���Ă��Ȃ����D�@�̂��́j���폜����B

        'NOTE: �����̃t�@�C������RRRSSSCCCCUU�͒[���̋@��R�[�h��
        '���邪�A�����̃t�@�C�����S�[�����i�V�i���I����
        '�u%T3R%T3S%T4C%T2U�v�ƋL�q�����ꍇ�ɕ��������S�s���j
        '�쐬�����Ƃ͌���Ȃ��B
        '����āA�V�i���I�́A���Y�t�@�C���𑗐M����ہA
        'ActiveOne�ł͂Ȃ��ATryActiveOne���g�p����B

        DeleteFiles(sMonitorMachineId, sContextDir, "GateProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now

        '�S�[���ɂ��ď������s���B
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            Dim oTerm As TermMachine = oTermEntry.Value

            If oTerm.PendingPrograms.Count = 0 Then
                Log.Debug(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɑ΂���v���O�����z�M�͂���܂���B")
                Continue For
            ElseIf oTerm.McpStatusFromKsb <> &H0 Then
                Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ��ẮA�吧��Ԃ�����ȊO�ɐݒ肳��Ă��邽�߁A�z�M������ۗ����܂��B")
                Continue For
            End If

            Dim isHoldingProUpdated As Boolean = False

            'TODO: �{���̊Ď��Ղ́A�Ō�ɃL���[�C���O�������̂������D�@�ɑ��M���Ȃ���������Ȃ��B
            '����͎d���Ȃ��Ƃ��Ă��A�Ō�ɃL���[�C���O�����v���ɑΉ�����DL�����ʒm�������������Ȃ��\����
            '����i�v�����ꂽ���Ƃ��s���Ȃ��Ȃ�u�ُ�v��DL�����ʒm�𔭐�������΍ςނɂ�������炸�j�B
            '�����������Ƃ�����A�^�ǂɑ΂��铭�������@�ƃV�~�����[�^�ňႤ... �Ƃ������ƂɂȂ��Ă��܂��̂ŁA
            '�V�~�����[�^�ł��A�Ō�ɃL���[�C���O����Ă�����̈ȊO�͓ǂݎ̂Ă������悢��������Ȃ��B
            '�Ȃ��A���Ƃ���������ɂ��Ă��ATermMachine�N���X��PendingPrograms�͕K�v�ł���B
            '�V�~�����[�^�̋@�\�Ƃ��āA���D�@�ɖ��z�M�̂��̂����[�U�Ɏ����K�v�����邽�߂ł���B

            '�z�M�����Ɏg���Ă��Ȃ��S�K�p���X�g�ɂ��ď������s���B
            For Each oPenPro As PendingProgram In oTerm.PendingPrograms
                'NOTE: ���̂̂Ȃ��iListHashValue Is Nothing �́j�K�p���X�g�Ŕz�M���s����\���͑z�肵�Ȃ��B
                Log.Info(sMonitorMachineId, "�K�p���X�g [" & oPenPro.ListVersion.ToString() & "] �Ɋ�Â��A�[�� [" & sTermMachineId & "] �ɑ΂���G���ANo [" & oPenPro.DataSubKind.ToString() & "] ��\Ver [" & oPenPro.DataVersion.ToString() & "] �̃v���O�����z�M�������s���܂�...")

                If oPenPro.ApplicableDate.Equals("99999999") Then
                    Log.Info(sMonitorMachineId, "�����Y�[���ɑ΂���v���͉������~�v���ł��B")
                End If

                '���Y�[���ɑ΂��čŌ�ɔz�M�����K�p���X�g�̏����擾����B
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

                '�K�p���X�g�̔z�M���ʁi�u����v�܂��́u�ُ�v�u�K�p�ς݁v�j�����߂�B
                'TODO: �{���̊Ď��Ղ͓K�p���X�g�o�[�W����������r���Ȃ���������Ȃ��B
                '�ŐV�́i�K�p�ςݑΉ��́j�Ď��Ղł͓K�p���X�g�̓��e���r����ƐM�������B
                'NOTE: ���������{���̊Ď��Ղ́A�K�p���X�g�́u�K�p�ς݁v�𔻒f����̂ɁA
                '���ݓK�p���̃v���O������K�p�����ۂɗp�����K�p���X�g�Ɣ�r����̂�������Ȃ��B
                '�K�p���X�g�̏ꍇ�A�v���O�����{�̂ƈقȂ�A���D�@���ҋ@�ʂɕێ����Ă�����
                '�Ď��Ղ����D�@�ւ̔z�M�҂��ɂ��Ă�����̂��A�����\�o�[�W�����ł���Ȃ���A
                '����ނ����蓾��킯������A�������ɂ��̂悤�Ȏd�l�ł͂Ȃ��Ǝv���邪...
                'NOTE: ���̃A�v���ł́A�{���̊Ď��Ղɍ��킹�āA�K�p���X�g�ɑ΂���u�K�p�ς݁v��
                '�����ł���悤�ɂ��Ă͂��邪�A���������A�K�p���X�g�ɑ΂���u�K�p�ς݁v�Ƃ���
                '���z���́A�ςł���B�K�p���X�g�ɂ͖��O�ȂǂȂ��A�ʂɁi�^�ǂ���DLL�̗v��
                '���ƂɁj�Ӗ���������̂Ȃ̂�����A�킴�킴��r���āu�K�p�ς݁v�ȂǂƂ����ɁA
                '�L�ӂȓK�p�����w�肳��Ă�����D�@�ɂ͕K������t��������A���ɓK���Ă���B
                Dim listDeliveryResult As Byte = &H0

                If latestListHashValue IsNot Nothing AndAlso _
                   oPenPro.DataSubKind = latestDataSubKind AndAlso _
                   oPenPro.DataVersion = latestDataVersion AndAlso _
                   oPenPro.ListVersion = latestListVersion AndAlso _
                   StringComparer.OrdinalIgnoreCase.Compare(oPenPro.ListHashValue, latestListHashValue) = 0 Then
                    '�u���D�@���K�p�҂��̕��ނƈꏏ�ɕێ����Ă���K�p���X�g�v�Ɠ������̂�z�M����
                    '���ƂɂȂ�ꍇ�́A�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                    'NOTE: ���̃P�[�X�ł́A�������~�v���̓K�p���X�g�ɑ΂��Ă��u�K�p�ς݁i�������~�ς݁H�j�v
                    '�ōς܂��Ă��܂����A�{���̊Ď��Ղ������ł��邩�͕s���B���������A�O��̔z�M����
                    '�������~�����Ă���Ƃ�����A�ҋ@�ʂ�������Ă���͂��Ȃ̂ŁA���ʂɂ͂��蓾�Ȃ�
                    '�P�[�X�Ǝv����B
                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�K�p���X�g��z�M�ς݂ł��B�K�p���X�g�̍Ĕz�M���s���܂���̂ŁA�������~���s���܂���B")
                    Else
                        Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�K�p���X�g��z�M�ς݂ł��B�K�p���X�g�̍Ĕz�M�͍s���܂���B�K�p���X�g�Ɋ�Â��v���O�����{�̂̔z�M���s���܂���B")
                    End If
                    listDeliveryResult = &HF
                End If

                If listDeliveryResult = &H0 Then
                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        'NOTE: oPenPro�̃o�[�W�����̃v���O���������ɓK�p���ɂȂ��Ă���P�[�X��
                        '�ȉ��̃P�[�X�i�����ȉ������~�v���j�ɓ��Ă͂܂�͂��ł���B
                        'NOTE: �uoTerm.HoldingPrograms(1) Is Nothing�v�łȂ��ꍇ�ɂ����ẮA
                        '������uoTerm.HoldingPrograms(1).DataVersion = 0�v�ł���Ƃ��Ă��A
                        '����́A�[�����o�[�W����0�̃v���O������ێ����Ă���Ƃ������Ƃł���B
                        '����āAoPenPro.DataVersion��0�ł���A�G���A�ԍ�����v����Ȃ�A
                        'oPenPro�͗L���ȉ������~�ł���A�ȉ��̏������U�ɂȂ��Ă悢�B
                        If oTerm.HoldingPrograms(1) Is Nothing OrElse _
                           oTerm.HoldingPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                           oTerm.HoldingPrograms(1).DataVersion <> oPenPro.DataVersion Then
                            'NOTE: �{���̊Ď��Ղ����̂悤�Ɍ����ȓ��������̂��́A�s���ł���B
                            Log.Error(sMonitorMachineId, "�����ȉ������~�v���ł��B���Y�[���ɂ����ē��Y�v���O�������K�p�҂��ɂȂ��Ă��܂���B")
                            listDeliveryResult = &H1
                        End If
                    End If
                End If

                'NOTE: �K�p���X�g���K�p�ς݂̏ꍇ��A�������~�v���̓K�p���X�g�������ȏꍇ�́A
                '�v���O�����{�̂�DL�����ʒm�͔��������Ȃ��B�����̃P�[�X�ł́A
                '�v���O�����{�͔̂z�M�ΏۂłȂ��͂��ł���A���Ȃ��͂��B
                '�������~�̍s���܂ޓK�p���X�g���v���O�����{�̂ƂƂ���DLL�����P�[�X�͑z�肵�Ȃ��B
                'TODO: �{���̊Ď��Ձ����D�@���ǂ��������͕�����Ȃ��B
                If listDeliveryResult = &H0 Then
                    If oPenPro.ApplicableDate.Equals("99999999") Then
                        'NOTE: �K�p�����u99999999�v�̍s�̉��D�@�ɂ��ẮA�v���O�����{�̂�DL�����ʒm��
                        '�i�K�p�ς݂Ȃǂ��܂߂āj�������Ȃ����Ƃɂ���B�{���̊Ď��Ղ��ǂ��Ȃ̂��͕s���B
                        'TODO: �^�ǂɂ����āA������D�@�ɑ΂��邠��o�[�W�����̃v���O�����̏���̔z�M�w���ŁA
                        '�K�p���X�g�Ɂu99999999�v���L�ڂ��Ă��܂�����A�u99999999�v���L�ڂ��ꂽ�K�p���X�g��
                        '�z�M���s���ۂɁu�v���O����+�v���O�����K�p���X�g �����z�M�v�Ƀ`�F�b�N������
                        '���܂����肷��ƁA�v���O�����{�̂Ɋւ��铖�Y���D�@�̎�M��Ԃ��u�z�M���v�ɂȂ�A
                        '���ꂪ���̂܂܎c���Ă��܂��Ǝv����B����ɂ��ẮA�K�p���u99999999�v���w��
                        '���ꂽ���D�@�ɂ��āu�z�M���v�̃��R�[�h���쐬���Ȃ��悤�ɁA�����āA�ł��邱��
                        '�Ȃ�u99999999�v���L�ڂ��ꂽ�K�p���X�g�Łu�v���O����+�v���O�����K�p���X�g �����z�M�v
                        '���w��ł��Ȃ��悤�ɁA�^�ǂ̎��������P����ׂ��ł���B

                        '���D�@�̃v���O�����ێ���Ԃ��X�V����B
                        'NOTE: �{���̊Ď��Ղ́A���Ƃ��ꕔ�̉��D�@�̉������~�ł����Ă��A
                        '�Ď��Ղ̕ێ��o�[�W�����܂ŕς���Ă�����������Ȃ�...
                        oTerm.HoldingPrograms(1) = Nothing
                        isHoldingProUpdated = True
                        Log.Info(sMonitorMachineId, "���Y�[���ɑ΂��ĉ������~���s���܂����B")
                    Else
                        'NOTE: ���Ƃ��K�p���X�g�Ɋւ���DL�����ʒm���u�K�p�ς݁v�ł������Ƃ��Ă��A
                        '�v���O�����{�̂�DL�����ʒm����������i�K�p���X�g�Ɋւ���u�K�p�ς݁v�Ȃ�
                        '�Ƃ����T�O���������܂ꂽ���ƂŁA��a�������邩������Ȃ����A�^�ǂ����
                        'DLL�v���ɂ́A�K�p���X�g�̃o�[�W�����ȂǂɊ֌W�Ȃ��A�ʂɈӖ�������j�B
                        '�܂��A���Ƃ��K�p���X�g���K�p�ς݁i= ���ۂ́A�P�Ȃ鑗�M�ς݁j�ł������Ƃ��Ă��A
                        '���̓K�p���X�g�ɂ����āA���Y�v���O�������K�p�̉��D�@�ɗL�ӂȓK�p�����L�ڂ����
                        '����΁A�v���O�����{�̂ɂ��Ắu�K�p�ς݁v�ł͂Ȃ��u����v��DL�����ʒm��
                        '��������B
                        'TODO: �{���̊Ď��Ղ́A�K�p���X�g�Ɋւ���DL�����ʒm���u�K�p�ς݁v�ł���ꍇ�ɁA
                        '�v���O�����{�̂�DL�����ʒm�i�����炭�u�K�p�ς݁v�j�𐶐����Ȃ���������Ȃ��B
                        '���̏󋵂ł́A�^�ǂɂ����铖�Y���D�@�̓��Y�v���O�����̎�M��Ԃ��u�z�M���v
                        '�ł͂Ȃ��u����v���ɂȂ��Ă���Ǝv���邪�A�{���ɂ��̕ۏ؂�����̂�
                        '���؂��������悢�B
                        'TODO: ���̃A�v���ł́A�ǐS�Ɋ�Â��Ĕ�r���Ă��邪�A�{���̊Ď��Ղ�
                        'CAB�̓��e���r���āA�s��v�̏ꍇ�ɍĔz�M���s�����͕s���ł���B

                        '�v���O�����{�̂̔z�M���ʁi�u����v�܂��́u�K�p�ς݁v�j�����߂�B
                        Dim dataDeliveryResult As Byte = &H0

                        'Dim sServiceDate As String = EkServiceDate.GenString(d)
                        'If String.CompareOrdinal(oPenPro.ApplicableDate, sServiceDate) < 0 Then
                        'End If

                        'NOTE: �{���̉��D�@���Ď��Ղ̓n�b�V���l�̔�r���s��Ȃ���������Ȃ��B
                        '�������A���������A����o�[�W�����̃v���O���������ɉ��D�@�ɓ����Ă���i���邱�Ƃ��m�肵�Ă���j�󋵂ł́A
                        '����Ɠ����o�[�W�����œ��e�̈قȂ�v���O�������Ď��Ղ��󂯓���邱�Ǝ��̂��Ȃ��͂��Ȃ̂ŁA
                        '��r���Ă�����ɈႢ�͂Ȃ��Ǝv����B

                        If oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                           oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion AndAlso _
                           StringComparer.OrdinalIgnoreCase.Compare(oTerm.HoldingPrograms(0).DataHashValue, oPenPro.DataHashValue) = 0 Then
                            'TODO: ���D�@���K�p���̂��̂Ɠ����o�[�W�����̃v���O���������D�@�ɔz�M���邱�Ƃ͂ł��Ȃ��͂��Ȃ̂ŁA
                            '�n�b�V���l���قȂ�ꍇ�ł��ُ툵���ɂ���ׂ���������Ȃ����A���������Ȃ�A��������
                            'AcceptGatePro�ɂĊĎ��Վ��g�ւ̎󂯓����h�~����ׂ��ł���B
                            'NOTE: �K�p���O�̂��̂����D�@���K�p���Ă���͂��͂Ȃ����A�K�p�����߂������̂����D�@�ɔz�M���悤�Ƃ���
                            '�͂����Ȃ��B�������A���D�@���K�p���̂��̂Ɠ��o�[�W�����̃v���O�������A�K�p�������ɁA
                            '�Ď��Ղ���M�����P�[�X��A�K�p���O�ɊĎ��Ղ���M���A���D�@�ɔz�M���Ȃ��܂ܓK�p�����߂���
                            '�P�[�X�Ȃǂ́A���蓾��B��҂͖{�֐����œK�p���Ɖ^�p�����r���āA�ʂُ̈툵���ɂ��邱�Ƃ�
                            '�\�ł��邪�A�O�҂͂����͂����Ȃ��B
                            'TODO: �Ƃ肠�����z�M���ʂ��u�K�p�ς݁v�Ƃ��邪�A�{���̊Ď��Ղɍ��킹�������悢�B
                            Log.Warn(sMonitorMachineId, "���Y�[���ɂ͓��o�[�W�����̃v���O������K�p�ς݂ł��B�v���O�����{�̂̍Ĕz�M�͍s���܂���B")
                            dataDeliveryResult = &HF
                        End If

                        If dataDeliveryResult = &H0 Then
                            If oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                               oTerm.HoldingPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                               oTerm.HoldingPrograms(1).DataVersion = oPenPro.DataVersion AndAlso _
                               StringComparer.OrdinalIgnoreCase.Compare(oTerm.HoldingPrograms(1).DataHashValue, oPenPro.DataHashValue) = 0 Then
                                '���D�@���K�p�҂��ɂ��Ă�����̂Ɠ������̂̔z�M�w�����������ꍇ�́A
                                '�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                                Log.Warn(sMonitorMachineId, "���Y�[���ɑ΂��Ă͓��Y�v���O������K�p�ς݁i���z�M�ς݁j�ł��B�v���O�����{�̂̍Ĕz�M�͍s���܂���B")
                                dataDeliveryResult = &HF
                            End If
                        End If

                        If dataDeliveryResult = &H0 Then
                            If Not oPenPro.ApplicableDate.Equals("19000101") AndAlso _
                               String.CompareOrdinal(oPenPro.ApplicableDate, oPenPro.RunnableDate) < 0 Then
                                Log.Error(sMonitorMachineId, "�v���O�����̓��싖�����K�p���ȍ~�ɐݒ肳��Ă��܂��B�z�M�͍s���܂���B")
                                dataDeliveryResult = &HC
                                listDeliveryResult = &H1 'TODO: �K�p���X�g�̔z�M���ʂ͂R��ނ����Ȃ��B�{���͓K�p���X�g��z�M����̂�������Ȃ��B
                            End If
                        End If

                        '���D�@�̃v���O�����ێ���Ԃ��X�V����B
                        If dataDeliveryResult = &H0 Then
                            If oTerm.HoldingPrograms(1) IsNot Nothing AndAlso _
                              (oTerm.HoldingPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                               oTerm.HoldingPrograms(1).DataVersion <> oPenPro.DataVersion OrElse _
                               StringComparer.OrdinalIgnoreCase.Compare(oTerm.HoldingPrograms(1).DataHashValue, oPenPro.DataHashValue) <> 0) Then
                                Log.Warn(sMonitorMachineId, "�V����v���O�������K�p�҂��ɂȂ��Ă���[���ɑ΂���A�ʂ̐V����v���O�����̔z�M�ł��B�z��O�̏󋵂ł����z�M�����s���܂��B")
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
                            Log.Info(sMonitorMachineId, "���Y�[���̑ҋ@�ʂɑ΂��ē��Y�v���O�����{�̂̔z�M���s���܂����B")
                            Log.Info(sMonitorMachineId, "���Y�[���̑ҋ@�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B")
                        ElseIf listDeliveryResult = &H0 Then
                            If oTerm.HoldingPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                               oTerm.HoldingPrograms(0).DataVersion = oPenPro.DataVersion Then
                                oTerm.HoldingPrograms(0).ListVersion = oPenPro.ListVersion
                                oTerm.HoldingPrograms(0).ListAcceptDate = oPenPro.ListAcceptDate
                                oTerm.HoldingPrograms(0).ListDeliverDate = d
                                oTerm.HoldingPrograms(0).ApplicableDate = oPenPro.ApplicableDate
                                oTerm.HoldingPrograms(0).ListContent = oPenPro.ListContent
                                oTerm.HoldingPrograms(0).ListHashValue = oPenPro.ListHashValue
                                'TODO: oTerm.HoldingPrograms(0).ListVersion���X�V���Ă��A
                                '��{�I�ɉ��D�@�v���O�����o�[�W�������ɕω��͖����͂��Ȃ̂ŁA
                                '�ȉ��͍s��Ȃ������悢��������Ȃ��B
                                'TODO: ���̃A�v���ł́A�ҋ@�ʂɂ���K�p���X�g�ɂ����Ӗ�������
                                '���̂Ƃ��āA���D�@�v���O�����o�[�W�������ɃZ�b�g���Ă��邪�A
                                '���������{���̉��D�@�V�X�e�����ǂ��ł��邩�͂킩��Ȃ��B
                                isHoldingProUpdated = True
                                Log.Warn(sMonitorMachineId, "���Y�[���̓K�p�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B���̓K�p���͈Ӗ��������܂���̂Œ��ӂ��Ă��������B")
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
                                Log.Info(sMonitorMachineId, "���Y�[���̑ҋ@�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B")
                            Else
                                'TODO: �悭���؂��Ȃ��Ƃ��蓾��P�[�X���ǂ���������Ȃ��B
                                Log.Error(sMonitorMachineId, "���Y�[���ɂ����āA���Y�K�p���X�g�ɕR�Â��v���O�����{�̂�����܂���B�K�p���X�g�̔z�M�͍s���܂���B")
                                listDeliveryResult = &H1
                            End If
                        End If

                        '�v���O�����{�̂Ɋւ���#GateProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
                        CreateFileOfGateProDlReflectReq( _
                           &H21, _
                           oPenPro.DataVersion, _
                           dataDeliveryResult, _
                           sMonitorMachineId, _
                           sTermMachineId, _
                           sMachineDir)
                    End If
                End If

                'NOTE: ���̔z�M�̑O�ɒ��ړ��������{�����ꍇ�ȂǁA���D�@�ɓK�p���X�g��
                '���݂��Ȃ��ꍇ�́A���L���s��Ȃ��B
                '�����̉��D�@�V�X�e���̋������i�ǂ������Ɋ֌W�Ȃ��j�����ɍČ�����B
                If latestListHashValue IsNot Nothing Then
                    '�K�p���X�g�Ɋւ���#GateProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
                    CreateFileOfGateProDlReflectReq( _
                       &H48, _
                       oPenPro.ListVersion, _
                       listDeliveryResult, _
                       sMonitorMachineId, _
                       sTermMachineId, _
                       sMachineDir)
                Else
                    Log.Warn(sMonitorMachineId, "���Y�[�����K�p���X�g��ێ����Ă��Ȃ��������߁A�K�p���X�g��DL�����ʒm�͍쐬���܂���ł����B����͎������D�@�V�X�e���̐��������ł��B")
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

        '�S���D�@�ɂ��āA�ҋ@�ʂɃv���O������ێ����Ă��邩�`�F�b�N���A
        '���̓K�p�����^�p���ȑO�ł���΁A�K�p�ʂɈړ�����B
        '�S�Ẳ��D�@�̓K�p���o�[�W����������ɂȂ�A
        '���ꂪ�Ď��Ղɂ����āA�V����ʂɊi�[����Ă���ꍇ�́A
        '������ʂɈړ�����B
        '�܂��A�v���O�����ێ���Ԃ��X�V�������D�@�ɂ��ẮA
        'sContextDir��GateProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AGateProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����X�V���Ă��Ȃ����D�@�̂��́j���폜����B

        'NOTE: �����̃t�@�C������RRRSSSCCCCUU�͒[���̋@��R�[�h��
        '���邪�A�����̃t�@�C�����S�[�����i�V�i���I����
        '�u%T3R%T3S%T4C%T2U�v�ƋL�q�����ꍇ�ɕ��������S�s���j
        '�쐬�����Ƃ͌���Ȃ��B
        '����āA�V�i���I�́A���Y�t�@�C���𑗐M����ہA
        'ActiveOne�ł͂Ȃ��ATryActiveOne���g�p����B

        DeleteFiles(sMonitorMachineId, sContextDir, "GateProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        '�S�[���ɂ��ď������s���B
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim sTermMachineId As String = oTermEntry.Key
            Dim oTerm As TermMachine = oTermEntry.Value
            If oTerm.HoldingPrograms(1) Is Nothing Then
                Log.Debug(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ́A�K�p�҂��̃v���O����������܂���B")
            ElseIf oTerm.HoldingPrograms(1).ListHashValue IsNot Nothing AndAlso _
                   String.CompareOrdinal(oTerm.HoldingPrograms(1).ApplicableDate, sServiceDate) > 0 Then
                Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ́A�K�p�҂��̃v���O����������܂����A�K�p���O�ł��邽�߁A�K�p���܂���B")
            ElseIf String.CompareOrdinal(oTerm.HoldingPrograms(1).RunnableDate, sServiceDate) > 0 Then
                Log.Warn(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ́A�K�p�҂��̃v���O����������܂����A���싖���O�ł��邽�߁A�K�p���܂���B")
            Else
                oTerm.HoldingPrograms(0) = oTerm.HoldingPrograms(1)
                oTerm.HoldingPrograms(0).ApplyDate = d
                oTerm.HoldingPrograms(1) = Nothing
                Log.Info(sMonitorMachineId, "�[�� [" & sTermMachineId & "] �ɂ����āA�K�p�҂��̃v���O������K�p���܂����B")
                CreateFileOfGateProVerInfo(sMonitorMachineId, sTermMachineId, oTerm, sContextDir)
                UpdateTable2OnTermStateChanged(sMachineDir, sTermMachineId, oTerm)
            End If
        Next oTermEntry

        'TODO: �S���D�@�̓K�p���o�[�W�������������ɂ�������炸�A
        '�Ď��Ղɂ����āA���ꂪ�V����ʂɂ�������ʂɂ��ێ��ł��Ă��Ȃ�
        '�P�[�X��A���ɋ�����ʂɈړ��ς݂ł���Ȃ���V����ʂ�
        '�������݂��Ă���P�[�X�ȂǁA�i�s���Ȕ}�̓������s���Ȃǂ��Ȃ�����j
        '���蓾�Ȃ��P�[�X�̏ꍇ�́A�x�����o������ŁA�Ď��Ղ̏�Ԃ�␳����
        '�ׂ���������Ȃ��B
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
                Log.Info(sMonitorMachineId, "�ێ�(2)�̃v���O�������S�[���ɓK�p���ꂽ���߁A�ێ�(1)�Ɉړ����܂����B")
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
            Log.Error("�v���O�����{�̂̃n�b�V���l�Z�o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim content As KsbProgramContent
        Try
            content = ExtractKsbProgramCab(sFilePath, Path.Combine(sContextDir, "KsbPro"))
        Catch ex As Exception
            Log.Error("�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim subKind As Integer
        Try
            subKind = Byte.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData), NumberStyles.HexNumber)
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̃G���ANo�̒��o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        Dim version As Integer
        Try
            version = Integer.Parse(ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", ""))
        Catch ex As Exception
            Log.Error("�o�[�W�������X�g����̑�\Ver�̒��o�ŗ�O���������܂����B", ex)
            Return False
        End Try

        'TODO: �����ŁA���ނ̃G���A�ƊĎ��Ղ��F�����Ă���Ď��ՃG���A�̐������`�F�b�N���s�����Ƃ��\�Ǝv����B
        '�������@���`�F�b�N���s���Ȃ�A����ɍ��킹�������悢�B

        If String.CompareOrdinal(EkServiceDate.GenString(), content.RunnableDate) < 0 Then
            Log.Warn("�v���O�����̓��싖�����������ɐݒ肳��Ă��܂��B" & content.RunnableDate & "�܂œK�p�ł��܂���̂ł����ӂ��������B")
        End If

        InstallKsbProgramDirectly(sContextDir, subKind, version, content, sHashValue)

        Return True
    End Function

    Protected Function AcceptKsbPro(ByVal sContextDir As String, ByRef sResult As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'sMachineDir��#PassiveDllReq.dat�������t�@�C����
        'oMonitorMachine.PendingKsbPrograms�ɃL���[�C���O���A
        'sContextDir��ExtOutput.dat���쐬����B
        '�������A�󂯓���s�\�ȏꍇ�́A�L���[�C���O�����ɁA
        'ContinueCode��FinishWithoutStoring��ExtOutput.dat���쐬����B

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        Dim oReqTeleg As EkMasProDllReqTelegram
        Using oInputStream As New FileStream(Path.Combine(sMachineDir, "#PassiveDllReq.dat"), FileMode.Open, FileAccess.Read)
            oReqTeleg = New EkMasProDllReqTelegram(TelegImporter.GetTelegramFromStream(oInputStream), 0)
        End Using

        If oReqTeleg.GetBodyFormatViolation() <> EkNakCauseCode.None Then
            Log.Error(sMonitorMachineId, "�d�������ɕs��������܂��B")
            Return False
        End If

        Log.Info(sMonitorMachineId, "�K�p���X�g�̃t�@�C������ [" & oReqTeleg.ListFileName & "] �ł��B")
        If oReqTeleg.DataFileName.Length <> 0 Then
            Log.Info(sMonitorMachineId, "�v���O�����{�̂̃t�@�C������ [" & oReqTeleg.DataFileName & "] �ł��B")
        Else
            Log.Info(sMonitorMachineId, "�v���O�����{�̂̃t�@�C�����͂���܂���B")
        End If

        Dim sListFileName As String = Path.GetFileName(oReqTeleg.ListFileName)
        If Not EkMasProListFileName.IsValid(sListFileName) Then
            Log.Error(sMonitorMachineId, "�K�p���X�g�̃t�@�C�������s���ł��B")
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
                Log.Error(sMonitorMachineId, "�v���O�����{�̂̃t�@�C�������s���ł��B")
                Return False
            End If
            If EkProgramDataFileName.GetApplicableModel(sDataFileName) <> sApplicableModel Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetKind(sDataFileName) <> sDataKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�v���O������ʁj�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetSubKindAsInt(sDataFileName) <> dataSubKind Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i�G���ANo�j�ɕs����������܂��B")
                Return False
            End If
            If EkProgramDataFileName.GetVersionAsInt(sDataFileName) <> dataVersion Then
                Log.Error(sMonitorMachineId, "�t�@�C�����i��\Ver�j�ɕs����������܂��B")
                Return False
            End If
        End If

        'NOTE: ��{�I�ɁAoReqTeleg��SubObjCode��ListFileName�̐������́A�V�i���I��
        '�ŕۏ؂���z��ł���B�܂��ADataFileName����łȂ��ꍇ��ListFileName�Ƃ�
        '�������ɂ��Ă��A�V�i���I���ŕۏ؂���z��ł���B�V�i���I���łȂ�A
        '�������̂Ȃ�REQ�ɑ΂��ANAK��ԐM����悤�ɐݒ�ł��邽�߂ł���B
        '�������A���Ƃ��A�V�i���I���Ń`�F�b�N���Ȃ��Ƃ��Ă��A���������A�������̂Ȃ�
        '�z�M���s���Ă��܂����Ȃ�A����͉^�ǃV�X�e���̕s��̂͂��ł��邩��A
        '�V�i���I�����{����NG�ŏI����������悢�B�ȏ�̂��Ƃ���A�����ł�
        '�������̃`�F�b�N���s���A���������Ȃ��ꍇ�́A�ے艞����ԋp����B
        If sApplicableModel <> Config.ModelSym Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�K�p��@��j���s���ł��B")
            Return False
        End If
        If sDataKind <> "WPG" Then
            Log.Error(sMonitorMachineId, "�t�@�C�����i�v���O������ʁj���s���ł��B")
            Return False
        End If
        If oReqTeleg.SubObjCode <> &H0 Then
            Log.Error(sMonitorMachineId, "�d���̃T�u��ʂ��s���ł��B")
            Return False
        End If

        'NOTE: �v���O������ێ����Ă��Ȃ��Ƃ��́A
        'holding0Version��holding1Version��0�ɂȂ�B
        'NOTE: EkMasProListFileName.IsValid()�ł̃`�F�b�N�ɂ���āA
        'dataVersion��0�ɂȂ邱�Ƃ͂��蓾�Ȃ����߁A��L�P�[�X�ł́A
        'holding0Version�����holding1Version���AdataVersion�ƈ�v
        '���邱�Ƃ͂��蓾�Ȃ��B
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

        '�K�p���X�g�݂̂���M�����i�}�X�^�{�̂���M���Ȃ��j�ꍇ�̂��߂ɁA
        '���Y�Ď��Ղ��Ō�Ɏ�M�����i��M�����K�p���X�g�ɕR�Â��j
        '�}�X�^�{�̂̏����擾���Ă����B
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
            '�ێ����L���[�C���O�����Ă��Ȃ��o�[�W�����̃v���O�����Ɋւ��āA�K�p���X�g�݂̂�
            '����t����ꂽ�ꍇ�́AContinueCode.FinishWithoutStoring��REQ�d�����쐬����B
            'NOTE: ���̃P�[�X�Ŗ{���̊Ď��Ղ��ǂ̂悤�Ȕ������������́A�������Ă��Ȃ��B
            If sDataHashValue Is Nothing Then
                Log.Error(sMonitorMachineId, "�K�p���X�g�ɕR�Â��v���O�����{�̂�����܂���B")
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End If
        Else
            '�ێ����Ă���o�[�W�����̃v���O�����Ɋւ��āACAB�{�̂�
            '����t����ꂽ�ꍇ�́A�ێ����Ă�����̂Ɠ��e��r���s���B
            '���e���قȂ�ꍇ���A���O�Ōx�����邾���ɂ���B
            'NOTE: ���̃P�[�X�Ŗ{���̊Ď��Ղ��ǂ̂悤�Ȕ������������́A�������Ă��Ȃ��B
            If sDataHashValue IsNot Nothing Then
                If StringComparer.OrdinalIgnoreCase.Compare(oReqTeleg.DataFileHashValue, sDataHashValue) <> 0 Then
                    Log.Warn(sMonitorMachineId, "��s���Ď󂯓��ꂽ���ꖼ�v���O�����Ɠ��e�i���n�b�V���l�j���Ⴂ�܂����A�󂯓�������s���܂��B")
                End If
            End If
        End If

        'TODO: �{���̊Ď��Ղɂ����āA�Ď��Ճv���O�����K�p���X�g�ɂ����D�@�}�X�^�K�p���X�g�Ɠ��l��
        '���������i�K�p���X�g�o�[�W�������ێ����Ă�����̂Ɠ���ł���ꍇ�͎̂Ă铙�j������Ȃ�A
        'isNewList��p�ӂ���ȂǁA���l�̎����ɂ���B
        Dim sListHashValue As String = oReqTeleg.ListFileHashValue.ToUpperInvariant()
        Dim listAcceptDate As DateTime = d

        'NOTE: �ȉ��̐���́AfullFlag��1��FinishWithoutStoring��Ԃ��P�[�X����邽�߂�
        '�p�ӂ������̂ł���B�{���̊Ď��Ղł́A�ҋ@�ʂւ̔z�M��ۗ�����@�\�͂Ȃ��A
        '��M������A���₩�Ɂi���Ɏ�M����O�Ɂj�ҋ@�ʂɔz�M����i�R�s�[����j��
        '�v���邽�߁A���������A���̂悤�Ȑ���͂��蓾�Ȃ��͂��B
        If oMonitorMachine.PendingKsbPrograms.Count <> 0 Then
            Log.Error(sMonitorMachineId, "��s���Ď󂯓��ꂽ���̂�ҋ@�ʂɈړ�����܂ŁA�V���ȃv���O�����̎󂯓���͂ł��܂���B")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 1, "FinishWithoutStoring")
            Return True
        End If

        '�K�p���X�g�̓��e����͂���B
        Dim sListContent As String
        Dim oListedMachines As New Dictionary(Of String, String)
        Try
            Dim sListFileNamePath As String = Utility.CombinePathWithVirtualPath(sFtpRootDir, oReqTeleg.ListFileName)

            '�Ƃ肠�����A�S�ēǂݎ��B
            'OPT: �u�K�p���X�g�̓��e��\������@�\�v��ǉ������ۂɁA�����葁��
            '�������邽�߂ɁA���̂悤�ɓ�x�ǂ݂��邱�ƂɂȂ��Ă���B
            '������ł���A���P�̗]�n������B
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))
                sListContent = oReader.ReadToEnd()
            End Using

            '�P�s���ǂݎ��B
            Dim sLine As String
            Dim aColumns As String()
            Using oReader As StreamReader _
               = New StreamReader(Path.Combine(sMachineDir, sListFileNamePath), Encoding.GetEncoding(932, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback))

                '�ȉ��A�K�p���X�g�̓��e���`�F�b�N���A��肪����ꍇ�́A
                'ContinueCode.FinishWithoutStoring��REQ�d�����쐬����B

                '�w�b�_���̂P�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g����ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂P�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 2 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g1�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�쐬�N�������`�F�b�N����B
                Dim createdDate As DateTime
                If DateTime.TryParseExact(aColumns(0), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, createdDate) = False Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�쐬�N�������s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '���X�gVer���`�F�b�N����B
                If Not listVersion.ToString("D2").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ���X�gVer���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ�ǂݍ��ށB
                sLine = oReader.ReadLine()
                If sLine Is Nothing Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g��2�s�ڂ�����܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�w�b�_���̂Q�s�ڂ��ɕ�������B
                aColumns = sLine.Split(","c)
                If aColumns.Length <> 3 Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g2�s�ڂ̍��ڐ����s���ł��B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�G���ANo���`�F�b�N����B
                If Not EkMasProListFileName.GetDataSubKind(sListFileName).Equals(aColumns(0)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�G���ANo���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '��\Ver���`�F�b�N����B
                If Not dataVersion.ToString("D8").Equals(aColumns(1)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ��\Ver���t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�@��R�[�h���`�F�b�N����B
                If Not sApplicableModel.Equals(aColumns(2)) Then
                    Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�킪�t�@�C�����Ɛ������Ă��܂���B")
                    sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                    Return True
                End If

                '�K�p���X�g�̂R�s�ڈȍ~����AoMonitorMachine�ɑ������鍆�@�𒊏o����B
                Dim lineNumber As Integer = 3
                sLine = oReader.ReadLine()
                While sLine IsNot Nothing
                    '�ǂݍ��񂾍s���ɕ�������B
                    aColumns = sLine.Split(","c)
                    If aColumns.Length <> 4 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��ڐ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�T�C�o�l����w���R�[�h�̏������`�F�b�N����B
                    If aColumns(0).Length <> 6 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(0), 0, 6) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̉w�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�R�[�i�[�R�[�h�̏������`�F�b�N����B
                    If aColumns(1).Length <> 4 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(1), 0, 4) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̃R�[�i�[�R�[�h���s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '���@�ԍ��̏������`�F�b�N����B
                    If aColumns(2).Length <> 2 OrElse _
                       Not Utility.IsDecimalStringFixed(aColumns(2), 0, 2) OrElse _
                       aColumns(2).Equals("00") Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̍��@�ԍ����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�K�p���̃����O�X���`�F�b�N����B
                    If aColumns(3).Length <> 8 AndAlso aColumns(3).Length <> 0 Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�K�p�����u�����N�łȂ��ꍇ�A�l���`�F�b�N����B
                    If aColumns(3).Length = 8 Then
                       If Not Utility.IsDecimalStringFixed(aColumns(3), 0, 8) Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If

                        Dim applicableDate As DateTime
                        If Not aColumns(3).Equals("99999999") AndAlso _
                           DateTime.TryParseExact(aColumns(3), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, applicableDate) = False Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ̓K�p�����s���ł��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    '�s�̏d�����`�F�b�N����B
                    Dim sLineKey As String = GetMachineId(sApplicableModel, aColumns(0), aColumns(1), aColumns(2))
                    If oListedMachines.ContainsKey(sLineKey) Then
                        Log.Error(sMonitorMachineId, "�K�p���X�g" & lineNumber.ToString() & "�s�ڂ����o�̍s�Əd�����Ă��܂��B")
                        sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                        Return True
                    End If

                    '�s�̓��e���ꎞ�ۑ�����B
                    oListedMachines.Add(sLineKey, aColumns(3))

                    '�s��oMonitorMachine�ɑ�������ꍇ
                    If sLineKey = sMonitorMachineId Then
                        '�G���A�ԍ����`�F�b�N����B
                        'TODO: �G���A0�̕��ނł���Ώ����G���A��0�ȊO�̊Ď��Ղɂ��K�p�ł���悤�ɂ��Ă��邪�A
                        '���������Ď��Ղ̏����G���A��0�ȊO�ł��邱�Ǝ��̂��ُ�Ȃ̂ŁA��߂������悢��������Ȃ��B
                        If dataSubKind <> 0 AndAlso _
                           dataSubKind <> DirectCast(oMonitorMachine.Profile(Config.MachineProfileFieldNamesIndices("AREA_CODE")), Integer) Then
                            Log.Error(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�� [" & sLineKey & "] �̏����G���A���A�K�p���X�g�̑ΏۃG���A�ƈقȂ�܂��B")
                            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                            Return True
                        End If
                    End If

                    sLine = oReader.ReadLine()
                    lineNumber += 1
                End While
            End Using
        Catch ex As Exception
            Log.Error(sMonitorMachineId, "�K�p���X�g�̓ǎ��ŗ�O���������܂����B", ex)
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
                Log.Error(sMonitorMachineId, "�v���O�����{�̂̉�͂ŗ�O���������܂����B", ex)
                sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
                Return True
            End Try

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �K�p�G���A", content.VersionListData).Equals(dataSubKind.ToString("X2")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̊Ď��Ղ̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ�G���ANo���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
            End If

            If Not ProgramVersionListUtil.GetFieldValueFromBytes("���ʕ� �v���O�����S��Ver�i�V�j", content.VersionListData).Replace(" ", "").Equals(dataVersion.ToString("D8")) Then
                'NOTE: CAB�̒��g���m�F�ł�������e�؂Ȃ̂ŁA���̂܂܏��������s����B
                'TODO: �{���̊Ď��Ղ̓���ɍ��킹�������悢�H
                Log.Warn(sMonitorMachineId, "�o�[�W�������X�g�ɋL�ڂ��ꂽ��\Ver���t�@�C�����Ɛ������Ă��܂��񂪁A���������s���܂��B")
            End If

            sRunnableDate = content.RunnableDate
            sArchiveCatalog = content.ArchiveCatalog
            oVersionListData = content.VersionListData
        End If

        '�z�M�̂��߂̏����L���[�C���O����B
        Dim targetCount As Integer = 0
        Dim targetFullCount As Integer = 0
        For Each oApplyEntry As KeyValuePair(Of String, String) In oListedMachines
            '�s��oMonitorMachine�ɑ�������ꍇ
            If oApplyEntry.Key = sMonitorMachineId Then
                '�K�p�������݂̉^�p���t�Ɠ������������邢�́u19000101�v���u99999999�v�̏ꍇ�̂݁A
                '�z�M���i���̃A�v���̏ꍇ�́ADL�����ʒm���j�K�v�Ƃ݂Ȃ��B
                'TODO: �{���̊Ď��Ղ́A���g�̍s�����̏����ɊY�����Ă��Ȃ��ꍇ�ɁA�u�K�p�ς݁v��DL�����ʒm��
                '����t���Ă��邩������Ȃ��i���D�@�����v���O�����ɂ��ẮA�����ł������悤�ɂ݂���j�B
                '����āA�ŐV�̊Ď��Ղ́A���������K�p�����ߋ����̍s�ł����Ă��A���Y�v���O�������ҋ@�ʂ�
                '�Ȃ���΁i�����K�p�ł���΁H�j�A�ҋ@�ʂɃR�s�[���Ă��܂���������Ȃ��B
                '����A�^�ǂ́A�K�p�����ߋ����̍s�́A�K�p�����u�����N�̍s�Ɠ��������ɂ��邱�ƂɂȂ��Ă���B
                '����䂦�ɁA�K�p���X�g�ɂ��̂悤�ȍs�����Ȃ���΁A�Ď��Ղɑ΂��đ��M���Ȃ��̂ŁA
                '�K�p���X�g����M�����Ď��Ղɂ����āA���g�̍s����L�����ɊY�����Ȃ��Ƃ������Ǝ��́A
                '�l���ɂ������Ƃł͂���B�����A��������l����ƁA���ۂɂǂ��Ȃ̂����؂��������悢�B
                '�Ȃ��A�^�ǂ̓����I/F�d�l�i�c�[���d�l���̕ʎ�6�j�Ɋ��S�ɍ��v���Ă���B
                If oApplyEntry.Value.Length = 8 AndAlso _
                  (oApplyEntry.Value.Equals("19000101") OrElse _
                   String.CompareOrdinal(oApplyEntry.Value, sServiceDate) >= 0) Then
                    Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�� [" & oApplyEntry.Key & "] �K�p�� [" & oApplyEntry.Value & "] �̍s���L���[�C���O���܂��B")
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
                    Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ�@�� [" & oApplyEntry.Key & "] �K�p�� [" & oApplyEntry.Value & "] �̍s�͏��O���܂��B")
                End If
                targetFullCount += 1
            End If
        Next oApplyEntry
        Log.Debug(sMonitorMachineId, "�K�p���X�g�ɋL�ڂ��ꂽ" & oListedMachines.Count.ToString() & "��̂����A" & targetFullCount.ToString() & "�䂪���Y�@��ł����B���̂���" & targetCount.ToString() & "��̓K�p�����L���ł����B")

        'NOTE: ���L�̃P�[�X�ŁA�{���̊Ď��Ղ��ǂ̂悤�Ȕ������������́A�悭�킩��Ȃ��B
        If targetCount = 0 Then
            Log.Error(sMonitorMachineId, "�z�M�𐶂ݏo���Ȃ��K�p���X�g����M���܂����B")
            sResult = CreateStringOfContinuousPassiveDllReq(holding0Version, holding1Version, 0, "FinishWithoutStoring")
            Return True
        End If

        UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        Log.Info(sMonitorMachineId, "�󂯓��ꂪ�������܂����B")

        sResult = CreateStringOfContinuousPassiveDllReq(0, 0, 0, "Finish")
        Return True
    End Function

    Protected Function DeliverKsbPro(ByVal sContextDir As String) As Boolean
        Dim sMachineDir As String = Path.GetDirectoryName(sContextDir)
        'Dim sFtpRootDir As String = Path.GetDirectoryName(Path.GetDirectoryName(sMachineDir))
        Dim sMonitorMachineId As String = GetMachineId(Path.GetFileName(sMachineDir))
        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        '�L���[����S�Ẵv���O�����K�p���X�g�����o���A
        '�K�p���X�g���ƂɁA�v���O�����ێ����
        '�ioMonitorMachine.HoldingKsbPrograms�j��
        '�X�V���AsMachineDir�ɓK�p���X�g�ʂ�
        '#KsbProDlReflectReq_RRRSSSCCCCUU_N.dat�iN��0�`�j���쐬����B
        '�܂��A�v���O�����ێ���Ԃ��X�V�����ꍇ�́A
        'sContextDir��KsbProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AKsbProDlReflectReq_RRRSSSCCCCUU_N.dat��
        'KsbProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́j���폜����B

        DeleteFiles(sMonitorMachineId, sContextDir, "KsbProVerInfo_*.dat")

        If oMonitorMachine.PendingKsbPrograms.Count = 0 Then
            Log.Debug(sMonitorMachineId, "���Y�@��ɑ΂���v���O�����z�M�͂���܂���B")
            Return True
        End If

        Dim d As DateTime = DateTime.Now
        Dim isHoldingProUpdated As Boolean = False

        '�z�M�����Ɏg���Ă��Ȃ��S�K�p���X�g�ɂ��ď������s���B
        For Each oPenPro As PendingKsbProgram In oMonitorMachine.PendingKsbPrograms
            'NOTE: ���̂̂Ȃ��iListHashValue Is Nothing �́j�K�p���X�g�Ŕz�M���s����\���͑z�肵�Ȃ��B
            Log.Info(sMonitorMachineId, "�K�p���X�g [" & oPenPro.ListVersion.ToString() & "] �Ɋ�Â��A���Y�@����ŃG���ANo [" & oPenPro.DataSubKind.ToString() & "] ��\Ver [" & oPenPro.DataVersion.ToString() & "] �̃v���O�����z�M�������s���܂�...")

            If oPenPro.ApplicableDate.Equals("99999999") Then
                Log.Info(sMonitorMachineId, "�����Y�@��ɑ΂���v���͉������~�v���ł��B")
            End If

            '�Ō�ɔz�M�����i�ҋ@�ʂɈړ������j�K�p���X�g�̏����擾����B
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

            '�K�p���X�g�̔z�M���ʁi�u����v�܂��́u�ُ�v�u�K�p�ς݁v�j�����߂�B
            'TODO: �{���̊Ď��Ղ͓K�p���X�g�o�[�W����������r���Ȃ���������Ȃ��B
            '�ŐV�́i�K�p�ςݑΉ��́j�Ď��Ղł͓K�p���X�g�̓��e���r����ƐM�������B
            'NOTE: ���������{���̊Ď��Ղ́A�K�p���X�g�́u�K�p�ς݁v�𔻒f����̂ɁA
            '�Ō�Ɏ󂯓��ꂽ���̂ł͂Ȃ��A���ݓK�p���̃v���O������K�p�����ۂɗp����
            '�K�p���X�g�Ɣ�r����̂�������Ȃ��B���ꂾ�ƁA�Ō�Ɏ󂯓��ꂽ���̂����K�p�ŁA
            '���ꂪ�����M�������̂Ɠ���ł���ꍇ�ɁA�u����v��DL�����ʒm�̌�A
            '�u�K�p�ς݁v�ł͂Ȃ��A�ēx�u����v��DL�����ʒm���o�����ƂɂȂ�킯�ł���A
            '�������ɂ��̂悤�Ȏd�l�ɂ͂��Ȃ��Ǝv���邪...
            Dim listDeliveryResult As Byte = &H0

            If latestListHashValue IsNot Nothing AndAlso _
               oPenPro.DataSubKind = latestDataSubKind AndAlso _
               oPenPro.DataVersion = latestDataVersion AndAlso _
               oPenPro.ListVersion = latestListVersion AndAlso _
               StringComparer.OrdinalIgnoreCase.Compare(oPenPro.ListHashValue, latestListHashValue) = 0 Then
                '�ҋ@�ʂɕێ����Ă���K�p���X�g�Ɠ������̂�ҋ@�ʂɔz�M���邱�ƂɂȂ�ꍇ�́A
                '�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                'NOTE: ���̃P�[�X�ł́A�������~�v���̓K�p���X�g�ɑ΂��Ă��u�K�p�ς݁i�������~�ς݁H�j�v
                '�ōς܂��Ă��܂����A�{���̊Ď��Ղ������ł��邩�͕s���B���������A�O��̔z�M����
                '�������~�����Ă���Ƃ�����A�ҋ@�ʂ�������Ă���͂��Ȃ̂ŁA���ʂɂ͂��蓾�Ȃ�
                '�P�[�X�Ǝv����B
                If oPenPro.ApplicableDate.Equals("99999999") Then
                    Log.Warn(sMonitorMachineId, "���Y�@��ɑ΂��Ă͓��Y�K�p���X�g��z�M�ς݂ł��B�K�p���X�g�̍Ĕz�M���s���܂���̂ŁA�������~���s���܂���B")
                Else
                    Log.Warn(sMonitorMachineId, "���Y�@��ɑ΂��Ă͓��Y�K�p���X�g��z�M�ς݂ł��B�K�p���X�g�̍Ĕz�M�͍s���܂���B�K�p���X�g�Ɋ�Â��v���O�����{�̂̔z�M���s���܂���B")
                End If
                listDeliveryResult = &HF
            End If

            If listDeliveryResult = &H0 Then
                If oPenPro.ApplicableDate.Equals("99999999") Then
                    'NOTE: oPenPro�̃o�[�W�����̃v���O���������ɓK�p���ɂȂ��Ă���P�[�X��
                    '�ȉ��̃P�[�X�i�����ȉ������~�v���j�ɓ��Ă͂܂�͂��ł���B
                    'NOTE: �uoMonitorMachine.HoldingKsbPrograms(1) Is Nothing�v�łȂ��ꍇ�ɂ����ẮA
                    '������uoMonitorMachine.HoldingKsbPrograms(1).DataVersion = 0�v�ł���Ƃ��Ă��A
                    '����́A�o�[�W����0�̃v���O������ێ����Ă���Ƃ������Ƃł���B
                    '����āAoPenPro.DataVersion��0�ł���A�G���A�ԍ�����v����Ȃ�A
                    'oPenPro�͗L���ȉ������~�ł���A�ȉ��̏������U�ɂȂ��Ă悢�B
                    If oMonitorMachine.HoldingKsbPrograms(1) Is Nothing OrElse _
                       oMonitorMachine.HoldingKsbPrograms(1).DataSubKind <> oPenPro.DataSubKind OrElse _
                       oMonitorMachine.HoldingKsbPrograms(1).DataVersion <> oPenPro.DataVersion Then
                        'NOTE: �{���̊Ď��Ղ����̂悤�Ɍ����ȓ��������̂��́A�s���ł���B
                        Log.Error(sMonitorMachineId, "�����ȉ������~�v���ł��B���Y�@��ɂ����ē��Y�v���O�������K�p�҂��ɂȂ��Ă��܂���B")
                        listDeliveryResult = &H1
                    End If
                End If
            End If

            'NOTE: �K�p���X�g���K�p�ς݂̏ꍇ��A�������~�v���̓K�p���X�g�������ȏꍇ�́A
            '�v���O�����{�̂�DL�����ʒm�͔��������Ȃ��B�����̃P�[�X�ł́A
            '�v���O�����{�͔̂z�M�ΏۂłȂ��͂��ł���A���Ȃ��͂��B
            '�������~�̍s���܂ޓK�p���X�g���v���O�����{�̂ƂƂ���DLL�����P�[�X�͑z�肵�Ȃ��B
            'TODO: �{���̊Ď��Ղ��ǂ��������͕�����Ȃ��B
            If listDeliveryResult = &H0 Then
                If oPenPro.ApplicableDate.Equals("99999999") Then
                    'NOTE: �K�p�����u99999999�v�̏ꍇ�́A�v���O�����{�̂�DL�����ʒm�́i�K�p�ς݂Ȃǂ��܂߂āj
                    '�������Ȃ����Ƃɂ���B�{���̊Ď��Ղ��ǂ��Ȃ̂��͕s���B
                    'TODO: �^�ǂɂ����āA����Ď��Ղɑ΂��邠��o�[�W�����̃v���O�����̏���̔z�M�w���ŁA
                    '�K�p���X�g�Ɂu99999999�v���L�ڂ��Ă��܂�����A�u99999999�v���L�ڂ��ꂽ�K�p���X�g��
                    '�z�M���s���ۂɁu�v���O����+�v���O�����K�p���X�g �����z�M�v�Ƀ`�F�b�N������
                    '���܂����肷��ƁA�v���O�����{�̂Ɋւ��铖�Y�Ď��Ղ̎�M��Ԃ��u�z�M���v�ɂȂ�A
                    '���ꂪ���̂܂܎c���Ă��܂��Ǝv����B����ɂ��ẮA�K�p���u99999999�v���w��
                    '���ꂽ�Ď��Ղɂ��āu�z�M���v�̃��R�[�h���쐬���Ȃ��悤�ɁA�����āA�ł��邱��
                    '�Ȃ�u99999999�v���L�ڂ��ꂽ�K�p���X�g�Łu�v���O����+�v���O�����K�p���X�g �����z�M�v
                    '���w��ł��Ȃ��悤�ɁA�^�ǂ̎��������P����ׂ��ł���B

                    '�Ď��Ճv���O�����ێ���Ԃ��X�V����B
                    oMonitorMachine.HoldingKsbPrograms(1) = Nothing
                    isHoldingProUpdated = True
                    Log.Info(sMonitorMachineId, "���Y�@��ɑ΂��ĉ������~���s���܂����B")
                Else
                    'NOTE: ���Ƃ��K�p���X�g�Ɋւ���DL�����ʒm���u�K�p�ς݁v�ł������Ƃ��Ă��A
                    '�v���O�����{�̂�DL�����ʒm����������i�K�p���X�g�Ɋւ���u�K�p�ς݁v�Ȃ�
                    '�Ƃ����T�O���������܂ꂽ���ƂŁA��a�������邩������Ȃ����A�^�ǂ����
                    'DLL�v���ɂ́A�K�p���X�g�̃o�[�W�����ȂǂɊ֌W�Ȃ��A�ʂɈӖ�������j�B
                    '�܂��A���Ƃ��K�p���X�g���K�p�ς݁i= ���ۂ́A�P�Ȃ鑗�M�ς݁j�ł������Ƃ��Ă��A
                    '���̓K�p���X�g�ɗL�ӂȓK�p�����L�ڂ���Ă���A���Y�v���O���������K�p�ł���
                    '�Ȃ�A�v���O�����{�̂ɂ��Ắu�K�p�ς݁v�ł͂Ȃ��u����v��DL�����ʒm��
                    '��������B
                    'TODO: �{���̊Ď��Ղ́A�K�p���X�g�Ɋւ���DL�����ʒm���u�K�p�ς݁v�ł���ꍇ�ɁA
                    '�v���O�����{�̂�DL�����ʒm�i�����炭�u�K�p�ς݁v�j�𐶐����Ȃ���������Ȃ��B
                    '���̏󋵂ł́A�^�ǂɂ����铖�Y�Ď��Ղ̃v���O�����̎�M��Ԃ��u�z�M���v
                    '�ł͂Ȃ��u����v���ɂȂ��Ă���Ǝv���邪�A�{���ɂ��̕ۏ؂�����̂�
                    '���؂��������悢�B
                    'TODO: ���̃A�v���ł́A�ǐS�Ɋ�Â��Ĕ�r���Ă��邪�A�{���̊Ď��Ղ�
                    'CAB�̓��e���r���āA�s��v�̏ꍇ�ɑҋ@�ʂւ̍ăR�s�[���s�����͕s���ł���B

                    '�v���O�����{�̂̔z�M���ʁi�u����v�܂��́u�K�p�ς݁v�j�����߂�B
                    Dim dataDeliveryResult As Byte = &H0

                    'TODO: DeliverGatePro�Ɠ����悤�ɁAHoldingKsbPrograms(0)���`�F�b�N����K�v�͂Ȃ����H
                    If oMonitorMachine.HoldingKsbPrograms(1) IsNot Nothing AndAlso _
                       oMonitorMachine.HoldingKsbPrograms(1).DataSubKind = oPenPro.DataSubKind AndAlso _
                       oMonitorMachine.HoldingKsbPrograms(1).DataVersion = oPenPro.DataVersion AndAlso _
                       StringComparer.OrdinalIgnoreCase.Compare(oMonitorMachine.HoldingKsbPrograms(1).DataHashValue, oPenPro.DataHashValue) = 0 Then
                        '�K�p���̂��̂Ɠ������̂�z�M���邱�ƂɂȂ�ꍇ�́A
                        '�z�M���ʂ��u�K�p�ς݁v�Ƃ���B
                        Log.Warn(sMonitorMachineId, "���Y�@��ɑ΂��Ă͓��Y�v���O������K�p�ς݁i���z�M�ς݁j�ł��B�v���O�����{�̂̍Ĕz�M�͍s���܂���B")
                        dataDeliveryResult = &HF
                    End If

                    If dataDeliveryResult = &H0 Then
                        If Not oPenPro.ApplicableDate.Equals("19000101") AndAlso _
                           String.CompareOrdinal(oPenPro.ApplicableDate, oPenPro.RunnableDate) < 0 Then
                            Log.Error(sMonitorMachineId, "�v���O�����̓��싖�����K�p���ȍ~�ɐݒ肳��Ă��܂��B�z�M�͍s���܂���B")
                            dataDeliveryResult = &HC
                            listDeliveryResult = &H1 'TODO: �K�p���X�g�̔z�M���ʂ͂R��ނ����Ȃ��B�{���͓K�p���X�g��z�M����̂�������Ȃ��B
                        End If
                    End If

                    '�Ď��Ճv���O�����ێ���Ԃ��X�V����B
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
                        Log.Info(sMonitorMachineId, "���Y�@��̑ҋ@�ʂɑ΂��ē��Y�v���O�����{�̂̔z�M���s���܂����B")
                        Log.Info(sMonitorMachineId, "���Y�@��̑ҋ@�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B")
                    ElseIf listDeliveryResult = &H0 Then
                        If oMonitorMachine.HoldingKsbPrograms(0).DataSubKind = oPenPro.DataSubKind AndAlso _
                           oMonitorMachine.HoldingKsbPrograms(0).DataVersion = oPenPro.DataVersion Then
                            oMonitorMachine.HoldingKsbPrograms(0).ListVersion = oPenPro.ListVersion
                            oMonitorMachine.HoldingKsbPrograms(0).ListAcceptDate = oPenPro.ListAcceptDate
                            oMonitorMachine.HoldingKsbPrograms(0).ListDeliverDate = d
                            oMonitorMachine.HoldingKsbPrograms(0).ApplicableDate = oPenPro.ApplicableDate
                            oMonitorMachine.HoldingKsbPrograms(0).ListContent = oPenPro.ListContent
                            oMonitorMachine.HoldingKsbPrograms(0).ListHashValue = oPenPro.ListHashValue
                            'TODO: oMonitorMachine.HoldingKsbPrograms(0).ListVersion���X�V���Ă��A
                            '��{�I�ɊĎ��Ճv���O�����o�[�W�������ɕω��͖����͂��Ȃ̂ŁA
                            '�ȉ��͍s��Ȃ������悢��������Ȃ��B
                            'TODO: ���̃A�v���ł́A�ҋ@�ʂɂ���K�p���X�g�ɂ����Ӗ�������
                            '���̂Ƃ��āA�Ď��Ճv���O�����o�[�W�������ɃZ�b�g���Ă��邪�A
                            '���������{���̊Ď��Ղ��ǂ��ł��邩�͂킩��Ȃ��B
                            isHoldingProUpdated = True
                            Log.Warn(sMonitorMachineId, "���Y�@��̓K�p�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B���̓K�p���͈Ӗ��������܂���̂Œ��ӂ��Ă��������B")
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
                            Log.Info(sMonitorMachineId, "���Y�@��̑ҋ@�ʂɑ΂��ē��Y�K�p���X�g�̔z�M���s���܂����B")
                        Else
                            'TODO: �悭���؂��Ȃ��Ƃ��蓾��P�[�X���ǂ���������Ȃ��B
                            Log.Error(sMonitorMachineId, "���Y�@��ɂ����āA���Y�K�p���X�g�ɕR�Â��v���O�����{�̂�����܂���B�K�p���X�g�̔z�M�͍s���܂���B")
                            listDeliveryResult = &H1
                        End If
                    End If

                    '�v���O�����{�̂Ɋւ���#KsbProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
                    CreateFileOfKsbProDlReflectReq( _
                       &H22, _
                       oPenPro.DataVersion, _
                       dataDeliveryResult, _
                       sMonitorMachineId, _
                       sMachineDir)
                End If
            End If

            'NOTE: ���̔z�M�̑O�ɒ��ړ��������{�����ꍇ�ȂǁA�Ď��ՂɓK�p���X�g��
            '���݂��Ȃ��ꍇ�́A���L���s��Ȃ��B
            '�����̉��D�@�V�X�e���̋������i�ǂ������Ɋ֌W�Ȃ��j�����ɍČ�����B
            If latestListHashValue IsNot Nothing Then
                '�K�p���X�g�Ɋւ���#KsbProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
                CreateFileOfKsbProDlReflectReq( _
                   &H49, _
                   oPenPro.ListVersion, _
                   listDeliveryResult, _
                   sMonitorMachineId, _
                   sMachineDir)
            Else
                Log.Warn(sMonitorMachineId, "���Y�@�킪�K�p���X�g��ێ����Ă��Ȃ��������߁A�K�p���X�g��DL�����ʒm�͍쐬���܂���ł����B����͎������D�@�V�X�e���̐��������ł��B")
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

        '�ҋ@�ʂɃv���O������ێ����Ă��邩�`�F�b�N���A
        '���̓K�p�����^�p���ȑO�ł���΁A�K�p�ʂɈړ�����B
        '�܂��A�v���O�����ێ���Ԃ��X�V�����ꍇ�́A
        'sContextDir��KsbProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AKsbProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��̂��폜����B

        DeleteFiles(sMonitorMachineId, sContextDir, "KsbProVerInfo_*.dat")

        Dim d As DateTime = DateTime.Now
        Dim sServiceDate As String = EkServiceDate.GenString(d)

        If oMonitorMachine.HoldingKsbPrograms(1) Is Nothing Then
            Log.Debug(sMonitorMachineId, "���Y�@��ɂ́A�K�p�҂��̃v���O����������܂���B")
        ElseIf oMonitorMachine.HoldingKsbPrograms(1).ListHashValue IsNot Nothing AndAlso _
               String.CompareOrdinal(oMonitorMachine.HoldingKsbPrograms(1).ApplicableDate, sServiceDate) > 0 Then
            Log.Warn(sMonitorMachineId, "���Y�@��ɂ́A�K�p�҂��̃v���O����������܂����A�K�p���O�ł��邽�߁A�K�p���܂���B")
        ElseIf String.CompareOrdinal(oMonitorMachine.HoldingKsbPrograms(1).RunnableDate, sServiceDate) > 0 Then
            Log.Warn(sMonitorMachineId, "���Y�@��ɂ́A�K�p�҂��̃v���O����������܂����A���싖���O�ł��邽�߁A�K�p���܂���B")
        Else
            oMonitorMachine.HoldingKsbPrograms(0) = oMonitorMachine.HoldingKsbPrograms(1)
            oMonitorMachine.HoldingKsbPrograms(0).ApplyDate = d
            oMonitorMachine.HoldingKsbPrograms(1) = Nothing
            Log.Info(sMonitorMachineId, "���Y�@��ɂ����āA�K�p�҂��̃v���O������K�p���܂����B")
            CreateFileOfKsbProVerInfo(sMonitorMachineId, oMonitorMachine, sContextDir)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMonitorMachine)
        End If

        Return True
    End Function

    Public Function ClearUpboundData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�@��̕ێ�������f�[�^���N���A���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
            Log.Info(sMonitorMachineId, "�N���A���������܂����B")
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Return True
    End Function

    Public Function StoreRandFaultData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�����_���ُ�f�[�^�𐶐����Ď��W�p�ɒ~�ς��܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine�܂���oMonitorMachine.TermMachines�ɐݒ肳��Ă�����Ɨ��������ƂɈُ�f�[�^�𐶐����A
        'sMonitorMachineDir��#FaultDataForPassiveUll.dat�ɒǋL����B

        Dim termCount As Integer = oMonitorMachine.TermMachines.Count

        'NOTE: ���W�����i12���ԁj������ő�300�l�i����150�l�j�̗��p�҂��P�̉��D�@�Ŗ����N�����z��ł���B
        'TODO: ���b�V�����̓����w�Ȃǂ͂����Ƒ�����������Ȃ����A
        '�S�w���ςōl����΂����Ə��Ȃ��Ǝv���邽�߁A�������e�ɉ�����
        '�����\�ɂ��������悢�B�f�[�^�O���b�h�Ɂu�l�����x�v�I�ȍ��ځi�l��
        '�ҏW�\�j��p�ӂ���ȂǁB
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
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB6, recCount, recLen, now, oOutputStream)
                Else
                    Dim totalRecCount As Integer = CInt((fileLen \ recLen) - 1) + recCount
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB6, totalRecCount, recLen, now, oOutputStream)
                    oOutputStream.Seek(0, SeekOrigin.End)
                End If

                For i As Integer = 1 To recCount
                    Dim oBytes(recLen - 1) As Byte

                    Dim t As DateTime = prevTime.AddSeconds(span * i / recCount)
                    Dim termIndex As Integer = Rand.Next(-1, termCount)
                    If termIndex = -1 Then
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �w�R�[�h", GetHypStationOf(sMonitorMachineId), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ��������", t.ToString("yyyyMMddHHmmss"), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �R�[�i�[", GetCornerOf(sMonitorMachineId), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", GetUnitOf(sMonitorMachineId), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", "0", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �o�[�W����", "01", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("�f�[�^�����O�X", "780", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��������", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("���@�ԍ�", "00", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("�ʘH����", FaultDataUtil.CreatePassDirectionValue(oMonitorMachine.LatchConf), oBytes)

                        'Dim errorcdIndex As Integer = Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)
                        'FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
                        'FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("�ُ퍀�� �\���f�[�^"), Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

                        Dim sErrorCode As String = Config.KsbFaultDataErrorCodeItems.Rows(Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
                        FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", sErrorCode, oBytes)

                        Dim sErrorText As String = Nothing
                        If Config.KsbFaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", sErrorText, oBytes)
                        End If
                        If Config.KsbFaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("�S�����\�� �\���f�[�^", sErrorText, oBytes)
                        End If
                        If Config.KsbFaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("�ϕ\���� �\���f�[�^", sErrorText, oBytes)
                        End If
                        If Config.KsbFaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("���u���e �\���f�[�^", sErrorText, oBytes)
                        End If
                    Else
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �w�R�[�h", GetHypStationOf(oTermEntries(termIndex).Key), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ��������", t.ToString("yyyyMMddHHmmss"), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �R�[�i�[", GetCornerOf(oTermEntries(termIndex).Key), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", "0", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", "0", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �o�[�W����", "01", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("�f�[�^�����O�X", "780", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("��������", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
                        FaultDataUtil.SetFieldValueToBytes("���@�ԍ�", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
                        FaultDataUtil.SetFieldValueToBytes("�ʘH����", FaultDataUtil.CreatePassDirectionValue(oTermEntries(termIndex).Value.LatchConf), oBytes)

                        'Dim errorcdIndex As Integer = Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)
                        'FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
                        'FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("�ُ퍀�� �\���f�[�^"), Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

                        Dim sErrorCode As String = Config.FaultDataErrorCodeItems.Rows(Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
                        FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", sErrorCode, oBytes)

                        Dim sErrorText As String = Nothing
                        If Config.FaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", sErrorText, oBytes)
                        End If
                        If Config.FaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("�S�����\�� �\���f�[�^", sErrorText, oBytes)
                        End If
                        If Config.FaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("�ϕ\���� �\���f�[�^", sErrorText, oBytes)
                        End If
                        If Config.FaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                            FaultDataUtil.SetFieldValueToBytes("���u���e �\���f�[�^", sErrorText, oBytes)
                        End If
                    End If

                    FaultDataUtil.AdjustByteCountField("�ُ퍀��", oBytes)
                    FaultDataUtil.AdjustByteCountField("�S�����\��", oBytes)
                    FaultDataUtil.AdjustByteCountField("�ϕ\����", oBytes)
                    FaultDataUtil.AdjustByteCountField("���u���e", oBytes)

                    oOutputStream.Write(oBytes, 0, oBytes.Length)

                    If termIndex = -1 Then
                        oMonitorMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
                        oMonitorMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                    Else
                        oTermEntries(termIndex).Value.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
                        oTermEntries(termIndex).Value.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                    End If
                Next i
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �ւ̃��R�[�h�ǉ������s���܂����B", ex)
            Return False
        End Try
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �� [" & recCount.ToString() & "] ���R�[�h��ǉ����܂����B")

        'NOTE: �Ď��@���X�̒[���̍s�ɂ��āA���x���X�V���邱�ƂɂȂ�\�����������߁A
        '�����ŊĎ��@��ƑS�[���̍s����x�����X�V���邱�Ƃɂ��Ă���B
        UpdateTable2OnMonitorStateChanged(sMonitorMachineDir, oMonitorMachine)
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            UpdateTable2OnTermStateChanged(sMonitorMachineDir, oTermEntry.Key, oTermEntry.Value)
        Next oTermEntry

        Return True
    End Function

    Public Function SendRandFaultData(ByVal sMonitorMachineId As String) As Boolean
        Log.Info(sMonitorMachineId, "�����_���ُ�f�[�^�𐶐����������M���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        Dim oMonitorMachine As Machine = UiState.Machines(sMonitorMachineId)

        'oMonitorMachine.TermMachines�ɐݒ肳��Ă�����Ɨ��������ƂɈُ�f�[�^�𐶐����A
        '�V�~�����[�^�{�̂ɑ��M������B

        Dim termCount As Integer = oMonitorMachine.TermMachines.Count

        Dim oTermEntries(termCount - 1) As KeyValuePair(Of String, TermMachine)
        CType(oMonitorMachine.TermMachines, ICollection(Of KeyValuePair(Of String, TermMachine))).CopyTo(oTermEntries, 0)

        Dim oBytes(FaultDataUtil.RecordLengthInBytes - 1) As Byte

        Dim t As DateTime = DateTime.Now
        Dim termIndex As Integer = Rand.Next(-1, termCount)
        If termIndex = -1 Then
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �w�R�[�h", GetHypStationOf(sMonitorMachineId), oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ��������", t.ToString("yyyyMMddHHmmss"), oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �R�[�i�[", GetCornerOf(sMonitorMachineId), oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", GetUnitOf(sMonitorMachineId), oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", "0", oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �o�[�W����", "01", oBytes)
            FaultDataUtil.SetFieldValueToBytes("�f�[�^�����O�X", "780", oBytes)
            FaultDataUtil.SetFieldValueToBytes("��������", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
            FaultDataUtil.SetFieldValueToBytes("���@�ԍ�", "00", oBytes)
            FaultDataUtil.SetFieldValueToBytes("�ʘH����", FaultDataUtil.CreatePassDirectionValue(oMonitorMachine.LatchConf), oBytes)

            'Dim errorcdIndex As Integer = Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)
            'FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
            'FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("�ُ퍀�� �\���f�[�^"), Config.KsbFaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

            Dim sErrorCode As String = Config.KsbFaultDataErrorCodeItems.Rows(Rand.Next(0, Config.KsbFaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
            FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", sErrorCode, oBytes)

            Dim sErrorText As String = Nothing
            If Config.KsbFaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", sErrorText, oBytes)
            End If
            If Config.KsbFaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("�S�����\�� �\���f�[�^", sErrorText, oBytes)
            End If
            If Config.KsbFaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("�ϕ\���� �\���f�[�^", sErrorText, oBytes)
            End If
            If Config.KsbFaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("���u���e �\���f�[�^", sErrorText, oBytes)
            End If
        Else
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A6", oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �w�R�[�h", GetHypStationOf(oTermEntries(termIndex).Key), oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ��������", t.ToString("yyyyMMddHHmmss"), oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �R�[�i�[", GetCornerOf(oTermEntries(termIndex).Key), oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ ���@", "0", oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", "0", oBytes)
            FaultDataUtil.SetFieldValueToBytes("��{�w�b�_�[ �o�[�W����", "01", oBytes)
            FaultDataUtil.SetFieldValueToBytes("�f�[�^�����O�X", "780", oBytes)
            FaultDataUtil.SetFieldValueToBytes("��������", t.ToString("yyyyMMddHHmmss") & "00", oBytes)
            FaultDataUtil.SetFieldValueToBytes("���@�ԍ�", GetUnitOf(oTermEntries(termIndex).Key), oBytes)
            FaultDataUtil.SetFieldValueToBytes("�ʘH����", FaultDataUtil.CreatePassDirectionValue(oTermEntries(termIndex).Value.LatchConf), oBytes)

            'Dim errorcdIndex As Integer = Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)
            'FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Key"), oBytes)
            'FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", MyUtility.GetRightPaddedValue(FaultDataUtil.Field("�ُ퍀�� �\���f�[�^"), Config.FaultDataErrorCodeItems.Rows(errorcdIndex).Field(Of String)("Value").Substring(9), &H20), oBytes)

            Dim sErrorCode As String = Config.FaultDataErrorCodeItems.Rows(Rand.Next(0, Config.FaultDataErrorCodeItems.Rows.Count)).Field(Of String)("Key")
            FaultDataUtil.SetFieldValueToBytes("�G���[�R�[�h", sErrorCode, oBytes)

            Dim sErrorText As String = Nothing
            If Config.FaultDataErrorOutlines.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("�ُ퍀�� �\���f�[�^", sErrorText, oBytes)
            End If
            If Config.FaultDataErrorLabels.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("�S�����\�� �\���f�[�^", sErrorText, oBytes)
            End If
            If Config.FaultDataErrorDetails.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("�ϕ\���� �\���f�[�^", sErrorText, oBytes)
            End If
            If Config.FaultDataErrorGuidances.TryGetValue(sErrorCode, sErrorText) = True Then
                FaultDataUtil.SetFieldValueToBytes("���u���e �\���f�[�^", sErrorText, oBytes)
            End If
        End If

        FaultDataUtil.AdjustByteCountField("�ُ퍀��", oBytes)
        FaultDataUtil.AdjustByteCountField("�S�����\��", oBytes)
        FaultDataUtil.AdjustByteCountField("�ϕ\����", oBytes)
        FaultDataUtil.AdjustByteCountField("���u���e", oBytes)

        If termIndex = -1 Then
            oMonitorMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
            oMonitorMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineDir, oMonitorMachine)
        Else
            oTermEntries(termIndex).Value.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
            oTermEntries(termIndex).Value.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
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
        Log.Info(sMonitorMachineId, "�@�� [" & sSourceMachineId & "] �ُ̈�f�[�^���Ď��W�p�ɒ~�ς��܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB6, 1, recLen, now, oOutputStream)
                Else
                    Dim totalRecCount As Integer = CInt((fileLen \ recLen) - 1) + 1
                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HB6, totalRecCount, recLen, now, oOutputStream)
                    oOutputStream.Seek(0, SeekOrigin.End)
                End If

                oOutputStream.Write(oBytes, 0, oBytes.Length)
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �ւ̃��R�[�h�ǉ������s���܂����B", ex)
            Return False
        End Try
        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �Ƀ��R�[�h��ǉ����܂����B")

        If sSourceMachineId = sMonitorMachineId Then
            Dim oMachine As Machine = UiState.Machines(sMonitorMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMachine)
        Else
            Dim oMachine As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnTermStateChanged(sMonitorMachineId, sSourceMachineId, oMachine)
        End If
        Return True
    End Function

    Public Function SendFaultData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()) As Boolean
        Log.Info(sMonitorMachineId, "�@�� [" & sSourceMachineId & "] �ُ̈�f�[�^�𑦎����M���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        If sSourceMachineId = sMonitorMachineId Then
            Dim oMachine As Machine = UiState.Machines(sMonitorMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            UpdateTable2OnMonitorStateChanged(sMonitorMachineId, oMachine)
        Else
            Dim oMachine As TermMachine = UiState.Machines(sMonitorMachineId).TermMachines(sSourceMachineId)
            oMachine.FaultSeqNumber = UInteger.Parse(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ �V�[�P���XNo", oBytes))
            oMachine.FaultDate = DateTime.ParseExact(FaultDataUtil.GetFieldValueFromBytes("��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
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
        Log.Info(sMonitorMachineId, "�ғ��ێ�f�[�^�������_���ɍX�V���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
                Return False
            End If
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "Unwelcome Exception caught.", ex)
            Return False
        End Try

        'sMonitorMachineDir��#KadoData.dat�̑S���R�[�h���X�V����B
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
                    Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̃T�C�Y���ُ�ł��B")
                    Return False
                End If

                Dim recCount As Integer = CInt((fileLen \ recLen) - 1)
                oStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                ExUpboundFileHeader.WriteToStream(&HA7, recCount, recLen, now, oStream)

                For Each oTerm As TermMachine In UiState.Machines(sMonitorMachineId).TermMachines.Values
                    For k As Integer = 0 To 1
                        oStream.Seek(recLen * oTerm.KadoSlot(k), SeekOrigin.Begin)
                        Dim pos As Integer = 0
                        Dim len As Integer = recLen
                        While pos < len
                            Dim readSize As Integer = oStream.Read(oBytes(k), pos, len - pos)
                            If readSize = 0 Then Exit While  'OPT: �O�̂��߂Ƀ`�F�b�N���Ă��邪�A�t�@�C�����r������Ă������A���蓾�Ȃ��͂��ł���A�s�v�B
                            pos += readSize
                        End While
                    Next k

                    For k As Integer = 0 To 1
                        If isHokurikuMode Then
                            KadoDataUtil073.SetFieldValueToBytes(k, "��{�w�b�_�[ ��������", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                            KadoDataUtil073.SetFieldValueToBytes(k, "��{�w�b�_�[ �V�[�P���XNo", MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber(k)).ToString(), oBytes(k))
                            KadoDataUtil073.SetFieldValueToBytes(k, "���ʕ� �W�v�I��(���W)����", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                        Else
                            KadoDataUtil.SetFieldValueToBytes(k, "��{�w�b�_�[ ��������", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                            KadoDataUtil.SetFieldValueToBytes(k, "��{�w�b�_�[ �V�[�P���XNo", MyUtility.GetNextSeqNumber(oTerm.KadoSeqNumber(k)).ToString(), oBytes(k))
                            KadoDataUtil.SetFieldValueToBytes(k, "���ʕ� �W�v�I��(���W)����", now.ToString("yyyyMMddHHmmss"), oBytes(k))
                        End If
                    Next k

                    If Rand.Next(0, 3) = 0 Then
                        If isHokurikuMode Then
                            Dim sOldDate As String = KadoDataUtil073.GetFieldValueFromBytes(0, "���ʕ� ���D���������_������", oBytes(0))
                            If sOldDate = "00000000000000" OrElse sOldDate < sYesterday Then
                                Dim sNewDate As String = yesterday.AddSeconds(Rand.Next(0, 24 * 60 * 60)).ToString("yyyyMMddHHmmss")
                                For k As Integer = 0 To 1
                                    KadoDataUtil073.SetFieldValueToBytes(k, "���ʕ� ���D���������_������", sNewDate, oBytes(k))
                                    KadoDataUtil073.SetFieldValueToBytes(k, "���ʕ� �W�D���������_������", sNewDate, oBytes(k))
                                Next k
                            End If
                        Else
                            Dim sOldDate As String = KadoDataUtil.GetFieldValueFromBytes(0, "���ʕ� ���D���������_������", oBytes(0))
                            If sOldDate = "00000000000000" OrElse sOldDate < sYesterday Then
                                Dim sNewDate As String = yesterday.AddSeconds(Rand.Next(0, 24 * 60 * 60)).ToString("yyyyMMddHHmmss")
                                For k As Integer = 0 To 1
                                    KadoDataUtil.SetFieldValueToBytes(k, "���ʕ� ���D���������_������", sNewDate, oBytes(k))
                                    KadoDataUtil.SetFieldValueToBytes(k, "���ʕ� �W�D���������_������", sNewDate, oBytes(k))
                                Next k
                            End If
                        End If
                    End If

                    For k As Integer = 0 To 1
                        If isHokurikuMode Then
                            For Each oField As XlsField In KadoDataUtil073.Fields(k)
                                If oField.MetaName.StartsWith("�W�v") AndAlso oField.MetaName.Substring(6) <> "�i�󂫁j" Then
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
                                If oField.MetaName.StartsWith("�W�v") AndAlso oField.MetaName.Substring(6) <> "�i�󂫁j" Then
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

                    'NOTE: UiState�ƃO���b�h�̍X�V�͉ғ��ێ�f�[�^���W�������ɍs���B
                Next oTerm
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̍X�V�����s���܂����B", ex)
            Return False
        End Try

        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���X�V���܂����B")
        Return True
    End Function

    Public Function UpdateKadoData(ByVal sMonitorMachineId As String, ByVal sSourceMachineId As String, ByVal oBytes As Byte()()) As Boolean
        Log.Info(sMonitorMachineId, "�@�� [" & sSourceMachineId & "] �̉ғ��ێ�f�[�^���X�V���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                    'OPT: ���L�̃P�[�X�͂��蓾�Ȃ��͂��ł���A�~���K�v���Ȃ��B
                    If oTerm.KadoSlot(k) = 0 Then
                        oTerm.KadoSlot(k) = slotCount
                        slotCount += 1
                    End If

                    oOutputStream.Seek(0, SeekOrigin.Begin)  'OPT: �s�v��������Ȃ��B
                    ExUpboundFileHeader.WriteToStream(&HA7, slotCount - 1, recLen, now, oOutputStream)

                    oOutputStream.Seek(recLen * oTerm.KadoSlot(k), SeekOrigin.Begin)
                    oOutputStream.Write(oBytes(k), 0, oBytes(k).Length)
                Next k
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̍X�V�����s���܂����B", ex)
            Return False
        End Try

        'NOTE: UiState�ƃO���b�h�̍X�V�͉ғ��ێ�f�[�^���W�������ɍs���B

        Log.Info(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] ���X�V���܂����B")
        Return True
    End Function

    Public Function CommitKadoData(ByVal sMonitorMachineId As String) As Boolean
        Dim isHokurikuMode As Boolean = GetStationOf(sMonitorMachineId).StartsWith("073")
        Log.Info(sMonitorMachineId, "�ғ��ێ�f�[�^�̎��W�����𔽉f���܂�...")

        Dim sSimWorkingDir As String = SimWorkingDirDialog.SelectedPath
        Dim sMonitorMachineDir As String
        Try
            Dim sModelDir As String = Path.Combine(sSimWorkingDir, Config.ModelPathInSimWorkingDir)
            sMonitorMachineDir = Path.Combine(sModelDir, GetMachineDirNameOf(sMonitorMachineId))
            If Not Directory.Exists(sMonitorMachineDir) Then
                Log.Error(sMonitorMachineId, "�Ď��@��̃f�B���N�g�� [" & sMonitorMachineDir & "] ���݂���܂���ł����B")
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
                    Log.Fatal(sMonitorMachineId, "�t�@�C�� [" & sFilePath & "] �̃T�C�Y���ُ�ł��B")
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
                        If readSize = 0 Then Exit While  'OPT: �O�̂��߂Ƀ`�F�b�N���Ă��邪�A�t�@�C�����r������Ă������A���蓾�Ȃ��͂��ł���A�s�v�B
                        pos += readSize
                    End While

                    Dim oTerm As TermMachine = oTerms(recIndex)
                    If oTerm IsNot Nothing Then
                        Dim k As Integer = kinds(recIndex)
                        If isHokurikuMode Then
                            oTerm.KadoSeqNumber(k) = UInteger.Parse(KadoDataUtil073.GetFieldValueFromBytes(k, "��{�w�b�_�[ �V�[�P���XNo", oBytes))
                            oTerm.KadoDate(k) = DateTime.ParseExact(KadoDataUtil073.GetFieldValueFromBytes(k, "��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                        Else
                            oTerm.KadoSeqNumber(k) = UInteger.Parse(KadoDataUtil.GetFieldValueFromBytes(k, "��{�w�b�_�[ �V�[�P���XNo", oBytes))
                            oTerm.KadoDate(k) = DateTime.ParseExact(KadoDataUtil.GetFieldValueFromBytes(k, "��{�w�b�_�[ ��������", oBytes), "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                        End If
                        UpdateTable2OnTermStateChanged(sMonitorMachineId, oTermKeys(recIndex), oTerm)  'OPT: ��ň�x�����s�������悢�B
                    End If
                Next recIndex
            End Using
        Catch ex As Exception
            Log.Fatal(sMonitorMachineId, "�ُ킪�������܂����B", ex)
            Return False
        End Try

        Log.Info(sMonitorMachineId, "���f���܂����B")
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
                Log.Info(sMonitorMachineId, "�V�~�����[�^�{�̂։��� [" & isCompleted.ToString() & "] ��ԐM���܂����B")
            Else
                Log.Info(sMonitorMachineId, "�V�~�����[�^�{�̂։��� [" & isCompleted.ToString() & "][" & sResult & "] ��ԐM���܂����B")
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
            Log.Info(sMonitorMachineId, "�V�~�����[�^�{�̂֗v�� [" & sVerb & "] ���s���܂����B")
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

        '�w�肳�ꂽ�Ď��Ղ̉��D�@�v���O�����ێ��󋵂���сA���Y�Ď��Ղ���
        '�e�[���ւ̉��D�@�v���O�����z�M�ۗ��󋵂����������A
        '�w�肳�ꂽ���D�@�v���O�����𓖊Y�Ď��Ղɓ�������i�ێ�������j�B
        '�܂��A���̊Ď��Քz���̑S���D�@�̉��D�@�v���O�����ێ��󋵂����������A
        '�w�肳�ꂽ���D�@�v���O�����������̑ҋ@�ʂɓ������A
        'sContextDir��GateProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AGateProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́A����X�V
        '���Ă��Ȃ����D�@�̂��́j���폜����B

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

        '�S�[���ɂ��ď������s���B
        For Each oTermEntry As KeyValuePair(Of String, TermMachine) In oMonitorMachine.TermMachines
            Dim oTerm As TermMachine = oTermEntry.Value

            oTerm.PendingPrograms.Clear()

            'TODO: ���L�̂悤�ȏ󋵂̏ꍇ�A�����̃V�X�e���ł͂ǂ��Ȃ�̂��H
            'If oTerm.McpStatusFromKsb <> &H0 Then
            '    Log.Warn(sMonitorMachineId, "�[�� [" & oTermEntry.Key & "] �ɂ��ẮA�吧��Ԃ�����ȊO�ɐݒ肳��Ă��邽�߁A�z�M������ۗ����܂��B")
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
            Log.Info(sMonitorMachineId, "�[�� [" & oTermEntry.Key & "] �̑ҋ@�ʂɑ΂��ĉ��D�@�v���O�����𒼐ړ������܂����B")

            '�v���O�����{�̂Ɋւ���#GateProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
            'TODO: ���ړ����̒���ɉ^�ǂ���z�M���s�����ۂ̂������ȋ�������A����͖����Ɛ������Ă��邪�A�L�����肩�łȂ��B
            '���@������������Ȃ�A�^�ǓI�ɂ����������č\��Ȃ��̂ŁA���@�ɍ��킹��ׂ��B
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

        '�w�肳�ꂽ�Ď��Փ����ւ̊Ď��Ճv���O�����z�M�ۗ��󋵂���сA
        '�w�肳�ꂽ�Ď��Փ����̊Ď��Ճv���O�����ێ��󋵂�����������A
        '����ɁA�w�肳�ꂽ�Ď��Ճv���O�����𓖊Y�Ď��Փ����̑ҋ@�ʂɓ������A
        'sContextDir��KsbProVerInfo_RRRSSSCCCCUU.dat���쐬����B
        '�Ȃ��AKsbProVerInfo_RRRSSSCCCCUU.dat�ɂ��ẮA
        '�ߋ��̂��́i����̔z�M�Ɩ��֌W�Ȃ��́j���폜����B

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

        Log.Info(sMonitorMachineId, "�@�� [" & sMonitorMachineId & "] �̑ҋ@�ʂɑ΂��ĊĎ��Ճv���O�����𒼐ړ������܂����B")

        '�v���O�����{�̂Ɋւ���#KsbProDlReflectReq_RRRSSSCCCCUU_N.dat���쐬����B
        'TODO: ���ړ����̒���ɉ^�ǂ���z�M���s�����ۂ̂������ȋ�������A����͖����Ɛ������Ă��邪�A�L�����肩�łȂ��B
        '���@������������Ȃ�A�^�ǓI�ɂ����������č\��Ȃ��̂ŁA���@�ɍ��킹��ׂ��B
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
