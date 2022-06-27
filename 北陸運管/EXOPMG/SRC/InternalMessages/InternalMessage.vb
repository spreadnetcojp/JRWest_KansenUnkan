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

Imports System.IO
Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' �������b�Z�[�W�B
''' </summary>
Public Structure InternalMessage
#Region "�萔"
    Private Const MsgSizePos As Integer = 0
    Private Const MsgSizeLen As Integer = 4
    Private Const MsgKindPos As Integer = MsgSizePos + MsgSizeLen
    Private Const MsgKindLen As Integer = 4

    'NOTE: ������Friend�ł���B
    Public Const ExtendPartPos As Integer = MsgKindPos + MsgKindLen

    Private Const MinMsgSize As Integer = 128
#End Region

#Region "�݊��\���̎����p�ϐ�"
    'NOTE: ������Friend�ł���B
    Public RawBytes As Byte()
#End Region

#Region "�v���p�e�B"
    Public ReadOnly Property Size() As Integer
        Get
            Return BitConverter.ToInt32(RawBytes, MsgSizePos)
        End Get
    End Property

    Public ReadOnly Property Kind() As Integer
        Get
            Return BitConverter.ToInt32(RawBytes, MsgKindPos)
        End Get
    End Property

    Public ReadOnly Property HasValue() As Boolean
        Get
            Return RawBytes IsNot Nothing
        End Get
    End Property
#End Region

#Region "�݊��\���̎����p�̋��ʃ��\�b�h"
    '�݊��\���̂�����̃w�b�_�����ځiSize, Kind�j��ǂݎ��ۂɗ��p���郁�\�b�h�B
    'NOTE: ������Friend�ł���B
    Public Shared Function Parse(ByVal rawBytes As Byte()) As InternalMessage
        Dim ret As InternalMessage
        ret.RawBytes = rawBytes
        Return ret
    End Function
#End Region

#Region "�C�Ӓ��̊g�����ڂ����݊��\���̂̎����p���\�b�h"
    '�݊��\���̂̃C���X�^���X�쐬���\�b�h����������ۂ́A��{�ƂȂ�R���X�g���N�^�B
    'NOTE: ���̃R���X�g���N�^�ł́A���b�Z�[�W��ʂ��Ƃ̔C�Ӎ��ڂɂ��ẮA�̈�̗p�ӂ̂ݍs���B
    '���Y�̈�ɑ΂���l�̐ݒ�́A�Ăяo�����i�݊��\���̑��j�ōs���B
    'NOTE: ������Friend�ł���B
    Public Sub New(ByVal kind As Integer, ByVal extendPartSize As Integer)
        Dim size As Integer = ExtendPartPos + extendPartSize
        If size < MinMsgSize Then
            size = MinMsgSize
        End If
        Dim bytes As Byte() = New Byte(size - 1) {}

        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)
        Me.RawBytes = bytes
    End Sub
#End Region

#Region "�g�����ڕs�v�Ȏ�ʂ̌݊��\���̂̎����p���\�b�h"
    'NOTE: ������Friend�ł���B
    Public Sub New(ByVal kind As Integer)
        Dim size As Integer = MinMsgSize
        Dim bytes As Byte() = New Byte(size - 1) {}
        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)
        Me.RawBytes = bytes
    End Sub
#End Region

#Region "Serializable��Object���g�����ڂƂ����ʂ̌݊��\���̂̎����p���\�b�h"
    'NOTE: ������Friend�ł���B
    Public Sub New(ByVal kind As Integer, ByVal obj As Object)
        Dim mem As New MemoryStream()
        Dim bf As New BinaryFormatter()
        bf.Serialize(mem, obj)

        Dim memLen As Integer = CInt(mem.Length)

        Dim size As Integer = ExtendPartPos + memLen
        If size < MinMsgSize Then
            size = MinMsgSize
        End If
        Dim bytes As Byte() = New Byte(size - 1) {}

        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)

        mem.Position = 0
        mem.Read(bytes, ExtendPartPos, memLen)

        Me.RawBytes = bytes
    End Sub

    'NOTE: ������Friend�ł���B
    Public Function GetExtendObject() As Object
        Dim mem As New MemoryStream(RawBytes, ExtendPartPos, RawBytes.Length - ExtendPartPos, False)
        Dim bf As New BinaryFormatter()
        Return bf.Deserialize(mem)
    End Function
#End Region

#Region "Integer���g�����ڂƂ����ʂ̌݊��\���̂̎����p���\�b�h"
    'NOTE: ������Friend�ł���B
    Public Sub New(ByVal kind As Integer, ByVal extend1 As Integer, ByVal extend2 As Integer)
        Dim size As Integer = ExtendPartPos + 4 + 4
        If size < MinMsgSize Then
            size = MinMsgSize
        End If
        Dim bytes As Byte() = New Byte(size - 1) {}

        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, bytes, MsgSizePos, MsgSizeLen)
        Buffer.BlockCopy(BitConverter.GetBytes(kind), 0, bytes, MsgKindPos, MsgKindLen)
        Buffer.BlockCopy(BitConverter.GetBytes(extend1), 0, bytes, ExtendPartPos, 4)
        Buffer.BlockCopy(BitConverter.GetBytes(extend2), 0, bytes, ExtendPartPos + 4, 4)
        Me.RawBytes = bytes
    End Sub

    'NOTE: ������Friend�ł���B
    Public Function GetExtendInteger1() As Integer
        Return BitConverter.ToInt32(RawBytes, ExtendPartPos)
    End Function

    'NOTE: ������Friend�ł���B
    Public Function GetExtendInteger2() As Integer
        Return BitConverter.ToInt32(RawBytes, ExtendPartPos + 4)
    End Function
#End Region

#Region "�\�P�b�g�ւ̏������݃��\�b�h"
    'NOTE: �����Socket�ɑ΂��A�����̃X���b�h���i�r������Ȃ��Ɂj���̃��\�b�h�����s
    '���邱�Ƃ͑z�肵�Ȃ��B����āA�������b�Z�[�W����M�pSocket�ɂ��ẮA�P���
    '�X���b�h�ŃA�N�Z�X����悤�A���L�Ҍ��߂ăA�v����݌v����̂��x�X�g�ł���B
    '���Ƃ��΁A�R�l�N�V�������L�[�v����^�C�v�̃N���C�A���g���A�v���ɂ����āA
    'Telegrapher���O���Ƃ̃R�l�N�V������ؒf�����ꍇ�̎����Đڑ�����́A
    '���̔\���I����Ɠ���̃X���b�h�𒆐��Ƃ��čs���ׂ��ł���A��p��
    '�X���b�h�i�ؒf�Ď��X���b�h�j�͗p�ӂ��Ȃ������悢�B�ꌩ����ƁA��p��
    '�X���b�h��p�ӂ��邱�ƂŁA�ؒf�Ď��̂��߂̎������i�����ɐؒf�Ď��X���b�h
    '������ConnectionKeeper�̂悤�ȃN���X�Ɂj�Ǐ�������āA�P���ɂȂ肻����
    '���邪�A���������ꍇ�ATelegrapher�ɑ΂���ConnectNotice���b�Z�[�W�̑��M��
    '�ؒf�Ď��X���b�h���s������ŁAActiveUllExecRequest�̂悤�ȃ��b�Z�[�W��
    '���M�̓��C���I�X���b�h���s�����ƂɂȂ�ATelegrapher�Ɠ����ʐM���s�����߂�
    'Socket�̔r�����䂪���G�ɂȂ�B���������A�ؒf�Ď������C���I�X���b�h�ōs�����Ƃ́A
    '���قǑ�ςȂ��Ƃł͂Ȃ��B�ڑ�������Ԃł́A���b�Z�[�W���[�v�̃^�C�}�n���h����
    '�ڑ���Ԃ̊Ď����s���A�ؒf��F��������BeginConnect���Đڑ����s����ԂɂȂ�A
    '�ڑ����s����Ԃł́ABeginConnect�̊����̊Ď����s�������ł���B
    '���ɁA�����Socket�Ɋւ��āA�����̃X���b�h�����̃��\�b�h���Ăяo���Ȃ�A
    '�Ăяo�������i���̎Q�ƂɊւ���SyncLock���s���Ȃǂ��āj�r��������s��
    '�K�v�����邪�AWSAEINPROGRESS�̂��Ƃ��l����ƁA���̃��\�b�h�݂̂Ȃ炸�A
    '�����Socket�Ɋւ���GetInstanceFromSocket()�Ƃ��r������ׂ��ł��邵�A
    '����ǂ��납�A�����Socket�Ɋւ���Socket.Select()�Ƃ��r�����Ȃ����
    '�Ȃ�Ȃ��Ǝv����B�����ASocket.Select()���Ăԉӏ��ŁAWSAEINPROGRESS��
    '�n���h�����O���s��Ȃ���΂Ȃ�Ȃ��Ȃ�B������Ńe�X�g��������ł́A
    'Socket�N���X�̃��\�b�h�Ăяo����WSAEINPROGRESS���������邱�Ƃ�
    '�Ȃ������ł��������A���g��WinSock�ł���ȏ�́A�������邱�Ƃ�O��Ƃ���
    '�ׂ��ł���i���Ƃ���.NET Compact Framework���ƁA�ǂ��Ȃ邩�킩��Ȃ��j�B
    '�Ȃ��A�����̃X���b�h������s������i���s�ł��đR��ׂ��ł���j�̂́A
    'WriteToSocket���m�ȊO�́AWriteToSocket�Ɠǂݏo�������ł���A�ǂݏo�����m��
    '�����̃X���b�h�Ŏ��s���邱�Ƃ��Ȃ��i���s�ł���K�v�͂Ȃ��j���߁A�A�v����
    '��u���b�L���O���[�h��Socket��p�ӂ��Ă��������ŁA�ȉ��̃��\�b�h��
    '������ sock �ւ̃A�N�Z�X��SyncLock sock�`End SyncLock�ň͂ނ��ƂŁA
    '�����̃��\�b�h���X���b�h�Z�[�t�ɂ���Ƃ������j�����݂���B
    '���̂悤�ɂ���΁A�ҋ@����������̂́ASelect()�ɂ��ǂݏo���҂����s��
    '�ӏ��i�܂�AManagementLoop�̃X���b�h�j�����ɂȂ邵�A��������
    '��u���b�L���O���[�h�ł���킯������ASelect()��WSAEINPROGRESS����������
    '�̂�O��ɂ���K�v�́i���؂���܂ł��Ȃ��j�S�������Ȃ�B�������A
    'WriteToSocket()�̓�����Send()�𕡐���J��Ԃ����ƂɂȂ�ȂǁA
    '�����ȓ_������i����������u���b�L���O���[�h�ɂ��Ă����Ȃ����
    '�Ȃ�Ȃ����Ǝ��̂������ł���j�̂ŁA��߂Ă����B
    Public Sub WriteToSocket(ByVal sock As Socket)
        Debug.Assert(sock.Blocking)
        sock.SendTimeout = 0
        sock.Send(RawBytes)
    End Sub

    'NOTE: ���M�o�b�t�@�ɓ��肫��Ȃ��傫�ȃ��b�Z�[�W���������ލۂɁA
    '���M��̃X���b�h���s����Ń\�P�b�g����̓ǂݏo����
    '�s��Ȃ��Ȃ��Ă��邱�Ɠ����z�肵���A�v�������̃o�[�W�����B
    '�e�X���b�h���q�X���b�h�Ƀ��b�Z�[�W�𑗐M����ۂɁA
    '�q�X���b�h�̒�~���e���ԂƓ����x���̎��Ԃ������ɂ���
    '���p���邱�Ƃ�z��B
    'NOTE: timeoutTicks��0�܂���-1���w�肷��Ɩ������ҋ@�ƂȂ�B
    Public Function WriteToSocket(ByVal sock As Socket, ByVal timeoutTicks As Integer) As Boolean
        Try
            Debug.Assert(sock.Blocking)
            sock.SendTimeout = timeoutTicks
            sock.Send(RawBytes)
        Catch ex As SocketException
            Select Case ex.ErrorCode
                '�w�肵�����ԓ��ɏ������݂ł��Ȃ������ꍇ�iWSAETIMEDOUT�j
                'TODO: ���ꂶ��Ȃ��C���iSocket�N���X�̎�������H�j
                Case 10060
                    Log.Error("I'm through waiting for all bytes of the msg to write.", ex)
                    Return False

                '���������Ȃ�A�A�v���̕s����v���ł���\�����Z���ł��邽�߁A
                '�A�v�����I�������đ��߂Ɂi�e�X�g���Ɂj�C�t�����������悢�G���[
                'NOTE: 10036�iWSAEINPROGRESS�j���������Ȃ������Ŏg����悤��
                '����\��ł��邽�߁A10036�iWSAEINPROGRESS�j�������ɂ���B
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    Throw

                'Send()�ɂ����āA�Β[�\�P�b�g����⑕�u���̏󋵂Ŕ����������ł��邽�߁A
                '�A�v�����I��������킯�ɂ͂����Ȃ��Ǝv����G���[
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    Log.Error("SocketException caught.", ex)
                    Return False

                '���������Ȃ��͂��ł��邪�A�����ǂ��Ȃ邩�킩��Ȃ����߁A
                '�A�v�����I�������Ȃ���������Ǝv����G���[
                Case Else
                    Log.Error("Surprising SocketException caught.", ex)
                    Return False
            End Select
        End Try
        Return True
    End Function
#End Region

#Region "�\�P�b�g����̓ǂݏo�����\�b�h"
    Public Shared Function GetInstanceFromSocket(ByVal sock As Socket) As InternalMessage
        Dim msgLen As Integer = MinMsgSize
        Dim bytes As Byte() = New Byte(msgLen - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False

        Debug.Assert(sock.Blocking)
        sock.ReceiveTimeout = 0
        Do
            Dim rcvlen As Integer = sock.Receive(bytes, offset, msgLen - offset, SocketFlags.None)
            offset = offset + rcvlen
            If offset = msgLen Then
                If isReceivedMinSize Then Exit Do

                msgLen = BitConverter.ToInt32(bytes, MsgSizePos)
                If msgLen = MinMsgSize Then Exit Do

                Debug.Assert(msgLen > MinMsgSize)
                Array.Resize(bytes, msgLen)
                isReceivedMinSize = True
            End If
        Loop

        Dim ret As InternalMessage
        ret.RawBytes = bytes
        Return ret
    End Function

    'NOTE: ���M���̃X���b�h���s����Ń\�P�b�g�ւ̏������݂̓r���Œ�~������A
    'Size�ɐݒ肵�����̃o�C�g���������܂Ȃ�������A�i���b�Z�[�W��
    '�������ނׂ��󋵂ł���ɂ�������炸�j�������������������܂Ȃ�������
    '���邱�Ɠ����z�肷��A�v�������̃o�[�W�����B
    'NOTE: �e�X���b�h���q�X���b�h����̃��b�Z�[�W����M����ۂɁA
    '�q�X���b�h�̒�~���e���ԂƓ����x���̎��Ԃ������ɂ���
    '���p���邱�Ƃ�z��B
    'NOTE: ���̃��\�b�h�̂�HasValue��False�ȃC���X�^���X��Ԃ����Ƃ�
    '���蓾��i���b�Z�[�W�̎��o���ŏ�L�̂悤�Ȉُ�����o�����ꍇ�j�B
    'NOTE: timeoutTicks��0�܂���-1���w�肷��Ɩ������ҋ@�ƂȂ�B
    Public Shared Function GetInstanceFromSocket(ByVal sock As Socket, ByVal timeoutTicks As Integer) As InternalMessage
        Dim msgLen As Integer = MinMsgSize
        Dim bytes As Byte() = New Byte(msgLen - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False

        Debug.Assert(sock.Blocking)
        Dim timer As TickTimer = Nothing
        Dim systemTick As Long
        If timeoutTicks > 0 Then
            timer = New TickTimer(timeoutTicks)
            systemTick = TickTimer.GetSystemTick()
            timer.Start(systemTick)
        Else
            sock.ReceiveTimeout = 0
        End If
        Try
            Do
                If timeoutTicks > 0 Then
                    Dim ticks As Long = timer.GetTicksToTimeout(systemTick)
                    If ticks < 1 Then
                        Log.Error("I'm through waiting for all bytes of the msg to read.")
                        Return Nothing
                    End If
                    sock.ReceiveTimeout = CInt(ticks)
                End If

                Dim rcvlen As Integer = sock.Receive(bytes, offset, msgLen - offset, SocketFlags.None)
                If rcvlen = 0 Then
                    Log.Error("Connection closed by peer.")
                    Return Nothing
                End If

                offset = offset + rcvlen
                If offset = msgLen Then
                    If isReceivedMinSize Then Exit Do

                    msgLen = BitConverter.ToInt32(bytes, MsgSizePos)
                    If msgLen = MinMsgSize Then Exit Do

                    Debug.Assert(msgLen > MinMsgSize)
                    Array.Resize(bytes, msgLen)
                    isReceivedMinSize = True
                End If
            Loop
        Catch ex As SocketException
            Select Case ex.ErrorCode
                '�w�肵�����ԓ��ɏ������݂ł��Ȃ������ꍇ�iWSAETIMEDOUT�j
                'TODO: ���ꂶ��Ȃ��C���iSocket�N���X�̎�������H�j
                Case 10060
                    Log.Error("I'm through waiting for all bytes of the telegram to read.", ex)
                    Return Nothing

                '���������Ȃ�A�A�v���̕s����v���ł���\�����Z���ł��邽�߁A
                '�A�v�����I�������đ��߂Ɂi�e�X�g���Ɂj�C�t�����������悢�G���[
                'NOTE: 10036�iWSAEINPROGRESS�j���������Ȃ������Ŏg����悤��
                '����\��ł��邽�߁A10036�iWSAEINPROGRESS�j�������ɂ���B
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    Throw

                'Receive()�ɂ����āA�Β[�\�P�b�g����⑕�u���̏󋵂Ŕ����������ł��邽�߁A
                '�A�v�����I��������킯�ɂ͂����Ȃ��Ǝv����G���[
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    Log.Error("SocketException caught.", ex)
                    Return Nothing

                '���������Ȃ��͂��ł��邪�A�����ǂ��Ȃ邩�킩��Ȃ����߁A
                '�A�v�����I�������Ȃ���������Ǝv����G���[
                Case Else
                    Log.Error("Surprising SocketException caught.", ex)
                    Return Nothing
            End Select
        End Try

        Dim ret As InternalMessage
        ret.RawBytes = bytes
        return ret
    End Function
#End Region
End Structure
