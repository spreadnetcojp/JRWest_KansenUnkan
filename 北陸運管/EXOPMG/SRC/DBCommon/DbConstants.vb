' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/25  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

''' <summary>
''' DB�d�l�̒萔���`����N���X�B
''' </summary>
Public Class DbConstants

    'NOTE: �f�[�^�x�[�X�d�l�ɂȂ��Ă���萔�ł���Ȃ���A���̎��A
    '�w���@��Ƃ�I/F�d�l�Ō��܂��Ă���萔�i�܂���EkCommon�Ŏg�p����萔�j
    '�ɂ��ẮA�����ł͂Ȃ��AEkConstants�Œ�`����B
    '�w���@��Ƃ�I/F�d�l�́A�^�ǃV�X�e���̓����d�l�ł���f�[�^�x�[�X�d�l
    '������Ɍ��߂��Ă���i����{�I�ȁj�d�l�ł���B
    '�O�҂�S������EkCommon���W���[���́A��҂�S������DBCommon���W���[��
    '������ʂ̃��W���[���ł���A�O�҂���҂Ɉˑ����Ă͂Ȃ�Ȃ��B

    '�ʐM��ԊǗ��e�[�u���ɃZ�b�g����|�[�g�敪
    Public Const PortPurposeGeneralData As String = "1" '�ʏ�f�[�^�p
    Public Const PortPurposeRiyoData As String = "2"    '���p�f�[�^�p

    'DLL��ԃe�[�u���̔z�M��Ԓl
    'NOTE: �S�ĉ^�ǃV�X�e�����̎d�l�ł���B
    Public Const DllStatusNormal As Integer = &H0
    Public Const DllStatusAbnormal As Integer = &H1
    Public Const DllStatusContentError As Integer = &H2
    Public Const DllStatusTimeout As Integer = &H3
    Public Const DllStatusExecuting As Integer = &HFFFF

    'DL��ԃe�[�u���̔z�M��Ԓl
    '-------Ver0.1�@�t�F�[�Y�Q�@�u�K�p�ς݁v��Ԃ�ǉ��@MOD START-----------
    'NOTE: ���L�Œ�`�����l�̂݉^�ǃV�X�e�������ɓ��荞��
    '�i���邢�͉^�ǃV�X�e�����Ŋ��������j�d�l�ł���B
    '���L�ȊO�͉w���@��̎d�l����ŗe�Ղɒǉ������\��������A
    '�^�ǃV�X�e���̏����ɉe������l�ł��Ȃ����߁A
    '�w���@�킩��󂯎�����l�����̂܂�DB�ɓo�^����B
    '�w���@�킪��`��ǉ������ۂ́ADL��Ԗ��̃e�[�u����
    '���̒l�ƕ\��������ǉ����邾���ł悢�B
    Public Const DlStatusNormal As Integer = &H0
    Public Const DlStatusContinuingNormal As Integer = &HF
    Public Const DlStatusPreExecuting As Integer = &HFFFE
    Public Const DlStatusExecuting As Integer = &HFFFF
    '-------Ver0.1�@�t�F�[�Y�Q�@�u�K�p�ς݁v��Ԃ�ǉ��@MOD END-----------

    '���W�f�[�^��L�e�[�u���̃��R�[�h���
    Public Const CdtKindAll As String = "�S�f�[�^���"
    Public Const CdtKindBesshuData As String = "�ʏW�D�f�[�^"
    Public Const CdtKindFuseiJoshaData As String = "�s����Ԍ��o�f�[�^"
    Public Const CdtKindKyokoToppaData As String = "���s�˔j���o�f�[�^"
    Public Const CdtKindFunshitsuData As String = "���������o�f�[�^"
    Public Const CdtKindFrexData As String = "FREX�����ID���o�f�[�^"
    Public Const CdtKindFaultData As String = "�ُ�f�[�^�i�Ď��Ձ^���D�@�^���������@�j"
    Public Const CdtKindKadoData As String = "�ғ��E�ێ�f�[�^�i���D�@�A���������@�j"
    Public Const CdtKindTrafficData As String = "���ԑѕʏ�~�f�[�^"
    Public Const CdtKindKsbConfig As String = "�Ď��Րݒ���"
    Public Const CdtKindConStatus As String = "�@��ڑ����"
    Public Const CdtKindServerError As String = "�T�[�o���ُ�"

    '�f�[�^��ʂɑΉ�������W�f�[�^��L�e�[�u���̃��R�[�h���
    Public Shared ReadOnly CdtKindsOfDataKinds As New Dictionary(Of String, String()) From { _
       {"BSY", New String() {CdtKindBesshuData}}, _
       {"MEI", New String() {CdtKindFuseiJoshaData, CdtKindKyokoToppaData, CdtKindFunshitsuData, CdtKindFrexData}}, _
       {"ERR", New String() {CdtKindFaultData}}, _
       {"KDO", New String() {CdtKindKadoData}}, _
       {"TIM", New String() {CdtKindTrafficData}}}

    'SNMP�ʒm�̏d��x
    Public Const SnmpSeverityWarning As String = "WARNING"   '���ӈ�
    Public Const SnmpSeverityCritical As String = "CRITICAL" '�댯��

End Class
