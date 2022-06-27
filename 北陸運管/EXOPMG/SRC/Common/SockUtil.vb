' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX�����F
'   Ver      ���t        �S��       �R�����g
'   0.0      2006/08/01  �a��       �V�K�쐬
'   0.1      2006/08/07  �a��       ���V�X�e���Ƃ̘A�g�����邽�߁AWrite()��Read()���\�b�h�ł���肷��f�[�^�ɕt���Ă����w�b�_�[�u���b�N������
'   0.2      2006/09/18  �@�s       Read()���\�b�h�ɂ�Receive�������f�[�^�����J��Ԃ��悤�C��
'   0.3      2006/11/21  �@�s       OP-003 �\�P�b�g�ڑ����AIP�A�h���X�̑O0���߂��폜����
'   0.4      2006/11/22  �@�s       �\�P�b�g�ڑ����AIP�A�h���X�w��̏ꍇ�́A�z�X�g�������Ȃ��悤�C��
'   0.5      2013/04/01  (NES)����  �N���X����SocketControl����ύX���ĕs�����ȃ��\�b�h�������A
'                                   �Ăь��őΏ��ł��Ȃ���O�������̃��[�N�������A
'                                   ���[�J���ڑ��p���\�b�h��ǉ�
' **********************************************************************
Option Strict On
Option Explicit On

Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

'NOTE: ��O�����̕��j�����߂������悢�B
'����A���葕�u���֌W���Ă���\���̂����O���A
'���̃v���O�������g�i���̃N���X���g��Ăь��j�̕s��Ƃ݂Ȃ��ׂ���O��
'�S�ăL���b�`���A�S��OPMGException�ɒu�������Ă��邽�߁A
'�Ăь��́A�ڑ��̂��Ȃ����ōς܂��ׂ��Ȃ̂��A�X���b�h��v���Z�X��
'�ċN���Ɏ������ނׂ��Ȃ̂����f�����Ȃ��B
'���葕�u���֌W���Ă���\���̂����O�̂�
'�L���b�`����iOPMGException�ɒu��������j���A
'�S�ăL���b�`���A���葕�u���֌W���Ă���\���̂����O�ɂ��Ă�
'�iOPMGException�ł͂Ȃ��jSocketExternalException�ɒu�������邩�A
'���̂悤�ȏꍇ�͖߂�l�ňُ��ʒm���邩�A���j�����߂Ȃ����
'�܂Ƃ��ɗ��p�ł��Ȃ��i�Ăь��œ�����O����͂���ȂǂƂ������ƂɁj�B

''' <summary>
''' �\�P�b�g�ʐM�p���[�e�B���e�B
''' </summary>
''' <remarks>
''' .NET Framework��Socket�N���X�𗘗p����A�v���P�[�V���������̃��[�e�B���e�B�B
''' ���̃N���X�ɂ̓T�[�o�[�����@�\�ƃN���C�A���g�����@�\�̗������������Ă���B
''' </remarks>
Public Class SockUtil

    ' ****************************************
    ' ���̃N���X�̎g����:
    '   ���̃N���X�̓T�[�o�[�����̃��\�b�h�ƃN���C�A���g�����̃��\�b�h���������Ă���B
    '
    '   �ڑ����@: TCP/IP�A�X�g���[���ڑ� �Œ�Ƃ���B
    '
    '   �T�[�o�[���̎菇:
    '       (1) StartListener()���\�b�h�Ń��X�j���O�\�P�b�g���쐬����B
    '           ����ɂ��AOS�́ATCP�̃|�[�g���쐬���A�N���C�A���g����̐ڑ���
    '           �󂯕t����i�ڑ�������΃n���h�V�F�[�N���s���j��ԂɂȂ�B
    '       (2) ���X�j���O�\�P�b�g�ɑ΂���Accept()���\�b�h�����s����B
    '           ���̃��\�b�h�͓�����������邽�߁A�N���C�A���g����̐ڑ����Ȃ���΁A
    '           �Ăь��̃X���b�h�͒�~����B�N���C�A���g����̐ڑ����󂯕t����ƁA
    '           �A�v���P�[�V�����C�Ӄf�[�^��ǂݏ����\��Socket��Ԃ��B
    '
    '   �N���C�A���g���̎菇:
    '       (1) Connect()���\�b�h�ŃT�[�o�[�ɐڑ�����B���̃��\�b�h�́A�ڑ�������
    '           ����ƁA�A�v���P�[�V�����C�Ӄf�[�^��ǂݏ����\��Socket��Ԃ��B
    ' ****************************************

    ' // //////////////////////////////////////// �����֐�
#Region " - CreateSocket()  Socket�쐬 "
    ''' <summary>
    ''' Socket�쐬
    ''' </summary>
    ''' <returns>�V�KSocket</returns>
    ''' <remarks>
    ''' �V�KSocket��TCP/IP�A�X�g���[���ڑ��Ő�������B
    ''' </remarks>
    Private Shared Function CreateSocket() As Socket
        ' Socket �̏������I�v�V����
        '   INI�t�@�C���w���R���X�g���N�^�̈����Ƃ��Ă��ǂ��ł����A
        '   �ʐM�����ł��̃I�v�V�����Ɉˑ����镔�������邽��
        '   �����ł͖{�����ɍœK�����āA�I�v�V�����̓��[�J���ϐ��ɂ��w��Ƃ���B
        Dim eAddress As AddressFamily = AddressFamily.InterNetwork       ' IP (v4)
        Dim eSocket As SocketType = SocketType.Stream    ' �X�g���[������M
        Dim eProtocol As ProtocolType = ProtocolType.Tcp  ' TCP
        Return New Socket(eAddress, eSocket, eProtocol)
    End Function
#End Region

    ' // //////////////////////////////////////// ���\�b�h
#Region " + StartListener()  ���X�i�[�J�n "
    ''' <summary>
    ''' ���X�i�[�J�n
    ''' </summary>
    ''' <param name="Address">IP�A�h���X</param>
    ''' <param name="PortNo">�|�[�g�ԍ�</param>
    ''' <returns>���X�i�[ �\�P�b�g</returns>
    ''' <exception cref="OPMGException">���̃��\�b�h�Ŕ���������O</exception>
    ''' <remarks>
    ''' �y�T�[�o�[�������z
    ''' �\�P�b�g�ʐM�̃T�[�o�[�������ɂčŏ��Ɏ��s����K�v������B
    ''' �w�肵���|�[�g�ԍ��Ń��X�i�[���J�n����B
    ''' ���X�i�[���J�n�ł������Accept()���\�b�h���ĂԂ��Ƃɂ��A
    ''' �N���C�A���g����̐ڑ���҂�ԂɂȂ�B
    ''' </remarks>
    Public Shared Function StartListener(ByVal Address As IPAddress, ByVal PortNo As Integer) As Socket
        Dim r As Socket = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Dim oListenerSock As Socket = Nothing
        Try
            ' ���b�X�� �|�[�g�̃G���h �|�C���g�𐶐�
            oEndPoint = New IPEndPoint(Address, PortNo)
            ' ���X�i�[�̐���
            oListenerSock = CreateSocket()
            ' �o�C���h
            oListenerSock.Bind(oEndPoint)
            oListenerSock.Listen(10)
            r = oListenerSock
        Catch ex As Exception
            ' �ڍ׃��O
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.StartListener()"))
            ' IP�A�h���X
            If IsNothing(Address) Then
                sb.AppendLine(OPMGException.DetailNull("Address"))
            Else
                sb.AppendFormat("Address is [{0}].", Address.ToString())
                sb.AppendLine()
            End If
            ' ���b�X�� �|�[�g
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return r
    End Function
#End Region
#Region " + StartLocalListener()  ���[�J���ڑ��p���X�i�[�J�n "
    ''' <summary>
    ''' ���X�i�[�J�n
    ''' </summary>
    ''' <param name="PortNo">�|�[�g�ԍ�</param>
    ''' <returns>���X�i�[ �\�P�b�g</returns>
    ''' <exception cref="OPMGException">���̃��\�b�h�Ŕ���������O</exception>
    ''' <remarks>
    ''' �y�T�[�o�[�������z
    ''' �\�P�b�g�ʐM�̃T�[�o�[�������ɂčŏ��Ɏ��s����K�v������B
    ''' �w�肵���|�[�g�ԍ��Ń��X�i�[���J�n����B
    ''' ���X�i�[���J�n�ł������Accept()���\�b�h���ĂԂ��Ƃɂ��A�N���C�A���g����̐ڑ���҂�ԂɂȂ�B
    ''' </remarks>
    Public Shared Function StartLocalListener(ByVal PortNo As Integer) As Socket
        Dim r As Socket = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Dim oListenerSock As Socket = Nothing
        Try
            ' ���b�X�� �|�[�g�̃G���h �|�C���g�𐶐�
            oEndPoint = New IPEndPoint(IPAddress.Parse("127.0.0.1"), PortNo)
            ' ���X�i�[�̐���
            oListenerSock = CreateSocket()
            ' �o�C���h
            oListenerSock.Bind(oEndPoint)
            oListenerSock.Listen(1)
            r = oListenerSock
        Catch ex As Exception
            ' �ڍ׃��O
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.StartLocalListener()"))
            ' ���b�X�� �|�[�g
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oListenerSock IsNot Nothing Then
                oListenerSock.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return r
    End Function
#End Region
#Region " + Accept()  �ڑ��ҋ@ "
    ''' <summary>
    ''' �ڑ��ҋ@
    ''' </summary>
    ''' <param name="listenerSocket">���X�i�[ �\�P�b�g</param>
    ''' <returns>����M�\�P�b�g</returns>
    ''' <exception cref="OPMGException">���̃��\�b�h�Ŕ���������O</exception>
    ''' <remarks>
    ''' �y�T�[�o�[�������z
    ''' �\�P�b�g�ʐM�͓������s����邽�߁A���̃��\�b�h�̓N���C�A���g����̎�M������܂ŏI�����Ȃ��B
    ''' �N���C�A���g����̎�M���������ꍇ�A����MSocket�̃C���X�^���X�𐶐����ĕԂ��B
    ''' �A�v���P�[�V������Read()���\�b�h��Socket���g���ăf�[�^�̎�M���s����B
    ''' </remarks>
    Public Shared Function Accept(ByVal listenerSocket As Socket) As Socket
        Try
            ' �ڑ�
            Return listenerSocket.Accept()
        Catch ex As Exception
            ' �ڍ׃��O
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.Accept()"))
            Throw New OPMGException(sb.ToString(), ex)
        End Try
    End Function
#End Region
#Region " + Connect()  �ڑ� "
    ''' <summary>
    ''' �ڑ�
    ''' </summary>
    ''' <param name="ServerName">�T�[�o�[��</param>
    ''' <param name="PortNo">�|�[�g�ԍ�</param>
    ''' <returns>�ǂݍ���/�������݉\��Socket</returns>
    ''' <exception cref="OPMGException">���̃��\�b�h�Ŕ���������O</exception>
    ''' <remarks>
    ''' �y�N���C�A���g�������z
    ''' �T�[�o�[�ɑ΂��ă\�P�b�g�ڑ����s���B
    ''' �T�[�o�[�ւ̐ڑ������������Ƃ��A����M�pSocket�̃C���X�^���X��Ԃ��B
    ''' �A�v���P�[�V������Write()���\�b�h��Socket���g���ăf�[�^�̑��M���s����B
    ''' </remarks>
    Public Shared Function Connect(ByVal ServerName As String, ByVal PortNo As Integer) As Socket
        Dim oSocket As Socket = Nothing
        Dim oHost As IPHostEntry = Nothing
        Dim Address As IPAddress = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Try

            Dim sIP As String()
            sIP = Split(ServerName, ".", -1, CompareMethod.Text)

            If sIP.Length = 4 Then
                '�@)�T�[�o�[����IP�A�h���X�̏ꍇ

                'IP���������U���l����������ɖ߂����ƂőO0���߂��폜����B
                For i As Integer = 0 To 3
                    sIP(i) = CStr(CInt(sIP(i)))
                Next

                'IP�A�h���X���Z�b�g
                Address = IPAddress.Parse(sIP(0) & "." & sIP(1) & "." & sIP(2) & "." & sIP(3))
            Else
                '�A)�T�[�o�[�����z�X�g���̏ꍇ

                ' �z�X�g �G���g���̎擾
                oHost = Dns.GetHostEntry(ServerName)

                ' �ŏ��̃l�b�g���[�N �J�[�h��IP�A�h���X���擾
                Address = oHost.AddressList(0)
            End If

            ' �T�[�o�[�����b�X�� �|�[�g�̃G���h �|�C���g�𐶐�
            oEndPoint = New IPEndPoint(Address, PortNo)
            ' �ڑ��̐���
            oSocket = CreateSocket()
            ' �ڑ�
            oSocket.Connect(oEndPoint)
        Catch ex As Exception
            ' �ڍ׃��O
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.Connect()"))
            ' �z�X�g �G���g��
            If IsNothing(oHost) Then
                sb.AppendLine(OPMGException.DetailNull("Host"))
            Else
                Try
                    sb.AppendFormat("HostName is [{0}].", oHost.HostName)
                Catch iex As Exception
                    sb.Append(OPMGException.DetailException("HostName", iex))
                End Try
                sb.AppendLine()
            End If
            ' IP�A�h���X
            If IsNothing(Address) Then
                sb.AppendLine(OPMGException.DetailNull("Address"))
            Else
                sb.AppendFormat("Address is [{0}].", Address.ToString())
                sb.AppendLine()
            End If
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oSocket IsNot Nothing Then
                oSocket.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return oSocket
    End Function
#End Region
#Region " + ConnectToLocal()  ���[�J���ڑ� "
    ''' <summary>
    ''' �ڑ�
    ''' </summary>
    ''' <param name="PortNo">�|�[�g�ԍ�</param>
    ''' <returns>�ǂݍ���/�������݉\��Socket</returns>
    ''' <exception cref="OPMGException">���̃��\�b�h�Ŕ���������O</exception>
    ''' <remarks>
    ''' �y�N���C�A���g�������z
    ''' �T�[�o�[�ɑ΂��ă\�P�b�g�ڑ����s���B
    ''' �T�[�o�[�ւ̐ڑ������������Ƃ��A����M�pSocket�̃C���X�^���X��Ԃ��B
    ''' �A�v���P�[�V������Write()���\�b�h��Socket���g���ăf�[�^�̑��M���s����B
    ''' </remarks>
    Public Shared Function ConnectToLocal(ByVal PortNo As Integer) As Socket
        Dim oSocket As Socket = Nothing
        Dim oEndPoint As IPEndPoint = Nothing
        Try
            ' �T�[�o�[�����b�X�� �|�[�g�̃G���h �|�C���g�𐶐�
            oEndPoint = New IPEndPoint(IPAddress.Parse("127.0.0.1"), PortNo)
            ' �ڑ��̐���
            oSocket = CreateSocket()
            ' �ڑ�
            oSocket.Connect(oEndPoint)
        Catch ex As Exception
            ' �ڍ׃��O
            Dim sb As New StringBuilder
            sb.AppendLine(OPMGException.DetailHeader("SockUtil.ConnectToLocal()"))
            ' ���b�X�� �|�[�g
            sb.AppendFormat("PortNo is [{0}].", PortNo.ToString())
            sb.AppendLine()

            If oSocket IsNot Nothing Then
                oSocket.Close()
            End If
            Throw New OPMGException(sb.ToString(), ex)
        End Try
        Return oSocket
    End Function
#End Region

End Class
