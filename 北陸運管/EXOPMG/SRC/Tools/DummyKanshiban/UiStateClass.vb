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

    '�e����
    <DataMember> Public TermMachines As Dictionary(Of String, TermMachine)

    'NOTE: HoldingMasters��Value�́A�v�f��2�̔z��ł���B
    '�o�[�W�����i�p�^�[���ԍ��j���ǂ���̗v�f�Ƃ��Ⴄ���̂���M�����ۂ́A
    '�󂯓���\�ȑ��i�󂢂Ă��鑤�A���D�@�ɓK�p����Ă�����̂ƈႤ���j
    '�̗v�f�𒲂ׁA������Ɏ󂯓�����s���B�������A�v�f1�ɉ����ێ�����
    '���Ȃ���A�v�f0���󂯓���\�ȏꍇ�́A�v�f1�ɂ�����̂�v�f0�Ɉړ�
    '���A�v�f1�Ɏ󂯓�����s���B�܂�A�v�f0�ɂ͗v�f1���Â�����̏���
    '�i�[����悤�ɓw�͂���B�����󂢂Ă���ꍇ�́A�����Ȃ�v�f0�Ɋi�[����B
    '���̏����́A���̂܂܁AContinueCode��FinishWithoutStoring��
    'DLL�I��REQ�d���ɂ�����u�Ď��Օێ��o�[�W�����v�̏����ɂȂ�B
    '�^�ǂ�����Ɏg��Ȃ��̂łǂ��ł��悢���A���̏����́A�{����
    '�Ď��ՂƂ͈Ⴄ��������Ȃ��B
    'NOTE: HoldingMasters��Nothing�ɂȂ邱�Ƃ͂��蓾�Ȃ����A�C�ӂ�
    '�}�X�^���XXX�ɂ��āAHoldingMasters("XXX")���o�^����Ă���Ƃ͌���Ȃ��B
    'HoldingMasters("XXX")���o�^����Ă���ꍇ�ł��A
    'HoldingMasters("XXX")(0)��HoldingMasters("XXX")(1)��Nothing�Ƃ������Ƃ�
    '���蓾��B
    <DataMember> Public HoldingMasters As Dictionary(Of String, HoldingMaster())

    'NOTE: HoldingPrograms���A�v�f��2�̔z��ł���B
    '��M�������̂́A��{�I�ɁA���̔z��̗v�f1�Ɋi�[����B
    '���̍ۂɁA�v�f1�Ɋi�[����Ă������̂�v�f0�Ɉړ����邱�Ƃ͂Ȃ��B
    '�v�f1�Ɋi�[����Ă���v���O�������A�z���̑S���D�@�ɓK�p����
    '���_�ŁA�����v�f0�Ɉړ����A�v�f1����i�S�����o0�j�ɂ���B
    '�������A��M�������̂��z���̑S���D�@�ɓK�p�ς݂ł���i����
    '�z��̗v�f0�Ɋi�[����Ă���j�ꍇ�́A�v�f0�ɏ㏑������B
    '���̓���͖{���̊Ď��ՂƈႤ��������Ȃ����A�S���D�@����
    '�o�[�W��������{�ł��邱�Ƃ�A���D�@���ێ�����v���O������
    '�Ď��Ղ��ێ�����Ƃ����O����f����ƁAAcceptGatePro()�ɂ�����
    '�v���O�����󂯓���̏����ƍ��킹�āA���̂悤�ɂ��邵���Ȃ�
    '�Ǝv����B���̕����ł́A�v�f0�ɂ͗v�f1���Â�����̏��
    '���i�[����邪�A���̏����́A���̂܂܁AContinueCode��
    'FinishWithoutStoring��DLL�I��REQ�d���́u�Ď��Օێ��o�[�W�����v��
    '�����ɂȂ�B�^�ǂ�����Ɏg��Ȃ��̂łǂ��ł��悢���A���̏����́A
    '�{���̊Ď��ՂƈႤ��������Ȃ��B
    'NOTE: HoldingPrograms��Nothing�ɂȂ邱�Ƃ͂��蓾�Ȃ����A
    'HoldingPrograms(0)��HoldingPrograms(1)��Nothing�Ƃ����̂͂��蓾��B
    <DataMember> Public HoldingPrograms As HoldingProgram()

    'NOTE: HoldingKsbPrograms���A�v�f��2�̔z��ł���B���̔z��ł́A
    '�K�p���̂��̂�v�f0�Ɋi�[���A�K�p�҂��̂��̂�v�f1�Ɋi�[����B
    'NOTE: HoldingKsbPrograms��HoldingKsbPrograms(0)��Nothing�ɂȂ邱�Ƃ�
    '���蓾�Ȃ����AHoldingKsbPrograms(1)��Nothing�Ƃ����̂͂��蓾��B
    <DataMember> Public HoldingKsbPrograms As HoldingKsbProgram()
    <DataMember> Public PendingKsbPrograms As LinkedList(Of PendingKsbProgram)
    <DataMember> Public LatchConf As Byte
    <DataMember> Public FaultSeqNumber As UInteger
    <DataMember> Public FaultDate As DateTime

    Public Sub New()
        Me.TermMachines = New Dictionary(Of String, TermMachine)
        Me.HoldingMasters = New Dictionary(Of String, HoldingMaster())
        Me.HoldingPrograms = New HoldingProgram(1) {}
        Me.HoldingKsbPrograms = New HoldingKsbProgram(1) {}
        Me.PendingKsbPrograms = New LinkedList(Of PendingKsbProgram)
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
    <DataMember> Public PwrStatusFromKsb As Byte
    <DataMember> Public McpStatusFromKsb As Byte
    <DataMember> Public IcmStatusFromMcp As Byte
    <DataMember> Public DlsStatusFromMcp As Byte
    <DataMember> Public DlsStatusFromIcm As Byte
    <DataMember> Public ExsStatusFromIcm As Byte
    <DataMember> Public HoldingMasters As Dictionary(Of String, HoldingMaster)
    <DataMember> Public PendingMasters As Dictionary(Of String, LinkedList(Of PendingMaster))
    <DataMember> Public HoldingPrograms As HoldingProgram()
    <DataMember> Public PendingPrograms As LinkedList(Of PendingProgram)
    <DataMember> Public LatchConf As Byte
    <DataMember> Public FaultSeqNumber As UInteger
    <DataMember> Public FaultDate As DateTime
    <DataMember> Public KadoSlot(1) As Integer
    <DataMember> Public KadoSeqNumber(1) As UInteger
    <DataMember> Public KadoDate(1) As DateTime

    Public Sub New()
        Me.PwrStatusFromKsb = &H1
        Me.McpStatusFromKsb = &H0
        Me.IcmStatusFromMcp = &H0
        Me.DlsStatusFromMcp = &H0
        Me.DlsStatusFromIcm = &H0
        Me.ExsStatusFromIcm = &H0
        Me.HoldingMasters = New Dictionary(Of String, HoldingMaster)
        Me.PendingMasters = New Dictionary(Of String, LinkedList(Of PendingMaster))
        Me.HoldingPrograms = New HoldingProgram(1) {}
        Me.PendingPrograms = New LinkedList(Of PendingProgram)
    End Sub
End Class

<DataContract> Public Class HoldingMaster
    'NOTE: ���̃C���X�^���X�����L����̂�TermMachine�̏ꍇ���AListVersion�ɂ�
    '�l���i�[����B���D�@�͓K�p���X�g��ێ����Ȃ����A�ǂ̓K�p���X�g��
    '�w���ɂ���ē��Y���D�@�Ƀ}�X�^�{�̂̔z�M���s��ꂽ����������悤��
    '���邽�߂ł���B����āA�����܂ŕ\����p�ł���A����ɂ͗p���Ȃ��B
    'ListContent��ListHashValue�ɂ��Ă����l�ł���B

    'NOTE: ���̃C���X�^���X�����L����̂�Machine�̏ꍇ���AListVersion�͐����
    '�g�p���Ȃ��i�Ď��Ղ��Ō�Ɏ󂯓��ꂽ�K�p���X�g�̃o�[�W������
    '�}�X�^�o�[�W�����ʂɉ�ʕ\�����邽�߂����Ɏg�p����j�B
    '���̂����ɁA�z�M��[�����ƂɁA�z�M���������{�̓K�p���X�g��
    '�C�ӂ̌����L���[�C���O�\�ɂ��Ă���B
    '���R�ɂ��ẮAHoldingProgram�N���X�̃R�����g���Q�ƁB

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
    '�����p����B���̕��@�͖{���̊Ď��ՂƈႤ��������Ȃ��B
    'NOTE: ���������A�}�X�^�̓��e������ł��邱�Ƃ��`�F�b�N���邱�Ǝ��́A
    '����������悤�ɂ݂��邪�A������`�F�b�N���Ȃ��ƁA���̃A�v����
    '�������t�ɕ��G�����Ă��܂��̂ŁA���̃A�v�����g�̂��߂ł���B
    '�܂��A���̃A�v�������^�p�̃��n�[�T���p�Ɏg���ꍇ�́A�{����
    '�Ď��Ղł����e����Ȃ��\���̂���i���邢�́A�{���I�Ɋ댯�ȁj
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
    'NOTE: �v���O�����z�M�ł́ADL�����ʒm�ɃG���A�ԍ���ݒ肷��K�v���Ȃ��B
    '�����āA�Ď��@��́A�K�p���X�g�݂̂���M�����ꍇ���A����ɕR�Â�
    '�v���O�����{�̂�T����ŁA�ێ����Ă���v���O�����{�̂̃G���A�ԍ���
    '��r����K�v���Ȃ��i��M�������̂̃G���A�ԍ��́A�K�p���X�g�ɋL��
    '���ꂽ����̔z���ɂ���[���̃G���A�ԍ��ƈ�v���邩���`�F�b�N����
    '���߁A�󂯓��ꂽ���̓��m�̃G���A�ԍ����H���Ⴄ���Ƃ��Ȃ��j�悤��
    '�v����B�ȏ�̂��Ƃ���A�����́A�����ɃG���A�ԍ���ۑ�����K�v
    '���Ȃ��悤�Ɏv���邩������Ȃ��B�������A�����̂悤�ȊĎ��@��́A
    '�G���A�ԍ��݂̂��قȂ�v���O�������i�ݒu�G���A�̈قȂ镡���̑�����
    '���߂Ɂj�����ɕێ�����K�v�����邩�����ꂸ�A�����ł���Ȃ�A
    '�����ɃG���A�ԍ���ۑ����邱�Ƃ͕K�{�ł���B���������AProfile��
    '�G���A�ԍ��Ɣ�r���邾�����ƁA���ꂪ�ς�����ꍇ�ɁA�O��Ɏ�M����
    '�v���O�����{�̂ƓK�p���X�g�̊֌W������ĉ��߂��邱�ƂɂȂ�B
    '�܂��A�����Ɏg����ŁA��M�������̂̃G���A�ԍ���UI��Ŋm�F�ł���
    '�����悢�͂��ł���B�ȏ�̂��Ƃ���A�����ɕۑ�����悤�ɂ��Ă����B

    'NOTE: ���̃C���X�^���X�����L����̂�TermMachine�ł͂Ȃ��AMachine�ł���
    '�ꍇ�́AListVersion��ApplicableDate��ListHashValue�͐���Ɏg�p
    '���Ȃ��i�Ď��Ղ��Ō�Ɏ󂯓��ꂽ�K�p���X�g�̃o�[�W������
    '��\�o�[�W�����ʂɉ�ʕ\�����邽�߂����Ɏg�p����j�B���̂����ɁA
    '�z�M��[�����ƂɁA�z�M���������{�̓K�p���X�g��C�ӂ̌���
    '�L���[�C���O�\�ɂ��Ă���B���ϓI�ɂ́A�Ď��@�킪�v���O����
    '�{�̂Ɠ��������́i�܂�ő�Q�́j�K�p���X�g��ێ�����̂�
    '���R�ɂ��v���邪�A������\�o�[�W�����ł��A�K�p���X�g�͂�����
    '�p�ӂ��邱�Ƃ�������Ă���A���̂P�P�ɈӖ������邽�߁A
    '����ł́A�܂Ƃ��ȋ@�\���������邱�Ƃ��s�\�ɂȂ�͂��ł���B���ɁA
    '�����̍ς�ł��Ȃ��K�p���X�g������󋵂ŁA�����\�o�[�W������
    '�v���O�����Ɋւ��鎟��DLL�v�����������ꍇ��BUSY����NAK��Ԃ���
    '���Ă��A�x�~�����Ă�����D�@���K�p���X�g�ɋL�ڂ���Ă��邾����
    '���g���C�I�[�o�[�ƂȂ�i���̉��D�@�ɂ��z�M���s���Ȃ��j�킯�ł���A
    '���ꂪ�����I�Ƃ͍l���ɂ����B�܂��A�i���ʂ�҂����ɔz�M�\�ȁj
    '�����\�o�[�W�����̓K�p���X�g�̌�����99���Ɍ��肳���̂��A
    '�Ď��Ղł̃L���[�C���O�̗e�Ղ����l�����Ă̎d�l�Ɛ����ł���B
    '�Ȃ��A�{���̊Ď��Ղ��ȏ�̂悤�Ȏd�l�ɂȂ��Ă��邩�͕s���ł��邪�A
    '����f�ƂȂ��Ă�����D�@�����݂��Ă���󋵂ł��A��\�o�[�W����
    '�������ł������i��\�o�[�W�����̕ύX�ŁA�v���O�����{�̂�
    '�o�[�W���������D�@�Ԃŕs��v�ɂȂ�ȂǂƂ������Ƃ��Ȃ���΁j
    '�V�����K�p���X�g���󂯕t���邱�Ƃ͊ԈႢ�Ȃ��B�܂��A�K�p���X�g
    '���󂯕t�����iDLL�V�[�P���X�𐳏�I���������j�ȏ�A������
    '�w�肳��Ă���S�Ẳ��D�@�ɔz�M���s���i���߂�ꍇ�́A
    '�Ď��Վ��g���u�z�M�ُ�v�̉��D�@DL�����ʒm�𐶐�����H�j
    '���Ƃ��Ď��Ղ̐Ӗ��Ƃ��Đ錾����Ă��邽�߁A�L���[�C���O
    '�����̂��Ƃ�����ƍl����̂����R�ł���B

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
    <DataMember> Public ModuleInfos As ProgramModuleInfo()
    <DataMember> Public ArchiveCatalog As String
    <DataMember> Public VersionListData As Byte()

    'NOTE: �����DataGridView2�̍s�̓K�p���X�g�o�[�W��������_�u���N���b�N����ƊJ��
    '�Ɨ��������[�h���X�_�C�A���O�ɕ\������z��ł���B
    <DataMember> Public ListContent As String

    'NOTE: ���̃A�v���ł́ACAB�̓��e������ł��邩�ۂ����r�����ŁA
    '�����p����B���̕��@�͖{���̊Ď��ՂƈႤ��������Ȃ��B
    'NOTE: ���������ACAB�̓��e������ł��邱�Ƃ��`�F�b�N���邱�Ǝ��́A
    '����������悤�ɂ݂��邪�A������`�F�b�N���Ȃ��ƁA���̃A�v����
    '�������t�ɕ��G�����Ă��܂��̂ŁA���̃A�v�����g�̂��߂ł���B
    '�܂��A���̃A�v�������^�p�̃��n�[�T���p�Ɏg���ꍇ�́A�{����
    '�Ď��Ղł����e����Ȃ��\���̂���i���邢�́A�{���I�Ɋ댯�ȁj
    '�^�p�ɑ΂��A���ꂪ������悤�ɂ��Ă����ɂ��������Ƃ͂Ȃ��B
    <DataMember> Public DataHashValue As String

    'NOTE: ���̃A�v���ł́A�K�p���X�g�̓��e������ł��邩�ۂ����r�����ŁA
    '�����p����B���̕��@�͖{���̊Ď��ՂƈႤ��������Ȃ��B
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
    'NOTE: �Ď��Ղɂ�����^�ǂ���̉��D�@�����v���O�����󂯓�������ɂ��
    '���肩��APendingProgram�Ɋi�[����Ă�����̂́AMachine��
    'HoldingPrograms�̉��ꂩ�̗v�f�ɕK���i�[����Ă���͂��ł���A
    '�{���I�ɁA�����ɂ���͕s�v�ł��邪�A�P�����̂��߂�
    '�i�[���Ă����B
    <DataMember> Public ModuleInfos As ProgramModuleInfo()
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

<DataContract> Public Structure ProgramModuleInfo
    <DataMember> Public Elements As ProgramElementInfo()
End Structure

<DataContract> Public Structure ProgramElementInfo
    <DataMember> Public FileName As String
    <DataMember> Public DispData As Byte()
End Structure

<DataContract> Public Class HoldingKsbProgram
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
    '�����p����B���̕��@�͖{���̊Ď��ՂƈႤ��������Ȃ��B
    'NOTE: ���������ACAB�̓��e������ł��邱�Ƃ��`�F�b�N���邱�Ǝ��́A
    '����������悤�ɂ݂��邪�A������`�F�b�N���Ȃ��ƁA���̃A�v����
    '�������t�ɕ��G�����Ă��܂��̂ŁA���̃A�v�����g�̂��߂ł���B
    '�܂��A���̃A�v�������^�p�̃��n�[�T���p�Ɏg���ꍇ�́A�{����
    '�Ď��Ղł����e����Ȃ��\���̂���i���邢�́A�{���I�Ɋ댯�ȁj
    '�^�p�ɑ΂��A���ꂪ������悤�ɂ��Ă����ɂ��������Ƃ͂Ȃ��B
    <DataMember> Public DataHashValue As String

    'NOTE: ���̃A�v���ł́A�K�p���X�g�̓��e������ł��邩�ۂ����r�����ŁA
    '�����p����B���̕��@�͖{���̊Ď��ՂƈႤ��������Ȃ��B
    <DataMember> Public ListHashValue As String
End Class

<DataContract> Public Class PendingKsbProgram
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
