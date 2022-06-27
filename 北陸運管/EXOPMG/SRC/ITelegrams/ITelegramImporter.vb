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

''' <summary>
''' �O������d������荞�ރI�u�W�F�N�g�̃C���^�t�F�[�X�B
''' </summary>
Public Interface ITelegramImporter
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
    Function GetTelegramFromSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As ITelegram
End Interface
