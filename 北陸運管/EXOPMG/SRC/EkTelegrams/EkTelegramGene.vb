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
''' �d�������B
''' </summary>
Public MustInherit Class EkTelegramGene

#Region "�e���ڂ̊i�[�ʒu"
    'NOTE: �ȉ��̕ϐ��́A�T�u�N���X�̃R���X�g���N�^�Œl��ݒ肷��B
    '�T�u�N���X�ȊO�́AEkTelegram�p�b�P�[�W���ł̎Q�Ƃ̂݉Ƃ���B
    Protected Friend CmdCodePos As Integer
    Protected Friend SubCmdCodePos As Integer
    Protected Friend ReqNumberPos As Integer
    Protected Friend ClientModelCodePos As Integer
    Protected Friend ClientRailSectionCodePos As Integer
    Protected Friend ClientStationOrderCodePos As Integer
    Protected Friend ClientCornerCodePos As Integer
    Protected Friend ClientUnitCodePos As Integer
    Protected Friend SendTimePos As Integer
    Protected Friend ObjSizePos As Integer
    Protected Friend ObjCodePos As Integer
    Protected Friend ObjDetailPos As Integer
#End Region

#Region "�e���ڂ̊i�[��"
    'NOTE: �ȉ��̕ϐ��́A�T�u�N���X�̃R���X�g���N�^�ł��Ƃ肠�����ύX�͕s�Ƃ���B
    '�T�u�N���X�ȊO�́AEkTelegram�p�b�P�[�W���ł̎Q�Ƃ̂݉Ƃ���B
    'NOTE: �n�b�V���}�b�v�������Ɉړ����Ă���Ȃǂɂ��A�قƂ�ǂ̍��ڂ�
    '�T�u�N���X�ŕύX�\�ɂȂ�͂��B
    Protected Friend CmdCodeLen As Integer = 3
    Protected Friend SubCmdCodeLen As Integer = 4
    Protected Friend ReqNumberLen As Integer = 6
    Protected Friend ClientModelCodeLen As Integer = 2
    Protected Friend ClientRailSectionCodeLen As Integer = 3
    Protected Friend ClientStationOrderCodeLen As Integer = 3
    Protected Friend ClientCornerCodeLen As Integer = 4
    Protected Friend ClientUnitCodeLen As Integer = 2
    Protected Friend SendTimeLen As Integer = 17
    Protected Friend ObjSizeLen As Integer = 4
    Protected Friend ObjCodeLen As Integer = 1
#End Region

#Region "���M�J�n�����̏���"
    'NOTE: �ȉ��̕ϐ��́A�T�u�N���X�̃R���X�g���N�^�Œl��ݒ肵�Ȃ����Ă��悢�B
    'EkTelegram�p�b�P�[�W�O�ł͕ύX���Q�Ƃ��s�Ƃ���B
    Protected Friend SendTimeFormat As String = "yyyyMMddHHmmssfff"
#End Region

#Region "�\�P�b�g��X�g���[�����琶������EkTelegram�̐����l"
    'NOTE: �ȉ��̕ϐ��́A�T�u�N���X�̃R���X�g���N�^�Œl��ݒ肷��B
    '�T�u�N���X�ȊO�́AEkTelegram�p�b�P�[�W���ł̎Q�Ƃ̂݉Ƃ���B
    'NOTE: ���L�̏����𖞂����l��ݒ肷�邱�ƁB
    'MaxReceiveSize > MinAllocSize >= Gene.GetRawLenByObjSize(Gene.GetObjSizeByObjDetailLen(0))
    Protected Friend MinAllocSize As Integer
    Protected Friend MaxReceiveSize As Integer
#End Region

    'NOTE: �ȍ~�̃����o�́AEkXxxxTelegram����̓A�N�Z�X���Ȃ����j�Ƃ���B
    '���R�́A�����̃N���X���������ɕ��ʂ��āA�ʂ̃v���W�F�N�g��
    '�ڂ����Ƃ��\�ɂ��Ă������߂ł���B
    '�����̃N���X�ł́A�ȍ~�̃����o�𒼐ڃA�N�Z�X���邩���ɁA
    'EkTelegram�ɗp�ӂ��ꂽ���\�b�h��p���ĊԐړI�ɃA�N�Z�X���邱�Ƃ��\�ł���B

#Region "XllReqTelegram�̔w��ɑ��݂�����"
    'NOTE: �ȉ��̕ϐ��́A�T�u�N���X�̃R���X�g���N�^�Œl��ݒ肷��B
    '�T�u�N���X�ȊO�́AEkTelegram�p�b�P�[�W���ł̎Q�Ƃ̂݉Ƃ���B

    'XllReqTelegram.TransferList�̃x�[�X�p�X�i���[�J���p�X�j
    Protected Friend XllBasePath As String
#End Region

#Region "���\�b�h"
    'NOTE: �ȉ��̃��\�b�h�́A�T�u�N���X�̃R���X�g���N�^�Ŏ�������B
    '�T�u�N���X�ȊO�́AEkTelegram�p�b�P�[�W���ł̂ݎg�p�Ƃ���B

    'ObjSize����d���S�̂̃o�C�g�����Z�o���郁�\�b�h
    Protected Friend MustOverride Function GetRawLenByObjSize(ByVal objSize As UInteger) As Integer

    '�d���S�̃o�C�g������ObjSize�ɃZ�b�g����ׂ��l���Z�o���郁�\�b�h
    Protected Friend MustOverride Function GetObjSizeByRawLen(ByVal rawLen As Integer) As UInteger

    'ObjSize����ObjDetail���̃o�C�g�����Z�o���郁�\�b�h
    Protected Friend MustOverride Function GetObjDetailLenByObjSize(ByVal objSize As UInteger) As Integer

    'ObjDetail���̃o�C�g������ObjSize�ɃZ�b�g����ׂ��l���Z�o���郁�\�b�h
    Protected Friend MustOverride Function GetObjSizeByObjDetailLen(ByVal objDetailLen As Integer) As UInteger

    'CRC���ɒl���Z�b�g���郁�\�b�h
    Protected Friend MustOverride Sub UpdateCrc(ByVal aRawBytes As Byte())

    'CRC���̒l�Ƃ��̑��̕��ʂ̒l�̐��������`�F�b�N���郁�\�b�h
    Protected Friend MustOverride Function IsCrcIndicatingOkay(ByVal aRawBytes As Byte()) As Boolean
#End Region

End Class
