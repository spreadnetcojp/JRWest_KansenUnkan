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

    Private Shared oFieldRefs(1) As Dictionary(Of String, FieldRef)
    Private Shared totalBitCount(1) As Integer

    Private Const AggregateFieldsOrigin As Integer = 15
    Private Shared ReadOnly oFields As XlsField()() = { _
        New XlsField() { _
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
            New XlsField(8*8, "X16", 1, " "c, "���ʕ� ���D���������ԍ�"), _
            New XlsField(8*8, "X16", 1, " "c, "���ʕ� �W�D���������ԍ�"), _
            New XlsField(8*1, "D", 48, " "c, "���ʕ� ���D�����m�Z���T���x��"), _
            New XlsField(8*1, "D", 48, " "c, "���ʕ� �W�D�����m�Z���T���x��"), _
            New XlsField(8*1, "X2", 48, " "c, "���ʕ� �\��"), _
            New XlsField(8*4, "D", 1, " "c, "�W�v001 ��(�`)����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v002 ��(�`)����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v003 ��(�`)�P����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v004 ��(�`)�Q����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v005 ��(�`)�R����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v006 ��(�`)�S����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v007 ��(�`)�T���ȏ㓊������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v008 ��(�`)�ꊇ���������i�Q���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v009 ��(�`)�ꊇ���������i�R���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v010 ��(�`)�ꊇ���������i�S���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v011 ��(�`)�ꊇ���������i�T���ȏ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v012 ��(�`)�S�����\��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v013 ��(�`)�S��������������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v014 ��(�`)���\������������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v015 ��(�`)�\���������i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v016 ��(�`)�\���������i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v017 ��(�`)�\���������i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v018 ��(�`)�\���������i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v019 ��(�`)�����������i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v020 ��(�`)�����������i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v021 ��(�`)�����������i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v022 ��(�`)�����������i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v023 ��(�`)������n�j����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v024 ��(�`)������n�j�����i���v�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v025 ��(�`)������n�j�����i�P�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v026 ��(�`)������n�j�����i�Q�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v027 ��(�`)������n�j�����i�R�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v028 ��(�`)������n�j�����i�S�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v029 ��(�`)������n�j�����iNRZ����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v030 ��(�`)������n�j�����iFM����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v031 ��(�`)������n�j�����iNRZ������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v032 ��(�`)������n�j�����iFM������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v033 ��(�`)������n�j�����iFM��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v034 ��(�`)������n�j�����i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v035 ��(�`)����ΏۊO����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v036 ��(�`)�ǉ������҂������i��Ԍ������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v037 ��(�`)�ǉ������҂������i���}�������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v038 ��(�`)�ǉ������҂������i���w���������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v039 ��(�`)�ǉ������҂������i��Ԍ�+���w���������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v040 ��(�`)�ǉ������҂������i���}��+���w���������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v041 ��(�`)�ǉ������҂������i��Ԍ�+���}�������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v042 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v043 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v044 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v045 ��(�`)���̑��h�b������t����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v046 ��(�`)�h�b�P����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v047 ��(�`)�ǉ������҂������i�ݗ��h�b�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v048 ��(�`)�ǉ������҂������i�V������p�����w�����Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v049 ��(�`)�����p�[���������i�݌v�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v050 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v051 ��(�`)������m�f����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v052 ��(�`)�ُ팔����m�f�i�\�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v053 ��(�`)�ُ팔����m�f�i�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v054 ��(�`)�ُ팔����m�f�i���è�װ�F����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v055 ��(�`)�ُ팔����m�f�i���è�װ�F������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v056 ��(�`)�ُ팔����m�f�i̫�ϯĴװ�F����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v057 ��(�`)�ُ팔����m�f�i̫�ϯĴװ�F������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v058 ��(�`)�ُ팔����m�f�i̫�ϯĴװ�F��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v059 ��(�`)�ُ팔����m�f�i̫�ϯĴװ�F���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v060 ��(�`)�ُ팔����m�f�i��d���װ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v061 ��(�`)�ُ팔����m�f�i�������װ�F����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v062 ��(�`)�ُ팔����m�f�i�������װ�F������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v063 ��(�`)�ُ팔����m�f�i�������װ�F��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v064 ��(�`)�ُ팔����m�f�i�������װ�F���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v065 ��(�`)�ُ팔����m�f�i�񎥋C�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v066 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v067 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v068 ��(�`)����������m�f�i���픻��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v069 ��(�`)����������m�f�i��l�����������ݔ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v070 ��(�`)����������m�f�i���Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v071 ��(�`)����������m�f�i��Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v072 ��(�`)����������m�f�i���ꌔ���Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v073 ��(�`)����������m�f�i�I��Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v074 ��(�`)����������m�f�i�g�p�ϔ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v075 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v076 ��(�`)����������m�f�i���攻��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v077 ��(�`)����������m�f�i�������L������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v078 ��(�`)����������m�f�i�g�p�J�n�㔻��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v079 ��(�`)����������m�f�i������������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v080 ��(�`)�L���g��������m�f�i��Ԍ������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v081 ��(�`)�L���g��������m�f�i���}�������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v082 ��(�`)�L���g��������m�f�i���w���������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v083 ��(�`)�L���g��������m�f�i��Ԍ��E���w���������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v084 ��(�`)�L���g��������m�f�i���}���E���w���������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v085 ��(�`)�L���g��������m�f�i��Ԍ��E���}�������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v086 ��(�`)�L���g��������m�f�i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v087 ��(�`)�g��������m�f�i��Ԍ�����}����Ԕ�r����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v088 ��(�`)�g�����ُ�i�V������p�����w�����Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v089 ��(�`)�g��������m�f�i�ڑ�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v090 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v091 ��(�`)�g��������m�f�i���p����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v092 ��(�`)�ݗ�IC�{�V�������C�R����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v093 ��(�`)�ǉ������҂������i�d�w�h�b�A(��)�����(IC)�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v094 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v095 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v096 ��(�`)�s������m�f�i������g�p�ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v097 ��(�`)�h�b�Q����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v098 ��(�`)���̑��m�f�i�x��������s�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v099 ��(�`)�h�c�`�F�b�N����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v100 ��(�`)���h�b���C���p����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v101 ��(�`)���C��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v102 ��(�`)���C���������i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v103 ��(�`)���C���������i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v104 ��(�`)���C���������i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v105 ��(�`)���C���������i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v106 ��(�`)���C������ײ����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v107 ��(�`)���C������ײ�񐔁i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v108 ��(�`)���C������ײ�񐔁i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v109 ��(�`)���C������ײ�񐔁i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v110 ��(�`)���C������ײ�񐔁i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v111 ��(�`)���C������ײ���n�j�񐔁i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v112 ��(�`)���C������ײ���n�j�񐔁i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v113 ��(�`)���C������ײ���n�j�񐔁i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v114 ��(�`)���C������ײ���n�j�񐔁i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v115 ��(�`)���C������ײ���m�f�񐔁i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v116 ��(�`)���C������ײ���m�f�񐔁i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v117 ��(�`)���C������ײ���m�f�񐔁i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v118 ��(�`)���C������ײ���m�f�񐔁i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v119 ��(�`)�p���`�񐔁i���ڈ�����F���D��ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v120 ��(�`)�p���`�񐔁i���ڈ�����F���D85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v121 ��(�`)�p���`�񐔁i���ڈ�����F�W�D��ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v122 ��(�`)�p���`�񐔁i���ڈ�����F�W�D85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v123 ��(�`)�p���`�񐔁i�]�ʈ�����F85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v124 ��(�`)����񐔁i���ڈ�����F�㑤��ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v125 ��(�`)����񐔁i���ڈ�����F�㑤85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v126 ��(�`)����񐔁i���ڈ�����F������ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v127 ��(�`)����񐔁i���ڈ�����F����85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v128 ��(�`)����񐔁i�]�ʈ�����F85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v129 ��(�e)�r�m�c�|�l�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v130 ��(�e)�r�m�c�|�l�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v131 ��(�e)�r�m�c�|�l�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v132 ��(�e)�r�m�c�|�l�U�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v133 ��(�e)�r�m�c�|�l�V�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v134 ��(�e)�r�m�c�|�o�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v135 ��(�e)�r�m�c�|�o�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v136 ��(�e)�r�m�c�|�o�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v137 ��(�e)�l�s�q�|�d�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v138 ��(�e)�l�s�q�|�d�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v139 ��(�e)�l�s�q�|�g�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v140 ��(�e)�l�s�q�|�g�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v141 ��(�e)�l�s�q�|�g�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v142 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v143 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v144 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v145 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v146 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v147 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v148 ��(�e)�������捞�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v149 ��(�e)�������J�o�������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v150 ��(�e)���񕔔�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v151 ��(�`)���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v152 ��(�`)�P���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v153 ��(�`)�Q���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v154 ��(�`)�R���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v155 ��(�`)�S���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v156 ��(�`)���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v157 ��(�`)�P���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v158 ��(�`)�Q���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v159 ��(�`)�R���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v160 ��(�`)�S���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v161 ��(�`)�ۗ������i�����ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v162 ��(�`)�ۗ������i�s���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v163 ��(�`)��d���ɂ��~�ϖ����iB,G�ׯ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v164 ��(�`)��d���ɂ��~�ϖ����iB,G�ׯ��ȊO�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v165 ��(�`)���񕔓����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v166 ��(�`)�����]��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v167 ��(�`)�d�w�h�b�{���C�P����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v168 ��(�`)�d�w�h�b�{���C�Q����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v169 ��(�`)�d�w�h�b�{���C�R����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v170 ��(�`)�ݗ��h�b�{�V�������C�P����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v171 ��(�`)�^�x�����Ώی���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v172 ��(�`)�S�Ԏ��R�ȑΏی���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v173 ��(�`)�x�����Ώی���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v174 ��(�`)�ݗ��h�b�{�V�������C�Q����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v175 ��(�e)�r�m�c�|�`�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v176 ��(�e)�r�m�c�|�`�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v177 ��(�e)�r�m�c�|�`�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v178 ��(�e)�r�m�c�|�`�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v179 ��(�e)�r�m�c�|�l�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v180 ��(�e)�r�m�c�|�o�U�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v181 ��(�e)�r�m�c�|�o�V�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v182 ��(�e)�r�m�c�|�o�W�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v183 ��(�e)�r�m�c�|�o�X�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v184 ��(�e)�r�m�c�|�d�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v185 ��(�e)�r�m�c�|�d�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v186 ��(�e)�r�m�c�|�d�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v187 ��(�e)�r�m�c�|�d�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v188 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v189 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v190 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v191 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v192 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v193 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v194 ��(�e)�l�s�q�|�`�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v195 ��(�e)�l�s�q�|�`�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v196 ��(�e)�l�s�q�|�`�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v197 ��(�e)�l�s�q�|�l�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v198 ��(�`)���h�b������t����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v199 ��(�`)�d�w�h�b������t����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v200 ��(�`)�ݗ��h�b������t����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v201 �W(�`)����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v202 �W(�`)����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v203 �W(�`)�P����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v204 �W(�`)�Q����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v205 �W(�`)�R����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v206 �W(�`)�S����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v207 �W(�`)�T���ȏ㓊������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v208 �W(�`)�ꊇ���������i�Q���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v209 �W(�`)�ꊇ���������i�R���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v210 �W(�`)�ꊇ���������i�S���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v211 �W(�`)�ꊇ���������i�T���ȏ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v212 �W(�`)�S�����\��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v213 �W(�`)�S��������������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v214 �W(�`)���\������������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v215 �W(�`)�\���������i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v216 �W(�`)�\���������i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v217 �W(�`)�\���������i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v218 �W(�`)�\���������i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v219 �W(�`)�����������i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v220 �W(�`)�����������i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v221 �W(�`)�����������i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v222 �W(�`)�����������i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v223 �W(�`)������n�j����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v224 �W(�`)������n�j�����i���v�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v225 �W(�`)������n�j�����i�P�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v226 �W(�`)������n�j�����i�Q�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v227 �W(�`)������n�j�����i�R�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v228 �W(�`)������n�j�����i�S�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v229 �W(�`)������n�j�����iNRZ����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v230 �W(�`)������n�j�����iFM����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v231 �W(�`)������n�j�����iNRZ������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v232 �W(�`)������n�j�����iFM������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v233 �W(�`)������n�j�����iFM��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v234 �W(�`)������n�j�����i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v235 �W(�`)����ΏۊO����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v236 �W(�`)�ǉ������҂������i��Ԍ������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v237 �W(�`)�ǉ������҂������i���}�������҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v238 (�`)�^���ꌔ�s�����p�i�ʘH��ʉ߂����߂�s�ׁj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v239 (�`)�^���ꌔ�s�����p�i�Q�l�g�ɂ��A����������s�ׁj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v240 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v241 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v242 �W(�`)�ǉ������҂������i���w���猔�����҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v243 �W(�`)�ǉ������҂������i���}���{���w���猔�����҂��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v244 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v245 �W(�`)���̑��h�b��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v246 �W(�`)�h�b���������i�P�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v247 �W(�`)�ǉ������҂������i�ݗ��h�b�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v248 �W(�`)�ǉ������҂������i�V������p�����w���猔�Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v249 �W(�`)�����p�[���������i�݌v�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v250 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v251 �W(�`)������m�f����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v252 �W(�`)�ُ팔����m�f�i�\�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v253 �W(�`)�ُ팔����m�f�i�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v254 �W(�`)�ُ팔����m�f�i���è�װ�F����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v255 �W(�`)�ُ팔����m�f�i���è�װ�F������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v256 �W(�`)�ُ팔����m�f�i̫�ϯĴװ�F����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v257 �W(�`)�ُ팔����m�f�i̫�ϯĴװ�F������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v258 �W(�`)�ُ팔����m�f�i̫�ϯĴװ�F��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v259 �W(�`)�ُ팔����m�f�i̫�ϯĴװ�F���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v260 �W(�`)�ُ팔����m�f�i��d���װ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v261 �W(�`)�ُ팔����m�f�i�������װ�F����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v262 �W(�`)�ُ팔����m�f�i�������װ�F������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v263 �W(�`)�ُ팔����m�f�i�������װ�F��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v264 �W(�`)�ُ팔����m�f�i�������װ�F���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v265 �W(�`)�ُ팔����m�f�i�񎥋C�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v266 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v267 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v268 �W(�`)����������m�f�i���픻��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v269 �W(�`)����������m�f�i��l�����������ݔ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v270 �W(�`)����������m�f�i���Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v271 �W(�`)����������m�f�i��Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v272 �W(�`)����������m�f�i���ꌔ���Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v273 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v274 �W(�`)����������m�f�i�g�p�ϔ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v275 �W(�`)����������m�f�i���w���Ԕ���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v276 �W(�`)����������m�f�i���攻��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v277 �W(�`)����������m�f�i�������L������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v278 �W(�`)����������m�f�i�g�p�J�n�㔻��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v279 �W(�`)����������m�f�i������������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v280 �W(�`)�L���g��������m�f�i��Ԍ������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v281 �W(�`)�L���g��������m�f�i���}�������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v282 �W(�`)�L���g��������m�f�i���w�����Ԍ��Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v283 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v284 �W(�`)�L���g��������m�f�i���}���{���w�����Ԍ��Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v285 �W(�`)�L���g��������m�f�i��Ԍ��E���}�������Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v286 �W(�`)�L���g��������m�f�i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v287 �W(�`)�g��������m�f�i��Ԍ�����}����Ԕ�r����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v288 �W(�`)�L���g��������m�f�i�V������p�����w�����Ԍ��Ȃ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v289 �W(�`)�g��������m�f�i�ڑ�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v290 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v291 �W(�`)�g��������m�f�i���p����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v292 �W(�`)�ݗ��h�b�{�V�������C�R����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v293 �W(�`)�ǉ������҂������i�d�w�h�b�A(��)�����(IC)�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v294 �W(�`)�s������m�f�i���o��T�C�N���ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v295 �W(�`)�s������m�f�i����w���o��ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v296 �W(�`)�s������m�f�i������g�p�ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v297 �W(�`)�h�b���������i�Q�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v298 �W(�`)���̑��m�f�i�x��������s�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v299 �W(�`)�h�c�`�F�b�N����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v300 �W(�`)���h�b���C���p����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v301 �W(�`)���C��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v302 �W(�`)���C���������i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v303 �W(�`)���C���������i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v304 �W(�`)���C���������i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v305 �W(�`)���C���������i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v306 �W(�`)���C������ײ����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v307 �W(�`)���C������ײ�񐔁i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v308 �W(�`)���C������ײ�񐔁i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v309 �W(�`)���C������ײ�񐔁i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v310 �W(�`)���C������ײ�񐔁i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v311 �W(�`)���C������ײ���n�j�񐔁i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v312 �W(�`)���C������ײ���n�j�񐔁i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v313 �W(�`)���C������ײ���n�j�񐔁i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v314 �W(�`)���C������ײ���n�j�񐔁i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v315 �W(�`)���C������ײ���m�f�񐔁i����ݿ݌��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v316 �W(�`)���C������ײ���m�f�񐔁i������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v317 �W(�`)���C������ײ���m�f�񐔁i��^���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v318 �W(�`)���C������ײ���m�f�񐔁i���̑��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v319 �W(�`)�p���`�񐔁i���ڈ�����F���D��ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v320 �W(�`)�p���`�񐔁i���ڈ�����F���D85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v321 �W(�`)�p���`�񐔁i���ڈ�����F�W�D��ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v322 �W(�`)�p���`�񐔁i���ڈ�����F�W�D85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v323 �W(�`)�p���`�񐔁i�]�ʈ�����F85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v324 �W(�`)����񐔁i���ڈ�����F�㑤��ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v325 �W(�`)����񐔁i���ڈ�����F�㑤85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v326 �W(�`)����񐔁i���ڈ�����F������ތ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v327 �W(�`)����񐔁i���ڈ�����F����85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v328 �W(�`)����񐔁i�]�ʒ��ڈ�����F85mm���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v329 �W(�e)�r�m�c�|�l�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v330 �W(�e)�r�m�c�|�l�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v331 �W(�e)�r�m�c�|�l�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v332 �W(�e)�r�m�c�|�l�U�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v333 �W(�e)�r�m�c�|�l�V�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v334 �W(�e)�r�m�c�|�o�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v335 �W(�e)�r�m�c�|�o�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v336 �W(�e)�r�m�c�|�o�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v337 �W(�e)�l�s�q�|�d�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v338 �W(�e)�l�s�q�|�d�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v339 �W(�e)�l�s�q�|�g�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v340 �W(�e)�l�s�q�|�g�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v341 �W(�e)�l�s�q�|�g�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v342 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v343 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v344 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v345 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v346 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v347 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v348 �W(�e)�������捞�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v349 �W(�e)�������J�o�������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v350 �W(�e)���񕔔�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v351 �W(�`)���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v352 �W(�`)�P���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v353 �W(�`)�Q���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v354 �W(�`)�R���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v355 �W(�`)�S���W�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v356 �W(�`)���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v357 �W(�`)�P���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v358 �W(�`)�Q���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v359 �W(�`)�R���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v360 �W(�`)�S���ʏW�D����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v361 �W(�`)�ۗ������i�����ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v362 �W(�`)�ۗ������i�s���j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v363 �W(�`)��d���ɂ��~�ϖ����iB,G�ׯ��j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v364 �W(�`)��d���ɂ��~�ϖ����iB,G�ׯ��ȊO�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v365 �W(�`)���񕔓����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v366 �W(�`)�����]��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v367 �W(�`)�d�w�h�b�{���C�P����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v368 �W(�`)�d�w�h�b�{���C�Q����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v369 �W(�`)�d�w�h�b�{���C�R����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v370 �W(�`)�ݗ��h�b�{�V�������C�P����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v371 �W(�`)�^�x ���o����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v372 �W(�`)�S�Ԏ��R�� ���o����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v373 �W(�`)�x���� �󎚖���", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v374 �W(�`)�ݗ��h�b�{�V�������C�Q����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v375 �W(�e)�r�m�c�|�`�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v376 �W(�e)�r�m�c�|�`�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v377 �W(�e)�r�m�c�|�`�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v378 �W(�e)�r�m�c�|�`�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v379 �W(�e)�r�m�c�|�l�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v380 �W(�e)�r�m�c�|�o�U�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v381 �W(�e)�r�m�c�|�o�V�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v382 �W(�e)�r�m�c�|�o�W�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v383 �W(�e)�r�m�c�|�o�X�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v384 �W(�e)�r�m�c�|�d�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v385 �W(�e)�r�m�c�|�d�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v386 �W(�e)�r�m�c�|�d�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v387 �W(�e)�r�m�c�|�d�T�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v388 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v389 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v390 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v391 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v392 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v393 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v394 �W(�e)�l�s�q�|�`�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v395 �W(�e)�l�s�q�|�`�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v396 �W(�e)�l�s�q�|�`�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v397 �W(�e)�l�s�q�|�l�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v398 �W(�`)���h�b������t����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v399 �W(�`)�d�w�h�b������t����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v400 �W(�`)�ݗ��h�b������t����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v401 (�e)��@�W�D��U�ۗ��`�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v402 (�e)��@�W�D��U�ۗ��a�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v403 (�e)�]�@�W�D��U�ۗ��`�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v404 (�`)�����J�E���^���t��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v405 (�e)�]�@�W�D��U�ۗ��a�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v406 (�e)��@�E�h�A�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v407 (�e)��@���h�A�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v408 (�e)�]�@�E�h�A�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v409 (�e)�]�@���h�A�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v410 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v411 ��(�e)�r�m�c�|�`�P�����  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v412 ��(�e)�r�m�c�|�l�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v413 ��(�e)�r�m�c�|�d�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v414 ��(�e)�l�s�q�|�l�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v415 ��(�e)�l�s�q�|�o�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v416 ��(�e)�l�s�q�|�o�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v417 ��(�e)�l�s�q�|�o�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v418 ��(�e)�l�s�q�|�o�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v419 ��(�e)�ǎ�蕔������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v420 ��(�e)�����]��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v421 ��(�e)�ۗ���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v422 ��(�e)���ڃp���`��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v423 ��(�e)���ڈ����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v424 ��(�e)�]�ʃp���`��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v425 ��(�e)�]�ʈ����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v426 ��(�e)���o��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v427 ��(�e)�W�D��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v428 ��(�e)�ʏW�D��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v429 ��(�e)���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v430 ��(�e)�s�o�g���ڂk�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v431 ��(�e)�s�o�g���ڂt�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v432 ��(�e)�s�o�g�]�ʈ����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v433 ��(�e)�s�o�g���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v434 ��(�e)���ڃ��R�p���`�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v435 ��(�e)�]�ʃ��R�p���`�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v436 ��(�e)�l�f�|�q�t��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v437 ��(�e)�l�f�|�q�t���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v438 ��(�e)�l�f�|�q�k��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v439 ��(�e)�l�f�|�q�k���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v440 ��(�e)�l�f�|�v��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v441 ��(�e)�l�f�|�v���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v442 ��(�e)�l�f�|�u��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v443 ��(�e)�l�f�|�u���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "�W�v456 �W(�e)�r�m�c�|�`�P�����  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v457 �W(�e)�r�m�c�|�l�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v458 �W(�e)�r�m�c�|�d�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v459 �W(�e)�l�s�q�|�l�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v460 �W(�e)�l�s�q�|�o�P�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v461 �W(�e)�l�s�q�|�o�Q�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v462 �W(�e)�l�s�q�|�o�R�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v463 �W(�e)�l�s�q�|�o�S�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v464 �W(�e)�ǎ�蕔������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v465 �W(�e)�����]��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v466 �W(�e)�ۗ���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v467 �W(�e)���ڃp���`��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v468 �W(�e)���ڈ����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v469 �W(�e)�]�ʃp���`��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v470 �W(�e)�]�ʈ����������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v471 �W(�e)���o��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v472 �W(�e)�W�D��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v473 �W(�e)�ʏW�D��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v474 �W(�e)���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v475 �W(�e)�s�o�g���ڂk�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v476 �W(�e)�s�o�g���ڂt�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v477 �W(�e)�s�o�g�]�ʈ����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v478 �W(�e)�s�o�g���������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v479 �W(�e)���ڃ��R�p���`�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v480 �W(�e)�]�ʃ��R�p���`�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v481 �W(�e)�l�f�|�q�t��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v482 �W(�e)�l�f�|�q�t���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v483 �W(�e)�l�f�|�q�k��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v484 �W(�e)�l�f�|�q�k���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v485 �W(�e)�l�f�|�v��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v486 �W(�e)�l�f�|�v���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v487 �W(�e)�l�f�|�u��^���ʉ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v488 �W(�e)�l�f�|�u���ʌ��ʉ�", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "�W�v500 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian)}, _
        New XlsField() { _
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
            New XlsField(8*8, "X16", 1, " "c, "���ʕ� ���D���������ԍ�"), _
            New XlsField(8*8, "X16", 1, " "c, "���ʕ� �W�D���������ԍ�"), _
            New XlsField(8*1, "D", 48, " "c, "���ʕ� ���D�����m�Z���T���x��"), _
            New XlsField(8*1, "D", 48, " "c, "���ʕ� �W�D�����m�Z���T���x��"), _
            New XlsField(8*1, "X2", 48, " "c, "���ʕ� �\��"), _
            New XlsField(8*4, "D", 1, " "c, "�W�v001 ��(�`)�ǎ�ُ�|��w�b�h�i�G�h�����\�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v002 ��(�`)�ǎ�ُ�|��w�b�h�i�W�T�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v003 ��(�`)�ǎ�ُ�|���w�b�h�i�G�h�����\�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v004 ��(�`)�ǎ�ُ�|���w�b�h�i�W�T�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v005 ��(�`)�ǎ�ُ�|��w�b�h�@�P�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v006 ��(�`)�ǎ�ُ�|��w�b�h�@�Q�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v007 ��(�`)�ǎ�ُ�|��w�b�h�@�R�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v008 ��(�`)�ǎ�ُ�|��w�b�h�@�S�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v009 ��(�`)�ǎ�ُ�|��w�b�h�@�T�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v010 ��(�`)�ǎ�ُ�|��w�b�h�@�U�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v011 ��(�`)�ǎ�ُ�|��w�b�h�@�V�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v012 ��(�`)�ǎ�ُ�|��w�b�h�@�W�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v013 ��(�`)�ǎ�ُ�|���w�b�h�@�P�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v014 ��(�`)�ǎ�ُ�|���w�b�h�@�Q�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v015 ��(�`)�ǎ�ُ�|���w�b�h�@�R�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v016 ��(�`)�ǎ�ُ�|���w�b�h�@�S�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v017 ��(�`)�ǎ�ُ�|���w�b�h�@�T�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v018 ��(�`)�ǎ�ُ�|���w�b�h�@�U�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v019 ��(�`)�ǎ�ُ�|���w�b�h�@�V�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v020 ��(�`)�ǎ�ُ�|���w�b�h�@�W�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v021 ��(�`)�����ُ�񐔁|�G�h�����\����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v022 ��(�`)�����ُ�񐔁|�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v023 ��(�`)�����ُ�񐔁|��^��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v024 ��(�`)�����ُ�񐔁|���̑��iSF�J�[�h�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v025 ��(�`)�����ُ�A���|�G�h�����\����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v026 ��(�`)�����ُ�A���|�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v027 ��(�`)�����ُ�A���|��^��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v028 ��(�`)�����ُ�A���|���̑��iSF�J�[�h�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v029 ��(�`)�����ُ�A���|���w�b�h�@�P�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v030 ��(�`)�����ُ�A���|���w�b�h�@�Q�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v031 ��(�`)�����ُ�A���|���w�b�h�@�R�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v032 ��(�`)�����ُ�A���|���w�b�h�@�S�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v033 ��(�`)�����ُ�A���|���w�b�h�@�T�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v034 ��(�`)�����ُ�A���|���w�b�h�@�U�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v035 ��(�`)�����ُ�A���|���w�b�h�@�V�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v036 ��(�`)�����ُ�A���|���w�b�h�@�W�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v037 ��(�`)�h�b�q�v�ُ팟�m��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v038 ��(�`)�����p�[�����ُ팏��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v039 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v040 ��(�`)���h�b��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v041 ��(�`)�h�b�ǎ�薢������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v042 ��(�`)�d�w�h�b�����ݖ��������i�P���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v043 ��(�`)�ݗ��h�b�����ݖ��������i�P���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v044 ��(�`)�h�b�ǎ攻��ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v045 ��(�`)�h�b��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v046 ��(�`)�h�b�h�c������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v047 ��(�`)�d�w�h�b�\���񌟍��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v048 ��(�`)�d�w�h�b�o�[�W��������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v049 ��(�`)�d�w�h�b�f�[�^���ڔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v050 ��(�`)�d�w�h�b�J�[�h�g�p�s����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v051 ��(�`)�d�w�h�b�ŏI���p���t����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v052 ��(�`)�d�w�h�b�l�K�`�F�b�N����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v053 ��(�`)�d�w�h�b���o��V�[�P���X����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v054 ��(�`)�d�w�h�b�\���񔻒�m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v055 ��(�`)�d�w�h�b�I��Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v056 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v057 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v058 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v059 ��(�`)�d�w�h�b�ݗ������o��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v060 ��(�`)EXIC���w�����Ȃ��m�f���w���猔�Ȃ��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v061 ��(�`)�ݗ��h�b�o�[�W��������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v062 ��(�`)�ݗ��h�b�h�b��ʔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v063 ��(�`)�ݗ��h�b�f�[�^���ڔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v064 ��(�`)�ݗ��h�b�J�[�h����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v065 ��(�`)�ݗ��h�b�}�X�^�f�[�^����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v066 ��(�`)�ݗ��h�b����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v067 ��(�`)�ݗ��h�b�J�[�h�g�p�s����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v068 ��(�`)�ݗ��h�b�l�K�`�F�b�N����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v069 ��(�`)�ݗ��h�b��������Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v070 ��(�`)�ݗ��h�b���o��V�[�P���X����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v071 ��(�`)�ݗ��h�b���p���t����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v072 ��(�`)�ݗ��h�b���w���Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v073 ��(�`)�ݗ��h�b��Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v074 ��(�`)�ݗ��h�b���o��R�[�h����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v075 ��(�`)�ݗ��h�b�c�z����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v076 ��(�`)�ݗ��h�b���Z����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v077 ��(�`)�ݗ��h�b��_�ʉߔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v078 ��(�`)�ݗ�IC�s������m�f�i���o��T�C�N���ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v079 ��(�`)�ݗ�IC�s������m�f�i���o�ꎞ�Ԉُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v080 ��(�`)�ݗ�IC�s������m�f�i����w���o��ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v081 ��(�`)�ݗ�IC�s������m�f�i�A������E�o��ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v082 ��(�`)�ݗ��h�b�V�����L�����Ȃ��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v083 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v084 ��(�`)���C�h�b���p�召���ݔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v085 ��(�`)���C�h�b���p�V������ԏd���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v086 ��(�`)���C�h�b���p�ڑ��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v087 ��(�`)���C�h�b���p���w�����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v088 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v089 ��(�`)���C�h�b���p�L�����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v090 ��(�`)���C�h�b���p���Z�s�m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v091 ��(�`)�d�w�h�b�����ُ݈�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v092 ��(�`)�ݗ��h�b�����ُ݈�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v093 ��(�`)�ݗ��h�b�e�X�g�J�[�h����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v094 ��(�`)�ݗ��h�b�����ԃG���A�m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v095 ��(�`)�ݗ��h�b�ŏI���p���t����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v096 ��(�`)�ݗ��h�b���Њ����h�b�J�[�h�r�e���p�m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v097 ��(�`)�d�w�h�b�����ݖ��������i�Q���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v098 ��(�`)�ݗ��h�b�����ݖ��������i�Q���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v099 ��(�`)���h�b����m�f����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v100 ��(�`)�ݗ��h�b��ЊԌo�H�A��������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v101 ��(�e)�������ݻ�d��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v102 ��(�e)��������ɲ�ޓd��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v103 ��(�e)��������ɲ��PL���m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v104 ��(�e)������Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v105 ��(�e)������Ӱ��d���װ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v106 ��(�e)�{�Q�S�u�d��          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v107 ��(�e)���C���ݻ�d��         ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v108 ��(�e)���C����ɲ�ޓd��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v109 ��(�e)���C����ɲ��PL���m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v110 ��(�e)���Cײıװ�(ON����)   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v111 ��(�e)���C��Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v112 ��(�e)���Cײēd���d��     ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v113 ��(�e)����`���o���ݻ�d��   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v114 ��(�e)����`���o����ɲ��PL  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v115 ��(�e)�����Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v116 ��(�e)���o��Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v117 ��(�e)�W�D��Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v118 ��(�e)������H1Ӱ��װ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v119 ��(�e)������H2Ӱ��װ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v120 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v121 ��(�e)�d�Q�o�q�n�l�ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v122 ��(�e)���ڃp���`�ُ�      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v123 ��(�e)�]�ʃp���`�ُ�      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v124 ��(�e)���ڏ�������ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v125 ��(�e)���ډ��������ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v126 ��(�e)�]�ʈ������ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v127 ��(�e)�]�ʃ��{���؂�      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v128 ��(�e)����۰َ��؂�       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v129 ��(�e)����۰َ���ĕs��    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v130 ��(�e)����������ʒu�ُ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v131 ��(�e)���������l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v132 ��(�e)���񕔌��l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v133 ��(�e)���]�����l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v134 ��(�e)�����O���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v135 ��(�e)�ۗ��P���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v136 ��(�e)�ۗ��Q���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v137 ��(�e)�ۗ��R���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v138 ��(�e)�����ۗ������l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v139 ��(�e)���ڃp���`�O���l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v140 ��(�e)���ڃp���`�㌔�l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v141 ��(�e)���ډ���������l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v142 ��(�e)���ڏ��������l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v143 ��(�e)�]�ʃp���`�O���l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v144 ��(�e)�]�ʃp���`�㌔�l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v145 ��(�e)�]�ʈ�������l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v146 ��(�e)���ڈ���ُ팔�l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v147 ��(�e)�]�ʈ���ُ팔�l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v148 ��(�e)�W�ϕ����l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v149 ��(�e)���o�����l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v150 ��(�e)�W�D�����l��    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v151 ��(�e)���o�����l��(��)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v152 ��(�e)�W�D�����l��(��)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v153 ��(�e)����`���o�����l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v154 ��(�e)���������l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v155 ��(�e)���������U���l��    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v156 ��(�e)�ۗ��P���蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v157 ��(�e)�ۗ��Q���蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v158 ��(�e)�ۗ��R���蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v159 ��(�e)���ڕ��į�߂��蔲�� ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v160 ��(�e)�]�ʕ��į�߂��蔲�� ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v161 ��(�e)���]�����蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v162 ��(�e)���]���U�蕪���ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v163 ��(�e)�ۗ�����U�蕪���ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v164 ��(�e)�������U�蕪���ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v165 ��(�e)�W�D�U�蕪���ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v166 ��(�e)���o�U�蕪���ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v167 ��(�e)��U�W�D�U�蕪���ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v168 ��(�e)���CCPU�ُ�P       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v169 ��(�e)���CCPU�ُ�Q       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v170 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v171 ��(�e)�Z���T�ُ�          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v172 ��(�e)�Z�b�g�s��          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v173 ��(�e)�R�}���h�ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v174 ��(�e)�d�����m��        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v175 ��(�e)�s���̏��        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v176 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v177 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v178 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v179 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v180 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v181 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v182 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v183 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v184 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v185 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v186 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v187 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v188 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v189 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v190 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v191 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v192 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v193 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v194 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v195 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v196 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v197 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v198 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v199 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v200 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v201 �W(�`)�ǎ�ُ�|��w�b�h�i�G�h�����\�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v202 �W(�`)�ǎ�ُ�|��w�b�h�i�W�T�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v203 �W(�`)�ǎ�ُ�|���w�b�h�i�G�h�����\�����j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v204 �W(�`)�ǎ�ُ�|���w�b�h�i�W�T�������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v205 �W(�`)�ǎ�ُ�|��w�b�h �P�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v206 �W(�`)�ǎ�ُ�|��w�b�h �Q�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v207 �W(�`)�ǎ�ُ�|��w�b�h �R�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v208 �W(�`)�ǎ�ُ�|��w�b�h �S�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v209 �W(�`)�ǎ�ُ�|��w�b�h �T�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v210 �W(�`)�ǎ�ُ�|��w�b�h �U�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v211 �W(�`)�ǎ�ُ�|��w�b�h �V�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v212 �W(�`)�ǎ�ُ�|��w�b�h �W�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v213 �W(�`)�ǎ�ُ�|���w�b�h �P�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v214 �W(�`)�ǎ�ُ�|���w�b�h �Q�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v215 �W(�`)�ǎ�ُ�|���w�b�h �R�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v216 �W(�`)�ǎ�ُ�|���w�b�h �S�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v217 �W(�`)�ǎ�ُ�|���w�b�h �T�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v218 �W(�`)�ǎ�ُ�|���w�b�h �U�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v219 �W(�`)�ǎ�ُ�|���w�b�h �V�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v220 �W(�`)�ǎ�ُ�|���w�b�h �W�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v221 �W(�`)�����ُ�񐔁|�G�h�����\����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v222 �W(�`)�����ُ�񐔁|�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v223 �W(�`)�����ُ�񐔁|��^��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v224 �W(�`)�����ُ�񐔁|���̑��iSF�J�[�h�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v225 �W(�`)�����ُ�A���|�G�h�����\����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v226 �W(�`)�����ُ�A���|�����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v227 �W(�`)�����ُ�A���|��^��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v228 �W(�`)�����ُ�A���|���̑��iSF�J�[�h�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v229 �W(�`)�����ُ�A���|���w�b�h �P�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v230 �W(�`)�����ُ�A���|���w�b�h �Q�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v231 �W(�`)�����ُ�A���|���w�b�h �R�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v232 �W(�`)�����ُ�A���|���w�b�h �S�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v233 �W(�`)�����ُ�A���|���w�b�h �T�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v234 �W(�`)�����ُ�A���|���w�b�h �U�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v235 �W(�`)�����ُ�A���|���w�b�h �V�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v236 �W(�`)�����ُ�A���|���w�b�h �W�g���b�N", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v237 �W(�`)�h�b�q�v�ُ팟�m��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v238 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v239 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v240 �W(�`)���h�b��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v241 �W(�`)�h�b�ǎ�薢������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v242 �W(�`)�d�w�h�b�����ݖ��������i�P���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v243 �W(�`)�ݗ��h�b�����ݖ��������i�P���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v244 �W(�`)�h�b�ǎ攻��ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v245 �W(�`)�h�b��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v246 �W(�`)�h�b�h�c������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v247 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v248 �W(�`)�d�w�h�b�o�[�W��������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v249 �W(�`)�d�w�h�b�f�[�^���ڔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v250 �W(�`)�d�w�h�b�J�[�h�g�p�s����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v251 �W(�`)�d�w�h�b�ŏI���p���t����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v252 �W(�`)�d�w�h�b�l�K�`�F�b�N����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v253 �W(�`)�d�w�h�b���o��V�[�P���X����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v254 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v255 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v256 �W(�`)�d�w�h�b���p���t����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v257 �W(�`)�d�w�h�b���w���Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v258 �W(�`)�d�w�h�b��Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v259 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v260 �W(�`)EXIC���w�����Ȃ�NG���w���猔�Ȃ�NG", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v261 �W(�`)�ݗ��h�b�o�[�W��������", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v262 �W(�`)�ݗ��h�b�h�b��ʔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v263 �W(�`)�ݗ��h�b�f�[�^���ڔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v264 �W(�`)�ݗ��h�b�J�[�h����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v265 �W(�`)�ݗ��h�b�}�X�^�f�[�^����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v266 �W(�`)�ݗ��h�b����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v267 �W(�`)�ݗ��h�b�J�[�h�g�p�s����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v268 �W(�`)�ݗ��h�b�l�K�`�F�b�N����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v269 �W(�`)�ݗ��h�b��������Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v270 �W(�`)�ݗ��h�b���o��V�[�P���X����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v271 �W(�`)�ݗ��h�b���p���t����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v272 �W(�`)�ݗ��h�b���w���Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v273 �W(�`)�ݗ��h�b��Ԕ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v274 �W(�`)�ݗ��h�b���o��R�[�h����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v275 �W(�`)�ݗ��h�b�c�z����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v276 �W(�`)�ݗ��h�b���Z����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v277 �W(�`)�ݗ��h�b��_�ʉߔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v278 �W(�`)�ݗ�IC�s������m�f�i���o��T�C�N���ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v279 �W(�`)�ݗ��h�b�s������m�f(���o�ꎞ�Ԉُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v280 �W(�`)�ݗ��h�b�s������m�f�i����w���o��ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v281 �W(�`)�ݗ�IC�s������m�f�i�A������E�o��ُ�j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v282 �W(�`)�ݗ��h�b�V�����L�����Ȃ��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v283 �W(�`)�ݗ��h�b���o��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v284 �W(�`)���C�h�b���p�召���ݔ���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v285 �W(�`)���C�h�b���p�V������ԏd���m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v286 �W(�`)���C�h�b���p�ڑ��m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v287 �W(�`)���C�h�b���p���w�����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v288 �W(�`)���CIC���p���w���猔�������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v289 �W(�`)���CIC���p�L�����������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v290 �W(�`)���C�h�b���p���Z�s�m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v291 �W(�`)�d�w�h�b�����ُ݈�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v292 �W(�`)�ݗ��h�b�����ُ݈�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v293 �W(�`)�ݗ��h�b�e�X�g�J�[�h����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v294 �W(�`)�ݗ��h�b�����ԃG���A�m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v295 �W(�`)�ݗ��h�b�ŏI���p���t����m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v296 �W(�`)�ݗ��h�b���Њ����h�b�J�[�h�r�e���p�m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v297 �W(�`)�d�w�h�b�����ݖ��������i�Q���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v298 �W(�`)�ݗ��h�b�����ݖ��������i�Q���������j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v299 �W(�`)���h�b����m�f����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v300 �W(�`)�ݗ��h�b��ЊԌo�H�A��������m�f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v301 �W(�e)�������ݻ�d��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v302 �W(�e)��������ɲ�ޓd��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v303 �W(�e)��������ɲ��PL���m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v304 �W(�e)������Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v305 �W(�e)������Ӱ��d���װ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v306 �W(�e)�{�Q�S�u�d��          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v307 �W(�e)���C���ݻ�d��         ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v308 �W(�e)���C����ɲ�ޓd��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v309 �W(�e)���C����ɲ��PL���m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v310 �W(�e)���Cײıװ�(ON����)   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v311 �W(�e)���C��Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v312 �W(�e)���Cײēd���d��     ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v313 �W(�e)����`���o���ݻ�d��   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v314 �W(�e)����`���o����ɲ��PL  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v315 �W(�e)�����Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v316 �W(�e)���o��Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v317 �W(�e)�W�D��Ӱ���ײ�ޱװ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v318 �W(�e)������H1Ӱ��װ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v319 �W(�e)������H2Ӱ��װ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v320 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v321 �W(�e)�d�Q�o�q�n�l�ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v322 �W(�e)���ڃp���`�ُ�      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v323 �W(�e)�]�ʃp���`�ُ�      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v324 �W(�e)���ڏ�������ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v325 �W(�e)���ډ��������ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v326 �W(�e)�]�ʈ������ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v327 �W(�e)�]�ʃ��{���؂�      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v328 �W(�e)����۰َ��؂�       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v329 �W(�e)����۰َ���ĕs��    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v330 �W(�e)����������ʒu�ُ�   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v331 �W(�e)���������l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v332 �W(�e)���񕔌��l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v333 �W(�e)���]�����l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v334 �W(�e)�����O���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v335 �W(�e)�ۗ��P���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v336 �W(�e)�ۗ��Q���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v337 �W(�e)�ۗ��R���l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v338 �W(�e)�����ۗ������l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v339 �W(�e)���ڃp���`�O���l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v340 �W(�e)���ڃp���`�㌔�l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v341 �W(�e)���ډ���������l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v342 �W(�e)���ڏ��������l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v343 �W(�e)�]�ʃp���`�O���l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v344 �W(�e)�]�ʃp���`�㌔�l��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v345 �W(�e)�]�ʈ�������l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v346 �W(�e)���ڈ���ُ팔�l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v347 �W(�e)�]�ʈ���ُ팔�l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v348 �W(�e)�W�ϕ����l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v349 �W(�e)���o�����l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v350 �W(�e)�W�D�����l��    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v351 �W(�e)���o�����l��(��)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v352 �W(�e)�W�D�����l��(��)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v353 �W(�e)����`���o�����l��  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v354 �W(�e)���������l��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v355 �W(�e)���������U���l��    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v356 �W(�e)�ۗ��P���蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v357 �W(�e)�ۗ��Q���蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v358 �W(�e)�ۗ��R���蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v359 �W(�e)���ڕ��į�߂��蔲�� ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v360 �W(�e)�]�ʕ��į�߂��蔲�� ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v361 �W(�e)���]�����蔲��      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v362 �W(�e)���]���U�蕪���ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v363 �W(�e)�ۗ�����U�蕪���ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v364 �W(�e)�������U�蕪���ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v365 �W(�e)�W�D�U�蕪���ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v366 �W(�e)���o�U�蕪���ُ�    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v367 �W(�e)��U�W�D�U�蕪���ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v368 �W(�e)���CCPU�ُ�P       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v369 �W(�e)���CCPU�ُ�Q       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v370 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v371 �W(�e)�Z���T�ُ�          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v372 �W(�e)�Z�b�g�s��          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v373 �W(�e)�R�}���h�ُ�  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v374 �W(�e)�d�����m��        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v375 �W(�e)�s���̏��        ", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "�W�v401 (�e)�l�Ԍ��m�̏�i���ˁj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v402 (�e)�l�Ԍ��m�̏�i���߁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v403 (�e)���C���Z���T�̏�x����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v404 (�e)��@�W�D��U�ۗ��`�ُ��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v405 (�e)��@�W�D��U�ۗ��a�ُ��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v406 (�e)�]�@�W�D��U�ۗ��`�ُ��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v407 (�e)�]�@�W�D��U�ۗ��a�ُ��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v408 �i�e�j��@�W�D��U�ۗ��`���t���m��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v409 �i�e�j��@�W�D��U�ۗ��a���t���m��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v410 �i�e�j�]�@�W�D��U�ۗ��`���t���m��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v411 �i�e�j�]�@�W�D��U�ۗ��a���t���m��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v412 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v413 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v414 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v415 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v416 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v417 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v418 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v419 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v420 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v421 (�`)�h�A�̏�|�W�D�E�\����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v422 (�`)�h�A�̏�|�W�D�E�\�O��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v423 (�`)�h�A�̏�|���D�E�\����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v424 (�`)�h�A�̏�|���D�E�\�O��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v425 (�`)�������f�ُ�", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v426 (�`)�@��ُ펩�����A�̍ċN����", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v427 (�`)�ȓd�̓��[�h�������A��", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "�W�v428 (�`)�ߐڃZ���T�̏��", Nothing, XlsByteOrder.LittleEndian), _
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
            New XlsField(8*4, "D", 1, " "c, "�W�v500 �i�󂫁j", Nothing, XlsByteOrder.LittleEndian)}}

    Shared Sub New()
        For k As Integer = 0 To 1
            oFieldRefs(k) = New Dictionary(Of String, FieldRef)
            Dim bits As Integer = 0
            For i As Integer = 0 To oFields(k).Length - 1
                Dim oField As XlsField = oFields(k)(i)
                oFieldRefs(k).Add(oField.MetaName, New FieldRef(oField, bits, i))
                bits += oField.ElementBits * oField.ElementCount
            Next i
            totalBitCount(k) = bits
        Next k
    End Sub

    Public Shared ReadOnly Property RecordLengthInBits(ByVal k As Integer) As Integer
        Get
            Return totalBitCount(k)
        End Get
    End Property

    Public Shared ReadOnly Property RecordLengthInBytes(ByVal k As Integer) As Integer
        Get
            Return (totalBitCount(k) + 7) \ 8
        End Get
    End Property

    Public Shared ReadOnly Property Fields(ByVal k As Integer) As XlsField()
        Get
            Return oFields(k)
        End Get
    End Property

    Public Shared ReadOnly Property Field(ByVal k As Integer, ByVal sMetaName As String) As XlsField
        Get
            Return oFieldRefs(k)(sMetaName).Field
        End Get
    End Property

    Public Shared Function FieldIndexOf(ByVal k As Integer, ByVal sMetaName As String) As Integer
        Return oFieldRefs(k)(sMetaName).Index
    End Function

    Public Shared Function GetFieldValueFromBytes(ByVal k As Integer, ByVal sMetaName As String, ByVal oBytes As Byte()) As String
        Dim oRef As FieldRef = oFieldRefs(k)(sMetaName)
        Return oRef.Field.CreateValueFromBytes(oBytes, oRef.BitOffset)
    End Function

    Public Shared Sub SetFieldValueToBytes(ByVal k As Integer, ByVal sMetaName As String, ByVal sValue As String, ByVal oBytes As Byte())
        Dim oRef As FieldRef = oFieldRefs(k)(sMetaName)
        oRef.Field.CopyValueToBytes(sValue, oBytes, oRef.BitOffset)
    End Sub

    Public Shared Sub InitBaseHeaderFields(ByVal k As Integer, ByVal machine As EkCode, ByVal d As DateTime, ByVal seqNum As UInteger, ByVal oBytes As Byte())
        SetFieldValueToBytes(k, "��{�w�b�_�[ �f�[�^���", If(k = 0, "A7", "A8"), oBytes)
        SetFieldValueToBytes(k, "��{�w�b�_�[ �w�R�[�h", machine.ToString("%3R-%3S"), oBytes)
        SetFieldValueToBytes(k, "��{�w�b�_�[ ��������", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes(k, "��{�w�b�_�[ �R�[�i�[", machine.ToString("%C"), oBytes)
        SetFieldValueToBytes(k, "��{�w�b�_�[ ���@", machine.ToString("%U"), oBytes)
        SetFieldValueToBytes(k, "��{�w�b�_�[ �V�[�P���XNo", seqNum.ToString(), oBytes)
        SetFieldValueToBytes(k, "��{�w�b�_�[ �o�[�W����", "01", oBytes)
    End Sub

    Public Shared Sub InitCommonPartFields(ByVal k As Integer, ByVal machine As EkCode, ByVal d As DateTime, ByVal oBytes As Byte())
        SetFieldValueToBytes(k, "���ʕ� �W�v�J�n����", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes(k, "���ʕ� �W�v�I��(���W)����", "00000000000000", oBytes)
        SetFieldValueToBytes(k, "���ʕ� ���D���������_������", "00000000000000", oBytes)
        SetFieldValueToBytes(k, "���ʕ� �W�D���������_������", "00000000000000", oBytes)
        'TODO: ���̂Q���ڂ͑��������̎����ɂȂ��Ă���A���D�@�p�ɂ���Ȃ����������A���ƂɂȂ��񂪂Ȃ��̂ŁA���̂܂܂ł悢�C���B
        SetFieldValueToBytes(k, "���ʕ� ���D���������ԍ�", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes(k, "���ʕ� �W�D���������ԍ�", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes(k, "���ʕ� ���D�����m�Z���T���x��", Field(k, "���ʕ� ���D�����m�Z���T���x��").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes(k, "���ʕ� �W�D�����m�Z���T���x��", Field(k, "���ʕ� �W�D�����m�Z���T���x��").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes(k, "���ʕ� �\��", Field(k, "���ʕ� �\��").CreateDefaultValue(), oBytes)
    End Sub

    Public Shared Sub UpdateSummaryFields(ByVal oBytes As Byte()())
        'TODO: ���D�@�p�ɂ���Ȃ����B
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 293).MetaName, GetSummary(294, 344, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 273).MetaName, GetSummary(274, 282, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 203).MetaName, GetSummary(204, 262, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 184).MetaName, GetSummary(185, 192, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 149).MetaName, GetSummary(150, 173, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 128).MetaName, GetSummary(129, 138, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 112).MetaName, GetSummary(113, 117, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 9).MetaName, GetSummary(99, 101, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 8).MetaName, GetSummary(97, 98, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 7).MetaName, GetSummary(79, 86, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 6).MetaName, GetSummary(57, 68, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 5).MetaName, GetSummary(41, 46, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 4).MetaName, GetSummary(23, 30, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 3).MetaName, GetFieldValueFromBytes(Fields(AggregateFieldsOrigin + 7).MetaName, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 2).MetaName, GetSummary(New Integer() {6, 8}, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 1).MetaName, GetSummary(New Integer() {4, 5, 9}, oBytes), oBytes)
    End Sub

    Private Shared Function GetSummary(ByVal k As Integer, ByVal firstAggregateNumber As Integer, ByVal lastAggregateNumber As Integer, ByVal oBytes As Byte()) As String
        Dim sum As Long = 0
        For i As Integer = AggregateFieldsOrigin + firstAggregateNumber To AggregateFieldsOrigin + lastAggregateNumber
            sum += Long.Parse(GetFieldValueFromBytes(k, oFields(k)(i).MetaName, oBytes))
        Next i
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Shared Function GetSummary(ByVal aggregateIds As AggregateIdentifier(), ByVal oBytes As Byte()()) As String
        Dim sum As Long = 0
        For Each id As AggregateIdentifier In aggregateIds
            Dim k As Integer = id.Kind
            Dim i As Integer = AggregateFieldsOrigin + id.Number
            sum += Long.Parse(GetFieldValueFromBytes(k, oFields(k)(i).MetaName, oBytes(k)))
        Next id
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Structure AggregateIdentifier
        Public Kind As Integer
        Public Number As Integer
        Public Sub New(ByVal k As Integer, ByVal n As Integer)
            Kind = k
            Number = n
        End Sub
    End Structure

End Class
