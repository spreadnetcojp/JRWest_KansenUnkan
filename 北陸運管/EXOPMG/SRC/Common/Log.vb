' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή��ɂāADebug�AInfo�AWarn��
'                                   ��O�����󂯎��I�[�o�[���[�h��ǉ�
' **********************************************************************
Option Strict On
Option Explicit On

Imports System.Reflection
Imports System.Text
Imports System.Threading

''' <summary>
''' ���O�g���o�̓f���Q�[�g
''' </summary>
Public Delegate Sub LogToOptionalDelegate( _
   ByVal number As Long, _
   ByVal sSecondName As String, _
   ByVal sDateTime As String, _
   ByVal sKind As String, _
   ByVal sClassName As String, _
   ByVal sMethodName As String, _
   ByVal sText As String)

'NOTE: ���̃N���X�̃��\�b�h�́ABaseConfig.Init()���s�O�ɌĂяo����邱�Ƃ�z��
'���Ȃ���΂Ȃ�Ȃ��B�����A���̃N���X�̃��\�b�h�₻������Ăяo����郁�\�b�h��
'BaseConfig���Q�Ƃ��Ă͂Ȃ�Ȃ��B

'NOTE: ���̃N���X�̃��\�b�h�́ABaseLexis.Init()���s�O�ɌĂяo����邱�Ƃ�z��
'���Ȃ���΂Ȃ�Ȃ��B�����A���̃N���X�̃��\�b�h�₻������Ăяo����郁�\�b�h��
'BaseLexis���Q�Ƃ���ׂ��ł͂Ȃ��B
'���������A���̃N���X�������ɋL�q���Ă���悤�ȁu�S���O�ɋ��ʂ̕�����v��
'�ւ��āA�O���ݒ�Œu�������\�ɂ���Ȃǂ̗v�]�͂Ȃ��Ǝv���邪�A
'Lexis���g���Ă������������̂ł���΁ALexis.Init()�̎��s�����O�ɏo�͂����
'���O�i���Ƃ���Lexis.Init()���g�����Y�����̒u�������O�ɏo�͂��郍�O�j�́A
'�u�������Ȃ��Ƃ������Ƃ��l������ׂ��ł���B
'���ꃉ�C�u�������Ƃ͂����A�ǂ��݂Ă��P�����̈ˑ��֌W�ɂ���ׂ��N���X���m��
'���݈ˑ�����̂͂悭�Ȃ��킯�ł���A���̃N���X���g��SetReplacementFormat()
'�̂悤�ȃ��\�b�h��p�ӂ���i���p����Log.Init()�ɑ����A������Ăяo���j����
'�ԈႢ�Ȃ�����ł���B

''' <summary>
''' ���K�[
''' </summary>
Public Class Log
#Region "�����N���X��"
    ''' <summary>
    ''' ���O���
    ''' </summary>
    Private Enum LogKind As Integer
        Debug = 0
        Info
        Warn
        [Error]
        Fatal
        Extra
    End Enum
#End Region

#Region "�萔"
    Private Shared ReadOnly aTextForLogKind As String() = {"[DEBUG]", "[INFO]", "[WARN]", "[ERROR]", "[FATAL]"}
    Private Const sTimestampFormat As String = "yyyy/MM/dd HH:mm:ss.fff"
    Private Const sSep As String = ","
    Private Const sQuot As String = Chr(&H22)
    Private Const sFileExtension As String = ".csv"
    Private Const maxBranchNumber As Integer = 99
#End Region

#Region "�ϐ�"
    Private Shared sDirName As String = Nothing
    Private Shared sFirstName As String = Nothing
    Private Shared seqNumber As Integer = -1
    Private Shared kindsMask As Integer = &HFF
    Private Shared oLogToOptionalDelegate As LogToOptionalDelegate = Nothing
#End Region


#Region "�v���p�e�B"
    Public Shared ReadOnly Property LoggingDebug() As Boolean
        Get
            Return (kindsMask And 1) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingInfo() As Boolean
        Get
            Return (kindsMask And 2) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingWarn() As Boolean
        Get
            Return (kindsMask And 4) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingError() As Boolean
        Get
            Return (kindsMask And 8) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingFatal() As Boolean
        Get
            Return (kindsMask And 16) <> 0
        End Get
    End Property

    Public Shared ReadOnly Property LoggingExtra() As Boolean
        Get
            Return (kindsMask And 32) <> 0
        End Get
    End Property
#End Region

#Region "���������\�b�h"
    'NOTE: �ʂ̃X���b�h���L�^���\�b�h���Ăяo���Ȃ����Ƃ��ۏ؂ł��鎞�_�ŁA���s���Ă��������B
    Public Shared Sub Init(ByVal sBasePath As String, ByVal sArgFirstName As String)
        sDirName = sBasePath
        sFirstName = sArgFirstName
        seqNumber = -1
    End Sub

    'NOTE: �ʂ̃X���b�h���L�^���\�b�h���Ăяo���Ȃ����Ƃ��ۏ؂ł��鎞�_�ŁA���s���Ă��������B
    Public Shared Sub SetKindsMask(ByVal value As Integer)
        kindsMask = value
    End Sub

    'NOTE: �ʂ̃X���b�h���L�^���\�b�h���Ăяo���Ȃ����Ƃ��ۏ؂ł��鎞�_�ŁA���s���Ă��������B
    Public Shared Sub SetOptionalWriter(ByVal oArgLogToOptionalDelegate As LogToOptionalDelegate)
        oLogToOptionalDelegate = oArgLogToOptionalDelegate
    End Sub
#End Region

#Region "�L�^���\�b�h"
    ''' <summary>
    ''' �f�o�b�O�p�����L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Debug(ByVal sText As String)
        Try
            Put(LogKind.Debug, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �f�o�b�O�p�����o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Debug(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Debug, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �f�o�b�O�p�����O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Debug(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Debug, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' ��ʏ����L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Info(ByVal sText As String)
        Try
            Put(LogKind.Info, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' ��ʏ����o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Info(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Info, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' ��ʏ����O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Info(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Info, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �x�����L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Warn(ByVal sText As String)
        Try
            Put(LogKind.Warn, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �x�����o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Warn(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Warn, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �x�����O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Warn(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Warn, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �ُ���L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub [Error](ByVal sText As String)
        Try
            Put(LogKind.Error, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �ُ���o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub [Error](ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Error, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �ُ���O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub [Error](ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Error, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �[���Ȉُ���L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Fatal(ByVal sText As String)
        Try
            Put(LogKind.Fatal, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �[���Ȉُ���o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Fatal(ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Fatal, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �[���Ȉُ���O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Fatal(ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Fatal, Nothing, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ńf�o�b�O�p�����L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Debug(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Debug, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ńf�o�b�O�p�����o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Debug(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Debug, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ńf�o�b�O�p�����O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Debug(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Debug, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���ň�ʏ����L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Info(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Info, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���ň�ʏ����o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Info(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Info, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���ň�ʏ����O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Info(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Info, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ōx�����L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Warn(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Warn, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ōx�����o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Warn(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Warn, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ōx�����O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Warn(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Warn, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���ňُ���L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub [Error](ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Error, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���ňُ���o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub [Error](ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Error, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���ňُ���O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub [Error](ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Error, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ő[���Ȉُ���L�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Fatal(ByVal sExtraName As String, ByVal sText As String)
        Try
            Put(LogKind.Fatal, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ő[���Ȉُ���o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Fatal(ByVal sExtraName As String, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Fatal, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �C�ӃX���b�h���Ő[���Ȉُ���O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Fatal(ByVal sExtraName As String, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Fatal, sExtraName, New StackTrace(0, True).GetFrame(1).GetMethod(), sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �ǉ������L�^����B
    ''' </summary>
    ''' <param name="sExtraName">�ǉ����</param>
    ''' <param name="sText">�L�^����</param>
    Public Shared Sub Extra(ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String)
        Try
            Put(LogKind.Extra, sExtraName, oCaller, sText)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �ǉ������o�C�i���f�[�^�ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sExtraName">�ǉ����</param>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="aBytes">�o�C�i���f�[�^�i�[�z��</param>
    ''' <param name="pos">�o�C�i���f�[�^�̈ʒu</param>
    ''' <param name="len">�o�C�i���f�[�^�̒���</param>
    Public Shared Sub Extra(ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        Try
            Put(LogKind.Extra, sExtraName, oCaller, sText, aBytes, pos, len)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub

    ''' <summary>
    ''' �ǉ������O�̏��ƂƂ��ɋL�^����B
    ''' </summary>
    ''' <param name="sExtraName">�ǉ����</param>
    ''' <param name="sText">�L�^����</param>
    ''' <param name="exception">�L�^��O</param>
    Public Shared Sub Extra(ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal exception As Exception)
        Try
            Put(LogKind.Extra, sExtraName, oCaller, sText, exception)
        Catch ex As Exception
            Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
            'Throw ���Ȃ�
        End Try
    End Sub
#End Region

#Region "�v���C�x�[�g���\�b�h"
    Private Shared Function GetSeqNumber() As Long
        'NOTE: ����ł�GlobalVariables.LockObject�ɂ��Ă�SyncLock�̒��ł̂�
        '���s���邱�ƂɂȂ��Ă���̂ŁAInterlocked�N���X���g���K�v�͂Ȃ��B
        Dim num As Integer = Interlocked.Increment(seqNumber)
        If num >= 0 Then
            Return num
        Else
            Return CLng(num + 1) + UInt32.MaxValue
        End If
    End Function

    Private Shared Sub Put(ByVal kind As LogKind, ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String)
        If (kindsMask And 1 << kind) = 0 Then Return

        Dim sClassName As String = oCaller.DeclaringType.ToString()
        Dim sMethodName As String = oCaller.Name

        'OPT: sCurThreadName�ɂ��ẮALogKind.Extra�̏ꍇ���X���b�h���Ƃ��A
        '�e��ʂ̃��O���ǂ̂悤�ȃZ�J���h���̃t�@�C���ɏ������ނ��́AInit()
        '�������Őݒ肷������悢�B
        Dim oCurThread As Thread = Thread.CurrentThread
        Dim sCurThreadName As String = If(sExtraName IsNot Nothing, sExtraName, oCurThread.Name)
        Dim sKind As String = If(kind = LogKind.Extra, "[" & sExtraName & "]", aTextForLogKind(kind))

        'NOTE: oLogToOptionalDelegate()�Ɉ����œn�����Q�Ƃ́A
        'oLogToOptionalDelegate()�����ŕʂ̃X���b�h��
        '�n���Ă��܂�Ȃ��B���Ƃ������̃X���b�h�ɓn�����Ƃ��Ă�
        '�����͑S�ĕύX���꓾�Ȃ��I�u�W�F�N�g�iString�j��
        '�w���Ă��邽�߁A���ɔz���͕s�v�ł���B

        'NOTE: �ȉ���SyncLock�ɂ��A����v���Z�X���̕ʃX���b�h���o�͂���Q��
        '���R�[�h�ɂ��āA�^�C���X�^���v�̑O��֌W��seqNumber�̑O��֌W�͕K��
        '�����ɂȂ�B�����̃X���b�h���������ޓ���t�@�C���������łȂ��A
        '�v���Z�X���̑S���O�t�@�C�����}�[�W�i�A����AseqNumber�Ń\�[�g�j����
        '�ꍇ�̊e���R�[�h�ɂ��Ă��A�����̑O��֌W�͓����ɂȂ�B

        'OPT: ����A�ȉ���SyncLock�́A�Q�ƌ^�ϐ�GlobalVariables.SysUserId��
        '���āA�ꕔ���������ς������Ԃœǂݎ����s��Ȃ����Ƃ�A
        'seqNumber�̑O��֌W�Ǝ��ۂ̏o�͍s�̑O��֌W����v�����邱�Ƃɉ����A
        '���̃v���Z�X���̕ʃX���b�h�ɂ��u�}�ԑO�܂ł����ꖼ�̃t�@�C���ւ�
        '���R�[�h�ǉ��v�𖳊����ő҂��Ƃ����˂ċL�q���Ă���B
        '�������A�ȉ��̗��R�ɂ��A�^�C���X�^���v�̑O��֌W��seqNumber��
        '�O��֌W�𓯂��ɂ��邱�Ƃ�����ړI�Ƃ�������悢������Ȃ��B
        '(1) �o�͐惊�\�[�X�Ɋւ���r������́AWriteToFile()�������̌Ăяo��
        '  �͈̔͂ŕʓr�s���ׂ��ł���B�Ȃ��A�P�Ȃ�����t���̃t�@�C�����b�N��
        '  ��������ƁA���̃v���Z�X�����b�N�������Ă���ꍇ�Ɂi������t�܂�
        '  �҂��ƂɂȂ�j�p�t�H�[�}���X�������邾�낤���A���̃v���Z�X����
        '  �ʃX���b�h�����O�o�͂̂��߂̏����Ń��b�N���Ă���ꍇ���A���̏�����
        '  �������ɏI����Ă���Ȃ���Γ��l�̂��ƂɂȂ�A���ʂɎ}�Ԃ��ł���
        '  ���܂��͂��ł���B�������A�t�@�C�����b�N�Ƃ͕ʂ̎d�g�݂ŁA�}�ԂȂ�
        '  �̃t�@�C���P�ʂŖ������ҋ@���s���悤�ɂ���΍ςޘb�ł���B
        '(2) �P�̃t�@�C�������b�N����Ă���Ȃǂɂ��WriteToFile()���u���b�N
        '  ���ꂽ�����ŁA���̃t�@�C���Ɗ֌W�̂Ȃ��X���b�h�܂Ńu���b�N���ꂩ��
        '  �Ȃ��͔̂���������B
        '(3) seqNumber�̑O��֌W�Əo�͍s�̑O��֌W�ȂǁA���seqNumber���L�[��
        '  �\�[�g����Έ�v����킯�ł��邵�A���Ȃ�ǂ��ł��悢�B
        '�ȏ�̗��R����A��͂�WriteToFile()�̏����̑啔���iGlobalVariables.
        'SysUserId�̓ǂݎ��ȊO�j��SyncLock�̊O�ɏo���������悢�B

        'OPT: �v�[���X���b�h�ɖ��O���������Ƃ��ۏ؂���Ă���Ȃ�A�㔼�̏����͕s�v�B
        '���������A�t���[�����[�N�����[�J�[�X���b�h�Ɏ��s������悤�ȏ����̈ꕔ��
        '�A�v���Ŏ�������i�����Ń��O�o�͂��s���j���Ƃ͖����O��ł悢����...�B
        If (sCurThreadName IsNot Nothing) AndAlso (Not oCurThread.IsThreadPoolThread) Then
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sText)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sText)
                End If
            End SyncLock
        Else
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sText)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sText)
                End If
            End SyncLock
        End If
    End Sub

    Private Shared Sub Put(ByVal kind As LogKind, ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal aBytes As Byte(), ByVal pos As Integer, ByVal len As Integer)
        If (kindsMask And 1 << kind) = 0 Then Return

        Dim sClassName As String = oCaller.DeclaringType.ToString()
        Dim sMethodName As String = oCaller.Name

        'OPT: sCurThreadName�ɂ��ẮALogKind.Extra�̏ꍇ���X���b�h���Ƃ��A
        '�e��ʂ̃��O���ǂ̂悤�ȃZ�J���h���̃t�@�C���ɏ������ނ��́AInit()
        '�������Őݒ肷������悢�B
        Dim oCurThread As Thread = Thread.CurrentThread
        Dim sCurThreadName As String = If(sExtraName IsNot Nothing, sExtraName, oCurThread.Name)
        Dim sKind As String = If(kind = LogKind.Extra, "[" & sExtraName & "]", aTextForLogKind(kind))

        'NOTE: oLogToOptionalDelegate()�Ɉ����œn�����Q�Ƃ́A
        'oLogToOptionalDelegate()�����ŕʂ̃X���b�h��
        '�n���Ă��܂�Ȃ��B���Ƃ������̃X���b�h�ɓn�����Ƃ��Ă�
        '�����͑S�ĕύX���꓾�Ȃ��I�u�W�F�N�g�iString�j��
        '�w���Ă��邽�߁A���ɔz���͕s�v�ł���B

        'NOTE: �ȉ���SyncLock�ɂ��A����v���Z�X���̕ʃX���b�h���o�͂���Q��
        '���R�[�h�ɂ��āA�^�C���X�^���v�̑O��֌W��seqNumber�̑O��֌W�͕K��
        '�����ɂȂ�B�����̃X���b�h���������ޓ���t�@�C���������łȂ��A
        '�v���Z�X���̑S���O�t�@�C�����}�[�W�i�A����AseqNumber�Ń\�[�g�j����
        '�ꍇ�̊e���R�[�h�ɂ��Ă��A�����̑O��֌W�͓����ɂȂ�B

        'OPT: ����A�ȉ���SyncLock�́A�Q�ƌ^�ϐ�GlobalVariables.SysUserId��
        '���āA�ꕔ���������ς������Ԃœǂݎ����s��Ȃ����Ƃ�A
        'seqNumber�̑O��֌W�Ǝ��ۂ̏o�͍s�̑O��֌W����v�����邱�Ƃɉ����A
        '���̃v���Z�X���̕ʃX���b�h�ɂ��u�}�ԑO�܂ł����ꖼ�̃t�@�C���ւ�
        '���R�[�h�ǉ��v�𖳊����ő҂��Ƃ����˂ċL�q���Ă���B
        '�������A�ȉ��̗��R�ɂ��A�^�C���X�^���v�̑O��֌W��seqNumber��
        '�O��֌W�𓯂��ɂ��邱�Ƃ�����ړI�Ƃ�������悢������Ȃ��B
        '(1) �o�͐惊�\�[�X�Ɋւ���r������́AWriteToFile()�������̌Ăяo��
        '  �͈̔͂ŕʓr�s���ׂ��ł���B�Ȃ��A�P�Ȃ�����t���̃t�@�C�����b�N��
        '  ��������ƁA���̃v���Z�X�����b�N�������Ă���ꍇ�Ɂi������t�܂�
        '  �҂��ƂɂȂ�j�p�t�H�[�}���X�������邾�낤���A���̃v���Z�X����
        '  �ʃX���b�h�����O�o�͂̂��߂̏����Ń��b�N���Ă���ꍇ���A���̏�����
        '  �������ɏI����Ă���Ȃ���Γ��l�̂��ƂɂȂ�A���ʂɎ}�Ԃ��ł���
        '  ���܂��͂��ł���B�������A�t�@�C�����b�N�Ƃ͕ʂ̎d�g�݂ŁA�}�ԂȂ�
        '  �̃t�@�C���P�ʂŖ������ҋ@���s���悤�ɂ���΍ςޘb�ł���B
        '(2) �P�̃t�@�C�������b�N����Ă���Ȃǂɂ��WriteToFile()���u���b�N
        '  ���ꂽ�����ŁA���̃t�@�C���Ɗ֌W�̂Ȃ��X���b�h�܂Ńu���b�N���ꂩ��
        '  �Ȃ��͔̂���������B
        '(3) seqNumber�̑O��֌W�Əo�͍s�̑O��֌W�ȂǁA���seqNumber���L�[��
        '  �\�[�g����Έ�v����킯�ł��邵�A���Ȃ�ǂ��ł��悢�B
        '�ȏ�̗��R����A��͂�WriteToFile()�̏����̑啔���iGlobalVariables.
        'SysUserId�̓ǂݎ��ȊO�j��SyncLock�̊O�ɏo���������悢�B

        Dim s As String = sText & vbCrLf & BitConverter.ToString(aBytes, pos, len)

        'OPT: �v�[���X���b�h�ɖ��O���������Ƃ��ۏ؂���Ă���Ȃ�A�㔼�̏����͕s�v�B
        '���������A�t���[�����[�N�����[�J�[�X���b�h�Ɏ��s������悤�ȏ����̈ꕔ��
        '�A�v���Ŏ�������i�����Ń��O�o�͂��s���j���Ƃ͖����O��ł悢�̂ł́B
        If (sCurThreadName IsNot Nothing) AndAlso (Not oCurThread.IsThreadPoolThread) Then
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, s)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, s)
                End If
            End SyncLock
        Else
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, "Someone", sCurTime, sKind, sClassName, sMethodName, s)
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, "Someone", sCurTime, sKind, sClassName, sMethodName, s)
                End If
            End SyncLock
        End If
    End Sub

    Private Shared Sub Put(ByVal kind As LogKind, ByVal sExtraName As String, ByVal oCaller As MethodBase, ByVal sText As String, ByVal ex As Exception)
        If (kindsMask And 1 << kind) = 0 Then Return

        Dim sClassName As String = oCaller.DeclaringType.ToString()
        Dim sMethodName As String = oCaller.Name

        'OPT: sCurThreadName�ɂ��ẮALogKind.Extra�̏ꍇ���X���b�h���Ƃ��A
        '�e��ʂ̃��O���ǂ̂悤�ȃZ�J���h���̃t�@�C���ɏ������ނ��́AInit()
        '�������Őݒ肷������悢�B
        Dim oCurThread As Thread = Thread.CurrentThread
        Dim sCurThreadName As String = If(sExtraName IsNot Nothing, sExtraName, oCurThread.Name)
        Dim sKind As String = If(kind = LogKind.Extra, "[" & sExtraName & "]", aTextForLogKind(kind))

        'NOTE: oLogToOptionalDelegate()�Ɉ����œn�����Q�Ƃ́A
        'oLogToOptionalDelegate()�����ŕʂ̃X���b�h��
        '�n���Ă��܂�Ȃ��B���Ƃ������̃X���b�h�ɓn�����Ƃ��Ă�
        '�����͑S�ĕύX���꓾�Ȃ��I�u�W�F�N�g�iString�j��
        '�w���Ă��邽�߁A���ɔz���͕s�v�ł���B

        'NOTE: �ȉ���SyncLock�ɂ��A����v���Z�X���̕ʃX���b�h���o�͂���Q��
        '���R�[�h�ɂ��āA�^�C���X�^���v�̑O��֌W��seqNumber�̑O��֌W�͕K��
        '�����ɂȂ�B�����̃X���b�h���������ޓ���t�@�C���������łȂ��A
        '�v���Z�X���̑S���O�t�@�C�����}�[�W�i�A����AseqNumber�Ń\�[�g�j����
        '�ꍇ�̊e���R�[�h�ɂ��Ă��A�����̑O��֌W�͓����ɂȂ�B

        'OPT: ����A�ȉ���SyncLock�́A�Q�ƌ^�ϐ�GlobalVariables.SysUserId��
        '���āA�ꕔ���������ς������Ԃœǂݎ����s��Ȃ����Ƃ�A
        'seqNumber�̑O��֌W�Ǝ��ۂ̏o�͍s�̑O��֌W����v�����邱�Ƃɉ����A
        '���̃v���Z�X���̕ʃX���b�h�ɂ��u�}�ԑO�܂ł����ꖼ�̃t�@�C���ւ�
        '���R�[�h�ǉ��v�𖳊����ő҂��Ƃ����˂ċL�q���Ă���B
        '�������A�ȉ��̗��R�ɂ��A�^�C���X�^���v�̑O��֌W��seqNumber��
        '�O��֌W�𓯂��ɂ��邱�Ƃ�����ړI�Ƃ�������悢������Ȃ��B
        '(1) �o�͐惊�\�[�X�Ɋւ���r������́AWriteToFile()�������̌Ăяo��
        '  �͈̔͂ŕʓr�s���ׂ��ł���B�Ȃ��A�P�Ȃ�����t���̃t�@�C�����b�N��
        '  ��������ƁA���̃v���Z�X�����b�N�������Ă���ꍇ�Ɂi������t�܂�
        '  �҂��ƂɂȂ�j�p�t�H�[�}���X�������邾�낤���A���̃v���Z�X����
        '  �ʃX���b�h�����O�o�͂̂��߂̏����Ń��b�N���Ă���ꍇ���A���̏�����
        '  �������ɏI����Ă���Ȃ���Γ��l�̂��ƂɂȂ�A���ʂɎ}�Ԃ��ł���
        '  ���܂��͂��ł���B�������A�t�@�C�����b�N�Ƃ͕ʂ̎d�g�݂ŁA�}�ԂȂ�
        '  �̃t�@�C���P�ʂŖ������ҋ@���s���悤�ɂ���΍ςޘb�ł���B
        '(2) �P�̃t�@�C�������b�N����Ă���Ȃǂɂ��WriteToFile()���u���b�N
        '  ���ꂽ�����ŁA���̃t�@�C���Ɗ֌W�̂Ȃ��X���b�h�܂Ńu���b�N���ꂩ��
        '  �Ȃ��͔̂���������B
        '(3) seqNumber�̑O��֌W�Əo�͍s�̑O��֌W�ȂǁA���seqNumber���L�[��
        '  �\�[�g����Έ�v����킯�ł��邵�A���Ȃ�ǂ��ł��悢�B
        '�ȏ�̗��R����A��͂�WriteToFile()�̏����̑啔���iGlobalVariables.
        'SysUserId�̓ǂݎ��ȊO�j��SyncLock�̊O�ɏo���������悢�B

        Dim sb As New StringBuilder(sText & vbCrLf)
        While ex IsNot Nothing
            sb.AppendLine(ex.GetType().ToString())
            sb.AppendLine(ex.Message)
            sb.AppendLine(ex.StackTrace)
            ex = ex.InnerException
            If ex IsNot Nothing Then
                sb.AppendLine("The exception has an inner exception...")
            End If
        End While

        'OPT: �v�[���X���b�h�ɖ��O���������Ƃ��ۏ؂���Ă���Ȃ�A�㔼�̏����͕s�v�B
        '���������A�t���[�����[�N�����[�J�[�X���b�h�Ɏ��s������悤�ȏ����̈ꕔ��
        '�A�v���Ŏ�������i�����Ń��O�o�͂��s���j���Ƃ͖����O��ł悢�̂ł́B
        If (sCurThreadName IsNot Nothing) AndAlso (Not oCurThread.IsThreadPoolThread) Then
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, sCurThreadName, sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                End If
            End SyncLock
        Else
            SyncLock GlobalVariables.LockObject
                Dim sCurTime As String = DateTime.Now.ToString(sTimestampFormat)
                Dim num As Long = GetSeqNumber()
                WriteToFile(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                If oLogToOptionalDelegate IsNot Nothing Then
                    oLogToOptionalDelegate(num, "Someone", sCurTime, sKind, sClassName, sMethodName, sb.ToString())
                End If
            End SyncLock
        End If
    End Sub

    Private Shared Sub WriteToFile( _
       ByVal number As Long, _
       ByVal sSecondName As String, _
       ByVal sDateTime As String, _
       ByVal sKind As String, _
       ByVal sClassName As String, _
       ByVal sMethodName As String, _
       ByVal sText As String)

        Dim sPath As String = sDirName
        'sPath = System.IO.Path.Combine(sDirName, sKind)  '���O��ʂ̃t�H���_��ǉ�

        '�t�H���_�쐬(�Ȃ��Ƃ�����)
        If Not System.IO.Directory.Exists(sPath) Then
            'NOTE: �]���@����p�������d�l�ł��邪�A�ȉ��͔����ł���B
            '�C�x���g���O�ւ̏������݂��ł��Ȃ��N���C�A���g�ւ̑΍�Ƃ��āA
            'Utility.WriteLogToEvent()�Ɂu�C�x���g���O�ɏ������ޑ����
            '�N��������I�[�v�����Ă���t�@�C���ɏ������ށv���̃t�H�[���o�b�N��
            '����Ƃ悢�B�N�����̃t�@�C���I�[�v���Ɏ��s����\���͂��邪�A
            '���̏ꍇ�́A�N���C�A���g�ɂ�����N�����̂P�񂾂��Ƃ������ƂŁA
            '���b�Z�[�W�{�b�N�X��\�����Ă��ς킵���͂Ȃ��͂��B
            If Utility.MakeFolder(sPath) = False Then Return
        End If

        sPath = System.IO.Path.Combine(sPath, Format(Now, "yyyyMMdd") & "-" & sFirstName & "-" & sSecondName)

        Dim sUser As String = "[" & GlobalVariables.SysUserId & "]"
        sText = sText.Replace(sQuot, sQuot & sQuot)
        Dim sMessage As String = _
           number.ToString("D10") & sSep & sDateTime & sSep & sKind & sSep _
           & sUser & sSep & sClassName & sSep & sMethodName & sSep _
           & sQuot & sText & sQuot

        Dim sTryPath As String = sPath & sFileExtension
        For i As Integer = 0 To maxBranchNumber
            Dim swFile As System.IO.StreamWriter = Nothing
            Try
                If Not i = 0 Then
                    sTryPath = sPath & "-" & i.ToString() & sFileExtension
                End If
                swFile = New System.IO.StreamWriter(sTryPath, True, Encoding.Default)
                swFile.WriteLine(sMessage)
                swFile.Flush()
                Return
            Catch ex As System.IO.DirectoryNotFoundException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.DriveNotFoundException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.FileNotFoundException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.InternalBufferOverflowException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.InvalidDataException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.PathTooLongException
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Catch ex As System.IO.IOException
                If i = maxBranchNumber Then
                    Utility.WriteLogToEvent(EventLogEntryType.FailureAudit, ex.Message, Utility.ClsName(), Utility.MethodName())
                    Return
                End If
            Catch ex As Exception
                Utility.WriteLogToEvent(EventLogEntryType.Error, ex.Message, Utility.ClsName(), Utility.MethodName())
                Return
            Finally
                If swFile IsNot Nothing Then
                    swFile.Close()
                End If
            End Try
        Next i
    End Sub
#End Region
End Class
