' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/01/14  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    '�E�B���h�E�^�C�g��
    Public Shared FormTitle As New Sentence("���d�w���@��")

    '���b�Z�[�W�{�b�N�X�����܂��̓f�[�^�O���b�h�r���[�̃G���[����
    Public Shared DoNotExecInSameWorkingDir As New Sentence("����̍�ƃt�H���_�ŕ����N�����Ȃ��ł��������B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UiStateDeserializeFailed As New Sentence("��ԃt�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UiStateSerializeFailed As New Sentence("��ԃt�@�C���̏������݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UnwelcomeExceptionCaught As New Sentence("�p���s�\�Ȉُ�����o���܂����B\n{0}", SentenceAttr.Error)
    Public Shared MessageQueueServiceNotAvailable As New Sentence("���b�Z�[�W�L���[�T�[�r�X�������ł��B\n�O���풓�v���Z�X�Ƃ̘A�g�͍s���܂���B", SentenceAttr.Warning)
    Public Shared MessageQueueDeleteFailed As New Sentence("���b�Z�[�W�L���[�̍폜�Ɏ��s���܂����B\n�s�v�ł���Ύ蓮�ō폜���Ă��������B", SentenceAttr.Error)
    Public Shared TheTelegrapherAborted As New Sentence("�d������M�X���b�h�̒�~�����o���܂����B\n�d������M�X���b�h�̍ċN�����s���܂�\n�@�햼: {0}", SentenceAttr.Error)
    Public Shared TheInputValueIsUnsuitableForObjCode As New Sentence("�f�[�^��ʂ�2����16�i������͂��Ă��������B", SentenceAttr.Warning)
    Public Shared TheInputValueIsDuplicative As New Sentence("�L�[������̍s�����݂��܂��B", SentenceAttr.Warning)
    Public Shared TransferNameIsInvalid As New Sentence("�]�������s���ł��B", SentenceAttr.Warning)
    Public Shared FilePathIsInvalid As New Sentence("�t�@�C�������s���ł��B", SentenceAttr.Warning)
    Public Shared LogDispFilterIsInvalid As New Sentence("�t�B���^���s���ł��B�ĕҏW���Ă��������B", SentenceAttr.Error)

    '���O�\���O���b�h�̗�w�b�_����
    Public Shared LogDispTimeColumnTitle As New Sentence("Time")
    Public Shared LogDispSourceColumnTitle As New Sentence("Source")
    Public Shared LogDispMessageColumnTitle As New Sentence("Message")

    '�c�[���`�b�v����
    Public Shared DataKindTipText As New Sentence("2����16�i�����w�肵�Ă��������B")
    Public Shared ActiveSeqTransferNameTipText As New Sentence( _
        "FTP�T�C�g��ł̃t�@�C�������w�肵�Ă��������B\n" & _
        "�q���g1: ""%""�Ŏn�܂镶����́A�O�����ňȉ��̂悤�ɒu������܂��B\n" & _
        " %��M  : �V�[�P���X�����s����w���@��̋@��R�[�h\n" & _
        " %��R  : �V�[�P���X�����s����w���@��̐���R�[�h\n" & _
        " %��S  : �V�[�P���X�����s����w���@��̉w���R�[�h\n" & _
        " %��C  : �V�[�P���X�����s����w���@��̃R�[�i�[�R�[�h\n" & _
        " %��U  : �V�[�P���X�����s����w���@��̍��@�ԍ�\n" & _
        " %��I  : �V�[�P���X�����s����w���@��̍��ԁi�V�~�����[�^���ł̒ʂ��ԍ��j\n" & _
        " %T��R : �[���@��̐���R�[�h\n" & _
        " %T��S : �[���@��̉w���R�[�h\n" & _
        " %T��C : �[���@��̃R�[�i�[�R�[�h\n" & _
        " %T��U : �[���@��̍��@�ԍ�\n" & _
        " %T��I : �[���@��̍��ԁi�w���@����ł̒ʂ��ԍ��j\n" & _
        " %%   : 1������""%""\n" & _
        " ���ɂ�1�`9�̐������L�q���Ă��������B���̌����ɂȂ�悤�[�����߂��s���܂��B\n" & _
        " �����L�q���Ȃ��ꍇ�́A�[���T�v���X���s���܂��B\n" & _
        " �[���@��Ƃ́A�V�[�P���X�����s����w���@��̔z���ɂ���@��̂��Ƃł��B\n" & _
        " �[���@��̃R�[�h�ɒu�������L���i%T�`�j���P�ł��L�q����ƁA\n" & _
        " �V�[�P���X�̎��s�͒[���@��ʂɍs���܂��B\n" & _
        "�q���g2: ""$[�V���{����]""��""$�֐���<�������X�g>""�Ƀ}�b�`���镔���́A���L��̂悤�ɃV�i���I�Ɠ������@�ŕ]������܂��B\n" & _
        " $[$] : 1������""$""\n" & _
        " $Trim<������> : �g���~���O����������\n" & _
        " �������A$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> �͕s���Ȏ��Ƃ݂Ȃ��܂��B")
    Public Shared ActiveSeqApplyFileTipText As New Sentence( _
        "��΃p�X�`���܂��͍�ƃt�H���_����̑��΃p�X�`���ŁA�t�@�C�����w�肵�Ă��������B\n" & _
        "�q���g1: ""%""�Ŏn�܂镶����́A�O�����ňȉ��̂悤�ɒu������܂��B\n" & _
        " %��M  : �V�[�P���X�����s����w���@��̋@��R�[�h\n" & _
        " %��R  : �V�[�P���X�����s����w���@��̐���R�[�h\n" & _
        " %��S  : �V�[�P���X�����s����w���@��̉w���R�[�h\n" & _
        " %��C  : �V�[�P���X�����s����w���@��̃R�[�i�[�R�[�h\n" & _
        " %��U  : �V�[�P���X�����s����w���@��̍��@�ԍ�\n" & _
        " %��I  : �V�[�P���X�����s����w���@��̍��ԁi�V�~�����[�^���ł̒ʂ��ԍ��j\n" & _
        " %T��R : �[���@��̐���R�[�h\n" & _
        " %T��S : �[���@��̉w���R�[�h\n" & _
        " %T��C : �[���@��̃R�[�i�[�R�[�h\n" & _
        " %T��U : �[���@��̍��@�ԍ�\n" & _
        " %T��I : �[���@��̍��ԁi�w���@����ł̒ʂ��ԍ��j\n" & _
        " %%   : 1������""%""\n" & _
        " ���ɂ�1�`9�̐������L�q���Ă��������B���̌����ɂȂ�悤�[�����߂��s���܂��B\n" & _
        " �����L�q���Ȃ��ꍇ�́A�[���T�v���X���s���܂��B\n" & _
        " �[���@��Ƃ́A�V�[�P���X�����s����w���@��̔z���ɂ���@��̂��Ƃł��B\n" & _
        " �[���@��̃R�[�h�ɒu�������L���i%T�`�j���P�ł��L�q����ƁA\n" & _
        " �V�[�P���X�̎��s�͒[���@��ʂɍs���܂��B\n" & _
        "�q���g2: ""$[�V���{����]""��""$�֐���<�������X�g>""�Ƀ}�b�`���镔���́A���L��̂悤�ɃV�i���I�Ɠ������@�ŕ]������܂��B\n" & _
        " $[$] : 1������""$""\n" & _
        " $MachineDir<> : �V�[�P���X�����s����w���@��̍�ƃf�B���N�g���i��΃p�X�j\n" & _
        " �������A$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> �͕s���Ȏ��Ƃ݂Ȃ��܂��B")
    Public Shared PassiveSeqApplyFileTipText As New Sentence( _
        "��΃p�X�`���܂��͍�ƃt�H���_����̑��΃p�X�`���ŁA�t�@�C�����w�肵�Ă��������B\n" & _
        "�q���g1: ""%""�Ŏn�܂镶����́A�O�����ňȉ��̂悤�ɒu������܂��B\n" & _
        " %��M  : �V�[�P���X�����s����w���@��̋@��R�[�h\n" & _
        " %��R  : �V�[�P���X�����s����w���@��̐���R�[�h\n" & _
        " %��S  : �V�[�P���X�����s����w���@��̉w���R�[�h\n" & _
        " %��C  : �V�[�P���X�����s����w���@��̃R�[�i�[�R�[�h\n" & _
        " %��U  : �V�[�P���X�����s����w���@��̍��@�ԍ�\n" & _
        " %��I  : �V�[�P���X�����s����w���@��̍��ԁi�V�~�����[�^���ł̒ʂ��ԍ��j\n" & _
        " %%    : 1������""%""\n" & _
        " ���ɂ�1�`9�̐������L�q���Ă��������B���̌����ɂȂ�悤�[�����߂��s���܂��B\n" & _
        " �����L�q���Ȃ��ꍇ�́A�[���T�v���X���s���܂��B\n" & _
        "�q���g2: ""$[�V���{����]""��""$�֐���<�������X�g>""�Ƀ}�b�`���镔���́A���L��̂悤�ɃV�i���I�Ɠ������@�ŕ]������܂��B\n" & _
        " $[$] : 1������""$""\n" & _
        " $MachineDir<> : �V�[�P���X�����s����w���@��̍�ƃf�B���N�g���i��΃p�X�j\n" & _
        " �������A$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> �͕s���Ȏ��Ƃ݂Ȃ��܂��B")
    Public Shared ScenarioFileTipText As New Sentence( _
        "��΃p�X�`���܂��͍�ƃt�H���_����̑��΃p�X�`���ŁA�t�@�C�����w�肵�Ă��������B\n" & _
        "�q���g1: ""%""�Ŏn�܂镶����́A�O�����ňȉ��̂悤�ɒu������܂��B\n" & _
        " %��M  : �V�i���I�����s����w���@��̋@��R�[�h\n" & _
        " %��R  : �V�i���I�����s����w���@��̐���R�[�h\n" & _
        " %��S  : �V�i���I�����s����w���@��̉w���R�[�h\n" & _
        " %��C  : �V�i���I�����s����w���@��̃R�[�i�[�R�[�h\n" & _
        " %��U  : �V�i���I�����s����w���@��̍��@�ԍ�\n" & _
        " %��I  : �V�i���I�����s����w���@��̍��ԁi�V�~�����[�^���ł̒ʂ��ԍ��j\n" & _
        " %%    : 1������""%""\n" & _
        " ���ɂ�1�`9�̐������L�q���Ă��������B���̌����ɂȂ�悤�[�����߂��s���܂��B\n" & _
        " �����L�q���Ȃ��ꍇ�́A�[���T�v���X���s���܂��B\n" & _
        "�q���g2: ""$[�V���{����]""��""$�֐���<�������X�g>""�Ƀ}�b�`���镔���́A���L��̂悤�ɃV�i���I�Ɠ������@�ŕ]������܂��B\n" & _
        " $[$] : 1������""$""\n" & _
        " $MachineDir<> : �V�i���I�����s����w���@��̍�ƃf�B���N�g���i��΃p�X�j\n" & _
        " �������A$ContextNum<>, $ContextDir<>, $SetRef<...>, $SetVal<...>, $Val<...>, $ExecDynFunc<...>, $ExecCmdFunc<...>, $ExecAppFunc<...> �͕s���Ȏ��Ƃ݂Ȃ��܂��B")

    '�����ԕ���
    Public Shared LineStatusInitial As New Sentence("")
    Public Shared LineStatusConnectWaiting As New Sentence("�ڑ���...")
    Public Shared LineStatusConnectFailed As New Sentence("�ڑ����s")
    Public Shared LineStatusConnected As New Sentence("�ڑ���(���J��)")
    Public Shared LineStatusComStartWaiting As New Sentence("�J�ǒ�...")
    Public Shared LineStatusSteady As New Sentence("��")
    Public Shared LineStatusDisconnected As New Sentence("�ؒf")

    '�V�i���I��ԕ���
    Public Shared ScenarioStatusInitial As New Sentence("")
    Public Shared ScenarioStatusLoaded As New Sentence("�ҋ@��...")
    Public Shared ScenarioStatusRunning As New Sentence("���s��")
    Public Shared ScenarioStatusAborted As New Sentence("�I��(�~)")
    Public Shared ScenarioStatusFinished As New Sentence("�I��(��)")
    Public Shared ScenarioStatusStopped As New Sentence("��~")

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
