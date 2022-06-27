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

''' <summary>
''' ServerTelegrapher��ClientTelegrapher���z�肷�鉼�zREQ�d���B
''' </summary>
Public Interface IReqTelegram
    Inherits ITelegram

    '�����ԐM����
    'NOTE: �v���g�R���ɂ���āA���ۂ̓d�����ɂ���ɑ������鍀�ڂ͑��݂��Ȃ��͂��ł���B
    '���̏ꍇ�́A�A�v�����ɂ����ăC���X�^���X������ۂ̈����Őݒ肷��B
    'REQ�d���𑗐M���鑤�݂̂��Q�Ƃ���̂ŁA��M�������e�s���d������REQ�d����
    '�C���X�^���X���쐬����ۂ́A�K���Ȓl�i0�Ȃǁj�������Őݒ肳���΂悢�B
    ReadOnly Property ReplyLimitTicks() As Integer

    '�n���ꂽ�d����ACK�Ƃ��Đ����������邩���肷�郁�\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: �����œn���d����CmdKind��Ack�ł��邱�Ƃ��m�F�ς݂Ƃ���B
    Function IsValidAck(ByVal oReplyTeleg As ITelegram) As Boolean

    '�n���ꂽ�d����NAK�Ƃ��Đ����������邩���肷�郁�\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: �����œn���d����CmdKind��Nak�ł��邱�Ƃ��m�F�ς݂Ƃ���B
    Function IsValidNak(ByVal oReplyTeleg As ITelegram) As Boolean

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: �ϊ���̃I�u�W�F�N�g�ɑ΂���GetBodyFormatViolation()�̎��s���A�Ăь��̐Ӗ��ł���B
    Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As ITelegram

    '�n���ꂽ�d���̌^��NAK�d���̌^�ɕϊ����郁�\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: �ϊ���̃I�u�W�F�N�g�ɑ΂���GetBodyFormatViolation()�̎��s���A�Ăь��̐Ӗ��ł���B
    Function ParseAsNak(ByVal oReplyTeleg As ITelegram) As INakTelegram
End Interface 
