' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2013/12/10  (NES)����  ������ԏ��̒ǉ��Ή�
'   0.2      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Imports JR.ExOpmg.Common

Public Class Lexis
    Inherits BaseLexis

    '���b�Z�[�W�{�b�N�X�ɕ\�����镶��
    Public Shared EnvVarNotFound As New Sentence("���ϐ�[{0}]���ݒ肳��Ă��܂���B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnReadingConfigFile As New Sentence("�ݒ�t�@�C���̓ǂݍ��݂ňُ킪�������܂����B", SentenceAttr.Error)
    Public Shared SomeErrorOccurredOnInitializingProcess As New Sentence("�v���Z�X�̏������ňُ킪�������܂����B", SentenceAttr.Error)

    '���W�f�[�^��L�e�[�u���ɓo�^����ُ���e�����̕��i
    Public Shared CdtKanshiban As New Sentence("�Ď���")
    Public Shared CdtGate As New Sentence("���D�@")
    Public Shared CdtTokatsu As New Sentence("�����^�d�w����")
    Public Shared CdtMadosho As New Sentence("���������@")
    Public Shared CdtGeneralDataPort As New Sentence("�ʏ�f�[�^�p�|�[�g")
    Public Shared CdtRiyoDataPort As New Sentence("���p�f�[�^�p�|�[�g")

    '���W�f�[�^��L�e�[�u���ɓo�^����ُ���e����
    Public Shared CdtProcessAbended As New Sentence("�v���Z�X���ُ�I�����܂����B({0})")
    Public Shared CdtThreadAbended As New Sentence("�X���b�h���ُ�I�����܂����B({0}-{1})")
    Public Shared CdtLineError As New Sentence("{0}��{2}�ŒʐM���s���܂���B(���@:{1})")
    Public Shared CdtNkanLineError As New Sentence("�m�ԃT�[�o�ƒʐM���s���܂���B")
    Public Shared CdtMachineMasterErratumDetected As New Sentence("�@��\���}�X�^�Ɉُ�����o���܂����B")
    Public Shared CdtScheduledUllFailed As New Sentence("{0}����̎��W�Ɏ��s���܂����B(���@:{1})")
    Public Shared CdtReadingTotallyFailed As New Sentence("�S�̂̉�͂����s���܂����B({0}, {1})")
    Public Shared CdtReadingPartiallyFailed As New Sentence("�ꕔ�̉�͂����s���܂����B({0}, {1})")
    Public Shared CdtRecordingFailed As New Sentence("�f�[�^�̓o�^�Ɏ��s���܂����B")
    Public Shared CdtTheUnitNotFound As New Sentence("�@�킪���݂��܂���B(����:{0} �w��:{1} �R�[�i:{2} ���@:{3})")
    Public Shared CdtTheCornerNotFound As New Sentence("�@�킪���݂��܂���B(����:{0} �w��:{1} �R�[�i:{2})")
    Public Shared CdtUnpairedKadoDataDetected As New Sentence("�ғ��E�ێ�f�[�^�̈ꕔ���������Ă��܂��B(���@:{0})")

    '-------Ver0.2 ������ԕ�Ή� MOD START-----------
    'NOTE; ������ԕ�Ή��ŁA���[�������̍��@�ԍ���Integer�œ��͂���i�����ւ����ɏ������w��\�Ƃ���j�悤�ɓ���B
    '�ُ�f�[�^�ʒm���[���̕���
    Public Shared FaultDataMailSubject As New Sentence("{0} {1} {2} {3:D}���@�ňُ�f�[�^���������܂���")
    Public Shared FaultDataMailBody As New Sentence("{0} {1}\n{2}")
    Public Shared DateTimeFormatInFaultDataMailBody As New Sentence("yyyy/MM/dd HH:mm:ss")
    '-------Ver0.2 ������ԕ�Ή� MOD END-------------

    '-------Ver0.2 ������ԕ�Ή� ADD START-----------
    '�ʐM�ُ�̌x�񃁁[���̕���
    Public Shared KanshibanLineErrorAlertMailSubject As New Sentence("�V�����^�ǃT�[�o�� {0} {1} �Ď��� {2:D}���@�Ƃ̒ʏ�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared Kanshiban2LineErrorAlertMailSubject As New Sentence("�V�����^�ǃT�[�o�� {0} {1} �Ď��� {2:D}���@�Ƃ̗��p�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared TokatsuLineErrorAlertMailSubject As New Sentence("�V�����^�ǃT�[�o�� {0} {1} ���� {2:D}���@�Ƃ̒ʏ�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")  'NOTE: �R�[�i�[���͍����ւ��ŏ�������B
    Public Shared MadoshoLineErrorAlertMailSubject As New Sentence("�V�����^�ǃT�[�o�� {0} {1} ���� {2:D}���@�Ƃ̒ʏ�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared Madosho2LineErrorAlertMailSubject As New Sentence("�V�����^�ǃT�[�o�� {0} {1} ���� {2:D}���@�Ƃ̗��p�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared KanshibanLineErrorAlertMailBody As New Sentence("{3} ���\n�V�����^�ǃT�[�o�� {0} {1} �Ď��� {2:D}���@�Ƃ̒ʏ�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared Kanshiban2LineErrorAlertMailBody As New Sentence("{3} ���\n�V�����^�ǃT�[�o�� {0} {1} �Ď��� {2:D}���@�Ƃ̗��p�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared TokatsuLineErrorAlertMailBody As New Sentence("{3} ���\n�V�����^�ǃT�[�o�� {0} {1} ���� {2:D}���@�Ƃ̒ʏ�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")  'NOTE: �R�[�i�[���͍����ւ��ŏ�������B
    Public Shared MadoshoLineErrorAlertMailBody As New Sentence("{3} ���\n�V�����^�ǃT�[�o�� {0} {1} ���� {2:D}���@�Ƃ̒ʏ�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared Madosho2LineErrorAlertMailBody As New Sentence("{3} ���\n�V�����^�ǃT�[�o�� {0} {1} ���� {2:D}���@�Ƃ̗��p�f�[�^�p�|�[�g�ʐM�ُ킪�������Ă��܂��B")
    Public Shared DateTimeFormatInLineErrorAlertMailBody As New Sentence("yyyy/MM/dd HH:mm:ss")
    '-------Ver0.2 ������ԕ�Ή� ADD END-------------

    '-------Ver0.2 ������ԕ�Ή� MOD START-----------
    'NOTE; ������ԕ�Ή��ŁA���[�������̍��@�ԍ���Integer�œ��͂���i�����ւ����ɏ������w��\�Ƃ���j�悤�ɓ���B
    '�@��ڑ���ԃ��[���̕���
    Public Shared ConStatusMailSubject As New Sentence("{0} �V�����^�ǃT�[�o�莞��")
    Public Shared DateTimeFormatInConStatusMailSubject As New Sentence("yyyy/MM/dd HH:mm")
    Public Shared GatePartTitleInConStatusMailBody As New Sentence("�y���D�@��ԏ��z")
    Public Shared KsbLabelInConStatusMailBody As New Sentence("{0} {1} �Ď��� {2:D}���@: ")
    Public Shared KsbOpmgErrorInConStatusMailBody As New Sentence("�^�ǁ~")
    Public Shared GateLabelInConStatusMailBody As New Sentence("{0} {1} ���D�@ {2:D}���@: ")
    Public Shared GatePowerErrorInConStatusMailBody As New Sentence("�d���~")
    Public Shared GateMainKsbErrorInConStatusMailBody As New Sentence("�Ď��Ձ~")
    Public Shared GateMainIcuErrorInConStatusMailBody As New Sentence("�吧��~")
    Public Shared GateMainDsvErrorInConStatusMailBody As New Sentence("�zSV(��)�~")
    Public Shared GateIcuDsvErrorInConStatusMailBody As New Sentence("�zSV(IC)�~")
    Public Shared GateIcuTktErrorInConStatusMailBody As New Sentence("�����~")
    'Ver0.1 ADD ������ԏ��̒ǉ��Ή�
    Public Shared TktPartTitleInConStatusMailBody As New Sentence("�y������ԏ��z")
    Public Shared MadoPartTitleInConStatusMailBody As New Sentence("�y������ԏ��z")
    Public Shared TktLabelInConStatusMailBody As New Sentence("{0} {1} ���� {2:D}���@: ")  'NOTE: �R�[�i�[���͍����ւ��ŏ�������B
    Public Shared TktOpmgErrorInConStatusMailBody As New Sentence("�^�ǁ~")
    'Ver0.1 ADD ������ԏ��̒ǉ��Ή�
    Public Shared TktIdcErrorInConStatusMailBody As New Sentence("�Z���^�[�~")
    Public Shared MadoLabelInConStatusMailBody As New Sentence("{0} {1} ���� {2:D}���@: ")
    Public Shared MadoTktIdErrorInConStatusMailBody As New Sentence("����(ID)�~")
    Public Shared MadoTktDlErrorInConStatusMailBody As New Sentence("����(DL)�~")
    Public Shared MadoKsbErrorInConStatusMailBody As New Sentence("�Ď��Ձ~")
    Public Shared MadoDsvErrorInConStatusMailBody As New Sentence("�zSV�~")
    Public Shared ErrorSeparatorInConStatusMailBody As New Sentence(" ")
    '-------Ver0.2 ������ԕ�Ή� MOD END-------------

    ''' <summary>INI�t�@�C���̓��e����荞�ށB</summary>
    ''' <remarks>
    ''' INI�t�@�C���̓��e����荞�ށB
    ''' </remarks>
    Public Shared Sub Init(ByVal sIniFilePath As String)
        BaseInit(sIniFilePath, GetType(Lexis))
    End Sub
End Class
