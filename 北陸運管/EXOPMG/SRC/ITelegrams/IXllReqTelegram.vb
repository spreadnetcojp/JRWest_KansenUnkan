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
''' Type5�`8�̃V�[�P���X��ServerTelegrapher��ClientTelegrapher��
''' �z�肷�鉼�zREQ�d���B
''' </summary>
Public Interface IXllReqTelegram
    Inherits IReqTelegram, IXllTelegram

    '�]���Ώۃt�@�C���p�X�ꗗ�̃��[�J�����x�[�X�f�B���N�g��
    'NOTE: �d�����̂��̗̂v�f�ł͂Ȃ��B
    ReadOnly Property TransferListBase() As String

    '�]���Ώۃt�@�C���p�X�ꗗ
    ReadOnly Property TransferList() As List(Of String)

    '�t�@�C���]���̊���
    'NOTE: �v���g�R���ɂ���āA���ۂ̓d�����ɂ���ɑ������鍀�ڂ͑��݂��Ȃ��͂��ł���B
    '���̏ꍇ�́A�A�v�����ɂ����ăC���X�^���X������ۂ̈����Őݒ肷��B
    ReadOnly Property TransferLimitTicks() As Integer

    'HashValue�����ݒ�ς݂��ۂ�
    'NOTE: �T�[�o����̗v���ŊJ�n����DLL�V�[�P���X��REQ�d���܂���
    '�N���C�A���g����̗v���ŊJ�n����ULL�V�[�P���X��REQ�d���ł���ꍇ�̂݁A
    'Telegrapher�͂��̃v���p�e�B���Q�Ƃ���B����āA����ȊO�̓d���̏ꍇ�́A
    '�K���ȌŒ�l��Ԃ��悤�Ɏ������Ă��悢�B
    'NOTE: HashValue����������̂悤�ɏ璷�ȏ����̃v���g�R���̏ꍇ�́A
    'HashValue���w��Ő����������_�ŁA�k�������̂悤�Ƀn�b�V���l��
    '�݂Ȃ��Ȃ��l���i�[���Ă����A����𔻒肷��Ƃ悢�B
    'HashValue���̏����ɏ璷�����Ȃ��v���g�R���̏ꍇ�́A�d���o�C�g��Ƃ�
    '�ʂ�Boolean�^�����o�i_HasHashValue�j��p�ӂ��AHashValue���w��Ő���
    '�������_�ł͂����False�Ƃ��AImportFileDependentValueFromFoo()��
    'UpdateHashValue()��True�ɂ���Ƃ悢�B
    ReadOnly Property IsHashValueReady() As Boolean

    'HashValue���̒l�ƃt�@�C���̓��e���������Ă��邩
    'NOTE: �T�[�o����̗v���ŊJ�n����ULL�V�[�P���X��REQ�d���ł���ꍇ�A
    '���ۂ̓d����HashValue���͂Ȃ��͂��ł���B
    '�n�b�V���l�̃`�F�b�N���s���v���g�R���ł���Ȃ�΁A���̏ꍇ���A
    'ImportFileDependentValueFromFoo()��UpdateHashValue()�ŕύX�����
    '�����o�ϐ����t�@�C������Z�o�����n�b�V���l�Ɣ�r���邱�ƁB
    '�n�b�V���l�̃`�F�b�N���s��Ȃ��v���g�R���ł���Ȃ�΁A��������
    'True��Ԃ��Ă悢�B
    ReadOnly Property IsHashValueIndicatingOkay() As Boolean

    'ACK�d���𐶐����郁�\�b�h
    'NOTE: ACK�d����HashValue�������݂���ꍇ�����A
    '�T�[�o����̗v���ŊJ�n����ULL�V�[�P���X�̏ꍇ�܂��́A
    '�N���C�A���g����̗v���ŊJ�n����DLL�V�[�P���X�̏ꍇ�܂��́A
    '�N���C�A���g����̗v���ŊJ�n����ULL�V�[�P���X�̏ꍇ�́A
    'Me�Ɋi�[����Ă���n�b�V���l�𐶐�����ACK�d���ɃR�s�[����B
    '�Ȃ��A�T�[�o����̗v���ŊJ�n����ULL�V�[�P���X�̏ꍇ�A
    '���ۂ�REQ�d����HashValue���͂Ȃ��͂��ł��邪�A
    '�n�b�V���l�̃`�F�b�N���s���v���g�R���ł���Ȃ�΁A
    '���̏ꍇ���AImportFileDependentValueFromAck()�Ȃǂ�
    '�ύX����郁���o�ϐ��̒l���R�s�[���邱�ƁB
    Function CreateAckTelegram() As IXllTelegram

    '�n���ꂽ�d���̌^��ACK�d���̌^�ɕϊ����郁�\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: �ϊ���̃I�u�W�F�N�g�ɑ΂���GetBodyFormatViolation()�̎��s���A�Ăь��̐Ӗ��ł���B
    Shadows Function ParseAsAck(ByVal oReplyTeleg As ITelegram) As IXllTelegram

    '�n���ꂽ�d���̌^�𓯈�^�ɕϊ����郁�\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: �ϊ���̃I�u�W�F�N�g�ɑ΂���GetBodyFormatViolation()�̎��s���A�Ăь��̐Ӗ��ł���B
    'NOTE: �v���g�R����Łi����M�o�C�g����ɁjReplyLimitTicks�����̏�񂪑��݂��Ȃ�
    '�d���t�H�[�}�b�g�̏ꍇ�AReplyLimitTicks��REQ�d���𑗐M���鑤�݂̂��Q�Ƃ�����̂�
    '����́A�K���Ȓl�i0�Ȃǁj�������Őݒ肳���΂悢�B
    'NOTE: �v���g�R����Łi����M�o�C�g����ɁjTransferLimitTicks�����̏�񂪑��݂��Ȃ�
    '�d���t�H�[�}�b�g�̏ꍇ�ATransferLimitTicks�ɂ�Me�Ɠ����l���ݒ肳�����̂Ƃ���B
    Function ParseAsSameKind(ByVal oNextTeleg As ITelegram) As IXllReqTelegram

    '�n���ꂽ����^�d����ObjDetail��������̃t�@�C���]���������Ă��邩���肷�郁�\�b�h
    'NOTE: ���̃��\�b�h�́A2��XllReq�d�����P��V�[�P���X�̂��̂ł��邩
    '���肷�邽�߂Ɂi�T�[�o���Łj�g�p�����B
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �d���C���X�^���X�̌^������ł��邱�Ƃ��A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���y�у{�f�B���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    Function IsContinuousWith(ByVal oXllReqTeleg As IXllReqTelegram) As Boolean

    'ACK�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �d���C���X�^���X�̌^��Me��ACK�d���̌^�ł��邱�Ƃ��A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: Me���T�[�o����̗v���ŊJ�n����ULL�V�[�P���X��REQ�d���̏ꍇ�܂���
    '�N���C�A���g����̗v���ŊJ�n����DLL�V�[�P���X��REQ�d���̏ꍇ�̂�
    '�Ăяo�����B�O�҂̂悤�ȓd���ɂ����āA�n�b�V���l���i�[�ł��鍀�ڂ�
    '�Ȃ��͂��ł��邪�A���̏ꍇ�́A���ۂ̓d�����e��ێ�����ϐ��Ƃ�
    '�ʂ̃����o�ϐ��Ƀn�b�V���l���i�[����B
    '��L�ɓ��Ă͂܂�Ȃ��d���ɂ����ẮAACK�d���Ƀn�b�V���l�����݂��Ȃ�
    '�ꍇ������Ǝv���邪�A���̃��\�b�h���́A�Ăяo����邱�Ƃ��Ȃ����߁A
    '�������Ȃ����\�b�h�ɂ��Ă悢�B
    'NOTE: �d����Ƀt�@�C���]�������ɑ������鍀�ڂ��Ȃ��v���g�R���̏ꍇ�A
    'Me�̍\�z���Ƀt�@�C���]��������ݒ�ς݂ł��邽�߁A���̃��\�b�h�ɂ�����
    '�t�@�C���]�������̎�荞�݂͕s�v�ł���B
    Sub ImportFileDependentValueFromAck(ByVal oReplyTeleg As IXllTelegram)

    '����^�d������n�b�V���l��t�@�C���]����������荞�ރ��\�b�h
    'NOTE: �d������������ł��邱�Ƃ́A�Ăь����ۏ؂���B
    'NOTE: �d���C���X�^���X�̌^������ł��邱�Ƃ��A�Ăь����ۏ؂���B
    'NOTE: �Ăь��́AMe�����łȂ��A�����œn���d���ɂ��Ă��A
    '�w�b�_���ɏ����ᔽ���������Ƃ��m�F�ς݂łȂ���΂Ȃ�Ȃ��B
    'NOTE: Me���T�[�o����̗v���ŊJ�n����ULL�V�[�P���X��REQ�d���̏ꍇ�܂���
    '�N���C�A���g����̗v���ŊJ�n����ULL�V�[�P���X��REQ�d���̏ꍇ�̂�
    '�Ăяo�����B�O�҂̂悤�ȓd���ɂ����āA�n�b�V���l���i�[�ł��鍀�ڂ�
    '�Ȃ��͂��ł��邪�A���̏ꍇ�́A���ۂ̓d�����e��ێ�����ϐ��Ƃ�
    '�ʂ̃����o�ϐ��Ƀn�b�V���l���i�[�ł���悤�ɂ��Ă����A������R�s�[����B
    Sub ImportFileDependentValueFromSameKind(ByVal oPreviousTeleg As IXllReqTelegram)

    'HashValue���ɒl��ݒ肷�郁�\�b�h
    'NOTE: �T�[�o����̗v���ŊJ�n����ULL�V�[�P���X��REQ�d���ł���ꍇ�A
    '���ۂ̓d����HashValue���͂Ȃ��͂��ł���B
    '���̂悤�ȃV�[�P���X�̓d���ł���ꍇ���A�n�b�V���l�̃`�F�b�N���s��
    '�v���g�R���ł���Ȃ�΁A���ۂ̓d�����e��ێ�����ϐ��Ƃ͕ʂ�
    '�����o�ϐ���p�ӂ��Ă����A�����Ƀn�b�V���l���i�[���Ȃ���΂Ȃ�Ȃ��B
    Sub UpdateHashValue()
End Interface
