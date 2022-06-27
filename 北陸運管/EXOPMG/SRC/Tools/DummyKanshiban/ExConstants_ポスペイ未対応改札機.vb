' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2015 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2015/02/16  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>
''' �C���^�t�F�[�X�d�l�Œ�߂�ꂽ�l���i�[����N���X�B
''' </summary>
Public Class ExConstants

    '���D�@�p�}�X�^��ʂɑΉ�����d���T�u���
    'NOTE: ���̎����̓}�X�^��ʂƓd���T�u��ʂ̑Ή��֌W��\�������łȂ��A
    '���p�@���󂯕t����ׂ��}�X�^��ʂ��\���Ă���B
    'TODO: ����A�����ꂩ�̃G���A�̒[����DLL�ł���K�v������}�X�^�́A
    '�S�Ă��̎����ɒ�`���邱�Ƃɂ��Ă��邪�A���Q�Ƃ��āADLL�\�Ȓ[����
    '���֌W�Ȓ��p�@�ɂ����Ă��A���̃}�X�^��ʂ��󂯕t���邱�ƂɂȂ���
    '���܂��B���Ƃ��΁A�u�}�X�^�f�[�^�d�l��25H�v�̃}�X�^��`�ꗗ�ɂ��ƁA
    'Suica�G���A�̒��p�@��IJC���󂯕t���Ȃ��Ɛ�������邽�߁A�����
    '���������Ȃ��i�����̍����͓��Y�Z���̒l���u���v�ł͂Ȃ��u�|�v�ɂȂ���
    '���邱�Ƃł���B�u�|�v�̈Ӗ����L�ڂ���Ă��Ȃ����ߒ肩�ł͂Ȃ����A
    '�����@�ɂ��u���p�@���g���󂯕t���Ȃ��v���Ƃ�\���Ă���̂ł͂Ȃ���
    '�Ɛ��������j�B�����A���̎d�l�����M���ł�����̂ł���A���̐�����
    '�����Ă���Ȃ�A���̎������G���A�ʂɗp�ӂ���ׂ��ł���B
    '�������A���Ƃ��Γ����Ǘ��T�[�o�������̃G���A�̑�����DLL���s����悤��
    '�Ȃ��Ă��邱�Ƃ���킩��悤�ɁA�{���I�ɂЂƂ̒��p�@���ЂƂ̎���
    '�������݂�΂悢�킯�ł͂Ȃ����Ƃɒ��ӂ��K�v�ł���B
    '�����Ō����G���A�́A���p�@�������ނ̃G���A�̂��Ƃł͂Ȃ��A�����܂ł�
    '�[���������ނ̃G���A�̂��Ƃł���B
    Public Shared ReadOnly GateMastersSubObjCodes As New Dictionary(Of String, Byte) From { _
       {"DSH", &H47}, _
       {"LOS", &H48}, _
       {"DSC", &H49}, _
       {"HLD", &H4A}, _
       {"EXP", &H4B}, _
       {"FRX", &H4C}, _
       {"LST", &H4D}, _
       {"FJW", &H3E}, _
       {"IJW", &H43}, _
       {"FJC", &H4E}, _
       {"IJC", &H4F}, _
       {"FJR", &H50}, _
       {"IJE", &H56}, _
       {"KEN", &H59}, _
       {"DLY", &H41}, _
       {"ICH", &H44}, _
       {"PAY", &H42}, _
       {"CYC", &H64}, _
       {"STP", &H63}, _
       {"PNO", &H62}, _
       {"FRC", &H61}, _
       {"DUS", &H66}, _
       {"NSI", &H70}, _
       {"NTO", &H71}, _
       {"NIC", &H72}, _
       {"NJW", &H73}, _
       {"IUK", &H86}, _
       {"IUZ", &H84}, _
       {"KSZ", &H85}, _
       {"SWK", &H87}, _
       {"FSK", &H80}, _
       {"HIR", &H8A}, _
       {"PPA", &H89}}

    '���C��Suica�G���A�̉��D�@����M�ł���}�X�^
    'TODO: ��M�ł��邩�ۂ��悭�킩���Ă��Ȃ����́iDLY, KEN, CYC�Ȃǁj�́A�Ƃ肠����
    '����Ă��邪�A�{���̉��D�@�V�X�e���ɍ��킹��ׂ��ł���B�K�p�ςݑΉ��O�̗��������Ȃǂɂ����āA
    'Suica�G���A�̉��D�@�ɂ�CYC��DLL����Ȃ������iDL�����ʒm���Ԃ��Ă��Ȃ������j�悤�ȋL��������B
    Private Shared ReadOnly GateReadyGateMastersInTokaidoSuicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJR", _
       "IJE", _
       "KEN", _
       "DLY", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NSI"}

    '���C��TOICA�G���A�̉��D�@����M�ł���}�X�^
    'TODO: �Ƃ肠�����u���C���R�z���� �V�����������D�V�X�e�� �V�X�e���d�l���v��
    '���킹�Ă��邪�A������������Ƃ��A���̃G���A�ɂ͓������̃}�X�^��
    '�z�M���Ă�����������Ȃ��B
    'TODO: ��M�ł��邩�ۂ��悭�킩���Ă��Ȃ����́iDLY, KEN�Ȃǁj�́A�Ƃ肠����
    '����Ă��邪�A�{���̉��D�@�V�X�e���ɍ��킹��ׂ��ł���B
    Private Shared ReadOnly GateReadyGateMastersInTokaidoToicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJC", _
       "IJC", _
       "KEN", _
       "DLY", _
       "ICH", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NTO"}

    '���C��ICOCA�G���A�̉��D�@����M�ł���}�X�^
    'TODO: ��M�ł��邩�ۂ��悭�킩���Ă��Ȃ����́iDLY�Ȃǁj�́A�Ƃ肠����
    '����Ă��邪�A�{���̉��D�@�V�X�e���ɍ��킹��ׂ��ł���B
    Private Shared ReadOnly GateReadyGateMastersInTokaidoIcocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "DLY", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NIC"}

    '�R�zICOCA�G���A�̉��D�@����M�ł���}�X�^
    'TODO: ��M�ł��邩�ۂ��悭�킩���Ă��Ȃ����́iPAY�Ȃǁj�́A�Ƃ肠����
    '����Ă��邪�A�{���̉��D�@�V�X�e���ɍ��킹��ׂ��ł���B
    Private Shared ReadOnly GateReadyGateMastersInSanyoIcocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW"}

    '�R�zSUGOCA�G���A�̉��D�@����M�ł���}�X�^
    'TODO: �悭������Ȃ����A�Ƃ肠�����R�zICOCA�G���A�Ɠ����ɂ��Ă���B
    '��p�̃}�X�^��ʂ�����Ȃ�AGateMastersSubObjCodes�ƂƂ��ɏC�����Ȃ���΂Ȃ�Ȃ��B
    Private Shared ReadOnly GateReadyGateMastersInSanyoSugocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW"}

    '�����w�������̉��D�@����M�ł���}�X�^
    'TODO: �悭������Ȃ��B�Ƃ肠�������C��Suica�G���A�Ɠ����ɂ��Ă��邪�A
    '�{���̉��D�@�V�X�e���ɍ��킹��ׂ��ł���B
    Private Shared ReadOnly GateReadyGateMastersInTokyoKanKanArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJR", _
       "IJE", _
       "KEN", _
       "DLY", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NSI"}

    '�k���G���A�̉��D�@����M�ł���}�X�^
    'TODO: �Ƃ肠�����R�zICOCA�G���A�̎��+�k����p���-�|�C���g�|�X�y�֘A��ʂɂ��Ă��邪�A
    '�{���̉��D�@�V�X�e���ɍ��킹��ׂ��ł���B
    Private Shared ReadOnly GateReadyGateMastersInHokurikuArea As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW", _
       "IUK", _
       "IUZ", _
       "KSZ", _
       "SWK", _
       "FSK"}

    '���C���R�z���拤�ʂ̊Ď��Ղ��󂯕t������D�@�}�X�^
    'NOTE: GateMastersSubObjCodes �ɑ��݂��Ă��āA�����ɑ��݂��Ȃ�
    '���D�@�}�X�^��DLL�V�[�P���X�ɂ��āA���C���R�z���拤�ʂ̊Ď��Ղ́A
    'NAK��ԐM�����肹���ɍŌ�܂Ŏ��s���邪�A�t�@�C���]���I�����
    '���M����REQ�d���̊J�n�E�I���R�[�h��0x03�iFinishWithoutStoring�j�Ƃ���B
    'TODO: �Ƃ肠�����A�k����p�ȊO�̑S�}�X�^�����Ă��邪�A�悭�킩��Ȃ��B
    '�k����p�̃}�X�^�ɂ��ẮA�^�Ǒ��œ��C���R�z���拤�ʂ̊Ď��Ղ��A������
    '�����m��Ȃ��̂ł���΁ANAK��ԐM����\��������B
    '�t�ɁA�Ď��Ղł̓m�[�K�[�h�ł���ADLL�V�[�P���X�̊�����A
    '���D�@���Ď��Ղ����M���Ȃ����ƂŁA�Ď��Ղ��u�K�p�ς݁v��DL�����ʒm��
    '�쐬����\��������B
    Private Shared ReadOnly KsbReadyGateMastersInTokaidoSanyo As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "FJC", _
       "IJC", _
       "FJR", _
       "IJE", _
       "KEN", _
       "DLY", _
       "ICH", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NSI", _
       "NTO", _
       "NIC", _
       "NJW"}

    '�k�������̊Ď��Ղ��󂯕t������D�@�}�X�^
    'NOTE: GateMastersSubObjCodes �ɑ��݂��Ă��āA�����ɑ��݂��Ȃ�
    '���D�@�}�X�^��DLL�V�[�P���X�ɂ��āA�k�������̊Ď��Ղ́A
    'NAK��ԐM�����肹���ɍŌ�܂Ŏ��s���邪�A�t�@�C���]���I�����
    '���M����REQ�d���̊J�n�E�I���R�[�h��0x03�iFinishWithoutStoring�j�Ƃ���B
    'TODO: �Ƃ肠�����A�k���G���A�̉��D�@����M����}�X�^�Ɓi�ڑ���������
    '�Ȃ����󂯕t����悤�ɂȂ��Ă����j�|�C���g�|�X�g�y�C�p�}�X�^��
    '����Ă��邪�A���̂܂܌��n�����[�X����̂��͂悭�킩��Ȃ��B
    Private Shared ReadOnly KsbReadyGateMastersInHokuriku As New HashSet(Of String) From { _
       "DSH", _
       "LOS", _
       "DSC", _
       "HLD", _
       "EXP", _
       "FRX", _
       "LST", _
       "FJW", _
       "IJW", _
       "KEN", _
       "PAY", _
       "CYC", _
       "STP", _
       "PNO", _
       "FRC", _
       "DUS", _
       "NJW", _
       "IUK", _
       "IUZ", _
       "KSZ", _
       "SWK", _
       "FSK"}

    Public Shared ReadOnly GateAreasSpecs As New Dictionary(Of Integer, ExAreaSpec) From { _
       {1, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokaidoSuicaArea)}, _
       {3, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokaidoToicaArea)}, _
       {2, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokaidoIcocaArea)}, _
       {4, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInSanyoIcocaArea)}, _
       {6, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInSanyoSugocaArea)}, _
       {7, New ExAreaSpec(KsbReadyGateMastersInTokaidoSanyo, GateReadyGateMastersInTokyoKanKanArea)}, _
       {8, New ExAreaSpec(KsbReadyGateMastersInHokuriku, GateReadyGateMastersInHokurikuArea)}}

    Public Const GateProgramVersionListPathInCab As String = "\KANSI\N_GATE\JPROWRK\Gversion.dat"
    Public Const GateProgramModuleBasePathInCab As String = "\KANSI\N_GATE"
    Public Const GateProgramModuleCatalogFileNameInCab As String = "FILELIST.TXT"
    Public Shared ReadOnly GateProgramModuleNamesInCab As String() = {"JHANWRK", "JPROWRK", "JSCPUWRK", "JOSWRK", "JICUWRK"}
    Public Shared ReadOnly GateProgramModuleNamesInVersionInfo As String() = {"JHANNOW", "JPRONOW", "JSCPUNOW", "JOSNOW", "JICUNOW"}

    Public Const KsbProgramVersionListPathInCab As String = "\KANSI_PROG\WRK\Kversion.dat"

End Class

Public Class ExAreaSpec

    Public KsbReadyGateMasters As HashSet(Of String)
    Public GateReadyGateMasters As HashSet(Of String)
    Public Sub New(ByVal oKsbReadyGateMasters As HashSet(Of String), ByVal oGateReadyGateMasters As HashSet(Of String))
        KsbReadyGateMasters = oKsbReadyGateMasters
        GateReadyGateMasters = oGateReadyGateMasters
    End Sub

End Class
