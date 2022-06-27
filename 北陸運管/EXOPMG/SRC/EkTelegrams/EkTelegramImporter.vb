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

Imports JR.ExOpmg.Common

''' <summary>
''' �O������d������荞�ރN���X�B
''' </summary>
Public Class EkTelegramImporter
    Implements ITelegramImporter

#Region "�ϐ�"
    Protected Gene As EkTelegramGene
#End Region

#Region "�R���X�g���N�^"
    Public Sub New(ByVal oGene As EkTelegramGene)
        Me.Gene = oGene
    End Sub
#End Region

#Region "���\�b�h"
    '�o�C�g�z�񂩂�̓d���擾���\�b�h
    'NOTE: �o�C�g�񂪓d���Ƃ��Ċ��S�ɕs���ł���i����ӏ��ɋL�ڂ���Ă���
    '�����O�X���K��l�ɖ����Ȃ��A���邢�͋K����傫���j���߂ɏ����ł��Ȃ�
    '�ꍇ��A�w�b�_���ɑ�������o�C�g����ǂݎ��Ȃ��܂��́A�w�b�_���ɋL��
    '���ꂽ���̃o�C�g����ǂݎ��Ȃ��ꍇ�́A�������ۂ�����ŋL�^���A
    'Nothing��ԋp����B
    Public Function GetTelegramFromBytes( _
       ByVal aBytes As Byte()) As EkDodgyTelegram

        Dim minReceiveSize As Integer = Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
        If aBytes.Length < minReceiveSize Then
            Log.Error("The bytes is too short as EkTelegram.")
            Return Nothing
        End If

        Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(aBytes, Gene.ObjSizePos)
        If objSize > Gene.GetObjSizeByRawLen(Integer.MaxValue) Then
            Log.Error("ObjSize of the telegram is too large.")
            Return Nothing
        End If

        Dim telegLen As Integer = Gene.GetRawLenByObjSize(objSize)
        If telegLen > aBytes.Length Then
            Log.Error("Telegram size based on the ObjSize is greater than the bytes.")
            Return Nothing
        End If

        Dim aTelegBytes As Byte() = New Byte(telegLen - 1) {}
        Buffer.BlockCopy(aBytes, 0, aTelegBytes, 0, telegLen)

        Return New EkDodgyTelegram(Gene, aTelegBytes)
    End Function

    '�X�g���[������̓d���擾���\�b�h
    'NOTE: �o�C�g�񂪓d���Ƃ��Ċ��S�ɕs���ł���i����ӏ��ɋL�ڂ���Ă���
    '�����O�X���K��l�ɖ����Ȃ��A���邢�͋K����傫���j���߂ɏ����ł��Ȃ�
    '�ꍇ�́A�������ۂ�����ŋL�^���ANothing��ԋp����B
    '�܂��AoStream���I�[�ɒB���āA�w�b�_���ɑ�������o�C�g����ǂݎ��Ȃ�
    '�܂��́A�w�b�_���ɋL�ڂ��ꂽ���̃o�C�g����ǂݎ��Ȃ��ꍇ���A
    '�������ۂ�����ŋL�^���ANothing��ԋp����B
    '�Ȃ��A���̃��\�b�h�́AoStream���w���C���X�^���X��Read���\�b�h��
    '�X���[������S�Ă̗�O���X���[������B�Ăь��́A�����̗�O�̂����A
    '�v���O���������̕s�����ł������������Ȃ���O�݂̂�\�����ʖ��
    '�Ƃ��Ĉ����ׂ��ł���B���Ƃ��΁A�\�t�g�E�F�A�Ńn���h�����O�\��
    '�n�[�h�E�F�A�ُ̈킪���炩��Exception�Ƃ��ăX���[����邱�Ƃ�����A
    '������A�v���P�[�V�����ŏ�������K�v������i���Ƃ��΁A���̏�����
    '�p������K�v�����邠�邢�́A�����ɗ�����̂ł͂Ȃ��A��������s��
    '�K�v������j�Ȃ�A����͗\�����ׂ���O�ł���A�I�ʉ\�ȕ��@��
    'Catch���Ȃ���΂Ȃ�Ȃ��B
    Public Function GetTelegramFromStream( _
       ByVal oStream As Stream) As EkDodgyTelegram

        Dim minReceiveSize As Integer = Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
        Debug.Assert(Gene.MinAllocSize >= minReceiveSize)
        Debug.Assert(Gene.MaxReceiveSize > Gene.MinAllocSize)

        Dim telegLen As Integer = minReceiveSize
        Dim aBytes As Byte() = New Byte(Gene.MinAllocSize - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False
        Do
            Dim rcvlen As Integer = oStream.Read(aBytes, offset, telegLen - offset)
            If rcvlen = 0 Then
                Log.Error("End of stream detected.")
                Return Nothing
            End If

            offset = offset + rcvlen
            If offset = telegLen Then
                If isReceivedMinSize Then
                    Exit Do
                End If

                Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(aBytes, Gene.ObjSizePos)
                If objSize > Gene.GetObjSizeByRawLen(Integer.MaxValue) Then
                    Log.Error("ObjSize of the telegram is too large.")
                    Return Nothing
                End If

                telegLen = Gene.GetRawLenByObjSize(objSize)
                If telegLen <= minReceiveSize Then
                    If telegLen < minReceiveSize Then
                        Log.Error("ObjSize of the telegram is too small.")
                        Return Nothing
                    End If
                    Exit Do
                End If

                If telegLen > Gene.MinAllocSize Then
                    If telegLen > Gene.MaxReceiveSize Then
                        Log.Error("Telegram size based on the ObjSize is greater than my buffer.")
                        Return Nothing
                    End If
                    Array.Resize(aBytes, telegLen)
                End If

                isReceivedMinSize = True
            End If
        Loop

        Return New EkDodgyTelegram(Gene, aBytes)
    End Function

    '�\�P�b�g����̓d���擾���\�b�h
    'NOTE: timeoutBaseTicks��0�܂���-1���w�肷��Ɩ������ҋ@�ƂȂ�B
    'NOTE: �o�C�g�񂪓d���Ƃ��Ċ��S�ɕs���ł���i����ӏ��ɋL�ڂ���Ă���
    '�����O�X���K��l�ɖ����Ȃ��A���邢�͋K����傫���j���߂ɏ����ł��Ȃ�
    '�ꍇ��A�w�莞�ԓ��Ƀw�b�_���ɑ�������o�C�g����ǂݎ��Ȃ��܂��́A
    '�w�b�_���ɋL�ڂ��ꂽ���̃o�C�g����ǂݎ��Ȃ��ꍇ�A�d���̓r����
    '���葕�u����I�[��������ꂽ�ꍇ�A�O���v���̉\��������
    'SocketException�����������ꍇ�ȂǁA�R�l�N�V�����I���Ɏ������ނׂ���
    '����i�v���O�����ُ̈�ƈ����ׂ��łȂ��j�P�[�X�ł́A�������ۂ������
    '�L�^���ANothing��ԋp����B
    Public Function GetTelegramFromSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As EkDodgyTelegram

        Dim minReceiveSize As Integer = Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
        Debug.Assert(Gene.MinAllocSize >= minReceiveSize)
        Debug.Assert(Gene.MaxReceiveSize > Gene.MinAllocSize)

        Dim telegLen As Integer = minReceiveSize
        Dim aBytes As Byte() = New Byte(Gene.MinAllocSize - 1) {}
        Dim offset As Integer = 0
        Dim isReceivedMinSize As Boolean = False

        Debug.Assert(oSocket.Blocking)
        Dim oTimer As TickTimer = Nothing
        Dim systemTick As Long
        If timeoutBaseTicks > 0 Then
            oTimer = New TickTimer(timeoutBaseTicks)
            systemTick = TickTimer.GetSystemTick()
            oTimer.Start(systemTick)
        Else
            oSocket.ReceiveTimeout = 0
        End If
        Try
            Do
                If timeoutBaseTicks > 0 Then
                    Dim ticks As Long = oTimer.GetTicksToTimeout(systemTick)
                    If ticks < 1 Then
                        LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                        Log.Error("I'm through waiting for all bytes of the telegram to read.")
                        Return Nothing
                    End If
                    oSocket.ReceiveTimeout = CInt(ticks)
                End If

                Dim rcvlen As Integer = oSocket.Receive(aBytes, offset, telegLen - offset, SocketFlags.None)
                If rcvlen = 0 Then
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("Connection closed by peer.")
                    Return Nothing
                End If

                offset = offset + rcvlen
                If offset = telegLen Then
                    If isReceivedMinSize Then
                        Exit Do
                    End If

                    Dim objSize As UInteger = Utility.GetUInt32FromLeBytes4(aBytes, Gene.ObjSizePos)
                    If objSize > Gene.GetObjSizeByRawLen(Integer.MaxValue) Then
                        LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                        Log.Error("ObjSize of the telegram is too large.")
                        Return Nothing
                    End If

                    telegLen = Gene.GetRawLenByObjSize(objSize)
                    If telegLen <= minReceiveSize Then
                        If telegLen < minReceiveSize Then
                            LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                            Log.Error("ObjSize of the telegram is too small.")
                            Return Nothing
                        End If
                        Exit Do
                    End If

                    If telegLen > Gene.MinAllocSize Then
                        If telegLen > Gene.MaxReceiveSize Then
                            LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                            Log.Error("Telegram size based on the ObjSize is greater than my buffer.")
                            Return Nothing
                        End If
                        Array.Resize(aBytes, telegLen)
                    End If

                    If timeoutBaseTicks > 0 Then
                        'NOTE: oTimer.Renew()�ɗ^���鎞�Ԃ����l�ɂȂ�\�������邪�A
                        '���[�v�̐擪�Ń^�C���A�E�g�Ɣ��肳���͂��ł��邽�߁A
                        '�����ł̔���͏ȗ�����B
                        systemTick = TickTimer.GetSystemTick()
                        Dim remainingTicks As Long = oTimer.GetTicksToTimeout(systemTick)
                        oTimer.Renew(remainingTicks + (CLng(timeoutExtraTicksPerMiB) * objSize) \ 1048576)
                        oTimer.Start(systemTick)
                    End If

                    isReceivedMinSize = True
                End If

                systemTick = TickTimer.GetSystemTick()
            Loop
        Catch ex As SocketException
            Select Case ex.ErrorCode
                '�w�肵�����ԓ��ɏ������݂ł��Ȃ������ꍇ�iWSAETIMEDOUT�j
                'TODO: ���ꂶ��Ȃ��C���iSocket�N���X�̎�������H�j
                Case 10060
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("I'm through waiting for all bytes of the telegram to read.", ex)
                    Return Nothing

                '���������Ȃ�A�A�v���̕s����v���ł���\�����Z���ł��邽�߁A
                '�A�v�����I�������đ��߂Ɂi�e�X�g���Ɂj�C�t�����������悢�G���[
                'NOTE: �O���ƒʐM���s�����߂̃\�P�b�g�𕡐��̃X���b�h���瑀�삷�邱�Ƃ�
                '���蓾�Ȃ��i�Ăь��̃o�O�ł���j�Ƃ����O��ŁA10036�iWSAEINPROGRESS�j
                '�������ɂ���B
                Case 10009, 10013, 10014, 10022, 10035, 10036, 10037, 10038, _
                     10039, 10040, 10041, 10042, 10043, 10044, 10045, 10046, _
                     10047, 10048, 10049, 10056, 10092, 10093
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Throw

                'Receive()�ɂ����āA���u�O�v���⑕�u���̏󋵂Ŕ����������ł��邽�߁A
                '�A�v�����I��������킯�ɂ͂����Ȃ��Ǝv����G���[
                Case 10004, 10050, 10051, 10052, 10053, 10054, 10055, 10057, _
                     10058, 10061, 10064, 10065, 10101
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("SocketException caught.", ex)
                    Return Nothing

                '���������Ȃ��͂��ł��邪�A�����ǂ��Ȃ邩�킩��Ȃ����߁A
                '�A�v�����I�������Ȃ���������Ǝv����G���[
                Case Else
                    LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
                    Log.Error("Surprising SocketException caught.", ex)
                    Return Nothing
            End Select
        End Try

        If Not Gene.IsCrcIndicatingOkay(aBytes) Then
            LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
            Log.Error("CRC Error detected.")
            Return Nothing
        End If

        LogReceivedBytes(aBytes, offset, telegLoggingMaxLength)
        Return New EkDodgyTelegram(Gene, aBytes)
    End Function

    Private Shared Sub LogReceivedBytes(ByVal aBytes As Byte(), ByVal validLen As Integer, ByVal loggingMaxLen As Integer)
        If loggingMaxLen > 0 Then
            If validLen <= 0 Then
                Log.Info("No byte received.")
            Else
                Dim loggingLen As Integer = validLen
                If loggingLen > loggingMaxLen Then loggingLen = loggingMaxLen
                Log.Info(validLen.ToString() & " bytes received.", aBytes, 0, loggingLen)
            End If
        End If
    End Sub

    '�\�P�b�g����̓d���擾���\�b�h
    Private Function GetITelegramFromSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As ITelegram Implements ITelegramImporter.GetTelegramFromSocket

        Return GetTelegramFromSocket(oSocket, timeoutBaseTicks, timeoutExtraTicksPerMiB, telegLoggingMaxLength)
    End Function
#End Region

End Class
