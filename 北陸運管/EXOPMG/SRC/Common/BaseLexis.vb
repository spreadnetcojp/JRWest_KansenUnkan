' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' ��������
''' </summary>
Public Enum SentenceAttr As Integer
    None
    Information
    Warning
    [Error]
    Question
End Enum

'NOTE: ���̃N���X�̃��\�b�h�́ABaseConfig.Init()���s�O�ɌĂяo����邱�Ƃ�z��
'���Ȃ���΂Ȃ�Ȃ��B�����A���̃N���X�̃��\�b�h�₻������Ăяo����郁�\�b�h��
'BaseConfig���Q�Ƃ��Ă͂Ȃ�Ȃ��B

''' <summary>
''' �u���\�ȏ�������
''' </summary>
Public Structure Sentence
    '�����������w�荀�ڂ݂̂Ƀ}�b�`���鐳�K�\��
    Private Shared ReadOnly oFormatItemRegx As New Regex("\{[0-9]+(\,[+-]{0,1}[0-9]){0,1}(:[^{}]+){0,1}\}", RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    '�������܂��͌���������w�荀�ڂɃ}�b�`���鐳�K�\��
    Private Shared ReadOnly oPseudoFormatItemRegx As New Regex("\{[^{}]*\}", RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Public Attr As SentenceAttr
    Friend RawValue As String
    Friend FormatItemCount As Integer

    '�����w�荀�ڂ̌����i���ۂ̂Ƃ���́A�ő��index + 1�j��ԋp����B
    'NOTE: �n���ꂽ�����񂪕��������w�蕶����Ƃ��Ė�肪����ꍇ��ArgumentException���X���[����B
    Public Shared Function CountFormatItems(ByVal s As String, ByVal isReplacement As Boolean) As Integer
        '�����񂩂�"{{"��"}}"������������ŁA�����w�荀�ڂ𔲂��o���B
        s = s.Replace("{{", "A").Replace("}}", "Z")
        Dim oMatches As MatchCollection = oFormatItemRegx.Matches(s)

        If oPseudoFormatItemRegx.Matches(s).Count <> oMatches.Count Then
            '�����ԈႢ�Ƃ݂Ȃ��鏑���w�荀�ڂ����݂���ꍇ
            Throw New ArgumentException("The string contains pseudo format item.")
        End If

        '�����w�荀�ڂ̍ő��index�𒲂ׂ�B
        '�܂��A����index�̏����w�荀�ڂ����݂��Ȃ����`�F�b�N���A
        'isReady�̊e�v�f�ɓ��Yindex�̏����w�荀�ڂ����݂��邩�ۂ����Z�b�g����B
        Dim isReady(99) As Boolean
        Dim maxIndex As Integer = -1
        For Each oMatch As Match In oMatches
            Dim indexAsDouble As Double = Val(oMatch.Value.Substring(1))
            If indexAsDouble > 99 Then
                '�����w�荀�ڂ�index���傫������ꍇ
                Throw New ArgumentException("The string contains invalid format item [" & oMatch.Value & "]. Its index is too large.")
            End If
            Dim index As Integer = CInt(indexAsDouble)
            If isReady(index) Then
                '����index�̏����w�荀�ڂ����݂���ꍇ
                Throw New ArgumentException("The string contains invalid format item [" & oMatch.Value & "]. Its index is duplicative.")
            End If
            isReady(index) = True
            If index > maxIndex Then maxIndex = index
        Next oMatch

        '�I���W�i���̕�����i�\�[�X�R�[�h�ɋL�q�������́j�̏ꍇ�́A
        '�����w�荀�ڂ�index����������ԂłȂ����`�F�b�N����B
        'NOTE: �ݒ莟��ŕ\���������Ȃ����������邩������Ȃ����߁A
        '�u������������̏ꍇ�́A���̃`�F�b�N�͂�߂Ă������Ƃɂ��Ă���B
        'NOTE: �I���W�i���̕�����i�\�[�X�R�[�h�ɋL�q�������́j�ɂ��ẮA
        '�����ɑΉ����鏑���w�荀�ڂ�S�ċL�q���邱�Ƃ��݌v�̑O��ł���
        '�i���Ȃ��Ƃ��Ō�̈����ɑΉ����鍀�ڂ͋L�q���Ă����Ȃ���΂Ȃ�Ȃ��j
        '���߁A���̃`�F�b�N���L�Q�ɂȂ邱�Ƃ͂Ȃ��B
        If Not isReplacement Then
            For index As Integer = 0 To maxIndex
                If Not isReady(index) Then
                    Throw New ArgumentException("The string should contain a format item whose index is [" & index.ToString() & "].")
                End If
            Next index
        End If

        Return maxIndex + 1
    End Function

    Private Sub Init(ByVal sRawValue As String, ByVal attr As SentenceAttr, ByVal isReplacement As Boolean)
        Dim count As Integer = CountFormatItems(sRawValue, isReplacement)
        Me.FormatItemCount = count
        Me.RawValue = Utility.TranslateClangLiteralToDosText(sRawValue)
        Me.Attr = attr
    End Sub

    Public Sub New(ByVal sRawValue As String, ByVal attr As SentenceAttr, Optional ByVal isReplacement As Boolean = False)
        Init(sRawValue, attr, isReplacement)
    End Sub

    Public Sub New(ByVal sRawValue As String,Optional ByVal isReplacement As Boolean = False)
        Init(sRawValue, SentenceAttr.None, isReplacement)
    End Sub

    Public Sub New(ByVal sRawValue As String, ByVal sAttr As String,Optional ByVal isReplacement As Boolean = False)
        Init(sRawValue, DirectCast([Enum].Parse(GetType(SentenceAttr), sAttr), SentenceAttr), isReplacement)
    End Sub

    Public Function Gen(ByVal ParamArray args As Object()) As String
        Return String.Format(RawValue, args)
    End Function
End Structure

''' <summary>
''' �����R���e�i�̊�{�N���X
''' </summary>
Public Class BaseLexis
    Public Shared NoneTitle As New Sentence("")
    Public Shared InformationTitle As New Sentence("�ʒm")
    Public Shared WarningTitle As New Sentence("�x��")
    Public Shared ErrorTitle As New Sentence("�G���[")
    Public Shared QuestionTitle As New Sentence("�m�F")
    Public Shared UnforeseenErrorOccurred As New Sentence("�\�����ʈُ킪�������܂����B", SentenceAttr.Error)

    Private Const sSection As String = "Lexis"
    Private Const sAttrSuffix As String = "_Attr"
    Private Const targetBindingFlags As BindingFlags = _
       BindingFlags.Static Or _
       BindingFlags.Public Or _
       BindingFlags.NonPublic Or _
       BindingFlags.FlattenHierarchy

    Private Declare Ansi Function GetPrivateProfileString Lib "KERNEL32.DLL" _
       Alias "GetPrivateProfileStringA" ( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpReturnedString As StringBuilder, _
        ByVal nSize As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String _
      ) As Integer

    Private Declare Ansi Function GetPrivateProfileStringToBytes Lib "KERNEL32.DLL" _
       Alias "GetPrivateProfileStringA" ( _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpApplicationName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String, _
        <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.U1)> ByVal lpReturnedString As Byte(), _
        ByVal nSize As Integer, _
        <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String _
      ) As Integer

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub BaseInit(ByVal sIniFilePath As String, ByVal t As Type)
        '�N���Xt�̐ÓI�Ȍ��JSentence�^�t�B�[���h�S�Ăɂ��āA
        '�����o������������������Ă��邱�Ƃ��`�F�b�N����B
        'NOTE: �ȉ����s�����Ƃ̒��ڂ̈Ӗ��́A�����̃t�B�[���h��
        '�R���X�g���N�^�����̏�ŋ����I�Ɏ��s�����邱�Ƃɂ���B
        '���L���s��Ȃ��ƁAINI�t�@�C����Lexis�Z�N�V�����ɃL�[���P��
        '���݂��Ȃ��ꍇ�ɁA���̃��\�b�h���ŃN���Xt�ɃA�N�Z�X����
        '���Ƃ������Ȃ��Ă��܂��B�܂�A���b�Z�[�W�{�b�N�X��\������
        '�Ȃǂ�t�̃����o�ɃA�N�Z�X����Ƃ��܂ŁAt�̃t�B�[���h��
        '�R���X�g���N�^�����s����Ȃ��i������ɕs���������Ă�
        '�N�����ɂ킩��Ȃ��j�Ƃ������ƂɂȂ��Ă��܂��B
        Dim aFields As FieldInfo() = t.GetFields(targetBindingFlags)
        For Each oField As FieldInfo In aFields
            Dim val As Object = oField.GetValue(Nothing)
            If val.GetType() Is GetType(Sentence) Then
                Dim value As Sentence = DirectCast(val, Sentence)
                If value.RawValue Is Nothing Then
                    'NOTE: ���ۂɃR���X�g���N�^���r���Ŏ��s���Ă���iThrow�Ŕ����Ă���j�ꍇ�́A
                    '�����͎��s����Ȃ��͂��ł���B
                    Throw New OPMGException(t.ToString() & "." & oField.Name & " refers to nothing.")
                End If
            End If
        Next oField

        'INI�t�@�C���̏���Z�N�V�������̑S�L�[���k����؂�Ńo�C�g����Ɏ擾����B
        Dim aBytes(16384) As Byte
        Dim validLengthOfBytes As Integer = _
           GetPrivateProfileStringToBytes(sSection, Nothing, "[]_", aBytes, aBytes.Length, sIniFilePath)
        If validLengthOfBytes = 0 Then
            'INI�t�@�C���⏊��Z�N�V�����͑��݂��A�L�[���P���Ȃ��ꍇ�ł���B
            Return
        End If

        '�o�C�g���String�ɕϊ���A�e�L�[��v�f�Ƃ���String�z����쐬����B
        Dim sNullSeparatedKeys As String = Encoding.Default.GetString(aBytes, 0, validLengthOfBytes - 1)
        If sNullSeparatedKeys.Equals("[]") Then
            'INI�t�@�C���܂��͏���Z�N�V���������݂��Ȃ��ꍇ�ł���B
            Throw New OPMGException("The [" & sSection & "] section not found.")
        End If
        Dim aKeys As String() = sNullSeparatedKeys.Split(Chr(0))

        For Each sKey As String In aKeys
            Dim sFieldName As String
            Dim isAttrKey As Boolean
            If sKey.EndsWith(sAttrSuffix) Then
                sFieldName = sKey.Substring(0, sKey.Length - sAttrSuffix.Length)
                isAttrKey = True
            Else
                sFieldName = sKey
                isAttrKey = False
            End If

            Dim oField As FieldInfo = t.GetField(sFieldName, targetBindingFlags Or BindingFlags.IgnoreCase)
            If oField Is Nothing Then
                '�]�v�ȃL�[�i�N���Xt�ɊY���t�B�[���h�̖����L�[�j���L�q����Ă���ꍇ�ł���B
                Throw New OPMGException("The [" & t.ToString() & "] does not have a field named [" & sFieldName & "].")
            End If

            Dim val As Object = oField.GetValue(Nothing)
            If val.GetType() IsNot GetType(Sentence) Then
                Throw New OPMGException("[" & t.ToString() & "." & sFieldName & "] is not a Sentence.")
            End If
            Dim value As Sentence = DirectCast(val, Sentence)
            If isAttrKey Then
                Try
                    Dim sb As StringBuilder = New StringBuilder(1024)
                    GetPrivateProfileString(sSection, sKey, "", sb, sb.Capacity, sIniFilePath)
                    value.Attr = DirectCast([Enum].Parse(GetType(SentenceAttr), sb.ToString()), SentenceAttr)
                    oField.SetValue(Nothing, value)
                Catch ex As Exception
                    Throw New OPMGException("Some error detected around [" & sKey & "].", ex)
                End Try
            Else
                Dim newValue As Sentence

                Try
                    'NOTE: �L�[�ɐݒ肳�ꂽ���ۂ̒l���f�t�H���g�p�Ɠ����l�ł���
                    '���A�L�[�̈ꗗ���擾���Ă���L�[�ɐݒ肳�ꂽ�l���擾����
                    '�܂ł̊Ԃ�INI�t�@�C������L�[���폜����Ȃ�����A
                    '�f�t�H���g�p�̒l���擾����邱�Ƃ͂Ȃ��B��҂̏ꍇ��
                    '�G���[�Ƃ��Ĉ����������A�O�҂ł����Ă���҂Ƌ�ʂ����Ȃ�
                    '���߁A�ǂ݂̂�TranslateClangLiteralToDosText()�ŗ�O������
                    '���邱�ƂɂȂ�s���ȃG�X�P�[�v�V�[�P���X���f�t�H���g�l��
                    '���Ă���B�����āA��҂̏ꍇ�̃G���[���o���A���̗�O��
                    '���o�Ɉς˂邱�Ƃɂ��Ă���B
                    Dim sb As StringBuilder = New StringBuilder(1024)
                    GetPrivateProfileString(sSection, sKey, "\a", sb, sb.Capacity, sIniFilePath)
                    newValue = New Sentence(sb.ToString(), True)
                Catch ex As Exception
                    Throw New OPMGException("Some error detected around [" & sKey & "].", ex)
                End Try

                If newValue.FormatItemCount > value.FormatItemCount Then
                    Throw New OPMGException("The value of [" & sKey & "] is disportionate to the original string.")
                End If

                newValue.Attr = value.Attr
                oField.SetValue(Nothing, newValue)
            End If
        Next sKey
    End Sub
End Class
