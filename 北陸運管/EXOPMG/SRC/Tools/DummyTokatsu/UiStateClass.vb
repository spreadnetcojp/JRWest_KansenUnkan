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

Imports System.Runtime.Serialization

<DataContract> Public Class UiStateClass
    'NOTE: �@��̏�Ԃ͂����ɕۑ����Ă��悢���A�V�~�����[�^�{�̂��w�肵�Ă���
    '�p�X�̋@��ʃf�B���N�g���ɕۑ����Ă��悢�B�^�p�����G�ɂȂ�̂ŁA
    '�ǂ��炩�ɓ��ꂵ�������悢�B�����ɕۑ����Ă������������ɎQ�Ƃł���B
    <DataMember> Public Machines As Dictionary(Of String, Machine)

    '���O�\���t�B���^�̗���
    <DataMember> Public LogDispFilterHistory As List(Of String)

    Public Sub New()
        Me.Machines = New Dictionary(Of String, Machine)
        Me.LogDispFilterHistory = New List(Of String)
    End Sub
End Class

<DataContract> Public Class Machine
    '�@��\���t�@�C���̍ŏI�m�F����
    <DataMember> Public LastConfirmed As DateTime

    '�@��\���t�@�C���̃^�C���X�^���v
    <DataMember> Public ProfileTimestamp As DateTime
    <DataMember> Public TermMachinesProfileTimestamp As DateTime

    '�@��\���t�@�C���̃L���b�V��
    <DataMember> Public Profile As Object()

    '�[���̏��
    <DataMember> Public TermMachines As Dictionary(Of String, TermMachine)

    '�ڑ����
    <DataMember> Public NegaStatus As Byte
    <DataMember> Public MeisaiStatus As Byte
    <DataMember> Public OnlineStatus As Byte

    'NOTE: �����́A�}�X�^��v���O�������u�����ʂɁv���萢�㕪�ێ�����͂��ł���
    '�i����ɂ��u�S�[���̕ێ�������̂���v����܂ŐV���Ȃ��̂��󂯓���Ȃ��v��
    '�������Ď��ՓI�Ȑ������Ȃ����Ă���B�܂��A����ɂ��A�z���ɂ��鑋���̃G���A��
    '�P��ނłȂ��󋵂ɑΉ����Ă���j�B����āA������HoldingFooBar�͔p�~���āA
    '�����ʂ́iTermMachine�N���X�́jPendingFoo���g���܂킷���������I�Ƃ��v����B
    '�������A�������I�t���C���̏ꍇ�ɑ����ւ̔z�M�����s�Ƃ���i�������g���u�ُ�v��
    'DL�����ʒm�𐶐�����j�̂ł͂Ȃ��A�Ď��Ղ̂悤�ɕۗ�����v�z�ɕύX���ꂽ���߁A
    '�V�~�����[�^�Ƃ��ď\���ȏ󋵂�\�������ŁA�ǂ݂̂�TermMachine�N���X��
    'PendingFoo�͕K�{������ł���B
    '�������Ȃ���A����P�̕��ނɂ��āA�ŏ��ɑ���A�ւ�DLL�v�����f�[�^�{�̓Y�t
    '�ōs���A���ɑ���B�ւ�DLL�v�����K�p���X�g�̂ݓY�t�ōs��ꂽ�ꍇ�A��ɓY�t
    '����Ă����f�[�^�{�̂��g�p���đ���B�ւ�DLL���s���ׂ��ł���A���̃f�[�^�{�̂�
    '�ւ�����́A����A�̍s�ł͂Ȃ��A�����̍s�i�S�������ʂ̍s�j�ɕ\�����������悢
    '���߁A������HoldingFooBar�ɕێ������邱�Ƃɂ���BTermMachine�N���X�ɂ����āA
    '���ۂɑ������ێ����Ă���f�[�^��\��HoldingFoo�Ƃ͕ʂɁA���������ێ����Ă���
    '�f�[�^�i*1�j��\�������o��p�ӂ��A�\���̍ۂɁA�STermMachine�̂���𓝍�����
    '�����̍s�ɕ\�����邱�Ƃ��s�\�ł͂Ȃ����A�����f�[�^�𕡐��̉ӏ��ɏd�����ĊǗ�
    '���邱�ƂɂȂ邽�߁A���������Ȃ��B�P�̓����z���̑S�����ɂ��Ă̏��萢�㕪��
    '�f�[�^���ߕs���Ȃ������ɕێ�������̂͏����ʓ|�ł��邪�A�����ւ̔z�M�����s����
    '���тɁATermMachines���ɕێ�����Ă���f�[�^�̖��O�i�G���A��o�[�W�����j�����ƂɁA
    '�����ɕێ����Ă���f�[�^�𐮗�����i�s�v�Ȃ��̂��폜����j�����ł���A
    '���Ƃ��Ȃ�͂��ł���B
    '*1 ���Ƃ����ꖼ�i����G���A�A����o�[�W�����j�̃f�[�^�ł����Ă��A�������ێ����Ă���
    '���̂Ɠ������ێ����Ă�����̂̃f�[�^���e�́A�ʕ��ƍl����K�v������B
    '�ߋ���DLL�v���œY�t����Ă������̂Ɠ��ꖼ�̃f�[�^�{�̂��V����DLL�v���ōēx�Y�t�����
    '�����ꍇ�A�����͎��g�̕ێ����Ă�����̂��㏑������i���t���V�����ʂ̃f�[�^�Ƃ݂Ȃ��j
    '���A�K�p���X�g��œK�p�ΏۂɂȂ��Ă��鑋���łȂ�����A�ߋ��̓��ꖼ�f�[�^��ێ�����
    '���鑋���ɑ΂��āA�V�������̂�����ɔz�M���Ȃ������Ƃ͂Ȃ��i����ɂ��ẮA�z�M�w����
    '�z�M�̖{���̊֌W������Ă���j�Ǝv���邽�߂ł���i���K�p���X�g��œK�p�ΏۂɂȂ���
    '���鑋���ɂ��ẮA���̂悤�Ƀf�[�^�{�̂��Y�t����Ă���DLL�v���̏ꍇ�́A�����I�ɔz�M
    '���Ȃ������ƂɂȂ�A�f�[�^�{�̂��Y�t����Ă��Ȃ�DLL�v���̏ꍇ�́A�f�[�^�{�̂�z�M
    '���Ȃ������Ɂu�K�p�ρv�Ƃ���͂��ł���j�B
    '����A�����������������Đڑ������ꍇ�ɁA�^�ǃT�[�o����̐V���Ȕz�M�w���Ȃ��ŁA
    '���g���ێ����Ă��镔�ނ����Ƃɑ����ւ̔z�M���s���̂��ۂ��ɂ��ẮA�s���ł���B

    'NOTE: �����ɕێ������Ă��鑋���v���O����������DLL���ނ݂̂ł���ƈӖ����Ȃ��̂ŁA
    '�Ō�Ɏ�M�����i�Ƃ������ŐVn����́j�S��DLL���ނƁA�i���ꂼ��̐���ɂ��āj
    '����ȍ~�Ɏ�M��������DLL���ޑS�Ă�ێ�����ׂ���������Ȃ��B
    '���邢�́A�����ƍ��@�\�ɁA�����̓����ŕ��ނ̃}�[�W���s���\�����Ȃ��Ƃ�
    '�����؂�Ȃ��i�������ɈႤ�Ǝv�����j�B
    '���������A�Ď��Փ��l�A����DLL�ɂ͑Ή����Ă��Ȃ�������������Ȃ��i�����Ƃ���
    '�l�����́A�킸���ȃo�[�W�����̈Ⴂ�𐢑�̈Ⴂ�Ƃ݂Ȃ�����̐���Ǘ���
    '��΂ɑ��e��Ȃ��͂��ł���j�B
    '�˓���G���A�݂̂ɑΉ��������ށi���Y�G���A�ɖ��֌W�ȃt�@�C���͏ȗ����邪�A
    '���Y�G���A�ɕK�v�ȃt�@�C���͑O��z�M�o�[�W�����Ɋ֌W�Ȃ��S�Ċi�[���Ă��镔�ށj
    '���u����DLL�v�Ŕz�M����悤�ł���B

    <DataMember> Public HoldingMasters As Dictionary(Of String, List(Of HoldingMaster))
    <DataMember> Public HoldingPrograms As List(Of HoldingProgram)

    Public Sub New()
        Me.TermMachines = New Dictionary(Of String, TermMachine)
        Me.NegaStatus = &H2
        Me.MeisaiStatus = &H2
        Me.OnlineStatus = &H2
        Me.HoldingMasters = New Dictionary(Of String, List(Of HoldingMaster))
        Me.HoldingPrograms = New List(Of HoldingProgram)
    End Sub
End Class

<DataContract> Public Class TermMachine
    '�@��\���t�@�C���̃L���b�V��
    <DataMember> Public Profile As Object()

    '�e����
    'NOTE: HoldingPrograms�́A�v�f��2�̔z��ł���B���̔z��ł́A
    '�K�p���̂��̂�v�f0�Ɋi�[���A�K�p�҂��̂��̂�v�f1�Ɋi�[����B
    'NOTE: HoldingPrograms��HoldingPrograms(0)��Nothing�ɂȂ邱�Ƃ�
    '���蓾�Ȃ����AHoldingPrograms(1)��Nothing�Ƃ����̂͂��蓾��B
    <DataMember> Public DlsStatus As Byte
    <DataMember> Public KsbStatus As Byte
    <DataMember> Public Tk1Status As Byte
    <DataMember> Public Tk2Status As Byte
    <DataMember> Public HoldingMasters As Dictionary(Of String, HoldingMaster)
    <DataMember> Public PendingMasters As Dictionary(Of String, LinkedList(Of PendingMaster))
    <DataMember> Public HoldingPrograms As HoldingProgram()
    <DataMember> Public PendingPrograms As LinkedList(Of PendingProgram)

    Public Sub New()
        Me.DlsStatus = &H2
        Me.KsbStatus = &H2
        Me.Tk1Status = &H2
        Me.Tk2Status = &H2
        Me.HoldingMasters = New Dictionary(Of String, HoldingMaster)
        Me.PendingMasters = New Dictionary(Of String, LinkedList(Of PendingMaster))
        Me.HoldingPrograms = New HoldingProgram(1) {}
        Me.PendingPrograms = New LinkedList(Of PendingProgram)
    End Sub
End Class

<DataContract> Public Class HoldingMaster
    'NOTE: ���̃N���X�ɂ�����ListVersion�ɂ����݈Ӌ`�͂���B
    '�����͓K�p���X�g��ێ����Ȃ����A�ǂ̓K�p���X�g��
    '�w���ɂ���ē��Y�����Ƀ}�X�^�{�̂̔z�M���s��ꂽ����������悤��
    '���邽�߂ł���B����āA�����܂ŕ\����p�ł���A����ɂ͗p���Ȃ��B
    'ListContent��ListHashValue�ɂ��Ă����l�ł���B

    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public DataDeliverDate As DateTime

    'NOTE: �����DataGridView2�̍s���_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public DataFooter As Byte()

    'NOTE: �����DataGridView2�̍s�̓K�p���X�g�o�[�W��������_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public ListContent As String

    'NOTE: ���̃A�v���ł́A�}�X�^�̓��e������ł��邩�ۂ����r�����ŁA
    '�����p����B���̕��@�͖{���̓����ƈႤ��������Ȃ��B
    'NOTE: ���������A�}�X�^�̓��e������ł��邱�Ƃ��`�F�b�N���邱�Ǝ��́A
    '����������悤�ɂ݂��邪�A������`�F�b�N���Ȃ��ƁA���̃A�v����
    '�������t�ɕ��G�����Ă��܂��̂ŁA���̃A�v�����g�̂��߂ł���B
    '�܂��A���̃A�v�������^�p�̃��n�[�T���p�Ɏg���ꍇ�́A�{����
    '�����ł����e����Ȃ��\���̂���i���邢�́A�{���I�Ɋ댯�ȁj
    '�^�p�ɑ΂��A���ꂪ������悤�ɂ��Ă����ɂ��������Ƃ͂Ȃ��B
    <DataMember> Public DataHashValue As String

    'NOTE: �{���I�ɕێ����Ă����K�R�����Ȃ����ł��邪�A
    '�K�p���X�g�{���E�B���h�E�̃L�[�̈ꕔ�Ƃ��Ďg�p����
    '���Ƃɂ��Ă���i�v���O�����{�̂̉{���E�B���h�E��
    '��т��������ɂ��邽�߁j�B
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class PendingMaster
    'NOTE: �}�X�^�̏ꍇ�A�K�p���X�g��DL�����������̂ŁAListVersion�͕s�v�ł��邪�A
    '�ǂ̓K�p���X�g�ɂ��z�M���ۗ��ɂȂ��Ă���̂��AUI�ɕ\����������
    '�g���₷���Ǝv���邽�߁A�ۑ����邱�Ƃɂ��Ă���B
    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime

    'NOTE: �����DataGridView2�̍s���_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public DataFooter As Byte()

    'NOTE: �����DataGridView2�̍s�̓K�p���X�g�o�[�W��������_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public ListContent As String

    'NOTE: �{���I�ɁA�����ɂ���͕s�v�ł��邪�A�P�����̂��߂�
    '�i�[���Ă����B
    <DataMember> Public DataHashValue As String
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class HoldingProgram
    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public DataDeliverDate As DateTime
    <DataMember> Public ListDeliverDate As DateTime
    <DataMember> Public RunnableDate As String
    <DataMember> Public ApplicableDate As String
    <DataMember> Public ApplyDate As DateTime

    'NOTE: �����DataGridView2�̍s���_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public ArchiveCatalog As String
    <DataMember> Public VersionListData As Byte()

    'NOTE: �����DataGridView2�̍s�̓K�p���X�g�o�[�W��������_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public ListContent As String

    'NOTE: ���̃A�v���ł́ACAB�̓��e������ł��邩�ۂ����r�����ŁA
    '�����p����B���̕��@�͖{���̓����ƈႤ��������Ȃ��B
    'NOTE: ���������ACAB�̓��e������ł��邱�Ƃ��`�F�b�N���邱�Ǝ��́A
    '����������悤�ɂ݂��邪�A������`�F�b�N���Ȃ��ƁA���̃A�v����
    '�������t�ɕ��G�����Ă��܂��̂ŁA���̃A�v�����g�̂��߂ł���B
    '�܂��A���̃A�v�������^�p�̃��n�[�T���p�Ɏg���ꍇ�́A�{����
    '�����ł����e����Ȃ��\���̂���i���邢�́A�{���I�Ɋ댯�ȁj
    '�^�p�ɑ΂��A���ꂪ������悤�ɂ��Ă����ɂ��������Ƃ͂Ȃ��B
    <DataMember> Public DataHashValue As String

    'NOTE: ���̃A�v���ł́A�K�p���X�g�̓��e������ł��邩�ۂ����r�����ŁA
    '�����p����B���̕��@�͖{���̓����ƈႤ��������Ȃ��B
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class PendingProgram
    <DataMember> Public DataSubKind As Integer
    <DataMember> Public DataVersion As Integer
    <DataMember> Public ListVersion As Integer
    <DataMember> Public DataAcceptDate As DateTime
    <DataMember> Public ListAcceptDate As DateTime
    <DataMember> Public RunnableDate As String
    <DataMember> Public ApplicableDate As String

    'NOTE: �����DataGridView2�̍s���_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public ArchiveCatalog As String
    <DataMember> Public VersionListData As Byte()

    'NOTE: �����DataGridView2�̍s�̓K�p���X�g�o�[�W��������_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public ListContent As String

    'NOTE: �{���I�ɁA�����ɂ���͕s�v�ł��邪�A�P�����̂��߂�
    '�i�[���Ă����B
    <DataMember> Public DataHashValue As String
    <DataMember> Public ListHashValue As String
End Class
