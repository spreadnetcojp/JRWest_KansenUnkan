' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/04/10  (NES)����  ������ԕ�Ή��ɂĐV�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Threading

Imports JR.ExOpmg.Common
Imports JR.ExOpmg.DBCommon

''' <summary>
''' ���p�f�[�^�o�^�X���b�h�B
''' </summary>
Public Class MyRecorder

#Region "�萔��ϐ�"
    '���p�f�[�^�e�[�u�����̏���
    Protected Const StaFormat As String = "%3R%3S"

    '�X���b�h
    Private oThread As Thread

    '�Ώۉw
    Private sTargetSta As String

    '�������f�[�^���i�[����Ă���f�B���N�g���̃p�X
    Private sInputDirPath As String

    '�o�^�ς݃f�[�^���i�[����f�B���N�g���̃p�X
    Private sOutputDirPath As String

    '���t�ʂ̃f�B���N�g�����쐬����K�v�����邩
    Private needsDateDir As Boolean

    '���莞�Ԃ����Z���Ԋu��SystemTick���������ށi0�`0xFFFFFFFF�j
    Private _LastPulseTick As Long

    '�e�X���b�h����̏I���v��
    Private _IsQuitRequest As Integer
#End Region

#Region "�R���X�g���N�^"
    Public Sub New( _
       ByVal sThreadName As String, _
       ByVal targetEkCode As EkCode, _
       ByVal needsDateDir As Boolean)

        Me.sTargetSta = targetEkCode.ToString(StaFormat)
        CreateTables()
        CreateProcs()

        Dim sBaseDirPath As String = Utility.CombinePathWithVirtualPath(Config.RiyoDataDirPath, targetEkCode.ToString(Config.RiyoDataStationBaseDirNameFormat))
        Me.sInputDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataInputDirPathInStationBase)
        Me.sOutputDirPath = Utility.CombinePathWithVirtualPath(sBaseDirPath, Config.RiyoDataOutputDirPathInStationBase)

        Me.needsDateDir = needsDateDir

        Me.oThread = New Thread(AddressOf Me.Task)
        Me.oThread.Name = sThreadName
        Me.LastPulseTick = 0
        Me.IsQuitRequest = False
    End Sub
#End Region

#Region "�v���p�e�B"
    'NOTE: �q�X���b�h���J�n���Ĉȍ~��_LastPulseTick�́A�J�[�l��������r������
    '�Ȃ��ɁA�q�X���b�h�ŏ������݁A�e�X���b�h�œǂݏo�����Ƃɂ��Ă���B
    '�Ȃ��A_LastPulseTick�́A���ۓI�ɂ́Ax86-64�v���Z�b�T�ɂ�����ʏ��
    '�]�����߂P�Łi�����A���Ȃ��Ƃ������ɂ�镪�f�͖����Ɂj�S�̂�ǂށi�����j
    '���Ƃ��\�ȃT�C�Y�ł���A�����R�A�ɂ��o�X�I�y���[�V�������x���ł�
    '�ǂݏ�������������邱�Ƃ̂Ȃ��ʒu�ɔz�u����Ă���Ǝv����B�܂��A
    '�������݂��s���X���b�h���P�ł��邽�߁A�������݂̋����ɂ��ẴP�A��
    '�s�v�ł���B�������Ȃ���AThread�N���X��VolatileRead��VolatileWrite��
    '�g�p���Ȃ����j�Ƃ���B�����̃��\�b�h�͕s���ȓ�����Ӑ}���Ă���
    '�킯�ł͂Ȃ��i���Ƃ��΁AVolatileWrite�́AVolatileRead���g�p����ʂ�
    '�X���b�h����̉�����ۏ؂��Ă��Ă��A�s���Ɍ����鏑��������ۏ؂��Ă���
    '�킯�ł͂Ȃ��j�Ǝv����̂ɑ΂��A�����̕ϐ��Ɋi�[����l�́A�ꉞ�S�o�C�g
    '�ňӖ��𐬂����̂ł��邽�߂ł���B_LastPulseTick�́A�����Ď��Ɏg������
    '�̏d�v�ȕϐ��ł��邩��A�p�t�H�[�}���X��̂�قǂ̕K�v�����Ȃ�����
    '�iLOCK�M���ɂ��o�X�̐��\�ቺ������ƂȂ�悤�ȏ󋵂ɂȂ�Ȃ�����j
    'VolatileRead��VolatileWrite�ɕύX���Ă͂Ȃ�Ȃ��B
    Public Property LastPulseTick() As Long
        Get
            Return Interlocked.Read(_LastPulseTick)
        End Get

        Protected Set(ByVal tick As Long)
            Interlocked.Exchange(_LastPulseTick, tick)
        End Set
    End Property

    Private Property IsQuitRequest() As Boolean
        Get
            Return CBool(Thread.VolatileRead(_IsQuitRequest))
        End Get

        Set(ByVal val As Boolean)
            Thread.VolatileWrite(_IsQuitRequest, CInt(val))
        End Set
    End Property
#End Region

#Region "�e�X���b�h�p���\�b�h"
    Public Sub Start()
        LastPulseTick = TickTimer.GetSystemTick()
        oThread.Start()
    End Sub

    Public Sub Quit()
        IsQuitRequest = True
        oThread.Interrupt()
    End Sub

    Public Sub Join()
        oThread.Join()
    End Sub

    Public Function Join(ByVal millisecondsTimeout As Integer) As Boolean
        Return oThread.Join(millisecondsTimeout)
    End Function

    'NOTE: ���̃N���X�ɖ�肪�Ȃ�����AQuit()�ōς܂���ׂ��ł���B
    Public Sub Abort()
        oThread.Abort()
    End Sub

    Public ReadOnly Property ThreadState() As ThreadState
        Get
            Return oThread.ThreadState
        End Get
    End Property
#End Region

#Region "���\�b�h"
    Private Sub Task()
        Dim spanMax As New TimeSpan(0, 0, 0, 0, Config.RecordingIntervalTicks)
        Dim sLastOutputDir As String = ""
        Try
            Log.Info("The recorder thread started.")

            '�A�N�Z�X����\��̃f�B���N�g���ɂ��āA������΍쐬���Ă����B
            'NOTE: �K���T�u�f�B���N�g���̍쐬����s�����ƂɂȂ���̂ɂ��ẮA�ΏۊO�Ƃ���B
            Directory.CreateDirectory(sInputDirPath)
            Directory.CreateDirectory(sOutputDirPath)

            Dim nextRecordingTime As DateTime = DateTime.Now.AddMilliseconds(Config.RecordingIntervalTicks)
            While Not IsQuitRequest
                LastPulseTick = TickTimer.GetSystemTick()

                Dim span As TimeSpan = nextRecordingTime - DateTime.Now
                If span < TimeSpan.Zero Then
                    span = TimeSpan.Zero
                ElseIf span > spanMax Then
                    span = spanMax
                End If

                '�����̌o�߂܂���Interrupt��҂B
                Try
                    Thread.Sleep(span)
                Catch ex As ThreadInterruptedException
                    '���[�v�擪�ɖ߂��āA���[�v���甲����B
                    Continue While
                End Try
                nextRecordingTime = DateTime.Now.AddMilliseconds(Config.RecordingIntervalTicks)

                '����o�^����t�@�C���̈ꗗ���쐬����B
                Dim sFiles As String() = Directory.GetFiles(sInputDirPath)
                Dim validCount As Integer = 0
                For i As Integer = 0 To sFiles.Length - 1
                    If UpboundDataPath2.IsMatch(sFiles(i)) Then
                        validCount += 1
                        If Config.RecordingFileCountAtOnce > 0 AndAlso _
                           validCount >= Config.RecordingFileCountAtOnce Then Exit For
                    Else
                        sFiles(i) = Nothing
                    End If
                Next i
                If validCount = 0 Then Continue While

                '�f�[�^�x�[�X�ւ̓o�^���s���B
                Dim completed As Boolean = False
                Dim procCount As Integer = 0
                Dim dbCtl As New DatabaseTalker()
                Try
                    dbCtl.ConnectOpen()
                    dbCtl.TransactionBegin()

                    dbCtl.ExecuteSQLToWrite("EXEC uspPrepareToImportRiyoData" & sTargetSta)
                    For Each sFilePath As String In sFiles
                        If sFilePath Is Nothing Then Continue For
                        LastPulseTick = TickTimer.GetSystemTick()

                        Log.Info("�t�@�C��[" & Path.GetFileName(sFilePath) & "]�̓o�^���s���܂�...")
                        dbCtl.ExecuteSQLToWrite("EXEC uspImportRiyoData" & sTargetSta & " '" & UpboundDataPath2.GetFormatCode(sFilePath) & "','" & sFilePath & "'")

                        procCount += 1
                        If procCount = validCount Then Exit For
                        If IsQuitRequest Then Exit For
                    Next sFilePath
                    dbCtl.ExecuteSQLToWrite("EXEC uspDispatchRiyoData" & sTargetSta)

                    dbCtl.TransactionCommit()
                    completed = True
                Catch ex As Exception
                    Log.Fatal("Unwelcome Exception caught.", ex)
                    dbCtl.TransactionRollBack()

                    'TODO: ���̃v���Z�X�́A���̃P�[�X�ł������Ȃ��悤�ɂ��Ă���
                    '���A����䂦�ɁA�u�v���Z�X�ُ�I����SNMP TRAP�v���������Ȃ�
                    '���߁A��肪�������Ă���i�����ŉ������錩���݂��Ȃ��j���Ƃ�
                    '�O���ɓ`���Ȃ��Ƃ������ԂɂȂ肩�˂Ȃ��B
                    '���Ƃ��΁ASQL Server���͓̂��삵�Ă��邪�A�f�B�X�N�t���ȂǂŁA
                    'Insert�����s����ꍇ�Ȃǂ́A���̂悤�Ȏ��ԂɂȂ邩������Ȃ��B
                    '����āA������SNMP TRAP�𔭐�������ׂ���������Ȃ��B
                    '�˂��������A�u�v���Z�X�ُ�I����SNMP TRAP�v�́A�v���Z�X�}�l�[�W��
                    '�����Ƃ��ꂽ�Ƃ��̂��߂ɒǉ�������̂ł���A���̎q�v���Z�X��
                    '�������ꍇ�́A�Ή��̑ΏۊO�ł���Ƃ̂���...�B�]���ǂ���A
                    '�v���Z�X�}�l�[�W�����q�v���Z�X��K���ċN��������΁A�����
                    '�悢�i�ċN�����J��Ԃ���邾���ŁA���ǉ����s���Ȃ�...�Ƃ���
                    '�󋵂͑z�肵�Ȃ��j�B

                    'NOTE: ���g���C�͎��̎����܂ő҂B
                Finally
                    dbCtl.ConnectClose()
                End Try

                If completed Then
                    Dim sDateDirPath As String
                    If needsDateDir Then
                        sDateDirPath = Path.Combine(sOutputDirPath, EkServiceDate.GenString(DateTime.Now))
                        If sLastOutputDir <> sDateDirPath Then
                            Directory.CreateDirectory(sDateDirPath)
                            sLastOutputDir = sDateDirPath
                        Else
                            sDateDirPath = sLastOutputDir
                        End If
                    Else
                        sDateDirPath = sOutputDirPath
                    End If

                    'NOTE: DB�o�^�̓r����Quit���ꂽ�Ƃ��Ă��A�����ɂ����āAprocCount�͕K��1�ȏ�ł���B
                    For Each sFilePath As String In sFiles
                        If sFilePath Is Nothing Then Continue For
                        LastPulseTick = TickTimer.GetSystemTick()

                        'NOTE: ���S���ꎞ���ɓ���@�킩��A���I�Ɂi���萔���́j���p�f�[�^��ULL�����ꍇ�́A
                        '�ʐM�v���Z�X��BUSY��NAK��ԐM���邱�ƂŁA���M�@�푤�ŏ����́i�傫�ȁj�t�@�C����
                        '����������悤�ɂȂ��Ă���B���̃v���Z�X��InputDirPath���痘�p�f�[�^���ړ�����
                        '����A���S���ꎞ���ɓ���@�킩�痘�p�f�[�^����M����΁A�ړ��ς݂̂��̂����萔��
                        '�B���Ă���ꍇ�́A�����Ɠ��ꎞ���̃t�@�C������t�^���邱�ƂɂȂ邪�A���̐���
                        '�m�ꂽ���̂ƂȂ�͂��ł���B�܂��A���p�f�[�^�ɂ��ẮA�^�p�゠�蓾�Ȃ��p�x��
                        'ULL���ꂽ����Ƃ����āA�o�^����Ɏ̂Ă�Ƃ����̂́A�������ɕ|���B����āA�����ł́A
                        '�ړ���̖��O�ɂ�����}�Ԃ������𒴂���ꍇ���A�̂Ă�Ƃ��������Ƃ͂��Ȃ��B

                        '�t�@�C����V�p�X�Ɉړ�����B
                        'NOTE: �t�@�C���͏����\�Ƃ����O��ł���B
                        Dim sDestPath As String = UpboundDataPath2.Gen(sDateDirPath, Path.GetFileName(sFilePath))
                        File.Move(sFilePath, sDestPath)
                        Log.Info("�t�@�C����[" & sDestPath & "]�Ɉړ����܂����B")

                        procCount -= 1
                        If procCount = 0 Then Exit For
                    Next sFilePath
                End If

            End While
            Log.Info("Quit requested by manager.")

        Catch ex As Exception
            Log.Fatal("Unwelcome Exception caught.", ex)
            'NOTE: TRAP�����i�܂��͎��W�f�[�^��L�e�[�u���ւ̓o�^�j�́A
            '�v���Z�X�}�l�[�W�����s���̂ŁA�����ł͕s�v�ł���B
        End Try
    End Sub


    Private Sub CreateTables()
        Dim sPath As String = Path.Combine(Config.RiyoDataImporterFilesBasePath, "RiyoDataTableCreator.sql")
        Dim sSQL As String
        Using oReader As StreamReader = New StreamReader(sPath, Encoding.GetEncoding(932))
            sSQL = oReader.ReadToEnd() _
                   .Replace("${Sta}", sTargetSta) _
                   .Replace("${RiyoDataDatabaseName}", Config.RiyoDataDatabaseName) _
                   .Replace("${ShiteiDataDatabaseName}", Config.ShiteiDataDatabaseName)
        End Using

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            For Each sBatch As String In sSQL.Split(New String() {"${GO}"}, StringSplitOptions.RemoveEmptyEntries)
                dbCtl.ExecuteSQLToWrite(sBatch)
            Next sBatch
            dbCtl.TransactionCommit()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub

    Private Sub CreateProcs()
        Dim sPath As String = Path.Combine(Config.RiyoDataImporterFilesBasePath, "RiyoDataProcCreator.sql")
        Dim sSQL As String
        Using oReader As StreamReader = New StreamReader(sPath, Encoding.GetEncoding(932))
            sSQL = oReader.ReadToEnd() _
                   .Replace("${Sta}", sTargetSta) _
                   .Replace("${BasePath}", Config.RiyoDataImporterFilesBasePath) _
                   .Replace("${RiyoDataDatabaseName}", Config.RiyoDataDatabaseName) _
                   .Replace("${ShiteiDataDatabaseName}", Config.ShiteiDataDatabaseName)
        End Using

        Dim dbCtl As New DatabaseTalker()
        Try
            dbCtl.ConnectOpen()
            dbCtl.TransactionBegin()
            For Each sBatch As String In sSQL.Split(New String() {"${GO}"}, StringSplitOptions.RemoveEmptyEntries)
                dbCtl.ExecuteSQLToWrite(sBatch)
            Next sBatch
            dbCtl.TransactionCommit()
        Catch ex As Exception
            dbCtl.TransactionRollBack()
            Throw
        Finally
            dbCtl.ConnectClose()
        End Try
    End Sub
#End Region

End Class
