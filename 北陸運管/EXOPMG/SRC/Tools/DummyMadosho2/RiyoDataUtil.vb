' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/06/27  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class RiyoDataUtil

    Private Class FieldRef
        Public Field As XlsField
        Public BitOffset As Integer
        Public Index As Integer

        Public Sub New(ByVal oField As XlsField, ByVal bitOfs As Integer, ByVal i As Integer)
            Field = oField
            BitOffset = bitOfs
            Index = i
        End Sub
    End Class

    Private Shared oFieldRefs As Dictionary(Of String, FieldRef)
    Private Shared totalBitCount As Integer

    Private Shared ReadOnly oFields As XlsField() = New XlsField() { _
        New XlsField(8*1, "X2", 1, " "c, "��{�w�b�_�[ �f�[�^���"), _
        New XlsField(8*7, "X14", 1, " "c, "��{�w�b�_�[ ��������"), _
        New XlsField(8*1, "D", 1, " "c, "��{�w�b�_�[ �R�[�i�["), _
        New XlsField(8*1, "D", 1, " "c, "��{�w�b�_�[ ���@"), _
        New XlsField(8*4, "D", 1, " "c, "��{�w�b�_�[ �V�[�P���XNo", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*1, "X2", 1, " "c, "��{�w�b�_�[ �o�[�W����"), _
        New XlsField(8*1, "D3", 2, "-"c, "��{�w�b�_�[ �w�R�[�h", "Station"), _
        New XlsField(8*1, "X2", 1, " "c, "�ʉߕ���", "PassDirection"), _
        New XlsField(8*1, "X2", 1, " "c, "���b�`�`��", "LatchConf"), _
        New XlsField(8*2, "X4", 1, " "c, "���茋��"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� ��Ԍ� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� ��Ԍ� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� ���}�� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� ���}�� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� �̂��݋�� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� �̂��݋�� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� �O���[����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� �O���[����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� IC��� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� IC��� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� �t���[���"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� FREX��� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "������� FREX��� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "����w��� ��Ԍ� ����w", "Station"), _
        New XlsField(8*1, "D", 1, " "c, "����w��� ��Ԍ� �R�[�i�["), _
        New XlsField(8*1, "D", 1, " "c, "����w��� ��Ԍ� ���@"), _
        New XlsField(8*1, "D3", 2, "-"c, "����w��� ���}�� ����w", "Station"), _
        New XlsField(8*4, "X8", 1, " "c, "���������� ��Ԍ� ��������"), _
        New XlsField(8*4, "X8", 1, " "c, "���������� ���}�� ��������"), _
        New XlsField(8*1, "D3", 2, "-"c, "���w������� ��Ԍ� ��ԉw", "Station"), _
        New XlsField(8*1, "D", 1, " "c, "���w������� ��Ԍ� �R�[�i�["), _
        New XlsField(8*1, "D", 1, " "c, "���w������� ��Ԍ� ���@"), _
        New XlsField(8*1, "D3", 2, "-"c, "���w������� ���}�� ��ԉw", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���w���猔��� ��Ԍ� ���w", "Station"), _
        New XlsField(8*1, "X2", 1, " "c, "�召�敪 ��l����", "AdultChild"), _
        New XlsField(8*1, "X2", 1, " "c, "���ʋ敪 �j������", "MaleFemale"), _
        New XlsField(8*1, "X2", 1, " "c, "IC���p �V����IC���p", "IcUseUnuse"), _
        New XlsField(8*1, "X2", 1, " "c, "IC���p �܂Ō�IC���p", "IcUseUnuse"), _
        New XlsField(8*1, "X2", 1, " "c, "IC���p ���猔IC���p", "IcUseUnuse"), _
        New XlsField(8*1, "D3", 2, "-"c, "�w�茔��� �w��P �w���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "�w�茔��� �w��P �w���� ���w", "Station"), _
        New XlsField(8*3, "D5", 1, " "c, "�w�茔��� �w��P ��Ԕԍ�"), _
        New XlsField(1*1, "D", 1, " "c, "�w�茔��� �w��P ���� G�r�b�g"), _
        New XlsField(1*1, "D", 1, " "c, "�w�茔��� �w��P ���� �����r�b�g"), _
        New XlsField(1*6, "D", 1, " "c, "�w�茔��� �w��P ���Ԕԍ�"), _
        New XlsField(8*1, "X", 1, " "c, "�w�茔��� �w��P ���Ȕԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "�w�茔��� �w��P ���Ȏ��", "SeatKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "�w�茔��� �w��Q �w���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "�w�茔��� �w��Q �w���� ���w", "Station"), _
        New XlsField(8*3, "D5", 1, " "c, "�w�茔��� �w��Q ��Ԕԍ�"), _
        New XlsField(1*1, "D", 1, " "c, "�w�茔��� �w��Q ���� G�r�b�g"), _
        New XlsField(1*1, "D", 1, " "c, "�w�茔��� �w��Q ���� �����r�b�g"), _
        New XlsField(1*6, "D", 1, " "c, "�w�茔��� �w��Q ���Ԕԍ�"), _
        New XlsField(8*1, "X", 1, " "c, "�w�茔��� �w��Q ���Ȕԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "�w�茔��� �w��Q ���Ȏ��", "SeatKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "�w�茔��� �w��R �w���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "�w�茔��� �w��R �w���� ���w", "Station"), _
        New XlsField(8*3, "D5", 1, " "c, "�w�茔��� �w��R ��Ԕԍ�"), _
        New XlsField(1*1, "D", 1, " "c, "�w�茔��� �w��R ���� G�r�b�g"), _
        New XlsField(1*1, "D", 1, " "c, "�w�茔��� �w��R ���� �����r�b�g"), _
        New XlsField(1*6, "D", 1, " "c, "�w�茔��� �w��R ���Ԕԍ�"), _
        New XlsField(8*1, "X", 1, " "c, "�w�茔��� �w��R ���Ȕԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "�w�茔��� �w��R ���Ȏ��", "SeatKind"), _
        New XlsField(8*1, "X2", 1, " "c, "�s������Ώۋ敪�r�b�g"), _
        New XlsField(8*1, "X2", 1, " "c, "�s������m�f����"), _
        New XlsField(8*1, "D", 1, " "c, "��������"), _
        New XlsField(8*1, "X2", 1, " "c, "���p�p�^�[�����"), _
        New XlsField(8*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� �W�v����", "TicketKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �P���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �P���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �P���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �P���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �P���ڏ�� �t���[���"), _
        New XlsField(8*2, "D", 1, " "c, "���ǎ��� �P���ڏ�� �搔"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �P���ڏ�� ���o����"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �P���ڏ�� �召�r�b�g", "AdultChildFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �P���ڏ�� �j���r�b�g", "MaleFemaleFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �P���ڏ�� �ʋΒʊw�r�b�g", "CommutingFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� ���Z�����r�b�g", "CombinedDiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� �����r�b�g", "DiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� �Ĕ��s�r�b�g", "ReissueFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� �e�X�g�r�b�g", "TestFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� �^���r�b�g", "FreightRateAmendFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� �A���r�b�g", "ConnectionFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� �A���r�b�g", "ContinuumFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� ���w�L�����r�b�g", "TicketValidityFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� ������o�r�b�g", "WithdrawFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� ���p�r�b�g", "CombineFlag"), _
        New XlsField(8*1, "D3", 1, " "c, "���ǎ��� �P���ڏ�� ����", "DiscountKind"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �P���ڏ�� EXIC����"), _
        New XlsField(8*3, "X6", 1, " "c, "���ǎ��� �P���ڏ�� ���i�ԍ�"), _
        New XlsField(8*1, "X2", 2, " "c, "���ǎ��� �P���ڏ�� ���s���"), _
        New XlsField(8*4, "X8", 1, " "c, "���ǎ��� �P���ڏ�� �L���J�n��"), _
        New XlsField(8*2, "X4", 1, " "c, "���ǎ��� �P���ڏ�� ���s����"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� ���� G�r�b�g"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �P���ڏ�� ���� �����r�b�g"), _
        New XlsField(1*6, "D", 1, " "c, "���ǎ��� �P���ڏ�� ���Ԕԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �P���ڏ�� �������敪"), _
        New XlsField(8*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �W�v����", "TicketKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �Q���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �Q���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �Q���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �Q���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �Q���ڏ�� �t���[���"), _
        New XlsField(8*2, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �搔"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �Q���ڏ�� ���o����"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �召�r�b�g", "AdultChildFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �j���r�b�g", "MaleFemaleFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �ʋΒʊw�r�b�g", "CommutingFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� ���Z�����r�b�g", "CombinedDiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �����r�b�g", "DiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �Ĕ��s�r�b�g", "ReissueFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �e�X�g�r�b�g", "TestFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �^���r�b�g", "FreightRateAmendFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �A���r�b�g", "ConnectionFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� �A���r�b�g", "ContinuumFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� ���w�L�����r�b�g", "TicketValidityFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� ������o�r�b�g", "WithdrawFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� ���p�r�b�g", "CombineFlag"), _
        New XlsField(8*1, "D3", 1, " "c, "���ǎ��� �Q���ڏ�� ����", "DiscountKind"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �Q���ڏ�� EXIC����"), _
        New XlsField(8*3, "X6", 1, " "c, "���ǎ��� �Q���ڏ�� ���i�ԍ�"), _
        New XlsField(8*1, "X2", 2, " "c, "���ǎ��� �Q���ڏ�� ���s���"), _
        New XlsField(8*4, "X8", 1, " "c, "���ǎ��� �Q���ڏ�� �L���J�n��"), _
        New XlsField(8*2, "X4", 1, " "c, "���ǎ��� �Q���ڏ�� ���s����"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� ���� G�r�b�g"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �Q���ڏ�� ���� �����r�b�g"), _
        New XlsField(1*6, "D", 1, " "c, "���ǎ��� �Q���ڏ�� ���Ԕԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �Q���ڏ�� �������敪"), _
        New XlsField(8*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� �W�v����", "TicketKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �R���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �R���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �R���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �R���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �R���ڏ�� �t���[���"), _
        New XlsField(8*2, "D", 1, " "c, "���ǎ��� �R���ڏ�� �搔"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �R���ڏ�� ���o����"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �R���ڏ�� �召�r�b�g", "AdultChildFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �R���ڏ�� �j���r�b�g", "MaleFemaleFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �R���ڏ�� �ʋΒʊw�r�b�g", "CommutingFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� ���Z�����r�b�g", "CombinedDiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� �����r�b�g", "DiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� �Ĕ��s�r�b�g", "ReissueFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� �e�X�g�r�b�g", "TestFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� �^���r�b�g", "FreightRateAmendFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� �A���r�b�g", "ConnectionFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� �A���r�b�g", "ContinuumFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� ���w�L�����r�b�g", "TicketValidityFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� ������o�r�b�g", "WithdrawFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� ���p�r�b�g", "CombineFlag"), _
        New XlsField(8*1, "D3", 1, " "c, "���ǎ��� �R���ڏ�� ����", "DiscountKind"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �R���ڏ�� EXIC����"), _
        New XlsField(8*3, "X6", 1, " "c, "���ǎ��� �R���ڏ�� ���i�ԍ�"), _
        New XlsField(8*1, "X2", 2, " "c, "���ǎ��� �R���ڏ�� ���s���"), _
        New XlsField(8*4, "X8", 1, " "c, "���ǎ��� �R���ڏ�� �L���J�n��"), _
        New XlsField(8*2, "X4", 1, " "c, "���ǎ��� �R���ڏ�� ���s����"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� ���� G�r�b�g"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �R���ڏ�� ���� �����r�b�g"), _
        New XlsField(1*6, "D", 1, " "c, "���ǎ��� �R���ڏ�� ���Ԕԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �R���ڏ�� �������敪"), _
        New XlsField(8*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� �W�v����", "TicketKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �S���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �S���ڏ�� ��Ԍ���� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �S���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �S���ڏ�� ���}����� ���w", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "���ǎ��� �S���ڏ�� �t���[���"), _
        New XlsField(8*2, "D", 1, " "c, "���ǎ��� �S���ڏ�� �搔"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �S���ڏ�� ���o����"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �S���ڏ�� �召�r�b�g", "AdultChildFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �S���ڏ�� �j���r�b�g", "MaleFemaleFlag"), _
        New XlsField(1*2, "D", 1, " "c, "���ǎ��� �S���ڏ�� �ʋΒʊw�r�b�g", "CommutingFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� ���Z�����r�b�g", "CombinedDiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� �����r�b�g", "DiscountFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� �Ĕ��s�r�b�g", "ReissueFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� �e�X�g�r�b�g", "TestFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� �^���r�b�g", "FreightRateAmendFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� �A���r�b�g", "ConnectionFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� �A���r�b�g", "ContinuumFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� ���w�L�����r�b�g", "TicketValidityFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� ������o�r�b�g", "WithdrawFlag"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� ���p�r�b�g", "CombineFlag"), _
        New XlsField(8*1, "D3", 1, " "c, "���ǎ��� �S���ڏ�� ����", "DiscountKind"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �S���ڏ�� EXIC����"), _
        New XlsField(8*3, "X6", 1, " "c, "���ǎ��� �S���ڏ�� ���i�ԍ�"), _
        New XlsField(8*1, "X2", 2, " "c, "���ǎ��� �S���ڏ�� ���s���"), _
        New XlsField(8*4, "X8", 1, " "c, "���ǎ��� �S���ڏ�� �L���J�n��"), _
        New XlsField(8*2, "X4", 1, " "c, "���ǎ��� �S���ڏ�� ���s����"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� ���� G�r�b�g"), _
        New XlsField(1*1, "D", 1, " "c, "���ǎ��� �S���ڏ�� ���� �����r�b�g"), _
        New XlsField(1*6, "D", 1, " "c, "���ǎ��� �S���ڏ�� ���Ԕԍ�"), _
        New XlsField(8*1, "X2", 1, " "c, "���ǎ��� �S���ڏ�� �������敪"), _
        New XlsField(1*4, "X1", 1, " "c, "�h�c�ԍ� �O�Œ�"), _
        New XlsField(1*4, "X1", 1, " "c, "�h�c�ԍ� �Ĕ��s"), _
        New XlsField(1*4, "X1", 1, " "c, "�h�c�ԍ� ��Ђ܂��͌���R�[�h"), _
        New XlsField(1*28, "X7", 1, " "c, "�h�c�ԍ� �h�c�R�[�h"), _
        New XlsField(8*4, "X8", 1, " "c, "�r�e��������z"), _
        New XlsField(8*1, "D3", 2, "-"c, "�r�e���p��ԂP ���p�w�P", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "�r�e���p��ԂP ���p�w�Q", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "�r�e���p��ԂQ ���p�w�P", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "�r�e���p��ԂQ ���p�w�Q", "Station"), _
        New XlsField(8*1, "D3", 2, "-"c, "��Ԏn�_�w", "Station"), _
        New XlsField(8*1, "X2", 1, " "c, "���ʂ��}�X�^�K�p�L��", "AbsencePresence"), _
        New XlsField(8*1, "X2", 6, " "c, "�\��"), _
        New XlsField(8*2, "X4", 1, " "c, "�T���l", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�P"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�P�Y����"), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�Q"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�Q�Y����"), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�R"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�R�Y����"), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�S"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�S�Y����"), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�T"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�T�Y����"), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�U"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�U�Y����"), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�V"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�V�Y����"), _
        New XlsField(8*2, "X4", 1, " "c, "����m�f�R�[�h�W"), _
        New XlsField(8*1, "X2", 4, " "c, "����m�f�R�[�h�W�Y����"), _
        New XlsField(8*1, "X2", 288, " "c, "���G���R�[�h��� �P���ڏ��"), _
        New XlsField(8*1, "X2", 288, " "c, "���G���R�[�h��� �Q���ڏ��"), _
        New XlsField(8*1, "X2", 288, " "c, "���G���R�[�h��� �R���ڏ��"), _
        New XlsField(8*1, "X2", 288, " "c, "���G���R�[�h��� �S���ڏ��")}

    Shared Sub New()
        oFieldRefs = New Dictionary(Of String, FieldRef)
        Dim bits As Integer = 0
        For i As Integer = 0 To oFields.Length - 1
            Dim oField As XlsField = oFields(i)
            oFieldRefs.Add(oField.MetaName, New FieldRef(oField, bits, i))
            bits += oField.ElementBits * oField.ElementCount
        Next i
        totalBitCount = bits
    End Sub

    Public Shared ReadOnly Property RecordLengthInBits As Integer
        Get
            Return totalBitCount
        End Get
    End Property

    Public Shared ReadOnly Property RecordLengthInBytes As Integer
        Get
            Return (totalBitCount + 7) \ 8
        End Get
    End Property

    Public Shared ReadOnly Property Fields As XlsField()
        Get
            Return oFields
        End Get
    End Property

    Public Shared ReadOnly Property Field(ByVal sMetaName As String) As XlsField
        Get
            Return oFieldRefs(sMetaName).Field
        End Get
    End Property

    Public Shared Function FieldIndexOf(ByVal sMetaName As String) As Integer
        Return oFieldRefs(sMetaName).Index
    End Function

    Public Shared Function GetFieldValueFromBytes(ByVal sMetaName As String, ByVal oBytes As Byte()) As String
        Dim oRef As FieldRef = oFieldRefs(sMetaName)
        Return oRef.Field.CreateValueFromBytes(oBytes, oRef.BitOffset)
    End Function

    Public Shared Sub SetFieldValueToBytes(ByVal sMetaName As String, ByVal sValue As String, ByVal oBytes As Byte())
        Dim oRef As FieldRef = oFieldRefs(sMetaName)
        oRef.Field.CopyValueToBytes(sValue, oBytes, oRef.BitOffset)
    End Sub

End Class
