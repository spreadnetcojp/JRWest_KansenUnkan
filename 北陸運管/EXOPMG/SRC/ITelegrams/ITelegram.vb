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
''' ServerTelegrapher��ClientTelegrapher���z�肷�鉼�z�d���B
''' </summary>
Public Interface ITelegram
    '�R�}���h��ʁi���z�j
    ReadOnly Property CmdKind() As CmdKind

    'NAK�d���𐶐����郁�\�b�h
    'NOTE: Telegrapher�͂��̃��\�b�h��Nothing��ԋp����\�����z�肷��B
    'Nothing���ԋp���ꂽ�ꍇ�́AcauseCode�����ł��낤�ƁA
    '�R�l�N�V������ؒf���邱�ƂɂȂ�B
    Function CreateNakTelegram(ByVal causeCode As NakCauseCode) As INakTelegram

    '�w�b�_���̏����ᔽ���`�F�b�N���郁�\�b�h
    'NOTE: ���e�s���d���ɃA�N�Z�X����ۂ́A�w�b�_���̃v���p�e�B��
    '�ǂݏ������邾���ł����Ă��A���̃��\�b�h�����O�����{���A
    '�����ᔽ���������Ƃ��m�F���Ă����Ȃ���΂Ȃ�Ȃ��B
    '�����ᔽ������ꍇ�ɌĂяo���\�ȃ��\�b�h��v���p�e�B�́A
    'CmdKind��CreateNakTelegram�݂̂ł���B
    'NOTE: �v���p�e�B�̎擾�ɕK�{�łȂ��`�F�b�N������Ɏ������邪�A
    '�d���P�̂̎d�l�ɂ����ĔF�߂��Ă���l�͑S�ċ��e����B
    '�܂�A�󋵂Ɉˑ������l�̃`�F�b�N�͌Ăь��̐Ӗ��ł���B
    Function GetHeaderFormatViolation() As NakCauseCode

    '�{�f�B���̏����ᔽ���`�F�b�N���郁�\�b�h
    'NOTE: ���e�s���d�������Ƃɐ��������e��d���C���X�^���X�ɑ΂��A
    '�ŗL�̃v���p�e�B��ǂݏ���������A�ŗL�̃��\�b�h���Ăяo������
    '����ہi�{�f�B���ɃA�N�Z�X����ہj�́A���̃��\�b�h�����O�Ɏ��{���A
    '�����ᔽ���������Ƃ��m�F���Ă����Ȃ���΂Ȃ�Ȃ��B
    'NOTE: �v���p�e�B�̎擾�ɕK�{�łȂ��`�F�b�N������Ɏ������邪�A
    '�d���P�̂̎d�l�ɂ����ĔF�߂��Ă���l�͑S�ċ��e����B
    '�܂�A�󋵂Ɉˑ������l�̃`�F�b�N�͌Ăь��̐Ӗ��ł���B
    Function GetBodyFormatViolation() As NakCauseCode

    '�n���ꂽ�d���̎�ނ������ł��邩���肷�郁�\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    Function IsSameKindWith(ByVal oTeleg As ITelegram) As Boolean

    '�\�P�b�g�ւ̏o�̓��\�b�h
    'NOTE: timeoutBaseTicks��0�܂���-1���w�肷��Ɩ������ҋ@�ƂȂ�B
    'NOTE: �O���v���̉\��������SocketException�����������ꍇ�ȂǁA
    '�R�l�N�V�����I���Ɏ������ނׂ��ł���i�v���O�����ُ̈�ƈ����ׂ��łȂ��j
    '�P�[�X�ł́A�������ۂ�����ŋL�^���AFalse�Ŗ߂�B
    Function WriteToSocket( _
       ByVal oSocket As Socket, _
       ByVal timeoutBaseTicks As Integer, _
       ByVal timeoutExtraTicksPerMiB As Integer, _
       Optional ByVal telegLoggingMaxLength As Integer = 0) As Boolean
End Interface

'�R�}���h��ʁi���z�j
Public Enum CmdKind As Integer
    None
    Req
    Ack
    Nak
End Enum
