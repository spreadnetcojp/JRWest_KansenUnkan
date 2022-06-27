' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2017/11/21  (NES)����  �V�K�쐬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class KadoDataUtil

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

    Private Const AggregateFieldsOrigin As Integer = 15
    Private Shared ReadOnly oFields As XlsField() = New XlsField() { _
        New XlsField(8*1, "X2", 1, " "c, "��{�w�b�_�[ �f�[�^���", "DataKind"), _
        New XlsField(8*1, "D3", 2, "-"c, "��{�w�b�_�[ �w�R�[�h", "Station"), _
        New XlsField(8*7, "X14", 1, " "c, "��{�w�b�_�[ ��������"), _
        New XlsField(8*1, "D", 1, " "c, "��{�w�b�_�[ �R�[�i�["), _
        New XlsField(8*1, "D", 1, " "c, "��{�w�b�_�[ ���@"), _
        New XlsField(8*4, "D", 1, " "c, "��{�w�b�_�[ �V�[�P���XNo", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*1, "X2", 1, " "c, "��{�w�b�_�[ �o�[�W����"), _
        New XlsField(8*7, "X14", 1, " "c, "���ʕ� �W�v�J�n����"), _
        New XlsField(8*7, "X14", 1, " "c, "���ʕ� �W�v�I��(���W)����"), _
        New XlsField(8*7, "X14", 1, " "c, "���ʕ� ���D���������_������"), _
        New XlsField(8*7, "X14", 1, " "c, "���ʕ� �W�D���������_������"), _
        New XlsField(8*8, "X10", 1, " "c, "���ʕ� ���D���������ԍ�"), _
        New XlsField(8*8, "X10", 1, " "c, "���ʕ� �W�D���������ԍ�"), _
        New XlsField(8*1, "D", 48, " "c, "���ʕ� ���D�����m�Z���T���x��"), _
        New XlsField(8*1, "D", 48, " "c, "���ʕ� �W�D�����m�Z���T���x��"), _
        New XlsField(8*1, "X2", 48, " "c, "���ʕ� �\��"), _
        New XlsField(8*4, "D", 1, " "c, "�W�v001 ����(APL)/��EXIC��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v002 ����(APL)/���ݗ�IC��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v003 ����(APL)/�����C����������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v004 ����(APL)/EXIC�Ɩ�EXIC��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v005 ����(APL)/EXIC���Ɖ�Ɩ�EXIC��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v006 ����(APL)/�ݗ�IC�Ɩ��ݗ�IC��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v007 ����(APL)/���C�Ɩ����C����������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v008 ����(APL)/�⏕ �ݗ�IC/���IC�Ɩ���������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v009 ����(APL)/�⏕ EXIC�Ɩ���������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v010 ����(APL)/�W���F�� �W���F�؏�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v011 ����(APL)/�Ɩ��F�� �Ɩ��F�؏�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v012 ����(APL)/��Q �x���ݒ菈������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v013 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v014 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v015 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v016 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v017 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v018 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v019 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v020 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v021 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v022 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v023 ����(APL)/EXIC �V�������ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v024 ����(APL)/EXIC �V�����o�ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v025 ����(APL)/EXIC �����o�ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v026 ����(APL)/EXIC ��������������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v027 ����(APL)/EXIC �x���o�ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v028 ����(APL)/EXIC �g�p��~����(����)����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v029 ����(APL)/EXIC �V�������ꔻ�菈������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v030 ����(APL)/EXIC �V�����o�ꔻ�菈������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v031 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v032 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v033 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v034 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v035 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v036 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v037 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v038 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v039 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v040 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v041 ����(APL)/EXIC���Ɖ� ���a������E���p�����Ɖ��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v042 ����(APL)/EXIC���Ɖ� ���/�����߂���������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v043 ����(APL)/EXIC���Ɖ� ����������������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v044 ����(APL)/EXIC���Ɖ� �\��ύX��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v045 ����(APL)/EXIC���Ɖ� �ē��\�ďo�͏�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v046 ����(APL)/EXIC���Ɖ� �g�p��~����(����)����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v047 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v048 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v049 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v050 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v051 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v052 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v053 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v054 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v055 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v056 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v057 ����(APL)/�ݗ�IC ���ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v058 ����(APL)/�ݗ�IC �o�ꥐ��Z��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v059 ����(APL)/�ݗ�IC ���z������������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v060 ����(APL)/�ݗ�IC ���w�o�ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v061 ����(APL)/�ݗ�IC �����o��/���w��ݾُ�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v062 ����(APL)/�ݗ�IC ���p�����󎚏�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v063 ����(APL)/�ݗ�IC �ݒ�ύX��������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v064 ����(APL)/�ݗ�IC �Ҍ����޽�󎚏�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v065 ����(APL)/�ݗ�IC �V���������o�ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v066 ����(APL)/�ݗ�IC �V�������ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v067 ����(APL)/�ݗ�IC �V�������w�o�ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v068 ����(APL)/�ݗ�IC �g�p��~����(����)����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v069 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v070 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v071 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v072 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v073 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v074 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v075 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v076 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v077 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v078 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v079 ����(APL)/���C�� �������ꏈ���i���ډ��D�j����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v080 ����(APL)/���C�� �������ꏈ���i�抷���D�j����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v081 ����(APL)/���C�� �ݗ������ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v082 ����(APL)/���C�� �����o�ꏈ���i���ډ��D�j����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v083 ����(APL)/���C�� �����o�ꏈ���i�抷���D�j����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v084 ����(APL)/���C�� �ݗ����o�ꏈ������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v085 ����(APL)/���C�� �x���o�ꏈ���i���ډ��D�j����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v086 ����(APL)/���C�� �x���o�ꏈ���i�抷���D�j����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v087 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v088 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v089 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v090 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v091 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v092 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v093 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v094 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v095 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v096 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v097 ����(APL)/�⏕ �ݗ�IC/��̌^�g�p��~����(�蓮)����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v098 ����(APL)/�⏕ �ݗ�IC/��̌^�g�p��~����(����)����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v099 ����(APL)/�⏕ EXIC�g�p��~����(�蓮)����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v100 ����(APL)/�⏕ EXIC���p��~�񕜏�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v101 ����(APL)/�⏕ EXIC�g�p��~����(����)����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v102 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v103 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v104 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v105 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v106 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v107 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v108 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v109 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v110 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v111 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v112 ����(APL)/����������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v113 ����(APL)/EXIC�Ɩ� �����ݖ�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v114 ����(APL)/EXIC���Ɖ�Ɩ� �����ݖ�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v115 ����(APL)/�ݗ�IC�Ɩ� �����ݖ�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v116 ����(APL)/�⏕�Ɩ� �ݗ�IC�����ݖ�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v117 ����(APL)/�⏕�Ɩ� EXIC�����ݖ�������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v118 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v119 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v120 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v121 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v122 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v123 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v124 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v125 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v126 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v127 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v128 ����(APL)/�W���F�� ������NG����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v129 ����(APL)/�W���F�� �ǎ攻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v130 ����(APL)/�W���F�� ������������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v131 ����(APL)/�W���F�� IDi����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v132 ����(APL)/�W���F�� �o�[�W��������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v133 ����(APL)/�W���F�� IC��ʔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v134 ����(APL)/�W���F�� �f�[�^���ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v135 ����(APL)/�W���F�� �}�X�^�f�[�^����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v136 ����(APL)/�W���F�� ����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v137 ����(APL)/�W���F�� �J�[�h�g�p�s����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v138 ����(APL)/�W���F�� �p�X���[�h���b�N����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v139 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v140 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v141 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v142 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v143 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v144 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v145 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v146 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v147 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v148 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v149 ����(APL)/EXIC ������NG����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v150 ����(APL)/EXIC ���������E�g��������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v151 ����(APL)/EXIC �ǎ攻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v152 ����(APL)/EXIC IDi����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v153 ����(APL)/EXIC �ް�ޮݔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v154 ����(APL)/EXIC �ް����ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v155 ����(APL)/EXIC ���ގg�p�s����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v156 ����(APL)/EXIC ����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v157 ����(APL)/EXIC EXIC�l�K�`�F�b�N����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v158 ����(APL)/EXIC ��̌^IC�ł̍ݗ�����`�F�b�N����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v159 ����(APL)/EXIC ���o�꼰�ݽ����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v160 ����(APL)/EXIC �\�񌟍�����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v161 ����(APL)/EXIC �\���񔻒�NG(�\��ύX��)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v162 ����(APL)/EXIC �\���񔻒�NG(�폜)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v163 ����(APL)/EXIC �\���񔻒�NG(����ς�)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v164 ����(APL)/EXIC �\���񔻒�NG(�����ς�)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v165 ����(APL)/EXIC �\���񔻒�(IDm����è)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v166 ����(APL)/EXIC �\���񔻒�(���̑�)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v167 ����(APL)/EXIC �\����o�H����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v168 ����(APL)/EXIC �I��Ԕ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v169 ����(APL)/EXIC ���p���t����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v170 ����(APL)/EXIC ���w���Ԕ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v171 ����(APL)/EXIC ��Ԕ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v172 ����(APL)/EXIC �܂�Ԃ�����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v173 ����(APL)/EXIC �^�x�E�S�Ԏ��R�E�x������������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v174 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v175 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v176 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v177 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v178 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v179 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v180 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v181 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v182 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v183 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v184 ����(APL)/EXIC���Ɖ� ������NG����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v185 ����(APL)/EXIC���Ɖ� ���������E�g��������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v186 ����(APL)/EXIC���Ɖ� �ǎ攻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v187 ����(APL)/EXIC���Ɖ� IDi����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v188 ����(APL)/EXIC���Ɖ� �ް�ޮݔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v189 ����(APL)/EXIC���Ɖ� �ް����ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v190 ����(APL)/EXIC���Ɖ� ���ގg�p�s����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v191 ����(APL)/EXIC���Ɖ� ����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v192 ����(APL)/EXIC���Ɖ� EXIC�l�K�`�F�b�N����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v193 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v194 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v195 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v196 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v197 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v198 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v199 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v200 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v201 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v202 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v203 ����(APL)/�ݗ�IC ������NG����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v204 ����(APL)/�ݗ�IC ������������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v205 ����(APL)/�ݗ�IC �ǎ攻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v206 ����(APL)/�ݗ�IC IDi����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v207 ����(APL)/�ݗ�IC �ް�ޮݔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v208 ����(APL)/�ݗ�IC ICýĶ��ޔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v209 ����(APL)/�ݗ�IC IC��ʔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v210 ����(APL)/�ݗ�IC �ް����ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v211 ����(APL)/�ݗ�IC ���Ў戵�}�̔���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v212 ����(APL)/�ݗ�IC �\���Ώ۶��ޔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v213 ����(APL)/�ݗ�IC IC���ސ���������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v214 ����(APL)/�ݗ�IC ����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v215 ����(APL)/�ݗ�IC ����������NG�i�O�񑀍삪�������j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v216 ����(APL)/�ݗ�IC 10�N��������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v217 ����(APL)/�ݗ�IC ���ގg�p�s����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v218 ����(APL)/�ݗ�IC ���ޗL����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v219 ����(APL)/�ݗ�IC �l�K�`�F�b�N����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v220 ����(APL)/�ݗ�IC Ͻ��ް�����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v221 ����(APL)/�ݗ�IC �������ُ�װ", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v222 ����(APL)/�ݗ�IC ����ُ�", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v223 ����(APL)/�ݗ�IC ���̑�����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v224 ����(APL)/�ݗ�IC ������Ԕ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v225 ����(APL)/�ݗ�IC ���o�꼰�ݽ����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v226 ����(APL)/�ݗ�IC ���p���t����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v227 ����(APL)/�ݗ�IC ���w���Ԕ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v228 ����(APL)/�ݗ�IC ���w���Ԕ���NG(��ň�v)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v229 ����(APL)/�ݗ�IC ��Ԕ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v230 ����(APL)/�ݗ�IC ������n�溰�ޔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v231 ����(APL)/�ݗ�IC IC�����ԉw���ޔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v232 ����(APL)/�ݗ�IC SF�n�溰�ޔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v233 ����(APL)/�ݗ�IC ��ЊԌo�H�A��������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v234 ����(APL)/�ݗ�IC ��ԉw�ر����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v235 ����(APL)/�ݗ�IC �ʉ߻��޽����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v236 ����(APL)/�ݗ�IC ���ԗp�m�FNG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v237 ����(APL)/�ݗ�IC �߲�Ċ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v238 ����(APL)/�ݗ�IC ��_�ʉߔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v239 ����(APL)/�ݗ�IC �s������(���o�ꎞ�Ԕ���)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v240 ����(APL)/�ݗ�IC �s������(����w���o�ꔻ��)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v241 ����(APL)/�ݗ�IC �s������(���o�껲�ٔ���)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v242 ����(APL)/�ݗ�IC �s������(�ē�������)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v243 ����(APL)/�ݗ�IC �s������(�o�H�O����)NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v244 ����(APL)/�ݗ�IC �c�z����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v245 ����(APL)/�ݗ�IC 1ׯ����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v246 ����(APL)/�ݗ�IC ̪�پ�̔���1NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v247 ����(APL)/�ݗ�IC ̪�پ�̔���2NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v248 ����(APL)/�ݗ�IC ̪�پ�̔���3NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v249 ����(APL)/�ݗ�IC ̪�پ�̔���4NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v250 ����(APL)/�ݗ�IC ����s��s��ر����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v251 ����(APL)/�ݗ�IC ���~�ߔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v252 ����(APL)/�ݗ�IC ���o�ꎞ�Ԓ��ߔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v253 ����(APL)/�ݗ�IC �ݗ�IC�������̑��^�p����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v254 ����(APL)/�ݗ�IC �V����IC�������o�꼰�ݽ����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v255 ����(APL)/�ݗ�IC �V����IC�������p���t����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v256 ����(APL)/�ݗ�IC �V����IC�������w����NG(���-��v)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v257 ����(APL)/�ݗ�IC �V����IC�����c�z����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v258 ����(APL)/�ݗ�IC �V����IC������ԉw����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v259 ����(APL)/�ݗ�IC �V����IC��������w��ԊONG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v260 ����(APL)/�ݗ�IC �V����IC�������픻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v261 ����(APL)/�ݗ�IC �V����IC�����f�[�^���ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v262 ����(APL)/�ݗ�IC �V����IC�������̑��^�p����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v263 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v264 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v265 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v266 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v267 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v268 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v269 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v270 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v271 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v272 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v273 ����(APL)/���C�� ������NG����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v274 ����(APL)/���C�� �ُ팔����NG(�񎥋C����)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v275 ����(APL)/���C�� �ُ팔����NG(̫�ϯĴװ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v276 ����(APL)/���C�� �ُ팔����NG(���è�װ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v277 ����(APL)/���C�� �ُ팔����NG(�������װ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v278 ����(APL)/���C�� �ُ팔����NG(��d���װ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v279 ����(APL)/���C�� �ُ팔����NG(,W1�EW2���è�װ)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v280 ����(APL)/���C�� �ُ팔����NG(,������ԏ�)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v281 ����(APL)/���C�� �ُ팔����NG(̪�پ��)", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v282 ����(APL)/���C�� ���p���Ԑ�������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v283 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v284 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v285 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v286 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v287 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v288 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v289 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v290 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v291 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v292 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v293 ����(APL)/�⏕ ������NG����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v294 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ ������������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v295 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ �ǎ攻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v296 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ IDi����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v297 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ �ް�ޮݔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v298 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ �\���Ώ۶��ޔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v299 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ ICýĶ��ޔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v300 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ IC��ʔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v301 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ �ް����ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v302 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ ���Ў戵�}�̔���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v303 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ IC���ސ���������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v304 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ ����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v305 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ ����������NG�i�O�񑀍삪�������j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v306 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ ���ގg�p�s����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v307 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ �l�K�`�F�b�N����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v308 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ Ͻ��ް�����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v309 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ �������ُ�װ", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v310 ����(APL)/�⏕ �ݗ�IC/��̌^IC�g�p��~ ���̑�����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v311 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v312 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v313 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v314 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v315 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v316 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v317 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v318 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v319 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v320 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v321 ����(APL)/�⏕ EXIC�g�p��~ �ΏۊO�}��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v322 ����(APL)/�⏕ EXIC�g�p��~ ���������E�g��������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v323 ����(APL)/�⏕ EXIC�g�p��~ �ǎ攻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v324 ����(APL)/�⏕ EXIC�g�p��~ IDi����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v325 ����(APL)/�⏕ EXIC�g�p��~ �ް�ޮݔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v326 ����(APL)/�⏕ EXIC�g�p��~ �ް����ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v327 ����(APL)/�⏕ EXIC�g�p��~ ���ގg�p�s����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v328 ����(APL)/�⏕ EXIC�g�p��~ ����������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v329 ����(APL)/�⏕ EXIC�g�p��~ EXIC�l�K�`�F�b�N����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v330 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v331 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v332 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v333 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v334 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v335 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v336 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v337 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v338 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v339 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v340 ����(APL)/�⏕ EXIC���p��~�� ���������E�g��������NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v341 ����(APL)/�⏕ EXIC���p��~�� �ǎ攻��NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v342 ����(APL)/�⏕ EXIC���p��~�� IDi����NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v343 ����(APL)/�⏕ EXIC���p��~�� �ް�ޮݔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v344 ����(APL)/�⏕ EXIC���p��~�� �ް����ڔ���NG", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v345 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v346 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v347 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v348 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v349 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v350 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v351 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v352 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v353 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v354 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v355 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v356 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v357 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v358 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v359 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v360 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v361 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v362 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v363 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v364 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v365 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v366 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v367 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v368 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v369 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v370 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v371 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v372 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v373 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v374 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v375 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v376 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v377 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v378 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v379 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v380 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v381 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v382 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v383 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v384 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v385 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v386 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v387 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v388 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v389 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v390 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v391 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v392 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v393 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v394 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v395 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v396 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v397 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v398 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v399 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v400 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v401 ����(FW)/���C���J�e�v�J�E���g �}������", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v402 ����(FW)/���C���J�e�v�J�E���g ���C�w�b�h�ʉߌ���", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v403 ����(FW)/���C���J�e�v�J�E���g ���[�h���g���C����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v404 ����(FW)/���C���J�e�v�J�E���g ���C�g���g���C����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v405 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v406 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v407 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v408 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v409 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v410 ����(FW)/���C���J�e�v�J�E���g �}�������������[�^PM01�����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v411 ����(FW)/���C���J�e�v�J�E���g �}�����V���b�^SOL1�����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v412 ����(FW)/���C���J�e�v�J�E���g �G�h������K�C�hSOL02�����", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v413 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v414 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v415 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v416 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v417 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v418 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v419 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v420 ����(FW)�\��", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v421 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v422 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v423 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v424 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v425 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v426 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v427 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v428 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v429 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v430 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v431 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v432 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v433 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v434 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v435 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v436 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v437 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v438 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v439 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v440 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v441 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v442 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v443 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v444 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v445 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v446 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v447 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v448 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v449 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v450 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v451 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v452 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v453 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v454 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v455 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v456 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v457 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v458 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v459 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v460 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v461 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v462 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v463 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v464 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v465 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v466 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v467 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v468 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v469 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v470 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v471 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v472 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v473 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v474 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v475 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v476 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v477 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v478 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v479 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v480 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v481 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v482 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v483 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v484 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v485 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v486 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v487 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v488 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v489 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v490 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v491 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v492 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v493 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v494 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v495 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v496 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v497 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v498 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v499 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
        New XlsField(8*4, "D", 1, " "c, "�W�v500 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian)}

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

    Public Shared Sub InitBaseHeaderFields(ByVal machine As EkCode, ByVal d As DateTime, ByVal seqNum As UInteger, ByVal oBytes As Byte())
        SetFieldValueToBytes("��{�w�b�_�[ �f�[�^���", "A7", oBytes)
        SetFieldValueToBytes("��{�w�b�_�[ �w�R�[�h", machine.ToString("%3R-%3S"), oBytes)
        SetFieldValueToBytes("��{�w�b�_�[ ��������", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes("��{�w�b�_�[ �R�[�i�[", machine.ToString("%C"), oBytes)
        SetFieldValueToBytes("��{�w�b�_�[ ���@", machine.ToString("%U"), oBytes)
        SetFieldValueToBytes("��{�w�b�_�[ �V�[�P���XNo", seqNum.ToString(), oBytes)
        SetFieldValueToBytes("��{�w�b�_�[ �o�[�W����", "01", oBytes)
    End Sub

    Public Shared Sub InitCommonPartFields(ByVal machine As EkCode, ByVal d As DateTime, ByVal oBytes As Byte())
        SetFieldValueToBytes("���ʕ� �W�v�J�n����", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes("���ʕ� �W�v�I��(���W)����", "00000000000000", oBytes)
        SetFieldValueToBytes("���ʕ� ���D���������_������", "00000000000000", oBytes)
        SetFieldValueToBytes("���ʕ� �W�D���������_������", "00000000000000", oBytes)
        SetFieldValueToBytes("���ʕ� ���D���������ԍ�", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes("���ʕ� �W�D���������ԍ�", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes("���ʕ� ���D�����m�Z���T���x��", Field("���ʕ� ���D�����m�Z���T���x��").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes("���ʕ� �W�D�����m�Z���T���x��", Field("���ʕ� �W�D�����m�Z���T���x��").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes("���ʕ� �\��", Field("���ʕ� �\��").CreateDefaultValue(), oBytes)
    End Sub

    Public Shared Sub UpdateSummaryFields(ByVal oBytes As Byte())
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 293).MetaName, GetSummary(294, 344, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 273).MetaName, GetSummary(274, 282, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 203).MetaName, GetSummary(204, 262, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 184).MetaName, GetSummary(185, 192, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 149).MetaName, GetSummary(150, 173, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 128).MetaName, GetSummary(129, 138, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 112).MetaName, GetSummary(113, 117, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 9).MetaName, GetSummary(99, 101, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 8).MetaName, GetSummary(97, 98, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 7).MetaName, GetSummary(79, 86, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 6).MetaName, GetSummary(57, 68, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 5).MetaName, GetSummary(41, 46, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 4).MetaName, GetSummary(23, 30, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 3).MetaName, GetFieldValueFromBytes(Fields(AggregateFieldsOrigin + 7).MetaName, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 2).MetaName, GetSummary(New Integer() {6, 8}, oBytes), oBytes)
        SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 1).MetaName, GetSummary(New Integer() {4, 5, 9}, oBytes), oBytes)
    End Sub

    Private Shared Function GetSummary(ByVal firstAggregateNumber As Integer, ByVal lastAggregateNumber As Integer, ByVal oBytes As Byte()) As String
        Dim sum As Long = 0
        For i As Integer = AggregateFieldsOrigin + firstAggregateNumber To AggregateFieldsOrigin + lastAggregateNumber
            sum += Long.Parse(GetFieldValueFromBytes(oFields(i).MetaName, oBytes))
        Next i
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Shared Function GetSummary(ByVal aggregateNumbers As Integer(), ByVal oBytes As Byte()) As String
        Dim sum As Long = 0
        For Each n As Integer In aggregateNumbers
            Dim i As Integer = AggregateFieldsOrigin + n
            sum += Long.Parse(GetFieldValueFromBytes(oFields(i).MetaName, oBytes))
        Next n
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

End Class
