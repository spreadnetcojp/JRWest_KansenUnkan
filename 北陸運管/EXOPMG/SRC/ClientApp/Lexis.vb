' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2013/11/11  (NES)����  �t�F�[�Y�Q�����Ή�
'   0.2      2014/06/10  (NES)����  �k���Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    'NOTE: ���샍�O�ȊO�̃��O�́A�����ł͂Ȃ�Log.Xxxx���\�b�h�̌Ăяo���ӏ��ɁA�����񃊃e�����𒼐ڋL�q���邱�ƁB

    'NOTE: �I���W�i�������ł́A�o�͂ł�����͑S�ďo�͂���悤�ɂ��A�����g�p�ӏ�������ɍ��킹�Ĉ�����n���悤�ɂ���B
    '���Ǝ҂��Ƃ̎d�l�ŕs�v�ȏ�񂪂���΁A���Y���Ǝҗp��INI�t�@�C���ɁA���̏����Ԉ������������`����B

    '���b�Z�[�W�{�b�N�X����
    Public Shared EnvVarNotFound As New Sentence("���ϐ�{0}���ݒ肳��Ă��܂���B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared SweepOperationLogsFailed As New Sentence("�Â����샍�O���폜�ł��܂���ł����B", SentenceAttr.Warning)
    Public Shared SweepLogsFailed As New Sentence("�Â����O���폜�ł��܂���ł����B", SentenceAttr.Warning)

    Public Shared DatabaseOpenErrorOccurred As New Sentence("DB�ڑ��Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared NoRecordsFound As New Sentence("���������Ɉ�v����f�[�^�͑��݂��܂���B", SentenceAttr.Information)
    Public Shared HugeRecordsFound As New Sentence("�������ʂ�{0}���𒴂��Ă��܂��B\n�������i�荞��ł��������B", SentenceAttr.Warning)
    Public Shared DatabaseSearchErrorOccurred As New Sentence("���������Ɏ��s���܂����B", SentenceAttr.Error)

    Public Shared SheetProcAbnormalEnd As New Sentence("�ꗗ�\�������Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared FormProcAbnormalEnd As New Sentence("��ʕ\�������Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ComboBoxSetupFailed As New Sentence("��ʕ\�������Ɏ��s���܂����B\n�y{0}�ݒ莸�s�z", SentenceAttr.Error)

    Public Shared LedgerTemplateNotFound As New Sentence("�\�����ʃG���[���������܂����B\n���ݒ�G���[�����B", SentenceAttr.Error)
    Public Shared ReallyPrinting As New Sentence("�f�[�^�̏o�͂Ɏ��Ԃ�������܂���\n��낵���ł����H", SentenceAttr.Question)
    Public Shared PrintingErrorOccurred As New Sentence("�o�͏����Ɏ��s���܂����B", SentenceAttr.Error)

    Public Shared LoginFailed As New Sentence("���O�C�������Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared LoginFailedBecauseTheIdCodeHasBeenLockedOut As New Sentence("ID�R�[�h�����b�N�A�E�g����Ă��܂��B\n�����̓V�X�e���Ǘ��҂ɘA�����Ă��������B", SentenceAttr.Error)
    Public Shared LoginFailedBecauseTheIdCodeIsIncorrect As New Sentence("���O�C�����ꂽID�R�[�h�͓o�^����Ă��܂���B", SentenceAttr.Warning)
    Public Shared LoginFailedBecauseThePasswordIsIncorrect As New Sentence("ID�R�[�h�ƃp�X���[�h����v���܂���B\n���͂������Ă��������B", SentenceAttr.Warning)

    Public Shared InputParameterIsIncomplete As New Sentence("{0}�����͂���Ă��܂���B", SentenceAttr.Warning)
    Public Shared CompetitiveOperationDetected As New Sentence("���̃��[�U�[�ɂ��Y���f�[�^���X�V����܂����̂ŁA\n�Č������Ă��������B", SentenceAttr.Warning)
    Public Shared ReallyInsert As New Sentence("�o�^���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared InsertCompleted As New Sentence("�o�^����������ɏI�����܂����B", SentenceAttr.Information)
    Public Shared InsertFailed As New Sentence("�o�^�����Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ReallyUpdate As New Sentence("�X�V���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared UpdateCompleted As New Sentence("�X�V����������ɏI�����܂����B", SentenceAttr.Information)
    Public Shared UpdateFailed As New Sentence("�X�V�����Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared ReallyDelete As New Sentence("�폜���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared DeleteCompleted As New Sentence("�폜����������ɏI�����܂����B", SentenceAttr.Information)
    Public Shared DeleteFailed As New Sentence("�폜�����Ɏ��s���܂����B", SentenceAttr.Error)

    Public Shared NoIdCodeExists As New Sentence("ID�}�X�^��񂪓o�^����Ă��܂���B", SentenceAttr.Warning)
    Public Shared TheIdCodeAlreadyExists As New Sentence("ID�R�[�h{0}�͊��ɓo�^����Ă��܂��B", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForIdCode As New Sentence("ID�R�[�h��8���̉p�����œ��͂��Ă��������B", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForPassword As New Sentence("�p�X���[�h��4���`8���̉p�����œ��͂��Ă��������B", SentenceAttr.Warning)
    Public Shared ThePasswordsDifferFromOneAnother As New Sentence("�p�X���[�h�ƃp�X���[�h�m�F���s��v�ł��B", SentenceAttr.Warning)
    Public Shared ReallyDeleteTheIdCode As New Sentence("�폜���Ă���낵���ł����H\nID�R�[�h{0}", SentenceAttr.Question)

    Public Shared ThePatternNoAlreadyExists As New Sentence("�p�^�[��No{0}�͊��ɓo�^����Ă��܂��B", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForPatternNo As New Sentence("�p�^�[��No��2���̐����œ��͂��Ă��������B", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForPatternName As New Sentence("���͒l���s���ł��B", SentenceAttr.Warning)
    Public Shared PatternNoIsFull As New Sentence("�@��P�ʂœo�^�ł���p�^�[�������𒴂��Ă��܂��B\n99���ȓ��œo�^���Ă��������B", SentenceAttr.Warning)

    Public Shared TheAreaNoAlreadyExists As New Sentence("�G���ANo{0}�͊��ɓo�^����Ă��܂��B", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForAreaNo As New Sentence("�G���ANo��2���̐����œ��͂��Ă��������B", SentenceAttr.Warning)
    Public Shared TheInputValueIsUnsuitableForAreaName As New Sentence("���͒l���s���ł��B", SentenceAttr.Warning)
    Public Shared AreaNoIsFull As New Sentence("�@��P�ʂœo�^�ł���G���A�����𒴂��Ă��܂��B\n10���ȓ��œo�^���Ă��������B", SentenceAttr.Warning)

    Public Shared MachineMasterFormatFileNotFound As New Sentence("������`�t�@�C�������݂��܂���B\n�ݒ���m�F���Ă��������B", SentenceAttr.Error)
    Public Shared TheFileNameIsUnsuitableForMachineMaster As New Sentence("�Ǎ��Ώۃt�@�C�����s���ł��B", SentenceAttr.Error)
    Public Shared MachineMasterFileNotFound As New Sentence("�Ǎ��Ώۃt�@�C�������݂��܂���B", SentenceAttr.Error)
    Public Shared MachineMasterFileReadFailed As New Sentence("�Ǎ������Ɏ��s���܂����B\n���O���m�F���Ă��������B", SentenceAttr.Error)
    Public Shared MachineMasterInsertFailed As New Sentence("�o�^�����Ɏ��s���܂����B\n�ݒ�t�@�C�����m�F���Ă��������B", SentenceAttr.Error)
    Public Shared MachineMasterInsertFailed2 As New Sentence("�o�^�����Ɏ��s���܂����B\n���O���m�F���Ă��������B", SentenceAttr.Error)

    '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD START-----------
    Public Shared IdMstFormatFileNotFound As New Sentence("������`�t�@�C�������݂��܂���B\n�ݒ���m�F���Ă��������B", SentenceAttr.Error)
    Public Shared TheFileNameIsUnsuitableForIdMst As New Sentence("�Ǎ��Ώۃt�@�C�����s���ł��B", SentenceAttr.Error)
    Public Shared IdMstFileNotFound As New Sentence("�Ǎ��Ώۃt�@�C�������݂��܂���B", SentenceAttr.Error)
    Public Shared IdMstFileReadFailed As New Sentence("�Ǎ������Ɏ��s���܂����B\n�ʂ̃v���Z�X�Ŏg�p����Ă��邽�߃A�N�Z�X�ł��܂���B", SentenceAttr.Error)
    Public Shared IdMstInsertFailed As New Sentence("�o�^�����Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared IdMstImport As New Sentence("�f�[�^�̎捞�݂Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared IdMstExport As New Sentence("�f�[�^�̕ۑ��Ɏ��s���܂����B", SentenceAttr.Error)
    Public Shared IdMstImportlog As New Sentence("���O�o�͂Ɏ��s���܂����B", SentenceAttr.Error)
    '-------Ver0.1�@�t�F�[�Y�Q�����Ή� ADD  END-------------
    Public Shared TheInputValueIsUnsuitableForFaultDataErrorCode As New Sentence("�G���[�R�[�h�̓��͂Ɍ�肪����܂��B", SentenceAttr.Warning)

    Public Shared TheFileTypeIsInvalid As New Sentence("�I�����ꂽ�t�@�C����{0}�ł͂���܂���B", SentenceAttr.Warning)
    Public Shared ThePatternNoDoesNotExist As New Sentence("�p�^�[��No���o�^����Ă��܂���B", SentenceAttr.Warning)
    Public Shared TheAreaNoDoesNotExist As New Sentence("�G���ANo���o�^����Ă��܂���B", SentenceAttr.Warning)
    Public Shared ConnectFailed As New Sentence("�ڑ������Ɏ��s���܂����B", SentenceAttr.Error)

    '-------Ver0.2�@�k���Ή��@ADD START-----------
    Public Shared ThePatternNoDoesNotRelated As New Sentence("�}�X�^�Ɋ֘A����p�^�[��No�ł͂���܂���B", SentenceAttr.Warning)
    Public Shared ApplicationListExcludedStationIncluded As New Sentence("�K�p���X�g�ɑΏۊO�̉w���܂܂�Ă��܂��B", SentenceAttr.Warning)
    '-------Ver0.2�@�k���Ή��@ADD END-----------

    Public Shared ReallyUllMasProFile As New Sentence("�o�^���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared UllMasProFileCompleted As New Sentence("�o�^���������܂����B", SentenceAttr.Information)
    Public Shared UllMasProFileFailed As New Sentence("�o�^�ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByBusy As New Sentence("���̒[���ő��쒆�̂��߁A�o�^�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByInvalidContent As New Sentence("�ُ�ȃt�@�C���̂��߁A�o�^�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared UllMasProFileFailedByUnknownLight As New Sentence("���炩�̌����ŁA�o�^�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared ReallyExitWithoutUll As New Sentence("�f�[�^���o�^����Ă��܂���B\n�I�����Ă���낵���ł����H", SentenceAttr.Question)

    Public Shared ReallyInvokeMasProDll As New Sentence("�z�M���J�n���Ă���낵���ł����H", SentenceAttr.Question)
    Public Shared InvokeMasProDllCompleted As New Sentence("�z�M���J�n���܂����B", SentenceAttr.Information)
    Public Shared InvokeMasProDllFailed As New Sentence("�z�M�̊J�n�ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByBusy As New Sentence("���̒[���ő��쒆�̂��߁A�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByNoData As New Sentence("�z�M�Ώۃf�[�^���o�^����Ă��Ȃ����߁A�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByUnnecessary As New Sentence("�K�p���X�g����V���Ȕz�M�悪�݂���܂���ł����B", SentenceAttr.Warning)
    Public Shared InvokeMasProDllFailedByInvalidContent As New Sentence("�z�M�Ώۃf�[�^���ُ�Ȃ��߁A�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)
    Public Shared InvokeMasProDllFailedByUnknownLight As New Sentence("���炩�̌����ŁA�z�M���J�n�ł��܂���ł����B", SentenceAttr.Error)

    Public Shared KensyuRangeIsInvalid As New Sentence("���C�̎w�肪����Ă��܂��B", SentenceAttr.Warning)
    Public Shared DateRangeIsInvalid As New Sentence("���t�̎w�肪����Ă��܂��B", SentenceAttr.Warning)
    Public Shared PrintEndItClearDate As New Sentence("�o�͂��I�����܂����B\n�N���A���t���ׂ����@�̂�����܂����B", SentenceAttr.Information)
    Public Shared PrintEndItMachineChange As New Sentence("�o�͂��I�����܂����B\n�@�̂̈ڐ݂�����܂����B", SentenceAttr.Information)
    Public Shared PrintEndItDateReverse As New Sentence("�o�͂��I�����܂����B\n�J�n�����ƏI�������̓��t�̊֌W���t�̋@�̂�����܂����B", SentenceAttr.Information)


    '���[�̕���
    Public Shared PassageInfo As New Sentence("�ʘH�����F{0}")
    Public Shared TimeSpan As New Sentence("{0} {1}�@����@{2} {3}�@�܂�")

    '���샍�O�̕���
    Public Shared WindowSuffix As New Sentence("���")
    Public Shared DialogSuffix As New Sentence("�_�C�A���O")
    Public Shared SheetCellDoubleClicked As New Sentence("{0}�ɂ�{1}��{2}�s{3}����_�u���N���b�N���܂����B�s���e:[{4}]")
    Public Shared DateTimePickerValueChanged As New Sentence("{0}�ɂ�{1}��{2}�ɕύX���܂����B")
    Public Shared ComboBoxSelectionChanged As New Sentence("{0}�ɂ�{1}��{2}�ɕύX���܂����B")
    Public Shared ComboBoxSelectionChangedToNothing As New Sentence("{0}�ɂ�{1}�𖢑I���ɕύX���܂����B")
    Public Shared ButtonClicked As New Sentence("{0}�ɂ�{1}���N���b�N���܂����B")
    Public Shared SomeControlInvoked As New Sentence("{0}�ɂ�{1}({2}�^)�𑀍삵�܂����B")
    Public Shared YesButtonClicked As New Sentence("�͂��{�^�������B")
    Public Shared NoButtonClicked As New Sentence("�������{�^�������B")
    Public Shared OkButtonClicked As New Sentence("OK�{�^�������B")

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
