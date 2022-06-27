' **********************************************************************
'   �V�X�e�����F�V�����������D�V�X�e���i�^�p�Ǘ��T�[�o�^�[���j
'
'   Copyright Toshiba Solutions Corporation 2013 All rights reserved.
'
' ----------------------------------------------------------------------
'   �ύX����:
'   Ver      ���t        �S��       �R�����g
'   0.0      2013/04/01  (NES)����  �V�K�쐬
'   0.1      2017/04/10  (NES)����  ������ԕ�Ή�
' **********************************************************************
Option Explicit On
Option Strict On

Public Class EkConstants

    '���p�f�[�^�̃��R�[�h���iNOTE: �g�p�֎~�j
    'TODO: ������ԕ�Ή��ɂ��A��N�ԒʐM�v���Z�X�ƂƂ��ɏ�������B
    Public Const RiyoDataRecordLen As Integer = 1460

    '�w���@�킩����W����t�@�C���̃w�b�_��
    Public Const UpboundDataHeaderLen As Integer = 17

    '�f�[�^�敪
    'NOTE: �f�[�^�x�[�X�̃e�[�u�����̕ҏW�Ɏg�p�\�ł���B
    Public Const DataPurposeMaster As String = "MST"    '�}�X�^
    Public Const DataPurposeProgram As String = "PRG"   '�v���O����

    '�t�@�C���敪
    Public Const FilePurposeData As String = "DAT"      '�f�[�^�iCAB��BIN�{�́j
    Public Const FilePurposeList As String = "LST"      '�K�p���X�g

    '�@��R�[�h
    Public Const ModelCodeNone As String = ""           '����`�@��
    Public Const ModelCodeKanshiban As String = "W"     '�Ď���
    Public Const ModelCodeGate As String = "G"          '���D�@
    Public Const ModelCodeTokatsu As String = "X"       '����
    Public Const ModelCodeMadosho As String = "Y"       '����

    '���i�R�[�h
    Public Const ProductCodeOfKanshiban As String = "75"
    Public Const ProductCodeOfGate As String = "70"
    Public Const ProductCodeOfMadosho As String = "86"

    '�@��ɑΉ����鐻�i�R�[�h
    Public Shared ReadOnly ProductCodeOfModels As New Dictionary(Of String, String) From { _
       {ModelCodeKanshiban, ProductCodeOfKanshiban}, _
       {ModelCodeGate, ProductCodeOfGate}, _
       {ModelCodeMadosho, ProductCodeOfMadosho}}

    '�d�l�R�[�h
    'NOTE: EkCommon���̑��̃N���X�́A�����̂Q�����ڂ��@��R�[�h�ł��邱��
    '����сA����炪�U�����ł��邱�Ƃ�O��Ɏ������Ă���̂ŁA������
    '�ύX����ۂ͒��ӂ��Ȃ���΂Ȃ�Ȃ��B
    Public Const SpecificCodeOfKanshiban As String = "EW7200"
    Public Const SpecificCodeOfGate As String = "EG7000"
    Public Const SpecificCodeOfMadosho As String = "EY4100"

    '�@��ɑΉ�����d�l�R�[�h
    Public Shared ReadOnly SpecificCodeOfModels As New Dictionary(Of String, String) From { _
       {ModelCodeKanshiban, SpecificCodeOfKanshiban}, _
       {ModelCodeGate, SpecificCodeOfGate}, _
       {ModelCodeMadosho, SpecificCodeOfMadosho}}

    '�v���O������\�o�[�W��������
    Public Const ProgramDataVersionFormatOfKanshiban As String = "D8"
    Public Const ProgramDataVersionFormatOfGate As String = "D4"
    Public Const ProgramDataVersionFormatOfMadosho As String = "D4"

    '�@��ɑΉ�����v���O������\�o�[�W��������
    Public Shared ReadOnly ProgramDataVersionFormatOfModels As New Dictionary(Of String, String) From { _
       {ModelCodeKanshiban, ProgramDataVersionFormatOfKanshiban}, _
       {ModelCodeGate, ProgramDataVersionFormatOfGate}, _
       {ModelCodeMadosho, ProgramDataVersionFormatOfMadosho}}

End Class
