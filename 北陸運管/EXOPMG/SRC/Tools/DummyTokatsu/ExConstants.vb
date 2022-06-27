' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/08/08  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>
''' �C���^�t�F�[�X�d�l�Œ�߂�ꂽ�l���i�[����N���X�B
''' </summary>
Public Class ExConstants

    '�����p�}�X�^��ʂɑΉ�����d���T�u���
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
    'TODO: ����A�k���V�����̉w��JRW���������@�����݂��Ȃ����߁A
    'FSK�`SWK�̒�`�͍폜���Ă��邪�A�K�v�ł���ǉ����邱�ƁB
    Public Shared ReadOnly MadoMastersSubObjCodes As New Dictionary(Of String, Byte) From { _
       {"DSH", &H47}, _
       {"LST", &H4D}, _
       {"FJW", &H3E}, _
       {"IJW", &H43}, _
       {"FJC", &H4E}, _
       {"IJC", &H4F}, _
       {"FJR", &H50}, _
       {"IJE", &H56}, _
       {"ICD", &H55}, _
       {"DLY", &H41}, _
       {"ICH", &H44}, _
       {"CYC", &H64}, _
       {"STP", &H63}, _
       {"PNO", &H62}, _
       {"FRC", &H61}, _
       {"DUS", &H66}, _
       {"NSI", &H70}, _
       {"NTO", &H71}, _
       {"NIC", &H72}, _
       {"NJW", &H73}}

    'Suica�G���A�̑�������M�ł���}�X�^
    'TODO: ��M�ł��邩�ۂ��悭�킩���Ă��Ȃ����́iDLY��CYC�Ȃǁj�́A�Ƃ肠����
    '����Ă��邪�A�{���̑����ɍ��킹��ׂ��ł���B���������Ȃǂɂ����āA
    'Suica�G���A�̉��D�@�ɂ�CYC��DLL����Ȃ������iDL�����ʒm���Ԃ��Ă��Ȃ������j
    '�悤�ȋL��������B
    Private Shared ReadOnly MadoMastersInSuicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJR", _
       "IJE", _
       "ICD", _
       "DLY", _
       "CYC", _
       "NSI"}

    '�����w�������̑�������M�ł���}�X�^
    'TODO: �悭������Ȃ��̂ŁA�Ƃ肠����Suica�G���A�Ɠ����ɂ��Ă��邪�A
    '�{���̑����ɍ��킹��ׂ��ł���B
    Private Shared ReadOnly MadoMastersInTokyoKanKanArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJR", _
       "IJE", _
       "ICD", _
       "DLY", _
       "CYC", _
       "NSI"}

    'TOICA�G���A�̑�������M�ł���}�X�^
    'TODO: �Ƃ肠�����u���C���R�z���� �V�����������D�V�X�e�� �V�X�e���d�l���v��
    '���킹�Ă��邪�A������������Ƃ��A���̃G���A�ɂ͓������̃}�X�^��
    '�z�M���Ă�����������Ȃ��B
    'TODO: ��M�ł��邩�ۂ��悭�킩���Ă��Ȃ����́iDLY�Ȃǁj�́A�Ƃ肠����
    '����Ă��邪�A�{���̑����ɍ��킹��ׂ��ł���B
    Private Shared ReadOnly MadoMastersInToicaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJC", _
       "IJC", _
       "ICD", _
       "DLY", _
       "ICH", _
       "CYC", _
       "NTO"}

    'ICOCA�G���A�̑�������M�ł���}�X�^
    'TODO: ��M�ł��邩�ۂ��悭�킩���Ă��Ȃ����́iDLY�Ȃǁj�́A�Ƃ肠����
    '����Ă��邪�A�{���̑����ɍ��킹��ׂ��ł���B
    '������ICOCA�G���A��JR���C�Ǌ�ICOCA�G���A��JR�����{�Ǌ�ICOCA�G���A��
    '������Ă��Ȃ����߁AICOCA�G���A�ł����NIC��NJW��DLL����z��ɂ��Ă���B
    '�����A�{���̑�����JR���C������JR�����{�����ňقȂ铮�������i�����
    '�w���f�[�^����DLL���Ȃ��悤�ɍ�荞�܂�Ă���j�Ȃ�A
    '�V�~�����[�^�����ƎҕʂɎ������Ȃ���΂Ȃ�Ȃ��B
    Private Shared ReadOnly MadoMastersInIcocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJW", _
       "IJW", _
       "ICD", _
       "DLY", _
       "CYC", _
       "NIC", _
       "NJW"}

    'SUGOCA�G���A�̑�������M�ł���}�X�^
    'TODO: �S�����m�̃G���A�Ȃ̂ŁA�Ƃ肠����ICOCA�G���A����JR���C��p�Ǝv����
    'DLY��NIC�𔲂����\���ɂ��Ă���B�����AJR���C�Ǌ�ICOCA�G���A��JR�����{�Ǌ�ICOCA�G���A
    '�̑��������ʉ�����Ă���̂Ɠ��l�ɁASUGOCA�G���A�̑�����ICOCA�G���A�Ƌ��ʉ�
    '����Ă���̂ł���΁ADLY��NIC�����ꂽ�����悢�i�ǂ݂̂�JRW�^�ǂ���z�M�s�\��
    '�Ȃ��Ă��邪�j�B�Ȃ��A���ʉ�����Ă���Ƃ��������́u�}�X�^�f�[�^�d�l��25H�v��
    '�}�X�^��`�ꗗ�ł���iJR�����{�̑�����ICOCA�G���A��SUGOCA�G���A�ɕ������Ă��Ȃ��j�B
    '�������AJR�����{�̑�����JR���C�̐����{�G���A�iICOCA�G���A�H�j�Ƌ�ʂ���Ă�����A
    'DLY���ǂ̃G���A�̂ǂ̒[���ɂ����p�@����DLL����Ȃ����ƂɂȂ��Ă�����A
    '�����������̂��l����������\�ɂȂ��Ă���j�B
    Private Shared ReadOnly MadoMastersInSugocaArea As New HashSet(Of String) From { _
       "DSH", _
       "LST", _
       "FJW", _
       "IJW", _
       "ICD", _
       "CYC", _
       "NJW"}

    Public Shared ReadOnly MadoAreasMasters As New Dictionary(Of Integer, HashSet(Of String)) From { _
       {1, MadoMastersInSuicaArea}, _
       {3, MadoMastersInToicaArea}, _
       {2, MadoMastersInIcocaArea}, _
       {6, MadoMastersInSugocaArea}, _
       {7, MadoMastersInTokyoKanKanArea}}

    Public Const MadoProgramVersionListPathInCab As String = "\Mversion.dat"

End Class
